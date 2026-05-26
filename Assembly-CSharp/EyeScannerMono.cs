using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Text;
using GorillaLocomotion;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000B5 RID: 181
public class EyeScannerMono : MonoBehaviour, ISpawnable, IGorillaSliceableSimple
{
	// Token: 0x1700004D RID: 77
	// (get) Token: 0x0600045D RID: 1117 RVA: 0x00019196 File Offset: 0x00017396
	// (set) Token: 0x0600045E RID: 1118 RVA: 0x000191A0 File Offset: 0x000173A0
	private Color32 KeyTextColor
	{
		get
		{
			return this.m_keyTextColor;
		}
		set
		{
			this.m_keyTextColor = value;
			this._keyRichTextColorTagString = string.Format(CultureInfo.InvariantCulture.NumberFormat, "<color=#{0:X2}{1:X2}{2:X2}>", value.r, value.g, value.b);
		}
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x0600045F RID: 1119 RVA: 0x000191EF File Offset: 0x000173EF
	private List<IEyeScannable> registeredScannables
	{
		get
		{
			return EyeScannerMono._registeredScannables;
		}
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x000191F6 File Offset: 0x000173F6
	public static void Register(IEyeScannable scannable)
	{
		if (EyeScannerMono._registeredScannableIds.Add(scannable.scannableId))
		{
			EyeScannerMono._registeredScannables.Add(scannable);
		}
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x00019215 File Offset: 0x00017415
	public static void Unregister(IEyeScannable scannable)
	{
		if (EyeScannerMono._registeredScannableIds.Remove(scannable.scannableId))
		{
			EyeScannerMono._registeredScannables.Remove(scannable);
		}
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x00019238 File Offset: 0x00017438
	protected void Awake()
	{
		this._sb = ZString.CreateStringBuilder();
		this.KeyTextColor = this.KeyTextColor;
		math.sign(this.m_textTyper.transform.parent.localScale);
		this.m_textTyper.SetText(string.Empty);
		this.m_reticle.gameObject.SetActive(false);
		this.m_textTyper.gameObject.SetActive(false);
		this.m_overlayBg.SetActive(false);
		this._line = base.GetComponent<LineRenderer>();
		this._line.enabled = false;
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x06000463 RID: 1123 RVA: 0x000192D2 File Offset: 0x000174D2
	// (set) Token: 0x06000464 RID: 1124 RVA: 0x000192DA File Offset: 0x000174DA
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x06000465 RID: 1125 RVA: 0x000192E3 File Offset: 0x000174E3
	// (set) Token: 0x06000466 RID: 1126 RVA: 0x000192EB File Offset: 0x000174EB
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x06000467 RID: 1127 RVA: 0x000192F4 File Offset: 0x000174F4
	// (set) Token: 0x06000468 RID: 1128 RVA: 0x000192FC File Offset: 0x000174FC
	public string DebugData { get; private set; }

	// Token: 0x06000469 RID: 1129 RVA: 0x00019308 File Offset: 0x00017508
	public void OnSpawn(VRRig rig)
	{
		if (rig != null && !rig.isOfflineVRRig)
		{
			Object.Destroy(base.gameObject);
		}
		if (GTPlayer.hasInstance)
		{
			GTPlayer instance = GTPlayer.Instance;
			this._firstPersonCamera = instance.GetComponentInChildren<Camera>();
			this._has_firstPersonCamera = (this._firstPersonCamera != null);
		}
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDespawn()
	{
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x00011DD7 File Offset: 0x0000FFD7
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x0001935C File Offset: 0x0001755C
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone == GTZone.bayou)
		{
			if (this._oldClosestScannable != null)
			{
				this._OnScannableChanged(null, false);
				this._oldClosestScannable = null;
			}
			return;
		}
		IEyeScannable eyeScannable = null;
		Transform transform = base.transform;
		Vector3 position = transform.position;
		Vector3 forward = transform.forward;
		float num = this.m_LookPrecision;
		for (int i = 0; i < EyeScannerMono._registeredScannables.Count; i++)
		{
			IEyeScannable eyeScannable2 = EyeScannerMono._registeredScannables[i];
			Vector3 normalized = (eyeScannable2.Position - position).normalized;
			float num2 = Vector3.Distance(position, eyeScannable2.Position);
			float num3 = Vector3.Dot(forward, normalized);
			if (num2 >= this.m_scanDistanceMin && num2 <= this.m_scanDistanceMax && num3 > num)
			{
				RaycastHit raycastHit;
				if (!this.m_xrayVision && Physics.Raycast(position, normalized, out raycastHit, this.m_scanDistanceMax, this._layerMask.value))
				{
					IEyeScannable componentInParent = raycastHit.collider.GetComponentInParent<IEyeScannable>();
					if (componentInParent == null || componentInParent != eyeScannable2)
					{
						goto IL_EF;
					}
				}
				num = num3;
				eyeScannable = eyeScannable2;
			}
			IL_EF:;
		}
		if (eyeScannable != this._oldClosestScannable)
		{
			if (this._oldClosestScannable != null)
			{
				this._oldClosestScannable.OnDataChange -= this.Scannable_OnDataChange;
			}
			this._OnScannableChanged(eyeScannable, true);
			this._oldClosestScannable = eyeScannable;
			if (this._oldClosestScannable != null)
			{
				this._oldClosestScannable.OnDataChange += this.Scannable_OnDataChange;
			}
		}
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x000194C5 File Offset: 0x000176C5
	private void Scannable_OnDataChange()
	{
		this._OnScannableChanged(this._oldClosestScannable, false);
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x000194D4 File Offset: 0x000176D4
	private void LateUpdate()
	{
		if (this._oldClosestScannable != null)
		{
			this.m_reticle.position = this._oldClosestScannable.Position;
			float num = math.distance(base.transform.position, this.m_reticle.position);
			Mathf.Clamp(num * 0.33333f, 0f, 1f);
			float num2 = num * this.m_reticleScale;
			float d = num * this.m_textScale;
			float num3 = num * this.m_overlayScale;
			this.m_reticle.localScale = new Vector3(num2, num2, num2);
			this.m_overlay.localPosition = new Vector3(this.m_position.x * num, this.m_position.y * num, num);
			this.m_overlay.localScale = new Vector3(num3, num3, 1f);
			this._line.SetPosition(0, this.m_reticle.position);
			this._line.SetPosition(1, this.m_textTyper.transform.position + this.m_pointerOffset * d);
			this._line.widthMultiplier = num2;
		}
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x00019600 File Offset: 0x00017800
	private void _OnScannableChanged(IEyeScannable scannable, bool typeingShow)
	{
		this._sb.Clear();
		if (scannable == null)
		{
			this.m_textTyper.SetText(this._sb);
			this.m_textTyper.gameObject.SetActive(false);
			this.m_reticle.gameObject.SetActive(false);
			this.m_overlayBg.SetActive(false);
			this.m_reticle.parent = base.transform;
			this._line.enabled = false;
			return;
		}
		this.m_reticle.gameObject.SetActive(true);
		this.m_textTyper.gameObject.SetActive(true);
		this.m_overlayBg.SetActive(true);
		this.m_reticle.position = scannable.Position;
		this._line.enabled = true;
		this._sb.AppendLine(this.DebugData);
		this._entryIndexes[0] = 0;
		int i = 1;
		int num = 0;
		for (int j = 0; j < scannable.Entries.Count; j++)
		{
			KeyValueStringPair keyValueStringPair = scannable.Entries[j];
			if (!string.IsNullOrEmpty(keyValueStringPair.Key))
			{
				this._sb.Append(this._keyRichTextColorTagString);
				this._sb.Append(keyValueStringPair.Key);
				this._sb.Append("</color>: ");
				num += keyValueStringPair.Key.Length + 2;
			}
			if (!string.IsNullOrEmpty(keyValueStringPair.Value))
			{
				this._sb.Append(keyValueStringPair.Value);
				num += keyValueStringPair.Value.Length;
			}
			this._sb.AppendLine();
			num += Environment.NewLine.Length;
			if (i < this._entryIndexes.Length)
			{
				this._entryIndexes[i++] = num - 1;
			}
		}
		while (i < this._entryIndexes.Length)
		{
			this._entryIndexes[i] = -1;
			i++;
		}
		if (typeingShow)
		{
			this.m_textTyper.SetText(this._sb, this._entryIndexes, num);
			return;
		}
		this.m_textTyper.UpdateText(this._sb, num);
	}

	// Token: 0x040004BE RID: 1214
	[FormerlySerializedAs("_scanDistance")]
	[Tooltip("Any scannables with transforms beyond this distance will be automatically ignored.")]
	[SerializeField]
	private float m_scanDistanceMax = 10f;

	// Token: 0x040004BF RID: 1215
	[SerializeField]
	private float m_scanDistanceMin = 0.5f;

	// Token: 0x040004C0 RID: 1216
	[FormerlySerializedAs("_textTyper")]
	[Tooltip("The component that handles setting text in the TextMeshPro and animates the text typing.")]
	[SerializeField]
	private TextTyperAnimatorMono m_textTyper;

	// Token: 0x040004C1 RID: 1217
	[SerializeField]
	private Transform m_reticle;

	// Token: 0x040004C2 RID: 1218
	[SerializeField]
	private Transform m_overlay;

	// Token: 0x040004C3 RID: 1219
	[SerializeField]
	private GameObject m_overlayBg;

	// Token: 0x040004C4 RID: 1220
	[SerializeField]
	private float m_reticleScale = 1f;

	// Token: 0x040004C5 RID: 1221
	[SerializeField]
	private float m_textScale = 1f;

	// Token: 0x040004C6 RID: 1222
	[SerializeField]
	private float m_overlayScale = 1f;

	// Token: 0x040004C7 RID: 1223
	[SerializeField]
	private Vector3 m_pointerOffset;

	// Token: 0x040004C8 RID: 1224
	[SerializeField]
	private Vector2 m_position;

	// Token: 0x040004C9 RID: 1225
	[HideInInspector]
	[SerializeField]
	private Color32 m_keyTextColor = new Color32(byte.MaxValue, 34, 0, byte.MaxValue);

	// Token: 0x040004CA RID: 1226
	private string _keyRichTextColorTagString = "";

	// Token: 0x040004CB RID: 1227
	private static readonly List<IEyeScannable> _registeredScannables = new List<IEyeScannable>(128);

	// Token: 0x040004CC RID: 1228
	private static readonly HashSet<int> _registeredScannableIds = new HashSet<int>(128);

	// Token: 0x040004CD RID: 1229
	private IEyeScannable _oldClosestScannable;

	// Token: 0x040004CE RID: 1230
	private Utf16ValueStringBuilder _sb;

	// Token: 0x040004CF RID: 1231
	private readonly int[] _entryIndexes = new int[16];

	// Token: 0x040004D0 RID: 1232
	[SerializeField]
	private LayerMask _layerMask;

	// Token: 0x040004D1 RID: 1233
	private Camera _firstPersonCamera;

	// Token: 0x040004D2 RID: 1234
	private bool _has_firstPersonCamera;

	// Token: 0x040004D6 RID: 1238
	[SerializeField]
	private float m_LookPrecision = 0.65f;

	// Token: 0x040004D7 RID: 1239
	[SerializeField]
	private bool m_xrayVision;

	// Token: 0x040004D8 RID: 1240
	private LineRenderer _line;
}
