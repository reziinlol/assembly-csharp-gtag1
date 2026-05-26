using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013D4 RID: 5076
	[WeaverGenerated]
	[Serializable]
	internal class UnityDictionarySurrogate@ElementReaderWriterInt32@ElementReaderWriterInt32 : UnityDictionarySurrogate<int, ElementReaderWriterInt32, int, ElementReaderWriterInt32>
	{
		// Token: 0x17000C06 RID: 3078
		// (get) Token: 0x06007EB5 RID: 32437 RVA: 0x00298D99 File Offset: 0x00296F99
		// (set) Token: 0x06007EB6 RID: 32438 RVA: 0x00298DA1 File Offset: 0x00296FA1
		[WeaverGenerated]
		public override SerializableDictionary<int, int> DataProperty
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

		// Token: 0x06007EB7 RID: 32439 RVA: 0x00298DAA File Offset: 0x00296FAA
		[WeaverGenerated]
		public UnityDictionarySurrogate@ElementReaderWriterInt32@ElementReaderWriterInt32()
		{
		}

		// Token: 0x040092F6 RID: 37622
		[WeaverGenerated]
		public SerializableDictionary<int, int> Data = SerializableDictionary.Create<int, int>();
	}
}
