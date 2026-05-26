using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200055C RID: 1372
public class PurchaseItemButton : GorillaPressableButton
{
	// Token: 0x060022DF RID: 8927 RVA: 0x000BB597 File Offset: 0x000B9797
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressPurchaseItemButton(this, isLeftHand);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x000BB5BA File Offset: 0x000B97BA
	private IEnumerator ButtonColorUpdate()
	{
		Debug.Log("did this happen?");
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}

	// Token: 0x04002DF3 RID: 11763
	public string buttonSide;
}
