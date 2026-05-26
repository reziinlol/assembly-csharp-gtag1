using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020004FD RID: 1277
public class CheckoutCartButton : GorillaPressableButton
{
	// Token: 0x06002004 RID: 8196 RVA: 0x000AC2D7 File Offset: 0x000AA4D7
	public override void Start()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
	}

	// Token: 0x06002005 RID: 8197 RVA: 0x000AC2EC File Offset: 0x000AA4EC
	public override void UpdateColor()
	{
		if (this.currentCosmeticItem.itemName == "null")
		{
			if (this.buttonRenderer.IsNotNull())
			{
				this.buttonRenderer.material = this.unpressedMaterial;
			}
			if (this.myText.IsNotNull())
			{
				this.myText.text = this.noCosmeticText;
			}
			if (this.myTmpText.IsNotNull())
			{
				this.myTmpText.text = this.noCosmeticText;
			}
			if (this.myTmpText2.IsNotNull())
			{
				this.myTmpText2.text = this.noCosmeticText;
				return;
			}
		}
		else
		{
			if (this.isOn)
			{
				if (this.buttonRenderer.IsNotNull())
				{
					this.buttonRenderer.material = this.pressedMaterial;
				}
				this.SetOnText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
				return;
			}
			if (this.buttonRenderer.IsNotNull())
			{
				this.buttonRenderer.material = this.unpressedMaterial;
			}
			this.SetOffText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
		}
	}

	// Token: 0x06002006 RID: 8198 RVA: 0x000AC41F File Offset: 0x000AA61F
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCheckoutCartButton(this, isLeftHand);
	}

	// Token: 0x06002007 RID: 8199 RVA: 0x000AC435 File Offset: 0x000AA635
	public void SetItem(CosmeticsController.CosmeticItem item, bool isCurrentItemToBuy)
	{
		this.currentCosmeticItem = item;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.currentCosmeticItem.itemPicture;
		}
		this.isOn = isCurrentItemToBuy;
		this.UpdateColor();
	}

	// Token: 0x06002008 RID: 8200 RVA: 0x000AC46E File Offset: 0x000AA66E
	public void ClearItem()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.blankSprite;
		}
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x04002AC8 RID: 10952
	public CosmeticsController.CosmeticItem currentCosmeticItem;

	// Token: 0x04002AC9 RID: 10953
	[SerializeField]
	private SpriteRenderer currentCosmeticSprite;

	// Token: 0x04002ACA RID: 10954
	[SerializeField]
	private Sprite blankSprite;

	// Token: 0x04002ACB RID: 10955
	public string noCosmeticText;
}
