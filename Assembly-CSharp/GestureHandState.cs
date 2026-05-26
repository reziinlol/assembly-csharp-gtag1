using System;

// Token: 0x020002C9 RID: 713
[Flags]
public enum GestureHandState : uint
{
	// Token: 0x0400163C RID: 5692
	None = 0U,
	// Token: 0x0400163D RID: 5693
	IsLeft = 1U,
	// Token: 0x0400163E RID: 5694
	IsRight = 2U,
	// Token: 0x0400163F RID: 5695
	Open = 4U,
	// Token: 0x04001640 RID: 5696
	Closed = 8U
}
