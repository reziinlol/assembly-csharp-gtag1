using System;
using UnityEngine;

// Token: 0x02000DF8 RID: 3576
public class UberCombinerPerMaterialMeshes : MonoBehaviour
{
	// Token: 0x0400678E RID: 26510
	public GameObject rootObject;

	// Token: 0x0400678F RID: 26511
	public bool deleteSelfOnPrefabBake;

	// Token: 0x04006790 RID: 26512
	[Space]
	public GameObject[] objects = new GameObject[0];

	// Token: 0x04006791 RID: 26513
	public MeshRenderer[] renderers = new MeshRenderer[0];

	// Token: 0x04006792 RID: 26514
	public MeshFilter[] filters = new MeshFilter[0];

	// Token: 0x04006793 RID: 26515
	public Material[] materials = new Material[0];
}
