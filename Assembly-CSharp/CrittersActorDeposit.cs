using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000049 RID: 73
public class CrittersActorDeposit : MonoBehaviour
{
	// Token: 0x06000161 RID: 353 RVA: 0x00008BC4 File Offset: 0x00006DC4
	public void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody.IsNotNull())
		{
			CrittersActor component = other.attachedRigidbody.GetComponent<CrittersActor>();
			if (CrittersManager.instance.LocalAuthority() && component.IsNotNull() && this.CanDeposit(component) && this.IsAttachAvailable())
			{
				this.HandleDeposit(component);
			}
		}
	}

	// Token: 0x06000162 RID: 354 RVA: 0x00008C18 File Offset: 0x00006E18
	protected virtual bool CanDeposit(CrittersActor depositActor)
	{
		if (depositActor.crittersActorType != this.actorType)
		{
			return false;
		}
		CrittersActor crittersActor;
		if (CrittersManager.instance.actorById.TryGetValue(depositActor.parentActorId, out crittersActor))
		{
			return crittersActor.crittersActorType == CrittersActor.CrittersActorType.Grabber;
		}
		return depositActor.parentActorId == -1;
	}

	// Token: 0x06000163 RID: 355 RVA: 0x00008C64 File Offset: 0x00006E64
	private bool IsAttachAvailable()
	{
		return this.allowMultiAttach || this.currentAttach == null;
	}

	// Token: 0x06000164 RID: 356 RVA: 0x00008C7C File Offset: 0x00006E7C
	protected virtual void HandleDeposit(CrittersActor depositedActor)
	{
		this.currentAttach = depositedActor;
		depositedActor.ReleasedEvent.AddListener(new UnityAction<CrittersActor>(this.HandleDetach));
		CrittersActor grabbingActor = this.attachPoint;
		bool positionOverride = this.snapOnAttach;
		bool disableGrabbing = this.disableGrabOnAttach;
		depositedActor.GrabbedBy(grabbingActor, positionOverride, default(Quaternion), default(Vector3), disableGrabbing);
	}

	// Token: 0x06000165 RID: 357 RVA: 0x00008CD4 File Offset: 0x00006ED4
	protected virtual void HandleDetach(CrittersActor detachingActor)
	{
		this.currentAttach = null;
	}

	// Token: 0x0400017A RID: 378
	public CrittersActor attachPoint;

	// Token: 0x0400017B RID: 379
	public CrittersActor.CrittersActorType actorType;

	// Token: 0x0400017C RID: 380
	public bool disableGrabOnAttach;

	// Token: 0x0400017D RID: 381
	public bool allowMultiAttach;

	// Token: 0x0400017E RID: 382
	public bool snapOnAttach;

	// Token: 0x0400017F RID: 383
	private CrittersActor currentAttach;
}
