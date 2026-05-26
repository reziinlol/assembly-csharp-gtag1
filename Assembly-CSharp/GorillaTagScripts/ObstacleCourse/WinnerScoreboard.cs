using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000F85 RID: 3973
	public class WinnerScoreboard : MonoBehaviour
	{
		// Token: 0x06006330 RID: 25392 RVA: 0x001FE9F4 File Offset: 0x001FCBF4
		public void UpdateBoard(string winner, ObstacleCourse.RaceState _currentState)
		{
			if (this.output == null)
			{
				return;
			}
			switch (_currentState)
			{
			case ObstacleCourse.RaceState.Started:
				Debug.Log(this.raceStarted);
				this.output.text = this.raceStarted;
				return;
			case ObstacleCourse.RaceState.Waiting:
				Debug.Log(this.raceLoading);
				this.output.text = this.raceLoading;
				return;
			case ObstacleCourse.RaceState.Finished:
				Debug.Log(winner + " WON!!");
				this.output.text = winner + " WON!!";
				return;
			default:
				return;
			}
		}

		// Token: 0x040071CA RID: 29130
		public string raceStarted = "RACE STARTED!";

		// Token: 0x040071CB RID: 29131
		public string raceLoading = "RACE LOADING...";

		// Token: 0x040071CC RID: 29132
		[SerializeField]
		private TextMeshPro output;
	}
}
