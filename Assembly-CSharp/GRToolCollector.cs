using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x020007F5 RID: 2037
public class GRToolCollector : MonoBehaviour, IGameEntityDebugComponent, IGameEntityComponent
{
	// Token: 0x06003403 RID: 13315 RVA: 0x0011E191 File Offset: 0x0011C391
	private void Awake()
	{
		this.state = GRToolCollector.State.Idle;
		this.stateTimeRemaining = -1f;
	}

	// Token: 0x06003404 RID: 13316 RVA: 0x0011E1A5 File Offset: 0x0011C3A5
	private void OnEnable()
	{
		this.SetState(GRToolCollector.State.Idle);
	}

	// Token: 0x06003405 RID: 13317 RVA: 0x0011E1AE File Offset: 0x0011C3AE
	public void OnEntityInit()
	{
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
		this.lastRechargeTime = (double)Time.time;
	}

	// Token: 0x06003406 RID: 13318 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06003407 RID: 13319 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06003408 RID: 13320 RVA: 0x0011E1F0 File Offset: 0x0011C3F0
	private void OnToolUpgraded(GRTool tool)
	{
		this.rechargeRate = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.RechargeRate);
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.CollectorBonus1))
		{
			this.vacuumSound = this.upgrade1vacuumSound;
			this.vacuumParticleEffect = this.upgrade1VacuumParticleEffect;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.CollectorBonus2))
		{
			this.vacuumSound = this.upgrade2vacuumSound;
			this.vacuumParticleEffect = this.upgrade2VacuumParticleEffect;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.CollectorBonus3))
		{
			this.vacuumSound = this.upgrade3vacuumSound;
			this.vacuumParticleEffect = this.upgrade3VacuumParticleEffect;
		}
	}

	// Token: 0x06003409 RID: 13321 RVA: 0x0011E278 File Offset: 0x0011C478
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x0600340A RID: 13322 RVA: 0x0011E291 File Offset: 0x0011C491
	public void OnUpdate(float dt)
	{
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x0600340B RID: 13323 RVA: 0x0011E2B4 File Offset: 0x0011C4B4
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x0600340C RID: 13324 RVA: 0x0011E2E8 File Offset: 0x0011C4E8
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolCollector.State.Idle:
		{
			bool flag = this.IsButtonHeld();
			this.waitingForButtonRelease = (this.waitingForButtonRelease && flag);
			if (flag && !this.waitingForButtonRelease)
			{
				this.SetStateAuthority(GRToolCollector.State.Vacuuming);
				this.activatedLocally = true;
			}
			if (this.rechargeRate > 0f && Time.timeAsDouble > this.lastRechargeTime + (double)this.rechargeInterval)
			{
				this.gameEntity.manager.ghostReactorManager.RequestChargeTool(this.gameEntity.id, this.gameEntity.id, (int)(this.rechargeRate * this.rechargeInterval), false);
				this.lastRechargeTime = Time.timeAsDouble;
				if (this.passiveChargeParticleEffect != null)
				{
					this.passiveChargeParticleEffect.Play();
					return;
				}
			}
			break;
		}
		case GRToolCollector.State.Vacuuming:
		{
			bool flag2 = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolCollector.State.Collect);
				return;
			}
			if (!flag2)
			{
				this.SetStateAuthority(GRToolCollector.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolCollector.State.Collect:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolCollector.State.Cooldown);
				return;
			}
			break;
		case GRToolCollector.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.activatedLocally = false;
				this.waitingForButtonRelease = true;
				this.SetStateAuthority(GRToolCollector.State.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600340D RID: 13325 RVA: 0x0011E45C File Offset: 0x0011C65C
	private void OnUpdateRemote(float dt)
	{
		GRToolCollector.State state = (GRToolCollector.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x0600340E RID: 13326 RVA: 0x0011E486 File Offset: 0x0011C686
	private void SetStateAuthority(GRToolCollector.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x0600340F RID: 13327 RVA: 0x0011E4A8 File Offset: 0x0011C6A8
	private void SetState(GRToolCollector.State newState)
	{
		this.state = newState;
		switch (this.state)
		{
		case GRToolCollector.State.Idle:
			this.StopVacuum();
			this.stateTimeRemaining = -1f;
			this.lastRechargeTime = (double)Time.time;
			return;
		case GRToolCollector.State.Vacuuming:
			this.StartVacuum();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolCollector.State.Collect:
			this.TryCollect();
			this.stateTimeRemaining = this.collectDuration;
			return;
		case GRToolCollector.State.Cooldown:
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x06003410 RID: 13328 RVA: 0x0011E52C File Offset: 0x0011C72C
	private void StartVacuum()
	{
		this.vacuumAudioSource.clip = this.vacuumSound;
		this.vacuumAudioSource.volume = this.vacuumSoundVolume;
		this.vacuumAudioSource.loop = true;
		this.vacuumAudioSource.Play();
		this.vacuumParticleEffect.Play();
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x06003411 RID: 13329 RVA: 0x0011E59B File Offset: 0x0011C79B
	private void StopVacuum()
	{
		this.vacuumAudioSource.loop = false;
		this.vacuumAudioSource.Stop();
		this.vacuumParticleEffect.Stop();
	}

	// Token: 0x06003412 RID: 13330 RVA: 0x0011E5C0 File Offset: 0x0011C7C0
	private void TryCollect()
	{
		if (this.IsHeldLocal())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 0.2f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, 1f, this.collectibleLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				GameObject gameObject = null;
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					gameObject = attachedRigidbody.gameObject;
				}
				else
				{
					GameEntity gameEntity = GameEntity.Get(raycastHit.collider);
					if (gameEntity != null)
					{
						gameObject = gameEntity.gameObject;
					}
				}
				if (gameObject != null)
				{
					GRCollectible component = gameObject.GetComponent<GRCollectible>();
					if (component != null && component.type != ProgressionManager.CoreType.ChaosSeed && this.tool.energy < this.tool.GetEnergyMax())
					{
						GhostReactorManager.Get(this.gameEntity).RequestCollectItem(component.entity.id, this.gameEntity.id);
						return;
					}
				}
			}
			for (int j = 0; j < num; j++)
			{
				RaycastHit raycastHit2 = this.tempHitResults[j];
				GameObject gameObject2 = null;
				Rigidbody attachedRigidbody2 = raycastHit2.collider.attachedRigidbody;
				if (attachedRigidbody2 != null)
				{
					gameObject2 = attachedRigidbody2.gameObject;
				}
				else
				{
					GameEntity gameEntity2 = GameEntity.Get(raycastHit2.collider);
					if (gameEntity2 != null)
					{
						gameObject2 = gameEntity2.gameObject;
					}
				}
				if (gameObject2 != null)
				{
					if (gameObject2.GetComponent<GRCurrencyDepositor>() != null)
					{
						if (this.tool.energy > 0)
						{
							GhostReactorManager.Get(this.gameEntity).RequestDepositCurrency(this.gameEntity.id);
						}
						return;
					}
					GRTool component2 = gameObject2.GetComponent<GRTool>();
					if (!(component2 == null) && !(component2 == this.tool))
					{
						GameEntity component3 = gameObject2.GetComponent<GameEntity>();
						if (component2 != null && component3 != null)
						{
							GhostReactorManager.Get(this.gameEntity).RequestChargeTool(this.gameEntity.id, component3.id, 0, true);
							if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.CollectorBonus3) && this.tool.energy > 50)
							{
								List<GRTool> list = new List<GRTool>();
								this.gameEntity.manager.GetEntitiesWithComponentInRadius<GRTool>(base.transform.position, this.level3ChargeRadius, true, list);
								for (int k = 0; k < list.Count; k++)
								{
									GRTool grtool = list[k];
									if (!(grtool.GetComponent<GRToolCollector>() != null) && !(grtool.gameEntity == this.gameEntity) && !(grtool.gameEntity == component3))
									{
										GhostReactorManager.Get(this.gameEntity).RequestChargeTool(this.gameEntity.id, grtool.gameEntity.id, 0, false);
									}
								}
							}
							return;
						}
					}
				}
			}
		}
	}

	// Token: 0x06003413 RID: 13331 RVA: 0x0011E8D0 File Offset: 0x0011CAD0
	public void PerformCollection(GRCollectible collectible)
	{
		this.tool.RefillEnergy(collectible.energyValue + this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HarvestGain), collectible.entity.id);
		this.collectAudioSource.volume = this.collectSoundVolume;
		this.collectAudioSource.PlayOneShot(this.collectSound);
	}

	// Token: 0x06003414 RID: 13332 RVA: 0x0011E928 File Offset: 0x0011CB28
	public void PlayChargeEffect(GRTool targetTool)
	{
		if (targetTool == null)
		{
			return;
		}
		if (targetTool == this.tool)
		{
			return;
		}
		this.collectAudioSource.volume = this.chargeBeamVolume;
		this.collectAudioSource.PlayOneShot(this.chargeBeamSound);
		for (int i = 0; i < targetTool.energyMeters.Count; i++)
		{
			if (targetTool.energyMeters[i].chargePoint != null)
			{
				this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetTool.energyMeters[i].chargePoint.position);
			}
			else
			{
				this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetTool.energyMeters[i].transform.position);
			}
		}
	}

	// Token: 0x06003415 RID: 13333 RVA: 0x0011EA0C File Offset: 0x0011CC0C
	public void PlayChargeEffect(GRCurrencyDepositor targetDepositor)
	{
		if (targetDepositor == null)
		{
			return;
		}
		this.collectAudioSource.volume = this.chargeBeamVolume;
		this.collectAudioSource.PlayOneShot(this.chargeBeamSound);
		this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetDepositor.depositingChargePoint.position);
	}

	// Token: 0x06003416 RID: 13334 RVA: 0x0011EA6C File Offset: 0x0011CC6C
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x06003417 RID: 13335 RVA: 0x0011EACC File Offset: 0x0011CCCC
	private void PlayVibration(float strength, float duration)
	{
		if (!this.IsHeldLocal())
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x06003418 RID: 13336 RVA: 0x0011EB20 File Offset: 0x0011CD20
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("Recharge Rate: <color=\"yellow\">{0}<color=\"white\">", this.rechargeRate));
	}

	// Token: 0x040043C3 RID: 17347
	public GameEntity gameEntity;

	// Token: 0x040043C4 RID: 17348
	public GRTool tool;

	// Token: 0x040043C5 RID: 17349
	public GRAttributes attributes;

	// Token: 0x040043C6 RID: 17350
	public int energyDepositPerUse = 100;

	// Token: 0x040043C7 RID: 17351
	public Transform shootFrom;

	// Token: 0x040043C8 RID: 17352
	public LayerMask collectibleLayerMask;

	// Token: 0x040043C9 RID: 17353
	public ParticleSystem vacuumParticleEffect;

	// Token: 0x040043CA RID: 17354
	public ParticleSystem upgrade1VacuumParticleEffect;

	// Token: 0x040043CB RID: 17355
	public ParticleSystem upgrade2VacuumParticleEffect;

	// Token: 0x040043CC RID: 17356
	public ParticleSystem upgrade3VacuumParticleEffect;

	// Token: 0x040043CD RID: 17357
	public ParticleSystem passiveChargeParticleEffect;

	// Token: 0x040043CE RID: 17358
	public AudioSource vacuumAudioSource;

	// Token: 0x040043CF RID: 17359
	public AudioClip vacuumSound;

	// Token: 0x040043D0 RID: 17360
	public AudioClip upgrade1vacuumSound;

	// Token: 0x040043D1 RID: 17361
	public AudioClip upgrade2vacuumSound;

	// Token: 0x040043D2 RID: 17362
	public AudioClip upgrade3vacuumSound;

	// Token: 0x040043D3 RID: 17363
	public float vacuumSoundVolume = 0.2f;

	// Token: 0x040043D4 RID: 17364
	public AudioSource collectAudioSource;

	// Token: 0x040043D5 RID: 17365
	[FormerlySerializedAs("flashSound")]
	public AudioClip collectSound;

	// Token: 0x040043D6 RID: 17366
	[FormerlySerializedAs("flashSoundVolume")]
	public float collectSoundVolume = 1f;

	// Token: 0x040043D7 RID: 17367
	public AudioClip chargeBeamSound;

	// Token: 0x040043D8 RID: 17368
	public float chargeBeamVolume = 0.2f;

	// Token: 0x040043D9 RID: 17369
	public LightningDispatcher lightningDispatcher;

	// Token: 0x040043DA RID: 17370
	public float chargeDuration = 0.75f;

	// Token: 0x040043DB RID: 17371
	[FormerlySerializedAs("flashDuration")]
	public float collectDuration = 0.1f;

	// Token: 0x040043DC RID: 17372
	public float cooldownDuration;

	// Token: 0x040043DD RID: 17373
	public AbilityHaptic collectHaptic;

	// Token: 0x040043DE RID: 17374
	[NonSerialized]
	public GhostReactorManager grManager;

	// Token: 0x040043DF RID: 17375
	private float rechargeRate;

	// Token: 0x040043E0 RID: 17376
	public float rechargeInterval = 1f;

	// Token: 0x040043E1 RID: 17377
	private double lastRechargeTime;

	// Token: 0x040043E2 RID: 17378
	public float level3ChargeRadius = 4f;

	// Token: 0x040043E3 RID: 17379
	private GRToolCollector.State state;

	// Token: 0x040043E4 RID: 17380
	private float stateTimeRemaining;

	// Token: 0x040043E5 RID: 17381
	private bool activatedLocally;

	// Token: 0x040043E6 RID: 17382
	private bool waitingForButtonRelease;

	// Token: 0x040043E7 RID: 17383
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x020007F6 RID: 2038
	private enum State
	{
		// Token: 0x040043E9 RID: 17385
		Idle,
		// Token: 0x040043EA RID: 17386
		Vacuuming,
		// Token: 0x040043EB RID: 17387
		Collect,
		// Token: 0x040043EC RID: 17388
		Cooldown
	}
}
