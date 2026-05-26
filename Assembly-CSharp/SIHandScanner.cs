using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000141 RID: 321
public class SIHandScanner : MonoBehaviour
{
	// Token: 0x06000809 RID: 2057 RVA: 0x0002C15B File Offset: 0x0002A35B
	public void HandScanned(SIPlayer scannedPlayer)
	{
		if (!scannedPlayer.gamePlayer.IsLocal())
		{
			return;
		}
		this.onHandScanned.Invoke(NetworkSystem.Instance.LocalPlayerID);
	}

	// Token: 0x04000A2B RID: 2603
	public UnityEvent<int> onHandScanned;
}
