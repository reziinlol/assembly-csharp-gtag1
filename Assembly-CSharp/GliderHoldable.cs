using System;
using System.Collections;
using System.Runtime.InteropServices;
using AA;
using Fusion;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000C97 RID: 3223
[RequireComponent(typeof(Rigidbody))]
[NetworkBehaviourWeaved(11)]
public class GliderHoldable : NetworkHoldableObject, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x17000779 RID: 1913
	// (get) Token: 0x06004FDD RID: 20445 RVA: 0x001A67C0 File Offset: 0x001A49C0
	private bool OutOfBounds
	{
		get
		{
			return this.maxDistanceRespawnOrigin != null && (this.maxDistanceRespawnOrigin.position - base.transform.position).sqrMagnitude > this.maxDistanceBeforeRespawn * this.maxDistanceBeforeRespawn;
		}
	}

	// Token: 0x06004FDE RID: 20446 RVA: 0x001A6810 File Offset: 0x001A4A10
	protected override void Awake()
	{
		base.Awake();
		base.transform.parent = null;
		this.defaultMaxDistanceBeforeRespawn = this.maxDistanceBeforeRespawn;
		this.spawnPosition = (this.skyJungleSpawnPostion = base.transform.position);
		this.spawnRotation = (this.skyJungleSpawnRotation = base.transform.rotation);
		this.skyJungleRespawnOrigin = this.maxDistanceRespawnOrigin;
		this.syncedState.Init(this.spawnPosition, this.spawnRotation);
		this.rb = base.GetComponent<Rigidbody>();
		this.yaw = base.transform.rotation.eulerAngles.y;
		this.oneHandRotationRateExp = Mathf.Exp(this.oneHandHoldRotationRate);
		this.twoHandRotationRateExp = Mathf.Exp(this.twoHandHoldRotationRate);
		this.subtlePlayerPitchRateExp = Mathf.Exp(this.subtlePlayerPitchRate);
		this.subtlePlayerRollRateExp = Mathf.Exp(this.subtlePlayerRollRate);
		this.accelSmoothingFollowRateExp = Mathf.Exp(this.accelSmoothingFollowRate);
		this.networkSyncFollowRateExp = Mathf.Exp(this.networkSyncFollowRate);
		this.ownershipGuard.AddCallbackTarget(this);
		this.calmAudio.volume = 0f;
		this.activeAudio.volume = 0f;
		this.whistlingAudio.volume = 0f;
	}

	// Token: 0x06004FDF RID: 20447 RVA: 0x001A695E File Offset: 0x001A4B5E
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (this.ownershipGuard != null)
		{
			this.ownershipGuard.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x06004FE0 RID: 20448 RVA: 0x000F5EA4 File Offset: 0x000F40A4
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
	}

	// Token: 0x06004FE1 RID: 20449 RVA: 0x001A6980 File Offset: 0x001A4B80
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		this.Respawn();
		base.OnDisable();
	}

	// Token: 0x06004FE2 RID: 20450 RVA: 0x001A6994 File Offset: 0x001A4B94
	public void Respawn()
	{
		if ((base.IsValid && base.IsMine) || !NetworkSystem.Instance.InRoom)
		{
			if (EquipmentInteractor.instance != null)
			{
				if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
				{
					this.OnRelease(null, EquipmentInteractor.instance.leftHand);
				}
				if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
				{
					this.OnRelease(null, EquipmentInteractor.instance.rightHand);
				}
			}
			this.rb.isKinematic = true;
			base.transform.position = this.spawnPosition;
			base.transform.rotation = this.spawnRotation;
			this.lastHeldTime = -1f;
			this.syncedState.Init(this.spawnPosition, this.spawnRotation);
		}
	}

	// Token: 0x06004FE3 RID: 20451 RVA: 0x001A6A65 File Offset: 0x001A4C65
	public void CustomMapLoad(Transform placeholderTransform, float respawnDistance)
	{
		this.maxDistanceRespawnOrigin = placeholderTransform;
		this.spawnPosition = placeholderTransform.position;
		this.spawnRotation = placeholderTransform.rotation;
		this.maxDistanceBeforeRespawn = respawnDistance;
		this.Respawn();
	}

	// Token: 0x06004FE4 RID: 20452 RVA: 0x001A6A93 File Offset: 0x001A4C93
	public void CustomMapUnload()
	{
		this.maxDistanceRespawnOrigin = this.skyJungleRespawnOrigin;
		this.spawnPosition = this.skyJungleSpawnPostion;
		this.spawnRotation = this.skyJungleSpawnRotation;
		this.maxDistanceBeforeRespawn = this.defaultMaxDistanceBeforeRespawn;
		this.Respawn();
	}

	// Token: 0x1700077A RID: 1914
	// (get) Token: 0x06004FE5 RID: 20453 RVA: 0x00023994 File Offset: 0x00021B94
	public override bool TwoHanded
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06004FE6 RID: 20454 RVA: 0x001A6ACC File Offset: 0x001A4CCC
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest && this.syncedState.riderId == -1)
		{
			this.ownershipGuard.RequestOwnershipImmediately(delegate
			{
				this.pendingOwnershipRequest = false;
			});
			this.pendingOwnershipRequest = true;
			if (this.reenableOwnershipRequestCoroutine != null)
			{
				base.StopCoroutine(this.reenableOwnershipRequestCoroutine);
			}
			this.reenableOwnershipRequestCoroutine = base.StartCoroutine(this.ReenableOwnershipRequest());
		}
	}

	// Token: 0x06004FE7 RID: 20455 RVA: 0x001A6B48 File Offset: 0x001A4D48
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (base.IsMine || !NetworkSystem.Instance.InRoom || this.pendingOwnershipRequest)
		{
			this.OnGrabAuthority(pointGrabbed, grabbingHand);
			return;
		}
		if (NetworkSystem.Instance.InRoom && !base.IsMine && !this.pendingOwnershipRequest && this.syncedState.riderId == -1)
		{
			this.ownershipGuard.RequestOwnershipImmediately(delegate
			{
				this.pendingOwnershipRequest = false;
			});
			this.pendingOwnershipRequest = true;
			if (this.reenableOwnershipRequestCoroutine != null)
			{
				base.StopCoroutine(this.reenableOwnershipRequestCoroutine);
			}
			this.reenableOwnershipRequestCoroutine = base.StartCoroutine(this.ReenableOwnershipRequest());
			this.OnGrabAuthority(pointGrabbed, grabbingHand);
		}
	}

	// Token: 0x06004FE8 RID: 20456 RVA: 0x001A6BF0 File Offset: 0x001A4DF0
	public void OnGrabAuthority(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if ((flag && !EquipmentInteractor.instance.isLeftGrabbing) || (!flag && !EquipmentInteractor.instance.isRightGrabbing))
		{
			return;
		}
		if (this.riderId != NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.riderId = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		Vector3 worldGrabPoint = this.ClosestPointInHandle(grabbingHand.transform.position, pointGrabbed);
		if (flag)
		{
			this.leftHold.Activate(grabbingHand.transform, base.transform, worldGrabPoint);
		}
		else
		{
			this.rightHold.Activate(grabbingHand.transform, base.transform, worldGrabPoint);
		}
		if (this.leftHold.active && this.rightHold.active)
		{
			Vector3 handsVector = this.GetHandsVector(this.leftHold.transform.position, this.rightHold.transform.position, GTPlayer.Instance.headCollider.transform.position, true);
			this.twoHandRotationOffsetAxis = Vector3.Cross(handsVector, base.transform.right).normalized;
			if ((double)this.twoHandRotationOffsetAxis.sqrMagnitude < 0.001)
			{
				this.twoHandRotationOffsetAxis = base.transform.right;
				this.twoHandRotationOffsetAngle = 0f;
			}
			else
			{
				this.twoHandRotationOffsetAngle = Vector3.SignedAngle(handsVector, base.transform.right, this.twoHandRotationOffsetAxis);
			}
		}
		this.rb.isKinematic = true;
		this.rb.useGravity = false;
		this.ridersMaterialOverideIndex = 0;
		if (this.cosmeticMaterialOverrides.Length != 0)
		{
			VRRig offlineVRRig = this.cachedRig;
			if (offlineVRRig == null)
			{
				offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			}
			if (offlineVRRig != null)
			{
				for (int i = 0; i < this.cosmeticMaterialOverrides.Length; i++)
				{
					if (this.cosmeticMaterialOverrides[i].cosmeticName != null && offlineVRRig.cosmeticSet != null && offlineVRRig.cosmeticSet.HasItem(this.cosmeticMaterialOverrides[i].cosmeticName))
					{
						this.ridersMaterialOverideIndex = i + 1;
						break;
					}
				}
			}
		}
		this.infectedState = false;
		if (GorillaGameManager.instance as GorillaTagManager != null)
		{
			this.infectedState = this.syncedState.tagged;
		}
		if (this.infectedState)
		{
			this.leafMesh.material = this.GetInfectedMaterial();
		}
		else
		{
			this.leafMesh.material = this.GetMaterialFromIndex((byte)this.ridersMaterialOverideIndex);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment.GetType() == typeof(GliderHoldable) && EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment.GetType() == typeof(GliderHoldable) && EquipmentInteractor.instance.leftHandHeldEquipment != EquipmentInteractor.instance.rightHandHeldEquipment)
		{
			this.holdingTwoGliders = true;
		}
	}

	// Token: 0x06004FE9 RID: 20457 RVA: 0x001A6F7C File Offset: 0x001A517C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		this.holdingTwoGliders = false;
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		if (this.leftHold.active && this.rightHold.active)
		{
			if (flag)
			{
				this.rightHold.Activate(this.rightHold.transform, base.transform, this.ClosestPointInHandle(this.rightHold.transform.position, this.handle));
			}
			else
			{
				this.leftHold.Activate(this.leftHold.transform, base.transform, this.ClosestPointInHandle(this.leftHold.transform.position, this.handle));
			}
		}
		Vector3 averageVelocity = GTPlayer.Instance.GetHandVelocityTracker(flag).GetAverageVelocity(true, 0.15f, false);
		(flag ? this.leftHold : this.rightHold).Deactivate();
		EquipmentInteractor.instance.UpdateHandEquipment(null, flag);
		if (!this.leftHold.active && !this.rightHold.active)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyDropped;
			this.audioLevel = 0f;
			this.riderId = -1;
			this.cachedRig = null;
			this.subtlePlayerPitch = 0f;
			this.subtlePlayerRoll = 0f;
			this.leftHoldPositionLocal = null;
			this.rightHoldPositionLocal = null;
			this.ridersMaterialOverideIndex = 0;
			if (base.IsMine || !NetworkSystem.Instance.InRoom)
			{
				this.rb.isKinematic = false;
				this.rb.useGravity = true;
				this.rb.linearVelocity = averageVelocity;
				this.syncedState.riderId = -1;
				this.syncedState.tagged = false;
				this.syncedState.materialIndex = 0;
				this.syncedState.position = base.transform.position;
				this.syncedState.rotation = base.transform.rotation;
				this.syncedState.audioLevel = 0;
			}
			this.leafMesh.material = this.baseLeafMaterial;
		}
		return true;
	}

	// Token: 0x06004FEA RID: 20458 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06004FEB RID: 20459 RVA: 0x001A7194 File Offset: 0x001A5394
	public void FixedUpdate()
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		GTPlayer instance = GTPlayer.Instance;
		if (this.holdingTwoGliders)
		{
			instance.AddForce(Physics.gravity, ForceMode.Acceleration);
			return;
		}
		if (this.leftHold.active || this.rightHold.active)
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.previousVelocity = this.currentVelocity;
			this.currentVelocity = instance.RigidbodyVelocity;
			float magnitude = this.currentVelocity.magnitude;
			this.accelerationAverage.AddSample((this.currentVelocity - this.previousVelocity) / Time.fixedDeltaTime, Time.fixedTime);
			float rollAngle180Wrapping = this.GetRollAngle180Wrapping();
			float angle = this.liftIncreaseVsRoll.Evaluate(Mathf.Clamp01(Mathf.Abs(rollAngle180Wrapping / 180f))) * this.liftIncreaseVsRollMaxAngle;
			Vector3 vector = Vector3.RotateTowards(this.currentVelocity, Quaternion.AngleAxis(angle, -base.transform.right) * base.transform.forward * magnitude, this.pitchVelocityFollowRateAngle * 0.017453292f * fixedDeltaTime, this.pitchVelocityFollowRateMagnitude * fixedDeltaTime);
			Vector3 a = vector - this.currentVelocity;
			float num = this.NormalizeAngle180(Vector3.SignedAngle(Vector3.ProjectOnPlane(this.currentVelocity, base.transform.right), base.transform.forward, base.transform.right));
			if (num > 90f)
			{
				num = Mathf.Lerp(0f, 90f, Mathf.InverseLerp(180f, 90f, num));
			}
			else if (num < -90f)
			{
				num = Mathf.Lerp(0f, -90f, Mathf.InverseLerp(-180f, -90f, num));
			}
			float time = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f, 90f, num));
			Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f, 90f, this.pitch));
			float d = this.liftVsAttack.Evaluate(time);
			instance.AddForce(a * d, ForceMode.VelocityChange);
			float num2 = this.dragVsAttack.Evaluate(time);
			float num3 = (this.syncedState.riderId != -1 && this.syncedState.materialIndex == 1) ? (this.dragVsSpeedMaxSpeed + this.infectedSpeedIncrease) : this.dragVsSpeedMaxSpeed;
			float num4 = this.dragVsSpeed.Evaluate(Mathf.Clamp01(magnitude / num3));
			float d2 = Mathf.Clamp01(num2 * this.attackDragFactor + num4 * this.dragVsSpeedDragFactor);
			instance.AddForce(-this.currentVelocity * d2, ForceMode.Acceleration);
			if (this.pitch > 0f && this.currentVelocity.y > 0f && (this.currentVelocity - this.previousVelocity).y > 0f)
			{
				float a2 = Mathf.InverseLerp(0f, this.pullUpLiftActivationVelocity, this.currentVelocity.y);
				float b = Mathf.InverseLerp(0f, this.pullUpLiftActivationAcceleration, (this.currentVelocity - this.previousVelocity).y / fixedDeltaTime);
				float d3 = Mathf.Min(a2, b);
				instance.AddForce(-Physics.gravity * this.pullUpLiftBonus * d3, ForceMode.Acceleration);
			}
			if (Vector3.Dot(vector, Physics.gravity) > 0f)
			{
				instance.AddForce(-Physics.gravity * this.gravityCompensation, ForceMode.Acceleration);
				return;
			}
		}
		else
		{
			Vector3 a3 = this.WindResistanceForceOffset(base.transform.up, Vector3.down);
			Vector3 position = base.transform.position - a3 * this.gravityUprightTorqueMultiplier;
			this.rb.AddForceAtPosition(-this.fallingGravityReduction * Physics.gravity * this.rb.mass, position, ForceMode.Force);
		}
	}

	// Token: 0x06004FEC RID: 20460 RVA: 0x001A759C File Offset: 0x001A579C
	public void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (base.IsMine || !NetworkSystem.Instance.InRoom || this.pendingOwnershipRequest)
		{
			this.AuthorityUpdate(deltaTime);
			return;
		}
		this.RemoteSyncUpdate(deltaTime);
	}

	// Token: 0x06004FED RID: 20461 RVA: 0x001A75DC File Offset: 0x001A57DC
	private void AuthorityUpdate(float dt)
	{
		if (!this.leftHold.active && !this.rightHold.active)
		{
			this.AuthorityUpdateUnheld(dt);
		}
		else if (this.leftHold.active || this.rightHold.active)
		{
			this.AuthorityUpdateHeld(dt);
		}
		this.syncedState.audioLevel = (byte)Mathf.FloorToInt(255f * this.audioLevel);
	}

	// Token: 0x06004FEE RID: 20462 RVA: 0x001A764C File Offset: 0x001A584C
	private void AuthorityUpdateHeld(float dt)
	{
		if (this.gliderState != GliderHoldable.GliderState.LocallyHeld)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyHeld;
		}
		this.rb.isKinematic = true;
		this.lastHeldTime = Time.time;
		if (this.leftHold.active)
		{
			this.leftHold.holdLocalPos = Vector3.Lerp(Vector3.zero, this.leftHold.holdLocalPos, Mathf.Exp(-5f * dt));
		}
		if (this.rightHold.active)
		{
			this.rightHold.holdLocalPos = Vector3.Lerp(Vector3.zero, this.rightHold.holdLocalPos, Mathf.Exp(-5f * dt));
		}
		Vector3 a = Vector3.zero;
		if (this.leftHold.active && this.rightHold.active)
		{
			a = (this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos)) * 0.5f;
		}
		else if (this.leftHold.active)
		{
			a = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos);
		}
		else if (this.rightHold.active)
		{
			a = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos);
		}
		this.UpdateGliderPosition();
		float magnitude = this.currentVelocity.magnitude;
		if (this.setMaxHandSlipDuringFlight && magnitude > this.maxSlipOverrideSpeedThreshold)
		{
			if (this.leftHold.active)
			{
				GTPlayer.Instance.SetLeftMaximumSlipThisFrame();
			}
			if (this.rightHold.active)
			{
				GTPlayer.Instance.SetRightMaximumSlipThisFrame();
			}
		}
		bool flag = false;
		GorillaTagManager gorillaTagManager = GorillaGameManager.instance as GorillaTagManager;
		if (gorillaTagManager != null)
		{
			flag = gorillaTagManager.IsInfected(NetworkSystem.Instance.LocalPlayer);
		}
		bool flag2 = flag != this.infectedState;
		this.infectedState = flag;
		if (flag2)
		{
			if (this.infectedState)
			{
				this.leafMesh.material = this.GetInfectedMaterial();
			}
			else
			{
				this.leafMesh.material = this.GetMaterialFromIndex(this.syncedState.materialIndex);
			}
		}
		Vector3 average = this.accelerationAverage.GetAverage();
		this.accelerationSmoothed = Mathf.Lerp(average.magnitude, this.accelerationSmoothed, Mathf.Exp(-this.accelSmoothingFollowRateExp * dt));
		float num = Mathf.InverseLerp(this.hapticMaxSpeedInputRange.x, this.hapticMaxSpeedInputRange.y, magnitude);
		float num2 = Mathf.InverseLerp(this.hapticAccelInputRange.x, this.hapticAccelInputRange.y, this.accelerationSmoothed);
		float num3 = Mathf.InverseLerp(this.hapticSpeedInputRange.x, this.hapticSpeedInputRange.y, magnitude);
		this.UpdateAudioSource(this.calmAudio, num * this.audioVolumeMultiplier);
		this.UpdateAudioSource(this.activeAudio, num2 * num * this.audioVolumeMultiplier);
		if (this.infectedState)
		{
			this.UpdateAudioSource(this.whistlingAudio, Mathf.InverseLerp(this.whistlingAudioSpeedInputRange.x, this.whistlingAudioSpeedInputRange.y, magnitude) * num2 * num * this.audioVolumeMultiplier);
		}
		else
		{
			this.UpdateAudioSource(this.whistlingAudio, 0f);
		}
		float amplitude = Mathf.Max(num2 * this.hapticAccelOutputMax * num, num3 * this.hapticSpeedOutputMax);
		if (this.rightHold.active)
		{
			GorillaTagger.Instance.DoVibration(XRNode.RightHand, amplitude, dt);
		}
		if (this.leftHold.active)
		{
			GorillaTagger.Instance.DoVibration(XRNode.LeftHand, amplitude, dt);
		}
		Vector3 origin = this.handle.transform.position + this.handle.transform.rotation * new Vector3(0f, 0f, 1f);
		if (Time.frameCount % 2 == 0)
		{
			Vector3 direction = this.handle.transform.rotation * new Vector3(-0.707f, 0f, 0.707f);
			RaycastHit raycastHit;
			if (this.leftWhooshStartTime < Time.time - this.whooshSoundRetriggerThreshold && magnitude > this.whooshSpeedThresholdInput.x && Physics.Raycast(new Ray(origin, direction), out raycastHit, this.whooshCheckDistance, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				this.leftWhooshStartTime = Time.time;
				this.leftWhooshHitPoint = raycastHit.point;
				this.leftWhooshAudio.GTStop();
				this.leftWhooshAudio.volume = Mathf.Lerp(this.whooshVolumeOutput.x, this.whooshVolumeOutput.y, Mathf.InverseLerp(this.whooshSpeedThresholdInput.x, this.whooshSpeedThresholdInput.y, magnitude));
				this.leftWhooshAudio.GTPlay();
			}
		}
		else
		{
			Vector3 direction2 = this.handle.transform.rotation * new Vector3(0.707f, 0f, 0.707f);
			RaycastHit raycastHit2;
			if (this.rightWhooshStartTime < Time.time - this.whooshSoundRetriggerThreshold && magnitude > this.whooshSpeedThresholdInput.x && Physics.Raycast(new Ray(origin, direction2), out raycastHit2, this.whooshCheckDistance, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				this.rightWhooshStartTime = Time.time;
				this.rightWhooshHitPoint = raycastHit2.point;
				this.rightWhooshAudio.GTStop();
				this.rightWhooshAudio.volume = Mathf.Lerp(this.whooshVolumeOutput.x, this.whooshVolumeOutput.y, Mathf.InverseLerp(this.whooshSpeedThresholdInput.x, this.whooshSpeedThresholdInput.y, magnitude));
				this.rightWhooshAudio.GTPlay();
			}
		}
		Vector3 headCenterPosition = GTPlayer.Instance.HeadCenterPosition;
		if (this.leftWhooshStartTime > Time.time - this.whooshSoundDuration)
		{
			this.leftWhooshAudio.transform.position = this.leftWhooshHitPoint;
		}
		else
		{
			this.leftWhooshAudio.transform.localPosition = new Vector3(-this.whooshAudioPositionOffset.x, this.whooshAudioPositionOffset.y, this.whooshAudioPositionOffset.z);
		}
		if (this.rightWhooshStartTime > Time.time - this.whooshSoundDuration)
		{
			this.rightWhooshAudio.transform.position = this.rightWhooshHitPoint;
		}
		else
		{
			this.rightWhooshAudio.transform.localPosition = new Vector3(this.whooshAudioPositionOffset.x, this.whooshAudioPositionOffset.y, this.whooshAudioPositionOffset.z);
		}
		if (this.extendTagRangeInFlight)
		{
			float tagRadiusOverrideThisFrame = Mathf.Lerp(this.tagRangeOutput.x, this.tagRangeOutput.y, Mathf.InverseLerp(this.tagRangeSpeedInput.x, this.tagRangeSpeedInput.y, magnitude));
			GorillaTagger.Instance.SetTagRadiusOverrideThisFrame(tagRadiusOverrideThisFrame);
			if (this.debugDrawTagRange)
			{
				GorillaTagger.Instance.DebugDrawTagCasts(Color.yellow);
			}
		}
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.right, Vector3.up).normalized;
		Vector3 normalized2 = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
		float num4 = -Vector3.Dot(a - this.handle.transform.position, normalized2);
		Vector3 b = this.handle.transform.position - normalized2 * (this.riderPosRange.y * 0.5f + this.riderPosRangeOffset + num4);
		float num5 = Vector3.Dot(headCenterPosition - b, normalized);
		float num6 = Vector3.Dot(headCenterPosition - b, normalized2);
		num5 /= this.riderPosRange.x * 0.5f;
		num6 /= this.riderPosRange.y * 0.5f;
		this.riderPosition.x = Mathf.Sign(num5) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.riderPosRangeNormalizedDeadzone.x, 1f, Mathf.Abs(num5)));
		this.riderPosition.y = Mathf.Sign(num6) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.riderPosRangeNormalizedDeadzone.y, 1f, Mathf.Abs(num6)));
		Vector3 vector;
		Vector3 vector2;
		if (this.leftHold.active && this.rightHold.active)
		{
			vector = this.leftHold.transform.position;
			this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector));
			vector2 = this.rightHold.transform.position;
			this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector2));
		}
		else if (this.leftHold.active)
		{
			vector = this.leftHold.transform.position;
			this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector));
			Vector3 vector3 = vector + this.leftHold.transform.forward * this.oneHandSimulatedHoldOffset.x;
			if (this.rightHoldPositionLocal != null)
			{
				this.rightHoldPositionLocal = new Vector3?(Vector3.Lerp(GTPlayer.Instance.transform.InverseTransformPoint(vector3), this.rightHoldPositionLocal.Value, Mathf.Exp(-5f * dt)));
				vector2 = GTPlayer.Instance.transform.TransformPoint(this.rightHoldPositionLocal.Value);
			}
			else
			{
				vector2 = vector3;
				this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector2));
			}
		}
		else
		{
			vector2 = this.rightHold.transform.position;
			this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector2));
			Vector3 vector4 = vector2 + this.rightHold.transform.forward * this.oneHandSimulatedHoldOffset.x;
			if (this.leftHoldPositionLocal != null)
			{
				this.leftHoldPositionLocal = new Vector3?(Vector3.Lerp(GTPlayer.Instance.transform.InverseTransformPoint(vector4), this.leftHoldPositionLocal.Value, Mathf.Exp(-5f * dt)));
				vector = GTPlayer.Instance.transform.TransformPoint(this.leftHoldPositionLocal.Value);
			}
			else
			{
				vector = vector4;
				this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector));
			}
		}
		Vector3 forward;
		Vector3 vector5;
		this.GetHandsOrientationVectors(vector, vector2, GTPlayer.Instance.headCollider.transform, false, out forward, out vector5);
		float num7 = this.riderPosition.y * this.riderPosDirectPitchMax;
		if (!this.leftHold.active || !this.rightHold.active)
		{
			num7 *= this.oneHandPitchMultiplier;
		}
		Spring.CriticalSpringDamperExact(ref this.pitch, ref this.pitchVel, num7, 0f, this.pitchHalfLife, dt);
		this.pitch = Mathf.Clamp(this.pitch, this.pitchMinMax.x, this.pitchMinMax.y);
		Quaternion rhs = Quaternion.AngleAxis(this.pitch, Vector3.right);
		this.twoHandRotationOffsetAngle = Mathf.Lerp(0f, this.twoHandRotationOffsetAngle, Mathf.Exp(-8f * dt));
		Vector3 upwards = this.twoHandGliderInversionOnYawInsteadOfRoll ? vector5 : Vector3.up;
		Quaternion lhs = Quaternion.AngleAxis(this.twoHandRotationOffsetAngle, this.twoHandRotationOffsetAxis) * Quaternion.LookRotation(forward, upwards) * Quaternion.AngleAxis(-90f, Vector3.up);
		float num8 = (this.leftHold.active && this.rightHold.active) ? this.twoHandRotationRateExp : this.oneHandRotationRateExp;
		base.transform.rotation = Quaternion.Slerp(lhs * rhs, base.transform.rotation, Mathf.Exp(-num8 * dt));
		if (this.subtlePlayerPitchActive || this.subtlePlayerRollActive)
		{
			float a2 = Mathf.InverseLerp(this.subtlePlayerRotationSpeedRampMinMax.x, this.subtlePlayerRotationSpeedRampMinMax.y, this.currentVelocity.magnitude);
			Quaternion rhs2 = Quaternion.identity;
			if (this.subtlePlayerRollActive)
			{
				float num9 = this.GetRollAngle180Wrapping();
				if (num9 > 90f)
				{
					num9 = Mathf.Lerp(0f, 90f, Mathf.InverseLerp(180f, 90f, num9));
				}
				else if (num9 < -90f)
				{
					num9 = Mathf.Lerp(0f, -90f, Mathf.InverseLerp(-180f, -90f, num9));
				}
				Vector3 normalized3 = new Vector3(this.currentVelocity.x, 0f, this.currentVelocity.z).normalized;
				Vector3 vector6 = new Vector3(average.x, 0f, average.z);
				float num10 = Vector3.Dot(vector6 - Vector3.Dot(vector6, normalized3) * normalized3, Vector3.Cross(normalized3, Vector3.up));
				this.turnAccelerationSmoothed = Mathf.Lerp(num10, this.turnAccelerationSmoothed, Mathf.Exp(-this.accelSmoothingFollowRateExp * dt));
				float b2 = 0f;
				if (num10 * num9 > 0f)
				{
					b2 = Mathf.InverseLerp(this.subtlePlayerRollAccelMinMax.x, this.subtlePlayerRollAccelMinMax.y, Mathf.Abs(this.turnAccelerationSmoothed));
				}
				float a3 = num9 * this.subtlePlayerRollFactor * Mathf.Min(a2, b2);
				this.subtlePlayerRoll = Mathf.Lerp(a3, this.subtlePlayerRoll, Mathf.Exp(-this.subtlePlayerRollRateExp * dt));
				rhs2 = Quaternion.AngleAxis(this.subtlePlayerRoll, base.transform.forward);
			}
			Quaternion lhs2 = Quaternion.identity;
			if (this.subtlePlayerPitchActive)
			{
				float a4 = this.pitch * this.subtlePlayerPitchFactor * Mathf.Min(a2, 1f);
				this.subtlePlayerPitch = Mathf.Lerp(a4, this.subtlePlayerPitch, Mathf.Exp(-this.subtlePlayerPitchRateExp * dt));
				lhs2 = Quaternion.AngleAxis(this.subtlePlayerPitch, -base.transform.right);
			}
			Quaternion quaternion = lhs2 * rhs2;
			GTPlayerTransform.ApplyRotationOverride(quaternion, Time.frameCount);
		}
		this.UpdateGliderPosition();
		if (this.syncedState.riderId != NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.riderId = (this.syncedState.riderId = NetworkSystem.Instance.LocalPlayer.ActorNumber);
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		this.syncedState.tagged = this.infectedState;
		this.syncedState.materialIndex = (byte)this.ridersMaterialOverideIndex;
		if (this.cachedRig != null)
		{
			this.syncedState.position = this.cachedRig.transform.InverseTransformPoint(base.transform.position);
			this.syncedState.rotation = Quaternion.Inverse(this.cachedRig.transform.rotation) * base.transform.rotation;
		}
		else
		{
			Debug.LogError("Glider failed to get a reference to the local player's VRRig while the player was flying", this);
		}
		this.audioLevel = num2 * num;
		if (this.OutOfBounds)
		{
			this.Respawn();
		}
		if (this.leftHold.active && EquipmentInteractor.instance.leftHandHeldEquipment != this)
		{
			this.OnRelease(null, EquipmentInteractor.instance.leftHand);
		}
		if (this.rightHold.active && EquipmentInteractor.instance.rightHandHeldEquipment != this)
		{
			this.OnRelease(null, EquipmentInteractor.instance.rightHand);
		}
	}

	// Token: 0x06004FEF RID: 20463 RVA: 0x001A85F8 File Offset: 0x001A67F8
	private void AuthorityUpdateUnheld(float dt)
	{
		this.syncedState.position = base.transform.position;
		this.syncedState.rotation = base.transform.rotation;
		if (this.gliderState != GliderHoldable.GliderState.LocallyDropped)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyDropped;
			this.syncedState.riderId = -1;
			this.syncedState.materialIndex = 0;
			this.syncedState.tagged = false;
			this.leafMesh.material = this.baseLeafMaterial;
		}
		if (this.audioLevel * this.audioVolumeMultiplier > 0.001f)
		{
			this.audioLevel = Mathf.Lerp(0f, this.audioLevel, Mathf.Exp(-2f * dt));
			this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.whistlingAudio, this.audioLevel * this.audioVolumeMultiplier);
		}
		if (this.OutOfBounds || (this.lastHeldTime > 0f && this.lastHeldTime < Time.time - this.maxDroppedTimeToRespawn))
		{
			this.Respawn();
		}
	}

	// Token: 0x06004FF0 RID: 20464 RVA: 0x001A872C File Offset: 0x001A692C
	private void RemoteSyncUpdate(float dt)
	{
		this.rb.isKinematic = true;
		int num = this.syncedState.riderId;
		bool flag = this.riderId != num;
		if (flag)
		{
			this.riderId = num;
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		if (this.riderId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.cachedRig = null;
			this.syncedState.riderId = -1;
			this.syncedState.materialIndex = 0;
			this.syncedState.audioLevel = 0;
		}
		if (this.syncedState.riderId == -1)
		{
			base.transform.position = Vector3.Lerp(this.syncedState.position, base.transform.position, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			base.transform.rotation = Quaternion.Slerp(this.syncedState.rotation, base.transform.rotation, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
		}
		else if (this.cachedRig != null)
		{
			this.positionLocalToVRRig = Vector3.Lerp(this.syncedState.position, this.positionLocalToVRRig, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			this.rotationLocalToVRRig = Quaternion.Slerp(this.syncedState.rotation, this.rotationLocalToVRRig, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			base.transform.position = this.cachedRig.transform.TransformPoint(this.positionLocalToVRRig);
			base.transform.rotation = this.cachedRig.transform.rotation * this.rotationLocalToVRRig;
		}
		bool flag2 = false;
		if (GorillaGameManager.instance as GorillaTagManager != null)
		{
			flag2 = this.syncedState.tagged;
		}
		bool flag3 = flag2 != this.infectedState;
		this.infectedState = flag2;
		if (flag3 || flag)
		{
			if (this.infectedState)
			{
				this.leafMesh.material = this.GetInfectedMaterial();
			}
			else
			{
				this.leafMesh.material = this.GetMaterialFromIndex(this.syncedState.materialIndex);
			}
		}
		float num2 = Mathf.Clamp01((float)this.syncedState.audioLevel / 255f);
		if (this.audioLevel != num2)
		{
			this.audioLevel = num2;
			if (this.syncedState.riderId != -1 && this.syncedState.tagged)
			{
				this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				this.UpdateAudioSource(this.whistlingAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				return;
			}
			this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.whistlingAudio, 0f);
		}
	}

	// Token: 0x06004FF1 RID: 20465 RVA: 0x001A8A30 File Offset: 0x001A6C30
	private VRRig getNewHolderRig(int riderId)
	{
		if (riderId >= 0)
		{
			NetPlayer netPlayer;
			if (riderId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				netPlayer = NetworkSystem.Instance.LocalPlayer;
			}
			else
			{
				netPlayer = NetworkSystem.Instance.GetPlayer(riderId);
			}
			RigContainer rigContainer;
			if (netPlayer != null && VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
			{
				return rigContainer.Rig;
			}
		}
		return null;
	}

	// Token: 0x06004FF2 RID: 20466 RVA: 0x001A8A88 File Offset: 0x001A6C88
	private Vector3 ClosestPointInHandle(Vector3 startingPoint, InteractionPoint interactionPoint)
	{
		CapsuleCollider component = interactionPoint.GetComponent<CapsuleCollider>();
		Vector3 vector = startingPoint;
		if (component != null)
		{
			Vector3 point = (component.direction == 0) ? Vector3.right : ((component.direction == 1) ? Vector3.up : Vector3.forward);
			Vector3 vector2 = component.transform.rotation * point;
			Vector3 vector3 = component.transform.position + component.transform.rotation * component.center;
			float d = Mathf.Clamp(Vector3.Dot(vector - vector3, vector2), -component.height * 0.5f, component.height * 0.5f);
			vector = vector3 + vector2 * d;
		}
		return vector;
	}

	// Token: 0x06004FF3 RID: 20467 RVA: 0x001A8B48 File Offset: 0x001A6D48
	private void UpdateGliderPosition()
	{
		if (this.leftHold.active && this.rightHold.active)
		{
			Vector3 a = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + base.transform.TransformVector(this.leftHold.handleLocalPos);
			Vector3 b = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos) + base.transform.TransformVector(this.rightHold.handleLocalPos);
			base.transform.position = (a + b) * 0.5f;
			return;
		}
		if (this.leftHold.active)
		{
			base.transform.position = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + base.transform.TransformVector(this.leftHold.handleLocalPos);
			return;
		}
		if (this.rightHold.active)
		{
			base.transform.position = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos) + base.transform.TransformVector(this.rightHold.handleLocalPos);
		}
	}

	// Token: 0x06004FF4 RID: 20468 RVA: 0x001A8CA0 File Offset: 0x001A6EA0
	private Vector3 GetHandsVector(Vector3 leftHandPos, Vector3 rightHandPos, Vector3 headPos, bool flipBasedOnFacingDir)
	{
		Vector3 vector = rightHandPos - leftHandPos;
		Vector3 rhs = (rightHandPos + leftHandPos) * 0.5f - headPos;
		Vector3 normalized = Vector3.Cross(Vector3.up, rhs).normalized;
		if (flipBasedOnFacingDir && Vector3.Dot(vector, normalized) < 0f)
		{
			vector = -vector;
		}
		return vector;
	}

	// Token: 0x06004FF5 RID: 20469 RVA: 0x001A8CFC File Offset: 0x001A6EFC
	private void GetHandsOrientationVectors(Vector3 leftHandPos, Vector3 rightHandPos, Transform head, bool flipBasedOnFacingDir, out Vector3 handsVector, out Vector3 handsUpVector)
	{
		handsVector = rightHandPos - leftHandPos;
		float magnitude = handsVector.magnitude;
		handsVector /= Mathf.Max(magnitude, 0.001f);
		Vector3 position = head.position;
		float d = 1f;
		Vector3 planeNormal = (Vector3.Dot(head.right, handsVector) < 0f) ? handsVector : (-handsVector);
		Vector3 normalized = Vector3.ProjectOnPlane(-head.forward, planeNormal).normalized;
		Vector3 a = normalized * d + position;
		Vector3 a2 = (leftHandPos + rightHandPos) * 0.5f;
		Vector3 a3 = Vector3.ProjectOnPlane(a2 - head.position, Vector3.up);
		float magnitude2 = a3.magnitude;
		a3 /= Mathf.Max(magnitude2, 0.001f);
		Vector3 normalized2 = Vector3.ProjectOnPlane(-base.transform.forward, Vector3.up).normalized;
		Vector3 a4 = -a3 * d + position;
		float num = Vector3.Dot(normalized2, -a3);
		float num2 = Vector3.Dot(normalized2, normalized);
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0f)
		{
			num = Mathf.Abs(num);
			num2 = Mathf.Abs(num2);
		}
		num = Mathf.Max(num, 0f);
		num2 = Mathf.Max(num2, 0f);
		Vector3 b = (a4 * num + a * num2) / Mathf.Max(num + num2, 0.001f);
		Vector3 vector = a2 - b;
		Vector3 normalized3 = Vector3.Cross(Vector3.up, vector).normalized;
		if (flipBasedOnFacingDir && Vector3.Dot(handsVector, normalized3) < 0f)
		{
			handsVector = -handsVector;
		}
		handsUpVector = Vector3.Cross(Vector3.ProjectOnPlane(vector, Vector3.up), handsVector).normalized;
	}

	// Token: 0x06004FF6 RID: 20470 RVA: 0x001A8F27 File Offset: 0x001A7127
	private Material GetMaterialFromIndex(byte materialIndex)
	{
		if (materialIndex < 1 || (int)materialIndex > this.cosmeticMaterialOverrides.Length)
		{
			return this.baseLeafMaterial;
		}
		return this.cosmeticMaterialOverrides[(int)(materialIndex - 1)].material;
	}

	// Token: 0x06004FF7 RID: 20471 RVA: 0x001A8F54 File Offset: 0x001A7154
	private float GetRollAngle180Wrapping()
	{
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
		float angle = Vector3.SignedAngle(Vector3.Cross(Vector3.up, normalized).normalized, base.transform.right, base.transform.forward);
		return this.NormalizeAngle180(angle);
	}

	// Token: 0x06004FF8 RID: 20472 RVA: 0x001A8FB5 File Offset: 0x001A71B5
	private float SignedAngleInPlane(Vector3 from, Vector3 to, Vector3 normal)
	{
		from = Vector3.ProjectOnPlane(from, normal);
		to = Vector3.ProjectOnPlane(to, normal);
		return Vector3.SignedAngle(from, to, normal);
	}

	// Token: 0x06004FF9 RID: 20473 RVA: 0x001A8FD1 File Offset: 0x001A71D1
	private float NormalizeAngle180(float angle)
	{
		angle = (angle + 180f) % 360f;
		if (angle < 0f)
		{
			angle += 360f;
		}
		return angle - 180f;
	}

	// Token: 0x06004FFA RID: 20474 RVA: 0x001A8FFC File Offset: 0x001A71FC
	private void UpdateAudioSource(AudioSource source, float level)
	{
		source.volume = level;
		if (!source.isPlaying && level > 0.01f)
		{
			source.GTPlay();
			return;
		}
		if (source.isPlaying && level < 0.01f && this.syncedState.riderId == -1)
		{
			source.GTStop();
		}
	}

	// Token: 0x06004FFB RID: 20475 RVA: 0x001A904B File Offset: 0x001A724B
	private Material GetInfectedMaterial()
	{
		if (GorillaGameManager.instance is GorillaFreezeTagManager)
		{
			return this.frozenLeafMaterial;
		}
		return this.infectedLeafMaterial;
	}

	// Token: 0x06004FFC RID: 20476 RVA: 0x001A9068 File Offset: 0x001A7268
	public void OnTriggerStay(Collider other)
	{
		GliderWindVolume component = other.GetComponent<GliderWindVolume>();
		if (component == null)
		{
			return;
		}
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		if (Time.frameCount == this.windVolumeForceAppliedFrame)
		{
			return;
		}
		if (this.leftHold.active || this.rightHold.active)
		{
			Vector3 accelFromVelocity = component.GetAccelFromVelocity(GTPlayer.Instance.RigidbodyVelocity);
			GTPlayer.Instance.AddForce(accelFromVelocity, ForceMode.Acceleration);
			this.windVolumeForceAppliedFrame = Time.frameCount;
			return;
		}
		Vector3 accelFromVelocity2 = component.GetAccelFromVelocity(this.rb.linearVelocity);
		Vector3 a = this.WindResistanceForceOffset(base.transform.up, component.WindDirection);
		Vector3 position = base.transform.position + a * this.windUprightTorqueMultiplier;
		this.rb.AddForceAtPosition(accelFromVelocity2 * this.rb.mass, position, ForceMode.Force);
		this.windVolumeForceAppliedFrame = Time.frameCount;
	}

	// Token: 0x06004FFD RID: 20477 RVA: 0x001A9166 File Offset: 0x001A7366
	private Vector3 WindResistanceForceOffset(Vector3 upDir, Vector3 windDir)
	{
		if (Vector3.Dot(upDir, windDir) < 0f)
		{
			upDir *= -1f;
		}
		return Vector3.ProjectOnPlane(upDir - windDir, upDir);
	}

	// Token: 0x1700077B RID: 1915
	// (get) Token: 0x06004FFE RID: 20478 RVA: 0x001A9190 File Offset: 0x001A7390
	// (set) Token: 0x06004FFF RID: 20479 RVA: 0x001A91BA File Offset: 0x001A73BA
	[Networked]
	[NetworkedWeaved(0, 11)]
	internal unsafe GliderHoldable.SyncedState Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GliderHoldable.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(GliderHoldable.SyncedState*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GliderHoldable.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(GliderHoldable.SyncedState*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06005000 RID: 20480 RVA: 0x001A91E8 File Offset: 0x001A73E8
	public override void ReadDataFusion()
	{
		int num = this.syncedState.riderId;
		this.syncedState = this.Data;
		if (num != this.syncedState.riderId)
		{
			this.positionLocalToVRRig = this.syncedState.position;
			this.rotationLocalToVRRig = this.syncedState.rotation;
		}
	}

	// Token: 0x06005001 RID: 20481 RVA: 0x001A923B File Offset: 0x001A743B
	public override void WriteDataFusion()
	{
		this.Data = this.syncedState;
	}

	// Token: 0x06005002 RID: 20482 RVA: 0x001A924C File Offset: 0x001A744C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		Player sender = info.Sender;
		PunNetPlayer punNetPlayer = (PunNetPlayer)this.ownershipGuard.actualOwner;
		if (sender != ((punNetPlayer != null) ? punNetPlayer.PlayerRef : null))
		{
			return;
		}
		int num = this.syncedState.riderId;
		this.syncedState.riderId = (int)stream.ReceiveNext();
		this.syncedState.tagged = (bool)stream.ReceiveNext();
		this.syncedState.materialIndex = (byte)stream.ReceiveNext();
		this.syncedState.audioLevel = (byte)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.syncedState.position.SetValueSafe(vector);
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		ref this.syncedState.rotation.SetValueSafe(quaternion);
		if (num != this.syncedState.riderId)
		{
			this.positionLocalToVRRig = this.syncedState.position;
			this.rotationLocalToVRRig = this.syncedState.rotation;
		}
	}

	// Token: 0x06005003 RID: 20483 RVA: 0x001A9354 File Offset: 0x001A7554
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		object sender = info.Sender;
		NetPlayer actualOwner = this.ownershipGuard.actualOwner;
		if (!sender.Equals((actualOwner != null) ? actualOwner.GetPlayerRef() : null))
		{
			return;
		}
		stream.SendNext(this.syncedState.riderId);
		stream.SendNext(this.syncedState.tagged);
		stream.SendNext(this.syncedState.materialIndex);
		stream.SendNext(this.syncedState.audioLevel);
		stream.SendNext(this.syncedState.position);
		stream.SendNext(this.syncedState.rotation);
	}

	// Token: 0x06005004 RID: 20484 RVA: 0x001A940F File Offset: 0x001A760F
	private IEnumerator ReenableOwnershipRequest()
	{
		yield return new WaitForSeconds(3f);
		this.pendingOwnershipRequest = false;
		yield break;
	}

	// Token: 0x06005005 RID: 20485 RVA: 0x001A9420 File Offset: 0x001A7620
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer == NetworkSystem.Instance.LocalPlayer)
		{
			this.pendingOwnershipRequest = false;
			if (!this.leftHold.active && !this.rightHold.active && (this.spawnPosition - base.transform.position).sqrMagnitude > 1f)
			{
				this.rb.isKinematic = false;
				this.rb.WakeUp();
				this.lastHeldTime = Time.time;
			}
		}
	}

	// Token: 0x06005006 RID: 20486 RVA: 0x001A94A2 File Offset: 0x001A76A2
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return !base.IsMine || !NetworkSystem.Instance.InRoom || (!this.leftHold.active && !this.rightHold.active);
	}

	// Token: 0x06005007 RID: 20487 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x06005008 RID: 20488 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x06005009 RID: 20489 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x0600500D RID: 20493 RVA: 0x001A9991 File Offset: 0x001A7B91
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x0600500E RID: 20494 RVA: 0x001A99A9 File Offset: 0x001A7BA9
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x0400619F RID: 24991
	[Header("Flight Settings")]
	[SerializeField]
	private Vector2 pitchMinMax = new Vector2(-80f, 80f);

	// Token: 0x040061A0 RID: 24992
	[SerializeField]
	private Vector2 rollMinMax = new Vector2(-70f, 70f);

	// Token: 0x040061A1 RID: 24993
	[SerializeField]
	private float pitchHalfLife = 0.2f;

	// Token: 0x040061A2 RID: 24994
	public Vector2 pitchVelocityTargetMinMax = new Vector2(-60f, 60f);

	// Token: 0x040061A3 RID: 24995
	public Vector2 pitchVelocityRampTimeMinMax = new Vector2(-1f, 1f);

	// Token: 0x040061A4 RID: 24996
	[SerializeField]
	private float pitchVelocityFollowRateAngle = 60f;

	// Token: 0x040061A5 RID: 24997
	[SerializeField]
	private float pitchVelocityFollowRateMagnitude = 5f;

	// Token: 0x040061A6 RID: 24998
	[SerializeField]
	private AnimationCurve liftVsAttack = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040061A7 RID: 24999
	[SerializeField]
	private AnimationCurve dragVsAttack = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040061A8 RID: 25000
	[SerializeField]
	[Range(0f, 1f)]
	public float attackDragFactor = 0.1f;

	// Token: 0x040061A9 RID: 25001
	[SerializeField]
	private AnimationCurve dragVsSpeed = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040061AA RID: 25002
	[SerializeField]
	public float dragVsSpeedMaxSpeed = 30f;

	// Token: 0x040061AB RID: 25003
	[SerializeField]
	[Range(0f, 1f)]
	public float dragVsSpeedDragFactor = 0.2f;

	// Token: 0x040061AC RID: 25004
	[SerializeField]
	private AnimationCurve liftIncreaseVsRoll = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040061AD RID: 25005
	[SerializeField]
	private float liftIncreaseVsRollMaxAngle = 20f;

	// Token: 0x040061AE RID: 25006
	[SerializeField]
	[Range(0f, 1f)]
	private float gravityCompensation = 0.8f;

	// Token: 0x040061AF RID: 25007
	[Range(0f, 1f)]
	public float pullUpLiftBonus = 0.1f;

	// Token: 0x040061B0 RID: 25008
	public float pullUpLiftActivationVelocity = 1f;

	// Token: 0x040061B1 RID: 25009
	public float pullUpLiftActivationAcceleration = 3f;

	// Token: 0x040061B2 RID: 25010
	[Header("Body Positioning Control")]
	[SerializeField]
	private float riderPosDirectPitchMax = 70f;

	// Token: 0x040061B3 RID: 25011
	[SerializeField]
	private Vector2 riderPosRange = new Vector2(2.2f, 0.75f);

	// Token: 0x040061B4 RID: 25012
	[SerializeField]
	private float riderPosRangeOffset = 0.15f;

	// Token: 0x040061B5 RID: 25013
	[SerializeField]
	private Vector2 riderPosRangeNormalizedDeadzone = new Vector2(0.15f, 0.05f);

	// Token: 0x040061B6 RID: 25014
	[Header("Direct Handle Control")]
	[SerializeField]
	private float oneHandHoldRotationRate = 2f;

	// Token: 0x040061B7 RID: 25015
	private Vector3 oneHandSimulatedHoldOffset = new Vector3(0.5f, -0.35f, 0.25f);

	// Token: 0x040061B8 RID: 25016
	private float oneHandPitchMultiplier = 0.8f;

	// Token: 0x040061B9 RID: 25017
	[SerializeField]
	private float twoHandHoldRotationRate = 4f;

	// Token: 0x040061BA RID: 25018
	[SerializeField]
	private bool twoHandGliderInversionOnYawInsteadOfRoll;

	// Token: 0x040061BB RID: 25019
	[Header("Player Settings")]
	[SerializeField]
	private bool setMaxHandSlipDuringFlight = true;

	// Token: 0x040061BC RID: 25020
	[SerializeField]
	private float maxSlipOverrideSpeedThreshold = 5f;

	// Token: 0x040061BD RID: 25021
	[Header("Player Camera Rotation")]
	[SerializeField]
	private float subtlePlayerPitchFactor = 0.2f;

	// Token: 0x040061BE RID: 25022
	[SerializeField]
	private float subtlePlayerPitchRate = 2f;

	// Token: 0x040061BF RID: 25023
	[SerializeField]
	private float subtlePlayerRollFactor = 0.2f;

	// Token: 0x040061C0 RID: 25024
	[SerializeField]
	private float subtlePlayerRollRate = 2f;

	// Token: 0x040061C1 RID: 25025
	[SerializeField]
	private Vector2 subtlePlayerRotationSpeedRampMinMax = new Vector2(2f, 8f);

	// Token: 0x040061C2 RID: 25026
	[SerializeField]
	private Vector2 subtlePlayerRollAccelMinMax = new Vector2(0f, 30f);

	// Token: 0x040061C3 RID: 25027
	[SerializeField]
	private Vector2 subtlePlayerPitchAccelMinMax = new Vector2(0f, 10f);

	// Token: 0x040061C4 RID: 25028
	[SerializeField]
	private float accelSmoothingFollowRate = 2f;

	// Token: 0x040061C5 RID: 25029
	[Header("Haptics")]
	[SerializeField]
	private Vector2 hapticAccelInputRange = new Vector2(5f, 20f);

	// Token: 0x040061C6 RID: 25030
	[SerializeField]
	private float hapticAccelOutputMax = 0.35f;

	// Token: 0x040061C7 RID: 25031
	[SerializeField]
	private Vector2 hapticMaxSpeedInputRange = new Vector2(5f, 10f);

	// Token: 0x040061C8 RID: 25032
	[SerializeField]
	private Vector2 hapticSpeedInputRange = new Vector2(3f, 30f);

	// Token: 0x040061C9 RID: 25033
	[SerializeField]
	private float hapticSpeedOutputMax = 0.15f;

	// Token: 0x040061CA RID: 25034
	[SerializeField]
	private Vector2 whistlingAudioSpeedInputRange = new Vector2(15f, 30f);

	// Token: 0x040061CB RID: 25035
	[Header("Audio")]
	[SerializeField]
	private float audioVolumeMultiplier = 0.25f;

	// Token: 0x040061CC RID: 25036
	[SerializeField]
	private float infectedAudioVolumeMultiplier = 0.5f;

	// Token: 0x040061CD RID: 25037
	[SerializeField]
	private Vector2 whooshSpeedThresholdInput = new Vector2(10f, 25f);

	// Token: 0x040061CE RID: 25038
	[SerializeField]
	private Vector2 whooshVolumeOutput = new Vector2(0.2f, 0.75f);

	// Token: 0x040061CF RID: 25039
	[SerializeField]
	private float whooshCheckDistance = 2f;

	// Token: 0x040061D0 RID: 25040
	[Header("Tag Adjustment")]
	[SerializeField]
	private bool extendTagRangeInFlight = true;

	// Token: 0x040061D1 RID: 25041
	[SerializeField]
	private Vector2 tagRangeSpeedInput = new Vector2(5f, 20f);

	// Token: 0x040061D2 RID: 25042
	[SerializeField]
	private Vector2 tagRangeOutput = new Vector2(0.03f, 3f);

	// Token: 0x040061D3 RID: 25043
	[SerializeField]
	private bool debugDrawTagRange = true;

	// Token: 0x040061D4 RID: 25044
	[Header("Infected State")]
	[SerializeField]
	private float infectedSpeedIncrease = 5f;

	// Token: 0x040061D5 RID: 25045
	[Header("Glider Materials")]
	[SerializeField]
	private MeshRenderer leafMesh;

	// Token: 0x040061D6 RID: 25046
	[SerializeField]
	private Material baseLeafMaterial;

	// Token: 0x040061D7 RID: 25047
	[SerializeField]
	private Material infectedLeafMaterial;

	// Token: 0x040061D8 RID: 25048
	[SerializeField]
	private Material frozenLeafMaterial;

	// Token: 0x040061D9 RID: 25049
	[SerializeField]
	private GliderHoldable.CosmeticMaterialOverride[] cosmeticMaterialOverrides;

	// Token: 0x040061DA RID: 25050
	[Header("Network Syncing")]
	[SerializeField]
	private float networkSyncFollowRate = 2f;

	// Token: 0x040061DB RID: 25051
	[Header("Life Cycle")]
	[SerializeField]
	private Transform maxDistanceRespawnOrigin;

	// Token: 0x040061DC RID: 25052
	[SerializeField]
	private float maxDistanceBeforeRespawn = 180f;

	// Token: 0x040061DD RID: 25053
	[SerializeField]
	private float maxDroppedTimeToRespawn = 120f;

	// Token: 0x040061DE RID: 25054
	[Header("Rigidbody")]
	[SerializeField]
	private float windUprightTorqueMultiplier = 1f;

	// Token: 0x040061DF RID: 25055
	[SerializeField]
	private float gravityUprightTorqueMultiplier = 0.5f;

	// Token: 0x040061E0 RID: 25056
	[SerializeField]
	private float fallingGravityReduction = 0.1f;

	// Token: 0x040061E1 RID: 25057
	[Header("References")]
	[SerializeField]
	private AudioSource calmAudio;

	// Token: 0x040061E2 RID: 25058
	[SerializeField]
	private AudioSource activeAudio;

	// Token: 0x040061E3 RID: 25059
	[SerializeField]
	private AudioSource whistlingAudio;

	// Token: 0x040061E4 RID: 25060
	[SerializeField]
	private AudioSource leftWhooshAudio;

	// Token: 0x040061E5 RID: 25061
	[SerializeField]
	private AudioSource rightWhooshAudio;

	// Token: 0x040061E6 RID: 25062
	[SerializeField]
	private InteractionPoint handle;

	// Token: 0x040061E7 RID: 25063
	[SerializeField]
	private RequestableOwnershipGuard ownershipGuard;

	// Token: 0x040061E8 RID: 25064
	private bool subtlePlayerPitchActive = true;

	// Token: 0x040061E9 RID: 25065
	private bool subtlePlayerRollActive = true;

	// Token: 0x040061EA RID: 25066
	private float subtlePlayerPitch;

	// Token: 0x040061EB RID: 25067
	private float subtlePlayerRoll;

	// Token: 0x040061EC RID: 25068
	private float subtlePlayerPitchRateExp = 0.75f;

	// Token: 0x040061ED RID: 25069
	private float subtlePlayerRollRateExp = 0.025f;

	// Token: 0x040061EE RID: 25070
	private float defaultMaxDistanceBeforeRespawn = 180f;

	// Token: 0x040061EF RID: 25071
	private GliderHoldable.HoldingHand leftHold = new GliderHoldable.HoldingHand();

	// Token: 0x040061F0 RID: 25072
	private GliderHoldable.HoldingHand rightHold = new GliderHoldable.HoldingHand();

	// Token: 0x040061F1 RID: 25073
	private GliderHoldable.SyncedState syncedState;

	// Token: 0x040061F2 RID: 25074
	private Vector3 twoHandRotationOffsetAxis = Vector3.forward;

	// Token: 0x040061F3 RID: 25075
	private float twoHandRotationOffsetAngle;

	// Token: 0x040061F4 RID: 25076
	private Rigidbody rb;

	// Token: 0x040061F5 RID: 25077
	private Vector2 riderPosition = Vector2.zero;

	// Token: 0x040061F6 RID: 25078
	private Vector3 previousVelocity;

	// Token: 0x040061F7 RID: 25079
	private Vector3 currentVelocity;

	// Token: 0x040061F8 RID: 25080
	private float pitch;

	// Token: 0x040061F9 RID: 25081
	private float yaw;

	// Token: 0x040061FA RID: 25082
	private float roll;

	// Token: 0x040061FB RID: 25083
	private float pitchVel;

	// Token: 0x040061FC RID: 25084
	private float yawVel;

	// Token: 0x040061FD RID: 25085
	private float rollVel;

	// Token: 0x040061FE RID: 25086
	private float oneHandRotationRateExp;

	// Token: 0x040061FF RID: 25087
	private float twoHandRotationRateExp;

	// Token: 0x04006200 RID: 25088
	private Quaternion playerFacingRotationOffset = Quaternion.identity;

	// Token: 0x04006201 RID: 25089
	private const float accelAveragingWindow = 0.1f;

	// Token: 0x04006202 RID: 25090
	private AverageVector3 accelerationAverage = new AverageVector3(0.1f);

	// Token: 0x04006203 RID: 25091
	private float accelerationSmoothed;

	// Token: 0x04006204 RID: 25092
	private float turnAccelerationSmoothed;

	// Token: 0x04006205 RID: 25093
	private float accelSmoothingFollowRateExp = 1f;

	// Token: 0x04006206 RID: 25094
	private float networkSyncFollowRateExp = 2f;

	// Token: 0x04006207 RID: 25095
	private bool pendingOwnershipRequest;

	// Token: 0x04006208 RID: 25096
	private Vector3 positionLocalToVRRig = Vector3.zero;

	// Token: 0x04006209 RID: 25097
	private Quaternion rotationLocalToVRRig = Quaternion.identity;

	// Token: 0x0400620A RID: 25098
	private Coroutine reenableOwnershipRequestCoroutine;

	// Token: 0x0400620B RID: 25099
	private Vector3 spawnPosition;

	// Token: 0x0400620C RID: 25100
	private Quaternion spawnRotation;

	// Token: 0x0400620D RID: 25101
	private Vector3 skyJungleSpawnPostion;

	// Token: 0x0400620E RID: 25102
	private Quaternion skyJungleSpawnRotation;

	// Token: 0x0400620F RID: 25103
	private Transform skyJungleRespawnOrigin;

	// Token: 0x04006210 RID: 25104
	private float lastHeldTime = -1f;

	// Token: 0x04006211 RID: 25105
	private Vector3? leftHoldPositionLocal;

	// Token: 0x04006212 RID: 25106
	private Vector3? rightHoldPositionLocal;

	// Token: 0x04006213 RID: 25107
	private float whooshSoundDuration = 1f;

	// Token: 0x04006214 RID: 25108
	private float whooshSoundRetriggerThreshold = 0.5f;

	// Token: 0x04006215 RID: 25109
	private float leftWhooshStartTime = -1f;

	// Token: 0x04006216 RID: 25110
	private Vector3 leftWhooshHitPoint = Vector3.zero;

	// Token: 0x04006217 RID: 25111
	private Vector3 whooshAudioPositionOffset = new Vector3(0.5f, -0.25f, 0.5f);

	// Token: 0x04006218 RID: 25112
	private float rightWhooshStartTime = -1f;

	// Token: 0x04006219 RID: 25113
	private Vector3 rightWhooshHitPoint = Vector3.zero;

	// Token: 0x0400621A RID: 25114
	private int ridersMaterialOverideIndex;

	// Token: 0x0400621B RID: 25115
	private int windVolumeForceAppliedFrame = -1;

	// Token: 0x0400621C RID: 25116
	private bool holdingTwoGliders;

	// Token: 0x0400621D RID: 25117
	private GliderHoldable.GliderState gliderState;

	// Token: 0x0400621E RID: 25118
	private float audioLevel;

	// Token: 0x0400621F RID: 25119
	private int riderId = -1;

	// Token: 0x04006220 RID: 25120
	[SerializeField]
	private VRRig cachedRig;

	// Token: 0x04006221 RID: 25121
	private bool infectedState;

	// Token: 0x04006222 RID: 25122
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 11)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private GliderHoldable.SyncedState _Data;

	// Token: 0x02000C98 RID: 3224
	private enum GliderState
	{
		// Token: 0x04006224 RID: 25124
		LocallyHeld,
		// Token: 0x04006225 RID: 25125
		LocallyDropped,
		// Token: 0x04006226 RID: 25126
		RemoteSyncing
	}

	// Token: 0x02000C99 RID: 3225
	private class HoldingHand
	{
		// Token: 0x0600500F RID: 20495 RVA: 0x001A99C0 File Offset: 0x001A7BC0
		public void Activate(Transform handTransform, Transform gliderTransform, Vector3 worldGrabPoint)
		{
			this.active = true;
			this.transform = handTransform.transform;
			this.holdLocalPos = handTransform.InverseTransformPoint(worldGrabPoint);
			this.handleLocalPos = gliderTransform.InverseTransformVector(gliderTransform.position - worldGrabPoint);
			this.localHoldRotation = Quaternion.Inverse(handTransform.rotation) * gliderTransform.rotation;
		}

		// Token: 0x06005010 RID: 20496 RVA: 0x001A9A21 File Offset: 0x001A7C21
		public void Deactivate()
		{
			this.active = false;
			this.transform = null;
			this.holdLocalPos = Vector3.zero;
			this.handleLocalPos = Vector3.zero;
			this.localHoldRotation = Quaternion.identity;
		}

		// Token: 0x04006227 RID: 25127
		public bool active;

		// Token: 0x04006228 RID: 25128
		public Transform transform;

		// Token: 0x04006229 RID: 25129
		public Vector3 holdLocalPos;

		// Token: 0x0400622A RID: 25130
		public Vector3 handleLocalPos;

		// Token: 0x0400622B RID: 25131
		public Quaternion localHoldRotation;
	}

	// Token: 0x02000C9A RID: 3226
	[NetworkStructWeaved(11)]
	[StructLayout(LayoutKind.Explicit, Size = 44)]
	internal struct SyncedState : INetworkStruct
	{
		// Token: 0x06005012 RID: 20498 RVA: 0x001A9A52 File Offset: 0x001A7C52
		public void Init(Vector3 defaultPosition, Quaternion defaultRotation)
		{
			this.riderId = -1;
			this.materialIndex = 0;
			this.audioLevel = 0;
			this.position = defaultPosition;
			this.rotation = defaultRotation;
		}

		// Token: 0x06005013 RID: 20499 RVA: 0x001A9A77 File Offset: 0x001A7C77
		public SyncedState(int id = -1)
		{
			this.riderId = id;
			this.materialIndex = 0;
			this.audioLevel = 0;
			this.tagged = default(NetworkBool);
			this.position = default(Vector3);
			this.rotation = default(Quaternion);
		}

		// Token: 0x0400622C RID: 25132
		[FieldOffset(0)]
		public int riderId;

		// Token: 0x0400622D RID: 25133
		[FieldOffset(4)]
		public byte materialIndex;

		// Token: 0x0400622E RID: 25134
		[FieldOffset(8)]
		public byte audioLevel;

		// Token: 0x0400622F RID: 25135
		[FieldOffset(12)]
		public NetworkBool tagged;

		// Token: 0x04006230 RID: 25136
		[FieldOffset(16)]
		public Vector3 position;

		// Token: 0x04006231 RID: 25137
		[FieldOffset(28)]
		public Quaternion rotation;
	}

	// Token: 0x02000C9B RID: 3227
	[Serializable]
	private struct CosmeticMaterialOverride
	{
		// Token: 0x04006232 RID: 25138
		public string cosmeticName;

		// Token: 0x04006233 RID: 25139
		public Material material;
	}
}
