using System;
using UnityEngine;

// Token: 0x02000D1F RID: 3359
public static class AnimatorUtils
{
	// Token: 0x060052EE RID: 21230 RVA: 0x001B2D10 File Offset: 0x001B0F10
	public static void ResetToEntryState(this Animator a)
	{
		if (a == null)
		{
			return;
		}
		a.Rebind();
		a.Update(0f);
	}
}
