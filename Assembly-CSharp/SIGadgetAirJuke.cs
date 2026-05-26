using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000EB RID: 235
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetAirJuke : SIGadget
{
	// Token: 0x1700005F RID: 95
	// (get) Token: 0x06000587 RID: 1415 RVA: 0x0001FA74 File Offset: 0x0001DC74
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

	// Token: 0x06000588 RID: 1416 RVA: 0x0001FAF0 File Offset: 0x0001DCF0
	private void Awake()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
		this._fxGObj = this.m_particleSystem.gameObject;
		this._fxXform = this.m_particleSystem.transform;
		this._fxMain = this.m_particleSystem.main;
		this._fxGObj.SetActive(false);
		this._fxEmission = this.m_particleSystem.emission;
		this._fxEmission.enabled = false;
		this._groundedUseCounter = new ResettableUseCounter(this.m_maxRegularUses, this.m_maxSuperchargeUses, new Action<bool>(this.OnRecharged));
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x0001FC18 File Offset: 0x0001DE18
	private void OnRecharged(bool recharged)
	{
		if (recharged)
		{
			this.rechargeAudio.Play();
		}
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x0001FC28 File Offset: 0x0001DE28
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
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x0001FCD9 File Offset: 0x0001DED9
	private void _HandleStartInteraction()
	{
		bool isQuitting = ApplicationQuittingState.IsQuitting;
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x0001FCE1 File Offset: 0x0001DEE1
	private void _HandleStopInteraction()
	{
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		if (this._state != SIGadgetAirJuke_EState.DashUsed)
		{
			this._SetStateAuthority(SIGadgetAirJuke_EState.Idle);
		}
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x0001FD04 File Offset: 0x0001DF04
	protected void FixedUpdate()
	{
		if ((!this.IsEquippedLocal() && !this.activatedLocally) || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._wasActivated = this._isActivated;
		this._isActivated = this._CheckInput();
		GTPlayer instance = GTPlayer.Instance;
		if (Time.unscaledTime < this._dashStartTime + this.m_slipperySurfacesTime)
		{
			instance.SetMaximumSlipThisFrame();
		}
		switch (this._state)
		{
		case SIGadgetAirJuke_EState.Idle:
			if (this._isActivated)
			{
				if (this._groundedUseCounter.IsReady)
				{
					this._PlayHaptic(0.1f);
					this._SetStateAuthority(SIGadgetAirJuke_EState.TriggerPressHold);
				}
			}
			else if ((instance.IsGroundedButt && !instance.bodyGroundIsSlippery) || SIGadgetAirJuke._IsHandGroundedSteerable(instance))
			{
				this._groundedUseCounter.Reset();
			}
			break;
		case SIGadgetAirJuke_EState.TriggerPressHold:
			if (!this._isActivated)
			{
				this._DoDash();
			}
			break;
		case SIGadgetAirJuke_EState.DashUsed:
			this._SetStateAuthority(SIGadgetAirJuke_EState.Idle);
			break;
		}
		this._OnUpdateShared();
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x0001FDE8 File Offset: 0x0001DFE8
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetAirJuke_EState sigadgetAirJuke_EState = (SIGadgetAirJuke_EState)this.gameEntity.GetState();
		if (sigadgetAirJuke_EState == SIGadgetAirJuke_EState.DashUsed && this._state != SIGadgetAirJuke_EState.DashUsed)
		{
			this._playingFxUntilTimestamp = Time.time + 0.75f;
			this._dashStartFxPos = this._fxXform.position;
			this.singleJukeAudio.Play();
		}
		this._TrySetStateShared(sigadgetAirJuke_EState);
		this._OnUpdateShared();
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x0001FE54 File Offset: 0x0001E054
	private void _OnUpdateShared()
	{
		if (this._state == SIGadgetAirJuke_EState.TriggerPressHold)
		{
			this._fxGObj.SetActive(true);
			this._fxEmission.enabled = true;
			this._fxMain.startColor = new ParticleSystem.MinMaxGradient(Color.gray3);
			this._UpdateFxRotation();
			return;
		}
		if (Time.time <= this._playingFxUntilTimestamp)
		{
			this._fxGObj.SetActive(true);
			this._fxEmission.enabled = (Vector3.Distance(this._fxXform.position, this._dashStartFxPos) < this.m_fxMaxDistance);
			this._fxMain.startColor = new ParticleSystem.MinMaxGradient(Color.white);
			this._UpdateFxRotation();
			return;
		}
		this._fxGObj.SetActive(false);
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x0001FF08 File Offset: 0x0001E108
	private void _UpdateFxRotation()
	{
		Vector3 vector = this._fxXform.rotation.eulerAngles * 0.017453292f;
		this._fxMain.startRotationX = new ParticleSystem.MinMaxCurve(vector.x);
		this._fxMain.startRotationY = new ParticleSystem.MinMaxCurve(vector.y);
		this._fxMain.startRotationZ = new ParticleSystem.MinMaxCurve(vector.z);
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x0001FF75 File Offset: 0x0001E175
	private void _SetStateAuthority(SIGadgetAirJuke_EState newState)
	{
		if (this._TrySetStateShared(newState))
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
		}
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x0001FF98 File Offset: 0x0001E198
	private bool _TrySetStateShared(SIGadgetAirJuke_EState newState)
	{
		long num = (long)newState;
		if (newState == this._state || num < 0L || num >= 3L)
		{
			return false;
		}
		if (newState == SIGadgetAirJuke_EState.DashUsed && this._state != SIGadgetAirJuke_EState.DashUsed)
		{
			this._playingFxUntilTimestamp = Time.time + 0.75f;
			this._dashStartFxPos = this._fxXform.position;
		}
		this._state = newState;
		return true;
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x0001FFF4 File Offset: 0x0001E1F4
	private bool _CheckInput()
	{
		float sensitivity = this._wasActivated ? this.m_inputDeactivateThreshold : this.m_inputActivateThreshold;
		return this.m_buttonActivatable.CheckInput(sensitivity);
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x00020024 File Offset: 0x0001E224
	private void _DoDash()
	{
		if (base.IsBlocked(SIExclusionType.AffectsLocalMovement))
		{
			this._SetStateAuthority(SIGadgetAirJuke_EState.Idle);
			return;
		}
		Vector3 handVelocity = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex);
		if (handVelocity.magnitude < this.m_handMinSpeed || !this._groundedUseCounter.TryUse())
		{
			this._SetStateAuthority(SIGadgetAirJuke_EState.Idle);
			return;
		}
		this._dashStartTime = Time.unscaledTime;
		float num = this._CalculateDashSpeed(handVelocity.magnitude);
		GTPlayer instance = GTPlayer.Instance;
		instance.SetMaximumSlipThisFrame();
		Vector3 normalized = handVelocity.normalized;
		instance.SetVelocity(normalized * -num);
		this._PlayHaptic(2f);
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		if (activeSuperInfectionManager != null && !activeSuperInfectionManager.IsSupercharged)
		{
			this.singleJukeAudio.Play();
		}
		else if (this._groundedUseCounter.IsReady)
		{
			this.reusableJukeAudio.Play();
		}
		else
		{
			this.finalJukeAudio.Play();
		}
		this._SetStateAuthority(SIGadgetAirJuke_EState.DashUsed);
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x0002010C File Offset: 0x0001E30C
	private float _CalculateDashSpeed(float currentYankSpeed)
	{
		float time = Mathf.InverseLerp(this.m_handMinSpeed, this.m_handMaxSpeed, currentYankSpeed);
		float t = this.m_speedMappingCurve.Evaluate(time);
		return Mathf.Lerp(this.m_minDashSpeed, this._maxDashSpeed, t);
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x0002014C File Offset: 0x0001E34C
	private void _PlayHaptic(float strengthMultiplier)
	{
		bool forLeftController;
		if (base.FindAttachedHand(out forLeftController))
		{
			GorillaTagger.Instance.StartVibration(forLeftController, GorillaTagger.Instance.tapHapticStrength * strengthMultiplier, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x00020184 File Offset: 0x0001E384
	private static bool _IsHandGroundedSteerable(GTPlayer player)
	{
		ref readonly GTPlayer.HandState leftHandRef = ref player.LeftHandRef;
		ref readonly GTPlayer.HandState rightHandRef = ref player.RightHandRef;
		return (leftHandRef.isColliding && !SIGadgetAirJuke._IsRechargeBlocked(leftHandRef.surfaceOverride)) || (rightHandRef.isColliding && !SIGadgetAirJuke._IsRechargeBlocked(rightHandRef.surfaceOverride)) || player.isClimbing || leftHandRef.isHolding || rightHandRef.isHolding;
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x000201E1 File Offset: 0x0001E3E1
	private static bool _IsRechargeBlocked(GorillaSurfaceOverride surface)
	{
		return surface != null && surface.extraVelMultiplier > 0.99f && surface.extraVelMultiplier < 1f;
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x00020208 File Offset: 0x0001E408
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._maxDashSpeed = (withUpgrades.Contains(SIUpgradeType.AirControl_AirJuke_Speed) ? this.m_maxDashSpeedUpgraded : this.m_maxDashSpeedDefault);
	}

	// Token: 0x0400069D RID: 1693
	private const string preLog = "[SIGadgetAirJuke]  ";

	// Token: 0x0400069E RID: 1694
	private const string preErr = "[SIGadgetAirJuke]  ERROR!!!  ";

	// Token: 0x0400069F RID: 1695
	[SerializeField]
	private GameSnappable m_snappable;

	// Token: 0x040006A0 RID: 1696
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x040006A1 RID: 1697
	[SerializeField]
	private float m_inputActivateThreshold = 0.35f;

	// Token: 0x040006A2 RID: 1698
	[SerializeField]
	private float m_inputDeactivateThreshold = 0.25f;

	// Token: 0x040006A3 RID: 1699
	[Tooltip("Hand min speed: How fast you have to be moving your hand for the dash to trigger.")]
	[SerializeField]
	private float m_handMinSpeed = 2f;

	// Token: 0x040006A4 RID: 1700
	[Tooltip("Hand move max speed: The fastest hand speed that will be registered.")]
	[SerializeField]
	private float m_handMaxSpeed = 8f;

	// Token: 0x040006A5 RID: 1701
	[Tooltip("Dash min/max speed: The fastest speed the player will move")]
	[SerializeField]
	private float m_minDashSpeed = 4f;

	// Token: 0x040006A6 RID: 1702
	private float _maxDashSpeed;

	// Token: 0x040006A7 RID: 1703
	[SerializeField]
	private float m_maxDashSpeedDefault = 5f;

	// Token: 0x040006A8 RID: 1704
	[SerializeField]
	private float m_maxDashSpeedUpgraded = 7f;

	// Token: 0x040006A9 RID: 1705
	[Tooltip("Maps yank speed to dash speed.\nX = Yank Speed (min to max)\nY = Dash Speed (min to max).")]
	[SerializeField]
	private AnimationCurve m_speedMappingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040006AA RID: 1706
	[SerializeField]
	private float m_slipperySurfacesTime = 0.25f;

	// Token: 0x040006AB RID: 1707
	[SerializeField]
	private float m_maxInfluenceAngleDefault = 10f;

	// Token: 0x040006AC RID: 1708
	[SerializeField]
	private float m_maxInfluenceAngleUpgrade = 15f;

	// Token: 0x040006AD RID: 1709
	[SerializeField]
	private int m_maxRegularUses = 1;

	// Token: 0x040006AE RID: 1710
	[SerializeField]
	private int m_maxSuperchargeUses = 2;

	// Token: 0x040006AF RID: 1711
	[SerializeField]
	private ParticleSystem m_particleSystem;

	// Token: 0x040006B0 RID: 1712
	[SerializeField]
	private SoundBankPlayer singleJukeAudio;

	// Token: 0x040006B1 RID: 1713
	[SerializeField]
	private SoundBankPlayer reusableJukeAudio;

	// Token: 0x040006B2 RID: 1714
	[SerializeField]
	private SoundBankPlayer finalJukeAudio;

	// Token: 0x040006B3 RID: 1715
	[SerializeField]
	private SoundBankPlayer rechargeAudio;

	// Token: 0x040006B4 RID: 1716
	private GameObject _fxGObj;

	// Token: 0x040006B5 RID: 1717
	private Transform _fxXform;

	// Token: 0x040006B6 RID: 1718
	private ParticleSystem.MainModule _fxMain;

	// Token: 0x040006B7 RID: 1719
	private ParticleSystem.EmissionModule _fxEmission;

	// Token: 0x040006B8 RID: 1720
	private bool _isActivated;

	// Token: 0x040006B9 RID: 1721
	private bool _wasActivated;

	// Token: 0x040006BA RID: 1722
	private float _dashStartTime;

	// Token: 0x040006BB RID: 1723
	private Vector3 _airReleaseVector;

	// Token: 0x040006BC RID: 1724
	private bool _isTagged;

	// Token: 0x040006BD RID: 1725
	private SIGadgetAirJuke_EState _state;

	// Token: 0x040006BE RID: 1726
	private ResettableUseCounter _groundedUseCounter;

	// Token: 0x040006BF RID: 1727
	private float _playingFxUntilTimestamp;

	// Token: 0x040006C0 RID: 1728
	private Vector3 _dashStartFxPos;

	// Token: 0x040006C1 RID: 1729
	[SerializeField]
	private float m_fxMaxDistance = 3f;
}
