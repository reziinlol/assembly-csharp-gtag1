using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000505 RID: 1285
public class CosmeticStand : GorillaPressableButton
{
	// Token: 0x0600201A RID: 8218 RVA: 0x000AC950 File Offset: 0x000AAB50
	public void InitializeCosmetic()
	{
		this.thisCosmeticItem = CosmeticsController.instance.allCosmetics.Find((CosmeticsController.CosmeticItem x) => this.thisCosmeticName == x.displayName || this.thisCosmeticName == x.overrideDisplayName || this.thisCosmeticName == x.itemName);
		if (this.slotPriceText != null)
		{
			this.slotPriceText.text = this.thisCosmeticItem.itemCategory.ToString().ToUpper() + " " + this.thisCosmeticItem.cost.ToString();
		}
	}

	// Token: 0x0600201B RID: 8219 RVA: 0x000AC9CE File Offset: 0x000AABCE
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCosmeticStandButton(this);
	}

	// Token: 0x04002AE6 RID: 10982
	public CosmeticsController.CosmeticItem thisCosmeticItem;

	// Token: 0x04002AE7 RID: 10983
	public string thisCosmeticName;

	// Token: 0x04002AE8 RID: 10984
	public HeadModel thisHeadModel;

	// Token: 0x04002AE9 RID: 10985
	public Text slotPriceText;

	// Token: 0x04002AEA RID: 10986
	public Text addToCartText;

	// Token: 0x04002AEB RID: 10987
	[Tooltip("If this is true then this cosmetic stand should have already been updated when the 'Update Cosmetic Stands' button was pressed in the CosmeticsController inspector.")]
	public bool skipMe;
}
