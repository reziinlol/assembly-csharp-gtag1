using System;
using UnityEngine;

// Token: 0x020005C9 RID: 1481
public class GorillaHandHistory : MonoBehaviour
{
	// Token: 0x06002525 RID: 9509 RVA: 0x000C5C7C File Offset: 0x000C3E7C
	private void Start()
	{
		this.direction = default(Vector3);
		this.lastPosition = default(Vector3);
	}

	// Token: 0x06002526 RID: 9510 RVA: 0x000C5C96 File Offset: 0x000C3E96
	private void FixedUpdate()
	{
		this.direction = this.lastPosition - base.transform.position;
		this.lastLastPosition = this.lastPosition;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x0400307C RID: 12412
	public Vector3 direction;

	// Token: 0x0400307D RID: 12413
	private Vector3 lastPosition;

	// Token: 0x0400307E RID: 12414
	private Vector3 lastLastPosition;
}
