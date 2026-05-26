using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013DD RID: 5085
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ElementReaderWriterSingle : UnityLinkedListSurrogate<float, ElementReaderWriterSingle>
	{
		// Token: 0x17000C0B RID: 3083
		// (get) Token: 0x06007EC4 RID: 32452 RVA: 0x00298E4D File Offset: 0x0029704D
		// (set) Token: 0x06007EC5 RID: 32453 RVA: 0x00298E55 File Offset: 0x00297055
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

		// Token: 0x06007EC6 RID: 32454 RVA: 0x00298E5E File Offset: 0x0029705E
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterSingle()
		{
		}

		// Token: 0x04009315 RID: 37653
		[WeaverGenerated]
		public float[] Data = Array.Empty<float>();
	}
}
