using System;
using UnityEngine;

// Token: 0x0200032E RID: 814
public static class GTAudioClipExtensions
{
	// Token: 0x0600141D RID: 5149 RVA: 0x0006C7F8 File Offset: 0x0006A9F8
	public static float GetPeakMagnitude(this AudioClip audioClip)
	{
		if (audioClip == null)
		{
			return 0f;
		}
		float num = float.NegativeInfinity;
		float[] array = new float[audioClip.samples];
		audioClip.GetData(array, 0);
		foreach (float f in array)
		{
			num = Mathf.Max(num, Mathf.Abs(f));
		}
		return num;
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x0006C854 File Offset: 0x0006AA54
	public static float GetRMSMagnitude(this AudioClip audioClip)
	{
		if (audioClip == null)
		{
			return 0f;
		}
		float num = 0f;
		float[] array = new float[audioClip.samples];
		audioClip.GetData(array, 0);
		foreach (float num2 in array)
		{
			num += num2 * num2;
		}
		return Mathf.Sqrt(num / (float)array.Length);
	}
}
