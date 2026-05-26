using System;
using UnityEngine;

// Token: 0x020005F8 RID: 1528
[Serializable]
public class MonkeBallTeam
{
	// Token: 0x0400316C RID: 12652
	public Color color;

	// Token: 0x0400316D RID: 12653
	public int score;

	// Token: 0x0400316E RID: 12654
	public Transform ballStartLocation;

	// Token: 0x0400316F RID: 12655
	public Transform ballLaunchPosition;

	// Token: 0x04003170 RID: 12656
	[Tooltip("The min/max random velocity of the ball when launched.")]
	public Vector2 ballLaunchVelocityRange = new Vector2(8f, 15f);

	// Token: 0x04003171 RID: 12657
	[Tooltip("The min/max random x-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleXRange = new Vector2(0f, 0f);

	// Token: 0x04003172 RID: 12658
	[Tooltip("The min/max random y-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleYRange = new Vector2(0f, 0f);
}
