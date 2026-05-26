using System;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x0200130D RID: 4877
	[Serializable]
	public abstract class LiteCrusher
	{
		// Token: 0x06007AD0 RID: 31440 RVA: 0x0028290C File Offset: 0x00280B0C
		public static int GetBitsForMaxValue(uint maxvalue)
		{
			for (int i = 0; i < 32; i++)
			{
				if (maxvalue >> i == 0U)
				{
					return i;
				}
			}
			return 32;
		}

		// Token: 0x04008C63 RID: 35939
		[SerializeField]
		protected int bits;
	}
}
