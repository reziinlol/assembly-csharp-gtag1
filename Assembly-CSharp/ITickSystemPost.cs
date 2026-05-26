using System;

// Token: 0x02000CFB RID: 3323
internal interface ITickSystemPost
{
	// Token: 0x170007C5 RID: 1989
	// (get) Token: 0x06005265 RID: 21093
	// (set) Token: 0x06005266 RID: 21094
	bool PostTickRunning { get; set; }

	// Token: 0x06005267 RID: 21095
	void PostTick();
}
