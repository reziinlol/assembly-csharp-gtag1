using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AE2 RID: 2786
[HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/rendering/material-instance")]
[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
[AddComponentMenu("Scripts/MRTK/Core/MaterialInstance")]
public class MaterialInstance : MonoBehaviour
{
	// Token: 0x06004709 RID: 18185 RVA: 0x0017F84D File Offset: 0x0017DA4D
	public Material AcquireMaterial(Object owner = null, bool instance = true)
	{
		if (owner != null)
		{
			this.materialOwners.Add(owner);
		}
		if (instance)
		{
			this.AcquireInstances();
		}
		Material[] array = this.instanceMaterials;
		if (array != null && array.Length != 0)
		{
			return this.instanceMaterials[0];
		}
		return null;
	}

	// Token: 0x0600470A RID: 18186 RVA: 0x0017F88B File Offset: 0x0017DA8B
	public Material[] AcquireMaterials(Object owner = null, bool instance = true)
	{
		if (owner != null)
		{
			this.materialOwners.Add(owner);
		}
		if (instance)
		{
			this.AcquireInstances();
		}
		base.gameObject.GetComponent<Material>();
		return this.instanceMaterials;
	}

	// Token: 0x0600470B RID: 18187 RVA: 0x0017F8BE File Offset: 0x0017DABE
	public void ReleaseMaterial(Object owner, bool autoDestroy = true)
	{
		this.materialOwners.Remove(owner);
		if (autoDestroy && this.materialOwners.Count == 0)
		{
			MaterialInstance.DestroySafe(this);
			if (!base.gameObject.activeInHierarchy)
			{
				this.RestoreRenderer();
			}
		}
	}

	// Token: 0x17000697 RID: 1687
	// (get) Token: 0x0600470C RID: 18188 RVA: 0x0017F8F6 File Offset: 0x0017DAF6
	public Material Material
	{
		get
		{
			return this.AcquireMaterial(null, true);
		}
	}

	// Token: 0x17000698 RID: 1688
	// (get) Token: 0x0600470D RID: 18189 RVA: 0x0017F900 File Offset: 0x0017DB00
	public Material[] Materials
	{
		get
		{
			return this.AcquireMaterials(null, true);
		}
	}

	// Token: 0x17000699 RID: 1689
	// (get) Token: 0x0600470E RID: 18190 RVA: 0x0017F90A File Offset: 0x0017DB0A
	// (set) Token: 0x0600470F RID: 18191 RVA: 0x0017F912 File Offset: 0x0017DB12
	public bool CacheSharedMaterialsFromRenderer
	{
		get
		{
			return this.cacheSharedMaterialsFromRenderer;
		}
		set
		{
			if (this.cacheSharedMaterialsFromRenderer != value)
			{
				if (value)
				{
					this.cachedSharedMaterials = this.CachedRenderer.sharedMaterials;
				}
				else
				{
					this.cachedSharedMaterials = null;
				}
				this.cacheSharedMaterialsFromRenderer = value;
			}
		}
	}

	// Token: 0x1700069A RID: 1690
	// (get) Token: 0x06004710 RID: 18192 RVA: 0x0017F941 File Offset: 0x0017DB41
	private Renderer CachedRenderer
	{
		get
		{
			if (this.cachedRenderer == null)
			{
				this.cachedRenderer = base.GetComponent<Renderer>();
				if (this.CacheSharedMaterialsFromRenderer)
				{
					this.cachedSharedMaterials = this.cachedRenderer.sharedMaterials;
				}
			}
			return this.cachedRenderer;
		}
	}

	// Token: 0x1700069B RID: 1691
	// (get) Token: 0x06004711 RID: 18193 RVA: 0x0017F97C File Offset: 0x0017DB7C
	// (set) Token: 0x06004712 RID: 18194 RVA: 0x0017F9B1 File Offset: 0x0017DBB1
	private Material[] CachedRendererSharedMaterials
	{
		get
		{
			if (this.CacheSharedMaterialsFromRenderer)
			{
				if (this.cachedSharedMaterials == null)
				{
					this.cachedSharedMaterials = this.cachedRenderer.sharedMaterials;
				}
				return this.cachedSharedMaterials;
			}
			return this.cachedRenderer.sharedMaterials;
		}
		set
		{
			if (this.CacheSharedMaterialsFromRenderer)
			{
				this.cachedSharedMaterials = value;
			}
			this.cachedRenderer.sharedMaterials = value;
		}
	}

	// Token: 0x06004713 RID: 18195 RVA: 0x0017F9CE File Offset: 0x0017DBCE
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06004714 RID: 18196 RVA: 0x0017F9D6 File Offset: 0x0017DBD6
	private void OnDestroy()
	{
		this.RestoreRenderer();
	}

	// Token: 0x06004715 RID: 18197 RVA: 0x0017F9DE File Offset: 0x0017DBDE
	private void RestoreRenderer()
	{
		if (this.CachedRenderer != null && this.defaultMaterials != null)
		{
			this.CachedRendererSharedMaterials = this.defaultMaterials;
		}
		MaterialInstance.DestroyMaterials(this.instanceMaterials);
		this.instanceMaterials = null;
	}

	// Token: 0x06004716 RID: 18198 RVA: 0x0017FA14 File Offset: 0x0017DC14
	private void Initialize()
	{
		if (!this.initialized && this.CachedRenderer != null)
		{
			if (!MaterialInstance.HasValidMaterial(this.defaultMaterials))
			{
				this.defaultMaterials = this.CachedRendererSharedMaterials;
			}
			else if (!this.materialsInstanced)
			{
				this.CachedRendererSharedMaterials = this.defaultMaterials;
			}
			this.initialized = true;
		}
	}

	// Token: 0x06004717 RID: 18199 RVA: 0x0017FA6D File Offset: 0x0017DC6D
	private void AcquireInstances()
	{
		if (this.CachedRenderer != null && !MaterialInstance.MaterialsMatch(this.CachedRendererSharedMaterials, this.instanceMaterials))
		{
			this.CreateInstances();
		}
	}

	// Token: 0x06004718 RID: 18200 RVA: 0x0017FA98 File Offset: 0x0017DC98
	private void CreateInstances()
	{
		this.Initialize();
		MaterialInstance.DestroyMaterials(this.instanceMaterials);
		this.instanceMaterials = MaterialInstance.InstanceMaterials(this.defaultMaterials);
		if (this.CachedRenderer != null && this.instanceMaterials != null)
		{
			this.CachedRendererSharedMaterials = this.instanceMaterials;
		}
		this.materialsInstanced = true;
	}

	// Token: 0x06004719 RID: 18201 RVA: 0x0017FAF0 File Offset: 0x0017DCF0
	private static bool MaterialsMatch(Material[] a, Material[] b)
	{
		int? num = (a != null) ? new int?(a.Length) : null;
		int? num2 = (b != null) ? new int?(b.Length) : null;
		if (!(num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null)))
		{
			return false;
		}
		int num3 = 0;
		for (;;)
		{
			int num4 = num3;
			num2 = ((a != null) ? new int?(a.Length) : null);
			if (!(num4 < num2.GetValueOrDefault() & num2 != null))
			{
				return true;
			}
			if (a[num3] != b[num3])
			{
				break;
			}
			num3++;
		}
		return false;
	}

	// Token: 0x0600471A RID: 18202 RVA: 0x0017FB94 File Offset: 0x0017DD94
	private static Material[] InstanceMaterials(Material[] source)
	{
		if (source == null)
		{
			return null;
		}
		Material[] array = new Material[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			if (source[i] != null)
			{
				if (MaterialInstance.IsInstanceMaterial(source[i]))
				{
					Debug.LogWarning("A material (" + source[i].name + ") which is already instanced was instanced multiple times.");
				}
				array[i] = new Material(source[i]);
				Material material = array[i];
				material.name += " (Instance)";
			}
		}
		return array;
	}

	// Token: 0x0600471B RID: 18203 RVA: 0x0017FC14 File Offset: 0x0017DE14
	private static void DestroyMaterials(Material[] materials)
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				MaterialInstance.DestroySafe(materials[i]);
			}
		}
	}

	// Token: 0x0600471C RID: 18204 RVA: 0x0017FC3A File Offset: 0x0017DE3A
	private static bool IsInstanceMaterial(Material material)
	{
		return material != null && material.name.Contains(" (Instance)");
	}

	// Token: 0x0600471D RID: 18205 RVA: 0x0017FC58 File Offset: 0x0017DE58
	private static bool HasValidMaterial(Material[] materials)
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				if (materials[i] != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600471E RID: 18206 RVA: 0x0017FC86 File Offset: 0x0017DE86
	private static void DestroySafe(Object toDestroy)
	{
		if (toDestroy != null && Application.isPlaying)
		{
			Object.Destroy(toDestroy);
		}
	}

	// Token: 0x04005998 RID: 22936
	private Renderer cachedRenderer;

	// Token: 0x04005999 RID: 22937
	[SerializeField]
	[HideInInspector]
	private Material[] defaultMaterials;

	// Token: 0x0400599A RID: 22938
	private Material[] instanceMaterials;

	// Token: 0x0400599B RID: 22939
	private Material[] cachedSharedMaterials;

	// Token: 0x0400599C RID: 22940
	private bool initialized;

	// Token: 0x0400599D RID: 22941
	private bool materialsInstanced;

	// Token: 0x0400599E RID: 22942
	[SerializeField]
	[Tooltip("Whether to use a cached copy of cachedRenderer.sharedMaterials or call sharedMaterials on the Renderer directly. Enabling the option will lead to better performance but you must turn it off before modifying sharedMaterials of the Renderer.")]
	private bool cacheSharedMaterialsFromRenderer;

	// Token: 0x0400599F RID: 22943
	private readonly HashSet<Object> materialOwners = new HashSet<Object>();

	// Token: 0x040059A0 RID: 22944
	private const string instancePostfix = " (Instance)";
}
