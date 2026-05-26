using System;
using System.Collections.Generic;
using UnityEngine;

namespace NetSynchrony
{
	// Token: 0x020010B6 RID: 4278
	[CreateAssetMenu(fileName = "RandomDispatcher", menuName = "NetSynchrony/RandomDispatcher", order = 0)]
	public class RandomDispatcher : ScriptableObject
	{
		// Token: 0x140000BC RID: 188
		// (add) Token: 0x06006B63 RID: 27491 RVA: 0x0022BA10 File Offset: 0x00229C10
		// (remove) Token: 0x06006B64 RID: 27492 RVA: 0x0022BA48 File Offset: 0x00229C48
		public event RandomDispatcher.RandomDispatcherEvent Dispatch;

		// Token: 0x06006B65 RID: 27493 RVA: 0x0022BA80 File Offset: 0x00229C80
		public void Init(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			this.index = 0;
			this.dispatchTimes = new List<float>();
			float num = 0f;
			float num2 = this.totalMinutes * 60f;
			Random.InitState(StaticHash.Compute(Application.buildGUID));
			while (num < num2)
			{
				float num3 = Random.Range(this.minWaitTime, this.maxWaitTime);
				num += num3;
				if ((double)num < seconds)
				{
					this.index = this.dispatchTimes.Count;
				}
				this.dispatchTimes.Add(num);
			}
			Random.InitState((int)DateTime.Now.Ticks);
		}

		// Token: 0x06006B66 RID: 27494 RVA: 0x0022BB24 File Offset: 0x00229D24
		public void Sync(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			this.index = 0;
			for (int i = 0; i < this.dispatchTimes.Count; i++)
			{
				if ((double)this.dispatchTimes[i] < seconds)
				{
					this.index = i;
				}
			}
		}

		// Token: 0x06006B67 RID: 27495 RVA: 0x0022BB78 File Offset: 0x00229D78
		public void Tick(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			if ((double)this.dispatchTimes[this.index] < seconds)
			{
				this.index = (this.index + 1) % this.dispatchTimes.Count;
				if (this.Dispatch != null)
				{
					this.Dispatch(this);
				}
			}
		}

		// Token: 0x04007B6E RID: 31598
		[SerializeField]
		private float minWaitTime = 1f;

		// Token: 0x04007B6F RID: 31599
		[SerializeField]
		private float maxWaitTime = 10f;

		// Token: 0x04007B70 RID: 31600
		[SerializeField]
		private float totalMinutes = 60f;

		// Token: 0x04007B71 RID: 31601
		private List<float> dispatchTimes;

		// Token: 0x04007B72 RID: 31602
		private int index = -1;

		// Token: 0x020010B7 RID: 4279
		// (Invoke) Token: 0x06006B6A RID: 27498
		public delegate void RandomDispatcherEvent(RandomDispatcher randomDispatcher);
	}
}
