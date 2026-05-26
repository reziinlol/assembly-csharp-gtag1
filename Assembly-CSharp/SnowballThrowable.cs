using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000205 RID: 517
public class SnowballThrowable : HoldableObject, IHeldItem
{
	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000D98 RID: 3480 RVA: 0x0004A779 File Offset: 0x00048979
	// (set) Token: 0x06000D99 RID: 3481 RVA: 0x0004A781 File Offset: 0x00048981
	public XformOffset SpawnOffset
	{
		get
		{
			return this.spawnOffset;
		}
		set
		{
			this.spawnOffset = value;
		}
	}

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000D9A RID: 3482 RVA: 0x0004A78C File Offset: 0x0004898C
	internal int ProjectileHash
	{
		get
		{
			return PoolUtils.GameObjHashCode((this.randomModelSelection && this.localModels != null && this.randModelIndex >= 0 && this.randModelIndex <= this.localModels.Count && this.localModels[this.randModelIndex] != null) ? this.localModels[this.randModelIndex].GetProjectilePrefab() : this.projectilePrefab);
		}
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x0004A804 File Offset: 0x00048A04
	protected virtual void Awake()
	{
		if (this.awakeHasBeenCalled)
		{
			return;
		}
		this.awakeHasBeenCalled = true;
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = (this.targetRig != null && this.targetRig.isOfflineVRRig);
		this.renderers = base.GetComponentsInChildren<Renderer>();
		this.randModelIndex = -1;
		foreach (RandomProjectileThrowable randomProjectileThrowable in this.localModels)
		{
			if (randomProjectileThrowable != null)
			{
				RandomProjectileThrowable randomProjectileThrowable2 = randomProjectileThrowable;
				randomProjectileThrowable2.OnDestroyRandomProjectile = (UnityAction<bool>)Delegate.Combine(randomProjectileThrowable2.OnDestroyRandomProjectile, new UnityAction<bool>(this.HandleOnDestroyRandomProjectile));
			}
		}
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x0004A8D0 File Offset: 0x00048AD0
	public bool IsMine()
	{
		return this.targetRig != null && this.targetRig.isOfflineVRRig;
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x0004A8ED File Offset: 0x00048AED
	bool IHeldItem.InLeftHand()
	{
		return this.isLeftHanded && base.gameObject.activeSelf;
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x0004A904 File Offset: 0x00048B04
	bool IHeldItem.InHand()
	{
		return base.gameObject.activeSelf;
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x0004A911 File Offset: 0x00048B11
	bool IHeldItem.IsMyItem()
	{
		return this.IsMine();
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x0004A91C File Offset: 0x00048B1C
	public virtual void OnEnable()
	{
		if (this.targetRig == null)
		{
			Debug.LogError("SnowballThrowable: targetRig is null! Deactivating.");
			base.gameObject.SetActive(false);
			return;
		}
		if (!this.targetRig.isOfflineVRRig)
		{
			if (this.targetRig.netView != null && this.targetRig.netView.IsMine)
			{
				base.gameObject.SetActive(false);
				return;
			}
			Color32 throwableProjectileColor = this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
			this.ApplyColor(throwableProjectileColor);
			if (this.randomModelSelection)
			{
				foreach (RandomProjectileThrowable randomProjectileThrowable in this.localModels)
				{
					randomProjectileThrowable.gameObject.SetActive(false);
				}
				this.randModelIndex = this.targetRig.GetRandomThrowableModelIndex();
				this.EnableRandomModel(this.randModelIndex, true);
			}
		}
		this.AnchorToHand();
		this.OnEnableHasBeenCalled = true;
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnDisable()
	{
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected new virtual void OnDestroy()
	{
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x0004AA2C File Offset: 0x00048C2C
	public void SetSnowballActiveLocal(bool enabled)
	{
		if (!this.awakeHasBeenCalled)
		{
			this.Awake();
		}
		if (!this.OnEnableHasBeenCalled)
		{
			this.OnEnable();
		}
		if (this.isLeftHanded)
		{
			this.targetRig.LeftThrowableProjectileIndex = (enabled ? this.throwableMakerIndex : -1);
		}
		else
		{
			this.targetRig.RightThrowableProjectileIndex = (enabled ? this.throwableMakerIndex : -1);
		}
		bool flag = !base.gameObject.activeSelf && enabled;
		base.gameObject.SetActive(enabled);
		if (flag && this.pickupSoundBankPlayer != null)
		{
			this.pickupSoundBankPlayer.Play();
			if (this.playHapticsOnPickup)
			{
				GorillaTagger.Instance.StartVibration(this.isLeftHanded, (this.pickupHapticStrength > 0f) ? this.pickupHapticStrength : GorillaTagger.Instance.tapHapticStrength, (this.pickupHapticDuration > 0f) ? this.pickupHapticDuration : GorillaTagger.Instance.tapHapticDuration);
			}
		}
		if (this.randomModelSelection)
		{
			if (enabled)
			{
				this.EnableRandomModel(this.GetRandomModelIndex(), true);
			}
			else
			{
				this.EnableRandomModel(this.randModelIndex, false);
			}
			this.targetRig.SetRandomThrowableModelIndex(this.randModelIndex);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(enabled ? this : null, this.isLeftHanded);
		if (this.randomizeColor)
		{
			Color color = enabled ? GTColor.RandomHSV(this.randomColorHSVRanges) : Color.white;
			this.targetRig.SetThrowableProjectileColor(this.isLeftHanded, color);
			this.ApplyColor(color);
		}
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x0004ABA8 File Offset: 0x00048DA8
	private int GetRandomModelIndex()
	{
		if (this.localModels.Count == 0)
		{
			return -1;
		}
		this.randModelIndex = Random.Range(0, this.localModels.Count);
		if ((float)Random.Range(1, 100) <= this.localModels[this.randModelIndex].spawnChance * 100f)
		{
			return this.randModelIndex;
		}
		return this.GetRandomModelIndex();
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x0004AC10 File Offset: 0x00048E10
	private void EnableRandomModel(int index, bool enable)
	{
		if (this.randModelIndex >= 0 && this.randModelIndex < this.localModels.Count)
		{
			this.localModels[this.randModelIndex].gameObject.SetActive(enable);
			if (enable && this.localModels[this.randModelIndex].autoDestroyAfterSeconds > 0f)
			{
				this.destroyTimer = 0f;
			}
			return;
		}
	}

	// Token: 0x06000DA6 RID: 3494 RVA: 0x0004AC84 File Offset: 0x00048E84
	protected virtual void LateUpdateLocal()
	{
		if (this.randomModelSelection && this.randModelIndex > -1 && this.localModels[this.randModelIndex].ForceDestroy)
		{
			this.localModels[this.randModelIndex].ForceDestroy = false;
			if (this.localModels[this.randModelIndex].gameObject.activeSelf)
			{
				this.PerformSnowballThrowAuthority();
			}
		}
		if (this.randomModelSelection && this.randModelIndex > -1 && this.localModels[this.randModelIndex].autoDestroyAfterSeconds > 0f)
		{
			this.destroyTimer += Time.deltaTime;
			if (this.destroyTimer > this.localModels[this.randModelIndex].autoDestroyAfterSeconds)
			{
				if (this.localModels[this.randModelIndex].gameObject.activeSelf)
				{
					this.PerformSnowballThrowAuthority();
				}
				this.destroyTimer = -1f;
			}
		}
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected void LateUpdateReplicated()
	{
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected void LateUpdateShared()
	{
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x0004AD83 File Offset: 0x00048F83
	private Transform Anchor()
	{
		return base.transform.parent;
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x0004AD90 File Offset: 0x00048F90
	private void AnchorToHand()
	{
		BodyDockPositions myBodyDockPositions = this.targetRig.myBodyDockPositions;
		Transform transform = this.Anchor();
		if (this.isLeftHanded)
		{
			transform.parent = myBodyDockPositions.leftHandTransform;
		}
		else
		{
			transform.parent = myBodyDockPositions.rightHandTransform;
		}
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		base.transform.localPosition = this.spawnOffset.pos;
		base.transform.localRotation = this.spawnOffset.rot;
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x0004AE14 File Offset: 0x00049014
	protected void LateUpdate()
	{
		if (this.IsMine())
		{
			this.LateUpdateLocal();
		}
		else
		{
			this.LateUpdateReplicated();
		}
		this.LateUpdateShared();
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x0004AE32 File Offset: 0x00049032
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.OnSnowballRelease();
		return true;
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x0004AE47 File Offset: 0x00049047
	protected virtual void OnSnowballRelease()
	{
		this.PerformSnowballThrowAuthority();
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x0004AE50 File Offset: 0x00049050
	protected virtual void PerformSnowballThrowAuthority()
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
		Vector3 velocity = a + b;
		Color32 throwableProjectileColor = this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float x = transform.lossyScale.x;
		SlingshotProjectile slingshotProjectile = this.LaunchSnowballLocal(position, velocity, x, this.randomizeColor, throwableProjectileColor);
		this.SetSnowballActiveLocal(false);
		if (this.randModelIndex > -1 && this.randModelIndex < this.localModels.Count)
		{
			if (this.localModels[this.randModelIndex].ForceDestroy || this.localModels[this.randModelIndex].destroyAfterRelease)
			{
				slingshotProjectile.DestroyAfterRelease();
			}
			else if (this.localModels[this.randModelIndex].moveOverPassedLifeTime)
			{
				float num2 = Time.time - this.localModels[this.randModelIndex].TimeEnabled;
				float remainingLifeTime = slingshotProjectile.GetRemainingLifeTime();
				if (remainingLifeTime > num2)
				{
					float newLifeTime = remainingLifeTime - num2;
					slingshotProjectile.UpdateRemainingLifeTime(newLifeTime);
				}
				else
				{
					slingshotProjectile.UpdateRemainingLifeTime(0f);
				}
			}
		}
		if (NetworkSystem.Instance.InRoom)
		{
			RoomSystem.SendLaunchProjectile(position, velocity, this.isLeftHanded ? RoomSystem.ProjectileSource.LeftHand : RoomSystem.ProjectileSource.RightHand, slingshotProjectile.myProjectileCount, this.randomizeColor, throwableProjectileColor.r, throwableProjectileColor.g, throwableProjectileColor.b, throwableProjectileColor.a);
		}
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x0004B058 File Offset: 0x00049258
	protected virtual SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale, bool randomColour, Color colour)
	{
		SlingshotProjectile component = ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].GetProjectilePrefab() : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
		int projectileCount = ProjectileTracker.AddAndIncrementLocalProjectile(component, velocity, location, scale);
		component.Launch(location, velocity, NetworkSystem.Instance.LocalPlayer, false, false, projectileCount, scale, randomColour, colour);
		GorillaTagger.Instance.StartVibration(this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		component.OnImpact += this.OnProjectileImpact;
		return component;
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x0004B12C File Offset: 0x0004932C
	protected virtual SlingshotProjectile SpawnProjectile()
	{
		return ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].GetProjectilePrefab() : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x0004B164 File Offset: 0x00049364
	protected virtual void OnProjectileImpact(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer)
	{
		if (hitPlayer != null)
		{
			ScienceExperimentManager instance = ScienceExperimentManager.instance;
			if (instance != null && this.projectilePrefab != null && this.projectilePrefab == instance.waterBalloonPrefab)
			{
				instance.OnWaterBalloonHitPlayer(hitPlayer);
			}
			if (hitPlayer.IsLocal)
			{
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			}
		}
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x0004B210 File Offset: 0x00049410
	private void ApplyColor(Color newColor)
	{
		foreach (Renderer renderer in this.renderers)
		{
			if (renderer)
			{
				foreach (Material material in renderer.materials)
				{
					if (!(material == null))
					{
						if (material.HasProperty(ShaderProps._BaseColor))
						{
							material.SetColor(ShaderProps._BaseColor, newColor);
						}
						if (material.HasProperty(ShaderProps._Color))
						{
							material.SetColor(ShaderProps._Color, newColor);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x0004B29D File Offset: 0x0004949D
	public override void DropItemCleanup()
	{
		if (base.gameObject.activeSelf)
		{
			this.OnSnowballRelease();
		}
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x0004B2B2 File Offset: 0x000494B2
	private void HandleOnDestroyRandomProjectile(bool enable)
	{
		this.SetSnowballActiveLocal(enable);
	}

	// Token: 0x04001038 RID: 4152
	[GorillaSoundLookup]
	public List<int> matDataIndexes = new List<int>
	{
		32
	};

	// Token: 0x04001039 RID: 4153
	[Tooltip("prefab to spawn from global object pools when thrown")]
	public GameObject projectilePrefab;

	// Token: 0x0400103A RID: 4154
	public SoundBankPlayer pickupSoundBankPlayer;

	// Token: 0x0400103B RID: 4155
	[Tooltip("If true, plays a haptic pulse on the grabbing hand when the snowball is picked up.")]
	public bool playHapticsOnPickup = true;

	// Token: 0x0400103C RID: 4156
	[Tooltip("Strength of the haptic pulse on pickup. Defaults to tapHapticStrength if left at 0.")]
	public float pickupHapticStrength;

	// Token: 0x0400103D RID: 4157
	[Tooltip("Duration of the haptic pulse on pickup. Defaults to tapHapticDuration if left at 0.")]
	public float pickupHapticDuration;

	// Token: 0x0400103E RID: 4158
	public bool isLeftHanded;

	// Token: 0x0400103F RID: 4159
	[Tooltip("This needs to match the index of the projectilePrefab on the Local Gorilla Player's BodyDockPositions LeftHandThrowables or RightHandThrowables list\nCheck the array in play mode to find the index")]
	public int throwableMakerIndex;

	// Token: 0x04001040 RID: 4160
	[Tooltip("Multiplier is applied to hand speed to get launch speed of the projectile")]
	public float linSpeedMultiplier = 1f;

	// Token: 0x04001041 RID: 4161
	[Tooltip("Maximum launch speed of the projectile")]
	public float maxLinSpeed = 12f;

	// Token: 0x04001042 RID: 4162
	[Space]
	[FormerlySerializedAs("shouldColorize")]
	public bool randomizeColor;

	// Token: 0x04001043 RID: 4163
	public GTColor.HSVRanges randomColorHSVRanges = new GTColor.HSVRanges(0f, 1f, 0.7f, 1f, 1f, 1f);

	// Token: 0x04001044 RID: 4164
	[Tooltip("Check this part only if we want to randomize the prefab meshes and projectile")]
	public bool randomModelSelection;

	// Token: 0x04001045 RID: 4165
	public List<RandomProjectileThrowable> localModels;

	// Token: 0x04001046 RID: 4166
	[Tooltip("projectile identifier sent out by the PlayerGameEvents.LaunchedProjectile event. Uses prefab name if empty")]
	public string throwEventName;

	// Token: 0x04001047 RID: 4167
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001048 RID: 4168
	protected VRRig targetRig;

	// Token: 0x04001049 RID: 4169
	protected bool isOfflineRig;

	// Token: 0x0400104A RID: 4170
	private bool awakeHasBeenCalled;

	// Token: 0x0400104B RID: 4171
	private bool OnEnableHasBeenCalled;

	// Token: 0x0400104C RID: 4172
	private Renderer[] renderers;

	// Token: 0x0400104D RID: 4173
	protected int randModelIndex;

	// Token: 0x0400104E RID: 4174
	private float destroyTimer = -1f;

	// Token: 0x0400104F RID: 4175
	private XformOffset spawnOffset;
}
