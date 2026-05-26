using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02001329 RID: 4905
	[CreateAssetMenu(fileName = "New CountdownText Date", menuName = "Game Object Scheduling/CountdownText Date", order = 1)]
	public class CountdownTextDate : ScriptableObject
	{
		// Token: 0x04008CE2 RID: 36066
		public string CountdownTo = "1/1/0001 00:00:00";

		// Token: 0x04008CE3 RID: 36067
		public string FormatString = "{0} {1}";

		// Token: 0x04008CE4 RID: 36068
		public string DefaultString = "";

		// Token: 0x04008CE5 RID: 36069
		public int DaysThreshold = 365;
	}
}
