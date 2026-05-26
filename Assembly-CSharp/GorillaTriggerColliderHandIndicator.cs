using System;
using UnityEngine;

// Token: 0x02000A1E RID: 2590
public class GorillaTriggerColliderHandIndicator : MonoBehaviourTick
{
	// Token: 0x06004242 RID: 16962 RVA: 0x00162100 File Offset: 0x00160300
	public override void Tick()
	{
		this.currentVelocity = (base.transform.position - this.lastPosition) / Time.deltaTime;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x06004243 RID: 16963 RVA: 0x00162139 File Offset: 0x00160339
	private void OnTriggerEnter(Collider other)
	{
		if (this.throwableController != null)
		{
			this.throwableController.GrabbableObjectHover(this.isLeftHand);
		}
	}

	// Token: 0x0400541D RID: 21533
	public Vector3 currentVelocity;

	// Token: 0x0400541E RID: 21534
	public Vector3 lastPosition = Vector3.zero;

	// Token: 0x0400541F RID: 21535
	public bool isLeftHand;

	// Token: 0x04005420 RID: 21536
	public GorillaThrowableController throwableController;
}
