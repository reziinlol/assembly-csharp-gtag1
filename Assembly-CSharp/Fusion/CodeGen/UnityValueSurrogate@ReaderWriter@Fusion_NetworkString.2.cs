using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013D1 RID: 5073
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@Fusion_NetworkString : UnityValueSurrogate<NetworkString<_128>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x17000C05 RID: 3077
		// (get) Token: 0x06007EB2 RID: 32434 RVA: 0x00298D80 File Offset: 0x00296F80
		// (set) Token: 0x06007EB3 RID: 32435 RVA: 0x00298D88 File Offset: 0x00296F88
		[WeaverGenerated]
		public override NetworkString<_128> DataProperty
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

		// Token: 0x06007EB4 RID: 32436 RVA: 0x00298D91 File Offset: 0x00296F91
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x040092AD RID: 37549
		[WeaverGenerated]
		public NetworkString<_128> Data;
	}
}
