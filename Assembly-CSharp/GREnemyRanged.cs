using System;
using System.Collections.Generic;
using System.IO;
using CjLib;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

// Token: 0x02000793 RID: 1939
public class GREnemyRanged : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameProjectileLauncher, IGameEntityDebugComponent
{
	// Token: 0x0600317A RID: 12666 RVA: 0x0010ED14 File Offset: 0x0010CF14
	private bool IsMoving()
	{
		return this.navAgent.velocity.sqrMagnitude > 0f;
	}

	// Token: 0x0600317B RID: 12667 RVA: 0x0010ED3C File Offset: 0x0010CF3C
	private void SoftResetThrowableHead()
	{
		this.headRemoved = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
		this.headLightReset = true;
		this.spitterLightTurnOffTime = Time.timeAsDouble + this.spitterLightTurnOffDelay;
	}

	// Token: 0x0600317C RID: 12668 RVA: 0x0010EDA8 File Offset: 0x0010CFA8
	private void ForceResetThrowableHead()
	{
		this.headRemoved = false;
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x0600317D RID: 12669 RVA: 0x0010EE0C File Offset: 0x0010D00C
	private void ForceHeadToDeadState()
	{
		this.headRemoved = false;
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x0600317E RID: 12670 RVA: 0x0010EE70 File Offset: 0x0010D070
	private void EnableVFXForShoulderHead()
	{
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(true);
		this.spitterHeadOnShouldersVFX.SetActive(true);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x0600317F RID: 12671 RVA: 0x0010EECC File Offset: 0x0010D0CC
	private void EnableVFXForHeadInHand()
	{
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(false);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(true);
		this.spitterHeadInHandLight.SetActive(true);
		this.spitterHeadInHandVFX.SetActive(true);
	}

	// Token: 0x06003180 RID: 12672 RVA: 0x0010EF28 File Offset: 0x0010D128
	private void DisableHeadInHand()
	{
		this.headLightReset = false;
		this.spitterHeadInHand.SetActive(false);
	}

	// Token: 0x06003181 RID: 12673 RVA: 0x0010EF40 File Offset: 0x0010D140
	private void DisableHeadOnShoulderAndHeadInHand()
	{
		this.headLightReset = false;
		this.headRemoved = false;
		this.spitterHeadOnShoulders.SetActive(false);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x06003182 RID: 12674 RVA: 0x0010EFA4 File Offset: 0x0010D1A4
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		this.visibilityLayerMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		this.senseNearby.Setup(this.headTransform, this.entity);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06003183 RID: 12675 RVA: 0x0010F060 File Offset: 0x0010D260
	public void OnEntityInit()
	{
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityPatrol.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityFlashed.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityKeepDistance.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
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

	// Token: 0x06003184 RID: 12676 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06003185 RID: 12677 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06003186 RID: 12678 RVA: 0x0010F260 File Offset: 0x0010D460
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
		this.DestroyProjectile();
	}

	// Token: 0x06003187 RID: 12679 RVA: 0x0010F298 File Offset: 0x0010D498
	public void Setup(long entityCreateData)
	{
		this.SetPatrolPath(entityCreateData);
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyRanged.Behavior.Patrol, true);
		}
		else
		{
			this.SetBehavior(GREnemyRanged.Behavior.Idle, true);
		}
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyRanged.BodyState.Shell, true);
		}
		else
		{
			this.SetBodyState(GREnemyRanged.BodyState.Bones, true);
		}
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
	}

	// Token: 0x06003188 RID: 12680 RVA: 0x0010F316 File Offset: 0x0010D516
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyRanged.Behavior.Jump, false);
	}

	// Token: 0x06003189 RID: 12681 RVA: 0x0010F331 File Offset: 0x0010D531
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 11)
		{
			return;
		}
		this.SetBehavior((GREnemyRanged.Behavior)newState, false);
	}

	// Token: 0x0600318A RID: 12682 RVA: 0x0010F345 File Offset: 0x0010D545
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyRanged.BodyState)newState, false);
	}

	// Token: 0x0600318B RID: 12683 RVA: 0x0010F358 File Offset: 0x0010D558
	public void SetPatrolPath(long entityCreateData)
	{
		this.abilityPatrol.SetPatrolPath(GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(entityCreateData));
	}

	// Token: 0x0600318C RID: 12684 RVA: 0x0010F37B File Offset: 0x0010D57B
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x0600318D RID: 12685 RVA: 0x0010F384 File Offset: 0x0010D584
	public bool TrySetBehavior(GREnemyRanged.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemyRanged.Behavior.Jump && newBehavior == GREnemyRanged.Behavior.Stagger)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x0600318E RID: 12686 RVA: 0x0010F3A0 File Offset: 0x0010D5A0
	public void SetBehavior(GREnemyRanged.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Patrol:
			this.abilityPatrol.Stop();
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.Stop();
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
			if (newBehavior != GREnemyRanged.Behavior.RangedAttack)
			{
				this.SoftResetThrowableHead();
			}
			break;
		case GREnemyRanged.Behavior.RangedAttack:
			if (newBehavior != GREnemyRanged.Behavior.RangedAttackCooldown)
			{
				this.ForceResetThrowableHead();
			}
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			this.ForceResetThrowableHead();
			this.abilityKeepDistance.Stop();
			break;
		case GREnemyRanged.Behavior.Flashed:
			this.abilityFlashed.Stop();
			break;
		case GREnemyRanged.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyRanged.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Idle:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyRangedIdleSearch", 0.1f, 1f);
			break;
		case GREnemyRanged.Behavior.Patrol:
			this.targetPlayer = null;
			this.abilityPatrol.Start();
			break;
		case GREnemyRanged.Behavior.Search:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyRangedWalk", 0.1f, 1f);
			this.navAgent.speed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
			this.lastMoving = false;
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.Start();
			if (this.entity.IsAuthority())
			{
				this.entity.manager.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
			this.PlayAnim("GREnemyRangedWalk", 0.1f, 1f);
			this.navAgent.speed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.ChaseSpeed);
			this.EnableVFXForShoulderHead();
			this.chaseAbilitySound.Play(this.audioSecondarySource);
			break;
		case GREnemyRanged.Behavior.RangedAttack:
			this.PlayAnim("GREnemyRangedAttack01", 0.1f, 1f);
			this.navAgent.speed = 0f;
			this.navAgent.velocity = Vector3.zero;
			this.headRemovaltime = PhotonNetwork.Time + (double)this.headRemovalFrame;
			this.attackAbilitySound.Play(this.audioSource);
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			this.lastMoving = true;
			this.abilityKeepDistance.SetTargetPlayer(this.targetPlayer);
			this.abilityKeepDistance.Start();
			break;
		case GREnemyRanged.Behavior.Flashed:
			this.abilityFlashed.Start();
			break;
		case GREnemyRanged.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyRanged.Behavior.Jump:
			this.abilityJump.Start();
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x0600318F RID: 12687 RVA: 0x0010F6A8 File Offset: 0x0010D8A8
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null)
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x06003190 RID: 12688 RVA: 0x0010F6D8 File Offset: 0x0010D8D8
	public void SetBodyState(GREnemyRanged.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
			this.ForceResetThrowableHead();
			for (int i = 0; i < this.colliders.Count; i++)
			{
				this.colliders[i].enabled = true;
			}
			break;
		case GREnemyRanged.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyRanged.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
			this.DisableHeadOnShoulderAndHeadInHand();
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyRanged.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyRanged.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x06003191 RID: 12689 RVA: 0x0010F7E8 File Offset: 0x0010D9E8
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, true);
			this.DisableHeadOnShoulderAndHeadInHand();
			return;
		case GREnemyRanged.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyRanged.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003192 RID: 12690 RVA: 0x0010F888 File Offset: 0x0010DA88
	private void Update()
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(Time.deltaTime);
		}
		else
		{
			this.OnUpdateRemote(Time.deltaTime);
		}
		this.UpdateShared();
	}

	// Token: 0x06003193 RID: 12691 RVA: 0x0010F8B8 File Offset: 0x0010DAB8
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		if (!GhostReactorManager.AggroDisabled)
		{
			GREnemyRanged.Behavior behavior = this.currBehavior;
			if (behavior > GREnemyRanged.Behavior.Search)
			{
				if (behavior == GREnemyRanged.Behavior.RangedAttackCooldown)
				{
					this.abilityKeepDistance.Think(dt);
					this.UpdateTarget();
					return;
				}
				if (behavior != GREnemyRanged.Behavior.Investigate)
				{
					return;
				}
			}
			this.UpdateTarget();
		}
	}

	// Token: 0x06003194 RID: 12692 RVA: 0x0010F908 File Offset: 0x0010DB08
	private void UpdateTarget()
	{
		this.bestTargetPlayer = null;
		this.bestTargetNetPlayer = null;
		GREnemyRanged.tempRigs.Clear();
		GREnemyRanged.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyRanged.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyRanged.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		if (vrrig != null)
		{
			GRPlayer component = vrrig.GetComponent<GRPlayer>();
			if (component != null && component.State != GRPlayer.GRPlayerState.Ghost)
			{
				this.bestTargetPlayer = component;
				this.bestTargetNetPlayer = vrrig.OwningNetPlayer;
				this.lastSeenTargetTime = Time.timeAsDouble;
				this.lastSeenTargetPosition = vrrig.transform.position;
			}
		}
	}

	// Token: 0x06003195 RID: 12693 RVA: 0x0010F9BC File Offset: 0x0010DBBC
	private void ChooseNewBehavior()
	{
		if (this.bestTargetPlayer != null && Time.timeAsDouble - this.lastSeenTargetTime < (double)this.sightLostFollowStopTime)
		{
			this.targetPlayer = this.bestTargetNetPlayer;
			this.lastSeenTargetTime = Time.timeAsDouble;
			this.investigateLocation = null;
			this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
			return;
		}
		if (Time.timeAsDouble - this.lastSeenTargetTime < (double)this.searchTime)
		{
			this.SetBehavior(GREnemyRanged.Behavior.Search, false);
			return;
		}
		this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
		if (this.investigateLocation != null)
		{
			this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
			this.SetBehavior(GREnemyRanged.Behavior.Investigate, false);
			return;
		}
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyRanged.Behavior.Patrol, false);
			return;
		}
		this.SetBehavior(GREnemyRanged.Behavior.Idle, false);
	}

	// Token: 0x06003196 RID: 12694 RVA: 0x0010FAA4 File Offset: 0x0010DCA4
	private void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Idle:
			this.ChooseNewBehavior();
			break;
		case GREnemyRanged.Behavior.Patrol:
			this.abilityPatrol.UpdateAuthority(dt);
			this.ChooseNewBehavior();
			break;
		case GREnemyRanged.Behavior.Search:
			this.UpdateSearch();
			this.ChooseNewBehavior();
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.UpdateAuthority(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyRanged.Behavior.Search, false);
				}
				else
				{
					this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				}
			}
			break;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.UpdateAuthority(dt);
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
			if (this.targetPlayer != null)
			{
				GRPlayer grplayer = GRPlayer.Get(this.targetPlayer.ActorNumber);
				if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
				{
					Vector3 position = grplayer.transform.position;
					Vector3 position2 = base.transform.position;
					float magnitude = (position - position2).magnitude;
					if (magnitude > this.loseSightDist)
					{
						this.ChooseNewBehavior();
					}
					else
					{
						float num = Vector3.Distance(position, this.headTransform.position);
						bool flag = false;
						if (num < this.sightDist)
						{
							flag = (Physics.RaycastNonAlloc(new Ray(this.headTransform.position, position - this.headTransform.position), GREnemyChaser.visibilityHits, num, this.visibilityLayerMask.value, QueryTriggerInteraction.Ignore) < 1);
						}
						if (flag)
						{
							this.lastSeenTargetPosition = position;
							this.lastSeenTargetTime = Time.timeAsDouble;
						}
						if (Time.timeAsDouble - this.lastSeenTargetTime < (double)this.sightLostFollowStopTime)
						{
							this.searchPosition = position;
							this.agent.RequestDestination(this.lastSeenTargetPosition);
							if (flag)
							{
								this.rangedTargetPosition = position;
								Vector3 b = Vector3.up * 0.4f;
								this.rangedTargetPosition += b;
								if (magnitude < this.rangedAttackDistMax)
								{
									this.behaviorEndTime = Time.timeAsDouble + (double)this.rangedAttackChargeTime;
									this.SetBehavior(GREnemyRanged.Behavior.RangedAttack, false);
									GhostReactorManager.Get(this.entity).RequestFireProjectile(this.entity.id, this.rangedProjectileFirePoint.position, this.rangedTargetPosition, PhotonNetwork.Time + (double)this.rangedAttackChargeTime);
								}
							}
						}
						else
						{
							this.ChooseNewBehavior();
						}
					}
				}
			}
			break;
		case GREnemyRanged.Behavior.RangedAttack:
			if (Time.timeAsDouble > this.behaviorEndTime)
			{
				if (this.targetPlayer != null)
				{
					GRPlayer grplayer2 = GRPlayer.Get(this.targetPlayer.ActorNumber);
					if (grplayer2 != null && grplayer2.State == GRPlayer.GRPlayerState.Alive)
					{
						this.rangedTargetPosition = grplayer2.transform.position;
					}
				}
				this.SetBehavior(GREnemyRanged.Behavior.RangedAttackCooldown, false);
				this.behaviorEndTime = Time.timeAsDouble + (double)this.rangedAttackRecoverTime;
			}
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			if (Time.timeAsDouble > this.behaviorEndTime)
			{
				this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				this.behaviorEndTime = Time.timeAsDouble;
			}
			else
			{
				this.abilityKeepDistance.UpdateAuthority(dt);
			}
			break;
		case GREnemyRanged.Behavior.Flashed:
			this.abilityFlashed.UpdateAuthority(dt);
			if (this.abilityFlashed.IsDone())
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyRanged.Behavior.Search, false);
				}
				else
				{
					this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				}
			}
			break;
		case GREnemyRanged.Behavior.Investigate:
			this.abilityInvestigate.UpdateAuthority(dt);
			if (GhostReactorManager.noiseDebugEnabled)
			{
				DebugUtil.DrawLine(base.transform.position, this.abilityInvestigate.GetTargetPos(), Color.green, true);
			}
			this.ChooseNewBehavior();
			break;
		case GREnemyRanged.Behavior.Jump:
			this.abilityJump.UpdateAuthority(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
			}
			break;
		}
		GameAgent.UpdateFacing(base.transform, this.navAgent, this.targetPlayer, this.turnSpeed);
	}

	// Token: 0x06003197 RID: 12695 RVA: 0x0010FE80 File Offset: 0x0010E080
	private void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Patrol:
			this.abilityPatrol.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.Search:
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
		case GREnemyRanged.Behavior.RangedAttack:
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			this.abilityKeepDistance.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.Flashed:
			this.abilityFlashed.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			if (GhostReactorManager.noiseDebugEnabled)
			{
				DebugUtil.DrawLine(base.transform.position, this.abilityInvestigate.GetTargetPos(), Color.green, true);
				return;
			}
			break;
		case GREnemyRanged.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			break;
		default:
			return;
		}
	}

	// Token: 0x06003198 RID: 12696 RVA: 0x0010FF48 File Offset: 0x0010E148
	public void UpdateShared()
	{
		if (this.rangedAttackQueued)
		{
			if (!this.headRemoved && this.currBehavior == GREnemyRanged.Behavior.RangedAttack && PhotonNetwork.Time >= this.headRemovaltime)
			{
				this.headRemoved = true;
				this.EnableVFXForHeadInHand();
			}
			if (PhotonNetwork.Time > this.queuedFiringTime)
			{
				this.rangedAttackQueued = false;
				this.FireRangedAttack(this.queuedFiringPosition, this.queuedTargetPosition);
			}
		}
		if (this.headLightReset && Time.timeAsDouble > this.spitterLightTurnOffTime)
		{
			this.spitterHeadOnShouldersLight.SetActive(false);
			this.headLightReset = false;
		}
	}

	// Token: 0x06003199 RID: 12697 RVA: 0x0010FFD8 File Offset: 0x0010E1D8
	private void UpdateSearch()
	{
		Vector3 vector = this.searchPosition - base.transform.position;
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
		if (vector2.sqrMagnitude < 0.15f)
		{
			Vector3 b = this.lastSeenTargetPosition - this.searchPosition;
			b.y = 0f;
			this.searchPosition = this.lastSeenTargetPosition + b;
		}
		if (this.IsMoving())
		{
			if (!this.lastMoving)
			{
				this.PlayAnim("GREnemyRangedWalk", 0.1f, 1f);
				this.lastMoving = true;
			}
		}
		else if (this.lastMoving)
		{
			this.PlayAnim("GREnemyRangedWalk", 0.1f, 1f);
			this.lastMoving = false;
		}
		this.agent.RequestDestination(this.searchPosition);
		if (Time.timeAsDouble - this.lastSeenTargetTime > (double)this.searchTime)
		{
			this.ChooseNewBehavior();
		}
	}

	// Token: 0x0600319A RID: 12698 RVA: 0x001100D0 File Offset: 0x0010E2D0
	private void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState != GREnemyRanged.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyRanged.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
		this.hp -= hit.hitAmount;
		this.audioSource.PlayOneShot(this.damagedSound, this.damagedSoundVolume);
		if (this.fxDamaged != null)
		{
			this.fxDamaged.SetActive(false);
			this.fxDamaged.SetActive(true);
		}
		if (this.hp <= 0)
		{
			this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hit.hitByEntityId));
			this.SetBodyState(GREnemyRanged.BodyState.Destroyed, false);
			this.SetBehavior(GREnemyRanged.Behavior.Dying, false);
			return;
		}
		this.lastSeenTargetPosition = tool.transform.position;
		this.lastSeenTargetTime = Time.timeAsDouble;
		Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
		vector.y = 0f;
		this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
		this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
		this.TrySetBehavior(GREnemyRanged.Behavior.Stagger);
	}

	// Token: 0x0600319B RID: 12699 RVA: 0x00110212 File Offset: 0x0010E412
	public void InstantDeath()
	{
		this.hp = 0;
		this.SetBodyState(GREnemyRanged.BodyState.Destroyed, false);
		this.SetBehavior(GREnemyRanged.Behavior.Dying, false);
	}

	// Token: 0x0600319C RID: 12700 RVA: 0x0011022C File Offset: 0x0010E42C
	private void OnHitByFlash(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyRanged.BodyState.Shell)
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
				this.SetBodyState(GREnemyRanged.BodyState.Bones, false);
				if (tool.gameEntity.IsHeldByLocalPlayer())
				{
					PlayerGameEvents.MiscEvent("GRArmorBreak_" + base.name, 1);
				}
				if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
				{
					this.armor.FragmentArmor();
				}
			}
			else if (tool != null)
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.lastSeenTargetPosition = tool.transform.position;
				this.lastSeenTargetTime = Time.timeAsDouble;
				Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
				vector.y = 0f;
				this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
				this.SetBehavior(GREnemyRanged.Behavior.Search, false);
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
		GRToolFlash component = tool.GetComponent<GRToolFlash>();
		if (component != null)
		{
			this.abilityFlashed.SetStunTime(component.stunDuration);
		}
		this.SetBehavior(GREnemyRanged.Behavior.Flashed, false);
	}

	// Token: 0x0600319D RID: 12701 RVA: 0x001103EE File Offset: 0x0010E5EE
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x0600319E RID: 12702 RVA: 0x001103F8 File Offset: 0x0010E5F8
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

	// Token: 0x0600319F RID: 12703 RVA: 0x00110464 File Offset: 0x0010E664
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyRanged.Behavior newBehavior = (GREnemyRanged.Behavior)reader.ReadByte();
		GREnemyRanged.BodyState newBodyState = (GREnemyRanged.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte b = reader.ReadByte();
		int playerID = reader.ReadInt32();
		this.SetPatrolPath((long)((int)this.entity.createData));
		this.abilityPatrol.SetNextPatrolNode((int)b);
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
	}

	// Token: 0x060031A0 RID: 12704 RVA: 0x00023994 File Offset: 0x00021B94
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x060031A1 RID: 12705 RVA: 0x001104E0 File Offset: 0x0010E6E0
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

	// Token: 0x060031A2 RID: 12706 RVA: 0x00110541 File Offset: 0x0010E741
	public void RequestRangedAttack(Vector3 firingPosition, Vector3 targetPosition, double fireTime)
	{
		this.rangedAttackQueued = true;
		this.queuedFiringTime = fireTime;
		this.queuedFiringPosition = firingPosition;
		this.queuedTargetPosition = targetPosition;
	}

	// Token: 0x060031A3 RID: 12707 RVA: 0x00110560 File Offset: 0x0010E760
	private void DestroyProjectile()
	{
		if (this.entity.IsAuthority() && this.rangedProjectileInstance != null)
		{
			GameEntity component = this.rangedProjectileInstance.GetComponent<GameEntity>();
			if (component != null)
			{
				component.manager.RequestDestroyItem(component.id);
			}
		}
	}

	// Token: 0x060031A4 RID: 12708 RVA: 0x001105B0 File Offset: 0x0010E7B0
	private void FireRangedAttack(Vector3 launchPosition, Vector3 targetPosition)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		this.DisableHeadInHand();
		this.DestroyProjectile();
		Vector3 forward;
		if (GREnemyRanged.CalculateLaunchDirection(launchPosition, targetPosition, this.projectileSpeed, out forward))
		{
			this.entity.manager.RequestCreateItem(this.rangedProjectilePrefab.name.GetStaticHash(), launchPosition, Quaternion.LookRotation(forward, Vector3.up), (long)this.entity.GetNetId());
		}
	}

	// Token: 0x060031A5 RID: 12709 RVA: 0x00110624 File Offset: 0x0010E824
	public static bool CalculateLaunchDirection(Vector3 startPos, Vector3 targetPos, float speed, out Vector3 direction)
	{
		direction = Vector3.zero;
		Vector3 vector = targetPos - startPos;
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
		float magnitude = vector2.magnitude;
		Vector3 normalized = vector2.normalized;
		float y = vector.y;
		float num = 9.8f;
		float num2 = speed * speed;
		float num3 = num2 * num2 - num * (num * magnitude * magnitude + 2f * y * num2);
		if (num3 < 0f)
		{
			return false;
		}
		int num4 = 0;
		float num5 = Mathf.Sqrt(num3);
		float num6 = (num2 + num5) / (num * magnitude);
		float num7 = (num2 - num5) / (num * magnitude);
		float num8 = num2 / (num6 * num6 + 1f);
		float num9 = num2 / (num7 * num7 + 1f);
		float num10 = (num4 != 0) ? Mathf.Min(num8, num9) : Mathf.Max(num8, num9);
		float num11 = (num4 != 0) ? ((num8 < num9) ? Mathf.Sign(num6) : Mathf.Sign(num7)) : ((num8 > num9) ? Mathf.Sign(num6) : Mathf.Sign(num7));
		float d = Mathf.Sqrt(num10);
		float num12 = Mathf.Sqrt(Mathf.Abs(num2 - num10));
		direction = (normalized * d + new Vector3(0f, num12 * num11, 0f)).normalized;
		return true;
	}

	// Token: 0x060031A6 RID: 12710 RVA: 0x00110780 File Offset: 0x0010E980
	public void OnProjectileInit(GRRangedEnemyProjectile projectile)
	{
		this.rangedProjectileInstance = projectile.gameObject;
	}

	// Token: 0x060031A7 RID: 12711 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnProjectileHit(GRRangedEnemyProjectile projectile, Collision collision)
	{
	}

	// Token: 0x060031A8 RID: 12712 RVA: 0x00110790 File Offset: 0x0010E990
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("speed: <color=\"yellow\">{0}<color=\"white\"> patrol node:<color=\"yellow\">{1}/{2}<color=\"white\">", this.navAgent.speed, this.abilityPatrol.nextPatrolNode, (this.abilityPatrol.GetPatrolPath() != null) ? this.abilityPatrol.GetPatrolPath().patrolNodes.Count : 0));
		if (this.targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(this.targetPlayer.ActorNumber);
			if (grplayer != null)
			{
				float magnitude = (grplayer.transform.position - base.transform.position).magnitude;
				strings.Add(string.Format("TargetDis: <color=\"yellow\">{0}<color=\"white\"> ", magnitude));
			}
		}
	}

	// Token: 0x04003FF7 RID: 16375
	public GameEntity entity;

	// Token: 0x04003FF8 RID: 16376
	public GameAgent agent;

	// Token: 0x04003FF9 RID: 16377
	public GREnemy enemy;

	// Token: 0x04003FFA RID: 16378
	public GRArmorEnemy armor;

	// Token: 0x04003FFB RID: 16379
	public GameHittable hittable;

	// Token: 0x04003FFC RID: 16380
	public GRAttributes attributes;

	// Token: 0x04003FFD RID: 16381
	public Animation anim;

	// Token: 0x04003FFE RID: 16382
	public GRSenseNearby senseNearby;

	// Token: 0x04003FFF RID: 16383
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04004000 RID: 16384
	public GRAbilityStagger abilityStagger;

	// Token: 0x04004001 RID: 16385
	public GRAbilityDie abilityDie;

	// Token: 0x04004002 RID: 16386
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x04004003 RID: 16387
	public GRAbilityPatrol abilityPatrol;

	// Token: 0x04004004 RID: 16388
	public GRAbilityFlashed abilityFlashed;

	// Token: 0x04004005 RID: 16389
	public GRAbilityKeepDistance abilityKeepDistance;

	// Token: 0x04004006 RID: 16390
	public GRAbilityJump abilityJump;

	// Token: 0x04004007 RID: 16391
	public List<Renderer> bones;

	// Token: 0x04004008 RID: 16392
	public List<Renderer> always;

	// Token: 0x04004009 RID: 16393
	public Transform coreMarker;

	// Token: 0x0400400A RID: 16394
	public GRCollectible corePrefab;

	// Token: 0x0400400B RID: 16395
	public Transform headTransform;

	// Token: 0x0400400C RID: 16396
	public float sightDist;

	// Token: 0x0400400D RID: 16397
	public float loseSightDist;

	// Token: 0x0400400E RID: 16398
	public float sightFOV;

	// Token: 0x0400400F RID: 16399
	public float sightLostFollowStopTime = 0.5f;

	// Token: 0x04004010 RID: 16400
	public float searchTime = 5f;

	// Token: 0x04004011 RID: 16401
	public float hearingRadius = 5f;

	// Token: 0x04004012 RID: 16402
	public float turnSpeed = 540f;

	// Token: 0x04004013 RID: 16403
	public Color chaseColor = Color.red;

	// Token: 0x04004014 RID: 16404
	public AbilitySound attackAbilitySound;

	// Token: 0x04004015 RID: 16405
	public AbilitySound chaseAbilitySound;

	// Token: 0x04004016 RID: 16406
	public float rangedAttackDistMin = 6f;

	// Token: 0x04004017 RID: 16407
	public float rangedAttackDistMax = 8f;

	// Token: 0x04004018 RID: 16408
	public float rangedAttackChargeTime = 0.5f;

	// Token: 0x04004019 RID: 16409
	public float rangedAttackRecoverTime = 2f;

	// Token: 0x0400401A RID: 16410
	public float projectileSpeed = 5f;

	// Token: 0x0400401B RID: 16411
	public float projectileHitRadius = 1f;

	// Token: 0x0400401C RID: 16412
	public GameObject rangedProjectilePrefab;

	// Token: 0x0400401D RID: 16413
	public Transform rangedProjectileFirePoint;

	// Token: 0x0400401E RID: 16414
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x0400401F RID: 16415
	public NavMeshAgent navAgent;

	// Token: 0x04004020 RID: 16416
	public AudioSource audioSource;

	// Token: 0x04004021 RID: 16417
	public AudioSource audioSecondarySource;

	// Token: 0x04004022 RID: 16418
	public AudioClip damagedSound;

	// Token: 0x04004023 RID: 16419
	public float damagedSoundVolume;

	// Token: 0x04004024 RID: 16420
	public GameObject fxDamaged;

	// Token: 0x04004025 RID: 16421
	public bool lastMoving;

	// Token: 0x04004026 RID: 16422
	private Vector3? investigateLocation;

	// Token: 0x04004027 RID: 16423
	public bool debugLog;

	// Token: 0x04004028 RID: 16424
	public GameObject spitterHeadOnShoulders;

	// Token: 0x04004029 RID: 16425
	public GameObject spitterHeadOnShouldersLight;

	// Token: 0x0400402A RID: 16426
	public GameObject spitterHeadOnShouldersVFX;

	// Token: 0x0400402B RID: 16427
	public GameObject spitterHeadInHand;

	// Token: 0x0400402C RID: 16428
	public GameObject spitterHeadInHandLight;

	// Token: 0x0400402D RID: 16429
	public GameObject spitterHeadInHandVFX;

	// Token: 0x0400402E RID: 16430
	public double spitterLightTurnOffDelay = 0.75;

	// Token: 0x0400402F RID: 16431
	private bool headLightReset;

	// Token: 0x04004030 RID: 16432
	private double spitterLightTurnOffTime;

	// Token: 0x04004031 RID: 16433
	[FormerlySerializedAs("headRemovalInterval")]
	public float headRemovalFrame = 0.23333333f;

	// Token: 0x04004032 RID: 16434
	private double headRemovaltime;

	// Token: 0x04004033 RID: 16435
	private bool headRemoved;

	// Token: 0x04004034 RID: 16436
	private Transform target;

	// Token: 0x04004035 RID: 16437
	[ReadOnly]
	public int hp;

	// Token: 0x04004036 RID: 16438
	[ReadOnly]
	public GREnemyRanged.Behavior currBehavior;

	// Token: 0x04004037 RID: 16439
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x04004038 RID: 16440
	[ReadOnly]
	public GREnemyRanged.BodyState currBodyState;

	// Token: 0x04004039 RID: 16441
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x0400403A RID: 16442
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x0400403B RID: 16443
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x0400403C RID: 16444
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x0400403D RID: 16445
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x0400403E RID: 16446
	[ReadOnly]
	public Vector3 rangedFiringPosition;

	// Token: 0x0400403F RID: 16447
	[ReadOnly]
	public Vector3 rangedTargetPosition;

	// Token: 0x04004040 RID: 16448
	[ReadOnly]
	private GRPlayer bestTargetPlayer;

	// Token: 0x04004041 RID: 16449
	[ReadOnly]
	private NetPlayer bestTargetNetPlayer;

	// Token: 0x04004042 RID: 16450
	private bool rangedAttackQueued;

	// Token: 0x04004043 RID: 16451
	private double queuedFiringTime;

	// Token: 0x04004044 RID: 16452
	private Vector3 queuedFiringPosition;

	// Token: 0x04004045 RID: 16453
	private Vector3 queuedTargetPosition;

	// Token: 0x04004046 RID: 16454
	private GameObject rangedProjectileInstance;

	// Token: 0x04004047 RID: 16455
	private bool projectileHasImpacted;

	// Token: 0x04004048 RID: 16456
	private double projectileImpactTime;

	// Token: 0x04004049 RID: 16457
	private Rigidbody rigidBody;

	// Token: 0x0400404A RID: 16458
	private List<Collider> colliders;

	// Token: 0x0400404B RID: 16459
	private LayerMask visibilityLayerMask;

	// Token: 0x0400404C RID: 16460
	private Color defaultColor;

	// Token: 0x0400404D RID: 16461
	private float lastHitPlayerTime;

	// Token: 0x0400404E RID: 16462
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x0400404F RID: 16463
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x02000794 RID: 1940
	public enum Behavior
	{
		// Token: 0x04004051 RID: 16465
		Idle,
		// Token: 0x04004052 RID: 16466
		Patrol,
		// Token: 0x04004053 RID: 16467
		Search,
		// Token: 0x04004054 RID: 16468
		Stagger,
		// Token: 0x04004055 RID: 16469
		Dying,
		// Token: 0x04004056 RID: 16470
		SeekRangedAttackPosition,
		// Token: 0x04004057 RID: 16471
		RangedAttack,
		// Token: 0x04004058 RID: 16472
		RangedAttackCooldown,
		// Token: 0x04004059 RID: 16473
		Flashed,
		// Token: 0x0400405A RID: 16474
		Investigate,
		// Token: 0x0400405B RID: 16475
		Jump,
		// Token: 0x0400405C RID: 16476
		Count
	}

	// Token: 0x02000795 RID: 1941
	public enum BodyState
	{
		// Token: 0x0400405E RID: 16478
		Destroyed,
		// Token: 0x0400405F RID: 16479
		Bones,
		// Token: 0x04004060 RID: 16480
		Shell,
		// Token: 0x04004061 RID: 16481
		Count
	}
}
