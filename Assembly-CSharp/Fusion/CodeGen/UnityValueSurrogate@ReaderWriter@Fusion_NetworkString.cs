using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013B2 RID: 5042
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@Fusion_NetworkString : UnityValueSurrogate<NetworkString<_32>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x17000BF9 RID: 3065
		// (get) Token: 0x06007E7C RID: 32380 RVA: 0x00298AAC File Offset: 0x00296CAC
		// (set) Token: 0x06007E7D RID: 32381 RVA: 0x00298AB4 File Offset: 0x00296CB4
		[WeaverGenerated]
		public override NetworkString<_32> DataProperty
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

		// Token: 0x06007E7E RID: 32382 RVA: 0x00298ABD File Offset: 0x00296CBD
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x04008FD1 RID: 36817
		[WeaverGenerated]
		public NetworkString<_32> Data;
	}
}
