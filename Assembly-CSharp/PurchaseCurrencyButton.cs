using System;
using System.Collections;
using GorillaNetworking.Store;
using UnityEngine;

// Token: 0x0200055A RID: 1370
public class PurchaseCurrencyButton : GorillaPressableButton
{
	// Token: 0x060022D6 RID: 8918 RVA: 0x000BB4C2 File Offset: 0x000B96C2
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		ATM_Manager.instance.PressCurrencyPurchaseButton(base.GetComponentInParent<ATM_UI>(), this.purchaseCurrencySize);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x060022D7 RID: 8919 RVA: 0x000BB4EF File Offset: 0x000B96EF
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.sharedMaterial = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.sharedMaterial = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04002DEE RID: 11758
	public string purchaseCurrencySize;

	// Token: 0x04002DEF RID: 11759
	public float buttonFadeTime = 0.25f;
}
