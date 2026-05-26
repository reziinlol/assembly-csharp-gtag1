using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x020013B5 RID: 5045
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ElementReaderWriterVector3 : UnityValueSurrogate<Vector3, ElementReaderWriterVector3>
	{
		// Token: 0x17000BFA RID: 3066
		// (get) Token: 0x06007E7F RID: 32383 RVA: 0x00298AC5 File Offset: 0x00296CC5
		// (set) Token: 0x06007E80 RID: 32384 RVA: 0x00298ACD File Offset: 0x00296CCD
		[WeaverGenerated]
		public override Vector3 DataProperty
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

		// Token: 0x06007E81 RID: 32385 RVA: 0x00298AD6 File Offset: 0x00296CD6
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterVector3()
		{
		}

		// Token: 0x04008FD6 RID: 36822
		[WeaverGenerated]
		public Vector3 Data;
	}
}
