using System;
using System.Diagnostics;

namespace UniLabs.Time
{
	// Token: 0x02000E92 RID: 3730
	[AttributeUsage(AttributeTargets.All)]
	[Conditional("UNITY_EDITOR")]
	public class TimeSpanRangeAttribute : Attribute
	{
		// Token: 0x06005B8D RID: 23437 RVA: 0x001D2877 File Offset: 0x001D0A77
		public TimeSpanRangeAttribute(string maxGetter, bool inline = false, TimeUnit snappingUnit = TimeUnit.Seconds)
		{
			this.MaxGetter = maxGetter;
			this.SnappingUnit = snappingUnit;
			this.Inline = inline;
		}

		// Token: 0x06005B8E RID: 23438 RVA: 0x001D2894 File Offset: 0x001D0A94
		public TimeSpanRangeAttribute(string minGetter, string maxGetter, bool inline = false, TimeUnit snappingUnit = TimeUnit.Seconds)
		{
			this.MinGetter = minGetter;
			this.MaxGetter = maxGetter;
			this.SnappingUnit = snappingUnit;
			this.Inline = inline;
		}

		// Token: 0x04006A6E RID: 27246
		public string MinGetter;

		// Token: 0x04006A6F RID: 27247
		public string MaxGetter;

		// Token: 0x04006A70 RID: 27248
		public TimeUnit SnappingUnit;

		// Token: 0x04006A71 RID: 27249
		public bool Inline;

		// Token: 0x04006A72 RID: 27250
		public string DisableMinMaxIf;
	}
}
