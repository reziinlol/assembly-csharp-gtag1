using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001240 RID: 4672
	public class ThrowablePickupableCosmetic : TransferrableObject
	{
		// Token: 0x060074F6 RID: 29942 RVA: 0x00264D7C File Offset: 0x00262F7C
		private new void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
		}

		// Token: 0x060074F7 RID: 29943 RVA: 0x00264D8C File Offset: 0x00262F8C
		internal override void OnEnable()
		{
			base.OnEnable();
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				this.owner = ((this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null));
				if (this.owner != null)
				{
					this._events.Init(this.owner);
					this.isLocal = this.owner.IsLocal;
				}
			}
			if (this._events != null)
			{
				this._events.Activate.reliable = true;
				this._events.Deactivate.reliable = true;
				this._events.Activate += this.OnReleaseEvent;
				this._events.Deactivate += this.OnReturnToDockEvent;
			}
		}

		// Token: 0x060074F8 RID: 29944 RVA: 0x00264EC0 File Offset: 0x002630C0
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnReleaseEvent;
				this._events.Deactivate -= this.OnReturnToDockEvent;
				this._events.Dispose();
				this._events = null;
			}
			if (this.pickupableVariant != null && this.pickupableVariant.enabled)
			{
				this.pickupableVariant.DelayedPickup();
			}
		}

		// Token: 0x060074F9 RID: 29945 RVA: 0x00264F60 File Offset: 0x00263160
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
			{
				return;
			}
			if (this.pickupableVariant.enabled)
			{
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					this._events.Activate.RaiseOthers(new object[]
					{
						false
					});
				}
				base.transform.position = this.pickupableVariant.transform.position;
				base.transform.rotation = this.pickupableVariant.transform.rotation;
				this.pickupableVariant.Pickup(false);
				if (grabbingHand == EquipmentInteractor.instance.leftHand && this.currentState == TransferrableObject.PositionState.OnLeftArm)
				{
					this.canAutoGrabLeft = false;
					this.interpState = TransferrableObject.InterpolateState.None;
					this.currentState = TransferrableObject.PositionState.InRightHand;
				}
				else if (grabbingHand == EquipmentInteractor.instance.rightHand && this.currentState == TransferrableObject.PositionState.OnRightArm)
				{
					this.canAutoGrabRight = false;
					this.interpState = TransferrableObject.InterpolateState.None;
					this.currentState = TransferrableObject.PositionState.InLeftHand;
				}
			}
			UnityEvent onGrabLocal = this.OnGrabLocal;
			if (onGrabLocal != null)
			{
				onGrabLocal.Invoke();
			}
			base.OnGrab(pointGrabbed, grabbingHand);
		}

		// Token: 0x060074FA RID: 29946 RVA: 0x002650A4 File Offset: 0x002632A4
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (!(VRRigCache.Instance.localRig.Rig == this.ownerRig))
			{
				return false;
			}
			Vector3 position = base.transform.position;
			bool isLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
			Vector3 averageVelocity = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand).GetAverageVelocity(true, 0.15f, false);
			float scale = GTPlayer.Instance.scale;
			bool flag = this.DistanceToDock() > this.returnToDockDistanceThreshold;
			if (PhotonNetwork.InRoom && this._events != null)
			{
				if (flag && this._events.Activate != null)
				{
					this._events.Activate.RaiseOthers(new object[]
					{
						true,
						position,
						averageVelocity,
						scale
					});
					this.OnReleaseEventLocal(position, averageVelocity, scale);
				}
				else if (!flag && this._events.Deactivate != null)
				{
					this._events.Deactivate.RaiseAll(Array.Empty<object>());
					UnityEvent onReturnToDockPositionLocal = this.OnReturnToDockPositionLocal;
					if (onReturnToDockPositionLocal != null)
					{
						onReturnToDockPositionLocal.Invoke();
					}
				}
			}
			else if (flag)
			{
				this.OnReleaseEventLocal(position, averageVelocity, scale);
			}
			else
			{
				UnityEvent onReturnToDockPositionLocal2 = this.OnReturnToDockPositionLocal;
				if (onReturnToDockPositionLocal2 != null)
				{
					onReturnToDockPositionLocal2.Invoke();
				}
				UnityEvent onReturnToDockPositionShared = this.OnReturnToDockPositionShared;
				if (onReturnToDockPositionShared != null)
				{
					onReturnToDockPositionShared.Invoke();
				}
			}
			return true;
		}

		// Token: 0x060074FB RID: 29947 RVA: 0x00265218 File Offset: 0x00263418
		private void OnReleaseEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnReleaseEvent");
			if (!this.callLimiterRelease.CheckCallTime(Time.time))
			{
				return;
			}
			object obj = args[0];
			if (obj is bool)
			{
				bool flag = (bool)obj;
				if (flag)
				{
					obj = args[1];
					if (obj is Vector3)
					{
						Vector3 vector = (Vector3)obj;
						obj = args[2];
						if (obj is Vector3)
						{
							Vector3 inVel = (Vector3)obj;
							obj = args[3];
							if (obj is float)
							{
								float value = (float)obj;
								Vector3 position = base.transform.position;
								Vector3 releaseVelocity = base.transform.forward;
								ref position.SetValueSafe(vector);
								if (this.ownerRig.IsPositionInRange(position, 20f))
								{
									releaseVelocity = this.ownerRig.ClampVelocityRelativeToPlayerSafe(inVel, 50f, 100f);
									float playerScale = value.ClampSafe(0.01f, 1f);
									this.OnReleaseEventLocal(position, releaseVelocity, playerScale);
									return;
								}
								return;
							}
						}
					}
					return;
				}
				this.pickupableVariant.Pickup(false);
				return;
			}
		}

		// Token: 0x060074FC RID: 29948 RVA: 0x00265338 File Offset: 0x00263538
		private void OnReturnToDockEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnReturnToDockEvent");
			if (!this.callLimiterReturn.CheckCallTime(Time.time))
			{
				return;
			}
			UnityEvent onReturnToDockPositionShared = this.OnReturnToDockPositionShared;
			if (onReturnToDockPositionShared == null)
			{
				return;
			}
			onReturnToDockPositionShared.Invoke();
		}

		// Token: 0x060074FD RID: 29949 RVA: 0x00265393 File Offset: 0x00263593
		private void OnReleaseEventLocal(Vector3 startPosition, Vector3 releaseVelocity, float playerScale)
		{
			this.pickupableVariant.Release(this, startPosition, releaseVelocity, playerScale);
		}

		// Token: 0x060074FE RID: 29950 RVA: 0x002653A4 File Offset: 0x002635A4
		private float DistanceToDock()
		{
			float result = 0f;
			if (this.currentState == TransferrableObject.PositionState.OnRightShoulder)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.rightBackTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnLeftShoulder)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.leftBackTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnRightArm)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.rightArmTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnLeftArm)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.leftArmTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnChest)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.chestTransform.position, base.transform.position);
			}
			return result;
		}

		// Token: 0x04008672 RID: 34418
		[Tooltip("Child object with the PickupableCosmetic script")]
		[SerializeField]
		private PickupableVariant pickupableVariant;

		// Token: 0x04008673 RID: 34419
		[Tooltip("cosmetics released at a greater distance from the dock than the threshold will be placed in world instead of returning to the dock")]
		[SerializeField]
		private float returnToDockDistanceThreshold = 0.7f;

		// Token: 0x04008674 RID: 34420
		[FormerlySerializedAs("OnReturnToDockPosition")]
		[Space]
		public UnityEvent OnReturnToDockPositionLocal;

		// Token: 0x04008675 RID: 34421
		public UnityEvent OnReturnToDockPositionShared;

		// Token: 0x04008676 RID: 34422
		[FormerlySerializedAs("OnGrabFromDockPosition")]
		public UnityEvent OnGrabLocal;

		// Token: 0x04008677 RID: 34423
		private RubberDuckEvents _events;

		// Token: 0x04008678 RID: 34424
		private TransferrableObject transferrableObject;

		// Token: 0x04008679 RID: 34425
		private bool isLocal;

		// Token: 0x0400867A RID: 34426
		private NetPlayer owner;

		// Token: 0x0400867B RID: 34427
		private CallLimiter callLimiterRelease = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x0400867C RID: 34428
		private CallLimiter callLimiterReturn = new CallLimiter(10, 2f, 0.5f);
	}
}
