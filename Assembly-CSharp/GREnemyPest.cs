using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200078C RID: 1932
public class GREnemyPest : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent, ITickSystemTick
{
	// Token: 0x17000483 RID: 1155
	// (get) Token: 0x06003132 RID: 12594 RVA: 0x0010CB57 File Offset: 0x0010AD57
	// (set) Token: 0x06003133 RID: 12595 RVA: 0x0010CB5F File Offset: 0x0010AD5F
	public bool TickRunning { get; set; }

	// Token: 0x06003134 RID: 12596 RVA: 0x0010CB68 File Offset: 0x0010AD68
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.behaviorStartTime = -1.0;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.senseNearby.Setup(this.headTransform, this.entity);
		GameEntity gameEntity = this.entity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.entity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
		base.Invoke("PlaySpawnAudio", 0.1f);
	}

	// Token: 0x06003135 RID: 12597 RVA: 0x00019E3F File Offset: 0x0001803F
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06003136 RID: 12598 RVA: 0x00019E47 File Offset: 0x00018047
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06003137 RID: 12599 RVA: 0x0010CC5A File Offset: 0x0010AE5A
	private void PlaySpawnAudio()
	{
		this.spawnSound.Play(null);
	}

	// Token: 0x06003138 RID: 12600 RVA: 0x0010CC68 File Offset: 0x0010AE68
	public void OnEntityInit()
	{
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityChase.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityAttack.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityWander.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityGrabbed.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityThrown.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityFlashed.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.SetBehavior(GREnemyPest.Behavior.Wander, false);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry entry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		this.navAgent.speed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
		this.SetHP(this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax));
		this.agent.navAgent.autoTraverseOffMeshLink = false;
		this.agent.onJumpRequested += this.OnAgentJumpRequested;
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyPest.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyPest.BodyState.Bones, true);
	}

	// Token: 0x06003139 RID: 12601 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x0600313A RID: 12602 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x0600313B RID: 12603 RVA: 0x0010CFB4 File Offset: 0x0010B1B4
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x0600313C RID: 12604 RVA: 0x0010CFCD File Offset: 0x0010B1CD
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyPest.Behavior.Jump, false);
	}

	// Token: 0x0600313D RID: 12605 RVA: 0x0010CFE8 File Offset: 0x0010B1E8
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 11)
		{
			return;
		}
		this.SetBehavior((GREnemyPest.Behavior)newState, false);
	}

	// Token: 0x0600313E RID: 12606 RVA: 0x0010CFFC File Offset: 0x0010B1FC
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x0600313F RID: 12607 RVA: 0x0010D005 File Offset: 0x0010B205
	public bool TrySetBehavior(GREnemyPest.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemyPest.Behavior.Jump && newBehavior == GREnemyPest.Behavior.Stagger)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x06003140 RID: 12608 RVA: 0x0010D020 File Offset: 0x0010B220
	public void SetBehavior(GREnemyPest.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Stop();
			break;
		case GREnemyPest.Behavior.Chase:
			this.abilityChase.Stop();
			break;
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.Stop();
			break;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyPest.Behavior.Grabbed:
			this.abilityGrabbed.Stop();
			break;
		case GREnemyPest.Behavior.Thrown:
			this.abilityThrown.Stop();
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.Stop();
			break;
		case GREnemyPest.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyPest.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		case GREnemyPest.Behavior.Flashed:
			this.abilityFlashed.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.abilityIdle.Start();
			break;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Start();
			break;
		case GREnemyPest.Behavior.Chase:
			this.abilityChase.Start();
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.Start();
			this.abilityAttack.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemyPest.Behavior.Grabbed:
			this.abilityGrabbed.Start();
			break;
		case GREnemyPest.Behavior.Thrown:
			this.abilityThrown.Start();
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.Start();
			break;
		case GREnemyPest.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyPest.Behavior.Jump:
			this.abilityJump.Start();
			break;
		case GREnemyPest.Behavior.Flashed:
			this.abilityFlashed.Start();
			break;
		}
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x06003141 RID: 12609 RVA: 0x0010D236 File Offset: 0x0010B436
	private void OnGrabbed()
	{
		if (this.currBehavior == GREnemyPest.Behavior.Destroyed)
		{
			return;
		}
		this.SetBehavior(GREnemyPest.Behavior.Grabbed, false);
	}

	// Token: 0x06003142 RID: 12610 RVA: 0x0010D24A File Offset: 0x0010B44A
	private void OnReleased()
	{
		if (this.currBehavior == GREnemyPest.Behavior.Destroyed)
		{
			return;
		}
		this.SetBehavior(GREnemyPest.Behavior.Thrown, false);
	}

	// Token: 0x06003143 RID: 12611 RVA: 0x0010D25E File Offset: 0x0010B45E
	public void Tick()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x06003144 RID: 12612 RVA: 0x0010D26C File Offset: 0x0010B46C
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyPest.tempRigs.Clear();
		GREnemyPest.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyPest.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyPest.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		GREnemyPest.Behavior behavior = this.currBehavior;
		switch (behavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.ChooseNewBehavior();
			return;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemyPest.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			return;
		default:
			if (behavior != GREnemyPest.Behavior.Investigate)
			{
				return;
			}
			this.abilityInvestigate.Think(dt);
			this.ChooseNewBehavior();
			return;
		}
	}

	// Token: 0x06003145 RID: 12613 RVA: 0x0010D36C File Offset: 0x0010B56C
	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
		{
			this.investigateLocation = null;
			this.SetBehavior(GREnemyPest.Behavior.Chase, false);
			return;
		}
		this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
		if (this.investigateLocation != null)
		{
			this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
			this.SetBehavior(GREnemyPest.Behavior.Investigate, false);
			return;
		}
		this.SetBehavior(GREnemyPest.Behavior.Wander, false);
	}

	// Token: 0x06003146 RID: 12614 RVA: 0x0010D3F8 File Offset: 0x0010B5F8
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06003147 RID: 12615 RVA: 0x0010D418 File Offset: 0x0010B618
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.abilityIdle.UpdateAuthority(dt);
			return;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.UpdateAuthority(dt);
			return;
		case GREnemyPest.Behavior.Chase:
		{
			this.abilityChase.UpdateAuthority(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Wander, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float num = this.attackRange * this.attackRange;
				if ((grplayer.transform.position - base.transform.position).sqrMagnitude < num)
				{
					this.SetBehavior(GREnemyPest.Behavior.Attack, false);
					return;
				}
			}
			break;
		}
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.UpdateAuthority(dt);
			if (this.abilityAttack.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.UpdateAuthority(dt);
			if (this.abilityStagger.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Wander, false);
				return;
			}
			break;
		case GREnemyPest.Behavior.Grabbed:
			break;
		case GREnemyPest.Behavior.Thrown:
			if (this.abilityThrown.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Wander, false);
				return;
			}
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.UpdateAuthority(dt);
			return;
		case GREnemyPest.Behavior.Investigate:
			this.abilityInvestigate.UpdateAuthority(dt);
			return;
		case GREnemyPest.Behavior.Jump:
			this.abilityJump.UpdateAuthority(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemyPest.Behavior.Flashed:
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

	// Token: 0x06003148 RID: 12616 RVA: 0x0010D5B4 File Offset: 0x0010B7B4
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Chase:
			this.abilityChase.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Grabbed:
		case GREnemyPest.Behavior.Thrown:
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Flashed:
			this.abilityFlashed.UpdateRemote(dt);
			break;
		default:
			return;
		}
	}

	// Token: 0x06003149 RID: 12617 RVA: 0x0010D660 File Offset: 0x0010B860
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		byte value2 = (byte)this.currBodyState;
		writer.Write(value);
		writer.Write(this.hp);
		writer.Write(value2);
	}

	// Token: 0x0600314A RID: 12618 RVA: 0x0010D698 File Offset: 0x0010B898
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyPest.Behavior newBehavior = (GREnemyPest.Behavior)reader.ReadByte();
		int num = reader.ReadInt32();
		GREnemyPest.BodyState newBodyState = (GREnemyPest.BodyState)reader.ReadByte();
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
	}

	// Token: 0x0600314B RID: 12619 RVA: 0x00023994 File Offset: 0x00021B94
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x0600314C RID: 12620 RVA: 0x0010D6D4 File Offset: 0x0010B8D4
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		GRTool gameComponent = this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId);
		if (gameComponent != null)
		{
			switch (hitTypeId)
			{
			case GameHitType.Club:
				this.OnHitByClub(hit);
				return;
			case GameHitType.Flash:
				this.OnHitByFlash(gameComponent, hit);
				return;
			case GameHitType.Shield:
				this.OnHitByShield(hit);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x0600314D RID: 12621 RVA: 0x0010D734 File Offset: 0x0010B934
	private void OnHitByClub(GameHitData hit)
	{
		if (this.currBodyState != GREnemyPest.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyPest.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
		if (this.currBehavior == GREnemyPest.Behavior.Destroyed)
		{
			return;
		}
		this.hp -= hit.hitAmount;
		if (this.hp <= 0)
		{
			this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hit.hitByEntityId));
			this.SetBehavior(GREnemyPest.Behavior.Destroyed, false);
			return;
		}
		this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
		this.TrySetBehavior(GREnemyPest.Behavior.Stagger);
	}

	// Token: 0x0600314E RID: 12622 RVA: 0x0010D7D7 File Offset: 0x0010B9D7
	public void InstantDeath()
	{
		this.hp = 0;
		this.SetBehavior(GREnemyPest.Behavior.Destroyed, false);
	}

	// Token: 0x0600314F RID: 12623 RVA: 0x0010D7E8 File Offset: 0x0010B9E8
	private void OnHitByFlash(GRTool tool, GameHitData hit)
	{
		this.abilityFlashed.SetStaggerVelocity(hit.hitImpulse);
		if (this.currBodyState == GREnemyPest.BodyState.Shell)
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
				this.SetBodyState(GREnemyPest.BodyState.Bones, false);
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
		this.TrySetBehavior(GREnemyPest.Behavior.Flashed);
	}

	// Token: 0x06003150 RID: 12624 RVA: 0x0010D910 File Offset: 0x0010BB10
	private void OnHitByShield(GameHitData hit)
	{
		this.OnHitByClub(hit);
	}

	// Token: 0x06003151 RID: 12625 RVA: 0x0010D91C File Offset: 0x0010BB1C
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBehavior != GREnemyPest.Behavior.Attack)
		{
			return;
		}
		GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
		if (component != null)
		{
			Vector3 enemyAttackDirection = this.abilityAttack.targetPos - this.abilityAttack.initialPos;
			GameHittable component2 = base.GetComponent<GameHittable>();
			component.BlockHittable(base.transform.position, enemyAttackDirection, component2);
			return;
		}
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component3 = attachedRigidbody.GetComponent<GRPlayer>();
			if (component3 != null && component3.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
			{
				if (this.tryHitPlayerCoroutine != null)
				{
					base.StopCoroutine(this.tryHitPlayerCoroutine);
				}
				this.tryHitPlayerCoroutine = base.StartCoroutine(this.TryHitPlayer(component3));
			}
			GRBreakable component4 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component5 = attachedRigidbody.GetComponent<GameHittable>();
			if (component4 != null && component5 != null)
			{
				GameHitData hitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component5.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component4.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component4.transform.position,
					hittablePoint = component5.FindHittablePoint(collider)
				};
				component5.RequestHit(hitData);
			}
		}
	}

	// Token: 0x06003152 RID: 12626 RVA: 0x0010DA96 File Offset: 0x0010BC96
	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if (this.currBehavior == GREnemyPest.Behavior.Attack && player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position);
		}
		yield break;
	}

	// Token: 0x06003153 RID: 12627 RVA: 0x0010DAAC File Offset: 0x0010BCAC
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyPest.BodyState.Destroyed:
			this.armor.SetHp(0);
			return;
		case GREnemyPest.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideObjects(this.bonesStateVisibleObjects, false);
			GREnemy.HideObjects(this.alwaysVisibleObjects, false);
			return;
		case GREnemyPest.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideObjects(this.bonesStateVisibleObjects, true);
			GREnemy.HideObjects(this.alwaysVisibleObjects, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003154 RID: 12628 RVA: 0x0010DB30 File Offset: 0x0010BD30
	public void SetBodyState(GREnemyPest.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyPest.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyPest.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyPest.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyPest.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyPest.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x06003155 RID: 12629 RVA: 0x0010DC08 File Offset: 0x0010BE08
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		float magnitude = (GRSenseNearby.GetRigTestLocation(VRRig.LocalRig) - base.transform.position).magnitude;
		bool flag = GRSenseLineOfSight.HasGeoLineOfSight(this.headTransform.position, GRSenseNearby.GetRigTestLocation(VRRig.LocalRig), this.senseLineOfSight.sightDist, this.senseLineOfSight.visibilityMask);
		strings.Add(string.Format("player rig dis: {0} has los: {1}", magnitude, flag));
	}

	// Token: 0x04003F74 RID: 16244
	public GameEntity entity;

	// Token: 0x04003F75 RID: 16245
	public GameAgent agent;

	// Token: 0x04003F76 RID: 16246
	public GREnemy enemy;

	// Token: 0x04003F77 RID: 16247
	public GRArmorEnemy armor;

	// Token: 0x04003F78 RID: 16248
	public GRAttributes attributes;

	// Token: 0x04003F79 RID: 16249
	public Animation anim;

	// Token: 0x04003F7A RID: 16250
	public GRSenseNearby senseNearby;

	// Token: 0x04003F7B RID: 16251
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003F7C RID: 16252
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003F7D RID: 16253
	public GRAbilityChase abilityChase;

	// Token: 0x04003F7E RID: 16254
	public GRAbilityWander abilityWander;

	// Token: 0x04003F7F RID: 16255
	public GRAbilityAttackJump abilityAttack;

	// Token: 0x04003F80 RID: 16256
	public GRAbilityStagger abilityStagger;

	// Token: 0x04003F81 RID: 16257
	public GRAbilityStagger abilityFlashed;

	// Token: 0x04003F82 RID: 16258
	public GRAbilityDie abilityDie;

	// Token: 0x04003F83 RID: 16259
	public GRAbilityGrabbed abilityGrabbed;

	// Token: 0x04003F84 RID: 16260
	public GRAbilityThrown abilityThrown;

	// Token: 0x04003F85 RID: 16261
	public AbilitySound spawnSound;

	// Token: 0x04003F86 RID: 16262
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x04003F87 RID: 16263
	public GRAbilityJump abilityJump;

	// Token: 0x04003F88 RID: 16264
	public List<GameObject> bonesStateVisibleObjects;

	// Token: 0x04003F89 RID: 16265
	public List<GameObject> alwaysVisibleObjects;

	// Token: 0x04003F8A RID: 16266
	public Transform coreMarker;

	// Token: 0x04003F8B RID: 16267
	public GRCollectible corePrefab;

	// Token: 0x04003F8C RID: 16268
	public Transform headTransform;

	// Token: 0x04003F8D RID: 16269
	public float attackRange = 2f;

	// Token: 0x04003F8E RID: 16270
	public List<VRRig> rigsNearby;

	// Token: 0x04003F8F RID: 16271
	public NavMeshAgent navAgent;

	// Token: 0x04003F90 RID: 16272
	public AudioSource audioSource;

	// Token: 0x04003F91 RID: 16273
	public float hearingRadius = 5f;

	// Token: 0x04003F92 RID: 16274
	private Vector3? investigateLocation;

	// Token: 0x04003F94 RID: 16276
	[ReadOnly]
	public int hp;

	// Token: 0x04003F95 RID: 16277
	[ReadOnly]
	public GREnemyPest.Behavior currBehavior;

	// Token: 0x04003F96 RID: 16278
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x04003F97 RID: 16279
	[ReadOnly]
	public GREnemyPest.BodyState currBodyState;

	// Token: 0x04003F98 RID: 16280
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x04003F99 RID: 16281
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x04003F9A RID: 16282
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x04003F9B RID: 16283
	private Rigidbody rigidBody;

	// Token: 0x04003F9C RID: 16284
	private List<Collider> colliders;

	// Token: 0x04003F9D RID: 16285
	private float lastHitPlayerTime;

	// Token: 0x04003F9E RID: 16286
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x04003F9F RID: 16287
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04003FA0 RID: 16288
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x0200078D RID: 1933
	public enum Behavior
	{
		// Token: 0x04003FA2 RID: 16290
		Idle,
		// Token: 0x04003FA3 RID: 16291
		Wander,
		// Token: 0x04003FA4 RID: 16292
		Chase,
		// Token: 0x04003FA5 RID: 16293
		Attack,
		// Token: 0x04003FA6 RID: 16294
		Stagger,
		// Token: 0x04003FA7 RID: 16295
		Grabbed,
		// Token: 0x04003FA8 RID: 16296
		Thrown,
		// Token: 0x04003FA9 RID: 16297
		Destroyed,
		// Token: 0x04003FAA RID: 16298
		Investigate,
		// Token: 0x04003FAB RID: 16299
		Jump,
		// Token: 0x04003FAC RID: 16300
		Flashed,
		// Token: 0x04003FAD RID: 16301
		Count
	}

	// Token: 0x0200078E RID: 1934
	public enum BodyState
	{
		// Token: 0x04003FAF RID: 16303
		Destroyed,
		// Token: 0x04003FB0 RID: 16304
		Bones,
		// Token: 0x04003FB1 RID: 16305
		Shell,
		// Token: 0x04003FB2 RID: 16306
		Count
	}
}
