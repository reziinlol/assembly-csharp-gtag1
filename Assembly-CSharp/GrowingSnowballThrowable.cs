using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000200 RID: 512
public class GrowingSnowballThrowable : SnowballThrowable
{
	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06000D6B RID: 3435 RVA: 0x00049202 File Offset: 0x00047402
	public int SizeLevel
	{
		get
		{
			return this.sizeLevel;
		}
	}

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x06000D6C RID: 3436 RVA: 0x0004920A File Offset: 0x0004740A
	public int MaxSizeLevel
	{
		get
		{
			return Mathf.Max(this.snowballSizeLevels.Count - 1, 0);
		}
	}

	// Token: 0x1700013B RID: 315
	// (get) Token: 0x06000D6D RID: 3437 RVA: 0x00049220 File Offset: 0x00047420
	public float CurrentSnowballRadius
	{
		get
		{
			if (this.snowballSizeLevels.Count > 0 && this.sizeLevel > -1 && this.sizeLevel < this.snowballSizeLevels.Count)
			{
				return this.snowballSizeLevels[this.sizeLevel].snowballScale * this.modelRadius * base.transform.lossyScale.x;
			}
			return this.modelRadius * base.transform.lossyScale.x;
		}
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x000492A0 File Offset: 0x000474A0
	protected override void Awake()
	{
		base.Awake();
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted += this.StartedMultiplayerSession;
		}
		else
		{
			Debug.LogError("NetworkSystem.Instance was null in SnowballThrowable Awake");
		}
		VRRigCache.OnRigActivated += this.VRRigActivated;
		VRRigCache.OnRigDeactivated += this.VRRigDeactivated;
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x00049310 File Offset: 0x00047510
	public override void OnEnable()
	{
		base.OnEnable();
		this.snowballModelParentTransform.localPosition = this.modelParentOffset;
		this.snowballModelTransform.localPosition = this.modelOffset;
		this.otherHandSnowball = (this.isLeftHanded ? (EquipmentInteractor.instance.rightHandHeldEquipment as GrowingSnowballThrowable) : (EquipmentInteractor.instance.leftHandHeldEquipment as GrowingSnowballThrowable));
		if (Time.time > this.maintainSizeLevelUntilLocalTime)
		{
			this.SetSizeLevelLocal(0);
		}
		this.CreatePhotonEventsIfNull();
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x00049391 File Offset: 0x00047591
	protected override void OnDestroy()
	{
		this.DestroyPhotonEvents();
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x0004939C File Offset: 0x0004759C
	private void VRRigActivated(RigContainer rigContainer)
	{
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = (this.targetRig != null && this.targetRig.isOfflineVRRig);
		if (rigContainer.Rig == this.targetRig)
		{
			this.CreatePhotonEventsIfNull();
		}
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x000493F1 File Offset: 0x000475F1
	private void VRRigDeactivated(RigContainer rigContainer)
	{
		if (rigContainer.Rig == this.targetRig)
		{
			this.DestroyPhotonEvents();
		}
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x0004940C File Offset: 0x0004760C
	private void StartedMultiplayerSession()
	{
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = (this.targetRig != null && this.targetRig.isOfflineVRRig);
		if (this.isOfflineRig)
		{
			this.DestroyPhotonEvents();
			this.CreatePhotonEventsIfNull();
		}
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x0004945C File Offset: 0x0004765C
	private void CreatePhotonEventsIfNull()
	{
		if (this.targetRig == null)
		{
			this.targetRig = base.GetComponentInParent<VRRig>(true);
			this.isOfflineRig = (this.targetRig != null && this.targetRig.isOfflineVRRig);
		}
		if (this.targetRig == null || this.targetRig.netView == null)
		{
			return;
		}
		if (this.changeSizeEvent == null)
		{
			"SnowballThrowable" + base.gameObject.name + (this.isLeftHanded ? "ChangeSizeEventLeft" : "ChangeSizeEventRight") + this.targetRig.netView.ViewID.ToString();
			int eventId = StaticHash.Compute("SnowballThrowable", base.gameObject.name, this.isLeftHanded ? "ChangeSizeEventLeft" : "ChangeSizeEventRight", this.targetRig.netView.ViewID.ToString());
			this.changeSizeEvent = new PhotonEvent(eventId);
			this.changeSizeEvent.reliable = true;
			this.changeSizeEvent += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ChangeSizeEventReceiver);
		}
		if (this.snowballThrowEvent == null)
		{
			"SnowballThrowable" + base.gameObject.name + (this.isLeftHanded ? "SnowballThrowEventLeft" : "SnowballThrowEventRight") + this.targetRig.netView.ViewID.ToString();
			int eventId2 = StaticHash.Compute("SnowballThrowable", base.gameObject.name, this.isLeftHanded ? "SnowballThrowEventLeft" : "SnowballThrowEventRight", this.targetRig.netView.ViewID.ToString());
			this.snowballThrowEvent = new PhotonEvent(eventId2);
			this.snowballThrowEvent.reliable = true;
			this.snowballThrowEvent += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SnowballThrowEventReceiver);
		}
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x0004965C File Offset: 0x0004785C
	private void DestroyPhotonEvents()
	{
		if (this.changeSizeEvent != null)
		{
			this.changeSizeEvent -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ChangeSizeEventReceiver);
			this.changeSizeEvent.Dispose();
			this.changeSizeEvent = null;
		}
		if (this.snowballThrowEvent != null)
		{
			this.snowballThrowEvent -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SnowballThrowEventReceiver);
			this.snowballThrowEvent.Dispose();
			this.snowballThrowEvent = null;
		}
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x000496E3 File Offset: 0x000478E3
	public void IncreaseSize(int increase)
	{
		this.SetSizeLevelAuthority(this.sizeLevel + increase);
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x000496F4 File Offset: 0x000478F4
	private void SetSizeLevelAuthority(int sizeLevel)
	{
		if (this.targetRig != null && this.targetRig.creator != null && this.targetRig.creator.IsLocal)
		{
			int validSizeLevel = this.GetValidSizeLevel(sizeLevel);
			if (validSizeLevel > this.sizeLevel)
			{
				this.sizeIncreaseSoundBankPlayer.Play();
			}
			this.SetSizeLevelLocal(validSizeLevel);
			PhotonEvent photonEvent = this.changeSizeEvent;
			if (photonEvent == null)
			{
				return;
			}
			photonEvent.RaiseOthers(new object[]
			{
				validSizeLevel
			});
		}
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x00049770 File Offset: 0x00047970
	private int GetValidSizeLevel(int inputSizeLevel)
	{
		int max = Mathf.Max(this.snowballSizeLevels.Count - 1, 0);
		return Mathf.Clamp(inputSizeLevel, 0, max);
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x0004979C File Offset: 0x0004799C
	private void SetSizeLevelLocal(int sizeLevel)
	{
		int validSizeLevel = this.GetValidSizeLevel(sizeLevel);
		if (validSizeLevel >= 0 && validSizeLevel != this.sizeLevel)
		{
			this.sizeLevel = validSizeLevel;
			this.snowballModelParentTransform.localScale = Vector3.one * this.snowballSizeLevels[this.sizeLevel].snowballScale;
		}
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x000497F0 File Offset: 0x000479F0
	private void ChangeSizeEventReceiver(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != receiver)
		{
			return;
		}
		if (args == null || args.Length < 1)
		{
			return;
		}
		int num = (this.targetRig != null && this.targetRig.gameObject.activeInHierarchy && this.targetRig.netView != null && this.targetRig.netView.Owner != null) ? this.targetRig.netView.Owner.ActorNumber : -1;
		if (info.senderID != num)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "ChangeSizeEventReceiver");
		int num2 = (int)args[0];
		if (this.GetValidSizeLevel(num2) > this.sizeLevel && this.sizeIncreaseSoundBankPlayer.gameObject.activeInHierarchy)
		{
			this.sizeIncreaseSoundBankPlayer.Play();
		}
		this.SetSizeLevelLocal(num2);
		if (!base.gameObject.activeSelf)
		{
			this.maintainSizeLevelUntilLocalTime = Time.time + 0.1f;
		}
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x000498DC File Offset: 0x00047ADC
	private void SnowballThrowEventReceiver(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != receiver)
		{
			return;
		}
		if (args == null || args.Length < 3)
		{
			return;
		}
		if (this.targetRig.IsNull() || !this.targetRig.gameObject.activeSelf)
		{
			return;
		}
		NetPlayer creator = this.targetRig.creator;
		if (info.senderID != this.targetRig.creator.ActorNumber)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "SnowballThrowEventReceiver");
		if (!FXSystem.CheckCallSpam(this.targetRig.fxSettings, 4, info.SentServerTime))
		{
			return;
		}
		object obj = args[0];
		if (obj is Vector3)
		{
			Vector3 vector = (Vector3)obj;
			obj = args[1];
			if (obj is Vector3)
			{
				Vector3 inVel = (Vector3)obj;
				obj = args[2];
				if (obj is int)
				{
					int index = (int)obj;
					Vector3 velocity = this.targetRig.ClampVelocityRelativeToPlayerSafe(inVel, 50f, 100f);
					float x = this.snowballModelTransform.lossyScale.x;
					float num = 10000f;
					if (!vector.IsValid(num) || !this.targetRig.IsPositionInRange(vector, 4f))
					{
						return;
					}
					this.LaunchSnowballRemote(vector, velocity, x, index, info);
					return;
				}
			}
		}
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x00049A08 File Offset: 0x00047C08
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (GrowingSnowballThrowable.twoHandedSnowballGrowing)
		{
			if (this.otherHandSnowball != null && this.otherHandSnowball.isActiveAndEnabled)
			{
				IHoldableObject holdableObject = this.isLeftHanded ? EquipmentInteractor.instance.rightHandHeldEquipment : EquipmentInteractor.instance.leftHandHeldEquipment;
				if (holdableObject != null && this.otherHandSnowball != (GrowingSnowballThrowable)holdableObject)
				{
					this.otherHandSnowball = null;
					return;
				}
				float num = this.otherHandSnowball.CurrentSnowballRadius + this.CurrentSnowballRadius;
				if (this.SizeLevel < this.MaxSizeLevel && this.otherHandSnowball.SizeLevel < this.otherHandSnowball.MaxSizeLevel && (this.otherHandSnowball.snowballModelTransform.position - this.snowballModelTransform.position).sqrMagnitude < num * num)
				{
					int num2 = this.SizeLevel - this.otherHandSnowball.SizeLevel;
					float magnitude = this.velocityEstimator.linearVelocity.magnitude;
					float magnitude2 = this.otherHandSnowball.velocityEstimator.linearVelocity.magnitude;
					bool flag;
					if (Mathf.Abs(magnitude - magnitude2) > this.combineBasedOnSpeedThreshold || num2 == 0)
					{
						flag = (magnitude > magnitude2);
					}
					else
					{
						flag = (num2 < 0);
					}
					if (flag)
					{
						this.otherHandSnowball.IncreaseSize(this.sizeLevel + 1);
						GorillaTagger.Instance.StartVibration(!this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
						base.SetSnowballActiveLocal(false);
						return;
					}
					this.IncreaseSize(this.otherHandSnowball.SizeLevel + 1);
					GorillaTagger.Instance.StartVibration(this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
					this.otherHandSnowball.SetSnowballActiveLocal(false);
					return;
				}
			}
			else
			{
				this.otherHandSnowball = null;
			}
		}
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x00049C0E File Offset: 0x00047E0E
	protected override void OnSnowballRelease()
	{
		if (base.isActiveAndEnabled)
		{
			this.PerformSnowballThrowAuthority();
		}
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x00049C20 File Offset: 0x00047E20
	protected override void PerformSnowballThrowAuthority()
	{
		if (!(this.targetRig != null) || this.targetRig.creator == null || !this.targetRig.creator.IsLocal)
		{
			return;
		}
		Vector3 b = Vector3.zero;
		Rigidbody component = GorillaTagger.Instance.GetComponent<Rigidbody>();
		if (component != null)
		{
			b = component.linearVelocity;
		}
		Vector3 a = this.velocityEstimator.linearVelocity - b;
		float magnitude = a.magnitude;
		if (magnitude > 0.001f)
		{
			float num = Mathf.Clamp(magnitude * this.linSpeedMultiplier, 0f, this.maxLinSpeed);
			a *= num / magnitude;
		}
		Vector3 vector = a + b;
		this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
		Transform transform = this.snowballModelTransform;
		Vector3 position = transform.position;
		float x = transform.lossyScale.x;
		SlingshotProjectile slingshotProjectile = this.LaunchSnowballLocal(position, vector, x);
		base.SetSnowballActiveLocal(false);
		if (this.randModelIndex > -1 && this.randModelIndex < this.localModels.Count && this.localModels[this.randModelIndex].destroyAfterRelease)
		{
			slingshotProjectile.DestroyAfterRelease();
		}
		PhotonEvent photonEvent = this.snowballThrowEvent;
		if (photonEvent == null)
		{
			return;
		}
		photonEvent.RaiseOthers(new object[]
		{
			position,
			vector,
			slingshotProjectile.myProjectileCount
		});
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x00049D84 File Offset: 0x00047F84
	protected virtual SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale)
	{
		return this.LaunchSnowballLocal(location, velocity, scale, false, Color.white);
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x00049D98 File Offset: 0x00047F98
	protected override SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale, bool randomizeColour, Color colour)
	{
		SlingshotProjectile slingshotProjectile = this.SpawnGrowingSnowball(ref velocity, scale);
		int projectileCount = ProjectileTracker.AddAndIncrementLocalProjectile(slingshotProjectile, velocity, location, scale);
		slingshotProjectile.Launch(location, velocity, NetworkSystem.Instance.LocalPlayer, false, false, projectileCount, scale, randomizeColour, colour);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		slingshotProjectile.OnImpact += this.OnProjectileImpact;
		return slingshotProjectile;
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x00049E0F File Offset: 0x0004800F
	protected virtual SlingshotProjectile LaunchSnowballRemote(Vector3 location, Vector3 velocity, float scale, int index, PhotonMessageInfoWrapped info)
	{
		return this.LaunchSnowballRemote(location, velocity, scale, index, false, Color.white, info);
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x00049E24 File Offset: 0x00048024
	protected virtual SlingshotProjectile LaunchSnowballRemote(Vector3 location, Vector3 velocity, float scale, int index, bool randomizeColour, Color colour, PhotonMessageInfoWrapped info)
	{
		SlingshotProjectile slingshotProjectile = this.SpawnGrowingSnowball(ref velocity, scale);
		ProjectileTracker.AddRemotePlayerProjectile(info.Sender, slingshotProjectile, index, info.SentServerTime, velocity, location, scale);
		slingshotProjectile.Launch(location, velocity, info.Sender, false, false, index, scale, randomizeColour, Color.white);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		slingshotProjectile.OnImpact += this.OnProjectileImpact;
		return slingshotProjectile;
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x00049EB0 File Offset: 0x000480B0
	private SlingshotProjectile SpawnGrowingSnowball(ref Vector3 velocity, float scale)
	{
		SlingshotProjectile component = ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].projectilePrefab : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
		if (this.snowballSizeLevels.Count > 0 && this.sizeLevel >= 0 && this.sizeLevel < this.snowballSizeLevels.Count)
		{
			float num = scale / this.snowballSizeLevels[this.sizeLevel].snowballScale;
			SlingshotProjectile.AOEKnockbackConfig aoeKnockbackConfig = this.snowballSizeLevels[this.sizeLevel].aoeKnockbackConfig;
			aoeKnockbackConfig.aeoInnerRadius *= num;
			aoeKnockbackConfig.aeoOuterRadius *= num;
			aoeKnockbackConfig.knockbackVelocity *= num;
			aoeKnockbackConfig.impactVelocityThreshold *= num;
			velocity *= this.snowballSizeLevels[this.sizeLevel].throwSpeedMultiplier;
			component.gravityMultiplier = this.snowballSizeLevels[this.sizeLevel].gravityMultiplier;
			component.impactEffectScaleMultiplier = this.snowballSizeLevels[this.sizeLevel].impactEffectScale;
			component.aoeKnockbackConfig = new SlingshotProjectile.AOEKnockbackConfig?(aoeKnockbackConfig);
			component.impactSoundVolumeOverride = new float?(this.snowballSizeLevels[this.sizeLevel].impactSoundVolume);
			component.impactSoundPitchOverride = new float?(this.snowballSizeLevels[this.sizeLevel].impactSoundPitch);
		}
		return component;
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x0004A038 File Offset: 0x00048238
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!(this.targetRig != null) || this.targetRig.creator == null || !this.targetRig.creator.IsLocal)
		{
			return;
		}
		SnowballThrowable snowballThrowable;
		if (((this.isLeftHanded && grabbingHand == EquipmentInteractor.instance.rightHand && EquipmentInteractor.instance.rightHandHeldEquipment == null) || (!this.isLeftHanded && grabbingHand == EquipmentInteractor.instance.leftHand && EquipmentInteractor.instance.leftHandHeldEquipment == null)) && (this.isLeftHanded ? SnowballMaker.rightHandInstance : SnowballMaker.leftHandInstance).TryCreateSnowball(this.matDataIndexes[0], out snowballThrowable))
		{
			GrowingSnowballThrowable growingSnowballThrowable = snowballThrowable as GrowingSnowballThrowable;
			if (growingSnowballThrowable != null)
			{
				growingSnowballThrowable.IncreaseSize(this.sizeLevel);
				GorillaTagger.Instance.StartVibration(!this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				base.SetSnowballActiveLocal(false);
			}
		}
	}

	// Token: 0x0400100E RID: 4110
	public Transform snowballModelParentTransform;

	// Token: 0x0400100F RID: 4111
	public Transform snowballModelTransform;

	// Token: 0x04001010 RID: 4112
	public Vector3 modelParentOffset = Vector3.zero;

	// Token: 0x04001011 RID: 4113
	public Vector3 modelOffset = Vector3.zero;

	// Token: 0x04001012 RID: 4114
	public float modelRadius = 0.055f;

	// Token: 0x04001013 RID: 4115
	[Tooltip("Snowballs will combine into the larger snowball unless they are moving faster than this threshold.Then the faster moving snowball will go in to the more stationary hand")]
	public float combineBasedOnSpeedThreshold = 0.5f;

	// Token: 0x04001014 RID: 4116
	public SoundBankPlayer sizeIncreaseSoundBankPlayer;

	// Token: 0x04001015 RID: 4117
	public List<GrowingSnowballThrowable.SizeParameters> snowballSizeLevels = new List<GrowingSnowballThrowable.SizeParameters>();

	// Token: 0x04001016 RID: 4118
	private int sizeLevel;

	// Token: 0x04001017 RID: 4119
	private float maintainSizeLevelUntilLocalTime;

	// Token: 0x04001018 RID: 4120
	private PhotonEvent changeSizeEvent;

	// Token: 0x04001019 RID: 4121
	private PhotonEvent snowballThrowEvent;

	// Token: 0x0400101A RID: 4122
	[HideInInspector]
	public static bool debugDrawAOERange = false;

	// Token: 0x0400101B RID: 4123
	[HideInInspector]
	public static bool twoHandedSnowballGrowing = true;

	// Token: 0x0400101C RID: 4124
	private Queue<GrowingSnowballThrowable.AOERangeDebugDraw> aoeRangeDebugDrawQueue = new Queue<GrowingSnowballThrowable.AOERangeDebugDraw>();

	// Token: 0x0400101D RID: 4125
	private GrowingSnowballThrowable otherHandSnowball;

	// Token: 0x0400101E RID: 4126
	private float debugDrawAOERangeTime = 1.5f;

	// Token: 0x02000201 RID: 513
	[Serializable]
	public struct SizeParameters
	{
		// Token: 0x0400101F RID: 4127
		public float snowballScale;

		// Token: 0x04001020 RID: 4128
		public float impactEffectScale;

		// Token: 0x04001021 RID: 4129
		public float impactSoundVolume;

		// Token: 0x04001022 RID: 4130
		public float impactSoundPitch;

		// Token: 0x04001023 RID: 4131
		public float throwSpeedMultiplier;

		// Token: 0x04001024 RID: 4132
		public float gravityMultiplier;

		// Token: 0x04001025 RID: 4133
		public SlingshotProjectile.AOEKnockbackConfig aoeKnockbackConfig;
	}

	// Token: 0x02000202 RID: 514
	private struct AOERangeDebugDraw
	{
		// Token: 0x04001026 RID: 4134
		public float impactTime;

		// Token: 0x04001027 RID: 4135
		public Vector3 position;

		// Token: 0x04001028 RID: 4136
		public float innerRadius;

		// Token: 0x04001029 RID: 4137
		public float outerRadius;
	}
}
