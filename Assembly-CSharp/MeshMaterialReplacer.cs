using System;
using GameObjectScheduling;
using UnityEngine;

// Token: 0x0200041C RID: 1052
public class MeshMaterialReplacer : MonoBehaviour
{
	// Token: 0x060018F9 RID: 6393 RVA: 0x0008D498 File Offset: 0x0008B698
	private void Start()
	{
		MeshRenderer meshRenderer;
		if (base.TryGetComponent<MeshRenderer>(out meshRenderer))
		{
			base.GetComponent<MeshFilter>().mesh = this.meshMaterialReplacement.mesh;
			meshRenderer.materials = this.meshMaterialReplacement.materials;
			return;
		}
		SkinnedMeshRenderer skinnedMeshRenderer;
		if (base.TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
		{
			skinnedMeshRenderer.sharedMesh = this.meshMaterialReplacement.mesh;
			skinnedMeshRenderer.materials = this.meshMaterialReplacement.materials;
		}
	}

	// Token: 0x04002418 RID: 9240
	[SerializeField]
	private MeshMaterialReplacement meshMaterialReplacement;
}
