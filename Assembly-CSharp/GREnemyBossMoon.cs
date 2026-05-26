using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using GorillaTagScripts.GhostReactor;
using JetBrains.Annotations;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000776 RID: 1910
public class GREnemyBossMoon : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent, IGRSummoningEntity
{
	// Token: 0x17000477 RID: 1143
	// (get) Token: 0x06003054 RID: 12372 RVA: 0x001067AC File Offset: 0x001049AC
	// (set) Token: 0x06003055 RID: 12373 RVA: 0x001067B4 File Offset: 0x001049B4
	public bool BossHasRevealed { get; private set; }

	// Token: 0x17000478 RID: 1144
	// (get) Token: 0x06003056 RID: 12374 RVA: 0x001067BD File Offset: 0x001049BD
	public GRAbilityBase CurrAbility
	{
		get
		{
			return this.currAbility;
		}
	}

	// Token: 0x06003057 RID: 12375 RVA: 0x001067C8 File Offset: 0x001049C8
	private void Awake()
	{
		this.trackedEntities = new List<int>(16);
		this.trackedGameEntities = new List<GameEntity>(16);
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.abilities = new GRAbilityBase[32];
		this.adaptiveMusicController = Object.FindObjectOfType<GRAdaptiveMusicController>();
	}

	// Token: 0x06003058 RID: 12376 RVA: 0x0010685C File Offset: 0x00104A5C
	public void OnEntityInit()
	{
		this.currBehavior = GREnemyBossMoon.Behavior.None;
		this.currAbility = null;
		this.SetupAbility(GREnemyBossMoon.Behavior.HiddenIdle, this.abilityHiddenIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Reveal, this.abilityReveal, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Idle, this.abilityIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Exposed, this.abilityExposed, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.ExposedIdle, this.abilityExposedIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTongue, this.abilityAttackTongue01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTongueSwipe, this.abilityAttackTongueSwipe01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle00, this.abilityAttackTentacle00, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle01, this.abilityAttackTentacle01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle02, this.abilityAttackTentacle02, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle03, this.abilityAttackTentacle03, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle04, this.abilityAttackTentacle04, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle05, this.abilityAttackTentacle05, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackQuickTentacle00, this.abilityAttackQuickTentacle00, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackQuickTentacle01, this.abilityAttackQuickTentacle01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackQuickTentacle02, this.abilityAttackQuickTentacle02, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackQuickTentacle03, this.abilityAttackQuickTentacle03, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.SummonStart, this.abilitySummonStart, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.SummonEnd, this.abilitySummonEnd, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Summon01, this.abilitySummon01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Summon02, this.abilitySummon02, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Summon03, this.abilitySummon03, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Summon04, this.abilitySummon04, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.RetreatStart, this.abilityRetreatStart, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.RetreatEnd, this.abilityRetreatEnd, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.RetreatIdle, this.abilityRetreatIdle, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Dying, this.abilityDie, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.DyingIdle, this.abilityDieIdle, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Runaway, this.abilityRunaway, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.NextPhase, this.abilityIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.senseNearby.Setup(this.headTransform, this.entity);
		this.Setup(this.entity.createData);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			GhostReactorLevelGenConfig currLevelGenConfig = this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig();
			foreach (GRBonusEntry entry in currLevelGenConfig.enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
			if (currLevelGenConfig.minEnemyKills.Count > 0)
			{
				GREnemyCount grenemyCount = currLevelGenConfig.minEnemyKills[0];
				GREnemyType enemyType = grenemyCount.EnemyType;
				if (enemyType != GREnemyType.MoonBoss_Phase1)
				{
					if (enemyType == GREnemyType.MoonBoss_Phase2)
					{
						this.phases[1].runawayAfterPhase = true;
					}
				}
				else
				{
					this.phases[0].runawayAfterPhase = true;
				}
				GRBreakableItemSpawnConfig lootTableForType = this.GetLootTableForType(grenemyCount.EnemyType);
				this.abilityDie.lootTable = lootTableForType;
				this.abilityRunaway.lootTable = lootTableForType;
			}
		}
		if (this.agent.navAgent != null)
		{
			this.agent.navAgent.autoTraverseOffMeshLink = false;
		}
		this.SetBehavior(GREnemyBossMoon.Behavior.HiddenIdle, true);
		int maxHP = this.CalcMaxHP();
		if (this.enemy != null)
		{
			this.enemy.SetMaxHP(maxHP);
		}
		this.SetHP(maxHP);
	}

	// Token: 0x06003059 RID: 12377 RVA: 0x00106F30 File Offset: 0x00105130
	private GRBreakableItemSpawnConfig GetLootTableForType(GREnemyType enemyType)
	{
		for (int i = 0; i < this.lootPhases.Count; i++)
		{
			if (this.lootPhases[i].enemyType == enemyType)
			{
				return this.lootPhases[i].lootTable;
			}
		}
		return null;
	}

	// Token: 0x0600305A RID: 12378 RVA: 0x00106F7A File Offset: 0x0010517A
	private void SetupAbility(GREnemyBossMoon.Behavior behavior, GRAbilityBase ability, GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		this.abilities[(int)behavior] = ability;
		ability.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x0600305B RID: 12379 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x0600305C RID: 12380 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x0600305D RID: 12381 RVA: 0x00106F96 File Offset: 0x00105196
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x0600305E RID: 12382 RVA: 0x00106FC6 File Offset: 0x001051C6
	public void Setup(long entityCreateData)
	{
		this.SetBehavior(GREnemyBossMoon.Behavior.HiddenIdle, true);
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyBossMoon.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyBossMoon.BodyState.Bones, true);
	}

	// Token: 0x0600305F RID: 12383 RVA: 0x00106FF0 File Offset: 0x001051F0
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 32)
		{
			return;
		}
		this.SetBehavior((GREnemyBossMoon.Behavior)newState, false);
	}

	// Token: 0x06003060 RID: 12384 RVA: 0x00107004 File Offset: 0x00105204
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyBossMoon.BodyState)newState, false);
	}

	// Token: 0x06003061 RID: 12385 RVA: 0x00107017 File Offset: 0x00105217
	public void SetHP(int hp)
	{
		this.hp = hp;
		if (this.enemy != null)
		{
			this.enemy.SetHP(hp);
		}
	}

	// Token: 0x06003062 RID: 12386 RVA: 0x0010703A File Offset: 0x0010523A
	public bool TrySetBehavior(GREnemyBossMoon.Behavior newBehavior)
	{
		if (newBehavior == GREnemyBossMoon.Behavior.Stagger)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x06003063 RID: 12387 RVA: 0x0010704C File Offset: 0x0010524C
	public void SetBehavior(GREnemyBossMoon.Behavior newBehavior, bool force = false)
	{
		if (newBehavior < GREnemyBossMoon.Behavior.HiddenIdle || newBehavior >= (GREnemyBossMoon.Behavior)this.abilities.Length)
		{
			Debug.LogErrorFormat("New Behavior Index is invalid {0} {1} {2}", new object[]
			{
				(int)newBehavior,
				newBehavior,
				base.gameObject.name
			});
			return;
		}
		GRAbilityBase grabilityBase = this.abilities[(int)newBehavior];
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		GREnemyBossMoon.Behavior behavior = this.currBehavior;
		if (behavior != GREnemyBossMoon.Behavior.AttackTongue)
		{
			if (behavior == GREnemyBossMoon.Behavior.NextPhase)
			{
				this.IncrementBossPhase();
			}
		}
		else
		{
			for (int i = 0; i < this.eyes.Count; i++)
			{
				this.eyes[i].ResetEye();
			}
			this.consecutiveCombos = 0;
			this.attacksAfterSummon = 0;
			this.currSummon = null;
			this.KillAllSummoned(true, true);
			if (this.triggerNextMusicTransition)
			{
				this.triggerNextMusicTransition = false;
				if (this.adaptiveMusicController != null)
				{
					this.adaptiveMusicController.TransitionToNextTrack();
				}
			}
		}
		Debug.LogFormat("Boss SetBehavior {0} -> {1}", new object[]
		{
			this.currBehavior,
			newBehavior
		});
		if (this.currAbility != null)
		{
			this.currAbility.Stop();
		}
		this.lastBehavior = this.currBehavior;
		this.currBehavior = newBehavior;
		this.currAbility = grabilityBase;
		if (this.currAbility != null)
		{
			this.currAbility.Start();
		}
		behavior = this.currBehavior;
		switch (behavior)
		{
		case GREnemyBossMoon.Behavior.Reveal:
			if (this.firstTimeReveal)
			{
				if (this.adaptiveMusicController != null)
				{
					this.adaptiveMusicController.Restart();
				}
				this.internalPhaseIndex = 0;
			}
			this.firstTimeReveal = false;
			this.BossHasRevealed = true;
			break;
		case GREnemyBossMoon.Behavior.Exposed:
			this.ToggleShockColliders(false);
			break;
		case GREnemyBossMoon.Behavior.ExposedIdle:
			break;
		case GREnemyBossMoon.Behavior.Stagger:
			this.lastStaggerTime = Time.time;
			break;
		case GREnemyBossMoon.Behavior.Dying:
			this.KillAllSummoned();
			this.TurnOffGrav();
			for (int j = 0; j < this.eyes.Count; j++)
			{
				this.eyes[j].TrySetBehavior(GREnemyBossMoonEye.Behavior.Dying);
			}
			if (this.adaptiveMusicController != null)
			{
				this.adaptiveMusicController.TransitionToLastTrack();
			}
			this.ToggleShockColliders(false);
			break;
		default:
			switch (behavior)
			{
			case GREnemyBossMoon.Behavior.AttackTongue:
				this.ToggleShockColliders(true);
				break;
			case GREnemyBossMoon.Behavior.Summon01:
			case GREnemyBossMoon.Behavior.Summon02:
			case GREnemyBossMoon.Behavior.Summon03:
			case GREnemyBossMoon.Behavior.Summon04:
				this.currSummon = (GRAbilitySummon)this.currAbility;
				break;
			case GREnemyBossMoon.Behavior.RetreatStart:
				this.TurnOnGrav();
				break;
			case GREnemyBossMoon.Behavior.RetreatEnd:
				this.TurnOffGrav();
				break;
			case GREnemyBossMoon.Behavior.Runaway:
				if (this.entity.manager.ghostReactorManager != null)
				{
					this.entity.manager.ghostReactorManager.InstantDeathForCurrentEnemies();
				}
				if (this.adaptiveMusicController != null)
				{
					this.adaptiveMusicController.TransitionToLastTrack();
				}
				break;
			}
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x06003064 RID: 12388 RVA: 0x00107358 File Offset: 0x00105558
	public void SetSquishVolumeState(bool squishEnabled)
	{
		for (int i = 0; i < this.squishVolumes.Count; i++)
		{
			this.squishVolumes[i].overrideDisabled = !squishEnabled;
			this.squishVolumes[i].SliceUpdate();
		}
	}

	// Token: 0x06003065 RID: 12389 RVA: 0x001073A4 File Offset: 0x001055A4
	private int CalcMaxHP()
	{
		float difficultyScalingForCurrentFloor = this.entity.manager.ghostReactorManager.reactor.difficultyScalingForCurrentFloor;
		int result = (int)((float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax) * difficultyScalingForCurrentFloor);
		for (int i = 0; i < this.phases.Count; i++)
		{
			this.phases[i].minHP = Mathf.RoundToInt((float)this.phases[i].minHP * difficultyScalingForCurrentFloor);
		}
		return result;
	}

	// Token: 0x06003066 RID: 12390 RVA: 0x00107420 File Offset: 0x00105620
	public int GetCurrPhaseIndex()
	{
		if (this.phases == null)
		{
			return -1;
		}
		for (int i = 0; i < this.phases.Count; i++)
		{
			if (this.hp > this.phases[i].minHP)
			{
				return i;
			}
		}
		return this.phases.Count - 1;
	}

	// Token: 0x06003067 RID: 12391 RVA: 0x00107478 File Offset: 0x00105678
	public GREnemyBossMoon.PhaseDef GetCurrPhase()
	{
		int currPhaseIndex = this.GetCurrPhaseIndex();
		if (currPhaseIndex < 0 || currPhaseIndex >= this.phases.Count)
		{
			return null;
		}
		return this.phases[currPhaseIndex];
	}

	// Token: 0x06003068 RID: 12392 RVA: 0x001074AC File Offset: 0x001056AC
	public void RestoreFullHealth()
	{
		this.SetHP(this.CalcMaxHP());
	}

	// Token: 0x06003069 RID: 12393 RVA: 0x001074BA File Offset: 0x001056BA
	public void HurtBossHP()
	{
		this.HurtBoss(100, this.entity.id, Vector3.zero);
	}

	// Token: 0x0600306A RID: 12394 RVA: 0x001074D4 File Offset: 0x001056D4
	public void KillAllEyes()
	{
		for (int i = 0; i < this.eyes.Count; i++)
		{
			this.eyes[i].InstantKill();
		}
	}

	// Token: 0x0600306B RID: 12395 RVA: 0x00107508 File Offset: 0x00105708
	public void KillAllSummoned()
	{
		this.KillAllSummoned(true, true);
	}

	// Token: 0x0600306C RID: 12396 RVA: 0x00107514 File Offset: 0x00105714
	public void KillAllSummoned(bool ignoreMonkeye = false, bool killAllEnemies = true)
	{
		int num = 0;
		for (int i = 0; i < this.trackedGameEntities.Count; i++)
		{
			if (!(this.trackedGameEntities[i] == null))
			{
				GREnemyChaser component = this.trackedGameEntities[i].GetComponent<GREnemyChaser>();
				if (component != null)
				{
					component.InstantDeath();
					num++;
				}
				else
				{
					GREnemyRanged component2 = this.trackedGameEntities[i].GetComponent<GREnemyRanged>();
					if (component2 != null)
					{
						component2.InstantDeath();
						num++;
					}
					else
					{
						GREnemyPest component3 = this.trackedGameEntities[i].GetComponent<GREnemyPest>();
						if (component3 != null)
						{
							component3.InstantDeath();
							num++;
						}
						else
						{
							GREnemySummoner component4 = this.trackedGameEntities[i].GetComponent<GREnemySummoner>();
							if (component4 != null)
							{
								component4.InstantDeath();
								num++;
							}
							else if (!ignoreMonkeye)
							{
								GREnemyMonkeye component5 = this.trackedGameEntities[i].GetComponent<GREnemyMonkeye>();
								if (component5 != null)
								{
									component5.InstantDeath();
									num++;
								}
							}
						}
					}
				}
			}
		}
		if (killAllEnemies && this.entity.manager.ghostReactorManager != null)
		{
			this.entity.manager.ghostReactorManager.InstantDeathForCurrentEnemies();
		}
		Debug.Log(string.Format("Report killed all summon {0}", num));
	}

	// Token: 0x0600306D RID: 12397 RVA: 0x0010766C File Offset: 0x0010586C
	public void GoBackPhase()
	{
		int currPhaseIndex = this.GetCurrPhaseIndex();
		if (currPhaseIndex <= 0)
		{
			Debug.LogWarning("GREnemyBossMoon - GoBackPhase - At first phase");
			return;
		}
		this.SetHP(this.phases[currPhaseIndex - 1].minHP);
	}

	// Token: 0x0600306E RID: 12398 RVA: 0x001076A8 File Offset: 0x001058A8
	public void GoToNextPhase()
	{
		int currPhaseIndex = this.GetCurrPhaseIndex();
		if (currPhaseIndex < 0 || currPhaseIndex >= this.phases.Count)
		{
			return;
		}
		this.SetHP(this.phases[currPhaseIndex].minHP);
	}

	// Token: 0x0600306F RID: 12399 RVA: 0x001076E8 File Offset: 0x001058E8
	private bool IsSummon(GREnemyBossMoon.Behavior behavior)
	{
		for (int i = 0; i < this.phases.Count; i++)
		{
			if (this.phases[i] != null && this.phases[i].summons != null && this.phases[i].summons.Contains(behavior))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003070 RID: 12400 RVA: 0x00107748 File Offset: 0x00105948
	private bool IsAnySummonBehavior(GREnemyBossMoon.Behavior behavior)
	{
		return this.currBehavior == GREnemyBossMoon.Behavior.SummonStart || this.currBehavior == GREnemyBossMoon.Behavior.SummonEnd || this.currBehavior == GREnemyBossMoon.Behavior.Summon01 || this.currBehavior == GREnemyBossMoon.Behavior.Summon02 || this.currBehavior == GREnemyBossMoon.Behavior.Summon03 || this.currBehavior == GREnemyBossMoon.Behavior.Summon04;
	}

	// Token: 0x06003071 RID: 12401 RVA: 0x00107788 File Offset: 0x00105988
	public GREnemyBossMoon.Behavior ChooseSummonForPhase()
	{
		GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
		if (currPhase == null)
		{
			return GREnemyBossMoon.Behavior.None;
		}
		return this.ChooseRandomBehavior(currPhase.summons);
	}

	// Token: 0x06003072 RID: 12402 RVA: 0x001077B0 File Offset: 0x001059B0
	public GREnemyBossMoon.Behavior ChooseAttackForPhase()
	{
		GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
		if (currPhase == null)
		{
			return GREnemyBossMoon.Behavior.None;
		}
		return this.ChooseRandomBehavior(currPhase.attacks);
	}

	// Token: 0x06003073 RID: 12403 RVA: 0x001077D8 File Offset: 0x001059D8
	public GREnemyBossMoon.Behavior ChooseRandomBehavior(List<GREnemyBossMoon.Behavior> behaviors)
	{
		if (behaviors == null || behaviors.Count <= 0)
		{
			return GREnemyBossMoon.Behavior.None;
		}
		int index = Random.Range(0, behaviors.Count);
		return behaviors[index];
	}

	// Token: 0x06003074 RID: 12404 RVA: 0x00107808 File Offset: 0x00105A08
	public void SetBodyState(GREnemyBossMoon.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		this.currBodyState = newBodyState;
		if (this.currBodyState == GREnemyBossMoon.BodyState.Destroyed)
		{
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
		}
		Debug.LogFormat("State Change {0} {1}", new object[]
		{
			this.entity.id.index,
			this.currBodyState
		});
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x06003075 RID: 12405 RVA: 0x00107898 File Offset: 0x00105A98
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyBossMoon.BodyState.Destroyed:
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyBossMoon.BodyState.Bones:
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyBossMoon.BodyState.Shell:
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003076 RID: 12406 RVA: 0x00107909 File Offset: 0x00105B09
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x06003077 RID: 12407 RVA: 0x00107918 File Offset: 0x00105B18
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyBossMoon.tempRigs.Clear();
		GREnemyBossMoon.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyBossMoon.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyBossMoon.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		if (this.currAbility != null)
		{
			this.currAbility.Think(dt);
		}
		GREnemyBossMoon.Behavior behavior = this.currBehavior;
		if (behavior != GREnemyBossMoon.Behavior.HiddenIdle)
		{
			if (behavior != GREnemyBossMoon.Behavior.Idle)
			{
				if (behavior != GREnemyBossMoon.Behavior.RetreatIdle)
				{
					return;
				}
				this.waitInRetreat += dt * 12f;
				if (this.trackedEntities.Count <= 0 || this.waitInRetreat > 20f)
				{
					this.TrySetBehavior(GREnemyBossMoon.Behavior.RetreatEnd);
				}
			}
			else if (this.currAbility.IsDone())
			{
				this.ChooseNewBehavior(false);
				return;
			}
			return;
		}
		this.ChooseNewBehavior(true);
	}

	// Token: 0x06003078 RID: 12408 RVA: 0x00107A1C File Offset: 0x00105C1C
	private GREnemyBossMoon.Behavior TryChooseAttackBehavior()
	{
		GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
		if (this.currBehavior == GREnemyBossMoon.Behavior.HiddenIdle)
		{
			if (currPhase != null && this.trackedEntities.Count <= currPhase.maxEnemiesForReveal && this.senseNearby.IsAnyoneNearby(this.abilityReveal.GetRange(), this.firstTimeReveal))
			{
				return GREnemyBossMoon.Behavior.Reveal;
			}
			return GREnemyBossMoon.Behavior.None;
		}
		else
		{
			if (GhostReactorManager.AggroDisabled)
			{
				return GREnemyBossMoon.Behavior.None;
			}
			if (currPhase == null)
			{
				return GREnemyBossMoon.Behavior.None;
			}
			if (currPhase.summons != null && currPhase.summons.Count > 0 && this.attacksAfterSummon <= 0 && this.trackedEntities.Count < currPhase.maxSimultaneousEnemies)
			{
				this.attacksAfterSummon = currPhase.attacksBetweenSummons;
				if (currPhase.summons.Count > 0)
				{
					this.currSummon = (GRAbilitySummon)this.abilities[(int)currPhase.summons[0]];
					if (this.currSummon != null)
					{
						for (int i = this.trackedEntities.Count; i < currPhase.maxSimultaneousEnemies; i++)
						{
							this.currSummon.ForceSpawn();
						}
					}
				}
			}
			List<GREnemyBossMoon.Behavior> list = currPhase.attacks;
			if (currPhase.comboAttacks != null && currPhase.comboAttacks.Count > 0 && ((currPhase.allowConsecutiveCombos && this.consecutiveCombos < 3) || this.consecutiveCombos <= 0) && Random.value < currPhase.comboAttackChance)
			{
				list = currPhase.comboAttacks;
				this.consecutiveCombos++;
			}
			else
			{
				this.consecutiveCombos = 0;
			}
			if (list != null && list.Count > 0)
			{
				GREnemyBossMoon.tempPotentialAttacks.Clear();
				for (int j = 0; j < list.Count; j++)
				{
					GREnemyBossMoon.tempPotentialAttacks.Add(list[j]);
				}
				for (int k = GREnemyBossMoon.tempPotentialAttacks.Count - 1; k >= 0; k--)
				{
					GRAbilityBase grabilityBase = this.abilities[(int)GREnemyBossMoon.tempPotentialAttacks[k]];
					if (grabilityBase == null || !this.senseNearby.IsAnyoneNearby(grabilityBase.GetRange(), false) || !grabilityBase.IsCoolDownOver())
					{
						GREnemyBossMoon.tempPotentialAttacks.RemoveAt(k);
					}
				}
				if (GREnemyBossMoon.tempPotentialAttacks.Count > 0)
				{
					this.attacksAfterSummon--;
					int index = Random.Range(0, GREnemyBossMoon.tempPotentialAttacks.Count);
					return GREnemyBossMoon.tempPotentialAttacks[index];
				}
			}
			return GREnemyBossMoon.Behavior.None;
		}
	}

	// Token: 0x06003079 RID: 12409 RVA: 0x00107C5C File Offset: 0x00105E5C
	private bool AreAllEyesClosed()
	{
		for (int i = 0; i < this.eyes.Count; i++)
		{
			if (this.eyes[i].hp > 0)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600307A RID: 12410 RVA: 0x00107C96 File Offset: 0x00105E96
	public void GotoDyingIdle()
	{
		this.SetBehavior(GREnemyBossMoon.Behavior.DyingIdle, true);
	}

	// Token: 0x0600307B RID: 12411 RVA: 0x00107CA4 File Offset: 0x00105EA4
	private void ChooseNewBehavior(bool forceAttack = false)
	{
		if (this.hp <= 0)
		{
			this.TrySetBehavior(GREnemyBossMoon.Behavior.Dying);
			return;
		}
		if (this.AreAllEyesClosed())
		{
			if (this.eyesPushVolume != null)
			{
				this.eyesPushVolume.Trigger();
			}
			this.TrySetBehavior(GREnemyBossMoon.Behavior.Exposed);
			return;
		}
		if (forceAttack || !this.restAfterAttack)
		{
			this.restAfterAttack = false;
			GREnemyBossMoon.Behavior behavior = this.TryChooseAttackBehavior();
			if (behavior != GREnemyBossMoon.Behavior.None)
			{
				if (this.TrySetBehavior(behavior) && this.currBehavior != GREnemyBossMoon.Behavior.AttackTongue)
				{
					GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
					this.restAfterAttack = currPhase.restAfterAttack;
				}
				if (this.currSummon != null)
				{
					GREnemyBossMoon.PhaseDef currPhase2 = this.GetCurrPhase();
					if (this.trackedEntities.Count < currPhase2.maxSimultaneousEnemies && Random.value < currPhase2.randomSummonChance)
					{
						this.currSummon.ForceSpawn();
					}
				}
				return;
			}
		}
		if (this.currBehavior == GREnemyBossMoon.Behavior.None)
		{
			this.restAfterAttack = false;
			this.TrySetBehavior(GREnemyBossMoon.Behavior.Idle);
		}
	}

	// Token: 0x0600307C RID: 12412 RVA: 0x00107D86 File Offset: 0x00105F86
	private void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x0600307D RID: 12413 RVA: 0x00107DA4 File Offset: 0x00105FA4
	private void OnUpdateAuthority(float dt)
	{
		if (this.currBehavior == GREnemyBossMoon.Behavior.Runaway)
		{
			this.currAbility.UpdateAuthority(dt);
			return;
		}
		if (this.currBehavior == GREnemyBossMoon.Behavior.ExposedIdle)
		{
			GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
			if (this.hp <= 0)
			{
				this.SetBehavior(GREnemyBossMoon.Behavior.Dying, false);
			}
			else if (this.hp <= currPhase.minHP)
			{
				this.SetBehavior(GREnemyBossMoon.Behavior.AttackTongue, false);
			}
		}
		if (this.currAbility != null)
		{
			this.currAbility.UpdateAuthority(dt);
			GREnemyBossMoon.PhaseDef currPhase2 = this.GetCurrPhase();
			if (this.currAbility.IsDone())
			{
				if (this.currBehavior == GREnemyBossMoon.Behavior.NextPhase)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.AttackTongue, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.Exposed)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.ExposedIdle, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.SummonStart)
				{
					GREnemyBossMoon.Behavior newBehavior = this.ChooseSummonForPhase();
					this.SetBehavior(newBehavior, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.SummonEnd && currPhase2.retreatAfterSummon)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.RetreatStart, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.RetreatStart)
				{
					this.waitInRetreat = 0f;
					this.SetBehavior(GREnemyBossMoon.Behavior.RetreatIdle, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.RetreatIdle)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.RetreatEnd, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.ExposedIdle)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.AttackTongue, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.AttackTongue)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.HiddenIdle, false);
					return;
				}
				if (!this.IsSummon(this.currBehavior))
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.None, false);
					this.ChooseNewBehavior(false);
					return;
				}
				if (currPhase2 == null || this.trackedEntities.Count >= currPhase2.maxSimultaneousEnemies)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.SummonEnd, false);
					return;
				}
				this.SetBehavior(GREnemyBossMoon.Behavior.None, false);
				GREnemyBossMoon.Behavior newBehavior2 = this.ChooseSummonForPhase();
				this.SetBehavior(newBehavior2, false);
				return;
			}
			else if (this.AreAllEyesClosed() && this.currBehavior != GREnemyBossMoon.Behavior.Exposed && this.currBehavior != GREnemyBossMoon.Behavior.ExposedIdle && this.lastBehavior != GREnemyBossMoon.Behavior.Exposed && this.lastBehavior != GREnemyBossMoon.Behavior.ExposedIdle)
			{
				this.TrySetBehavior(GREnemyBossMoon.Behavior.Exposed);
			}
		}
	}

	// Token: 0x0600307E RID: 12414 RVA: 0x00107F71 File Offset: 0x00106171
	private void OnUpdateRemote(float dt)
	{
		if (this.currAbility != null)
		{
			this.currAbility.UpdateRemote(dt);
		}
	}

	// Token: 0x0600307F RID: 12415 RVA: 0x00107F87 File Offset: 0x00106187
	private void CatchUpPhase(int phase)
	{
		this.BossHasRevealed = true;
		this.internalPhaseIndex = phase;
		this.AdjustByPhaseIndex(phase);
		if (this.adaptiveMusicController != null)
		{
			this.adaptiveMusicController.RestartAt(phase);
		}
	}

	// Token: 0x06003080 RID: 12416 RVA: 0x00107FB8 File Offset: 0x001061B8
	private void IncrementBossPhase()
	{
		this.internalPhaseIndex++;
		this.triggerNextMusicTransition = true;
		this.AdjustByPhaseIndex(this.internalPhaseIndex);
		Debug.Log(string.Format("Incrementing phase to phase {0}!", this.internalPhaseIndex));
	}

	// Token: 0x06003081 RID: 12417 RVA: 0x00107FF8 File Offset: 0x001061F8
	private void SyncPhase(int phase)
	{
		this.internalPhaseIndex = phase;
		if (this.adaptiveMusicController != null)
		{
			this.adaptiveMusicController.GoToTrack(this.internalPhaseIndex, false);
		}
		this.AdjustByPhaseIndex(this.internalPhaseIndex);
		Debug.Log(string.Format("Syncing phase to phase {0}!", this.internalPhaseIndex));
	}

	// Token: 0x06003082 RID: 12418 RVA: 0x00108054 File Offset: 0x00106254
	private void AdjustByPhaseIndex(int phase)
	{
		switch (this.internalPhaseIndex)
		{
		case 1:
			this.abilityIdle.SpeedUp(3f);
			this.AdjustAttackAnimSpeed(1.2f);
			return;
		case 2:
			this.abilityIdle.SpeedUp(4f);
			this.AdjustAttackAnimSpeed(1.4f);
			return;
		case 3:
			this.abilityIdle.SpeedUp(4f);
			this.AdjustAttackAnimSpeed(1.6f);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003083 RID: 12419 RVA: 0x001080D0 File Offset: 0x001062D0
	private void AdjustAttackAnimSpeed(float speed)
	{
		this.abilityAttackTentacle00.attackAnimData.speed = speed;
		this.abilityAttackTentacle01.attackAnimData.speed = speed;
		this.abilityAttackTentacle02.attackAnimData.speed = speed;
		this.abilityAttackTentacle03.attackAnimData.speed = speed;
		this.abilityAttackTentacle04.attackAnimData.speed = speed;
		this.abilityAttackTentacle05.attackAnimData.speed = speed;
	}

	// Token: 0x06003084 RID: 12420 RVA: 0x00108143 File Offset: 0x00106343
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		this.HurtBoss(hit.hitAmount, hit.hitEntityId, tool.transform.position);
	}

	// Token: 0x06003085 RID: 12421 RVA: 0x00108164 File Offset: 0x00106364
	private void HurtBoss(int hitAmount, GameEntityId hitByEntityId, Vector3 toolPosition)
	{
		if (this.currBehavior == GREnemyBossMoon.Behavior.Dying || this.currBehavior == GREnemyBossMoon.Behavior.DyingIdle || this.currBehavior == GREnemyBossMoon.Behavior.Runaway || this.IsAnySummonBehavior(this.currBehavior))
		{
			return;
		}
		if (this.currBodyState == GREnemyBossMoon.BodyState.Bones)
		{
			int num = this.hp;
			GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
			this.SetHP(this.hp - hitAmount);
			if (this.damagedSounds.Count > 0)
			{
				this.damagedSoundIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.damagedSounds.Count, this.damagedSoundIndex);
				this.audioSource.PlayOneShot(this.damagedSounds[this.damagedSoundIndex], this.damagedSoundVolume);
			}
			if (this.fxDamaged != null)
			{
				this.fxDamaged.SetActive(false);
				this.fxDamaged.SetActive(true);
			}
			if (this.hp <= 0)
			{
				if (hitByEntityId != GameEntityId.Invalid)
				{
					this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hitByEntityId));
				}
				this.SetBodyState(GREnemyBossMoon.BodyState.Destroyed, false);
				this.SetBehavior(GREnemyBossMoon.Behavior.Dying, false);
				return;
			}
			if (num > currPhase.minHP && this.hp <= currPhase.minHP)
			{
				if (currPhase.runawayAfterPhase)
				{
					Debug.Log("Force runaway!");
					if (hitByEntityId != GameEntityId.Invalid)
					{
						this.abilityRunaway.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hitByEntityId));
					}
					this.SetBehavior(GREnemyBossMoon.Behavior.Runaway, false);
				}
				else
				{
					Debug.Log("Force next phase transition!");
					this.SetBehavior(GREnemyBossMoon.Behavior.NextPhase, false);
				}
			}
			this.lastSeenTargetPosition = toolPosition;
			this.lastSeenTargetTime = Time.timeAsDouble;
			Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
			vector.y = 0f;
			this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
		}
	}

	// Token: 0x06003086 RID: 12422 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
	}

	// Token: 0x06003087 RID: 12423 RVA: 0x00108338 File Offset: 0x00106538
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x06003088 RID: 12424 RVA: 0x00108344 File Offset: 0x00106544
	public void ReportDeathStat()
	{
		if (this.currAbility != null)
		{
			GRAbilityDie grabilityDie = this.currAbility as GRAbilityDie;
			if (grabilityDie != null)
			{
				grabilityDie.ReportDeathStat();
			}
		}
	}

	// Token: 0x06003089 RID: 12425 RVA: 0x0010836E File Offset: 0x0010656E
	private bool IsAttackBehavior(GREnemyBossMoon.Behavior behavior)
	{
		return behavior == GREnemyBossMoon.Behavior.AttackTentacle00 || behavior == GREnemyBossMoon.Behavior.AttackTentacle01 || behavior == GREnemyBossMoon.Behavior.AttackTentacle02 || behavior == GREnemyBossMoon.Behavior.AttackTentacle03 || behavior == GREnemyBossMoon.Behavior.AttackTentacle04 || behavior == GREnemyBossMoon.Behavior.AttackTentacle05 || behavior == GREnemyBossMoon.Behavior.AttackQuickTentacle00 || behavior == GREnemyBossMoon.Behavior.AttackQuickTentacle01 || behavior == GREnemyBossMoon.Behavior.AttackQuickTentacle02 || behavior == GREnemyBossMoon.Behavior.AttackQuickTentacle03 || behavior == GREnemyBossMoon.Behavior.AttackTongue || behavior == GREnemyBossMoon.Behavior.AttackTongueSwipe;
	}

	// Token: 0x0600308A RID: 12426 RVA: 0x001083AC File Offset: 0x001065AC
	[CanBeNull]
	private GRAbilityBase GetAssociatedAbilityForBehavior(GREnemyBossMoon.Behavior behavior)
	{
		switch (behavior)
		{
		case GREnemyBossMoon.Behavior.AttackTentacle00:
			return this.abilityAttackTentacle00;
		case GREnemyBossMoon.Behavior.AttackTentacle01:
			return this.abilityAttackTentacle01;
		case GREnemyBossMoon.Behavior.AttackTentacle02:
			return this.abilityAttackTentacle02;
		case GREnemyBossMoon.Behavior.AttackTentacle03:
			return this.abilityAttackTentacle03;
		case GREnemyBossMoon.Behavior.AttackTentacle04:
			return this.abilityAttackTentacle04;
		case GREnemyBossMoon.Behavior.AttackTentacle05:
			return this.abilityAttackTentacle05;
		case GREnemyBossMoon.Behavior.AttackQuickTentacle00:
			return this.abilityAttackQuickTentacle00;
		case GREnemyBossMoon.Behavior.AttackQuickTentacle01:
			return this.abilityAttackQuickTentacle01;
		case GREnemyBossMoon.Behavior.AttackQuickTentacle02:
			return this.abilityAttackQuickTentacle02;
		case GREnemyBossMoon.Behavior.AttackQuickTentacle03:
			return this.abilityAttackQuickTentacle03;
		case GREnemyBossMoon.Behavior.AttackTongue:
			return this.abilityAttackTongue01;
		case GREnemyBossMoon.Behavior.AttackTongueSwipe:
			return this.abilityAttackTongueSwipe01;
		}
		return null;
	}

	// Token: 0x0600308B RID: 12427 RVA: 0x00108490 File Offset: 0x00106690
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyBossMoon.BodyState.Destroyed)
		{
			return;
		}
		if (!this.IsAttackBehavior(this.currBehavior))
		{
			return;
		}
		if (collider.isTrigger)
		{
			return;
		}
		GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
		if (component != null)
		{
			GameHittable component2 = base.GetComponent<GameHittable>();
			component.BlockHittable(this.headTransform.position, base.transform.forward, component2);
			return;
		}
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component3 = attachedRigidbody.GetComponent<GRPlayer>();
			if (component3 == null)
			{
				GorillaTagger component4 = attachedRigidbody.GetComponent<GorillaTagger>();
				if (component4 != null && component4.offlineVRRig != null)
				{
					component3 = component4.offlineVRRig.GetComponent<GRPlayer>();
				}
			}
			if (component3 != null && component3.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
			{
				this.HitPlayer(component3, false);
			}
			GRBreakable component5 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component6 = attachedRigidbody.GetComponent<GameHittable>();
			if (component5 != null && component6 != null)
			{
				GameHitData hitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component6.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component5.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component5.transform.position,
					hittablePoint = component6.FindHittablePoint(collider)
				};
				component6.RequestHit(hitData);
			}
		}
	}

	// Token: 0x0600308C RID: 12428 RVA: 0x00108623 File Offset: 0x00106823
	private void TurnOnGrav()
	{
		if (this.currentGravActivator != null)
		{
			return;
		}
		this.currentGravActivator = this.gravActivators[Random.Range(0, this.gravActivators.Length)];
		this.currentGravActivator.SetActive(true);
	}

	// Token: 0x0600308D RID: 12429 RVA: 0x0010865B File Offset: 0x0010685B
	private void TurnOffGrav()
	{
		if (this.currentGravActivator == null)
		{
			return;
		}
		this.currentGravActivator.SetActive(false);
		this.currentGravActivator = null;
	}

	// Token: 0x0600308E RID: 12430 RVA: 0x0010867F File Offset: 0x0010687F
	[ContextMenu("Debug Hit Player")]
	private void DebugHitPlayer()
	{
		this.HitPlayer(VRRig.LocalRig.GetComponent<GRPlayer>(), true);
	}

	// Token: 0x0600308F RID: 12431 RVA: 0x00108692 File Offset: 0x00106892
	public void HitPlayer(GRPlayer player, bool useImpulse = false)
	{
		if (this.currBodyState == GREnemyBossMoon.BodyState.Destroyed || this.tryHitPlayerCoroutine != null)
		{
			base.StopCoroutine(this.tryHitPlayerCoroutine);
		}
		this.tryHitPlayerCoroutine = base.StartCoroutine(this.TryHitPlayer(player, useImpulse));
	}

	// Token: 0x06003090 RID: 12432 RVA: 0x001086C4 File Offset: 0x001068C4
	private IEnumerator TryHitPlayer(GRPlayer player, bool useImpulse = false)
	{
		yield return new WaitForUpdate();
		if (player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			ICustomKnockbackAbility customKnockbackAbility = this.GetAssociatedAbilityForBehavior(this.currBehavior) as ICustomKnockbackAbility;
			Vector3 vector2;
			if (customKnockbackAbility != null)
			{
				Vector3? vector = customKnockbackAbility.CalculateImpulse(player.transform);
				if (vector != null)
				{
					Vector3 valueOrDefault = vector.GetValueOrDefault();
					vector2 = valueOrDefault;
					goto IL_F4;
				}
			}
			vector2 = (player.transform.position - this.knockbackTransform.position).normalized * this.knockbackImpulse;
			IL_F4:
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position, vector2);
			this.cameraShaker.Shake();
			float magnitude = vector2.magnitude;
			GorillaTagger.Instance.StartVibration(true, magnitude, 0.333f);
			GorillaTagger.Instance.StartVibration(false, magnitude, 0.333f);
			if (useImpulse)
			{
				GTPlayer.Instance.ApplyKnockback(vector2 / magnitude, magnitude, true);
			}
		}
		yield break;
	}

	// Token: 0x06003091 RID: 12433 RVA: 0x001086E1 File Offset: 0x001068E1
	public void ShockPlayer()
	{
		if (this.currBodyState == GREnemyBossMoon.BodyState.Destroyed || this.tryShockPlayerCoroutine != null)
		{
			return;
		}
		this.tryShockPlayerCoroutine = base.StartCoroutine(this.TryShockPlayer());
	}

	// Token: 0x06003092 RID: 12434 RVA: 0x00108706 File Offset: 0x00106906
	private IEnumerator TryShockPlayer()
	{
		this.bodyRenderer.sharedMaterials = this.shockedBodyMaterials;
		yield return new WaitForSecondsRealtime(1f);
		this.bodyRenderer.sharedMaterials = this.defaultBodyMaterials;
		this.tryShockPlayerCoroutine = null;
		yield break;
	}

	// Token: 0x06003093 RID: 12435 RVA: 0x00108718 File Offset: 0x00106918
	private void ToggleShockColliders(bool toggle)
	{
		for (int i = 0; i < this.shockColliders.Count; i++)
		{
			this.shockColliders[i].enabled = toggle;
		}
	}

	// Token: 0x06003094 RID: 12436 RVA: 0x0010874D File Offset: 0x0010694D
	public void GroundSlamWeak(Transform slamCenter)
	{
		this._GroundSlam(slamCenter, 0.1f, 6f, 5f);
	}

	// Token: 0x06003095 RID: 12437 RVA: 0x00108765 File Offset: 0x00106965
	public void GroundSlam(Transform slamCenter)
	{
		this._GroundSlam(slamCenter, 1f, 11f, 8f);
	}

	// Token: 0x06003096 RID: 12438 RVA: 0x00108780 File Offset: 0x00106980
	public void _GroundSlam(Transform slamCenter, float duration, float distance, float hitVelocity)
	{
		GREnemyBossMoon.<_GroundSlam>d__173 <_GroundSlam>d__;
		<_GroundSlam>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<_GroundSlam>d__.<>4__this = this;
		<_GroundSlam>d__.slamCenter = slamCenter;
		<_GroundSlam>d__.duration = duration;
		<_GroundSlam>d__.distance = distance;
		<_GroundSlam>d__.hitVelocity = hitVelocity;
		<_GroundSlam>d__.<>1__state = -1;
		<_GroundSlam>d__.<>t__builder.Start<GREnemyBossMoon.<_GroundSlam>d__173>(ref <_GroundSlam>d__);
	}

	// Token: 0x06003097 RID: 12439 RVA: 0x001087D8 File Offset: 0x001069D8
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Concat(new string[]
		{
			"<color=\"white\">State:</color> <color=\"yellow\">",
			this.currBehavior.ToString(),
			"</color>\n",
			string.Format("<color=\"white\">Phase:</color> <color=\"yellow\">{0}</color>\n", this.GetCurrPhaseIndex()),
			string.Format("<color=\"white\">HP:</color> <color=\"yellow\">{0}</color>", this.hp)
		}));
	}

	// Token: 0x06003098 RID: 12440 RVA: 0x00108854 File Offset: 0x00106A54
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		byte value2 = (byte)this.currBodyState;
		int value3 = (this.targetPlayer == null) ? -1 : this.targetPlayer.ActorNumber;
		writer.Write(value);
		writer.Write(value2);
		writer.Write(this.hp);
		writer.Write(value3);
		writer.Write(this.internalPhaseIndex);
	}

	// Token: 0x06003099 RID: 12441 RVA: 0x001088B8 File Offset: 0x00106AB8
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyBossMoon.Behavior newBehavior = (GREnemyBossMoon.Behavior)reader.ReadByte();
		GREnemyBossMoon.BodyState newBodyState = (GREnemyBossMoon.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		int playerID = reader.ReadInt32();
		int num2 = reader.ReadInt32();
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
		if (num2 != -1)
		{
			if (this.internalPhaseIndex == -1)
			{
				Debug.Log(string.Format("Catching up to boss phase {0}.", num2));
				this.CatchUpPhase(num2);
				return;
			}
			if (num2 != this.internalPhaseIndex)
			{
				Debug.Log(string.Format("Syncing up to boss phase {0}.", this.internalPhaseIndex));
				this.SyncPhase(num2);
			}
		}
	}

	// Token: 0x0600309A RID: 12442 RVA: 0x00023994 File Offset: 0x00021B94
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x0600309B RID: 12443 RVA: 0x0010896C File Offset: 0x00106B6C
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		GRTool gameComponent = this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId);
		if (gameComponent != null)
		{
			switch (hitTypeId)
			{
			case GameHitType.Club:
				this.OnHitByClub(gameComponent, hit);
				return;
			case GameHitType.Flash:
				this.OnHitByFlash(gameComponent, hit);
				return;
			case GameHitType.Shield:
				this.OnHitByShield(gameComponent, hit);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x0600309C RID: 12444 RVA: 0x001089D0 File Offset: 0x00106BD0
	private void AddTrackedEntity(GameEntity entityToTrack)
	{
		int netId = entityToTrack.GetNetId();
		this.trackedEntities.AddIfNew(netId);
		if (!this.trackedGameEntities.Contains(entityToTrack))
		{
			this.trackedGameEntities.Add(entityToTrack);
		}
	}

	// Token: 0x0600309D RID: 12445 RVA: 0x00108A0C File Offset: 0x00106C0C
	private void RemoveTrackedEntity(GameEntity entityToRemove)
	{
		int netId = entityToRemove.GetNetId();
		if (this.trackedEntities.Contains(netId))
		{
			this.trackedEntities.Remove(netId);
		}
		if (this.trackedGameEntities.Contains(entityToRemove))
		{
			this.trackedGameEntities.Remove(entityToRemove);
		}
	}

	// Token: 0x0600309E RID: 12446 RVA: 0x00108A56 File Offset: 0x00106C56
	public void OnSummonedEntityInit(GameEntity entity)
	{
		this.AddTrackedEntity(entity);
	}

	// Token: 0x0600309F RID: 12447 RVA: 0x00108A5F File Offset: 0x00106C5F
	public void OnSummonedEntityDestroy(GameEntity entity)
	{
		this.RemoveTrackedEntity(entity);
	}

	// Token: 0x04003DDE RID: 15838
	public GameEntity entity;

	// Token: 0x04003DDF RID: 15839
	public GameAgent agent;

	// Token: 0x04003DE0 RID: 15840
	public GREnemy enemy;

	// Token: 0x04003DE1 RID: 15841
	public GameHittable hittable;

	// Token: 0x04003DE2 RID: 15842
	[SerializeField]
	private GRAttributes attributes;

	// Token: 0x04003DE3 RID: 15843
	public List<GREnemyBossMoon.PhaseDef> phases;

	// Token: 0x04003DE4 RID: 15844
	private int internalPhaseIndex = -1;

	// Token: 0x04003DE5 RID: 15845
	public List<GREnemyBossMoon.LootPhase> lootPhases;

	// Token: 0x04003DE6 RID: 15846
	public GRSenseNearby senseNearby;

	// Token: 0x04003DE7 RID: 15847
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003DE8 RID: 15848
	public List<GREnemyBossMoonEye> eyes;

	// Token: 0x04003DE9 RID: 15849
	public GRSpherePushVolume eyesPushVolume;

	// Token: 0x04003DEA RID: 15850
	public Animation anim;

	// Token: 0x04003DEB RID: 15851
	public GRAbilityIdle abilityReveal;

	// Token: 0x04003DEC RID: 15852
	private bool firstTimeReveal = true;

	// Token: 0x04003DEE RID: 15854
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003DEF RID: 15855
	public GRAbilityIdle abilityHiddenIdle;

	// Token: 0x04003DF0 RID: 15856
	public GRBossMoonTentacleAttack abilityAttackTentacle00;

	// Token: 0x04003DF1 RID: 15857
	public GRBossMoonTentacleAttack abilityAttackTentacle01;

	// Token: 0x04003DF2 RID: 15858
	public GRBossMoonTentacleAttack abilityAttackTentacle02;

	// Token: 0x04003DF3 RID: 15859
	public GRBossMoonTentacleAttack abilityAttackTentacle03;

	// Token: 0x04003DF4 RID: 15860
	public GRBossMoonTentacleAttack abilityAttackTentacle04;

	// Token: 0x04003DF5 RID: 15861
	public GRBossMoonTentacleAttack abilityAttackTentacle05;

	// Token: 0x04003DF6 RID: 15862
	public GRBossMoonTentacleAttack abilityAttackQuickTentacle00;

	// Token: 0x04003DF7 RID: 15863
	public GRBossMoonTentacleAttack abilityAttackQuickTentacle01;

	// Token: 0x04003DF8 RID: 15864
	public GRBossMoonTentacleAttack abilityAttackQuickTentacle02;

	// Token: 0x04003DF9 RID: 15865
	public GRBossMoonTentacleAttack abilityAttackQuickTentacle03;

	// Token: 0x04003DFA RID: 15866
	public GRBossMoonTentacleAttack abilityAttackTongue01;

	// Token: 0x04003DFB RID: 15867
	public GRBossMoonTentacleAttack abilityAttackTongueSwipe01;

	// Token: 0x04003DFC RID: 15868
	public GRAbilityIdle abilitySummonStart;

	// Token: 0x04003DFD RID: 15869
	public GRAbilityIdle abilitySummonEnd;

	// Token: 0x04003DFE RID: 15870
	public GRAbilitySummon abilitySummon01;

	// Token: 0x04003DFF RID: 15871
	public GRAbilitySummon abilitySummon02;

	// Token: 0x04003E00 RID: 15872
	public GRAbilitySummon abilitySummon03;

	// Token: 0x04003E01 RID: 15873
	public GRAbilitySummon abilitySummon04;

	// Token: 0x04003E02 RID: 15874
	public GRAbilityIdle abilityRetreatStart;

	// Token: 0x04003E03 RID: 15875
	public GRAbilityIdle abilityRetreatEnd;

	// Token: 0x04003E04 RID: 15876
	public GRAbilityIdle abilityRetreatIdle;

	// Token: 0x04003E05 RID: 15877
	public GRAbilityIdle abilityExposed;

	// Token: 0x04003E06 RID: 15878
	public GRAbilityIdle abilityExposedIdle;

	// Token: 0x04003E07 RID: 15879
	public GRAbilityDie abilityDie;

	// Token: 0x04003E08 RID: 15880
	public GRAbilityDie abilityDieIdle;

	// Token: 0x04003E09 RID: 15881
	public GRAbilityDie abilityRunaway;

	// Token: 0x04003E0A RID: 15882
	public GRAbilityIdle abilityNextPhase;

	// Token: 0x04003E0B RID: 15883
	private GRAbilityBase[] abilities;

	// Token: 0x04003E0C RID: 15884
	private GRAbilityBase currAbility;

	// Token: 0x04003E0D RID: 15885
	private GRAbilitySummon currSummon;

	// Token: 0x04003E0E RID: 15886
	public GRAbilityAgent abilityAgent;

	// Token: 0x04003E0F RID: 15887
	public List<Renderer> bones;

	// Token: 0x04003E10 RID: 15888
	public List<Renderer> always;

	// Token: 0x04003E11 RID: 15889
	public Transform headTransform;

	// Token: 0x04003E12 RID: 15890
	public AudioSource audioSource;

	// Token: 0x04003E13 RID: 15891
	public AudioClip damagedSound;

	// Token: 0x04003E14 RID: 15892
	public float damagedSoundVolume;

	// Token: 0x04003E15 RID: 15893
	public List<AudioClip> damagedSounds;

	// Token: 0x04003E16 RID: 15894
	private int damagedSoundIndex;

	// Token: 0x04003E17 RID: 15895
	public GameObject fxDamaged;

	// Token: 0x04003E18 RID: 15896
	public GameObject[] gravActivators;

	// Token: 0x04003E19 RID: 15897
	private GameObject currentGravActivator;

	// Token: 0x04003E1A RID: 15898
	public Renderer bodyRenderer;

	// Token: 0x04003E1B RID: 15899
	public Material[] defaultBodyMaterials;

	// Token: 0x04003E1C RID: 15900
	public Material[] shockedBodyMaterials;

	// Token: 0x04003E1D RID: 15901
	private float lastStaggerTime;

	// Token: 0x04003E1E RID: 15902
	public float staggerImmuneTime = 10f;

	// Token: 0x04003E1F RID: 15903
	private Transform target;

	// Token: 0x04003E20 RID: 15904
	[ReadOnly]
	public int hp;

	// Token: 0x04003E21 RID: 15905
	[ReadOnly]
	public GREnemyBossMoon.Behavior currBehavior;

	// Token: 0x04003E22 RID: 15906
	[ReadOnly]
	public GREnemyBossMoon.BodyState currBodyState;

	// Token: 0x04003E23 RID: 15907
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003E24 RID: 15908
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x04003E25 RID: 15909
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x04003E26 RID: 15910
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x04003E27 RID: 15911
	private GREnemyBossMoon.Behavior lastBehavior;

	// Token: 0x04003E28 RID: 15912
	private bool restAfterAttack;

	// Token: 0x04003E29 RID: 15913
	private int consecutiveCombos;

	// Token: 0x04003E2A RID: 15914
	private int attacksAfterSummon = 3;

	// Token: 0x04003E2B RID: 15915
	private float waitInRetreat;

	// Token: 0x04003E2C RID: 15916
	private double lastJumpEndtime;

	// Token: 0x04003E2D RID: 15917
	public bool canChaseJump = true;

	// Token: 0x04003E2E RID: 15918
	public float chaseJumpDistance = 5f;

	// Token: 0x04003E2F RID: 15919
	public float chaseJumpMinInterval = 1f;

	// Token: 0x04003E30 RID: 15920
	public float minChaseJumpDistance = 2f;

	// Token: 0x04003E31 RID: 15921
	public float knockbackImpulse = 11f;

	// Token: 0x04003E32 RID: 15922
	public Transform knockbackTransform;

	// Token: 0x04003E33 RID: 15923
	private Rigidbody rigidBody;

	// Token: 0x04003E34 RID: 15924
	private List<Collider> colliders;

	// Token: 0x04003E35 RID: 15925
	private float lastHitPlayerTime;

	// Token: 0x04003E36 RID: 15926
	private float minTimeBetweenHits = 2f;

	// Token: 0x04003E37 RID: 15927
	public float hearingRadius = 5f;

	// Token: 0x04003E38 RID: 15928
	public List<GREnemyBossMoonColliderHelper> shockColliders;

	// Token: 0x04003E39 RID: 15929
	public List<GRSquishVolume> squishVolumes;

	// Token: 0x04003E3A RID: 15930
	public CameraShakeDispatcher cameraShaker;

	// Token: 0x04003E3B RID: 15931
	private List<int> trackedEntities;

	// Token: 0x04003E3C RID: 15932
	private List<GameEntity> trackedGameEntities;

	// Token: 0x04003E3D RID: 15933
	private GRAdaptiveMusicController adaptiveMusicController;

	// Token: 0x04003E3E RID: 15934
	private bool triggerNextMusicTransition;

	// Token: 0x04003E3F RID: 15935
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04003E40 RID: 15936
	private static List<GREnemyBossMoon.Behavior> tempPotentialAttacks = new List<GREnemyBossMoon.Behavior>(16);

	// Token: 0x04003E41 RID: 15937
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x04003E42 RID: 15938
	private Coroutine tryShockPlayerCoroutine;

	// Token: 0x02000777 RID: 1911
	[Serializable]
	public class PhaseDef
	{
		// Token: 0x04003E43 RID: 15939
		public int minHP;

		// Token: 0x04003E44 RID: 15940
		public List<GREnemyBossMoon.Behavior> attacks;

		// Token: 0x04003E45 RID: 15941
		public List<GREnemyBossMoon.Behavior> comboAttacks;

		// Token: 0x04003E46 RID: 15942
		public bool restAfterAttack = true;

		// Token: 0x04003E47 RID: 15943
		public float comboAttackChance = 0.25f;

		// Token: 0x04003E48 RID: 15944
		public bool allowConsecutiveCombos;

		// Token: 0x04003E49 RID: 15945
		public List<GREnemyBossMoon.Behavior> summons;

		// Token: 0x04003E4A RID: 15946
		public int maxSimultaneousEnemies = 6;

		// Token: 0x04003E4B RID: 15947
		public int maxEnemiesForReveal = 4;

		// Token: 0x04003E4C RID: 15948
		public int attacksBetweenSummons = 4;

		// Token: 0x04003E4D RID: 15949
		public bool retreatAfterSummon = true;

		// Token: 0x04003E4E RID: 15950
		public float randomSummonChance = 0.1f;

		// Token: 0x04003E4F RID: 15951
		public bool runawayAfterPhase;
	}

	// Token: 0x02000778 RID: 1912
	[Serializable]
	public class LootPhase
	{
		// Token: 0x04003E50 RID: 15952
		public GREnemyType enemyType;

		// Token: 0x04003E51 RID: 15953
		public GRBreakableItemSpawnConfig lootTable;
	}

	// Token: 0x02000779 RID: 1913
	public enum Behavior
	{
		// Token: 0x04003E53 RID: 15955
		HiddenIdle,
		// Token: 0x04003E54 RID: 15956
		Idle,
		// Token: 0x04003E55 RID: 15957
		Reveal,
		// Token: 0x04003E56 RID: 15958
		Exposed,
		// Token: 0x04003E57 RID: 15959
		ExposedIdle,
		// Token: 0x04003E58 RID: 15960
		Stagger,
		// Token: 0x04003E59 RID: 15961
		Dying,
		// Token: 0x04003E5A RID: 15962
		AttackTentacle00,
		// Token: 0x04003E5B RID: 15963
		AttackTentacle01,
		// Token: 0x04003E5C RID: 15964
		AttackTentacle02,
		// Token: 0x04003E5D RID: 15965
		AttackTentacle03,
		// Token: 0x04003E5E RID: 15966
		AttackTentacle04,
		// Token: 0x04003E5F RID: 15967
		AttackTentacle05,
		// Token: 0x04003E60 RID: 15968
		AttackQuickTentacle00,
		// Token: 0x04003E61 RID: 15969
		AttackQuickTentacle01,
		// Token: 0x04003E62 RID: 15970
		AttackQuickTentacle02,
		// Token: 0x04003E63 RID: 15971
		AttackQuickTentacle03,
		// Token: 0x04003E64 RID: 15972
		AttackTongue,
		// Token: 0x04003E65 RID: 15973
		SummonStart,
		// Token: 0x04003E66 RID: 15974
		SummonEnd,
		// Token: 0x04003E67 RID: 15975
		Summon01,
		// Token: 0x04003E68 RID: 15976
		Summon02,
		// Token: 0x04003E69 RID: 15977
		Summon03,
		// Token: 0x04003E6A RID: 15978
		Summon04,
		// Token: 0x04003E6B RID: 15979
		RetreatStart,
		// Token: 0x04003E6C RID: 15980
		RetreatEnd,
		// Token: 0x04003E6D RID: 15981
		RetreatIdle,
		// Token: 0x04003E6E RID: 15982
		DyingIdle,
		// Token: 0x04003E6F RID: 15983
		Runaway,
		// Token: 0x04003E70 RID: 15984
		AttackTongueSwipe,
		// Token: 0x04003E71 RID: 15985
		NextPhase,
		// Token: 0x04003E72 RID: 15986
		None,
		// Token: 0x04003E73 RID: 15987
		Count
	}

	// Token: 0x0200077A RID: 1914
	public enum BodyState
	{
		// Token: 0x04003E75 RID: 15989
		Destroyed,
		// Token: 0x04003E76 RID: 15990
		Bones,
		// Token: 0x04003E77 RID: 15991
		Shell,
		// Token: 0x04003E78 RID: 15992
		Count
	}
}
