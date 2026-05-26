using System;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000E7 RID: 231
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetDashYoyo : SIGadget
{
	// Token: 0x1700005E RID: 94
	// (get) Token: 0x06000569 RID: 1385 RVA: 0x0001E9FC File Offset: 0x0001CBFC
	private int _HandIndex
	{
		get
		{
			if ((this.m_snappable.snappedToJoint != null && this.m_snappable.snappedToJoint.jointType == SnapJointType.HandL) || this.gameEntity.heldByHandIndex == 0)
			{
				return 0;
			}
			if ((this.m_snappable.snappedToJoint != null && this.m_snappable.snappedToJoint.jointType == SnapJointType.HandR) || this.gameEntity.heldByHandIndex == 1)
			{
				return 1;
			}
			return -1;
		}
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x0001EA78 File Offset: 0x0001CC78
	private void Start()
	{
		this._stateMaterials = this.m_baseStateMats;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x0001EB30 File Offset: 0x0001CD30
	private void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Remove(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
		if (this._attachedVRRig != null)
		{
			VRRig attachedVRRig = this._attachedVRRig;
			attachedVRRig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(attachedVRRig.OnMaterialIndexChanged, new Action<int, int>(this._HandleVRRigMaterialIndexChanged));
		}
		this._ResetYoYo();
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x0001EC1C File Offset: 0x0001CE1C
	private void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		SIGadgetDashYoyo.EState state = this._state;
		if (state - SIGadgetDashYoyo.EState.Thrown <= 2)
		{
			this.m_tetherLineRenderer.SetPosition(1, this.m_tetherLineRenderer.transform.InverseTransformPoint(this.m_yoyoTarget.position));
		}
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x0001EC68 File Offset: 0x0001CE68
	private void _HandleStartInteraction()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._attachedPlayerActorNr = this.gameEntity.AttachedPlayerActorNr;
		this._attachedNetPlayer = NetworkSystem.Instance.GetPlayer(this._attachedPlayerActorNr);
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this._attachedPlayerActorNr, out gamePlayer))
		{
			return;
		}
		if (this._attachedVRRig != null)
		{
			VRRig attachedVRRig = this._attachedVRRig;
			attachedVRRig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(attachedVRRig.OnMaterialIndexChanged, new Action<int, int>(this._HandleVRRigMaterialIndexChanged));
		}
		this._attachedVRRig = gamePlayer.rig;
		VRRig attachedVRRig2 = this._attachedVRRig;
		attachedVRRig2.OnMaterialIndexChanged = (Action<int, int>)Delegate.Combine(attachedVRRig2.OnMaterialIndexChanged, new Action<int, int>(this._HandleVRRigMaterialIndexChanged));
		int num = this._isTagged ? 2 : 0;
		if (num != this._attachedVRRig.setMatIndex)
		{
			this._HandleVRRigMaterialIndexChanged(num, this._attachedVRRig.setMatIndex);
		}
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x0001ED4C File Offset: 0x0001CF4C
	private void _HandleStopInteraction()
	{
		this._attachedPlayerActorNr = -1;
		this._attachedNetPlayer = null;
		if (this._attachedVRRig != null)
		{
			VRRig attachedVRRig = this._attachedVRRig;
			attachedVRRig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(attachedVRRig.OnMaterialIndexChanged, new Action<int, int>(this._HandleVRRigMaterialIndexChanged));
		}
		this._attachedVRRig = null;
		if (this._isTagged)
		{
			this._HandleVRRigMaterialIndexChanged(2, 0);
		}
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		if (this._state == SIGadgetDashYoyo.EState.DashUsed || this._state == SIGadgetDashYoyo.EState.OnCooldown)
		{
			this.SetStateAuthority(SIGadgetDashYoyo.EState.OnCooldown);
		}
		else
		{
			this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
		}
		GTPlayer.Instance.ResetRigidbodyInterpolation();
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x0001EDF0 File Offset: 0x0001CFF0
	private void _HandleVRRigMaterialIndexChanged(int oldMatIndex, int newMatIndex)
	{
		if (this._attachedPlayerActorNr != -1 && (newMatIndex == 2 || newMatIndex == 1) && this._hasTagUpgrade)
		{
			SuperInfectionGame superInfectionGame = GorillaGameManager.instance as SuperInfectionGame;
			if (superInfectionGame != null)
			{
				this._isTagged = (this._attachedNetPlayer != null && superInfectionGame.IsInfected(this._attachedNetPlayer));
				this._OnTagStateOrUpgradesChanged();
				return;
			}
		}
		this._isTagged = false;
		this._OnTagStateOrUpgradesChanged();
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x0001EE58 File Offset: 0x0001D058
	protected override void OnUpdateAuthority(float dt)
	{
		base.OnUpdateAuthority(dt);
		this._wasActivated = this._isActivated;
		this._isActivated = this._CheckInput();
		if (Time.unscaledTime < this._successfulYankTime + this.m_slipperySurfacesTime)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
		}
		switch (this._state)
		{
		case SIGadgetDashYoyo.EState.Idle:
			if (this._isActivated)
			{
				this._PlayHaptic(0.1f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.PreparedToThrow);
				return;
			}
			break;
		case SIGadgetDashYoyo.EState.OnCooldown:
			if (Time.unscaledTime > this._successfulYankTime + this._cooldownDuration)
			{
				this._PlayHaptic(0.5f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
				return;
			}
			break;
		case SIGadgetDashYoyo.EState.PreparedToThrow:
			if (!this._isActivated)
			{
				if (this._ThrowYoYoTarget())
				{
					this._PlayHaptic(0.5f);
					GTPlayer.Instance.RigidbodyInterpolation = RigidbodyInterpolation.None;
					this.SetStateAuthority(SIGadgetDashYoyo.EState.Thrown);
					return;
				}
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
				return;
			}
			break;
		case SIGadgetDashYoyo.EState.Thrown:
			if (Time.unscaledTime > this._timeLastThrown + this.m_waitBeforeAutoReturn)
			{
				this._PlayHaptic(0.75f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
				GTPlayer.Instance.ResetRigidbodyInterpolation();
				return;
			}
			if (GTPlayer.Instance.RigidbodyInterpolation != RigidbodyInterpolation.None)
			{
				GTPlayer.Instance.RigidbodyInterpolation = RigidbodyInterpolation.None;
			}
			if (this._isActivated)
			{
				this.SetStateAuthority(SIGadgetDashYoyo.EState.PreparedToDash);
				return;
			}
			break;
		case SIGadgetDashYoyo.EState.PreparedToDash:
			if (Time.unscaledTime > this._timeLastThrown + this.m_waitBeforeAutoReturn)
			{
				this._PlayHaptic(0.75f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
				return;
			}
			if (!this._isActivated)
			{
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Thrown);
				return;
			}
			this._CheckYankProgression();
			return;
		case SIGadgetDashYoyo.EState.DashUsed:
			if (Time.unscaledTime > this._successfulYankTime + this.m_postYankCooldown)
			{
				this._PlayHaptic(0.1f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.OnCooldown);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x0001F008 File Offset: 0x0001D208
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetDashYoyo.EState estate = (SIGadgetDashYoyo.EState)this.gameEntity.GetState();
		if (estate != this._state)
		{
			this._SetStateShared(estate);
		}
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x0001F039 File Offset: 0x0001D239
	private static bool _CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 6L;
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x0001F047 File Offset: 0x0001D247
	private void SetStateAuthority(SIGadgetDashYoyo.EState newState)
	{
		this._SetStateShared(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x0001F068 File Offset: 0x0001D268
	private void _SetStateShared(SIGadgetDashYoyo.EState newState)
	{
		if (newState == this._state || !SIGadgetDashYoyo._CanChangeState((long)newState))
		{
			return;
		}
		SIGadgetDashYoyo.EState state = this._state;
		this._state = newState;
		switch (this._state)
		{
		case SIGadgetDashYoyo.EState.Idle:
			if (state == SIGadgetDashYoyo.EState.OnCooldown)
			{
				this._PlayAudio(4);
			}
			else if (state == SIGadgetDashYoyo.EState.PreparedToThrow)
			{
				this._PlayAudio(5);
			}
			this._ResetYoYo();
			this._SetMaterials(this._stateMaterials.idle);
			return;
		case SIGadgetDashYoyo.EState.OnCooldown:
			this._PlayAudio(3);
			this._ResetYoYo();
			this._SetMaterials(this._stateMaterials.cooldown);
			return;
		case SIGadgetDashYoyo.EState.PreparedToThrow:
			this._PlayAudio(0);
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.Thrown:
			if (state != SIGadgetDashYoyo.EState.PreparedToDash)
			{
				this._PlayAudio(1);
			}
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.PreparedToDash:
			this._yankBeginPos = this.m_yoyoDefaultPosXform.position;
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.DashUsed:
			this._PlayAudio(2);
			this._FreezeYoYo();
			this._SetMaterials(this._stateMaterials.cooldown);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x0001F184 File Offset: 0x0001D384
	private bool _CheckInput()
	{
		float sensitivity = this._wasActivated ? this.m_inputDeactivateThreshold : this.m_inputActivateThreshold;
		return this.m_buttonActivatable.CheckInput(sensitivity);
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x0001F1B4 File Offset: 0x0001D3B4
	private bool _ThrowYoYoTarget()
	{
		Vector3 vector = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex);
		if (vector.magnitude < this.m_minThrowSpeed)
		{
			return false;
		}
		Vector3 handAngularVelocity = GamePlayerLocal.instance.GetHandAngularVelocity(this._HandIndex);
		GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
		vector *= this._throwMultiplier;
		vector += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
		this._LaunchYoYoShared(vector, handAngularVelocity, this.m_yoyoTargetRB.transform.position, this.m_yoyoTargetRB.transform.rotation);
		this._timeLastThrown = Time.unscaledTime;
		if (!NetworkSystem.Instance.InRoom)
		{
			return true;
		}
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone == null)
		{
			return true;
		}
		this._launchYoyoRPCArgs[0] = this.gameEntity.GetNetId();
		this._launchYoyoRPCArgs[1] = vector;
		this._launchYoyoRPCArgs[2] = handAngularVelocity;
		this._launchYoyoRPCArgs[3] = this.m_yoyoTargetRB.transform.position;
		this._launchYoyoRPCArgs[4] = this.m_yoyoTargetRB.transform.rotation;
		simanagerForZone.CallRPC(SuperInfectionManager.ClientToClientRPC.LaunchDashYoyo, this._launchYoyoRPCArgs);
		return true;
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0001F302 File Offset: 0x0001D502
	internal void RemoteThrowYoYoTarget(Vector3 velocity, Vector3 angVelocity, Vector3 targetPosition, Quaternion targetRotation)
	{
		this._LaunchYoYoShared(velocity, angVelocity, targetPosition, targetRotation);
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0001F310 File Offset: 0x0001D510
	private void _LaunchYoYoShared(Vector3 velocity, Vector3 angVelocity, Vector3 targetPosition, Quaternion targetRotation)
	{
		this.m_yoyoTargetRB.transform.parent = null;
		float x = base.transform.lossyScale.x;
		this.m_yoyoTargetRB.transform.localScale = new Vector3(x, x, x);
		this.m_yoyoTargetRB.transform.position = targetPosition;
		this.m_yoyoTargetRB.transform.rotation = targetRotation;
		this.m_yoyoTargetRB.gameObject.SetActive(true);
		this.m_yoyoTarget.parent = this.m_yoyoTargetRB.transform;
		this.m_yoyoTargetRB.isKinematic = false;
		this.m_yoyoTargetRB.linearVelocity = velocity;
		this.m_yoyoTargetRB.angularVelocity = angVelocity;
		this.m_tetherLineRenderer.gameObject.SetActive(true);
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x0001F3D6 File Offset: 0x0001D5D6
	private void _FreezeYoYo()
	{
		this.m_yoyoTargetRB.gameObject.SetActive(false);
		this.m_yoyoTarget.parent = null;
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x0001F3F8 File Offset: 0x0001D5F8
	internal void OnHitPlayer_Authority(SuperInfectionGame siTagGameManager, NetPlayer victimNetPlayer)
	{
		bool flag = siTagGameManager.IsInfected(this._attachedNetPlayer);
		bool flag2 = siTagGameManager.IsInfected(victimNetPlayer);
		if (flag == flag2)
		{
			return;
		}
		if (this._hasTagUpgrade && !flag2)
		{
			siTagGameManager.ReportTag(victimNetPlayer, this._attachedNetPlayer);
			return;
		}
		RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.SetSlowedTime, victimNetPlayer);
		RoomSystem.SendSoundEffectOnOther(5, 0.125f, victimNetPlayer, false);
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x0001F44C File Offset: 0x0001D64C
	private void _ResetYoYo()
	{
		this.m_tetherLineRenderer.gameObject.SetActive(false);
		this.m_yoyoTargetRB.gameObject.SetActive(false);
		this.m_yoyoTarget.SetParent(this.m_yoyoDefaultPosXform, false);
		this.m_yoyoTarget.transform.localPosition = Vector3.zero;
		this.m_yoyoTarget.transform.localRotation = Quaternion.identity;
		this.m_yoyoTargetRB.transform.localScale = Vector3.one;
		this.m_yoyoTargetRB.transform.SetParent(this.m_yoyoDefaultPosXform, false);
		this.m_yoyoTargetRB.transform.localPosition = Vector3.zero;
		this.m_yoyoTargetRB.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x0001F50D File Offset: 0x0001D70D
	private void _SetMaterials(Material mat)
	{
		this.m_yoyoRenderer.sharedMaterial = mat;
		this.m_tetherLineRenderer.sharedMaterial = mat;
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x0001F528 File Offset: 0x0001D728
	private void _CheckYankProgression()
	{
		Vector3 handVelocity = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex);
		this._maxEncounteredYankSpeed = Mathf.Max(this._maxEncounteredYankSpeed, handVelocity.magnitude);
		Vector3 vector = this._yankBeginPos - this.m_yoyoDefaultPosXform.position;
		Vector3 normalized = (-handVelocity.normalized + vector.normalized).normalized;
		Vector3 from = this.m_yoyoTarget.position - this.m_yoyoDefaultPosXform.position;
		if (vector.magnitude < this.m_yankMinDistance || this._maxEncounteredYankSpeed < this.m_yankMinSpeed || Vector3.Angle(from, normalized) > this.m_yankMaxAngle)
		{
			return;
		}
		if (base.IsBlocked(SIExclusionType.AffectsLocalMovement))
		{
			return;
		}
		this._successfulYankTime = Time.unscaledTime;
		float d = this._CalculateDashSpeed(handVelocity.magnitude);
		GTPlayer instance = GTPlayer.Instance;
		instance.SetMaximumSlipThisFrame();
		instance.SetVelocity(Vector3.RotateTowards(from.normalized, normalized, this._maxInfluenceAngle * 0.017453292f, 0f) * d);
		this._PlayHaptic(2f);
		this.SetStateAuthority(SIGadgetDashYoyo.EState.DashUsed);
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x0001F650 File Offset: 0x0001D850
	private float _CalculateDashSpeed(float currentYankSpeed)
	{
		float time = Mathf.InverseLerp(this.m_yankMinSpeed, this.m_yankMaxSpeed, currentYankSpeed);
		float t = this.m_speedMappingCurve.Evaluate(time);
		return Mathf.Lerp(this.m_minDashSpeed, this._maxDashSpeed, t);
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x0001F690 File Offset: 0x0001D890
	private void _PlayHaptic(float strengthMultiplier)
	{
		bool forLeftController;
		if (base.FindAttachedHand(out forLeftController))
		{
			GorillaTagger.Instance.StartVibration(forLeftController, GorillaTagger.Instance.tapHapticStrength * strengthMultiplier, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x0001F6C8 File Offset: 0x0001D8C8
	private void _PlayAudio(int index)
	{
		this.m_audioSource.clip = this.m_clips[index];
		this.m_audioSource.volume = this.m_clipVolumes[index];
		this.m_audioSource.GTPlay();
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x0001F6FC File Offset: 0x0001D8FC
	private void _OnTagStateOrUpgradesChanged()
	{
		this._stateMaterials = (this._hasTagUpgrade ? (this._isTagged ? this.m_tagUpgradeStateMatsWhileTagged : this.m_tagUpgradeStateMatsWhileUntagged) : this.m_baseStateMats);
		switch (this._state)
		{
		case SIGadgetDashYoyo.EState.Idle:
			this._SetMaterials(this._stateMaterials.idle);
			return;
		case SIGadgetDashYoyo.EState.OnCooldown:
			this._SetMaterials(this._stateMaterials.cooldown);
			return;
		case SIGadgetDashYoyo.EState.PreparedToThrow:
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.Thrown:
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.PreparedToDash:
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.DashUsed:
			this._SetMaterials(this._stateMaterials.cooldown);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x0001F7C8 File Offset: 0x0001D9C8
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._cooldownDuration = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Cooldown) ? this.m_cooldownDurationUpgrade : this.m_cooldownDurationDefault);
		this._throwMultiplier = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Range) ? this.m_throwMultiplierUpgrade : this.m_throwMultiplierDefault);
		this._maxDashSpeed = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Speed) ? this.m_maxDashSpeedUpgraded : this.m_maxDashSpeedDefault);
		this._maxInfluenceAngle = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Dynamic) ? this.m_maxInfluenceAngleUpgrade : this.m_maxInfluenceAngleDefault);
		this._hasStunUpgrade = withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Stun);
		this._hasTagUpgrade = withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Tag);
		this._OnTagStateOrUpgradesChanged();
	}

	// Token: 0x04000658 RID: 1624
	private const string preLog = "[SIGadgetDashYoyo]  ";

	// Token: 0x04000659 RID: 1625
	private const string preErr = "[SIGadgetDashYoyo]  ERROR!!!  ";

	// Token: 0x0400065A RID: 1626
	[SerializeField]
	private GameSnappable m_snappable;

	// Token: 0x0400065B RID: 1627
	[SerializeField]
	private Transform m_yoyoDefaultPosXform;

	// Token: 0x0400065C RID: 1628
	[SerializeField]
	private Transform m_yoyoTarget;

	// Token: 0x0400065D RID: 1629
	[SerializeField]
	private Rigidbody m_yoyoTargetRB;

	// Token: 0x0400065E RID: 1630
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x0400065F RID: 1631
	[SerializeField]
	private float m_inputActivateThreshold = 0.35f;

	// Token: 0x04000660 RID: 1632
	[SerializeField]
	private float m_inputDeactivateThreshold = 0.25f;

	// Token: 0x04000661 RID: 1633
	private SIGadgetDashYoyo.StateMaterialsInfo _stateMaterials;

	// Token: 0x04000662 RID: 1634
	[SerializeField]
	private SIGadgetDashYoyo.StateMaterialsInfo m_baseStateMats;

	// Token: 0x04000663 RID: 1635
	[SerializeField]
	private SIGadgetDashYoyo.StateMaterialsInfo m_tagUpgradeStateMatsWhileTagged;

	// Token: 0x04000664 RID: 1636
	[SerializeField]
	private SIGadgetDashYoyo.StateMaterialsInfo m_tagUpgradeStateMatsWhileUntagged;

	// Token: 0x04000665 RID: 1637
	[SerializeField]
	private MeshRenderer m_yoyoRenderer;

	// Token: 0x04000666 RID: 1638
	[SerializeField]
	private AudioSource m_audioSource;

	// Token: 0x04000667 RID: 1639
	[SerializeField]
	public AudioClip[] m_clips;

	// Token: 0x04000668 RID: 1640
	[SerializeField]
	public float[] m_clipVolumes;

	// Token: 0x04000669 RID: 1641
	private float _throwMultiplier;

	// Token: 0x0400066A RID: 1642
	[SerializeField]
	private float m_throwMultiplierDefault = 1.5f;

	// Token: 0x0400066B RID: 1643
	[SerializeField]
	private float m_throwMultiplierUpgrade = 2f;

	// Token: 0x0400066C RID: 1644
	[FormerlySerializedAs("m_tether")]
	[SerializeField]
	private LineRenderer m_tetherLineRenderer;

	// Token: 0x0400066D RID: 1645
	[SerializeField]
	private float m_minThrowSpeed = 2f;

	// Token: 0x0400066E RID: 1646
	[SerializeField]
	private float m_waitBeforeAutoReturn = 3f;

	// Token: 0x0400066F RID: 1647
	[SerializeField]
	private float m_postYankCooldown = 2f;

	// Token: 0x04000670 RID: 1648
	[SerializeField]
	private float m_maxYankRecheckTime = 0.2f;

	// Token: 0x04000671 RID: 1649
	[SerializeField]
	private float m_yankMinDistance = 0.5f;

	// Token: 0x04000672 RID: 1650
	[SerializeField]
	private float m_yankMaxAngle = 60f;

	// Token: 0x04000673 RID: 1651
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMinSpeed = 2f;

	// Token: 0x04000674 RID: 1652
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMaxSpeed = 8f;

	// Token: 0x04000675 RID: 1653
	[Tooltip("Dash min/max speed: The fastest speed the player will move")]
	[SerializeField]
	private float m_minDashSpeed = 4f;

	// Token: 0x04000676 RID: 1654
	private float _maxDashSpeed;

	// Token: 0x04000677 RID: 1655
	[SerializeField]
	private float m_maxDashSpeedDefault = 11f;

	// Token: 0x04000678 RID: 1656
	[SerializeField]
	private float m_maxDashSpeedUpgraded = 13f;

	// Token: 0x04000679 RID: 1657
	[Tooltip("Maps yank speed to dash speed.\nX = Yank Speed (min to max)\nY = Dash Speed (min to max).")]
	[SerializeField]
	private AnimationCurve m_speedMappingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400067A RID: 1658
	[SerializeField]
	private float m_slipperySurfacesTime = 0.25f;

	// Token: 0x0400067B RID: 1659
	private float _maxInfluenceAngle;

	// Token: 0x0400067C RID: 1660
	[SerializeField]
	private float m_maxInfluenceAngleDefault = 10f;

	// Token: 0x0400067D RID: 1661
	[SerializeField]
	private float m_maxInfluenceAngleUpgrade = 15f;

	// Token: 0x0400067E RID: 1662
	private float _cooldownDuration;

	// Token: 0x0400067F RID: 1663
	[SerializeField]
	private float m_cooldownDurationDefault = 6f;

	// Token: 0x04000680 RID: 1664
	[SerializeField]
	private float m_cooldownDurationUpgrade = 5f;

	// Token: 0x04000681 RID: 1665
	private bool _hasStunUpgrade;

	// Token: 0x04000682 RID: 1666
	private bool _hasTagUpgrade;

	// Token: 0x04000683 RID: 1667
	private bool _isActivated;

	// Token: 0x04000684 RID: 1668
	private bool _wasActivated;

	// Token: 0x04000685 RID: 1669
	private float _timeLastThrown;

	// Token: 0x04000686 RID: 1670
	private float _successfulYankTime;

	// Token: 0x04000687 RID: 1671
	private float _maxEncounteredYankSpeed;

	// Token: 0x04000688 RID: 1672
	private Vector3 _yankBeginPos;

	// Token: 0x04000689 RID: 1673
	private bool _isRecheckingYank;

	// Token: 0x0400068A RID: 1674
	private VRRig _attachedVRRig;

	// Token: 0x0400068B RID: 1675
	private int _lastAttachedPlayerActorNr;

	// Token: 0x0400068C RID: 1676
	private int _attachedPlayerActorNr = int.MinValue;

	// Token: 0x0400068D RID: 1677
	private NetPlayer _attachedNetPlayer;

	// Token: 0x0400068E RID: 1678
	private bool _isTagged;

	// Token: 0x0400068F RID: 1679
	private readonly object[] _launchYoyoRPCArgs = new object[5];

	// Token: 0x04000690 RID: 1680
	private SIGadgetDashYoyo.EState _state;

	// Token: 0x020000E8 RID: 232
	[Serializable]
	public struct StateMaterialsInfo
	{
		// Token: 0x04000691 RID: 1681
		public Material idle;

		// Token: 0x04000692 RID: 1682
		public Material ready;

		// Token: 0x04000693 RID: 1683
		public Material cooldown;
	}

	// Token: 0x020000E9 RID: 233
	private enum EState
	{
		// Token: 0x04000695 RID: 1685
		Idle,
		// Token: 0x04000696 RID: 1686
		OnCooldown,
		// Token: 0x04000697 RID: 1687
		PreparedToThrow,
		// Token: 0x04000698 RID: 1688
		Thrown,
		// Token: 0x04000699 RID: 1689
		PreparedToDash,
		// Token: 0x0400069A RID: 1690
		DashUsed,
		// Token: 0x0400069B RID: 1691
		Count
	}
}
