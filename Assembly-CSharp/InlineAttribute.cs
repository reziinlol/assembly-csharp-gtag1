using System;
using System.Diagnostics;

// Token: 0x020002A5 RID: 677
[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.All)]
public class InlineAttribute : Attribute
{
	// Token: 0x060011B2 RID: 4530 RVA: 0x0005ED94 File Offset: 0x0005CF94
	public InlineAttribute(bool keepLabel = false, bool asGroup = false)
	{
		this.keepLabel = keepLabel;
		this.asGroup = asGroup;
	}

	// Token: 0x0400153A RID: 5434
	public readonly bool keepLabel;

	// Token: 0x0400153B RID: 5435
	public readonly bool asGroup;
}
