using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020007EA RID: 2026
public static class MaterialUtils
{
	// Token: 0x060033C3 RID: 13251 RVA: 0x0011CFB6 File Offset: 0x0011B1B6
	public static string GetTrimmedMaterialName(Material material)
	{
		return material.name.Replace(" (Instance)", "").Trim();
	}

	// Token: 0x060033C4 RID: 13252 RVA: 0x0011CFD4 File Offset: 0x0011B1D4
	public static void SwapMaterial(MeshAndMaterials meshAndMaterial, bool isOnToOff)
	{
		List<Material> list;
		using (ListPool<Material>.Get(out list))
		{
			meshAndMaterial.meshRenderer.GetSharedMaterials(list);
			for (int i = 0; i < list.Count; i++)
			{
				string trimmedMaterialName = MaterialUtils.GetTrimmedMaterialName(list[i]);
				string text = isOnToOff ? ((meshAndMaterial.onMaterial != null) ? MaterialUtils.GetTrimmedMaterialName(meshAndMaterial.onMaterial) : null) : ((meshAndMaterial.offMaterial != null) ? MaterialUtils.GetTrimmedMaterialName(meshAndMaterial.offMaterial) : null);
				if (text != null && trimmedMaterialName == text)
				{
					list[i] = (isOnToOff ? meshAndMaterial.offMaterial : meshAndMaterial.onMaterial);
				}
			}
			meshAndMaterial.meshRenderer.SetSharedMaterials(list);
		}
	}
}
