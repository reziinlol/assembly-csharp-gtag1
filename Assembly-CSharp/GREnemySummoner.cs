using System;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000796 RID: 1942
public class GREnemySummoner : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameEntityDebugComponent, IGameAgentComponent, IGRSummoningEntity
{
	// Token: 0x060031AB RID: 12715 RVA: 0x00110950 File Offset: 0x0010EB50
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		this.trackedEntities = new List<int>();
		base.GetComponentsInChildren<Collider>(this.colliders);
		this.agent = base.GetComponent<GameAgent>();
		this.entity = base.GetComponent<GameEntity>();
		this.enemy = base.GetComponent<GREnemy>();
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.behaviorStartTime = -1.0;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.senseNearby.Setup(this.headTransform, this.entity);
	}

	// Token: 0x060031AC RID: 12716 RVA: 0x00110A14 File Offset: 0x0010EC14
	public void OnEntityInit()
	{
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityWander.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilitySummon.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityKeepDistance.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityMoveToTarget.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityFlashed.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetBehavior(GREnemySummoner.Behavior.Idle, true);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry entry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		this.SetHP(this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax));
		this.navAgent.speed = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.PatrolSpeed);
		this.agent.navAgent.autoTraverseOffMeshLink = false;
		this.agent.onJumpRequested += this.OnAgentJumpRequested;
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemySummoner.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemySummoner.BodyState.Bones, true);
	}

	// Token: 0x060031AD RID: 12717 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060031AE RID: 12718 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060031AF RID: 12719 RVA: 0x00110CC8 File Offset: 0x0010EEC8
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x060031B0 RID: 12720 RVA: 0x00110CE1 File Offset: 0x0010EEE1
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemySummoner.Behavior.Jump, false);
	}

	// Token: 0x060031B1 RID: 12721 RVA: 0x00110CFB File Offset: 0x0010EEFB
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 10)
		{
			return;
		}
		this.SetBehavior((GREnemySummoner.Behavior)newState, false);
	}

	// Token: 0x060031B2 RID: 12722 RVA: 0x00110D0F File Offset: 0x0010EF0F
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x060031B3 RID: 12723 RVA: 0x00110D18 File Offset: 0x0010EF18
	public bool TrySetBehavior(GREnemySummoner.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemySummoner.Behavior.Jump && newBehavior == GREnemySummoner.Behavior.Stagger)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x060031B4 RID: 12724 RVA: 0x00110D34 File Offset: 0x0010EF34
	public void SetBehavior(GREnemySummoner.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Stop();
			break;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemySummoner.Behavior.Destroyed:
			this.abilityDie.Stop();
			break;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.Stop();
			if (this.summonLight != null)
			{
				this.summonLight.gameObject.SetActive(false);
			}
			break;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.Stop();
			break;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.Stop();
			break;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemySummoner.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		case GREnemySummoner.Behavior.Flashed:
			this.abilityFlashed.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.Start();
			break;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Start();
			this.soundWander.Play(this.audioSource);
			break;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemySummoner.Behavior.Destroyed:
			if (this.entity.IsAuthority())
			{
				this.entity.manager.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			this.abilityDie.Start();
			break;
		case GREnemySummoner.Behavior.Summon:
			if (this.summonLight != null)
			{
				this.summonLight.gameObject.SetActive(true);
			}
			this.lastSummonTime = Time.timeAsDouble;
			this.abilitySummon.SetLookAtTarget(this.GetPlayerTransform(this.agent.targetPlayer));
			this.abilitySummon.Start();
			break;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.SetTargetPlayer(this.agent.targetPlayer);
			this.abilityKeepDistance.Start();
			break;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.SetTarget(this.GetPlayerTransform(this.agent.targetPlayer));
			this.abilityMoveToTarget.Start();
			break;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemySummoner.Behavior.Jump:
			this.abilityJump.Start();
			break;
		case GREnemySummoner.Behavior.Flashed:
			this.abilityFlashed.Start();
			break;
		}
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x060031B5 RID: 12725 RVA: 0x00110FF8 File Offset: 0x0010F1F8
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x060031B6 RID: 12726 RVA: 0x00111008 File Offset: 0x0010F208
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		this.lastUpdateTime = Time.time;
		GREnemySummoner.tempRigs.Clear();
		GREnemySummoner.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemySummoner.tempRigs);
		this.senseNearby.UpdateNearby(GREnemySummoner.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemySummoner.Behavior.Stagger:
		case GREnemySummoner.Behavior.Destroyed:
			break;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.Think(dt);
			if (this.abilitySummon.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.Think(dt);
			this.ChooseNewBehavior();
			break;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.Think(dt);
			this.ChooseNewBehavior();
			return;
		default:
			return;
		}
	}

	// Token: 0x060031B7 RID: 12727 RVA: 0x00111144 File Offset: 0x0010F344
	public bool CanSummon()
	{
		return !GhostReactorManager.AggroDisabled && (this.currBehavior != GREnemySummoner.Behavior.Summon || !this.abilitySummon.IsDone()) && Time.timeAsDouble - this.lastSummonTime >= (double)this.minSummonInterval && this.trackedEntities.Count < this.maxSimultaneousSummonedEntities;
	}

	// Token: 0x060031B8 RID: 12728 RVA: 0x0011119C File Offset: 0x0010F39C
	public Transform GetPlayerTransform(NetPlayer targetPlayer)
	{
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				return grplayer.transform;
			}
		}
		return null;
	}

	// Token: 0x060031B9 RID: 12729 RVA: 0x001111D4 File Offset: 0x0010F3D4
	private void ChooseNewBehavior()
	{
		float num = 0f;
		VRRig x = this.senseNearby.PickClosest(out num);
		if (!GhostReactorManager.AggroDisabled && x != null)
		{
			this.investigateLocation = null;
			float num2 = (this.currBehavior == GREnemySummoner.Behavior.KeepDistance) ? (this.keepDistanceThreshold + 1f) : this.keepDistanceThreshold;
			if (num < num2 * num2)
			{
				this.SetBehavior(GREnemySummoner.Behavior.KeepDistance, false);
				return;
			}
			if (this.CanSummon())
			{
				this.SetBehavior(GREnemySummoner.Behavior.Summon, false);
				return;
			}
			float num3 = this.tooFarDistanceThreshold * this.tooFarDistanceThreshold;
			if (num > num3)
			{
				this.SetBehavior(GREnemySummoner.Behavior.MoveToTarget, false);
				return;
			}
			this.SetBehavior(GREnemySummoner.Behavior.Idle, false);
			return;
		}
		else
		{
			this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
			if (this.investigateLocation != null)
			{
				this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
				this.SetBehavior(GREnemySummoner.Behavior.Investigate, false);
				return;
			}
			double num4 = Time.timeAsDouble - this.abilityIdle.startTime;
			if (this.currBehavior == GREnemySummoner.Behavior.Idle && num4 < (double)this.idleDuration)
			{
				this.SetBehavior(GREnemySummoner.Behavior.Idle, false);
				return;
			}
			this.SetBehavior(GREnemySummoner.Behavior.Wander, false);
			return;
		}
	}

	// Token: 0x060031BA RID: 12730 RVA: 0x001112F8 File Offset: 0x0010F4F8
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x060031BB RID: 12731 RVA: 0x00111318 File Offset: 0x0010F518
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.UpdateAuthority(dt);
			if (this.abilityStagger.IsDone())
			{
				this.SetBehavior(GREnemySummoner.Behavior.Wander, false);
				return;
			}
			break;
		case GREnemySummoner.Behavior.Destroyed:
			this.abilityDie.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.Jump:
			this.abilityJump.UpdateAuthority(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemySummoner.Behavior.Flashed:
			this.abilityFlashed.UpdateAuthority(dt);
			if (this.abilityFlashed.IsDone())
			{
				this.ChooseNewBehavior();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060031BC RID: 12732 RVA: 0x0011141C File Offset: 0x0010F61C
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Destroyed:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Flashed:
			this.abilityFlashed.UpdateRemote(dt);
			return;
		default:
			return;
		}
	}

	// Token: 0x060031BD RID: 12733 RVA: 0x001114D4 File Offset: 0x0010F6D4
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		byte value2 = (byte)this.currBodyState;
		writer.Write(value);
		writer.Write(this.hp);
		writer.Write(value2);
	}

	// Token: 0x060031BE RID: 12734 RVA: 0x0011150C File Offset: 0x0010F70C
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemySummoner.Behavior newBehavior = (GREnemySummoner.Behavior)reader.ReadByte();
		int num = reader.ReadInt32();
		GREnemySummoner.BodyState newBodyState = (GREnemySummoner.BodyState)reader.ReadByte();
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
	}

	// Token: 0x060031BF RID: 12735 RVA: 0x00023994 File Offset: 0x00021B94
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x060031C0 RID: 12736 RVA: 0x00111548 File Offset: 0x0010F748
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

	// Token: 0x060031C1 RID: 12737 RVA: 0x001115AC File Offset: 0x0010F7AC
	private void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBehavior == GREnemySummoner.Behavior.Destroyed)
		{
			return;
		}
		if (this.currBodyState != GREnemySummoner.BodyState.Bones)
		{
			if (this.currBodyState == GREnemySummoner.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
		this.hp -= hit.hitAmount;
		if (this.hp <= 0)
		{
			this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hit.hitByEntityId));
			this.abilityDie.SetStaggerVelocity(hit.hitImpulse);
			this.SetBehavior(GREnemySummoner.Behavior.Destroyed, false);
			return;
		}
		this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
		this.TrySetBehavior(GREnemySummoner.Behavior.Stagger);
	}

	// Token: 0x060031C2 RID: 12738 RVA: 0x00111660 File Offset: 0x0010F860
	public void InstantDeath()
	{
		this.hp = 0;
		this.SetBehavior(GREnemySummoner.Behavior.Destroyed, false);
	}

	// Token: 0x060031C3 RID: 12739 RVA: 0x00111674 File Offset: 0x0010F874
	private void OnHitByFlash(GRTool tool, GameHitData hit)
	{
		this.abilityFlashed.SetStaggerVelocity(hit.hitImpulse);
		if (this.currBodyState == GREnemySummoner.BodyState.Shell)
		{
			this.hp -= hit.hitAmount;
			if (this.armor != null)
			{
				this.armor.SetHp(this.hp);
			}
			if (this.hp <= 0)
			{
				if (this.armor != null)
				{
					this.armor.PlayDestroyFx(this.armor.transform.position);
				}
				this.SetBodyState(GREnemySummoner.BodyState.Bones, false);
				if (tool.gameEntity.IsHeldByLocalPlayer())
				{
					PlayerGameEvents.MiscEvent("GRArmorBreak_" + base.name, 1);
				}
				if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
				{
					this.armor.FragmentArmor();
				}
			}
			else
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.RefreshBody();
			}
		}
		GRToolFlash component = tool.GetComponent<GRToolFlash>();
		if (component != null)
		{
			this.abilityFlashed.SetStunTime(component.stunDuration);
		}
		this.TrySetBehavior(GREnemySummoner.Behavior.Flashed);
	}

	// Token: 0x060031C4 RID: 12740 RVA: 0x0011179C File Offset: 0x0010F99C
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x060031C5 RID: 12741 RVA: 0x001117A8 File Offset: 0x0010F9A8
	private void OnTriggerEnter(Collider collider)
	{
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component = attachedRigidbody.GetComponent<GRPlayer>();
			if (component != null && component.gamePlayer.IsLocal())
			{
				GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Phantom, this.entity.id, component, base.transform.position);
			}
			GRBreakable component2 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component3 = attachedRigidbody.GetComponent<GameHittable>();
			if (component2 != null && component3 != null)
			{
				GameHitData hitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component3.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component2.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component2.transform.position,
					hittablePoint = component3.FindHittablePoint(collider)
				};
				component3.RequestHit(hitData);
			}
		}
	}

	// Token: 0x060031C6 RID: 12742 RVA: 0x001118B0 File Offset: 0x0010FAB0
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemySummoner.BodyState.Destroyed:
			this.armor.SetHp(0);
			return;
		case GREnemySummoner.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			GREnemy.HideObjects(this.bonesStateVisibleObjects, false);
			GREnemy.HideObjects(this.alwaysVisibleObjects, false);
			return;
		case GREnemySummoner.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			GREnemy.HideObjects(this.bonesStateVisibleObjects, true);
			GREnemy.HideObjects(this.alwaysVisibleObjects, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x060031C7 RID: 12743 RVA: 0x00111964 File Offset: 0x0010FB64
	public void SetBodyState(GREnemySummoner.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemySummoner.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemySummoner.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemySummoner.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemySummoner.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemySummoner.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x060031C8 RID: 12744 RVA: 0x00111A3C File Offset: 0x0010FC3C
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("Nearby rigs: <color=\"yellow\">{0}<color=\"white\">", this.senseNearby.rigsNearby.Count));
		strings.Add(string.Format("Spawned entities: <color=\"yellow\">{0}<color=\"white\">", this.trackedEntities.Count));
	}

	// Token: 0x060031C9 RID: 12745 RVA: 0x00111AC4 File Offset: 0x0010FCC4
	public void AddTrackedEntity(GameEntity entityToTrack)
	{
		int netId = entityToTrack.GetNetId();
		this.trackedEntities.AddIfNew(netId);
	}

	// Token: 0x060031CA RID: 12746 RVA: 0x00111AE4 File Offset: 0x0010FCE4
	public void RemoveTrackedEntity(GameEntity entityToRemove)
	{
		int netId = entityToRemove.GetNetId();
		if (this.trackedEntities.Contains(netId))
		{
			this.trackedEntities.Remove(netId);
		}
	}

	// Token: 0x060031CB RID: 12747 RVA: 0x00111B13 File Offset: 0x0010FD13
	public void OnSummonedEntityInit(GameEntity entity)
	{
		this.AddTrackedEntity(entity);
	}

	// Token: 0x060031CC RID: 12748 RVA: 0x00111B1C File Offset: 0x0010FD1C
	public void OnSummonedEntityDestroy(GameEntity entity)
	{
		this.RemoveTrackedEntity(entity);
	}

	// Token: 0x04004062 RID: 16482
	private GameEntity entity;

	// Token: 0x04004063 RID: 16483
	private GameAgent agent;

	// Token: 0x04004064 RID: 16484
	private GREnemy enemy;

	// Token: 0x04004065 RID: 16485
	public GRArmorEnemy armor;

	// Token: 0x04004066 RID: 16486
	public GRAttributes attributes;

	// Token: 0x04004067 RID: 16487
	public Animation anim;

	// Token: 0x04004068 RID: 16488
	public GRSenseNearby senseNearby;

	// Token: 0x04004069 RID: 16489
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x0400406A RID: 16490
	public GRAbilityIdle abilityIdle;

	// Token: 0x0400406B RID: 16491
	public GRAbilityWander abilityWander;

	// Token: 0x0400406C RID: 16492
	public GRAbilityAttackJump abilityAttack;

	// Token: 0x0400406D RID: 16493
	public GRAbilityStagger abilityStagger;

	// Token: 0x0400406E RID: 16494
	public GRAbilityDie abilityDie;

	// Token: 0x0400406F RID: 16495
	public GRAbilitySummon abilitySummon;

	// Token: 0x04004070 RID: 16496
	public GRAbilityKeepDistance abilityKeepDistance;

	// Token: 0x04004071 RID: 16497
	public GRAbilityMoveToTarget abilityMoveToTarget;

	// Token: 0x04004072 RID: 16498
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x04004073 RID: 16499
	public GRAbilityJump abilityJump;

	// Token: 0x04004074 RID: 16500
	public GRAbilityStagger abilityFlashed;

	// Token: 0x04004075 RID: 16501
	public AbilitySound soundWander;

	// Token: 0x04004076 RID: 16502
	public AbilitySound soundAttack;

	// Token: 0x04004077 RID: 16503
	public GameLight summonLight;

	// Token: 0x04004078 RID: 16504
	public List<Renderer> bones;

	// Token: 0x04004079 RID: 16505
	public List<Renderer> always;

	// Token: 0x0400407A RID: 16506
	public List<GameObject> bonesStateVisibleObjects;

	// Token: 0x0400407B RID: 16507
	public List<GameObject> alwaysVisibleObjects;

	// Token: 0x0400407C RID: 16508
	public Transform coreMarker;

	// Token: 0x0400407D RID: 16509
	public GRCollectible corePrefab;

	// Token: 0x0400407E RID: 16510
	public Transform headTransform;

	// Token: 0x0400407F RID: 16511
	public float attackRange = 2f;

	// Token: 0x04004080 RID: 16512
	public List<VRRig> rigsNearby;

	// Token: 0x04004081 RID: 16513
	public NavMeshAgent navAgent;

	// Token: 0x04004082 RID: 16514
	public AudioSource audioSource;

	// Token: 0x04004083 RID: 16515
	public float idleDuration = 2f;

	// Token: 0x04004084 RID: 16516
	public float keepDistanceThreshold = 3f;

	// Token: 0x04004085 RID: 16517
	public float tooFarDistanceThreshold = 5f;

	// Token: 0x04004086 RID: 16518
	public double lastSummonTime;

	// Token: 0x04004087 RID: 16519
	public float minSummonInterval = 4f;

	// Token: 0x04004088 RID: 16520
	public int maxSimultaneousSummonedEntities = 3;

	// Token: 0x04004089 RID: 16521
	public float hearingRadius = 7f;

	// Token: 0x0400408A RID: 16522
	[ReadOnly]
	public int hp;

	// Token: 0x0400408B RID: 16523
	[ReadOnly]
	public GREnemySummoner.Behavior currBehavior;

	// Token: 0x0400408C RID: 16524
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x0400408D RID: 16525
	[ReadOnly]
	public GREnemySummoner.BodyState currBodyState;

	// Token: 0x0400408E RID: 16526
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x0400408F RID: 16527
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x04004090 RID: 16528
	private Rigidbody rigidBody;

	// Token: 0x04004091 RID: 16529
	private List<Collider> colliders;

	// Token: 0x04004092 RID: 16530
	private List<int> trackedEntities;

	// Token: 0x04004093 RID: 16531
	private Vector3? investigateLocation;

	// Token: 0x04004094 RID: 16532
	private float lastUpdateTime;

	// Token: 0x04004095 RID: 16533
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x02000797 RID: 1943
	public enum Behavior
	{
		// Token: 0x04004097 RID: 16535
		Idle,
		// Token: 0x04004098 RID: 16536
		Wander,
		// Token: 0x04004099 RID: 16537
		Stagger,
		// Token: 0x0400409A RID: 16538
		Destroyed,
		// Token: 0x0400409B RID: 16539
		Summon,
		// Token: 0x0400409C RID: 16540
		KeepDistance,
		// Token: 0x0400409D RID: 16541
		MoveToTarget,
		// Token: 0x0400409E RID: 16542
		Investigate,
		// Token: 0x0400409F RID: 16543
		Jump,
		// Token: 0x040040A0 RID: 16544
		Flashed,
		// Token: 0x040040A1 RID: 16545
		Count
	}

	// Token: 0x02000798 RID: 1944
	public enum BodyState
	{
		// Token: 0x040040A3 RID: 16547
		Destroyed,
		// Token: 0x040040A4 RID: 16548
		Bones,
		// Token: 0x040040A5 RID: 16549
		Shell,
		// Token: 0x040040A6 RID: 16550
		Count
	}
}
