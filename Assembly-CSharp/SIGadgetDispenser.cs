using System;
using System.Collections.Generic;
using System.IO;
using GorillaTag;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x0200013E RID: 318
public class SIGadgetDispenser : MonoBehaviour, ITouchScreenStation
{
	// Token: 0x1700009A RID: 154
	// (get) Token: 0x060007E1 RID: 2017 RVA: 0x0002B056 File Offset: 0x00029256
	internal bool isTryOn
	{
		get
		{
			return this.m_isTryOn;
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x060007E2 RID: 2018 RVA: 0x0002B05E File Offset: 0x0002925E
	public SIScreenRegion ScreenRegion
	{
		get
		{
			return this.screenRegion;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x060007E3 RID: 2019 RVA: 0x0002B066 File Offset: 0x00029266
	public SIPlayer ActivePlayer
	{
		get
		{
			return this.parentTerminal.activePlayer;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x060007E4 RID: 2020 RVA: 0x0002B073 File Offset: 0x00029273
	public string ActivePlayerName
	{
		get
		{
			return this.ActivePlayer.gamePlayer.rig.Creator.SanitizedNickName;
		}
	}

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x060007E5 RID: 2021 RVA: 0x0002B08F File Offset: 0x0002928F
	public bool IsAuthority
	{
		get
		{
			return this.parentTerminal.superInfection.siManager.gameEntityManager.IsAuthority();
		}
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x060007E6 RID: 2022 RVA: 0x0002B0AB File Offset: 0x000292AB
	public SuperInfectionManager SIManager
	{
		get
		{
			return this.parentTerminal.superInfection.siManager;
		}
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x060007E7 RID: 2023 RVA: 0x0002B0BD File Offset: 0x000292BD
	public GameEntityManager GameEntityManager
	{
		get
		{
			return this.parentTerminal.superInfection.siManager.gameEntityManager;
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x060007E8 RID: 2024 RVA: 0x0002B0D4 File Offset: 0x000292D4
	public SITechTreeNode CurrentNode
	{
		get
		{
			return this.parentTerminal.superInfection.techTreeSO.GetTreeNode(this.parentTerminal.ActivePage, this._currentNode);
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x060007E9 RID: 2025 RVA: 0x0002B0FC File Offset: 0x000292FC
	public SITechTreePage CurrentPage
	{
		get
		{
			return this.parentTerminal.superInfection.techTreeSO.GetTreePage((SITechTreePageId)this.parentTerminal.ActivePage);
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x060007EA RID: 2026 RVA: 0x0002B11E File Offset: 0x0002931E
	public SITechTreeSO TechTreeSO
	{
		get
		{
			return this.parentTerminal.superInfection.techTreeSO;
		}
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x0002B130 File Offset: 0x00029330
	protected void OnEnable()
	{
		if (this.m_isTryOn)
		{
			SIGadgetDispenser.g_tryOnOptions = this.m_tryOnOptions;
		}
		this._RefreshButtonsUsableState();
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0002B14C File Offset: 0x0002934C
	private void _RefreshButtonsUsableState()
	{
		foreach (SIGadgetListEntry sigadgetListEntry in this.gadgetPages)
		{
			SITechTreePageId id = (SITechTreePageId)sigadgetListEntry.Id;
			SITechTreePage sitechTreePage;
			if (this.TechTreeSO.TryGetTreePage(id, out sitechTreePage))
			{
				sigadgetListEntry.ButtonContainer.SetUsable(sitechTreePage.IsAllowed);
			}
		}
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0002B1C0 File Offset: 0x000293C0
	private void SetNonPopupButtonsEnabled(bool enable)
	{
		foreach (Collider collider in this._nonPopupButtonColliders)
		{
			collider.enabled = enable;
		}
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x0002B214 File Offset: 0x00029414
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
		this.screenData = new Dictionary<SIGadgetDispenser.GadgetDispenserTerminalState, GameObject>();
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan, this.waitingForScanScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetType, this.gadgetTypeScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList, this.gadgetListScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetInformation, this.gadgetInformationScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed, this.gadgetDispensedScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen, this.gadgetsHelpScreen);
		this.parentTerminal.superInfection.techTreeSO.EnsureInitialized();
		int num = 0;
		int count = this.parentTerminal.superInfection.techTreeSO.TreePages.Count;
		for (int i = 0; i < count; i++)
		{
			SITechTreePage sitechTreePage = this.parentTerminal.superInfection.techTreeSO.TreePages[i];
			SIGadgetListEntry sigadgetListEntry = Object.Instantiate<SIGadgetListEntry>(this.pageListEntryPrefab, this.pageListParent);
			StaticLodManager.TryAddLateInstantiatedMembers(sigadgetListEntry.gameObject);
			sigadgetListEntry.Configure(this, sitechTreePage, this.parentTerminal.zeroZeroImage, this.parentTerminal.onePointTwoText, SITouchscreenButton.SITouchscreenButtonType.Select, i, -0.07f, count);
			this.gadgetPages.Add(sigadgetListEntry);
			num = Math.Max(num, sitechTreePage.DispensableGadgets.Count);
		}
		this.gadgetEntries = new List<SIDispenserGadgetListEntry>();
		for (int j = 0; j < num; j++)
		{
			SIDispenserGadgetListEntry sidispenserGadgetListEntry = Object.Instantiate<SIDispenserGadgetListEntry>(this.gadgetListEntryPrefab, this.gadgetListParent);
			sidispenserGadgetListEntry.transform.localPosition += new Vector3(0f, (float)j * -0.07f, 0f);
			sidispenserGadgetListEntry.SetStation(this, this.parentTerminal.zeroZeroImage, this.parentTerminal.onePointTwoText);
			this.gadgetEntries.Add(sidispenserGadgetListEntry);
		}
		if (this.m_isTryOn && base.isActiveAndEnabled)
		{
			SIGadgetDispenser.g_tryOnOptions = this.m_tryOnOptions;
		}
		this._RefreshButtonsUsableState();
		this.Reset();
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0002B437 File Offset: 0x00029637
	public void Reset()
	{
		this.currentState = SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan;
		this.SetScreenVisibility(this.currentState, this.currentState);
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0002B454 File Offset: 0x00029654
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy)
		{
			this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan, SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
		}
		stream.SendNext(this.helpScreenIndex);
		stream.SendNext(this._currentNode);
		stream.SendNext((int)this.currentState);
		stream.SendNext((int)this.lastState);
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0002B4D0 File Offset: 0x000296D0
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.helpScreenIndex = Mathf.Clamp((int)stream.ReceiveNext(), 0, this.helpPopupScreens.Length - 1);
		this._currentNode = (int)stream.ReceiveNext();
		if (this.CurrentNode == null && this.CurrentPage != null && this.CurrentPage.AllNodes.Count > 0 && this.CurrentPage.AllNodes[0].Value != null)
		{
			this._currentNode = (int)this.CurrentPage.AllNodes[0].Value.upgradeType;
		}
		SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState = (SIGadgetDispenser.GadgetDispenserTerminalState)stream.ReceiveNext();
		SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState2 = (SIGadgetDispenser.GadgetDispenserTerminalState)stream.ReceiveNext();
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SIGadgetDispenser.GadgetDispenserTerminalState), gadgetDispenserTerminalState) || !Enum.IsDefined(typeof(SIGadgetDispenser.GadgetDispenserTerminalState), gadgetDispenserTerminalState2))
		{
			this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan, SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(gadgetDispenserTerminalState, gadgetDispenserTerminalState2);
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x0002B5E2 File Offset: 0x000297E2
	public void ZoneDataSerializeWrite(BinaryWriter writer)
	{
		writer.Write(this.helpScreenIndex);
		writer.Write(this._currentNode);
		writer.Write((int)this.currentState);
		writer.Write((int)this.lastState);
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x0002B614 File Offset: 0x00029814
	public void ZoneDataSerializeRead(BinaryReader reader)
	{
		this.helpScreenIndex = Mathf.Clamp(reader.ReadInt32(), 0, this.helpPopupScreens.Length - 1);
		int value = reader.ReadInt32();
		if (this.CurrentPage != null && this.CurrentPage.AllNodes != null)
		{
			this._currentNode = Mathf.Clamp(value, 0, this.CurrentPage.AllNodes.Count - 1);
		}
		else
		{
			this._currentNode = 0;
		}
		SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState = (SIGadgetDispenser.GadgetDispenserTerminalState)reader.ReadInt32();
		SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState2 = (SIGadgetDispenser.GadgetDispenserTerminalState)reader.ReadInt32();
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SIGadgetDispenser.GadgetDispenserTerminalState), gadgetDispenserTerminalState) || !Enum.IsDefined(typeof(SIGadgetDispenser.GadgetDispenserTerminalState), gadgetDispenserTerminalState2))
		{
			this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan, SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(gadgetDispenserTerminalState, gadgetDispenserTerminalState2);
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x0002B6EE File Offset: 0x000298EE
	public void UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState newState, SIGadgetDispenser.GadgetDispenserTerminalState newLastState)
	{
		if (!this.IsPopupState(newLastState))
		{
			this.currentState = newLastState;
		}
		this.UpdateState(newState);
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0002B708 File Offset: 0x00029908
	public void UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState newState)
	{
		if (!this.IsPopupState(this.currentState))
		{
			this.lastState = this.currentState;
		}
		this.currentState = newState;
		this.SetScreenVisibility(this.currentState, this.lastState);
		switch (this.currentState)
		{
		case SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan:
			break;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetType:
			this.screenDescription.text = "GADGET TYPES";
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList:
			this.screenDescription.text = "UNLOCKED " + this.CurrentPage.nickName + " GADGETS";
			this.UpdateGadgetListVisibility();
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetInformation:
			this.screenDescription.text = this.CurrentNode.nickName;
			this.gadgetDescriptionText.text = this.CurrentNode.description;
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed:
			this.gadgetDispensedText.text = this.ActivePlayerName + " HAS DISPENSED A " + this.CurrentNode.nickName + "!";
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen:
			this.UpdateHelpButtonPage(this.helpScreenIndex);
			break;
		default:
			return;
		}
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0002B810 File Offset: 0x00029A10
	public void SetScreenVisibility(SIGadgetDispenser.GadgetDispenserTerminalState currentState, SIGadgetDispenser.GadgetDispenserTerminalState lastState)
	{
		bool flag = this.IsPopupState(currentState);
		this.background.color = ((currentState == SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan) ? Color.white : ((this.ActivePlayer != null && this.ActivePlayer.gamePlayer.IsLocal()) ? this.active : this.notActive));
		foreach (SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState in this.screenData.Keys)
		{
			bool flag2 = gadgetDispenserTerminalState == currentState || (flag && gadgetDispenserTerminalState == lastState);
			if (this.screenData[gadgetDispenserTerminalState].activeSelf != flag2)
			{
				this.screenData[gadgetDispenserTerminalState].SetActive(flag2);
			}
		}
		if (this.popupScreen.activeSelf != flag)
		{
			this.popupScreen.SetActive(flag);
		}
		this.screenDescription.gameObject.SetActive(currentState > SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
		this.SetNonPopupButtonsEnabled(!flag);
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0002B91C File Offset: 0x00029B1C
	public void UpdateGadgetListVisibility()
	{
		foreach (SIDispenserGadgetListEntry sidispenserGadgetListEntry in this.gadgetEntries)
		{
			sidispenserGadgetListEntry.gameObject.SetActive(false);
		}
		int num = 0;
		foreach (SITechTreeNode sitechTreeNode in this.CurrentPage.DispensableGadgets)
		{
			if (this.m_isTryOn || this.ActivePlayer.CurrentProgression.IsUnlocked(sitechTreeNode.upgradeType))
			{
				SIDispenserGadgetListEntry sidispenserGadgetListEntry2 = this.gadgetEntries[num++];
				sidispenserGadgetListEntry2.SetTechTreeNode(sitechTreeNode);
				sidispenserGadgetListEntry2.gameObject.SetActive(true);
				sidispenserGadgetListEntry2.DispenseButton.SetUsable(this.m_isTryOn || sitechTreeNode.IsAllowed);
			}
		}
		this.noDispensableGadgetsMessage.SetActive(num == 0);
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0002BA28 File Offset: 0x00029C28
	public bool IsPopupState(SIGadgetDispenser.GadgetDispenserTerminalState state)
	{
		return state == SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed || state == SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen;
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0002BA34 File Offset: 0x00029C34
	public void PlayerHandScanned(int actorNr)
	{
		this.UpdateState(this.handScannedState);
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0002BA42 File Offset: 0x00029C42
	public void AddButton(SITouchscreenButton button, bool isPopupButton = false)
	{
		if (!isPopupButton)
		{
			this._nonPopupButtonColliders.Add(button.GetComponent<Collider>());
		}
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0002BA58 File Offset: 0x00029C58
	public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
	{
		if (actorNr == SIPlayer.LocalPlayer.ActorNr && (this.ActivePlayer == null || this.ActivePlayer != SIPlayer.LocalPlayer))
		{
			this.parentTerminal.PlayWrongPlayerBuzz(this.uiCenter);
		}
		else
		{
			this.touchSoundBankPlayer.Play();
		}
		if (!this.IsAuthority)
		{
			this.parentTerminal.TouchscreenButtonPressed(buttonType, data, actorNr, SICombinedTerminal.TerminalSubFunction.GadgetDispenser);
			return;
		}
		if (actorNr != this.ActivePlayer.ActorNr)
		{
			return;
		}
		this.touchSoundBankPlayer.Play();
		switch (this.currentState)
		{
		case SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen);
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetType:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Select)
			{
				this.parentTerminal.SetActivePage(data);
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen);
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetType);
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Select)
			{
				SITechTreeNode treeNode = this.TechTreeSO.GetTreeNode((int)this.CurrentPage.pageId, data);
				if (treeNode != null && treeNode.IsDispensableGadget)
				{
					this._currentNode = data;
					this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetInformation);
				}
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Dispense)
			{
				SITechTreeNode treeNode2 = this.TechTreeSO.GetTreeNode((int)this.CurrentPage.pageId, data);
				if (treeNode2 != null && treeNode2.IsDispensableGadget)
				{
					this._currentNode = data;
					this.AuthorityDispenseGadgetForPlayer(this.ActivePlayer);
					this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed);
				}
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetInformation:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen);
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList);
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Dispense)
			{
				this.AuthorityDispenseGadgetForPlayer(this.ActivePlayer);
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed);
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Exit)
			{
				this.UpdateState(this.lastState);
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Exit)
			{
				this.helpScreenIndex = 0;
				this.UpdateState(this.lastState);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Next)
			{
				this.helpScreenIndex = Mathf.Clamp(this.helpScreenIndex + 1, 0, this.helpPopupScreens.Length - 1);
				this.UpdateHelpButtonPage(this.helpScreenIndex);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.helpScreenIndex = Mathf.Clamp(this.helpScreenIndex - 1, 0, this.helpPopupScreens.Length - 1);
				this.UpdateHelpButtonPage(this.helpScreenIndex);
			}
			return;
		default:
			return;
		}
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void TouchscreenToggleButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, bool isToggledOn)
	{
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x0002BC68 File Offset: 0x00029E68
	public void UpdateHelpButtonPage(int helpButtonPageIndex)
	{
		for (int i = 0; i < this.helpPopupScreens.Length; i++)
		{
			this.helpPopupScreens[i].SetActive(i == helpButtonPageIndex);
		}
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0002BC9C File Offset: 0x00029E9C
	public void AuthorityDispenseGadgetForPlayer(SIPlayer player)
	{
		if (!this.IsAuthority)
		{
			return;
		}
		int num = 0;
		int staticHash = this.CurrentNode.unlockedGadgetPrefab.name.GetStaticHash();
		for (int i = player.activePlayerGadgets.Count - 1; i >= 0; i--)
		{
			GameEntity gameEntityFromNetId = this.GameEntityManager.GetGameEntityFromNetId(player.activePlayerGadgets[i]);
			if (gameEntityFromNetId == null)
			{
				player.activePlayerGadgets.RemoveAt(i);
			}
			else
			{
				num++;
				if (num >= player.TotalGadgetLimit)
				{
					this.GameEntityManager.RequestDestroyItem(gameEntityFromNetId.id);
					break;
				}
			}
		}
		SIUpgradeSet upgrades = player.GetUpgrades(this.CurrentPage.pageId);
		int num2 = 0;
		foreach (GraphNode<SITechTreeNode> graphNode in this.CurrentPage.AllNodes)
		{
			num2 |= 1 << graphNode.Value.upgradeType.GetNodeId();
		}
		upgrades.SetBits(this.m_isTryOn ? num2 : (upgrades.GetBits() & num2));
		foreach (SITechTreeNode sitechTreeNode in this.CurrentPage.DispensableGadgets)
		{
			if (sitechTreeNode != this.CurrentNode)
			{
				upgrades.Remove(sitechTreeNode.upgradeType);
			}
		}
		long num3 = upgrades.GetCreateData(player);
		if (this.m_isTryOn)
		{
			num3 |= long.MinValue;
		}
		this.GameEntityManager.RequestCreateItem(staticHash, this.gadgetDispensePosition.position, this.gadgetDispensePosition.rotation, num3);
		this.dispenseSoundBankPlayer.Play();
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0002BE74 File Offset: 0x0002A074
	public void SetActivePage()
	{
		if (this.CurrentNode == null)
		{
			this._currentNode = this.CurrentPage.AllNodes[0].Value.upgradeType.GetNodeId();
		}
		if (this.ActivePlayer != null)
		{
			this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList);
			return;
		}
		this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0002BECC File Offset: 0x0002A0CC
	public bool IsValidPage(int pageId)
	{
		if (pageId < 0)
		{
			return false;
		}
		using (List<SIGadgetListEntry>.Enumerator enumerator = this.gadgetPages.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Id == pageId)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x0000636B File Offset: 0x0000456B
	GameObject ITouchScreenStation.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x040009F8 RID: 2552
	public SIGadgetDispenser.GadgetDispenserTerminalState handScannedState = SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList;

	// Token: 0x040009F9 RID: 2553
	public SIGadgetDispenser.GadgetDispenserTerminalState currentState;

	// Token: 0x040009FA RID: 2554
	public SIGadgetDispenser.GadgetDispenserTerminalState lastState;

	// Token: 0x040009FB RID: 2555
	public Transform gadgetDispensePosition;

	// Token: 0x040009FC RID: 2556
	public int _currentNode;

	// Token: 0x040009FD RID: 2557
	public SICombinedTerminal parentTerminal;

	// Token: 0x040009FE RID: 2558
	[Header("TryOn")]
	[SerializeField]
	private bool m_isTryOn;

	// Token: 0x040009FF RID: 2559
	[SerializeField]
	private GameEntityDelayedDestroy.Options m_tryOnOptions = new GameEntityDelayedDestroy.Options
	{
		delay = 30f,
		explosionVolume = 1f,
		beepVolume = 1f,
		beepPhases = new GameEntityDelayedDestroy.BeepPhase[]
		{
			new GameEntityDelayedDestroy.BeepPhase
			{
				timeRemaining = 10f,
				interval = 1f
			},
			new GameEntityDelayedDestroy.BeepPhase
			{
				timeRemaining = 5f,
				interval = 0.5f
			},
			new GameEntityDelayedDestroy.BeepPhase
			{
				timeRemaining = 2f,
				interval = 0.25f
			}
		}
	};

	// Token: 0x04000A00 RID: 2560
	internal static GameEntityDelayedDestroy.Options g_tryOnOptions = new GameEntityDelayedDestroy.Options
	{
		delay = 1f
	};

	// Token: 0x04000A01 RID: 2561
	public GameObject waitingForScanScreen;

	// Token: 0x04000A02 RID: 2562
	public GameObject gadgetTypeScreen;

	// Token: 0x04000A03 RID: 2563
	public GameObject gadgetListScreen;

	// Token: 0x04000A04 RID: 2564
	public GameObject gadgetInformationScreen;

	// Token: 0x04000A05 RID: 2565
	public GameObject gadgetDispensedScreen;

	// Token: 0x04000A06 RID: 2566
	public GameObject gadgetsHelpScreen;

	// Token: 0x04000A07 RID: 2567
	[SerializeField]
	private SIScreenRegion screenRegion;

	// Token: 0x04000A08 RID: 2568
	[Header("Main Screen Shared")]
	public TextMeshProUGUI screenDescription;

	// Token: 0x04000A09 RID: 2569
	public Image background;

	// Token: 0x04000A0A RID: 2570
	public Color active;

	// Token: 0x04000A0B RID: 2571
	public Color notActive;

	// Token: 0x04000A0C RID: 2572
	public Transform uiCenter;

	// Token: 0x04000A0D RID: 2573
	[Header("Popup Shared")]
	public GameObject popupScreen;

	// Token: 0x04000A0E RID: 2574
	[Header("Gadgets Type")]
	[SerializeField]
	private RectTransform pageListParent;

	// Token: 0x04000A0F RID: 2575
	[SerializeField]
	private SIGadgetListEntry pageListEntryPrefab;

	// Token: 0x04000A10 RID: 2576
	private List<SIGadgetListEntry> gadgetPages = new List<SIGadgetListEntry>();

	// Token: 0x04000A11 RID: 2577
	[FormerlySerializedAs("noDispensableGadgetsNotif")]
	[Header("Gadgets List")]
	[SerializeField]
	private GameObject noDispensableGadgetsMessage;

	// Token: 0x04000A12 RID: 2578
	[SerializeField]
	private RectTransform gadgetListParent;

	// Token: 0x04000A13 RID: 2579
	[SerializeField]
	private SIDispenserGadgetListEntry gadgetListEntryPrefab;

	// Token: 0x04000A14 RID: 2580
	private List<SIDispenserGadgetListEntry> gadgetEntries;

	// Token: 0x04000A15 RID: 2581
	[Header("Gadgets Description")]
	public TextMeshProUGUI gadgetDescriptionText;

	// Token: 0x04000A16 RID: 2582
	[Header("Gadget Dispensed")]
	public TextMeshProUGUI gadgetDispensedText;

	// Token: 0x04000A17 RID: 2583
	[Header("Help")]
	public int helpScreenIndex;

	// Token: 0x04000A18 RID: 2584
	public GameObject[] helpPopupScreens;

	// Token: 0x04000A19 RID: 2585
	[Header("Audio")]
	[SerializeField]
	private SoundBankPlayer touchSoundBankPlayer;

	// Token: 0x04000A1A RID: 2586
	[SerializeField]
	private SoundBankPlayer dispenseSoundBankPlayer;

	// Token: 0x04000A1B RID: 2587
	[Header("Main Screen Colliders")]
	[Tooltip("Button colliders to disable while popup screen is shown.  Gets updated live to include page and gadget buttons.")]
	[SerializeField]
	private List<Collider> _nonPopupButtonColliders;

	// Token: 0x04000A1C RID: 2588
	private Dictionary<SIGadgetDispenser.GadgetDispenserTerminalState, GameObject> screenData;

	// Token: 0x04000A1D RID: 2589
	private bool initialized;

	// Token: 0x0200013F RID: 319
	public enum GadgetDispenserTerminalState
	{
		// Token: 0x04000A1F RID: 2591
		WaitingForScan,
		// Token: 0x04000A20 RID: 2592
		GadgetType,
		// Token: 0x04000A21 RID: 2593
		GadgetList,
		// Token: 0x04000A22 RID: 2594
		GadgetInformation,
		// Token: 0x04000A23 RID: 2595
		GadgetDispensed,
		// Token: 0x04000A24 RID: 2596
		HelpScreen
	}
}
