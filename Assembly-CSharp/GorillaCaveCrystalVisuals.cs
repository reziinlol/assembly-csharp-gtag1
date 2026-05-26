using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x0200084F RID: 2127
public class GorillaCaveCrystalVisuals : MonoBehaviour
{
	// Token: 0x170004E4 RID: 1252
	// (get) Token: 0x0600371F RID: 14111 RVA: 0x0012EA06 File Offset: 0x0012CC06
	// (set) Token: 0x06003720 RID: 14112 RVA: 0x0012EA0E File Offset: 0x0012CC0E
	public float lerp
	{
		get
		{
			return this._lerp;
		}
		set
		{
			this._lerp = value;
		}
	}

	// Token: 0x06003721 RID: 14113 RVA: 0x0012EA18 File Offset: 0x0012CC18
	public void Setup()
	{
		base.TryGetComponent<MeshRenderer>(out this._renderer);
		if (this._renderer == null)
		{
			return;
		}
		this._setup = GorillaCaveCrystalSetup.Instance;
		this._sharedMaterial = this._renderer.sharedMaterial;
		this._initialized = (this.crysalPreset != null && this._renderer != null && this._sharedMaterial != null);
		this.Update();
	}

	// Token: 0x06003722 RID: 14114 RVA: 0x0012EA94 File Offset: 0x0012CC94
	private void Start()
	{
		this.UpdateAlbedo();
		this.ForceUpdate();
	}

	// Token: 0x06003723 RID: 14115 RVA: 0x0012EAA4 File Offset: 0x0012CCA4
	public void UpdateAlbedo()
	{
		if (!this._initialized)
		{
			return;
		}
		if (this.instanceAlbedo == null)
		{
			return;
		}
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		this._renderer.GetPropertyBlock(this._block);
		this._block.SetTexture(GorillaCaveCrystalVisuals._MainTex, this.instanceAlbedo);
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x06003724 RID: 14116 RVA: 0x0012EB19 File Offset: 0x0012CD19
	private void Awake()
	{
		this.UpdateAlbedo();
		this.Update();
	}

	// Token: 0x06003725 RID: 14117 RVA: 0x0012EB28 File Offset: 0x0012CD28
	private void Update()
	{
		if (!this._initialized)
		{
			return;
		}
		if (Application.isPlaying)
		{
			int hashCode = new ValueTuple<CrystalVisualsPreset, float>(this.crysalPreset, this._lerp).GetHashCode();
			if (this._lastState == hashCode)
			{
				return;
			}
			this._lastState = hashCode;
		}
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		CrystalVisualsPreset.VisualState stateA = this.crysalPreset.stateA;
		CrystalVisualsPreset.VisualState stateB = this.crysalPreset.stateB;
		Color value = Color.Lerp(stateA.albedo, stateB.albedo, this._lerp);
		Color value2 = Color.Lerp(stateA.emission, stateB.emission, this._lerp);
		this._renderer.GetPropertyBlock(this._block);
		this._block.SetColor(GorillaCaveCrystalVisuals._Color, value);
		this._block.SetColor(GorillaCaveCrystalVisuals._EmissionColor, value2);
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x06003726 RID: 14118 RVA: 0x0012EC1E File Offset: 0x0012CE1E
	public void ForceUpdate()
	{
		this._lastState = 0;
		this.Update();
	}

	// Token: 0x06003727 RID: 14119 RVA: 0x0012EC30 File Offset: 0x0012CE30
	private static void InitializeCrystals()
	{
		foreach (GorillaCaveCrystalVisuals gorillaCaveCrystalVisuals in Object.FindObjectsByType<GorillaCaveCrystalVisuals>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
		{
			gorillaCaveCrystalVisuals.UpdateAlbedo();
			gorillaCaveCrystalVisuals.ForceUpdate();
			gorillaCaveCrystalVisuals._lastState = -1;
		}
	}

	// Token: 0x04004735 RID: 18229
	public CrystalVisualsPreset crysalPreset;

	// Token: 0x04004736 RID: 18230
	[SerializeField]
	[Range(0f, 1f)]
	private float _lerp;

	// Token: 0x04004737 RID: 18231
	[Space]
	public MeshRenderer _renderer;

	// Token: 0x04004738 RID: 18232
	public Material _sharedMaterial;

	// Token: 0x04004739 RID: 18233
	[SerializeField]
	public Texture2D instanceAlbedo;

	// Token: 0x0400473A RID: 18234
	[SerializeField]
	private bool _initialized;

	// Token: 0x0400473B RID: 18235
	[SerializeField]
	private int _lastState;

	// Token: 0x0400473C RID: 18236
	[SerializeField]
	public GorillaCaveCrystalSetup _setup;

	// Token: 0x0400473D RID: 18237
	private MaterialPropertyBlock _block;

	// Token: 0x0400473E RID: 18238
	[NonSerialized]
	private bool _ranSetupOnce;

	// Token: 0x0400473F RID: 18239
	private static readonly ShaderHashId _Color = "_Color";

	// Token: 0x04004740 RID: 18240
	private static readonly ShaderHashId _EmissionColor = "_EmissionColor";

	// Token: 0x04004741 RID: 18241
	private static readonly ShaderHashId _MainTex = "_MainTex";
}
