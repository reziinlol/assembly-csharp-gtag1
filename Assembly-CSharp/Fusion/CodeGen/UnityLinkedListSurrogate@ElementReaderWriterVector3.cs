using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x020013CA RID: 5066
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ElementReaderWriterVector3 : UnityLinkedListSurrogate<Vector3, ElementReaderWriterVector3>
	{
		// Token: 0x17000C03 RID: 3075
		// (get) Token: 0x06007EA6 RID: 32422 RVA: 0x00298CBC File Offset: 0x00296EBC
		// (set) Token: 0x06007EA7 RID: 32423 RVA: 0x00298CC4 File Offset: 0x00296EC4
		[WeaverGenerated]
		public override Vector3[] DataProperty
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

		// Token: 0x06007EA8 RID: 32424 RVA: 0x00298CCD File Offset: 0x00296ECD
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterVector3()
		{
		}

		// Token: 0x04009170 RID: 37232
		[WeaverGenerated]
		public Vector3[] Data = Array.Empty<Vector3>();
	}
}
