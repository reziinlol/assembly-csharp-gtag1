using System;

// Token: 0x02000CFA RID: 3322
internal interface ITickSystemTick
{
	// Token: 0x170007C4 RID: 1988
	// (get) Token: 0x06005262 RID: 21090
	// (set) Token: 0x06005263 RID: 21091
	bool TickRunning { get; set; }

	// Token: 0x06005264 RID: 21092
	void Tick();
}
