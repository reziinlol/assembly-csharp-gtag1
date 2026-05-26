using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class RotationStepper : MonoBehaviour
{
	// Token: 0x06000093 RID: 147 RVA: 0x00004F21 File Offset: 0x00003121
	public void OnEnable()
	{
		this.m_phase = 0f;
		Random.InitState(0);
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00004F34 File Offset: 0x00003134
	public void Update()
	{
		this.m_phase += this.Frequency * Time.deltaTime;
		RotationStepper.ModeEnum mode = this.Mode;
		if (mode == RotationStepper.ModeEnum.Fixed)
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, (Mathf.Repeat(this.m_phase, 2f) < 1f) ? -25f : 25f);
			return;
		}
		if (mode != RotationStepper.ModeEnum.Random)
		{
			return;
		}
		while (this.m_phase >= 1f)
		{
			Random.InitState(Time.frameCount);
			base.transform.rotation = Random.rotationUniform;
			this.m_phase -= 1f;
		}
	}

	// Token: 0x040000B1 RID: 177
	public RotationStepper.ModeEnum Mode;

	// Token: 0x040000B2 RID: 178
	[ConditionalField("Mode", RotationStepper.ModeEnum.Fixed, null, null, null, null, null)]
	public float Angle = 25f;

	// Token: 0x040000B3 RID: 179
	public float Frequency;

	// Token: 0x040000B4 RID: 180
	private float m_phase;

	// Token: 0x02000028 RID: 40
	public enum ModeEnum
	{
		// Token: 0x040000B6 RID: 182
		Fixed,
		// Token: 0x040000B7 RID: 183
		Random
	}
}
