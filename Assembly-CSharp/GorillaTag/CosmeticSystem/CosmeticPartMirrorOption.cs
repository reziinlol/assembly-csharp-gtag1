using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x020011CD RID: 4557
	[Serializable]
	public struct CosmeticPartMirrorOption
	{
		// Token: 0x040082BB RID: 33467
		public ECosmeticPartMirrorAxis axis;

		// Token: 0x040082BC RID: 33468
		[Tooltip("This will multiply the local scale for the selected axis by -1.")]
		public bool negativeScale;
	}
}
