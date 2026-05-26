using System;

namespace BoingKit
{
	// Token: 0x0200138C RID: 5004
	public struct BitArray
	{
		// Token: 0x17000BF4 RID: 3060
		// (get) Token: 0x06007DF5 RID: 32245 RVA: 0x00296A50 File Offset: 0x00294C50
		public int[] Blocks
		{
			get
			{
				return this.m_aBlock;
			}
		}

		// Token: 0x06007DF6 RID: 32246 RVA: 0x00296A58 File Offset: 0x00294C58
		private static int GetBlockIndex(int index)
		{
			return index / 4;
		}

		// Token: 0x06007DF7 RID: 32247 RVA: 0x00296A5D File Offset: 0x00294C5D
		private static int GetSubIndex(int index)
		{
			return index % 4;
		}

		// Token: 0x06007DF8 RID: 32248 RVA: 0x00296A64 File Offset: 0x00294C64
		private static void SetBit(int index, bool value, int[] blocks)
		{
			int blockIndex = BitArray.GetBlockIndex(index);
			int subIndex = BitArray.GetSubIndex(index);
			if (value)
			{
				blocks[blockIndex] |= 1 << subIndex;
				return;
			}
			blocks[blockIndex] &= ~(1 << subIndex);
		}

		// Token: 0x06007DF9 RID: 32249 RVA: 0x00296AA6 File Offset: 0x00294CA6
		private static bool IsBitSet(int index, int[] blocks)
		{
			return (blocks[BitArray.GetBlockIndex(index)] & 1 << BitArray.GetSubIndex(index)) != 0;
		}

		// Token: 0x06007DFA RID: 32250 RVA: 0x00296AC0 File Offset: 0x00294CC0
		public BitArray(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			this.m_aBlock = new int[num];
			this.Clear();
		}

		// Token: 0x06007DFB RID: 32251 RVA: 0x00296AE8 File Offset: 0x00294CE8
		public void Resize(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			if (num <= this.m_aBlock.Length)
			{
				return;
			}
			int[] array = new int[num];
			int i = 0;
			int num2 = this.m_aBlock.Length;
			while (i < num2)
			{
				array[i] = this.m_aBlock[i];
				i++;
			}
			this.m_aBlock = array;
		}

		// Token: 0x06007DFC RID: 32252 RVA: 0x00296B37 File Offset: 0x00294D37
		public void Clear()
		{
			this.SetAllBits(false);
		}

		// Token: 0x06007DFD RID: 32253 RVA: 0x00296B40 File Offset: 0x00294D40
		public void SetAllBits(bool value)
		{
			int num = value ? -1 : 1;
			int i = 0;
			int num2 = this.m_aBlock.Length;
			while (i < num2)
			{
				this.m_aBlock[i] = num;
				i++;
			}
		}

		// Token: 0x06007DFE RID: 32254 RVA: 0x00296B73 File Offset: 0x00294D73
		public void SetBit(int index, bool value)
		{
			BitArray.SetBit(index, value, this.m_aBlock);
		}

		// Token: 0x06007DFF RID: 32255 RVA: 0x00296B82 File Offset: 0x00294D82
		public bool IsBitSet(int index)
		{
			return BitArray.IsBitSet(index, this.m_aBlock);
		}

		// Token: 0x04008F5A RID: 36698
		private int[] m_aBlock;
	}
}
