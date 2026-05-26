using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001156 RID: 4438
	[CreateAssetMenu(fileName = "WatchableIntSO", menuName = "ScriptableObjects/WatchableIntSO")]
	public class WatchableIntSO : WatchableGenericSO<int>
	{
		// Token: 0x17000ABC RID: 2748
		// (get) Token: 0x0600706B RID: 28779 RVA: 0x0024A71E File Offset: 0x0024891E
		private int currentValue
		{
			get
			{
				return base.Value;
			}
		}
	}
}
