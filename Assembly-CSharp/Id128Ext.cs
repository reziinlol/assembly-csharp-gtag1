using System;
using UnityEngine;

// Token: 0x02000AD6 RID: 2774
public static class Id128Ext
{
	// Token: 0x060046D6 RID: 18134 RVA: 0x0017EE78 File Offset: 0x0017D078
	public static Id128 ToId128(this Hash128 h)
	{
		return new Id128(h);
	}

	// Token: 0x060046D7 RID: 18135 RVA: 0x0017EE70 File Offset: 0x0017D070
	public static Id128 ToId128(this Guid g)
	{
		return new Id128(g);
	}
}
