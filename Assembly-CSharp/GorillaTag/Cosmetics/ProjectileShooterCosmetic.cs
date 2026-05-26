using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200129F RID: 4767
	public class ProjectileShooterCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06007754 RID: 30548 RVA: 0x00271EB4 File Offset: 0x002700B4
		private bool IsMovementShoot()
		{
			return this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold;
		}

		// Token: 0x06007755 RID: 30549 RVA: 0x00271EBF File Offset: 0x002700BF
		private bool IsRigDirection()
		{
			return this.shootDirectionType == ProjectileShooterCosmetic.ShootDirection.LineFromRigToLaunchTransform;
		}

		// Token: 0x17000B82 RID: 2946
		// (get) Token: 0x06007756 RID: 30550 RVA: 0x00271ECA File Offset: 0x002700CA
		// (set) Token: 0x06007757 RID: 30551 RVA: 0x00271ED2 File Offset: 0x002700D2
		public bool shootingAllowed { get; set; } = true;

		// Token: 0x17000B83 RID: 2947
		// (get) Token: 0x06007758 RID: 30552 RVA: 0x00271EDB File Offset: 0x002700DB
		private bool IsCoolingDown
		{
			get
			{
				return this.cooldownRemaining > 0f;
			}
		}

		// Token: 0x06007759 RID: 30553 RVA: 0x00271EEC File Offset: 0x002700EC
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
			this.rig = ((this.transferrableObject == null) ? base.GetComponentInParent<VRRig>() : this.transferrableObject.ownerRig);
			UnityEvent<int> unityEvent = this.onMovedToNextStep;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.currentStep);
			}
			this.isLocal = ((this.transferrableObject != null && this.transferrableObject.IsMyItem()) || (this.rig != null && this.rig == GorillaTagger.Instance.offlineVRRig));
		}

		// Token: 0x17000B84 RID: 2948
		// (get) Token: 0x0600775A RID: 30554 RVA: 0x00271F8D File Offset: 0x0027018D
		// (set) Token: 0x0600775B RID: 30555 RVA: 0x00271F95 File Offset: 0x00270195
		public bool TickRunning { get; set; }

		// Token: 0x0600775C RID: 30556 RVA: 0x00271FA0 File Offset: 0x002701A0
		public void Tick()
		{
			if (this.IsCoolingDown)
			{
				this.cooldownRemaining -= Time.deltaTime;
				if (this.cooldownRemaining <= 0f)
				{
					this.cooldownRemaining = 0f;
					UnityEvent unityEvent = this.onCooldownFinished;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					if (this.isPressed)
					{
						this.SetPressState(true);
					}
					if (!this.allowCharging && this.shootActivatorType != ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
					{
						TickSystem<object>.RemoveTickCallback(this);
					}
				}
			}
			if (!this.IsCoolingDown && this.allowCharging)
			{
				if (this.isPressed)
				{
					if (this.chargeTime < this.maxChargeSeconds)
					{
						this.chargeTime += Time.deltaTime;
						if (this.chargeTime >= this.maxChargeSeconds || this.chargeTime >= this.snapToMaxChargeAt)
						{
							this.chargeTime = this.maxChargeSeconds;
							UnityEvent unityEvent2 = this.onMaxCharge;
							if (unityEvent2 != null)
							{
								unityEvent2.Invoke();
							}
						}
					}
					float chargeFrac = this.GetChargeFrac();
					ContinuousPropertyArray continuousPropertyArray = this.continuousChargingProperties;
					if (continuousPropertyArray != null)
					{
						continuousPropertyArray.ApplyAll(chargeFrac);
					}
					UnityEvent<float> unityEvent3 = this.whileCharging;
					if (unityEvent3 != null)
					{
						unityEvent3.Invoke(chargeFrac);
					}
					this.TryRunHaptics((chargeFrac >= 1f) ? this.maxChargeHapticsIntensity : (chargeFrac * this.chargeHapticsIntensity), Time.deltaTime);
					this.lastStep = this.currentStep;
					this.currentStep = Mathf.Clamp(Mathf.FloorToInt(chargeFrac * (float)this.numberOfProgressSteps), 0, this.numberOfProgressSteps - 1);
					if (this.currentStep >= 0 && this.currentStep != this.lastStep)
					{
						UnityEvent<int> unityEvent4 = this.onMovedToNextStep;
						if (unityEvent4 != null)
						{
							unityEvent4.Invoke(this.currentStep);
						}
						if (this.currentStep == this.numberOfProgressSteps - 1)
						{
							UnityEvent<int> unityEvent5 = this.onReachedLastProgressStep;
							if (unityEvent5 != null)
							{
								unityEvent5.Invoke(this.currentStep);
							}
						}
					}
					if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
					{
						Vector3 linearVelocity = this.velocityEstimator.linearVelocity;
						float num = linearVelocity.magnitude;
						float num2 = Vector3.Dot(linearVelocity / num, this.GetVectorFromBodyToLaunchPosition().normalized);
						num *= Mathf.Ceil(num2 - this.velocityEstimatorMinRigDotProduct);
						if (num >= this.velocityEstimatorStartGestureSpeed)
						{
							this.velocityEstimatorThresholdMet = true;
							return;
						}
						if (this.velocityEstimatorThresholdMet && num < this.velocityEstimatorStopGestureSpeed)
						{
							this.TryShoot();
							return;
						}
					}
				}
				else if (this.chargeTime > 0f)
				{
					this.chargeTime -= Time.deltaTime * this.chargeDecaySpeed;
					if (this.chargeTime <= 0f)
					{
						this.chargeTime = 0f;
						TickSystem<object>.RemoveTickCallback(this);
						ContinuousPropertyArray continuousPropertyArray2 = this.continuousChargingProperties;
						if (continuousPropertyArray2 != null)
						{
							continuousPropertyArray2.ApplyAll(0f);
						}
						UnityEvent<float> unityEvent6 = this.whileCharging;
						if (unityEvent6 == null)
						{
							return;
						}
						unityEvent6.Invoke(0f);
						return;
					}
					else
					{
						float chargeFrac2 = this.GetChargeFrac();
						ContinuousPropertyArray continuousPropertyArray3 = this.continuousChargingProperties;
						if (continuousPropertyArray3 != null)
						{
							continuousPropertyArray3.ApplyAll(chargeFrac2);
						}
						UnityEvent<float> unityEvent7 = this.whileCharging;
						if (unityEvent7 == null)
						{
							return;
						}
						unityEvent7.Invoke(chargeFrac2);
					}
				}
			}
		}

		// Token: 0x0600775D RID: 30557 RVA: 0x0027227D File Offset: 0x0027047D
		private Vector3 GetVectorFromBodyToLaunchPosition()
		{
			return this.shootFromTransform.position - this.rig.bodyTransform.TransformPoint(this.offsetRigPosition);
		}

		// Token: 0x0600775E RID: 30558 RVA: 0x002722A8 File Offset: 0x002704A8
		private void GetShootPositionAndRotation(out Vector3 position, out Quaternion rotation)
		{
			ProjectileShooterCosmetic.ShootDirection shootDirection = this.shootDirectionType;
			if (shootDirection != ProjectileShooterCosmetic.ShootDirection.LaunchTransformRotation && shootDirection == ProjectileShooterCosmetic.ShootDirection.LineFromRigToLaunchTransform)
			{
				position = this.shootFromTransform.position;
				rotation = Quaternion.LookRotation(position - this.rig.bodyTransform.TransformPoint(this.offsetRigPosition));
				return;
			}
			this.shootFromTransform.GetPositionAndRotation(out position, out rotation);
		}

		// Token: 0x0600775F RID: 30559 RVA: 0x00272310 File Offset: 0x00270510
		private void Shoot()
		{
			float chargeFrac = this.GetChargeFrac();
			float num = Mathf.Lerp(this.shootMinSpeed, this.shootMaxSpeed, this.chargeToShotSpeedCurve.Evaluate(chargeFrac));
			GameObject gameObject = ObjectPools.instance.Instantiate(this.projectilePrefab, true);
			gameObject.transform.localScale = Vector3.one * this.rig.scaleFactor;
			IProjectile component = gameObject.GetComponent<IProjectile>();
			if (component != null)
			{
				Vector3 vector;
				Quaternion quaternion;
				this.GetShootPositionAndRotation(out vector, out quaternion);
				Vector3 velocity = quaternion * Vector3.forward * (num * this.rig.scaleFactor);
				component.Launch(vector, quaternion, velocity, chargeFrac, this.rig, this.currentStep);
				if (this.projectileTrailPrefab != -1)
				{
					this.AttachTrail(this.projectileTrailPrefab, gameObject, vector, false, false);
				}
			}
			UnityEvent<float> unityEvent = this.onShoot;
			if (unityEvent != null)
			{
				unityEvent.Invoke(chargeFrac);
			}
			this.continuousChargingProperties.ApplyAll(0f);
			UnityEvent<float> unityEvent2 = this.whileCharging;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(0f);
			}
			if (this.isLocal)
			{
				UnityEvent<float> unityEvent3 = this.onShootLocal;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke(chargeFrac);
				}
			}
			if (this.allowCharging && this.runChargeCancelledEventOnShoot)
			{
				UnityEvent unityEvent4 = this.onChargeCancelled;
				if (unityEvent4 != null)
				{
					unityEvent4.Invoke();
				}
			}
			this.TryRunHaptics(chargeFrac * this.shootHapticsIntensity, this.shootHapticsDuration);
			this.SetPressState(false);
			this.cooldownRemaining = this.cooldownSeconds;
			this.chargeTime = 0f;
			this.currentStep = -1;
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06007760 RID: 30560 RVA: 0x0027249C File Offset: 0x0027069C
		private bool TryShoot()
		{
			if ((!this.IsCoolingDown && this.shootingAllowed && this.shootActivatorType != ProjectileShooterCosmetic.ShootActivator.ButtonReleasedFullCharge) || (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.ButtonReleasedFullCharge && this.chargeTime >= this.maxChargeSeconds))
			{
				this.Shoot();
				return true;
			}
			return false;
		}

		// Token: 0x06007761 RID: 30561 RVA: 0x002724D8 File Offset: 0x002706D8
		private void TryRunHaptics(float intensity, float duration)
		{
			if (!this.enableHaptics || !this.isLocal || intensity <= 0f)
			{
				return;
			}
			bool flag = this.transferrableObject != null && this.transferrableObject.InLeftHand();
			GorillaTagger.Instance.StartVibration(flag, intensity, duration);
			if (this.hapticsBothHands)
			{
				GorillaTagger.Instance.StartVibration(!flag, intensity, duration);
			}
		}

		// Token: 0x06007762 RID: 30562 RVA: 0x00272540 File Offset: 0x00270740
		private float GetChargeFrac()
		{
			if (!this.allowCharging)
			{
				return 1f;
			}
			if (this.chargeTime <= 0f)
			{
				return 0f;
			}
			if (this.chargeTime < this.maxChargeSeconds)
			{
				return this.chargeRateCurve.Evaluate(this.chargeTime / this.maxChargeSeconds);
			}
			return 1f;
		}

		// Token: 0x06007763 RID: 30563 RVA: 0x0027259A File Offset: 0x0027079A
		private void SetPressState(bool pressed)
		{
			this.isPressed = pressed;
			this.velocityEstimatorThresholdMet = false;
		}

		// Token: 0x06007764 RID: 30564 RVA: 0x002725AA File Offset: 0x002707AA
		public void OnButtonPressed()
		{
			this.SetPressState(true);
			if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.ButtonPressed)
			{
				this.TryShoot();
				return;
			}
			if (this.allowCharging || this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06007765 RID: 30565 RVA: 0x002725DC File Offset: 0x002707DC
		public void OnButtonReleased()
		{
			if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold && this.velocityEstimatorThresholdMet)
			{
				return;
			}
			ProjectileShooterCosmetic.ShootActivator shootActivator = this.shootActivatorType;
			if ((shootActivator != ProjectileShooterCosmetic.ShootActivator.ButtonReleased && shootActivator != ProjectileShooterCosmetic.ShootActivator.ButtonReleasedFullCharge) || !this.TryShoot())
			{
				this.SetPressState(false);
				if (this.allowCharging)
				{
					ContinuousPropertyArray continuousPropertyArray = this.continuousChargingProperties;
					if (continuousPropertyArray != null)
					{
						continuousPropertyArray.ApplyAll(0f);
					}
					UnityEvent<float> unityEvent = this.whileCharging;
					if (unityEvent != null)
					{
						unityEvent.Invoke(0f);
					}
					UnityEvent unityEvent2 = this.onChargeCancelled;
					if (unityEvent2 == null)
					{
						return;
					}
					unityEvent2.Invoke();
				}
			}
		}

		// Token: 0x06007766 RID: 30566 RVA: 0x0027265F File Offset: 0x0027085F
		public void ResetShoot()
		{
			this.isPressed = false;
			this.velocityEstimatorThresholdMet = false;
			this.currentStep = -1;
			this.lastStep = -1;
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06007767 RID: 30567 RVA: 0x00272684 File Offset: 0x00270884
		private void AttachTrail(int trailHash, GameObject newProjectile, Vector3 location, bool blueTeam, bool orangeTeam)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(trailHash, true);
			SlingshotProjectileTrail component = gameObject.GetComponent<SlingshotProjectileTrail>();
			if (component.IsNull())
			{
				ObjectPools.instance.Destroy(gameObject);
			}
			newProjectile.transform.position = location;
			component.AttachTrail(newProjectile, blueTeam, orangeTeam, false, default(Color));
		}

		// Token: 0x04008991 RID: 35217
		private const string CHARGE_STR = "allowCharging";

		// Token: 0x04008992 RID: 35218
		private const string CHARGE_MSG = "only enabled when allowCharging is true.";

		// Token: 0x04008993 RID: 35219
		private const string HAPTICS_STR = "enableHaptics";

		// Token: 0x04008994 RID: 35220
		private const string MOVE_STR = "IsMovementShoot";

		// Token: 0x04008995 RID: 35221
		[SerializeField]
		private HashWrapper projectilePrefab;

		// Token: 0x04008996 RID: 35222
		[SerializeField]
		private HashWrapper projectileTrailPrefab;

		// Token: 0x04008997 RID: 35223
		[FormerlySerializedAs("launchActivatorType")]
		[SerializeField]
		private ProjectileShooterCosmetic.ShootActivator shootActivatorType;

		// Token: 0x04008998 RID: 35224
		[FormerlySerializedAs("launchDirectionType")]
		[SerializeField]
		private ProjectileShooterCosmetic.ShootDirection shootDirectionType;

		// Token: 0x04008999 RID: 35225
		[SerializeField]
		private Vector3 offsetRigPosition;

		// Token: 0x0400899A RID: 35226
		[FormerlySerializedAs("launchTransform")]
		[SerializeField]
		private Transform shootFromTransform;

		// Token: 0x0400899B RID: 35227
		[SerializeField]
		private bool drawShootVector;

		// Token: 0x0400899C RID: 35228
		[FormerlySerializedAs("cooldown")]
		[SerializeField]
		private float cooldownSeconds;

		// Token: 0x0400899D RID: 35229
		[Space]
		[SerializeField]
		private bool enableHaptics = true;

		// Token: 0x0400899E RID: 35230
		[FormerlySerializedAs("hapticsIntensity")]
		[SerializeField]
		private float shootHapticsIntensity = 0.5f;

		// Token: 0x0400899F RID: 35231
		[FormerlySerializedAs("hapticsDuration")]
		[SerializeField]
		private float shootHapticsDuration = 0.2f;

		// Token: 0x040089A0 RID: 35232
		[SerializeField]
		[Tooltip("only enabled when allowCharging is true.")]
		private float chargeHapticsIntensity = 0.3f;

		// Token: 0x040089A1 RID: 35233
		[SerializeField]
		[Tooltip("only enabled when allowCharging is true.")]
		private float maxChargeHapticsIntensity = 0.3f;

		// Token: 0x040089A2 RID: 35234
		[SerializeField]
		private bool hapticsBothHands;

		// Token: 0x040089A3 RID: 35235
		[Space]
		[SerializeField]
		private GorillaVelocityEstimator velocityEstimator;

		// Token: 0x040089A4 RID: 35236
		[SerializeField]
		private float velocityEstimatorStartGestureSpeed = 0.5f;

		// Token: 0x040089A5 RID: 35237
		[SerializeField]
		private float velocityEstimatorStopGestureSpeed = 0.2f;

		// Token: 0x040089A6 RID: 35238
		[SerializeField]
		private float velocityEstimatorMinRigDotProduct = 0.5f;

		// Token: 0x040089A7 RID: 35239
		[SerializeField]
		private bool logVelocityEstimatorSpeed;

		// Token: 0x040089A8 RID: 35240
		[FormerlySerializedAs("launchMinSpeed")]
		[SerializeField]
		[Tooltip("only enabled when allowCharging is true.")]
		private float shootMinSpeed;

		// Token: 0x040089A9 RID: 35241
		[FormerlySerializedAs("launchMaxSpeed")]
		[SerializeField]
		private float shootMaxSpeed;

		// Token: 0x040089AA RID: 35242
		[SerializeField]
		private bool allowCharging;

		// Token: 0x040089AB RID: 35243
		[SerializeField]
		private float maxChargeSeconds = 2f;

		// Token: 0x040089AC RID: 35244
		[SerializeField]
		private float snapToMaxChargeAt = 9999999f;

		// Token: 0x040089AD RID: 35245
		[SerializeField]
		private float chargeDecaySpeed = 9999999f;

		// Token: 0x040089AE RID: 35246
		[SerializeField]
		private bool runChargeCancelledEventOnShoot;

		// Token: 0x040089AF RID: 35247
		[SerializeField]
		private AnimationCurve chargeRateCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040089B0 RID: 35248
		[SerializeField]
		private AnimationCurve chargeToShotSpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040089B1 RID: 35249
		[FormerlySerializedAs("onReadyToShoot")]
		public UnityEvent onCooldownFinished;

		// Token: 0x040089B2 RID: 35250
		public ContinuousPropertyArray continuousChargingProperties;

		// Token: 0x040089B3 RID: 35251
		public UnityEvent<float> whileCharging;

		// Token: 0x040089B4 RID: 35252
		public UnityEvent onMaxCharge;

		// Token: 0x040089B5 RID: 35253
		public UnityEvent onChargeCancelled;

		// Token: 0x040089B6 RID: 35254
		[FormerlySerializedAs("onLaunchProjectileShared")]
		public UnityEvent<float> onShoot;

		// Token: 0x040089B7 RID: 35255
		[FormerlySerializedAs("onOwnerLaunchProjectile")]
		public UnityEvent<float> onShootLocal;

		// Token: 0x040089B8 RID: 35256
		[SerializeField]
		private int numberOfProgressSteps;

		// Token: 0x040089B9 RID: 35257
		public UnityEvent<int> onMovedToNextStep;

		// Token: 0x040089BA RID: 35258
		public UnityEvent<int> onReachedLastProgressStep;

		// Token: 0x040089BB RID: 35259
		private int currentStep = -1;

		// Token: 0x040089BC RID: 35260
		private int lastStep = -1;

		// Token: 0x040089BE RID: 35262
		private bool isPressed;

		// Token: 0x040089BF RID: 35263
		private bool velocityEstimatorThresholdMet;

		// Token: 0x040089C0 RID: 35264
		private float cooldownRemaining;

		// Token: 0x040089C1 RID: 35265
		private float chargeTime;

		// Token: 0x040089C2 RID: 35266
		private TransferrableObject transferrableObject;

		// Token: 0x040089C3 RID: 35267
		private VRRig rig;

		// Token: 0x040089C4 RID: 35268
		private bool isLocal;

		// Token: 0x040089C5 RID: 35269
		private Transform debugShootDirection;

		// Token: 0x020012A0 RID: 4768
		private enum ShootActivator
		{
			// Token: 0x040089C8 RID: 35272
			ButtonReleased,
			// Token: 0x040089C9 RID: 35273
			ButtonPressed,
			// Token: 0x040089CA RID: 35274
			ButtonStayed,
			// Token: 0x040089CB RID: 35275
			VelocityEstimatorThreshold,
			// Token: 0x040089CC RID: 35276
			ButtonReleasedFullCharge
		}

		// Token: 0x020012A1 RID: 4769
		private enum ShootDirection
		{
			// Token: 0x040089CE RID: 35278
			LaunchTransformRotation,
			// Token: 0x040089CF RID: 35279
			LineFromRigToLaunchTransform
		}
	}
}
