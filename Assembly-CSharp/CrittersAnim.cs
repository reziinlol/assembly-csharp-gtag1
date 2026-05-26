using System;
using UnityEngine;

// Token: 0x02000050 RID: 80
[Serializable]
public class CrittersAnim
{
	// Token: 0x0600019A RID: 410 RVA: 0x0000A00C File Offset: 0x0000820C
	public bool IsModified()
	{
		return (this.squashAmount != null && this.squashAmount.length > 1) || (this.forwardOffset != null && this.forwardOffset.length > 1) || (this.horizontalOffset != null && this.horizontalOffset.length > 1) || (this.verticalOffset != null && this.verticalOffset.length > 1);
	}

	// Token: 0x0600019B RID: 411 RVA: 0x0000A075 File Offset: 0x00008275
	public static bool IsModified(CrittersAnim anim)
	{
		return anim != null && anim.IsModified();
	}

	// Token: 0x040001B5 RID: 437
	public AnimationCurve squashAmount;

	// Token: 0x040001B6 RID: 438
	public AnimationCurve forwardOffset;

	// Token: 0x040001B7 RID: 439
	public AnimationCurve horizontalOffset;

	// Token: 0x040001B8 RID: 440
	public AnimationCurve verticalOffset;

	// Token: 0x040001B9 RID: 441
	public float playSpeed;
}
