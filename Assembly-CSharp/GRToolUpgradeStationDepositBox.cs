using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000818 RID: 2072
public class GRToolUpgradeStationDepositBox : MonoBehaviour
{
	// Token: 0x0600353C RID: 13628 RVA: 0x00126C94 File Offset: 0x00124E94
	public void OnTriggerEnter(Collider other)
	{
		GRTool component = other.attachedRigidbody.GetComponent<GRTool>();
		if (component.IsNotNull() && component.gameEntity.IsNotNull() && component.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber && component.gameEntity.IsHeldByLocalPlayer())
		{
			Debug.LogError("Tool Deposited");
			this.upgradeStation.ToolInserted(component);
		}
	}

	// Token: 0x040045C4 RID: 17860
	public GRToolUpgradeStation upgradeStation;
}
