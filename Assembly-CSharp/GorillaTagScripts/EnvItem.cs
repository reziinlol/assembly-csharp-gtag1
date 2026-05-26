using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000EFF RID: 3839
	public class EnvItem : MonoBehaviour, IPunInstantiateMagicCallback
	{
		// Token: 0x06005F73 RID: 24435 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnEnable()
		{
		}

		// Token: 0x06005F74 RID: 24436 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnDisable()
		{
		}

		// Token: 0x06005F75 RID: 24437 RVA: 0x001EB9B0 File Offset: 0x001E9BB0
		public void OnPhotonInstantiate(PhotonMessageInfo info)
		{
			object[] instantiationData = info.photonView.InstantiationData;
			this.spawnedByPhotonViewId = (int)instantiationData[0];
		}

		// Token: 0x04006E2C RID: 28204
		public int spawnedByPhotonViewId;
	}
}
