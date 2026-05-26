using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x020011C9 RID: 4553
	[Serializable]
	public struct CosmeticCollectionSlotDefinition
	{
		// Token: 0x0400828E RID: 33422
		[Tooltip("Position, rotation and scale of this slot relative to the parent cosmetic's root transform. Edit visually using the Cosmetic Editor Stage.")]
		public XformOffset offset;
	}
}
