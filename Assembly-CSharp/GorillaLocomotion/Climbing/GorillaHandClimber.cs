using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02001110 RID: 4368
	public class GorillaHandClimber : MonoBehaviour
	{
		// Token: 0x17000A9E RID: 2718
		// (get) Token: 0x06006E05 RID: 28165 RVA: 0x0023FFB8 File Offset: 0x0023E1B8
		public bool isClimbingOrGrabbing
		{
			get
			{
				return this.isClimbing || this.grabber.isGrabbing;
			}
		}

		// Token: 0x06006E06 RID: 28166 RVA: 0x0023FFCF File Offset: 0x0023E1CF
		private void Awake()
		{
			this.col = base.GetComponent<Collider>();
			this.grabber = base.GetComponent<GorillaGrabber>();
		}

		// Token: 0x06006E07 RID: 28167 RVA: 0x0023FFEC File Offset: 0x0023E1EC
		public void CheckHandClimber()
		{
			for (int i = this.potentialClimbables.Count - 1; i >= 0; i--)
			{
				GorillaClimbable gorillaClimbable = this.potentialClimbables[i];
				if (gorillaClimbable == null || !gorillaClimbable.isActiveAndEnabled)
				{
					this.potentialClimbables.RemoveAt(i);
				}
				else if (gorillaClimbable.climbOnlyWhileSmall && !ZoneManagement.IsInZone(GTZone.monkeBlocksShared) && this.player.scale > 0.99f)
				{
					this.potentialClimbables.RemoveAt(i);
				}
			}
			bool grab = ControllerInputPoller.GetGrab(this.xrNode);
			bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
			if (!this.isClimbing)
			{
				if (this.queuedToBecomeValidToGrabAgain && Vector3.Distance(this.lastAutoReleasePos, this.handRoot.localPosition) >= 0.35f)
				{
					this.queuedToBecomeValidToGrabAgain = false;
				}
				if (grabRelease)
				{
					this.queuedToBecomeValidToGrabAgain = false;
					this.dontReclimbLast = null;
				}
				GorillaClimbable closestClimbable = this.GetClosestClimbable();
				if (!this.queuedToBecomeValidToGrabAgain && closestClimbable && grab && this.CanInitiateClimb() && closestClimbable != this.dontReclimbLast)
				{
					GorillaClimbableRef gorillaClimbableRef = closestClimbable as GorillaClimbableRef;
					if (gorillaClimbableRef != null)
					{
						this.player.BeginClimbing(gorillaClimbableRef.climb, this, gorillaClimbableRef);
					}
					else
					{
						this.player.BeginClimbing(closestClimbable, this, null);
					}
				}
			}
			else if (grabRelease && this.canRelease)
			{
				this.player.EndClimbing(this, false, false);
			}
			this.grabber.CheckGrabber(this.CanInitiateClimb() && grab);
		}

		// Token: 0x06006E08 RID: 28168 RVA: 0x00240164 File Offset: 0x0023E364
		private bool CanInitiateClimb()
		{
			return !this.isClimbing && !this.equipmentInteractor.GetIsHolding(this.xrNode) && !this.equipmentInteractor.builderPieceInteractor.GetIsHolding(this.xrNode) && !this.equipmentInteractor.IsGrabDisabled(this.xrNode) && !GamePlayerLocal.IsHandHolding(this.xrNode) && !this.player.inOverlay;
		}

		// Token: 0x06006E09 RID: 28169 RVA: 0x002401D4 File Offset: 0x0023E3D4
		public void SetCanRelease(bool canRelease)
		{
			this.canRelease = canRelease;
		}

		// Token: 0x06006E0A RID: 28170 RVA: 0x002401E0 File Offset: 0x0023E3E0
		public GorillaClimbable GetClosestClimbable()
		{
			if (this.potentialClimbables.Count == 0)
			{
				return null;
			}
			if (this.potentialClimbables.Count == 1)
			{
				return this.potentialClimbables[0];
			}
			Vector3 position = base.transform.position;
			Bounds bounds = this.col.bounds;
			float num = 0.15f;
			GorillaClimbable result = null;
			foreach (GorillaClimbable gorillaClimbable in this.potentialClimbables)
			{
				float num2;
				if (gorillaClimbable.colliderCache)
				{
					if (!bounds.Intersects(gorillaClimbable.colliderCache.bounds))
					{
						continue;
					}
					Vector3 b = gorillaClimbable.colliderCache.ClosestPoint(position);
					num2 = Vector3.Distance(position, b);
				}
				else
				{
					num2 = Vector3.Distance(position, gorillaClimbable.transform.position);
				}
				if (num2 < num)
				{
					result = gorillaClimbable;
					num = num2;
				}
			}
			return result;
		}

		// Token: 0x06006E0B RID: 28171 RVA: 0x002402E0 File Offset: 0x0023E4E0
		private void OnTriggerEnter(Collider other)
		{
			GorillaClimbable item;
			if (other.TryGetComponent<GorillaClimbable>(out item))
			{
				this.potentialClimbables.Add(item);
				return;
			}
			GorillaClimbableRef item2;
			if (other.TryGetComponent<GorillaClimbableRef>(out item2))
			{
				this.potentialClimbables.Add(item2);
			}
		}

		// Token: 0x06006E0C RID: 28172 RVA: 0x0024031C File Offset: 0x0023E51C
		private void OnTriggerExit(Collider other)
		{
			GorillaClimbable item;
			if (other.TryGetComponent<GorillaClimbable>(out item))
			{
				this.potentialClimbables.Remove(item);
				return;
			}
			GorillaClimbableRef item2;
			if (other.TryGetComponent<GorillaClimbableRef>(out item2))
			{
				this.potentialClimbables.Remove(item2);
			}
		}

		// Token: 0x06006E0D RID: 28173 RVA: 0x00240358 File Offset: 0x0023E558
		public void ForceStopClimbing(bool startingNewClimb = false, bool doDontReclimb = false)
		{
			this.player.EndClimbing(this, startingNewClimb, doDontReclimb);
		}

		// Token: 0x04007F1D RID: 32541
		[SerializeField]
		private GTPlayer player;

		// Token: 0x04007F1E RID: 32542
		[SerializeField]
		private EquipmentInteractor equipmentInteractor;

		// Token: 0x04007F1F RID: 32543
		private List<GorillaClimbable> potentialClimbables = new List<GorillaClimbable>();

		// Token: 0x04007F20 RID: 32544
		[Header("Non-hand input should have the component disabled")]
		public XRNode xrNode = XRNode.LeftHand;

		// Token: 0x04007F21 RID: 32545
		[NonSerialized]
		public bool isClimbing;

		// Token: 0x04007F22 RID: 32546
		[NonSerialized]
		public bool queuedToBecomeValidToGrabAgain;

		// Token: 0x04007F23 RID: 32547
		[NonSerialized]
		public GorillaClimbable dontReclimbLast;

		// Token: 0x04007F24 RID: 32548
		[NonSerialized]
		public Vector3 lastAutoReleasePos = Vector3.zero;

		// Token: 0x04007F25 RID: 32549
		public GorillaGrabber grabber;

		// Token: 0x04007F26 RID: 32550
		public Transform handRoot;

		// Token: 0x04007F27 RID: 32551
		private const float DIST_FOR_CLEAR_RELEASE = 0.35f;

		// Token: 0x04007F28 RID: 32552
		private const float DIST_FOR_GRAB = 0.15f;

		// Token: 0x04007F29 RID: 32553
		private Collider col;

		// Token: 0x04007F2A RID: 32554
		private bool canRelease = true;
	}
}
