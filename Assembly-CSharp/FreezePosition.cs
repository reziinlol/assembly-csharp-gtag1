using System;
using UnityEngine;

// Token: 0x020000A1 RID: 161
public class FreezePosition : MonoBehaviour
{
	// Token: 0x060003FC RID: 1020 RVA: 0x00017A93 File Offset: 0x00015C93
	private void FixedUpdate()
	{
		if (this.target)
		{
			this.target.localPosition = this.localPosition;
		}
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x00017A93 File Offset: 0x00015C93
	private void LateUpdate()
	{
		if (this.target)
		{
			this.target.localPosition = this.localPosition;
		}
	}

	// Token: 0x04000469 RID: 1129
	public Transform target;

	// Token: 0x0400046A RID: 1130
	public Vector3 localPosition;
}
