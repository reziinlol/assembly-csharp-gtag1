using System;
using System.Globalization;
using GameObjectScheduling;
using UnityEngine;

// Token: 0x020000AB RID: 171
[CreateAssetMenu(fileName = "New Game Object Schedule Generator", menuName = "Game Object Scheduling/Game Object Schedule Generator")]
public class GameObjectScheduleGenerator : ScriptableObject
{
	// Token: 0x0600042A RID: 1066 RVA: 0x000187E0 File Offset: 0x000169E0
	private void GenerateSchedule()
	{
		DateTime startDate;
		try
		{
			startDate = DateTime.Parse(this.scheduleStart, CultureInfo.InvariantCulture);
		}
		catch
		{
			Debug.LogError("Don't understand Start Date " + this.scheduleStart);
			return;
		}
		DateTime endDate;
		try
		{
			endDate = DateTime.Parse(this.scheduleEnd, CultureInfo.InvariantCulture);
		}
		catch
		{
			Debug.LogError("Don't understand End Date " + this.scheduleEnd);
			return;
		}
		if (this.scheduleType == GameObjectScheduleGenerator.ScheduleType.DailyShuffle)
		{
			GameObjectSchedule.GenerateDailyShuffle(startDate, endDate, this.schedules);
		}
	}

	// Token: 0x04000489 RID: 1161
	[SerializeField]
	private GameObjectSchedule[] schedules;

	// Token: 0x0400048A RID: 1162
	[SerializeField]
	private string scheduleStart = "1/1/0001 00:00:00";

	// Token: 0x0400048B RID: 1163
	[SerializeField]
	private string scheduleEnd = "1/1/0001 00:00:00";

	// Token: 0x0400048C RID: 1164
	[SerializeField]
	private GameObjectScheduleGenerator.ScheduleType scheduleType;

	// Token: 0x020000AC RID: 172
	private enum ScheduleType
	{
		// Token: 0x0400048E RID: 1166
		DailyShuffle
	}
}
