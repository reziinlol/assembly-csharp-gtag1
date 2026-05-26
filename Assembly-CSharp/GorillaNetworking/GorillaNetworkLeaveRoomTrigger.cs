using System;
using System.Runtime.CompilerServices;
using GorillaTagScripts;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02001061 RID: 4193
	public class GorillaNetworkLeaveRoomTrigger : GorillaTriggerBox
	{
		// Token: 0x06006958 RID: 26968 RVA: 0x00221868 File Offset: 0x0021FA68
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (NetworkSystem.Instance.InRoom && (!this.excludePrivateRooms || !NetworkSystem.Instance.SessionIsPrivate))
			{
				if (FriendshipGroupDetection.Instance.IsInParty)
				{
					FriendshipGroupDetection.Instance.LeaveParty();
					this.DisconnectAfterDelay(1f);
					return;
				}
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
		}

		// Token: 0x06006959 RID: 26969 RVA: 0x002218C8 File Offset: 0x0021FAC8
		private void DisconnectAfterDelay(float seconds)
		{
			GorillaNetworkLeaveRoomTrigger.<DisconnectAfterDelay>d__2 <DisconnectAfterDelay>d__;
			<DisconnectAfterDelay>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DisconnectAfterDelay>d__.seconds = seconds;
			<DisconnectAfterDelay>d__.<>1__state = -1;
			<DisconnectAfterDelay>d__.<>t__builder.Start<GorillaNetworkLeaveRoomTrigger.<DisconnectAfterDelay>d__2>(ref <DisconnectAfterDelay>d__);
		}

		// Token: 0x04007980 RID: 31104
		[SerializeField]
		private bool excludePrivateRooms;
	}
}
