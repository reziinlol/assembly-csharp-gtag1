using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio.Mods;
using Modio.Users;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A93 RID: 2707
public class CustomMapsListScreen : CustomMapsTerminalScreen
{
	// Token: 0x1700065D RID: 1629
	// (get) Token: 0x06004514 RID: 17684 RVA: 0x00174FAB File Offset: 0x001731AB
	public bool OfficialMapsOnly
	{
		get
		{
			return this.officialMapsOnly;
		}
	}

	// Token: 0x1700065E RID: 1630
	// (get) Token: 0x06004515 RID: 17685 RVA: 0x00174FB3 File Offset: 0x001731B3
	public int CurrentModPage
	{
		get
		{
			return this.currentModPage;
		}
	}

	// Token: 0x1700065F RID: 1631
	// (get) Token: 0x06004516 RID: 17686 RVA: 0x00174FBB File Offset: 0x001731BB
	public int ModsPerPage
	{
		get
		{
			return this.modsPerPage;
		}
	}

	// Token: 0x17000660 RID: 1632
	// (get) Token: 0x06004517 RID: 17687 RVA: 0x00174FC3 File Offset: 0x001731C3
	// (set) Token: 0x06004518 RID: 17688 RVA: 0x00174FCC File Offset: 0x001731CC
	public SortModsBy SortType
	{
		get
		{
			return this.sortType;
		}
		set
		{
			if (this.sortType != value)
			{
				this.currentAvailableModsRequestPage = 0;
			}
			this.sortType = value;
			switch (this.sortType)
			{
			case SortModsBy.Name:
				this.isAscendingOrder = true;
				return;
			case SortModsBy.Price:
				break;
			case SortModsBy.Rating:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.Popular:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.Downloads:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.Subscribers:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.DateSubmitted:
				this.isAscendingOrder = false;
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06004519 RID: 17689 RVA: 0x00175049 File Offset: 0x00173249
	private void Awake()
	{
		this.subscribedBttnPosition = this.subscribedMapsButton.transform.position;
		this.searchBttnPosition = this.searchButton.transform.position;
	}

	// Token: 0x0600451A RID: 17690 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void Initialize()
	{
	}

	// Token: 0x0600451B RID: 17691 RVA: 0x00175078 File Offset: 0x00173278
	public override void Show()
	{
		base.Show();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOUserChanged.AddListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOCacheRefreshing.RemoveListener(new UnityAction(this.OnModCacheRefreshing));
		ModIOManager.OnModIOCacheRefreshing.AddListener(new UnityAction(this.OnModCacheRefreshing));
		ModIOManager.OnModIOCacheRefreshed.RemoveListener(new UnityAction(this.OnModCacheRefreshed));
		ModIOManager.OnModIOCacheRefreshed.AddListener(new UnityAction(this.OnModCacheRefreshed));
		if (this.featuredMods.IsNullOrEmpty<Mod>())
		{
			this.RetrieveFeaturedMods();
		}
		if (this.availableMods.IsNullOrEmpty<Mod>())
		{
			this.RetrieveAvailableMods();
		}
		this.RetrieveInstalledMods(false);
		this.RetrieveFavoriteMods(false);
		this.RetrieveSubscribedMods();
		this.RefreshScreenState();
	}

	// Token: 0x0600451C RID: 17692 RVA: 0x001751AC File Offset: 0x001733AC
	public override void Hide()
	{
		base.Hide();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOCacheRefreshing.RemoveListener(new UnityAction(this.OnModCacheRefreshing));
		ModIOManager.OnModIOCacheRefreshed.RemoveListener(new UnityAction(this.OnModCacheRefreshed));
	}

	// Token: 0x0600451D RID: 17693 RVA: 0x0017522D File Offset: 0x0017342D
	private void OnModIOLoggedIn()
	{
		if (CustomMapsTerminal.IsDriver)
		{
			this.subscribedMapsButton.gameObject.SetActive(true);
		}
		this.subscribedMods = null;
		this.filteredSubscribedMods.Clear();
		this.totalSubscribedMods = 0;
		this.RetrieveSubscribedMods();
	}

	// Token: 0x0600451E RID: 17694 RVA: 0x00175267 File Offset: 0x00173467
	private void OnModIOLoggedOut()
	{
		this.subscribedMapsButton.gameObject.SetActive(false);
		this.subscribedMods = null;
		this.filteredSubscribedMods.Clear();
		this.totalSubscribedMods = 0;
	}

	// Token: 0x0600451F RID: 17695 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnModIOUserChanged(User user)
	{
	}

	// Token: 0x06004520 RID: 17696 RVA: 0x00175293 File Offset: 0x00173493
	private void OnModCacheRefreshing()
	{
		this.RefreshScreenState();
	}

	// Token: 0x06004521 RID: 17697 RVA: 0x0017529B File Offset: 0x0017349B
	private void OnModCacheRefreshed()
	{
		this.RetrieveFavoriteMods(false);
		this.RetrieveInstalledMods(false);
		if (ModIOManager.IsLoggedIn())
		{
			this.RetrieveSubscribedMods();
		}
	}

	// Token: 0x06004522 RID: 17698 RVA: 0x001752BC File Offset: 0x001734BC
	public override void PressButton(CustomMapKeyboardBinding buttonPressed)
	{
		if (Time.time < this.showTime + this.activationTime)
		{
			return;
		}
		GTDev.Log<string>("[CustomMapsListScreen::PressButton] Is Driver: " + CustomMapsTerminal.IsDriver.ToString() + ", Button Pressed: " + buttonPressed.ToString(), null);
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.goback)
		{
			return;
		}
		if (this.loadingText.gameObject.activeSelf)
		{
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.option3)
		{
			ModIOManager.RefreshUserProfile(delegate(bool result)
			{
				if (result)
				{
					this.Refresh();
				}
			}, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.option4)
		{
			CustomMapsTerminal.ShowSearchScreen();
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.up)
		{
			this.currentModPage--;
			this.RefreshScreenState();
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.down)
		{
			this.currentModPage++;
			this.RefreshScreenState();
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.all)
		{
			bool flag = this.officialMapsOnly;
			this.officialMapsOnly = false;
			this.displayFeaturedMods = (this.sortType == SortModsBy.Popular);
			if (flag)
			{
				this.RefreshModSearch();
			}
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.AvailableMods, flag);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.mustplay)
		{
			bool flag2 = !this.officialMapsOnly;
			this.officialMapsOnly = true;
			this.displayFeaturedMods = false;
			if (flag2)
			{
				this.RefreshModSearch();
			}
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.AvailableMods, flag2);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.sub)
		{
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.SubscribedMods, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.fav)
		{
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.FavoriteMods, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.inst)
		{
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.InstalledMods, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.sort)
		{
			this.SetSortType();
			this.RefreshModSearch();
			return;
		}
		if (CustomMapKeyboardBinding.one <= buttonPressed && buttonPressed <= CustomMapKeyboardBinding.nine && !this.customMapsGalleryView.IsNull())
		{
			this.customMapsGalleryView.ShowDetailsForEntry(buttonPressed - CustomMapKeyboardBinding.one);
		}
	}

	// Token: 0x06004523 RID: 17699 RVA: 0x0017544C File Offset: 0x0017364C
	private void SetSortType()
	{
		this.currentAvailableModsRequestPage = 0;
		this.sortTypeIndex++;
		if (this.sortTypeIndex >= 6)
		{
			this.sortTypeIndex = 0;
		}
		switch (this.sortTypeIndex)
		{
		case 0:
			this.SortType = SortModsBy.Popular;
			this.useMapName = true;
			this.displayFeaturedMods = !this.officialMapsOnly;
			return;
		case 1:
			this.SortType = SortModsBy.DateSubmitted;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		case 2:
			this.SortType = SortModsBy.Rating;
			this.useMapName = false;
			this.displayFeaturedMods = false;
			return;
		case 3:
			this.SortType = SortModsBy.Downloads;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		case 4:
			this.SortType = SortModsBy.Subscribers;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		case 5:
			this.SortType = SortModsBy.Name;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		default:
			this.sortTypeIndex = 0;
			this.SortType = SortModsBy.Popular;
			this.useMapName = true;
			this.displayFeaturedMods = !this.officialMapsOnly;
			return;
		}
	}

	// Token: 0x06004524 RID: 17700 RVA: 0x00175558 File Offset: 0x00173758
	public void SwapListDisplay(CustomMapsListScreen.ListScreenState newState, bool force = false)
	{
		if (this.currentState == newState && !force)
		{
			return;
		}
		if (newState == CustomMapsListScreen.ListScreenState.SubscribedMods && !ModIOManager.IsLoggedIn())
		{
			return;
		}
		this.currentState = newState;
		this.currentModPage = 0;
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			this.allMapsButton.SetButtonActive(!this.officialMapsOnly);
			this.officialMapsButton.SetButtonActive(this.officialMapsOnly);
			this.favoriteMapsButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			break;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			this.allMapsButton.SetButtonActive(false);
			this.officialMapsButton.SetButtonActive(false);
			this.favoriteMapsButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(true);
			break;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			this.allMapsButton.SetButtonActive(false);
			this.officialMapsButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			this.favoriteMapsButton.SetButtonActive(true);
			break;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			this.allMapsButton.SetButtonActive(false);
			this.officialMapsButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(false);
			this.favoriteMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(true);
			break;
		}
		this.RefreshScreenState();
	}

	// Token: 0x06004525 RID: 17701 RVA: 0x001756F0 File Offset: 0x001738F0
	public void RefreshModSearch()
	{
		if (this.loadingAvailableMods || this.loadingFavoriteMods || this.loadingInstalledMods || this.loadingSubscribedMods)
		{
			return;
		}
		this.currentModPage = 0;
		this.availableMods.Clear();
		this.filteredAvailableMods.Clear();
		this.currentAvailableModsRequestPage = 0;
		this.errorLoadingAvailableMods = false;
		this.totalAvailableMods = 0;
		this.RetrieveAvailableMods();
	}

	// Token: 0x06004526 RID: 17702 RVA: 0x00175758 File Offset: 0x00173958
	public void Refresh()
	{
		if (this.loadingAvailableMods || this.loadingFavoriteMods || this.loadingFeaturedMods || this.loadingInstalledMods || this.loadingSubscribedMods)
		{
			return;
		}
		this.currentModPage = 0;
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			this.featuredMods.Clear();
			this.availableMods.Clear();
			this.filteredAvailableMods.Clear();
			this.currentAvailableModsRequestPage = 0;
			this.errorLoadingAvailableMods = false;
			this.totalAvailableMods = 0;
			this.RetrieveFeaturedMods();
			this.RetrieveAvailableMods();
			return;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			this.RetrieveInstalledMods(true);
			return;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			this.RetrieveFavoriteMods(true);
			return;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			this.RetrieveSubscribedMods();
			return;
		default:
			return;
		}
	}

	// Token: 0x06004527 RID: 17703 RVA: 0x00175810 File Offset: 0x00173A10
	private void RetrieveFeaturedMods()
	{
		if (this.loadingFeaturedMods || this.featuredMods.Count > 0)
		{
			return;
		}
		this.loadingFeaturedMods = true;
		PlayFabTitleDataCache.Instance.GetTitleData(this.featuredModsPlayFabKey, new Action<string>(this.OnGetFeaturedModsTitleData), delegate(PlayFabError error)
		{
			this.loadingFeaturedMods = false;
			this.RefreshScreenState();
		}, false);
	}

	// Token: 0x06004528 RID: 17704 RVA: 0x00175864 File Offset: 0x00173A64
	private void OnGetFeaturedModsTitleData(string data)
	{
		CustomMapsListScreen.<OnGetFeaturedModsTitleData>d__102 <OnGetFeaturedModsTitleData>d__;
		<OnGetFeaturedModsTitleData>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnGetFeaturedModsTitleData>d__.<>4__this = this;
		<OnGetFeaturedModsTitleData>d__.data = data;
		<OnGetFeaturedModsTitleData>d__.<>1__state = -1;
		<OnGetFeaturedModsTitleData>d__.<>t__builder.Start<CustomMapsListScreen.<OnGetFeaturedModsTitleData>d__102>(ref <OnGetFeaturedModsTitleData>d__);
	}

	// Token: 0x06004529 RID: 17705 RVA: 0x001758A4 File Offset: 0x00173AA4
	private void RetrieveAvailableMods()
	{
		CustomMapsListScreen.<RetrieveAvailableMods>d__103 <RetrieveAvailableMods>d__;
		<RetrieveAvailableMods>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RetrieveAvailableMods>d__.<>4__this = this;
		<RetrieveAvailableMods>d__.<>1__state = -1;
		<RetrieveAvailableMods>d__.<>t__builder.Start<CustomMapsListScreen.<RetrieveAvailableMods>d__103>(ref <RetrieveAvailableMods>d__);
	}

	// Token: 0x0600452A RID: 17706 RVA: 0x001758DC File Offset: 0x00173ADC
	private void FilterAvailableMods()
	{
		this.filteredAvailableMods.Clear();
		if (this.availableMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		this.totalAvailableMods = Mathf.Max(0, this.totalAvailableMods - 1);
		foreach (Mod mod in this.availableMods)
		{
			ModId right;
			ModIOManager.TryGetNewMapsModId(out right);
			if (!(mod.Id == right) && (!this.displayFeaturedMods || this.featuredModIds.IsNullOrEmpty<long>() || !this.featuredModIds.Contains(mod.Id)))
			{
				this.filteredAvailableMods.Add(mod);
			}
		}
		if (this.displayFeaturedMods && !this.featuredMods.IsNullOrEmpty<Mod>())
		{
			this.filteredAvailableMods.InsertRange(0, this.featuredMods);
		}
	}

	// Token: 0x0600452B RID: 17707 RVA: 0x001759CC File Offset: 0x00173BCC
	private Task RetrieveSubscribedMods()
	{
		CustomMapsListScreen.<RetrieveSubscribedMods>d__105 <RetrieveSubscribedMods>d__;
		<RetrieveSubscribedMods>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RetrieveSubscribedMods>d__.<>4__this = this;
		<RetrieveSubscribedMods>d__.<>1__state = -1;
		<RetrieveSubscribedMods>d__.<>t__builder.Start<CustomMapsListScreen.<RetrieveSubscribedMods>d__105>(ref <RetrieveSubscribedMods>d__);
		return <RetrieveSubscribedMods>d__.<>t__builder.Task;
	}

	// Token: 0x0600452C RID: 17708 RVA: 0x00175A10 File Offset: 0x00173C10
	private void FilterSubscribedMods()
	{
		this.filteredSubscribedMods.Clear();
		if (this.subscribedMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		foreach (Mod mod in this.subscribedMods)
		{
			ModId right;
			ModIOManager.TryGetNewMapsModId(out right);
			if (!(mod.Id == right))
			{
				this.filteredSubscribedMods.Add(mod);
			}
		}
	}

	// Token: 0x0600452D RID: 17709 RVA: 0x00175A74 File Offset: 0x00173C74
	private Task RetrieveInstalledMods(bool forceRefresh = false)
	{
		CustomMapsListScreen.<RetrieveInstalledMods>d__107 <RetrieveInstalledMods>d__;
		<RetrieveInstalledMods>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RetrieveInstalledMods>d__.<>4__this = this;
		<RetrieveInstalledMods>d__.forceRefresh = forceRefresh;
		<RetrieveInstalledMods>d__.<>1__state = -1;
		<RetrieveInstalledMods>d__.<>t__builder.Start<CustomMapsListScreen.<RetrieveInstalledMods>d__107>(ref <RetrieveInstalledMods>d__);
		return <RetrieveInstalledMods>d__.<>t__builder.Task;
	}

	// Token: 0x0600452E RID: 17710 RVA: 0x00175AC0 File Offset: 0x00173CC0
	private void FilterInstalledMods()
	{
		this.filteredInstalledMods.Clear();
		if (this.installedMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		foreach (Mod mod in this.installedMods)
		{
			ModId right;
			if (!ModIOManager.TryGetNewMapsModId(out right) || !(mod.Id == right))
			{
				this.filteredInstalledMods.Add(mod);
			}
		}
	}

	// Token: 0x0600452F RID: 17711 RVA: 0x00175B24 File Offset: 0x00173D24
	private Task RetrieveFavoriteMods(bool forceRefresh = false)
	{
		CustomMapsListScreen.<RetrieveFavoriteMods>d__109 <RetrieveFavoriteMods>d__;
		<RetrieveFavoriteMods>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RetrieveFavoriteMods>d__.<>4__this = this;
		<RetrieveFavoriteMods>d__.forceRefresh = forceRefresh;
		<RetrieveFavoriteMods>d__.<>1__state = -1;
		<RetrieveFavoriteMods>d__.<>t__builder.Start<CustomMapsListScreen.<RetrieveFavoriteMods>d__109>(ref <RetrieveFavoriteMods>d__);
		return <RetrieveFavoriteMods>d__.<>t__builder.Task;
	}

	// Token: 0x06004530 RID: 17712 RVA: 0x00175B70 File Offset: 0x00173D70
	private void FilterFavoriteMods()
	{
		this.filteredFavoriteMods.Clear();
		if (this.favoriteMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		foreach (Mod mod in this.favoriteMods)
		{
			ModId right;
			if (!ModIOManager.TryGetNewMapsModId(out right) || !(mod.Id == right))
			{
				this.filteredFavoriteMods.Add(mod);
			}
		}
	}

	// Token: 0x06004531 RID: 17713 RVA: 0x00175BF8 File Offset: 0x00173DF8
	public void GetDisplayedModList(out long[] modList)
	{
		if (this.displayedModProfiles.IsNullOrEmpty<Mod>())
		{
			modList = Array.Empty<long>();
			return;
		}
		modList = new long[this.displayedModProfiles.Count];
		for (int i = 0; i < this.displayedModProfiles.Count; i++)
		{
			modList[i] = this.displayedModProfiles[i].Id;
		}
	}

	// Token: 0x06004532 RID: 17714 RVA: 0x00175C5C File Offset: 0x00173E5C
	private void RefreshScreenState()
	{
		this.displayedModProfiles.Clear();
		this.errorText.gameObject.SetActive(false);
		this.sortTypeText.gameObject.SetActive(false);
		this.modPageText.gameObject.SetActive(false);
		this.titleText.text = this.GetTitleForCurrentState();
		this.loadingText.gameObject.SetActive(true);
		if (CustomMapsTerminal.IsDriver && ModIOManager.IsLoggedIn())
		{
			this.subscribedMapsButton.gameObject.SetActive(true);
			this.subscribedMapsButton.transform.position = this.subscribedBttnPosition;
			this.searchButton.transform.position = this.searchBttnPosition;
		}
		else
		{
			this.subscribedMapsButton.gameObject.SetActive(false);
			this.subscribedMapsButton.transform.position = this.searchBttnPosition;
			this.searchButton.transform.position = this.subscribedBttnPosition;
		}
		if (this.currentState == CustomMapsListScreen.ListScreenState.AvailableMods)
		{
			this.RefreshScreenForAvailableMods();
			return;
		}
		this.sortByButton.SetActive(false);
		this.RefreshScreenForCurrentState();
	}

	// Token: 0x06004533 RID: 17715 RVA: 0x00175D74 File Offset: 0x00173F74
	private void RefreshScreenForAvailableMods()
	{
		string text = (this.sortType == SortModsBy.DateSubmitted) ? "NEWEST" : this.sortType.ToString().ToUpper();
		this.sortByButton.SetActive(true);
		this.sortTypeText.gameObject.SetActive(true);
		this.sortTypeText.text = text;
		this.customMapsGalleryView.ResetGallery();
		if (this.loadingAvailableMods)
		{
			return;
		}
		if (this.errorLoadingAvailableMods)
		{
			this.errorText.text = this.failedToRetrieveModsString;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.UpdatePageCount(this.totalAvailableMods);
		int num = 0;
		int num2 = this.modsPerPage - 1;
		if (!this.IsOnFirstPage())
		{
			num = this.currentModPage * this.modsPerPage;
			num2 = num + this.modsPerPage - 1;
			this.pageUpButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageUpButton.gameObject.SetActive(false);
		}
		if (!this.IsOnLastPage())
		{
			this.pageDownButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageDownButton.gameObject.SetActive(false);
		}
		if (this.filteredAvailableMods.Count <= num2 && this.totalAvailableMods > this.availableMods.Count)
		{
			this.displayedModProfiles.Clear();
			this.RetrieveAvailableMods();
			return;
		}
		int num3 = num;
		while (num3 <= num2 && this.filteredAvailableMods.Count > num3)
		{
			this.displayedModProfiles.Add(this.filteredAvailableMods[num3]);
			num3++;
		}
		string text2;
		if (!this.customMapsGalleryView.DisplayGallery(this.displayedModProfiles, this.useMapName, out text2))
		{
			this.errorText.text = text2;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		if (this.displayFeaturedMods && !this.featuredModIds.IsNullOrEmpty<long>())
		{
			for (int i = 0; i < this.displayedModProfiles.Count; i++)
			{
				if (this.featuredModIds.Contains(this.displayedModProfiles[i].Id))
				{
					this.customMapsGalleryView.HighlightTileAtIndex(num + i);
				}
			}
		}
		this.loadingText.gameObject.SetActive(false);
	}

	// Token: 0x06004534 RID: 17716 RVA: 0x00175FD0 File Offset: 0x001741D0
	private void RefreshScreenForCurrentState()
	{
		this.customMapsGalleryView.ResetGallery();
		if (this.GetLoadingStatusForCurrentState())
		{
			return;
		}
		if (this.HasModLoadingErrorForCurrentState())
		{
			this.modPageText.gameObject.SetActive(false);
			if (CustomMapsTerminal.IsDriver)
			{
				this.currentModPage = -1;
			}
			this.errorText.text = this.failedToRetrieveModsString;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.UpdatePageCount(this.GetTotalModsForCurrentState());
		if (!this.IsOnFirstPage())
		{
			this.pageUpButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageUpButton.gameObject.SetActive(false);
		}
		if (!this.IsOnLastPage())
		{
			this.pageDownButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageDownButton.gameObject.SetActive(false);
		}
		List<Mod> modListForCurrentState = this.GetModListForCurrentState();
		if (modListForCurrentState != null)
		{
			if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList)
			{
				this.displayedModProfiles.AddRange(modListForCurrentState);
			}
			else
			{
				int num = this.currentModPage * this.modsPerPage;
				int num2 = num;
				while (num2 < num + this.modsPerPage && modListForCurrentState.Count > num2)
				{
					this.displayedModProfiles.Add(modListForCurrentState[num2]);
					num2++;
				}
			}
		}
		string text;
		if (!this.customMapsGalleryView.DisplayGallery(this.displayedModProfiles, true, out text))
		{
			this.errorText.text = text;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.loadingText.gameObject.SetActive(false);
	}

	// Token: 0x06004535 RID: 17717 RVA: 0x00176164 File Offset: 0x00174364
	private bool GetLoadingStatusForCurrentState()
	{
		if (ModIOManager.IsRefreshing())
		{
			return true;
		}
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.loadingAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.loadingInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.loadingFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.loadingSubscribedMods;
		default:
			return false;
		}
	}

	// Token: 0x06004536 RID: 17718 RVA: 0x001761B8 File Offset: 0x001743B8
	private bool HasModLoadingErrorForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.errorLoadingAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.errorLoadingInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.errorLoadingFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.errorLoadingSubscribedMods;
		default:
			return false;
		}
	}

	// Token: 0x06004537 RID: 17719 RVA: 0x00176204 File Offset: 0x00174404
	private List<Mod> GetModListForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.filteredAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.filteredInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.filteredFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.filteredSubscribedMods;
		default:
			return null;
		}
	}

	// Token: 0x06004538 RID: 17720 RVA: 0x00176250 File Offset: 0x00174450
	private int GetTotalModsForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.totalAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.totalInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.totalFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.totalSubscribedMods;
		default:
			return 0;
		}
	}

	// Token: 0x06004539 RID: 17721 RVA: 0x0017629C File Offset: 0x0017449C
	private string GetTitleForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			if (this.officialMapsOnly)
			{
				return this.officialModsTitle;
			}
			return this.browseModsTitle;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.installedModsTitle;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.favoriteModsTitle;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.subscribedModsTitle;
		default:
			return "";
		}
	}

	// Token: 0x0600453A RID: 17722 RVA: 0x001762F8 File Offset: 0x001744F8
	private void UpdatePageCount(int totalMods)
	{
		this.totalModCount = totalMods;
		this.modPageText.gameObject.SetActive(false);
		if (this.totalModCount != 0)
		{
			int numPages = this.GetNumPages();
			if (numPages > 1)
			{
				this.modPageText.text = string.Format("{0} / {1}", this.currentModPage + 1, numPages);
				this.modPageText.gameObject.SetActive(true);
			}
			return;
		}
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			this.errorText.text = this.noModsAvailableString;
			return;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			this.errorText.text = this.noInstalledModsString;
			return;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			this.errorText.text = this.noFavoriteModsString;
			return;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			this.errorText.text = this.noSubscribedModsString;
			return;
		case CustomMapsListScreen.ListScreenState.CustomModList:
			this.errorText.text = this.noModsFoundGenericString;
			return;
		default:
			return;
		}
	}

	// Token: 0x0600453B RID: 17723 RVA: 0x001763E8 File Offset: 0x001745E8
	public int GetNumPages()
	{
		int num = this.totalModCount % this.modsPerPage;
		int num2 = this.totalModCount / this.modsPerPage;
		if (num > 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x0600453C RID: 17724 RVA: 0x00176418 File Offset: 0x00174618
	private bool IsOnFirstPage()
	{
		return this.currentModPage == 0;
	}

	// Token: 0x0600453D RID: 17725 RVA: 0x00176424 File Offset: 0x00174624
	private bool IsOnLastPage()
	{
		long num = (long)this.GetNumPages();
		return (long)(this.currentModPage + 1) == num;
	}

	// Token: 0x0600453E RID: 17726 RVA: 0x00176448 File Offset: 0x00174648
	public void RefreshDriverNickname(string driverNickname)
	{
		if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList)
		{
			this.titleText.text = driverNickname;
		}
	}

	// Token: 0x0400574C RID: 22348
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x0400574D RID: 22349
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x0400574E RID: 22350
	[SerializeField]
	private TMP_Text modPageText;

	// Token: 0x0400574F RID: 22351
	[SerializeField]
	private TMP_Text titleText;

	// Token: 0x04005750 RID: 22352
	[SerializeField]
	private TMP_Text sortTypeText;

	// Token: 0x04005751 RID: 22353
	[SerializeField]
	private GameObject sortByButton;

	// Token: 0x04005752 RID: 22354
	[SerializeField]
	private CustomMapsScreenButton allMapsButton;

	// Token: 0x04005753 RID: 22355
	[SerializeField]
	private CustomMapsScreenButton officialMapsButton;

	// Token: 0x04005754 RID: 22356
	[SerializeField]
	private CustomMapsScreenButton favoriteMapsButton;

	// Token: 0x04005755 RID: 22357
	[SerializeField]
	private CustomMapsScreenButton installedMapsButton;

	// Token: 0x04005756 RID: 22358
	[SerializeField]
	private CustomMapsScreenButton subscribedMapsButton;

	// Token: 0x04005757 RID: 22359
	[SerializeField]
	private CustomMapsScreenButton searchButton;

	// Token: 0x04005758 RID: 22360
	[SerializeField]
	private CustomMapsScreenButton pageUpButton;

	// Token: 0x04005759 RID: 22361
	[SerializeField]
	private CustomMapsScreenButton pageDownButton;

	// Token: 0x0400575A RID: 22362
	[SerializeField]
	private CustomMapsGalleryView customMapsGalleryView;

	// Token: 0x0400575B RID: 22363
	[SerializeField]
	private string browseModsTitle = "AVAILABLE MODS";

	// Token: 0x0400575C RID: 22364
	[SerializeField]
	private string officialModsTitle = "OFFICIAL MODS";

	// Token: 0x0400575D RID: 22365
	[SerializeField]
	private string installedModsTitle = "INSTALLED MODS";

	// Token: 0x0400575E RID: 22366
	[SerializeField]
	private string favoriteModsTitle = "FAVORITE MODS";

	// Token: 0x0400575F RID: 22367
	[SerializeField]
	private string subscribedModsTitle = "SUBSCRIBED MODS";

	// Token: 0x04005760 RID: 22368
	[SerializeField]
	private string noModsAvailableString = "NO MODS AVAILABLE";

	// Token: 0x04005761 RID: 22369
	[SerializeField]
	private string noModsFoundGenericString = "NO MODS FOUND";

	// Token: 0x04005762 RID: 22370
	[SerializeField]
	private string noSubscribedModsString = "NOT SUBSCRIBED TO ANY MODS";

	// Token: 0x04005763 RID: 22371
	[SerializeField]
	private string noInstalledModsString = "NO MODS INSTALLED";

	// Token: 0x04005764 RID: 22372
	[SerializeField]
	private string noFavoriteModsString = "NO FAVORITE MODS FOUND";

	// Token: 0x04005765 RID: 22373
	[SerializeField]
	private string failedToRetrieveModsString = "FAILED TO RETRIEVE MODS FROM MOD.IO \nPRESS THE 'REFRESH' BUTTON TO RETRY";

	// Token: 0x04005766 RID: 22374
	[SerializeField]
	private int modsPerPage = 12;

	// Token: 0x04005767 RID: 22375
	[SerializeField]
	private int numModsPerRequest = 24;

	// Token: 0x04005768 RID: 22376
	[SerializeField]
	private int maxModListItemLength = 25;

	// Token: 0x04005769 RID: 22377
	[SerializeField]
	private string officialMapsTag = "Official Maps";

	// Token: 0x0400576A RID: 22378
	[SerializeField]
	private string featuredModsPlayFabKey = "VStumpFeaturedMaps";

	// Token: 0x0400576B RID: 22379
	private bool loadingFeaturedMods;

	// Token: 0x0400576C RID: 22380
	private bool displayFeaturedMods = true;

	// Token: 0x0400576D RID: 22381
	private int totalFeaturedMods;

	// Token: 0x0400576E RID: 22382
	private List<long> featuredModIds = new List<long>();

	// Token: 0x0400576F RID: 22383
	private List<Mod> featuredMods = new List<Mod>();

	// Token: 0x04005770 RID: 22384
	private int currentAvailableModsRequestPage;

	// Token: 0x04005771 RID: 22385
	private bool loadingAvailableMods;

	// Token: 0x04005772 RID: 22386
	private int totalAvailableMods;

	// Token: 0x04005773 RID: 22387
	private bool errorLoadingAvailableMods;

	// Token: 0x04005774 RID: 22388
	private List<Mod> availableMods = new List<Mod>();

	// Token: 0x04005775 RID: 22389
	private List<Mod> filteredAvailableMods = new List<Mod>();

	// Token: 0x04005776 RID: 22390
	private bool loadingInstalledMods;

	// Token: 0x04005777 RID: 22391
	private bool errorLoadingInstalledMods;

	// Token: 0x04005778 RID: 22392
	private int totalInstalledMods;

	// Token: 0x04005779 RID: 22393
	private Mod[] installedMods;

	// Token: 0x0400577A RID: 22394
	private List<Mod> filteredInstalledMods = new List<Mod>();

	// Token: 0x0400577B RID: 22395
	private bool loadingFavoriteMods;

	// Token: 0x0400577C RID: 22396
	private bool errorLoadingFavoriteMods;

	// Token: 0x0400577D RID: 22397
	private int totalFavoriteMods;

	// Token: 0x0400577E RID: 22398
	private List<Mod> favoriteMods = new List<Mod>();

	// Token: 0x0400577F RID: 22399
	private List<Mod> filteredFavoriteMods = new List<Mod>();

	// Token: 0x04005780 RID: 22400
	private bool loadingSubscribedMods;

	// Token: 0x04005781 RID: 22401
	private bool errorLoadingSubscribedMods;

	// Token: 0x04005782 RID: 22402
	private int totalSubscribedMods;

	// Token: 0x04005783 RID: 22403
	private Mod[] subscribedMods;

	// Token: 0x04005784 RID: 22404
	private List<Mod> filteredSubscribedMods = new List<Mod>();

	// Token: 0x04005785 RID: 22405
	private int currentModPage;

	// Token: 0x04005786 RID: 22406
	private int totalModCount;

	// Token: 0x04005787 RID: 22407
	private List<Mod> displayedModProfiles = new List<Mod>();

	// Token: 0x04005788 RID: 22408
	private int sortTypeIndex;

	// Token: 0x04005789 RID: 22409
	private SortModsBy sortType = SortModsBy.Popular;

	// Token: 0x0400578A RID: 22410
	private const int MAX_SORT_TYPES = 6;

	// Token: 0x0400578B RID: 22411
	private List<string> searchTags = new List<string>();

	// Token: 0x0400578C RID: 22412
	private bool isAscendingOrder;

	// Token: 0x0400578D RID: 22413
	private bool officialMapsOnly;

	// Token: 0x0400578E RID: 22414
	private bool useMapName = true;

	// Token: 0x0400578F RID: 22415
	private Vector3 subscribedBttnPosition;

	// Token: 0x04005790 RID: 22416
	private Vector3 searchBttnPosition;

	// Token: 0x04005791 RID: 22417
	private bool restartCustomModListRetrieval;

	// Token: 0x04005792 RID: 22418
	private bool restartCustomModListRetrievalForceRefresh;

	// Token: 0x04005793 RID: 22419
	private bool restartInstalledModsRetrieval;

	// Token: 0x04005794 RID: 22420
	private bool restartInstalledModsRetrievalForceRefresh;

	// Token: 0x04005795 RID: 22421
	private bool restartFavoriteModsRetrieval;

	// Token: 0x04005796 RID: 22422
	private bool restartFavoriteModsRetrievalForceRefresh;

	// Token: 0x04005797 RID: 22423
	private bool restartSubscribedModsRetrieval;

	// Token: 0x04005798 RID: 22424
	public CustomMapsListScreen.ListScreenState currentState;

	// Token: 0x02000A94 RID: 2708
	public enum ListScreenState
	{
		// Token: 0x0400579A RID: 22426
		AvailableMods,
		// Token: 0x0400579B RID: 22427
		InstalledMods,
		// Token: 0x0400579C RID: 22428
		FavoriteMods,
		// Token: 0x0400579D RID: 22429
		SubscribedMods,
		// Token: 0x0400579E RID: 22430
		CustomModList
	}
}
