using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x020011C8 RID: 4552
	[Serializable]
	public struct CosmeticAttachInfo
	{
		// Token: 0x17000B01 RID: 2817
		// (get) Token: 0x060072B6 RID: 29366 RVA: 0x00255334 File Offset: 0x00253534
		public static CosmeticAttachInfo Identity
		{
			get
			{
				return new CosmeticAttachInfo
				{
					selectSide = ECosmeticSelectSide.Both,
					parentBone = GTHardCodedBones.EBone.None,
					offset = XformOffset.Identity
				};
			}
		}

		// Token: 0x060072B7 RID: 29367 RVA: 0x00255370 File Offset: 0x00253570
		public CosmeticAttachInfo(ECosmeticSelectSide selectSide, GTHardCodedBones.EBone parentBone, XformOffset offset)
		{
			this.selectSide = selectSide;
			this.parentBone = parentBone;
			this.offset = offset;
		}

		// Token: 0x0400828B RID: 33419
		[Tooltip("(Not used for holdables) Determines if the cosmetic part be shown depending on the hand that is used to press the in-game wardrobe \"EQUIP\" button.\n- Both: Show no matter what hand is used.\n- Left: Only show if the left hand selected.\n- Right: Only show if the right hand selected.\n")]
		public StringEnum<ECosmeticSelectSide> selectSide;

		// Token: 0x0400828C RID: 33420
		public GTHardCodedBones.SturdyEBone parentBone;

		// Token: 0x0400828D RID: 33421
		public XformOffset offset;
	}
}
