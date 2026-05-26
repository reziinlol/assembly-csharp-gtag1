using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013D7 RID: 5079
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ElementReaderWriterByte : UnityLinkedListSurrogate<byte, ElementReaderWriterByte>
	{
		// Token: 0x17000C07 RID: 3079
		// (get) Token: 0x06007EB8 RID: 32440 RVA: 0x00298DBD File Offset: 0x00296FBD
		// (set) Token: 0x06007EB9 RID: 32441 RVA: 0x00298DC5 File Offset: 0x00296FC5
		[WeaverGenerated]
		public override byte[] DataProperty
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

		// Token: 0x06007EBA RID: 32442 RVA: 0x00298DCE File Offset: 0x00296FCE
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterByte()
		{
		}

		// Token: 0x040092FE RID: 37630
		[WeaverGenerated]
		public byte[] Data = Array.Empty<byte>();
	}
}
