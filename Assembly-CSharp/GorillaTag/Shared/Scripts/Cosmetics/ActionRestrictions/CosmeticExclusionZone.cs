using System;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x020011E2 RID: 4578
	[RequireComponent(typeof(Collider))]
	public class CosmeticExclusionZone : MonoBehaviour
	{
		// Token: 0x06007309 RID: 29449 RVA: 0x00256A3C File Offset: 0x00254C3C
		private void Awake()
		{
			this.zoneCollider = base.GetComponent<Collider>();
			this.zoneCollider.isTrigger = true;
			CosmeticExclusionZoneRegistryUtility.RegisterZone(this.zoneCollider);
		}

		// Token: 0x0600730A RID: 29450 RVA: 0x00256A61 File Offset: 0x00254C61
		private void OnDestroy()
		{
			CosmeticExclusionZoneRegistryUtility.UnregisterZone(this.zoneCollider);
		}

		// Token: 0x0600730B RID: 29451 RVA: 0x00256A70 File Offset: 0x00254C70
		private void OnTriggerEnter(Collider other)
		{
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			if (componentInParent != null)
			{
				CosmeticExclusionZoneRegistry.Enter(componentInParent);
			}
		}

		// Token: 0x0600730C RID: 29452 RVA: 0x00256A94 File Offset: 0x00254C94
		private void OnTriggerExit(Collider other)
		{
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			if (componentInParent != null)
			{
				CosmeticExclusionZoneRegistry.Exit(componentInParent);
			}
		}

		// Token: 0x0400837F RID: 33663
		private Collider zoneCollider;
	}
}
