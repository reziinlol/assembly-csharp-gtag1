using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio.Mods;
using TMPro;
using UnityEngine;

// Token: 0x02000AA0 RID: 2720
public class CustomMapsSearchScreen : CustomMapsTerminalScreen
{
	// Token: 0x06004573 RID: 17779 RVA: 0x0017781C File Offset: 0x00175A1C
	public override void Show()
	{
		base.Show();
		this.searchPhraseText.gameObject.SetActive(true);
		this.customMapsGalleryView.gameObject.SetActive(false);
		this.searchMessageText.gameObject.SetActive(false);
		this.leftPageButton.gameObject.SetActive(false);
		this.rightPageButton.gameObject.SetActive(false);
		this.searchedMods.Clear();
		this.filteredSearchedMods.Clear();
		this.displayedMods.Clear();
		this.searchPhraseText.text = this.defaultSearchString;
		this.searchPhrase = string.Empty;
		this.currentSearchModsRequestPage = 0;
		this.currentModPage = 0;
	}

	// Token: 0x06004574 RID: 17780 RVA: 0x001778CF File Offset: 0x00175ACF
	public override void Hide()
	{
		base.Hide();
		this.customMapsGalleryView.ShowTileText(false, true);
	}

	// Token: 0x06004575 RID: 17781 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void Initialize()
	{
	}

	// Token: 0x06004576 RID: 17782 RVA: 0x001778E4 File Offset: 0x00175AE4
	public void ReturnFromDetailsScreen()
	{
		base.Show();
		this.customMapsGalleryView.ShowTileText(true, true);
	}

	// Token: 0x06004577 RID: 17783 RVA: 0x001778FC File Offset: 0x00175AFC
	public override void PressButton(CustomMapKeyboardBinding pressedButton)
	{
		if (Time.time < this.showTime + this.activationTime)
		{
			return;
		}
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (CustomMapKeyboardBinding.tile1 <= pressedButton && pressedButton <= CustomMapKeyboardBinding.tile6 && !this.customMapsGalleryView.IsNull())
		{
			this.customMapsGalleryView.ShowDetailsForEntry(pressedButton - CustomMapKeyboardBinding.tile1);
		}
		if (pressedButton < CustomMapKeyboardBinding.up)
		{
			string str = this.searchPhrase;
			int num = (int)pressedButton;
			this.searchPhrase = str + num.ToString();
			this.RefreshSearchText();
			return;
		}
		if (pressedButton > CustomMapKeyboardBinding.option3 && pressedButton < CustomMapKeyboardBinding.at)
		{
			this.searchPhrase += pressedButton.ToString();
			this.RefreshSearchText();
			return;
		}
		if (pressedButton != CustomMapKeyboardBinding.delete)
		{
			if (pressedButton != CustomMapKeyboardBinding.enter)
			{
				switch (pressedButton)
				{
				case CustomMapKeyboardBinding.goback:
					if (this.loadingSearchMods)
					{
						return;
					}
					CustomMapsTerminal.ReturnFromSearchScreen();
					break;
				case CustomMapKeyboardBinding.left:
					this.currentModPage--;
					this.RefreshScreenState();
					break;
				case CustomMapKeyboardBinding.right:
					this.currentModPage++;
					this.RefreshScreenState();
					break;
				}
			}
			else
			{
				if (this.loadingSearchMods)
				{
					return;
				}
				this.searchedMods.Clear();
				this.filteredSearchedMods.Clear();
				this.currentSearchModsRequestPage = 0;
				this.searchMessageText.gameObject.SetActive(true);
				this.searchMessageText.text = this.searchingString;
				this.RetrieveMods();
			}
		}
		else if (!this.searchPhrase.IsNullOrEmpty())
		{
			this.searchPhrase = this.searchPhrase.Remove(this.searchPhrase.Length - 1);
		}
		this.RefreshSearchText();
	}

	// Token: 0x06004578 RID: 17784 RVA: 0x00177A89 File Offset: 0x00175C89
	private void RefreshSearchText()
	{
		if (this.searchPhrase.IsNullOrEmpty())
		{
			this.searchPhraseText.text = this.defaultSearchString;
			return;
		}
		this.searchPhraseText.text = this.searchPhrase;
	}

	// Token: 0x06004579 RID: 17785 RVA: 0x00177ABC File Offset: 0x00175CBC
	private Task RetrieveMods()
	{
		CustomMapsSearchScreen.<RetrieveMods>d__26 <RetrieveMods>d__;
		<RetrieveMods>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RetrieveMods>d__.<>4__this = this;
		<RetrieveMods>d__.<>1__state = -1;
		<RetrieveMods>d__.<>t__builder.Start<CustomMapsSearchScreen.<RetrieveMods>d__26>(ref <RetrieveMods>d__);
		return <RetrieveMods>d__.<>t__builder.Task;
	}

	// Token: 0x0600457A RID: 17786 RVA: 0x00177B00 File Offset: 0x00175D00
	private void FilterSearchMods()
	{
		if (this.searchedMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		this.filteredSearchedMods.Clear();
		foreach (Mod mod in this.searchedMods)
		{
			ModId right;
			if (ModIOManager.TryGetNewMapsModId(out right) && mod.Id == right)
			{
				this.totalSearchMods = Mathf.Max(0, this.totalSearchMods - 1);
			}
			else
			{
				this.filteredSearchedMods.Add(mod);
			}
		}
	}

	// Token: 0x0600457B RID: 17787 RVA: 0x00177BA0 File Offset: 0x00175DA0
	private void RefreshScreenState()
	{
		this.searchMessageText.gameObject.SetActive(false);
		this.customMapsGalleryView.ResetGallery();
		this.customMapsGalleryView.gameObject.SetActive(false);
		this.displayedMods.Clear();
		if (this.errorLoadingSearchMods)
		{
			this.searchMessageText.gameObject.SetActive(true);
			this.searchMessageText.text = this.errorMessage;
			this.leftPageButton.SetActive(false);
			this.rightPageButton.SetActive(false);
			return;
		}
		if (this.filteredSearchedMods.IsNullOrEmpty<Mod>())
		{
			this.searchMessageText.gameObject.SetActive(true);
			this.searchMessageText.text = this.noMapsFoundString;
			this.leftPageButton.SetActive(false);
			this.rightPageButton.SetActive(false);
			return;
		}
		int num = 0;
		int num2 = this.modsPerPage - 1;
		if (!this.IsOnFirstPage())
		{
			num = this.currentModPage * this.modsPerPage;
			num2 = num + this.modsPerPage - 1;
			this.leftPageButton.gameObject.SetActive(true);
		}
		else
		{
			this.leftPageButton.gameObject.SetActive(false);
		}
		if (!this.IsOnLastPage())
		{
			this.rightPageButton.gameObject.SetActive(true);
		}
		else
		{
			this.rightPageButton.gameObject.SetActive(false);
		}
		if (this.filteredSearchedMods.Count <= num2 && this.totalSearchMods > this.searchedMods.Count)
		{
			this.RetrieveMods();
			return;
		}
		int num3 = num;
		while (num3 <= num2 && this.filteredSearchedMods.Count > num3)
		{
			this.displayedMods.Add(this.filteredSearchedMods[num3]);
			num3++;
		}
		this.customMapsGalleryView.gameObject.SetActive(true);
		string text;
		if (!this.customMapsGalleryView.DisplayGallery(this.displayedMods, true, out text))
		{
			this.searchMessageText.gameObject.SetActive(true);
			this.searchMessageText.text = text;
			this.customMapsGalleryView.gameObject.SetActive(false);
			this.leftPageButton.SetActive(false);
			this.rightPageButton.SetActive(false);
		}
	}

	// Token: 0x0600457C RID: 17788 RVA: 0x00177DB4 File Offset: 0x00175FB4
	private int GetNumPages()
	{
		int num = this.totalSearchMods % this.modsPerPage;
		int num2 = this.totalSearchMods / this.modsPerPage;
		if (num > 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x0600457D RID: 17789 RVA: 0x00177DE4 File Offset: 0x00175FE4
	private bool IsOnFirstPage()
	{
		return this.currentModPage == 0;
	}

	// Token: 0x0600457E RID: 17790 RVA: 0x00177DF0 File Offset: 0x00175FF0
	private bool IsOnLastPage()
	{
		long num = (long)this.GetNumPages();
		return (long)(this.currentModPage + 1) == num;
	}

	// Token: 0x040057E2 RID: 22498
	[SerializeField]
	private TMP_Text searchPhraseText;

	// Token: 0x040057E3 RID: 22499
	[SerializeField]
	private TMP_Text searchMessageText;

	// Token: 0x040057E4 RID: 22500
	[SerializeField]
	private CustomMapsGalleryView customMapsGalleryView;

	// Token: 0x040057E5 RID: 22501
	[SerializeField]
	private GameObject leftPageButton;

	// Token: 0x040057E6 RID: 22502
	[SerializeField]
	private GameObject rightPageButton;

	// Token: 0x040057E7 RID: 22503
	[SerializeField]
	private string defaultSearchString = "SEARCH PHRASE";

	// Token: 0x040057E8 RID: 22504
	[SerializeField]
	private string noMapsFoundString = "NO RESULTS FOUND";

	// Token: 0x040057E9 RID: 22505
	[SerializeField]
	private string searchingString = "SEARCHING";

	// Token: 0x040057EA RID: 22506
	[SerializeField]
	private int numModsPerRequest = 60;

	// Token: 0x040057EB RID: 22507
	[SerializeField]
	private int modsPerPage = 6;

	// Token: 0x040057EC RID: 22508
	private string searchPhrase = "";

	// Token: 0x040057ED RID: 22509
	private List<Mod> searchedMods = new List<Mod>();

	// Token: 0x040057EE RID: 22510
	private List<Mod> filteredSearchedMods = new List<Mod>();

	// Token: 0x040057EF RID: 22511
	private List<Mod> displayedMods = new List<Mod>();

	// Token: 0x040057F0 RID: 22512
	private int currentSearchModsRequestPage;

	// Token: 0x040057F1 RID: 22513
	private bool loadingSearchMods;

	// Token: 0x040057F2 RID: 22514
	private bool errorLoadingSearchMods;

	// Token: 0x040057F3 RID: 22515
	private int totalSearchMods;

	// Token: 0x040057F4 RID: 22516
	private int currentModPage;

	// Token: 0x040057F5 RID: 22517
	private string errorMessage = "";
}
