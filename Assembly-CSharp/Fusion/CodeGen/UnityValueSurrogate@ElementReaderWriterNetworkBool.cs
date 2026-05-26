using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013AE RID: 5038
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ElementReaderWriterNetworkBool : UnityValueSurrogate<NetworkBool, ElementReaderWriterNetworkBool>
	{
		// Token: 0x17000BF8 RID: 3064
		// (get) Token: 0x06007E73 RID: 32371 RVA: 0x00298A15 File Offset: 0x00296C15
		// (set) Token: 0x06007E74 RID: 32372 RVA: 0x00298A1D File Offset: 0x00296C1D
		[WeaverGenerated]
		public override NetworkBool DataProperty
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

		// Token: 0x06007E75 RID: 32373 RVA: 0x00298A26 File Offset: 0x00296C26
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterNetworkBool()
		{
		}

		// Token: 0x04008FAD RID: 36781
		[WeaverGenerated]
		public NetworkBool Data;
	}
}
