using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaNetworking;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace GorillaTagScripts.GhostReactor
{
	// Token: 0x02000F8B RID: 3979
	public class GRKiosk : MonoBehaviour
	{
		// Token: 0x0600633F RID: 25407 RVA: 0x001FED98 File Offset: 0x001FCF98
		private void Start()
		{
			GRKiosk.<Start>d__16 <Start>d__;
			<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<GRKiosk.<Start>d__16>(ref <Start>d__);
		}

		// Token: 0x06006340 RID: 25408 RVA: 0x001FEDD0 File Offset: 0x001FCFD0
		private void ProcessPurchaseItemState(GRKiosk.ButtonSide? button, HashSet<GRKiosk.PurchaseState> recentStates = null)
		{
			if (recentStates == null)
			{
				recentStates = new HashSet<GRKiosk.PurchaseState>();
			}
			recentStates.Add(this._purchaseState);
			switch (this._purchaseState)
			{
			case GRKiosk.PurchaseState.Initialize:
				throw new Exception("ProcessPurchaseItemState called in non-initialized GRKiosk!");
			case GRKiosk.PurchaseState.AlreadyOwned:
				this.ResetButtons();
				break;
			case GRKiosk.PurchaseState.AvailableForPurchase:
				this.SetAvailableForPurchaseDisplays(button);
				break;
			case GRKiosk.PurchaseState.CheckoutPressed:
				this.SetCheckoutConfirmationDisplays(button);
				break;
			case GRKiosk.PurchaseState.CheckoutConfirmation:
				this.ConfirmCheckout(button);
				break;
			}
			if (!recentStates.Contains(this._purchaseState))
			{
				this.ProcessPurchaseItemState(null, recentStates);
			}
			this.FormattedPurchaseText();
		}

		// Token: 0x06006341 RID: 25409 RVA: 0x001FEE67 File Offset: 0x001FD067
		private bool PlayerOwnsItem()
		{
			return CosmeticsController.instance.unlockedCosmetics.Any(new Func<CosmeticsController.CosmeticItem, bool>(this.MatchesCosmeticForPurchase));
		}

		// Token: 0x06006342 RID: 25410 RVA: 0x001FEE88 File Offset: 0x001FD088
		private void OnGetCurrency()
		{
			this.ProcessPurchaseItemState(null, null);
		}

		// Token: 0x06006343 RID: 25411 RVA: 0x001FEEA8 File Offset: 0x001FD0A8
		private void ResetButtons()
		{
			this.LeftPurchaseButton.myTmpText.text = "-";
			this.RightPurchaseButton.myTmpText.text = "-";
			this.LeftPurchaseButton.buttonRenderer.material = this.LeftPurchaseButton.pressedMaterial;
			this.RightPurchaseButton.buttonRenderer.material = this.RightPurchaseButton.pressedMaterial;
		}

		// Token: 0x06006344 RID: 25412 RVA: 0x001FEF18 File Offset: 0x001FD118
		private void SetAvailableForPurchaseDisplays(GRKiosk.ButtonSide? button)
		{
			if (this._cosmeticForPurchase.cost <= CosmeticsController.instance.currencyBalance)
			{
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CANCEL", out text, "NO!");
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CONFIRM", out text2, "YES!");
				this.LeftPurchaseButton.myTmpText.text = text;
				this.RightPurchaseButton.myTmpText.text = text2;
				this.LeftPurchaseButton.buttonRenderer.material = this.LeftPurchaseButton.unpressedMaterial;
				this.RightPurchaseButton.buttonRenderer.material = this.RightPurchaseButton.unpressedMaterial;
				GRKiosk.ButtonSide? buttonSide = button;
				GRKiosk.ButtonSide buttonSide2 = GRKiosk.ButtonSide.Right;
				if (buttonSide.GetValueOrDefault() == buttonSide2 & buttonSide != null)
				{
					this._purchaseState = GRKiosk.PurchaseState.CheckoutPressed;
					return;
				}
			}
			else
			{
				this.LeftPurchaseButton.myTmpText.text = "-";
				this.RightPurchaseButton.myTmpText.text = "-";
				this.LeftPurchaseButton.buttonRenderer.material = this.LeftPurchaseButton.pressedMaterial;
				this.RightPurchaseButton.buttonRenderer.material = this.RightPurchaseButton.pressedMaterial;
				this._purchaseState = GRKiosk.PurchaseState.AvailableForPurchase;
			}
		}

		// Token: 0x06006345 RID: 25413 RVA: 0x001FF044 File Offset: 0x001FD244
		private void SetCheckoutConfirmationDisplays(GRKiosk.ButtonSide? button)
		{
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CANCEL", out text, "LET ME THINK ABOUT IT");
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CONFIRM", out text2, "YES! I NEED IT!");
			this.LeftPurchaseButton.myTmpText.text = text2;
			this.RightPurchaseButton.myTmpText.text = text;
			this.LeftPurchaseButton.buttonRenderer.material = this.LeftPurchaseButton.unpressedMaterial;
			this.RightPurchaseButton.buttonRenderer.material = this.RightPurchaseButton.unpressedMaterial;
			this._purchaseState = GRKiosk.PurchaseState.CheckoutConfirmation;
		}

		// Token: 0x06006346 RID: 25414 RVA: 0x001FF0D4 File Offset: 0x001FD2D4
		private void ConfirmCheckout(GRKiosk.ButtonSide? button)
		{
			GRKiosk.ButtonSide? buttonSide = button;
			GRKiosk.ButtonSide buttonSide2 = GRKiosk.ButtonSide.Left;
			if (buttonSide.GetValueOrDefault() == buttonSide2 & buttonSide != null)
			{
				this.PurchaseItem();
				return;
			}
			buttonSide = button;
			buttonSide2 = GRKiosk.ButtonSide.Right;
			if (buttonSide.GetValueOrDefault() == buttonSide2 & buttonSide != null)
			{
				this._purchaseState = GRKiosk.PurchaseState.AvailableForPurchase;
			}
		}

		// Token: 0x06006347 RID: 25415 RVA: 0x001FF120 File Offset: 0x001FD320
		private void PurchaseItem()
		{
			PurchaseItemRequest purchaseItemRequest = new PurchaseItemRequest();
			purchaseItemRequest.ItemId = this._cosmeticForPurchase.itemName;
			purchaseItemRequest.Price = this._cosmeticForPurchase.cost;
			purchaseItemRequest.VirtualCurrency = "SR";
			PlayFabClientAPI.PurchaseItem(purchaseItemRequest, delegate(PurchaseItemResult result)
			{
				this._purchaseState = ((result.Items.Count > 0) ? GRKiosk.PurchaseState.AlreadyOwned : GRKiosk.PurchaseState.AvailableForPurchase);
				if (this._purchaseParticles != null)
				{
					this._purchaseParticles.Play();
				}
				GorillaTagger.Instance.offlineVRRig.AddCosmetic(this._cosmeticForPurchase.itemName);
				this.ProcessPurchaseItemState(null, null);
			}, delegate(PlayFabError error)
			{
				Debug.LogError(error.ToString());
			}, null, null);
		}

		// Token: 0x06006348 RID: 25416 RVA: 0x001FF191 File Offset: 0x001FD391
		private bool MatchesCosmeticForPurchase(CosmeticsController.CosmeticItem item)
		{
			return this.CosmeticNameForPurchase == item.displayName || this.CosmeticNameForPurchase == item.overrideDisplayName || this.CosmeticNameForPurchase == item.itemName;
		}

		// Token: 0x06006349 RID: 25417 RVA: 0x001FF1CC File Offset: 0x001FD3CC
		private void OnLeftPurchaseButtonPressed(GorillaPressableButton button, bool isLeftHand)
		{
			this.ProcessPurchaseItemState(new GRKiosk.ButtonSide?(GRKiosk.ButtonSide.Left), null);
		}

		// Token: 0x0600634A RID: 25418 RVA: 0x001FF1DB File Offset: 0x001FD3DB
		private void OnRightPurchaseButtonPressed(GorillaPressableButton button, bool isLeftHand)
		{
			this.ProcessPurchaseItemState(new GRKiosk.ButtonSide?(GRKiosk.ButtonSide.Right), null);
		}

		// Token: 0x0600634B RID: 25419 RVA: 0x001FF1EC File Offset: 0x001FD3EC
		private void FormattedPurchaseText()
		{
			if (this._itemNameVar == null || this._itemCostVar == null || this._currencyBalanceVar == null)
			{
				Debug.LogError("[LOCALIZATION::GRKIOSK] One of the dynamic variables is NULL and cannot update the [PurchaseText] screen");
				return;
			}
			this._itemNameVar.Value = this._cosmeticForPurchase.displayName.ToUpper();
			this._itemCostVar.Value = this._cosmeticForPurchase.cost;
			this._currencyBalanceVar.Value = CosmeticsController.instance.currencyBalance;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ITEM: ").Append(this._cosmeticForPurchase.overrideDisplayName.ToUpper());
			stringBuilder.Append("\nITEM COST: ").Append(this._cosmeticForPurchase.cost);
			stringBuilder.Append("\nYOU HAVE: ").Append(CosmeticsController.instance.currencyBalance);
			StringBuilder stringBuilder2 = stringBuilder.Append("\n");
			string value;
			switch (this._purchaseState)
			{
			case GRKiosk.PurchaseState.AlreadyOwned:
				value = "YOU ALREADY OWN THIS!";
				break;
			case GRKiosk.PurchaseState.AvailableForPurchase:
				value = "PURCHASE?";
				break;
			case GRKiosk.PurchaseState.CheckoutPressed:
				value = "CONFIRM PURCHASE?";
				break;
			case GRKiosk.PurchaseState.CheckoutConfirmation:
				value = "CONFIRMING PURCHASE...";
				break;
			default:
				value = "ERROR";
				break;
			}
			stringBuilder2.Append(value);
			this.PurchaseText.text = stringBuilder.ToString();
		}

		// Token: 0x040071E5 RID: 29157
		[SerializeField]
		public string CosmeticNameForPurchase;

		// Token: 0x040071E6 RID: 29158
		[SerializeField]
		public GorillaPressableButton LeftPurchaseButton;

		// Token: 0x040071E7 RID: 29159
		[SerializeField]
		public GorillaPressableButton RightPurchaseButton;

		// Token: 0x040071E8 RID: 29160
		[SerializeField]
		public TMP_Text PurchaseText;

		// Token: 0x040071E9 RID: 29161
		private CosmeticsController.CosmeticItem _cosmeticForPurchase;

		// Token: 0x040071EA RID: 29162
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x040071EB RID: 29163
		[SerializeField]
		private AudioClip _purchaseAudioClip;

		// Token: 0x040071EC RID: 29164
		[SerializeField]
		private ParticleSystem _purchaseParticles;

		// Token: 0x040071ED RID: 29165
		[SerializeField]
		private LocalizedText _purchaseTextLoc;

		// Token: 0x040071EE RID: 29166
		private LocalizedString _purchaseTextLocStr;

		// Token: 0x040071EF RID: 29167
		private StringVariable _itemNameVar;

		// Token: 0x040071F0 RID: 29168
		private IntVariable _itemCostVar;

		// Token: 0x040071F1 RID: 29169
		private IntVariable _currencyBalanceVar;

		// Token: 0x040071F2 RID: 29170
		private GRKiosk.PurchaseState _purchaseState;

		// Token: 0x02000F8C RID: 3980
		private enum PurchaseState
		{
			// Token: 0x040071F4 RID: 29172
			Initialize,
			// Token: 0x040071F5 RID: 29173
			AlreadyOwned,
			// Token: 0x040071F6 RID: 29174
			AvailableForPurchase,
			// Token: 0x040071F7 RID: 29175
			CheckoutPressed,
			// Token: 0x040071F8 RID: 29176
			CheckoutConfirmation
		}

		// Token: 0x02000F8D RID: 3981
		private enum ButtonSide
		{
			// Token: 0x040071FA RID: 29178
			Left,
			// Token: 0x040071FB RID: 29179
			Right
		}
	}
}
