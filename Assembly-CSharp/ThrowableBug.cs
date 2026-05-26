using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000DD7 RID: 3543
public class ThrowableBug : TransferrableObject, ITickSystemTick
{
	// Token: 0x17000827 RID: 2087
	// (get) Token: 0x060056B7 RID: 22199 RVA: 0x001C1BC6 File Offset: 0x001BFDC6
	// (set) Token: 0x060056B8 RID: 22200 RVA: 0x001C1BCE File Offset: 0x001BFDCE
	public bool TickRunning { get; set; }

	// Token: 0x060056B9 RID: 22201 RVA: 0x001C1BD8 File Offset: 0x001BFDD8
	protected override void Start()
	{
		base.Start();
		float f = Random.Range(0f, 6.2831855f);
		this.targetVelocity = new Vector3(Mathf.Sin(f) * this.maxNaturalSpeed, 0f, Mathf.Cos(f) * this.maxNaturalSpeed);
		this.currentState = TransferrableObject.PositionState.Dropped;
		this.rayCastNonAllocColliders = new RaycastHit[5];
		this.rayCastNonAllocColliders2 = new RaycastHit[5];
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
		this.currentZone = this.startZone;
	}

	// Token: 0x060056BA RID: 22202 RVA: 0x001C1C60 File Offset: 0x001BFE60
	internal override void OnEnable()
	{
		base.OnEnable();
		ThrowableBugBeacon.OnCall += this.ThrowableBugBeacon_OnCall;
		ThrowableBugBeacon.OnDismiss += this.ThrowableBugBeacon_OnDismiss;
		ThrowableBugBeacon.OnLock += this.ThrowableBugBeacon_OnLock;
		ThrowableBugBeacon.OnUnlock += this.ThrowableBugBeacon_OnUnlock;
		ThrowableBugBeacon.OnChangeSpeedMultiplier += this.ThrowableBugBeacon_OnChangeSpeedMultiplier;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060056BB RID: 22203 RVA: 0x001C1CD0 File Offset: 0x001BFED0
	internal override void OnDisable()
	{
		base.OnDisable();
		ThrowableBugBeacon.OnCall -= this.ThrowableBugBeacon_OnCall;
		ThrowableBugBeacon.OnDismiss -= this.ThrowableBugBeacon_OnDismiss;
		ThrowableBugBeacon.OnLock -= this.ThrowableBugBeacon_OnLock;
		ThrowableBugBeacon.OnUnlock -= this.ThrowableBugBeacon_OnUnlock;
		ThrowableBugBeacon.OnChangeSpeedMultiplier -= this.ThrowableBugBeacon_OnChangeSpeedMultiplier;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x060056BC RID: 22204 RVA: 0x001C1D40 File Offset: 0x001BFF40
	private bool isValid(ThrowableBugBeacon tbb)
	{
		return tbb.BugName == this.bugName && (tbb.Range <= 0f || Vector3.Distance(tbb.transform.position, base.transform.position) <= tbb.Range);
	}

	// Token: 0x060056BD RID: 22205 RVA: 0x001C1D92 File Offset: 0x001BFF92
	private void ThrowableBugBeacon_OnCall(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = tbb.transform.position - base.transform.position;
		}
	}

	// Token: 0x060056BE RID: 22206 RVA: 0x001C1DC4 File Offset: 0x001BFFC4
	private void ThrowableBugBeacon_OnLock(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = tbb.transform.position - base.transform.position;
			this.lockedTarget = tbb.transform;
			this.locked = true;
		}
	}

	// Token: 0x060056BF RID: 22207 RVA: 0x001C1E13 File Offset: 0x001C0013
	private void ThrowableBugBeacon_OnDismiss(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = base.transform.position - tbb.transform.position;
			this.locked = false;
		}
	}

	// Token: 0x060056C0 RID: 22208 RVA: 0x001C1E4B File Offset: 0x001C004B
	private void ThrowableBugBeacon_OnUnlock(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.locked = false;
		}
	}

	// Token: 0x060056C1 RID: 22209 RVA: 0x001C1E5D File Offset: 0x001C005D
	private void ThrowableBugBeacon_OnChangeSpeedMultiplier(ThrowableBugBeacon tbb, float f)
	{
		if (this.isValid(tbb))
		{
			this.speedMultiplier = f;
		}
	}

	// Token: 0x060056C2 RID: 22210 RVA: 0x00023994 File Offset: 0x00021B94
	public override bool ShouldBeKinematic()
	{
		return true;
	}

	// Token: 0x060056C3 RID: 22211 RVA: 0x001C1E70 File Offset: 0x001C0070
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.raycastFrameCounter = (this.raycastFrameCounter + 1) % this.raycastFramePeriod;
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
		if (this.animator.enabled)
		{
			this.animator.SetBool(ThrowableBug._g_IsHeld, flag);
		}
		this.animator.enabled = (GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone == this.currentZone);
		if (!this.audioSource)
		{
			return;
		}
		switch (this.currentAudioState)
		{
		case ThrowableBug.AudioState.JustGrabbed:
			if (!flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustReleased;
				return;
			}
			if (this.grabBugAudioClip && this.audioSource.clip != this.grabBugAudioClip)
			{
				this.audioSource.clip = this.grabBugAudioClip;
				this.audioSource.time = 0f;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			else if (!this.audioSource.isPlaying)
			{
				this.currentAudioState = ThrowableBug.AudioState.ContinuallyGrabbed;
				return;
			}
			break;
		case ThrowableBug.AudioState.ContinuallyGrabbed:
			if (!flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustReleased;
				return;
			}
			break;
		case ThrowableBug.AudioState.JustReleased:
			if (!flag)
			{
				if (this.releaseBugAudioClip && this.audioSource.clip != this.releaseBugAudioClip)
				{
					this.audioSource.clip = this.releaseBugAudioClip;
					this.audioSource.time = 0f;
					if (this.audioSource.isActiveAndEnabled)
					{
						this.audioSource.GTPlay();
						return;
					}
				}
				else if (!this.audioSource.isPlaying)
				{
					this.currentAudioState = ThrowableBug.AudioState.NotHeld;
					return;
				}
			}
			else
			{
				this.currentAudioState = ThrowableBug.AudioState.JustGrabbed;
			}
			break;
		case ThrowableBug.AudioState.NotHeld:
			if (flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustGrabbed;
				return;
			}
			if (this.flyingBugAudioClip && !this.audioSource.isPlaying)
			{
				this.audioSource.clip = this.flyingBugAudioClip;
				this.audioSource.time = 0f;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060056C4 RID: 22212 RVA: 0x001C2098 File Offset: 0x001C0298
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!this.reliableState)
		{
			return;
		}
		if ((this.currentState & TransferrableObject.PositionState.Dropped) == TransferrableObject.PositionState.None)
		{
			return;
		}
		if (this.locked && Vector3.Distance(this.lockedTarget.position, base.transform.position) > 0.1f)
		{
			this.reliableState.travelingDirection = this.lockedTarget.position - base.transform.position;
		}
		if (this.slowingDownProgress < 1f)
		{
			this.slowingDownProgress += this.slowdownAcceleration * Time.deltaTime;
			this.reliableState.travelingDirection = Vector3.Slerp(this.thrownVeloicity, this.targetVelocity, Mathf.SmoothStep(0f, 1f, this.slowingDownProgress));
		}
		else
		{
			this.reliableState.travelingDirection = this.reliableState.travelingDirection.normalized * this.maxNaturalSpeed;
		}
		this.bobingFrequency = (this.shouldRandomizeFrequency ? this.RandomizeBobingFrequency() : this.bobbingDefaultFrequency);
		float num = this.bobingState + this.bobingSpeed * Time.deltaTime;
		float num2 = Mathf.Sin(num / this.bobingFrequency) - Mathf.Sin(this.bobingState / this.bobingFrequency);
		Vector3 vector = Vector3.up * (num2 * this.bobMagnintude);
		this.bobingState = num;
		if (this.bobingState > 6.2831855f)
		{
			this.bobingState -= 6.2831855f;
		}
		vector += this.reliableState.travelingDirection * Time.deltaTime;
		float maxDistance = this.isTooHighTravelingDown ? this.minimumHeightOffOfTheGroundBeforeStoppingDescent : this.maximumHeightOffOfTheGroundBeforeStartingDescent;
		float num3 = this.isTooLowTravelingUp ? this.maximumHeightOffOfTheGroundBeforeStoppingAscent : this.minimumHeightOffOfTheGroundBeforeStartingAscent;
		if (this.raycastFrameCounter == 0)
		{
			if (Physics.RaycastNonAlloc(base.transform.position, Vector3.down, this.rayCastNonAllocColliders2, maxDistance, this.collisionCheckMask) > 0)
			{
				this.isTooHighTravelingDown = false;
				if (this.descentSlerp > 0f)
				{
					this.descentSlerp = Mathf.Clamp01(this.descentSlerp - this.descentSlerpRate * Time.deltaTime);
				}
				RaycastHit raycastHit = this.rayCastNonAllocColliders2[0];
				this.isTooLowTravelingUp = (raycastHit.distance < num3);
				if (this.isTooLowTravelingUp)
				{
					if (this.ascentSlerp < 1f)
					{
						this.ascentSlerp = Mathf.Clamp01(this.ascentSlerp + this.ascentSlerpRate * Time.deltaTime);
					}
				}
				else if (this.ascentSlerp > 0f)
				{
					this.ascentSlerp = Mathf.Clamp01(this.ascentSlerp - this.ascentSlerpRate * Time.deltaTime);
				}
			}
			else
			{
				this.isTooHighTravelingDown = true;
				if (this.descentSlerp < 1f)
				{
					this.descentSlerp = Mathf.Clamp01(this.descentSlerp + this.descentSlerpRate * Time.deltaTime);
				}
			}
		}
		vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, this.descentSlerp) * this.descentRate * Vector3.down;
		vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, this.ascentSlerp) * this.ascentRate * Vector3.up;
		float num4;
		Vector3 axis;
		Quaternion.FromToRotation(base.transform.rotation * Vector3.up, Quaternion.identity * Vector3.up).ToAngleAxis(out num4, out axis);
		Quaternion quaternion = Quaternion.AngleAxis(num4 * 0.02f, axis);
		float num5;
		Vector3 axis2;
		Quaternion.FromToRotation(base.transform.rotation * Vector3.forward, this.reliableState.travelingDirection.normalized).ToAngleAxis(out num5, out axis2);
		Quaternion lhs = Quaternion.AngleAxis(num5 * 0.005f, axis2);
		quaternion = lhs * quaternion;
		vector = quaternion * quaternion * quaternion * quaternion * vector;
		vector *= this.speedMultiplier;
		this.speedMultiplier = Mathf.MoveTowards(this.speedMultiplier, 1f, Time.deltaTime);
		if (this.raycastFrameCounter == 0)
		{
			if (Physics.SphereCastNonAlloc(base.transform.position, this.collisionHitRadius, vector.normalized, this.rayCastNonAllocColliders, vector.magnitude, this.collisionCheckMask) > 0)
			{
				Vector3 normal = this.rayCastNonAllocColliders[0].normal;
				this.reliableState.travelingDirection = Vector3.Reflect(this.reliableState.travelingDirection, normal).x0z();
				base.transform.position += Vector3.Reflect(vector, normal);
				this.thrownVeloicity = Vector3.Reflect(this.thrownVeloicity, normal);
				this.targetVelocity = Vector3.Reflect(this.targetVelocity, normal).x0z();
			}
			else
			{
				base.transform.position += vector;
			}
		}
		else
		{
			base.transform.position += vector;
		}
		this.bugRotationalVelocity = quaternion * this.bugRotationalVelocity;
		float num6;
		Vector3 axis3;
		this.bugRotationalVelocity.ToAngleAxis(out num6, out axis3);
		this.bugRotationalVelocity = Quaternion.AngleAxis(num6 * 0.9f, axis3);
		base.transform.rotation = this.bugRotationalVelocity * base.transform.rotation;
	}

	// Token: 0x060056C5 RID: 22213 RVA: 0x001C261D File Offset: 0x001C081D
	private float RandomizeBobingFrequency()
	{
		return Random.Range(this.minRandFrequency, this.maxRandFrequency);
	}

	// Token: 0x060056C6 RID: 22214 RVA: 0x001C2630 File Offset: 0x001C0830
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.slowingDownProgress = 0f;
		Vector3 linearVelocity = this.velocityEstimator.linearVelocity;
		this.thrownVeloicity = linearVelocity;
		this.reliableState.travelingDirection = linearVelocity;
		this.bugRotationalVelocity = Quaternion.Euler(this.velocityEstimator.angularVelocity);
		this.startingSpeed = linearVelocity.magnitude;
		Vector3 normalized = this.reliableState.travelingDirection.x0z().normalized;
		this.targetVelocity = normalized * this.maxNaturalSpeed;
		return true;
	}

	// Token: 0x060056C7 RID: 22215 RVA: 0x001C26C2 File Offset: 0x001C08C2
	public void OnCollisionEnter(Collision collision)
	{
		this.reliableState.travelingDirection *= -1f;
	}

	// Token: 0x060056C8 RID: 22216 RVA: 0x001C26E0 File Offset: 0x001C08E0
	public void Tick()
	{
		if (this.updateMultiplier > 0)
		{
			for (int i = 0; i < this.updateMultiplier; i++)
			{
				this.LateUpdateLocal();
			}
		}
	}

	// Token: 0x040066A6 RID: 26278
	public ThrowableBugReliableState reliableState;

	// Token: 0x040066A7 RID: 26279
	public float slowingDownProgress;

	// Token: 0x040066A8 RID: 26280
	public float startingSpeed;

	// Token: 0x040066A9 RID: 26281
	public int raycastFramePeriod = 5;

	// Token: 0x040066AA RID: 26282
	private int raycastFrameCounter;

	// Token: 0x040066AB RID: 26283
	public float bobingSpeed = 1f;

	// Token: 0x040066AC RID: 26284
	public float bobMagnintude = 0.1f;

	// Token: 0x040066AD RID: 26285
	public bool shouldRandomizeFrequency;

	// Token: 0x040066AE RID: 26286
	public float minRandFrequency = 0.008f;

	// Token: 0x040066AF RID: 26287
	public float maxRandFrequency = 1f;

	// Token: 0x040066B0 RID: 26288
	public float bobingFrequency = 1f;

	// Token: 0x040066B1 RID: 26289
	public float bobingState;

	// Token: 0x040066B2 RID: 26290
	public float thrownYVelocity;

	// Token: 0x040066B3 RID: 26291
	public float collisionHitRadius;

	// Token: 0x040066B4 RID: 26292
	public LayerMask collisionCheckMask;

	// Token: 0x040066B5 RID: 26293
	public Vector3 thrownVeloicity;

	// Token: 0x040066B6 RID: 26294
	public Vector3 targetVelocity;

	// Token: 0x040066B7 RID: 26295
	public Quaternion bugRotationalVelocity;

	// Token: 0x040066B8 RID: 26296
	private RaycastHit[] rayCastNonAllocColliders;

	// Token: 0x040066B9 RID: 26297
	private RaycastHit[] rayCastNonAllocColliders2;

	// Token: 0x040066BA RID: 26298
	public VRRig followingRig;

	// Token: 0x040066BB RID: 26299
	public bool isTooHighTravelingDown;

	// Token: 0x040066BC RID: 26300
	public float descentSlerp;

	// Token: 0x040066BD RID: 26301
	public float ascentSlerp;

	// Token: 0x040066BE RID: 26302
	public float maxNaturalSpeed;

	// Token: 0x040066BF RID: 26303
	public float slowdownAcceleration;

	// Token: 0x040066C0 RID: 26304
	public float maximumHeightOffOfTheGroundBeforeStartingDescent = 5f;

	// Token: 0x040066C1 RID: 26305
	public float minimumHeightOffOfTheGroundBeforeStoppingDescent = 3f;

	// Token: 0x040066C2 RID: 26306
	public float descentRate = 0.2f;

	// Token: 0x040066C3 RID: 26307
	public float descentSlerpRate = 0.2f;

	// Token: 0x040066C4 RID: 26308
	public float minimumHeightOffOfTheGroundBeforeStartingAscent = 0.5f;

	// Token: 0x040066C5 RID: 26309
	public float maximumHeightOffOfTheGroundBeforeStoppingAscent = 0.75f;

	// Token: 0x040066C6 RID: 26310
	public float ascentRate = 0.4f;

	// Token: 0x040066C7 RID: 26311
	public float ascentSlerpRate = 1f;

	// Token: 0x040066C8 RID: 26312
	private bool isTooLowTravelingUp;

	// Token: 0x040066C9 RID: 26313
	public Animator animator;

	// Token: 0x040066CA RID: 26314
	[FormerlySerializedAs("grabBugAudioSource")]
	public AudioClip grabBugAudioClip;

	// Token: 0x040066CB RID: 26315
	[FormerlySerializedAs("releaseBugAudioSource")]
	public AudioClip releaseBugAudioClip;

	// Token: 0x040066CC RID: 26316
	[FormerlySerializedAs("flyingBugAudioSource")]
	public AudioClip flyingBugAudioClip;

	// Token: 0x040066CD RID: 26317
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040066CE RID: 26318
	public GTZone startZone;

	// Token: 0x040066CF RID: 26319
	private GTZone currentZone;

	// Token: 0x040066D0 RID: 26320
	private float bobbingDefaultFrequency = 1f;

	// Token: 0x040066D1 RID: 26321
	public int updateMultiplier;

	// Token: 0x040066D2 RID: 26322
	private ThrowableBug.AudioState currentAudioState;

	// Token: 0x040066D3 RID: 26323
	private float speedMultiplier = 1f;

	// Token: 0x040066D4 RID: 26324
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040066D6 RID: 26326
	[SerializeField]
	private ThrowableBug.BugName bugName;

	// Token: 0x040066D7 RID: 26327
	private Transform lockedTarget;

	// Token: 0x040066D8 RID: 26328
	private bool locked;

	// Token: 0x040066D9 RID: 26329
	private static readonly int _g_IsHeld = Animator.StringToHash("isHeld");

	// Token: 0x02000DD8 RID: 3544
	public enum BugName
	{
		// Token: 0x040066DB RID: 26331
		NONE,
		// Token: 0x040066DC RID: 26332
		DougTheBug,
		// Token: 0x040066DD RID: 26333
		MattTheBat
	}

	// Token: 0x02000DD9 RID: 3545
	private enum AudioState
	{
		// Token: 0x040066DF RID: 26335
		JustGrabbed,
		// Token: 0x040066E0 RID: 26336
		ContinuallyGrabbed,
		// Token: 0x040066E1 RID: 26337
		JustReleased,
		// Token: 0x040066E2 RID: 26338
		NotHeld
	}
}
