using System;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics
{
	// Token: 0x0200112A RID: 4394
	public class CosmeticsLocalHandReactor : MonoBehaviour
	{
		// Token: 0x06006F98 RID: 28568 RVA: 0x00247258 File Offset: 0x00245458
		protected void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
			if (this.ownerRig == null)
			{
				GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
				if (componentInParent != null)
				{
					this.ownerRig = componentInParent.offlineVRRig;
					this.ownerIsLocal = (this.ownerRig != null);
				}
			}
			if (this.ownerRig == null)
			{
				Debug.LogError("TriggerToggler: Disabling cannot find VRRig.");
				base.enabled = false;
				return;
			}
		}

		// Token: 0x06006F99 RID: 28569 RVA: 0x002472D0 File Offset: 0x002454D0
		protected void LateUpdate()
		{
			if (this.ownerIsLocal)
			{
				if (Time.time < this.lastTriggerTime + this.cooldownTime)
				{
					return;
				}
				Transform transform = base.transform;
				if (Physics.OverlapSphereNonAlloc(base.transform.position, this.proximityThreshold * transform.lossyScale.x, this.colliders, this.handLayer) > 0)
				{
					GorillaTriggerColliderHandIndicator component = this.colliders[0].GetComponent<GorillaTriggerColliderHandIndicator>();
					if (component != null)
					{
						GorillaTagger.Instance.StartVibration(component.isLeftHand, this.hapticStrength, this.hapticDuration);
						UnityEvent<bool> unityEvent = this.onTrigger;
						if (unityEvent != null)
						{
							unityEvent.Invoke(component.isLeftHand);
						}
						this.lastTriggerTime = Time.time;
					}
				}
			}
		}

		// Token: 0x04007F91 RID: 32657
		[SerializeField]
		private float hapticStrength = 0.2f;

		// Token: 0x04007F92 RID: 32658
		[SerializeField]
		private float hapticDuration = 0.2f;

		// Token: 0x04007F93 RID: 32659
		[Tooltip("The distance threshold (in meters) for triggering the interaction.\nIf the hand enters this range, onTrigger is fired.")]
		public float proximityThreshold = 0.15f;

		// Token: 0x04007F94 RID: 32660
		[Tooltip("Minimum time (in seconds) between consecutive triggers.\n")]
		[SerializeField]
		private float cooldownTime = 0.5f;

		// Token: 0x04007F95 RID: 32661
		public UnityEvent<bool> onTrigger;

		// Token: 0x04007F96 RID: 32662
		private VRRig ownerRig;

		// Token: 0x04007F97 RID: 32663
		private bool ownerIsLocal;

		// Token: 0x04007F98 RID: 32664
		private float lastTriggerTime = float.MinValue;

		// Token: 0x04007F99 RID: 32665
		private readonly Collider[] colliders = new Collider[1];

		// Token: 0x04007F9A RID: 32666
		private LayerMask handLayer = 1024;
	}
}
