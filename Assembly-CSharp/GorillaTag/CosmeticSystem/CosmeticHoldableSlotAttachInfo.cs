using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x020011CA RID: 4554
	[Serializable]
	public struct CosmeticHoldableSlotAttachInfo
	{
		// Token: 0x0400828F RID: 33423
		[Tooltip("The anchor that this holdable cosmetic can attach to.")]
		public GTSturdyEnum<GTHardCodedBones.EHandAndStowSlots> stowSlot;

		// Token: 0x04008290 RID: 33424
		public XformOffset offset;
	}
}
