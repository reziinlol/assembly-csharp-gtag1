using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x0200020B RID: 523
public class ScheduledTimelinePlayer : MonoBehaviour
{
	// Token: 0x06000DCB RID: 3531 RVA: 0x0004B8F6 File Offset: 0x00049AF6
	protected void OnEnable()
	{
		this.scheduledEventID = BetterDayNightManager.RegisterScheduledEvent(this.eventHour, new Action(this.HandleScheduledEvent));
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x0004B915 File Offset: 0x00049B15
	protected void OnDisable()
	{
		BetterDayNightManager.UnregisterScheduledEvent(this.scheduledEventID);
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x0004B922 File Offset: 0x00049B22
	private void HandleScheduledEvent()
	{
		this.timeline.Play();
	}

	// Token: 0x04001073 RID: 4211
	public PlayableDirector timeline;

	// Token: 0x04001074 RID: 4212
	public int eventHour = 7;

	// Token: 0x04001075 RID: 4213
	private int scheduledEventID;
}
