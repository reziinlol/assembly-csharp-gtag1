using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000F83 RID: 3971
	public class TappableBell : Tappable
	{
		// Token: 0x140000A7 RID: 167
		// (add) Token: 0x06006328 RID: 25384 RVA: 0x001FE928 File Offset: 0x001FCB28
		// (remove) Token: 0x06006329 RID: 25385 RVA: 0x001FE960 File Offset: 0x001FCB60
		public event TappableBell.ObstacleCourseTriggerEvent OnTapped;

		// Token: 0x0600632A RID: 25386 RVA: 0x001FE998 File Offset: 0x001FCB98
		public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
		{
			if (!PhotonNetwork.LocalPlayer.IsMasterClient)
			{
				return;
			}
			if (!this.rpcCooldown.CheckCallTime(Time.time))
			{
				return;
			}
			this.winnerRig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
			if (this.winnerRig != null)
			{
				TappableBell.ObstacleCourseTriggerEvent onTapped = this.OnTapped;
				if (onTapped == null)
				{
					return;
				}
				onTapped(this.winnerRig);
			}
		}

		// Token: 0x040071C7 RID: 29127
		private VRRig winnerRig;

		// Token: 0x040071C9 RID: 29129
		public CallLimiter rpcCooldown;

		// Token: 0x02000F84 RID: 3972
		// (Invoke) Token: 0x0600632D RID: 25389
		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
