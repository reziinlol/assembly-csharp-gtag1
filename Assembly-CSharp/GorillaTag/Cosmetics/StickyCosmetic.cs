using System;
using GorillaExtensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012AA RID: 4778
	public class StickyCosmetic : MonoBehaviour
	{
		// Token: 0x06007799 RID: 30617 RVA: 0x00273A9F File Offset: 0x00271C9F
		private void Start()
		{
			this.endRigidbody.isKinematic = false;
			this.endRigidbody.useGravity = false;
			this.UpdateState(StickyCosmetic.ObjectState.Idle);
		}

		// Token: 0x0600779A RID: 30618 RVA: 0x00273AC0 File Offset: 0x00271CC0
		public void Extend()
		{
			if (this.currentState == StickyCosmetic.ObjectState.Idle || this.currentState == StickyCosmetic.ObjectState.Extending)
			{
				this.UpdateState(StickyCosmetic.ObjectState.Extending);
			}
		}

		// Token: 0x0600779B RID: 30619 RVA: 0x00273ADA File Offset: 0x00271CDA
		public void Retract()
		{
			this.UpdateState(StickyCosmetic.ObjectState.Retracting);
		}

		// Token: 0x0600779C RID: 30620 RVA: 0x00273AE4 File Offset: 0x00271CE4
		private void Extend_Internal()
		{
			if (this.endRigidbody.isKinematic)
			{
				return;
			}
			this.rayLength = Mathf.Lerp(0f, this.maxObjectLength, this.blendShapeCosmetic.GetBlendValue() / this.blendShapeCosmetic.maxBlendShapeWeight);
			this.endRigidbody.MovePosition(this.startPosition.position + this.startPosition.forward * this.rayLength);
		}

		// Token: 0x0600779D RID: 30621 RVA: 0x00273B60 File Offset: 0x00271D60
		private void Retract_Internal()
		{
			this.endRigidbody.isKinematic = false;
			Vector3 position = Vector3.MoveTowards(this.endRigidbody.position, this.startPosition.position, this.retractSpeed * Time.fixedDeltaTime);
			this.endRigidbody.MovePosition(position);
		}

		// Token: 0x0600779E RID: 30622 RVA: 0x00273BB0 File Offset: 0x00271DB0
		private void FixedUpdate()
		{
			switch (this.currentState)
			{
			case StickyCosmetic.ObjectState.Extending:
			{
				if (Time.time - this.extendingStartedTime > this.retractAfterSecond)
				{
					this.UpdateState(StickyCosmetic.ObjectState.AutoRetract);
				}
				this.Extend_Internal();
				RaycastHit raycastHit;
				if (Physics.Raycast(this.rayOrigin.position, this.rayOrigin.forward, out raycastHit, this.rayLength, this.collisionLayers))
				{
					this.endRigidbody.isKinematic = true;
					this.endRigidbody.transform.parent = null;
					UnityEvent unityEvent = this.onStick;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					this.UpdateState(StickyCosmetic.ObjectState.Stuck);
				}
				break;
			}
			case StickyCosmetic.ObjectState.Retracting:
				if (Vector3.Distance(this.endRigidbody.position, this.startPosition.position) <= 0.01f)
				{
					this.endRigidbody.position = this.startPosition.position;
					Transform transform = this.endRigidbody.transform;
					transform.parent = this.endPositionParent;
					transform.localRotation = quaternion.identity;
					transform.localScale = Vector3.one;
					if (this.lastState == StickyCosmetic.ObjectState.AutoUnstuck || this.lastState == StickyCosmetic.ObjectState.AutoRetract)
					{
						this.UpdateState(StickyCosmetic.ObjectState.JustRetracted);
					}
					else
					{
						this.UpdateState(StickyCosmetic.ObjectState.Idle);
					}
				}
				else
				{
					this.Retract_Internal();
				}
				break;
			case StickyCosmetic.ObjectState.Stuck:
				if (this.endRigidbody.isKinematic && (this.endRigidbody.position - this.startPosition.position).IsLongerThan(this.autoRetractThreshold))
				{
					this.UpdateState(StickyCosmetic.ObjectState.AutoUnstuck);
				}
				break;
			case StickyCosmetic.ObjectState.AutoUnstuck:
				this.UpdateState(StickyCosmetic.ObjectState.Retracting);
				break;
			case StickyCosmetic.ObjectState.AutoRetract:
				this.UpdateState(StickyCosmetic.ObjectState.Retracting);
				break;
			}
			Debug.DrawRay(this.rayOrigin.position, this.rayOrigin.forward * this.rayLength, Color.red);
		}

		// Token: 0x0600779F RID: 30623 RVA: 0x00273D90 File Offset: 0x00271F90
		private void UpdateState(StickyCosmetic.ObjectState newState)
		{
			this.lastState = this.currentState;
			if (this.lastState == StickyCosmetic.ObjectState.Stuck && newState != this.currentState)
			{
				this.onUnstick.Invoke();
			}
			if (this.lastState != StickyCosmetic.ObjectState.Extending && newState == StickyCosmetic.ObjectState.Extending)
			{
				this.extendingStartedTime = Time.time;
			}
			this.currentState = newState;
		}

		// Token: 0x04008A1C RID: 35356
		[Tooltip("Optional reference to an UpdateBlendShapeCosmetic component. Used to drive extension length based on blend shape weight (e.g. finger flex input).")]
		[SerializeField]
		private UpdateBlendShapeCosmetic blendShapeCosmetic;

		// Token: 0x04008A1D RID: 35357
		[Tooltip("Defines which physics layers this sticky object can attach to when extending (checked via raycast).")]
		[SerializeField]
		private LayerMask collisionLayers;

		// Token: 0x04008A1E RID: 35358
		[Tooltip("Transform origin from which the raycast will be fired forward to detect stickable surfaces.")]
		[SerializeField]
		private Transform rayOrigin;

		// Token: 0x04008A1F RID: 35359
		[Tooltip("Transform representing the start or base position of the sticky object (where extension originates).")]
		[SerializeField]
		private Transform startPosition;

		// Token: 0x04008A20 RID: 35360
		[Tooltip("Rigidbody controlling the physical end of the sticky object (the part that extends and can attach).")]
		[SerializeField]
		private Rigidbody endRigidbody;

		// Token: 0x04008A21 RID: 35361
		[Tooltip("Parent transform the end object will reattach to when fully retracted. This keeps local transform resets consistent.")]
		[SerializeField]
		private Transform endPositionParent;

		// Token: 0x04008A22 RID: 35362
		[Tooltip("Maximum distance the object can extend from its start position (in meters).")]
		[SerializeField]
		private float maxObjectLength = 0.7f;

		// Token: 0x04008A23 RID: 35363
		[Tooltip("If the sticky object remains stuck but the distance from start exceeds this threshold, it will automatically unstuck and begin retracting.")]
		[SerializeField]
		private float autoRetractThreshold = 1f;

		// Token: 0x04008A24 RID: 35364
		[Tooltip("Speed (units per second) at which the end rigidbody retracts toward its start position when returning.")]
		[SerializeField]
		private float retractSpeed = 5f;

		// Token: 0x04008A25 RID: 35365
		[Tooltip("If the sticky end remains extended but doesn’t stick to anything, it will automatically start retracting after this many seconds.")]
		[SerializeField]
		private float retractAfterSecond = 2f;

		// Token: 0x04008A26 RID: 35366
		[Tooltip("Invoked when the sticky object successfully attaches to a surface.")]
		public UnityEvent onStick;

		// Token: 0x04008A27 RID: 35367
		[Tooltip("Invoked when the sticky object becomes unstuck — either manually or automatically.")]
		public UnityEvent onUnstick;

		// Token: 0x04008A28 RID: 35368
		private StickyCosmetic.ObjectState currentState;

		// Token: 0x04008A29 RID: 35369
		private float rayLength;

		// Token: 0x04008A2A RID: 35370
		private bool stick;

		// Token: 0x04008A2B RID: 35371
		private StickyCosmetic.ObjectState lastState;

		// Token: 0x04008A2C RID: 35372
		private float extendingStartedTime;

		// Token: 0x020012AB RID: 4779
		private enum ObjectState
		{
			// Token: 0x04008A2E RID: 35374
			Extending,
			// Token: 0x04008A2F RID: 35375
			Retracting,
			// Token: 0x04008A30 RID: 35376
			Stuck,
			// Token: 0x04008A31 RID: 35377
			JustRetracted,
			// Token: 0x04008A32 RID: 35378
			Idle,
			// Token: 0x04008A33 RID: 35379
			AutoUnstuck,
			// Token: 0x04008A34 RID: 35380
			AutoRetract
		}
	}
}
