using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000451 RID: 1105
public struct PhotonMessageInfoWrapped
{
	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x06001A5F RID: 6751 RVA: 0x000939A9 File Offset: 0x00091BA9
	public double SentServerTime
	{
		get
		{
			return this.sentTick / 1000.0;
		}
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x000939BD File Offset: 0x00091BBD
	public PhotonMessageInfoWrapped(PhotonMessageInfo info)
	{
		Player sender = info.Sender;
		this.senderID = ((sender != null) ? sender.ActorNumber : -1);
		this.Sender = NetPlayer.Get(info.Sender);
		this.sentTick = info.SentServerTimestamp;
		this.punInfo = info;
	}

	// Token: 0x06001A61 RID: 6753 RVA: 0x000939FC File Offset: 0x00091BFC
	public PhotonMessageInfoWrapped(RpcInfo info)
	{
		this.senderID = info.Source.PlayerId;
		this.Sender = NetPlayer.Get(info.Source);
		this.sentTick = info.Tick.Raw;
		this.punInfo = default(PhotonMessageInfo);
	}

	// Token: 0x06001A62 RID: 6754 RVA: 0x00093A49 File Offset: 0x00091C49
	public PhotonMessageInfoWrapped(int playerID, int tick)
	{
		this.senderID = playerID;
		this.Sender = NetworkSystem.Instance.GetPlayer(this.senderID);
		this.sentTick = tick;
		this.punInfo = default(PhotonMessageInfo);
	}

	// Token: 0x06001A63 RID: 6755 RVA: 0x00093A7B File Offset: 0x00091C7B
	public static implicit operator PhotonMessageInfoWrapped(PhotonMessageInfo info)
	{
		return new PhotonMessageInfoWrapped(info);
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x00093A83 File Offset: 0x00091C83
	public static implicit operator PhotonMessageInfoWrapped(RpcInfo info)
	{
		return new PhotonMessageInfoWrapped(info);
	}

	// Token: 0x0400252A RID: 9514
	public readonly int senderID;

	// Token: 0x0400252B RID: 9515
	public readonly int sentTick;

	// Token: 0x0400252C RID: 9516
	public readonly PhotonMessageInfo punInfo;

	// Token: 0x0400252D RID: 9517
	public readonly NetPlayer Sender;
}
