using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200138B RID: 5003
	[Serializable]
	public struct Bits32
	{
		// Token: 0x17000BF3 RID: 3059
		// (get) Token: 0x06007DF0 RID: 32240 RVA: 0x002969F7 File Offset: 0x00294BF7
		public int IntValue
		{
			get
			{
				return this.m_bits;
			}
		}

		// Token: 0x06007DF1 RID: 32241 RVA: 0x002969FF File Offset: 0x00294BFF
		public Bits32(int bits = 0)
		{
			this.m_bits = bits;
		}

		// Token: 0x06007DF2 RID: 32242 RVA: 0x00296A08 File Offset: 0x00294C08
		public void Clear()
		{
			this.m_bits = 0;
		}

		// Token: 0x06007DF3 RID: 32243 RVA: 0x00296A11 File Offset: 0x00294C11
		public void SetBit(int index, bool value)
		{
			if (value)
			{
				this.m_bits |= 1 << index;
				return;
			}
			this.m_bits &= ~(1 << index);
		}

		// Token: 0x06007DF4 RID: 32244 RVA: 0x00296A3E File Offset: 0x00294C3E
		public bool IsBitSet(int index)
		{
			return (this.m_bits & 1 << index) != 0;
		}

		// Token: 0x04008F59 RID: 36697
		[SerializeField]
		private int m_bits;
	}
}
