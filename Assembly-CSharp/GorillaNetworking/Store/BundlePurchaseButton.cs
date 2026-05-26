using System;
using System.Collections;
using Cosmetics;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02001099 RID: 4249
	public class BundlePurchaseButton : GorillaPressableButton, IGorillaSliceableSimple
	{
		// Token: 0x06006A88 RID: 27272 RVA: 0x00018E08 File Offset: 0x00017008
		public new void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06006A89 RID: 27273 RVA: 0x00018E11 File Offset: 0x00017011
		public new void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06006A8A RID: 27274 RVA: 0x00227078 File Offset: 0x00225278
		public void SliceUpdate()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion && !this.bError)
			{
				base.enabled = false;
				base.GetComponent<BoxCollider>().enabled = false;
				this.buttonRenderer.material = this.pressedMaterial;
				this.myText.text = this.UnavailableText;
			}
		}

		// Token: 0x06006A8B RID: 27275 RVA: 0x002270DB File Offset: 0x002252DB
		public override void ButtonActivation()
		{
			if (this.bError)
			{
				return;
			}
			base.ButtonActivation();
			BundleManager.instance.BundlePurchaseButtonPressed(this.playfabID, this.codeProvider);
			base.StartCoroutine(this.ButtonColorUpdate());
		}

		// Token: 0x06006A8C RID: 27276 RVA: 0x00227114 File Offset: 0x00225314
		public void AlreadyOwn()
		{
			if (this.bError)
			{
				return;
			}
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.onText = this.AlreadyOwnText;
			this.myText.text = this.AlreadyOwnText;
			this.isOn = true;
		}

		// Token: 0x06006A8D RID: 27277 RVA: 0x00227172 File Offset: 0x00225372
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
			this.isOn = false;
		}

		// Token: 0x06006A8E RID: 27278 RVA: 0x002271B1 File Offset: 0x002253B1
		private IEnumerator ButtonColorUpdate()
		{
			this.buttonRenderer.material = this.pressedMaterial;
			yield return new WaitForSeconds(this.debounceTime);
			this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
			yield break;
		}

		// Token: 0x06006A8F RID: 27279 RVA: 0x002271C0 File Offset: 0x002253C0
		public void ErrorHappened()
		{
			this.bError = true;
			this.myText.text = this.ErrorText;
			this.buttonRenderer.material = this.unpressedMaterial;
			base.enabled = false;
			this.offText = this.ErrorText;
			this.onText = this.ErrorText;
			this.isOn = false;
		}

		// Token: 0x06006A90 RID: 27280 RVA: 0x0022721C File Offset: 0x0022541C
		public void InitializeData()
		{
			if (this.bError)
			{
				return;
			}
			this.SetOffText(true, false, false);
			this.buttonRenderer.material = this.unpressedMaterial;
			base.enabled = true;
			this.isOn = false;
		}

		// Token: 0x06006A91 RID: 27281 RVA: 0x0022724F File Offset: 0x0022544F
		public void UpdatePurchaseButtonText(string purchaseText)
		{
			if (!this.bError)
			{
				this.offText = purchaseText;
				this.UpdateColor();
			}
		}

		// Token: 0x04007AC0 RID: 31424
		private const string MONKE_BLOCKS_BUNDLE_ALREADY_OWN_KEY = "MONKE_BLOCKS_BUNDLE_ALREADY_OWN";

		// Token: 0x04007AC1 RID: 31425
		private const string MONKE_BLOCKS_BUNDLE_UNAVAILABLE_KEY = "MONKE_BLOCKS_BUNDLE_UNAVAILABLE";

		// Token: 0x04007AC2 RID: 31426
		private const string MONKE_BLOCKS_BUNDLE_ERROR_KEY = "MONKE_BLOCKS_BUNDLE_ERROR";

		// Token: 0x04007AC3 RID: 31427
		public bool bError;

		// Token: 0x04007AC4 RID: 31428
		public string ErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME";

		// Token: 0x04007AC5 RID: 31429
		public string AlreadyOwnText = "YOU OWN THE BUNDLE ALREADY! THANK YOU!";

		// Token: 0x04007AC6 RID: 31430
		public string UnavailableText = "UNAVAILABLE";

		// Token: 0x04007AC7 RID: 31431
		public string playfabID = "";

		// Token: 0x04007AC8 RID: 31432
		public ICreatorCodeProvider codeProvider;
	}
}
