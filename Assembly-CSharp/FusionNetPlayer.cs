using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

// Token: 0x02000430 RID: 1072
public class FusionNetPlayer : NetPlayer
{
	// Token: 0x1700027B RID: 635
	// (get) Token: 0x06001980 RID: 6528 RVA: 0x0008F015 File Offset: 0x0008D215
	// (set) Token: 0x06001981 RID: 6529 RVA: 0x0008F01D File Offset: 0x0008D21D
	public PlayerRef PlayerRef { get; private set; }

	// Token: 0x06001982 RID: 6530 RVA: 0x0008F028 File Offset: 0x0008D228
	public FusionNetPlayer()
	{
		this.PlayerRef = default(PlayerRef);
	}

	// Token: 0x06001983 RID: 6531 RVA: 0x0008F04A File Offset: 0x0008D24A
	public FusionNetPlayer(PlayerRef playerRef)
	{
		this.PlayerRef = playerRef;
	}

	// Token: 0x1700027C RID: 636
	// (get) Token: 0x06001984 RID: 6532 RVA: 0x0008F059 File Offset: 0x0008D259
	private NetworkRunner runner
	{
		get
		{
			return ((NetworkSystemFusion)NetworkSystem.Instance).runner;
		}
	}

	// Token: 0x1700027D RID: 637
	// (get) Token: 0x06001985 RID: 6533 RVA: 0x0008F06C File Offset: 0x0008D26C
	public override bool IsValid
	{
		get
		{
			return this.validPlayer && this.PlayerRef.IsRealPlayer;
		}
	}

	// Token: 0x1700027E RID: 638
	// (get) Token: 0x06001986 RID: 6534 RVA: 0x0008F094 File Offset: 0x0008D294
	public override int ActorNumber
	{
		get
		{
			return this.PlayerRef.PlayerId;
		}
	}

	// Token: 0x1700027F RID: 639
	// (get) Token: 0x06001987 RID: 6535 RVA: 0x0008F0B0 File Offset: 0x0008D2B0
	public override string UserId
	{
		get
		{
			return NetworkSystem.Instance.GetUserID(this.PlayerRef.PlayerId);
		}
	}

	// Token: 0x17000280 RID: 640
	// (get) Token: 0x06001988 RID: 6536 RVA: 0x0008F0D8 File Offset: 0x0008D2D8
	public override bool IsMasterClient
	{
		get
		{
			if (!(this.runner == null))
			{
				return (this.IsLocal && this.runner.IsSharedModeMasterClient) || NetworkSystem.Instance.MasterClient == this;
			}
			return this.PlayerRef == default(PlayerRef);
		}
	}

	// Token: 0x17000281 RID: 641
	// (get) Token: 0x06001989 RID: 6537 RVA: 0x0008F12C File Offset: 0x0008D32C
	public override bool IsLocal
	{
		get
		{
			if (!(this.runner == null))
			{
				return this.PlayerRef == this.runner.LocalPlayer;
			}
			return this.PlayerRef == default(PlayerRef);
		}
	}

	// Token: 0x17000282 RID: 642
	// (get) Token: 0x0600198A RID: 6538 RVA: 0x0008F172 File Offset: 0x0008D372
	public override bool IsNull
	{
		get
		{
			PlayerRef playerRef = this.PlayerRef;
			return false;
		}
	}

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x0600198B RID: 6539 RVA: 0x0008F17C File Offset: 0x0008D37C
	public override string NickName
	{
		get
		{
			return NetworkSystem.Instance.GetNickName(this);
		}
	}

	// Token: 0x17000284 RID: 644
	// (get) Token: 0x0600198C RID: 6540 RVA: 0x0008F18C File Offset: 0x0008D38C
	public override string DefaultName
	{
		get
		{
			if (string.IsNullOrEmpty(this._defaultName))
			{
				this._defaultName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
			}
			return this._defaultName;
		}
	}

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x0600198D RID: 6541 RVA: 0x0008F1D8 File Offset: 0x0008D3D8
	public override bool InRoom
	{
		get
		{
			using (IEnumerator<PlayerRef> enumerator = this.runner.ActivePlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == this.PlayerRef)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x0600198E RID: 6542 RVA: 0x0008F238 File Offset: 0x0008D438
	public override bool Equals(NetPlayer myPlayer, NetPlayer other)
	{
		return myPlayer != null && other != null && ((FusionNetPlayer)myPlayer).PlayerRef.Equals(((FusionNetPlayer)other).PlayerRef);
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x0008F26B File Offset: 0x0008D46B
	public void InitPlayer(PlayerRef player)
	{
		this.PlayerRef = player;
		this.validPlayer = true;
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x0008F27C File Offset: 0x0008D47C
	public override void OnReturned()
	{
		base.OnReturned();
		this.PlayerRef = default(PlayerRef);
		if (this.PlayerRef.PlayerId != -1)
		{
			Debug.LogError("Returned Player to pool but isnt -1, broken");
		}
	}

	// Token: 0x06001991 RID: 6545 RVA: 0x0008F2B9 File Offset: 0x0008D4B9
	public override void OnTaken()
	{
		base.OnTaken();
	}

	// Token: 0x04002459 RID: 9305
	private string _defaultName;

	// Token: 0x0400245A RID: 9306
	private bool validPlayer;
}
