using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x020013BD RID: 5053
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion : UnityValueSurrogate<Quaternion, ReaderWriter@UnityEngine_Quaternion>
	{
		// Token: 0x17000BFD RID: 3069
		// (get) Token: 0x06007E8E RID: 32398 RVA: 0x00298B90 File Offset: 0x00296D90
		// (set) Token: 0x06007E8F RID: 32399 RVA: 0x00298B98 File Offset: 0x00296D98
		[WeaverGenerated]
		public override Quaternion DataProperty
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

		// Token: 0x06007E90 RID: 32400 RVA: 0x00298BA1 File Offset: 0x00296DA1
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion()
		{
		}

		// Token: 0x04008FF4 RID: 36852
		[WeaverGenerated]
		public Quaternion Data;
	}
}
