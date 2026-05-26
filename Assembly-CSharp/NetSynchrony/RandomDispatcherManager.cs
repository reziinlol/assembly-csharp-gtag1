using System;
using GorillaNetworking;
using UnityEngine;

namespace NetSynchrony
{
	// Token: 0x020010B8 RID: 4280
	public class RandomDispatcherManager : MonoBehaviour
	{
		// Token: 0x06006B6D RID: 27501 RVA: 0x0022BC0C File Offset: 0x00229E0C
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (GorillaComputer.instance != null)
			{
				GorillaComputer instance = GorillaComputer.instance;
				instance.OnServerTimeUpdated = (Action)Delegate.Remove(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			}
		}

		// Token: 0x06006B6E RID: 27502 RVA: 0x0022BC58 File Offset: 0x00229E58
		private void OnTimeChanged()
		{
			this.AdjustedServerTime();
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Sync(this.serverTime);
			}
		}

		// Token: 0x06006B6F RID: 27503 RVA: 0x0022BC94 File Offset: 0x00229E94
		private void AdjustedServerTime()
		{
			DateTime dateTime = new DateTime(2020, 1, 1);
			long num = GorillaComputer.instance.GetServerTime().Ticks - dateTime.Ticks;
			this.serverTime = (double)((float)num / 10000000f);
		}

		// Token: 0x06006B70 RID: 27504 RVA: 0x0022BCDC File Offset: 0x00229EDC
		private void Start()
		{
			GorillaComputer instance = GorillaComputer.instance;
			instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Init(this.serverTime);
			}
		}

		// Token: 0x06006B71 RID: 27505 RVA: 0x0022BD38 File Offset: 0x00229F38
		private void Update()
		{
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Tick(this.serverTime);
			}
			this.serverTime += (double)Time.deltaTime;
		}

		// Token: 0x04007B73 RID: 31603
		[SerializeField]
		private RandomDispatcher[] randomDispatchers;

		// Token: 0x04007B74 RID: 31604
		private static RandomDispatcherManager __instance;

		// Token: 0x04007B75 RID: 31605
		private double serverTime;
	}
}
