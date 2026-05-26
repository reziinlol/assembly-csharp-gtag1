using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000E9B RID: 3739
	public abstract class AverageCalculator<T> where T : struct
	{
		// Token: 0x170008CF RID: 2255
		// (get) Token: 0x06005BE7 RID: 23527 RVA: 0x001D3D22 File Offset: 0x001D1F22
		public T Average
		{
			get
			{
				return this.m_average;
			}
		}

		// Token: 0x06005BE8 RID: 23528 RVA: 0x001D3D2A File Offset: 0x001D1F2A
		public AverageCalculator(int sampleCount)
		{
			this.m_samples = new T[sampleCount];
		}

		// Token: 0x06005BE9 RID: 23529 RVA: 0x001D3D40 File Offset: 0x001D1F40
		public virtual void AddSample(T sample)
		{
			T sample2 = this.m_samples[this.m_index];
			this.m_total = this.MinusEquals(this.m_total, sample2);
			this.m_total = this.PlusEquals(this.m_total, sample);
			this.m_average = this.Divide(this.m_total, this.m_samples.Length);
			this.m_samples[this.m_index] = sample;
			int num = this.m_index + 1;
			this.m_index = num;
			this.m_index = num % this.m_samples.Length;
		}

		// Token: 0x06005BEA RID: 23530 RVA: 0x001D3DD4 File Offset: 0x001D1FD4
		public virtual void Reset()
		{
			T t = this.DefaultTypeValue();
			for (int i = 0; i < this.m_samples.Length; i++)
			{
				this.m_samples[i] = t;
			}
			this.m_index = 0;
			this.m_average = t;
			this.m_total = this.Multiply(t, this.m_samples.Length);
		}

		// Token: 0x06005BEB RID: 23531 RVA: 0x001D3E2C File Offset: 0x001D202C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual T DefaultTypeValue()
		{
			return default(T);
		}

		// Token: 0x06005BEC RID: 23532
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T PlusEquals(T value, T sample);

		// Token: 0x06005BED RID: 23533
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T MinusEquals(T value, T sample);

		// Token: 0x06005BEE RID: 23534
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T Divide(T value, int sampleCount);

		// Token: 0x06005BEF RID: 23535
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T Multiply(T value, int sampleCount);

		// Token: 0x04006A80 RID: 27264
		private T[] m_samples;

		// Token: 0x04006A81 RID: 27265
		private T m_average;

		// Token: 0x04006A82 RID: 27266
		private T m_total;

		// Token: 0x04006A83 RID: 27267
		private int m_index;
	}
}
