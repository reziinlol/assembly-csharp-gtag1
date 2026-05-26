using System;

// Token: 0x02000051 RID: 81
public class CrittersAttachPoint : CrittersActor
{
	// Token: 0x0600019D RID: 413 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void ProcessRemote()
	{
	}

	// Token: 0x040001BA RID: 442
	public bool fixedOrientation = true;

	// Token: 0x040001BB RID: 443
	public CrittersAttachPoint.AnchoredLocationTypes anchorLocation;

	// Token: 0x040001BC RID: 444
	public bool isLeft;

	// Token: 0x02000052 RID: 82
	public enum AnchoredLocationTypes
	{
		// Token: 0x040001BE RID: 446
		Arm,
		// Token: 0x040001BF RID: 447
		Chest,
		// Token: 0x040001C0 RID: 448
		Back
	}
}
