using System;
using UnityEngine;

// Token: 0x02000894 RID: 2196
public class GorillaTagCompetitivePrivateRoomBlocker : MonoBehaviour
{
	// Token: 0x0600398B RID: 14731 RVA: 0x00139E27 File Offset: 0x00138027
	private void Update()
	{
		this.blocker.SetActive(NetworkSystem.Instance.SessionIsPrivate);
	}

	// Token: 0x04004970 RID: 18800
	[SerializeField]
	private GameObject blocker;
}
