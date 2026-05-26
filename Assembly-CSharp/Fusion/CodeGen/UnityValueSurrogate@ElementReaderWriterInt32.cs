using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013BE RID: 5054
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ElementReaderWriterInt32 : UnityValueSurrogate<int, ElementReaderWriterInt32>
	{
		// Token: 0x17000BFE RID: 3070
		// (get) Token: 0x06007E91 RID: 32401 RVA: 0x00298BA9 File Offset: 0x00296DA9
		// (set) Token: 0x06007E92 RID: 32402 RVA: 0x00298BB1 File Offset: 0x00296DB1
		[WeaverGenerated]
		public override int DataProperty
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

		// Token: 0x06007E93 RID: 32403 RVA: 0x00298BBA File Offset: 0x00296DBA
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterInt32()
		{
		}

		// Token: 0x04008FF5 RID: 36853
		[WeaverGenerated]
		public int Data;
	}
}
