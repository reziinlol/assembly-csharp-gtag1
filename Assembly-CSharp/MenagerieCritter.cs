using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000087 RID: 135
public class MenagerieCritter : MonoBehaviour, IHoldableObject, IEyeScannable
{
	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000362 RID: 866 RVA: 0x000141EA File Offset: 0x000123EA
	public Menagerie.CritterData CritterData
	{
		get
		{
			return this._critterData;
		}
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x06000363 RID: 867 RVA: 0x000141F2 File Offset: 0x000123F2
	// (set) Token: 0x06000364 RID: 868 RVA: 0x000141FC File Offset: 0x000123FC
	public MenagerieSlot Slot
	{
		get
		{
			return this._slot;
		}
		set
		{
			if (value == this._slot)
			{
				return;
			}
			if (this._slot && this._slot.critter == this)
			{
				this._slot.critter = null;
			}
			this._slot = value;
			if (this._slot)
			{
				this._slot.critter = this;
			}
		}
	}

	// Token: 0x06000365 RID: 869 RVA: 0x00014264 File Offset: 0x00012464
	private void Update()
	{
		this.UpdateAnimation();
	}

	// Token: 0x06000366 RID: 870 RVA: 0x0001426C File Offset: 0x0001246C
	public void ApplyCritterData(Menagerie.CritterData critterData)
	{
		this._critterData = critterData;
		this._critterConfiguration = this._critterData.GetConfiguration();
		this._critterData.instance = this;
		this._critterData.GetConfiguration().ApplyVisualsTo(this.visuals, false);
		this.visuals.SetAppearance(this._critterData.appearance);
		this._animRoot = this.visuals.bodyRoot;
		this._bodyScale = this._animRoot.localScale;
		this.PlayAnimation(this.heldAnimation, UnityEngine.Random.value);
	}

	// Token: 0x06000367 RID: 871 RVA: 0x00014300 File Offset: 0x00012500
	private void PlayAnimation(CrittersAnim anim, float time = 0f)
	{
		this._currentAnim = anim;
		this._currentAnimTime = time;
		if (this._currentAnim == null)
		{
			this._animRoot.localPosition = Vector3.zero;
			this._animRoot.localRotation = Quaternion.identity;
			this._animRoot.localScale = this._bodyScale;
		}
	}

	// Token: 0x06000368 RID: 872 RVA: 0x00014354 File Offset: 0x00012554
	private void UpdateAnimation()
	{
		if (this._currentAnim != null)
		{
			this._currentAnimTime += Time.deltaTime * this._currentAnim.playSpeed;
			this._currentAnimTime %= 1f;
			float num = this._currentAnim.squashAmount.Evaluate(this._currentAnimTime);
			float z = this._currentAnim.forwardOffset.Evaluate(this._currentAnimTime);
			float x = this._currentAnim.horizontalOffset.Evaluate(this._currentAnimTime);
			float y = this._currentAnim.verticalOffset.Evaluate(this._currentAnimTime);
			this._animRoot.localPosition = Vector3.Scale(this._bodyScale, new Vector3(x, y, z));
			float num2 = 1f - num;
			num2 *= 0.5f;
			num2 += 1f;
			this._animRoot.localScale = Vector3.Scale(this._bodyScale, new Vector3(num2, num, num2));
		}
	}

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x06000369 RID: 873 RVA: 0x00002076 File Offset: 0x00000276
	public bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600036A RID: 874 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x0600036B RID: 875 RVA: 0x00014454 File Offset: 0x00012654
	public void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		this.isHeld = true;
		this.isHeldLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
		if (this.grabbedHaptics)
		{
			CrittersManager.PlayHaptics(this.grabbedHaptics, this.grabbedHapticsStrength, this.isHeldLeftHand);
		}
		if (this.grabbedFX)
		{
			this.grabbedFX.SetActive(true);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		base.transform.parent = grabbingHand.transform;
		this.isHeld = true;
		this.heldBy = grabbingHand;
		Action onDataChange = this.OnDataChange;
		if (onDataChange == null)
		{
			return;
		}
		onDataChange();
	}

	// Token: 0x0600036C RID: 876 RVA: 0x00014500 File Offset: 0x00012700
	public bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
		{
			return false;
		}
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
		{
			return false;
		}
		if (this.grabbedHaptics)
		{
			CrittersManager.StopHaptics(this.isHeldLeftHand);
		}
		if (this.grabbedFX)
		{
			this.grabbedFX.SetActive(false);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
		this.isHeld = false;
		this.isHeldLeftHand = false;
		Action<MenagerieCritter> onReleased = this.OnReleased;
		if (onReleased != null)
		{
			onReleased(this);
		}
		Action onDataChange = this.OnDataChange;
		if (onDataChange != null)
		{
			onDataChange();
		}
		this.ResetToTransform();
		return true;
	}

	// Token: 0x0600036D RID: 877 RVA: 0x000145D3 File Offset: 0x000127D3
	public void ResetToTransform()
	{
		base.transform.parent = this._slot.transform;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = quaternion.identity;
	}

	// Token: 0x0600036E RID: 878 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void DropItemCleanup()
	{
	}

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x0600036F RID: 879 RVA: 0x00010CFB File Offset: 0x0000EEFB
	int IEyeScannable.scannableId
	{
		get
		{
			return base.gameObject.GetInstanceID();
		}
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x06000370 RID: 880 RVA: 0x00014610 File Offset: 0x00012810
	Vector3 IEyeScannable.Position
	{
		get
		{
			return this.bodyCollider.bounds.center;
		}
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x06000371 RID: 881 RVA: 0x00014630 File Offset: 0x00012830
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return this.bodyCollider.bounds;
		}
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x06000372 RID: 882 RVA: 0x0001463D File Offset: 0x0001283D
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.BuildEyeScannerData();
		}
	}

	// Token: 0x06000373 RID: 883 RVA: 0x00014645 File Offset: 0x00012845
	public void OnEnable()
	{
		EyeScannerMono.Register(this);
	}

	// Token: 0x06000374 RID: 884 RVA: 0x0001464D File Offset: 0x0001284D
	public void OnDisable()
	{
		EyeScannerMono.Unregister(this);
	}

	// Token: 0x06000375 RID: 885 RVA: 0x00014658 File Offset: 0x00012858
	private IList<KeyValueStringPair> BuildEyeScannerData()
	{
		this.eyeScanData[0] = new KeyValueStringPair("Name", this._critterConfiguration.critterName);
		this.eyeScanData[1] = new KeyValueStringPair("Type", this._critterConfiguration.animalType.ToString());
		this.eyeScanData[2] = new KeyValueStringPair("Temperament", this._critterConfiguration.behaviour.temperament);
		this.eyeScanData[3] = new KeyValueStringPair("Habitat", this._critterConfiguration.biome.GetHabitatDescription());
		this.eyeScanData[4] = new KeyValueStringPair("Size", this.visuals.Appearance.size.ToString("0.00"));
		this.eyeScanData[5] = new KeyValueStringPair("State", this.GetCurrentStateName());
		return this.eyeScanData;
	}

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x06000376 RID: 886 RVA: 0x00014754 File Offset: 0x00012954
	// (remove) Token: 0x06000377 RID: 887 RVA: 0x0001478C File Offset: 0x0001298C
	public event Action OnDataChange;

	// Token: 0x06000378 RID: 888 RVA: 0x000147C1 File Offset: 0x000129C1
	private string GetCurrentStateName()
	{
		if (!this.isHeld)
		{
			return "Content";
		}
		return "Happy";
	}

	// Token: 0x0600037A RID: 890 RVA: 0x0000636B File Offset: 0x0000456B
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0600037B RID: 891 RVA: 0x00014807 File Offset: 0x00012A07
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x0600037C RID: 892 RVA: 0x0001480F File Offset: 0x00012A0F
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}

	// Token: 0x040003E9 RID: 1001
	public CritterVisuals visuals;

	// Token: 0x040003EA RID: 1002
	public Collider bodyCollider;

	// Token: 0x040003EB RID: 1003
	[Header("Feedback")]
	public CrittersAnim heldAnimation;

	// Token: 0x040003EC RID: 1004
	public AudioClip grabbedHaptics;

	// Token: 0x040003ED RID: 1005
	public float grabbedHapticsStrength = 1f;

	// Token: 0x040003EE RID: 1006
	public GameObject grabbedFX;

	// Token: 0x040003EF RID: 1007
	private CrittersAnim _currentAnim;

	// Token: 0x040003F0 RID: 1008
	private float _currentAnimTime;

	// Token: 0x040003F1 RID: 1009
	private Transform _animRoot;

	// Token: 0x040003F2 RID: 1010
	private Vector3 _bodyScale;

	// Token: 0x040003F3 RID: 1011
	public MenagerieCritter.MenagerieCritterState currentState = MenagerieCritter.MenagerieCritterState.Displaying;

	// Token: 0x040003F4 RID: 1012
	private CritterConfiguration _critterConfiguration;

	// Token: 0x040003F5 RID: 1013
	private Menagerie.CritterData _critterData;

	// Token: 0x040003F6 RID: 1014
	private MenagerieSlot _slot;

	// Token: 0x040003F7 RID: 1015
	private List<GorillaGrabber> activeGrabbers = new List<GorillaGrabber>();

	// Token: 0x040003F8 RID: 1016
	private GameObject heldBy;

	// Token: 0x040003F9 RID: 1017
	private bool isHeld;

	// Token: 0x040003FA RID: 1018
	private bool isHeldLeftHand;

	// Token: 0x040003FB RID: 1019
	public Action<MenagerieCritter> OnReleased;

	// Token: 0x040003FC RID: 1020
	private KeyValueStringPair[] eyeScanData = new KeyValueStringPair[6];

	// Token: 0x02000088 RID: 136
	public enum MenagerieCritterState
	{
		// Token: 0x040003FF RID: 1023
		Donating,
		// Token: 0x04000400 RID: 1024
		Displaying
	}
}
