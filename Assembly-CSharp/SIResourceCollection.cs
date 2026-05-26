using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000156 RID: 342
public class SIResourceCollection : MonoBehaviour, ITouchScreenStation
{
	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x0600090E RID: 2318 RVA: 0x00030F71 File Offset: 0x0002F171
	public SIScreenRegion ScreenRegion
	{
		get
		{
			return this.screenRegion;
		}
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x0600090F RID: 2319 RVA: 0x00030F79 File Offset: 0x0002F179
	public bool IsAuthority
	{
		get
		{
			return this.SIManager.gameEntityManager.IsAuthority();
		}
	}

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000910 RID: 2320 RVA: 0x00030F8B File Offset: 0x0002F18B
	public SIPlayer ActivePlayer
	{
		get
		{
			return this.parentTerminal.activePlayer;
		}
	}

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000911 RID: 2321 RVA: 0x00030F98 File Offset: 0x0002F198
	public SuperInfectionManager SIManager
	{
		get
		{
			return this.parentTerminal.superInfection.siManager;
		}
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x00030FAC File Offset: 0x0002F1AC
	private void CollectButtonColliders()
	{
		SIResourceCollection.<>c__DisplayClass45_0 CS$<>8__locals1;
		CS$<>8__locals1.buttons = base.GetComponentsInChildren<SITouchscreenButton>(true).ToList<SITouchscreenButton>();
		SIResourceCollection.<CollectButtonColliders>g__RemoveButtonsInside|45_2((from d in base.GetComponentsInChildren<DestroyIfNotBeta>()
		select d.gameObject).ToArray<GameObject>(), ref CS$<>8__locals1);
		SIResourceCollection.<CollectButtonColliders>g__RemoveButtonsInside|45_2(new GameObject[]
		{
			this.helpScreen
		}, ref CS$<>8__locals1);
		this._nonPopupButtonColliders = (from b in CS$<>8__locals1.buttons
		select b.GetComponent<Collider>()).ToList<Collider>();
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x00031050 File Offset: 0x0002F250
	private void SetNonPopupButtonsEnabled(bool enable)
	{
		foreach (Collider collider in this._nonPopupButtonColliders)
		{
			collider.enabled = enable;
		}
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x000310A4 File Offset: 0x0002F2A4
	public void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
		if (this.parentTerminal == null)
		{
			this.parentTerminal = base.GetComponentInParent<SICombinedTerminal>();
		}
		this.screenData = new Dictionary<SIResourceCollection.ResourceCollectorTerminalState, GameObject>();
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan, this.waitingForScanScreen);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.CurrentResources, this.currentResourcesScreen);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.HelpScreen, this.helpScreen);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.PurchaseRemote, this.purchasingRemote);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart, this.purchasingStart);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.PurchaseInProgress, this.purchaseInProgress);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.PurchaseSuccess, this.purchasingSuccess);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.PurchaseFailure, this.purchasingFailure);
		this.Reset();
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x0003117C File Offset: 0x0002F37C
	public void Reset()
	{
		this.currentState = SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan;
		this.lastState = this.currentState;
		this.SetScreenVisibility(this.currentState, this.lastState);
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x000311A4 File Offset: 0x0002F3A4
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy)
		{
			this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan, SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan);
		}
		stream.SendNext(this.currentHelpButtonPageIndex);
		stream.SendNext((int)this.currentState);
		stream.SendNext((int)this.lastState);
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x0003120C File Offset: 0x0002F40C
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.currentHelpButtonPageIndex = Mathf.Clamp((int)stream.ReceiveNext(), 0, this.helpPopupScreens.Length - 1);
		this.UpdateHelpButtonPage(this.currentHelpButtonPageIndex);
		SIResourceCollection.ResourceCollectorTerminalState resourceCollectorTerminalState = (SIResourceCollection.ResourceCollectorTerminalState)stream.ReceiveNext();
		SIResourceCollection.ResourceCollectorTerminalState resourceCollectorTerminalState2 = (SIResourceCollection.ResourceCollectorTerminalState)stream.ReceiveNext();
		if (!Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState) || !Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState2))
		{
			resourceCollectorTerminalState = SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan;
			resourceCollectorTerminalState2 = SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan;
		}
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState) || !Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState2))
		{
			this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan, SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(resourceCollectorTerminalState, resourceCollectorTerminalState2);
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x000312EF File Offset: 0x0002F4EF
	public void ZoneDataSerializeWrite(BinaryWriter writer)
	{
		writer.Write(this.currentHelpButtonPageIndex);
		writer.Write((int)this.currentState);
		writer.Write((int)this.lastState);
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x00031318 File Offset: 0x0002F518
	public void ZoneDataSerializeRead(BinaryReader reader)
	{
		this.currentHelpButtonPageIndex = Mathf.Clamp(reader.ReadInt32(), 0, this.helpPopupScreens.Length - 1);
		this.UpdateHelpButtonPage(this.currentHelpButtonPageIndex);
		SIResourceCollection.ResourceCollectorTerminalState resourceCollectorTerminalState = (SIResourceCollection.ResourceCollectorTerminalState)reader.ReadInt32();
		SIResourceCollection.ResourceCollectorTerminalState resourceCollectorTerminalState2 = (SIResourceCollection.ResourceCollectorTerminalState)reader.ReadInt32();
		if (!Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState) || !Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState2))
		{
			resourceCollectorTerminalState = SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan;
			resourceCollectorTerminalState2 = SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan;
		}
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState) || !Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState2))
		{
			this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan, SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(resourceCollectorTerminalState, resourceCollectorTerminalState2);
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x000313EC File Offset: 0x0002F5EC
	public bool PopupActive()
	{
		return this.IsPopupState(this.currentState);
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x000313FA File Offset: 0x0002F5FA
	public bool IsPopupState(SIResourceCollection.ResourceCollectorTerminalState state)
	{
		return state == SIResourceCollection.ResourceCollectorTerminalState.HelpScreen || state == SIResourceCollection.ResourceCollectorTerminalState.PurchaseInProgress || state == SIResourceCollection.ResourceCollectorTerminalState.PurchaseRemote || state == SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart || state == SIResourceCollection.ResourceCollectorTerminalState.PurchaseFailure || state == SIResourceCollection.ResourceCollectorTerminalState.PurchaseSuccess;
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x00031416 File Offset: 0x0002F616
	public bool HasHelpButton(SIResourceCollection.ResourceCollectorTerminalState state)
	{
		return state == SIResourceCollection.ResourceCollectorTerminalState.CurrentResources || state == SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan;
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x00031422 File Offset: 0x0002F622
	public void UpdateState(SIResourceCollection.ResourceCollectorTerminalState newState, SIResourceCollection.ResourceCollectorTerminalState newLastState)
	{
		if (!this.IsPopupState(newLastState))
		{
			this.currentState = newLastState;
		}
		this.UpdateState(newState);
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x0003143C File Offset: 0x0002F63C
	public void UpdateState(SIResourceCollection.ResourceCollectorTerminalState newState)
	{
		if (!this.IsPopupState(this.currentState))
		{
			this.lastState = this.currentState;
		}
		this.currentState = newState;
		this.SetScreenVisibility(this.currentState, this.lastState);
		switch (this.currentState)
		{
		case SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan:
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseInProgress:
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseSuccess:
			break;
		case SIResourceCollection.ResourceCollectorTerminalState.CurrentResources:
			this.currentResourcesResourceCounts.text = this.FormattedPlayerResourceCount(this.ActivePlayer);
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.HelpScreen:
			this.UpdateHelpButtonPage(this.currentHelpButtonPageIndex);
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseRemote:
			if (this.ActivePlayer != null && this.ActivePlayer == SIPlayer.LocalPlayer)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart);
			}
			this.currentResourceCountsLocal.text = this.FormattedPlayerResourceCountWithMax(this.ActivePlayer);
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart:
			if (this.ActivePlayer != null && this.ActivePlayer != SIPlayer.LocalPlayer)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseRemote);
			}
			else
			{
				this.shinyRockInfo.text = "PRICE: 500 SHINY ROCKS\n\nYOU HAVE:\n" + ProgressionManager.Instance.GetShinyRocksTotal().ToString() + " SHINY ROCKS";
			}
			this.currentResourceCountsLocal.text = this.FormattedPlayerResourceCountWithMax(this.ActivePlayer);
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseFailure:
			switch (this.failureReason)
			{
			case SIResourceCollection.FailReason.NotEnoughRocks:
				this.failureReasonText.text = "NOT ENOUGH SHINY ROCKS! PLEASE TRY AGAIN LATER, OR PURCHASE MORE SHINY ROCKS!";
				return;
			case SIResourceCollection.FailReason.ResourcesFull:
				this.failureReasonText.text = "YOU ARE ALREADY AT MAX RESOURCES! DONATE YOUR SHINY ROCKS TO A GOOD CAUSE INSTEAD OF US, KNUCKLEHEAD!";
				return;
			case SIResourceCollection.FailReason.Unknown:
				this.failureReasonText.text = "UHHHHH SOMETHING WENT WRONG, I'M NOT SURE WHAT, SORRY TRY AGAIN LATER MAYBE!";
				break;
			default:
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x000315C8 File Offset: 0x0002F7C8
	public string FormattedPlayerResourceCount(SIPlayer player)
	{
		return string.Concat(new string[]
		{
			this.GetFormattedResource(player, SIResource.ResourceType.TechPoint),
			"\n",
			this.GetFormattedResource(player, SIResource.ResourceType.StrangeWood),
			"\n",
			this.GetFormattedResource(player, SIResource.ResourceType.WeirdGear),
			"\n",
			this.GetFormattedResource(player, SIResource.ResourceType.VibratingSpring),
			"\n",
			this.GetFormattedResource(player, SIResource.ResourceType.BouncySand),
			"\n",
			this.GetFormattedResource(player, SIResource.ResourceType.FloppyMetal)
		});
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x00031650 File Offset: 0x0002F850
	public string FormattedPlayerResourceCountWithMax(SIPlayer player)
	{
		return string.Concat(new string[]
		{
			this.GetFormattedResource(player, SIResource.ResourceType.StrangeWood),
			" -> 20\n",
			this.GetFormattedResource(player, SIResource.ResourceType.WeirdGear),
			" -> 20\n",
			this.GetFormattedResource(player, SIResource.ResourceType.VibratingSpring),
			" -> 20\n",
			this.GetFormattedResource(player, SIResource.ResourceType.BouncySand),
			" -> 20\n",
			this.GetFormattedResource(player, SIResource.ResourceType.FloppyMetal),
			" -> 20"
		});
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x000316CC File Offset: 0x0002F8CC
	private string GetFormattedResource(SIPlayer player, SIResource.ResourceType resource)
	{
		int resourceMaxCap = SIProgression.Instance.GetResourceMaxCap(resource);
		if (resourceMaxCap == 2147483647)
		{
			return player.CurrentProgression.resourceArray[(int)resource].ToString();
		}
		return string.Format("{0}/{1}", player.CurrentProgression.resourceArray[(int)resource], resourceMaxCap);
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x00031728 File Offset: 0x0002F928
	public void UpdateHelpButtonPage(int helpButtonPageIndex)
	{
		for (int i = 0; i < this.helpPopupScreens.Length; i++)
		{
			this.helpPopupScreens[i].SetActive(i == helpButtonPageIndex);
		}
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x0003175C File Offset: 0x0002F95C
	public void SetScreenVisibility(SIResourceCollection.ResourceCollectorTerminalState currentState, SIResourceCollection.ResourceCollectorTerminalState lastState)
	{
		bool flag = this.IsPopupState(currentState);
		this.background.color = ((currentState == SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan) ? Color.white : ((this.ActivePlayer != null && this.ActivePlayer.gamePlayer.IsLocal()) ? this.active : this.notActive));
		foreach (SIResourceCollection.ResourceCollectorTerminalState resourceCollectorTerminalState in this.screenData.Keys)
		{
			bool flag2 = resourceCollectorTerminalState == currentState || (flag && resourceCollectorTerminalState == lastState);
			if (this.screenData[resourceCollectorTerminalState].activeSelf != flag2)
			{
				this.screenData[resourceCollectorTerminalState].SetActive(flag2);
			}
		}
		if (this.popupScreen.activeSelf != flag)
		{
			this.popupScreen.SetActive(flag);
		}
		this.SetNonPopupButtonsEnabled(!flag);
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x00031854 File Offset: 0x0002FA54
	public void PlayerHandScanned(int actorNr)
	{
		this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.CurrentResources);
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x00031860 File Offset: 0x0002FA60
	public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
	{
		if (actorNr == SIPlayer.LocalPlayer.ActorNr && (this.ActivePlayer == null || this.ActivePlayer != SIPlayer.LocalPlayer))
		{
			this.parentTerminal.PlayWrongPlayerBuzz(this.uiCenter);
		}
		else
		{
			this.soundBankPlayer.Play();
		}
		if (actorNr == SIPlayer.LocalPlayer.ActorNr && this.ActivePlayer == SIPlayer.LocalPlayer && this.currentState == SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart && buttonType == SITouchscreenButton.SITouchscreenButtonType.Confirm)
		{
			bool flag = ProgressionManager.Instance.GetShinyRocksTotal() >= 500;
			bool flag2 = SIProgression.ResourcesMaxed();
			if (flag && !flag2)
			{
				ProgressionManager.Instance.PurchaseResources(delegate(ProgressionManager.UserInventory userInventoryResponse)
				{
					SIProgression.Instance.SendPurchaseResourcesData();
					ProgressionManager.Instance.RefreshUserInventory();
					this.TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType.Collect, -1, SIPlayer.LocalPlayer.ActorNr);
				}, delegate(string error)
				{
					SIResourceCollection.FailReason data2;
					if (!(error == "Not enough Shiny Rocks to complete this purchase"))
					{
						if (!(error == "already maxed resources"))
						{
							data2 = SIResourceCollection.FailReason.Unknown;
						}
						else
						{
							data2 = SIResourceCollection.FailReason.ResourcesFull;
						}
					}
					else
					{
						data2 = SIResourceCollection.FailReason.NotEnoughRocks;
					}
					this.TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType.OverrideFailure, (int)data2, SIPlayer.LocalPlayer.ActorNr);
				});
			}
			else
			{
				buttonType = SITouchscreenButton.SITouchscreenButtonType.OverrideFailure;
				if (!flag)
				{
					data = 0;
				}
				else if (flag2)
				{
					data = 1;
				}
				else
				{
					data = 2;
				}
			}
		}
		if (!this.IsAuthority)
		{
			this.parentTerminal.TouchscreenButtonPressed(buttonType, data, actorNr, SICombinedTerminal.TerminalSubFunction.ResourceCollection);
			return;
		}
		if (this.ActivePlayer == null || actorNr != this.ActivePlayer.ActorNr)
		{
			return;
		}
		this.soundBankPlayer.Play();
		switch (this.currentState)
		{
		case SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.HelpScreen);
			}
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.CurrentResources:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Purchase)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart);
			}
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.HelpScreen:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Exit)
			{
				this.currentHelpButtonPageIndex = 0;
				this.UpdateState(this.lastState);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Next)
			{
				this.currentHelpButtonPageIndex = Mathf.Clamp(this.currentHelpButtonPageIndex + 1, 0, this.helpPopupScreens.Length - 1);
				this.UpdateHelpButtonPage(this.currentHelpButtonPageIndex);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.currentHelpButtonPageIndex = Mathf.Clamp(this.currentHelpButtonPageIndex - 1, 0, this.helpPopupScreens.Length - 1);
				this.UpdateHelpButtonPage(this.currentHelpButtonPageIndex);
			}
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseRemote:
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Confirm)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseInProgress);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Cancel)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.CurrentResources);
				return;
			}
			this.failureReason = (SIResourceCollection.FailReason)data;
			this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseFailure);
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseInProgress:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.OverrideFailure)
			{
				this.failureReason = (SIResourceCollection.FailReason)data;
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseFailure);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Collect)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseSuccess);
			}
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseSuccess:
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseFailure:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Exit)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.CurrentResources);
			}
			return;
		default:
			return;
		}
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void TouchscreenToggleButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, bool isToggledOn)
	{
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void AddButton(SITouchscreenButton button, bool isPopupButton = false)
	{
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x0000636B File Offset: 0x0000456B
	GameObject ITouchScreenStation.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x00031A94 File Offset: 0x0002FC94
	[CompilerGenerated]
	internal static void <CollectButtonColliders>g__RemoveButtonsInside|45_2(GameObject[] roots, ref SIResourceCollection.<>c__DisplayClass45_0 A_1)
	{
		for (int i = 0; i < roots.Length; i++)
		{
			foreach (SITouchscreenButton item in roots[i].GetComponentsInChildren<SITouchscreenButton>(true))
			{
				A_1.buttons.Remove(item);
			}
		}
	}

	// Token: 0x04000B10 RID: 2832
	public const int REFILL_PURCHASE_SHINY_ROCK_COST = 500;

	// Token: 0x04000B11 RID: 2833
	private const string lineBreak = "\n";

	// Token: 0x04000B12 RID: 2834
	private const string appendToMax = " -> 20";

	// Token: 0x04000B13 RID: 2835
	public SIResourceCollection.ResourceCollectorTerminalState currentState;

	// Token: 0x04000B14 RID: 2836
	public SIResourceCollection.ResourceCollectorTerminalState lastState;

	// Token: 0x04000B15 RID: 2837
	public int resourceDepositedCount;

	// Token: 0x04000B16 RID: 2838
	private int currentHelpButtonPageIndex;

	// Token: 0x04000B17 RID: 2839
	public GameObject waitingForScanScreen;

	// Token: 0x04000B18 RID: 2840
	public GameObject currentResourcesScreen;

	// Token: 0x04000B19 RID: 2841
	public GameObject helpScreen;

	// Token: 0x04000B1A RID: 2842
	public SICombinedTerminal parentTerminal;

	// Token: 0x04000B1B RID: 2843
	public Sprite[] resourceImageSprites;

	// Token: 0x04000B1C RID: 2844
	[SerializeField]
	private SIScreenRegion screenRegion;

	// Token: 0x04000B1D RID: 2845
	public GameObject[] helpPopupScreens;

	// Token: 0x04000B1E RID: 2846
	public GameObject purchasingRemote;

	// Token: 0x04000B1F RID: 2847
	public GameObject purchasingStart;

	// Token: 0x04000B20 RID: 2848
	public GameObject purchaseInProgress;

	// Token: 0x04000B21 RID: 2849
	public GameObject purchasingSuccess;

	// Token: 0x04000B22 RID: 2850
	public GameObject purchasingFailure;

	// Token: 0x04000B23 RID: 2851
	public GameObject popupScreen;

	// Token: 0x04000B24 RID: 2852
	public Transform uiCenter;

	// Token: 0x04000B25 RID: 2853
	[Header("Purchasing Pages")]
	public TextMeshProUGUI shinyRockInfo;

	// Token: 0x04000B26 RID: 2854
	public TextMeshProUGUI currentResourceCountsLocal;

	// Token: 0x04000B27 RID: 2855
	public TextMeshProUGUI currentResourceCountsRemote;

	// Token: 0x04000B28 RID: 2856
	public TextMeshProUGUI failureReasonText;

	// Token: 0x04000B29 RID: 2857
	public const string failureFull = "YOU ARE ALREADY AT MAX RESOURCES! DONATE YOUR SHINY ROCKS TO A GOOD CAUSE INSTEAD OF US, KNUCKLEHEAD!";

	// Token: 0x04000B2A RID: 2858
	public const string failureNotEnoughRocks = "NOT ENOUGH SHINY ROCKS! PLEASE TRY AGAIN LATER, OR PURCHASE MORE SHINY ROCKS!";

	// Token: 0x04000B2B RID: 2859
	public const string failureUnknown = "UHHHHH SOMETHING WENT WRONG, I'M NOT SURE WHAT, SORRY TRY AGAIN LATER MAYBE!";

	// Token: 0x04000B2C RID: 2860
	private SIResourceCollection.FailReason failureReason;

	// Token: 0x04000B2D RID: 2861
	public Image background;

	// Token: 0x04000B2E RID: 2862
	public Color active;

	// Token: 0x04000B2F RID: 2863
	public Color notActive;

	// Token: 0x04000B30 RID: 2864
	public TextMeshProUGUI currentResourcesResourceCounts;

	// Token: 0x04000B31 RID: 2865
	private Dictionary<SIResourceCollection.ResourceCollectorTerminalState, GameObject> screenData;

	// Token: 0x04000B32 RID: 2866
	private bool initialized;

	// Token: 0x04000B33 RID: 2867
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x04000B34 RID: 2868
	[Tooltip("Button colliders to disable while popup screen is shown.")]
	[SerializeField]
	private List<Collider> _nonPopupButtonColliders;

	// Token: 0x02000157 RID: 343
	public enum FailReason
	{
		// Token: 0x04000B36 RID: 2870
		NotEnoughRocks,
		// Token: 0x04000B37 RID: 2871
		ResourcesFull,
		// Token: 0x04000B38 RID: 2872
		Unknown
	}

	// Token: 0x02000158 RID: 344
	public enum ResourceCollectorTerminalState
	{
		// Token: 0x04000B3A RID: 2874
		WaitingForScan,
		// Token: 0x04000B3B RID: 2875
		CurrentResources,
		// Token: 0x04000B3C RID: 2876
		HelpScreen,
		// Token: 0x04000B3D RID: 2877
		PurchaseRemote,
		// Token: 0x04000B3E RID: 2878
		PurchaseStart,
		// Token: 0x04000B3F RID: 2879
		PurchaseInProgress,
		// Token: 0x04000B40 RID: 2880
		PurchaseSuccess,
		// Token: 0x04000B41 RID: 2881
		PurchaseFailure
	}
}
