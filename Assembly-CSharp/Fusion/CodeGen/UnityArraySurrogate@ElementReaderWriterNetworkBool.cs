using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013B9 RID: 5049
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterNetworkBool : UnityArraySurrogate<NetworkBool, ElementReaderWriterNetworkBool>
	{
		// Token: 0x17000BFC RID: 3068
		// (get) Token: 0x06007E85 RID: 32389 RVA: 0x00298AF7 File Offset: 0x00296CF7
		// (set) Token: 0x06007E86 RID: 32390 RVA: 0x00298AFF File Offset: 0x00296CFF
		[WeaverGenerated]
		public override NetworkBool[] DataProperty
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

		// Token: 0x06007E87 RID: 32391 RVA: 0x00298B08 File Offset: 0x00296D08
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterNetworkBool()
		{
		}

		// Token: 0x04008FED RID: 36845
		[WeaverGenerated]
		public NetworkBool[] Data = Array.Empty<NetworkBool>();
	}
}
