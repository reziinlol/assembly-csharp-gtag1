using System;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaTag.Cosmetics;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000228 RID: 552
public class RCHoverboard : RCVehicle
{
	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000E99 RID: 3737 RVA: 0x0004F59B File Offset: 0x0004D79B
	// (set) Token: 0x06000E9A RID: 3738 RVA: 0x0004F5A3 File Offset: 0x0004D7A3
	private float _MaxForwardSpeed
	{
		get
		{
			return this.m_maxForwardSpeed;
		}
		set
		{
			this.m_maxForwardSpeed = value;
			this._forwardAccel = value / math.max(0.01f, this.m_forwardAccelTime);
		}
	}

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06000E9B RID: 3739 RVA: 0x0004F5C4 File Offset: 0x0004D7C4
	// (set) Token: 0x06000E9C RID: 3740 RVA: 0x0004F5CC File Offset: 0x0004D7CC
	private float _MaxTurnRate
	{
		get
		{
			return this.m_maxTurnRate;
		}
		set
		{
			this.m_maxTurnRate = value;
			this._turnAccel = value / math.max(1E-06f, this.m_turnAccelTime);
		}
	}

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06000E9D RID: 3741 RVA: 0x0004F5ED File Offset: 0x0004D7ED
	// (set) Token: 0x06000E9E RID: 3742 RVA: 0x0004F5F5 File Offset: 0x0004D7F5
	private float _MaxTiltAngle
	{
		get
		{
			return this.m_maxTiltAngle;
		}
		set
		{
			this.m_maxTiltAngle = value;
			this._tiltAccel = value / math.max(1E-06f, this.m_tiltTime);
		}
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x0004F618 File Offset: 0x0004D818
	protected override void Awake()
	{
		base.Awake();
		this._hasAudioSource = (this.m_audioSource != null);
		this._hasHoverSound = (this.m_hoverSound != null);
		this._MaxForwardSpeed = this.m_maxForwardSpeed;
		this._MaxTurnRate = this.m_maxTurnRate;
		this._MaxTiltAngle = this.m_maxTiltAngle;
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x0004F674 File Offset: 0x0004D874
	protected override void AuthorityBeginDocked()
	{
		base.AuthorityBeginDocked();
		this._currentTurnRate = 0f;
		this._currentTiltAngle = 0f;
		float3 to = this._ProjectOnPlane(base.transform.forward, math.up());
		this._currentTurnAngle = this._SignedAngle(new float3(0f, 0f, 1f), to, new float3(0f, 1f, 0f));
		this._motorLevel = 0f;
		if (this._hasAudioSource)
		{
			this.m_audioSource.Stop();
			this.m_audioSource.volume = 0f;
		}
		if (this.connectedRemote == null)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x0004F738 File Offset: 0x0004D938
	protected override void AuthorityUpdate(float dt)
	{
		base.AuthorityUpdate(dt);
		if (this.localState == RCVehicle.State.Mobilized)
		{
			float x = math.length(this.activeInput.joystick);
			this._motorLevel = math.saturate(x);
			if (this.hasNetworkSync)
			{
				this.networkSync.syncedState.dataA = (byte)((uint)(this._motorLevel * 255f));
				return;
			}
		}
		else
		{
			this._motorLevel = 0f;
		}
	}

	// Token: 0x06000EA2 RID: 3746 RVA: 0x0004F7AC File Offset: 0x0004D9AC
	protected override void RemoteUpdate(float dt)
	{
		base.RemoteUpdate(dt);
		if (this.localState == RCVehicle.State.Mobilized && this.hasNetworkSync)
		{
			this._motorLevel = (float)this.networkSync.syncedState.dataA / 255f;
			return;
		}
		this._motorLevel = 0f;
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x0004F7FC File Offset: 0x0004D9FC
	protected override void SharedUpdate(float dt)
	{
		base.SharedUpdate(dt);
		switch (this.localState)
		{
		case RCVehicle.State.Disabled:
		case RCVehicle.State.DockedLeft:
		case RCVehicle.State.DockedRight:
		case RCVehicle.State.Crashed:
			break;
		case RCVehicle.State.Mobilized:
			if (this._hasAudioSource && this._hasHoverSound)
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.m_audioSource.volume = 0f;
					this.m_audioSource.clip = this.m_hoverSound;
					this.m_audioSource.loop = true;
					this.m_audioSource.GTPlay();
					return;
				}
				float target = math.lerp(this.m_hoverSoundVolumeMinMax.x, this.m_hoverSoundVolumeMinMax.y, this._motorLevel);
				float maxDelta = this.m_hoverSoundVolumeMinMax.y / this.m_hoverSoundVolumeRampTime * dt;
				this.m_audioSource.volume = this._MoveTowards(this.m_audioSource.volume, target, maxDelta);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x0004F8E0 File Offset: 0x0004DAE0
	protected void FixedUpdate()
	{
		if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
		{
			return;
		}
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = this.m_inputThrustForward.Get(this.activeInput) - this.m_inputThrustBack.Get(this.activeInput);
		float num2 = this.m_inputTurn.Get(this.activeInput);
		float num3 = this.m_inputJump.Get(this.activeInput);
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 10f, this.raycastLayers, QueryTriggerInteraction.Collide);
		bool flag2 = flag && raycastHit.distance <= this.m_hoverHeight + 0.1f;
		if (this.enableJumpInput && num3 > 0.001f && flag2 && !this._hasJumped)
		{
			this.rb.AddForce(Vector3.up * this.m_jumpForce, ForceMode.Impulse);
			this._hasJumped = true;
		}
		else if (num3 <= 0.001f)
		{
			this._hasJumped = false;
		}
		float target = num2 * this._MaxTurnRate;
		this._currentTurnRate = this._MoveTowards(this._currentTurnRate, target, this._turnAccel * fixedDeltaTime);
		this._currentTurnAngle += this._currentTurnRate * fixedDeltaTime;
		float target2 = math.lerp(-this.m_maxTiltAngle, this.m_maxTiltAngle, math.unlerp(-1f, 1f, num));
		this._currentTiltAngle = this._MoveTowards(this._currentTiltAngle, target2, this._tiltAccel * fixedDeltaTime);
		base.transform.rotation = quaternion.EulerXYZ(math.radians(new float3(this._currentTiltAngle, this._currentTurnAngle, 0f)));
		float3 @float = base.transform.forward;
		float num4 = math.dot(@float, this.rb.linearVelocity);
		float num5 = num * this.m_maxForwardSpeed;
		float rhs = (math.abs(num5) > 0.001f && ((num5 > 0f && num4 < num5) || (num5 < 0f && num4 > num5))) ? math.sign(num5) : 0f;
		this.rb.AddForce(@float * this._forwardAccel * rhs * this.rb.mass, ForceMode.Force);
		if (flag)
		{
			float num6 = math.saturate(this.m_hoverHeight - raycastHit.distance);
			float num7 = math.dot(this.rb.linearVelocity, Vector3.up);
			float rhs2 = num6 * this.m_hoverForce - num7 * this.m_hoverDamp;
			this.rb.AddForce(math.up() * rhs2, ForceMode.Force);
		}
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x0004FBAC File Offset: 0x0004DDAC
	protected void OnCollisionEnter(Collision collision)
	{
		GameObject gameObject = collision.collider.gameObject;
		bool flag = gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
		bool flag2 = gameObject.IsOnLayer(UnityLayer.GorillaHand);
		if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
		{
			Vector3 vector = Vector3.zero;
			if (flag2)
			{
				GorillaHandClimber component = gameObject.GetComponent<GorillaHandClimber>();
				if (component != null)
				{
					vector = GTPlayer.Instance.GetHandVelocityTracker(component.xrNode == XRNode.LeftHand).GetAverageVelocity(true, 0.15f, false);
				}
			}
			else if (collision.rigidbody != null)
			{
				vector = collision.rigidbody.linearVelocity;
			}
			if ((flag || vector.sqrMagnitude > 0.01f) && base.HasLocalAuthority)
			{
				this.AuthorityApplyImpact(vector, flag);
				if (this.networkSync != null)
				{
					this.networkSync.photonView.RPC("HitRCVehicleRPC", RpcTarget.Others, new object[]
					{
						vector,
						flag
					});
				}
			}
		}
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x0004FCA0 File Offset: 0x0004DEA0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float _MoveTowards(float current, float target, float maxDelta)
	{
		if (math.abs(target - current) > maxDelta)
		{
			return current + math.sign(target - current) * maxDelta;
		}
		return target;
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x0004FCBC File Offset: 0x0004DEBC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float _SignedAngle(float3 from, float3 to, float3 axis)
	{
		float3 x = math.normalize(from);
		float3 y = math.normalize(to);
		float x2 = math.acos(math.dot(x, y));
		float num = math.sign(math.dot(math.cross(x, y), axis));
		return math.degrees(x2) * num;
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x0004FCFD File Offset: 0x0004DEFD
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float3 _ProjectOnPlane(float3 vector, float3 planeNormal)
	{
		return vector - math.dot(vector, planeNormal) * planeNormal;
	}

	// Token: 0x0400117B RID: 4475
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputTurn = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.StickX, new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
		new Keyframe(0.1f, 0f, 0f, 1.25f, 0f, 0f),
		new Keyframe(0.9f, 1f, 1.25f, 0f, 0f, 0f),
		new Keyframe(1f, 1f, 0f, 0f, 0f, 0f)
	}));

	// Token: 0x0400117C RID: 4476
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputThrustForward = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.Trigger, AnimationCurves.EaseInCirc);

	// Token: 0x0400117D RID: 4477
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputThrustBack = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.StickBack, new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
		new Keyframe(0.9f, 0f, 0f, 9.9999f, 0.5825f, 0.3767f),
		new Keyframe(1f, 1f, 9.9999f, 1f, 0f, 0f)
	}));

	// Token: 0x0400117E RID: 4478
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputJump = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.PrimaryFaceButton, AnimationCurves.Linear);

	// Token: 0x0400117F RID: 4479
	[Tooltip("Desired hover height above ground from this transform's position.")]
	[SerializeField]
	private float m_hoverHeight = 0.2f;

	// Token: 0x04001180 RID: 4480
	[Tooltip("Upward force to maintain hover when below hoverHeight.")]
	[SerializeField]
	private float m_hoverForce = 200f;

	// Token: 0x04001181 RID: 4481
	[Tooltip("Damping factor to smooth out vertical movement.")]
	[SerializeField]
	private float m_hoverDamp = 5f;

	// Token: 0x04001182 RID: 4482
	[SerializeField]
	private LayerMask raycastLayers = -1;

	// Token: 0x04001183 RID: 4483
	[SerializeField]
	private bool enableJumpInput = true;

	// Token: 0x04001184 RID: 4484
	[Tooltip("Upward impulse force for jump.")]
	[SerializeField]
	private float m_jumpForce = 3.5f;

	// Token: 0x04001185 RID: 4485
	private bool _hasJumped;

	// Token: 0x04001186 RID: 4486
	[SerializeField]
	[HideInInspector]
	private float m_maxForwardSpeed = 6f;

	// Token: 0x04001187 RID: 4487
	[SerializeField]
	[Tooltip("Time (seconds) to reach max forward speed from zero.")]
	private float m_forwardAccelTime = 2f;

	// Token: 0x04001188 RID: 4488
	[SerializeField]
	[HideInInspector]
	private float m_maxTurnRate = 720f;

	// Token: 0x04001189 RID: 4489
	[Tooltip("Time (seconds) to reach max turning rate.")]
	[SerializeField]
	private float m_turnAccelTime = 0.75f;

	// Token: 0x0400118A RID: 4490
	[SerializeField]
	[HideInInspector]
	private float m_maxTiltAngle = 30f;

	// Token: 0x0400118B RID: 4491
	[Tooltip("Time (seconds) to reach max tilt angle.")]
	[SerializeField]
	private float m_tiltTime = 0.1f;

	// Token: 0x0400118C RID: 4492
	[Tooltip("Audio source for any motor or hover sound.")]
	[SerializeField]
	private AudioSource m_audioSource;

	// Token: 0x0400118D RID: 4493
	[Tooltip("Looping motor/hover sound clip.")]
	[SerializeField]
	private AudioClip m_hoverSound;

	// Token: 0x0400118E RID: 4494
	[Tooltip("Volume range for the hover sound (x = min, y = max).")]
	[SerializeField]
	private float2 m_hoverSoundVolumeMinMax = new float2(0.1f, 0.5f);

	// Token: 0x0400118F RID: 4495
	[Tooltip("Time it takes for the volume to reach max value.")]
	[SerializeField]
	private float m_hoverSoundVolumeRampTime = 1f;

	// Token: 0x04001190 RID: 4496
	private bool _hasAudioSource;

	// Token: 0x04001191 RID: 4497
	private bool _hasHoverSound;

	// Token: 0x04001192 RID: 4498
	private float _forwardAccel;

	// Token: 0x04001193 RID: 4499
	private float _turnAccel;

	// Token: 0x04001194 RID: 4500
	private float _tiltAccel;

	// Token: 0x04001195 RID: 4501
	private float _currentTurnRate;

	// Token: 0x04001196 RID: 4502
	private float _currentTurnAngle;

	// Token: 0x04001197 RID: 4503
	private float _currentTiltAngle;

	// Token: 0x04001198 RID: 4504
	private float _motorLevel;

	// Token: 0x02000229 RID: 553
	private enum _EInputSource
	{
		// Token: 0x0400119A RID: 4506
		None,
		// Token: 0x0400119B RID: 4507
		StickX,
		// Token: 0x0400119C RID: 4508
		StickForward,
		// Token: 0x0400119D RID: 4509
		StickBack,
		// Token: 0x0400119E RID: 4510
		Trigger,
		// Token: 0x0400119F RID: 4511
		PrimaryFaceButton
	}

	// Token: 0x0200022A RID: 554
	[Serializable]
	private struct _SingleInputOption
	{
		// Token: 0x06000EAA RID: 3754 RVA: 0x0004FF3E File Offset: 0x0004E13E
		public _SingleInputOption(RCHoverboard._EInputSource source, AnimationCurve remapCurve)
		{
			this.source = new GTOption<StringEnum<RCHoverboard._EInputSource>>(source);
			this.remapCurve = new GTOption<AnimationCurve>(remapCurve);
		}

		// Token: 0x06000EAB RID: 3755 RVA: 0x0004FF60 File Offset: 0x0004E160
		public float Get(RCRemoteHoldable.RCInput input)
		{
			float num;
			switch (this.source.ResolvedValue.Value)
			{
			case RCHoverboard._EInputSource.None:
				num = 0f;
				break;
			case RCHoverboard._EInputSource.StickX:
				num = input.joystick.x;
				break;
			case RCHoverboard._EInputSource.StickForward:
				num = math.saturate(input.joystick.y);
				break;
			case RCHoverboard._EInputSource.StickBack:
				num = math.saturate(-input.joystick.y);
				break;
			case RCHoverboard._EInputSource.Trigger:
				num = input.trigger;
				break;
			case RCHoverboard._EInputSource.PrimaryFaceButton:
				num = (float)input.buttons;
				break;
			default:
				num = 0f;
				break;
			}
			float x = num;
			return this.remapCurve.ResolvedValue.Evaluate(math.abs(x)) * math.sign(x);
		}

		// Token: 0x040011A0 RID: 4512
		public GTOption<StringEnum<RCHoverboard._EInputSource>> source;

		// Token: 0x040011A1 RID: 4513
		public GTOption<AnimationCurve> remapCurve;
	}
}
