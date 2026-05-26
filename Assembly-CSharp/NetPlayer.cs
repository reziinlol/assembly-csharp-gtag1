using System;
using System.Collections.Generic;
using Fusion;
using GorillaTag;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000452 RID: 1106
[Serializable]
public abstract class NetPlayer : ObjectPoolEvents
{
	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x06001A65 RID: 6757
	public abstract bool IsValid { get; }

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x06001A66 RID: 6758
	public abstract int ActorNumber { get; }

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x06001A67 RID: 6759
	public abstract string UserId { get; }

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x06001A68 RID: 6760
	public abstract bool IsMasterClient { get; }

	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x06001A69 RID: 6761
	public abstract bool IsLocal { get; }

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x06001A6A RID: 6762
	public abstract bool IsNull { get; }

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x06001A6B RID: 6763
	public abstract string NickName { get; }

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x06001A6C RID: 6764 RVA: 0x00093A8B File Offset: 0x00091C8B
	// (set) Token: 0x06001A6D RID: 6765 RVA: 0x00093A93 File Offset: 0x00091C93
	public virtual string SanitizedNickName { get; set; } = string.Empty;

	// Token: 0x170002AC RID: 684
	// (get) Token: 0x06001A6E RID: 6766
	public abstract string DefaultName { get; }

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x06001A6F RID: 6767
	public abstract bool InRoom { get; }

	// Token: 0x170002AE RID: 686
	// (get) Token: 0x06001A70 RID: 6768 RVA: 0x00093A9C File Offset: 0x00091C9C
	// (set) Token: 0x06001A71 RID: 6769 RVA: 0x00093AA4 File Offset: 0x00091CA4
	public virtual float JoinedTime { get; private set; }

	// Token: 0x170002AF RID: 687
	// (get) Token: 0x06001A72 RID: 6770 RVA: 0x00093AAD File Offset: 0x00091CAD
	// (set) Token: 0x06001A73 RID: 6771 RVA: 0x00093AB5 File Offset: 0x00091CB5
	public virtual float LeftTime { get; private set; }

	// Token: 0x06001A74 RID: 6772
	public abstract bool Equals(NetPlayer myPlayer, NetPlayer other);

	// Token: 0x06001A75 RID: 6773 RVA: 0x00093ABE File Offset: 0x00091CBE
	public virtual void OnReturned()
	{
		this.LeftTime = Time.time;
		HashSet<int> singleCallRPCStatus = this.SingleCallRPCStatus;
		if (singleCallRPCStatus != null)
		{
			singleCallRPCStatus.Clear();
		}
		this.SanitizedNickName = string.Empty;
	}

	// Token: 0x06001A76 RID: 6774 RVA: 0x00093AE7 File Offset: 0x00091CE7
	public virtual void OnTaken()
	{
		this.JoinedTime = Time.time;
		HashSet<int> singleCallRPCStatus = this.SingleCallRPCStatus;
		if (singleCallRPCStatus == null)
		{
			return;
		}
		singleCallRPCStatus.Clear();
	}

	// Token: 0x06001A77 RID: 6775 RVA: 0x00093B04 File Offset: 0x00091D04
	public virtual bool CheckSingleCallRPC(NetPlayer.SingleCallRPC RPCType)
	{
		return this.SingleCallRPCStatus.Contains((int)RPCType);
	}

	// Token: 0x06001A78 RID: 6776 RVA: 0x00093B12 File Offset: 0x00091D12
	public virtual void ReceivedSingleCallRPC(NetPlayer.SingleCallRPC RPCType)
	{
		this.SingleCallRPCStatus.Add((int)RPCType);
	}

	// Token: 0x06001A79 RID: 6777 RVA: 0x00093B21 File Offset: 0x00091D21
	public Player GetPlayerRef()
	{
		return (this as PunNetPlayer).PlayerRef;
	}

	// Token: 0x06001A7A RID: 6778 RVA: 0x00093B2E File Offset: 0x00091D2E
	public string ToStringFull()
	{
		return string.Format("#{0: 0:00} '{1}', Not sure what to do with inactive yet, Or custom props?", this.ActorNumber, this.NickName);
	}

	// Token: 0x06001A7B RID: 6779 RVA: 0x00093B4B File Offset: 0x00091D4B
	public static implicit operator NetPlayer(Player player)
	{
		Utils.Log("Using an implicit cast from Player to NetPlayer. Please make sure this was intended as this has potential to cause errors when switching between network backends");
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001A7C RID: 6780 RVA: 0x00093B6E File Offset: 0x00091D6E
	public static implicit operator NetPlayer(PlayerRef player)
	{
		Utils.Log("Using an implicit cast from PlayerRef to NetPlayer. Please make sure this was intended as this has potential to cause errors when switching between network backends");
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001A7D RID: 6781 RVA: 0x00093B91 File Offset: 0x00091D91
	public static NetPlayer Get(Player player)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001A7E RID: 6782 RVA: 0x00093BAA File Offset: 0x00091DAA
	public static NetPlayer Get(PlayerRef player)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001A7F RID: 6783 RVA: 0x00093BC3 File Offset: 0x00091DC3
	public static NetPlayer Get(int actorNr)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(actorNr) : null) ?? null;
	}

	// Token: 0x04002531 RID: 9521
	private HashSet<int> SingleCallRPCStatus = new HashSet<int>(5);

	// Token: 0x02000453 RID: 1107
	public enum SingleCallRPC
	{
		// Token: 0x04002533 RID: 9523
		CMS_RequestRoomInitialization,
		// Token: 0x04002534 RID: 9524
		CMS_RequestTriggerHistory,
		// Token: 0x04002535 RID: 9525
		CMS_SyncTriggerHistory,
		// Token: 0x04002536 RID: 9526
		CMS_SyncTriggerCounts,
		// Token: 0x04002537 RID: 9527
		RankedSendScoreToLateJoiner,
		// Token: 0x04002538 RID: 9528
		Count
	}
}
