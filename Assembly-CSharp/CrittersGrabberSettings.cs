using System;
using UnityEngine;

// Token: 0x02000062 RID: 98
public class CrittersGrabberSettings : CrittersActorSettings
{
	// Token: 0x060001E8 RID: 488 RVA: 0x0000B412 File Offset: 0x00009612
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersGrabber crittersGrabber = (CrittersGrabber)this.parentActor;
		crittersGrabber.grabPosition = this._grabPosition;
		crittersGrabber.grabDistance = this._grabDistance;
	}

	// Token: 0x0400022C RID: 556
	public Transform _grabPosition;

	// Token: 0x0400022D RID: 557
	public float _grabDistance;
}
