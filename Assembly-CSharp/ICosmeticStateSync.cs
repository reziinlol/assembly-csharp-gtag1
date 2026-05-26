using System;

// Token: 0x020004F6 RID: 1270
public interface ICosmeticStateSync
{
	// Token: 0x1700036C RID: 876
	// (get) Token: 0x06001FCF RID: 8143
	int StateValue { get; }

	// Token: 0x06001FD0 RID: 8144
	void OnStateUpdate(int state);
}
