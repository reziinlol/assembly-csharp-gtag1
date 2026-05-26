using System;
using System.Diagnostics;

namespace UniLabs.Time
{
	// Token: 0x02000E91 RID: 3729
	[Conditional("UNITY_EDITOR")]
	public class TimeSpanDrawerSettingsAttribute : Attribute
	{
		// Token: 0x06005B89 RID: 23433 RVA: 0x001D27E9 File Offset: 0x001D09E9
		public TimeSpanDrawerSettingsAttribute()
		{
		}

		// Token: 0x06005B8A RID: 23434 RVA: 0x001D27FF File Offset: 0x001D09FF
		public TimeSpanDrawerSettingsAttribute(TimeUnit highestUnit, TimeUnit lowestUnit)
		{
			this.HighestUnit = highestUnit;
			this.LowestUnit = lowestUnit;
		}

		// Token: 0x06005B8B RID: 23435 RVA: 0x001D2823 File Offset: 0x001D0A23
		public TimeSpanDrawerSettingsAttribute(TimeUnit highestUnit, bool drawMilliseconds = false)
		{
			this.HighestUnit = highestUnit;
			this.LowestUnit = (drawMilliseconds ? TimeUnit.Milliseconds : TimeUnit.Seconds);
		}

		// Token: 0x06005B8C RID: 23436 RVA: 0x001D284D File Offset: 0x001D0A4D
		public TimeSpanDrawerSettingsAttribute(bool drawMilliseconds)
		{
			this.HighestUnit = TimeUnit.Days;
			this.LowestUnit = (drawMilliseconds ? TimeUnit.Milliseconds : TimeUnit.Seconds);
		}

		// Token: 0x04006A6C RID: 27244
		public TimeUnit HighestUnit = TimeUnit.Days;

		// Token: 0x04006A6D RID: 27245
		public TimeUnit LowestUnit = TimeUnit.Seconds;
	}
}
