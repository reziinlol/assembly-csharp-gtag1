using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000CDB RID: 3291
internal class PaintbrawlRPCs : RPCNetworkBase
{
	// Token: 0x06005195 RID: 20885 RVA: 0x001ADD6D File Offset: 0x001ABF6D
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.paintbrawlManager = (GorillaPaintbrawlManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x06005196 RID: 20886 RVA: 0x001ADD88 File Offset: 0x001ABF88
	[PunRPC]
	public void RPC_ReportSlingshotHit(Player taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RPC_ReportSlingshotHit");
		if (!NetworkSystem.Instance.IsMasterClient || taggedPlayer == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(taggedPlayer);
		PhotonMessageInfoWrapped info2 = new PhotonMessageInfoWrapped(info);
		this.paintbrawlManager.ReportSlingshotHit(player, hitLocation, projectileCount, info2);
	}

	// Token: 0x040062EC RID: 25324
	private GameModeSerializer serializer;

	// Token: 0x040062ED RID: 25325
	private GorillaPaintbrawlManager paintbrawlManager;
}
