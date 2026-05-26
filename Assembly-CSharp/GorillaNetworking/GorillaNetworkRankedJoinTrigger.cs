using System;
using GorillaGameModes;

namespace GorillaNetworking
{
	// Token: 0x02001064 RID: 4196
	public class GorillaNetworkRankedJoinTrigger : GorillaNetworkJoinTrigger
	{
		// Token: 0x0600695E RID: 26974 RVA: 0x00221A2E File Offset: 0x0021FC2E
		public override string GetFullDesiredGameModeString()
		{
			return new GameModeString
			{
				zone = this.networkZone,
				gameType = base.GetDesiredGameType()
			}.ToString();
		}

		// Token: 0x0600695F RID: 26975 RVA: 0x00221A52 File Offset: 0x0021FC52
		public override void OnBoxTriggered()
		{
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			PhotonNetworkController.Instance.ClearDeferredJoin();
			PhotonNetworkController.Instance.AttemptToJoinRankedPublicRoom(this, JoinType.Solo);
		}
	}
}
