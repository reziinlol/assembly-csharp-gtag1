using System;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001225 RID: 4645
	public class RCVehicle : MonoBehaviour, ISpawnable
	{
		// Token: 0x17000B1F RID: 2847
		// (get) Token: 0x06007432 RID: 29746 RVA: 0x00260858 File Offset: 0x0025EA58
		public bool HasLocalAuthority
		{
			get
			{
				return !PhotonNetwork.InRoom || (this.networkSync != null && this.networkSync.photonView.IsMine);
			}
		}

		// Token: 0x06007433 RID: 29747 RVA: 0x00260884 File Offset: 0x0025EA84
		public virtual void WakeUpRemote(RCCosmeticNetworkSync sync)
		{
			this.networkSync = sync;
			this.hasNetworkSync = (sync != null);
			if (this.HasLocalAuthority)
			{
				return;
			}
			if (!base.enabled || !base.gameObject.activeSelf)
			{
				this.localStatePrev = RCVehicle.State.Disabled;
				base.enabled = true;
				base.gameObject.SetActive(true);
				this.RemoteUpdate(Time.deltaTime);
			}
		}

		// Token: 0x06007434 RID: 29748 RVA: 0x002608E8 File Offset: 0x0025EAE8
		public virtual void StartConnection(RCRemoteHoldable remote, RCCosmeticNetworkSync sync)
		{
			this.connectedRemote = remote;
			this.networkSync = sync;
			this.hasNetworkSync = (sync != null);
			base.enabled = true;
			base.gameObject.SetActive(true);
			this.useLeftDock = (remote.XRNode == XRNode.LeftHand);
			if (this.HasLocalAuthority && this.localState != RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginDocked();
			}
		}

		// Token: 0x06007435 RID: 29749 RVA: 0x00260949 File Offset: 0x0025EB49
		public virtual void EndConnection()
		{
			this.connectedRemote = null;
			this.activeInput = default(RCRemoteHoldable.RCInput);
			this.disconnectionTime = Time.time;
		}

		// Token: 0x06007436 RID: 29750 RVA: 0x0026096C File Offset: 0x0025EB6C
		protected virtual void ResetToSpawnPosition()
		{
			if (this.rb == null)
			{
				this.rb = base.GetComponent<Rigidbody>();
			}
			if (this.rb != null)
			{
				this.rb.isKinematic = true;
			}
			base.transform.parent = (this.useLeftDock ? this.leftDockParent : this.rightDockParent);
			base.transform.SetLocalPositionAndRotation(this.useLeftDock ? this.dockLeftOffset.pos : this.dockRightOffset.pos, this.useLeftDock ? this.dockLeftOffset.rot : this.dockRightOffset.rot);
			base.transform.localScale = (this.useLeftDock ? this.dockLeftOffset.scale : this.dockRightOffset.scale);
		}

		// Token: 0x06007437 RID: 29751 RVA: 0x00260A44 File Offset: 0x0025EC44
		protected virtual void AuthorityBeginDocked()
		{
			this.localState = (this.useLeftDock ? RCVehicle.State.DockedLeft : RCVehicle.State.DockedRight);
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
			this.waitingForTriggerRelease = true;
			this.ResetToSpawnPosition();
			if (this.connectedRemote == null)
			{
				this.SetDisabledState();
			}
		}

		// Token: 0x06007438 RID: 29752 RVA: 0x00260AB4 File Offset: 0x0025ECB4
		protected virtual void AuthorityBeginMobilization()
		{
			this.localState = RCVehicle.State.Mobilized;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
			base.transform.parent = null;
			this.rb.isKinematic = false;
		}

		// Token: 0x06007439 RID: 29753 RVA: 0x00260B10 File Offset: 0x0025ED10
		protected virtual void AuthorityBeginCrash()
		{
			this.localState = RCVehicle.State.Crashed;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
		}

		// Token: 0x0600743A RID: 29754 RVA: 0x00260B4C File Offset: 0x0025ED4C
		protected virtual void SetDisabledState()
		{
			this.localState = RCVehicle.State.Disabled;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.ResetToSpawnPosition();
			base.enabled = false;
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600743B RID: 29755 RVA: 0x00260B9E File Offset: 0x0025ED9E
		protected virtual void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x0600743C RID: 29756 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected virtual void OnEnable()
		{
		}

		// Token: 0x17000B20 RID: 2848
		// (get) Token: 0x0600743D RID: 29757 RVA: 0x00260BAC File Offset: 0x0025EDAC
		// (set) Token: 0x0600743E RID: 29758 RVA: 0x00260BB4 File Offset: 0x0025EDB4
		bool ISpawnable.IsSpawned { get; set; }

		// Token: 0x17000B21 RID: 2849
		// (get) Token: 0x0600743F RID: 29759 RVA: 0x00260BBD File Offset: 0x0025EDBD
		// (set) Token: 0x06007440 RID: 29760 RVA: 0x00260BC5 File Offset: 0x0025EDC5
		ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

		// Token: 0x06007441 RID: 29761 RVA: 0x00260BD0 File Offset: 0x0025EDD0
		void ISpawnable.OnSpawn(VRRig rig)
		{
			if (rig == null)
			{
				GTDev.LogError<string>("RCVehicle: Could not find VRRig in parents. If you are trying to make this a world item rather than a cosmetic then you'll have to refactor how it teleports back to the arms.", this, null);
				return;
			}
			string str;
			if (!GTHardCodedBones.TryGetBoneXforms(rig, out this._vrRigBones, out str))
			{
				Debug.LogError("RCVehicle: " + str, this);
				return;
			}
			if (this.leftDockParent == null && !GTHardCodedBones.TryGetBoneXform(this._vrRigBones, this.dockLeftOffset.bone, out this.leftDockParent))
			{
				GTDev.LogError<string>("RCVehicle: Could not find left dock transform.", this, null);
			}
			if (this.rightDockParent == null && !GTHardCodedBones.TryGetBoneXform(this._vrRigBones, this.dockRightOffset.bone, out this.rightDockParent))
			{
				GTDev.LogError<string>("RCVehicle: Could not find right dock transform.", this, null);
			}
		}

		// Token: 0x06007442 RID: 29762 RVA: 0x000028C5 File Offset: 0x00000AC5
		void ISpawnable.OnDespawn()
		{
		}

		// Token: 0x06007443 RID: 29763 RVA: 0x00260C8B File Offset: 0x0025EE8B
		protected virtual void OnDisable()
		{
			this.localState = RCVehicle.State.Disabled;
			this.localStatePrev = RCVehicle.State.Disabled;
		}

		// Token: 0x06007444 RID: 29764 RVA: 0x00260C9C File Offset: 0x0025EE9C
		public void ApplyRemoteControlInput(RCRemoteHoldable.RCInput rcInput)
		{
			this.activeInput.joystick.y = Mathf.Sign(rcInput.joystick.y) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.joystickDeadzone, 1f, Mathf.Abs(rcInput.joystick.y)));
			this.activeInput.joystick.x = Mathf.Sign(rcInput.joystick.x) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.joystickDeadzone, 1f, Mathf.Abs(rcInput.joystick.x)));
			this.activeInput.trigger = Mathf.Clamp(rcInput.trigger, -1f, 1f);
			this.activeInput.buttons = rcInput.buttons;
		}

		// Token: 0x06007445 RID: 29765 RVA: 0x00260D7C File Offset: 0x0025EF7C
		private void Update()
		{
			float deltaTime = Time.deltaTime;
			if (this.HasLocalAuthority)
			{
				this.AuthorityUpdate(deltaTime);
			}
			else
			{
				this.RemoteUpdate(deltaTime);
			}
			this.SharedUpdate(deltaTime);
			this.localStatePrev = this.localState;
		}

		// Token: 0x06007446 RID: 29766 RVA: 0x00260DBC File Offset: 0x0025EFBC
		protected virtual void AuthorityUpdate(float dt)
		{
			switch (this.localState)
			{
			default:
				if (this.localState != this.localStatePrev)
				{
					this.ResetToSpawnPosition();
				}
				if (this.connectedRemote == null)
				{
					this.SetDisabledState();
					return;
				}
				if (this.waitingForTriggerRelease && this.activeInput.trigger < 0.25f)
				{
					this.waitingForTriggerRelease = false;
				}
				if (!this.waitingForTriggerRelease && this.activeInput.trigger > 0.25f)
				{
					this.AuthorityBeginMobilization();
					return;
				}
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.networkSync != null)
				{
					this.networkSync.syncedState.position = base.transform.position;
					this.networkSync.syncedState.rotation = base.transform.rotation;
				}
				bool flag = (base.transform.position - this.leftDockParent.position).sqrMagnitude > this.maxRange * this.maxRange;
				bool flag2 = this.connectedRemote == null && Time.time - this.disconnectionTime > this.maxDisconnectionTime;
				if (flag || flag2)
				{
					this.AuthorityBeginCrash();
					return;
				}
				break;
			}
			case RCVehicle.State.Crashed:
				if (Time.time > this.stateStartTime + this.crashRespawnDelay)
				{
					this.AuthorityBeginDocked();
				}
				break;
			}
		}

		// Token: 0x06007447 RID: 29767 RVA: 0x00260F1C File Offset: 0x0025F11C
		protected virtual void RemoteUpdate(float dt)
		{
			if (this.networkSync == null)
			{
				this.SetDisabledState();
				return;
			}
			this.localState = (RCVehicle.State)this.networkSync.syncedState.state;
			switch (this.localState)
			{
			case RCVehicle.State.Disabled:
				this.SetDisabledState();
				break;
			default:
				if (this.localStatePrev != RCVehicle.State.DockedLeft)
				{
					this.useLeftDock = true;
					this.ResetToSpawnPosition();
					return;
				}
				break;
			case RCVehicle.State.DockedRight:
				if (this.localStatePrev != RCVehicle.State.DockedRight)
				{
					this.useLeftDock = false;
					this.ResetToSpawnPosition();
					return;
				}
				break;
			case RCVehicle.State.Mobilized:
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.rb.isKinematic = true;
					base.transform.parent = null;
				}
				base.transform.position = Vector3.Lerp(this.networkSync.syncedState.position, base.transform.position, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
				base.transform.rotation = Quaternion.Slerp(this.networkSync.syncedState.rotation, base.transform.rotation, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
				return;
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.rb.isKinematic = false;
					base.transform.parent = null;
					if (this.localStatePrev != RCVehicle.State.Mobilized)
					{
						base.transform.position = this.networkSync.syncedState.position;
						base.transform.rotation = this.networkSync.syncedState.rotation;
						return;
					}
				}
				break;
			}
		}

		// Token: 0x06007448 RID: 29768 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected virtual void SharedUpdate(float dt)
		{
		}

		// Token: 0x06007449 RID: 29769 RVA: 0x002610A4 File Offset: 0x0025F2A4
		public virtual void AuthorityApplyImpact(Vector3 hitVelocity, bool isProjectile)
		{
			if (this.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				float d = isProjectile ? this.projectileVelocityTransfer : this.hitVelocityTransfer;
				this.rb.AddForce(Vector3.ClampMagnitude(hitVelocity * d, this.hitMaxHitSpeed) * this.rb.mass, ForceMode.Impulse);
				if (isProjectile || (this.crashOnHit && hitVelocity.sqrMagnitude > this.crashOnHitSpeedThreshold * this.crashOnHitSpeedThreshold))
				{
					this.AuthorityBeginCrash();
				}
			}
			UnityEvent onHitImpact = this.OnHitImpact;
			if (onHitImpact == null)
			{
				return;
			}
			onHitImpact.Invoke();
		}

		// Token: 0x0600744A RID: 29770 RVA: 0x001A8FD1 File Offset: 0x001A71D1
		protected float NormalizeAngle180(float angle)
		{
			angle = (angle + 180f) % 360f;
			if (angle < 0f)
			{
				angle += 360f;
			}
			return angle - 180f;
		}

		// Token: 0x0600744B RID: 29771 RVA: 0x0026113C File Offset: 0x0025F33C
		protected static void AddScaledGravityCompensationForce(Rigidbody rb, float scaleFactor, float gravityCompensation)
		{
			Vector3 gravity = Physics.gravity;
			Vector3 vector = -gravity * gravityCompensation;
			Vector3 vector2 = gravity + vector;
			Vector3 b = vector2 * scaleFactor - vector2;
			rb.AddForce((vector + b) * rb.mass, ForceMode.Force);
		}

		// Token: 0x0400855B RID: 34139
		[SerializeField]
		private Transform leftDockParent;

		// Token: 0x0400855C RID: 34140
		[SerializeField]
		private Transform rightDockParent;

		// Token: 0x0400855D RID: 34141
		[SerializeField]
		private float maxRange = 100f;

		// Token: 0x0400855E RID: 34142
		[SerializeField]
		private float maxDisconnectionTime = 10f;

		// Token: 0x0400855F RID: 34143
		[SerializeField]
		private float crashRespawnDelay = 3f;

		// Token: 0x04008560 RID: 34144
		[SerializeField]
		private bool crashOnHit;

		// Token: 0x04008561 RID: 34145
		[SerializeField]
		private float crashOnHitSpeedThreshold = 5f;

		// Token: 0x04008562 RID: 34146
		[SerializeField]
		[Range(0f, 1f)]
		private float hitVelocityTransfer = 0.5f;

		// Token: 0x04008563 RID: 34147
		[SerializeField]
		[Range(0f, 1f)]
		private float projectileVelocityTransfer = 0.1f;

		// Token: 0x04008564 RID: 34148
		[SerializeField]
		private float hitMaxHitSpeed = 4f;

		// Token: 0x04008565 RID: 34149
		[SerializeField]
		[Range(0f, 1f)]
		private float joystickDeadzone = 0.1f;

		// Token: 0x04008566 RID: 34150
		[Header("RCVehicle - Shared Event")]
		public UnityEvent OnHitImpact;

		// Token: 0x04008567 RID: 34151
		protected RCVehicle.State localState;

		// Token: 0x04008568 RID: 34152
		protected RCVehicle.State localStatePrev;

		// Token: 0x04008569 RID: 34153
		protected float stateStartTime;

		// Token: 0x0400856A RID: 34154
		protected RCRemoteHoldable connectedRemote;

		// Token: 0x0400856B RID: 34155
		protected RCCosmeticNetworkSync networkSync;

		// Token: 0x0400856C RID: 34156
		protected bool hasNetworkSync;

		// Token: 0x0400856D RID: 34157
		protected RCRemoteHoldable.RCInput activeInput;

		// Token: 0x0400856E RID: 34158
		protected Rigidbody rb;

		// Token: 0x0400856F RID: 34159
		private bool waitingForTriggerRelease;

		// Token: 0x04008570 RID: 34160
		private float disconnectionTime;

		// Token: 0x04008571 RID: 34161
		private bool useLeftDock;

		// Token: 0x04008572 RID: 34162
		private BoneOffset dockLeftOffset = new BoneOffset(GTHardCodedBones.EBone.forearm_L, new Vector3(-0.062f, 0.283f, -0.136f), new Vector3(275f, 0f, 25f));

		// Token: 0x04008573 RID: 34163
		private BoneOffset dockRightOffset = new BoneOffset(GTHardCodedBones.EBone.forearm_R, new Vector3(0.069f, 0.265f, -0.128f), new Vector3(275f, 0f, 335f));

		// Token: 0x04008574 RID: 34164
		private float networkSyncFollowRateExp = 2f;

		// Token: 0x04008575 RID: 34165
		private Transform[] _vrRigBones;

		// Token: 0x02001226 RID: 4646
		protected enum State
		{
			// Token: 0x04008579 RID: 34169
			Disabled,
			// Token: 0x0400857A RID: 34170
			DockedLeft,
			// Token: 0x0400857B RID: 34171
			DockedRight,
			// Token: 0x0400857C RID: 34172
			Mobilized,
			// Token: 0x0400857D RID: 34173
			Crashed
		}
	}
}
