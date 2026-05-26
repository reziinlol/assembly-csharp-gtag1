using System;
using UnityEngine;

// Token: 0x020005C4 RID: 1476
public class GorillaBodyPhysics : MonoBehaviour
{
	// Token: 0x06002517 RID: 9495 RVA: 0x000C5A21 File Offset: 0x000C3C21
	private void FixedUpdate()
	{
		this.bodyCollider.transform.position = this.headsetTransform.position + this.bodyColliderOffset;
	}

	// Token: 0x04003068 RID: 12392
	public GameObject bodyCollider;

	// Token: 0x04003069 RID: 12393
	public Vector3 bodyColliderOffset;

	// Token: 0x0400306A RID: 12394
	public Transform headsetTransform;
}
