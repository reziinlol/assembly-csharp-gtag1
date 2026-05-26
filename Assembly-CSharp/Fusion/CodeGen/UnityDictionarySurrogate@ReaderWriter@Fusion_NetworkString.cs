using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013C1 RID: 5057
	[WeaverGenerated]
	[Serializable]
	internal class UnityDictionarySurrogate@ReaderWriter@Fusion_NetworkString`1<Fusion__32>@ReaderWriter@Fusion_NetworkString : UnityDictionarySurrogate<NetworkString<_32>, ReaderWriter@Fusion_NetworkString, NetworkString<_32>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x17000BFF RID: 3071
		// (get) Token: 0x06007E94 RID: 32404 RVA: 0x00298BC2 File Offset: 0x00296DC2
		// (set) Token: 0x06007E95 RID: 32405 RVA: 0x00298BCA File Offset: 0x00296DCA
		[WeaverGenerated]
		public override SerializableDictionary<NetworkString<_32>, NetworkString<_32>> DataProperty
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

		// Token: 0x06007E96 RID: 32406 RVA: 0x00298BD3 File Offset: 0x00296DD3
		[WeaverGenerated]
		public UnityDictionarySurrogate@ReaderWriter@Fusion_NetworkString`1<Fusion__32>@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x040090C6 RID: 37062
		[WeaverGenerated]
		public SerializableDictionary<NetworkString<_32>, NetworkString<_32>> Data = SerializableDictionary.Create<NetworkString<_32>, NetworkString<_32>>();
	}
}
