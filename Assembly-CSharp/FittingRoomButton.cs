using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200050C RID: 1292
public class FittingRoomButton : GorillaPressableButton
{
	// Token: 0x0600203F RID: 8255 RVA: 0x000AD333 File Offset: 0x000AB533
	public override void Start()
	{
		if (this.currentCosmeticItem.itemName == "")
		{
			this.currentCosmeticItem = CosmeticsController.instance.nullItem;
		}
	}

	// Token: 0x06002040 RID: 8256 RVA: 0x000AD360 File Offset: 0x000AB560
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

	// Token: 0x06002041 RID: 8257 RVA: 0x000AD493 File Offset: 0x000AB693
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		CosmeticsController.instance.PressFittingRoomButton(this, isLeftHand);
	}

	// Token: 0x06002042 RID: 8258 RVA: 0x000AD4AA File Offset: 0x000AB6AA
	public void SetItem(CosmeticsController.CosmeticItem item, bool isInTryOnSet)
	{
		this.currentCosmeticItem = item;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.currentCosmeticItem.itemPicture;
		}
		this.isOn = isInTryOnSet;
		this.UpdateColor();
	}

	// Token: 0x06002043 RID: 8259 RVA: 0x000AD4E4 File Offset: 0x000AB6E4
	public void ClearItem()
	{
		if (this.currentCosmeticItem.isNullItem)
		{
			return;
		}
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.blankSprite;
		}
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x04002B09 RID: 11017
	public CosmeticsController.CosmeticItem currentCosmeticItem;

	// Token: 0x04002B0A RID: 11018
	[SerializeField]
	private SpriteRenderer currentCosmeticSprite;

	// Token: 0x04002B0B RID: 11019
	[SerializeField]
	private Sprite blankSprite;

	// Token: 0x04002B0C RID: 11020
	public string noCosmeticText;
}
