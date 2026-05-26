using System;
using UnityEngine;

// Token: 0x02000E0B RID: 3595
[Serializable]
public class VoiceLoudnessReactorTransformTarget
{
	// Token: 0x1700083E RID: 2110
	// (get) Token: 0x060057A2 RID: 22434 RVA: 0x001C6775 File Offset: 0x001C4975
	// (set) Token: 0x060057A3 RID: 22435 RVA: 0x001C677D File Offset: 0x001C497D
	public Vector3 Initial
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

	// Token: 0x0400685C RID: 26716
	public Transform transform;

	// Token: 0x0400685D RID: 26717
	private Vector3 initial;

	// Token: 0x0400685E RID: 26718
	public Vector3 Max = Vector3.one;

	// Token: 0x0400685F RID: 26719
	public float Scale = 1f;

	// Token: 0x04006860 RID: 26720
	public bool UseSmoothedLoudness;
}
