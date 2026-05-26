using System;
using UnityEngine;

// Token: 0x020008CB RID: 2251
public class GorillaUITransformFollow : MonoBehaviour
{
	// Token: 0x06003ADB RID: 15067 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Start()
	{
	}

	// Token: 0x06003ADC RID: 15068 RVA: 0x001433A0 File Offset: 0x001415A0
	private void LateUpdate()
	{
		if (this.doesMove)
		{
			base.transform.rotation = this.transformToFollow.rotation;
			base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
		}
	}

	// Token: 0x04004B4C RID: 19276
	public Transform transformToFollow;

	// Token: 0x04004B4D RID: 19277
	public Vector3 offset;

	// Token: 0x04004B4E RID: 19278
	public bool doesMove;
}
