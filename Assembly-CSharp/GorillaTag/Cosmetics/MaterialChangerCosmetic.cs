using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001291 RID: 4753
	public class MaterialChangerCosmetic : MonoBehaviour
	{
		// Token: 0x060076FC RID: 30460 RVA: 0x002706A8 File Offset: 0x0026E8A8
		public void ChangeMaterial(Material newMaterial)
		{
			if (this.targetRenderer == null || newMaterial == null || this.materialIndex < 0)
			{
				return;
			}
			Material[] materials = this.targetRenderer.materials;
			if (this.materialIndex >= materials.Length)
			{
				Debug.LogWarning(string.Format("Material index {0} is out of range.", this.materialIndex));
				return;
			}
			materials[this.materialIndex] = newMaterial;
			this.targetRenderer.materials = materials;
		}

		// Token: 0x060076FD RID: 30461 RVA: 0x00270720 File Offset: 0x0026E920
		public void ChangeAllMaterials(Material newMat)
		{
			if (this.targetRenderer == null || newMat == null)
			{
				return;
			}
			Material[] array = new Material[this.targetRenderer.materials.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = newMat;
			}
			this.targetRenderer.materials = array;
		}

		// Token: 0x0400893C RID: 35132
		[SerializeField]
		private SkinnedMeshRenderer targetRenderer;

		// Token: 0x0400893D RID: 35133
		[SerializeField]
		private int materialIndex;
	}
}
