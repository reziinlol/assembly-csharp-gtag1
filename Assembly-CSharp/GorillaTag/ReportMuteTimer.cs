using System;
using Photon.Realtime;

namespace GorillaTag
{
	// Token: 0x0200116D RID: 4461
	internal class ReportMuteTimer : TickSystemTimerAbstract, ObjectPoolEvents
	{
		// Token: 0x17000ACE RID: 2766
		// (get) Token: 0x060070FB RID: 28923 RVA: 0x0024F2A4 File Offset: 0x0024D4A4
		// (set) Token: 0x060070FC RID: 28924 RVA: 0x0024F2AC File Offset: 0x0024D4AC
		public int Muted { get; set; }

		// Token: 0x060070FD RID: 28925 RVA: 0x0024F2B8 File Offset: 0x0024D4B8
		public override void OnTimedEvent()
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				this.Stop();
				return;
			}
			ReportMuteTimer.content[0] = this.m_playerID;
			ReportMuteTimer.content[1] = this.Muted;
			ReportMuteTimer.content[2] = ((this.m_nickName.Length > 12) ? this.m_nickName.Remove(12) : this.m_nickName);
			ReportMuteTimer.content[3] = NetworkSystem.Instance.LocalPlayer.NickName;
			ReportMuteTimer.content[4] = !NetworkSystem.Instance.SessionIsPrivate;
			ReportMuteTimer.content[5] = NetworkSystem.Instance.RoomStringStripped();
			NetworkSystemRaiseEvent.RaiseEvent(51, ReportMuteTimer.content, ReportMuteTimer.netEventOptions, true);
			this.Stop();
		}

		// Token: 0x060070FE RID: 28926 RVA: 0x0024F37A File Offset: 0x0024D57A
		public void SetReportData(string id, string name, int muted)
		{
			this.Muted = muted;
			this.m_playerID = id;
			this.m_nickName = name;
		}

		// Token: 0x060070FF RID: 28927 RVA: 0x000028C5 File Offset: 0x00000AC5
		void ObjectPoolEvents.OnTaken()
		{
		}

		// Token: 0x06007100 RID: 28928 RVA: 0x0024F391 File Offset: 0x0024D591
		void ObjectPoolEvents.OnReturned()
		{
			if (base.Running)
			{
				this.OnTimedEvent();
			}
			this.m_playerID = string.Empty;
			this.m_nickName = string.Empty;
			this.Muted = 0;
		}

		// Token: 0x0400811F RID: 33055
		private static readonly NetEventOptions netEventOptions = new NetEventOptions
		{
			Flags = new WebFlags(3),
			TargetActors = new int[]
			{
				-1
			}
		};

		// Token: 0x04008120 RID: 33056
		private static readonly object[] content = new object[6];

		// Token: 0x04008121 RID: 33057
		private const byte evCode = 51;

		// Token: 0x04008123 RID: 33059
		private string m_playerID;

		// Token: 0x04008124 RID: 33060
		private string m_nickName;
	}
}
