using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaTag;
using UnityEngine;

// Token: 0x02000CF3 RID: 3315
[CreateAssetMenu(menuName = "ScriptableObjects/RoomSystemSettings", order = 2)]
internal class RoomSystemSettings : ScriptableObject
{
	// Token: 0x170007B5 RID: 1973
	// (get) Token: 0x06005245 RID: 21061 RVA: 0x001B18DB File Offset: 0x001AFADB
	public ExpectedUsersDecayTimer ExpectedUsersTimer
	{
		get
		{
			return this.expectedUsersTimer;
		}
	}

	// Token: 0x170007B6 RID: 1974
	// (get) Token: 0x06005246 RID: 21062 RVA: 0x001B18E3 File Offset: 0x001AFAE3
	public TickSystemTimer ResyncNetworkTimeTimer
	{
		get
		{
			return this.resyncNetworkTimeTimer;
		}
	}

	// Token: 0x170007B7 RID: 1975
	// (get) Token: 0x06005247 RID: 21063 RVA: 0x001B18EB File Offset: 0x001AFAEB
	public CallLimiterWithCooldown StatusEffectLimiter
	{
		get
		{
			return this.statusEffectLimiter;
		}
	}

	// Token: 0x170007B8 RID: 1976
	// (get) Token: 0x06005248 RID: 21064 RVA: 0x001B18F3 File Offset: 0x001AFAF3
	public CallLimiterWithCooldown SoundEffectLimiter
	{
		get
		{
			return this.soundEffectLimiter;
		}
	}

	// Token: 0x170007B9 RID: 1977
	// (get) Token: 0x06005249 RID: 21065 RVA: 0x001B18FB File Offset: 0x001AFAFB
	public CallLimiterWithCooldown SoundEffectOtherLimiter
	{
		get
		{
			return this.soundEffectOtherLimiter;
		}
	}

	// Token: 0x170007BA RID: 1978
	// (get) Token: 0x0600524A RID: 21066 RVA: 0x001B1903 File Offset: 0x001AFB03
	public CallLimiterWithCooldown PlayerEffectLimiter
	{
		get
		{
			return this.playerEffectLimiter;
		}
	}

	// Token: 0x170007BB RID: 1979
	// (get) Token: 0x0600524B RID: 21067 RVA: 0x001B190B File Offset: 0x001AFB0B
	public CallLimiterWithCooldown LavaSyncLimiter
	{
		get
		{
			return this.lavaSyncLimiter;
		}
	}

	// Token: 0x170007BC RID: 1980
	// (get) Token: 0x0600524C RID: 21068 RVA: 0x001B1913 File Offset: 0x001AFB13
	public GameObject PlayerImpactEffect
	{
		get
		{
			return this.playerImpactEffect;
		}
	}

	// Token: 0x170007BD RID: 1981
	// (get) Token: 0x0600524D RID: 21069 RVA: 0x001B191B File Offset: 0x001AFB1B
	public List<RoomSystem.PlayerEffectConfig> PlayerEffects
	{
		get
		{
			return this.playerEffects;
		}
	}

	// Token: 0x170007BE RID: 1982
	// (get) Token: 0x0600524E RID: 21070 RVA: 0x001B1923 File Offset: 0x001AFB23
	public int PausedDCTimer
	{
		get
		{
			return this.pausedDCTimer;
		}
	}

	// Token: 0x0600524F RID: 21071 RVA: 0x001B192B File Offset: 0x001AFB2B
	public int GetRoomCount(bool privateRoom, bool sub)
	{
		if (privateRoom)
		{
			if (!sub)
			{
				return this.privateRoomCountZoneModeMapping.GetRoomCount();
			}
			return this.subsPrivateRoomCountZoneModeMapping.GetRoomCount();
		}
		else
		{
			if (!sub)
			{
				return this.publicRoomCountZoneModeMapping.GetRoomCount();
			}
			return this.subsPublicRoomCountZoneModeMapping.GetRoomCount();
		}
	}

	// Token: 0x06005250 RID: 21072 RVA: 0x001B1968 File Offset: 0x001AFB68
	public int GetRoomCount(GTZone zone, GameModeType mode, bool privateRoom, bool sub)
	{
		if (privateRoom)
		{
			if (!sub)
			{
				return this.privateRoomCountZoneModeMapping.GetRoomCount(zone, mode);
			}
			return this.subsPrivateRoomCountZoneModeMapping.GetRoomCount(zone, mode);
		}
		else
		{
			if (!sub)
			{
				return this.publicRoomCountZoneModeMapping.GetRoomCount(zone, mode);
			}
			return this.subsPublicRoomCountZoneModeMapping.GetRoomCount(zone, mode);
		}
	}

	// Token: 0x04006391 RID: 25489
	[SerializeField]
	private ExpectedUsersDecayTimer expectedUsersTimer;

	// Token: 0x04006392 RID: 25490
	[SerializeField]
	private TickSystemTimer resyncNetworkTimeTimer;

	// Token: 0x04006393 RID: 25491
	[SerializeField]
	private CallLimiterWithCooldown statusEffectLimiter;

	// Token: 0x04006394 RID: 25492
	[SerializeField]
	private CallLimiterWithCooldown soundEffectLimiter;

	// Token: 0x04006395 RID: 25493
	[SerializeField]
	private CallLimiterWithCooldown soundEffectOtherLimiter;

	// Token: 0x04006396 RID: 25494
	[SerializeField]
	private CallLimiterWithCooldown playerEffectLimiter;

	// Token: 0x04006397 RID: 25495
	[SerializeField]
	private CallLimiterWithCooldown lavaSyncLimiter;

	// Token: 0x04006398 RID: 25496
	[SerializeField]
	private GameObject playerImpactEffect;

	// Token: 0x04006399 RID: 25497
	[SerializeField]
	private List<RoomSystem.PlayerEffectConfig> playerEffects = new List<RoomSystem.PlayerEffectConfig>();

	// Token: 0x0400639A RID: 25498
	[SerializeField]
	private int pausedDCTimer;

	// Token: 0x0400639B RID: 25499
	[SerializeField]
	private RoomCount publicRoomCountZoneModeMapping;

	// Token: 0x0400639C RID: 25500
	[SerializeField]
	private PrivateRoomCount privateRoomCountZoneModeMapping;

	// Token: 0x0400639D RID: 25501
	[SerializeField]
	private RoomCount subsPublicRoomCountZoneModeMapping;

	// Token: 0x0400639E RID: 25502
	[SerializeField]
	private PrivateRoomCount subsPrivateRoomCountZoneModeMapping;
}
