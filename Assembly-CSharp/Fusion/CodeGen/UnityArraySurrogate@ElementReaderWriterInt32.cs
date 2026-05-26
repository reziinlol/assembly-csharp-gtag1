using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013C5 RID: 5061
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterInt32 : UnityArraySurrogate<int, ElementReaderWriterInt32>
	{
		// Token: 0x17000C01 RID: 3073
		// (get) Token: 0x06007E9A RID: 32410 RVA: 0x00298C0A File Offset: 0x00296E0A
		// (set) Token: 0x06007E9B RID: 32411 RVA: 0x00298C12 File Offset: 0x00296E12
		[WeaverGenerated]
		public override int[] DataProperty
		{
			[WeaverGenerated]
			get
			{
				return this.Data;
			}
			[WeaverGenerated]
			set
			{
				this.Data = value;
			}
		}

		// Token: 0x06007E9C RID: 32412 RVA: 0x00298C1B File Offset: 0x00296E1B
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterInt32()
		{
		}

		// Token: 0x040090D3 RID: 37075
		[WeaverGenerated]
		public int[] Data = Array.Empty<int>();
	}
}
