using System;
using System.Diagnostics;

// Token: 0x020002A3 RID: 675
[Conditional("UNITY_EDITOR")]
public class DarkBoxAttribute : Attribute
{
	// Token: 0x060011AF RID: 4527 RVA: 0x00002376 File Offset: 0x00000576
	public DarkBoxAttribute()
	{
	}

	// Token: 0x060011B0 RID: 4528 RVA: 0x0005ED85 File Offset: 0x0005CF85
	public DarkBoxAttribute(bool withBorders)
	{
		this.withBorders = withBorders;
	}

	// Token: 0x04001539 RID: 5433
	public readonly bool withBorders;
}
