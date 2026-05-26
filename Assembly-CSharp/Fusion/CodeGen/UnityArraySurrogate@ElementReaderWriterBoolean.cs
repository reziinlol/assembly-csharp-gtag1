using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020013D9 RID: 5081
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterBoolean : UnityArraySurrogate<bool, ElementReaderWriterBoolean>
	{
		// Token: 0x17000C09 RID: 3081
		// (get) Token: 0x06007EBE RID: 32446 RVA: 0x00298E05 File Offset: 0x00297005
		// (set) Token: 0x06007EBF RID: 32447 RVA: 0x00298E0D File Offset: 0x0029700D
		[WeaverGenerated]
		public override bool[] DataProperty
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

		// Token: 0x06007EC0 RID: 32448 RVA: 0x00298E16 File Offset: 0x00297016
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterBoolean()
		{
		}

		// Token: 0x04009300 RID: 37632
		[WeaverGenerated]
		public bool[] Data = Array.Empty<bool>();
	}
}
