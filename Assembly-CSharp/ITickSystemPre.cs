using System;

// Token: 0x02000CF9 RID: 3321
internal interface ITickSystemPre
{
	// Token: 0x170007C3 RID: 1987
	// (get) Token: 0x0600525F RID: 21087
	// (set) Token: 0x06005260 RID: 21088
	bool PreTickRunning { get; set; }

	// Token: 0x06005261 RID: 21089
	void PreTick();
}
