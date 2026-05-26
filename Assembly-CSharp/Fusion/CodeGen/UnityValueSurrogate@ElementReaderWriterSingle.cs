using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013B6 RID: 5046
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ElementReaderWriterSingle : UnityValueSurrogate<float, ElementReaderWriterSingle>
	{
		// Token: 0x17000BFB RID: 3067
		// (get) Token: 0x06007E82 RID: 32386 RVA: 0x00298ADE File Offset: 0x00296CDE
		// (set) Token: 0x06007E83 RID: 32387 RVA: 0x00298AE6 File Offset: 0x00296CE6
		[WeaverGenerated]
		public override float DataProperty
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

		// Token: 0x06007E84 RID: 32388 RVA: 0x00298AEF File Offset: 0x00296CEF
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterSingle()
		{
		}

		// Token: 0x04008FD7 RID: 36823
		[WeaverGenerated]
		public float Data;
	}
}
