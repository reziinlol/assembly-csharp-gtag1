using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

// Token: 0x02000811 RID: 2065
public class GRToolUpgradePurchaseStationFull : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170004A7 RID: 1191
	// (get) Token: 0x060034EB RID: 13547 RVA: 0x00123B4C File Offset: 0x00121D4C
	public int SelectedShelf
	{
		get
		{
			return this.selectedShelf;
		}
	}

	// Token: 0x170004A8 RID: 1192
	// (get) Token: 0x060034EC RID: 13548 RVA: 0x00123B54 File Offset: 0x00121D54
	public int SelectedItem
	{
		get
		{
			return this.selectedItem;
		}
	}

	// Token: 0x170004A9 RID: 1193
	// (get) Token: 0x060034ED RID: 13549 RVA: 0x00123B5C File Offset: 0x00121D5C
	// (set) Token: 0x060034EE RID: 13550 RVA: 0x00123B64 File Offset: 0x00121D64
	public bool TickRunning { get; set; }

	// Token: 0x060034EF RID: 13551 RVA: 0x00019E3F File Offset: 0x0001803F
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060034F0 RID: 13552 RVA: 0x00019E47 File Offset: 0x00018047
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x060034F1 RID: 13553 RVA: 0x00123B70 File Offset: 0x00121D70
	public void Init(GRToolProgressionManager progression, GhostReactor reactor)
	{
		this.reactor = reactor;
		this.grManager = reactor.grManager;
		this.toolProgressionManager = progression;
		this.toolProgressionManager.OnProgressionUpdated += this.ProgressionUpdated;
		this.nextVisibleShelfIndex = -1;
		this.prefabMagnetHeightOffset = this.ropeTop.position.y;
		this.frontBackShelfMovement = new GRSpringMovement(0.5f, 0.7f);
		this.raiseLowerShelfMovement = new GRSpringMovement(1f, 0.7f);
		this.magnetMovement = new GRSpringMovement(1f, 0.7f);
		ProgressionManager.Instance.OnGetShiftCredit += this.OnShiftCreditChanged;
		this.needsUIRefresh = true;
		this.InitPageSelectionWheel();
		this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
		this.SetActivePlayer(-1);
	}

	// Token: 0x060034F2 RID: 13554 RVA: 0x00123C3B File Offset: 0x00121E3B
	public void OnShiftCreditChanged(string targetMothershipId, int newShiftCredits)
	{
		this.needsUIRefresh = true;
	}

	// Token: 0x060034F3 RID: 13555 RVA: 0x00123C44 File Offset: 0x00121E44
	public void HideOrShowTextBasedOnLocalPlayerDistance()
	{
		Vector3 position = GRPlayer.Get(VRRig.LocalRig).transform.position;
		Vector3 position2 = base.transform.position;
		float num = this.currentlyShowingText ? 8f : 6f;
		bool flag = (position - position2).sqrMagnitude < num * num;
		if (flag != this.currentlyShowingText)
		{
			this.shelfSelectionText.enabled = flag;
			this.playerInfo.enabled = flag;
			this.itemDescription.enabled = flag;
			this.itemDescriptionName.enabled = flag;
			this.itemDescriptionAnnotation.enabled = flag;
			this.purchaseButtonText.enabled = flag;
			this.pageSelectionWheel.ShowText(flag);
			for (int i = 0; i < this.gameShelves.Count; i++)
			{
				if (!(this.gameShelves[i] == null))
				{
					foreach (GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot in this.gameShelves[i].gRPurchaseSlots)
					{
						if (grpurchaseSlot.Name != null)
						{
							grpurchaseSlot.Name.enabled = flag;
						}
						if (grpurchaseSlot.Price != null)
						{
							grpurchaseSlot.Price.enabled = flag;
						}
					}
				}
			}
		}
		this.currentlyShowingText = flag;
	}

	// Token: 0x060034F4 RID: 13556 RVA: 0x00123DBC File Offset: 0x00121FBC
	public void Tick()
	{
		this.HideOrShowTextBasedOnLocalPlayerDistance();
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (this.toolProgressionManager == null)
		{
			return;
		}
		if (grplayer != null && (this.lastKnownLocalPlayerCredits != grplayer.ShiftCredits || this.lastKnownLocalPlayerJuice != this.toolProgressionManager.GetNumberOfResearchPoints()))
		{
			this.needsUIRefresh = true;
			this.lastKnownLocalPlayerCredits = grplayer.ShiftCredits;
			this.lastKnownLocalPlayerJuice = this.toolProgressionManager.GetNumberOfResearchPoints();
		}
		this.UpdateActivePlayer();
		this.UpdateSelectionLever();
		this.UpdateShelf();
		this.UpdateMagnet();
		if (this.disablePurchaseButton)
		{
			if (this.purchaseButtonPressed > 0f)
			{
				this.purchaseButtonPressed -= Time.deltaTime;
			}
			else
			{
				this.disablePurchaseButton = false;
			}
		}
		if (this.needsUIRefresh)
		{
			this.needsUIRefresh = false;
			this.UpdateShelfDisplayElements(this.currentVisibleShelfIndex);
			this.UpdateShelfDisplayElements(this.nextVisibleShelfIndex);
			this.UpdateShelfDisplayElements(this.selectedShelf);
			this.UpdatePlayerCurrencyUI();
			this.UpdatePurchaseButtonText();
		}
	}

	// Token: 0x060034F5 RID: 13557 RVA: 0x00123EC0 File Offset: 0x001220C0
	public void SetActivePlayer(int actorNum)
	{
		this.currentActivePlayerActorNumber = actorNum;
		this.needsUIRefresh = true;
		if (this.currentActivePlayerActorNumber == -1)
		{
			this.itemDescriptionName.text = "SWIPE FOR ACCESS";
			this.itemDescription.text = "Welcome to the Tool-o-matic v2 automated vending machine. Please swipe your ID card for access.";
			this.itemDescriptionAnnotation.text = "Remember: Compliance leads to success!";
			return;
		}
		if (this.IsValidShelfItemIndex(this.selectedShelf, this.selectedItem) && this.toolProgressionManager != null)
		{
			GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(this.gameShelves[this.selectedShelf].gRPurchaseSlots[this.selectedItem].PurchaseID);
			if (partMetadata != null)
			{
				this.itemDescriptionName.text = partMetadata.name;
				this.itemDescription.text = partMetadata.description;
				this.itemDescriptionAnnotation.text = partMetadata.annotation;
			}
			this.select1.SetButtonState(this.selectedItem == 0);
			this.select2.SetButtonState(this.selectedItem == 1);
			this.select3.SetButtonState(this.selectedItem == 2);
			this.select4.SetButtonState(this.selectedItem == 3);
		}
	}

	// Token: 0x060034F6 RID: 13558 RVA: 0x00123FF8 File Offset: 0x001221F8
	public void UpdateActivePlayer()
	{
		if (!this.grManager.IsAuthority())
		{
			return;
		}
		if (this.currentActivePlayerActorNumber != -1)
		{
			GRPlayer grplayer = GRPlayer.Get(this.currentActivePlayerActorNumber);
			if (grplayer != null)
			{
				BoxCollider component = base.GetComponent<BoxCollider>();
				Vector3 position = grplayer.transform.position;
				Vector3 vector = component.transform.worldToLocalMatrix.MultiplyPoint(position) - component.center;
				Vector3 vector2 = component.size * 0.5f;
				if (Mathf.Abs(vector.x) > vector2.x || Mathf.Abs(vector.y) > vector2.y || Mathf.Abs(vector.z) > vector2.z)
				{
					this.grManager.SetActivePlayerAuthority(this, -1);
					return;
				}
			}
			else
			{
				this.currentActivePlayerActorNumber = -1;
			}
		}
	}

	// Token: 0x060034F7 RID: 13559 RVA: 0x001240D0 File Offset: 0x001222D0
	private void UpdateShelf()
	{
		switch (this.shelfMovementState)
		{
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle:
			if (this.currentVisibleShelfIndex != this.selectedShelf)
			{
				this.SetNextShelf(this.selectedShelf);
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward);
				return;
			}
			this.SetNextShelf(-1);
			return;
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward:
		{
			if (this.currentVisibleShelfIndex == this.selectedShelf)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfForward);
				return;
			}
			this.frontBackShelfMovement.target = 1f;
			this.frontBackShelfMovement.Update();
			float pos = this.frontBackShelfMovement.pos;
			this.gameShelves[this.currentVisibleShelfIndex].transform.position = Vector3.Lerp(this.shelfRootTransform.position, this.shelfBackTransform.position, pos);
			this.UpdateSoundsForMovement(this.frontBackShelfMovement);
			if (this.frontBackShelfMovement.IsAtTarget())
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward);
				return;
			}
			break;
		}
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfForward:
		{
			if (this.currentVisibleShelfIndex != this.selectedShelf)
			{
				this.SetNextShelf(this.selectedShelf);
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward);
				return;
			}
			this.frontBackShelfMovement.target = 0f;
			this.frontBackShelfMovement.Update();
			float pos2 = this.frontBackShelfMovement.pos;
			this.gameShelves[this.currentVisibleShelfIndex].transform.position = Vector3.Lerp(this.shelfRootTransform.position, this.shelfBackTransform.position, pos2);
			this.UpdateSoundsForMovement(this.frontBackShelfMovement);
			if (this.frontBackShelfMovement.IsAtTarget())
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
				return;
			}
			break;
		}
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward:
		{
			if (this.nextVisibleShelfIndex == -1)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
				return;
			}
			if (this.nextVisibleShelfIndex != this.selectedShelf && this.raiseLowerShelfMovement.pos <= 0.5f)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfDownward);
				return;
			}
			this.raiseLowerShelfMovement.target = 1f;
			this.raiseLowerShelfMovement.Update();
			float pos3 = this.raiseLowerShelfMovement.pos;
			this.gameShelves[this.nextVisibleShelfIndex].transform.position = Vector3.Lerp(this.shelfLowerTransform.position, this.shelfRootTransform.position, pos3);
			this.UpdateSoundsForMovement(this.raiseLowerShelfMovement);
			if (this.raiseLowerShelfMovement.IsAtTarget())
			{
				this.SetCurrentShelf(this.nextVisibleShelfIndex);
				if (this.nextVisibleShelfIndex == this.selectedShelf)
				{
					this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
					return;
				}
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward);
				return;
			}
			break;
		}
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfDownward:
			if (this.nextVisibleShelfIndex == -1)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
				return;
			}
			if (this.nextVisibleShelfIndex != this.selectedShelf)
			{
				this.raiseLowerShelfMovement.target = 0f;
				this.raiseLowerShelfMovement.Update();
				float pos4 = this.raiseLowerShelfMovement.pos;
				this.gameShelves[this.nextVisibleShelfIndex].transform.position = Vector3.Lerp(this.shelfLowerTransform.position, this.shelfRootTransform.position, pos4);
				this.UpdateSoundsForMovement(this.raiseLowerShelfMovement);
				if (this.raiseLowerShelfMovement.IsAtTarget())
				{
					this.SetNextShelf(this.selectedShelf);
					this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward);
					return;
				}
			}
			else
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060034F8 RID: 13560 RVA: 0x00124404 File Offset: 0x00122604
	private void UpdateSoundsForMovement(GRSpringMovement movement)
	{
		if (movement.IsAtTarget())
		{
			this.audioSourceLooping.volume = 0f;
			if (movement.HitTargetLastUpdate())
			{
				this.audioSourceClang.Play();
				return;
			}
		}
		else
		{
			this.audioSourceLooping.volume = Mathf.Clamp01(Math.Abs(movement.speed) * this.audioSourceLoopingVolume);
		}
	}

	// Token: 0x060034F9 RID: 13561 RVA: 0x00124460 File Offset: 0x00122660
	public void SetCurrentShelf(int idx)
	{
		if (idx == -1)
		{
			return;
		}
		if (idx == this.currentVisibleShelfIndex)
		{
			return;
		}
		if (!this.IsValidShelfItemIndex(idx, 0))
		{
			return;
		}
		if (idx == this.nextVisibleShelfIndex)
		{
			this.SetNextShelf(-1);
		}
		this.UpdateShelfVisibility(this.currentVisibleShelfIndex, false);
		this.frontBackShelfMovement.Reset();
		this.gameShelves[idx].transform.position = this.shelfRootTransform.position;
		this.UpdateShelfVisibility(idx, true);
		this.currentVisibleShelfIndex = idx;
	}

	// Token: 0x060034FA RID: 13562 RVA: 0x001244E0 File Offset: 0x001226E0
	public void SetNextShelf(int idx)
	{
		if (idx == this.nextVisibleShelfIndex)
		{
			return;
		}
		if (idx == this.currentVisibleShelfIndex)
		{
			return;
		}
		if (this.nextVisibleShelfIndex != -1)
		{
			this.UpdateShelfVisibility(this.nextVisibleShelfIndex, false);
		}
		if (idx != -1)
		{
			this.raiseLowerShelfMovement.Reset();
			this.gameShelves[idx].transform.position = this.shelfLowerTransform.position;
			this.UpdateShelfVisibility(idx, true);
		}
		this.nextVisibleShelfIndex = idx;
	}

	// Token: 0x060034FB RID: 13563 RVA: 0x00124558 File Offset: 0x00122758
	public void ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState newState)
	{
		this.shelfMovementState = newState;
		switch (newState)
		{
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle:
			this.SetCurrentShelf(this.selectedShelf);
			this.SetNextShelf(-1);
			return;
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward:
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfForward:
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfDownward:
			this.audioSourceLooping.volume = 0f;
			this.audioSourceLooping.GTPlay();
			return;
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward:
			if (this.currentVisibleShelfIndex == this.selectedShelf)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfForward);
			}
			else
			{
				this.SetNextShelf(this.selectedShelf);
			}
			this.audioSourceLooping.volume = 0f;
			this.audioSourceLooping.GTPlay();
			return;
		default:
			return;
		}
	}

	// Token: 0x060034FC RID: 13564 RVA: 0x001245F5 File Offset: 0x001227F5
	public void UpdateShelfVisibility(int shelfID, bool isVisible)
	{
		if (!this.IsValidShelfItemIndex(shelfID, 0))
		{
			return;
		}
		this.gameShelves[shelfID].gameObject.SetActive(isVisible);
		if (isVisible)
		{
			this.UpdateShelfDisplayElements(shelfID);
		}
	}

	// Token: 0x060034FD RID: 13565 RVA: 0x00124624 File Offset: 0x00122824
	public void UpdateShelfDisplayElements(int shelfID)
	{
		if (!this.IsValidShelfItemIndex(shelfID, 0))
		{
			return;
		}
		GRToolUpgradePurchaseStationShelf grtoolUpgradePurchaseStationShelf = this.gameShelves[shelfID];
		for (int i = 0; i < grtoolUpgradePurchaseStationShelf.gRPurchaseSlots.Count; i++)
		{
			this.UpdateShelfItemDisplayElements(shelfID, i);
		}
	}

	// Token: 0x060034FE RID: 13566 RVA: 0x00124668 File Offset: 0x00122868
	public void UpdatePurchaseButtonText()
	{
		if (!this.IsValidShelfItemIndex(this.selectedShelf, this.selectedItem))
		{
			this.purchaseButtonText.text = "ERROR";
			return;
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[this.selectedShelf].gRPurchaseSlots[this.selectedItem];
		Color color = grpurchaseSlot.canAfford ? this.colorPurchaseButtonCanAfford : this.colorCantBuy;
		string purchaseText = grpurchaseSlot.purchaseText;
		if (color != this.purchaseButtonText.color)
		{
			this.purchaseButtonText.color = color;
		}
		if (purchaseText != this.purchaseButtonText.text)
		{
			this.purchaseButtonText.text = purchaseText;
		}
	}

	// Token: 0x060034FF RID: 13567 RVA: 0x00124718 File Offset: 0x00122918
	public void UpdateShelfItemDisplayElements(int shelf, int slotID)
	{
		if (!this.IsValidShelfItemIndex(shelf, slotID))
		{
			return;
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[shelf].gRPurchaseSlots[slotID];
		if (this.toolProgressionManager)
		{
			GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(grpurchaseSlot.PurchaseID);
			if (partMetadata == null)
			{
				grpurchaseSlot.Name.text = "ERROR";
				return;
			}
			string text = "ERROR";
			string text2 = "";
			Color white = Color.white;
			bool flag = true;
			int num = 10000;
			int num2;
			this.toolProgressionManager.GetPlayerShiftCredit(out num2);
			int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
			grpurchaseSlot.canAfford = false;
			grpurchaseSlot.purchaseText = "LOCKED";
			bool flag2;
			if (this.toolProgressionManager.IsPartUnlocked(grpurchaseSlot.PurchaseID, out flag2))
			{
				if (flag2)
				{
					this.gameShelves[shelf].SetMaterialOverride(slotID, null);
					if (this.toolProgressionManager.GetShiftCreditCost(grpurchaseSlot.PurchaseID, out num))
					{
						text = string.Format("⑭ {0}", num);
					}
					bool flag3 = num2 >= num;
					grpurchaseSlot.Name.text = partMetadata.name;
					grpurchaseSlot.Name.color = ((slotID == this.selectedItem) ? this.colorSelectedItem : this.colorUnselectedItem);
					grpurchaseSlot.Price.text = text;
					grpurchaseSlot.Price.color = (flag3 ? this.colorCanBuyCredits : this.colorCantBuy);
					grpurchaseSlot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
					grpurchaseSlot.canAfford = flag3;
					if (flag3)
					{
						grpurchaseSlot.purchaseText = string.Format("BUY FOR\n⑭ {0}", num);
					}
					else
					{
						grpurchaseSlot.purchaseText = string.Format("NEED\n⑭ {0}", num);
					}
				}
				else
				{
					this.gameShelves[shelf].SetMaterialOverride(slotID, this.unresearchedItemMaterial);
					grpurchaseSlot.Name.text = partMetadata.name;
					grpurchaseSlot.Name.color = ((slotID == this.selectedItem) ? this.colorUnresearchedItem : this.colorUnselectedUnresearchedItem);
					flag = true;
					GRToolProgressionTree.EmployeeLevelRequirement employeeLevelRequirement;
					if (this.toolProgressionManager.GetPartUnlockEmployeeRequiredLevel(grpurchaseSlot.PurchaseID, out employeeLevelRequirement) && this.toolProgressionManager.GetCurrentEmployeeLevel() < employeeLevelRequirement)
					{
						this.toolProgressionManager.GetEmployeeLevelDisplayName(employeeLevelRequirement);
						text2 += string.Format("⑱ {0}\n", employeeLevelRequirement);
						flag = false;
					}
					this.cachedRequiredPartsList.Clear();
					if (this.toolProgressionManager.GetPartUnlockRequiredParentParts(grpurchaseSlot.PurchaseID, out this.cachedRequiredPartsList))
					{
						foreach (GRToolProgressionManager.ToolParts part in this.cachedRequiredPartsList)
						{
							bool flag4 = false;
							GRToolProgressionManager.ToolProgressionMetaData partMetadata2 = this.toolProgressionManager.GetPartMetadata(part);
							if (partMetadata2 == null)
							{
								text2 += "⑱ ERROR\n";
								flag = false;
							}
							else if (!this.toolProgressionManager.IsPartUnlocked(part, out flag4) || !flag4)
							{
								text2 = text2 + "⑱ " + partMetadata2.name + "\n";
								flag = false;
							}
						}
					}
					if (!flag)
					{
						grpurchaseSlot.Price.text = text2;
						grpurchaseSlot.Price.color = this.colorCantBuy;
						grpurchaseSlot.Price.fontSize = ((text2.Length <= 8) ? 2.25f : 1.6f);
						grpurchaseSlot.canAfford = false;
						grpurchaseSlot.purchaseText = "LOCKED";
					}
					else
					{
						if (this.toolProgressionManager.GetPartUnlockJuiceCost(grpurchaseSlot.PurchaseID, out num))
						{
							text = string.Format("⑮ {0}", num);
						}
						bool flag3 = numberOfResearchPoints >= num;
						grpurchaseSlot.Price.text = text;
						grpurchaseSlot.Price.color = (flag3 ? this.colorCanBuyJuice : this.colorCantBuy);
						grpurchaseSlot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
						grpurchaseSlot.canAfford = flag3;
						if (flag3)
						{
							grpurchaseSlot.purchaseText = string.Format("RESEARCH\n⑮ {0}", num);
						}
						else
						{
							grpurchaseSlot.purchaseText = string.Format("NEED\n⑮ {0}", num);
						}
					}
				}
			}
			if (slotID != this.selectedItem)
			{
				this.gameShelves[shelf].SetBacklightStateAndMaterial(slotID, false, this.backlightLocked);
				return;
			}
			if (grpurchaseSlot.Price.color == this.colorCanBuyJuice)
			{
				this.gameShelves[shelf].SetBacklightStateAndMaterial(slotID, true, this.backlightResearch);
				return;
			}
			if (grpurchaseSlot.Price.color == this.colorCanBuyCredits)
			{
				this.gameShelves[shelf].SetBacklightStateAndMaterial(slotID, true, this.backlightPurchase);
				return;
			}
			this.gameShelves[shelf].SetBacklightStateAndMaterial(slotID, true, this.backlightLocked);
		}
	}

	// Token: 0x06003500 RID: 13568 RVA: 0x00124BF8 File Offset: 0x00122DF8
	public void UpdatePlayerCurrencyUI()
	{
		if (this.currentActivePlayerActorNumber == -1)
		{
			this.playerInfo.text = "AVAILABLE";
			return;
		}
		GRPlayer y = GRPlayer.Get(VRRig.LocalRig);
		GRPlayer grplayer = GRPlayer.Get(this.currentActivePlayerActorNumber);
		if (grplayer == null)
		{
			this.currentActivePlayerActorNumber = -1;
			this.playerInfo.text = "AVAILABLE";
			return;
		}
		string text2;
		if (grplayer == y)
		{
			int shiftCredits = grplayer.ShiftCredits;
			int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
			NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentActivePlayerActorNumber);
			string text = (player != null) ? player.SanitizedNickName : "RANDO MONKE";
			string employeeLevelDisplayName = this.toolProgressionManager.GetEmployeeLevelDisplayName(this.toolProgressionManager.GetCurrentEmployeeLevel());
			text2 = string.Format("<color=#c0c0c0>{0}\n{1}</color>\n\n<color=purple><size=2>⑮ {2}</size></color>\n<color=white><size=2>⑭ {3}</size></color>\n", new object[]
			{
				text,
				employeeLevelDisplayName,
				numberOfResearchPoints,
				shiftCredits
			});
		}
		else
		{
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(this.currentActivePlayerActorNumber);
			text2 = (((player2 != null) ? player2.SanitizedNickName : "RANDO MONKE") ?? "");
		}
		this.playerInfo.text = text2;
	}

	// Token: 0x06003501 RID: 13569 RVA: 0x00124D28 File Offset: 0x00122F28
	public bool CanLocalPlayerPurchaseItem(int shelf, int slotID)
	{
		if (!this.IsValidShelfItemIndex(shelf, slotID))
		{
			return false;
		}
		if (this.grManager && this.grManager.DebugIsToolStationHacked())
		{
			return true;
		}
		this.UpdateShelfItemDisplayElements(shelf, slotID);
		return this.gameShelves[shelf].gRPurchaseSlots[slotID].canAfford;
	}

	// Token: 0x06003502 RID: 13570 RVA: 0x00124D84 File Offset: 0x00122F84
	public bool CheckActivePlayer()
	{
		GRPlayer y = GRPlayer.Get(VRRig.LocalRig);
		if (this.currentActivePlayerActorNumber == -1)
		{
			this.RequestActivePlayerToken();
			return false;
		}
		GRPlayer x = GRPlayer.Get(this.currentActivePlayerActorNumber);
		if (x == null)
		{
			this.currentActivePlayerActorNumber = -1;
		}
		return !(x != y);
	}

	// Token: 0x06003503 RID: 13571 RVA: 0x00124DD3 File Offset: 0x00122FD3
	public void SelectOption1()
	{
		this.OnLocalSelectionButtonPressed(0);
	}

	// Token: 0x06003504 RID: 13572 RVA: 0x00124DDC File Offset: 0x00122FDC
	public void SelectOption2()
	{
		this.OnLocalSelectionButtonPressed(1);
	}

	// Token: 0x06003505 RID: 13573 RVA: 0x00124DE5 File Offset: 0x00122FE5
	public void SelectOption3()
	{
		this.OnLocalSelectionButtonPressed(2);
	}

	// Token: 0x06003506 RID: 13574 RVA: 0x00124DEE File Offset: 0x00122FEE
	public void SelectOption4()
	{
		this.OnLocalSelectionButtonPressed(3);
	}

	// Token: 0x06003507 RID: 13575 RVA: 0x00124DF8 File Offset: 0x00122FF8
	public void OnLocalSelectionButtonPressed(int index)
	{
		if (!this.CheckActivePlayer())
		{
			if (index == 0 && this.selectedItem != 0)
			{
				this.select1.SetButtonState(false);
			}
			if (index == 1 && this.selectedItem != 1)
			{
				this.select2.SetButtonState(false);
			}
			if (index == 2 && this.selectedItem != 2)
			{
				this.select3.SetButtonState(false);
			}
			if (index == 3 && this.selectedItem != 3)
			{
				this.select4.SetButtonState(false);
			}
			return;
		}
		if (index != 0)
		{
			this.select1.SetButtonState(false);
		}
		if (index != 1)
		{
			this.select2.SetButtonState(false);
		}
		if (index != 2)
		{
			this.select3.SetButtonState(false);
		}
		if (index != 3)
		{
			this.select4.SetButtonState(false);
		}
		if (this.shelfMovementState == GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle)
		{
			this.SetSelectedShelfAndItem(this.selectedShelf, index, false);
		}
	}

	// Token: 0x06003508 RID: 13576 RVA: 0x00124EC5 File Offset: 0x001230C5
	public void SelectPageDown()
	{
		this.OnLocalSelectionPageChange(1);
	}

	// Token: 0x06003509 RID: 13577 RVA: 0x00124ECE File Offset: 0x001230CE
	public void SelectPageUp()
	{
		this.OnLocalSelectionPageChange(-1);
	}

	// Token: 0x0600350A RID: 13578 RVA: 0x00124ED7 File Offset: 0x001230D7
	public void OnLocalSelectionPageChange(int delta)
	{
		if (!this.CheckActivePlayer())
		{
			return;
		}
		this.pageSelectionWheel.SetTargetShelf((this.pageSelectionWheel.targetPage + delta + this.gameShelves.Count) % this.gameShelves.Count);
	}

	// Token: 0x0600350B RID: 13579 RVA: 0x00124F12 File Offset: 0x00123112
	public void CardSwiped()
	{
		this.RequestActivePlayerToken();
	}

	// Token: 0x0600350C RID: 13580 RVA: 0x00124F1C File Offset: 0x0012311C
	public void PurchaseButtonPressed()
	{
		if (this.disablePurchaseButton)
		{
			return;
		}
		this.purchaseButtonPressed = this.purchaseButtonCooldown;
		this.disablePurchaseButton = true;
		if (!this.CheckActivePlayer())
		{
			return;
		}
		if (this.shelfMovementState == GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle && this.desiredMagnetEntityTypeId == this.currentMagnetEntityTypeId)
		{
			this.RequestPurchaseItem(this.selectedShelf, this.selectedItem);
		}
	}

	// Token: 0x0600350D RID: 13581 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void DEBUGSetHackToolStation()
	{
	}

	// Token: 0x0600350E RID: 13582 RVA: 0x00124F76 File Offset: 0x00123176
	public void RequestActivePlayerToken()
	{
		if (this.lastRequestedActivePlayerTokenTime > Time.time || this.lastRequestedActivePlayerTokenTime + this.requestActivePlayerTokenThrottleTime < Time.time)
		{
			this.lastRequestedActivePlayerTokenTime = Time.time;
			this.grManager.RequestStationExclusivity(this);
		}
	}

	// Token: 0x0600350F RID: 13583 RVA: 0x00124FB0 File Offset: 0x001231B0
	private void UpdateMagnet()
	{
		if (this.desiredMagnetEntityTypeId != this.currentMagnetEntityTypeId || this.currentMagnetEntityTypeId == -1 || this.currentMagnetEntity == null)
		{
			this.magnetMovement.SetHardStopAtTarget(true);
			this.magnetMovement.target = 0f;
			this.magnetMovement.Update();
			Vector3 position = this.ropeTop.transform.position;
			position.y = Mathf.Lerp(this.prefabMagnetHeightOffset, this.prefabMagnetHeightOffset - this.maxMagnetDistance, this.magnetMovement.pos);
			if (position.y != this.ropeTop.transform.position.y)
			{
				this.ropeTop.transform.position = position;
			}
			if (this.magnetMovement.IsAtTarget() && this.grManager.IsAuthority() && this.grManager.IsZoneActive())
			{
				if (this.currentMagnetEntity != null)
				{
					this.currentMagnetEntity.transform.parent = null;
					this.currentMagnetEntity.gameObject.SetActive(false);
					this.grManager.gameEntityManager.RequestDestroyItem(this.currentMagnetEntity.id);
					this.currentMagnetEntity = null;
					this.currentMagnetEntityTypeId = -1;
				}
				if (this.desiredMagnetEntityTypeId != -1)
				{
					GhostReactor.ToolEntityCreateData toolEntityCreateData = default(GhostReactor.ToolEntityCreateData);
					toolEntityCreateData.decayTime = 0f;
					toolEntityCreateData.stationIndex = this.grManager.GetIndexForToolUpgradeStationFull(this);
					this.grManager.gameEntityManager.RequestCreateItem(this.desiredMagnetEntityTypeId, this.ropeEnd.position, this.ropeEnd.rotation, toolEntityCreateData.Pack());
					this.currentMagnetEntityTypeId = this.desiredMagnetEntityTypeId;
					return;
				}
			}
		}
		else if (this.desiredMagnetEntityTypeId == this.currentMagnetEntityTypeId && this.currentMagnetEntity != null)
		{
			this.magnetMovement.SetHardStopAtTarget(false);
			this.magnetMovement.target = 1f;
			this.magnetMovement.Update();
			Vector3 position2 = this.ropeTop.transform.position;
			position2.y = Mathf.Lerp(this.prefabMagnetHeightOffset, this.prefabMagnetHeightOffset - this.maxMagnetDistance, this.magnetMovement.pos);
			if (this.ropeTop.transform.position.y != position2.y)
			{
				this.ropeTop.transform.position = position2;
			}
		}
	}

	// Token: 0x06003510 RID: 13584 RVA: 0x00125224 File Offset: 0x00123424
	public void InitLinkedEntity(GameEntity entity)
	{
		if (this.currentMagnetEntity != null)
		{
			this.currentMagnetEntity.gameObject.SetActive(false);
		}
		entity.pickupable = false;
		Rigidbody component = entity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = true;
		}
		GRToolUpgradePurchaseStationMagnetPoint component2 = entity.GetComponent<GRToolUpgradePurchaseStationMagnetPoint>();
		GameDockable component3 = entity.GetComponent<GameDockable>();
		Transform dock = (component2 != null) ? component2.magnetAttachTransform : ((component3 != null) ? component3.dockablePoint : entity.transform);
		GRToolUpgradePurchaseStationFull.AttachEntityToMagnet_DockGoesToLocation(this.magnet, entity.transform, dock, new Vector3(0f, -0.03f, 0f));
		float angle = 0f;
		float angle2 = 0f;
		bool flag = false;
		for (int i = 0; i < this.gameShelves.Count; i++)
		{
			for (int j = 0; j < this.gameShelves[i].gRPurchaseSlots.Count; j++)
			{
				GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[i].gRPurchaseSlots[j];
				if (grpurchaseSlot != null && !(grpurchaseSlot.ToolEntityPrefab == null) && grpurchaseSlot.ToolEntityPrefab.name != null && grpurchaseSlot.ToolEntityPrefab.name.GetStaticHash() == entity.typeId)
				{
					angle = grpurchaseSlot.RopeYaw;
					angle2 = grpurchaseSlot.RopePitch;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		Quaternion quaternion = Quaternion.Euler(0f, 0f, 180f);
		quaternion = Quaternion.AngleAxis(angle, Vector3.up) * quaternion;
		quaternion = Quaternion.AngleAxis(angle2, Vector3.forward) * quaternion;
		this.magnet.localRotation = quaternion;
		this.magnet.localPosition = quaternion * new Vector3(0f, 0.055f, 0f);
		this.currentMagnetEntity = entity;
		this.currentMagnetEntityTypeId = entity.typeId;
	}

	// Token: 0x06003511 RID: 13585 RVA: 0x0012541C File Offset: 0x0012361C
	public void UpdateSelectionLever()
	{
		GRPlayer x = GRPlayer.Get(VRRig.LocalRig);
		GRPlayer y = GRPlayer.Get(this.currentActivePlayerActorNumber);
		bool flag = ControllerInputPoller.GripFloat(XRNode.LeftHand) > 0.7f;
		bool flag2 = ControllerInputPoller.GripFloat(XRNode.RightHand) > 0.7f;
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		Transform handTransform = gamePlayer.GetHandTransform(0);
		Transform handTransform2 = gamePlayer.GetHandTransform(1);
		Vector3 position = this.pageSelectionHandle.transform.position;
		Vector3 lhs = handTransform.position - position;
		Vector3 lhs2 = handTransform2.position - position;
		float num = this.pageSelectionLever.transform.localRotation.eulerAngles.x;
		float num2 = 0.2f;
		float num3 = this.bIsGrippingLeft ? 0.15f : 0.1f;
		float num4 = this.bIsGrippingRight ? 0.15f : 0.1f;
		if (lhs.sqrMagnitude > num3 * num3)
		{
			flag = false;
		}
		if (lhs2.sqrMagnitude > num4 * num4)
		{
			flag2 = false;
		}
		if (!this.bGripLeftLastFrame && flag)
		{
			this.bIsGrippingLeft = true;
		}
		else if (this.bGripLeftLastFrame && flag)
		{
			Vector3 forward = this.pageSelectionHandle.transform.forward;
			float num5 = Vector3.Dot(lhs, forward);
			num += num5 / num2 * 180f / 3.1415925f;
		}
		else
		{
			this.bIsGrippingLeft = false;
		}
		if (!this.bGripRightLastFrame && flag2)
		{
			this.bIsGrippingRight = true;
		}
		else if (this.bGripRightLastFrame && flag2)
		{
			Vector3 forward2 = this.pageSelectionHandle.transform.forward;
			float num6 = Vector3.Dot(lhs2, forward2);
			num += num6 / num2 * 180f / 3.1415925f;
		}
		else
		{
			this.bIsGrippingRight = false;
		}
		if (!this.bIsGrippingLeft && !this.bIsGrippingRight && x == y)
		{
			num = 30f + (num - 30f) * Mathf.Exp(-20f * Time.deltaTime);
		}
		num = Mathf.Clamp(num, 0f, 60f);
		if ((x == y || this.currentActivePlayerActorNumber == -1) && this.lastHandleAngle != num)
		{
			this.pageSelectionLever.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
			this.lastHandleAngle = num;
		}
		float rotationSpeed = 0f;
		if (this.bIsGrippingLeft || this.bIsGrippingRight)
		{
			rotationSpeed = (num - 30f) / 30f;
		}
		this.bGripLeftLastFrame = flag;
		this.bGripRightLastFrame = flag2;
		if (x == y)
		{
			this.pageSelectionWheel.isBeingDrivenRemotely = false;
			this.pageSelectionWheel.SetRotationSpeed(rotationSpeed);
			if (this.pageSelectionWheel.targetPage != this.selectedShelf)
			{
				this.SetSelectedShelfAndItem(this.pageSelectionWheel.targetPage, 0, false);
			}
			float num7 = 0.25f;
			this.timeSinceLastHandleBroadcast += Time.deltaTime;
			if (this.timeSinceLastHandleBroadcast > num7 && (Math.Abs(num - this.angleOfLastHandleBroadcast) > 0.02f || Math.Abs(this.pageSelectionWheel.currentAngle - this.selectionWheelAngleOfLastBroadcast) > 0.02f))
			{
				this.timeSinceLastHandleBroadcast = 0f;
				this.angleOfLastHandleBroadcast = num;
				this.selectionWheelAngleOfLastBroadcast = this.pageSelectionWheel.currentAngle;
				this.grManager.BroadcastHandleAndSelectionWheelPosition(this, (int)(num * this.quantMult), (int)(this.selectionWheelAngleOfLastBroadcast * this.quantMult));
				return;
			}
		}
		else if (this.bIsGrippingLeft || this.bIsGrippingRight)
		{
			this.CheckActivePlayer();
		}
	}

	// Token: 0x06003512 RID: 13586 RVA: 0x001257A0 File Offset: 0x001239A0
	public static void AttachEntityToMagnet_DockGoesToLocation(Transform magnet, Transform entity, Transform dock, Vector3 magnetDockOffset)
	{
		if (magnet == null || entity == null || dock == null)
		{
			return;
		}
		if (!dock.IsChildOf(entity))
		{
			return;
		}
		Matrix4x4 m = entity.worldToLocalMatrix * dock.localToWorldMatrix;
		Vector3 s = GRToolUpgradePurchaseStationFull.ExtractLossyScale(m);
		Vector3 localPosition;
		Quaternion localRotation;
		Vector3 localScale;
		GRToolUpgradePurchaseStationFull.DecomposeTRS(Matrix4x4.TRS(magnetDockOffset, Quaternion.identity, s) * m.inverse, out localPosition, out localRotation, out localScale);
		entity.SetParent(magnet, false);
		entity.localPosition = localPosition;
		entity.localRotation = localRotation;
		entity.localScale = localScale;
	}

	// Token: 0x06003513 RID: 13587 RVA: 0x00125830 File Offset: 0x00123A30
	public void SetHandleAndSelectionWheelPositionRemote(int handlePos, int wheelPos)
	{
		this.pageSelectionWheel.isBeingDrivenRemotely = true;
		float num = (float)handlePos / this.quantMult;
		num = Mathf.Clamp(num, 0f, 60f);
		this.pageSelectionLever.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
		this.pageSelectionWheel.SetTargetAngle((float)wheelPos / this.quantMult);
	}

	// Token: 0x06003514 RID: 13588 RVA: 0x00123C3B File Offset: 0x00121E3B
	public void ProgressionUpdated()
	{
		this.needsUIRefresh = true;
	}

	// Token: 0x06003515 RID: 13589 RVA: 0x00125898 File Offset: 0x00123A98
	public void SetSelectedShelfAndItem(int shelf, int item, bool fromNetworkRPC)
	{
		if (!this.IsValidShelfItemIndex(shelf, item))
		{
			return;
		}
		if (this.toolProgressionManager == null)
		{
			return;
		}
		GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(this.gameShelves[shelf].gRPurchaseSlots[item].PurchaseID);
		if (partMetadata != null)
		{
			this.itemDescriptionName.text = partMetadata.name;
			this.itemDescription.text = partMetadata.description;
			this.itemDescriptionAnnotation.text = partMetadata.annotation;
		}
		this.shelfSelectionText.text = this.gameShelves[shelf].ShelfName;
		if (this.gameShelves[shelf].gRPurchaseSlots[item].ToolEntityPrefab != null)
		{
			this.desiredMagnetEntityTypeId = this.gameShelves[shelf].gRPurchaseSlots[item].ToolEntityPrefab.name.GetStaticHash();
		}
		else
		{
			this.desiredMagnetEntityTypeId = -1;
		}
		bool flag = this.selectedShelf != shelf;
		bool flag2 = this.selectedItem != item;
		this.selectedShelf = shelf;
		this.selectedItem = item;
		this.needsUIRefresh = true;
		if (!fromNetworkRPC)
		{
			if (flag || flag2)
			{
				this.grManager.RequestNetworkShelfAndItemChange(this, this.selectedShelf, this.selectedItem);
				return;
			}
		}
		else
		{
			this.pageSelectionWheel.SetTargetShelf(this.selectedShelf);
			this.select1.SetButtonState(this.selectedItem == 0);
			this.select2.SetButtonState(this.selectedItem == 1);
			this.select3.SetButtonState(this.selectedItem == 2);
			this.select4.SetButtonState(this.selectedItem == 3);
		}
	}

	// Token: 0x06003516 RID: 13590 RVA: 0x00125A44 File Offset: 0x00123C44
	public void RequestPurchaseItem(int shelf, int item)
	{
		if (!this.IsValidShelfItemIndex(shelf, item))
		{
			return;
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[shelf].gRPurchaseSlots[item];
		if (!this.CanLocalPlayerPurchaseItem(shelf, item))
		{
			if (this.scanner != null)
			{
				UnityEvent onFailed = this.scanner.onFailed;
				if (onFailed == null)
				{
					return;
				}
				onFailed.Invoke();
			}
			return;
		}
		if (this.scanner != null)
		{
			UnityEvent onSucceeded = this.scanner.onSucceeded;
			if (onSucceeded != null)
			{
				onSucceeded.Invoke();
			}
		}
		bool flag;
		if (!this.grManager.DebugIsToolStationHacked() && (!this.toolProgressionManager.IsPartUnlocked(grpurchaseSlot.PurchaseID, out flag) || !flag))
		{
			this.toolProgressionManager.AttemptToUnlockPart(grpurchaseSlot.PurchaseID);
			return;
		}
		this.grManager.RequestPurchaseToolOrUpgrade(this, shelf, item);
	}

	// Token: 0x06003517 RID: 13591 RVA: 0x00125B0C File Offset: 0x00123D0C
	public ValueTuple<bool, bool> TryPurchaseAuthority(GRPlayer player, int shelf, int item)
	{
		if (this.currentActivePlayerActorNumber == -1)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		GRPlayer grplayer = GRPlayer.Get(this.currentActivePlayerActorNumber);
		if (grplayer == null)
		{
			this.currentActivePlayerActorNumber = -1;
			return new ValueTuple<bool, bool>(false, false);
		}
		if (player != grplayer)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		if (!this.grManager.IsAuthority())
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		if (!this.IsValidShelfItemIndex(shelf, item))
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		if (!this.toolProgressionManager)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[shelf].gRPurchaseSlots[item];
		this.toolProgressionManager.GetPartMetadata(grpurchaseSlot.PurchaseID);
		return new ValueTuple<bool, bool>(true, true);
	}

	// Token: 0x06003518 RID: 13592 RVA: 0x00125BCC File Offset: 0x00123DCC
	public void ToolPurchaseResponseLocal(GRPlayer player, int shelf, int item, bool success)
	{
		if (!this.IsValidShelfItemIndex(shelf, item))
		{
			return;
		}
		if (!this.toolProgressionManager)
		{
			return;
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[shelf].gRPurchaseSlots[item];
		GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(grpurchaseSlot.PurchaseID);
		if (partMetadata == null)
		{
			return;
		}
		if (success)
		{
			int shiftCreditCost = partMetadata.shiftCreditCost;
			if (player != null)
			{
				if (player == GRPlayer.Get(VRRig.LocalRig))
				{
					player.IncrementCoresSpentPlayer(shiftCreditCost);
					player.SendToolPurchasedTelemetry(partMetadata.name, item, shiftCreditCost, 0);
				}
				else
				{
					player.IncrementCoresSpentGroup(shiftCreditCost);
				}
				player.AddItemPurchased(partMetadata.name);
				player.SubtractShiftCredit(shiftCreditCost);
				player.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.SpentCredits, (float)shiftCreditCost);
				this.reactor.RefreshScoreboards();
			}
			if (this.currentMagnetEntity != null)
			{
				this.currentMagnetEntity.transform.parent = null;
				this.currentMagnetEntity.GetComponent<Rigidbody>().isKinematic = false;
				this.currentMagnetEntity.pickupable = true;
				this.currentMagnetEntity.createData = 0L;
				this.currentMagnetEntity = null;
				this.currentMagnetEntityTypeId = -1;
			}
			UnityEvent unityEvent = this.purchaseSucceded;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
			return;
		}
		else
		{
			UnityEvent unityEvent2 = this.purchaseFailed;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke();
			return;
		}
	}

	// Token: 0x06003519 RID: 13593 RVA: 0x00125D08 File Offset: 0x00123F08
	public void InitPageSelectionWheel()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < this.gameShelves.Count; i++)
		{
			list.Add(this.gameShelves[i].ShelfName);
		}
		this.pageSelectionWheel.InitFromNameList(list);
	}

	// Token: 0x0600351A RID: 13594 RVA: 0x00125D54 File Offset: 0x00123F54
	public static Color ColorFromRGB32(int r, int g, int b)
	{
		return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
	}

	// Token: 0x0600351B RID: 13595 RVA: 0x00125D74 File Offset: 0x00123F74
	public bool IsValidShelfItemIndex(int shelf, int idx)
	{
		return shelf >= 0 && shelf < this.gameShelves.Count && this.gameShelves[shelf].gRPurchaseSlots != null && idx >= 0 && idx < this.gameShelves[shelf].gRPurchaseSlots.Count && this.gameShelves[shelf].gRPurchaseSlots[idx].PurchaseID > GRToolProgressionManager.ToolParts.None;
	}

	// Token: 0x0600351C RID: 13596 RVA: 0x00125DE4 File Offset: 0x00123FE4
	private static Vector3 ExtractLossyScale(Matrix4x4 m)
	{
		float magnitude = new Vector3(m.m00, m.m10, m.m20).magnitude;
		float magnitude2 = new Vector3(m.m01, m.m11, m.m21).magnitude;
		float magnitude3 = new Vector3(m.m02, m.m12, m.m22).magnitude;
		return new Vector3(magnitude, magnitude2, magnitude3);
	}

	// Token: 0x0600351D RID: 13597 RVA: 0x00125E58 File Offset: 0x00124058
	private static void DecomposeTRS(Matrix4x4 m, out Vector3 pos, out Quaternion rot, out Vector3 scale)
	{
		pos = m.GetColumn(3);
		Vector3 a = m.GetColumn(0);
		Vector3 a2 = m.GetColumn(1);
		Vector3 a3 = m.GetColumn(2);
		scale = new Vector3(a.magnitude, a2.magnitude, a3.magnitude);
		a / scale.x;
		Vector3 upwards = a2 / scale.y;
		Vector3 forward = a3 / scale.z;
		rot = Quaternion.LookRotation(forward, upwards);
	}

	// Token: 0x0400453A RID: 17722
	private GhostReactor reactor;

	// Token: 0x0400453B RID: 17723
	private GhostReactorManager grManager;

	// Token: 0x0400453C RID: 17724
	public List<GRToolUpgradePurchaseStationShelf> gameShelves;

	// Token: 0x0400453D RID: 17725
	[NonSerialized]
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x0400453E RID: 17726
	private Color colorPurchaseButtonCanAfford = GRToolUpgradePurchaseStationFull.ColorFromRGB32(0, 0, 0);

	// Token: 0x0400453F RID: 17727
	private Color colorCanBuyCredits = GRToolUpgradePurchaseStationFull.ColorFromRGB32(140, 229, 37);

	// Token: 0x04004540 RID: 17728
	private Color colorCanBuyJuice = GRToolUpgradePurchaseStationFull.ColorFromRGB32(232, 65, 255);

	// Token: 0x04004541 RID: 17729
	private Color colorCantBuy = GRToolUpgradePurchaseStationFull.ColorFromRGB32(140, 38, 38);

	// Token: 0x04004542 RID: 17730
	private Color colorSelectedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(251, 240, 229);

	// Token: 0x04004543 RID: 17731
	private Color colorUnselectedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(147, 145, 140);

	// Token: 0x04004544 RID: 17732
	private Color colorUnresearchedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(230, 19, 17);

	// Token: 0x04004545 RID: 17733
	private Color colorUnselectedUnresearchedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(133, 11, 10);

	// Token: 0x04004546 RID: 17734
	private int selectedShelf;

	// Token: 0x04004547 RID: 17735
	private int selectedItem;

	// Token: 0x04004548 RID: 17736
	[NonSerialized]
	public int currentActivePlayerActorNumber = -1;

	// Token: 0x04004549 RID: 17737
	private GRToolUpgradePurchaseStationFull.ShelfMovementState shelfMovementState;

	// Token: 0x0400454A RID: 17738
	private int currentVisibleShelfIndex;

	// Token: 0x0400454B RID: 17739
	private int nextVisibleShelfIndex;

	// Token: 0x0400454C RID: 17740
	private GRSpringMovement frontBackShelfMovement;

	// Token: 0x0400454D RID: 17741
	private GRSpringMovement raiseLowerShelfMovement;

	// Token: 0x0400454E RID: 17742
	public Transform shelfRootTransform;

	// Token: 0x0400454F RID: 17743
	public Transform shelfBackTransform;

	// Token: 0x04004550 RID: 17744
	public Transform shelfLowerTransform;

	// Token: 0x04004551 RID: 17745
	public TMP_Text shelfSelectionText;

	// Token: 0x04004552 RID: 17746
	public TMP_Text playerInfo;

	// Token: 0x04004553 RID: 17747
	public TMP_Text itemDescription;

	// Token: 0x04004554 RID: 17748
	public TMP_Text itemDescriptionName;

	// Token: 0x04004555 RID: 17749
	public TMP_Text itemDescriptionAnnotation;

	// Token: 0x04004556 RID: 17750
	public TMP_Text purchaseButtonText;

	// Token: 0x04004557 RID: 17751
	public GorillaPhysicalButton select1;

	// Token: 0x04004558 RID: 17752
	public GorillaPhysicalButton select2;

	// Token: 0x04004559 RID: 17753
	public GorillaPhysicalButton select3;

	// Token: 0x0400455A RID: 17754
	public GorillaPhysicalButton select4;

	// Token: 0x0400455B RID: 17755
	public AudioSource audioSourceLooping;

	// Token: 0x0400455C RID: 17756
	public AudioSource audioSourceClang;

	// Token: 0x0400455D RID: 17757
	public float audioSourceLoopingVolume = 0.5f;

	// Token: 0x0400455E RID: 17758
	public Material unresearchedItemMaterial;

	// Token: 0x0400455F RID: 17759
	public AudioSource interactAudioSource;

	// Token: 0x04004560 RID: 17760
	public IDCardScanner scanner;

	// Token: 0x04004561 RID: 17761
	public UnityEvent purchaseSucceded;

	// Token: 0x04004562 RID: 17762
	public UnityEvent purchaseFailed;

	// Token: 0x04004563 RID: 17763
	public Material backlightPurchase;

	// Token: 0x04004564 RID: 17764
	public Material backlightResearch;

	// Token: 0x04004565 RID: 17765
	public Material backlightLocked;

	// Token: 0x04004566 RID: 17766
	private int lastKnownLocalPlayerCredits;

	// Token: 0x04004567 RID: 17767
	private int lastKnownLocalPlayerJuice;

	// Token: 0x04004568 RID: 17768
	private bool needsUIRefresh;

	// Token: 0x04004569 RID: 17769
	public Transform ropeTop;

	// Token: 0x0400456A RID: 17770
	public Transform ropeEnd;

	// Token: 0x0400456B RID: 17771
	public Transform magnet;

	// Token: 0x0400456C RID: 17772
	private GameEntity currentMagnetEntity;

	// Token: 0x0400456D RID: 17773
	private int currentMagnetEntityTypeId = -1;

	// Token: 0x0400456E RID: 17774
	private int desiredMagnetEntityTypeId = -1;

	// Token: 0x0400456F RID: 17775
	private float prefabMagnetHeightOffset;

	// Token: 0x04004570 RID: 17776
	public float maxMagnetDistance = 0.75f;

	// Token: 0x04004571 RID: 17777
	private GRSpringMovement magnetMovement;

	// Token: 0x04004572 RID: 17778
	public GRSelectionWheel pageSelectionWheel;

	// Token: 0x04004573 RID: 17779
	public GameObject pageSelectionHandle;

	// Token: 0x04004574 RID: 17780
	public GameObject pageSelectionLever;

	// Token: 0x04004575 RID: 17781
	public float playerQueueTimeLimit = 30f;

	// Token: 0x04004576 RID: 17782
	private bool disablePurchaseButton;

	// Token: 0x04004577 RID: 17783
	private float purchaseButtonCooldown = 2f;

	// Token: 0x04004578 RID: 17784
	private float purchaseButtonPressed;

	// Token: 0x04004579 RID: 17785
	private const int ShelfIndex_None = -1;

	// Token: 0x0400457B RID: 17787
	public bool currentlyShowingText = true;

	// Token: 0x0400457C RID: 17788
	private List<GRToolProgressionManager.ToolParts> cachedRequiredPartsList = new List<GRToolProgressionManager.ToolParts>(5);

	// Token: 0x0400457D RID: 17789
	private float lastRequestedActivePlayerTokenTime;

	// Token: 0x0400457E RID: 17790
	private float requestActivePlayerTokenThrottleTime = 0.25f;

	// Token: 0x0400457F RID: 17791
	private bool bIsGrippingLeft;

	// Token: 0x04004580 RID: 17792
	private bool bIsGrippingRight;

	// Token: 0x04004581 RID: 17793
	private bool bGripLeftLastFrame;

	// Token: 0x04004582 RID: 17794
	private bool bGripRightLastFrame;

	// Token: 0x04004583 RID: 17795
	private float maxHandleRange = 0.09f;

	// Token: 0x04004584 RID: 17796
	private float timeSinceLastHandleBroadcast;

	// Token: 0x04004585 RID: 17797
	private float angleOfLastHandleBroadcast;

	// Token: 0x04004586 RID: 17798
	private float selectionWheelAngleOfLastBroadcast;

	// Token: 0x04004587 RID: 17799
	private float quantMult = 100000f;

	// Token: 0x04004588 RID: 17800
	private float lastHandleAngle = -10000f;

	// Token: 0x02000812 RID: 2066
	public enum ShelfMovementState
	{
		// Token: 0x0400458A RID: 17802
		Idle,
		// Token: 0x0400458B RID: 17803
		MoveCurrentShelfBackward,
		// Token: 0x0400458C RID: 17804
		MoveCurrentShelfForward,
		// Token: 0x0400458D RID: 17805
		MoveNextShelfUpward,
		// Token: 0x0400458E RID: 17806
		MoveNextShelfDownward,
		// Token: 0x0400458F RID: 17807
		Count
	}
}
