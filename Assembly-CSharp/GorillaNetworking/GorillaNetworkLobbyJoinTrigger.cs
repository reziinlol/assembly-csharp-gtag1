using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02001063 RID: 4195
	public class GorillaNetworkLobbyJoinTrigger : GorillaTriggerBox
	{
		// Token: 0x04007985 RID: 31109
		public GameObject[] makeSureThisIsDisabled;

		// Token: 0x04007986 RID: 31110
		public GameObject[] makeSureThisIsEnabled;

		// Token: 0x04007987 RID: 31111
		public string gameModeName;

		// Token: 0x04007988 RID: 31112
		public PhotonNetworkController photonNetworkController;

		// Token: 0x04007989 RID: 31113
		public string componentTypeToRemove;

		// Token: 0x0400798A RID: 31114
		public GameObject componentRemoveTarget;

		// Token: 0x0400798B RID: 31115
		public string componentTypeToAdd;

		// Token: 0x0400798C RID: 31116
		public GameObject componentAddTarget;

		// Token: 0x0400798D RID: 31117
		public GameObject gorillaParent;

		// Token: 0x0400798E RID: 31118
		public GameObject joinFailedBlock;
	}
}
