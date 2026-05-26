using System;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class Oscillator : MonoBehaviour
{
	// Token: 0x0600008E RID: 142 RVA: 0x00004D55 File Offset: 0x00002F55
	public void Init(Vector3 center, Vector3 radius, Vector3 frequency, Vector3 startPhase)
	{
		this.Center = center;
		this.Radius = radius;
		this.Frequency = frequency;
		this.Phase = startPhase;
	}

	// Token: 0x0600008F RID: 143 RVA: 0x00004D74 File Offset: 0x00002F74
	private float SampleWave(float phase)
	{
		switch (this.WaveType)
		{
		case Oscillator.WaveTypeEnum.Sine:
			return Mathf.Sin(phase);
		case Oscillator.WaveTypeEnum.Square:
			phase = Mathf.Repeat(phase, 6.2831855f);
			if (phase >= 3.1415927f)
			{
				return -1f;
			}
			return 1f;
		case Oscillator.WaveTypeEnum.Triangle:
			phase = Mathf.Repeat(phase, 6.2831855f);
			if (phase < 1.5707964f)
			{
				return phase / 1.5707964f;
			}
			if (phase < 3.1415927f)
			{
				return 1f - (phase - 1.5707964f) / 1.5707964f;
			}
			if (phase < 4.712389f)
			{
				return (3.1415927f - phase) / 1.5707964f;
			}
			return (phase - 4.712389f) / 1.5707964f - 1f;
		default:
			return 0f;
		}
	}

	// Token: 0x06000090 RID: 144 RVA: 0x00004E2F File Offset: 0x0000302F
	public void OnEnable()
	{
		this.m_initCenter = base.transform.position;
	}

	// Token: 0x06000091 RID: 145 RVA: 0x00004E44 File Offset: 0x00003044
	public void Update()
	{
		this.Phase += this.Frequency * 2f * 3.1415927f * Time.deltaTime;
		Vector3 position = this.UseCenter ? this.Center : this.m_initCenter;
		position.x += this.Radius.x * this.SampleWave(this.Phase.x);
		position.y += this.Radius.y * this.SampleWave(this.Phase.y);
		position.z += this.Radius.z * this.SampleWave(this.Phase.z);
		base.transform.position = position;
	}

	// Token: 0x040000A6 RID: 166
	public Oscillator.WaveTypeEnum WaveType;

	// Token: 0x040000A7 RID: 167
	private Vector3 m_initCenter;

	// Token: 0x040000A8 RID: 168
	public bool UseCenter;

	// Token: 0x040000A9 RID: 169
	public Vector3 Center;

	// Token: 0x040000AA RID: 170
	public Vector3 Radius;

	// Token: 0x040000AB RID: 171
	public Vector3 Frequency;

	// Token: 0x040000AC RID: 172
	public Vector3 Phase;

	// Token: 0x02000026 RID: 38
	public enum WaveTypeEnum
	{
		// Token: 0x040000AE RID: 174
		Sine,
		// Token: 0x040000AF RID: 175
		Square,
		// Token: 0x040000B0 RID: 176
		Triangle
	}
}
