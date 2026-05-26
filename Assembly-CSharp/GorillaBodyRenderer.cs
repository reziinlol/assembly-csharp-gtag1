using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000849 RID: 2121
public class GorillaBodyRenderer : MonoBehaviour
{
	// Token: 0x170004DE RID: 1246
	// (get) Token: 0x060036F5 RID: 14069 RVA: 0x0012E0F0 File Offset: 0x0012C2F0
	// (set) Token: 0x060036F6 RID: 14070 RVA: 0x0012E0F8 File Offset: 0x0012C2F8
	public GorillaBodyType bodyType
	{
		get
		{
			return this._bodyType;
		}
		set
		{
			this.SetBodyType(value);
		}
	}

	// Token: 0x170004DF RID: 1247
	// (get) Token: 0x060036F7 RID: 14071 RVA: 0x0012E101 File Offset: 0x0012C301
	public bool renderFace
	{
		get
		{
			return this._renderFace;
		}
	}

	// Token: 0x170004E0 RID: 1248
	// (get) Token: 0x060036F8 RID: 14072 RVA: 0x0012E109 File Offset: 0x0012C309
	public static bool ForceSkeleton
	{
		get
		{
			return GorillaBodyRenderer.oopsAllSkeletons;
		}
	}

	// Token: 0x170004E1 RID: 1249
	// (get) Token: 0x060036F9 RID: 14073 RVA: 0x0012E110 File Offset: 0x0012C310
	// (set) Token: 0x060036FA RID: 14074 RVA: 0x0012E118 File Offset: 0x0012C318
	public GorillaBodyType gameModeBodyType { get; private set; }

	// Token: 0x170004E2 RID: 1250
	// (get) Token: 0x060036FB RID: 14075 RVA: 0x0012E121 File Offset: 0x0012C321
	// (set) Token: 0x060036FC RID: 14076 RVA: 0x0012E129 File Offset: 0x0012C329
	public Material myDefaultSkinMaterialInstance { get; private set; }

	// Token: 0x060036FD RID: 14077 RVA: 0x0012E134 File Offset: 0x0012C334
	public SkinnedMeshRenderer GetBody(GorillaBodyType type)
	{
		if (type < GorillaBodyType.Default || type >= (GorillaBodyType)this._renderersCache.Length)
		{
			return null;
		}
		return this._renderersCache[(int)type];
	}

	// Token: 0x170004E3 RID: 1251
	// (get) Token: 0x060036FE RID: 14078 RVA: 0x0012E15C File Offset: 0x0012C35C
	public SkinnedMeshRenderer ActiveBody
	{
		get
		{
			return this.GetBody(this._bodyType);
		}
	}

	// Token: 0x060036FF RID: 14079 RVA: 0x0012E16C File Offset: 0x0012C36C
	public static void SetAllSkeletons(bool allSkeletons)
	{
		GorillaBodyRenderer.oopsAllSkeletons = allSkeletons;
		GorillaTagger.Instance.offlineVRRig.bodyRenderer.Refresh();
		foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
		{
			rigContainer.Rig.bodyRenderer.Refresh();
		}
	}

	// Token: 0x06003700 RID: 14080 RVA: 0x0012E1DC File Offset: 0x0012C3DC
	public void SetSkeletonBodyActive(bool active)
	{
		this.bodySkeleton.gameObject.SetActive(active);
	}

	// Token: 0x06003701 RID: 14081 RVA: 0x0012E1EF File Offset: 0x0012C3EF
	public static void EnableSkeletonOverlays(Material bodyMaterial, Material skeletonMaterial)
	{
		GorillaBodyRenderer.<>c__DisplayClass33_0 CS$<>8__locals1 = new GorillaBodyRenderer.<>c__DisplayClass33_0();
		CS$<>8__locals1.bodyMaterial = bodyMaterial;
		CS$<>8__locals1.skeletonMaterial = skeletonMaterial;
		CS$<>8__locals1.<EnableSkeletonOverlays>g__ShowSkeletonOverlay|0(GorillaTagger.Instance.offlineVRRig);
		VRRigCache.ApplyToAllRigs(new Action<VRRig>(CS$<>8__locals1.<EnableSkeletonOverlays>g__ShowSkeletonOverlay|0));
	}

	// Token: 0x06003702 RID: 14082 RVA: 0x0012E224 File Offset: 0x0012C424
	public static void DisableSkeletonOverlays()
	{
		GorillaBodyRenderer.HideSkeletonOverlay(GorillaTagger.Instance.offlineVRRig);
		VRRigCache.ApplyToAllRigs(new Action<VRRig>(GorillaBodyRenderer.HideSkeletonOverlay));
	}

	// Token: 0x06003703 RID: 14083 RVA: 0x0012E246 File Offset: 0x0012C446
	private static void HideSkeletonOverlay(VRRig rig)
	{
		rig.bodyRenderer.Refresh();
		rig.bodyRenderer.bodyDefault.sharedMaterial = rig.bodyRenderer.myDefaultSkinMaterialInstance;
	}

	// Token: 0x06003704 RID: 14084 RVA: 0x0012E26E File Offset: 0x0012C46E
	public void SetGameModeBodyType(GorillaBodyType bodyType)
	{
		if (this.gameModeBodyType == bodyType)
		{
			return;
		}
		this.gameModeBodyType = bodyType;
		this.Refresh();
	}

	// Token: 0x06003705 RID: 14085 RVA: 0x0012E287 File Offset: 0x0012C487
	public void SetCosmeticBodyType(GorillaBodyType bodyType)
	{
		if (this.cosmeticBodyType == bodyType)
		{
			return;
		}
		this.cosmeticBodyType = bodyType;
		this.Refresh();
	}

	// Token: 0x06003706 RID: 14086 RVA: 0x0012E2A0 File Offset: 0x0012C4A0
	public void SetDefaults()
	{
		this.gameModeBodyType = GorillaBodyType.Default;
		this.cosmeticBodyType = GorillaBodyType.Default;
		this.Refresh();
	}

	// Token: 0x06003707 RID: 14087 RVA: 0x0012E2B6 File Offset: 0x0012C4B6
	private void Refresh()
	{
		this.SetBodyType(this.GetActiveBodyType());
	}

	// Token: 0x06003708 RID: 14088 RVA: 0x0012E2C4 File Offset: 0x0012C4C4
	public void SetMaterialIndex(int materialIndex)
	{
		this._lastMatIndex = materialIndex;
		switch (this.bodyType)
		{
		case GorillaBodyType.Default:
			this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[materialIndex];
			return;
		case GorillaBodyType.NoHead:
			if (materialIndex == 0 && !this._applySkinToHeadlessMesh)
			{
				this.bodyNoHead.sharedMaterial = this.myDefaultSkinMaterialInstance;
				return;
			}
			this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[materialIndex];
			return;
		case GorillaBodyType.Skeleton:
			this.rig.skeleton.SetMaterialIndex(materialIndex);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003709 RID: 14089 RVA: 0x0012E354 File Offset: 0x0012C554
	public void SetSkinMaterials(Material bodyMat, Material chestMat, bool allowHeadless)
	{
		this.EnsureInstantiatedMaterial();
		if (chestMat == null)
		{
			if (this._cachedSkinMaterials.Length != 1)
			{
				this._cachedSkinMaterials = new Material[1];
			}
			this._cachedSkinMaterials[0] = bodyMat;
		}
		else
		{
			if (this._cachedSkinMaterials.Length < 2)
			{
				this._cachedSkinMaterials = new Material[2];
			}
			this._cachedSkinMaterials[0] = bodyMat;
			this._cachedSkinMaterials[1] = chestMat;
		}
		this._applySkinToHeadlessMesh = allowHeadless;
		GorillaBodyType bodyType = this.bodyType;
		if (bodyType == GorillaBodyType.Default)
		{
			this.bodyDefault.sharedMaterials = this._cachedSkinMaterials;
			this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
			return;
		}
		if (bodyType != GorillaBodyType.NoHead)
		{
			return;
		}
		if (this._applySkinToHeadlessMesh)
		{
			this.bodyNoHead.sharedMaterials = this._cachedSkinMaterials;
			this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
			return;
		}
		this.bodyNoHead.sharedMaterials = this._defaultSkinMaterials;
		if (this._lastMatIndex != 0)
		{
			this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
		}
	}

	// Token: 0x0600370A RID: 14090 RVA: 0x0012E46D File Offset: 0x0012C66D
	public void SetupAsLocalPlayerBody()
	{
		this.faceRenderer.gameObject.layer = 22;
	}

	// Token: 0x0600370B RID: 14091 RVA: 0x0012E481 File Offset: 0x0012C681
	public GorillaBodyType GetActiveBodyType()
	{
		if (GorillaBodyRenderer.oopsAllSkeletons)
		{
			return GorillaBodyType.Skeleton;
		}
		if (this.gameModeBodyType == GorillaBodyType.Default)
		{
			return this.cosmeticBodyType;
		}
		return this.gameModeBodyType;
	}

	// Token: 0x0600370C RID: 14092 RVA: 0x0012E4A4 File Offset: 0x0012C6A4
	private void SetBodyType(GorillaBodyType type)
	{
		if (this._bodyType == type)
		{
			return;
		}
		this.SetBodyEnabled(this._bodyType, false);
		this._bodyType = type;
		this.SetBodyEnabled(type, true);
		this._renderFace = (this._bodyType != GorillaBodyType.NoHead && this._bodyType != GorillaBodyType.Skeleton && this._bodyType != GorillaBodyType.Invisible);
		if (this.faceRenderer != null)
		{
			this.faceRenderer.enabled = this._renderFace;
		}
		switch (type)
		{
		case GorillaBodyType.Default:
			this.bodyDefault.sharedMaterials = this._cachedSkinMaterials;
			this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
			this.UpdateBodyMaterialColor(this.rig.playerColor);
			return;
		case GorillaBodyType.NoHead:
			if (this._applySkinToHeadlessMesh)
			{
				this.bodyNoHead.sharedMaterials = this._cachedSkinMaterials;
				this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
			}
			else
			{
				this.bodyNoHead.sharedMaterials = this._defaultSkinMaterials;
				if (this._lastMatIndex != 0)
				{
					this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
				}
			}
			this.UpdateBodyMaterialColor(this.rig.playerColor);
			return;
		case GorillaBodyType.Skeleton:
			this.rig.skeleton.SetMaterialIndex(this._lastMatIndex);
			this.rig.skeleton.UpdateColor(this.rig.playerColor);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600370D RID: 14093 RVA: 0x0012E621 File Offset: 0x0012C821
	public void SetCosmeticBodyMesh(Mesh mesh)
	{
		if (this.defaultBodyMesh == null)
		{
			this.defaultBodyMesh = this.bodyDefault.sharedMesh;
		}
		this.bodyDefault.sharedMesh = mesh;
	}

	// Token: 0x0600370E RID: 14094 RVA: 0x0012E64E File Offset: 0x0012C84E
	public void ClearCosmeticBodyMesh()
	{
		if (this.defaultBodyMesh != null)
		{
			this.bodyDefault.sharedMesh = this.defaultBodyMesh;
		}
	}

	// Token: 0x0600370F RID: 14095 RVA: 0x0012E670 File Offset: 0x0012C870
	private void SetBodyEnabled(GorillaBodyType bodyType, bool enabled)
	{
		SkinnedMeshRenderer body = this.GetBody(bodyType);
		if (body == null)
		{
			return;
		}
		body.enabled = enabled;
		Transform[] bones = body.bones;
		for (int i = 0; i < bones.Length; i++)
		{
			bones[i].gameObject.SetActive(enabled);
		}
	}

	// Token: 0x06003710 RID: 14096 RVA: 0x0012E6B9 File Offset: 0x0012C8B9
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06003711 RID: 14097 RVA: 0x0012E6C1 File Offset: 0x0012C8C1
	public void SharedStart()
	{
		if (this.rig == null)
		{
			this.rig = base.GetComponentInParent<VRRig>();
		}
		this.EnsureInstantiatedMaterial();
	}

	// Token: 0x06003712 RID: 14098 RVA: 0x0012E6E4 File Offset: 0x0012C8E4
	private void Setup()
	{
		if (this.rig == null)
		{
			this.rig = base.GetComponentInParent<VRRig>();
		}
		this._renderersCache = new SkinnedMeshRenderer[EnumData<GorillaBodyType>.Shared.Values.Length];
		this._renderersCache[0] = this.bodyDefault;
		this._renderersCache[1] = this.bodyNoHead;
		this._renderersCache[2] = this.bodySkeleton;
		this.SetBodyEnabled(GorillaBodyType.Default, true);
		this.SetBodyEnabled(GorillaBodyType.NoHead, false);
		this.SetBodyEnabled(GorillaBodyType.Skeleton, false);
		this._cachedSkinMaterials = this.bodyDefault.sharedMaterials;
		this._bodyType = GorillaBodyType.Default;
		this._bodyType = GorillaBodyType.Default;
		this.defaultBodyMesh = this.bodyDefault.sharedMesh;
		this.EnsureInstantiatedMaterial();
		this.UpdateColor(this.rig.playerColor);
		this.Refresh();
	}

	// Token: 0x06003713 RID: 14099 RVA: 0x0012E7B4 File Offset: 0x0012C9B4
	public void EnsureInstantiatedMaterial()
	{
		if (this.myDefaultSkinMaterialInstance == null)
		{
			this.myDefaultSkinMaterialInstance = Object.Instantiate<Material>(this.rig.materialsToChangeTo[0]);
			this.rig.materialsToChangeTo[0] = this.myDefaultSkinMaterialInstance;
		}
		if (this._defaultSkinMaterials.Length == 0)
		{
			this._defaultSkinMaterials = new Material[2];
			this._defaultSkinMaterials[0] = this.myDefaultSkinMaterialInstance;
			this._defaultSkinMaterials[1] = this.rig.defaultSkin.chestMaterial;
		}
	}

	// Token: 0x06003714 RID: 14100 RVA: 0x0012E838 File Offset: 0x0012CA38
	public void ResetBodyMaterial()
	{
		this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[0];
		this.bodyNoHead.sharedMaterial = (this._applySkinToHeadlessMesh ? this.rig.materialsToChangeTo[0] : this.myDefaultSkinMaterialInstance);
	}

	// Token: 0x06003715 RID: 14101 RVA: 0x0012E885 File Offset: 0x0012CA85
	public void UpdateColor(Color color)
	{
		this.UpdateBodyMaterialColor(color);
		if (this.bodyType == GorillaBodyType.Skeleton)
		{
			this.rig.skeleton.UpdateColor(color);
		}
	}

	// Token: 0x06003716 RID: 14102 RVA: 0x0012E8A8 File Offset: 0x0012CAA8
	private void UpdateBodyMaterialColor(Color color)
	{
		this.EnsureInstantiatedMaterial();
		if (this.myDefaultSkinMaterialInstance != null)
		{
			this.myDefaultSkinMaterialInstance.color = color;
		}
	}

	// Token: 0x04004705 RID: 18181
	[SerializeField]
	private GorillaBodyType _bodyType;

	// Token: 0x04004706 RID: 18182
	[SerializeField]
	private bool _renderFace = true;

	// Token: 0x04004707 RID: 18183
	public MeshRenderer faceRenderer;

	// Token: 0x04004708 RID: 18184
	[SerializeField]
	private SkinnedMeshRenderer bodyDefault;

	// Token: 0x04004709 RID: 18185
	[SerializeField]
	private SkinnedMeshRenderer bodyNoHead;

	// Token: 0x0400470A RID: 18186
	[SerializeField]
	private SkinnedMeshRenderer bodySkeleton;

	// Token: 0x0400470B RID: 18187
	private int _lastMatIndex;

	// Token: 0x0400470C RID: 18188
	private Mesh defaultBodyMesh;

	// Token: 0x0400470D RID: 18189
	private static bool oopsAllSkeletons;

	// Token: 0x0400470F RID: 18191
	private GorillaBodyType cosmeticBodyType;

	// Token: 0x04004711 RID: 18193
	[SerializeField]
	private Material[] _cachedSkinMaterials = new Material[0];

	// Token: 0x04004712 RID: 18194
	[SerializeField]
	private Material[] _defaultSkinMaterials = new Material[0];

	// Token: 0x04004713 RID: 18195
	private bool _applySkinToHeadlessMesh;

	// Token: 0x04004714 RID: 18196
	[Space]
	[NonSerialized]
	private SkinnedMeshRenderer[] _renderersCache = new SkinnedMeshRenderer[0];

	// Token: 0x04004715 RID: 18197
	private static readonly List<Material> gEmptyDefaultMats = new List<Material>();

	// Token: 0x04004716 RID: 18198
	[Space]
	public VRRig rig;
}
