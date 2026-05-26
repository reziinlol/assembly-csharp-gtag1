using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02001331 RID: 4913
	[CreateAssetMenu(fileName = "New Options", menuName = "Game Object Scheduling/Options", order = 0)]
	public class SchedulingOptions : ScriptableObject
	{
		// Token: 0x17000BC0 RID: 3008
		// (get) Token: 0x06007BAF RID: 31663 RVA: 0x0028585E File Offset: 0x00283A5E
		public DateTime DtDebugServerTime
		{
			get
			{
				return this.dtDebugServerTime.AddSeconds((double)(Time.time * this.timescale));
			}
		}

		// Token: 0x04008D00 RID: 36096
		[SerializeField]
		private string debugServerTime;

		// Token: 0x04008D01 RID: 36097
		[SerializeField]
		private DateTime dtDebugServerTime;

		// Token: 0x04008D02 RID: 36098
		[SerializeField]
		[Range(-60f, 3660f)]
		private float timescale = 1f;
	}
}
