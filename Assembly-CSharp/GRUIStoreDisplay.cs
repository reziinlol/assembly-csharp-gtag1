using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000824 RID: 2084
public class GRUIStoreDisplay : MonoBehaviour
{
	// Token: 0x0600358E RID: 13710 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void Awake()
	{
	}

	// Token: 0x0600358F RID: 13711 RVA: 0x001288CD File Offset: 0x00126ACD
	public void OnEnable()
	{
		this.RefreshUI();
	}

	// Token: 0x06003590 RID: 13712 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDisable()
	{
	}

	// Token: 0x06003591 RID: 13713 RVA: 0x001288D5 File Offset: 0x00126AD5
	public void Setup(int playerActorId, GhostReactor reactor)
	{
		this.reactor = reactor;
		this.toolProgressionManager = reactor.toolProgression;
		this.playerActorId = playerActorId;
		this.RefreshUI();
		this.toolProgressionManager.OnProgressionUpdated += this.onProgressionUpdated;
	}

	// Token: 0x06003592 RID: 13714 RVA: 0x001288CD File Offset: 0x00126ACD
	private void onProgressionUpdated()
	{
		this.RefreshUI();
	}

	// Token: 0x06003593 RID: 13715 RVA: 0x0012890E File Offset: 0x00126B0E
	private void RefreshUI()
	{
		this.RefreshItemInfo();
	}

	// Token: 0x06003594 RID: 13716 RVA: 0x00128918 File Offset: 0x00126B18
	public void OnBuy(int playerActorNumber)
	{
		if (playerActorNumber != this.playerActorId)
		{
			return;
		}
		if (GRPlayer.Get(this.playerActorId) == null)
		{
			return;
		}
		if (!this.CanLocalPlayerPurchaseItem())
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
		if (!this.reactor.grManager.DebugIsToolStationHacked() && (!this.toolProgressionManager.IsPartUnlocked(this.slot.PurchaseID, out flag) || !flag))
		{
			if (this.slot.drillUpgradeLevel == ProgressionManager.DrillUpgradeLevel.Base)
			{
				if (ProgressionManager.Instance.GetShinyRocksTotal() >= 2500)
				{
					ProgressionManager.Instance.PurchaseDrillUpgrade(ProgressionManager.DrillUpgradeLevel.Base);
					return;
				}
			}
			else
			{
				this.toolProgressionManager.AttemptToUnlockPart(this.slot.PurchaseID);
			}
		}
	}

	// Token: 0x06003595 RID: 13717 RVA: 0x00128A03 File Offset: 0x00126C03
	private bool CanLocalPlayerPurchaseItem()
	{
		return this.slot.canAfford;
	}

	// Token: 0x06003596 RID: 13718 RVA: 0x00128A10 File Offset: 0x00126C10
	public void RefreshItemInfo()
	{
		bool flag = true;
		if (this.toolProgressionManager != null)
		{
			GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(this.slot.PurchaseID);
			if (partMetadata == null)
			{
				this.slot.Name.text = "ERROR";
				return;
			}
			string text = "ERROR";
			string text2 = "";
			Color white = Color.white;
			bool flag2 = true;
			int num = 10000;
			int num2;
			this.toolProgressionManager.GetPlayerShiftCredit(out num2);
			int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
			this.slot.canAfford = false;
			this.slot.purchaseText = "LOCKED";
			if (this.slot.Description != null)
			{
				this.slot.Description.text = partMetadata.description;
			}
			bool flag3;
			if (this.toolProgressionManager.IsPartUnlocked(this.slot.PurchaseID, out flag3))
			{
				if (flag3)
				{
					if (this.slot.drillUpgradeLevel != ProgressionManager.DrillUpgradeLevel.None)
					{
						this.slot.Price.color = this.colorCanBuyCredits;
						this.slot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
						this.slot.canAfford = true;
						this.slot.purchaseText = "Purchased";
						text = this.slot.purchaseText;
						this.slot.Price.text = text;
						return;
					}
					if (this.toolProgressionManager.GetShiftCreditCost(this.slot.PurchaseID, out num))
					{
						text = string.Format("⑭ {0}", num);
					}
					bool flag4 = num2 >= num;
					this.slot.Name.text = partMetadata.name;
					this.slot.Name.color = (flag ? this.colorSelectedItem : this.colorUnselectedItem);
					this.slot.Price.text = text;
					this.slot.Price.color = (flag4 ? this.colorCanBuyCredits : this.colorCantBuy);
					this.slot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
					this.slot.canAfford = flag4;
					if (flag4)
					{
						this.slot.purchaseText = string.Format("BUY FOR\n⑭ {0}", num);
						return;
					}
					this.slot.purchaseText = string.Format("NEED\n⑭ {0}", num);
					return;
				}
				else
				{
					this.slot.Name.text = partMetadata.name;
					this.slot.Name.color = (flag ? this.colorUnresearchedItem : this.colorUnselectedUnresearchedItem);
					flag2 = true;
					GRToolProgressionTree.EmployeeLevelRequirement employeeLevelRequirement;
					if (this.toolProgressionManager.GetPartUnlockEmployeeRequiredLevel(this.slot.PurchaseID, out employeeLevelRequirement) && this.toolProgressionManager.GetCurrentEmployeeLevel() < employeeLevelRequirement)
					{
						this.toolProgressionManager.GetEmployeeLevelDisplayName(employeeLevelRequirement);
						text2 += string.Format("⑱ {0}\n", employeeLevelRequirement);
						flag2 = false;
					}
					this.cachedRequiredPartsList.Clear();
					if (this.toolProgressionManager.GetPartUnlockRequiredParentParts(this.slot.PurchaseID, out this.cachedRequiredPartsList))
					{
						foreach (GRToolProgressionManager.ToolParts part in this.cachedRequiredPartsList)
						{
							bool flag5 = false;
							GRToolProgressionManager.ToolProgressionMetaData partMetadata2 = this.toolProgressionManager.GetPartMetadata(part);
							if (partMetadata2 == null)
							{
								text2 += "⑱ ERROR\n";
								flag2 = false;
							}
							else if (!this.toolProgressionManager.IsPartUnlocked(part, out flag5) || !flag5)
							{
								text2 = text2 + "⑱ " + partMetadata2.name + "\n";
								flag2 = false;
							}
						}
					}
					if (!flag2)
					{
						this.slot.Price.text = text2;
						this.slot.Price.color = this.colorCantBuy;
						this.slot.Price.fontSize = ((text2.Length <= 8) ? 2.25f : 1.6f);
						this.slot.canAfford = false;
						this.slot.purchaseText = "LOCKED";
						return;
					}
					if (this.slot.drillUpgradeLevel == ProgressionManager.DrillUpgradeLevel.Base)
					{
						this.slot.Price.color = this.colorCanBuyCredits;
						this.slot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
						this.slot.canAfford = true;
						this.slot.purchaseText = string.Format("Cost {0}⑯ Shiny Rocks", 2500);
						text = this.slot.purchaseText;
						this.slot.Price.text = text;
						return;
					}
					if (this.toolProgressionManager.GetPartUnlockJuiceCost(this.slot.PurchaseID, out num))
					{
						text = string.Format("⑮ {0}", num);
					}
					bool flag4 = numberOfResearchPoints >= num;
					this.slot.Price.text = text;
					this.slot.Price.color = (flag4 ? this.colorCanBuyJuice : this.colorCantBuy);
					this.slot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
					this.slot.canAfford = flag4;
					if (flag4)
					{
						this.slot.purchaseText = string.Format("RESEARCH\n⑮ {0}", num);
						return;
					}
					this.slot.purchaseText = string.Format("NEED\n⑮ {0}", num);
				}
			}
		}
	}

	// Token: 0x04004623 RID: 17955
	public IDCardScanner scanner;

	// Token: 0x04004624 RID: 17956
	public GRUIStoreDisplay.GRPurchaseSlot slot;

	// Token: 0x04004625 RID: 17957
	private GhostReactor reactor;

	// Token: 0x04004626 RID: 17958
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x04004627 RID: 17959
	private int playerActorId;

	// Token: 0x04004628 RID: 17960
	private Color colorPurchaseButtonCanAfford = GRToolUpgradePurchaseStationFull.ColorFromRGB32(0, 0, 0);

	// Token: 0x04004629 RID: 17961
	private Color colorCanBuyCredits = GRToolUpgradePurchaseStationFull.ColorFromRGB32(140, 229, 37);

	// Token: 0x0400462A RID: 17962
	private Color colorCanBuyJuice = GRToolUpgradePurchaseStationFull.ColorFromRGB32(232, 65, 255);

	// Token: 0x0400462B RID: 17963
	private Color colorCantBuy = GRToolUpgradePurchaseStationFull.ColorFromRGB32(140, 38, 38);

	// Token: 0x0400462C RID: 17964
	private Color colorSelectedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(251, 240, 229);

	// Token: 0x0400462D RID: 17965
	private Color colorUnselectedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(147, 145, 140);

	// Token: 0x0400462E RID: 17966
	private Color colorUnresearchedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(230, 19, 17);

	// Token: 0x0400462F RID: 17967
	private Color colorUnselectedUnresearchedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(133, 11, 10);

	// Token: 0x04004630 RID: 17968
	private List<GRToolProgressionManager.ToolParts> cachedRequiredPartsList = new List<GRToolProgressionManager.ToolParts>(5);

	// Token: 0x02000825 RID: 2085
	[Serializable]
	public class GRPurchaseSlot
	{
		// Token: 0x04004631 RID: 17969
		public TMP_Text Name;

		// Token: 0x04004632 RID: 17970
		public TMP_Text Price;

		// Token: 0x04004633 RID: 17971
		public TMP_Text Description;

		// Token: 0x04004634 RID: 17972
		public GRToolProgressionManager.ToolParts PurchaseID;

		// Token: 0x04004635 RID: 17973
		[NonSerialized]
		public Material overrideMaterial;

		// Token: 0x04004636 RID: 17974
		[NonSerialized]
		public bool canAfford;

		// Token: 0x04004637 RID: 17975
		[NonSerialized]
		public string purchaseText = "";

		// Token: 0x04004638 RID: 17976
		public ProgressionManager.DrillUpgradeLevel drillUpgradeLevel;
	}
}
