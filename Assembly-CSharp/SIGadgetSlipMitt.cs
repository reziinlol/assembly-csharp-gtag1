using System;
using Drawing;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000F2 RID: 242
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetSlipMitt : SIGadget
{
	// Token: 0x17000061 RID: 97
	// (get) Token: 0x060005D1 RID: 1489 RVA: 0x00021A08 File Offset: 0x0001FC08
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

	// Token: 0x060005D2 RID: 1490 RVA: 0x00021A84 File Offset: 0x0001FC84
	private void Start()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
		foreach (AudioClip audioClip in this.m_clips)
		{
			if (audioClip)
			{
				audioClip.LoadAudioData();
			}
		}
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x00021B58 File Offset: 0x0001FD58
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

	// Token: 0x060005D4 RID: 1492 RVA: 0x00021C0C File Offset: 0x0001FE0C
	private void _HandleStartInteraction()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._attachedPlayerActorNr = this.gameEntity.AttachedPlayerActorNr;
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this._attachedPlayerActorNr, out gamePlayer))
		{
			return;
		}
		this._attachedVRRig = gamePlayer.rig;
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x00021C4E File Offset: 0x0001FE4E
	private void _HandleStopInteraction()
	{
		this._attachedPlayerActorNr = -1;
		this._attachedVRRig = null;
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		this.SetStateAuthority(SIGadgetSlipMitt.EState.Idle);
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x00021C74 File Offset: 0x0001FE74
	protected void FixedUpdate()
	{
		if ((!this.IsEquippedLocal() && !this.activatedLocally) || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._wasActivated = this._isActivated;
		this._isActivated = this._CheckInput();
		if (Time.unscaledTime < this._airGrabTime + this.m_slipperySurfacesTime)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
		}
		SIGadgetSlipMitt.EState state = this._state;
		if (state != SIGadgetSlipMitt.EState.Idle)
		{
			if (state != SIGadgetSlipMitt.EState.Slip)
			{
				return;
			}
			if (!this._isActivated)
			{
				this.SetStateAuthority(SIGadgetSlipMitt.EState.Idle);
				GTPlayer.Instance.UnsetGravityOverride(this);
				return;
			}
			this._airReleaseSpeed = 0f;
			if (this._HandIndex == 0)
			{
				GTPlayer.Instance.SetLeftMaximumSlipThisFrame();
				this._attachedHandState = GTPlayer.Instance.LeftHand;
				return;
			}
			GTPlayer.Instance.SetRightMaximumSlipThisFrame();
			this._attachedHandState = GTPlayer.Instance.RightHand;
		}
		else if (this._isActivated && !base.IsBlocked(SIExclusionType.AffectsLocalMovement))
		{
			this._PlayHaptic(0.1f);
			GTPlayer.Instance.SetGravityOverride(this, new Action<GTPlayer>(this._HandleGTPlayerOnUpdateGravity));
			this.SetStateAuthority(SIGadgetSlipMitt.EState.Slip);
			return;
		}
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x00021D84 File Offset: 0x0001FF84
	private void _HandleGTPlayerOnUpdateGravity(GTPlayer gtPlayer)
	{
		Transform handFollower = this._attachedHandState.handFollower;
		Ray ray = new Ray(handFollower.position, handFollower.forward);
		int value = gtPlayer.locomotionEnabledLayers.value;
		float maxDistance = 1f;
		float d = 20f;
		int num = Physics.RaycastNonAlloc(ray, this._raycastHitResults, maxDistance, value, QueryTriggerInteraction.Ignore);
		RaycastHit[] raycastHitResults = this._raycastHitResults;
		Vector3 gravity = Physics.gravity;
		Vector3 vector = ray.direction * d;
		Vector3 a = (num > 0) ? vector : gravity;
		Draw.ingame.Arrow(ray.origin, ray.origin + ray.direction);
		gtPlayer.AddForce(a * gtPlayer.scale, ForceMode.Acceleration);
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x00021E4C File Offset: 0x0002004C
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetSlipMitt.EState estate = (SIGadgetSlipMitt.EState)this.gameEntity.GetState();
		if (estate != this._state)
		{
			this._SetStateShared(estate);
		}
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x00021E7D File Offset: 0x0002007D
	private static bool _CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 3L;
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x00021E8B File Offset: 0x0002008B
	private void SetStateAuthority(SIGadgetSlipMitt.EState newState)
	{
		this._SetStateShared(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x00021EAC File Offset: 0x000200AC
	private void _SetStateShared(SIGadgetSlipMitt.EState newState)
	{
		if (newState == this._state || !SIGadgetSlipMitt._CanChangeState((long)newState))
		{
			return;
		}
		this._state = newState;
		SIGadgetSlipMitt.EState state = this._state;
		if (state != SIGadgetSlipMitt.EState.Idle)
		{
		}
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x00021EE4 File Offset: 0x000200E4
	private bool _CheckInput()
	{
		float sensitivity = this._wasActivated ? this.m_inputDeactivateThreshold : this.m_inputActivateThreshold;
		return this.m_buttonActivatable.CheckInput(sensitivity);
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00021F14 File Offset: 0x00020114
	private void _DoAirGrab()
	{
		float magnitude = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex).magnitude;
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00021F3C File Offset: 0x0002013C
	private void _DoDash()
	{
		this._airGrabTime = Time.unscaledTime;
		Vector3 handVelocity = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex);
		float num = this._CalculateDashSpeed(handVelocity.magnitude);
		GTPlayer instance = GTPlayer.Instance;
		instance.SetMaximumSlipThisFrame();
		instance.SetVelocity(handVelocity.normalized * -num);
		this._PlayHaptic(2f);
		this.SetStateAuthority(SIGadgetSlipMitt.EState.DashUsed);
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00021FA8 File Offset: 0x000201A8
	private float _CalculateDashSpeed(float currentYankSpeed)
	{
		float time = Mathf.InverseLerp(this.m_yankMinSpeed, this.m_yankMaxSpeed, currentYankSpeed);
		float t = this.m_speedMappingCurve.Evaluate(time);
		return Mathf.Lerp(this.m_minDashSpeed, this._maxDashSpeed, t);
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00021FE8 File Offset: 0x000201E8
	private void _PlayHaptic(float strengthMultiplier)
	{
		bool forLeftController;
		if (base.FindAttachedHand(out forLeftController))
		{
			GorillaTagger.Instance.StartVibration(forLeftController, GorillaTagger.Instance.tapHapticStrength * strengthMultiplier, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x00022020 File Offset: 0x00020220
	private void _PlayAudio(int index)
	{
		this.m_audioSource.clip = this.m_clips[index];
		this.m_audioSource.volume = this.m_clipVolumes[index];
		this.m_audioSource.GTPlay();
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x00022053 File Offset: 0x00020253
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._maxDashSpeed = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Speed) ? this.m_maxDashSpeedUpgraded : this.m_maxDashSpeedDefault);
	}

	// Token: 0x04000729 RID: 1833
	private const string preLog = "[SIGadgetSlipMitt]  ";

	// Token: 0x0400072A RID: 1834
	private const string preErr = "[SIGadgetSlipMitt]  ERROR!!!  ";

	// Token: 0x0400072B RID: 1835
	[SerializeField]
	private GameSnappable m_snappable;

	// Token: 0x0400072C RID: 1836
	[SerializeField]
	private Transform m_yoyoDefaultPosXform;

	// Token: 0x0400072D RID: 1837
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x0400072E RID: 1838
	[SerializeField]
	private float m_inputActivateThreshold = 0.35f;

	// Token: 0x0400072F RID: 1839
	[SerializeField]
	private float m_inputDeactivateThreshold = 0.25f;

	// Token: 0x04000730 RID: 1840
	[SerializeField]
	private MeshRenderer m_yoyoRenderer;

	// Token: 0x04000731 RID: 1841
	[SerializeField]
	private AudioSource m_audioSource;

	// Token: 0x04000732 RID: 1842
	[SerializeField]
	public AudioClip[] m_clips;

	// Token: 0x04000733 RID: 1843
	[SerializeField]
	public float[] m_clipVolumes;

	// Token: 0x04000734 RID: 1844
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMinSpeed = 2f;

	// Token: 0x04000735 RID: 1845
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMaxSpeed = 8f;

	// Token: 0x04000736 RID: 1846
	[Tooltip("Dash min/max speed: The fastest speed the player will move")]
	[SerializeField]
	private float m_minDashSpeed = 4f;

	// Token: 0x04000737 RID: 1847
	private float _maxDashSpeed;

	// Token: 0x04000738 RID: 1848
	[SerializeField]
	private float m_maxDashSpeedDefault = 11f;

	// Token: 0x04000739 RID: 1849
	[SerializeField]
	private float m_maxDashSpeedUpgraded = 13f;

	// Token: 0x0400073A RID: 1850
	[Tooltip("Maps yank speed to dash speed.\nX = Yank Speed (min to max)\nY = Dash Speed (min to max).")]
	[SerializeField]
	private AnimationCurve m_speedMappingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400073B RID: 1851
	[SerializeField]
	private float m_slipperySurfacesTime = 0.25f;

	// Token: 0x0400073C RID: 1852
	[SerializeField]
	private float m_maxInfluenceAngleDefault = 10f;

	// Token: 0x0400073D RID: 1853
	[SerializeField]
	private float m_maxInfluenceAngleUpgrade = 15f;

	// Token: 0x0400073E RID: 1854
	[SerializeField]
	private float m_cooldownDurationDefault = 6f;

	// Token: 0x0400073F RID: 1855
	[SerializeField]
	private float m_cooldownDurationUpgrade = 5f;

	// Token: 0x04000740 RID: 1856
	[SerializeField]
	private Transform m_airGrabXform;

	// Token: 0x04000741 RID: 1857
	private bool _isActivated;

	// Token: 0x04000742 RID: 1858
	private bool _wasActivated;

	// Token: 0x04000743 RID: 1859
	private float _airGrabTime;

	// Token: 0x04000744 RID: 1860
	private float _airReleaseSpeed;

	// Token: 0x04000745 RID: 1861
	private Vector3 _airReleaseVector;

	// Token: 0x04000746 RID: 1862
	private VRRig _attachedVRRig;

	// Token: 0x04000747 RID: 1863
	private GTPlayer.HandState _attachedHandState;

	// Token: 0x04000748 RID: 1864
	private int _lastAttachedPlayerActorNr;

	// Token: 0x04000749 RID: 1865
	private int _attachedPlayerActorNr = int.MinValue;

	// Token: 0x0400074A RID: 1866
	private bool _isTagged;

	// Token: 0x0400074B RID: 1867
	private SIGadgetSlipMitt.EState _state;

	// Token: 0x0400074C RID: 1868
	private RaycastHit[] _raycastHitResults = new RaycastHit[1];

	// Token: 0x020000F3 RID: 243
	private enum EState
	{
		// Token: 0x0400074E RID: 1870
		Idle,
		// Token: 0x0400074F RID: 1871
		Slip,
		// Token: 0x04000750 RID: 1872
		DashUsed,
		// Token: 0x04000751 RID: 1873
		Count
	}
}
