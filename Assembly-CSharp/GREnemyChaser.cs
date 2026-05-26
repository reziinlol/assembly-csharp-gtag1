using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CjLib;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000783 RID: 1923
public class GREnemyChaser : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent
{
	// Token: 0x060030DC RID: 12508 RVA: 0x00109D2C File Offset: 0x00107F2C
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

	// Token: 0x060030DD RID: 12509 RVA: 0x00109DB4 File Offset: 0x00107FB4
	public void OnEntityInit()
	{
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityChase.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilitySearch.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityAttackSwipe.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityPatrol.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityWander.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityFlashed.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
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

	// Token: 0x060030DE RID: 12510 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060030DF RID: 12511 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060030E0 RID: 12512 RVA: 0x0010A064 File Offset: 0x00108264
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x060030E1 RID: 12513 RVA: 0x0010A094 File Offset: 0x00108294
	private void Setup(long entityCreateData)
	{
		this.SetPatrolPath(entityCreateData);
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyChaser.Behavior.Patrol, true);
		}
		else
		{
			this.SetBehavior(GREnemyChaser.Behavior.Wander, true);
		}
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyChaser.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyChaser.BodyState.Bones, true);
	}

	// Token: 0x060030E2 RID: 12514 RVA: 0x0010A0E7 File Offset: 0x001082E7
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyChaser.Behavior.Jump, false);
	}

	// Token: 0x060030E3 RID: 12515 RVA: 0x0010A102 File Offset: 0x00108302
	private void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 11)
		{
			return;
		}
		this.SetBehavior((GREnemyChaser.Behavior)newState, false);
	}

	// Token: 0x060030E4 RID: 12516 RVA: 0x0010A116 File Offset: 0x00108316
	private void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyChaser.BodyState)newState, false);
	}

	// Token: 0x060030E5 RID: 12517 RVA: 0x0010A12C File Offset: 0x0010832C
	private void SetPatrolPath(long entityCreateData)
	{
		GRPatrolPath grpatrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(entityCreateData);
		this.abilityPatrol.SetPatrolPath(grpatrolPath);
	}

	// Token: 0x060030E6 RID: 12518 RVA: 0x0010A15C File Offset: 0x0010835C
	private void SetNextPatrolNode(int nextPatrolNode)
	{
		this.abilityPatrol.SetNextPatrolNode(nextPatrolNode);
	}

	// Token: 0x060030E7 RID: 12519 RVA: 0x0010A16A File Offset: 0x0010836A
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x060030E8 RID: 12520 RVA: 0x0010A173 File Offset: 0x00108373
	private bool TrySetBehavior(GREnemyChaser.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemyChaser.Behavior.Jump && newBehavior == GREnemyChaser.Behavior.Stagger)
		{
			return false;
		}
		if (newBehavior == GREnemyChaser.Behavior.Stagger && Time.time < this.lastStaggerTime + this.staggerImmuneTime)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x060030E9 RID: 12521 RVA: 0x0010A1A8 File Offset: 0x001083A8
	private void SetBehavior(GREnemyChaser.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.Stop();
			break;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.Stop();
			break;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyChaser.Behavior.Dying:
			this.abilityDie.Stop();
			break;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.Stop();
			break;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.Stop();
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.Stop();
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.Stop();
			break;
		case GREnemyChaser.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyChaser.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilitySearch.Start();
			break;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.Start();
			break;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.Start();
			break;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Start();
			this.lastStaggerTime = Time.time;
			break;
		case GREnemyChaser.Behavior.Dying:
			if (this.entity.IsAuthority())
			{
				this.entity.manager.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			this.abilityDie.Start();
			break;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.Start();
			this.investigateLocation = null;
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.Start();
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.Start();
			this.investigateLocation = null;
			this.abilityAttackSwipe.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.Start();
			break;
		case GREnemyChaser.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyChaser.Behavior.Jump:
			this.abilityJump.Start();
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x060030EA RID: 12522 RVA: 0x0010A433 File Offset: 0x00108633
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null)
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x060030EB RID: 12523 RVA: 0x0010A464 File Offset: 0x00108664
	private void SetBodyState(GREnemyChaser.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyChaser.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyChaser.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyChaser.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x060030EC RID: 12524 RVA: 0x0010A53C File Offset: 0x0010873C
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyChaser.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyChaser.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x060030ED RID: 12525 RVA: 0x0010A5D6 File Offset: 0x001087D6
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x060030EE RID: 12526 RVA: 0x0010A5E4 File Offset: 0x001087E4
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyChaser.tempRigs.Clear();
		GREnemyChaser.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyChaser.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyChaser.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
		case GREnemyChaser.Behavior.Patrol:
		case GREnemyChaser.Behavior.Investigate:
			this.ChooseNewBehavior();
			return;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemyChaser.Behavior.Stagger:
		case GREnemyChaser.Behavior.Dying:
		case GREnemyChaser.Behavior.Attack:
		case GREnemyChaser.Behavior.Flashed:
			break;
		case GREnemyChaser.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			this.ChooseNewBehavior();
			break;
		case GREnemyChaser.Behavior.Search:
			this.ChooseNewBehavior();
			return;
		default:
			return;
		}
	}

	// Token: 0x060030EF RID: 12527 RVA: 0x0010A6F4 File Offset: 0x001088F4
	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
		{
			if (this.agent.targetPlayer != null)
			{
				Vector3 position = GRPlayer.Get(this.agent.targetPlayer).transform.position;
				Vector3 a = position - base.transform.position;
				float magnitude = a.magnitude;
				if (magnitude < this.attackRange)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Attack, false);
				}
				else if (this.canChaseJump && this.abilityJump.IsCoolDownOver(this.chaseJumpMinInterval) && magnitude > this.attackRange + this.minChaseJumpDistance && GRSenseLineOfSight.HasNavmeshLineOfSight(base.transform.position, position, 10f))
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
			this.TrySetBehavior(GREnemyChaser.Behavior.Chase);
			return;
		}
		this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
		if (this.investigateLocation != null)
		{
			this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
			this.SetBehavior(GREnemyChaser.Behavior.Investigate, false);
			return;
		}
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyChaser.Behavior.Patrol, false);
			return;
		}
		this.SetBehavior(GREnemyChaser.Behavior.Wander, false);
	}

	// Token: 0x060030F0 RID: 12528 RVA: 0x0010A8C4 File Offset: 0x00108AC4
	private void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x060030F1 RID: 12529 RVA: 0x0010A8E4 File Offset: 0x00108AE4
	private void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.UpdateAuthority(dt);
			return;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.UpdateAuthority(dt);
			return;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.UpdateAuthority(dt);
			return;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.UpdateAuthority(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.agent.targetPlayer == null)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Search, false);
					return;
				}
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Dying:
			this.abilityDie.UpdateAuthority(dt);
			return;
		case GREnemyChaser.Behavior.Chase:
		{
			this.abilityChase.UpdateAuthority(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Search, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float num = this.attackRange * this.attackRange;
				if ((grplayer.transform.position - base.transform.position).sqrMagnitude < num)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Attack, false);
					return;
				}
			}
			break;
		}
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.UpdateAuthority(dt);
			if (this.abilitySearch.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.UpdateAuthority(dt);
			if (this.abilityAttackSwipe.IsDone())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.UpdateAuthority(dt);
			if (this.abilityFlashed.IsDone())
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Search, false);
					return;
				}
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Investigate:
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
		case GREnemyChaser.Behavior.Jump:
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

	// Token: 0x060030F2 RID: 12530 RVA: 0x0010AB08 File Offset: 0x00108D08
	private void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Dying:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			return;
		default:
			return;
		}
	}

	// Token: 0x060030F3 RID: 12531 RVA: 0x0010ABE0 File Offset: 0x00108DE0
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState != GREnemyChaser.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyChaser.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
		this.hp -= hit.hitAmount;
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
			this.SetBodyState(GREnemyChaser.BodyState.Destroyed, false);
			this.SetBehavior(GREnemyChaser.Behavior.Dying, false);
			return;
		}
		this.lastSeenTargetPosition = tool.transform.position;
		this.lastSeenTargetTime = Time.timeAsDouble;
		Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
		vector.y = 0f;
		this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
		this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
		this.TrySetBehavior(GREnemyChaser.Behavior.Stagger);
	}

	// Token: 0x060030F4 RID: 12532 RVA: 0x0010AD58 File Offset: 0x00108F58
	public void InstantDeath()
	{
		this.hp = 0;
		this.SetBodyState(GREnemyChaser.BodyState.Destroyed, false);
		this.SetBehavior(GREnemyChaser.Behavior.Dying, false);
	}

	// Token: 0x060030F5 RID: 12533 RVA: 0x0010AD74 File Offset: 0x00108F74
	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyChaser.BodyState.Shell)
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
				this.SetBodyState(GREnemyChaser.BodyState.Bones, false);
				if (grTool.gameEntity.IsHeldByLocalPlayer())
				{
					PlayerGameEvents.MiscEvent("GRArmorBreak_" + base.name, 1);
				}
				if (grTool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
				{
					this.armor.FragmentArmor();
				}
			}
			else if (grTool != null)
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.lastSeenTargetPosition = grTool.transform.position;
				this.lastSeenTargetTime = Time.timeAsDouble;
				Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
				vector.y = 0f;
				this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
				this.RefreshBody();
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
		GRToolFlash component = grTool.GetComponent<GRToolFlash>();
		if (component != null)
		{
			this.abilityFlashed.SetStunTime(component.stunDuration);
		}
		this.TrySetBehavior(GREnemyChaser.Behavior.Flashed);
	}

	// Token: 0x060030F6 RID: 12534 RVA: 0x0010AF2E File Offset: 0x0010912E
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		Debug.Log(string.Format("Chaser On Hit By Shield dmg:{0} impulse:{1} size:{2}", hit.hitAmount, hit.hitImpulse, hit.hitImpulse.magnitude));
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x060030F7 RID: 12535 RVA: 0x0010AF70 File Offset: 0x00109170
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyChaser.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyChaser.Behavior.Attack)
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

	// Token: 0x060030F8 RID: 12536 RVA: 0x0010B0DD File Offset: 0x001092DD
	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if (this.currBehavior == GREnemyChaser.Behavior.Attack && player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position);
		}
		yield break;
	}

	// Token: 0x060030F9 RID: 12537 RVA: 0x0010B0F4 File Offset: 0x001092F4
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("speed: <color=\"yellow\">{0}<color=\"white\"> patrol node:<color=\"yellow\">{1}/{2}<color=\"white\">", this.navAgent.speed, this.abilityPatrol.nextPatrolNode, (this.abilityPatrol.GetPatrolPath() != null) ? this.abilityPatrol.GetPatrolPath().patrolNodes.Count : 0));
		strings.Add(string.Format("Dest: <color=\"yellow\">{0}<color=\"white\"> Pos: <color=\"yellow\">{1}<color=\"white\">", this.agent.navAgent.destination, this.agent.transform.position));
	}

	// Token: 0x060030FA RID: 12538 RVA: 0x0010B1D4 File Offset: 0x001093D4
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

	// Token: 0x060030FB RID: 12539 RVA: 0x0010B240 File Offset: 0x00109440
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyChaser.Behavior newBehavior = (GREnemyChaser.Behavior)reader.ReadByte();
		GREnemyChaser.BodyState newBodyState = (GREnemyChaser.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte nextPatrolNode = reader.ReadByte();
		int playerID = reader.ReadInt32();
		this.SetPatrolPath(this.entity.createData);
		this.SetNextPatrolNode((int)nextPatrolNode);
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
	}

	// Token: 0x060030FC RID: 12540 RVA: 0x00023994 File Offset: 0x00021B94
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x060030FD RID: 12541 RVA: 0x0010B2B4 File Offset: 0x001094B4
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

	// Token: 0x04003ED2 RID: 16082
	public GameEntity entity;

	// Token: 0x04003ED3 RID: 16083
	public GameAgent agent;

	// Token: 0x04003ED4 RID: 16084
	public GREnemy enemy;

	// Token: 0x04003ED5 RID: 16085
	public GRArmorEnemy armor;

	// Token: 0x04003ED6 RID: 16086
	public GameHittable hittable;

	// Token: 0x04003ED7 RID: 16087
	[SerializeField]
	private GRAttributes attributes;

	// Token: 0x04003ED8 RID: 16088
	public GRSenseNearby senseNearby;

	// Token: 0x04003ED9 RID: 16089
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003EDA RID: 16090
	public Animation anim;

	// Token: 0x04003EDB RID: 16091
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003EDC RID: 16092
	public GRAbilityChase abilityChase;

	// Token: 0x04003EDD RID: 16093
	public GRAbilityIdle abilitySearch;

	// Token: 0x04003EDE RID: 16094
	public GRAbilityAttackSwipe abilityAttackSwipe;

	// Token: 0x04003EDF RID: 16095
	public GRAbilityStagger abilityStagger;

	// Token: 0x04003EE0 RID: 16096
	public GRAbilityDie abilityDie;

	// Token: 0x04003EE1 RID: 16097
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x04003EE2 RID: 16098
	public GRAbilityPatrol abilityPatrol;

	// Token: 0x04003EE3 RID: 16099
	public GRAbilityWander abilityWander;

	// Token: 0x04003EE4 RID: 16100
	public GRAbilityFlashed abilityFlashed;

	// Token: 0x04003EE5 RID: 16101
	public GRAbilityJump abilityJump;

	// Token: 0x04003EE6 RID: 16102
	public List<Renderer> bones;

	// Token: 0x04003EE7 RID: 16103
	public List<Renderer> always;

	// Token: 0x04003EE8 RID: 16104
	public Transform coreMarker;

	// Token: 0x04003EE9 RID: 16105
	public GRCollectible corePrefab;

	// Token: 0x04003EEA RID: 16106
	public Transform headTransform;

	// Token: 0x04003EEB RID: 16107
	public float turnSpeed = 540f;

	// Token: 0x04003EEC RID: 16108
	public SoundBankPlayer chaseSoundBank;

	// Token: 0x04003EED RID: 16109
	public float attackRange = 1.5f;

	// Token: 0x04003EEE RID: 16110
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x04003EEF RID: 16111
	public NavMeshAgent navAgent;

	// Token: 0x04003EF0 RID: 16112
	public AudioSource audioSource;

	// Token: 0x04003EF1 RID: 16113
	public AudioClip damagedSound;

	// Token: 0x04003EF2 RID: 16114
	public float damagedSoundVolume;

	// Token: 0x04003EF3 RID: 16115
	public List<AudioClip> damagedSounds;

	// Token: 0x04003EF4 RID: 16116
	private int damagedSoundIndex;

	// Token: 0x04003EF5 RID: 16117
	public GameObject fxDamaged;

	// Token: 0x04003EF6 RID: 16118
	private Vector3? investigateLocation;

	// Token: 0x04003EF7 RID: 16119
	private float lastStaggerTime;

	// Token: 0x04003EF8 RID: 16120
	public float staggerImmuneTime = 10f;

	// Token: 0x04003EF9 RID: 16121
	private Transform target;

	// Token: 0x04003EFA RID: 16122
	[ReadOnly]
	public int hp;

	// Token: 0x04003EFB RID: 16123
	[ReadOnly]
	public GREnemyChaser.Behavior currBehavior;

	// Token: 0x04003EFC RID: 16124
	[ReadOnly]
	public GREnemyChaser.BodyState currBodyState;

	// Token: 0x04003EFD RID: 16125
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003EFE RID: 16126
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x04003EFF RID: 16127
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x04003F00 RID: 16128
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x04003F01 RID: 16129
	public bool canChaseJump = true;

	// Token: 0x04003F02 RID: 16130
	public float chaseJumpDistance = 5f;

	// Token: 0x04003F03 RID: 16131
	public float chaseJumpMinInterval = 1f;

	// Token: 0x04003F04 RID: 16132
	public float minChaseJumpDistance = 2f;

	// Token: 0x04003F05 RID: 16133
	public static RaycastHit[] visibilityHits = new RaycastHit[16];

	// Token: 0x04003F06 RID: 16134
	private Rigidbody rigidBody;

	// Token: 0x04003F07 RID: 16135
	private List<Collider> colliders;

	// Token: 0x04003F08 RID: 16136
	private float lastHitPlayerTime;

	// Token: 0x04003F09 RID: 16137
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x04003F0A RID: 16138
	public float hearingRadius = 5f;

	// Token: 0x04003F0B RID: 16139
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04003F0C RID: 16140
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x02000784 RID: 1924
	public enum Behavior
	{
		// Token: 0x04003F0E RID: 16142
		Idle,
		// Token: 0x04003F0F RID: 16143
		Patrol,
		// Token: 0x04003F10 RID: 16144
		Wander,
		// Token: 0x04003F11 RID: 16145
		Stagger,
		// Token: 0x04003F12 RID: 16146
		Dying,
		// Token: 0x04003F13 RID: 16147
		Chase,
		// Token: 0x04003F14 RID: 16148
		Search,
		// Token: 0x04003F15 RID: 16149
		Attack,
		// Token: 0x04003F16 RID: 16150
		Flashed,
		// Token: 0x04003F17 RID: 16151
		Investigate,
		// Token: 0x04003F18 RID: 16152
		Jump,
		// Token: 0x04003F19 RID: 16153
		Count
	}

	// Token: 0x02000785 RID: 1925
	public enum BodyState
	{
		// Token: 0x04003F1B RID: 16155
		Destroyed,
		// Token: 0x04003F1C RID: 16156
		Bones,
		// Token: 0x04003F1D RID: 16157
		Shell,
		// Token: 0x04003F1E RID: 16158
		Count
	}
}
