using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200006E RID: 110
public class CrittersPawn : CrittersActor, IEyeScannable
{
	// Token: 0x06000276 RID: 630 RVA: 0x0000E8C0 File Offset: 0x0000CAC0
	public override void Initialize()
	{
		base.Initialize();
		this.rB = base.GetComponentInChildren<Rigidbody>();
		this.soundsHeard = new Dictionary<int, CrittersActor>();
		base.transform.eulerAngles = new Vector3(0f, Random.value * 360f, 0f);
		this.raycastHits = new RaycastHit[20];
		this.wasSomethingInTheWay = false;
		this._spawnAnimationDuration = this.spawnInHeighMovement.keys.Last<Keyframe>().time;
		this._despawnAnimationDuration = this.despawnInHeighMovement.keys.Last<Keyframe>().time;
	}

	// Token: 0x06000277 RID: 631 RVA: 0x0000E960 File Offset: 0x0000CB60
	private void InitializeTemplateValues()
	{
		this.sensoryRange *= this.sensoryRange;
		this.autoSeeFoodDistance *= this.autoSeeFoodDistance;
		this.currentSleepiness = Random.value * this.tiredThreshold;
		this.currentHunger = Random.value * this.hungryThreshold;
		this.currentFear = 0f;
		this.currentStruggle = 0f;
		this.currentAttraction = 0f;
	}

	// Token: 0x06000278 RID: 632 RVA: 0x0000E9D8 File Offset: 0x0000CBD8
	public float JumpVelocityForDistanceAtAngle(float horizontalDistance, float angle)
	{
		return Mathf.Min(this.maxJumpVel, Mathf.Sqrt(horizontalDistance * Physics.gravity.magnitude / Mathf.Sin(2f * angle)));
	}

	// Token: 0x06000279 RID: 633 RVA: 0x0000EA11 File Offset: 0x0000CC11
	public override void OnEnable()
	{
		base.OnEnable();
		CrittersManager.RegisterCritter(this);
		this.lifeTimeStart = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		EyeScannerMono.Register(this);
	}

	// Token: 0x0600027A RID: 634 RVA: 0x0000EA3F File Offset: 0x0000CC3F
	public override void OnDisable()
	{
		base.OnDisable();
		CrittersManager.DeregisterCritter(this);
		if (this.currentOngoingStateFX.IsNotNull())
		{
			this.currentOngoingStateFX.SetActive(false);
			this.currentOngoingStateFX = null;
		}
		EyeScannerMono.Unregister(this);
	}

	// Token: 0x0600027B RID: 635 RVA: 0x0000EA73 File Offset: 0x0000CC73
	private float GetAdditiveJumpDelay()
	{
		if (this.currentState == CrittersPawn.CreatureState.Running)
		{
			return 0f;
		}
		return Mathf.Max(0f, this.jumpCooldown * Random.value * this.jumpVariabilityTime);
	}

	// Token: 0x0600027C RID: 636 RVA: 0x0000EAA4 File Offset: 0x0000CCA4
	public void LocalJump(float maxVel, float jumpAngle)
	{
		maxVel *= this.slowSpeedMod;
		this.lastImpulsePosition = base.transform.position;
		this.lastImpulseVelocity = base.transform.forward * (Mathf.Sin(0.017453292f * jumpAngle) * maxVel) + Vector3.up * (Mathf.Cos(0.017453292f * jumpAngle) * maxVel);
		this.lastImpulseTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		this.lastImpulseTime += (double)this.GetAdditiveJumpDelay();
		this.lastImpulseQuaternion = base.transform.rotation;
		this.rB.linearVelocity = this.lastImpulseVelocity;
		this.rb.angularVelocity = Vector3.zero;
	}

	// Token: 0x0600027D RID: 637 RVA: 0x0000EB70 File Offset: 0x0000CD70
	private bool CanSeeActor(Vector3 actorPosition)
	{
		Vector3 to = actorPosition - base.transform.position;
		return to.sqrMagnitude < this.autoSeeFoodDistance || (to.sqrMagnitude < this.sensoryRange && Vector3.Angle(base.transform.forward, to) < this.visionConeAngle);
	}

	// Token: 0x0600027E RID: 638 RVA: 0x0000EBCC File Offset: 0x0000CDCC
	private bool IsGrabPossible(CrittersGrabber actor)
	{
		return actor.grabbing && (base.transform.position - actor.grabPosition.position).magnitude < actor.grabDistance;
	}

	// Token: 0x0600027F RID: 639 RVA: 0x0000EC10 File Offset: 0x0000CE10
	private bool WithinCaptureDistance(CrittersCage actor)
	{
		return (this.bodyCollider.bounds.center - actor.grabPosition.position).magnitude < actor.grabDistance;
	}

	// Token: 0x06000280 RID: 640 RVA: 0x0000EC50 File Offset: 0x0000CE50
	public bool AwareOfActor(CrittersActor actor)
	{
		CrittersActor.CrittersActorType crittersActorType = actor.crittersActorType;
		switch (crittersActorType)
		{
		case CrittersActor.CrittersActorType.Creature:
			return this.CanSeeActor(actor.transform.position);
		case CrittersActor.CrittersActorType.Food:
			return ((CrittersFood)actor).currentFood > 0f && this.CanSeeActor(((CrittersFood)actor).food.transform.position);
		case CrittersActor.CrittersActorType.LoudNoise:
			return (actor.transform.position - base.transform.position).sqrMagnitude < this.sensoryRange;
		case CrittersActor.CrittersActorType.BrightLight:
			return this.CanSeeActor(actor.transform.position);
		case CrittersActor.CrittersActorType.Darkness:
		case CrittersActor.CrittersActorType.HidingArea:
		case CrittersActor.CrittersActorType.Disappear:
		case CrittersActor.CrittersActorType.Spawn:
		case CrittersActor.CrittersActorType.Player:
		case CrittersActor.CrittersActorType.AttachPoint:
			break;
		case CrittersActor.CrittersActorType.Grabber:
			return this.CanSeeActor(actor.transform.position);
		case CrittersActor.CrittersActorType.Cage:
			return this.CanSeeActor(actor.transform.position);
		case CrittersActor.CrittersActorType.FoodSpawner:
			return this.CanSeeActor(actor.transform.position);
		case CrittersActor.CrittersActorType.StunBomb:
			return this.CanSeeActor(actor.transform.position);
		default:
			if (crittersActorType == CrittersActor.CrittersActorType.StickyGoo)
			{
				return ((CrittersStickyGoo)actor).CanAffect(base.transform.position);
			}
			break;
		}
		return false;
	}

	// Token: 0x06000281 RID: 641 RVA: 0x0000ED8C File Offset: 0x0000CF8C
	public override bool ProcessLocal()
	{
		CrittersPawn.CreatureUpdateData creatureUpdateData = new CrittersPawn.CreatureUpdateData(this);
		bool flag = base.ProcessLocal();
		if (!this.isEnabled)
		{
			return flag;
		}
		this.wasSomethingInTheWay = false;
		this.UpdateMoodSourceData();
		this.StuckCheck();
		switch (this.currentState)
		{
		case CrittersPawn.CreatureState.Idle:
			this.IdleStateUpdate();
			this.DespawnCheck();
			break;
		case CrittersPawn.CreatureState.Eating:
			this.EatingStateUpdate();
			this.DespawnCheck();
			break;
		case CrittersPawn.CreatureState.AttractedTo:
			this.AttractedStateUpdate();
			this.DespawnCheck();
			break;
		case CrittersPawn.CreatureState.Running:
			this.RunningStateUpdate();
			this.DespawnCheck();
			break;
		case CrittersPawn.CreatureState.Grabbed:
			this.GrabbedStateUpdate();
			break;
		case CrittersPawn.CreatureState.Sleeping:
			this.SleepingStateUpdate();
			this.DespawnCheck();
			break;
		case CrittersPawn.CreatureState.SeekingFood:
			this.SeekingFoodStateUpdate();
			this.DespawnCheck();
			break;
		case CrittersPawn.CreatureState.Captured:
			this.CapturedStateUpdate();
			break;
		case CrittersPawn.CreatureState.Stunned:
			this.StunnedStateUpdate();
			break;
		case CrittersPawn.CreatureState.WaitingToDespawn:
			this.WaitingToDespawnStateUpdate();
			break;
		case CrittersPawn.CreatureState.Despawning:
			this.DespawningStateUpdate();
			break;
		case CrittersPawn.CreatureState.Spawning:
			this.SpawningStateUpdate();
			break;
		}
		this.UpdateStateAnim();
		this.updatedSinceLastFrame = (flag || this.updatedSinceLastFrame || !creatureUpdateData.SameData(this));
		return this.updatedSinceLastFrame;
	}

	// Token: 0x06000282 RID: 642 RVA: 0x0000EEB4 File Offset: 0x0000D0B4
	private void StuckCheck()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (this._nextStuckCheck > (double)realtimeSinceStartup)
		{
			return;
		}
		this._nextStuckCheck = (double)(realtimeSinceStartup + 1f);
		if (!this.canJump && this.rb.IsSleeping())
		{
			this.canJump = true;
		}
		if (base.transform.position.y < this.killHeight)
		{
			this.SetState(CrittersPawn.CreatureState.Despawning);
		}
	}

	// Token: 0x06000283 RID: 643 RVA: 0x0000EF1C File Offset: 0x0000D11C
	private void DespawnCheck()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (this._nextDespawnCheck > (double)realtimeSinceStartup)
		{
			return;
		}
		this._nextDespawnCheck = (double)(realtimeSinceStartup + 1f);
		bool flag;
		if (this.lifeTime <= 0.0)
		{
			flag = (this.creatureConfiguration != null && !this.creatureConfiguration.ShouldDespawn());
		}
		else
		{
			flag = ((PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) - this.lifeTimeStart > this.lifeTime);
		}
		if (flag)
		{
			this.SetState(CrittersPawn.CreatureState.WaitingToDespawn);
			this.spawningStartingPosition = base.gameObject.transform.position;
			this.despawnStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		}
	}

	// Token: 0x06000284 RID: 644 RVA: 0x0000EFD6 File Offset: 0x0000D1D6
	public void SetTemplate(int templateIndex)
	{
		this.TemplateIndex = templateIndex;
		this.UpdateTemplate();
	}

	// Token: 0x06000285 RID: 645 RVA: 0x0000EFE8 File Offset: 0x0000D1E8
	private void UpdateTemplate()
	{
		if (this.TemplateIndex != this.LastTemplateIndex)
		{
			this.creatureConfiguration = CrittersManager.instance.creatureIndex[this.TemplateIndex];
			if (this.creatureConfiguration != null)
			{
				this.creatureConfiguration.ApplyToCreature(this);
				this.InitializeAttractors();
			}
			this.LastTemplateIndex = this.TemplateIndex;
			this.InitializeTemplateValues();
		}
		if (this.OnDataChange != null)
		{
			this.OnDataChange();
		}
	}

	// Token: 0x06000286 RID: 646 RVA: 0x0000F060 File Offset: 0x0000D260
	private void InitializeAttractors()
	{
		this.attractedToTypes = new Dictionary<CrittersActor.CrittersActorType, float>();
		this.afraidOfTypes = new Dictionary<CrittersActor.CrittersActorType, float>();
		if (this.attractedToList != null)
		{
			for (int i = 0; i < this.attractedToList.Count; i++)
			{
				this.attractedToTypes.Add(this.attractedToList[i].type, this.attractedToList[i].multiplier);
			}
		}
		if (this.afraidOfList != null)
		{
			for (int j = 0; j < this.afraidOfList.Count; j++)
			{
				this.afraidOfTypes.Add(this.afraidOfList[j].type, this.afraidOfList[j].multiplier);
			}
		}
	}

	// Token: 0x06000287 RID: 647 RVA: 0x0000F119 File Offset: 0x0000D319
	public override void ProcessRemote()
	{
		this.UpdateTemplate();
		base.ProcessRemote();
		this.UpdateStateAnim();
	}

	// Token: 0x06000288 RID: 648 RVA: 0x0000F130 File Offset: 0x0000D330
	public void SetState(CrittersPawn.CreatureState newState)
	{
		if (this.currentState == newState)
		{
			return;
		}
		if (this.currentState == CrittersPawn.CreatureState.Captured)
		{
			base.transform.localScale = Vector3.one;
		}
		this.ClearOngoingStateFX();
		this.currentState = newState;
		if (newState != CrittersPawn.CreatureState.Despawning)
		{
			if (newState == CrittersPawn.CreatureState.Spawning && CrittersManager.instance.LocalAuthority())
			{
				this.spawningStartingPosition = base.gameObject.transform.position;
				this.spawnStartTime = (double)(PhotonNetwork.InRoom ? ((float)PhotonNetwork.Time) : Time.time);
			}
		}
		else if (CrittersManager.instance.LocalAuthority())
		{
			this.spawningStartingPosition = base.gameObject.transform.position;
			this.despawnStartTime = (double)(PhotonNetwork.InRoom ? ((float)PhotonNetwork.Time) : Time.time);
		}
		this.StartOngoingStateFX(newState);
		GameObject valueOrDefault = this.StartStateFX.GetValueOrDefault(this.currentState);
		if (valueOrDefault.IsNotNull())
		{
			GameObject pooled = CrittersPool.GetPooled(valueOrDefault);
			if (pooled != null)
			{
				pooled.transform.position = base.transform.position;
			}
		}
		this.currentAnimTime = 0f;
		CrittersAnim crittersAnim;
		if (this.stateAnim.TryGetValue(this.currentState, out crittersAnim))
		{
			this.currentAnim = crittersAnim;
		}
		else
		{
			this.currentAnim = null;
			this.animTarget.localPosition = Vector3.zero;
			this.animTarget.localScale = Vector3.one;
		}
		if (this.OnDataChange != null)
		{
			this.OnDataChange();
		}
	}

	// Token: 0x06000289 RID: 649 RVA: 0x0000F2A4 File Offset: 0x0000D4A4
	private void ClearOngoingStateFX()
	{
		if (this.currentOngoingStateFX.IsNotNull())
		{
			CrittersPool.Return(this.currentOngoingStateFX);
			this.currentOngoingStateFX = null;
		}
	}

	// Token: 0x0600028A RID: 650 RVA: 0x0000F2C8 File Offset: 0x0000D4C8
	private void StartOngoingStateFX(CrittersPawn.CreatureState state)
	{
		GameObject valueOrDefault = this.OngoingStateFX.GetValueOrDefault(state);
		if (valueOrDefault.IsNotNull())
		{
			this.currentOngoingStateFX = CrittersPool.GetPooled(valueOrDefault);
			if (this.currentOngoingStateFX.IsNotNull())
			{
				this.currentOngoingStateFX.transform.SetParent(base.transform, false);
				this.currentOngoingStateFX.transform.localPosition = Vector3.zero;
			}
		}
	}

	// Token: 0x0600028B RID: 651 RVA: 0x0000F330 File Offset: 0x0000D530
	[Conditional("UNITY_EDITOR")]
	public void UpdateStateColor()
	{
		switch (this.currentState)
		{
		case CrittersPawn.CreatureState.Idle:
			this.debugStateIndicator.material.color = this.debugColorIdle;
			return;
		case CrittersPawn.CreatureState.Eating:
			this.debugStateIndicator.material.color = this.debugColorEating;
			return;
		case CrittersPawn.CreatureState.AttractedTo:
			this.debugStateIndicator.material.color = this.debugColorAttracted;
			return;
		case CrittersPawn.CreatureState.Running:
			this.debugStateIndicator.material.color = this.debugColorScared;
			return;
		case CrittersPawn.CreatureState.Grabbed:
			this.debugStateIndicator.material.color = this.debugColorCaught;
			return;
		case CrittersPawn.CreatureState.Sleeping:
			this.debugStateIndicator.material.color = this.debugColorSleeping;
			return;
		case CrittersPawn.CreatureState.SeekingFood:
			this.debugStateIndicator.material.color = this.debugColorSeekingFood;
			return;
		case CrittersPawn.CreatureState.Captured:
			this.debugStateIndicator.material.color = this.debugColorCaged;
			return;
		case CrittersPawn.CreatureState.Stunned:
			this.debugStateIndicator.material.color = this.debugColorStunned;
			return;
		default:
			this.debugStateIndicator.material.color = new Color(1f, 0f, 1f);
			return;
		}
	}

	// Token: 0x0600028C RID: 652 RVA: 0x0000F468 File Offset: 0x0000D668
	public void UpdateStateAnim()
	{
		if (this.currentAnim != null)
		{
			this.currentAnimTime += Time.deltaTime * this.currentAnim.playSpeed;
			this.currentAnimTime %= 1f;
			float num = this.currentAnim.squashAmount.Evaluate(this.currentAnimTime);
			float z = this.currentAnim.forwardOffset.Evaluate(this.currentAnimTime);
			float x = this.currentAnim.horizontalOffset.Evaluate(this.currentAnimTime);
			float y = this.currentAnim.verticalOffset.Evaluate(this.currentAnimTime);
			this.animTarget.localPosition = new Vector3(x, y, z);
			float num2 = 1f - num;
			num2 *= 0.5f;
			num2 += 1f;
			this.animTarget.localScale = new Vector3(num2, num, num2);
		}
	}

	// Token: 0x0600028D RID: 653 RVA: 0x0000F554 File Offset: 0x0000D754
	public void IdleStateUpdate()
	{
		if (this.AboveFearThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Running);
			return;
		}
		if (this.AboveAttractedThreshold() && (!this.AboveHungryThreshold() || !CrittersManager.AnyFoodNearby(this)))
		{
			this.SetState(CrittersPawn.CreatureState.AttractedTo);
			return;
		}
		if (this.AboveHungryThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.SeekingFood);
			return;
		}
		if (this.AboveSleepyThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Sleeping);
			return;
		}
		if (this.CanJump())
		{
			this.RandomJump();
		}
	}

	// Token: 0x0600028E RID: 654 RVA: 0x0000F5C0 File Offset: 0x0000D7C0
	public void EatingStateUpdate()
	{
		if (this.AboveFearThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Running);
			return;
		}
		if (this.BelowNotHungryThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Idle);
			return;
		}
		if (!this.withinEatingRadius || this.eatingTarget.IsNull() || this.eatingTarget.currentFood <= 0f)
		{
			this.SetState(CrittersPawn.CreatureState.SeekingFood);
		}
	}

	// Token: 0x0600028F RID: 655 RVA: 0x0000F61B File Offset: 0x0000D81B
	public void SleepingStateUpdate()
	{
		if (this.AboveFearThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Running);
			return;
		}
		if (this.BelowNotSleepyThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Idle);
		}
	}

	// Token: 0x06000290 RID: 656 RVA: 0x0000F63C File Offset: 0x0000D83C
	public void AttractedStateUpdate()
	{
		if (this.AboveFearThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Running);
			return;
		}
		if (this.BelowUnAttractedThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Idle);
			return;
		}
		if (this.CanJump())
		{
			if (this.AboveHungryThreshold() && CrittersManager.AnyFoodNearby(this))
			{
				this.SetState(CrittersPawn.CreatureState.SeekingFood);
				return;
			}
			if (CrittersManager.instance.awareOfActors[this].Contains(this.attractionTarget))
			{
				this.lastSeenAttractionPosition = this.attractionTarget.transform.position;
			}
			this.JumpTowards(this.lastSeenAttractionPosition);
		}
	}

	// Token: 0x06000291 RID: 657 RVA: 0x0000F6CC File Offset: 0x0000D8CC
	public void RunningStateUpdate()
	{
		if (this.CanJump())
		{
			if (CrittersManager.instance.awareOfActors[this].Contains(this.fearTarget))
			{
				this.lastSeenFearPosition = this.fearTarget.transform.position;
			}
			this.JumpAwayFrom(this.lastSeenFearPosition);
		}
		if (this.BelowNotAfraidThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Idle);
		}
	}

	// Token: 0x06000292 RID: 658 RVA: 0x0000F734 File Offset: 0x0000D934
	public void SeekingFoodStateUpdate()
	{
		if (this.AboveFearThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Running);
			return;
		}
		if (this.CanJump())
		{
			if (CrittersManager.CritterAwareOfAny(this))
			{
				this.eatingTarget = CrittersManager.ClosestFood(this);
				if (this.eatingTarget != null)
				{
					this.withinEatingRadius = ((this.eatingTarget.food.transform.position - base.transform.position).sqrMagnitude < this.eatingRadiusMaxSquared);
					if (!this.withinEatingRadius)
					{
						this.JumpTowards(this.eatingTarget.food.transform.position);
						return;
					}
					base.transform.forward = (this.eatingTarget.food.transform.position - base.transform.position).X_Z().normalized;
					this.SetState(CrittersPawn.CreatureState.Eating);
					this.debugStateIndicator.material.color = this.debugColorEating;
					return;
				}
				else
				{
					if (this.AboveAttractedThreshold())
					{
						this.SetState(CrittersPawn.CreatureState.AttractedTo);
						return;
					}
					this.RandomJump();
					return;
				}
			}
			else
			{
				this.RandomJump();
			}
		}
	}

	// Token: 0x06000293 RID: 659 RVA: 0x0000F85C File Offset: 0x0000DA5C
	public void GrabbedStateUpdate()
	{
		if (this.currentState == CrittersPawn.CreatureState.Grabbed && this.grabbedTarget != null)
		{
			if (this.currentStruggle >= this.escapeThreshold || !this.grabbedTarget.grabbing)
			{
				this.Released(true, default(Quaternion), default(Vector3), default(Vector3), default(Vector3));
				return;
			}
		}
		else if (this.grabbedTarget == null)
		{
			this.Released(true, default(Quaternion), default(Vector3), default(Vector3), default(Vector3));
		}
	}

	// Token: 0x06000294 RID: 660 RVA: 0x0000F900 File Offset: 0x0000DB00
	protected override void HandleRemoteReleased()
	{
		base.HandleRemoteReleased();
		if (this.cageTarget.IsNotNull())
		{
			this.fearTarget = this.cageTarget;
			this.cageTarget.SetHasCritter(false);
			this.cageTarget = null;
		}
		if (this.grabbedTarget.IsNotNull())
		{
			this.fearTarget = this.grabbedTarget;
			this.grabbedTarget = null;
			if (this.OnReleasedFX)
			{
				CrittersPool.GetPooled(this.OnReleasedFX).transform.position = base.transform.position;
			}
		}
	}

	// Token: 0x06000295 RID: 661 RVA: 0x0000F98C File Offset: 0x0000DB8C
	public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulse = default(Vector3), Vector3 impulseRotation = default(Vector3))
	{
		base.Released(keepWorldPosition, rotation, position, impulse, impulseRotation);
		if (this.currentState != CrittersPawn.CreatureState.Grabbed && this.currentState != CrittersPawn.CreatureState.Captured)
		{
			return;
		}
		if (this.grabbedTarget.IsNotNull() && this.grabbedTarget.grabbedActors.Contains(this))
		{
			this.grabbedTarget.grabbedActors.Remove(this);
		}
		if (this.currentState == CrittersPawn.CreatureState.Grabbed)
		{
			this.fearTarget = this.grabbedTarget;
			this.grabbedTarget = null;
			if (this.OnReleasedFX)
			{
				CrittersPool.GetPooled(this.OnReleasedFX).transform.position = base.transform.position;
			}
		}
		else if (this.currentState == CrittersPawn.CreatureState.Captured)
		{
			base.transform.localScale = Vector3.one;
			this.fearTarget = this.cageTarget;
			this.cageTarget.SetHasCritter(false);
			this.cageTarget = null;
		}
		if (this.struggleGainedPerSecond > 0f)
		{
			this.currentFear = this.maxFear;
			this.SetState(CrittersPawn.CreatureState.Running);
			this.lastSeenFearPosition = this.fearTarget.transform.position;
			return;
		}
		this.currentFear = 0f;
		this.SetState(CrittersPawn.CreatureState.Idle);
	}

	// Token: 0x06000296 RID: 662 RVA: 0x0000FAB8 File Offset: 0x0000DCB8
	public void CapturedStateUpdate()
	{
		if (this.cageTarget.IsNull())
		{
			this.cageTarget = (CrittersCage)CrittersManager.instance.actorById[this.actorIdTarget];
			this.cageTarget.SetHasCritter(false);
		}
		if (this.cageTarget.inReleasingPosition && this.cageTarget.heldByPlayer)
		{
			this.Released(true, default(Quaternion), default(Vector3), default(Vector3), default(Vector3));
		}
	}

	// Token: 0x06000297 RID: 663 RVA: 0x0000FB45 File Offset: 0x0000DD45
	public void StunnedStateUpdate()
	{
		this.remainingStunnedTime = Mathf.Max(0f, this.remainingStunnedTime - Time.deltaTime);
		if (this.remainingStunnedTime <= 0f)
		{
			this.currentFear = this.maxFear;
			this.SetState(CrittersPawn.CreatureState.Running);
		}
	}

	// Token: 0x06000298 RID: 664 RVA: 0x0000FB84 File Offset: 0x0000DD84
	public void WaitingToDespawnStateUpdate()
	{
		if (Mathf.FloorToInt(this.rb.linearVelocity.magnitude * 10f) == 0)
		{
			this.SetState(CrittersPawn.CreatureState.Despawning);
		}
	}

	// Token: 0x06000299 RID: 665 RVA: 0x0000FBBC File Offset: 0x0000DDBC
	public void DespawningStateUpdate()
	{
		this._despawnAnimTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) - this.despawnStartTime;
		if (this._despawnAnimTime >= (double)this._despawnAnimationDuration)
		{
			base.gameObject.SetActive(false);
			this.TemplateIndex = -1;
		}
	}

	// Token: 0x0600029A RID: 666 RVA: 0x0000FC0C File Offset: 0x0000DE0C
	public void SpawningStateUpdate()
	{
		this._spawnAnimTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) - this.spawnStartTime;
		base.MoveActor(this.spawningStartingPosition + new Vector3(0f, this.spawnInHeighMovement.Evaluate(Mathf.Clamp((float)this._spawnAnimTime, 0f, this._spawnAnimationDuration)), 0f), base.transform.rotation, false, true, true);
		if (this._spawnAnimTime >= (double)this._spawnAnimationDuration)
		{
			this.SetState(CrittersPawn.CreatureState.Idle);
		}
	}

	// Token: 0x0600029B RID: 667 RVA: 0x0000FCA0 File Offset: 0x0000DEA0
	public void UpdateMoodSourceData()
	{
		this.UpdateHunger();
		this.UpdateFearAndAttraction();
		this.UpdateSleepiness();
		this.UpdateStruggle();
		this.UpdateSlowed();
		this.UpdateGrabbed();
		this.UpdateCaged();
	}

	// Token: 0x0600029C RID: 668 RVA: 0x0000FCCC File Offset: 0x0000DECC
	public void UpdateHunger()
	{
		if (this.currentState == CrittersPawn.CreatureState.Eating && !this.eatingTarget.IsNull())
		{
			this.eatingTarget.Feed(this.hungerLostPerSecond * Time.deltaTime);
			this.currentHunger = Mathf.Max(0f, this.currentHunger - this.hungerLostPerSecond * Time.deltaTime);
			return;
		}
		this.currentHunger = Mathf.Min(this.maxHunger, this.currentHunger + this.hungerGainedPerSecond * Time.deltaTime);
	}

	// Token: 0x0600029D RID: 669 RVA: 0x0000FD50 File Offset: 0x0000DF50
	public void UpdateFearAndAttraction()
	{
		if (this.currentState == CrittersPawn.CreatureState.Spawning)
		{
			return;
		}
		this.currentFear = Mathf.Max(0f, this.currentFear - this.fearLostPerSecond * Time.deltaTime);
		this.currentAttraction = Mathf.Max(0f, this.currentAttraction - this.attractionLostPerSecond * Time.deltaTime);
		for (int i = 0; i < CrittersManager.instance.awareOfActors[this].Count; i++)
		{
			CrittersActor crittersActor = CrittersManager.instance.awareOfActors[this][i];
			float multiplier;
			float multiplier2;
			if (this.afraidOfTypes != null && this.afraidOfTypes.TryGetValue(crittersActor.crittersActorType, out multiplier))
			{
				crittersActor.CalculateFear(this, multiplier);
			}
			else if (this.attractedToTypes != null && this.attractedToTypes.TryGetValue(crittersActor.crittersActorType, out multiplier2))
			{
				crittersActor.CalculateAttraction(this, multiplier2);
			}
		}
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0000FE38 File Offset: 0x0000E038
	public void IncreaseFear(float fearAmount, CrittersActor actor)
	{
		if (fearAmount > 0f)
		{
			this.currentFear += fearAmount;
			this.currentFear = Mathf.Min(this.maxFear, this.currentFear);
			this.fearTarget = actor;
			this.lastSeenFearPosition = this.fearTarget.transform.position;
		}
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0000FE90 File Offset: 0x0000E090
	public void IncreaseAttraction(float attractionAmount, CrittersActor actor)
	{
		if (attractionAmount > 0f)
		{
			this.currentAttraction += attractionAmount;
			this.currentAttraction = Mathf.Min(this.maxAttraction, this.currentAttraction);
			this.attractionTarget = actor;
			this.lastSeenAttractionPosition = this.attractionTarget.transform.position;
		}
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0000FEE8 File Offset: 0x0000E0E8
	public void UpdateSleepiness()
	{
		if (this.currentState == CrittersPawn.CreatureState.Sleeping)
		{
			this.currentSleepiness = Mathf.Max(0f, this.currentSleepiness - Time.deltaTime * this.sleepinessLostPerSecond);
			return;
		}
		this.currentSleepiness = Mathf.Min(this.maxSleepiness, this.currentSleepiness + Time.deltaTime * this.sleepinessGainedPerSecond);
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x0000FF48 File Offset: 0x0000E148
	public void UpdateStruggle()
	{
		if (this.currentState == CrittersPawn.CreatureState.Grabbed)
		{
			this.currentStruggle = Mathf.Clamp(this.currentStruggle + this.struggleGainedPerSecond * Time.deltaTime, 0f, this.maxStruggle);
			return;
		}
		this.currentStruggle = Mathf.Max(0f, this.currentStruggle - this.struggleLostPerSecond * Time.deltaTime);
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x0000FFAC File Offset: 0x0000E1AC
	private void UpdateSlowed()
	{
		if (this.remainingSlowedTime > 0f)
		{
			this.remainingSlowedTime -= Time.deltaTime;
			if (this.remainingSlowedTime < 0f)
			{
				this.slowSpeedMod = 1f;
				return;
			}
		}
		else if (this.currentState != CrittersPawn.CreatureState.Captured && this.currentState != CrittersPawn.CreatureState.Despawning && this.currentState != CrittersPawn.CreatureState.Grabbed && this.currentState != CrittersPawn.CreatureState.WaitingToDespawn && this.currentState != CrittersPawn.CreatureState.Spawning)
		{
			for (int i = 0; i < CrittersManager.instance.awareOfActors[this].Count; i++)
			{
				CrittersActor crittersActor = CrittersManager.instance.awareOfActors[this][i];
				if (crittersActor.crittersActorType == CrittersActor.CrittersActorType.StickyGoo)
				{
					CrittersStickyGoo crittersStickyGoo = crittersActor as CrittersStickyGoo;
					this.slowSpeedMod = crittersStickyGoo.slowModifier;
					this.remainingSlowedTime = crittersStickyGoo.slowDuration;
					crittersStickyGoo.EffectApplied(this);
				}
			}
		}
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x00010098 File Offset: 0x0000E298
	public void UpdateGrabbed()
	{
		if (this.currentState == CrittersPawn.CreatureState.Grabbed || this.currentState == CrittersPawn.CreatureState.Captured)
		{
			return;
		}
		for (int i = 0; i < CrittersManager.instance.awareOfActors[this].Count; i++)
		{
			CrittersActor crittersActor = CrittersManager.instance.awareOfActors[this][i];
			if (crittersActor.crittersActorType == CrittersActor.CrittersActorType.Grabber && !crittersActor.isOnPlayer && this.IsGrabPossible((CrittersGrabber)crittersActor))
			{
				this.GrabbedBy(crittersActor, true, default(Quaternion), default(Vector3), false);
			}
		}
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x00010130 File Offset: 0x0000E330
	public void UpdateCaged()
	{
		if (this.currentState == CrittersPawn.CreatureState.Captured)
		{
			return;
		}
		for (int i = 0; i < CrittersManager.instance.awareOfActors[this].Count; i++)
		{
			CrittersActor crittersActor = CrittersManager.instance.awareOfActors[this][i];
			CrittersCage crittersCage = crittersActor as CrittersCage;
			if (crittersActor.crittersActorType == CrittersActor.CrittersActorType.Cage && crittersCage.IsNotNull() && crittersCage.CanCatch && this.WithinCaptureDistance(crittersCage))
			{
				this.GrabbedBy(crittersActor, true, crittersCage.cagePosition.localRotation, crittersCage.cagePosition.localPosition, false);
			}
		}
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x000101CC File Offset: 0x0000E3CC
	public void RandomJump()
	{
		for (int i = 0; i < 5; i++)
		{
			base.transform.eulerAngles = new Vector3(0f, 360f * Random.value, 0f);
			if (!this.SomethingInTheWay(default(Vector3)))
			{
				break;
			}
		}
		this.LocalJump(this.maxJumpVel, 45f);
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x0001022C File Offset: 0x0000E42C
	public void JumpTowards(Vector3 targetPos)
	{
		if (this.SomethingInTheWay((targetPos - base.transform.position).X_Z()))
		{
			this.RandomJump();
			return;
		}
		base.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(targetPos - base.transform.position, Vector3.up), Vector3.up);
		this.LocalJump(this.JumpVelocityForDistanceAtAngle(Vector3.ProjectOnPlane(targetPos - base.transform.position, Vector3.up).magnitude * this.fudge, 45f), 45f);
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x000102D0 File Offset: 0x0000E4D0
	public void JumpAwayFrom(Vector3 targetPos)
	{
		Vector3 vector = (base.transform.position - targetPos).X_Z();
		if (vector == Vector3.zero)
		{
			vector = base.transform.forward;
		}
		Vector3 vector2 = Quaternion.Euler(0f, (float)Random.Range(-30, 30), 0f) * vector;
		if (this.SomethingInTheWay(vector2))
		{
			this.RandomJump();
			return;
		}
		base.transform.rotation = Quaternion.LookRotation(vector2, Vector3.up);
		this.LocalJump(this.maxJumpVel, 45f);
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x00010364 File Offset: 0x0000E564
	public bool SomethingInTheWay(Vector3 direction = default(Vector3))
	{
		if (direction == default(Vector3))
		{
			direction = base.transform.forward;
		}
		bool flag = Physics.RaycastNonAlloc(this.bodyCollider.bounds.center, direction, this.raycastHits, this.obstacleSeeDistance, CrittersManager.instance.movementLayers) > 0;
		this.wasSomethingInTheWay = (this.wasSomethingInTheWay || flag);
		return flag;
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x000103D8 File Offset: 0x0000E5D8
	public override bool CanBeGrabbed(CrittersActor grabbedBy)
	{
		return this.currentState != CrittersPawn.CreatureState.Captured && base.CanBeGrabbed(grabbedBy);
	}

	// Token: 0x060002AA RID: 682 RVA: 0x000103EC File Offset: 0x0000E5EC
	public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		CrittersActor.CrittersActorType crittersActorType = grabbingActor.crittersActorType;
		if (crittersActorType == CrittersActor.CrittersActorType.Grabber)
		{
			this.SetState(CrittersPawn.CreatureState.Grabbed);
			this.grabbedTarget = (CrittersGrabber)grabbingActor;
			this.actorIdTarget = this.grabbedTarget.actorId;
			base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
			return;
		}
		if (crittersActorType != CrittersActor.CrittersActorType.Cage)
		{
			return;
		}
		this.SetState(CrittersPawn.CreatureState.Captured);
		this.cageTarget = (CrittersCage)grabbingActor;
		this.cageTarget.SetHasCritter(true);
		this.actorIdTarget = this.cageTarget.actorId;
		if (CrittersManager.instance.LocalAuthority())
		{
			base.transform.localScale = this.cageTarget.critterScale;
		}
		base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
	}

	// Token: 0x060002AB RID: 683 RVA: 0x000104A0 File Offset: 0x0000E6A0
	protected override void RemoteGrabbedBy(CrittersActor grabbingActor)
	{
		base.RemoteGrabbedBy(grabbingActor);
		CrittersActor.CrittersActorType crittersActorType = grabbingActor.crittersActorType;
		if (crittersActorType != CrittersActor.CrittersActorType.Grabber)
		{
			if (crittersActorType == CrittersActor.CrittersActorType.Cage)
			{
				this.cageTarget = (CrittersCage)grabbingActor;
				this.cageTarget.SetHasCritter(true);
				this.actorIdTarget = this.cageTarget.actorId;
				if (CrittersManager.instance.LocalAuthority())
				{
					base.transform.localScale = this.cageTarget.critterScale;
					return;
				}
			}
		}
		else
		{
			this.grabbedTarget = (CrittersGrabber)grabbingActor;
			this.actorIdTarget = this.grabbedTarget.actorId;
		}
	}

	// Token: 0x060002AC RID: 684 RVA: 0x00010530 File Offset: 0x0000E730
	public void Stunned(float duration)
	{
		if (this.currentState == CrittersPawn.CreatureState.Captured || this.currentState == CrittersPawn.CreatureState.Grabbed || this.currentState == CrittersPawn.CreatureState.Despawning || this.currentState == CrittersPawn.CreatureState.WaitingToDespawn)
		{
			return;
		}
		this.remainingStunnedTime = duration;
		this.SetState(CrittersPawn.CreatureState.Stunned);
		this.updatedSinceLastFrame = true;
	}

	// Token: 0x060002AD RID: 685 RVA: 0x0001056E File Offset: 0x0000E76E
	public bool AboveFearThreshold()
	{
		return this.currentFear >= this.scaredThreshold;
	}

	// Token: 0x060002AE RID: 686 RVA: 0x00010581 File Offset: 0x0000E781
	public bool BelowNotAfraidThreshold()
	{
		return this.currentFear < this.calmThreshold;
	}

	// Token: 0x060002AF RID: 687 RVA: 0x00010591 File Offset: 0x0000E791
	public bool AboveAttractedThreshold()
	{
		return this.currentAttraction >= this.attractedThreshold;
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x000105A4 File Offset: 0x0000E7A4
	public bool BelowUnAttractedThreshold()
	{
		return this.currentAttraction < this.unattractedThreshold;
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x000105B4 File Offset: 0x0000E7B4
	public bool AboveHungryThreshold()
	{
		return this.currentHunger >= this.hungryThreshold;
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x000105C7 File Offset: 0x0000E7C7
	public bool BelowNotHungryThreshold()
	{
		return this.currentHunger < this.satiatedThreshold;
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x000105D7 File Offset: 0x0000E7D7
	public bool AboveSleepyThreshold()
	{
		return this.currentSleepiness >= this.tiredThreshold;
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x000105EA File Offset: 0x0000E7EA
	public bool BelowNotSleepyThreshold()
	{
		return this.currentSleepiness < this.awakeThreshold;
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x000105FC File Offset: 0x0000E7FC
	public bool CanJump()
	{
		if (!this.canJump)
		{
			return false;
		}
		float num;
		if (this.currentState == CrittersPawn.CreatureState.Running)
		{
			num = this.scaredJumpCooldown;
		}
		else
		{
			num = this.jumpCooldown;
		}
		float num2 = PhotonNetwork.InRoom ? ((float)PhotonNetwork.Time) : Time.time;
		if (this.lastImpulseTime > (double)(num2 + this.jumpCooldown + this.jumpVariabilityTime))
		{
			this.lastImpulseTime = (double)(num2 + this.GetAdditiveJumpDelay());
		}
		return (double)num2 > this.lastImpulseTime + (double)num;
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x00010675 File Offset: 0x0000E875
	public void OnCollisionEnter(Collision collision)
	{
		this.canJump = true;
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x0001067E File Offset: 0x0000E87E
	public void OnCollisionExit(Collision collision)
	{
		this.canJump = false;
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x00010687 File Offset: 0x0000E887
	public void SetVelocity(Vector3 linearVelocity)
	{
		this.rb.linearVelocity = linearVelocity;
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x00010698 File Offset: 0x0000E898
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(Mathf.FloorToInt(this.currentFear));
		objList.Add(Mathf.FloorToInt(this.currentHunger));
		objList.Add(Mathf.FloorToInt(this.currentSleepiness));
		objList.Add(Mathf.FloorToInt(this.currentStruggle));
		objList.Add(this.currentState);
		objList.Add(this.actorIdTarget);
		objList.Add(this.lifeTimeStart);
		objList.Add(this.TemplateIndex);
		objList.Add(Mathf.FloorToInt(this.remainingStunnedTime));
		objList.Add(this.spawnStartTime);
		objList.Add(this.despawnStartTime);
		objList.AddRange(this.visuals.Appearance.WriteToRPCData());
		return this.TotalActorDataLength();
	}

	// Token: 0x060002BA RID: 698 RVA: 0x000107AC File Offset: 0x0000E9AC
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 11 + CritterAppearance.DataLength();
	}

	// Token: 0x060002BB RID: 699 RVA: 0x000107C0 File Offset: 0x0000E9C0
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		int num;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex], out num))
		{
			return this.TotalActorDataLength();
		}
		int num2;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 1], out num2))
		{
			return this.TotalActorDataLength();
		}
		int num3;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 2], out num3))
		{
			return this.TotalActorDataLength();
		}
		int num4;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 3], out num4))
		{
			return this.TotalActorDataLength();
		}
		int num5;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 4], out num5))
		{
			return this.TotalActorDataLength();
		}
		if (!Enum.IsDefined(typeof(CrittersPawn.CreatureState), (CrittersPawn.CreatureState)num5))
		{
			return this.TotalActorDataLength();
		}
		int num6;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 5], out num6))
		{
			return this.TotalActorDataLength();
		}
		double value;
		if (!CrittersManager.ValidateDataType<double>(data[startingIndex + 6], out value))
		{
			return this.TotalActorDataLength();
		}
		int templateIndex;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 7], out templateIndex))
		{
			return this.TotalActorDataLength();
		}
		int num7;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 8], out num7))
		{
			return this.TotalActorDataLength();
		}
		double value2;
		if (!CrittersManager.ValidateDataType<double>(data[startingIndex + 9], out value2))
		{
			return this.TotalActorDataLength();
		}
		double value3;
		if (!CrittersManager.ValidateDataType<double>(data[startingIndex + 10], out value3))
		{
			return this.TotalActorDataLength();
		}
		this.currentFear = (float)num;
		this.currentHunger = (float)num2;
		this.currentSleepiness = (float)num3;
		this.currentStruggle = (float)num4;
		this.SetState((CrittersPawn.CreatureState)num5);
		this.actorIdTarget = num6;
		this.lifeTimeStart = value.GetFinite();
		this.TemplateIndex = templateIndex;
		this.remainingStunnedTime = (float)num7;
		this.spawnStartTime = value2.GetFinite();
		this.despawnStartTime = value3.GetFinite();
		CrittersActor crittersActor = null;
		CrittersPawn.CreatureState creatureState = this.currentState;
		if (creatureState != CrittersPawn.CreatureState.Grabbed)
		{
			if (creatureState != CrittersPawn.CreatureState.Captured)
			{
				this.grabbedTarget = null;
				this.cageTarget = null;
			}
			else
			{
				if (CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor))
				{
					this.cageTarget = (CrittersCage)crittersActor;
					if (this.cageTarget != null)
					{
						base.transform.localScale = this.cageTarget.critterScale;
					}
				}
				this.grabbedTarget = null;
			}
		}
		else
		{
			if (CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor))
			{
				this.grabbedTarget = (CrittersGrabber)crittersActor;
			}
			this.cageTarget = null;
		}
		this.UpdateTemplate();
		this.visuals.SetAppearance(CritterAppearance.ReadFromRPCData(RuntimeHelpers.GetSubArray<object>(data, Range.StartAt(startingIndex + 11))));
		return this.TotalActorDataLength();
	}

	// Token: 0x060002BC RID: 700 RVA: 0x00010A28 File Offset: 0x0000EC28
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		int num;
		int num2;
		int num3;
		int num4;
		int num5;
		int num6;
		double num7;
		int templateIndex;
		int num8;
		double num9;
		double num10;
		if (!(base.UpdateSpecificActor(stream) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num2) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num3) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num4) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num5) & Enum.IsDefined(typeof(CrittersPawn.CreatureState), (CrittersPawn.CreatureState)num5) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num6) & CrittersManager.ValidateDataType<double>(stream.ReceiveNext(), out num7) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out templateIndex) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num8) & CrittersManager.ValidateDataType<double>(stream.ReceiveNext(), out num9) & CrittersManager.ValidateDataType<double>(stream.ReceiveNext(), out num10)))
		{
			return false;
		}
		this.currentFear = (float)num;
		this.currentHunger = (float)num2;
		this.currentSleepiness = (float)num3;
		this.currentStruggle = (float)num4;
		this.SetState((CrittersPawn.CreatureState)num5);
		this.actorIdTarget = num6;
		this.lifeTimeStart = num7;
		this.TemplateIndex = templateIndex;
		this.remainingStunnedTime = (float)num8;
		this.spawnStartTime = num9;
		this.despawnStartTime = num10;
		this.UpdateTemplate();
		CrittersActor crittersActor = null;
		CrittersPawn.CreatureState creatureState = this.currentState;
		if (creatureState != CrittersPawn.CreatureState.Grabbed)
		{
			if (creatureState != CrittersPawn.CreatureState.Captured)
			{
				this.grabbedTarget = null;
				this.cageTarget = null;
			}
			else
			{
				if (CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor))
				{
					this.cageTarget = (CrittersCage)crittersActor;
					if (this.cageTarget != null)
					{
						base.transform.localScale = this.cageTarget.critterScale;
					}
				}
				this.grabbedTarget = null;
			}
		}
		else
		{
			if (CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor))
			{
				this.grabbedTarget = (CrittersGrabber)crittersActor;
			}
			this.cageTarget = null;
		}
		return true;
	}

	// Token: 0x060002BD RID: 701 RVA: 0x00010C00 File Offset: 0x0000EE00
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(Mathf.FloorToInt(this.currentFear));
		stream.SendNext(Mathf.FloorToInt(this.currentHunger));
		stream.SendNext(Mathf.FloorToInt(this.currentSleepiness));
		stream.SendNext(Mathf.FloorToInt(this.currentStruggle));
		stream.SendNext(this.currentState);
		stream.SendNext(this.actorIdTarget);
		stream.SendNext(this.lifeTimeStart);
		stream.SendNext(this.TemplateIndex);
		stream.SendNext(Mathf.FloorToInt(this.remainingStunnedTime));
		stream.SendNext(this.spawnStartTime);
		stream.SendNext(this.despawnStartTime);
	}

	// Token: 0x060002BE RID: 702 RVA: 0x00002AF8 File Offset: 0x00000CF8
	public void SetConfiguration(CritterConfiguration getRandomConfiguration)
	{
		throw new NotImplementedException();
	}

	// Token: 0x060002BF RID: 703 RVA: 0x00010CE8 File Offset: 0x0000EEE8
	public void SetSpawnData(object[] spawnData)
	{
		this.visuals.SetAppearance(CritterAppearance.ReadFromRPCData(spawnData));
	}

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x060002C0 RID: 704 RVA: 0x00010CFB File Offset: 0x0000EEFB
	int IEyeScannable.scannableId
	{
		get
		{
			return base.gameObject.GetInstanceID();
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x060002C1 RID: 705 RVA: 0x00010D08 File Offset: 0x0000EF08
	Vector3 IEyeScannable.Position
	{
		get
		{
			return this.bodyCollider.bounds.center;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x060002C2 RID: 706 RVA: 0x00010D28 File Offset: 0x0000EF28
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return this.bodyCollider.bounds;
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x060002C3 RID: 707 RVA: 0x00010D35 File Offset: 0x0000EF35
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.BuildEyeScannerData();
		}
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x00010D40 File Offset: 0x0000EF40
	private IList<KeyValueStringPair> BuildEyeScannerData()
	{
		this.eyeScanData[0] = new KeyValueStringPair("Name", this.creatureConfiguration.critterName);
		this.eyeScanData[1] = new KeyValueStringPair("Type", this.creatureConfiguration.animalType.ToString());
		this.eyeScanData[2] = new KeyValueStringPair("Temperament", this.creatureConfiguration.behaviour.temperament);
		this.eyeScanData[3] = new KeyValueStringPair("Habitat", this.creatureConfiguration.biome.GetHabitatDescription());
		this.eyeScanData[4] = new KeyValueStringPair("Size", this.visuals.Appearance.size.ToString("0.00"));
		this.eyeScanData[5] = new KeyValueStringPair("State", this.GetCurrentStateName());
		return this.eyeScanData;
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x00010E3C File Offset: 0x0000F03C
	private string GetCurrentStateName()
	{
		string text;
		switch (this.currentState)
		{
		case CrittersPawn.CreatureState.Idle:
			text = "Adventuring";
			break;
		case CrittersPawn.CreatureState.Eating:
			text = "Eating";
			break;
		case CrittersPawn.CreatureState.AttractedTo:
			text = "Curious";
			break;
		case CrittersPawn.CreatureState.Running:
			text = "Scared";
			break;
		case CrittersPawn.CreatureState.Grabbed:
			text = ((this.struggleGainedPerSecond > 0f) ? "Struggling" : "Happy");
			break;
		case CrittersPawn.CreatureState.Sleeping:
			text = "Sleeping";
			break;
		case CrittersPawn.CreatureState.SeekingFood:
			text = "Foraging";
			break;
		case CrittersPawn.CreatureState.Captured:
			text = "Captured";
			break;
		case CrittersPawn.CreatureState.Stunned:
			text = "Stunned";
			break;
		default:
			text = "Contemplating Life";
			break;
		}
		string text2 = text;
		if (this.slowSpeedMod < 1f)
		{
			text2 = "Slowed, " + text2;
		}
		return text2;
	}

	// Token: 0x14000007 RID: 7
	// (add) Token: 0x060002C6 RID: 710 RVA: 0x00010EFC File Offset: 0x0000F0FC
	// (remove) Token: 0x060002C7 RID: 711 RVA: 0x00010F34 File Offset: 0x0000F134
	public event Action OnDataChange;

	// Token: 0x040002C4 RID: 708
	[NonSerialized]
	public CritterConfiguration creatureConfiguration;

	// Token: 0x040002C5 RID: 709
	public Collider bodyCollider;

	// Token: 0x040002C6 RID: 710
	[HideInInspector]
	[NonSerialized]
	public float maxJumpVel;

	// Token: 0x040002C7 RID: 711
	[HideInInspector]
	[NonSerialized]
	public float jumpCooldown;

	// Token: 0x040002C8 RID: 712
	[HideInInspector]
	[NonSerialized]
	public float scaredJumpCooldown;

	// Token: 0x040002C9 RID: 713
	[HideInInspector]
	[NonSerialized]
	public float jumpVariabilityTime;

	// Token: 0x040002CA RID: 714
	[HideInInspector]
	[NonSerialized]
	public float visionConeAngle;

	// Token: 0x040002CB RID: 715
	[HideInInspector]
	[NonSerialized]
	public float sensoryRange;

	// Token: 0x040002CC RID: 716
	[HideInInspector]
	[NonSerialized]
	public float maxHunger;

	// Token: 0x040002CD RID: 717
	[HideInInspector]
	[NonSerialized]
	public float hungryThreshold;

	// Token: 0x040002CE RID: 718
	[HideInInspector]
	[NonSerialized]
	public float satiatedThreshold;

	// Token: 0x040002CF RID: 719
	[HideInInspector]
	[NonSerialized]
	public float hungerLostPerSecond;

	// Token: 0x040002D0 RID: 720
	[HideInInspector]
	[NonSerialized]
	public float hungerGainedPerSecond;

	// Token: 0x040002D1 RID: 721
	[HideInInspector]
	[NonSerialized]
	public float maxFear;

	// Token: 0x040002D2 RID: 722
	[HideInInspector]
	[NonSerialized]
	public float scaredThreshold;

	// Token: 0x040002D3 RID: 723
	[HideInInspector]
	[NonSerialized]
	public float calmThreshold;

	// Token: 0x040002D4 RID: 724
	[HideInInspector]
	[NonSerialized]
	public float fearLostPerSecond;

	// Token: 0x040002D5 RID: 725
	[NonSerialized]
	public float maxAttraction;

	// Token: 0x040002D6 RID: 726
	[NonSerialized]
	public float attractedThreshold;

	// Token: 0x040002D7 RID: 727
	[NonSerialized]
	public float unattractedThreshold;

	// Token: 0x040002D8 RID: 728
	[NonSerialized]
	public float attractionLostPerSecond;

	// Token: 0x040002D9 RID: 729
	[HideInInspector]
	[NonSerialized]
	public float maxSleepiness;

	// Token: 0x040002DA RID: 730
	[HideInInspector]
	[NonSerialized]
	public float tiredThreshold;

	// Token: 0x040002DB RID: 731
	[HideInInspector]
	[NonSerialized]
	public float awakeThreshold;

	// Token: 0x040002DC RID: 732
	[HideInInspector]
	[NonSerialized]
	public float sleepinessGainedPerSecond;

	// Token: 0x040002DD RID: 733
	[HideInInspector]
	[NonSerialized]
	public float sleepinessLostPerSecond;

	// Token: 0x040002DE RID: 734
	[HideInInspector]
	[NonSerialized]
	public float maxStruggle;

	// Token: 0x040002DF RID: 735
	[HideInInspector]
	[NonSerialized]
	public float escapeThreshold;

	// Token: 0x040002E0 RID: 736
	[HideInInspector]
	[NonSerialized]
	public float catchableThreshold;

	// Token: 0x040002E1 RID: 737
	[HideInInspector]
	[NonSerialized]
	public float struggleGainedPerSecond;

	// Token: 0x040002E2 RID: 738
	[HideInInspector]
	[NonSerialized]
	public float struggleLostPerSecond;

	// Token: 0x040002E3 RID: 739
	public List<crittersAttractorStruct> attractedToList;

	// Token: 0x040002E4 RID: 740
	public List<crittersAttractorStruct> afraidOfList;

	// Token: 0x040002E5 RID: 741
	public Dictionary<CrittersActor.CrittersActorType, float> afraidOfTypes;

	// Token: 0x040002E6 RID: 742
	public Dictionary<CrittersActor.CrittersActorType, float> attractedToTypes;

	// Token: 0x040002E7 RID: 743
	private Rigidbody rB;

	// Token: 0x040002E8 RID: 744
	[NonSerialized]
	public CrittersPawn.CreatureState currentState;

	// Token: 0x040002E9 RID: 745
	[NonSerialized]
	public float currentHunger;

	// Token: 0x040002EA RID: 746
	[NonSerialized]
	public float currentFear;

	// Token: 0x040002EB RID: 747
	[NonSerialized]
	public float currentAttraction;

	// Token: 0x040002EC RID: 748
	[NonSerialized]
	public float currentSleepiness;

	// Token: 0x040002ED RID: 749
	[NonSerialized]
	public float currentStruggle;

	// Token: 0x040002EE RID: 750
	public double lifeTime = 10.0;

	// Token: 0x040002EF RID: 751
	public double lifeTimeStart;

	// Token: 0x040002F0 RID: 752
	private CrittersFood eatingTarget;

	// Token: 0x040002F1 RID: 753
	private CrittersActor fearTarget;

	// Token: 0x040002F2 RID: 754
	private CrittersActor attractionTarget;

	// Token: 0x040002F3 RID: 755
	private Vector3 lastSeenFearPosition;

	// Token: 0x040002F4 RID: 756
	private Vector3 lastSeenAttractionPosition;

	// Token: 0x040002F5 RID: 757
	private CrittersGrabber grabbedTarget;

	// Token: 0x040002F6 RID: 758
	private CrittersCage cageTarget;

	// Token: 0x040002F7 RID: 759
	private int actorIdTarget;

	// Token: 0x040002F8 RID: 760
	[FormerlySerializedAs("eatingRadiusMax")]
	public float eatingRadiusMaxSquared;

	// Token: 0x040002F9 RID: 761
	private bool withinEatingRadius;

	// Token: 0x040002FA RID: 762
	public Transform animTarget;

	// Token: 0x040002FB RID: 763
	public MeshRenderer myRenderer;

	// Token: 0x040002FC RID: 764
	public float autoSeeFoodDistance;

	// Token: 0x040002FD RID: 765
	public Dictionary<int, CrittersActor> soundsHeard;

	// Token: 0x040002FE RID: 766
	public float fudge = 1.1f;

	// Token: 0x040002FF RID: 767
	public float obstacleSeeDistance = 0.25f;

	// Token: 0x04000300 RID: 768
	private RaycastHit[] raycastHits;

	// Token: 0x04000301 RID: 769
	private bool canJump;

	// Token: 0x04000302 RID: 770
	private bool wasSomethingInTheWay;

	// Token: 0x04000303 RID: 771
	public Transform hat;

	// Token: 0x04000304 RID: 772
	private int LastTemplateIndex = -1;

	// Token: 0x04000305 RID: 773
	private int TemplateIndex = -1;

	// Token: 0x04000306 RID: 774
	private double _nextDespawnCheck;

	// Token: 0x04000307 RID: 775
	private double _nextStuckCheck;

	// Token: 0x04000308 RID: 776
	public float killHeight = -500f;

	// Token: 0x04000309 RID: 777
	private float remainingStunnedTime;

	// Token: 0x0400030A RID: 778
	private float remainingSlowedTime;

	// Token: 0x0400030B RID: 779
	private float slowSpeedMod = 1f;

	// Token: 0x0400030C RID: 780
	[Header("Visuals")]
	public CritterVisuals visuals;

	// Token: 0x0400030D RID: 781
	[HideInInspector]
	public Dictionary<CrittersPawn.CreatureState, GameObject> StartStateFX = new Dictionary<CrittersPawn.CreatureState, GameObject>();

	// Token: 0x0400030E RID: 782
	[HideInInspector]
	public Dictionary<CrittersPawn.CreatureState, GameObject> OngoingStateFX = new Dictionary<CrittersPawn.CreatureState, GameObject>();

	// Token: 0x0400030F RID: 783
	[NonSerialized]
	public GameObject OnReleasedFX;

	// Token: 0x04000310 RID: 784
	private GameObject currentOngoingStateFX;

	// Token: 0x04000311 RID: 785
	[HideInInspector]
	public Dictionary<CrittersPawn.CreatureState, CrittersAnim> stateAnim = new Dictionary<CrittersPawn.CreatureState, CrittersAnim>();

	// Token: 0x04000312 RID: 786
	private CrittersAnim currentAnim;

	// Token: 0x04000313 RID: 787
	private float currentAnimTime;

	// Token: 0x04000314 RID: 788
	public AudioClip grabbedHaptics;

	// Token: 0x04000315 RID: 789
	public float grabbedHapticsStrength;

	// Token: 0x04000316 RID: 790
	public AnimationCurve spawnInHeighMovement;

	// Token: 0x04000317 RID: 791
	public AnimationCurve despawnInHeighMovement;

	// Token: 0x04000318 RID: 792
	private Vector3 spawningStartingPosition;

	// Token: 0x04000319 RID: 793
	private double spawnStartTime;

	// Token: 0x0400031A RID: 794
	private double despawnStartTime;

	// Token: 0x0400031B RID: 795
	private float _spawnAnimationDuration;

	// Token: 0x0400031C RID: 796
	private float _despawnAnimationDuration;

	// Token: 0x0400031D RID: 797
	private double _spawnAnimTime;

	// Token: 0x0400031E RID: 798
	private double _despawnAnimTime;

	// Token: 0x0400031F RID: 799
	public MeshRenderer debugStateIndicator;

	// Token: 0x04000320 RID: 800
	public Color debugColorIdle;

	// Token: 0x04000321 RID: 801
	public Color debugColorSeekingFood;

	// Token: 0x04000322 RID: 802
	public Color debugColorEating;

	// Token: 0x04000323 RID: 803
	public Color debugColorScared;

	// Token: 0x04000324 RID: 804
	public Color debugColorSleeping;

	// Token: 0x04000325 RID: 805
	public Color debugColorCaught;

	// Token: 0x04000326 RID: 806
	public Color debugColorCaged;

	// Token: 0x04000327 RID: 807
	public Color debugColorStunned;

	// Token: 0x04000328 RID: 808
	public Color debugColorAttracted;

	// Token: 0x04000329 RID: 809
	[NonSerialized]
	public int regionId;

	// Token: 0x0400032A RID: 810
	private KeyValueStringPair[] eyeScanData = new KeyValueStringPair[6];

	// Token: 0x0200006F RID: 111
	public enum CreatureState
	{
		// Token: 0x0400032D RID: 813
		Idle,
		// Token: 0x0400032E RID: 814
		Eating,
		// Token: 0x0400032F RID: 815
		AttractedTo,
		// Token: 0x04000330 RID: 816
		Running,
		// Token: 0x04000331 RID: 817
		Grabbed,
		// Token: 0x04000332 RID: 818
		Sleeping,
		// Token: 0x04000333 RID: 819
		SeekingFood,
		// Token: 0x04000334 RID: 820
		Captured,
		// Token: 0x04000335 RID: 821
		Stunned,
		// Token: 0x04000336 RID: 822
		WaitingToDespawn,
		// Token: 0x04000337 RID: 823
		Despawning,
		// Token: 0x04000338 RID: 824
		Spawning
	}

	// Token: 0x02000070 RID: 112
	internal struct CreatureUpdateData
	{
		// Token: 0x060002C9 RID: 713 RVA: 0x00010FF5 File Offset: 0x0000F1F5
		internal CreatureUpdateData(CrittersPawn creature)
		{
			this.lastImpulseTime = creature.lastImpulseTime;
			this.state = creature.currentState;
		}

		// Token: 0x060002CA RID: 714 RVA: 0x0001100F File Offset: 0x0000F20F
		internal bool SameData(CrittersPawn creature)
		{
			return this.lastImpulseTime == creature.lastImpulseTime && this.state == creature.currentState;
		}

		// Token: 0x04000339 RID: 825
		private double lastImpulseTime;

		// Token: 0x0400033A RID: 826
		private CrittersPawn.CreatureState state;
	}
}
