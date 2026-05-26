using System;
using UnityEngine;

// Token: 0x020005D7 RID: 1495
public class GorillaWalkingGrab : MonoBehaviour
{
	// Token: 0x06002549 RID: 9545 RVA: 0x000C5FF5 File Offset: 0x000C41F5
	private void Start()
	{
		this.thisRigidbody = base.gameObject.GetComponent<Rigidbody>();
		this.positionHistory = new Vector3[this.historySteps];
		this.historyIndex = 0;
	}

	// Token: 0x0600254A RID: 9546 RVA: 0x000C6020 File Offset: 0x000C4220
	private void FixedUpdate()
	{
		this.historyIndex++;
		if (this.historyIndex >= this.historySteps)
		{
			this.historyIndex = 0;
		}
		this.positionHistory[this.historyIndex] = this.handToStickTo.transform.position;
		this.thisRigidbody.MovePosition(this.handToStickTo.transform.position);
		base.transform.rotation = this.handToStickTo.transform.rotation;
	}

	// Token: 0x0600254B RID: 9547 RVA: 0x00002076 File Offset: 0x00000276
	private bool MakeJump()
	{
		return false;
	}

	// Token: 0x0600254C RID: 9548 RVA: 0x000C60A8 File Offset: 0x000C42A8
	private void OnCollisionStay(Collision collision)
	{
		if (!this.MakeJump())
		{
			Vector3 b = Vector3.ProjectOnPlane(this.positionHistory[(this.historyIndex != 0) ? (this.historyIndex - 1) : (this.historySteps - 1)] - this.handToStickTo.transform.position, collision.GetContact(0).normal);
			Vector3 b2 = this.thisRigidbody.transform.position - this.handToStickTo.transform.position;
			this.playspaceRigidbody.MovePosition(this.playspaceRigidbody.transform.position + b - b2);
		}
	}

	// Token: 0x040030C9 RID: 12489
	public GameObject handToStickTo;

	// Token: 0x040030CA RID: 12490
	public float ratioToUse;

	// Token: 0x040030CB RID: 12491
	public float forceMultiplier;

	// Token: 0x040030CC RID: 12492
	public int historySteps;

	// Token: 0x040030CD RID: 12493
	public Rigidbody playspaceRigidbody;

	// Token: 0x040030CE RID: 12494
	private Rigidbody thisRigidbody;

	// Token: 0x040030CF RID: 12495
	private Vector3 lastPosition;

	// Token: 0x040030D0 RID: 12496
	private Vector3 maybeLastPositionIDK;

	// Token: 0x040030D1 RID: 12497
	private Vector3[] positionHistory;

	// Token: 0x040030D2 RID: 12498
	private int historyIndex;
}
