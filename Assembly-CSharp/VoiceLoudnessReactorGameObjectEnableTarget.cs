using System;
using UnityEngine;

// Token: 0x02000E0E RID: 3598
[Serializable]
public class VoiceLoudnessReactorGameObjectEnableTarget
{
	// Token: 0x04006871 RID: 26737
	public GameObject GameObject;

	// Token: 0x04006872 RID: 26738
	public float Threshold;

	// Token: 0x04006873 RID: 26739
	public bool TurnOnAtThreshhold = true;

	// Token: 0x04006874 RID: 26740
	public bool UseSmoothedLoudness;

	// Token: 0x04006875 RID: 26741
	public float Scale = 1f;
}
