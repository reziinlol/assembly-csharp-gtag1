using System;
using UnityEngine;

// Token: 0x02000E0A RID: 3594
[Serializable]
public class VoiceLoudnessReactorBlendShapeTarget
{
	// Token: 0x04006857 RID: 26711
	public SkinnedMeshRenderer SkinnedMeshRenderer;

	// Token: 0x04006858 RID: 26712
	public int BlendShapeIndex;

	// Token: 0x04006859 RID: 26713
	[Tooltip("Blend shape weight at minimum loudness ")]
	public float minValue;

	// Token: 0x0400685A RID: 26714
	[Tooltip("Blend shape weight at maximum loudness (use 100 for full weighting)\nA number higher than 100 can be used to have full weighting at lower voice loudness")]
	public float maxValue = 1f;

	// Token: 0x0400685B RID: 26715
	public bool UseSmoothedLoudness;
}
