using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x020013CD RID: 5069
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion : UnityLinkedListSurrogate<Quaternion, ReaderWriter@UnityEngine_Quaternion>
	{
		// Token: 0x17000C04 RID: 3076
		// (get) Token: 0x06007EA9 RID: 32425 RVA: 0x00298CE0 File Offset: 0x00296EE0
		// (set) Token: 0x06007EAA RID: 32426 RVA: 0x00298CE8 File Offset: 0x00296EE8
		[WeaverGenerated]
		public override Quaternion[] DataProperty
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

		// Token: 0x06007EAB RID: 32427 RVA: 0x00298CF1 File Offset: 0x00296EF1
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion()
		{
		}

		// Token: 0x04009229 RID: 37417
		[WeaverGenerated]
		public Quaternion[] Data = Array.Empty<Quaternion>();
	}
}
