using System;
using System.Collections;
using GorillaNetworking.Store;
using UnityEngine;

// Token: 0x0200056A RID: 1386
public class TryOnPurchaseButton : GorillaPressableButton
{
	// Token: 0x06002343 RID: 9027 RVA: 0x000BDE64 File Offset: 0x000BC064
	public void Update()
	{
		if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion && !this.bError)
		{
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.myText.text = "UNAVAILABLE";
		}
	}

	// Token: 0x06002344 RID: 9028 RVA: 0x000BDEC6 File Offset: 0x000BC0C6
	public override void ButtonActivation()
	{
		if (this.bError)
		{
			return;
		}
		base.ButtonActivation();
		BundleManager.instance.PressPurchaseTryOnBundleButton();
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06002345 RID: 9029 RVA: 0x000BDEF0 File Offset: 0x000BC0F0
	public void AlreadyOwn()
	{
		if (this.bError)
		{
			return;
		}
		base.enabled = false;
		base.GetComponent<BoxCollider>().enabled = false;
		this.buttonRenderer.material = this.pressedMaterial;
		this.myText.text = this.AlreadyOwnText;
	}

	// Token: 0x06002346 RID: 9030 RVA: 0x000BDF30 File Offset: 0x000BC130
	public void ResetButton()
	{
		if (this.bError)
		{
			return;
		}
		base.enabled = true;
		base.GetComponent<BoxCollider>().enabled = true;
		this.buttonRenderer.material = this.unpressedMaterial;
		this.SetOffText(true, false, false);
	}

	// Token: 0x06002347 RID: 9031 RVA: 0x000BDF68 File Offset: 0x000BC168
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}

	// Token: 0x06002348 RID: 9032 RVA: 0x000BDF77 File Offset: 0x000BC177
	public void ErrorHappened()
	{
		this.bError = true;
		this.myText.text = this.ErrorText;
		this.buttonRenderer.material = this.unpressedMaterial;
		base.enabled = false;
		this.isOn = false;
	}

	// Token: 0x04002E60 RID: 11872
	public bool bError;

	// Token: 0x04002E61 RID: 11873
	public string ErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME";

	// Token: 0x04002E62 RID: 11874
	public string AlreadyOwnText;
}
