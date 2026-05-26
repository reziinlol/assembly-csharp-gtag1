using System;
using UnityEngine;

// Token: 0x02000603 RID: 1539
public class MonkeBallTeamZoneSelector : MonoBehaviour
{
	// Token: 0x0600266A RID: 9834 RVA: 0x000CB5E8 File Offset: 0x000C97E8
	private void OnTriggerEnter(Collider other)
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(other, true);
		if (gamePlayer != null && gamePlayer.IsLocalPlayer() && gamePlayer.teamId != this.teamId)
		{
			MonkeBallGame.Instance.RequestSetTeam(this.teamId);
		}
	}

	// Token: 0x040031C7 RID: 12743
	public int teamId;
}
