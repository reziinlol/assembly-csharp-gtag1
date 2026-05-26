using System;
using UnityEngine;

// Token: 0x0200056D RID: 1389
public class CosmeticCategoryButton : CosmeticButton
{
	// Token: 0x06002356 RID: 9046 RVA: 0x000BE262 File Offset: 0x000BC462
	public void SetIcon(Sprite sprite)
	{
		this.equippedLeftIcon.enabled = false;
		this.equippedRightIcon.enabled = false;
		this.equippedIcon.enabled = (sprite != null);
		this.equippedIcon.sprite = sprite;
	}

	// Token: 0x06002357 RID: 9047 RVA: 0x000BE29C File Offset: 0x000BC49C
	public void SetDualIcon(Sprite leftSprite, Sprite rightSprite)
	{
		this.equippedLeftIcon.enabled = (leftSprite != null);
		this.equippedRightIcon.enabled = (rightSprite != null);
		this.equippedIcon.enabled = false;
		this.equippedLeftIcon.sprite = leftSprite;
		this.equippedRightIcon.sprite = rightSprite;
	}

	// Token: 0x06002358 RID: 9048 RVA: 0x000BE2F4 File Offset: 0x000BC4F4
	public override void UpdatePosition()
	{
		base.UpdatePosition();
		if (this.equippedIcon != null)
		{
			this.equippedIcon.transform.position += this.posOffset;
		}
		if (this.equippedLeftIcon != null)
		{
			this.equippedLeftIcon.transform.position += this.posOffset;
		}
		if (this.equippedRightIcon != null)
		{
			this.equippedRightIcon.transform.position += this.posOffset;
		}
	}

	// Token: 0x04002E6C RID: 11884
	[SerializeField]
	private SpriteRenderer equippedIcon;

	// Token: 0x04002E6D RID: 11885
	[SerializeField]
	private SpriteRenderer equippedLeftIcon;

	// Token: 0x04002E6E RID: 11886
	[SerializeField]
	private SpriteRenderer equippedRightIcon;
}
