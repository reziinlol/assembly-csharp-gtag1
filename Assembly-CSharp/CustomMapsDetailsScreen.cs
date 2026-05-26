using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio;
using Modio.Mods;
using Modio.Users;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A89 RID: 2697
public class CustomMapsDetailsScreen : CustomMapsTerminalScreen
{
	// Token: 0x1700065A RID: 1626
	// (get) Token: 0x060044B4 RID: 17588 RVA: 0x00171FD0 File Offset: 0x001701D0
	// (set) Token: 0x060044B3 RID: 17587 RVA: 0x00171FC7 File Offset: 0x001701C7
	public Mod currentMapMod { get; private set; }

	// Token: 0x060044B5 RID: 17589 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void Initialize()
	{
	}

	// Token: 0x060044B6 RID: 17590 RVA: 0x00171FD8 File Offset: 0x001701D8
	public override void Show()
	{
		base.Show();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOUserChanged.AddListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
		ModIOManager.OnModManagementEvent.AddListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
		CustomMapManager.OnMapLoadStatusChanged.RemoveListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadStatusChanged.AddListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnRoomMapChanged.AddListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnMapUnloadComplete.RemoveListener(new UnityAction(this.OnMapUnloaded));
		CustomMapManager.OnMapUnloadComplete.AddListener(new UnityAction(this.OnMapUnloaded));
		if (!ModIOManager.IsLoggedIn())
		{
			this.subscriptionToggleButton.gameObject.SetActive(false);
		}
		this.deleteButton.gameObject.SetActive(false);
		this.ResetToDefaultView();
	}

	// Token: 0x060044B7 RID: 17591 RVA: 0x0017217C File Offset: 0x0017037C
	public override void Hide()
	{
		base.Hide();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
		CustomMapManager.OnMapLoadStatusChanged.RemoveListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnMapUnloadComplete.RemoveListener(new UnityAction(this.OnMapUnloaded));
	}

	// Token: 0x060044B8 RID: 17592 RVA: 0x00172240 File Offset: 0x00170440
	private void OnModUpdated()
	{
		ModRating currentUserRating = this.currentMapMod.CurrentUserRating;
		this.rateUpButton.SetButtonActive(currentUserRating == ModRating.Positive);
		this.rateDownButton.SetButtonActive(currentUserRating == ModRating.Negative);
	}

	// Token: 0x060044B9 RID: 17593 RVA: 0x00172277 File Offset: 0x00170477
	private void OnModIOLoggedIn()
	{
		if (this.currentMapMod.Creator == null)
		{
			this.RefreshCurrentMapMod();
			return;
		}
		if (this.currentMapMod.IsHidden())
		{
			this.UpdateMapDetails(true);
			return;
		}
		this.UpdateStatus(false);
	}

	// Token: 0x060044BA RID: 17594 RVA: 0x001722B0 File Offset: 0x001704B0
	private void OnModIOLoggedOut()
	{
		if (this.currentMapMod.IsHidden())
		{
			this.UpdateMapDetails(true);
			return;
		}
		this.UpdateStatus(false);
	}

	// Token: 0x060044BB RID: 17595 RVA: 0x001722CF File Offset: 0x001704CF
	private void OnModIOUserChanged(User user)
	{
		this.UpdateStatus(false);
	}

	// Token: 0x060044BC RID: 17596 RVA: 0x001722DC File Offset: 0x001704DC
	private void HandleModManagementEvent(Mod mod, Modfile modfile, ModInstallationManagement.OperationType jobType, ModInstallationManagement.OperationPhase jobPhase)
	{
		if (base.isActiveAndEnabled && this.hasModProfile && this.GetModId() == mod.Id)
		{
			this.UpdateStatus(jobPhase == ModInstallationManagement.OperationPhase.Cancelled || jobPhase == ModInstallationManagement.OperationPhase.Failed);
			if (jobPhase == ModInstallationManagement.OperationPhase.Failed)
			{
				this.modDescriptionText.gameObject.SetActive(false);
				this.loadingMapLabelText.text = this.mapLoadingErrorString;
				this.loadingMapLabelText.gameObject.SetActive(true);
				this.loadingMapMessageText.text = this.mapLoadingErrorInvalidModFile;
				this.loadingMapMessageText.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060044BD RID: 17597 RVA: 0x00172380 File Offset: 0x00170580
	private void Update()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		string str;
		if (this.GetModId().IsValid() && ModInstallationManagement.CurrentOperationOnMod != null && ModInstallationManagement.CurrentOperationOnMod.Id == this.GetModId() && ModInstallationManagement.CurrentOperationOnMod.File.State != ModFileState.Installed && CustomMapsDetailsScreen.modStatusStrings.TryGetValue(ModInstallationManagement.CurrentOperationOnMod.File.State, out str))
		{
			float f = this.currentMapMod.File.FileStateProgress * 100f;
			this.modStatusText.text = str + string.Format(" {0}%", Mathf.RoundToInt(f));
		}
	}

	// Token: 0x060044BE RID: 17598 RVA: 0x00172438 File Offset: 0x00170638
	public void RetrieveModFromModIO(long id, bool forceUpdate = false, Action<Error, Mod> callback = null)
	{
		if (this.hasModProfile && this.GetModId()._id == id)
		{
			this.UpdateMapDetails(true);
			return;
		}
		this.pendingModId = id;
		ModIOManager.GetMod(new ModId(id), forceUpdate, (callback != null) ? callback : new Action<Error, Mod>(this.OnProfileReceived));
	}

	// Token: 0x060044BF RID: 17599 RVA: 0x0017248C File Offset: 0x0017068C
	public void SetModProfile(Mod mod)
	{
		if (mod.Id != ModId.Null)
		{
			this.pendingModId = 0L;
			this.currentMapMod = mod;
			this.hasModProfile = true;
			this.currentMapMod.OnModUpdated += this.OnModUpdated;
			this.isFavorite = ModIOManager.IsModFavorited(mod.Id);
			this.favoriteToggleButton.SetButtonActive(this.isFavorite);
			PlayerCountHelper.GetPlayerCount(this.currentMapMod, delegate(string count)
			{
				this.playerCountText.text = count;
			}, null);
			this.UpdateMapDetails(true);
		}
	}

	// Token: 0x060044C0 RID: 17600 RVA: 0x0017251C File Offset: 0x0017071C
	public override void PressButton(CustomMapKeyboardBinding buttonPressed)
	{
		if (Time.time < this.showTime + this.activationTime)
		{
			return;
		}
		GTDev.Log<string>("[CustomMapsDetailsScreen::PressButton] Is Driver: " + CustomMapsTerminal.IsDriver.ToString() + ", Button Pressed: " + buttonPressed.ToString(), null);
		if (!base.isActiveAndEnabled || !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.goback)
		{
			if (CustomMapManager.IsLoading())
			{
				return;
			}
			if (CustomMapManager.IsUnloading())
			{
				return;
			}
			if (this.mapLoadError)
			{
				this.mapLoadError = false;
				this.loadingMapMessageText.fontSize = 40f;
				CustomMapManager.ClearRoomMap();
				this.ResetToDefaultView();
				return;
			}
			if (CustomMapLoader.IsMapLoaded() || CustomMapManager.GetRoomMapId() != ModId.Null)
			{
				string text;
				if (!this.CanChangeMapState(false, out text))
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.errorText.text = text;
					this.errorText.gameObject.SetActive(true);
					return;
				}
				this.UnloadMap();
				return;
			}
			else
			{
				if (ModInstallationManagement.CurrentOperationOnMod != null && ModInstallationManagement.CurrentOperationOnMod.Id == this.GetModId())
				{
					GTDev.Log<string>("[CustomMapsDetailsScreen::PressButton] Attempted to go back while this mod is " + ModInstallationManagement.CurrentOperationOnMod.File.State.ToString() + ", ignoring...", null);
					return;
				}
				CustomMapsTerminal.ReturnFromDetailsScreen();
				this.hasModProfile = false;
				this.currentMapMod.OnModUpdated -= this.OnModUpdated;
				this.currentMapMod = null;
				return;
			}
		}
		else
		{
			if (!this.hasModProfile || this.mapLoadError)
			{
				bool flag = this.mapLoadError;
				return;
			}
			if (buttonPressed == CustomMapKeyboardBinding.option3)
			{
				this.RefreshCurrentMapMod();
				return;
			}
			if (buttonPressed == CustomMapKeyboardBinding.map)
			{
				if (this.currentMapMod == null || CustomMapLoader.IsMapLoaded() || CustomMapManager.IsLoading() || CustomMapManager.IsUnloading())
				{
					return;
				}
				this.errorText.gameObject.SetActive(false);
				this.errorText.text = "";
				this.loadingMapLabelText.gameObject.SetActive(false);
				this.loadingMapMessageText.gameObject.SetActive(false);
				this.modDescriptionText.gameObject.SetActive(true);
				ModIOManager.RefreshUserProfile(delegate(bool result)
				{
					if (this.currentMapMod.IsSubscribed)
					{
						ModIOManager.UnsubscribeFromMod(this.GetModId(), delegate(Error error)
						{
							if (!error)
							{
								this.UpdateMapDetails(false);
							}
						});
						return;
					}
					ModIOManager.SubscribeToMod(this.GetModId(), delegate(Error error)
					{
						if (!error)
						{
							this.UpdateMapDetails(false);
						}
					});
				}, false);
			}
			if (buttonPressed == CustomMapKeyboardBinding.enter && !CustomMapManager.IsLoading() && !CustomMapManager.IsUnloading() && !CustomMapLoader.IsMapLoaded() && this.currentMapMod != null && !this.IsCurrentModHidden())
			{
				if (this.currentMapMod.File.State == ModFileState.Installed)
				{
					string text2;
					if (!this.CanChangeMapState(true, out text2))
					{
						this.modDescriptionText.gameObject.SetActive(false);
						this.errorText.text = text2;
						this.errorText.gameObject.SetActive(true);
					}
					else
					{
						this.LoadMap();
					}
				}
				else
				{
					ModFileState state = this.currentMapMod.File.State;
					if (state == ModFileState.Queued || state == ModFileState.None)
					{
						ModIOManager.DownloadMod(this.GetModId(), delegate(bool modDownloadStarted)
						{
							if (modDownloadStarted)
							{
								this.UpdateStatus(false);
							}
						});
					}
					else
					{
						Debug.Log(string.Format("[CustomMapsDetailsScreen::PressButton] mod has status: {0}, ", this.currentMapMod.File.State) + "cannot start download or attempt to load map...");
					}
				}
			}
			if (buttonPressed == CustomMapKeyboardBinding.fav && this.currentMapMod != null)
			{
				if (this.isFavorite)
				{
					ModIOManager.RemoveFavorite(this.currentMapMod.Id);
					this.isFavorite = ModIOManager.IsModFavorited(this.currentMapMod.Id);
					this.favoriteToggleButton.SetButtonActive(this.isFavorite);
					if (this.IsCurrentModHidden())
					{
						this.favoriteToggleButton.gameObject.SetActive(false);
					}
				}
				else if (!this.IsCurrentModHidden())
				{
					ModIOManager.AddFavorite(this.currentMapMod.Id, delegate(Error error)
					{
						this.isFavorite = ModIOManager.IsModFavorited(this.currentMapMod.Id);
						this.favoriteToggleButton.SetButtonActive(this.isFavorite);
					});
				}
			}
			if (buttonPressed == CustomMapKeyboardBinding.delete)
			{
				if (CustomMapManager.IsLoading() || CustomMapManager.IsUnloading() || CustomMapLoader.IsMapLoaded())
				{
					return;
				}
				Mod currentMapMod = this.currentMapMod;
				bool flag2;
				if (currentMapMod != null)
				{
					Modfile file = currentMapMod.File;
					if (file != null)
					{
						ModFileState state = file.State;
						if (state == ModFileState.Queued || state == ModFileState.Installed)
						{
							flag2 = true;
							goto IL_3EC;
						}
					}
				}
				flag2 = false;
				IL_3EC:
				if (flag2)
				{
					this.currentMapMod.UninstallOtherUserMod(true);
					this.UpdateStatus(false);
				}
			}
			if (buttonPressed == CustomMapKeyboardBinding.rateUp)
			{
				this.currentMapMod.RateMod((this.currentMapMod.CurrentUserRating == ModRating.Positive) ? ModRating.None : ModRating.Positive);
			}
			if (buttonPressed == CustomMapKeyboardBinding.rateDown)
			{
				this.currentMapMod.RateMod((this.currentMapMod.CurrentUserRating == ModRating.Negative) ? ModRating.None : ModRating.Negative);
			}
			return;
		}
	}

	// Token: 0x060044C1 RID: 17601 RVA: 0x00172974 File Offset: 0x00170B74
	private void RefreshCurrentMapMod()
	{
		if (CustomMapLoader.IsMapLoaded() || CustomMapManager.IsLoading() || CustomMapManager.IsUnloading())
		{
			return;
		}
		if (this.hasModProfile)
		{
			long id = this.GetModId()._id;
			this.hasModProfile = false;
			this.currentMapMod.OnModUpdated -= this.OnModUpdated;
			this.currentMapMod = null;
			this.ResetToDefaultView();
			this.RetrieveModFromModIO(id, true, null);
		}
	}

	// Token: 0x060044C2 RID: 17602 RVA: 0x001729E0 File Offset: 0x00170BE0
	private void OnProfileReceived(Error error, Mod mod)
	{
		if (error)
		{
			this.modDescriptionText.gameObject.SetActive(false);
			this.errorText.text = string.Format("FAILED TO RETRIEVE MOD DETAILS FOR MOD: {0}", this.GetModId());
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.SetModProfile(mod);
	}

	// Token: 0x060044C3 RID: 17603 RVA: 0x00172A40 File Offset: 0x00170C40
	private void ResetToDefaultView()
	{
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.loadingMapMessageText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		this.modNameText.gameObject.SetActive(false);
		this.modCreatorLabelText.gameObject.SetActive(false);
		this.modCreatorText.gameObject.SetActive(false);
		this.modDescriptionText.gameObject.SetActive(false);
		this.modStatusText.gameObject.SetActive(false);
		this.modSubscriptionStatusText.gameObject.SetActive(false);
		this.mapScreenshotImage.gameObject.SetActive(false);
		this.hiddenRoomMapText.gameObject.SetActive(false);
		this.outdatedText.gameObject.SetActive(false);
		this.unloadPromptText.gameObject.SetActive(false);
		this.playerCountText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(true);
		if (CustomMapLoader.IsMapLoaded() || CustomMapManager.IsLoading() || CustomMapManager.IsUnloading())
		{
			ModId modId = new ModId(CustomMapLoader.IsMapLoaded() ? CustomMapLoader.LoadedMapModId : (CustomMapManager.IsLoading() ? CustomMapManager.LoadingMapId : CustomMapManager.UnloadingMapId));
			if (this.hasModProfile && this.GetModId() == modId)
			{
				this.UpdateMapDetails(true);
				return;
			}
			this.RetrieveModFromModIO(modId, false, delegate(Error error, Mod mod)
			{
				this.OnProfileReceived(error, mod);
			});
			return;
		}
		else
		{
			if (CustomMapManager.GetRoomMapId() != ModId.Null)
			{
				this.OnRoomMapChanged(CustomMapManager.GetRoomMapId());
				return;
			}
			if (this.hasModProfile)
			{
				this.UpdateMapDetails(true);
			}
			return;
		}
	}

	// Token: 0x060044C4 RID: 17604 RVA: 0x00172C04 File Offset: 0x00170E04
	private void UpdateMapDetails(bool refreshScreenState = true)
	{
		if (!this.hasModProfile)
		{
			return;
		}
		if (this.IsCurrentModHidden())
		{
			this.modNameText.text = this.hiddenMapTitle;
			this.modDescriptionText.text = this.hiddenMapDesc;
			this.modCreatorLabelText.gameObject.SetActive(false);
			this.modCreatorText.text = "";
			this.mapScreenshotImage.sprite = this.hiddenMapLogo;
			this.mapScreenshotImage.gameObject.SetActive(true);
		}
		else
		{
			this.modNameText.text = this.currentMapMod.Name;
			this.modDescriptionText.text = this.currentMapMod.Description;
			this.modCreatorLabelText.gameObject.SetActive(true);
			this.modCreatorText.text = this.currentMapMod.Creator.Username;
			ModIOManager.GetModLogo(this.currentMapMod, new Action<Error, Texture2D>(this.OnGetModLogo));
		}
		this.UpdateStatus(false);
		if (refreshScreenState)
		{
			this.loadingText.gameObject.SetActive(false);
			this.loadingMapLabelText.gameObject.SetActive(false);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.hiddenRoomMapText.gameObject.SetActive(false);
			this.mapReadyText.gameObject.SetActive(false);
			this.unloadPromptText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(false);
			this.modNameText.gameObject.SetActive(true);
			this.modDescriptionText.gameObject.SetActive(true);
			if (!this.IsCurrentModHidden())
			{
				this.modCreatorLabelText.gameObject.SetActive(true);
				this.modCreatorText.gameObject.SetActive(true);
			}
			if (CustomMapLoader.IsMapLoaded())
			{
				ModId modId = new ModId(CustomMapLoader.LoadedMapModId);
				if (this.GetModId() == modId)
				{
					this.OnMapLoadComplete_UIUpdate();
					return;
				}
				this.RetrieveModFromModIO(modId, false, delegate(Error error, Mod mod)
				{
					this.OnProfileReceived(error, mod);
				});
				return;
			}
			else
			{
				if (CustomMapManager.IsLoading() && !this.mapLoadError)
				{
					this.modDescriptionText.gameObject.SetActive(false);
					if (!CustomMapManager.IsUnloading())
					{
						this.loadingMapLabelText.text = this.mapLoadingString + " 0%";
					}
					else
					{
						this.loadingMapLabelText.text = this.mapUnloadingString;
					}
					this.loadingMapLabelText.gameObject.SetActive(true);
					return;
				}
				if (CustomMapManager.IsUnloading())
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadingMapLabelText.text = this.mapUnloadingString;
					this.loadingMapLabelText.gameObject.SetActive(true);
					return;
				}
				if (CustomMapManager.GetRoomMapId() != ModId.Null)
				{
					this.ShowLoadRoomMapPrompt();
					return;
				}
				if (this.mapLoadError)
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadingMapLabelText.gameObject.SetActive(true);
					this.loadingMapMessageText.gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x060044C5 RID: 17605 RVA: 0x00172F08 File Offset: 0x00171108
	private void OnGetModLogo(Error error, Texture2D modLogo)
	{
		if (error)
		{
			Debug.LogError(string.Format("[CustomMapsDetailsScreen::OnGetModLogo] Failed to retrieve logo for Mod {0}", this.GetModId()));
			return;
		}
		this.mapScreenshotImage.sprite = Sprite.Create(modLogo, new Rect(0f, 0f, 320f, 180f), new Vector2(0.5f, 0.5f));
		this.mapScreenshotImage.gameObject.SetActive(true);
	}

	// Token: 0x060044C6 RID: 17606 RVA: 0x00172F84 File Offset: 0x00171184
	private Task UpdateStatus(bool errorEncountered = false)
	{
		CustomMapsDetailsScreen.<UpdateStatus>d__73 <UpdateStatus>d__;
		<UpdateStatus>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<UpdateStatus>d__.<>4__this = this;
		<UpdateStatus>d__.errorEncountered = errorEncountered;
		<UpdateStatus>d__.<>1__state = -1;
		<UpdateStatus>d__.<>t__builder.Start<CustomMapsDetailsScreen.<UpdateStatus>d__73>(ref <UpdateStatus>d__);
		return <UpdateStatus>d__.<>t__builder.Task;
	}

	// Token: 0x060044C7 RID: 17607 RVA: 0x00172FD0 File Offset: 0x001711D0
	private bool CanChangeMapState(bool load, out string disallowedReason)
	{
		disallowedReason = "";
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			if (!CustomMapManager.AreAllPlayersInVirtualStump())
			{
				disallowedReason = "ALL PLAYERS IN THE ROOM MUST BE INSIDE THE VIRTUAL STUMP BEFORE " + (load ? "" : "UN") + "LOADING A MAP.";
				return false;
			}
			return true;
		}
		else
		{
			if (!CustomMapManager.IsLocalPlayerInVirtualStump())
			{
				disallowedReason = "YOU MUST BE INSIDE THE VIRTUAL STUMP TO " + (load ? "" : "UN") + "LOAD A MAP.";
				return false;
			}
			return true;
		}
	}

	// Token: 0x060044C8 RID: 17608 RVA: 0x00173054 File Offset: 0x00171254
	private void LoadMap()
	{
		this.modDescriptionText.gameObject.SetActive(false);
		this.modStatusText.gameObject.SetActive(false);
		this.modSubscriptionStatusText.gameObject.SetActive(false);
		this.outdatedText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(true);
		if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
		{
			NetworkSystem.Instance.ReturnToSinglePlayer();
		}
		this.deleteButton.gameObject.SetActive(false);
		this.subscriptionToggleButton.gameObject.SetActive(false);
		this.networkObject.LoadMapSynced(this.GetModId());
	}

	// Token: 0x060044C9 RID: 17609 RVA: 0x00173111 File Offset: 0x00171311
	private void UnloadMap()
	{
		this.networkObject.UnloadMapSynced();
	}

	// Token: 0x060044CA RID: 17610 RVA: 0x0017311E File Offset: 0x0017131E
	public void OnMapLoadComplete(bool success)
	{
		if (success)
		{
			this.OnMapLoadComplete_UIUpdate();
		}
	}

	// Token: 0x060044CB RID: 17611 RVA: 0x0017312C File Offset: 0x0017132C
	private void OnMapLoadComplete_UIUpdate()
	{
		this.modDescriptionText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.loadingMapMessageText.gameObject.SetActive(false);
		this.hiddenRoomMapText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(true);
		this.unloadPromptText.gameObject.SetActive(true);
	}

	// Token: 0x060044CC RID: 17612 RVA: 0x001731B0 File Offset: 0x001713B0
	private void OnMapUnloaded()
	{
		this.mapLoadError = false;
		this.loadingMapMessageText.fontSize = 40f;
		this.UpdateMapDetails(true);
	}

	// Token: 0x060044CD RID: 17613 RVA: 0x001731D0 File Offset: 0x001713D0
	private void OnRoomMapChanged(ModId roomMapID)
	{
		if (roomMapID == ModId.Null)
		{
			this.UpdateMapDetails(true);
			return;
		}
		if (this.GetModId() != roomMapID)
		{
			this.RetrieveModFromModIO(roomMapID, false, new Action<Error, Mod>(this.OnRoomMapRetrieved));
			return;
		}
		this.ShowLoadRoomMapPrompt();
	}

	// Token: 0x060044CE RID: 17614 RVA: 0x00173220 File Offset: 0x00171420
	private void OnRoomMapRetrieved(Error error, Mod mod)
	{
		this.OnProfileReceived(error, mod);
		if (!error)
		{
			this.ShowLoadRoomMapPrompt();
		}
	}

	// Token: 0x060044CF RID: 17615 RVA: 0x00173238 File Offset: 0x00171438
	private void ShowLoadRoomMapPrompt()
	{
		if (CustomMapManager.IsUnloading() || CustomMapManager.IsLoading() || CustomMapLoader.IsMapLoaded(this.GetModId()))
		{
			return;
		}
		this.modDescriptionText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(false);
		this.unloadPromptText.gameObject.SetActive(false);
		this.hiddenRoomMapText.gameObject.SetActive(false);
		if (this.IsCurrentModHidden())
		{
			this.hiddenRoomMapText.gameObject.SetActive(true);
		}
	}

	// Token: 0x060044D0 RID: 17616 RVA: 0x001732E0 File Offset: 0x001714E0
	public void OnMapLoadProgress(MapLoadStatus loadStatus, int progress, string message)
	{
		if (loadStatus != MapLoadStatus.None)
		{
			this.mapLoadError = false;
			this.loadingMapMessageText.fontSize = 40f;
			this.hiddenRoomMapText.gameObject.SetActive(false);
			this.modDescriptionText.gameObject.SetActive(false);
		}
		switch (loadStatus)
		{
		case MapLoadStatus.Downloading:
			this.loadingMapLabelText.text = this.mapAutoDownloadingString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadingMapMessageText.text = "";
			return;
		case MapLoadStatus.Loading:
			this.loadingMapLabelText.text = this.mapLoadingString + " " + progress.ToString() + "%";
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.text = message;
			this.loadingMapMessageText.gameObject.SetActive(true);
			return;
		case MapLoadStatus.Unloading:
			this.mapReadyText.gameObject.SetActive(false);
			this.unloadPromptText.gameObject.SetActive(false);
			this.loadingMapLabelText.text = this.mapUnloadingString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadingMapMessageText.text = "";
			return;
		case MapLoadStatus.Error:
			this.mapLoadError = true;
			this.loadingMapLabelText.text = this.mapLoadingErrorString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			if (CustomMapsTerminal.IsDriver)
			{
				this.loadingMapMessageText.text = message + "\n" + this.mapLoadingErrorDriverString;
			}
			else
			{
				this.loadingMapMessageText.text = message + "\n" + this.mapLoadingErrorNonDriverString;
			}
			if (this.loadingMapMessageText.text.Length > 150)
			{
				this.loadingMapMessageText.fontSize = 30f;
			}
			else
			{
				this.loadingMapMessageText.fontSize = 40f;
			}
			this.loadingMapMessageText.gameObject.SetActive(true);
			return;
		default:
			return;
		}
	}

	// Token: 0x060044D1 RID: 17617 RVA: 0x001734F6 File Offset: 0x001716F6
	public ModId GetModId()
	{
		Mod currentMapMod = this.currentMapMod;
		if (currentMapMod == null)
		{
			return ModId.Null;
		}
		return currentMapMod.Id;
	}

	// Token: 0x060044D2 RID: 17618 RVA: 0x0017350D File Offset: 0x0017170D
	public bool IsCurrentModHidden()
	{
		return this.hasModProfile && (this.currentMapMod.Creator == null || (!ModIOManager.IsLoggedIn() && this.currentMapMod.IsHidden()));
	}

	// Token: 0x040056E0 RID: 22240
	[SerializeField]
	private SpriteRenderer mapScreenshotImage;

	// Token: 0x040056E1 RID: 22241
	[SerializeField]
	private Sprite hiddenMapLogo;

	// Token: 0x040056E2 RID: 22242
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x040056E3 RID: 22243
	[SerializeField]
	private TMP_Text modNameText;

	// Token: 0x040056E4 RID: 22244
	[SerializeField]
	private TMP_Text modCreatorLabelText;

	// Token: 0x040056E5 RID: 22245
	[SerializeField]
	private TMP_Text modCreatorText;

	// Token: 0x040056E6 RID: 22246
	[SerializeField]
	private TMP_Text modDescriptionText;

	// Token: 0x040056E7 RID: 22247
	[SerializeField]
	private TMP_Text modStatusText;

	// Token: 0x040056E8 RID: 22248
	[SerializeField]
	private TMP_Text modStatusLabelText;

	// Token: 0x040056E9 RID: 22249
	[SerializeField]
	private TMP_Text modSubscriptionStatusText;

	// Token: 0x040056EA RID: 22250
	[SerializeField]
	private TMP_Text loadingMapLabelText;

	// Token: 0x040056EB RID: 22251
	[SerializeField]
	private TMP_Text loadingMapMessageText;

	// Token: 0x040056EC RID: 22252
	[SerializeField]
	private TMP_Text hiddenRoomMapText;

	// Token: 0x040056ED RID: 22253
	[SerializeField]
	private TMP_Text mapReadyText;

	// Token: 0x040056EE RID: 22254
	[SerializeField]
	private TMP_Text unloadPromptText;

	// Token: 0x040056EF RID: 22255
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x040056F0 RID: 22256
	[SerializeField]
	private TMP_Text outdatedText;

	// Token: 0x040056F1 RID: 22257
	[SerializeField]
	private TMP_Text playerCountText;

	// Token: 0x040056F2 RID: 22258
	[SerializeField]
	private CustomMapsScreenButton subscriptionToggleButton;

	// Token: 0x040056F3 RID: 22259
	[SerializeField]
	private CustomMapsScreenButton favoriteToggleButton;

	// Token: 0x040056F4 RID: 22260
	[SerializeField]
	private CustomMapsScreenButton rateUpButton;

	// Token: 0x040056F5 RID: 22261
	[SerializeField]
	private CustomMapsScreenButton rateDownButton;

	// Token: 0x040056F6 RID: 22262
	[SerializeField]
	private CustomMapsScreenButton loadButton;

	// Token: 0x040056F7 RID: 22263
	[SerializeField]
	private CustomMapsScreenButton deleteButton;

	// Token: 0x040056F8 RID: 22264
	[SerializeField]
	private string modAvailableString = "AVAILABLE";

	// Token: 0x040056F9 RID: 22265
	[SerializeField]
	private string mapAutoDownloadingString = "DOWNLOADING...";

	// Token: 0x040056FA RID: 22266
	[SerializeField]
	private string mapDownloadQueuedString = "DOWNLOAD QUEUED";

	// Token: 0x040056FB RID: 22267
	[SerializeField]
	private string mapLoadingString = "LOADING:";

	// Token: 0x040056FC RID: 22268
	[SerializeField]
	private string mapUnloadingString = "UNLOADING...";

	// Token: 0x040056FD RID: 22269
	[SerializeField]
	private string mapLoadingErrorString = "ERROR:";

	// Token: 0x040056FE RID: 22270
	[SerializeField]
	private string mapLoadingErrorDriverString = "PRESS THE 'BACK' BUTTON TO TRY AGAIN";

	// Token: 0x040056FF RID: 22271
	[SerializeField]
	private string mapLoadingErrorNonDriverString = "LEAVE AND REJOIN THE VIRTUAL STUMP TO TRY AGAIN";

	// Token: 0x04005700 RID: 22272
	[SerializeField]
	private string mapLoadingErrorInvalidModFile = "INSTALL FAILED DUE TO INVALID MAP FILE";

	// Token: 0x04005701 RID: 22273
	[SerializeField]
	private VirtualStumpSerializer networkObject;

	// Token: 0x04005702 RID: 22274
	public static Dictionary<ModFileState, string> modStatusStrings = new Dictionary<ModFileState, string>
	{
		{
			ModFileState.Installed,
			"READY"
		},
		{
			ModFileState.Queued,
			"QUEUED"
		},
		{
			ModFileState.Downloading,
			"DOWNLOADING"
		},
		{
			ModFileState.Installing,
			"INSTALLING"
		},
		{
			ModFileState.Uninstalling,
			"UNINSTALLING"
		},
		{
			ModFileState.Updating,
			"UPDATING"
		},
		{
			ModFileState.FileOperationFailed,
			"ERROR"
		},
		{
			ModFileState.None,
			"AVAILABLE"
		}
	};

	// Token: 0x04005703 RID: 22275
	[SerializeField]
	private string mapNotDownloadedString = "NOT DOWNLOADED";

	// Token: 0x04005704 RID: 22276
	[SerializeField]
	private string mapNeedsUpdateString = "NEEDS UPDATE";

	// Token: 0x04005705 RID: 22277
	[SerializeField]
	private string subscribeString = "SUBSCRIBE";

	// Token: 0x04005706 RID: 22278
	[SerializeField]
	private string unsubscribeString = "UNSUBSCRIBE";

	// Token: 0x04005707 RID: 22279
	[SerializeField]
	private string subscribedStatusString = "SUBSCRIBED";

	// Token: 0x04005708 RID: 22280
	[SerializeField]
	private string unsubscribedStatusString = "NOT SUBSCRIBED";

	// Token: 0x04005709 RID: 22281
	[SerializeField]
	private string loadMapString = "LOAD";

	// Token: 0x0400570A RID: 22282
	[SerializeField]
	private string downloadMapString = "DOWNLOAD";

	// Token: 0x0400570B RID: 22283
	[SerializeField]
	private string updateMapString = "UPDATE";

	// Token: 0x0400570C RID: 22284
	[SerializeField]
	private string hiddenMapTitle = "HIDDEN MAP";

	// Token: 0x0400570D RID: 22285
	[SerializeField]
	private string hiddenMapDesc = "YOU DON'T CURRENTLY HAVE ACCESS TO THIS HIDDEN MAP.\nCHECK THAT YOU'RE LOGGED IN TO THE CORRECT MOD.IO ACCOUNT.";

	// Token: 0x0400570E RID: 22286
	private const float LOGO_WIDTH = 320f;

	// Token: 0x0400570F RID: 22287
	private const float LOGO_HEIGHT = 180f;

	// Token: 0x04005710 RID: 22288
	public long pendingModId;

	// Token: 0x04005712 RID: 22290
	private bool hasModProfile;

	// Token: 0x04005713 RID: 22291
	private bool mapLoadError;

	// Token: 0x04005714 RID: 22292
	private bool isFavorite;
}
