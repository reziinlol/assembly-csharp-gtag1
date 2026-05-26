using System;
using UnityEngine;

// Token: 0x020006D9 RID: 1753
public class GameLightOverride : MonoBehaviour
{
	// Token: 0x06002C19 RID: 11289 RVA: 0x000EE937 File Offset: 0x000ECB37
	public void MaxGameLightOverride(int newMaxLights)
	{
		GameLightingManager.instance.SetMaxLights(newMaxLights);
	}

	// Token: 0x06002C1A RID: 11290 RVA: 0x000EE946 File Offset: 0x000ECB46
	private void OnDisable()
	{
		GameLightingManager.instance.SetMaxLights(20);
	}
}
