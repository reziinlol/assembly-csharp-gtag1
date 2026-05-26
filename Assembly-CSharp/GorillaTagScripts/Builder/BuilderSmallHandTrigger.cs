using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FBC RID: 4028
	public class BuilderSmallHandTrigger : MonoBehaviour
	{
		// Token: 0x17000972 RID: 2418
		// (get) Token: 0x060064B8 RID: 25784 RVA: 0x00207650 File Offset: 0x00205850
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x060064B9 RID: 25785 RVA: 0x00207660 File Offset: 0x00205860
		private void OnTriggerEnter(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent == null)
			{
				return;
			}
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
			if (this.onlySmallHands && !this.ignoreScale && (double)VRRigCache.Instance.localRig.Rig.scaleFactor > 0.99)
			{
				return;
			}
			if (this.requireMinimumVelocity)
			{
				float num = this.minimumVelocityMagnitude * GorillaTagger.Instance.offlineVRRig.scaleFactor;
				if (GTPlayer.Instance.GetHandVelocityTracker(componentInParent.isLeftHand).GetAverageVelocity(true, 0.1f, false).sqrMagnitude < num * num)
				{
					return;
				}
			}
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration * 1.5f);
			this.lastTriggeredFrame = Time.frameCount;
			UnityEvent triggeredEvent = this.TriggeredEvent;
			if (triggeredEvent != null)
			{
				triggeredEvent.Invoke();
			}
			if (this.timeline != null && (this.timeline.time == 0.0 || this.timeline.time >= this.timeline.duration))
			{
				this.timeline.Play();
			}
			if (this.animation != null && this.animation.clip != null)
			{
				this.animation.Play();
			}
		}

		// Token: 0x040073A6 RID: 29606
		[Tooltip("Optional timeline to play to animate the thing getting activated, play sound, particles, etc...")]
		public PlayableDirector timeline;

		// Token: 0x040073A7 RID: 29607
		[Tooltip("Optional animation to play")]
		public Animation animation;

		// Token: 0x040073A8 RID: 29608
		private int lastTriggeredFrame = -1;

		// Token: 0x040073A9 RID: 29609
		public bool onlySmallHands;

		// Token: 0x040073AA RID: 29610
		[SerializeField]
		protected bool requireMinimumVelocity;

		// Token: 0x040073AB RID: 29611
		[SerializeField]
		protected float minimumVelocityMagnitude = 0.1f;

		// Token: 0x040073AC RID: 29612
		private bool hasCheckedZone;

		// Token: 0x040073AD RID: 29613
		private bool ignoreScale;

		// Token: 0x040073AE RID: 29614
		internal UnityEvent TriggeredEvent = new UnityEvent();

		// Token: 0x040073AF RID: 29615
		[SerializeField]
		private BuilderPiece myPiece;
	}
}
