using System;
using UnityEngine;

// Token: 0x02000D60 RID: 3424
public static class PoolUtils
{
	// Token: 0x0600542E RID: 21550 RVA: 0x001B8220 File Offset: 0x001B6420
	public static int GameObjHashCode(GameObject obj)
	{
		return obj.tag.GetHashCode();
	}
}
