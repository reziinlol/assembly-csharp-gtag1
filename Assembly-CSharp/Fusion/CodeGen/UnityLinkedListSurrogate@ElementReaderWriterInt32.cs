using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013D8 RID: 5080
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ElementReaderWriterInt32 : UnityLinkedListSurrogate<int, ElementReaderWriterInt32>
	{
		// Token: 0x17000C08 RID: 3080
		// (get) Token: 0x06007EBB RID: 32443 RVA: 0x00298DE1 File Offset: 0x00296FE1
		// (set) Token: 0x06007EBC RID: 32444 RVA: 0x00298DE9 File Offset: 0x00296FE9
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

		// Token: 0x06007EBD RID: 32445 RVA: 0x00298DF2 File Offset: 0x00296FF2
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterInt32()
		{
		}

		// Token: 0x040092FF RID: 37631
		[WeaverGenerated]
		public int[] Data = Array.Empty<int>();
	}
}
