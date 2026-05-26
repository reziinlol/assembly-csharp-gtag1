using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x020011CE RID: 4558
	[Serializable]
	public struct CosmeticPlacementInfo
	{
		// Token: 0x040082BD RID: 33469
		[Tooltip("The bone to attach the cosmetic to.")]
		public GTHardCodedBones.SturdyEBone parentBone;

		// Token: 0x040082BE RID: 33470
		public XformOffset offset;
	}
}
