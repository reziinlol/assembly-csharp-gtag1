using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using Modio;
using Modio.API;
using Modio.Authentication;
using Modio.Customizations;
using Modio.Errors;
using Modio.FileIO;
using Modio.Mods;
using Modio.Users;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

// Token: 0x02000A5C RID: 2652
public class ModIOManager : MonoBehaviour, ISteamCredentialProvider, IOculusCredentialProvider
{
	// Token: 0x060043FD RID: 17405 RVA: 0x0016C0C4 File Offset: 0x0016A2C4
	private void Awake()
	{
		if (ModIOManager.instance == null)
		{
			ModIOManager.instance = this;
			ModIOManager.hasInstance = true;
			UGCPermissionManager.SubscribeToUGCEnabled(new Action(ModIOManager.OnUGCEnabled));
			UGCPermissionManager.SubscribeToUGCDisabled(new Action(ModIOManager.OnUGCDisabled));
			ModioServices.Bind<IModioAuthService>().FromInstance(ModIOManager.accountLinkingAuthService, (ModioServicePriority)41, null);
			ModioServices.Bind<IModioAuthService>().FromInstance(ModIOManager.steamAuthService, ModioServicePriority.DeveloperOverride, null);
			long gameId = ModioServices.Resolve<ModioSettings>().GameId;
			ModIOManager.ModIODirectory = Path.Combine(ModioServices.Resolve<IModioRootPathProvider>().Path, "mod.io", gameId.ToString()) + Path.DirectorySeparatorChar.ToString();
			return;
		}
		if (ModIOManager.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060043FE RID: 17406 RVA: 0x0016C18E File Offset: 0x0016A38E
	private void Start()
	{
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
	}

	// Token: 0x060043FF RID: 17407 RVA: 0x0016C1B4 File Offset: 0x0016A3B4
	private void OnDestroy()
	{
		if (ModIOManager.instance == this)
		{
			ModIOManager.instance = null;
			ModIOManager.hasInstance = false;
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(ModIOManager.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(ModIOManager.OnUGCDisabled));
		}
		NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
	}

	// Token: 0x06004400 RID: 17408 RVA: 0x0016C221 File Offset: 0x0016A421
	private void Update()
	{
		bool flag = ModIOManager.hasInstance;
	}

	// Token: 0x06004401 RID: 17409 RVA: 0x000028C5 File Offset: 0x00000AC5
	private static void OnUGCEnabled()
	{
	}

	// Token: 0x06004402 RID: 17410 RVA: 0x000028C5 File Offset: 0x00000AC5
	private static void OnUGCDisabled()
	{
	}

	// Token: 0x06004403 RID: 17411 RVA: 0x0016C229 File Offset: 0x0016A429
	public static bool IsInitialized()
	{
		return ModIOManager.initialized;
	}

	// Token: 0x06004404 RID: 17412 RVA: 0x0016C230 File Offset: 0x0016A430
	public static Task<Error> Initialize()
	{
		ModIOManager.<Initialize>d__47 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<ModIOManager.<Initialize>d__47>(ref <Initialize>d__);
		return <Initialize>d__.<>t__builder.Task;
	}

	// Token: 0x06004405 RID: 17413 RVA: 0x0016C26C File Offset: 0x0016A46C
	private static Task<Error> InitInternal()
	{
		ModIOManager.<InitInternal>d__48 <InitInternal>d__;
		<InitInternal>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<InitInternal>d__.<>1__state = -1;
		<InitInternal>d__.<>t__builder.Start<ModIOManager.<InitInternal>d__48>(ref <InitInternal>d__);
		return <InitInternal>d__.<>t__builder.Task;
	}

	// Token: 0x06004406 RID: 17414 RVA: 0x0016C2A8 File Offset: 0x0016A4A8
	private Task<ValueTuple<Error, bool, bool>> HasAcceptedLatestTerms()
	{
		ModIOManager.<HasAcceptedLatestTerms>d__49 <HasAcceptedLatestTerms>d__;
		<HasAcceptedLatestTerms>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, bool, bool>>.Create();
		<HasAcceptedLatestTerms>d__.<>1__state = -1;
		<HasAcceptedLatestTerms>d__.<>t__builder.Start<ModIOManager.<HasAcceptedLatestTerms>d__49>(ref <HasAcceptedLatestTerms>d__);
		return <HasAcceptedLatestTerms>d__.<>t__builder.Task;
	}

	// Token: 0x06004407 RID: 17415 RVA: 0x0016C2E4 File Offset: 0x0016A4E4
	private Task<Error> ShowModIOTermsOfUse()
	{
		ModIOManager.<ShowModIOTermsOfUse>d__50 <ShowModIOTermsOfUse>d__;
		<ShowModIOTermsOfUse>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<ShowModIOTermsOfUse>d__.<>4__this = this;
		<ShowModIOTermsOfUse>d__.<>1__state = -1;
		<ShowModIOTermsOfUse>d__.<>t__builder.Start<ModIOManager.<ShowModIOTermsOfUse>d__50>(ref <ShowModIOTermsOfUse>d__);
		return <ShowModIOTermsOfUse>d__.<>t__builder.Task;
	}

	// Token: 0x06004408 RID: 17416 RVA: 0x0016C328 File Offset: 0x0016A528
	private void OnModIOTermsOfUseAcknowledged(bool accepted)
	{
		if (accepted)
		{
			CustomMapManager.RequestEnableTeleportHUD(true);
			Action<ModIORequestResultAnd<bool>> action = ModIOManager.modIOTermsAcknowledgedCallback;
			if (action != null)
			{
				action(ModIORequestResultAnd<bool>.CreateSuccessResult(true));
			}
		}
		else
		{
			Action<ModIORequestResultAnd<bool>> action2 = ModIOManager.modIOTermsAcknowledgedCallback;
			if (action2 != null)
			{
				action2(ModIORequestResultAnd<bool>.CreateFailureResult("MOD.IO TERMS OF USE HAVE NOT BEEN ACCEPTED. YOU MUST ACCEPT THE MOD.IO TERMS OF USE TO LOGIN WITH YOUR PLATFORM CREDENTIALS OR YOU CAN LOGIN WITH AN EXISTING MOD.IO ACCOUNT BY PRESSING THE 'LINK MOD.IO ACCOUNT' BUTTON AND FOLLOWING THE INSTRUCTIONS."));
			}
		}
		ModIOManager.modIOTermsAcknowledgedCallback = null;
	}

	// Token: 0x06004409 RID: 17417 RVA: 0x0016C376 File Offset: 0x0016A576
	private static void EnableModManagement()
	{
		if (!ModIOManager.modManagementEnabled)
		{
			ModInstallationManagement.ManagementEvents += ModIOManager.HandleModManagementEvent;
			ModInstallationManagement.Activate();
			ModIOManager.modManagementEnabled = true;
			ModioLog verbose = ModioLog.Verbose;
			if (verbose == null)
			{
				return;
			}
			verbose.Log("[ModIOManager::EnableModManagement] Mod Management enabled.");
		}
	}

	// Token: 0x0600440A RID: 17418 RVA: 0x0016C3AF File Offset: 0x0016A5AF
	private static void DisableModManagement()
	{
		if (ModIOManager.modManagementEnabled)
		{
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("[ModIOManager::EnableModManagement] Mod Management disabled!");
			}
			ModInstallationManagement.ManagementEvents -= ModIOManager.HandleModManagementEvent;
			ModInstallationManagement.Deactivate(false);
			ModIOManager.modManagementEnabled = false;
		}
	}

	// Token: 0x0600440B RID: 17419 RVA: 0x0016C3EC File Offset: 0x0016A5EC
	private static void HandleModManagementEvent(Mod mod, Modfile modfile, ModInstallationManagement.OperationType jobType, ModInstallationManagement.OperationPhase jobPhase)
	{
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::HandleModManagementEvent] Mod " + mod.Id.ToString() + " | FileState: " + string.Format("{0} | JobType: {1} | JobPhase: {2}", modfile.State.ToString(), jobType, jobPhase));
		}
		try
		{
			if ((jobType == ModInstallationManagement.OperationType.Install || jobType == ModInstallationManagement.OperationType.Download) && jobPhase == ModInstallationManagement.OperationPhase.Completed && modfile.State == ModFileState.Installed)
			{
				ModIOManager.outdatedModCMSVersions.Remove(mod.Id);
				ModIOManager.IsModOutdated(mod);
			}
			if (jobPhase == ModInstallationManagement.OperationPhase.Started && (jobType == ModInstallationManagement.OperationType.Download || jobType == ModInstallationManagement.OperationType.Update || jobType == ModInstallationManagement.OperationType.Uninstall))
			{
				ModIOManager.outdatedModCMSVersions.Remove(mod.Id);
			}
		}
		catch (Exception arg)
		{
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log(string.Format("[ModIOManager::HandleModManagementEvent] Exception: {0}", arg));
			}
		}
		UnityEvent<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase> onModManagementEvent = ModIOManager.OnModManagementEvent;
		if (onModManagementEvent == null)
		{
			return;
		}
		onModManagementEvent.Invoke(mod, modfile, jobType, jobPhase);
	}

	// Token: 0x0600440C RID: 17420 RVA: 0x0016C4E8 File Offset: 0x0016A6E8
	public static Task RefreshModCache()
	{
		ModIOManager.<RefreshModCache>d__55 <RefreshModCache>d__;
		<RefreshModCache>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RefreshModCache>d__.<>1__state = -1;
		<RefreshModCache>d__.<>t__builder.Start<ModIOManager.<RefreshModCache>d__55>(ref <RefreshModCache>d__);
		return <RefreshModCache>d__.<>t__builder.Task;
	}

	// Token: 0x0600440D RID: 17421 RVA: 0x0016C523 File Offset: 0x0016A723
	public static bool IsRefreshing()
	{
		return ModIOManager.refreshingModCache;
	}

	// Token: 0x0600440E RID: 17422 RVA: 0x0016C52C File Offset: 0x0016A72C
	public static Task<ValueTuple<bool, int>> IsModOutdated(ModId modId)
	{
		ModIOManager.<IsModOutdated>d__57 <IsModOutdated>d__;
		<IsModOutdated>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, int>>.Create();
		<IsModOutdated>d__.modId = modId;
		<IsModOutdated>d__.<>1__state = -1;
		<IsModOutdated>d__.<>t__builder.Start<ModIOManager.<IsModOutdated>d__57>(ref <IsModOutdated>d__);
		return <IsModOutdated>d__.<>t__builder.Task;
	}

	// Token: 0x0600440F RID: 17423 RVA: 0x0016C570 File Offset: 0x0016A770
	public static ValueTuple<bool, int> IsModOutdated(Mod mod)
	{
		int item;
		if (ModIOManager.outdatedModCMSVersions.TryGetValue(mod.Id, out item))
		{
			return new ValueTuple<bool, int>(true, item);
		}
		if (mod.File != null)
		{
			if (mod.File.State == ModFileState.Installed)
			{
				ValueTuple<bool, int> valueTuple = ModIOManager.IsInstalledModOutdated(mod);
				bool item2 = valueTuple.Item1;
				int item3 = valueTuple.Item2;
				return new ValueTuple<bool, int>(item2, item3);
			}
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log("[ModIOManager::IsModOutdated] Mod File for " + mod.Name + " is not installed. " + string.Format("State: {0}.", mod.File.State));
			}
		}
		else
		{
			ModioLog error2 = ModioLog.Error;
			if (error2 != null)
			{
				error2.Log("[ModIOManager::IsModOutdated] Mod File for " + mod.Name + " is null.");
			}
		}
		return new ValueTuple<bool, int>(false, -1);
	}

	// Token: 0x06004410 RID: 17424 RVA: 0x0016C638 File Offset: 0x0016A838
	public static void SaveFavoriteMods()
	{
		if (!ModIOManager.initialized || !ModIOManager.modManagementEnabled)
		{
			return;
		}
		try
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(ModIOManager.ModIODirectory);
			if (!directoryInfo.Exists)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log("[ModIOManager::SaveFavoriteMods] ModIO Directory for GorillaTag does not exist!");
				}
			}
			else
			{
				long[] array = new long[ModIOManager.favoriteMods.Count];
				int num = 0;
				foreach (KeyValuePair<ModId, Mod> keyValuePair in ModIOManager.favoriteMods)
				{
					array[num++] = keyValuePair.Key;
				}
				string contents = JsonConvert.SerializeObject(array);
				File.WriteAllText(Path.Join(directoryInfo.FullName, "favoriteMods.json"), contents);
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06004411 RID: 17425 RVA: 0x0016C720 File Offset: 0x0016A920
	[return: TupleElementNames(new string[]
	{
		"error",
		"favoriteMods"
	})]
	public static Task<ValueTuple<Error, List<Mod>>> GetFavoriteMods(bool forceRefresh = false)
	{
		ModIOManager.<GetFavoriteMods>d__60 <GetFavoriteMods>d__;
		<GetFavoriteMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, List<Mod>>>.Create();
		<GetFavoriteMods>d__.forceRefresh = forceRefresh;
		<GetFavoriteMods>d__.<>1__state = -1;
		<GetFavoriteMods>d__.<>t__builder.Start<ModIOManager.<GetFavoriteMods>d__60>(ref <GetFavoriteMods>d__);
		return <GetFavoriteMods>d__.<>t__builder.Task;
	}

	// Token: 0x06004412 RID: 17426 RVA: 0x0016C764 File Offset: 0x0016A964
	public static Task<Error> AddFavorite(ModId modId, Action<Error> callback = null)
	{
		ModIOManager.<AddFavorite>d__61 <AddFavorite>d__;
		<AddFavorite>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<AddFavorite>d__.modId = modId;
		<AddFavorite>d__.callback = callback;
		<AddFavorite>d__.<>1__state = -1;
		<AddFavorite>d__.<>t__builder.Start<ModIOManager.<AddFavorite>d__61>(ref <AddFavorite>d__);
		return <AddFavorite>d__.<>t__builder.Task;
	}

	// Token: 0x06004413 RID: 17427 RVA: 0x0016C7AF File Offset: 0x0016A9AF
	public static Error RemoveFavorite(ModId modId)
	{
		if (!ModIOManager.favoriteMods.ContainsKey(modId))
		{
			return new Error(ErrorCode.UNKNOWN, "MOD NOT FAVORITED");
		}
		ModIOManager.favoriteMods.Remove(modId);
		ModIOManager.SaveFavoriteMods();
		return Error.None;
	}

	// Token: 0x06004414 RID: 17428 RVA: 0x0016C7E5 File Offset: 0x0016A9E5
	public static bool IsModFavorited(ModId modId)
	{
		return ModIOManager.favoriteMods.ContainsKey(modId);
	}

	// Token: 0x06004415 RID: 17429 RVA: 0x0016C7F4 File Offset: 0x0016A9F4
	[return: TupleElementNames(new string[]
	{
		"error",
		"installedMods"
	})]
	public static Task<ValueTuple<Error, Mod[]>> GetInstalledMods(bool forceRefresh = false)
	{
		ModIOManager.<GetInstalledMods>d__64 <GetInstalledMods>d__;
		<GetInstalledMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Mod[]>>.Create();
		<GetInstalledMods>d__.forceRefresh = forceRefresh;
		<GetInstalledMods>d__.<>1__state = -1;
		<GetInstalledMods>d__.<>t__builder.Start<ModIOManager.<GetInstalledMods>d__64>(ref <GetInstalledMods>d__);
		return <GetInstalledMods>d__.<>t__builder.Task;
	}

	// Token: 0x06004416 RID: 17430 RVA: 0x0016C837 File Offset: 0x0016AA37
	public static bool ValidateInstalledMod(Mod mod)
	{
		return ModIOManager.initialized && ModInstallationManagement.ValidateInstalledMod(mod);
	}

	// Token: 0x06004417 RID: 17431 RVA: 0x0016C848 File Offset: 0x0016AA48
	private static ValueTuple<bool, int> IsInstalledModOutdated(Mod mod)
	{
		int item = -1;
		if (!ModIOManager.hasInstance)
		{
			return new ValueTuple<bool, int>(false, item);
		}
		if (mod.File == null || mod.File.State != ModFileState.Installed)
		{
			ModioLog message = ModioLog.Message;
			if (message != null)
			{
				message.Log("[ModIOManager::IsInstalledModOutdated] Mod " + mod.Id.ToString() + " is not currently installed.");
			}
			return new ValueTuple<bool, int>(false, item);
		}
		try
		{
			FileInfo[] files = new DirectoryInfo(mod.File.InstallLocation).GetFiles("package.json");
			if (files.Length == 0)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(string.Concat(new string[]
					{
						"[ModIOManager::IsInstalledModOutdated] Directory (",
						mod.File.InstallLocation,
						") for mod ",
						mod.Name,
						" does not contain a package.json file!"
					}));
				}
			}
			if (files.Length > 1)
			{
				ModioLog warning = ModioLog.Warning;
				if (warning != null)
				{
					warning.Log(string.Concat(new string[]
					{
						"[ModIOManager::IsInstalledModOutdated] Directory (",
						mod.File.InstallLocation,
						") for mod ",
						mod.Name,
						" contains more than one package.json file! Only the first one found will be used!"
					}));
				}
			}
			MapPackageInfo packageInfo = CustomMapLoader.GetPackageInfo(files[0].FullName);
			if (packageInfo.customMapSupportVersion != GT_CustomMapSupportRuntime.Constants.customMapSupportVersion)
			{
				ModIOManager.outdatedModCMSVersions.Add(mod.Id, packageInfo.customMapSupportVersion);
				return new ValueTuple<bool, int>(true, packageInfo.customMapSupportVersion);
			}
		}
		catch (Exception arg)
		{
			ModioLog error2 = ModioLog.Error;
			if (error2 != null)
			{
				error2.Log(string.Format("[ModIOManager::IsInstalledModOutdated] Exception while reading package.json: {0}", arg));
			}
			ModInstallationManagement.RefreshMod(mod);
			return new ValueTuple<bool, int>(false, item);
		}
		return new ValueTuple<bool, int>(false, item);
	}

	// Token: 0x06004418 RID: 17432 RVA: 0x0016CA00 File Offset: 0x0016AC00
	public static Task RefreshUserProfile(Action<bool> callback = null, bool force = false)
	{
		ModIOManager.<RefreshUserProfile>d__67 <RefreshUserProfile>d__;
		<RefreshUserProfile>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RefreshUserProfile>d__.callback = callback;
		<RefreshUserProfile>d__.force = force;
		<RefreshUserProfile>d__.<>1__state = -1;
		<RefreshUserProfile>d__.<>t__builder.Start<ModIOManager.<RefreshUserProfile>d__67>(ref <RefreshUserProfile>d__);
		return <RefreshUserProfile>d__.<>t__builder.Task;
	}

	// Token: 0x06004419 RID: 17433 RVA: 0x0016CA4C File Offset: 0x0016AC4C
	[return: TupleElementNames(new string[]
	{
		"error",
		"mods"
	})]
	public static Task<ValueTuple<Error, ICollection<Mod>>> GetMods(ICollection<long> modIds, bool forceRefresh = false, Action<Error, ICollection<Mod>> callback = null)
	{
		ModIOManager.<GetMods>d__68 <GetMods>d__;
		<GetMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ICollection<Mod>>>.Create();
		<GetMods>d__.modIds = modIds;
		<GetMods>d__.forceRefresh = forceRefresh;
		<GetMods>d__.callback = callback;
		<GetMods>d__.<>1__state = -1;
		<GetMods>d__.<>t__builder.Start<ModIOManager.<GetMods>d__68>(ref <GetMods>d__);
		return <GetMods>d__.<>t__builder.Task;
	}

	// Token: 0x0600441A RID: 17434 RVA: 0x0016CAA0 File Offset: 0x0016ACA0
	[return: TupleElementNames(new string[]
	{
		"error",
		"result"
	})]
	public static Task<ValueTuple<Error, Mod>> GetMod(ModId modId, bool forceUpdate = false, Action<Error, Mod> callback = null)
	{
		ModIOManager.<GetMod>d__69 <GetMod>d__;
		<GetMod>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Mod>>.Create();
		<GetMod>d__.modId = modId;
		<GetMod>d__.forceUpdate = forceUpdate;
		<GetMod>d__.callback = callback;
		<GetMod>d__.<>1__state = -1;
		<GetMod>d__.<>t__builder.Start<ModIOManager.<GetMod>d__69>(ref <GetMod>d__);
		return <GetMod>d__.<>t__builder.Task;
	}

	// Token: 0x0600441B RID: 17435 RVA: 0x0016CAF4 File Offset: 0x0016ACF4
	[return: TupleElementNames(new string[]
	{
		"error",
		"logo"
	})]
	public static Task<ValueTuple<Error, Texture2D>> GetModLogo(Mod mod, Action<Error, Texture2D> callback)
	{
		ModIOManager.<GetModLogo>d__70 <GetModLogo>d__;
		<GetModLogo>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Texture2D>>.Create();
		<GetModLogo>d__.mod = mod;
		<GetModLogo>d__.callback = callback;
		<GetModLogo>d__.<>1__state = -1;
		<GetModLogo>d__.<>t__builder.Start<ModIOManager.<GetModLogo>d__70>(ref <GetModLogo>d__);
		return <GetModLogo>d__.<>t__builder.Task;
	}

	// Token: 0x0600441C RID: 17436 RVA: 0x0016CB40 File Offset: 0x0016AD40
	[return: TupleElementNames(new string[]
	{
		"error",
		"modsPage"
	})]
	public static Task<ValueTuple<Error, ModioPage<Mod>>> GetMods(ModioAPI.Mods.GetModsFilter searchFilter)
	{
		ModIOManager.<GetMods>d__71 <GetMods>d__;
		<GetMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModioPage<Mod>>>.Create();
		<GetMods>d__.searchFilter = searchFilter;
		<GetMods>d__.<>1__state = -1;
		<GetMods>d__.<>t__builder.Start<ModIOManager.<GetMods>d__71>(ref <GetMods>d__);
		return <GetMods>d__.<>t__builder.Task;
	}

	// Token: 0x0600441D RID: 17437 RVA: 0x0016CB84 File Offset: 0x0016AD84
	private static void ModIOUserChanged(User currentUser)
	{
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::ModIOUserChanged] CurrentUser: " + ((currentUser == null) ? "NULL" : currentUser.Profile.Username));
		}
		UnityEvent<User> onModIOUserChanged = ModIOManager.OnModIOUserChanged;
		if (onModIOUserChanged == null)
		{
			return;
		}
		onModIOUserChanged.Invoke(currentUser);
	}

	// Token: 0x0600441E RID: 17438 RVA: 0x0016CBD0 File Offset: 0x0016ADD0
	private static void ModIOUserSyncComplete()
	{
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::ModIOUserSyncComplete] Refreshing mod cache...");
		}
		ModIOManager.RefreshModCache();
	}

	// Token: 0x0600441F RID: 17439 RVA: 0x0016CBED File Offset: 0x0016ADED
	public static bool IsLoggedIn()
	{
		return User.Current != null && User.Current.IsAuthenticated;
	}

	// Token: 0x06004420 RID: 17440 RVA: 0x0016CC02 File Offset: 0x0016AE02
	public static bool IsLoggingIn()
	{
		return ModIOManager.loggingIn;
	}

	// Token: 0x06004421 RID: 17441 RVA: 0x0016CC09 File Offset: 0x0016AE09
	public static bool IsLoggingOut()
	{
		return ModIOManager.loggingOut;
	}

	// Token: 0x06004422 RID: 17442 RVA: 0x0016CC10 File Offset: 0x0016AE10
	public static string GetCurrentUsername()
	{
		if (!ModIOManager.IsLoggedIn())
		{
			return "";
		}
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::GetCurrentUsername] Username: " + User.Current.Profile.Username);
		}
		return User.Current.Profile.Username;
	}

	// Token: 0x06004423 RID: 17443 RVA: 0x0016CC64 File Offset: 0x0016AE64
	public static string GetCurrentUserId()
	{
		if (!ModIOManager.IsLoggedIn())
		{
			return "";
		}
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log(string.Format("[ModIOManager::GetCurrentUserId] User ID: {0}", User.Current.Profile.UserId));
		}
		return User.Current.Profile.UserId.ToString();
	}

	// Token: 0x06004424 RID: 17444 RVA: 0x0016CCC3 File Offset: 0x0016AEC3
	public static string GetCurrentAuthToken()
	{
		if (!ModIOManager.IsLoggedIn())
		{
			return "";
		}
		return User.Current.Token;
	}

	// Token: 0x06004425 RID: 17445 RVA: 0x0016CCDC File Offset: 0x0016AEDC
	public static bool IsAuthenticated(bool sendEvents = false)
	{
		if (!ModIOManager.hasInstance)
		{
			return false;
		}
		bool isAuthenticated = User.Current.IsAuthenticated;
		if (isAuthenticated)
		{
			ModIOManager.loggingIn = false;
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("[ModIOManager::IsAuthenticated] User already authenticated...");
			}
			if (sendEvents)
			{
				UnityEvent onModIOLoggedIn = ModIOManager.OnModIOLoggedIn;
				if (onModIOLoggedIn != null)
				{
					onModIOLoggedIn.Invoke();
				}
			}
		}
		else
		{
			try
			{
				ModioLog verbose2 = ModioLog.Verbose;
				if (verbose2 != null)
				{
					verbose2.Log("[ModIOManager::IsAuthenticated] User not authenticated");
				}
				if (sendEvents)
				{
					UnityEvent onModIOLoggedOut = ModIOManager.OnModIOLoggedOut;
					if (onModIOLoggedOut != null)
					{
						onModIOLoggedOut.Invoke();
					}
				}
			}
			catch (Exception arg)
			{
				ModioLog verbose3 = ModioLog.Verbose;
				if (verbose3 != null)
				{
					verbose3.Log(string.Format("[ModIOManager::IsAuthenticated] error {0}", arg));
				}
			}
		}
		ModioLog verbose4 = ModioLog.Verbose;
		if (verbose4 != null)
		{
			verbose4.Log(string.Format("[ModIOManager::IsAuthenticated] returning {0}", isAuthenticated));
		}
		return isAuthenticated;
	}

	// Token: 0x06004426 RID: 17446 RVA: 0x0016CDAC File Offset: 0x0016AFAC
	public static void LogoutFromModIO()
	{
		if (!ModIOManager.hasInstance || ModIOManager.loggingIn || !ModIOManager.IsLoggedIn())
		{
			return;
		}
		ModIOManager.loggingOut = true;
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::LogoutFromModIO] Logging out of mod.io...");
		}
		ModIOManager.CancelExternalAuthentication();
		ModIOManager.loggingIn = false;
		User.DeleteUserData();
		ModioLog verbose2 = ModioLog.Verbose;
		if (verbose2 != null)
		{
			verbose2.Log("[ModIOManager::LogoutFromModIO] User data deleted...");
		}
		PlayerPrefs.SetInt("modIOLassSuccessfulAuthMethod", ModIOManager.ModIOAuthMethod.Invalid.GetIndex<ModIOManager.ModIOAuthMethod>());
		ModioLog verbose3 = ModioLog.Verbose;
		if (verbose3 != null)
		{
			verbose3.Log("[ModIOManager::LogoutFromModIO] User fully logged out.");
		}
		ModIOManager.loggingOut = false;
		UnityEvent onModIOLoggedOut = ModIOManager.OnModIOLoggedOut;
		if (onModIOLoggedOut != null)
		{
			onModIOLoggedOut.Invoke();
		}
		ModIOManager.RefreshModCache();
	}

	// Token: 0x06004427 RID: 17447 RVA: 0x0016CE50 File Offset: 0x0016B050
	public static void SetAccountLinkPrompter(IWssAuthPrompter prompter)
	{
		if (ModIOManager.accountLinkingAuthService != null)
		{
			ModIOManager.accountLinkingAuthService.SetPrompter(prompter);
		}
	}

	// Token: 0x06004428 RID: 17448 RVA: 0x0016CE64 File Offset: 0x0016B064
	public static Task<Error> RequestAccountLinkCode()
	{
		ModIOManager.<RequestAccountLinkCode>d__83 <RequestAccountLinkCode>d__;
		<RequestAccountLinkCode>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<RequestAccountLinkCode>d__.<>1__state = -1;
		<RequestAccountLinkCode>d__.<>t__builder.Start<ModIOManager.<RequestAccountLinkCode>d__83>(ref <RequestAccountLinkCode>d__);
		return <RequestAccountLinkCode>d__.<>t__builder.Task;
	}

	// Token: 0x06004429 RID: 17449 RVA: 0x0016CE9F File Offset: 0x0016B09F
	public static void CancelExternalAuthentication()
	{
		if (!ModIOManager.hasInstance)
		{
			return;
		}
		if (ModIOManager.accountLinkingAuthService != null && ModIOManager.accountLinkingAuthService.InProgress())
		{
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("[ModIOManager::CancelExternalAuthentication] Cancelling Mod.io Account Linking process...");
			}
			ModIOManager.accountLinkingAuthService.Cancel();
		}
	}

	// Token: 0x0600442A RID: 17450 RVA: 0x0016CEDC File Offset: 0x0016B0DC
	public static Task<Error> RequestPlatformLogin()
	{
		ModIOManager.<RequestPlatformLogin>d__85 <RequestPlatformLogin>d__;
		<RequestPlatformLogin>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<RequestPlatformLogin>d__.<>1__state = -1;
		<RequestPlatformLogin>d__.<>t__builder.Start<ModIOManager.<RequestPlatformLogin>d__85>(ref <RequestPlatformLogin>d__);
		return <RequestPlatformLogin>d__.<>t__builder.Task;
	}

	// Token: 0x0600442B RID: 17451 RVA: 0x0016CF18 File Offset: 0x0016B118
	private Task<Error> InitiatePlatformLogin()
	{
		ModIOManager.<InitiatePlatformLogin>d__86 <InitiatePlatformLogin>d__;
		<InitiatePlatformLogin>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<InitiatePlatformLogin>d__.<>4__this = this;
		<InitiatePlatformLogin>d__.<>1__state = -1;
		<InitiatePlatformLogin>d__.<>t__builder.Start<ModIOManager.<InitiatePlatformLogin>d__86>(ref <InitiatePlatformLogin>d__);
		return <InitiatePlatformLogin>d__.<>t__builder.Task;
	}

	// Token: 0x0600442C RID: 17452 RVA: 0x0016CF5C File Offset: 0x0016B15C
	private Task<Error> ContinuePlatformLogin()
	{
		ModIOManager.<ContinuePlatformLogin>d__87 <ContinuePlatformLogin>d__;
		<ContinuePlatformLogin>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<ContinuePlatformLogin>d__.<>4__this = this;
		<ContinuePlatformLogin>d__.<>1__state = -1;
		<ContinuePlatformLogin>d__.<>t__builder.Start<ModIOManager.<ContinuePlatformLogin>d__87>(ref <ContinuePlatformLogin>d__);
		return <ContinuePlatformLogin>d__.<>t__builder.Task;
	}

	// Token: 0x0600442D RID: 17453 RVA: 0x0016CFA0 File Offset: 0x0016B1A0
	public void RequestEncryptedAppTicket(Action<bool, string> callback)
	{
		if (this.requestEncryptedAppTicketCallback != null)
		{
			ModioLog warning = ModioLog.Warning;
			if (warning != null)
			{
				warning.Log("[ModIOManager::RequestEncryptedAppTicket] Callback already set, Encrypted App Ticket request already in progress!");
			}
			if (callback != null)
			{
				callback(false, "AN ENCRYPTED APP TICKET REQUEST IS ALREADY IN PROGRESS");
			}
			return;
		}
		this.requestEncryptedAppTicketCallback = callback;
		if (ModIOManager.requestEncryptedAppTicketResponse == null)
		{
			ModIOManager.requestEncryptedAppTicketResponse = CallResult<EncryptedAppTicketResponse_t>.Create(new CallResult<EncryptedAppTicketResponse_t>.APIDispatchDelegate(this.OnRequestEncryptedAppTicketFinished));
		}
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::RequestEncryptedAppTicket] Requesting Steam Encrypted App Ticket...");
		}
		SteamAPICall_t hAPICall = SteamUser.RequestEncryptedAppTicket(null, 0);
		ModIOManager.requestEncryptedAppTicketResponse.Set(hAPICall, null);
	}

	// Token: 0x0600442E RID: 17454 RVA: 0x0016D028 File Offset: 0x0016B228
	private void OnRequestEncryptedAppTicketFinished(EncryptedAppTicketResponse_t response, bool bIOFailure)
	{
		if (bIOFailure)
		{
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log("Failed to retrieve EncryptedAppTicket due to a Steam API IO failure...");
			}
			Action<bool, string> action = this.requestEncryptedAppTicketCallback;
			if (action != null)
			{
				action(false, "FAILED TO RETRIEVE 'EncryptedAppTicket' DUE TO A STEAM API IO FAILURE.");
			}
			this.requestEncryptedAppTicketCallback = null;
			return;
		}
		EResult eResult = response.m_eResult;
		if (eResult <= EResult.k_EResultNoConnection)
		{
			if (eResult != EResult.k_EResultOK)
			{
				if (eResult == EResult.k_EResultNoConnection)
				{
					ModioLog error2 = ModioLog.Error;
					if (error2 != null)
					{
						error2.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] Not connected to steam.");
					}
					Action<bool, string> action2 = this.requestEncryptedAppTicketCallback;
					if (action2 != null)
					{
						action2(false, "NOT CONNECTED TO STEAM.");
					}
					this.requestEncryptedAppTicketCallback = null;
					return;
				}
			}
			else
			{
				if (!SteamUser.GetEncryptedAppTicket(ModIOManager.ticketBlob, ModIOManager.ticketBlob.Length, out ModIOManager.ticketSize))
				{
					ModioLog error3 = ModioLog.Error;
					if (error3 != null)
					{
						error3.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] Failed to retrieve " + string.Format("EncryptedAppTicket! Needed size: {0}", ModIOManager.ticketSize));
					}
					Action<bool, string> action3 = this.requestEncryptedAppTicketCallback;
					if (action3 != null)
					{
						action3(false, "FAILED TO RETRIEVE 'EncryptedAppTicket'.");
					}
					this.requestEncryptedAppTicketCallback = null;
					return;
				}
				Array.Resize<byte>(ref ModIOManager.ticketBlob, (int)ModIOManager.ticketSize);
				string text = Convert.ToBase64String(ModIOManager.ticketBlob);
				ModioLog verbose = ModioLog.Verbose;
				if (verbose != null)
				{
					verbose.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] Successfully retrieved Steam Encrypted App Ticket: " + text);
				}
				Action<bool, string> action4 = this.requestEncryptedAppTicketCallback;
				if (action4 != null)
				{
					action4(true, text);
				}
				this.requestEncryptedAppTicketCallback = null;
				return;
			}
		}
		else
		{
			if (eResult == EResult.k_EResultLimitExceeded)
			{
				ModioLog error4 = ModioLog.Error;
				if (error4 != null)
				{
					error4.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] Rate Limit exceeded, this function should not be called more than once per minute.");
				}
				Action<bool, string> action5 = this.requestEncryptedAppTicketCallback;
				if (action5 != null)
				{
					action5(false, "RATE LIMIT EXCEEDED, CAN ONLY REQUEST ONE 'EncryptedAppTicket' PER MINUTE.");
				}
				this.requestEncryptedAppTicketCallback = null;
				return;
			}
			if (eResult == EResult.k_EResultDuplicateRequest)
			{
				ModioLog error5 = ModioLog.Error;
				if (error5 != null)
				{
					error5.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] There is already a pending EncryptedAppTicket request.");
				}
				Action<bool, string> action6 = this.requestEncryptedAppTicketCallback;
				if (action6 != null)
				{
					action6(false, "THERE IS ALREADY AN 'EncryptedAppTicket' REQUEST IN PROGRESS.");
				}
				this.requestEncryptedAppTicketCallback = null;
				return;
			}
		}
		ModioLog error6 = ModioLog.Error;
		if (error6 != null)
		{
			error6.Log(string.Format("[ModIOManager::OnRequestEncryptedAppTicketFinished] Unknown Error: {0}", response.m_eResult));
		}
		Action<bool, string> action7 = this.requestEncryptedAppTicketCallback;
		if (action7 != null)
		{
			action7(false, string.Format("{0}", response.m_eResult));
		}
		this.requestEncryptedAppTicketCallback = null;
	}

	// Token: 0x0600442F RID: 17455 RVA: 0x0016D240 File Offset: 0x0016B440
	public Task<ValueTuple<Error, string>> GetOculusUserId()
	{
		ModIOManager.<GetOculusUserId>d__90 <GetOculusUserId>d__;
		<GetOculusUserId>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, string>>.Create();
		<GetOculusUserId>d__.<>1__state = -1;
		<GetOculusUserId>d__.<>t__builder.Start<ModIOManager.<GetOculusUserId>d__90>(ref <GetOculusUserId>d__);
		return <GetOculusUserId>d__.<>t__builder.Task;
	}

	// Token: 0x06004430 RID: 17456 RVA: 0x0016D27C File Offset: 0x0016B47C
	public Task<string> GetOculusAccessToken()
	{
		ModIOManager.<GetOculusAccessToken>d__91 <GetOculusAccessToken>d__;
		<GetOculusAccessToken>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetOculusAccessToken>d__.<>1__state = -1;
		<GetOculusAccessToken>d__.<>t__builder.Start<ModIOManager.<GetOculusAccessToken>d__91>(ref <GetOculusAccessToken>d__);
		return <GetOculusAccessToken>d__.<>t__builder.Task;
	}

	// Token: 0x06004431 RID: 17457 RVA: 0x0016D2B8 File Offset: 0x0016B4B8
	public Task<string> GetOculusUserProof()
	{
		ModIOManager.<GetOculusUserProof>d__92 <GetOculusUserProof>d__;
		<GetOculusUserProof>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetOculusUserProof>d__.<>1__state = -1;
		<GetOculusUserProof>d__.<>t__builder.Start<ModIOManager.<GetOculusUserProof>d__92>(ref <GetOculusUserProof>d__);
		return <GetOculusUserProof>d__.<>t__builder.Task;
	}

	// Token: 0x06004432 RID: 17458 RVA: 0x0016D2F3 File Offset: 0x0016B4F3
	public string GetOculusDevice()
	{
		return "";
	}

	// Token: 0x06004433 RID: 17459 RVA: 0x0016D2FA File Offset: 0x0016B4FA
	private static void OnAuthenticationComplete(Error error)
	{
		ModIOManager.loggingIn = false;
		if (error)
		{
			UnityEvent<string> onModIOLoginFailed = ModIOManager.OnModIOLoginFailed;
			if (onModIOLoginFailed == null)
			{
				return;
			}
			onModIOLoginFailed.Invoke(string.Format("FAILED TO LOGIN TO MOD.IO: {0}", error));
			return;
		}
		else
		{
			UnityEvent onModIOLoggedIn = ModIOManager.OnModIOLoggedIn;
			if (onModIOLoggedIn == null)
			{
				return;
			}
			onModIOLoggedIn.Invoke();
			return;
		}
	}

	// Token: 0x06004434 RID: 17460 RVA: 0x0016D334 File Offset: 0x0016B534
	public static ModIOManager.ModIOAuthMethod GetLastAuthMethod()
	{
		int @int = PlayerPrefs.GetInt("modIOLassSuccessfulAuthMethod", -1);
		if (@int == -1)
		{
			return ModIOManager.ModIOAuthMethod.Invalid;
		}
		return (ModIOManager.ModIOAuthMethod)@int;
	}

	// Token: 0x06004435 RID: 17461 RVA: 0x0016D354 File Offset: 0x0016B554
	public static Task<ValueTuple<Error, Mod[]>> GetSubscribedMods()
	{
		ModIOManager.<GetSubscribedMods>d__96 <GetSubscribedMods>d__;
		<GetSubscribedMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Mod[]>>.Create();
		<GetSubscribedMods>d__.<>1__state = -1;
		<GetSubscribedMods>d__.<>t__builder.Start<ModIOManager.<GetSubscribedMods>d__96>(ref <GetSubscribedMods>d__);
		return <GetSubscribedMods>d__.<>t__builder.Task;
	}

	// Token: 0x06004436 RID: 17462 RVA: 0x0016D390 File Offset: 0x0016B590
	public static Task<Error> SubscribeToMod(ModId modId, Action<Error> callback)
	{
		ModIOManager.<SubscribeToMod>d__97 <SubscribeToMod>d__;
		<SubscribeToMod>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<SubscribeToMod>d__.modId = modId;
		<SubscribeToMod>d__.callback = callback;
		<SubscribeToMod>d__.<>1__state = -1;
		<SubscribeToMod>d__.<>t__builder.Start<ModIOManager.<SubscribeToMod>d__97>(ref <SubscribeToMod>d__);
		return <SubscribeToMod>d__.<>t__builder.Task;
	}

	// Token: 0x06004437 RID: 17463 RVA: 0x0016D3DC File Offset: 0x0016B5DC
	public static Task<Error> UnsubscribeFromMod(ModId modId, Action<Error> callback)
	{
		ModIOManager.<UnsubscribeFromMod>d__98 <UnsubscribeFromMod>d__;
		<UnsubscribeFromMod>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<UnsubscribeFromMod>d__.modId = modId;
		<UnsubscribeFromMod>d__.callback = callback;
		<UnsubscribeFromMod>d__.<>1__state = -1;
		<UnsubscribeFromMod>d__.<>t__builder.Start<ModIOManager.<UnsubscribeFromMod>d__98>(ref <UnsubscribeFromMod>d__);
		return <UnsubscribeFromMod>d__.<>t__builder.Task;
	}

	// Token: 0x06004438 RID: 17464 RVA: 0x0016D428 File Offset: 0x0016B628
	public static Task<ValueTuple<bool, ModFileState>> GetSubscribedModStatus(ModId modId)
	{
		ModIOManager.<GetSubscribedModStatus>d__99 <GetSubscribedModStatus>d__;
		<GetSubscribedModStatus>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, ModFileState>>.Create();
		<GetSubscribedModStatus>d__.modId = modId;
		<GetSubscribedModStatus>d__.<>1__state = -1;
		<GetSubscribedModStatus>d__.<>t__builder.Start<ModIOManager.<GetSubscribedModStatus>d__99>(ref <GetSubscribedModStatus>d__);
		return <GetSubscribedModStatus>d__.<>t__builder.Task;
	}

	// Token: 0x06004439 RID: 17465 RVA: 0x0016D46C File Offset: 0x0016B66C
	public static Task<ValueTuple<bool, Mod>> GetSubscribedModProfile(ModId modId, Action<bool, Mod> callback = null)
	{
		ModIOManager.<GetSubscribedModProfile>d__100 <GetSubscribedModProfile>d__;
		<GetSubscribedModProfile>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Mod>>.Create();
		<GetSubscribedModProfile>d__.modId = modId;
		<GetSubscribedModProfile>d__.callback = callback;
		<GetSubscribedModProfile>d__.<>1__state = -1;
		<GetSubscribedModProfile>d__.<>t__builder.Start<ModIOManager.<GetSubscribedModProfile>d__100>(ref <GetSubscribedModProfile>d__);
		return <GetSubscribedModProfile>d__.<>t__builder.Task;
	}

	// Token: 0x0600443A RID: 17466 RVA: 0x0016D4B8 File Offset: 0x0016B6B8
	public static Task<ModFileState> GetModStatus(ModId modId)
	{
		ModIOManager.<GetModStatus>d__101 <GetModStatus>d__;
		<GetModStatus>d__.<>t__builder = AsyncTaskMethodBuilder<ModFileState>.Create();
		<GetModStatus>d__.modId = modId;
		<GetModStatus>d__.<>1__state = -1;
		<GetModStatus>d__.<>t__builder.Start<ModIOManager.<GetModStatus>d__101>(ref <GetModStatus>d__);
		return <GetModStatus>d__.<>t__builder.Task;
	}

	// Token: 0x0600443B RID: 17467 RVA: 0x0016D4FC File Offset: 0x0016B6FC
	public static Task<bool> DownloadMod(ModId modId, Action<bool> callback = null)
	{
		ModIOManager.<DownloadMod>d__102 <DownloadMod>d__;
		<DownloadMod>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<DownloadMod>d__.modId = modId;
		<DownloadMod>d__.callback = callback;
		<DownloadMod>d__.<>1__state = -1;
		<DownloadMod>d__.<>t__builder.Start<ModIOManager.<DownloadMod>d__102>(ref <DownloadMod>d__);
		return <DownloadMod>d__.<>t__builder.Task;
	}

	// Token: 0x0600443C RID: 17468 RVA: 0x0016D548 File Offset: 0x0016B748
	private void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.RoomName.Contains(GorillaComputer.instance.VStumpRoomPrepend) && !GorillaComputer.instance.IsPlayerInVirtualStump() && !CustomMapManager.IsLocalPlayerInVirtualStump())
		{
			Debug.LogError("[ModIOManager::OnJoinedRoom] Player joined @ room while not in the VStump! Leaving the room...");
			NetworkSystem.Instance.ReturnToSinglePlayer();
		}
	}

	// Token: 0x0600443D RID: 17469 RVA: 0x0016D59C File Offset: 0x0016B79C
	public static bool TryGetNewMapsModId(out ModId newMapsModId)
	{
		newMapsModId = ModId.Null;
		if (!ModIOManager.hasInstance)
		{
			return false;
		}
		newMapsModId = new ModId(ModIOManager.instance.newMapsModId);
		return true;
	}

	// Token: 0x0600443E RID: 17470 RVA: 0x0016D5CA File Offset: 0x0016B7CA
	public static IEnumerator AssociateMothershipAndModIOAccounts(AssociateMotherhsipAndModIOAccountsRequest data, Action<AssociateMotherhsipAndModIOAccountsResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/AssociatePlayFabAndModIO", "POST");
		string s = JsonUtility.ToJson(data);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		request.timeout = 15;
		yield return request.SendWebRequest();
		if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
		{
			AssociateMotherhsipAndModIOAccountsResponse obj = JsonUtility.FromJson<AssociateMotherhsipAndModIOAccountsResponse>(request.downloadHandler.text);
			callback(obj);
		}
		else if (request.result == UnityWebRequest.Result.ProtocolError && request.responseCode != 400L)
		{
			retry = true;
			Debug.LogError(string.Format("HTTP {0} error: {1} message:{2}", request.responseCode, request.error, request.downloadHandler.text));
		}
		else if (request.result == UnityWebRequest.Result.ConnectionError)
		{
			retry = true;
			Debug.LogError("NETWORK ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
		}
		else
		{
			Debug.LogError("HTTP ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
			retry = true;
		}
		if (retry)
		{
			if (ModIOManager.currentAssociationRetries < ModIOManager.associationMaxRetries)
			{
				int num = (int)Mathf.Pow(2f, (float)(ModIOManager.currentAssociationRetries + 1));
				Debug.LogWarning(string.Format("Retrying Account Association... Retry attempt #{0}, waiting for {1} seconds", ModIOManager.currentAssociationRetries + 1, num));
				ModIOManager.currentAssociationRetries++;
				yield return new WaitForSecondsRealtime((float)num);
				ModIOManager.AssociateMothershipAndModIOAccounts(data, callback);
			}
			else
			{
				Debug.LogError("Maximum retries attempted. Please check your network connection.");
				callback(null);
			}
		}
		yield break;
	}

	// Token: 0x040055D5 RID: 21973
	private const string MODIO_ACCEPTED_TERMS_KEY = "modIOAcceptedTermsHash";

	// Token: 0x040055D6 RID: 21974
	private const string MODIO_ACCEPTED_TERMS_OF_USE_ID_KEY = "modIOAcceptedTermsOfUseId";

	// Token: 0x040055D7 RID: 21975
	private const string MODIO_ACCEPTED_PRIVACY_POLICY_ID_KEY = "modIOAcceptedPrivacyPolicyId";

	// Token: 0x040055D8 RID: 21976
	private const string MODIO_LAST_AUTH_METHOD_KEY = "modIOLassSuccessfulAuthMethod";

	// Token: 0x040055D9 RID: 21977
	private const string FAVORITES_FILE_NAME = "favoriteMods.json";

	// Token: 0x040055DA RID: 21978
	private const float REFRESH_RATE_LIMIT = 5f;

	// Token: 0x040055DB RID: 21979
	[OnEnterPlay_SetNull]
	private static volatile ModIOManager instance;

	// Token: 0x040055DC RID: 21980
	[OnEnterPlay_Set(false)]
	private static bool hasInstance;

	// Token: 0x040055DD RID: 21981
	private static string ModIODirectory;

	// Token: 0x040055DE RID: 21982
	private static ModioWssAuthService accountLinkingAuthService = new ModioWssAuthService();

	// Token: 0x040055DF RID: 21983
	private static bool initialized;

	// Token: 0x040055E0 RID: 21984
	private static bool refreshing;

	// Token: 0x040055E1 RID: 21985
	private static bool modManagementEnabled;

	// Token: 0x040055E2 RID: 21986
	private static bool loggingIn;

	// Token: 0x040055E3 RID: 21987
	private static bool loggingOut;

	// Token: 0x040055E4 RID: 21988
	private static bool refreshingModCache;

	// Token: 0x040055E5 RID: 21989
	private static bool favoriteModsLoaded;

	// Token: 0x040055E6 RID: 21990
	private static bool restartRefreshModCache;

	// Token: 0x040055E7 RID: 21991
	private static Coroutine refreshDisabledCoroutine;

	// Token: 0x040055E8 RID: 21992
	private static float lastRefreshTime;

	// Token: 0x040055E9 RID: 21993
	private static List<Action<bool>> currentRefreshCallbacks = new List<Action<bool>>();

	// Token: 0x040055EA RID: 21994
	private static Action<ModIORequestResultAnd<bool>> modIOTermsAcknowledgedCallback;

	// Token: 0x040055EB RID: 21995
	private static Dictionary<ModId, Mod> favoriteMods = new Dictionary<ModId, Mod>();

	// Token: 0x040055EC RID: 21996
	private static Dictionary<ModId, int> outdatedModCMSVersions = new Dictionary<ModId, int>();

	// Token: 0x040055ED RID: 21997
	private static byte[] ticketBlob = new byte[1024];

	// Token: 0x040055EE RID: 21998
	private static uint ticketSize;

	// Token: 0x040055EF RID: 21999
	protected static CallResult<EncryptedAppTicketResponse_t> requestEncryptedAppTicketResponse = null;

	// Token: 0x040055F0 RID: 22000
	private Action<bool, string> requestEncryptedAppTicketCallback;

	// Token: 0x040055F1 RID: 22001
	private static ModioSteamAuthService steamAuthService = new ModioSteamAuthService();

	// Token: 0x040055F2 RID: 22002
	[SerializeField]
	private GameObject modIOTermsOfUsePrefab;

	// Token: 0x040055F3 RID: 22003
	[SerializeField]
	private long newMapsModId;

	// Token: 0x040055F4 RID: 22004
	public static UnityEvent OnModIOLoginStarted = new UnityEvent();

	// Token: 0x040055F5 RID: 22005
	public static UnityEvent OnModIOLoggedIn = new UnityEvent();

	// Token: 0x040055F6 RID: 22006
	public static UnityEvent<string> OnModIOLoginFailed = new UnityEvent<string>();

	// Token: 0x040055F7 RID: 22007
	public static UnityEvent OnModIOLoggedOut = new UnityEvent();

	// Token: 0x040055F8 RID: 22008
	public static UnityEvent<User> OnModIOUserChanged = new UnityEvent<User>();

	// Token: 0x040055F9 RID: 22009
	public static UnityEvent<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase> OnModManagementEvent = new UnityEvent<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>();

	// Token: 0x040055FA RID: 22010
	public static UnityEvent OnModIOCacheRefreshing = new UnityEvent();

	// Token: 0x040055FB RID: 22011
	public static UnityEvent OnModIOCacheRefreshed = new UnityEvent();

	// Token: 0x040055FC RID: 22012
	private static int associationMaxRetries = 5;

	// Token: 0x040055FD RID: 22013
	private static int currentAssociationRetries = 0;

	// Token: 0x02000A5D RID: 2653
	public enum ModIOAuthMethod
	{
		// Token: 0x040055FF RID: 22015
		Invalid,
		// Token: 0x04005600 RID: 22016
		LinkedAccount,
		// Token: 0x04005601 RID: 22017
		Steam,
		// Token: 0x04005602 RID: 22018
		Oculus
	}
}
