using System;
using UnityEngine;

// Token: 0x0200030A RID: 778
public static class GlobalDeactivatedSpawnRoot
{
	// Token: 0x060013B8 RID: 5048 RVA: 0x0006B24C File Offset: 0x0006944C
	public static Transform GetOrCreate()
	{
		if (!GlobalDeactivatedSpawnRoot._xform)
		{
			GlobalDeactivatedSpawnRoot._xform = new GameObject("GlobalDeactivatedSpawnRoot").transform;
			GlobalDeactivatedSpawnRoot._xform.gameObject.SetActive(false);
			Object.DontDestroyOnLoad(GlobalDeactivatedSpawnRoot._xform.gameObject);
		}
		GlobalDeactivatedSpawnRoot._xform.gameObject.SetActive(false);
		return GlobalDeactivatedSpawnRoot._xform;
	}

	// Token: 0x04001853 RID: 6227
	private static Transform _xform;
}
