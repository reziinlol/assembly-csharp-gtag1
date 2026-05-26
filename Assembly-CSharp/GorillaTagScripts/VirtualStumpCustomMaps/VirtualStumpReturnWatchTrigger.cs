using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000F53 RID: 3923
	public class VirtualStumpReturnWatchTrigger : MonoBehaviour
	{
		// Token: 0x060061E7 RID: 25063 RVA: 0x001F952B File Offset: 0x001F772B
		public void OnTriggerEnter(Collider other)
		{
			if (other == GTPlayer.Instance.headCollider)
			{
				VRRig.LocalRig.EnableVStumpReturnWatch(false);
			}
		}

		// Token: 0x060061E8 RID: 25064 RVA: 0x001F954A File Offset: 0x001F774A
		public void OnTriggerExit(Collider other)
		{
			if (other == GTPlayer.Instance.headCollider && GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				VRRig.LocalRig.EnableVStumpReturnWatch(true);
			}
		}
	}
}
