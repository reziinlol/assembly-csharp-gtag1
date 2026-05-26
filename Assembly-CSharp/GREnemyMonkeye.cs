using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CjLib;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

// Token: 0x02000788 RID: 1928
public class GREnemyMonkeye : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent
{
	// Token: 0x06003108 RID: 12552 RVA: 0x0010B4CC File Offset: 0x001096CC
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
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06003109 RID: 12553 RVA: 0x0010B554 File Offset: 0x00109754
	public void OnEntityInit()
	{
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityChase.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilitySearch.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityAttackLaser.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityAttackDiscoWander.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityAttackSlamdown.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityPatrol.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.senseNearby.Setup(this.headTransform, this.entity);
		this.Setup(this.entity.createData);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry entry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		this.agent.navAgent.autoTraverseOffMeshLink = false;
		this.agent.onJumpRequested += this.OnAgentJumpRequested;
	}

	// Token: 0x0600310A RID: 12554 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x0600310B RID: 12555 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x0600310C RID: 12556 RVA: 0x0010B814 File Offset: 0x00109A14
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x0600310D RID: 12557 RVA: 0x0010B844 File Offset: 0x00109A44
	public void Setup(long entityCreateData)
	{
		this.SetPatrolPath(entityCreateData);
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.Patrol, true);
		}
		else
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.Idle, true);
		}
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyMonkeye.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyMonkeye.BodyState.Bones, true);
	}

	// Token: 0x0600310E RID: 12558 RVA: 0x0010B897 File Offset: 0x00109A97
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyMonkeye.Behavior.Jump, false);
	}

	// Token: 0x0600310F RID: 12559 RVA: 0x0010B8B2 File Offset: 0x00109AB2
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 11)
		{
			return;
		}
		this.SetBehavior((GREnemyMonkeye.Behavior)newState, false);
	}

	// Token: 0x06003110 RID: 12560 RVA: 0x0010B8C6 File Offset: 0x00109AC6
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyMonkeye.BodyState)newState, false);
	}

	// Token: 0x06003111 RID: 12561 RVA: 0x0010B8DC File Offset: 0x00109ADC
	public void SetPatrolPath(long entityCreateData)
	{
		GRPatrolPath grpatrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(entityCreateData);
		this.abilityPatrol.SetPatrolPath(grpatrolPath);
	}

	// Token: 0x06003112 RID: 12562 RVA: 0x0010B90C File Offset: 0x00109B0C
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x06003113 RID: 12563 RVA: 0x0010B915 File Offset: 0x00109B15
	public bool TrySetBehavior(GREnemyMonkeye.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemyMonkeye.Behavior.Jump && newBehavior == GREnemyMonkeye.Behavior.Stagger)
		{
			return false;
		}
		if (newBehavior == GREnemyMonkeye.Behavior.Stagger && Time.time < this.lastStaggerTime + this.staggerImmuneTime)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x06003114 RID: 12564 RVA: 0x0010B94C File Offset: 0x00109B4C
	public void SetBehavior(GREnemyMonkeye.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyMonkeye.Behavior.Patrol:
			this.abilityPatrol.Stop();
			break;
		case GREnemyMonkeye.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyMonkeye.Behavior.Dying:
			this.abilityDie.Stop();
			break;
		case GREnemyMonkeye.Behavior.Chase:
			this.abilityChase.Stop();
			break;
		case GREnemyMonkeye.Behavior.Search:
			this.abilitySearch.Stop();
			break;
		case GREnemyMonkeye.Behavior.Attack:
			this.abilityAttackLaser.Stop();
			break;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.Stop();
			break;
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			this.abilityAttackSlamdown.Stop();
			break;
		case GREnemyMonkeye.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyMonkeye.Behavior.Jump:
			this.abilityJump.Stop();
			this.lastJumpEndtime = Time.timeAsDouble;
			break;
		}
		this.currBehavior = newBehavior;
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
			this.abilitySearch.Start();
			break;
		case GREnemyMonkeye.Behavior.Patrol:
			this.abilityPatrol.Start();
			break;
		case GREnemyMonkeye.Behavior.Stagger:
			this.abilityStagger.Start();
			this.lastStaggerTime = Time.time;
			break;
		case GREnemyMonkeye.Behavior.Dying:
			this.abilityDie.Start();
			break;
		case GREnemyMonkeye.Behavior.Chase:
			this.abilityChase.Start();
			this.investigateLocation = null;
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyMonkeye.Behavior.Search:
			this.abilitySearch.Start();
			break;
		case GREnemyMonkeye.Behavior.Attack:
			this.abilityAttackLaser.Start();
			this.investigateLocation = null;
			this.abilityAttackLaser.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.Start();
			this.investigateLocation = null;
			break;
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			this.abilityAttackSlamdown.Start();
			this.investigateLocation = null;
			this.abilityAttackSlamdown.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyMonkeye.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyMonkeye.Behavior.Jump:
			this.abilityJump.Start();
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x06003115 RID: 12565 RVA: 0x0010BBC4 File Offset: 0x00109DC4
	private int CalcMaxHP()
	{
		float difficultyScalingForCurrentFloor = this.entity.manager.ghostReactorManager.reactor.difficultyScalingForCurrentFloor;
		return (int)((float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax) * difficultyScalingForCurrentFloor);
	}

	// Token: 0x06003116 RID: 12566 RVA: 0x0010BC00 File Offset: 0x00109E00
	public void SetBodyState(GREnemyMonkeye.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyMonkeye.BodyState.Bones:
			this.hp = this.CalcMaxHP();
			this.enemy.SetMaxHP(this.hp);
			this.enemy.SetHP(this.hp);
			break;
		case GREnemyMonkeye.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyMonkeye.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyMonkeye.BodyState.Bones:
			this.hp = this.CalcMaxHP();
			this.enemy.SetMaxHP(this.hp);
			this.enemy.SetHP(this.hp);
			break;
		case GREnemyMonkeye.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x06003117 RID: 12567 RVA: 0x0010BD10 File Offset: 0x00109F10
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyMonkeye.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyMonkeye.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyMonkeye.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003118 RID: 12568 RVA: 0x0010BDAA File Offset: 0x00109FAA
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x06003119 RID: 12569 RVA: 0x0010BDB8 File Offset: 0x00109FB8
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyMonkeye.tempRigs.Clear();
		GREnemyMonkeye.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyMonkeye.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyMonkeye.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
		case GREnemyMonkeye.Behavior.Patrol:
		case GREnemyMonkeye.Behavior.Investigate:
			this.ChooseNewBehavior();
			return;
		case GREnemyMonkeye.Behavior.Stagger:
		case GREnemyMonkeye.Behavior.Dying:
		case GREnemyMonkeye.Behavior.Attack:
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			break;
		case GREnemyMonkeye.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemyMonkeye.Behavior.Search:
			this.ChooseNewBehavior();
			return;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.Think(dt);
			break;
		default:
			return;
		}
	}

	// Token: 0x0600311A RID: 12570 RVA: 0x0010BEC4 File Offset: 0x0010A0C4
	private bool TryChooseAttackBehavior(float toPlayerDistSq)
	{
		if (toPlayerDistSq < this.abilityAttackLaser.GetRange() * this.abilityAttackLaser.GetRange() && this.abilityAttackLaser.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.Attack, false);
			return true;
		}
		if (this.senseNearby.IsAnyoneNearby(this.abilityAttackDiscoWander.GetRange(), false) && this.abilityAttackDiscoWander.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.AttackDisco, false);
			return true;
		}
		if (this.senseNearby.IsAnyoneNearby(this.abilityAttackSlamdown.GetRange(), false) && this.abilityAttackSlamdown.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.AttackSlamdown, false);
			return true;
		}
		return false;
	}

	// Token: 0x0600311B RID: 12571 RVA: 0x0010BF68 File Offset: 0x0010A168
	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
		{
			if (this.agent.targetPlayer != null)
			{
				Vector3 position = GRPlayer.Get(this.agent.targetPlayer).transform.position;
				Vector3 a = position - base.transform.position;
				float magnitude = a.magnitude;
				if (this.TryChooseAttackBehavior(magnitude * magnitude))
				{
					return;
				}
				if (this.canChaseJump && this.abilityJump.IsCoolDownOver(this.chaseJumpMinInterval) && magnitude > this.attackRange + this.minChaseJumpDistance && GRSenseLineOfSight.HasNavmeshLineOfSight(base.transform.position, position, 10f))
				{
					Vector3 a2 = a / magnitude;
					float d = Mathf.Clamp(this.chaseJumpDistance, this.minChaseJumpDistance, magnitude - this.attackRange * 0.5f);
					NavMeshHit navMeshHit;
					if (NavMesh.SamplePosition(base.transform.position + a2 * d, out navMeshHit, 0.5f, AbilityHelperFunctions.GetNavMeshWalkableArea()))
					{
						this.agent.GetGameAgentManager().RequestJump(this.agent, base.transform.position, navMeshHit.position, 0.25f, 1.5f);
						return;
					}
				}
			}
			if (!this.abilityAttackLaser.IsCoolDownOver())
			{
				this.TrySetBehavior(GREnemyMonkeye.Behavior.Idle);
				return;
			}
			this.TrySetBehavior(GREnemyMonkeye.Behavior.Chase);
			return;
		}
		else
		{
			this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
			if (this.investigateLocation != null)
			{
				this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
				this.SetBehavior(GREnemyMonkeye.Behavior.Investigate, false);
				return;
			}
			if (this.abilityPatrol.HasValidPatrolPath())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Patrol, false);
				return;
			}
			this.SetBehavior(GREnemyMonkeye.Behavior.Idle, false);
			return;
		}
	}

	// Token: 0x0600311C RID: 12572 RVA: 0x0010C142 File Offset: 0x0010A342
	private void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x0600311D RID: 12573 RVA: 0x0010C160 File Offset: 0x0010A360
	private void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
			this.abilityIdle.UpdateAuthority(dt);
			return;
		case GREnemyMonkeye.Behavior.Patrol:
			this.abilityPatrol.UpdateAuthority(dt);
			return;
		case GREnemyMonkeye.Behavior.Stagger:
			this.abilityStagger.UpdateAuthority(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.agent.targetPlayer == null)
				{
					this.SetBehavior(GREnemyMonkeye.Behavior.Search, false);
					return;
				}
				this.SetBehavior(GREnemyMonkeye.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.Dying:
			this.abilityDie.UpdateAuthority(dt);
			return;
		case GREnemyMonkeye.Behavior.Chase:
		{
			this.abilityChase.UpdateAuthority(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Search, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float sqrMagnitude = (grplayer.transform.position - base.transform.position).sqrMagnitude;
				this.TryChooseAttackBehavior(sqrMagnitude);
				return;
			}
			break;
		}
		case GREnemyMonkeye.Behavior.Search:
			this.abilitySearch.UpdateAuthority(dt);
			if (this.abilitySearch.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.Attack:
			this.abilityAttackLaser.UpdateAuthority(dt);
			if (this.abilityAttackLaser.IsDone())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.UpdateAuthority(dt);
			if (this.abilityAttackDiscoWander.IsDone())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			this.abilityAttackSlamdown.UpdateAuthority(dt);
			if (this.abilityAttackSlamdown.IsDone())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.Investigate:
			this.abilityInvestigate.UpdateAuthority(dt);
			if (this.abilityInvestigate.IsDone())
			{
				this.investigateLocation = null;
			}
			if (GhostReactorManager.noiseDebugEnabled)
			{
				DebugUtil.DrawLine(base.transform.position, this.abilityInvestigate.GetTargetPos(), Color.green, true);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.Jump:
			this.abilityJump.UpdateAuthority(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600311E RID: 12574 RVA: 0x0010C374 File Offset: 0x0010A574
	private void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
			this.abilityIdle.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Patrol:
			this.abilityPatrol.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Dying:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Chase:
			this.abilityChase.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Search:
			this.abilitySearch.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Attack:
			this.abilityAttackLaser.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			this.abilityAttackSlamdown.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600311F RID: 12575 RVA: 0x0010C44C File Offset: 0x0010A64C
	private void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyMonkeye.BodyState.Bones)
		{
			this.hp -= hit.hitAmount;
			this.enemy.SetHP(this.hp);
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
				this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hit.hitByEntityId));
				this.SetBodyState(GREnemyMonkeye.BodyState.Destroyed, false);
				this.SetBehavior(GREnemyMonkeye.Behavior.Dying, false);
				return;
			}
			this.lastSeenTargetPosition = tool.transform.position;
			this.lastSeenTargetTime = Time.timeAsDouble;
			Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
			vector.y = 0f;
			this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
			if (this.allowStagger)
			{
				this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
				this.TrySetBehavior(GREnemyMonkeye.Behavior.Stagger);
				return;
			}
		}
		else if (this.currBodyState == GREnemyMonkeye.BodyState.Shell && this.armor != null)
		{
			this.armor.PlayBlockFx(hit.hitEntityPosition);
		}
	}

	// Token: 0x06003120 RID: 12576 RVA: 0x0010C5DD File Offset: 0x0010A7DD
	public void InstantDeath()
	{
		this.hp = 0;
		this.SetBodyState(GREnemyMonkeye.BodyState.Destroyed, false);
		this.SetBehavior(GREnemyMonkeye.Behavior.Dying, false);
	}

	// Token: 0x06003121 RID: 12577 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
	}

	// Token: 0x06003122 RID: 12578 RVA: 0x0010C5F6 File Offset: 0x0010A7F6
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x06003123 RID: 12579 RVA: 0x0010C600 File Offset: 0x0010A800
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyMonkeye.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyMonkeye.Behavior.Attack && this.currBehavior != GREnemyMonkeye.Behavior.AttackDisco && this.currBehavior != GREnemyMonkeye.Behavior.AttackSlamdown)
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

	// Token: 0x06003124 RID: 12580 RVA: 0x0010C77F File Offset: 0x0010A97F
	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if ((this.currBehavior == GREnemyMonkeye.Behavior.Attack || this.currBehavior == GREnemyMonkeye.Behavior.AttackDisco || this.currBehavior == GREnemyMonkeye.Behavior.AttackSlamdown) && player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			Vector3 hitImpulse = player.transform.position - base.transform.position;
			hitImpulse.y = 0f;
			hitImpulse = hitImpulse.normalized * 6f;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position, hitImpulse);
		}
		yield break;
	}

	// Token: 0x06003125 RID: 12581 RVA: 0x0010C798 File Offset: 0x0010A998
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("speed: <color=\"yellow\">{0}<color=\"white\"> patrol node:<color=\"yellow\">{1}/{2}<color=\"white\">", this.navAgent.speed, this.abilityPatrol.nextPatrolNode, (this.abilityPatrol.GetPatrolPath() != null) ? this.abilityPatrol.GetPatrolPath().patrolNodes.Count : 0));
	}

	// Token: 0x06003126 RID: 12582 RVA: 0x0010C83C File Offset: 0x0010AA3C
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		byte value2 = (byte)this.currBodyState;
		byte value3 = (byte)this.abilityPatrol.nextPatrolNode;
		int value4 = (this.targetPlayer == null) ? -1 : this.targetPlayer.ActorNumber;
		writer.Write(value);
		writer.Write(value2);
		writer.Write(this.hp);
		writer.Write(value3);
		writer.Write(value4);
	}

	// Token: 0x06003127 RID: 12583 RVA: 0x0010C8A8 File Offset: 0x0010AAA8
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyMonkeye.Behavior newBehavior = (GREnemyMonkeye.Behavior)reader.ReadByte();
		GREnemyMonkeye.BodyState newBodyState = (GREnemyMonkeye.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte nextPatrolNode = reader.ReadByte();
		int playerID = reader.ReadInt32();
		this.SetPatrolPath(this.entity.createData);
		this.abilityPatrol.SetNextPatrolNode((int)nextPatrolNode);
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
	}

	// Token: 0x06003128 RID: 12584 RVA: 0x00023994 File Offset: 0x00021B94
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06003129 RID: 12585 RVA: 0x0010C920 File Offset: 0x0010AB20
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
				break;
			case GameHitType.Flash:
				this.OnHitByFlash(gameComponent, hit);
				break;
			case GameHitType.Shield:
				this.OnHitByShield(gameComponent, hit);
				break;
			}
			if (gameComponent.gameEntity != null)
			{
				this.senseNearby.OnHitByPlayer(gameComponent.gameEntity.lastHeldByActorNumber);
			}
		}
	}

	// Token: 0x04003F25 RID: 16165
	public GameEntity entity;

	// Token: 0x04003F26 RID: 16166
	public GameAgent agent;

	// Token: 0x04003F27 RID: 16167
	public GREnemy enemy;

	// Token: 0x04003F28 RID: 16168
	public GRArmorEnemy armor;

	// Token: 0x04003F29 RID: 16169
	public GameHittable hittable;

	// Token: 0x04003F2A RID: 16170
	[SerializeField]
	private GRAttributes attributes;

	// Token: 0x04003F2B RID: 16171
	public GRSenseNearby senseNearby;

	// Token: 0x04003F2C RID: 16172
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003F2D RID: 16173
	public Animation anim;

	// Token: 0x04003F2E RID: 16174
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003F2F RID: 16175
	public GRAbilityChase abilityChase;

	// Token: 0x04003F30 RID: 16176
	public GRAbilityIdle abilitySearch;

	// Token: 0x04003F31 RID: 16177
	[FormerlySerializedAs("abilityAttackSwipe")]
	public GRAbilityAttackLaser abilityAttackLaser;

	// Token: 0x04003F32 RID: 16178
	public GRAbilityAttackSimpleWander abilityAttackDiscoWander;

	// Token: 0x04003F33 RID: 16179
	public GRAbilityAttackSimple abilityAttackSlamdown;

	// Token: 0x04003F34 RID: 16180
	public bool allowStagger;

	// Token: 0x04003F35 RID: 16181
	public GRAbilityStagger abilityStagger;

	// Token: 0x04003F36 RID: 16182
	public GRAbilityDie abilityDie;

	// Token: 0x04003F37 RID: 16183
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x04003F38 RID: 16184
	public GRAbilityPatrol abilityPatrol;

	// Token: 0x04003F39 RID: 16185
	public GRAbilityJump abilityJump;

	// Token: 0x04003F3A RID: 16186
	public List<Renderer> bones;

	// Token: 0x04003F3B RID: 16187
	public List<Renderer> always;

	// Token: 0x04003F3C RID: 16188
	public Transform headTransform;

	// Token: 0x04003F3D RID: 16189
	public float turnSpeed = 540f;

	// Token: 0x04003F3E RID: 16190
	public float attackRange = 1.5f;

	// Token: 0x04003F3F RID: 16191
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x04003F40 RID: 16192
	public NavMeshAgent navAgent;

	// Token: 0x04003F41 RID: 16193
	public AudioSource audioSource;

	// Token: 0x04003F42 RID: 16194
	public AudioClip damagedSound;

	// Token: 0x04003F43 RID: 16195
	public float damagedSoundVolume;

	// Token: 0x04003F44 RID: 16196
	public List<AudioClip> damagedSounds;

	// Token: 0x04003F45 RID: 16197
	private int damagedSoundIndex;

	// Token: 0x04003F46 RID: 16198
	public GameObject fxDamaged;

	// Token: 0x04003F47 RID: 16199
	private Vector3? investigateLocation;

	// Token: 0x04003F48 RID: 16200
	private float lastStaggerTime;

	// Token: 0x04003F49 RID: 16201
	public float staggerImmuneTime = 10f;

	// Token: 0x04003F4A RID: 16202
	private Transform target;

	// Token: 0x04003F4B RID: 16203
	[ReadOnly]
	public int hp;

	// Token: 0x04003F4C RID: 16204
	[ReadOnly]
	public GREnemyMonkeye.Behavior currBehavior;

	// Token: 0x04003F4D RID: 16205
	[ReadOnly]
	public GREnemyMonkeye.BodyState currBodyState;

	// Token: 0x04003F4E RID: 16206
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003F4F RID: 16207
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x04003F50 RID: 16208
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x04003F51 RID: 16209
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x04003F52 RID: 16210
	private double lastJumpEndtime;

	// Token: 0x04003F53 RID: 16211
	public bool canChaseJump = true;

	// Token: 0x04003F54 RID: 16212
	public float chaseJumpDistance = 5f;

	// Token: 0x04003F55 RID: 16213
	public float chaseJumpMinInterval = 1f;

	// Token: 0x04003F56 RID: 16214
	public float minChaseJumpDistance = 2f;

	// Token: 0x04003F57 RID: 16215
	private Rigidbody rigidBody;

	// Token: 0x04003F58 RID: 16216
	private List<Collider> colliders;

	// Token: 0x04003F59 RID: 16217
	private float lastHitPlayerTime;

	// Token: 0x04003F5A RID: 16218
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x04003F5B RID: 16219
	public float hearingRadius = 5f;

	// Token: 0x04003F5C RID: 16220
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04003F5D RID: 16221
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x02000789 RID: 1929
	public enum Behavior
	{
		// Token: 0x04003F5F RID: 16223
		Idle,
		// Token: 0x04003F60 RID: 16224
		Patrol,
		// Token: 0x04003F61 RID: 16225
		Stagger,
		// Token: 0x04003F62 RID: 16226
		Dying,
		// Token: 0x04003F63 RID: 16227
		Chase,
		// Token: 0x04003F64 RID: 16228
		Search,
		// Token: 0x04003F65 RID: 16229
		Attack,
		// Token: 0x04003F66 RID: 16230
		AttackDisco,
		// Token: 0x04003F67 RID: 16231
		AttackSlamdown,
		// Token: 0x04003F68 RID: 16232
		Investigate,
		// Token: 0x04003F69 RID: 16233
		Jump,
		// Token: 0x04003F6A RID: 16234
		Count
	}

	// Token: 0x0200078A RID: 1930
	public enum BodyState
	{
		// Token: 0x04003F6C RID: 16236
		Destroyed,
		// Token: 0x04003F6D RID: 16237
		Bones,
		// Token: 0x04003F6E RID: 16238
		Shell,
		// Token: 0x04003F6F RID: 16239
		Count
	}
}
