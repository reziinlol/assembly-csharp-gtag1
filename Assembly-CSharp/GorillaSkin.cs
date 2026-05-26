using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020002D0 RID: 720
public class GorillaSkin : ScriptableObject
{
	// Token: 0x170001CD RID: 461
	// (get) Token: 0x06001277 RID: 4727 RVA: 0x000626E6 File Offset: 0x000608E6
	public Mesh bodyMesh
	{
		get
		{
			return this._bodyMesh;
		}
	}

	// Token: 0x170001CE RID: 462
	// (get) Token: 0x06001278 RID: 4728 RVA: 0x000626EE File Offset: 0x000608EE
	public bool allowHeadless
	{
		get
		{
			return !this._disableHeadless;
		}
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x000626FC File Offset: 0x000608FC
	public static GorillaSkin CopyWithInstancedMaterials(GorillaSkin basis)
	{
		GorillaSkin gorillaSkin = ScriptableObject.CreateInstance<GorillaSkin>();
		gorillaSkin._chestMaterial = ((basis._chestMaterial != null) ? new Material(basis._chestMaterial) : null);
		gorillaSkin._bodyMaterial = ((basis._bodyMaterial != null) ? new Material(basis._bodyMaterial) : null);
		gorillaSkin._scoreboardMaterial = ((basis._scoreboardMaterial != null) ? new Material(basis._scoreboardMaterial) : null);
		gorillaSkin._bodyMesh = basis.bodyMesh;
		return gorillaSkin;
	}

	// Token: 0x170001CF RID: 463
	// (get) Token: 0x0600127A RID: 4730 RVA: 0x00062780 File Offset: 0x00060980
	public Material bodyMaterial
	{
		get
		{
			return this._bodyMaterial;
		}
	}

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x0600127B RID: 4731 RVA: 0x00062788 File Offset: 0x00060988
	public Material chestMaterial
	{
		get
		{
			return this._chestMaterial;
		}
	}

	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x0600127C RID: 4732 RVA: 0x00062790 File Offset: 0x00060990
	public Material scoreboardMaterial
	{
		get
		{
			return this._scoreboardMaterial;
		}
	}

	// Token: 0x0600127D RID: 4733 RVA: 0x00062798 File Offset: 0x00060998
	public static void ShowActiveSkin(VRRig rig)
	{
		bool useDefaultBodySkin;
		GorillaSkin activeSkin = GorillaSkin.GetActiveSkin(rig, out useDefaultBodySkin);
		GorillaSkin.ShowSkin(rig, activeSkin, useDefaultBodySkin);
	}

	// Token: 0x0600127E RID: 4734 RVA: 0x000627B8 File Offset: 0x000609B8
	public void ApplySkinToMannequin(GameObject mannequin, bool swapMesh = false)
	{
		SkinnedMeshRenderer skinnedMeshRenderer;
		if (!mannequin.TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
		{
			MeshRenderer meshRenderer;
			if (mannequin.TryGetComponent<MeshRenderer>(out meshRenderer))
			{
				meshRenderer.GetSharedMaterials(GorillaSkin._g_sharedMaterialsCache);
				GorillaSkin._g_sharedMaterialsCache[0] = this.bodyMaterial;
				GorillaSkin._g_sharedMaterialsCache[1] = this.chestMaterial;
				meshRenderer.SetSharedMaterials(GorillaSkin._g_sharedMaterialsCache);
			}
			return;
		}
		int subMeshCount = skinnedMeshRenderer.sharedMesh.subMeshCount;
		if (swapMesh && this.bodyMesh != null)
		{
			skinnedMeshRenderer.sharedMesh = this.bodyMesh;
		}
		int subMeshCount2 = skinnedMeshRenderer.sharedMesh.subMeshCount;
		skinnedMeshRenderer.GetSharedMaterials(GorillaSkin._g_sharedMaterialsCache);
		if (subMeshCount == subMeshCount2)
		{
			GorillaSkin._g_sharedMaterialsCache[0] = this.bodyMaterial;
			if (subMeshCount > 2)
			{
				GorillaSkin._g_sharedMaterialsCache[1] = this.chestMaterial;
			}
			skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_sharedMaterialsCache);
			return;
		}
		if (GorillaSkin._g_sharedMaterialsCache.Count == subMeshCount)
		{
			if (subMeshCount2 == 2 && subMeshCount > subMeshCount2)
			{
				GorillaSkin._g_materialsWriteCache.Clear();
				GorillaSkin._g_materialsWriteCache.Add(this.bodyMaterial);
				GorillaSkin._g_materialsWriteCache.Add(GorillaSkin._g_sharedMaterialsCache[2]);
				skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_materialsWriteCache);
				return;
			}
			if (subMeshCount2 == 3 && subMeshCount < subMeshCount2 && GorillaSkin._g_sharedMaterialsCache.Count > 1)
			{
				GorillaSkin._g_materialsWriteCache.Clear();
				GorillaSkin._g_materialsWriteCache.Add(this.bodyMaterial);
				GorillaSkin._g_materialsWriteCache.Add(this.chestMaterial);
				GorillaSkin._g_materialsWriteCache.Add(GorillaSkin._g_sharedMaterialsCache[1]);
				skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_materialsWriteCache);
				return;
			}
			Debug.LogError(string.Format("Unexpected Submesh count {0} {1}", subMeshCount, subMeshCount2));
			return;
		}
		else
		{
			if (subMeshCount2 == 2)
			{
				GorillaSkin._g_materialsWriteCache.Clear();
				GorillaSkin._g_materialsWriteCache.Add(this.bodyMaterial);
				skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_materialsWriteCache);
				return;
			}
			if (subMeshCount2 == 3)
			{
				GorillaSkin._g_materialsWriteCache.Clear();
				GorillaSkin._g_materialsWriteCache.Add(this.bodyMaterial);
				GorillaSkin._g_materialsWriteCache.Add(this.chestMaterial);
				skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_materialsWriteCache);
				return;
			}
			Debug.LogError(string.Format("Unexpected Submesh count {0}", subMeshCount2));
			return;
		}
	}

	// Token: 0x0600127F RID: 4735 RVA: 0x000629D4 File Offset: 0x00060BD4
	public static GorillaSkin GetActiveSkin(VRRig rig, out bool useDefaultBodySkin)
	{
		if (rig.CurrentModeSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.CurrentModeSkin;
		}
		if (rig.TemporaryEffectSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.TemporaryEffectSkin;
		}
		if (rig.CurrentCosmeticSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.CurrentCosmeticSkin;
		}
		useDefaultBodySkin = true;
		return rig.defaultSkin;
	}

	// Token: 0x06001280 RID: 4736 RVA: 0x00062A30 File Offset: 0x00060C30
	public static void ShowSkin(VRRig rig, GorillaSkin skin, bool useDefaultBodySkin = false)
	{
		if (skin.bodyMesh != null)
		{
			rig.bodyRenderer.SetCosmeticBodyMesh(skin.bodyMesh);
		}
		else
		{
			rig.bodyRenderer.ClearCosmeticBodyMesh();
		}
		if (useDefaultBodySkin)
		{
			rig.materialsToChangeTo[0] = rig.myDefaultSkinMaterialInstance;
		}
		else
		{
			rig.materialsToChangeTo[0] = skin.bodyMaterial;
		}
		rig.bodyRenderer.SetSkinMaterials(rig.materialsToChangeTo[rig.setMatIndex], skin.chestMaterial, skin.allowHeadless);
		rig.scoreboardMaterial = skin.scoreboardMaterial;
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x00062ABC File Offset: 0x00060CBC
	public static void ApplyToRig(VRRig rig, GorillaSkin skin, GorillaSkin.SkinType type)
	{
		bool flag;
		GorillaSkin activeSkin = GorillaSkin.GetActiveSkin(rig, out flag);
		switch (type)
		{
		case GorillaSkin.SkinType.cosmetic:
			rig.CurrentCosmeticSkin = skin;
			break;
		case GorillaSkin.SkinType.gameMode:
			rig.CurrentModeSkin = skin;
			break;
		case GorillaSkin.SkinType.temporaryEffect:
			rig.TemporaryEffectSkin = skin;
			break;
		default:
			Debug.LogError("Unknown skin slot");
			break;
		}
		bool useDefaultBodySkin;
		GorillaSkin activeSkin2 = GorillaSkin.GetActiveSkin(rig, out useDefaultBodySkin);
		if (activeSkin != activeSkin2)
		{
			GorillaSkin.ShowSkin(rig, activeSkin2, useDefaultBodySkin);
		}
	}

	// Token: 0x0400167D RID: 5757
	[FormerlySerializedAs("chestMaterial")]
	[FormerlySerializedAs("chestEarsMaterial")]
	[SerializeField]
	private Material _chestMaterial;

	// Token: 0x0400167E RID: 5758
	[FormerlySerializedAs("bodyMaterial")]
	[SerializeField]
	private Material _bodyMaterial;

	// Token: 0x0400167F RID: 5759
	[SerializeField]
	private Material _scoreboardMaterial;

	// Token: 0x04001680 RID: 5760
	[Tooltip("Check this if skin materials are incompatible with HeadlessMonkeRig mesh")]
	[SerializeField]
	private bool _disableHeadless;

	// Token: 0x04001681 RID: 5761
	[Space]
	[SerializeField]
	private Mesh _bodyMesh;

	// Token: 0x04001682 RID: 5762
	[Space]
	[NonSerialized]
	private Material _bodyRuntime;

	// Token: 0x04001683 RID: 5763
	[NonSerialized]
	private Material _chestRuntime;

	// Token: 0x04001684 RID: 5764
	[NonSerialized]
	private Material _scoreRuntime;

	// Token: 0x04001685 RID: 5765
	private static List<Material> _g_sharedMaterialsCache = new List<Material>(2);

	// Token: 0x04001686 RID: 5766
	private static List<Material> _g_materialsWriteCache = new List<Material>(3);

	// Token: 0x020002D1 RID: 721
	public enum SkinType
	{
		// Token: 0x04001688 RID: 5768
		cosmetic,
		// Token: 0x04001689 RID: 5769
		gameMode,
		// Token: 0x0400168A RID: 5770
		temporaryEffect
	}
}
