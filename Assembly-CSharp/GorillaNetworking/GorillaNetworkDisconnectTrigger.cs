using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x0200105E RID: 4190
	public class GorillaNetworkDisconnectTrigger : GorillaTriggerBox
	{
		// Token: 0x0600693F RID: 26943 RVA: 0x00220FE0 File Offset: 0x0021F1E0
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (this.makeSureThisIsEnabled != null)
			{
				this.makeSureThisIsEnabled.SetActive(true);
			}
			GameObject[] array = this.makeSureTheseAreEnabled;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			if (PhotonNetwork.InRoom)
			{
				if (this.componentTypeToRemove != "" && this.componentTarget.GetComponent(this.componentTypeToRemove) != null)
				{
					Object.Destroy(this.componentTarget.GetComponent(this.componentTypeToRemove));
				}
				PhotonNetwork.Disconnect();
				SkinnedMeshRenderer[] array2 = this.photonNetworkController.offlineVRRig;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].enabled = true;
				}
				PhotonNetwork.ConnectUsingSettings();
			}
		}

		// Token: 0x0400796A RID: 31082
		public PhotonNetworkController photonNetworkController;

		// Token: 0x0400796B RID: 31083
		public GameObject offlineVRRig;

		// Token: 0x0400796C RID: 31084
		public GameObject makeSureThisIsEnabled;

		// Token: 0x0400796D RID: 31085
		public GameObject[] makeSureTheseAreEnabled;

		// Token: 0x0400796E RID: 31086
		public string componentTypeToRemove;

		// Token: 0x0400796F RID: 31087
		public GameObject componentTarget;
	}
}
