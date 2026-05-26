using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000481 RID: 1153
[Serializable]
public class PunNetPlayer : NetPlayer
{
	// Token: 0x170002FA RID: 762
	// (get) Token: 0x06001C17 RID: 7191 RVA: 0x00098A40 File Offset: 0x00096C40
	// (set) Token: 0x06001C18 RID: 7192 RVA: 0x00098A48 File Offset: 0x00096C48
	public Player PlayerRef { get; private set; }

	// Token: 0x06001C1A RID: 7194 RVA: 0x00098A59 File Offset: 0x00096C59
	public void InitPlayer(Player playerRef)
	{
		this.PlayerRef = playerRef;
	}

	// Token: 0x170002FB RID: 763
	// (get) Token: 0x06001C1B RID: 7195 RVA: 0x00098A62 File Offset: 0x00096C62
	public override bool IsValid
	{
		get
		{
			return !this.PlayerRef.IsInactive;
		}
	}

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x06001C1C RID: 7196 RVA: 0x00098A72 File Offset: 0x00096C72
	public override int ActorNumber
	{
		get
		{
			Player playerRef = this.PlayerRef;
			if (playerRef == null)
			{
				return -1;
			}
			return playerRef.ActorNumber;
		}
	}

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x06001C1D RID: 7197 RVA: 0x00098A85 File Offset: 0x00096C85
	public override string UserId
	{
		get
		{
			return this.PlayerRef.UserId;
		}
	}

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06001C1E RID: 7198 RVA: 0x00098A92 File Offset: 0x00096C92
	public override bool IsMasterClient
	{
		get
		{
			return this.PlayerRef.IsMasterClient;
		}
	}

	// Token: 0x170002FF RID: 767
	// (get) Token: 0x06001C1F RID: 7199 RVA: 0x00098A9F File Offset: 0x00096C9F
	public override bool IsLocal
	{
		get
		{
			return this.PlayerRef == PhotonNetwork.LocalPlayer;
		}
	}

	// Token: 0x17000300 RID: 768
	// (get) Token: 0x06001C20 RID: 7200 RVA: 0x00098AAE File Offset: 0x00096CAE
	public override bool IsNull
	{
		get
		{
			return this.PlayerRef == null;
		}
	}

	// Token: 0x17000301 RID: 769
	// (get) Token: 0x06001C21 RID: 7201 RVA: 0x00098AB9 File Offset: 0x00096CB9
	public override string NickName
	{
		get
		{
			return this.PlayerRef.NickName;
		}
	}

	// Token: 0x17000302 RID: 770
	// (get) Token: 0x06001C22 RID: 7202 RVA: 0x00098AC6 File Offset: 0x00096CC6
	public override string DefaultName
	{
		get
		{
			return this.PlayerRef.DefaultName;
		}
	}

	// Token: 0x17000303 RID: 771
	// (get) Token: 0x06001C23 RID: 7203 RVA: 0x00098AD3 File Offset: 0x00096CD3
	public override bool InRoom
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return currentRoom != null && currentRoom.Players.ContainsValue(this.PlayerRef);
		}
	}

	// Token: 0x06001C24 RID: 7204 RVA: 0x00098AF0 File Offset: 0x00096CF0
	public override bool Equals(NetPlayer myPlayer, NetPlayer other)
	{
		return myPlayer != null && other != null && ((PunNetPlayer)myPlayer).PlayerRef.Equals(((PunNetPlayer)other).PlayerRef);
	}

	// Token: 0x06001C25 RID: 7205 RVA: 0x00098B15 File Offset: 0x00096D15
	public override void OnReturned()
	{
		base.OnReturned();
	}

	// Token: 0x06001C26 RID: 7206 RVA: 0x00098B1D File Offset: 0x00096D1D
	public override void OnTaken()
	{
		base.OnTaken();
		this.PlayerRef = null;
	}
}
