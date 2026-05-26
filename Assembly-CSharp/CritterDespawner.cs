using System;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class CritterDespawner : MonoBehaviour
{
	// Token: 0x06000112 RID: 274 RVA: 0x0000694E File Offset: 0x00004B4E
	public void DespawnAllCritters()
	{
		CrittersManager.instance.QueueDespawnAllCritters();
	}
}
