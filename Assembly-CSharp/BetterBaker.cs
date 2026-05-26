using System;
using UnityEngine;

// Token: 0x02000D29 RID: 3369
public class BetterBaker : MonoBehaviour
{
	// Token: 0x04006465 RID: 25701
	public string bakeryLightmapDirectory;

	// Token: 0x04006466 RID: 25702
	public string dayNightLightmapsDirectory;

	// Token: 0x04006467 RID: 25703
	public GameObject[] allLights;

	// Token: 0x02000D2A RID: 3370
	public struct LightMapMap
	{
		// Token: 0x04006468 RID: 25704
		public string timeOfDayName;

		// Token: 0x04006469 RID: 25705
		public GameObject lightObject;
	}
}
