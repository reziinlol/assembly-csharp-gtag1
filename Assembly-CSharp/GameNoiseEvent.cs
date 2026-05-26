using System;
using UnityEngine;

// Token: 0x020007B2 RID: 1970
public struct GameNoiseEvent
{
	// Token: 0x0600322B RID: 12843 RVA: 0x00113876 File Offset: 0x00111A76
	public bool IsValid()
	{
		return (float)(Time.timeAsDouble - this.eventTime) <= this.duration;
	}

	// Token: 0x04004129 RID: 16681
	public Vector3 position;

	// Token: 0x0400412A RID: 16682
	public double eventTime;

	// Token: 0x0400412B RID: 16683
	public float duration;

	// Token: 0x0400412C RID: 16684
	public float magnitude;
}
