using System;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x02000FEE RID: 4078
	public class Chase_State : IState
	{
		// Token: 0x17000993 RID: 2451
		// (get) Token: 0x060065F7 RID: 26103 RVA: 0x0020DFEC File Offset: 0x0020C1EC
		// (set) Token: 0x060065F8 RID: 26104 RVA: 0x0020DFF4 File Offset: 0x0020C1F4
		public Transform FollowTarget { get; set; }

		// Token: 0x060065F9 RID: 26105 RVA: 0x0020DFFD File Offset: 0x0020C1FD
		public Chase_State(AIEntity entity)
		{
			this.entity = entity;
			this.agent = this.entity.navMeshAgent;
		}

		// Token: 0x060065FA RID: 26106 RVA: 0x0020E01D File Offset: 0x0020C21D
		public void Tick()
		{
			this.agent.SetDestination(this.FollowTarget.position);
			if (this.agent.remainingDistance < this.entity.attackDistance)
			{
				this.chaseOver = true;
			}
		}

		// Token: 0x060065FB RID: 26107 RVA: 0x0020E055 File Offset: 0x0020C255
		public void OnEnter()
		{
			this.chaseOver = false;
			string str = "Current State: ";
			Type typeFromHandle = typeof(Chase_State);
			Debug.Log(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
		}

		// Token: 0x060065FC RID: 26108 RVA: 0x0020E083 File Offset: 0x0020C283
		public void OnExit()
		{
			this.chaseOver = true;
		}

		// Token: 0x0400755E RID: 30046
		private AIEntity entity;

		// Token: 0x0400755F RID: 30047
		private NavMeshAgent agent;

		// Token: 0x04007561 RID: 30049
		public bool chaseOver;
	}
}
