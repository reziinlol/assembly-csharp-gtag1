using System;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x02000FF0 RID: 4080
	public class Patrol_State : IState
	{
		// Token: 0x06006601 RID: 26113 RVA: 0x0020E133 File Offset: 0x0020C333
		public Patrol_State(AIEntity entity)
		{
			this.entity = entity;
			this.agent = this.entity.navMeshAgent;
		}

		// Token: 0x06006602 RID: 26114 RVA: 0x0020E154 File Offset: 0x0020C354
		public void Tick()
		{
			if (this.agent.remainingDistance <= this.agent.stoppingDistance)
			{
				Vector3 position = this.entity.waypoints[Random.Range(0, this.entity.waypoints.Count - 1)].transform.position;
				this.agent.SetDestination(position);
			}
		}

		// Token: 0x06006603 RID: 26115 RVA: 0x0020E1BC File Offset: 0x0020C3BC
		public void OnEnter()
		{
			string str = "Current State: ";
			Type typeFromHandle = typeof(Patrol_State);
			Debug.Log(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
			if (this.entity.waypoints.Count > 0)
			{
				this.agent.SetDestination(this.entity.waypoints[0].transform.position);
			}
		}

		// Token: 0x06006604 RID: 26116 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnExit()
		{
		}

		// Token: 0x04007564 RID: 30052
		private AIEntity entity;

		// Token: 0x04007565 RID: 30053
		private NavMeshAgent agent;
	}
}
