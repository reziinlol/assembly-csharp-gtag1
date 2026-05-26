using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x02001180 RID: 4480
	public class ObjectPool<T> where T : ObjectPoolEvents, new()
	{
		// Token: 0x06007168 RID: 29032 RVA: 0x00250093 File Offset: 0x0024E293
		protected ObjectPool()
		{
		}

		// Token: 0x06007169 RID: 29033 RVA: 0x002500A6 File Offset: 0x0024E2A6
		public ObjectPool(int amount) : this(amount, amount)
		{
		}

		// Token: 0x0600716A RID: 29034 RVA: 0x002500B0 File Offset: 0x0024E2B0
		public ObjectPool(int initialAmount, int maxAmount)
		{
			this.InitializePool(initialAmount, maxAmount);
		}

		// Token: 0x0600716B RID: 29035 RVA: 0x002500CC File Offset: 0x0024E2CC
		protected void InitializePool(int initialAmount, int maxAmount)
		{
			this.maxInstances = maxAmount;
			this.pool = new Stack<T>(initialAmount);
			for (int i = 0; i < initialAmount; i++)
			{
				this.pool.Push(this.CreateInstance());
			}
		}

		// Token: 0x0600716C RID: 29036 RVA: 0x0025010C File Offset: 0x0024E30C
		public T Take()
		{
			T result;
			if (this.pool.Count < 1)
			{
				result = this.CreateInstance();
			}
			else
			{
				result = this.pool.Pop();
			}
			result.OnTaken();
			return result;
		}

		// Token: 0x0600716D RID: 29037 RVA: 0x0025014A File Offset: 0x0024E34A
		public void Return(T instance)
		{
			instance.OnReturned();
			if (this.pool.Count == this.maxInstances)
			{
				return;
			}
			this.pool.Push(instance);
		}

		// Token: 0x0600716E RID: 29038 RVA: 0x00250179 File Offset: 0x0024E379
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual T CreateInstance()
		{
			return Activator.CreateInstance<T>();
		}

		// Token: 0x04008142 RID: 33090
		private Stack<T> pool;

		// Token: 0x04008143 RID: 33091
		public int maxInstances = 500;
	}
}
