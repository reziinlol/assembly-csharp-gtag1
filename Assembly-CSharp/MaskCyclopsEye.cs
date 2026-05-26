using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000BA RID: 186
public class MaskCyclopsEye : MonoBehaviour
{
	// Token: 0x0600048B RID: 1163 RVA: 0x00019B08 File Offset: 0x00017D08
	private void OnEnable()
	{
		this.ScheduleNextBlink();
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnDisable()
	{
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x00019B10 File Offset: 0x00017D10
	public void Update()
	{
		if (Time.time >= this.nextBlinkTime)
		{
			UnityEvent onBlink = this.OnBlink;
			if (onBlink != null)
			{
				onBlink.Invoke();
			}
			this.ScheduleNextBlink();
		}
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x00019B10 File Offset: 0x00017D10
	public void Tick()
	{
		if (Time.time >= this.nextBlinkTime)
		{
			UnityEvent onBlink = this.OnBlink;
			if (onBlink != null)
			{
				onBlink.Invoke();
			}
			this.ScheduleNextBlink();
		}
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x00019B38 File Offset: 0x00017D38
	private void ScheduleNextBlink()
	{
		float num = Random.Range(this.minWaitTime, this.maxWaitTime);
		this.nextBlinkTime = Time.time + num;
	}

	// Token: 0x040004E7 RID: 1255
	[Tooltip("Invoked when it's time to trigger a blink (e.g., play animation one-shot).")]
	public UnityEvent OnBlink;

	// Token: 0x040004E8 RID: 1256
	[Tooltip("Minimum time in seconds between blinks.")]
	[SerializeField]
	private float minWaitTime = 3f;

	// Token: 0x040004E9 RID: 1257
	[Tooltip("Maximum time in seconds between blinks.")]
	[SerializeField]
	private float maxWaitTime = 5f;

	// Token: 0x040004EA RID: 1258
	private float nextBlinkTime;
}
