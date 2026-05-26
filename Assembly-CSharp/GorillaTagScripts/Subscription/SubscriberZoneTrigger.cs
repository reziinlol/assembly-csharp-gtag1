using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Subscription
{
	// Token: 0x02000F69 RID: 3945
	public class SubscriberZoneTrigger : MonoBehaviour
	{
		// Token: 0x06006251 RID: 25169 RVA: 0x001FBB50 File Offset: 0x001F9D50
		private void OnTriggerEnter(Collider other)
		{
			if (GTPlayer.Instance != null && other == GTPlayer.Instance.bodyCollider && this.parentZone != null)
			{
				this.parentZone.OnZoneEnter(this.isRestrictedZone);
			}
		}

		// Token: 0x06006252 RID: 25170 RVA: 0x001FBB90 File Offset: 0x001F9D90
		private void OnTriggerExit(Collider other)
		{
			if (GTPlayer.Instance != null && other == GTPlayer.Instance.bodyCollider && this.parentZone != null)
			{
				this.parentZone.OnZoneExit(this.isRestrictedZone);
			}
		}

		// Token: 0x04007124 RID: 28964
		public SubscriberExclusiveZone parentZone;

		// Token: 0x04007125 RID: 28965
		public bool isRestrictedZone;
	}
}
