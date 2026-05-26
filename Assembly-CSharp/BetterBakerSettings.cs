using System;
using UnityEngine;

// Token: 0x02000D2E RID: 3374
public class BetterBakerSettings : MonoBehaviour
{
	// Token: 0x04006471 RID: 25713
	[SerializeField]
	public GameObject[] lightMapMaps = new GameObject[9];

	// Token: 0x02000D2F RID: 3375
	[Serializable]
	public struct LightMapMap
	{
		// Token: 0x04006472 RID: 25714
		[SerializeField]
		public string timeOfDayName;

		// Token: 0x04006473 RID: 25715
		[SerializeField]
		public GameObject sceneLightObject;
	}
}
