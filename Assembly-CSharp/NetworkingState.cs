using System;

// Token: 0x0200038F RID: 911
public enum NetworkingState
{
	// Token: 0x04002045 RID: 8261
	IsOwner,
	// Token: 0x04002046 RID: 8262
	IsBlindClient,
	// Token: 0x04002047 RID: 8263
	IsClient,
	// Token: 0x04002048 RID: 8264
	ForcefullyTakingOver,
	// Token: 0x04002049 RID: 8265
	RequestingOwnership,
	// Token: 0x0400204A RID: 8266
	RequestingOwnershipWaitingForSight,
	// Token: 0x0400204B RID: 8267
	ForcefullyTakingOverWaitingForSight
}
