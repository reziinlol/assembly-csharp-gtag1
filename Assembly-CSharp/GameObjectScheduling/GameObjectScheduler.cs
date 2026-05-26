using System;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x0200132D RID: 4909
	public class GameObjectScheduler : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06007BA0 RID: 31648 RVA: 0x002853BC File Offset: 0x002835BC
		private void Start()
		{
			GameObjectScheduler.<Start>d__8 <Start>d__;
			<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<GameObjectScheduler.<Start>d__8>(ref <Start>d__);
		}

		// Token: 0x06007BA1 RID: 31649 RVA: 0x002853F4 File Offset: 0x002835F4
		private void SetInitialState()
		{
			double num;
			this.getActiveState(out this.previousState, out num);
			for (int i = 0; i < this.scheduledGameObject.Length; i++)
			{
				this.scheduledGameObject[i].SetActive(this.previousState);
				if (num > 0.0)
				{
					Animator[] componentsInChildren = this.scheduledGameObject[i].GetComponentsInChildren<Animator>();
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						int fullPathHash = componentsInChildren[j].GetCurrentAnimatorStateInfo(0).fullPathHash;
						componentsInChildren[j].PlayInFixedTime(fullPathHash, 0, (float)num);
					}
				}
			}
			this.lastMinuteCheck = this.getServerTime().Minute;
		}

		// Token: 0x06007BA2 RID: 31650 RVA: 0x00285493 File Offset: 0x00283693
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (this.ready)
			{
				this.SetInitialState();
			}
		}

		// Token: 0x06007BA3 RID: 31651 RVA: 0x00018E11 File Offset: 0x00017011
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06007BA4 RID: 31652 RVA: 0x002854AC File Offset: 0x002836AC
		private void getActiveState(out bool state, out double totalSeconds)
		{
			DateTime serverTime = this.getServerTime();
			DateTime dateTime;
			this.currentNodeIndex = this.schedule.GetCurrentNodeIndex(serverTime, out dateTime);
			if (this.currentNodeIndex == -1)
			{
				state = this.schedule.InitialState;
				totalSeconds = 0.0;
				return;
			}
			if (this.currentNodeIndex < this.schedule.Nodes.Length)
			{
				state = this.schedule.Nodes[this.currentNodeIndex].ActiveState;
				totalSeconds = (serverTime - this.schedule.Nodes[this.currentNodeIndex].DateTime).TotalSeconds;
				return;
			}
			state = this.schedule.Nodes[this.schedule.Nodes.Length - 1].ActiveState;
			totalSeconds = (serverTime - this.schedule.Nodes[this.schedule.Nodes.Length - 1].DateTime).TotalSeconds;
		}

		// Token: 0x06007BA5 RID: 31653 RVA: 0x0022AC13 File Offset: 0x00228E13
		private DateTime getServerTime()
		{
			return GorillaComputer.instance.GetServerTime();
		}

		// Token: 0x06007BA6 RID: 31654 RVA: 0x002855A0 File Offset: 0x002837A0
		private void changeActiveState(bool state)
		{
			if (state)
			{
				for (int i = 0; i < this.scheduledGameObject.Length; i++)
				{
					this.scheduledGameObject[i].SetActive(true);
				}
				if (this.dispatcher != null && this.dispatcher.OnScheduledActivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
			}
			else
			{
				if (this.dispatcher != null && this.dispatcher.OnScheduledDeactivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
				for (int j = 0; j < this.scheduledGameObject.Length; j++)
				{
					this.scheduledGameObject[j].SetActive(false);
				}
			}
		}

		// Token: 0x06007BA7 RID: 31655 RVA: 0x0028564C File Offset: 0x0028384C
		public void SliceUpdate()
		{
			if (!this.ready || (!this.useSecondsFidelity && this.getServerTime().Minute == this.lastMinuteCheck))
			{
				return;
			}
			bool flag;
			double num;
			this.getActiveState(out flag, out num);
			if (this.previousState != flag)
			{
				this.changeActiveState(flag);
				this.previousState = flag;
			}
			this.lastMinuteCheck = this.getServerTime().Minute;
		}

		// Token: 0x04008CEF RID: 36079
		[SerializeField]
		private GameObjectSchedule schedule;

		// Token: 0x04008CF0 RID: 36080
		private GameObject[] scheduledGameObject;

		// Token: 0x04008CF1 RID: 36081
		private GameObjectSchedulerEventDispatcher dispatcher;

		// Token: 0x04008CF2 RID: 36082
		private int currentNodeIndex = -1;

		// Token: 0x04008CF3 RID: 36083
		private bool ready;

		// Token: 0x04008CF4 RID: 36084
		private bool previousState;

		// Token: 0x04008CF5 RID: 36085
		private int lastMinuteCheck = -1;

		// Token: 0x04008CF6 RID: 36086
		public bool useSecondsFidelity;

		// Token: 0x04008CF7 RID: 36087
		public bool debugTime;
	}
}
