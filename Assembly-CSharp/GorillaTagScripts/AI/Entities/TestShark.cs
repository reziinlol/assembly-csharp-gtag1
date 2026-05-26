using System;
using System.Runtime.CompilerServices;
using GorillaTagScripts.AI.States;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.AI.Entities
{
	// Token: 0x02000FF1 RID: 4081
	public class TestShark : AIEntity
	{
		// Token: 0x06006605 RID: 26117 RVA: 0x0020E228 File Offset: 0x0020C428
		private new void Awake()
		{
			base.Awake();
			this.chasingTimer = 0f;
			this._stateMachine = new StateMachine();
			this.circularPatrol = new CircularPatrol_State(this);
			this.patrol = new Patrol_State(this);
			this.chase = new Chase_State(this);
			this._stateMachine.AddTransition(this.patrol, this.chase, this.<Awake>g__ShouldChase|7_0());
			this._stateMachine.AddTransition(this.chase, this.patrol, this.<Awake>g__ShouldPatrol|7_1());
			this._stateMachine.SetState(this.patrol);
		}

		// Token: 0x06006606 RID: 26118 RVA: 0x0020E2C0 File Offset: 0x0020C4C0
		private void Update()
		{
			this._stateMachine.Tick();
			this.shouldChase = false;
			this.chasingTimer += Time.deltaTime;
			if (this.chasingTimer >= this.nextTimeToChasePlayer)
			{
				base.ChooseClosestTarget();
				if (this.followTarget != null)
				{
					this.chase.FollowTarget = this.followTarget;
					this.shouldChase = true;
				}
				this.chasingTimer = 0f;
			}
		}

		// Token: 0x06006608 RID: 26120 RVA: 0x0020E349 File Offset: 0x0020C549
		[CompilerGenerated]
		private Func<bool> <Awake>g__ShouldChase|7_0()
		{
			return () => this.shouldChase && PhotonNetwork.InRoom;
		}

		// Token: 0x0600660A RID: 26122 RVA: 0x0020E368 File Offset: 0x0020C568
		[CompilerGenerated]
		private Func<bool> <Awake>g__ShouldPatrol|7_1()
		{
			return () => this.chase.chaseOver;
		}

		// Token: 0x04007566 RID: 30054
		public float nextTimeToChasePlayer = 30f;

		// Token: 0x04007567 RID: 30055
		private float chasingTimer;

		// Token: 0x04007568 RID: 30056
		private bool shouldChase;

		// Token: 0x04007569 RID: 30057
		private StateMachine _stateMachine;

		// Token: 0x0400756A RID: 30058
		private CircularPatrol_State circularPatrol;

		// Token: 0x0400756B RID: 30059
		private Patrol_State patrol;

		// Token: 0x0400756C RID: 30060
		private Chase_State chase;
	}
}
