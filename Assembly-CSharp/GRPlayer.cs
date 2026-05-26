using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020007B7 RID: 1975
public class GRPlayer : MonoBehaviourTick
{
	// Token: 0x17000488 RID: 1160
	// (get) Token: 0x0600323F RID: 12863 RVA: 0x00113DEE File Offset: 0x00111FEE
	public GRPlayer.GRPlayerState State
	{
		get
		{
			return this.state;
		}
	}

	// Token: 0x17000489 RID: 1161
	// (get) Token: 0x06003240 RID: 12864 RVA: 0x00113DF6 File Offset: 0x00111FF6
	public int Juice
	{
		get
		{
			return this.playerJuice;
		}
	}

	// Token: 0x1700048A RID: 1162
	// (get) Token: 0x06003241 RID: 12865 RVA: 0x00113DFE File Offset: 0x00111FFE
	// (set) Token: 0x06003242 RID: 12866 RVA: 0x00113E06 File Offset: 0x00112006
	public int ShiftCreditCapIncreases { get; set; }

	// Token: 0x1700048B RID: 1163
	// (get) Token: 0x06003243 RID: 12867 RVA: 0x00113E0F File Offset: 0x0011200F
	// (set) Token: 0x06003244 RID: 12868 RVA: 0x00113E17 File Offset: 0x00112017
	public int ShiftCreditCapIncreasesMax { get; set; }

	// Token: 0x1700048C RID: 1164
	// (get) Token: 0x06003245 RID: 12869 RVA: 0x00113E20 File Offset: 0x00112020
	public int ShiftCredits
	{
		get
		{
			return this.shiftCreditCache;
		}
	}

	// Token: 0x06003246 RID: 12870 RVA: 0x00113E28 File Offset: 0x00112028
	public bool HasXRayVision()
	{
		return this.xRayVisionRefCount > 0;
	}

	// Token: 0x1700048D RID: 1165
	// (get) Token: 0x06003247 RID: 12871 RVA: 0x00113E33 File Offset: 0x00112033
	public int MaxHp
	{
		get
		{
			return this.maxHp;
		}
	}

	// Token: 0x1700048E RID: 1166
	// (get) Token: 0x06003248 RID: 12872 RVA: 0x00113E3B File Offset: 0x0011203B
	public int MaxShieldHp
	{
		get
		{
			return this.maxShieldHp;
		}
	}

	// Token: 0x1700048F RID: 1167
	// (get) Token: 0x06003249 RID: 12873 RVA: 0x00113E43 File Offset: 0x00112043
	public int Hp
	{
		get
		{
			return this.hp;
		}
	}

	// Token: 0x17000490 RID: 1168
	// (get) Token: 0x0600324A RID: 12874 RVA: 0x00113E4B File Offset: 0x0011204B
	public int ShieldHp
	{
		get
		{
			return this.shieldHp;
		}
	}

	// Token: 0x17000491 RID: 1169
	// (get) Token: 0x0600324B RID: 12875 RVA: 0x00113E53 File Offset: 0x00112053
	public int ShieldFlags
	{
		get
		{
			return this.shieldFlags;
		}
	}

	// Token: 0x17000492 RID: 1170
	// (get) Token: 0x0600324C RID: 12876 RVA: 0x00113E5B File Offset: 0x0011205B
	public bool InStealthMode
	{
		get
		{
			return this.inStealthMode;
		}
	}

	// Token: 0x17000493 RID: 1171
	// (get) Token: 0x0600324D RID: 12877 RVA: 0x00113E63 File Offset: 0x00112063
	public VRRig MyRig
	{
		get
		{
			return this.vrRig;
		}
	}

	// Token: 0x17000494 RID: 1172
	// (get) Token: 0x0600324E RID: 12878 RVA: 0x00113E6B File Offset: 0x0011206B
	// (set) Token: 0x0600324F RID: 12879 RVA: 0x00113E73 File Offset: 0x00112073
	public float ShiftPlayTime
	{
		get
		{
			return this.shiftPlayTime;
		}
		set
		{
			this.shiftPlayTime = value;
		}
	}

	// Token: 0x17000495 RID: 1173
	// (get) Token: 0x06003250 RID: 12880 RVA: 0x00113E7C File Offset: 0x0011207C
	// (set) Token: 0x06003251 RID: 12881 RVA: 0x00113E84 File Offset: 0x00112084
	public int LastShiftCut
	{
		get
		{
			return this.lastShiftCut;
		}
		set
		{
			this.lastShiftCut = value;
		}
	}

	// Token: 0x17000496 RID: 1174
	// (get) Token: 0x06003252 RID: 12882 RVA: 0x00113E8D File Offset: 0x0011208D
	// (set) Token: 0x06003253 RID: 12883 RVA: 0x00113E95 File Offset: 0x00112095
	public GRPlayer.ProgressionData CurrentProgression
	{
		get
		{
			return this.currentProgression;
		}
		set
		{
			this.currentProgression = value;
		}
	}

	// Token: 0x06003254 RID: 12884 RVA: 0x00113EA0 File Offset: 0x001120A0
	private void Awake()
	{
		this.vrRig = base.GetComponent<VRRig>();
		this.lowHealthVisualPropertyBlock = new MaterialPropertyBlock();
		this.damageEffects = GTPlayer.Instance.mainCamera.GetComponent<GRPlayerDamageEffects>();
		this.lowHealthTintPropertyId = Shader.PropertyToID("_TintColor");
		this.isEmployee = false;
		this.SetHp(this.maxHp);
		this.SetShieldHp(0);
		this.state = GRPlayer.GRPlayerState.Alive;
		this.RefreshDamageVignetteVisual();
		this.shieldHeadVisual.gameObject.SetActive(false);
		this.shieldBodyVisual.gameObject.SetActive(false);
		this.shieldGameLight = this.shieldBodyVisual.gameObject.GetComponentInChildren<GameLight>(true);
		this.requestCollectItemLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestChargeToolLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestDepositCurrencyLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestShiftStartLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestToolPurchaseStationLimiter = new CallLimiter(25, 1f, 0.5f);
		this.applyEnemyHitLimiter = new CallLimiter(25, 1f, 0.5f);
		this.reportLocalHitLimiter = new CallLimiter(25, 1f, 0.5f);
		this.reportBreakableBrokenLimiter = new CallLimiter(25, 1f, 0.5f);
		this.playerStateChangeLimiter = new CallLimiter(25, 1f, 0.5f);
		this.promotionBotLimiter = new CallLimiter(25, 1f, 0.5f);
		this.progressionBroadcastLimiter = new CallLimiter(25, 1f, 0.5f);
		this.scoreboardPageLimiter = new CallLimiter(25, 1f, 0.5f);
		this.fireShieldLimiter = new CallLimiter(25, 1f, 0.5f);
		this.shuttleData = new GRPlayer.ShuttleData();
		this.lastLeftWithBadgeAttachedTime = -10000.0;
	}

	// Token: 0x06003255 RID: 12885 RVA: 0x00114090 File Offset: 0x00112290
	private void Start()
	{
		if (this.gamePlayer != null && this.gamePlayer.IsLocal())
		{
			this.LoadMyProgression();
			ProgressionManager.Instance.OnGetShiftCredit += this.OnShiftCreditChanged;
			ProgressionManager.Instance.OnGetShiftCreditCapData += this.OnShiftCreditCapChanged;
			this.soak = new GhostReactorSoak();
			this.soak.Setup(this);
		}
		else
		{
			this.currentProgression = new GRPlayer.ProgressionData
			{
				points = 0,
				redeemedPoints = 0
			};
		}
		if (ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.OnGetShiftCredit += this.OnShiftCreditChanged;
			ProgressionManager.Instance.OnGetShiftCreditCapData += this.OnShiftCreditCapChanged;
		}
	}

	// Token: 0x06003256 RID: 12886 RVA: 0x0011415B File Offset: 0x0011235B
	private new void OnDisable()
	{
		this.Reset();
	}

	// Token: 0x06003257 RID: 12887 RVA: 0x00114164 File Offset: 0x00112364
	public void Reset()
	{
		this.SetHp(this.maxHp);
		this.SetShieldHp(0);
		this.state = GRPlayer.GRPlayerState.Alive;
		this.RefreshDamageVignetteVisual();
		this.RefreshPlayerVisuals();
		for (int i = 0; i < 8; i++)
		{
			this.synchronizedSessionStats[i] = 0f;
		}
	}

	// Token: 0x06003258 RID: 12888 RVA: 0x001141B0 File Offset: 0x001123B0
	private void SetHp(int newHp)
	{
		this.hp = Mathf.Max(newHp, 0);
	}

	// Token: 0x06003259 RID: 12889 RVA: 0x001141BF File Offset: 0x001123BF
	private void SetShieldHp(int newShieldHp)
	{
		this.shieldHp = Mathf.Max(newShieldHp, 0);
	}

	// Token: 0x0600325A RID: 12890 RVA: 0x001141D0 File Offset: 0x001123D0
	public void OnShiftCreditCapChanged(string targetMothershipId, int newCap, int newCapMax)
	{
		if (this.mothershipId != null && targetMothershipId == this.mothershipId)
		{
			if (this.gamePlayer.IsLocal() && (newCap != this.ShiftCreditCapIncreases || newCapMax != this.ShiftCreditCapIncreasesMax) && GhostReactor.instance != null)
			{
				GhostReactor.instance.grManager.RefreshShiftCredit();
			}
			this.ShiftCreditCapIncreases = newCap;
			this.ShiftCreditCapIncreasesMax = newCapMax;
		}
	}

	// Token: 0x0600325B RID: 12891 RVA: 0x0011423C File Offset: 0x0011243C
	public void OnShiftCreditChanged(string targetMothershipId, int newShiftCredits)
	{
		if (this.mothershipId != null && targetMothershipId == this.mothershipId)
		{
			int num = this.shiftCreditCache;
			this.shiftCreditCache = newShiftCredits;
			if (GhostReactor.instance != null && this.gamePlayer.IsLocal() && num != newShiftCredits && GhostReactor.instance != null)
			{
				if (GhostReactor.instance.promotionBot != null)
				{
					GhostReactor.instance.promotionBot.Refresh();
				}
				if (GhostReactor.instance.grManager != null)
				{
					GhostReactor.instance.grManager.RefreshShiftCredit();
				}
			}
		}
		if (GhostReactor.instance != null)
		{
			GhostReactor.instance.RefreshScoreboards();
		}
	}

	// Token: 0x0600325C RID: 12892 RVA: 0x001142F4 File Offset: 0x001124F4
	public void OnShiftCreditCapData(string targetMothershipId, int shiftCreditCapNumberOfIncreases, int shiftCreditMaxNumberOfIncreases)
	{
		if (this.mothershipId != null)
		{
			targetMothershipId == this.mothershipId;
		}
	}

	// Token: 0x0600325D RID: 12893 RVA: 0x0011430B File Offset: 0x0011250B
	public void SubtractShiftCredit(int shiftCreditDelta)
	{
		if (this.gamePlayer.IsLocal())
		{
			ProgressionManager.Instance.SubtractShiftCredit(shiftCreditDelta);
		}
	}

	// Token: 0x0600325E RID: 12894 RVA: 0x00114328 File Offset: 0x00112528
	public void OnPlayerHit(Vector3 hitPosition, Vector3 hitImpulse, GhostReactorManager manager, GameEntityId hitByEntityId)
	{
		GameEntity gameEntity = manager.gameEntityManager.GetGameEntity(hitByEntityId);
		int num = 1;
		if (this.gamePlayer.IsLocal())
		{
			GTPlayer instance = GTPlayer.Instance;
			float magnitude = hitImpulse.magnitude;
			if (magnitude > 0f)
			{
				instance.ApplyKnockback(hitImpulse / magnitude, magnitude, true);
			}
		}
		if (this.State == GRPlayer.GRPlayerState.Alive)
		{
			if (this.shieldHp > 0)
			{
				if (gameEntity != null)
				{
					GRAttributes component = gameEntity.GetComponent<GRAttributes>();
					if (component != null)
					{
						num = component.CalculateFinalValueForAttribute(GRAttributeType.PlayerShieldDamage);
					}
				}
				this.SetShieldHp(this.shieldHp - num);
				if (this.shieldHp > 0)
				{
					if (this.shieldDamagedSound != null)
					{
						this.audioSource.PlayOneShot(this.shieldDamagedSound, this.shieldDamagedVolume);
					}
					this.shieldDamagedEffect.Play();
				}
				else
				{
					if (this.shieldDestroyedSound != null)
					{
						this.audioSource.PlayOneShot(this.shieldDestroyedSound, this.shieldDestroyedVolume);
					}
					this.shieldDestroyedEffect.Play();
				}
				this.RefreshPlayerVisuals();
				return;
			}
			if (gameEntity != null)
			{
				GRAttributes component2 = gameEntity.GetComponent<GRAttributes>();
				if (component2 != null)
				{
					num = component2.CalculateFinalValueForAttribute(GRAttributeType.PlayerDamage);
				}
			}
			Debug.Log(string.Format("GRPlayer OnPlayerHit, hit by: {0} damage: {1}, state: {2}, hp: {3}, shield hp: {4}", new object[]
			{
				hitByEntityId.index,
				num,
				this.state,
				this.hp,
				this.shieldHp
			}));
			this.PlayHitFx(hitPosition);
			this.SetHp(this.hp - num);
			this.RefreshDamageVignetteVisual();
			if (this.hp <= 0)
			{
				this.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, manager);
			}
		}
	}

	// Token: 0x0600325F RID: 12895 RVA: 0x001144DB File Offset: 0x001126DB
	public void OnPlayerRevive(GhostReactorManager manager)
	{
		this.SetHp(this.maxHp);
		this.RefreshDamageVignetteVisual();
		this.ChangePlayerState(GRPlayer.GRPlayerState.Alive, manager);
	}

	// Token: 0x06003260 RID: 12896 RVA: 0x001144F8 File Offset: 0x001126F8
	public void ChangePlayerState(GRPlayer.GRPlayerState newState, GhostReactorManager manager)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			newState = GRPlayer.GRPlayerState.Alive;
		}
		if (this.state == newState)
		{
			return;
		}
		this.state = newState;
		GRPlayer.GRPlayerState grplayerState = this.state;
		if (grplayerState != GRPlayer.GRPlayerState.Alive)
		{
			if (grplayerState == GRPlayer.GRPlayerState.Ghost)
			{
				this.SetHp(0);
				this.SetShieldHp(0);
				this.RefreshDamageVignetteVisual();
				if (this.playerTurnedGhostEffect != null)
				{
					this.playerTurnedGhostEffect.Play();
				}
				this.playerTurnedGhostSoundBank.Play();
				manager.ReportPlayerDeath(this);
				this.IncrementDeaths(1);
			}
		}
		else
		{
			this.SetHp(this.maxHp);
			this.RefreshDamageVignetteVisual();
			this.IncrementRevives(1);
			if (this.playerRevivedEffect != null)
			{
				this.playerRevivedEffect.Play();
			}
			if (this.audioSource != null && this.playerRevivedSound != null)
			{
				this.audioSource.PlayOneShot(this.playerRevivedSound, this.playerRevivedVolume);
			}
		}
		this.RefreshPlayerVisuals();
		if (this.vrRig.isLocal)
		{
			this.vrRigs.Clear();
			VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
			for (int i = 0; i < this.vrRigs.Count; i++)
			{
				this.vrRigs[i].GetComponent<GRPlayer>().RefreshPlayerVisuals();
			}
		}
	}

	// Token: 0x06003261 RID: 12897 RVA: 0x00114640 File Offset: 0x00112840
	public void RefreshPlayerVisuals()
	{
		this.RefreshDamageVignetteVisual();
		GRPlayer.GRPlayerState grplayerState = this.state;
		if (grplayerState == GRPlayer.GRPlayerState.Alive)
		{
			this.gamePlayer.DisableGrabbing(false);
			if (this.badge != null)
			{
				this.badge.UnHide();
			}
			this.vrRig.ChangeMaterialLocal(0);
			this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Default);
			this.vrRig.SetInvisibleToLocalPlayer(false);
			if (this.vrRig.isLocal)
			{
				CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
				GameLightingManager.instance.SetDesaturateAndTintEnabled(false, Color.black);
				Color ambientLightDynamic = Color.black;
				GhostReactor instance = GhostReactor.instance;
				if (instance != null && instance.zone != GTZone.customMaps)
				{
					ambientLightDynamic = instance.GetCurrLevelGenConfig().ambientLight;
				}
				GameLightingManager.instance.SetAmbientLightDynamic(ambientLightDynamic);
			}
			if (this.shieldHp > 0)
			{
				this.shieldHeadVisual.gameObject.SetActive(true);
				this.shieldBodyVisual.gameObject.SetActive(true);
				Color value = this.shieldColorNormal;
				if ((this.shieldFlags & 1) != 0)
				{
					value = this.shieldColorLight;
				}
				else if ((this.shieldFlags & 2) != 0)
				{
					value = this.shieldColorStealth;
				}
				else if ((this.shieldFlags & 4) != 0)
				{
					value = this.shieldColorHeal;
				}
				Renderer component = this.shieldBodyVisual.GetComponent<Renderer>();
				if (component != null)
				{
					component.material.SetColor("_BaseColor", value);
				}
				Renderer component2 = this.shieldHeadVisual.GetComponent<Renderer>();
				if (component2 != null)
				{
					component2.material.SetColor("_BaseColor", value);
				}
			}
			else
			{
				this.shieldHeadVisual.gameObject.SetActive(false);
				this.shieldBodyVisual.gameObject.SetActive(false);
			}
			this.shieldGameLight.gameObject.SetActive((this.shieldFlags & 1) != 0);
			return;
		}
		if (grplayerState != GRPlayer.GRPlayerState.Ghost)
		{
			return;
		}
		if (this.vrRig.isLocal)
		{
			this.gamePlayer.RequestDropAllSnapped();
		}
		this.gamePlayer.DisableGrabbing(true);
		this.shieldHeadVisual.gameObject.SetActive(false);
		this.shieldBodyVisual.gameObject.SetActive(false);
		this.shieldGameLight.gameObject.SetActive(false);
		if (this.badge != null)
		{
			this.badge.Hide();
		}
		if (this.vrRig.isLocal)
		{
			GamePlayerLocal.instance.OnUpdateInteract();
			this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Skeleton);
			this.vrRig.ChangeMaterialLocal(13);
			this.vrRig.SetInvisibleToLocalPlayer(false);
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(true);
			GameLightingManager.instance.SetDesaturateAndTintEnabled(true, this.deathTintColor);
			GameLightingManager.instance.SetAmbientLightDynamic(this.deathAmbientLightColor);
			return;
		}
		if (VRRigCache.Instance.localRig.GetComponent<GRPlayer>().State == GRPlayer.GRPlayerState.Ghost)
		{
			this.vrRig.ChangeMaterialLocal(13);
			this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Skeleton);
			this.vrRig.SetInvisibleToLocalPlayer(false);
			return;
		}
		this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Invisible);
		this.vrRig.SetInvisibleToLocalPlayer(true);
	}

	// Token: 0x06003262 RID: 12898 RVA: 0x0011495C File Offset: 0x00112B5C
	public static GRPlayer Get(int actorNumber)
	{
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(actorNumber, out gamePlayer))
		{
			return null;
		}
		return gamePlayer.GetComponent<GRPlayer>();
	}

	// Token: 0x06003263 RID: 12899 RVA: 0x0011497B File Offset: 0x00112B7B
	public static GRPlayer Get(NetPlayer player)
	{
		if (player == null)
		{
			return null;
		}
		return GRPlayer.Get(player.ActorNumber);
	}

	// Token: 0x06003264 RID: 12900 RVA: 0x0011498D File Offset: 0x00112B8D
	public static GRPlayer Get(VRRig vrRig)
	{
		if (!(vrRig != null))
		{
			return null;
		}
		return vrRig.GetComponent<GRPlayer>();
	}

	// Token: 0x06003265 RID: 12901 RVA: 0x001149A0 File Offset: 0x00112BA0
	public static GRPlayer GetLocal()
	{
		return GRPlayer.Get(VRRig.LocalRig);
	}

	// Token: 0x06003266 RID: 12902 RVA: 0x001149AC File Offset: 0x00112BAC
	public void AttachBadge(GRBadge grBadge)
	{
		this.badge = grBadge;
		this.badge.transform.SetParent(this.badgeBodyAnchor);
		this.badge.GetComponent<Rigidbody>().isKinematic = true;
		this.badge.StartRetracting();
	}

	// Token: 0x06003267 RID: 12903 RVA: 0x001149E7 File Offset: 0x00112BE7
	public bool CanActivateShield(int shieldHitPoints)
	{
		return this.state == GRPlayer.GRPlayerState.Alive && shieldHitPoints > 0;
	}

	// Token: 0x06003268 RID: 12904 RVA: 0x001149F8 File Offset: 0x00112BF8
	public bool TryActivateShield(int shieldHitpoints, int shieldFlags)
	{
		if (this.state == GRPlayer.GRPlayerState.Alive)
		{
			if (this.shieldHp <= 0 && this.shieldActivatedSound != null)
			{
				this.audioSource.PlayOneShot(this.shieldActivatedSound, this.shieldActivatedVolume);
			}
			this.SetShieldHp(Mathf.Min(shieldHitpoints, this.maxShieldHp));
			this.shieldFlags = shieldFlags;
			this.inStealthMode = ((shieldFlags & 2) != 0);
			if (this.inStealthMode)
			{
				if (this.damageEffects.stealthModeVisualRenderer != null)
				{
					this.damageEffects.stealthModeVisualRenderer.gameObject.SetActive(true);
				}
				this.shieldStealthModeEndTime = Time.timeAsDouble + (double)this.shieldStealthModeDuration;
			}
			if ((shieldFlags & 4) != 0)
			{
				this.SetHp(this.maxHp);
			}
			this.RefreshPlayerVisuals();
			return true;
		}
		return false;
	}

	// Token: 0x06003269 RID: 12905 RVA: 0x00114AC1 File Offset: 0x00112CC1
	public void ClearStealthMode()
	{
		this.inStealthMode = false;
		if (this.damageEffects.stealthModeVisualRenderer != null)
		{
			this.damageEffects.stealthModeVisualRenderer.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600326A RID: 12906 RVA: 0x00114AF4 File Offset: 0x00112CF4
	public void SerializeNetworkState(BinaryWriter writer, NetPlayer player)
	{
		writer.Write((byte)this.state);
		writer.Write(this.hp);
		writer.Write(this.shieldHp);
		writer.Write(this.shiftJoinTime);
		writer.Write(this.isEmployee ? 1 : 0);
		writer.Write(this.CurrentProgression.points);
		writer.Write(this.CurrentProgression.redeemedPoints);
		writer.Write(this.dropPodLevel);
		writer.Write(this.dropPodChasisLevel);
		for (int i = 0; i < 8; i++)
		{
			writer.Write(this.synchronizedSessionStats[i]);
		}
	}

	// Token: 0x0600326B RID: 12907 RVA: 0x00114B9C File Offset: 0x00112D9C
	public static void DeserializeNetworkStateAndBurn(BinaryReader reader, GRPlayer player, GhostReactorManager grManager)
	{
		GRPlayer.GRPlayerState newState = (GRPlayer.GRPlayerState)reader.ReadByte();
		int num = reader.ReadInt32();
		int num2 = reader.ReadInt32();
		double num3 = reader.ReadDouble();
		bool flag = reader.ReadByte() > 0;
		int points = reader.ReadInt32();
		int redeemedPoints = reader.ReadInt32();
		int num4 = reader.ReadInt32();
		int num5 = reader.ReadInt32();
		for (int i = 0; i < 8; i++)
		{
			player.synchronizedSessionStats[i] = reader.ReadSingle();
		}
		if (player != null)
		{
			player.SetHp(num);
			player.SetShieldHp(num2);
			player.isEmployee = flag;
			player.ChangePlayerState(newState, grManager);
			player.RefreshPlayerVisuals();
			if (!player.gamePlayer.IsLocal())
			{
				player.SetProgressionData(points, redeemedPoints, false);
				player.dropPodLevel = num4;
				player.dropPodChasisLevel = num5;
			}
			if (double.IsNaN(num3) || double.IsInfinity(num3))
			{
				player.shiftJoinTime = PhotonNetwork.Time;
			}
			else
			{
				player.shiftJoinTime = Math.Min(num3, PhotonNetwork.Time);
			}
		}
		if (grManager != null)
		{
			grManager.SendMothershipId();
		}
	}

	// Token: 0x0600326C RID: 12908 RVA: 0x00114CA4 File Offset: 0x00112EA4
	public void PlayHitFx(Vector3 attackLocation)
	{
		if (this.playerDamageAudioSource != null)
		{
			this.playerDamageAudioSource.PlayOneShot(this.playerDamageSound, this.playerDamageVolume);
		}
		if (this.bodyCenter != null)
		{
			Vector3 vector = attackLocation - this.bodyCenter.position;
			vector.y = 0f;
			Vector3 b = vector.normalized * this.playerDamageOffsetDist;
			if (this.playerDamageEffect != null)
			{
				this.playerDamageEffect.transform.position = this.bodyCenter.position + b;
				this.playerDamageEffect.Play();
			}
			if (this.vrRig.isLocal)
			{
				Vector3 normalized = Vector3.ProjectOnPlane(GTPlayer.Instance.mainCamera.transform.forward, Vector3.up).normalized;
				vector = Vector3.ProjectOnPlane(vector, Vector3.up).normalized;
				float num = Vector3.SignedAngle(normalized, vector, Vector3.up);
				this.damageEffects.radialDamageEffect.transform.localRotation = Quaternion.Euler(0f, 0f, -num);
				this.damageEffects.radialDamageEffect.Play();
			}
		}
		if (this.gamePlayer == GamePlayerLocal.instance.gamePlayer)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength, 0.5f);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength, 0.5f);
		}
	}

	// Token: 0x0600326D RID: 12909 RVA: 0x00114E28 File Offset: 0x00113028
	public void SendGameStartedTelemetry(float timeIntoShift, bool wasPlayerInAtStart, int currentFloor)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		string titleNameFromLevel = GhostReactorProgression.GetTitleNameFromLevel(GhostReactorProgression.GetTitleLevel(this.CurrentProgression.redeemedPoints));
		GorillaTelemetry.GhostReactorShiftStart(this.gameId, this.ShiftCredits, timeIntoShift, wasPlayerInAtStart, this.vrRigs.Count + 1, currentFloor, titleNameFromLevel);
		this.wasPlayerInAtShiftStart = wasPlayerInAtStart;
		this.ResetGameTelemetryTracking();
	}

	// Token: 0x0600326E RID: 12910 RVA: 0x00114E98 File Offset: 0x00113098
	public void SendGameEndedTelemetry(bool isShiftActuallyEnding, ZoneClearReason zoneClearReason)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		GorillaTelemetry.GhostReactorGameEnd(this.gameId, this.ShiftCredits, this.totalCoresCollectedByPlayer, this.totalCoresCollectedByGroup, this.totalCoresSpentByPlayer, this.totalCoresSpentByGroup, this.totalGatesUnlocked, this.totalDeaths, this.totalItemsPurchased, this.lastShiftCut, isShiftActuallyEnding, this.timeIntoShiftAtJoin, (float)(PhotonNetwork.Time - (double)this.gameStartTime), this.wasPlayerInAtShiftStart, zoneClearReason, this.maxNumberOfPlayersInShift, this.vrRigs.Count + 1, this.totalItemTypesHeldThisShift, this.totalRevives, this.numShiftsPlayed);
		this.isFirstShift = true;
	}

	// Token: 0x0600326F RID: 12911 RVA: 0x00114F4C File Offset: 0x0011314C
	public void SendFloorStartedTelemetry(float timeIntoShift, bool wasPlayerInAtStart, int currentFloor, string floorPreset, string floorModifier)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		string titleNameFromLevel = GhostReactorProgression.GetTitleNameFromLevel(GhostReactorProgression.GetTitleLevel(this.CurrentProgression.redeemedPoints));
		GorillaTelemetry.GhostReactorFloorStart(this.gameId, this.ShiftCredits, timeIntoShift, wasPlayerInAtStart, this.vrRigs.Count + 1, titleNameFromLevel, currentFloor, floorPreset, floorModifier);
		this.wasPlayerInAtShiftStart = wasPlayerInAtStart;
	}

	// Token: 0x06003270 RID: 12912 RVA: 0x00114FB8 File Offset: 0x001131B8
	public void SendFloorEndedTelemetry(bool isShiftActuallyEnding, float shiftStartTime, ZoneClearReason zoneClearReason, int currentFloor, string floorPreset, string floorModifier, bool objectivesCompleted, string section, int xpGained)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		GorillaTelemetry.GhostReactorFloorComplete(this.gameId, this.ShiftCredits, this.coresCollectedByPlayer, this.coresCollectedByGroup, this.coresSpentByPlayer, this.coresSpentByGroup, this.gatesUnlocked, this.deaths, this.itemsPurchased, this.lastShiftCut, isShiftActuallyEnding, this.timeIntoShiftAtJoin, (float)(PhotonNetwork.Time - (double)(this.timeIntoShiftAtJoin + shiftStartTime)), this.wasPlayerInAtShiftStart, zoneClearReason, this.maxNumberOfPlayersInShift, this.vrRigs.Count + 1, this.itemTypesHeldThisShift, this.revives, currentFloor, floorPreset, floorModifier, this.sentientCoresCollected, objectivesCompleted, section, xpGained);
	}

	// Token: 0x06003271 RID: 12913 RVA: 0x00115070 File Offset: 0x00113270
	public void SendToolPurchasedTelemetry(string toolName, int toolLevel, int coresSpent, int shinyRocksSpent)
	{
		int floor = -1;
		string preset = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			floor = instance.GetDepthLevel();
			preset = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorToolPurchased(this.gameId, toolName, toolLevel, coresSpent, shinyRocksSpent, floor, preset);
	}

	// Token: 0x06003272 RID: 12914 RVA: 0x001150C4 File Offset: 0x001132C4
	public void SendRankUpTelemetry(string newRank)
	{
		int floor = -1;
		string preset = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			floor = instance.GetDepthLevel();
			preset = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorRankUp(this.gameId, newRank, floor, preset);
	}

	// Token: 0x06003273 RID: 12915 RVA: 0x00115114 File Offset: 0x00113314
	public void SendToolUpgradeTelemetry(string upgradeType, string toolName, int newLevel, int juiceSpent, int griftSpent, int coresSpent)
	{
		int floor = -1;
		string preset = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			floor = instance.GetDepthLevel();
			preset = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorToolUpgrade(this.gameId, upgradeType, toolName, newLevel, juiceSpent, griftSpent, coresSpent, floor, preset);
	}

	// Token: 0x06003274 RID: 12916 RVA: 0x0011516C File Offset: 0x0011336C
	public void SendSeedDepositedTelemetry(string unlockTime, int seedsInQueue)
	{
		int floor = -1;
		string preset = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			floor = instance.GetDepthLevel();
			preset = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorChaosSeedStart(this.gameId, unlockTime, seedsInQueue, floor, preset);
	}

	// Token: 0x06003275 RID: 12917 RVA: 0x001151BC File Offset: 0x001133BC
	public void SendJuiceCollectedTelemetry(int juiceCollected, int coresProcessedByOverdrive)
	{
		GorillaTelemetry.GhostReactorChaosJuiceCollected(this.gameId, juiceCollected, coresProcessedByOverdrive);
	}

	// Token: 0x06003276 RID: 12918 RVA: 0x001151CC File Offset: 0x001133CC
	public void SendOverdrivePurchasedTelemetry(int shinyRocksUsed, int seedsInQueue)
	{
		int floor = -1;
		string preset = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			floor = instance.GetDepthLevel();
			preset = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorOverdrivePurchased(this.gameId, shinyRocksUsed, seedsInQueue, floor, preset);
	}

	// Token: 0x06003277 RID: 12919 RVA: 0x0011521C File Offset: 0x0011341C
	public void SendPodUpgradeTelemetry(string toolName, int level, int shinyRocksSpent, int juiceSpent)
	{
		GorillaTelemetry.GhostReactorPodUpgradePurchased(this.gameId, toolName, level, shinyRocksSpent, juiceSpent);
	}

	// Token: 0x06003278 RID: 12920 RVA: 0x00115230 File Offset: 0x00113430
	public void SendCreditsRefilledTelemetry(int shinyRocksSpent, int finalCredits)
	{
		int floor = -1;
		string preset = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			floor = instance.GetDepthLevel();
			preset = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorCreditsRefillPurchased(this.gameId, shinyRocksSpent, finalCredits, floor, preset);
	}

	// Token: 0x06003279 RID: 12921 RVA: 0x00115280 File Offset: 0x00113480
	public void ResetTelemetryTracking(string newGameId, float timeSinceShiftStart)
	{
		this.gameId = newGameId;
		this.coresCollectedByPlayer = 0;
		this.coresCollectedByGroup = 0;
		this.gatesUnlocked = 0;
		this.deaths = 0;
		this.caughtByAnomaly = false;
		this.itemsPurchased = new List<string>();
		this.levelsUnlocked = new List<string>();
		this.sentientCoresCollected = 0;
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		this.maxNumberOfPlayersInShift = this.vrRigs.Count + 1;
		this.timeIntoShiftAtJoin = timeSinceShiftStart;
		this.itemsHeldThisShift.Clear();
		this.itemTypesHeldThisShift.Clear();
	}

	// Token: 0x0600327A RID: 12922 RVA: 0x00115320 File Offset: 0x00113520
	public void ResetGameTelemetryTracking()
	{
		this.totalCoresCollectedByPlayer = 0;
		this.totalCoresCollectedByGroup = 0;
		this.totalGatesUnlocked = 0;
		this.totalDeaths = 0;
		this.totalItemsPurchased = new List<string>();
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		this.maxNumberOfPlayersIngame = this.vrRigs.Count + 1;
		this.totalItemsHeldThisShift.Clear();
		this.totalItemTypesHeldThisShift.Clear();
		this.numShiftsPlayed = 0;
		this.isFirstShift = false;
	}

	// Token: 0x0600327B RID: 12923 RVA: 0x001153A6 File Offset: 0x001135A6
	public void IncrementCoresCollectedPlayer(int coreValue)
	{
		this.totalCoresCollectedByPlayer += coreValue;
		this.coresCollectedByPlayer += coreValue;
	}

	// Token: 0x0600327C RID: 12924 RVA: 0x001153C4 File Offset: 0x001135C4
	public void IncrementCoresCollectedGroup(int coreValue)
	{
		this.totalCoresCollectedByGroup += coreValue;
		this.coresCollectedByGroup += coreValue;
	}

	// Token: 0x0600327D RID: 12925 RVA: 0x001153E2 File Offset: 0x001135E2
	public void IncrementCoresSpentPlayer(int coreValue)
	{
		this.totalCoresSpentByPlayer += coreValue;
		this.coresSpentByPlayer += coreValue;
	}

	// Token: 0x0600327E RID: 12926 RVA: 0x00115400 File Offset: 0x00113600
	public void IncrementCoresSpentGroup(int coreValue)
	{
		this.totalCoresSpentByGroup += coreValue;
		this.coresSpentByGroup += coreValue;
	}

	// Token: 0x0600327F RID: 12927 RVA: 0x0011541E File Offset: 0x0011361E
	public void IncrementChaosSeedsCollected(int numSeeds)
	{
		this.sentientCoresCollected += numSeeds;
	}

	// Token: 0x06003280 RID: 12928 RVA: 0x0011542E File Offset: 0x0011362E
	public void IncrementGatesUnlocked(int numGatesUnlocked)
	{
		this.gatesUnlocked += numGatesUnlocked;
		this.totalGatesUnlocked += numGatesUnlocked;
	}

	// Token: 0x06003281 RID: 12929 RVA: 0x0011544C File Offset: 0x0011364C
	public void IncrementDeaths(int numDeaths)
	{
		this.deaths += numDeaths;
		this.totalDeaths += numDeaths;
	}

	// Token: 0x06003282 RID: 12930 RVA: 0x0011546A File Offset: 0x0011366A
	public void IncrementRevives(int numRevives)
	{
		this.revives += numRevives;
		this.totalRevives += numRevives;
	}

	// Token: 0x06003283 RID: 12931 RVA: 0x00115488 File Offset: 0x00113688
	public void IncrementShiftsPlayed(int numShifts)
	{
		this.numShiftsPlayed += numShifts;
	}

	// Token: 0x06003284 RID: 12932 RVA: 0x00115498 File Offset: 0x00113698
	public void AddItemPurchased(string newItemPurchased)
	{
		this.itemsPurchased.Add(newItemPurchased);
		this.totalItemsPurchased.Add(newItemPurchased);
	}

	// Token: 0x06003285 RID: 12933 RVA: 0x001154B4 File Offset: 0x001136B4
	public void GrabbedItem(GameEntityId id, string itemName)
	{
		if (this.itemsHeldThisShift.Contains(id))
		{
			return;
		}
		this.itemsHeldThisShift.Add(id);
		if (this.itemTypesHeldThisShift.ContainsKey(itemName))
		{
			this.itemTypesHeldThisShift[itemName] = this.itemTypesHeldThisShift[itemName] + 1;
		}
		else
		{
			this.itemTypesHeldThisShift[itemName] = 1;
		}
		if (this.totalItemsHeldThisShift.Contains(id))
		{
			return;
		}
		this.totalItemsHeldThisShift.Add(id);
		if (this.totalItemTypesHeldThisShift.ContainsKey(itemName))
		{
			this.totalItemTypesHeldThisShift[itemName] = this.totalItemTypesHeldThisShift[itemName] + 1;
			return;
		}
		this.totalItemTypesHeldThisShift[itemName] = 1;
	}

	// Token: 0x06003286 RID: 12934 RVA: 0x00115568 File Offset: 0x00113768
	public GRShuttle GetAssignedShuttle(bool isOnDrillovator)
	{
		GhostReactor instance = GhostReactor.instance;
		GRShuttle drillShuttleForPlayer = GRElevatorManager._instance.GetDrillShuttleForPlayer(this.gamePlayer.rig.OwningNetPlayer.ActorNumber);
		GRShuttle stagingShuttleForPlayer = GRElevatorManager._instance.GetStagingShuttleForPlayer(this.gamePlayer.rig.OwningNetPlayer.ActorNumber);
		if (!isOnDrillovator)
		{
			return stagingShuttleForPlayer;
		}
		return drillShuttleForPlayer;
	}

	// Token: 0x06003287 RID: 12935 RVA: 0x001155C4 File Offset: 0x001137C4
	public void RefreshShuttles()
	{
		GRShuttle assignedShuttle = this.GetAssignedShuttle(true);
		if (assignedShuttle != null)
		{
			assignedShuttle.Refresh();
		}
		assignedShuttle = this.GetAssignedShuttle(false);
		if (assignedShuttle != null)
		{
			assignedShuttle.Refresh();
		}
	}

	// Token: 0x06003288 RID: 12936 RVA: 0x00115600 File Offset: 0x00113800
	public static GRPlayer GetFromUserId(string userId)
	{
		GRPlayer.tempRigs.Clear();
		GRPlayer.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GRPlayer.tempRigs);
		for (int i = 0; i < GRPlayer.tempRigs.Count; i++)
		{
			if (GRPlayer.tempRigs[i].OwningNetPlayer != null && GRPlayer.tempRigs[i].OwningNetPlayer.UserId == userId)
			{
				return GRPlayer.Get(GRPlayer.tempRigs[i].OwningNetPlayer);
			}
		}
		return null;
	}

	// Token: 0x06003289 RID: 12937 RVA: 0x00115690 File Offset: 0x00113890
	[ContextMenu("Refresh Damage Vignette Visual")]
	public void RefreshDamageVignetteVisual()
	{
		if (this.vrRig.isLocal && this.currentHealthVisualValue != this.hp)
		{
			this.currentHealthVisualValue = this.hp;
			if (this.hp <= this.damageOverlayMaxHp && this.hp > 0)
			{
				if (this.lowHeathVisualCoroutine != null)
				{
					base.StopCoroutine(this.lowHeathVisualCoroutine);
				}
				this.damageEffects.lowHealthVisualRenderer.gameObject.SetActive(true);
				this.lowHeathVisualCoroutine = base.StartCoroutine(this.LowHeathVisualCoroutine());
				return;
			}
			this.damageEffects.lowHealthVisualRenderer.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600328A RID: 12938 RVA: 0x00115731 File Offset: 0x00113931
	private IEnumerator LowHeathVisualCoroutine()
	{
		int index = this.hp - 1;
		if (index >= 0 && index < this.damageOverlayValues.Count)
		{
			float startTime = Time.time;
			while (Time.time - startTime < this.damageOverlayValues[index].effectDuration)
			{
				float time = Mathf.Clamp01((Time.time - startTime) / this.damageOverlayValues[index].effectDuration);
				float num = this.damageOverlayValues[index].effectCurve.Evaluate(time);
				Color tint = this.damageOverlayValues[index].tint;
				tint.a *= num;
				this.damageEffects.lowHealthVisualRenderer.GetPropertyBlock(this.lowHealthVisualPropertyBlock);
				this.lowHealthVisualPropertyBlock.SetColor(this.lowHealthTintPropertyId, tint);
				this.damageEffects.lowHealthVisualRenderer.SetPropertyBlock(this.lowHealthVisualPropertyBlock);
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x0600328B RID: 12939 RVA: 0x00115740 File Offset: 0x00113940
	public void SetGooParticleSystemEnabled(bool bIsLeftHand, bool newEnableState)
	{
		if (this.vrRig != null)
		{
			this.vrRig.SetGooParticleSystemStatus(bIsLeftHand, newEnableState);
		}
	}

	// Token: 0x0600328C RID: 12940 RVA: 0x00115760 File Offset: 0x00113960
	public void SetAsFrozen(float duration)
	{
		if (GorillaTagger.Instance.currentStatus != GorillaTagger.StatusEffect.Frozen)
		{
			this.freezeDuration = duration;
			if (this.gamePlayer.rig.OwningNetPlayer.IsLocal)
			{
				GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, duration);
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
				GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
				if (this.damageEffects.frozenVisualRenderer != null)
				{
					this.damageEffects.frozenVisualRenderer.gameObject.SetActive(true);
				}
				this.playerDamageAudioSource.PlayOneShot(this.playerFrozenSound, 1f);
			}
			this.gamePlayer.rig.UpdateFrozenEffect(true);
			base.Invoke("RemoveFrozen", duration);
		}
	}

	// Token: 0x0600328D RID: 12941 RVA: 0x00115854 File Offset: 0x00113A54
	public void RemoveFrozen()
	{
		this.gamePlayer.rig.UpdateFrozenEffect(false);
		this.freezeDuration = 0f;
		if (this.damageEffects.frozenVisualRenderer != null)
		{
			this.damageEffects.frozenVisualRenderer.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600328E RID: 12942 RVA: 0x001158A8 File Offset: 0x00113AA8
	public override void Tick()
	{
		if (this.lastPlayerPosition != Vector3.zero)
		{
			Vector3 position = this.vrRig.transform.position;
			float magnitude = (this.lastPlayerPosition - position).magnitude;
			this.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.DistanceTraveled, magnitude);
		}
		this.lastPlayerPosition = this.vrRig.transform.position;
		if (this.freezeDuration > 0f)
		{
			this.gamePlayer.rig.UpdateFrozen(Time.deltaTime, this.freezeDuration);
		}
		if (this.inStealthMode && Time.timeAsDouble > this.shieldStealthModeEndTime)
		{
			this.ClearStealthMode();
		}
		GRShuttle.UpdateGRPlayerShuttle(this);
		if (this.soak != null && this.soak.IsSoaking())
		{
			this.soak.OnUpdate();
		}
	}

	// Token: 0x0600328F RID: 12943 RVA: 0x00115974 File Offset: 0x00113B74
	public void SetSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat stat, float amt)
	{
		this.synchronizedSessionStats[(int)stat] = amt;
	}

	// Token: 0x06003290 RID: 12944 RVA: 0x0011597F File Offset: 0x00113B7F
	public void IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat stat, float amt)
	{
		this.synchronizedSessionStats[(int)stat] += amt;
	}

	// Token: 0x06003291 RID: 12945 RVA: 0x00115994 File Offset: 0x00113B94
	public void ResetSynchronizedSessionStats()
	{
		for (int i = 0; i < 8; i++)
		{
			this.synchronizedSessionStats[i] = 0f;
		}
	}

	// Token: 0x06003292 RID: 12946 RVA: 0x001159BC File Offset: 0x00113BBC
	private void RequestSetMothershipUserData(string keyName, string value)
	{
		if (this.saveEquipmentInProgress)
		{
			Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: request already in progress");
			return;
		}
		this.saveEquipmentInProgress = true;
		try
		{
			if (!MothershipClientApiUnity.SetUserDataValue(keyName, value, new Action<SetUserDataResponse>(this.OnSetMothershipUserDataSuccess), new Action<MothershipError, int>(this.OnSetMothershipUserDataFail), ""))
			{
				Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: SetUserDataValue Fail");
				this.OnSetMothershipDataComplete(false);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: exception " + ex.Message);
			this.OnSetMothershipDataComplete(false);
		}
	}

	// Token: 0x06003293 RID: 12947 RVA: 0x00115A4C File Offset: 0x00113C4C
	private void OnSetMothershipUserDataSuccess(SetUserDataResponse response)
	{
		GTDev.Log<string>("GRPlayer OnSetMothershipUserDataSuccess", null);
		this.OnSetMothershipDataComplete(true);
		response.Dispose();
	}

	// Token: 0x06003294 RID: 12948 RVA: 0x00115A68 File Offset: 0x00113C68
	private void OnSetMothershipUserDataFail(MothershipError error, int status)
	{
		string str = (error == null) ? status.ToString() : error.Message;
		GTDev.LogError<string>("GRPlayer OnSetMothershipUserDataFail: " + str, null);
		this.OnSetMothershipDataComplete(false);
		if (error != null)
		{
			error.Dispose();
		}
	}

	// Token: 0x06003295 RID: 12949 RVA: 0x00115AA9 File Offset: 0x00113CA9
	private void OnSetMothershipDataComplete(bool success)
	{
		this.saveEquipmentInProgress = false;
	}

	// Token: 0x06003296 RID: 12950 RVA: 0x00115AB4 File Offset: 0x00113CB4
	public void RequestFetchMothershipUserData(string key)
	{
		if (!this.hasPulledEquipment)
		{
			try
			{
				if (!MothershipClientApiUnity.GetUserDataValue(key, new Action<MothershipUserData>(this.OnGetMothershipFetchUserDataSuccess), new Action<MothershipError, int>(this.OnGetMothershipFetchUserDataFail), ""))
				{
					Debug.LogError("GRPlayer RequestFetchMothershipUserData failed ");
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("GRPlayer RequestFetchMothershipUserData exception " + ex.Message);
			}
		}
	}

	// Token: 0x06003297 RID: 12951 RVA: 0x00115B24 File Offset: 0x00113D24
	private void OnGetMothershipFetchUserDataSuccess(MothershipUserData response)
	{
		GTDev.Log<string>("GRPlayer OnGetMothershipFetchUserDataSuccess", null);
		bool flag = response != null && response.value != null && response.value.Length > 0;
		if (response != null)
		{
		}
		if (response != null)
		{
			response.Dispose();
		}
	}

	// Token: 0x06003298 RID: 12952 RVA: 0x00115B68 File Offset: 0x00113D68
	private void OnGetMothershipFetchUserDataFail(MothershipError error, int status)
	{
		string str = (error == null) ? status.ToString() : error.Message;
		GTDev.LogError<string>("GRPlayer OnGetMothershipFetchUserDataFail: " + str, null);
		if (error != null)
		{
			error.Dispose();
		}
	}

	// Token: 0x06003299 RID: 12953 RVA: 0x00115BA2 File Offset: 0x00113DA2
	public bool IsDropPodUnlocked()
	{
		return this.dropPodLevel > 0;
	}

	// Token: 0x0600329A RID: 12954 RVA: 0x00115BB0 File Offset: 0x00113DB0
	public int GetMaxDropFloor()
	{
		switch (this.dropPodChasisLevel + this.dropPodLevel)
		{
		case 0:
			return 1;
		case 1:
			return 5;
		case 2:
			return 10;
		case 3:
			return 15;
		case 4:
			return 20;
		default:
			return 0;
		}
	}

	// Token: 0x0600329B RID: 12955 RVA: 0x00115BF5 File Offset: 0x00113DF5
	public void CollectShiftCut()
	{
		this.SetProgressionData(this.currentProgression.points + this.LastShiftCut, this.currentProgression.redeemedPoints, true);
	}

	// Token: 0x0600329C RID: 12956 RVA: 0x00115C1C File Offset: 0x00113E1C
	public bool AttemptPromotion()
	{
		ValueTuple<int, int, int, int> gradePointDetails = GhostReactorProgression.GetGradePointDetails(this.CurrentProgression.redeemedPoints);
		int item = gradePointDetails.Item3;
		int item2 = gradePointDetails.Item4;
		if (item - item2 < this.CurrentProgression.points - this.CurrentProgression.redeemedPoints)
		{
			this.SetProgressionData(this.currentProgression.points, this.currentProgression.points, false);
			return true;
		}
		return false;
	}

	// Token: 0x0600329D RID: 12957 RVA: 0x00115C84 File Offset: 0x00113E84
	public void SetProgressionData(int _points, int _redeemedPoints, bool saveProgression = false)
	{
		if (_points < 0 || _redeemedPoints < 0)
		{
			return;
		}
		this.currentProgression = new GRPlayer.ProgressionData
		{
			points = _points,
			redeemedPoints = _redeemedPoints
		};
		if (this.gamePlayer.IsLocal() && saveProgression)
		{
			this.SaveMyProgression();
		}
	}

	// Token: 0x0600329E RID: 12958 RVA: 0x00115CCE File Offset: 0x00113ECE
	public void LoadMyProgression()
	{
		GhostReactorProgression.instance.GetStartingProgression(this);
	}

	// Token: 0x0600329F RID: 12959 RVA: 0x00115CDB File Offset: 0x00113EDB
	public void SaveMyProgression()
	{
		GhostReactorProgression.instance.SetProgression(this.LastShiftCut, this);
	}

	// Token: 0x04004136 RID: 16694
	public const int MAX_CURRENCY = 500;

	// Token: 0x04004137 RID: 16695
	public GamePlayer gamePlayer;

	// Token: 0x04004138 RID: 16696
	private GRPlayer.GRPlayerState state;

	// Token: 0x04004139 RID: 16697
	private int shiftCreditCache;

	// Token: 0x0400413A RID: 16698
	public int startingShiftCreditCache;

	// Token: 0x0400413B RID: 16699
	public int playerJuice;

	// Token: 0x0400413E RID: 16702
	public double shiftJoinTime;

	// Token: 0x0400413F RID: 16703
	public bool isEmployee;

	// Token: 0x04004140 RID: 16704
	public AudioSource audioSource;

	// Token: 0x04004141 RID: 16705
	[Header("Hit / Revive Effects")]
	public ParticleSystem playerTurnedGhostEffect;

	// Token: 0x04004142 RID: 16706
	public SoundBankPlayer playerTurnedGhostSoundBank;

	// Token: 0x04004143 RID: 16707
	public ParticleSystem playerRevivedEffect;

	// Token: 0x04004144 RID: 16708
	public AudioClip playerRevivedSound;

	// Token: 0x04004145 RID: 16709
	public float playerRevivedVolume = 1f;

	// Token: 0x04004146 RID: 16710
	public AudioSource playerDamageAudioSource;

	// Token: 0x04004147 RID: 16711
	public Transform bodyCenter;

	// Token: 0x04004148 RID: 16712
	public ParticleSystem playerDamageEffect;

	// Token: 0x04004149 RID: 16713
	public float playerDamageVolume = 1f;

	// Token: 0x0400414A RID: 16714
	public AudioClip playerDamageSound;

	// Token: 0x0400414B RID: 16715
	public float playerDamageOffsetDist = 0.25f;

	// Token: 0x0400414C RID: 16716
	[ColorUsage(true, true)]
	[SerializeField]
	private Color deathTintColor;

	// Token: 0x0400414D RID: 16717
	[ColorUsage(true, true)]
	[SerializeField]
	private Color deathAmbientLightColor;

	// Token: 0x0400414E RID: 16718
	public GameLight shieldGameLight;

	// Token: 0x0400414F RID: 16719
	[Header("Attach")]
	public Transform attachEnemy;

	// Token: 0x04004150 RID: 16720
	[Header("Shield")]
	public Transform shieldHeadVisual;

	// Token: 0x04004151 RID: 16721
	public Transform shieldBodyVisual;

	// Token: 0x04004152 RID: 16722
	public AudioClip shieldActivatedSound;

	// Token: 0x04004153 RID: 16723
	public float shieldActivatedVolume = 0.5f;

	// Token: 0x04004154 RID: 16724
	public ParticleSystem shieldDamagedEffect;

	// Token: 0x04004155 RID: 16725
	public AudioClip shieldDamagedSound;

	// Token: 0x04004156 RID: 16726
	public float shieldDamagedVolume = 0.5f;

	// Token: 0x04004157 RID: 16727
	public ParticleSystem shieldDestroyedEffect;

	// Token: 0x04004158 RID: 16728
	public AudioClip shieldDestroyedSound;

	// Token: 0x04004159 RID: 16729
	public float shieldDestroyedVolume = 0.5f;

	// Token: 0x0400415A RID: 16730
	public float shieldStealthModeDuration = 20f;

	// Token: 0x0400415B RID: 16731
	private double shieldStealthModeEndTime;

	// Token: 0x0400415C RID: 16732
	public Color shieldColorNormal = new Color(0.42352942f, 0.25490198f, 1f, 0.45490196f);

	// Token: 0x0400415D RID: 16733
	public Color shieldColorLight = new Color(1f, 1f, 1f, 0.5f);

	// Token: 0x0400415E RID: 16734
	public Color shieldColorStealth = new Color(1f, 0.2f, 0f, 0.5f);

	// Token: 0x0400415F RID: 16735
	public Color shieldColorHeal = new Color(0f, 1f, 1f, 0.5f);

	// Token: 0x04004160 RID: 16736
	public int xRayVisionRefCount;

	// Token: 0x04004161 RID: 16737
	[Header("Badge")]
	public Transform badgeBodyAnchor;

	// Token: 0x04004162 RID: 16738
	[SerializeField]
	private Transform badgeBodyStringAttach;

	// Token: 0x04004163 RID: 16739
	[NonSerialized]
	public double lastLeftWithBadgeAttachedTime;

	// Token: 0x04004164 RID: 16740
	[Header("Health")]
	[SerializeField]
	private int maxHp = 1;

	// Token: 0x04004165 RID: 16741
	[SerializeField]
	private int maxShieldHp = 1;

	// Token: 0x04004166 RID: 16742
	public string mothershipId;

	// Token: 0x04004167 RID: 16743
	private int hp;

	// Token: 0x04004168 RID: 16744
	private int shieldHp;

	// Token: 0x04004169 RID: 16745
	private int shieldFlags;

	// Token: 0x0400416A RID: 16746
	private bool inStealthMode;

	// Token: 0x0400416B RID: 16747
	[Header("Damage Vignette")]
	[SerializeField]
	[Tooltip("First entry is 1 hp, second entry is 2 hp, etc.")]
	private List<GRPlayer.DamageOverlayValues> damageOverlayValues = new List<GRPlayer.DamageOverlayValues>();

	// Token: 0x0400416C RID: 16748
	[SerializeField]
	private int damageOverlayMaxHp = 1;

	// Token: 0x0400416D RID: 16749
	[HideInInspector]
	public GRBadge badge;

	// Token: 0x0400416E RID: 16750
	public CallLimiter requestCollectItemLimiter;

	// Token: 0x0400416F RID: 16751
	public CallLimiter requestChargeToolLimiter;

	// Token: 0x04004170 RID: 16752
	public CallLimiter requestDepositCurrencyLimiter;

	// Token: 0x04004171 RID: 16753
	public CallLimiter requestShiftStartLimiter;

	// Token: 0x04004172 RID: 16754
	public CallLimiter requestToolPurchaseStationLimiter;

	// Token: 0x04004173 RID: 16755
	public CallLimiter applyEnemyHitLimiter;

	// Token: 0x04004174 RID: 16756
	public CallLimiter reportLocalHitLimiter;

	// Token: 0x04004175 RID: 16757
	public CallLimiter reportBreakableBrokenLimiter;

	// Token: 0x04004176 RID: 16758
	public CallLimiter playerStateChangeLimiter;

	// Token: 0x04004177 RID: 16759
	public CallLimiter promotionBotLimiter;

	// Token: 0x04004178 RID: 16760
	public CallLimiter progressionBroadcastLimiter;

	// Token: 0x04004179 RID: 16761
	public CallLimiter scoreboardPageLimiter;

	// Token: 0x0400417A RID: 16762
	public CallLimiter fireShieldLimiter;

	// Token: 0x0400417B RID: 16763
	private VRRig vrRig;

	// Token: 0x0400417C RID: 16764
	private List<VRRig> vrRigs = new List<VRRig>();

	// Token: 0x0400417D RID: 16765
	private string gameId;

	// Token: 0x0400417E RID: 16766
	public int coresCollectedByPlayer;

	// Token: 0x0400417F RID: 16767
	public int coresCollectedByGroup;

	// Token: 0x04004180 RID: 16768
	public int coresSpentByPlayer;

	// Token: 0x04004181 RID: 16769
	public int coresSpentByGroup;

	// Token: 0x04004182 RID: 16770
	public int gatesUnlocked;

	// Token: 0x04004183 RID: 16771
	public int deaths;

	// Token: 0x04004184 RID: 16772
	public bool caughtByAnomaly;

	// Token: 0x04004185 RID: 16773
	public List<string> itemsPurchased;

	// Token: 0x04004186 RID: 16774
	public List<string> levelsUnlocked;

	// Token: 0x04004187 RID: 16775
	public float timeIntoShiftAtJoin;

	// Token: 0x04004188 RID: 16776
	public bool wasPlayerInAtShiftStart;

	// Token: 0x04004189 RID: 16777
	public int sentientCoresCollected;

	// Token: 0x0400418A RID: 16778
	public int maxNumberOfPlayersInShift;

	// Token: 0x0400418B RID: 16779
	public int revives;

	// Token: 0x0400418C RID: 16780
	public float[] synchronizedSessionStats = new float[8];

	// Token: 0x0400418D RID: 16781
	private HashSet<GameEntityId> itemsHeldThisShift = new HashSet<GameEntityId>();

	// Token: 0x0400418E RID: 16782
	private Dictionary<string, int> itemTypesHeldThisShift = new Dictionary<string, int>();

	// Token: 0x0400418F RID: 16783
	public int totalCoresCollectedByPlayer;

	// Token: 0x04004190 RID: 16784
	public int totalCoresCollectedByGroup;

	// Token: 0x04004191 RID: 16785
	public int totalCoresSpentByPlayer;

	// Token: 0x04004192 RID: 16786
	public int totalCoresSpentByGroup;

	// Token: 0x04004193 RID: 16787
	public int totalGatesUnlocked;

	// Token: 0x04004194 RID: 16788
	public int totalDeaths;

	// Token: 0x04004195 RID: 16789
	public List<string> totalItemsPurchased;

	// Token: 0x04004196 RID: 16790
	public float timeIntoGameAtJoin;

	// Token: 0x04004197 RID: 16791
	public bool wasPlayerInAtGameStart;

	// Token: 0x04004198 RID: 16792
	public int maxNumberOfPlayersIngame;

	// Token: 0x04004199 RID: 16793
	public int totalRevives;

	// Token: 0x0400419A RID: 16794
	public int numShiftsPlayed;

	// Token: 0x0400419B RID: 16795
	public float gameStartTime;

	// Token: 0x0400419C RID: 16796
	public bool isFirstShift = true;

	// Token: 0x0400419D RID: 16797
	private HashSet<GameEntityId> totalItemsHeldThisShift = new HashSet<GameEntityId>();

	// Token: 0x0400419E RID: 16798
	private Dictionary<string, int> totalItemTypesHeldThisShift = new Dictionary<string, int>();

	// Token: 0x0400419F RID: 16799
	private GRPlayerDamageEffects damageEffects;

	// Token: 0x040041A0 RID: 16800
	private MaterialPropertyBlock lowHealthVisualPropertyBlock;

	// Token: 0x040041A1 RID: 16801
	private int lowHealthTintPropertyId;

	// Token: 0x040041A2 RID: 16802
	private int currentHealthVisualValue;

	// Token: 0x040041A3 RID: 16803
	private Coroutine lowHeathVisualCoroutine;

	// Token: 0x040041A4 RID: 16804
	public AudioClip playerFrozenSound;

	// Token: 0x040041A5 RID: 16805
	public GRPlayer.ShuttleData shuttleData;

	// Token: 0x040041A6 RID: 16806
	private GRPlayer.ProgressionData currentProgression;

	// Token: 0x040041A7 RID: 16807
	private float shiftPlayTime;

	// Token: 0x040041A8 RID: 16808
	private int lastShiftCut;

	// Token: 0x040041A9 RID: 16809
	private GhostReactorSoak soak;

	// Token: 0x040041AA RID: 16810
	private static List<VRRig> tempRigs = new List<VRRig>(32);

	// Token: 0x040041AB RID: 16811
	private float freezeDuration;

	// Token: 0x040041AC RID: 16812
	private Vector3 lastPlayerPosition = Vector3.zero;

	// Token: 0x040041AD RID: 16813
	private bool saveEquipmentInProgress;

	// Token: 0x040041AE RID: 16814
	private bool hasPulledEquipment;

	// Token: 0x040041AF RID: 16815
	public int dropPodLevel;

	// Token: 0x040041B0 RID: 16816
	public int dropPodChasisLevel;

	// Token: 0x020007B8 RID: 1976
	public enum GRPlayerState
	{
		// Token: 0x040041B2 RID: 16818
		Alive,
		// Token: 0x040041B3 RID: 16819
		Ghost,
		// Token: 0x040041B4 RID: 16820
		Shielded
	}

	// Token: 0x020007B9 RID: 1977
	public enum GRPlayerShieldFlags
	{
		// Token: 0x040041B6 RID: 16822
		Light = 1,
		// Token: 0x040041B7 RID: 16823
		Stealth,
		// Token: 0x040041B8 RID: 16824
		Heal = 4
	}

	// Token: 0x020007BA RID: 1978
	public enum SynchronizedSessionStat
	{
		// Token: 0x040041BA RID: 16826
		CoresDeposited,
		// Token: 0x040041BB RID: 16827
		EarnedCredits,
		// Token: 0x040041BC RID: 16828
		SpentCredits,
		// Token: 0x040041BD RID: 16829
		DistanceTraveled,
		// Token: 0x040041BE RID: 16830
		Deaths,
		// Token: 0x040041BF RID: 16831
		Kills,
		// Token: 0x040041C0 RID: 16832
		Assists,
		// Token: 0x040041C1 RID: 16833
		TimeChaosExposure,
		// Token: 0x040041C2 RID: 16834
		Count
	}

	// Token: 0x020007BB RID: 1979
	[Serializable]
	private struct DamageOverlayValues
	{
		// Token: 0x040041C3 RID: 16835
		public Color tint;

		// Token: 0x040041C4 RID: 16836
		public float effectDuration;

		// Token: 0x040041C5 RID: 16837
		public AnimationCurve effectCurve;
	}

	// Token: 0x020007BC RID: 1980
	public enum ShuttleState
	{
		// Token: 0x040041C7 RID: 16839
		Idle,
		// Token: 0x040041C8 RID: 16840
		Moving,
		// Token: 0x040041C9 RID: 16841
		WaitForLeaveRoom,
		// Token: 0x040041CA RID: 16842
		JoinRoom,
		// Token: 0x040041CB RID: 16843
		WaitForLeadPlayer,
		// Token: 0x040041CC RID: 16844
		Teleport,
		// Token: 0x040041CD RID: 16845
		TeleportToMyShuttleSafety,
		// Token: 0x040041CE RID: 16846
		PostTeleport
	}

	// Token: 0x020007BD RID: 1981
	public class ShuttleData
	{
		// Token: 0x040041CF RID: 16847
		public string ownerUserId;

		// Token: 0x040041D0 RID: 16848
		public int currShuttleId;

		// Token: 0x040041D1 RID: 16849
		public int targetShuttleId;

		// Token: 0x040041D2 RID: 16850
		public int targetLevel;

		// Token: 0x040041D3 RID: 16851
		public GRPlayer.ShuttleState state;

		// Token: 0x040041D4 RID: 16852
		public double stateStartTime;
	}

	// Token: 0x020007BE RID: 1982
	[Serializable]
	public struct ProgressionData
	{
		// Token: 0x040041D5 RID: 16853
		public int points;

		// Token: 0x040041D6 RID: 16854
		public int redeemedPoints;
	}

	// Token: 0x020007BF RID: 1983
	[Serializable]
	public struct ProgressionLevels
	{
		// Token: 0x040041D7 RID: 16855
		public int tierId;

		// Token: 0x040041D8 RID: 16856
		public string tierName;

		// Token: 0x040041D9 RID: 16857
		public int grades;

		// Token: 0x040041DA RID: 16858
		public int pointsPerGrade;
	}
}
