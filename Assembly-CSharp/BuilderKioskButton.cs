using System;
using UnityEngine.UI;

// Token: 0x02000624 RID: 1572
public class BuilderKioskButton : GorillaPressableButton
{
	// Token: 0x06002730 RID: 10032 RVA: 0x000D01A6 File Offset: 0x000CE3A6
	public override void Start()
	{
		this.currentPieceSet = BuilderKiosk.nullItem;
	}

	// Token: 0x06002731 RID: 10033 RVA: 0x000D01B3 File Offset: 0x000CE3B3
	public override void UpdateColor()
	{
		if (this.currentPieceSet.isNullItem)
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			this.myText.text = "";
			return;
		}
		base.UpdateColor();
	}

	// Token: 0x06002732 RID: 10034 RVA: 0x000D01EA File Offset: 0x000CE3EA
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
	}

	// Token: 0x040032D7 RID: 13015
	public BuilderSetManager.BuilderSetStoreItem currentPieceSet;

	// Token: 0x040032D8 RID: 13016
	public BuilderKiosk kiosk;

	// Token: 0x040032D9 RID: 13017
	public Text setNameText;
}
