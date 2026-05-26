using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013C4 RID: 5060
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterInt64 : UnityArraySurrogate<long, ElementReaderWriterInt64>
	{
		// Token: 0x17000C00 RID: 3072
		// (get) Token: 0x06007E97 RID: 32407 RVA: 0x00298BE6 File Offset: 0x00296DE6
		// (set) Token: 0x06007E98 RID: 32408 RVA: 0x00298BEE File Offset: 0x00296DEE
		[WeaverGenerated]
		public override long[] DataProperty
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

		// Token: 0x06007E99 RID: 32409 RVA: 0x00298BF7 File Offset: 0x00296DF7
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterInt64()
		{
		}

		// Token: 0x040090D2 RID: 37074
		[WeaverGenerated]
		public long[] Data = Array.Empty<long>();
	}
}
