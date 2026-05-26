using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012B8 RID: 4792
	[RequireComponent(typeof(Collider))]
	public class CosmeticSwipeReactor : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x060077DD RID: 30685 RVA: 0x00274F5C File Offset: 0x0027315C
		private void Awake()
		{
			this._rig = base.GetComponentInParent<VRRig>();
			if (this._rig == null && base.gameObject.GetComponentInParent<GTPlayer>() != null)
			{
				this._rig = GorillaTagger.Instance.offlineVRRig;
			}
			this.isLocal = (this._rig != null && this._rig.isLocal);
			this.col = base.GetComponent<Collider>();
			switch (this.localSwipeAxis)
			{
			case CosmeticSwipeReactor.Axis.X:
				this.swipeDir = Vector3.right;
				return;
			case CosmeticSwipeReactor.Axis.Y:
				this.swipeDir = Vector3.up;
				return;
			case CosmeticSwipeReactor.Axis.Z:
				this.swipeDir = Vector3.forward;
				return;
			default:
				return;
			}
		}

		// Token: 0x060077DE RID: 30686 RVA: 0x00275014 File Offset: 0x00273214
		private void OnTriggerEnter(Collider other)
		{
			if (!this.isLocal || !base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component != null)
			{
				if (component.isLeftHand)
				{
					this.handIndicatorL = component;
					Vector3 pos = base.transform.InverseTransformPoint(component.transform.position);
					this.ResetProgress(true, pos);
					this.handInTriggerL = true;
				}
				else
				{
					this.handIndicatorR = component;
					Vector3 pos2 = base.transform.InverseTransformPoint(component.transform.position);
					this.ResetProgress(false, pos2);
					this.handInTriggerR = true;
				}
			}
			if ((this.handInTriggerL || this.handInTriggerR) && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x060077DF RID: 30687 RVA: 0x002750C4 File Offset: 0x002732C4
		private void OnTriggerExit(Collider other)
		{
			if (!this.isLocal || !base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component != null)
			{
				if (component.isLeftHand)
				{
					this.handInTriggerL = false;
					if (this.resetCooldownOnTriggerExit)
					{
						this.isCoolingDownL = false;
						this.cooldownEndL = double.MinValue;
					}
				}
				else
				{
					this.handInTriggerR = false;
					if (this.resetCooldownOnTriggerExit)
					{
						this.isCoolingDownR = false;
						this.cooldownEndR = double.MinValue;
					}
				}
			}
			if (!this.handInTriggerL && !this.handInTriggerR && this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x17000B8B RID: 2955
		// (get) Token: 0x060077E0 RID: 30688 RVA: 0x00275164 File Offset: 0x00273364
		// (set) Token: 0x060077E1 RID: 30689 RVA: 0x0027516C File Offset: 0x0027336C
		public bool TickRunning { get; set; }

		// Token: 0x060077E2 RID: 30690 RVA: 0x00275178 File Offset: 0x00273378
		public void Tick()
		{
			if (this.handInTriggerL)
			{
				this.ProcessHandMovement(this.handIndicatorL, this.startPosL, ref this.lastFramePosL, ref this.swipingUpL, ref this.distanceL, ref this.isCoolingDownL, ref this.cooldownEndL);
			}
			if (this.handInTriggerR)
			{
				this.ProcessHandMovement(this.handIndicatorR, this.startPosR, ref this.lastFramePosR, ref this.swipingUpR, ref this.distanceR, ref this.isCoolingDownR, ref this.cooldownEndR);
			}
			if (!this.handInTriggerL && !this.handInTriggerR && this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x060077E3 RID: 30691 RVA: 0x00275214 File Offset: 0x00273414
		private void ResetProgress(bool left, Vector3 pos)
		{
			if (left)
			{
				this.startPosL = pos;
				this.lastFramePosL = this.startPosL;
				this.distanceL = 0f;
				return;
			}
			this.startPosR = pos;
			this.lastFramePosR = this.startPosR;
			this.distanceR = 0f;
		}

		// Token: 0x060077E4 RID: 30692 RVA: 0x00275264 File Offset: 0x00273464
		private void ProcessHandMovement(GorillaTriggerColliderHandIndicator hand, Vector3 start, ref Vector3 last, ref bool swipingUp, ref float dist, ref bool isCoolingDown, ref double cooldownEndTime)
		{
			if (isCoolingDown)
			{
				if (Time.timeAsDouble < cooldownEndTime)
				{
					return;
				}
				isCoolingDown = false;
				cooldownEndTime = double.MinValue;
				this.ResetProgress(hand.isLeftHand, base.transform.InverseTransformPoint(hand.transform.position));
				return;
			}
			else
			{
				Vector3 vector = base.transform.InverseTransformPoint(hand.transform.position);
				float num = Mathf.Abs(this.GetAxisComponent(hand.currentVelocity));
				if (num < this.minimumVelocity * this._rig.scaleFactor || num > this.maximumVelocity * this._rig.scaleFactor)
				{
					this.ResetProgress(hand.isLeftHand, vector);
					return;
				}
				float num2 = this.GetAxisComponent(vector) - this.GetAxisComponent(last);
				if (num2 >= 0f && !swipingUp)
				{
					swipingUp = true;
					this.ResetProgress(hand.isLeftHand, vector);
					return;
				}
				if (num2 < 0f & swipingUp)
				{
					swipingUp = false;
					this.ResetProgress(hand.isLeftHand, vector);
					return;
				}
				if ((this.GetLateralMovement(start) - this.GetLateralMovement(vector)).sqrMagnitude > this.lateralMovementTolerance * this.lateralMovementTolerance)
				{
					this.ResetProgress(hand.isLeftHand, vector);
					return;
				}
				last = vector;
				dist += Mathf.Abs(num2);
				GorillaTagger.Instance.StartVibration(hand.isLeftHand, this.swipeHaptics.Evaluate(dist / this.swipeDistance), Time.deltaTime);
				if (dist >= this.swipeDistance)
				{
					if (swipingUp)
					{
						UnityEvent<bool> onSwipe = this.OnSwipe;
						if (onSwipe != null)
						{
							onSwipe.Invoke(hand.isLeftHand);
						}
						cooldownEndTime = Time.timeAsDouble + (double)this.swipeCooldown;
						isCoolingDown = true;
					}
					else
					{
						UnityEvent<bool> onReverseSwipe = this.OnReverseSwipe;
						if (onReverseSwipe != null)
						{
							onReverseSwipe.Invoke(hand.isLeftHand);
						}
						cooldownEndTime = Time.timeAsDouble + (double)this.swipeCooldown;
						isCoolingDown = true;
					}
					this.ResetProgress(hand.isLeftHand, vector);
				}
				return;
			}
		}

		// Token: 0x060077E5 RID: 30693 RVA: 0x00275454 File Offset: 0x00273654
		private float GetAxisComponent(Vector3 vec)
		{
			CosmeticSwipeReactor.Axis axis = this.localSwipeAxis;
			if (axis == CosmeticSwipeReactor.Axis.X)
			{
				return vec.x;
			}
			if (axis != CosmeticSwipeReactor.Axis.Y)
			{
				return vec.z;
			}
			return vec.y;
		}

		// Token: 0x060077E6 RID: 30694 RVA: 0x00275488 File Offset: 0x00273688
		private Vector2 GetLateralMovement(Vector3 vec)
		{
			CosmeticSwipeReactor.Axis axis = this.localSwipeAxis;
			if (axis == CosmeticSwipeReactor.Axis.X)
			{
				return new Vector2(vec.y, vec.z);
			}
			if (axis != CosmeticSwipeReactor.Axis.Y)
			{
				return new Vector2(vec.x, vec.y);
			}
			return new Vector2(vec.x, vec.z);
		}

		// Token: 0x04008AA5 RID: 35493
		[SerializeField]
		private CosmeticSwipeReactor.Axis localSwipeAxis = CosmeticSwipeReactor.Axis.Y;

		// Token: 0x04008AA6 RID: 35494
		private Vector3 swipeDir = Vector3.up;

		// Token: 0x04008AA7 RID: 35495
		[Tooltip("Distance hand can move perpindicular to the swipe without cancelling the gesture")]
		[SerializeField]
		private float lateralMovementTolerance = 0.1f;

		// Token: 0x04008AA8 RID: 35496
		[Tooltip("How far the hand has to move along the axis to count as a swipe\nThis distance must be contained within the trigger area")]
		[SerializeField]
		private float swipeDistance = 0.3f;

		// Token: 0x04008AA9 RID: 35497
		[SerializeField]
		private float minimumVelocity = 0.1f;

		// Token: 0x04008AAA RID: 35498
		[SerializeField]
		private float maximumVelocity = 3f;

		// Token: 0x04008AAB RID: 35499
		[Tooltip("Delay after completing a swipe before starting the next")]
		[SerializeField]
		private float swipeCooldown = 0.25f;

		// Token: 0x04008AAC RID: 35500
		[SerializeField]
		private bool resetCooldownOnTriggerExit = true;

		// Token: 0x04008AAD RID: 35501
		[Tooltip("Amplitude of haptics from normalized swiped distance")]
		[SerializeField]
		private AnimationCurve swipeHaptics = AnimationCurve.EaseInOut(0f, 0.02f, 1f, 0.5f);

		// Token: 0x04008AAE RID: 35502
		public UnityEvent<bool> OnSwipe;

		// Token: 0x04008AAF RID: 35503
		public UnityEvent<bool> OnReverseSwipe;

		// Token: 0x04008AB0 RID: 35504
		private VRRig _rig;

		// Token: 0x04008AB1 RID: 35505
		private Collider col;

		// Token: 0x04008AB2 RID: 35506
		private bool isLocal;

		// Token: 0x04008AB3 RID: 35507
		private bool handInTriggerR;

		// Token: 0x04008AB4 RID: 35508
		private bool handInTriggerL;

		// Token: 0x04008AB5 RID: 35509
		private GorillaTriggerColliderHandIndicator handIndicatorR;

		// Token: 0x04008AB6 RID: 35510
		private GorillaTriggerColliderHandIndicator handIndicatorL;

		// Token: 0x04008AB7 RID: 35511
		private Vector3 startPosR;

		// Token: 0x04008AB8 RID: 35512
		private Vector3 startPosL;

		// Token: 0x04008AB9 RID: 35513
		private Vector3 lastFramePosR;

		// Token: 0x04008ABA RID: 35514
		private Vector3 lastFramePosL;

		// Token: 0x04008ABB RID: 35515
		private float distanceR;

		// Token: 0x04008ABC RID: 35516
		private float distanceL;

		// Token: 0x04008ABD RID: 35517
		private bool swipingUpL;

		// Token: 0x04008ABE RID: 35518
		private bool swipingUpR;

		// Token: 0x04008ABF RID: 35519
		private double cooldownEndL = double.MinValue;

		// Token: 0x04008AC0 RID: 35520
		private double cooldownEndR = double.MinValue;

		// Token: 0x04008AC1 RID: 35521
		private bool isCoolingDownL;

		// Token: 0x04008AC2 RID: 35522
		private bool isCoolingDownR;

		// Token: 0x020012B9 RID: 4793
		public enum Axis
		{
			// Token: 0x04008AC5 RID: 35525
			X,
			// Token: 0x04008AC6 RID: 35526
			Y,
			// Token: 0x04008AC7 RID: 35527
			Z
		}
	}
}
