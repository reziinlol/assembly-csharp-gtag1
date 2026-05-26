using System;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02001193 RID: 4499
	public class SportGoalExitTrigger : MonoBehaviour
	{
		// Token: 0x060071E7 RID: 29159 RVA: 0x00251478 File Offset: 0x0024F678
		private void OnTriggerExit(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.goalTrigger != null)
			{
				this.goalTrigger.BallExitedGoalTrigger(componentInParent);
			}
		}

		// Token: 0x04008196 RID: 33174
		[SerializeField]
		private SportGoalTrigger goalTrigger;
	}
}
