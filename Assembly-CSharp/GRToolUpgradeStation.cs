using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000816 RID: 2070
public class GRToolUpgradeStation : MonoBehaviour
{
	// Token: 0x06003525 RID: 13605 RVA: 0x00126233 File Offset: 0x00124433
	public void Init(GRToolProgressionManager tree, GhostReactor reactor)
	{
		this._reactor = reactor;
		this.defaultCostText = this.CostText.text;
		this.toolProgressionManager = tree;
		this.toolProgressionManager.OnProgressionUpdated += this.ResearchTreeUpdated;
		this.ResetScreen();
	}

	// Token: 0x170004AA RID: 1194
	// (get) Token: 0x06003526 RID: 13606 RVA: 0x00126271 File Offset: 0x00124471
	public bool canInsertTool
	{
		get
		{
			return this.currentState == GRToolUpgradeStation.UpgradeStationState.Idle && !this.bIsToolInserted;
		}
	}

	// Token: 0x06003527 RID: 13607 RVA: 0x00126286 File Offset: 0x00124486
	public void ResearchTreeUpdated()
	{
		this.UpdateUI();
	}

	// Token: 0x06003528 RID: 13608 RVA: 0x0012628E File Offset: 0x0012448E
	public void Update()
	{
		if (this.currentState == GRToolUpgradeStation.UpgradeStationState.Upgrading)
		{
			this.UpgradingUpdate(PhotonNetwork.Time);
		}
	}

	// Token: 0x06003529 RID: 13609 RVA: 0x001262A4 File Offset: 0x001244A4
	public void ToolInserted(GRTool tool)
	{
		if (!this.canInsertTool)
		{
			return;
		}
		this.bIsToolInserted = true;
		this.insertedTool = tool;
		this.insertedToolType = this.insertedTool.toolType;
		this.selectedToolUpgrades = this.toolProgressionManager.GetToolUpgrades(this.insertedToolType);
		this.ResetScreen();
		this.UpdateUI();
		this.SelectUpgrade(0);
		this.LocalPlacedToolInUpgradeStation(tool.gameEntity.id);
	}

	// Token: 0x0600352A RID: 13610 RVA: 0x00126314 File Offset: 0x00124514
	public void UpdateUI()
	{
		this.UpdateUpgradeTexts();
		this.UpdateSelectedUpgrade();
	}

	// Token: 0x0600352B RID: 13611 RVA: 0x00126324 File Offset: 0x00124524
	public void UpdateUpgradeTexts()
	{
		this.ToolNameText.text = GRUtils.GetToolName(this.insertedToolType);
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (this.selectedToolUpgrades.Count > i)
			{
				this.UpgradeTitlesText[i].text = this.selectedToolUpgrades[i].partMetadata.name;
			}
			else
			{
				this.UpgradeTitlesText[i].text = null;
			}
		}
	}

	// Token: 0x0600352C RID: 13612 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void UnlockAllUpgrades()
	{
	}

	// Token: 0x0600352D RID: 13613 RVA: 0x0012639C File Offset: 0x0012459C
	public void UpdateSelectedUpgrade()
	{
		if (this.selectedToolUpgrades != null && this.selectedToolUpgrades.Count > this.selectedUpgradeIndex && this.selectedToolUpgrades[this.selectedUpgradeIndex] != null)
		{
			if (this.selectedToolUpgrades[this.selectedUpgradeIndex].unlocked)
			{
				this.DescriptionText.text = this.selectedToolUpgrades[this.selectedUpgradeIndex].partMetadata.description;
				int researchCost = this.selectedToolUpgrades[this.selectedUpgradeIndex].researchCost;
				this.CostText.text = string.Format(this.defaultCostText, researchCost.ToString());
				GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
				this.CostText.color = ((researchCost > grplayer.ShiftCredits) ? this.lockedColor : this.unlockedColor);
				return;
			}
			this.CostText.text = "NEEDS RESEARCH";
			this.CostText.color = this.lockedColor;
		}
	}

	// Token: 0x0600352E RID: 13614 RVA: 0x001264A4 File Offset: 0x001246A4
	public void ResetScreen()
	{
		this.DescriptionText.text = "PLEASE INSERT A TOOL";
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			this.UpgradeTitlesText[i].text = "----";
			this.UpgradeTitlesText[i].color = this.lockedColor;
			this.MFD_ButtonTexts[i].color = this.unSelectedColor;
		}
		this.ToolNameText.text = "----";
		this.CostText.text = "-";
		this.ToolNameText.color = this.unSelectedColor;
		this.DescriptionText.color = this.unSelectedColor;
		this.CostText.color = this.unSelectedColor;
	}

	// Token: 0x0600352F RID: 13615 RVA: 0x00126560 File Offset: 0x00124760
	public void SelectUpgrade(int index)
	{
		if (index >= this.selectedToolUpgrades.Count)
		{
			return;
		}
		this.selectedUpgradeIndex = index;
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (i < this.selectedToolUpgrades.Count)
			{
				bool unlocked = this.selectedToolUpgrades[i].unlocked;
				this.UpgradeTitlesText[i].color = (unlocked ? this.unlockedColor : this.lockedColor);
				this.UpgradeLockedImage[i].gameObject.SetActive(!unlocked);
			}
			else
			{
				this.UpgradeLockedImage[i].gameObject.SetActive(true);
				this.UpgradeTitlesText[i].color = this.lockedColor;
			}
			this.UpgradeButtons[i].isOn = false;
			this.UpgradeButtons[i].UpdateColor();
		}
		if (this.selectedToolUpgrades != null && this.selectedToolUpgrades.Count > this.selectedUpgradeIndex && this.selectedToolUpgrades[this.selectedUpgradeIndex] != null)
		{
			this.UpgradeButtons[this.selectedUpgradeIndex].isOn = true;
			this.UpgradeButtons[this.selectedUpgradeIndex].UpdateColor();
			this.DescriptionText.color = this.UpgradeTitlesText[this.selectedUpgradeIndex].color;
			this.CostText.color = this.UpgradeTitlesText[this.selectedUpgradeIndex].color;
		}
		this.UpdateUI();
	}

	// Token: 0x06003530 RID: 13616 RVA: 0x001266C9 File Offset: 0x001248C9
	public void UpgradeTool()
	{
		this._reactor.grManager.ToolUpgradeStationRequestUpgrade(this.selectedToolUpgrades[this.selectedUpgradeIndex].type, this.insertedToolEntity.GetNetId());
	}

	// Token: 0x06003531 RID: 13617 RVA: 0x001266FC File Offset: 0x001248FC
	public void LocalPlacedToolInUpgradeStation(GameEntityId entityId)
	{
		GameEntity gameEntity = this._reactor.grManager.gameEntityManager.GetGameEntity(entityId);
		this.currentState = GRToolUpgradeStation.UpgradeStationState.ItemInserted;
		if (gameEntity.heldByActorNumber >= 0)
		{
			GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
			int handIndex = gamePlayer.FindHandIndex(entityId);
			gamePlayer.ClearGrabbedIfHeld(entityId, gameEntity.manager);
			if (gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				GamePlayerLocal.instance.gamePlayer.ClearGrabbed(handIndex);
				GamePlayerLocal.instance.ClearGrabbed(handIndex);
			}
			gameEntity.heldByActorNumber = -1;
			gameEntity.heldByHandIndex = -1;
			Action onReleased = gameEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased();
			}
			this.PositionInsertedTool(gameEntity);
			this.SelectUpgrade(0);
		}
	}

	// Token: 0x06003532 RID: 13618 RVA: 0x001267B0 File Offset: 0x001249B0
	public void PositionInsertedTool(GameEntity entity)
	{
		this.insertedToolEntity = entity;
		entity.transform.SetParent(this.startingLocation);
		entity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		Rigidbody component = entity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.startingLocation.position;
			component.rotation = this.startingLocation.rotation;
			component.linearVelocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
		entity.pickupable = false;
	}

	// Token: 0x06003533 RID: 13619 RVA: 0x00126840 File Offset: 0x00124A40
	public void PayForUpgrade(int Player)
	{
		if (Player == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			int researchCost = this.selectedToolUpgrades[this.selectedUpgradeIndex].researchCost;
			GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
			bool flag = researchCost <= grplayer.ShiftCredits;
			bool unlocked = this.selectedToolUpgrades[this.selectedUpgradeIndex].unlocked;
			if (flag && unlocked)
			{
				UnityEvent onSucceeded = this.IDCardScanner.onSucceeded;
				if (onSucceeded != null)
				{
					onSucceeded.Invoke();
				}
				this.StartUpgrade(PhotonNetwork.Time);
			}
		}
	}

	// Token: 0x06003534 RID: 13620 RVA: 0x001268C4 File Offset: 0x00124AC4
	public void StartUpgrade(double startTime)
	{
		if (this.currentState != GRToolUpgradeStation.UpgradeStationState.ItemInserted)
		{
			return;
		}
		this.upgradeStartTime = startTime;
		this.insertedToolEntity.transform.SetParent(this.startingLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Upgrading;
	}

	// Token: 0x06003535 RID: 13621 RVA: 0x00126919 File Offset: 0x00124B19
	public void UpgradingUpdate(double currentTime)
	{
		if (currentTime >= this.upgradeStartTime + this.upgradeAnimationLength)
		{
			this.CompleteUpgrade();
		}
	}

	// Token: 0x06003536 RID: 13622 RVA: 0x00126931 File Offset: 0x00124B31
	public void CompleteUpgrade()
	{
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Complete;
		this.ResetScreen();
		this.MoveToolToFinished();
	}

	// Token: 0x06003537 RID: 13623 RVA: 0x00126948 File Offset: 0x00124B48
	public void MoveItemToUpgradeSlot()
	{
		this.insertedToolEntity.transform.SetParent(this.upgradingLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.upgradingLocation.position;
			component.rotation = this.upgradingLocation.rotation;
			component.linearVelocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = false;
	}

	// Token: 0x06003538 RID: 13624 RVA: 0x001269E8 File Offset: 0x00124BE8
	public void MoveToolToFinished()
	{
		this.insertedToolEntity.transform.SetParent(this.depositedLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Complete;
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.startingLocation.position;
			component.rotation = this.startingLocation.rotation;
			component.linearVelocity = this.ejectionTransform.forward * this.ejectionVelocity;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = true;
		this.UpgradeTool();
		this.EjectToolFromEnd();
		this.ResetScreen();
	}

	// Token: 0x06003539 RID: 13625 RVA: 0x00126AB0 File Offset: 0x00124CB0
	public void EjectToolFromStart()
	{
		this.insertedToolEntity.transform.SetParent(this.startingLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.insertedToolEntity.transform.SetParent(null, true);
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.startingLocation.position;
			component.rotation = this.startingLocation.rotation;
			component.linearVelocity = this.ejectionTransform.forward * this.ejectionVelocity;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = true;
		this.insertedToolEntity = null;
		this.insertedTool = null;
		this.insertedToolType = GRTool.GRToolType.None;
		this.bIsToolInserted = false;
		this.ResetScreen();
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Idle;
	}

	// Token: 0x0600353A RID: 13626 RVA: 0x00126B9C File Offset: 0x00124D9C
	public void EjectToolFromEnd()
	{
		this.insertedToolEntity.transform.SetParent(this.depositedLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.insertedToolEntity.transform.SetParent(null, true);
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.depositedLocation.position;
			component.rotation = this.depositedLocation.rotation;
			component.linearVelocity = this.ejectionTransform.forward * this.ejectionVelocity;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = true;
		this.insertedToolEntity = null;
		this.insertedTool = null;
		this.insertedToolType = GRTool.GRToolType.None;
		this.bIsToolInserted = false;
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Idle;
	}

	// Token: 0x040045A0 RID: 17824
	private GRTool insertedTool;

	// Token: 0x040045A1 RID: 17825
	private GRTool.GRToolType insertedToolType;

	// Token: 0x040045A2 RID: 17826
	private GameEntity insertedToolEntity;

	// Token: 0x040045A3 RID: 17827
	[NonSerialized]
	private GhostReactor _reactor;

	// Token: 0x040045A4 RID: 17828
	[NonSerialized]
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x040045A5 RID: 17829
	[NonSerialized]
	private List<GRToolProgressionTree.GRToolProgressionNode> selectedToolUpgrades = new List<GRToolProgressionTree.GRToolProgressionNode>();

	// Token: 0x040045A6 RID: 17830
	[NonSerialized]
	public bool bIsToolInserted;

	// Token: 0x040045A7 RID: 17831
	public Transform startingLocation;

	// Token: 0x040045A8 RID: 17832
	public Transform upgradingLocation;

	// Token: 0x040045A9 RID: 17833
	public Transform depositedLocation;

	// Token: 0x040045AA RID: 17834
	public Transform ejectionTransform;

	// Token: 0x040045AB RID: 17835
	public float ejectionVelocity;

	// Token: 0x040045AC RID: 17836
	public Color selectedColor;

	// Token: 0x040045AD RID: 17837
	public Color unSelectedColor;

	// Token: 0x040045AE RID: 17838
	public Color lockedColor;

	// Token: 0x040045AF RID: 17839
	public Color unlockedColor;

	// Token: 0x040045B0 RID: 17840
	public TMP_Text[] UpgradeTitlesText;

	// Token: 0x040045B1 RID: 17841
	public TMP_Text[] MFD_ButtonTexts;

	// Token: 0x040045B2 RID: 17842
	public GorillaPressableButton[] UpgradeButtons;

	// Token: 0x040045B3 RID: 17843
	public Image[] UpgradeLockedImage;

	// Token: 0x040045B4 RID: 17844
	public TMP_Text ToolNameText;

	// Token: 0x040045B5 RID: 17845
	public TMP_Text DescriptionText;

	// Token: 0x040045B6 RID: 17846
	public TMP_Text CostText;

	// Token: 0x040045B7 RID: 17847
	private string defaultCostText;

	// Token: 0x040045B8 RID: 17848
	public IDCardScanner IDCardScanner;

	// Token: 0x040045B9 RID: 17849
	private int selectedUpgradeIndex;

	// Token: 0x040045BA RID: 17850
	private double upgradeStartTime;

	// Token: 0x040045BB RID: 17851
	public double upgradeAnimationLength;

	// Token: 0x040045BC RID: 17852
	public Vector3 rotationAnimation;

	// Token: 0x040045BD RID: 17853
	private GRToolUpgradeStation.UpgradeStationState currentState;

	// Token: 0x040045BE RID: 17854
	public GameEntity attachedItem;

	// Token: 0x02000817 RID: 2071
	private enum UpgradeStationState
	{
		// Token: 0x040045C0 RID: 17856
		Idle,
		// Token: 0x040045C1 RID: 17857
		ItemInserted,
		// Token: 0x040045C2 RID: 17858
		Upgrading,
		// Token: 0x040045C3 RID: 17859
		Complete
	}
}
