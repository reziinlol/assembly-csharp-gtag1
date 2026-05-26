using System;

// Token: 0x020002C8 RID: 712
[Flags]
public enum GestureNodeFlags : uint
{
	// Token: 0x0400162F RID: 5679
	None = 0U,
	// Token: 0x04001630 RID: 5680
	HandLeft = 1U,
	// Token: 0x04001631 RID: 5681
	HandRight = 2U,
	// Token: 0x04001632 RID: 5682
	HandOpen = 4U,
	// Token: 0x04001633 RID: 5683
	HandClosed = 8U,
	// Token: 0x04001634 RID: 5684
	DigitOpen = 16U,
	// Token: 0x04001635 RID: 5685
	DigitClosed = 32U,
	// Token: 0x04001636 RID: 5686
	DigitBent = 64U,
	// Token: 0x04001637 RID: 5687
	TowardFace = 128U,
	// Token: 0x04001638 RID: 5688
	AwayFromFace = 256U,
	// Token: 0x04001639 RID: 5689
	AxisWorldUp = 512U,
	// Token: 0x0400163A RID: 5690
	AxisWorldDown = 1024U
}
