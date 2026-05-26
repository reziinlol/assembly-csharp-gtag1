using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x0200014C RID: 332
[DefaultExecutionOrder(500)]
public class SIPurchaseTerminal : MonoBehaviour, ITouchScreenStation
{
	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x060008B2 RID: 2226 RVA: 0x0002F964 File Offset: 0x0002DB64
	public SIScreenRegion ScreenRegion
	{
		get
		{
			return this.screenRegion;
		}
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x0002F96C File Offset: 0x0002DB6C
	private void OnEnable()
	{
		if (CosmeticsController.hasInstance)
		{
			this.DelayedOnEnable();
			return;
		}
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Combine(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.DelayedOnEnable));
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x0002F99C File Offset: 0x0002DB9C
	private void DelayedOnEnable()
	{
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.DelayedOnEnable));
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnGetCurrency = (Action)Delegate.Combine(instance.OnGetCurrency, new Action(this.OnUpdateCurrencyBalance));
		this.OnUpdateCurrencyBalance();
		this.PopupBackgroundScreen.SetActive(false);
		this.ConfirmPurchasePopupScreen.SetActive(false);
		this.PendingPurchasePopupScreen.SetActive(false);
		this.PurchaseCompletePopupScreen.SetActive(false);
		this.InsufficientFundsPopupScreen.SetActive(false);
		this.UnableToCompletePurchasePopupScreen.SetActive(false);
		this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.PurchaseAmountSelection, true);
		this.purchaseSize = 1;
		this.UpdatePurchaseAmount();
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x0002FA54 File Offset: 0x0002DC54
	private void OnDisable()
	{
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnGetCurrency = (Action)Delegate.Remove(instance.OnGetCurrency, new Action(this.OnUpdateCurrencyBalance));
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x0002FA7E File Offset: 0x0002DC7E
	public void UpdateCurrentTechPoints()
	{
		this.PurchaseAmountCurrentTechPointsCount.text = SIPlayer.LocalPlayer.CurrentProgression.resourceArray[0].ToString();
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x0002FAA5 File Offset: 0x0002DCA5
	private void OnUpdateCurrencyBalance()
	{
		this.PurchaseAmountCurrentShinyRockCount.text = CosmeticsController.instance.currencyBalance.ToString().ToUpperInvariant();
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void AddButton(SITouchscreenButton button, bool isPopupButton = false)
	{
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x0002FAC8 File Offset: 0x0002DCC8
	public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
	{
		switch (this.currentState)
		{
		case SIPurchaseTerminal.PurchaseTerminalState.PurchaseAmountSelection:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Purchase)
			{
				this.SelectPurchase();
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Next)
			{
				this.IncreasePurchase();
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.DecreasePurcahse();
				return;
			}
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.ConfirmPurchasePopup:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Confirm)
			{
				this.ConfirmPurchase();
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Cancel)
			{
				this.ReturnToBaseScreen();
				return;
			}
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.PendingPurchasePopup:
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.PurchaseCompletePopup:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Confirm)
			{
				this.ReturnToBaseScreen();
				return;
			}
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.InsufficientFundsPopup:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.ReturnToBaseScreen();
				return;
			}
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.UnableToCompletePurchasePopup:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.ReturnToBaseScreen();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void TouchscreenToggleButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, bool isToggledOn)
	{
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x0002FB53 File Offset: 0x0002DD53
	private void IncreasePurchase()
	{
		this.purchaseSize = Math.Min(this.purchaseSize + 1, this.maxPurchaseSize);
		this.UpdatePurchaseAmount();
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x0002FB74 File Offset: 0x0002DD74
	private void DecreasePurcahse()
	{
		this.purchaseSize = Math.Max(this.purchaseSize - 1, this.minPurchaseSize);
		this.UpdatePurchaseAmount();
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x0002FB98 File Offset: 0x0002DD98
	private void UpdatePurchaseAmount()
	{
		this.PurchaseAmountShinyRockCount.text = (this.purchaseSize * this.costPerTechPoint).ToString().ToUpperInvariant();
		this.PurchaseAmountTechPointCount.text = this.purchaseSize.ToString().ToUpperInvariant();
		this.ConfirmPurchaseShinyRockCount.text = (this.purchaseSize * this.costPerTechPoint).ToString().ToUpperInvariant();
		this.ConfirmPurchaseTechPointCount.text = this.purchaseSize.ToString().ToUpperInvariant();
		this.PurchasedTechPointCount.text = this.purchaseSize.ToString().ToUpperInvariant();
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x0002FC40 File Offset: 0x0002DE40
	private void SelectPurchase()
	{
		this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.ConfirmPurchasePopup, false);
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x0002FC4C File Offset: 0x0002DE4C
	private void ConfirmPurchase()
	{
		int num = this.purchaseSize * this.costPerTechPoint;
		if (CosmeticsController.instance.currencyBalance < num)
		{
			this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.InsufficientFundsPopup, false);
			return;
		}
		this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.PendingPurchasePopup, false);
		ProgressionManager.Instance.PurchaseTechPoints(this.purchaseSize, delegate
		{
			SIProgression.Instance.SendPurchaseTechPointsData(this.purchaseSize);
			this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.PurchaseCompletePopup, false);
			ProgressionManager.Instance.RefreshUserInventory();
		}, delegate(string error)
		{
			Debug.LogError("[SIPurchaseTerminal] PurchaseTechPoints failed: " + error);
			this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.UnableToCompletePurchasePopup, false);
		});
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x0002FCAF File Offset: 0x0002DEAF
	private void ReturnToBaseScreen()
	{
		this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.PurchaseAmountSelection, false);
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x0002FCB9 File Offset: 0x0002DEB9
	private void UpdateState(SIPurchaseTerminal.PurchaseTerminalState newState, bool forceUpdate = false)
	{
		if (!forceUpdate && this.currentState == newState)
		{
			return;
		}
		this.SetScreenVisibility(this.currentState, false);
		this.currentState = newState;
		this.SetScreenVisibility(this.currentState, true);
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x0002FCEC File Offset: 0x0002DEEC
	private void SetScreenVisibility(SIPurchaseTerminal.PurchaseTerminalState screenState, bool isEnabled)
	{
		switch (screenState)
		{
		case SIPurchaseTerminal.PurchaseTerminalState.ConfirmPurchasePopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.ConfirmPurchasePopupScreen.SetActive(isEnabled);
			return;
		case SIPurchaseTerminal.PurchaseTerminalState.PendingPurchasePopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.PendingPurchasePopupScreen.SetActive(isEnabled);
			return;
		case SIPurchaseTerminal.PurchaseTerminalState.PurchaseCompletePopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.PurchaseCompletePopupScreen.SetActive(isEnabled);
			return;
		case SIPurchaseTerminal.PurchaseTerminalState.InsufficientFundsPopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.InsufficientFundsPopupScreen.SetActive(isEnabled);
			return;
		case SIPurchaseTerminal.PurchaseTerminalState.UnableToCompletePurchasePopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.UnableToCompletePurchasePopupScreen.SetActive(isEnabled);
			return;
		default:
			return;
		}
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x0000636B File Offset: 0x0000456B
	GameObject ITouchScreenStation.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04000AC8 RID: 2760
	private SIPurchaseTerminal.PurchaseTerminalState currentState;

	// Token: 0x04000AC9 RID: 2761
	[SerializeField]
	private SIScreenRegion screenRegion;

	// Token: 0x04000ACA RID: 2762
	[SerializeField]
	private GameObject PopupBackgroundScreen;

	// Token: 0x04000ACB RID: 2763
	[SerializeField]
	private GameObject ConfirmPurchasePopupScreen;

	// Token: 0x04000ACC RID: 2764
	[SerializeField]
	private GameObject PurchaseCompletePopupScreen;

	// Token: 0x04000ACD RID: 2765
	[SerializeField]
	private GameObject PendingPurchasePopupScreen;

	// Token: 0x04000ACE RID: 2766
	[SerializeField]
	private GameObject InsufficientFundsPopupScreen;

	// Token: 0x04000ACF RID: 2767
	[SerializeField]
	private GameObject UnableToCompletePurchasePopupScreen;

	// Token: 0x04000AD0 RID: 2768
	[SerializeField]
	private TextMeshProUGUI PurchaseAmountShinyRockCount;

	// Token: 0x04000AD1 RID: 2769
	[SerializeField]
	private TextMeshProUGUI PurchaseAmountTechPointCount;

	// Token: 0x04000AD2 RID: 2770
	[SerializeField]
	private TextMeshProUGUI PurchaseAmountCurrentShinyRockCount;

	// Token: 0x04000AD3 RID: 2771
	[SerializeField]
	private TextMeshProUGUI PurchaseAmountCurrentTechPointsCount;

	// Token: 0x04000AD4 RID: 2772
	[SerializeField]
	private TextMeshProUGUI ConfirmPurchaseShinyRockCount;

	// Token: 0x04000AD5 RID: 2773
	[SerializeField]
	private TextMeshProUGUI ConfirmPurchaseTechPointCount;

	// Token: 0x04000AD6 RID: 2774
	[SerializeField]
	private TextMeshProUGUI PurchasedTechPointCount;

	// Token: 0x04000AD7 RID: 2775
	[SerializeField]
	private int maxPurchaseSize = 10;

	// Token: 0x04000AD8 RID: 2776
	[SerializeField]
	private int minPurchaseSize = 1;

	// Token: 0x04000AD9 RID: 2777
	[SerializeField]
	private int costPerTechPoint = 100;

	// Token: 0x04000ADA RID: 2778
	private int purchaseSize = 1;

	// Token: 0x0200014D RID: 333
	public enum PurchaseTerminalState
	{
		// Token: 0x04000ADC RID: 2780
		PurchaseAmountSelection,
		// Token: 0x04000ADD RID: 2781
		ConfirmPurchasePopup,
		// Token: 0x04000ADE RID: 2782
		PendingPurchasePopup,
		// Token: 0x04000ADF RID: 2783
		PurchaseCompletePopup,
		// Token: 0x04000AE0 RID: 2784
		InsufficientFundsPopup,
		// Token: 0x04000AE1 RID: 2785
		UnableToCompletePurchasePopup
	}
}
