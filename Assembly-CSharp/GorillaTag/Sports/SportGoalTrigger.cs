using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02001194 RID: 4500
	public class SportGoalTrigger : MonoBehaviour
	{
		// Token: 0x060071E9 RID: 29161 RVA: 0x002514AF File Offset: 0x0024F6AF
		public void BallExitedGoalTrigger(SportBall ball)
		{
			if (this.ballsPendingTriggerExit.Contains(ball))
			{
				this.ballsPendingTriggerExit.Remove(ball);
			}
		}

		// Token: 0x060071EA RID: 29162 RVA: 0x002514CC File Offset: 0x0024F6CC
		private void PruneBallsPendingTriggerExitByDistance()
		{
			foreach (SportBall sportBall in this.ballsPendingTriggerExit)
			{
				if ((sportBall.transform.position - base.transform.position).sqrMagnitude > this.ballTriggerExitDistanceFallback * this.ballTriggerExitDistanceFallback)
				{
					this.ballsPendingTriggerExit.Remove(sportBall);
				}
			}
		}

		// Token: 0x060071EB RID: 29163 RVA: 0x00251558 File Offset: 0x0024F758
		private void OnTriggerEnter(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.scoreboard != null)
			{
				this.PruneBallsPendingTriggerExitByDistance();
				if (!this.ballsPendingTriggerExit.Contains(componentInParent))
				{
					this.scoreboard.TeamScored(this.teamScoringOnThisGoal);
					this.ballsPendingTriggerExit.Add(componentInParent);
				}
			}
		}

		// Token: 0x04008197 RID: 33175
		[SerializeField]
		private SportScoreboard scoreboard;

		// Token: 0x04008198 RID: 33176
		[SerializeField]
		private int teamScoringOnThisGoal = 1;

		// Token: 0x04008199 RID: 33177
		[SerializeField]
		private float ballTriggerExitDistanceFallback = 3f;

		// Token: 0x0400819A RID: 33178
		private HashSet<SportBall> ballsPendingTriggerExit = new HashSet<SportBall>();
	}
}
