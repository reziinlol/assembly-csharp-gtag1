using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using GorillaNetworking.Store;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000563 RID: 1379
public class ATM_Manager : MonoBehaviour, IBuildValidation
{
	// Token: 0x170003B7 RID: 951
	// (get) Token: 0x06002304 RID: 8964 RVA: 0x000BB969 File Offset: 0x000B9B69
	public ATM_Manager.ATMStages CurrentATMStage
	{
		get
		{
			return this.currentATMStage;
		}
	}

	// Token: 0x06002305 RID: 8965 RVA: 0x000BB974 File Offset: 0x000B9B74
	public void Awake()
	{
		if (ATM_Manager.instance)
		{
			Object.Destroy(this);
		}
		else
		{
			ATM_Manager.instance = this;
		}
		string defaultResult = "CREATOR CODE: ";
		string creatorCodeTitle;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("ATM_CREATOR_CODE", out creatorCodeTitle, defaultResult))
		{
			Debug.LogError("[LOCALIZATION::ATM_MANAGER] Failed to get key for [ATM_CREATOR_CODE]");
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.SetCreatorCodeTitle(creatorCodeTitle);
		}
		this.SwitchToStage(ATM_Manager.ATMStages.Unavailable);
		this.smallDisplays = new List<CreatorCodeSmallDisplay>();
		this.ATM_TERMINAL_ID = string.Empty;
		for (int i = 0; i < this.nexusGroups.Length; i++)
		{
			string atm_TERMINAL_ID = this.ATM_TERMINAL_ID;
			NexusGroupId nexusGroupId = this.nexusGroups[i];
			this.ATM_TERMINAL_ID = atm_TERMINAL_ID + ((nexusGroupId != null) ? nexusGroupId.ToString() : null);
		}
		this.HookupToCreatorCodes();
	}

	// Token: 0x06002306 RID: 8966 RVA: 0x000BBA60 File Offset: 0x000B9C60
	public void Start()
	{
		Debug.Log("ATM COUNT: " + this.atmUIs.Count.ToString());
		Debug.Log("SMALL DISPLAY COUNT: " + this.smallDisplays.Count.ToString());
		GameEvents.OnGorrillaATMKeyButtonPressedEvent.AddListener(new UnityAction<GorillaATMKeyBindings>(this.PressButton));
	}

	// Token: 0x06002307 RID: 8967 RVA: 0x000BBAC8 File Offset: 0x000B9CC8
	public void HookupToCreatorCodes()
	{
		CreatorCodes.InitializedEvent += this.CreatorCodesInitialized;
		CreatorCodes.OnCreatorCodeChangedEvent += this.OnCreatorCodeChanged;
		CreatorCodes.OnCreatorCodeFailureEvent += this.OnOnCreatorCodeFailureEvent;
		if (CreatorCodes.Intialized)
		{
			this.CreatorCodesInitialized();
		}
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x000BBB18 File Offset: 0x000B9D18
	public void CreatorCodesInitialized()
	{
		foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
		{
			creatorCodeSmallDisplay.SetCode(CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID));
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.SetCreatorCodeField(CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID));
		}
	}

	// Token: 0x06002309 RID: 8969 RVA: 0x000BBBC0 File Offset: 0x000B9DC0
	public void OnCreatorCodeChanged(string id)
	{
		if (id != this.ATM_TERMINAL_ID)
		{
			return;
		}
		foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
		{
			creatorCodeSmallDisplay.SetCode(CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID));
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.SetCreatorCodeField(CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID));
		}
		string text = "CREATOR CODE:";
		CreatorCodes.CreatorCodeStatus currentCreatorCodeStatus = CreatorCodes.getCurrentCreatorCodeStatus(this.ATM_TERMINAL_ID);
		if (currentCreatorCodeStatus != CreatorCodes.CreatorCodeStatus.Validating)
		{
			if (currentCreatorCodeStatus == CreatorCodes.CreatorCodeStatus.Valid)
			{
				text += " VALID";
			}
		}
		else
		{
			text += " VALIDATING";
		}
		foreach (ATM_UI atm_UI2 in this.atmUIs)
		{
			atm_UI2.SetCreatorCodeTitle(text);
		}
	}

	// Token: 0x0600230A RID: 8970 RVA: 0x000BBCE8 File Offset: 0x000B9EE8
	private void OnOnCreatorCodeFailureEvent(string id)
	{
		if (id != this.ATM_TERMINAL_ID)
		{
			return;
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.SetCreatorCodeTitle("CREATOR CODE: INVALID");
			string creatorCodeTitle;
			LocalisationManager.TryGetKeyForCurrentLocale("ATM_CREATOR_CODE_INVALID", out creatorCodeTitle, atm_UI.atmText.text);
			atm_UI.SetCreatorCodeTitle(creatorCodeTitle);
		}
		Debug.Log("ATM CODE FAILURE");
	}

	// Token: 0x0600230B RID: 8971 RVA: 0x000BBD78 File Offset: 0x000B9F78
	public void OnCreatorCodeInvalid(string id)
	{
		if (id != this.ATM_TERMINAL_ID)
		{
			return;
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.SetCreatorCodeTitle("CREATOR CODE: INVALID");
		}
	}

	// Token: 0x0600230C RID: 8972 RVA: 0x000BBDDC File Offset: 0x000B9FDC
	private void OnEnable()
	{
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		this.SwitchToStage(this.currentATMStage);
	}

	// Token: 0x0600230D RID: 8973 RVA: 0x000BBDFB File Offset: 0x000B9FFB
	private void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x0600230E RID: 8974 RVA: 0x000BBE0E File Offset: 0x000BA00E
	private void OnLanguageChanged()
	{
		this.SwitchToStage(this.currentATMStage);
	}

	// Token: 0x0600230F RID: 8975 RVA: 0x000BBE1C File Offset: 0x000BA01C
	public void PressButton(GorillaATMKeyBindings buttonPressed)
	{
		if (this.currentATMStage == ATM_Manager.ATMStages.Confirm && CreatorCodes.getCurrentCreatorCodeStatus(this.ATM_TERMINAL_ID) != CreatorCodes.CreatorCodeStatus.Validating)
		{
			string defaultResult = "CREATOR CODE: ";
			string creatorCodeTitle;
			LocalisationManager.TryGetKeyForCurrentLocale("ATM_CREATOR_CODE", out creatorCodeTitle, defaultResult);
			foreach (ATM_UI atm_UI in this.atmUIs)
			{
				atm_UI.SetCreatorCodeTitle(creatorCodeTitle);
			}
			if (buttonPressed == GorillaATMKeyBindings.delete)
			{
				CreatorCodes.DeleteCharacter(this.ATM_TERMINAL_ID);
				return;
			}
			string atm_TERMINAL_ID = this.ATM_TERMINAL_ID;
			string input;
			if (buttonPressed >= GorillaATMKeyBindings.delete)
			{
				input = buttonPressed.ToString();
			}
			else
			{
				int num = (int)buttonPressed;
				input = num.ToString();
			}
			CreatorCodes.AppendKey(atm_TERMINAL_ID, input);
		}
	}

	// Token: 0x06002310 RID: 8976 RVA: 0x000BBED8 File Offset: 0x000BA0D8
	public void ProcessATMState(ATM_UI atm_ui, string currencyButton)
	{
		ATM_Manager.<ProcessATMState>d__56 <ProcessATMState>d__;
		<ProcessATMState>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<ProcessATMState>d__.<>4__this = this;
		<ProcessATMState>d__.atm_ui = atm_ui;
		<ProcessATMState>d__.currencyButton = currencyButton;
		<ProcessATMState>d__.<>1__state = -1;
		<ProcessATMState>d__.<>t__builder.Start<ATM_Manager.<ProcessATMState>d__56>(ref <ProcessATMState>d__);
	}

	// Token: 0x06002311 RID: 8977 RVA: 0x000BBF1F File Offset: 0x000BA11F
	public void AddATM(ATM_UI newATM, Tuple<string, string> creatorCode)
	{
		this.atmUIs.Add(newATM);
		if (creatorCode != null)
		{
			this.atmUIToMemberCode.Add(newATM, creatorCode);
		}
		else
		{
			newATM.SetCreatorCodeField(CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID));
		}
		this.SwitchToStage(this.currentATMStage);
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x000BBF5C File Offset: 0x000BA15C
	public void RemoveATM(ATM_UI atmToRemove)
	{
		this.atmUIs.Remove(atmToRemove);
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x000BBF6C File Offset: 0x000BA16C
	public void CreatorCodeValidating()
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.SetCreatorCodeTitle("CREATOR CODE: VALIDATING");
		}
	}

	// Token: 0x06002314 RID: 8980 RVA: 0x000BBFC4 File Offset: 0x000BA1C4
	public void CreatorCodeValid()
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.SetCreatorCodeTitle("CREATOR CODE: VALIDATING");
		}
		if (this.currentATMStage == ATM_Manager.ATMStages.Confirm)
		{
			this.SwitchToStage(ATM_Manager.ATMStages.Purchasing);
		}
	}

	// Token: 0x06002315 RID: 8981 RVA: 0x000BC02C File Offset: 0x000BA22C
	public void SwitchToStage(ATM_Manager.ATMStages newStage)
	{
		this.currentATMStage = newStage;
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			if (atm_UI.atmText)
			{
				string text = "";
				string text2 = "";
				string text3 = "";
				string text4 = "";
				string text5 = "";
				switch (newStage)
				{
				case ATM_Manager.ATMStages.Unavailable:
					atm_UI.atmText.text = "ATM NOT AVAILABLE! PLEASE TRY AGAIN LATER!";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_NOT_AVAILABLE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.HideCreatorCode();
					break;
				case ATM_Manager.ATMStages.Begin:
					atm_UI.atmText.text = "WELCOME! PRESS ANY BUTTON TO BEGIN.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_STARTUP", out text, atm_UI.atmText.text);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_BEGIN", out text5, "BEGIN");
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = text5;
					atm_UI.ATM_RightColumnArrowText[3].enabled = true;
					atm_UI.HideCreatorCode();
					break;
				case ATM_Manager.ATMStages.Menu:
					if (PlayFabAuthenticator.instance.GetSafety())
					{
						atm_UI.atmText.text = "CHECK YOUR BALANCE.";
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_CHECK_YOUR_BALANCE", out text, atm_UI.atmText.text);
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_BALANCE", out text2, atm_UI.atmText.text);
						atm_UI.atmText.text = text;
						atm_UI.ATM_RightColumnButtonText[0].text = text2;
						atm_UI.ATM_RightColumnArrowText[0].enabled = true;
						atm_UI.ATM_RightColumnButtonText[1].text = "";
						atm_UI.ATM_RightColumnArrowText[1].enabled = false;
						atm_UI.ATM_RightColumnButtonText[2].text = "";
						atm_UI.ATM_RightColumnArrowText[2].enabled = false;
						atm_UI.ATM_RightColumnButtonText[3].text = "";
						atm_UI.ATM_RightColumnArrowText[3].enabled = false;
						atm_UI.HideCreatorCode();
					}
					else
					{
						atm_UI.atmText.text = "CHECK YOUR BALANCE OR PURCHASE MORE SHINY ROCKS.";
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_MAIN_SCREEN", out text, atm_UI.atmText.text);
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_BALANCE", out text2, atm_UI.atmText.text);
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE", out text3, atm_UI.atmText.text);
						atm_UI.atmText.text = text;
						atm_UI.ATM_RightColumnButtonText[0].text = text2;
						atm_UI.ATM_RightColumnArrowText[0].enabled = true;
						atm_UI.ATM_RightColumnButtonText[1].text = text3;
						atm_UI.ATM_RightColumnArrowText[1].enabled = true;
						atm_UI.ATM_RightColumnButtonText[2].text = "";
						atm_UI.ATM_RightColumnArrowText[2].enabled = false;
						atm_UI.ATM_RightColumnButtonText[3].text = "";
						atm_UI.ATM_RightColumnArrowText[3].enabled = false;
						atm_UI.HideCreatorCode();
					}
					break;
				case ATM_Manager.ATMStages.Balance:
					atm_UI.atmText.text = "CURRENT BALANCE:\n\n" + CosmeticsController.instance.CurrencyBalance.ToString();
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_CURRENT_BALANCE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text + "\n\n" + CosmeticsController.instance.CurrencyBalance.ToString();
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.HideCreatorCode();
					break;
				case ATM_Manager.ATMStages.Choose:
				{
					string defaultResult = "{numShinyRocksToBuy} - {currencySymbol}{shinyRocksCost}";
					string defaultResult2 = "{numShinyRocksToBuy} - {currencySymbol}{shinyRocksCost}\r\n({discount}% BONUS!";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_FIRST", out text2, defaultResult);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_SECOND", out text3, defaultResult2);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_SECOND", out text4, defaultResult2);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_SECOND", out text5, defaultResult2);
					text2 = text2.Replace("{numShinyRocksToBuy}", "1000").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "4.99");
					text3 = text3.Replace("{numShinyRocksToBuy}", "2200").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "9.99").Replace("{discount}", "10");
					text4 = text4.Replace("{numShinyRocksToBuy}", "5000").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "19.99").Replace("{discount}", "25");
					text5 = text5.Replace("{numShinyRocksToBuy}", "11000").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "39.99").Replace("{discount}", "37");
					atm_UI.atmText.text = "CHOOSE AN AMOUNT OF SHINY ROCKS TO PURCHASE.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_CHOOSE_PURCHASE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = text2;
					atm_UI.ATM_RightColumnArrowText[0].enabled = true;
					atm_UI.ATM_RightColumnButtonText[1].text = text3;
					atm_UI.ATM_RightColumnArrowText[1].enabled = true;
					atm_UI.ATM_RightColumnButtonText[2].text = text4;
					atm_UI.ATM_RightColumnArrowText[2].enabled = true;
					atm_UI.ATM_RightColumnButtonText[3].text = text5;
					atm_UI.ATM_RightColumnArrowText[3].enabled = true;
					atm_UI.HideCreatorCode();
					break;
				}
				case ATM_Manager.ATMStages.Confirm:
					atm_UI.atmText.text = string.Concat(new string[]
					{
						"YOU HAVE CHOSEN TO PURCHASE ",
						this.numShinyRocksToBuy.ToString(),
						" SHINY ROCKS FOR $",
						this.shinyRocksCost.ToString(),
						". CONFIRM TO LAUNCH A STEAM WINDOW TO COMPLETE YOUR PURCHASE."
					});
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_CONFIRMATION_STEAM", out text, atm_UI.atmText.text);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_CONFIRM", out text2, "CONFIRM");
					text = text.Replace("{numShinyRocksToBuy}", this.numShinyRocksToBuy.ToString());
					text = text.Replace("{currencySymbol}", "$");
					text = text.Replace("{shinyRocksCost}", this.shinyRocksCost.ToString());
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = text2;
					atm_UI.ATM_RightColumnArrowText[0].enabled = true;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.ShowCreatorCode();
					break;
				case ATM_Manager.ATMStages.Purchasing:
					atm_UI.atmText.text = "PURCHASING IN STEAM...";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASING", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.HideCreatorCode();
					break;
				case ATM_Manager.ATMStages.Success:
					atm_UI.atmText.text = "SUCCESS! NEW SHINY ROCKS BALANCE: " + (CosmeticsController.instance.CurrencyBalance + this.numShinyRocksToBuy).ToString();
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_SUCCESS_NEW_BALANCE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text + (CosmeticsController.instance.CurrencyBalance + this.numShinyRocksToBuy).ToString();
					if (CreatorCodes.getCurrentCreatorCodeStatus(this.ATM_TERMINAL_ID) == CreatorCodes.CreatorCodeStatus.Valid)
					{
						string name = CreatorCodes.supportedMember.name;
						if (!string.IsNullOrEmpty(name))
						{
							TMP_Text atmText = atm_UI.atmText;
							atmText.text = atmText.text + "\n\nTHIS PURCHASE SUPPORTED\n" + name + "!";
							foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
							{
								creatorCodeSmallDisplay.SuccessfulPurchase(name);
							}
						}
					}
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.HideCreatorCode();
					break;
				case ATM_Manager.ATMStages.Failure:
					atm_UI.atmText.text = "PURCHASE CANCELLED. NO FUNDS WERE SPENT.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_CANCELLED", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.HideCreatorCode();
					break;
				case ATM_Manager.ATMStages.SafeAccount:
					atm_UI.atmText.text = "Out Of Order.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASING_DISABLED_OUT_OF_ORDER", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.HideCreatorCode();
					break;
				}
			}
		}
	}

	// Token: 0x06002316 RID: 8982 RVA: 0x000BCBC4 File Offset: 0x000BADC4
	public void SetATMText(string newText)
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.atmText.text = newText;
		}
	}

	// Token: 0x06002317 RID: 8983 RVA: 0x000BCC1C File Offset: 0x000BAE1C
	public void PressCurrencyPurchaseButton(ATM_UI atm_ui, string currencyPurchaseSize)
	{
		this.ProcessATMState(atm_ui, currencyPurchaseSize);
	}

	// Token: 0x06002318 RID: 8984 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void LeaveSystemMenu()
	{
	}

	// Token: 0x06002319 RID: 8985 RVA: 0x000BCC26 File Offset: 0x000BAE26
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.nexusGroups.Length == 0)
		{
			Debug.LogError("You have to set at least one nexusGroup in " + base.name + " or things will not work!");
			return false;
		}
		return true;
	}

	// Token: 0x0600231A RID: 8986 RVA: 0x000BCC50 File Offset: 0x000BAE50
	internal void SetTemporaryCreatorCode(string code)
	{
		if (code == null)
		{
			CreatorCodes.ResetCreatorCode(this.ATM_TERMINAL_ID);
			CreatorCodes.AppendKey(this.ATM_TERMINAL_ID, this._tempCreatorCodeOveride);
			this._tempCreatorCodeOveride = null;
			return;
		}
		if (this._tempCreatorCodeOveride == null)
		{
			this._tempCreatorCodeOveride = CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID);
		}
		CreatorCodes.ResetCreatorCode(this.ATM_TERMINAL_ID);
		CreatorCodes.AppendKey(this.ATM_TERMINAL_ID, code);
	}

	// Token: 0x04002E04 RID: 11780
	private const string ATM_STARTUP_KEY = "ATM_STARTUP";

	// Token: 0x04002E05 RID: 11781
	private const string ATM_SCREEN_KEY = "ATM_SCREEN";

	// Token: 0x04002E06 RID: 11782
	private const string ATM_NOT_AVAILABLE_KEY = "ATM_NOT_AVAILABLE";

	// Token: 0x04002E07 RID: 11783
	private const string ATM_BEGIN_KEY = "ATM_BEGIN";

	// Token: 0x04002E08 RID: 11784
	private const string ATM_MAIN_SCREEN_KEY = "ATM_MAIN_SCREEN";

	// Token: 0x04002E09 RID: 11785
	private const string ATM_CHECK_YOUR_BALANCE_KEY = "ATM_CHECK_YOUR_BALANCE";

	// Token: 0x04002E0A RID: 11786
	private const string ATM_PURCHASING_DISABLED_OUT_OF_ORDER_KEY = "ATM_PURCHASING_DISABLED_OUT_OF_ORDER";

	// Token: 0x04002E0B RID: 11787
	private const string ATM_CURRENT_BALANCE_KEY = "ATM_CURRENT_BALANCE";

	// Token: 0x04002E0C RID: 11788
	private const string ATM_MODDED_CLIENT_KEY = "ATM_MODDED_CLIENT";

	// Token: 0x04002E0D RID: 11789
	private const string ATM_CHOOSE_PURCHASE_KEY = "ATM_CHOOSE_PURCHASE";

	// Token: 0x04002E0E RID: 11790
	private const string ATM_PURCHASE_CONFIRMATION_KEY = "ATM_PURCHASE_CONFIRMATION";

	// Token: 0x04002E0F RID: 11791
	private const string ATM_PURCHASE_CONFIRMATION_STEAM_KEY = "ATM_PURCHASE_CONFIRMATION_STEAM";

	// Token: 0x04002E10 RID: 11792
	private const string ATM_PURCHASING_KEY = "ATM_PURCHASING";

	// Token: 0x04002E11 RID: 11793
	private const string ATM_SUCCESS_NEW_BALANCE_KEY = "ATM_SUCCESS_NEW_BALANCE";

	// Token: 0x04002E12 RID: 11794
	private const string ATM_PURCHASE_CANCELLED_KEY = "ATM_PURCHASE_CANCELLED";

	// Token: 0x04002E13 RID: 11795
	private const string ATM_LOCKED_KEY = "ATM_LOCKED";

	// Token: 0x04002E14 RID: 11796
	private const string ATM_RETURN_KEY = "ATM_RETURN";

	// Token: 0x04002E15 RID: 11797
	private const string ATM_BACK_KEY = "ATM_BACK";

	// Token: 0x04002E16 RID: 11798
	private const string ATM_CONFIRM_KEY = "ATM_CONFIRM";

	// Token: 0x04002E17 RID: 11799
	private const string ATM_IAP_NOT_AVAILABLE_KEY = "ATM_IAP_NOT_AVAILABLE";

	// Token: 0x04002E18 RID: 11800
	private const string ATM_BALANCE_KEY = "ATM_BALANCE";

	// Token: 0x04002E19 RID: 11801
	private const string ATM_PURCHASE_KEY = "ATM_PURCHASE";

	// Token: 0x04002E1A RID: 11802
	private const string ATM_CREATOR_CODE_KEY = "ATM_CREATOR_CODE";

	// Token: 0x04002E1B RID: 11803
	private const string ATM_CREATOR_CODE_VALIDATING_KEY = "ATM_CREATOR_CODE_VALIDATING";

	// Token: 0x04002E1C RID: 11804
	private const string ATM_CREATOR_CODE_VALID_KEY = "ATM_CREATOR_CODE_VALID";

	// Token: 0x04002E1D RID: 11805
	private const string ATM_CREATOR_CODE_INVALID_KEY = "ATM_CREATOR_CODE_INVALID";

	// Token: 0x04002E1E RID: 11806
	private const string ATM_PURCHASE_OPTION_FIRST_KEY = "ATM_PURCHASE_OPTION_FIRST";

	// Token: 0x04002E1F RID: 11807
	private const string ATM_PURCHASE_OPTION_SECOND_KEY = "ATM_PURCHASE_OPTION_SECOND";

	// Token: 0x04002E20 RID: 11808
	private const string ATM_PURCHASE_OPTION_THIRD_KEY = "ATM_PURCHASE_OPTION_THIRD";

	// Token: 0x04002E21 RID: 11809
	private const string ATM_PURCHASE_OPTION_FOURTH_KEY = "ATM_PURCHASE_OPTION_FOURTH";

	// Token: 0x04002E22 RID: 11810
	[OnEnterPlay_SetNull]
	public static volatile ATM_Manager instance;

	// Token: 0x04002E23 RID: 11811
	private const int MAX_CODE_LENGTH = 10;

	// Token: 0x04002E24 RID: 11812
	public List<ATM_UI> atmUIs = new List<ATM_UI>();

	// Token: 0x04002E25 RID: 11813
	public Dictionary<ATM_UI, Tuple<string, string>> atmUIToMemberCode = new Dictionary<ATM_UI, Tuple<string, string>>();

	// Token: 0x04002E26 RID: 11814
	[HideInInspector]
	public List<CreatorCodeSmallDisplay> smallDisplays;

	// Token: 0x04002E27 RID: 11815
	private ATM_Manager.ATMStages currentATMStage;

	// Token: 0x04002E28 RID: 11816
	public int numShinyRocksToBuy;

	// Token: 0x04002E29 RID: 11817
	public float shinyRocksCost;

	// Token: 0x04002E2A RID: 11818
	public bool alreadyBegan;

	// Token: 0x04002E2B RID: 11819
	[SerializeField]
	private NexusGroupId[] nexusGroups;

	// Token: 0x04002E2C RID: 11820
	private string _tempCreatorCodeOveride;

	// Token: 0x04002E2D RID: 11821
	private string ATM_TERMINAL_ID = "atm_terminal_id";

	// Token: 0x02000564 RID: 1380
	public enum ATMStages
	{
		// Token: 0x04002E2F RID: 11823
		Unavailable,
		// Token: 0x04002E30 RID: 11824
		Begin,
		// Token: 0x04002E31 RID: 11825
		Menu,
		// Token: 0x04002E32 RID: 11826
		Balance,
		// Token: 0x04002E33 RID: 11827
		Choose,
		// Token: 0x04002E34 RID: 11828
		Confirm,
		// Token: 0x04002E35 RID: 11829
		Purchasing,
		// Token: 0x04002E36 RID: 11830
		Success,
		// Token: 0x04002E37 RID: 11831
		Failure,
		// Token: 0x04002E38 RID: 11832
		SafeAccount
	}
}
