using System;
using UnityEngine;

// Token: 0x020007A1 RID: 1953
public class GRFtueExitTrigger : GorillaTriggerBox
{
	// Token: 0x060031EA RID: 12778 RVA: 0x00112345 File Offset: 0x00110545
	public override void OnBoxTriggered()
	{
		this.startTime = Time.time;
		this.ftueObject.InterruptWaitingTimer();
		this.ftueObject.playerLight.GetComponentInChildren<Light>().intensity = 0.25f;
	}

	// Token: 0x060031EB RID: 12779 RVA: 0x00112377 File Offset: 0x00110577
	private void Update()
	{
		if (this.startTime > 0f && Time.time - this.startTime > this.delayTime)
		{
			this.ftueObject.ChangeState(GRFirstTimeUserExperience.TransitionState.Flicker);
			this.startTime = -1f;
		}
	}

	// Token: 0x040040D2 RID: 16594
	public GRFirstTimeUserExperience ftueObject;

	// Token: 0x040040D3 RID: 16595
	public float delayTime = 5f;

	// Token: 0x040040D4 RID: 16596
	private float startTime = -1f;
}
