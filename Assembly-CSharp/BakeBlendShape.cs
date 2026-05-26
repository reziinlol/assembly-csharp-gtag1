using System;
using UnityEngine;

// Token: 0x020001DA RID: 474
public class BakeBlendShape : MonoBehaviour
{
	// Token: 0x06000C98 RID: 3224 RVA: 0x00044E08 File Offset: 0x00043008
	private void Update()
	{
		Mesh mesh = new Mesh();
		MeshCollider component = base.GetComponent<MeshCollider>();
		base.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
		component.sharedMesh = null;
		component.sharedMesh = mesh;
	}
}
