using System;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x0200056F RID: 1391
public class CosmeticWardrobe : MonoBehaviour
{
	// Token: 0x170003BC RID: 956
	// (get) Token: 0x0600235B RID: 9051 RVA: 0x000BE3B3 File Offset: 0x000BC5B3
	// (set) Token: 0x0600235C RID: 9052 RVA: 0x000BE3BB File Offset: 0x000BC5BB
	public bool UseTemporarySet
	{
		get
		{
			return this.m_useTemporarySet;
		}
		set
		{
			bool flag = value != this.m_useTemporarySet;
			this.m_useTemporarySet = value;
			if (flag)
			{
				this.HandleCosmeticsUpdated();
			}
		}
	}

	// Token: 0x0600235D RID: 9053 RVA: 0x000BE3D8 File Offset: 0x000BC5D8
	private void Start()
	{
		for (int i = 0; i < this.cosmeticCategoryButtons.Length; i++)
		{
			if (this.cosmeticCategoryButtons[i].category == CosmeticWardrobe.selectedCategory)
			{
				CosmeticWardrobe.selectedCategoryIndex = i;
				break;
			}
		}
		for (int j = 0; j < this.cosmeticCollectionDisplays.Length; j++)
		{
			this.cosmeticCollectionDisplays[j].displayHead.transform.localScale = this.startingHeadSize;
		}
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged += this.HandleLocalColorChanged;
			this.HandleLocalColorChanged(GorillaTagger.Instance.offlineVRRig.playerColor);
		}
		this.nextSelection.onPressed += this.HandlePressedNextSelection;
		this.prevSelection.onPressed += this.HandlePressedPrevSelection;
		for (int k = 0; k < this.cosmeticCollectionDisplays.Length; k++)
		{
			this.cosmeticCollectionDisplays[k].selectButton.onPressed += this.HandlePressedSelectCosmeticButton;
		}
		for (int l = 0; l < this.cosmeticCategoryButtons.Length; l++)
		{
			this.cosmeticCategoryButtons[l].button.onPressed += this.HandleChangeCategory;
			this.cosmeticCategoryButtons[l].slot1RemovedItem = CosmeticsController.instance.nullItem;
			this.cosmeticCategoryButtons[l].slot2RemovedItem = CosmeticsController.instance.nullItem;
		}
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnCosmeticsUpdated = (Action)Delegate.Combine(instance.OnCosmeticsUpdated, new Action(this.HandleCosmeticsUpdated));
		CosmeticsController instance2 = CosmeticsController.instance;
		instance2.OnOutfitsUpdated = (Action)Delegate.Combine(instance2.OnOutfitsUpdated, new Action(this.UpdateOutfitButtons));
		CosmeticWardrobe.OnWardrobeUpdateCategories = (Action)Delegate.Combine(CosmeticWardrobe.OnWardrobeUpdateCategories, new Action(this.UpdateCategoryButtons));
		CosmeticWardrobe.OnWardrobeUpdateDisplays = (Action)Delegate.Combine(CosmeticWardrobe.OnWardrobeUpdateDisplays, new Action(this.UpdateCosmeticDisplays));
		this.previousOutfit.onPressed += this.HandlePressedPrevOutfitButton;
		this.nextOutfit.onPressed += this.HandlePressedNextOutfitButton;
		this.HandleCosmeticsUpdated();
	}

	// Token: 0x0600235E RID: 9054 RVA: 0x000BE618 File Offset: 0x000BC818
	private void OnDestroy()
	{
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged -= this.HandleLocalColorChanged;
		}
		this.nextSelection.onPressed -= this.HandlePressedNextSelection;
		this.prevSelection.onPressed -= this.HandlePressedPrevSelection;
		for (int i = 0; i < this.cosmeticCollectionDisplays.Length; i++)
		{
			this.cosmeticCollectionDisplays[i].selectButton.onPressed -= this.HandlePressedSelectCosmeticButton;
		}
		for (int j = 0; j < this.cosmeticCategoryButtons.Length; j++)
		{
			this.cosmeticCategoryButtons[j].button.onPressed -= this.HandleChangeCategory;
		}
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnCosmeticsUpdated = (Action)Delegate.Remove(instance.OnCosmeticsUpdated, new Action(this.HandleCosmeticsUpdated));
		CosmeticsController instance2 = CosmeticsController.instance;
		instance2.OnOutfitsUpdated = (Action)Delegate.Remove(instance2.OnOutfitsUpdated, new Action(this.UpdateOutfitButtons));
		CosmeticWardrobe.OnWardrobeUpdateCategories = (Action)Delegate.Remove(CosmeticWardrobe.OnWardrobeUpdateCategories, new Action(this.UpdateCategoryButtons));
		CosmeticWardrobe.OnWardrobeUpdateDisplays = (Action)Delegate.Remove(CosmeticWardrobe.OnWardrobeUpdateDisplays, new Action(this.UpdateCosmeticDisplays));
		this.previousOutfit.onPressed -= this.HandlePressedPrevOutfitButton;
		this.nextOutfit.onPressed -= this.HandlePressedNextOutfitButton;
	}

	// Token: 0x0600235F RID: 9055 RVA: 0x000BE7AC File Offset: 0x000BC9AC
	private void HandlePressedNextSelection(GorillaPressableButton button, bool isLeft)
	{
		CosmeticWardrobe.startingDisplayIndex += this.cosmeticCollectionDisplays.Length;
		if (CosmeticWardrobe.startingDisplayIndex >= CosmeticsController.instance.GetCategorySize(CosmeticWardrobe.selectedCategory))
		{
			CosmeticWardrobe.startingDisplayIndex = 0;
		}
		Action onWardrobeUpdateDisplays = CosmeticWardrobe.OnWardrobeUpdateDisplays;
		if (onWardrobeUpdateDisplays == null)
		{
			return;
		}
		onWardrobeUpdateDisplays();
	}

	// Token: 0x06002360 RID: 9056 RVA: 0x000BE7FC File Offset: 0x000BC9FC
	private void HandlePressedPrevSelection(GorillaPressableButton button, bool isLeft)
	{
		CosmeticWardrobe.startingDisplayIndex -= this.cosmeticCollectionDisplays.Length;
		if (CosmeticWardrobe.startingDisplayIndex < 0)
		{
			int categorySize = CosmeticsController.instance.GetCategorySize(CosmeticWardrobe.selectedCategory);
			int num;
			if (categorySize % this.cosmeticCollectionDisplays.Length == 0)
			{
				num = categorySize - this.cosmeticCollectionDisplays.Length;
			}
			else
			{
				num = categorySize / this.cosmeticCollectionDisplays.Length;
				num *= this.cosmeticCollectionDisplays.Length;
			}
			CosmeticWardrobe.startingDisplayIndex = num;
		}
		Action onWardrobeUpdateDisplays = CosmeticWardrobe.OnWardrobeUpdateDisplays;
		if (onWardrobeUpdateDisplays == null)
		{
			return;
		}
		onWardrobeUpdateDisplays();
	}

	// Token: 0x06002361 RID: 9057 RVA: 0x000BE87C File Offset: 0x000BCA7C
	private void RepressButton(GorillaPressableButton button, bool isLeft, string itemName)
	{
		CosmeticWardrobe.<RepressButton>d__25 <RepressButton>d__;
		<RepressButton>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RepressButton>d__.<>4__this = this;
		<RepressButton>d__.button = button;
		<RepressButton>d__.isLeft = isLeft;
		<RepressButton>d__.itemName = itemName;
		<RepressButton>d__.<>1__state = -1;
		<RepressButton>d__.<>t__builder.Start<CosmeticWardrobe.<RepressButton>d__25>(ref <RepressButton>d__);
	}

	// Token: 0x06002362 RID: 9058 RVA: 0x000BE8CC File Offset: 0x000BCACC
	private void HandlePressedSelectCosmeticButton(GorillaPressableButton button, bool isLeft)
	{
		for (int i = 0; i < this.cosmeticCollectionDisplays.Length; i++)
		{
			if (this.cosmeticCollectionDisplays[i].selectButton == button)
			{
				if (string.IsNullOrEmpty(this.cosmeticCollectionDisplays[i].currentCosmeticItem.itemName) || this.cosmeticCollectionDisplays[i].currentCosmeticItem.itemName == "NOTHING")
				{
					return;
				}
				if (VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(this.cosmeticCollectionDisplays[i].currentCosmeticItem.itemName) == null)
				{
					this.RepressButton(button, isLeft, this.cosmeticCollectionDisplays[i].currentCosmeticItem.itemName);
				}
				else
				{
					CosmeticsController.instance.PressWardrobeItemButton(this.cosmeticCollectionDisplays[i].currentCosmeticItem, isLeft, this.m_useTemporarySet);
					if (isLeft)
					{
						this.cosmeticCategoryButtons[CosmeticWardrobe.selectedCategoryIndex].slot2RemovedItem = CosmeticsController.instance.nullItem;
						return;
					}
					this.cosmeticCategoryButtons[CosmeticWardrobe.selectedCategoryIndex].slot1RemovedItem = CosmeticsController.instance.nullItem;
					return;
				}
			}
		}
	}

	// Token: 0x06002363 RID: 9059 RVA: 0x000BE9E0 File Offset: 0x000BCBE0
	private void HandleChangeCategory(GorillaPressableButton button, bool isLeft)
	{
		for (int i = 0; i < this.cosmeticCategoryButtons.Length; i++)
		{
			CosmeticWardrobe.CosmeticWardrobeCategory cosmeticWardrobeCategory = this.cosmeticCategoryButtons[i];
			if (cosmeticWardrobeCategory.button == button)
			{
				if (CosmeticWardrobe.selectedCategory == cosmeticWardrobeCategory.category)
				{
					CosmeticsController.CosmeticItem cosmeticItem = CosmeticsController.instance.nullItem;
					if (cosmeticWardrobeCategory.slot1 != CosmeticsController.CosmeticSlots.Count)
					{
						cosmeticItem = CosmeticsController.instance.GetSlotItem(cosmeticWardrobeCategory.slot1, true, this.m_useTemporarySet);
					}
					CosmeticsController.CosmeticItem cosmeticItem2 = CosmeticsController.instance.nullItem;
					if (cosmeticWardrobeCategory.slot2 != CosmeticsController.CosmeticSlots.Count)
					{
						cosmeticItem2 = CosmeticsController.instance.GetSlotItem(cosmeticWardrobeCategory.slot2, true, this.m_useTemporarySet);
					}
					bool flag = CosmeticWardrobe.selectedCategory == CosmeticsController.CosmeticCategory.Arms;
					if (!cosmeticItem.isNullItem || !cosmeticItem2.isNullItem)
					{
						if (!cosmeticItem.isNullItem)
						{
							cosmeticWardrobeCategory.slot1RemovedItem = cosmeticItem;
							CosmeticsController.instance.PressWardrobeItemButton(cosmeticItem, flag, this.m_useTemporarySet);
						}
						if (!cosmeticItem2.isNullItem)
						{
							cosmeticWardrobeCategory.slot2RemovedItem = cosmeticItem2;
							CosmeticsController.instance.PressWardrobeItemButton(cosmeticItem2, !flag, this.m_useTemporarySet);
						}
						Action onWardrobeUpdateDisplays = CosmeticWardrobe.OnWardrobeUpdateDisplays;
						if (onWardrobeUpdateDisplays != null)
						{
							onWardrobeUpdateDisplays();
						}
						Action onWardrobeUpdateCategories = CosmeticWardrobe.OnWardrobeUpdateCategories;
						if (onWardrobeUpdateCategories == null)
						{
							return;
						}
						onWardrobeUpdateCategories();
						return;
					}
					else if (!cosmeticWardrobeCategory.slot1RemovedItem.isNullItem || !cosmeticWardrobeCategory.slot2RemovedItem.isNullItem)
					{
						if (!cosmeticWardrobeCategory.slot1RemovedItem.isNullItem)
						{
							CosmeticsController.instance.PressWardrobeItemButton(cosmeticWardrobeCategory.slot1RemovedItem, flag, this.m_useTemporarySet);
							cosmeticWardrobeCategory.slot1RemovedItem = CosmeticsController.instance.nullItem;
						}
						if (!cosmeticWardrobeCategory.slot2RemovedItem.isNullItem)
						{
							CosmeticsController.instance.PressWardrobeItemButton(cosmeticWardrobeCategory.slot2RemovedItem, !flag, this.m_useTemporarySet);
							cosmeticWardrobeCategory.slot2RemovedItem = CosmeticsController.instance.nullItem;
						}
						Action onWardrobeUpdateDisplays2 = CosmeticWardrobe.OnWardrobeUpdateDisplays;
						if (onWardrobeUpdateDisplays2 != null)
						{
							onWardrobeUpdateDisplays2();
						}
						Action onWardrobeUpdateCategories2 = CosmeticWardrobe.OnWardrobeUpdateCategories;
						if (onWardrobeUpdateCategories2 == null)
						{
							return;
						}
						onWardrobeUpdateCategories2();
						return;
					}
				}
				else
				{
					CosmeticWardrobe.selectedCategory = cosmeticWardrobeCategory.category;
					CosmeticWardrobe.selectedCategoryIndex = i;
					CosmeticWardrobe.startingDisplayIndex = 0;
					Action onWardrobeUpdateDisplays3 = CosmeticWardrobe.OnWardrobeUpdateDisplays;
					if (onWardrobeUpdateDisplays3 != null)
					{
						onWardrobeUpdateDisplays3();
					}
					Action onWardrobeUpdateCategories3 = CosmeticWardrobe.OnWardrobeUpdateCategories;
					if (onWardrobeUpdateCategories3 == null)
					{
						return;
					}
					onWardrobeUpdateCategories3();
				}
				return;
			}
		}
	}

	// Token: 0x06002364 RID: 9060 RVA: 0x000BEC04 File Offset: 0x000BCE04
	private void HandleCosmeticsUpdated()
	{
		string[] currentlyWornCosmetics = CosmeticsController.instance.GetCurrentlyWornCosmetics(this.m_useTemporarySet);
		bool[] currentRightEquippedSided = CosmeticsController.instance.GetCurrentRightEquippedSided(this.m_useTemporarySet);
		this.currentEquippedDisplay.SetCosmeticActiveArray(currentlyWornCosmetics, currentRightEquippedSided);
		this.UpdateCategoryButtons();
		this.UpdateCosmeticDisplays();
		this.UpdateOutfitButtons();
	}

	// Token: 0x06002365 RID: 9061 RVA: 0x000BEC58 File Offset: 0x000BCE58
	private void HandleLocalColorChanged(Color newColor)
	{
		MeshRenderer component = this.currentEquippedDisplay.GetComponent<MeshRenderer>();
		if (component != null)
		{
			component.material.color = newColor;
		}
	}

	// Token: 0x06002366 RID: 9062 RVA: 0x000BEC86 File Offset: 0x000BCE86
	private void HandlePressedPrevOutfitButton(GorillaPressableButton button, bool isLeft)
	{
		CosmeticsController.instance.PressWardrobeScrollOutfit(false);
	}

	// Token: 0x06002367 RID: 9063 RVA: 0x000BEC95 File Offset: 0x000BCE95
	private void HandlePressedNextOutfitButton(GorillaPressableButton button, bool isLeft)
	{
		CosmeticsController.instance.PressWardrobeScrollOutfit(true);
	}

	// Token: 0x06002368 RID: 9064 RVA: 0x000BECA4 File Offset: 0x000BCEA4
	private void UpdateCosmeticDisplays()
	{
		for (int i = 0; i < this.cosmeticCollectionDisplays.Length; i++)
		{
			CosmeticsController.CosmeticItem cosmetic = CosmeticsController.instance.GetCosmetic(CosmeticWardrobe.selectedCategory, CosmeticWardrobe.startingDisplayIndex + i);
			CosmeticWardrobe.CosmeticWardrobeSelection cosmeticWardrobeSelection = this.cosmeticCollectionDisplays[i];
			cosmeticWardrobeSelection.currentCosmeticItem = cosmetic;
			cosmeticWardrobeSelection.displayHead.SetCosmeticActive(cosmetic.displayName, false);
			cosmeticWardrobeSelection.selectButton.enabled = !cosmetic.isNullItem;
			cosmeticWardrobeSelection.selectButton.isOn = (!cosmetic.isNullItem && CosmeticsController.instance.IsCosmeticEquipped(cosmetic, this.m_useTemporarySet));
			cosmeticWardrobeSelection.selectButton.UpdateColor();
		}
		int categorySize = CosmeticsController.instance.GetCategorySize(CosmeticWardrobe.selectedCategory);
		this.nextSelection.enabled = (categorySize > this.cosmeticCollectionDisplays.Length);
		this.nextSelection.UpdateColor();
		this.prevSelection.enabled = (categorySize > this.cosmeticCollectionDisplays.Length);
		this.prevSelection.UpdateColor();
	}

	// Token: 0x06002369 RID: 9065 RVA: 0x000BEDA0 File Offset: 0x000BCFA0
	private void UpdateCategoryButtons()
	{
		for (int i = 0; i < this.cosmeticCategoryButtons.Length; i++)
		{
			CosmeticWardrobe.CosmeticWardrobeCategory cosmeticWardrobeCategory = this.cosmeticCategoryButtons[i];
			if (cosmeticWardrobeCategory.slot1 != CosmeticsController.CosmeticSlots.Count)
			{
				CosmeticsController.CosmeticItem slotItem = CosmeticsController.instance.GetSlotItem(cosmeticWardrobeCategory.slot1, false, this.m_useTemporarySet);
				if (cosmeticWardrobeCategory.slot2 != CosmeticsController.CosmeticSlots.Count)
				{
					CosmeticsController.CosmeticItem slotItem2 = CosmeticsController.instance.GetSlotItem(cosmeticWardrobeCategory.slot2, false, this.m_useTemporarySet);
					if (slotItem.bothHandsHoldable)
					{
						cosmeticWardrobeCategory.button.SetIcon(slotItem.isNullItem ? null : slotItem.itemPicture);
					}
					else if (slotItem2.bothHandsHoldable)
					{
						cosmeticWardrobeCategory.button.SetIcon(slotItem2.isNullItem ? null : slotItem2.itemPicture);
					}
					else
					{
						cosmeticWardrobeCategory.button.SetDualIcon(slotItem.isNullItem ? null : slotItem.itemPicture, slotItem2.isNullItem ? null : slotItem2.itemPicture);
					}
				}
				else
				{
					cosmeticWardrobeCategory.button.SetIcon(slotItem.isNullItem ? null : slotItem.itemPicture);
				}
			}
			int categorySize = CosmeticsController.instance.GetCategorySize(cosmeticWardrobeCategory.category);
			cosmeticWardrobeCategory.button.enabled = (categorySize > 0);
			cosmeticWardrobeCategory.button.isOn = (CosmeticWardrobe.selectedCategory == cosmeticWardrobeCategory.category);
			cosmeticWardrobeCategory.button.UpdateColor();
		}
	}

	// Token: 0x0600236A RID: 9066 RVA: 0x000BEF00 File Offset: 0x000BD100
	private void UpdateOutfitButtons()
	{
		bool enabled = CosmeticsController.CanScrollOutfits();
		int num = CosmeticsController.SelectedOutfit + 1;
		this.nextOutfit.enabled = enabled;
		this.previousOutfit.enabled = enabled;
		this.nextOutfit.UpdateColor();
		this.previousOutfit.UpdateColor();
		this.outfitText.text = "Outfit #" + num.ToString();
	}

	// Token: 0x0600236B RID: 9067 RVA: 0x000BEF68 File Offset: 0x000BD168
	public bool WardrobeButtonsInitialized()
	{
		for (int i = 0; i < this.cosmeticCategoryButtons.Length; i++)
		{
			if (!this.cosmeticCategoryButtons[i].button.Initialized)
			{
				return false;
			}
		}
		for (int i = 0; i < this.cosmeticCollectionDisplays.Length; i++)
		{
			if (!this.cosmeticCollectionDisplays[i].selectButton.Initialized)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04002E75 RID: 11893
	[SerializeField]
	private CosmeticWardrobe.CosmeticWardrobeSelection[] cosmeticCollectionDisplays;

	// Token: 0x04002E76 RID: 11894
	[SerializeField]
	private CosmeticWardrobe.CosmeticWardrobeCategory[] cosmeticCategoryButtons;

	// Token: 0x04002E77 RID: 11895
	[SerializeField]
	private HeadModel currentEquippedDisplay;

	// Token: 0x04002E78 RID: 11896
	[SerializeField]
	private GorillaPressableButton nextSelection;

	// Token: 0x04002E79 RID: 11897
	[SerializeField]
	private GorillaPressableButton prevSelection;

	// Token: 0x04002E7A RID: 11898
	[SerializeField]
	private bool m_useTemporarySet;

	// Token: 0x04002E7B RID: 11899
	[SerializeField]
	private CosmeticButton previousOutfit;

	// Token: 0x04002E7C RID: 11900
	[SerializeField]
	private CosmeticButton nextOutfit;

	// Token: 0x04002E7D RID: 11901
	[SerializeField]
	private TMP_Text outfitText;

	// Token: 0x04002E7E RID: 11902
	private static int selectedCategoryIndex = 0;

	// Token: 0x04002E7F RID: 11903
	private static CosmeticsController.CosmeticCategory selectedCategory = CosmeticsController.CosmeticCategory.Hat;

	// Token: 0x04002E80 RID: 11904
	private static int startingDisplayIndex = 0;

	// Token: 0x04002E81 RID: 11905
	private static int selectedOutfitIndex = 0;

	// Token: 0x04002E82 RID: 11906
	private static Action OnWardrobeUpdateCategories;

	// Token: 0x04002E83 RID: 11907
	private static Action OnWardrobeUpdateDisplays;

	// Token: 0x04002E84 RID: 11908
	public Vector3 startingHeadSize = new Vector3(0.25f, 0.25f, 0.25f);

	// Token: 0x02000570 RID: 1392
	[Serializable]
	public class CosmeticWardrobeSelection
	{
		// Token: 0x04002E85 RID: 11909
		public HeadModel displayHead;

		// Token: 0x04002E86 RID: 11910
		public CosmeticButton selectButton;

		// Token: 0x04002E87 RID: 11911
		public CosmeticsController.CosmeticItem currentCosmeticItem;
	}

	// Token: 0x02000571 RID: 1393
	[Serializable]
	public class CosmeticWardrobeCategory
	{
		// Token: 0x04002E88 RID: 11912
		public CosmeticCategoryButton button;

		// Token: 0x04002E89 RID: 11913
		public CosmeticsController.CosmeticCategory category;

		// Token: 0x04002E8A RID: 11914
		public CosmeticsController.CosmeticSlots slot1 = CosmeticsController.CosmeticSlots.Count;

		// Token: 0x04002E8B RID: 11915
		public CosmeticsController.CosmeticSlots slot2 = CosmeticsController.CosmeticSlots.Count;

		// Token: 0x04002E8C RID: 11916
		public CosmeticsController.CosmeticItem slot1RemovedItem;

		// Token: 0x04002E8D RID: 11917
		public CosmeticsController.CosmeticItem slot2RemovedItem;
	}
}
