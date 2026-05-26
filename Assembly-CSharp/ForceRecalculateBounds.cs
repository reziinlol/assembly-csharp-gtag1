using System;
using UnityEngine;

// Token: 0x02000494 RID: 1172
public class ForceRecalculateBounds : MonoBehaviourTick
{
	// Token: 0x06001C69 RID: 7273 RVA: 0x00099E49 File Offset: 0x00098049
	private void Awake()
	{
		this.skinnedMesh = base.GetComponentInChildren<SkinnedMeshRenderer>();
		this.bounds = Vector3.one * 1000f;
		this.mainCamera = Camera.main.transform;
	}

	// Token: 0x06001C6A RID: 7274 RVA: 0x00099E7C File Offset: 0x0009807C
	public override void Tick()
	{
		if (this.skinnedMesh == null)
		{
			return;
		}
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main.transform;
			return;
		}
		this.skinnedMesh.bounds = new Bounds(this.mainCamera.position, this.bounds);
	}

	// Token: 0x0400267B RID: 9851
	private SkinnedMeshRenderer skinnedMesh;

	// Token: 0x0400267C RID: 9852
	private Transform mainCamera;

	// Token: 0x0400267D RID: 9853
	private Vector3 bounds;
}
