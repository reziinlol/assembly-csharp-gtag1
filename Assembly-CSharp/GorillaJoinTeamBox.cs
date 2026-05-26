using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000877 RID: 2167
public class GorillaJoinTeamBox : GorillaTriggerBox
{
	// Token: 0x06003886 RID: 14470 RVA: 0x00135304 File Offset: 0x00133504
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		if (GameObject.FindGameObjectWithTag("GorillaGameManager").GetComponent<GorillaGameManager>() != null)
		{
			bool inRoom = PhotonNetwork.InRoom;
		}
	}

	// Token: 0x0400488A RID: 18570
	public bool joinRedTeam;
}
