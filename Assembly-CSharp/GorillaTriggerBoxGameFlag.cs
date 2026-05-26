using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005D4 RID: 1492
public class GorillaTriggerBoxGameFlag : GorillaTriggerBox
{
	// Token: 0x06002542 RID: 9538 RVA: 0x000C5F01 File Offset: 0x000C4101
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		PhotonView.Get(Object.FindAnyObjectByType<GorillaGameManager>()).RPC(this.functionName, RpcTarget.MasterClient, null);
	}

	// Token: 0x040030C3 RID: 12483
	public string functionName;
}
