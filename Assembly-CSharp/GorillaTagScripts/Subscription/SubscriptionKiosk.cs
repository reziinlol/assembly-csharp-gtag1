using System;
using System.Collections.Generic;
using GorillaNetworking;
using Oculus.Platform;
using Oculus.Platform.Models;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace GorillaTagScripts.Subscription
{
	// Token: 0x02000F6A RID: 3946
	public class SubscriptionKiosk : MonoBehaviour, ITouchScreenStation, IGorillaSliceableSimple
	{
		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x06006254 RID: 25172 RVA: 0x001FBBD0 File Offset: 0x001F9DD0
		// (set) Token: 0x06006255 RID: 25173 RVA: 0x001FBBD7 File Offset: 0x001F9DD7
		public static bool ProcessingSubscriptionPurchase { get; set; }

		// Token: 0x17000944 RID: 2372
		// (get) Token: 0x06006256 RID: 25174 RVA: 0x001FBBDF File Offset: 0x001F9DDF
		public SIScreenRegion ScreenRegion { get; }

		// Token: 0x06006257 RID: 25175 RVA: 0x001FBBE8 File Offset: 0x001F9DE8
		private void Awake()
		{
			this.toggleButtonContainers = new List<SITouchscreenButtonContainer>(base.GetComponentsInChildren<SITouchscreenButtonContainer>(true));
			for (int i = this.toggleButtonContainers.Count - 1; i >= 0; i--)
			{
				if (this.toggleButtonContainers[i].button.buttonMode != SITouchscreenButton.ButtonMode.Toggle)
				{
					this.toggleButtonContainers.RemoveAt(i);
				}
			}
			this.screensByState = new Dictionary<SubscriptionKiosk.ScreenState, GameObject>();
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SafeAccount, this.safeAccountScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.WaitingForScan, this.waitingForScanScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.Scanning, this.scanningScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionStatusUnknown, this.subStatusUnknownScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.MainMenuSubscribed, this.mainMenuSubscribedScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.MainMenuUnsubscribed, this.mainMenuUnsubscribedScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionData, this.subDataScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.PurchaseSubscription, this.purchaseSubScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionPurchaseInProgress, this.purchaseProgressScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult, this.purchaseResultScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.FeatureToggles, this.featureTogglesScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionSteamWarning, this.steamComingSoon);
		}

		// Token: 0x06006258 RID: 25176 RVA: 0x001FBD2C File Offset: 0x001F9F2C
		private void OnEnable()
		{
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.SafeAccount);
				Object.Destroy(this);
				return;
			}
			this.UpdateState(SubscriptionKiosk.ScreenState.WaitingForScan);
			this.subsVideoPlayer.clip = this.defaultVideoClip;
			GorillaSlicerSimpleManager.RegisterSliceable(this);
			SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.LocalSubscriptionDataUpdated));
		}

		// Token: 0x06006259 RID: 25177 RVA: 0x001FBD93 File Offset: 0x001F9F93
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
			SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.LocalSubscriptionDataUpdated));
			Callback<MicroTxnAuthorizationResponse_t> steamMicroTransactionAuthorizationResponse = this._steamMicroTransactionAuthorizationResponse;
			if (steamMicroTransactionAuthorizationResponse == null)
			{
				return;
			}
			steamMicroTransactionAuthorizationResponse.Unregister();
		}

		// Token: 0x0600625A RID: 25178 RVA: 0x001FBDCC File Offset: 0x001F9FCC
		public void HandScanAborted()
		{
			if (this.currentState == SubscriptionKiosk.ScreenState.Scanning)
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.WaitingForScan);
			}
		}

		// Token: 0x0600625B RID: 25179 RVA: 0x001FBDDE File Offset: 0x001F9FDE
		public void KioskAbandoned()
		{
			this.UpdateState(SubscriptionKiosk.ScreenState.WaitingForScan);
		}

		// Token: 0x0600625C RID: 25180 RVA: 0x001FBDE7 File Offset: 0x001F9FE7
		public void HandScanStarted()
		{
			if (this.currentState == SubscriptionKiosk.ScreenState.WaitingForScan)
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.Scanning);
			}
		}

		// Token: 0x0600625D RID: 25181 RVA: 0x001FBDFC File Offset: 0x001F9FFC
		public void HandScanned()
		{
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				return;
			}
			SubscriptionManager.SubscriptionStatus subscriptionStatus = SubscriptionManager.LocalSubscriptionStatus();
			if (subscriptionStatus == SubscriptionManager.SubscriptionStatus.Active)
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.MainMenuSubscribed);
				return;
			}
			if (subscriptionStatus == SubscriptionManager.SubscriptionStatus.Inactive)
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.MainMenuUnsubscribed);
				return;
			}
			this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionStatusUnknown);
		}

		// Token: 0x0600625E RID: 25182 RVA: 0x001FBE3C File Offset: 0x001FA03C
		private void UpdateState(SubscriptionKiosk.ScreenState newState)
		{
			this.lastState = this.currentState;
			this.currentState = newState;
			if (this.lastState == this.currentState)
			{
				return;
			}
			this.ActivateScreen(this.currentState);
			switch (this.currentState)
			{
			case SubscriptionKiosk.ScreenState.WaitingForScan:
			case SubscriptionKiosk.ScreenState.Scanning:
			case SubscriptionKiosk.ScreenState.SubscriptionStatusUnknown:
			case SubscriptionKiosk.ScreenState.PurchaseSubscription:
			case SubscriptionKiosk.ScreenState.SubscriptionPurchaseInProgress:
			case SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult:
				break;
			case SubscriptionKiosk.ScreenState.MainMenuSubscribed:
				this.UpdateSubscribedMenu();
				return;
			case SubscriptionKiosk.ScreenState.MainMenuUnsubscribed:
				this.UpdateUnsubscribedMenu();
				return;
			case SubscriptionKiosk.ScreenState.SubscriptionData:
				this.UpdateSubscriptionData();
				return;
			case SubscriptionKiosk.ScreenState.FeatureToggles:
			{
				FeatureTogglesScreen component = this.screensByState[SubscriptionKiosk.ScreenState.FeatureToggles].GetComponent<FeatureTogglesScreen>();
				if (component != null)
				{
					component.enabled = true;
					component.MarkDirty();
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x0600625F RID: 25183 RVA: 0x001FBEE8 File Offset: 0x001FA0E8
		private void ActivateScreen(SubscriptionKiosk.ScreenState activeScreen)
		{
			foreach (KeyValuePair<SubscriptionKiosk.ScreenState, GameObject> keyValuePair in this.screensByState)
			{
				keyValuePair.Value.SetActive(keyValuePair.Key == activeScreen);
			}
		}

		// Token: 0x06006260 RID: 25184 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void AddButton(SITouchscreenButton button, bool isPopupButton = false)
		{
		}

		// Token: 0x06006261 RID: 25185 RVA: 0x001FBF4C File Offset: 0x001FA14C
		public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
		{
			if (actorNr != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return;
			}
			switch (this.currentState)
			{
			case SubscriptionKiosk.ScreenState.MainMenuSubscribed:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionData);
					return;
				}
				if (buttonType != SITouchscreenButton.SITouchscreenButtonType.PageSelect)
				{
					return;
				}
				this.UpdateState(SubscriptionKiosk.ScreenState.FeatureToggles);
				return;
			case SubscriptionKiosk.ScreenState.MainMenuUnsubscribed:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Subscribe)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionSteamWarning);
					this.subsVideoPlayer.clip = this.steamSubsVideoClip;
					this.subsVideoObservable.ObservableBehaviorRule = this.steamObservableRule;
					return;
				}
				break;
			case SubscriptionKiosk.ScreenState.SubscriptionData:
				if (buttonType != SITouchscreenButton.SITouchscreenButtonType.Cancel)
				{
					if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
					{
						this.HandScanned();
						return;
					}
					if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Subscribe)
					{
						this.UpdateState(SubscriptionKiosk.ScreenState.PurchaseSubscription);
						return;
					}
				}
				break;
			case SubscriptionKiosk.ScreenState.PurchaseSubscription:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Subscribe)
				{
					this.PurchaseSubscription((SubscriptionManager.SubscriptionTerm)data);
				}
				else if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
				{
					this.HandScanned();
				}
				this.subsVideoPlayer.clip = this.defaultVideoClip;
				this.subsVideoObservable.ObservableBehaviorRule = this.defaultObservableRule;
				return;
			case SubscriptionKiosk.ScreenState.SubscriptionPurchaseInProgress:
			case SubscriptionKiosk.ScreenState.FeatureToggles:
				break;
			case SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Confirm)
				{
					this.HandScanned();
				}
				break;
			case SubscriptionKiosk.ScreenState.SubscriptionSteamWarning:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Subscribe)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.PurchaseSubscription);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006262 RID: 25186 RVA: 0x001FC058 File Offset: 0x001FA258
		public void TouchscreenToggleButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, bool isToggledOn)
		{
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}

		// Token: 0x06006263 RID: 25187 RVA: 0x001FC06C File Offset: 0x001FA26C
		public void OnToggleFeaturesExitButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
		{
			if (actorNr != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return;
			}
			this.UpdateState(SubscriptionKiosk.ScreenState.MainMenuSubscribed);
		}

		// Token: 0x06006264 RID: 25188 RVA: 0x001FC088 File Offset: 0x001FA288
		private void UpdateToggleButtonState(int buttonData, bool state)
		{
			foreach (SITouchscreenButtonContainer sitouchscreenButtonContainer in this.toggleButtonContainers)
			{
				if (sitouchscreenButtonContainer.button.data == buttonData)
				{
					sitouchscreenButtonContainer.button.SetToggleState(state, true);
					break;
				}
			}
		}

		// Token: 0x06006265 RID: 25189 RVA: 0x001FC0F4 File Offset: 0x001FA2F4
		private bool GetSubscriptionFeatureState(int buttonData)
		{
			switch (buttonData)
			{
			case 0:
				return SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.GoldenName);
			case 1:
				return SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.IOBT);
			case 2:
				return SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.HandTracking);
			default:
				Debug.Log(string.Format("Getting current state for subscription kiosk {0}", buttonData));
				return false;
			}
		}

		// Token: 0x06006266 RID: 25190 RVA: 0x001FC140 File Offset: 0x001FA340
		public void UpdateGoldNameTag(bool state)
		{
			this.ToggleSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures.GoldenName, state);
			VRRig.LocalRig.OnSubscriptionData();
			if (GorillaScoreboardTotalUpdater.instance != null)
			{
				GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
			}
		}

		// Token: 0x06006267 RID: 25191 RVA: 0x001FC16B File Offset: 0x001FA36B
		public void UpdateIOTBExperimentalFeature(bool state)
		{
			this.ToggleSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures.IOBT, state);
			if (GorillaIK.playerIK != null)
			{
				GorillaIK.playerIK.ResetIKData();
				GorillaIK.playerIK.usingUpdatedIK = state;
			}
		}

		// Token: 0x06006268 RID: 25192 RVA: 0x001FC197 File Offset: 0x001FA397
		public void UpdateHandTrackingExperimentalFeature(bool state)
		{
			this.ToggleSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures.HandTracking, state);
		}

		// Token: 0x06006269 RID: 25193 RVA: 0x001FC1A1 File Offset: 0x001FA3A1
		private void ToggleSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures feature, bool state)
		{
			SubscriptionManager.SetSubscriptionSettingValue(feature, state ? 1 : 0);
		}

		// Token: 0x0600626A RID: 25194 RVA: 0x001FC1B0 File Offset: 0x001FA3B0
		private void UpdateSubscribedMenu()
		{
			this.subMenuPlayerName.text = NetworkSystem.Instance.LocalPlayer.SanitizedNickName;
			this.subMenuDaysAccrued.text = SubscriptionManager.GetSubscriptionDetails().daysAccrued.ToString();
			foreach (SITouchscreenButtonContainer sitouchscreenButtonContainer in this.toggleButtonContainers)
			{
				this.UpdateToggleButtonState(sitouchscreenButtonContainer.data, this.GetSubscriptionFeatureState(sitouchscreenButtonContainer.data));
			}
		}

		// Token: 0x0600626B RID: 25195 RVA: 0x001FC24C File Offset: 0x001FA44C
		private void UpdateUnsubscribedMenu()
		{
			this.unsubscribedMenuPlayerName.text = NetworkSystem.Instance.LocalPlayer.SanitizedNickName;
			this.mainMenuUnsubscribedQuestText.SetActive(false);
			this.mainMenuUnsubscribedSteamText.SetActive(true);
			this.subsVideoPlayer.clip = this.defaultVideoClip;
			this.subsVideoObservable.ObservableBehaviorRule = this.defaultObservableRule;
		}

		// Token: 0x0600626C RID: 25196 RVA: 0x001FC2B0 File Offset: 0x001FA4B0
		private void UpdateSubscriptionData()
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails = SubscriptionManager.GetSubscriptionDetails();
			this.subDataPlayerName.text = NetworkSystem.Instance.LocalPlayer.SanitizedNickName;
			this.subDataDaysAccrued.text = subscriptionDetails.daysAccrued.ToString();
			this.subDataDaysRemaining.text = Mathf.RoundToInt((float)(subscriptionDetails.subscriptionActiveUntilDate - DateTime.UtcNow).TotalDays).ToString();
			this.subDataAutoRenew.text = (subscriptionDetails.autoRenew ? "ENABLED" : "DISABLED");
			this.subDataRenewDate.text = subscriptionDetails.subscriptionActiveUntilDate.ToString("MMM d, yyyy").ToUpper();
			this.subDataSubscriptionTerm.text = subscriptionDetails.autoRenewMonths.ToString() + " MONTH" + ((subscriptionDetails.autoRenewMonths > 1) ? "S" : "");
			if (this.subDataSubscribeButton.activeSelf == subscriptionDetails.autoRenew)
			{
				this.subDataSubscribeButton.SetActive(!subscriptionDetails.autoRenew);
			}
		}

		// Token: 0x0600626D RID: 25197 RVA: 0x001FC3C4 File Offset: 0x001FA5C4
		private void UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult result)
		{
			this.lastPurchase = result;
			string text = "";
			if (result == SubscriptionKiosk.PurchaseResult.Success)
			{
				text = "SUBSCRIPTION SUCCESSFUL! WELCOME TO THE FAN CLUB, YOU ARE NOW A VERY IMPORTANT MONKE (V.I.M.)!";
				LocalisationManager.TryGetKeyForCurrentLocale("SUBKIOSKPURCHASE_SUCCESS", out text, text);
			}
			else if (result == SubscriptionKiosk.PurchaseResult.Failure)
			{
				text = "PURCHASE FAILED! WE'RE NOT SURE WHAT HAPPENED, BUT PLEASE CHECK YOUR INFORMATION, OR TRY AGAIN LATER. IF IT LOOKED LIKE THE PURCHASE SHOULD HAVE SUCCEEDED, TRY RESTARTING THE GAME.";
				LocalisationManager.TryGetKeyForCurrentLocale("SUBKIOSKPURCHASE_FAIL", out text, text);
			}
			else if (result == SubscriptionKiosk.PurchaseResult.Cancel)
			{
				text = "PURCHASE CANCELED! WE'LL BE HERE IF YOU CHANGE YOUR MIND!";
				LocalisationManager.TryGetKeyForCurrentLocale("SUBKIOSKPURCHASE_CANCEL", out text, text);
			}
			this.purchaseResultText.text = text;
		}

		// Token: 0x0600626E RID: 25198 RVA: 0x001FC438 File Offset: 0x001FA638
		private void ProcessSteamCallback(MicroTxnAuthorizationResponse_t callBackResponse)
		{
			if (callBackResponse.m_bAuthorized == 0)
			{
				Debug.Log("The user did not authorize the steam subscription purchase");
				this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Cancel);
				this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult);
			}
			MothershipClientApiUnity.FinalizeSteamSubscriptionTransaction(this.steamOrderId, delegate(FinalizeSteamSubscriptionPurchaseResponse Response)
			{
				SubscriptionKiosk.ProcessingSubscriptionPurchase = false;
				this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Success);
				this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult);
				SubscriptionManager.InitializePersonalSubscriptionData();
			}, delegate(MothershipError Error, int Status)
			{
				SubscriptionKiosk.ProcessingSubscriptionPurchase = false;
				this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Failure);
				this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult);
				Debug.LogError("SubscriptionKiosk could not finalzie STEAM iap. Trace ID " + Error.TraceId + ", Error Code: " + Error.MothershipErrorCode);
			});
		}

		// Token: 0x0600626F RID: 25199 RVA: 0x001FC48C File Offset: 0x001FA68C
		private void PurchaseSubscription(SubscriptionManager.SubscriptionTerm subTerm)
		{
			if (SteamManager.Initialized && this._steamMicroTransactionAuthorizationResponse == null)
			{
				this._steamMicroTransactionAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(new Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate(this.ProcessSteamCallback));
			}
			Debug.Log("Starting Steam Subscription Purchase");
			int frequency = 1;
			int priceInUSDCents = 999;
			string frequencyUnit = "Month";
			switch (subTerm)
			{
			case SubscriptionManager.SubscriptionTerm.MONTHLY:
				frequency = 1;
				priceInUSDCents = 999;
				frequencyUnit = "Month";
				break;
			case SubscriptionManager.SubscriptionTerm.QUARTERLY:
				frequency = 3;
				priceInUSDCents = 2699;
				frequencyUnit = "Month";
				break;
			case SubscriptionManager.SubscriptionTerm.SEMIANNUAL:
				frequency = 6;
				priceInUSDCents = 4999;
				frequencyUnit = "Month";
				break;
			case SubscriptionManager.SubscriptionTerm.ANNUAL:
				frequency = 1;
				priceInUSDCents = 9499;
				frequencyUnit = "Year";
				break;
			}
			SubscriptionKiosk.ProcessingSubscriptionPurchase = true;
			MothershipClientApiUnity.InitSteamSubscriptionTransaction("40494", frequencyUnit, frequency, priceInUSDCents, delegate(InitSteamSubscriptionPurchaseResponse Response)
			{
				this.steamOrderId = Response.SteamOrderId;
			}, delegate(MothershipError Error, int Status)
			{
				this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Failure);
				this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult);
				Debug.LogError("SubscriptionKiosk could not start STEAM iap. Trace ID " + Error.TraceId + ", Error Code: " + Error.MothershipErrorCode);
				SubscriptionKiosk.ProcessingSubscriptionPurchase = false;
			});
			this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseInProgress);
		}

		// Token: 0x06006270 RID: 25200 RVA: 0x001FC560 File Offset: 0x001FA760
		public void LaunchCheckoutFlowCallback(Message<Purchase> msg)
		{
			Debug.Log(string.Format("SubscriptionKiosk Purchase result: {0}   isError: {1}   Data: {2}", msg.Type, msg.IsError, msg.Data.ToString()));
			if (msg.IsError)
			{
				Error error = msg.GetError();
				if (error != null && error.Message != null && error.Message.Contains("cancel"))
				{
					this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Cancel);
					return;
				}
				this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Failure);
			}
			else
			{
				Purchase purchase = msg.GetPurchase();
				if (purchase != null && !string.IsNullOrEmpty(purchase.Sku))
				{
					this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Success);
				}
				else
				{
					this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Failure);
				}
			}
			SubscriptionManager.InitializePersonalSubscriptionData();
			this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult);
		}

		// Token: 0x06006271 RID: 25201 RVA: 0x001FC610 File Offset: 0x001FA810
		public void LocalSubscriptionDataUpdated()
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails = SubscriptionManager.LocalSubscriptionDetails();
			if (subscriptionDetails.active)
			{
				if (this.lastPurchase == SubscriptionKiosk.PurchaseResult.Failure)
				{
					this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Success);
				}
				if (this.currentState == SubscriptionKiosk.ScreenState.MainMenuUnsubscribed)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.MainMenuSubscribed);
				}
				if (this.currentState == SubscriptionKiosk.ScreenState.PurchaseSubscription && subscriptionDetails.autoRenew)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionData);
				}
			}
			this.subsVideoPlayer.clip = this.defaultVideoClip;
		}

		// Token: 0x06006272 RID: 25202 RVA: 0x001FC674 File Offset: 0x001FA874
		private void UpdateSubsVideo()
		{
			if (SubscriptionManager.IsLocalSubscribed())
			{
				this.subsVideoObservable.ObservableBehaviorRule = this.defaultObservableRule;
				this.subsVideoPlayer.clip = this.defaultVideoClip;
				return;
			}
			this.subsVideoPlayer.clip = this.steamSubsVideoClip;
			this.subsVideoObservable.ObservableBehaviorRule = this.steamObservableRule;
		}

		// Token: 0x06006273 RID: 25203 RVA: 0x001FC6CD File Offset: 0x001FA8CD
		public void SliceUpdate()
		{
			if (this.currentState == SubscriptionKiosk.ScreenState.SubscriptionStatusUnknown)
			{
				this.HandScanned();
			}
		}

		// Token: 0x06006275 RID: 25205 RVA: 0x0000636B File Offset: 0x0000456B
		GameObject ITouchScreenStation.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04007126 RID: 28966
		private const string SUBSCRIPTION_KIOSK_PREFIX = "SUBKIOSK";

		// Token: 0x04007127 RID: 28967
		private const string PURCHASE_SUCCESS_KEY = "SUBKIOSKPURCHASE_SUCCESS";

		// Token: 0x04007128 RID: 28968
		private const string PURCHASE_CANCEL_KEY = "SUBKIOSKPURCHASE_CANCEL";

		// Token: 0x04007129 RID: 28969
		private const string PURCHASE_FAIL_KEY = "SUBKIOSKPURCHASE_FAIL";

		// Token: 0x0400712A RID: 28970
		private const string subSKU = "fan_club";

		// Token: 0x0400712B RID: 28971
		[SerializeField]
		private VideoPlayer subsVideoPlayer;

		// Token: 0x0400712C RID: 28972
		[SerializeField]
		private ObservableBehavior subsVideoObservable;

		// Token: 0x0400712D RID: 28973
		[SerializeField]
		private VideoClip defaultVideoClip;

		// Token: 0x0400712E RID: 28974
		[SerializeField]
		private VideoClip steamSubsVideoClip;

		// Token: 0x0400712F RID: 28975
		[SerializeField]
		private ObservableBehaviorRule defaultObservableRule;

		// Token: 0x04007130 RID: 28976
		[SerializeField]
		private ObservableBehaviorRule steamObservableRule;

		// Token: 0x04007131 RID: 28977
		[Space]
		[SerializeField]
		private GameObject steamComingSoon;

		// Token: 0x04007132 RID: 28978
		[SerializeField]
		private GameObject safeAccountScreen;

		// Token: 0x04007133 RID: 28979
		[SerializeField]
		private GameObject waitingForScanScreen;

		// Token: 0x04007134 RID: 28980
		[SerializeField]
		private GameObject scanningScreen;

		// Token: 0x04007135 RID: 28981
		[SerializeField]
		private GameObject subStatusUnknownScreen;

		// Token: 0x04007136 RID: 28982
		[SerializeField]
		private GameObject mainMenuSubscribedScreen;

		// Token: 0x04007137 RID: 28983
		[Space]
		[SerializeField]
		private GameObject mainMenuUnsubscribedScreen;

		// Token: 0x04007138 RID: 28984
		[SerializeField]
		private GameObject mainMenuUnsubscribedQuestText;

		// Token: 0x04007139 RID: 28985
		[SerializeField]
		private GameObject mainMenuUnsubscribedSteamText;

		// Token: 0x0400713A RID: 28986
		[Space]
		[SerializeField]
		private GameObject subDataScreen;

		// Token: 0x0400713B RID: 28987
		[SerializeField]
		private GameObject purchaseSubScreen;

		// Token: 0x0400713C RID: 28988
		[SerializeField]
		private GameObject purchaseProgressScreen;

		// Token: 0x0400713D RID: 28989
		[SerializeField]
		private GameObject purchaseResultScreen;

		// Token: 0x0400713E RID: 28990
		[SerializeField]
		private GameObject featureTogglesScreen;

		// Token: 0x04007141 RID: 28993
		private List<SITouchscreenButtonContainer> toggleButtonContainers;

		// Token: 0x04007142 RID: 28994
		private Dictionary<SubscriptionKiosk.ScreenState, GameObject> screensByState;

		// Token: 0x04007143 RID: 28995
		private string steamOrderId = "";

		// Token: 0x04007144 RID: 28996
		[SerializeField]
		private TextMeshPro subMenuPlayerName;

		// Token: 0x04007145 RID: 28997
		[SerializeField]
		private TextMeshPro subMenuDaysAccrued;

		// Token: 0x04007146 RID: 28998
		[SerializeField]
		private TextMeshPro unsubscribedMenuPlayerName;

		// Token: 0x04007147 RID: 28999
		[SerializeField]
		private TextMeshPro subDataPlayerName;

		// Token: 0x04007148 RID: 29000
		[SerializeField]
		private TextMeshPro subDataDaysAccrued;

		// Token: 0x04007149 RID: 29001
		[SerializeField]
		private TextMeshPro subDataDaysRemaining;

		// Token: 0x0400714A RID: 29002
		[SerializeField]
		private TextMeshPro subDataAutoRenew;

		// Token: 0x0400714B RID: 29003
		[SerializeField]
		private TextMeshPro subDataRenewDate;

		// Token: 0x0400714C RID: 29004
		[SerializeField]
		private TextMeshPro subDataSubscriptionTerm;

		// Token: 0x0400714D RID: 29005
		[SerializeField]
		private GameObject subDataSubscribeButton;

		// Token: 0x0400714E RID: 29006
		[SerializeField]
		private TextMeshPro purchaseResultText;

		// Token: 0x0400714F RID: 29007
		private SubscriptionKiosk.ScreenState currentState = SubscriptionKiosk.ScreenState.WaitingForScan;

		// Token: 0x04007150 RID: 29008
		private SubscriptionKiosk.ScreenState lastState;

		// Token: 0x04007151 RID: 29009
		private SubscriptionKiosk.PurchaseResult lastPurchase;

		// Token: 0x04007152 RID: 29010
		private Callback<MicroTxnAuthorizationResponse_t> _steamMicroTransactionAuthorizationResponse;

		// Token: 0x02000F6B RID: 3947
		private enum ScreenState
		{
			// Token: 0x04007154 RID: 29012
			SafeAccount,
			// Token: 0x04007155 RID: 29013
			WaitingForScan,
			// Token: 0x04007156 RID: 29014
			Scanning,
			// Token: 0x04007157 RID: 29015
			SubscriptionStatusUnknown,
			// Token: 0x04007158 RID: 29016
			MainMenuSubscribed,
			// Token: 0x04007159 RID: 29017
			MainMenuUnsubscribed,
			// Token: 0x0400715A RID: 29018
			SubscriptionData,
			// Token: 0x0400715B RID: 29019
			PurchaseSubscription,
			// Token: 0x0400715C RID: 29020
			SubscriptionPurchaseInProgress,
			// Token: 0x0400715D RID: 29021
			SubscriptionPurchaseResult,
			// Token: 0x0400715E RID: 29022
			FeatureToggles,
			// Token: 0x0400715F RID: 29023
			SubscriptionSteamWarning,
			// Token: 0x04007160 RID: 29024
			None
		}

		// Token: 0x02000F6C RID: 3948
		private enum PurchaseResult
		{
			// Token: 0x04007162 RID: 29026
			Success,
			// Token: 0x04007163 RID: 29027
			Failure,
			// Token: 0x04007164 RID: 29028
			Cancel
		}
	}
}
