using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x0200132A RID: 4906
	[CreateAssetMenu(fileName = "New Game Object Schedule", menuName = "Game Object Scheduling/Game Object Schedule", order = 0)]
	public class GameObjectSchedule : ScriptableObject
	{
		// Token: 0x17000BBA RID: 3002
		// (get) Token: 0x06007B92 RID: 31634 RVA: 0x002850EE File Offset: 0x002832EE
		public GameObjectSchedule.GameObjectScheduleNode[] Nodes
		{
			get
			{
				return this.nodes;
			}
		}

		// Token: 0x17000BBB RID: 3003
		// (get) Token: 0x06007B93 RID: 31635 RVA: 0x002850F6 File Offset: 0x002832F6
		public bool InitialState
		{
			get
			{
				return this.initialState;
			}
		}

		// Token: 0x06007B94 RID: 31636 RVA: 0x00285100 File Offset: 0x00283300
		public int GetCurrentNodeIndex(DateTime currentDate, out DateTime startDate)
		{
			int i = -1;
			startDate = default(DateTime);
			while (i < this.nodes.Length - 1)
			{
				if (currentDate < this.nodes[i + 1].DateTime)
				{
					if (i >= 0)
					{
						startDate = this.nodes[i].DateTime;
					}
					return i;
				}
				i++;
			}
			return int.MaxValue;
		}

		// Token: 0x06007B95 RID: 31637 RVA: 0x0028515E File Offset: 0x0028335E
		public void Validate()
		{
			if (this.validated)
			{
				return;
			}
			this._validate();
			this.validated = true;
		}

		// Token: 0x06007B96 RID: 31638 RVA: 0x00285178 File Offset: 0x00283378
		private void _validate()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i].Validate();
			}
			List<GameObjectSchedule.GameObjectScheduleNode> list = new List<GameObjectSchedule.GameObjectScheduleNode>(this.nodes);
			list.Sort((GameObjectSchedule.GameObjectScheduleNode e1, GameObjectSchedule.GameObjectScheduleNode e2) => e1.DateTime.CompareTo(e2.DateTime));
			this.nodes = list.ToArray();
		}

		// Token: 0x06007B97 RID: 31639 RVA: 0x002851E4 File Offset: 0x002833E4
		public static void GenerateDailyShuffle(DateTime startDate, DateTime endDate, GameObjectSchedule[] schedules)
		{
			TimeSpan t = TimeSpan.FromDays(1.0);
			int num = schedules.Length - 1;
			int num2 = schedules.Length - 2;
			DateTime dateTime = startDate;
			List<GameObjectSchedule.GameObjectScheduleNode>[] array = new List<GameObjectSchedule.GameObjectScheduleNode>[schedules.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new List<GameObjectSchedule.GameObjectScheduleNode>();
			}
			while (dateTime < endDate)
			{
				int num3 = Random.Range(0, schedules.Length - 2);
				if (num <= num3)
				{
					num3++;
					if (num2 <= num3)
					{
						num3++;
					}
				}
				else if (num2 <= num3)
				{
					num3++;
					if (num <= num3)
					{
						num3++;
					}
				}
				array[num].Add(new GameObjectSchedule.GameObjectScheduleNode
				{
					activeDateTime = dateTime.ToString(),
					activeState = false
				});
				array[num3].Add(new GameObjectSchedule.GameObjectScheduleNode
				{
					activeDateTime = dateTime.ToString(),
					activeState = true
				});
				dateTime += t;
				num2 = num;
				num = num3;
			}
			array[num].Add(new GameObjectSchedule.GameObjectScheduleNode
			{
				activeDateTime = dateTime.ToString(),
				activeState = false
			});
			for (int j = 0; j < array.Length; j++)
			{
				schedules[j].nodes = array[j].ToArray();
			}
		}

		// Token: 0x04008CE6 RID: 36070
		[SerializeField]
		private bool initialState;

		// Token: 0x04008CE7 RID: 36071
		[SerializeField]
		private GameObjectSchedule.GameObjectScheduleNode[] nodes;

		// Token: 0x04008CE8 RID: 36072
		[SerializeField]
		private SchedulingOptions options;

		// Token: 0x04008CE9 RID: 36073
		private bool validated;

		// Token: 0x0200132B RID: 4907
		[Serializable]
		public class GameObjectScheduleNode
		{
			// Token: 0x17000BBC RID: 3004
			// (get) Token: 0x06007B99 RID: 31641 RVA: 0x0028531B File Offset: 0x0028351B
			public bool ActiveState
			{
				get
				{
					return this.activeState;
				}
			}

			// Token: 0x17000BBD RID: 3005
			// (get) Token: 0x06007B9A RID: 31642 RVA: 0x00285323 File Offset: 0x00283523
			public DateTime DateTime
			{
				get
				{
					return this.dateTime;
				}
			}

			// Token: 0x06007B9B RID: 31643 RVA: 0x0028532C File Offset: 0x0028352C
			public void Validate()
			{
				try
				{
					this.dateTime = DateTime.Parse(this.activeDateTime, CultureInfo.InvariantCulture);
				}
				catch
				{
					this.dateTime = DateTime.MinValue;
				}
			}

			// Token: 0x04008CEA RID: 36074
			[SerializeField]
			public string activeDateTime = "1/1/0001 00:00:00";

			// Token: 0x04008CEB RID: 36075
			[SerializeField]
			[Tooltip("Check to turn on. Uncheck to turn off.")]
			public bool activeState = true;

			// Token: 0x04008CEC RID: 36076
			private DateTime dateTime;
		}
	}
}
