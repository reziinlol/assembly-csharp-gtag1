using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000DC1 RID: 3521
public class ServerTimeEvent : TimeEvent
{
	// Token: 0x06005656 RID: 22102 RVA: 0x001C05CD File Offset: 0x001BE7CD
	private void Awake()
	{
		this.eventTimes = new HashSet<ServerTimeEvent.EventTime>(this.times);
	}

	// Token: 0x06005657 RID: 22103 RVA: 0x001C05E0 File Offset: 0x001BE7E0
	private void Update()
	{
		if (GorillaComputer.instance == null || Time.time - this.lastQueryTime < this.queryTime)
		{
			return;
		}
		ServerTimeEvent.EventTime item = new ServerTimeEvent.EventTime(GorillaComputer.instance.GetServerTime().Hour, GorillaComputer.instance.GetServerTime().Minute);
		bool flag = this.eventTimes.Contains(item);
		if (!this._ongoing && flag)
		{
			base.StartEvent();
		}
		if (this._ongoing && !flag)
		{
			base.StopEvent();
		}
		this.lastQueryTime = Time.time;
	}

	// Token: 0x0400663A RID: 26170
	[SerializeField]
	private ServerTimeEvent.EventTime[] times;

	// Token: 0x0400663B RID: 26171
	[SerializeField]
	private float queryTime = 60f;

	// Token: 0x0400663C RID: 26172
	private float lastQueryTime;

	// Token: 0x0400663D RID: 26173
	private HashSet<ServerTimeEvent.EventTime> eventTimes;

	// Token: 0x02000DC2 RID: 3522
	[Serializable]
	public struct EventTime
	{
		// Token: 0x06005659 RID: 22105 RVA: 0x001C068F File Offset: 0x001BE88F
		public EventTime(int h, int m)
		{
			this.hour = h;
			this.minute = m;
		}

		// Token: 0x0400663E RID: 26174
		public int hour;

		// Token: 0x0400663F RID: 26175
		public int minute;
	}
}
