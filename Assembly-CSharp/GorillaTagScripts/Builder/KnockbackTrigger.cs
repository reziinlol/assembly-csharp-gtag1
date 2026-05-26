using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FC1 RID: 4033
	public class KnockbackTrigger : MonoBehaviour
	{
		// Token: 0x17000975 RID: 2421
		// (get) Token: 0x060064D9 RID: 25817 RVA: 0x002085B6 File Offset: 0x002067B6
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x060064DA RID: 25818 RVA: 0x002085C8 File Offset: 0x002067C8
		private void CheckZone()
		{
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
		}

		// Token: 0x060064DB RID: 25819 RVA: 0x00208618 File Offset: 0x00206818
		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHead) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
			{
				return;
			}
			this.CheckZone();
			if (!this.ignoreScale && this.onlySmallMonke && (double)VRRigCache.Instance.localRig.Rig.scaleFactor > 0.99)
			{
				return;
			}
			this.collidersEntered.Add(other);
			if (this.collidersEntered.Count > 1)
			{
				return;
			}
			Vector3 vector = this.triggerVolume.ClosestPoint(GorillaTagger.Instance.headCollider.transform.position);
			Vector3 vector2 = vector - base.transform.TransformPoint(this.triggerVolume.center);
			vector2 -= Vector3.Project(vector2, base.transform.TransformDirection(this.localAxis));
			float magnitude = vector2.magnitude;
			Vector3 direction = Vector3.up;
			if (magnitude >= 0.01f)
			{
				direction = vector2 / magnitude;
			}
			GTPlayer.Instance.SetMaximumSlipThisFrame();
			GTPlayer.Instance.ApplyKnockback(direction, this.knockbackVelocity * VRRigCache.Instance.localRig.Rig.scaleFactor, false);
			if (this.impactFX != null)
			{
				ObjectPools.instance.Instantiate(this.impactFX, vector, true);
			}
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
			this.lastTriggeredFrame = Time.frameCount;
		}

		// Token: 0x060064DC RID: 25820 RVA: 0x002087BA File Offset: 0x002069BA
		private void OnTriggerExit(Collider other)
		{
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHead) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
			{
				return;
			}
			this.collidersEntered.Remove(other);
		}

		// Token: 0x060064DD RID: 25821 RVA: 0x002087F6 File Offset: 0x002069F6
		private void OnDisable()
		{
			this.collidersEntered.Clear();
		}

		// Token: 0x040073E3 RID: 29667
		[SerializeField]
		private BoxCollider triggerVolume;

		// Token: 0x040073E4 RID: 29668
		[SerializeField]
		private float knockbackVelocity;

		// Token: 0x040073E5 RID: 29669
		[SerializeField]
		private Vector3 localAxis;

		// Token: 0x040073E6 RID: 29670
		[SerializeField]
		private GameObject impactFX;

		// Token: 0x040073E7 RID: 29671
		[SerializeField]
		private bool onlySmallMonke;

		// Token: 0x040073E8 RID: 29672
		private bool hasCheckedZone;

		// Token: 0x040073E9 RID: 29673
		private bool ignoreScale;

		// Token: 0x040073EA RID: 29674
		private int lastTriggeredFrame = -1;

		// Token: 0x040073EB RID: 29675
		private List<Collider> collidersEntered = new List<Collider>(4);
	}
}
