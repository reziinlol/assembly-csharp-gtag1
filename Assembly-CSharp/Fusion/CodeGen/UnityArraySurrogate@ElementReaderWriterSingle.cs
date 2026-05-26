using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013DA RID: 5082
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterSingle : UnityArraySurrogate<float, ElementReaderWriterSingle>
	{
		// Token: 0x17000C0A RID: 3082
		// (get) Token: 0x06007EC1 RID: 32449 RVA: 0x00298E29 File Offset: 0x00297029
		// (set) Token: 0x06007EC2 RID: 32450 RVA: 0x00298E31 File Offset: 0x00297031
		[WeaverGenerated]
		public override float[] DataProperty
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

		// Token: 0x06007EC3 RID: 32451 RVA: 0x00298E3A File Offset: 0x0029703A
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterSingle()
		{
		}

		// Token: 0x04009301 RID: 37633
		[WeaverGenerated]
		public float[] Data = Array.Empty<float>();
	}
}
