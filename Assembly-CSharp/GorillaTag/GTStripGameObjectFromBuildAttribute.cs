using System;

namespace GorillaTag
{
	// Token: 0x02001133 RID: 4403
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class GTStripGameObjectFromBuildAttribute : Attribute
	{
		// Token: 0x17000AAA RID: 2730
		// (get) Token: 0x06006FD1 RID: 28625 RVA: 0x0024827F File Offset: 0x0024647F
		public string Condition { get; }

		// Token: 0x06006FD2 RID: 28626 RVA: 0x00248287 File Offset: 0x00246487
		public GTStripGameObjectFromBuildAttribute(string condition = "")
		{
			this.Condition = condition;
		}
	}
}
