using System;
using UnityEngine;

// Token: 0x02000E0D RID: 3597
[Serializable]
public class VoiceLoudnessReactorParticleSystemTarget
{
	// Token: 0x17000840 RID: 2112
	// (get) Token: 0x060057A8 RID: 22440 RVA: 0x001C67D3 File Offset: 0x001C49D3
	// (set) Token: 0x060057A9 RID: 22441 RVA: 0x001C67DB File Offset: 0x001C49DB
	public float InitialSpeed
	{
		get
		{
			return this.initialSpeed;
		}
		set
		{
			this.initialSpeed = value;
		}
	}

	// Token: 0x17000841 RID: 2113
	// (get) Token: 0x060057AA RID: 22442 RVA: 0x001C67E4 File Offset: 0x001C49E4
	// (set) Token: 0x060057AB RID: 22443 RVA: 0x001C67EC File Offset: 0x001C49EC
	public float InitialRate
	{
		get
		{
			return this.initialRate;
		}
		set
		{
			this.initialRate = value;
		}
	}

	// Token: 0x17000842 RID: 2114
	// (get) Token: 0x060057AC RID: 22444 RVA: 0x001C67F5 File Offset: 0x001C49F5
	// (set) Token: 0x060057AD RID: 22445 RVA: 0x001C67FD File Offset: 0x001C49FD
	public float InitialSize
	{
		get
		{
			return this.initialSize;
		}
		set
		{
			this.initialSize = value;
		}
	}

	// Token: 0x04006866 RID: 26726
	public ParticleSystem particleSystem;

	// Token: 0x04006867 RID: 26727
	public bool UseSmoothedLoudness;

	// Token: 0x04006868 RID: 26728
	public float Scale = 1f;

	// Token: 0x04006869 RID: 26729
	private float initialSpeed;

	// Token: 0x0400686A RID: 26730
	private float initialRate;

	// Token: 0x0400686B RID: 26731
	private float initialSize;

	// Token: 0x0400686C RID: 26732
	public AnimationCurve speed;

	// Token: 0x0400686D RID: 26733
	public AnimationCurve rate;

	// Token: 0x0400686E RID: 26734
	public AnimationCurve size;

	// Token: 0x0400686F RID: 26735
	[HideInInspector]
	public ParticleSystem.MainModule Main;

	// Token: 0x04006870 RID: 26736
	[HideInInspector]
	public ParticleSystem.EmissionModule Emission;
}
