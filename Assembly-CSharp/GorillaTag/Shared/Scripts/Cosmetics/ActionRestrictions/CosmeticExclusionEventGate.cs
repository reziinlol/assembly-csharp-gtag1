using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x020011DF RID: 4575
	public class CosmeticExclusionEventGate : MonoBehaviour
	{
		// Token: 0x06007303 RID: 29443 RVA: 0x002569A5 File Offset: 0x00254BA5
		private void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
		}

		// Token: 0x06007304 RID: 29444 RVA: 0x002569B3 File Offset: 0x00254BB3
		public void InvokeEvent()
		{
			if (CosmeticExclusionQuery.IsRestricted(this.ownerRig, this.effectSource))
			{
				UnityEvent unityEvent = this.onRestricted;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke();
				return;
			}
			else
			{
				UnityEvent unityEvent2 = this.onNormal;
				if (unityEvent2 == null)
				{
					return;
				}
				unityEvent2.Invoke();
				return;
			}
		}

		// Token: 0x0400837B RID: 33659
		[Header("Context")]
		[Tooltip("Optional effect source.\nIf set and has CosmeticExclusionSource, world position will be checked.")]
		[SerializeField]
		private GameObject effectSource;

		// Token: 0x0400837C RID: 33660
		[Header("Forwarded Events")]
		[SerializeField]
		private UnityEvent onNormal;

		// Token: 0x0400837D RID: 33661
		[SerializeField]
		private UnityEvent onRestricted;

		// Token: 0x0400837E RID: 33662
		private VRRig ownerRig;
	}
}
