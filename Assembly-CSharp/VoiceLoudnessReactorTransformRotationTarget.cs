using System;
using UnityEngine;

// Token: 0x02000E0C RID: 3596
[Serializable]
public class VoiceLoudnessReactorTransformRotationTarget
{
	// Token: 0x1700083F RID: 2111
	// (get) Token: 0x060057A5 RID: 22437 RVA: 0x001C67A4 File Offset: 0x001C49A4
	// (set) Token: 0x060057A6 RID: 22438 RVA: 0x001C67AC File Offset: 0x001C49AC
	public Quaternion Initial
	{
		get
		{
			return this.initial;
		}
		set
		{
			this.initial = value;
		}
	}

	// Token: 0x04006861 RID: 26721
	public Transform transform;

	// Token: 0x04006862 RID: 26722
	private Quaternion initial;

	// Token: 0x04006863 RID: 26723
	public Quaternion Max = Quaternion.identity;

	// Token: 0x04006864 RID: 26724
	public float Scale = 1f;

	// Token: 0x04006865 RID: 26725
	public bool UseSmoothedLoudness;
}
