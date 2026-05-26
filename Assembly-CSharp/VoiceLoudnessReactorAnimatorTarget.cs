using System;
using UnityEngine;

// Token: 0x02000E0F RID: 3599
[Serializable]
public class VoiceLoudnessReactorAnimatorTarget
{
	// Token: 0x04006876 RID: 26742
	public Animator animator;

	// Token: 0x04006877 RID: 26743
	public bool useSmoothedLoudness;

	// Token: 0x04006878 RID: 26744
	public float animatorSpeedToLoudness = 1f;
}
