using System;

// Token: 0x02000053 RID: 83
public class CrittersAttachPointSettings : CrittersActorSettings
{
	// Token: 0x0600019F RID: 415 RVA: 0x0000A091 File Offset: 0x00008291
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersAttachPoint crittersAttachPoint = (CrittersAttachPoint)this.parentActor;
		crittersAttachPoint.anchorLocation = this.anchoredLocation;
		crittersAttachPoint.rb.isKinematic = true;
		crittersAttachPoint.isLeft = this.isLeft;
	}

	// Token: 0x040001C1 RID: 449
	public bool isLeft;

	// Token: 0x040001C2 RID: 450
	public CrittersAttachPoint.AnchoredLocationTypes anchoredLocation;
}
