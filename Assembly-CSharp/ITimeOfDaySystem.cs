using System;

// Token: 0x020009F9 RID: 2553
public interface ITimeOfDaySystem
{
	// Token: 0x17000617 RID: 1559
	// (get) Token: 0x06004158 RID: 16728
	double currentTimeInSeconds { get; }

	// Token: 0x17000618 RID: 1560
	// (get) Token: 0x06004159 RID: 16729
	double totalTimeInSeconds { get; }
}
