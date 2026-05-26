using System;

namespace GorillaTag.Shared.Scripts.Utilities
{
	// Token: 0x020011DE RID: 4574
	public sealed class GTBitArray
	{
		// Token: 0x17000B0C RID: 2828
		public bool this[int idx]
		{
			get
			{
				if (idx < 0 || idx >= this.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				int num = idx / 32;
				int num2 = idx % 32;
				return ((ulong)this._data[num] & (ulong)(1L << (num2 & 31))) > 0UL;
			}
			set
			{
				if (idx < 0 || idx >= this.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				int num = idx / 32;
				int num2 = idx % 32;
				if (value)
				{
					this._data[num] |= 1U << num2;
					return;
				}
				this._data[num] &= ~(1U << num2);
			}
		}

		// Token: 0x06007300 RID: 29440 RVA: 0x002568D8 File Offset: 0x00254AD8
		public GTBitArray(int length)
		{
			this.Length = length;
			this._data = ((length % 32 == 0) ? new uint[length / 32] : new uint[length / 32 + 1]);
			for (int i = 0; i < this._data.Length; i++)
			{
				this._data[i] = 0U;
			}
		}

		// Token: 0x06007301 RID: 29441 RVA: 0x00256930 File Offset: 0x00254B30
		public void Clear()
		{
			for (int i = 0; i < this._data.Length; i++)
			{
				this._data[i] = 0U;
			}
		}

		// Token: 0x06007302 RID: 29442 RVA: 0x0025695C File Offset: 0x00254B5C
		public void CopyFrom(GTBitArray other)
		{
			if (this.Length != other.Length)
			{
				throw new ArgumentException("Can only copy bit arrays of the same length.");
			}
			for (int i = 0; i < this._data.Length; i++)
			{
				this._data[i] = other._data[i];
			}
		}

		// Token: 0x04008379 RID: 33657
		public readonly int Length;

		// Token: 0x0400837A RID: 33658
		private readonly uint[] _data;
	}
}
