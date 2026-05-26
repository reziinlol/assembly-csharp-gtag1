using System;
using UnityEngine;

// Token: 0x020000A2 RID: 162
public class GameLightingManagerEventRelay : MonoBehaviour
{
	// Token: 0x060003FF RID: 1023 RVA: 0x00017AB3 File Offset: 0x00015CB3
	public void SetCustomDynamicLightingEnabled(bool value)
	{
		if (GameLightingManager.instance == null)
		{
			Debug.LogError("GameLightingManagerEventRelay :: GameLightingManager has not been instanced!");
			return;
		}
		GameLightingManager.instance.ZoneEnableCustomDynamicLighting(value);
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x00017ADC File Offset: 0x00015CDC
	public void SetNearsightedDimLightIntensity(float value)
	{
		if (GameLightingManager.instance == null)
		{
			Debug.LogError("GameLightingManagerEventRelay :: GameLightingManager has not been instanced!");
			return;
		}
		GameLightingManager.instance.GR_NearsightedDimLight.intensity = value;
	}
}
