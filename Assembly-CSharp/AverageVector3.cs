using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000D24 RID: 3364
public class AverageVector3
{
	// Token: 0x0600530A RID: 21258 RVA: 0x001B3059 File Offset: 0x001B1259
	public AverageVector3(float averagingWindow = 0.1f)
	{
		this.timeWindow = averagingWindow;
	}

	// Token: 0x0600530B RID: 21259 RVA: 0x001B3080 File Offset: 0x001B1280
	public void AddSample(Vector3 sample, float time)
	{
		this.samples.Add(new AverageVector3.Sample
		{
			timeStamp = time,
			value = sample
		});
		this.RefreshSamples();
	}

	// Token: 0x0600530C RID: 21260 RVA: 0x001B30B8 File Offset: 0x001B12B8
	public Vector3 GetAverage()
	{
		this.RefreshSamples();
		Vector3 a = Vector3.zero;
		for (int i = 0; i < this.samples.Count; i++)
		{
			a += this.samples[i].value;
		}
		return a / (float)this.samples.Count;
	}

	// Token: 0x0600530D RID: 21261 RVA: 0x001B3113 File Offset: 0x001B1313
	public void Clear()
	{
		this.samples.Clear();
	}

	// Token: 0x0600530E RID: 21262 RVA: 0x001B3120 File Offset: 0x001B1320
	private void RefreshSamples()
	{
		float num = Time.time - this.timeWindow;
		for (int i = this.samples.Count - 1; i >= 0; i--)
		{
			if (this.samples[i].timeStamp < num)
			{
				this.samples.RemoveAt(i);
			}
		}
	}

	// Token: 0x0400645A RID: 25690
	private List<AverageVector3.Sample> samples = new List<AverageVector3.Sample>();

	// Token: 0x0400645B RID: 25691
	private float timeWindow = 0.1f;

	// Token: 0x02000D25 RID: 3365
	public struct Sample
	{
		// Token: 0x0400645C RID: 25692
		public float timeStamp;

		// Token: 0x0400645D RID: 25693
		public Vector3 value;
	}
}
