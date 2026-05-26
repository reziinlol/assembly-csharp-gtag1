using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000661 RID: 1633
[Serializable]
internal class HandTransformFollowOffset
{
	// Token: 0x060028A9 RID: 10409 RVA: 0x000DCC68 File Offset: 0x000DAE68
	internal void UpdatePositionRotation()
	{
		if (this.followTransform == null || this.targetTransforms == null)
		{
			return;
		}
		this.position = this.followTransform.position + this.followTransform.rotation * this.positionOffset * GTPlayer.Instance.scale;
		this.rotation = this.followTransform.rotation * this.rotationOffset;
		foreach (Transform transform in this.targetTransforms)
		{
			transform.position = this.position;
			transform.rotation = this.rotation;
		}
	}

	// Token: 0x04003518 RID: 13592
	internal Transform followTransform;

	// Token: 0x04003519 RID: 13593
	[SerializeField]
	private Transform[] targetTransforms;

	// Token: 0x0400351A RID: 13594
	[SerializeField]
	internal Vector3 positionOffset;

	// Token: 0x0400351B RID: 13595
	[SerializeField]
	internal Quaternion rotationOffset;

	// Token: 0x0400351C RID: 13596
	private Vector3 position;

	// Token: 0x0400351D RID: 13597
	private Quaternion rotation;
}
