using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CosmeticRoom;
using CustomMapSupport;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion.Swimming;
using GorillaNetworking;
using GorillaNetworking.Store;
using GorillaTag.Rendering;
using GorillaTagScripts;
using GorillaTagScripts.CustomMapSupport;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using Modio;
using Modio.Mods;
using Newtonsoft.Json;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;

// Token: 0x02000A34 RID: 2612
public class CustomMapLoader : MonoBehaviour, IBuildValidation
{
	// Token: 0x060042C5 RID: 17093 RVA: 0x00164A8F File Offset: 0x00162C8F
	internal static void SetZoneDynamicLighting(bool enable)
	{
		if (enable && !CustomMapLoader.usingDynamicLighting)
		{
			GameLightingManager.instance.ZoneEnableCustomDynamicLighting(true);
			CustomMapLoader.usingDynamicLighting = true;
			return;
		}
		if (!enable && CustomMapLoader.usingDynamicLighting)
		{
			GameLightingManager.instance.ZoneEnableCustomDynamicLighting(false);
			CustomMapLoader.usingDynamicLighting = false;
		}
	}

	// Token: 0x060042C6 RID: 17094 RVA: 0x00164ACC File Offset: 0x00162CCC
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitOnLoad()
	{
		GTDev.Log<string>("CML::InitOnLoad", null);
		CustomMapLoader.instance = null;
		CustomMapLoader.hasInstance = false;
		CustomMapLoader.isLoading = false;
		CustomMapLoader.isUnloading = false;
		CustomMapLoader.runningAsyncLoad = false;
		CustomMapLoader.attemptedLoadID = 0L;
		CustomMapLoader.attemptedSceneToLoad = null;
		CustomMapLoader.shouldAbortMapLoading = false;
		CustomMapLoader.shouldAbortSceneLoad = false;
		CustomMapLoader.errorEncounteredDuringLoad = false;
		CustomMapLoader.unloadMapCallback = null;
		CustomMapLoader.cachedExceptionMessage = "";
		CustomMapLoader.mapBundle = null;
		CustomMapLoader.initialSceneNames = new List<string>();
		CustomMapLoader.initialSceneIndexes = new List<int>();
		CustomMapLoader.maxPlayersForMap = 20;
		CustomMapLoader.loadedMapModId = ModId.Null;
		CustomMapLoader.loadedMapModFileId = -1L;
		CustomMapLoader.loadedMapPackageInfo = null;
		CustomMapLoader.cachedLuauScript = null;
		CustomMapLoader.devModeEnabled = false;
		CustomMapLoader.disableHoldingHandsAllModes = false;
		CustomMapLoader.disableHoldingHandsCustomMode = false;
		CustomMapLoader.mapLoadProgressCallback = null;
		CustomMapLoader.mapLoadFinishedCallback = null;
		CustomMapLoader.zoneLoadingCoroutine = null;
		CustomMapLoader.sceneLoadedCallback = null;
		CustomMapLoader.sceneUnloadedCallback = null;
		CustomMapLoader.queuedLoadZoneRequests = new List<CustomMapLoader.LoadZoneRequest>();
		CustomMapLoader.assetBundleSceneFilePaths = null;
		CustomMapLoader.loadedSceneFilePaths = new List<string>();
		CustomMapLoader.loadedSceneNames = new List<string>();
		CustomMapLoader.loadedSceneIndexes = new List<int>();
		CustomMapLoader.leafGliderIndex = 0;
		CustomMapLoader.usingDynamicLighting = false;
		CustomMapLoader.totalObjectsInLoadingScene = 0;
		CustomMapLoader.objectsProcessedForLoadingScene = 0;
		CustomMapLoader.objectsProcessedThisFrame = 0;
		CustomMapLoader.initializePhaseTwoComponents = new List<Component>();
		CustomMapLoader.entitiesToCreate = new List<MapEntity>(Constants.aiAgentLimit);
		CustomMapLoader.lightmaps = null;
		CustomMapLoader.lightmapsToKeep = new List<Texture2D>();
		CustomMapLoader.placeholderReplacements = new List<GameObject>();
		CustomMapLoader.customMapATM = null;
		CustomMapLoader.storeCheckouts = new List<GameObject>();
		CustomMapLoader.storeDisplayStands = new List<GameObject>();
		CustomMapLoader.storeTryOnConsoles = new List<GameObject>();
		CustomMapLoader.storeTryOnAreas = new List<GameObject>();
	}

	// Token: 0x060042C7 RID: 17095 RVA: 0x00164C4E File Offset: 0x00162E4E
	private void Awake()
	{
		if (CustomMapLoader.instance == null)
		{
			CustomMapLoader.instance = this;
			CustomMapLoader.hasInstance = true;
			return;
		}
		if (CustomMapLoader.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060042C8 RID: 17096 RVA: 0x00164C88 File Offset: 0x00162E88
	private void Start()
	{
		byte[] bytes = new byte[]
		{
			Convert.ToByte(68),
			Convert.ToByte(111),
			Convert.ToByte(110),
			Convert.ToByte(116),
			Convert.ToByte(68),
			Convert.ToByte(101),
			Convert.ToByte(115),
			Convert.ToByte(116),
			Convert.ToByte(114),
			Convert.ToByte(111),
			Convert.ToByte(121),
			Convert.ToByte(79),
			Convert.ToByte(110),
			Convert.ToByte(76),
			Convert.ToByte(111),
			Convert.ToByte(97),
			Convert.ToByte(100)
		};
		this.dontDestroyOnLoadSceneName = Encoding.ASCII.GetString(bytes);
		if (this.publicJoinTrigger != null)
		{
			this.publicJoinTrigger.SetActive(false);
		}
	}

	// Token: 0x060042C9 RID: 17097 RVA: 0x00164D7A File Offset: 0x00162F7A
	public static void Initialize(Action<MapLoadStatus, int, string> onLoadProgress, Action<bool> onLoadFinished, Action<string> onSceneLoaded, Action<string> onSceneUnloaded)
	{
		CustomMapLoader.mapLoadProgressCallback = onLoadProgress;
		CustomMapLoader.mapLoadFinishedCallback = onLoadFinished;
		CustomMapLoader.sceneLoadedCallback = onSceneLoaded;
		CustomMapLoader.sceneUnloadedCallback = onSceneUnloaded;
	}

	// Token: 0x060042CA RID: 17098 RVA: 0x00164D94 File Offset: 0x00162F94
	public static void LoadMap(long mapModId, string mapFilePath)
	{
		if (!CustomMapLoader.hasInstance)
		{
			return;
		}
		if (CustomMapLoader.isLoading)
		{
			return;
		}
		if (CustomMapLoader.isUnloading)
		{
			Action<bool> action = CustomMapLoader.mapLoadFinishedCallback;
			if (action == null)
			{
				return;
			}
			action(false);
			return;
		}
		else
		{
			if (!CustomMapLoader.IsMapLoaded(mapModId))
			{
				GorillaNetworkJoinTrigger.DisableTriggerJoins();
				CustomMapLoader.CanLoadEntities = false;
				CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadAssetBundle(mapModId, mapFilePath, new Action<bool, bool>(CustomMapLoader.OnAssetBundleLoaded)));
				return;
			}
			Action<bool> action2 = CustomMapLoader.mapLoadFinishedCallback;
			if (action2 == null)
			{
				return;
			}
			action2(true);
			return;
		}
	}

	// Token: 0x060042CB RID: 17099 RVA: 0x00164E12 File Offset: 0x00163012
	public static bool OpenDoorToMap()
	{
		if (!CustomMapLoader.hasInstance)
		{
			return false;
		}
		if (CustomMapLoader.instance.accessDoor != null)
		{
			CustomMapLoader.instance.accessDoor.OpenDoor();
			return true;
		}
		return false;
	}

	// Token: 0x060042CC RID: 17100 RVA: 0x00164E45 File Offset: 0x00163045
	private static IEnumerator LoadAssetBundle(long mapModID, string packageInfoFilePath, Action<bool, bool> OnLoadComplete)
	{
		CustomMapLoader.isLoading = true;
		CustomMapLoader.errorEncounteredDuringLoad = false;
		CustomMapLoader.attemptedLoadID = mapModID;
		CustomMapLoader.refreshReviveStations = false;
		CustomMapLoader.instance.ghostReactorManager.reactor.RefreshReviveStations(false);
		Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
		if (action != null)
		{
			action(MapLoadStatus.Loading, 1, "CACHING LIGHTMAP DATA");
		}
		CustomMapLoader.CacheLightmaps();
		Action<MapLoadStatus, int, string> action2 = CustomMapLoader.mapLoadProgressCallback;
		if (action2 != null)
		{
			action2(MapLoadStatus.Loading, 2, "LOADING PACKAGE INFO");
		}
		try
		{
			CustomMapLoader.loadedMapPackageInfo = CustomMapLoader.GetPackageInfo(packageInfoFilePath);
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("[CML.LoadAssetBundle] GetPackageInfo Exception: {0}", ex));
			Action<MapLoadStatus, int, string> action3 = CustomMapLoader.mapLoadProgressCallback;
			if (action3 != null)
			{
				action3(MapLoadStatus.Error, 0, ex.ToString());
			}
			OnLoadComplete(false, false);
			yield break;
		}
		if (CustomMapLoader.loadedMapPackageInfo == null)
		{
			Action<MapLoadStatus, int, string> action4 = CustomMapLoader.mapLoadProgressCallback;
			if (action4 != null)
			{
				action4(MapLoadStatus.Error, 0, "FAILED TO READ FILE AT " + packageInfoFilePath);
			}
			OnLoadComplete(false, false);
			yield break;
		}
		CustomMapLoader.LoadInitialSceneNames();
		Action<MapLoadStatus, int, string> action5 = CustomMapLoader.mapLoadProgressCallback;
		if (action5 != null)
		{
			action5(MapLoadStatus.Loading, 3, "PACKAGE INFO LOADED");
		}
		string path = Path.GetDirectoryName(packageInfoFilePath) + "/" + CustomMapLoader.loadedMapPackageInfo.pcFileName;
		Action<MapLoadStatus, int, string> action6 = CustomMapLoader.mapLoadProgressCallback;
		if (action6 != null)
		{
			action6(MapLoadStatus.Loading, 4, "LOADING MAP ASSET BUNDLE");
		}
		AssetBundleCreateRequest loadBundleRequest = AssetBundle.LoadFromFileAsync(path);
		yield return loadBundleRequest;
		CustomMapLoader.mapBundle = loadBundleRequest.assetBundle;
		if (CustomMapLoader.shouldAbortMapLoading || CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(-1);
			OnLoadComplete(false, true);
			yield break;
		}
		if (CustomMapLoader.mapBundle == null)
		{
			Action<MapLoadStatus, int, string> action7 = CustomMapLoader.mapLoadProgressCallback;
			if (action7 != null)
			{
				action7(MapLoadStatus.Error, 0, "CUSTOM MAP ASSET BUNDLE FAILED TO LOAD");
			}
			OnLoadComplete(false, false);
			yield break;
		}
		if (!CustomMapLoader.mapBundle.isStreamedSceneAssetBundle)
		{
			CustomMapLoader.mapBundle.Unload(true);
			Action<MapLoadStatus, int, string> action8 = CustomMapLoader.mapLoadProgressCallback;
			if (action8 != null)
			{
				action8(MapLoadStatus.Error, 0, "AssetBundle does not contain a Unity Scene file");
			}
			OnLoadComplete(false, false);
			yield break;
		}
		Action<MapLoadStatus, int, string> action9 = CustomMapLoader.mapLoadProgressCallback;
		if (action9 != null)
		{
			action9(MapLoadStatus.Loading, 10, "MAP ASSET BUNDLE LOADED");
		}
		CustomMapLoader.assetBundleSceneFilePaths = CustomMapLoader.mapBundle.GetAllScenePaths();
		if (CustomMapLoader.assetBundleSceneFilePaths.Length == 0)
		{
			CustomMapLoader.mapBundle.Unload(true);
			Action<MapLoadStatus, int, string> action10 = CustomMapLoader.mapLoadProgressCallback;
			if (action10 != null)
			{
				action10(MapLoadStatus.Error, 0, "AssetBundle does not contain a Unity Scene file");
			}
			OnLoadComplete(false, false);
			yield break;
		}
		foreach (string text in CustomMapLoader.assetBundleSceneFilePaths)
		{
			if (text.Equals(CustomMapLoader.instance.dontDestroyOnLoadSceneName, StringComparison.OrdinalIgnoreCase))
			{
				CustomMapLoader.mapBundle.Unload(true);
				Action<MapLoadStatus, int, string> action11 = CustomMapLoader.mapLoadProgressCallback;
				if (action11 != null)
				{
					action11(MapLoadStatus.Error, 0, "Map name is " + text + " this is an invalid name");
				}
				OnLoadComplete(false, false);
				yield break;
			}
		}
		OnLoadComplete(true, false);
		yield break;
	}

	// Token: 0x060042CD RID: 17101 RVA: 0x00164E64 File Offset: 0x00163064
	private static void LoadInitialSceneNames()
	{
		CustomMapLoader.initialSceneNames.Clear();
		if (CustomMapLoader.loadedMapPackageInfo != null)
		{
			if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion <= 2)
			{
				CustomMapLoader.initialSceneNames.Add(CustomMapLoader.loadedMapPackageInfo.initialScene);
				return;
			}
			if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion > 2)
			{
				CustomMapLoader.initialSceneNames.AddRange(CustomMapLoader.loadedMapPackageInfo.initialScenes);
			}
		}
	}

	// Token: 0x060042CE RID: 17102 RVA: 0x00164EC8 File Offset: 0x001630C8
	private static void OnAssetBundleLoaded(bool loadSucceeded, bool loadAborted)
	{
		if (loadAborted)
		{
			return;
		}
		if (loadSucceeded)
		{
			CustomMapLoader.loadedMapModId = CustomMapLoader.attemptedLoadID;
			CustomMapLoader.loadedMapModFileId = 0L;
			ModIOManager.GetMod(new ModId(CustomMapLoader.loadedMapModId), false, delegate(Error error, Mod mod)
			{
				if (!error && mod != null && mod.File != null)
				{
					CustomMapLoader.loadedMapModFileId = mod.File.Id;
				}
			});
			foreach (string text in CustomMapLoader.initialSceneNames)
			{
				int num = -1;
				if (text != string.Empty)
				{
					num = CustomMapLoader.GetSceneIndex(text);
				}
				if (num == -1)
				{
					GTDev.LogError<string>("[CustomMapLoader::OnAssetBundleLoaded] Encountered invalid initial scene, could not get scene index for: \"" + text + "\"", null);
				}
				else
				{
					CustomMapLoader.initialSceneIndexes.Add(num);
				}
			}
			if (CustomMapLoader.initialSceneIndexes.Count == 0)
			{
				if (CustomMapLoader.assetBundleSceneFilePaths.Length == 1)
				{
					GTDev.LogWarning<string>("[CustomMapLoader::OnAssetBundleLoaded] Asset Bundle only contains 1 Scene, but it isn't marked as an initial scene. Treating it as an initial scene...", null);
					CustomMapLoader.initialSceneIndexes.Add(0);
				}
				else if (CustomMapLoader.mapBundle != null)
				{
					string arg = "";
					if (CustomMapLoader.assetBundleSceneFilePaths.Length == 0)
					{
						arg = "MAP ASSET BUNDLE CONTAINS NO VALID SCENES.";
					}
					else if (CustomMapLoader.assetBundleSceneFilePaths.Length > 1)
					{
						arg = "MAP ASSET BUNDLE CONTAINS MULTIPLE SCENES, BUT NONE ARE SET AS INITIAL SCENE.";
					}
					Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
					if (action != null)
					{
						action(MapLoadStatus.Error, 0, arg);
					}
					CustomMapLoader.OnInitialLoadComplete(false, true);
				}
			}
			CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadInitialScenesCoroutine(CustomMapLoader.initialSceneIndexes.ToArray()));
		}
	}

	// Token: 0x060042CF RID: 17103 RVA: 0x0016503C File Offset: 0x0016323C
	private static IEnumerator LoadInitialScenesCoroutine(int[] sceneIndexes)
	{
		CustomMapLoader.<>c__DisplayClass100_0 CS$<>8__locals1 = new CustomMapLoader.<>c__DisplayClass100_0();
		CS$<>8__locals1.sceneIndexes = sceneIndexes;
		if (!CustomMapLoader.loadedSceneIndexes.IsNullOrEmpty<int>())
		{
			GTDev.LogError<string>("[CustomMapLoader::LoadInitialScenesCoroutine] loadedSceneIndexes is not empty, LoadInitialScenes should not be called in this case!", null);
			yield break;
		}
		int progressAmountPerScene = 89 / CS$<>8__locals1.sceneIndexes.Length;
		GTDev.Log<string>(string.Format("[CustomMapLoader::LoadInitialScenesCoroutine] loading {0} scenes...", CS$<>8__locals1.sceneIndexes.Length), null);
		CS$<>8__locals1.i = 0;
		while (CS$<>8__locals1.i < CS$<>8__locals1.sceneIndexes.Length)
		{
			CustomMapLoader.<>c__DisplayClass100_1 CS$<>8__locals2 = new CustomMapLoader.<>c__DisplayClass100_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			int num = 10 + CS$<>8__locals2.CS$<>8__locals1.i * progressAmountPerScene;
			int endingProgress = num + progressAmountPerScene;
			CS$<>8__locals2.isLastScene = (CS$<>8__locals2.CS$<>8__locals1.i == CS$<>8__locals2.CS$<>8__locals1.sceneIndexes.Length - 1);
			CS$<>8__locals2.stopLoading = false;
			CS$<>8__locals2.initialLoadAborted = false;
			yield return CustomMapLoader.LoadSceneFromAssetBundle(CS$<>8__locals2.CS$<>8__locals1.sceneIndexes[CS$<>8__locals2.CS$<>8__locals1.i], delegate(bool loadSucceeded, bool loadAborted, string loadedSceneName)
			{
				if (!loadSucceeded || loadAborted)
				{
					GTDev.Log<string>("[CustomMapLoader::LoadInitialScenesCoroutine] failed to load scene at index " + string.Format("\"{0}\", aborting initial load...", CS$<>8__locals2.CS$<>8__locals1.sceneIndexes[CS$<>8__locals2.CS$<>8__locals1.i]), null);
					CS$<>8__locals2.stopLoading = true;
					CS$<>8__locals2.initialLoadAborted = loadAborted;
					return;
				}
				if (CS$<>8__locals2.isLastScene)
				{
					CustomMapLoader.OnInitialLoadComplete(true, false);
				}
			}, true, num, endingProgress);
			if (CS$<>8__locals2.stopLoading || CustomMapLoader.shouldAbortMapLoading)
			{
				CustomMapLoader.OnInitialLoadComplete(false, CS$<>8__locals2.initialLoadAborted);
				break;
			}
			CS$<>8__locals2 = null;
			int i = CS$<>8__locals1.i;
			CS$<>8__locals1.i = i + 1;
		}
		yield break;
	}

	// Token: 0x060042D0 RID: 17104 RVA: 0x0016504C File Offset: 0x0016324C
	private static void OnInitialLoadComplete(bool loadSucceeded, bool loadAborted)
	{
		if (loadAborted || !loadSucceeded)
		{
			if (!loadAborted)
			{
				CustomMapLoader.instance.StartCoroutine(CustomMapLoader.AbortMapLoad());
				return;
			}
			Action<bool> action = CustomMapLoader.mapLoadFinishedCallback;
			if (action == null)
			{
				return;
			}
			action(false);
			return;
		}
		else
		{
			if (CustomMapLoader.loadedMapPackageInfo != null && CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion >= 3)
			{
				CustomMapLoader.maxPlayersForMap = (byte)Math.Clamp(CustomMapLoader.loadedMapPackageInfo.maxPlayers, 1, 20);
				if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion >= 5)
				{
					CustomMapModeSelector.SetAvailableGameModes(CustomMapLoader.loadedMapPackageInfo.availableGameModes, CustomMapLoader.loadedMapPackageInfo.defaultGameMode);
					if (RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
					{
						if (GameMode.ActiveGameMode.IsNull())
						{
							GameModeType defaultGameMode = (GameModeType)CustomMapLoader.loadedMapPackageInfo.defaultGameMode;
							GameMode.ChangeGameMode(defaultGameMode.ToString());
						}
						else if (GameMode.ActiveGameMode.GameType() != (GameModeType)CustomMapLoader.loadedMapPackageInfo.defaultGameMode)
						{
							GameModeType defaultGameMode = (GameModeType)CustomMapLoader.loadedMapPackageInfo.defaultGameMode;
							GameMode.ChangeGameMode(defaultGameMode.ToString());
						}
					}
				}
				else
				{
					List<int> list = new List<int>();
					foreach (GameModeType item in CustomMapLoader.instance.availableModesForOldMaps)
					{
						list.Add((int)item);
					}
					GameModeType gameModeType = CustomMapLoader.instance.defaultGameModeForNonCustomOldMaps;
					if (!CustomMapLoader.loadedMapPackageInfo.customGamemodeScript.IsNullOrEmpty())
					{
						gameModeType = GameModeType.Custom;
						list.Add(7);
					}
					CustomMapModeSelector.SetAvailableGameModes(list.ToArray(), (int)gameModeType);
					if (RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
					{
						if (GameMode.ActiveGameMode.IsNull())
						{
							GameMode.ChangeGameMode(gameModeType.ToString());
						}
						else if (GameMode.ActiveGameMode.GameType() != gameModeType)
						{
							GameMode.ChangeGameMode(gameModeType.ToString());
						}
					}
				}
				CustomMapLoader.cachedLuauScript = CustomMapLoader.loadedMapPackageInfo.customGamemodeScript;
				CustomMapLoader.devModeEnabled = CustomMapLoader.loadedMapPackageInfo.devMode;
				CustomMapLoader.disableHoldingHandsAllModes = CustomMapLoader.loadedMapPackageInfo.disableHoldingHandsAllModes;
				CustomMapLoader.disableHoldingHandsCustomMode = CustomMapLoader.loadedMapPackageInfo.disableHoldingHandsCustomMode;
				Color ambientLightDynamic = new Color(CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_R, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_G, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_B, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_A);
				if (CustomMapLoader.loadedMapPackageInfo.useUberShaderDynamicLighting)
				{
					CustomMapLoader.SetZoneDynamicLighting(true);
					GameLightingManager.instance.SetAmbientLightDynamic(ambientLightDynamic);
				}
				VirtualStumpReturnWatch.SetWatchProperties(CustomMapLoader.loadedMapPackageInfo.GetReturnToVStumpWatchProps());
			}
			CustomMapLoader.isLoading = false;
			CustomMapLoader.CanLoadEntities = true;
			GorillaNetworkJoinTrigger.EnableTriggerJoins();
			Action<MapLoadStatus, int, string> action2 = CustomMapLoader.mapLoadProgressCallback;
			if (action2 != null)
			{
				action2(MapLoadStatus.Loading, 100, "LOAD COMPLETE");
			}
			if (CustomMapLoader.instance.publicJoinTrigger != null)
			{
				CustomMapLoader.instance.publicJoinTrigger.SetActive(true);
			}
			foreach (string obj in CustomMapLoader.loadedSceneNames)
			{
				Action<string> action3 = CustomMapLoader.sceneLoadedCallback;
				if (action3 != null)
				{
					action3(obj);
				}
			}
			Action<bool> action4 = CustomMapLoader.mapLoadFinishedCallback;
			if (action4 == null)
			{
				return;
			}
			action4(true);
			return;
		}
	}

	// Token: 0x060042D1 RID: 17105 RVA: 0x001653AC File Offset: 0x001635AC
	private static IEnumerator LoadScenesCoroutine(int[] sceneIndexes, Action<bool, bool, List<string>> loadCompleteCallback = null)
	{
		CustomMapLoader.<>c__DisplayClass102_0 CS$<>8__locals1 = new CustomMapLoader.<>c__DisplayClass102_0();
		CS$<>8__locals1.loadCompleteCallback = loadCompleteCallback;
		if (sceneIndexes.IsNullOrEmpty<int>())
		{
			Action<bool, bool, List<string>> loadCompleteCallback2 = CS$<>8__locals1.loadCompleteCallback;
			if (loadCompleteCallback2 != null)
			{
				loadCompleteCallback2(false, false, null);
			}
			yield break;
		}
		CustomMapLoader.isLoading = true;
		CS$<>8__locals1.successfullyLoadedSceneNames = new List<string>();
		CS$<>8__locals1.successfullyLoadedAllScenes = true;
		int num;
		for (int i = 0; i < sceneIndexes.Length; i = num + 1)
		{
			CustomMapLoader.<>c__DisplayClass102_1 CS$<>8__locals2 = new CustomMapLoader.<>c__DisplayClass102_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			if (CustomMapLoader.loadedSceneIndexes.Contains(sceneIndexes[i]))
			{
				GTDev.LogWarning<string>("[CustomMapLoader::LoadScenesCoroutine] Cannot load scene " + string.Format("{0}:\"{1}\" because it's already loaded!", sceneIndexes[i], CustomMapLoader.assetBundleSceneFilePaths[sceneIndexes[i]]), null);
			}
			else
			{
				CS$<>8__locals2.shouldAbortLoad = false;
				CS$<>8__locals2.isLastScene = (i == sceneIndexes.Length - 1);
				yield return CustomMapLoader.LoadSceneFromAssetBundle(sceneIndexes[i], delegate(bool loadSucceeded, bool loadAborted, string loadedSceneName)
				{
					if (!loadSucceeded || loadAborted)
					{
						CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedAllScenes = false;
					}
					else
					{
						Action<string> action = CustomMapLoader.sceneLoadedCallback;
						if (action != null)
						{
							action(loadedSceneName);
						}
						CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedSceneNames.Add(loadedSceneName);
					}
					if (loadAborted)
					{
						CS$<>8__locals2.shouldAbortLoad = true;
						return;
					}
					if (CS$<>8__locals2.isLastScene)
					{
						Action<bool, bool, List<string>> loadCompleteCallback4 = CS$<>8__locals2.CS$<>8__locals1.loadCompleteCallback;
						if (loadCompleteCallback4 == null)
						{
							return;
						}
						loadCompleteCallback4(CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedAllScenes, false, CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedSceneNames);
					}
				}, false, 10, 90);
				if (CS$<>8__locals2.shouldAbortLoad)
				{
					CustomMapLoader.isLoading = false;
					Action<bool, bool, List<string>> loadCompleteCallback3 = CS$<>8__locals2.CS$<>8__locals1.loadCompleteCallback;
					if (loadCompleteCallback3 == null)
					{
						break;
					}
					loadCompleteCallback3(false, true, CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedSceneNames);
					break;
				}
				else
				{
					CS$<>8__locals2 = null;
				}
			}
			num = i;
		}
		CustomMapLoader.isLoading = false;
		yield break;
	}

	// Token: 0x060042D2 RID: 17106 RVA: 0x001653C2 File Offset: 0x001635C2
	private static IEnumerator LoadSceneFromAssetBundle(int sceneIndex, Action<bool, bool, string> OnLoadComplete, bool useProgressCallback = false, int startingProgress = 10, int endingProgress = 90)
	{
		int progressAmount = endingProgress - startingProgress;
		int currentProgress = startingProgress;
		CustomMapLoader.refreshReviveStations = false;
		LoadSceneParameters parameters = new LoadSceneParameters
		{
			loadSceneMode = LoadSceneMode.Additive,
			localPhysicsMode = LocalPhysicsMode.None
		};
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			OnLoadComplete(false, true, "");
			yield break;
		}
		CustomMapLoader.runningAsyncLoad = true;
		if (useProgressCallback)
		{
			int arg = startingProgress + Mathf.RoundToInt((float)progressAmount * 0.02f);
			Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
			if (action != null)
			{
				action(MapLoadStatus.Loading, arg, "LOADING MAP SCENE");
			}
		}
		CustomMapLoader.attemptedSceneToLoad = CustomMapLoader.assetBundleSceneFilePaths[sceneIndex];
		string sceneName = CustomMapLoader.GetSceneNameFromFilePath(CustomMapLoader.attemptedSceneToLoad);
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(CustomMapLoader.attemptedSceneToLoad, parameters);
		yield return asyncOperation;
		CustomMapLoader.runningAsyncLoad = false;
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			OnLoadComplete(false, true, "");
			yield break;
		}
		if (useProgressCallback)
		{
			currentProgress += Mathf.RoundToInt((float)progressAmount * 0.28f);
			Action<MapLoadStatus, int, string> action2 = CustomMapLoader.mapLoadProgressCallback;
			if (action2 != null)
			{
				action2(MapLoadStatus.Loading, currentProgress, "SANITIZING MAP");
			}
		}
		GameObject[] rootGameObjects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
		List<MapDescriptor> list = new List<MapDescriptor>();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			MapDescriptor component = rootGameObjects[i].GetComponent<MapDescriptor>();
			if (component.IsNotNull())
			{
				list.Add(component);
			}
		}
		MapDescriptor mapDescriptor = null;
		bool flag = false;
		foreach (MapDescriptor mapDescriptor2 in list)
		{
			if (!mapDescriptor.IsNull())
			{
				flag = true;
				break;
			}
			mapDescriptor = mapDescriptor2;
		}
		if (flag)
		{
			GTDev.LogWarning<string>("[CustomMapLoader::LoadSceneFromAssetBundle] Found multiple MapDescriptor components in Scene \"" + sceneName + "\". Only the first one found will be used...", null);
		}
		if (mapDescriptor.IsNull())
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			if (useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action3 = CustomMapLoader.mapLoadProgressCallback;
				if (action3 != null)
				{
					action3(MapLoadStatus.Error, 0, "SCENE \"" + sceneName + "\" DOES NOT CONTAIN A MAP DESCRIPTOR ON ONE OF ITS ROOT GAME OBJECTS.");
				}
			}
			OnLoadComplete(false, false, "");
			yield break;
		}
		GameObject gameObject = mapDescriptor.gameObject;
		if (!CustomMapLoader.SanitizeObject(gameObject, gameObject))
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			if (useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action4 = CustomMapLoader.mapLoadProgressCallback;
				if (action4 != null)
				{
					action4(MapLoadStatus.Error, 0, "MAP DESCRIPTOR GAME OBJECT ON SCENE \"" + sceneName + "\" HAS UNAPPROVED COMPONENTS ON IT");
				}
			}
			OnLoadComplete(false, false, "");
			yield break;
		}
		if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion < 4)
		{
			foreach (TextMeshPro textMeshPro in gameObject.transform.GetComponentsInChildren<TextMeshPro>(true))
			{
				if (textMeshPro.font == null || textMeshPro.font.material == null)
				{
					textMeshPro.font = CustomMapLoader.instance.DefaultFont;
				}
			}
			foreach (TextMeshProUGUI textMeshProUGUI in gameObject.transform.GetComponentsInChildren<TextMeshProUGUI>(true))
			{
				if (textMeshProUGUI.font == null || textMeshProUGUI.font.material == null)
				{
					textMeshProUGUI.font = CustomMapLoader.instance.DefaultFont;
				}
			}
		}
		CustomMapLoader.totalObjectsInLoadingScene = 0;
		for (int l = 0; l < rootGameObjects.Length; l++)
		{
			CustomMapLoader.SanitizeObjectRecursive(rootGameObjects[l], gameObject);
		}
		CustomMapLoader.ResolveVirtualStumpColliderOverlaps(sceneName);
		if (useProgressCallback)
		{
			currentProgress += Mathf.RoundToInt((float)progressAmount * 0.2f);
			Action<MapLoadStatus, int, string> action5 = CustomMapLoader.mapLoadProgressCallback;
			if (action5 != null)
			{
				action5(MapLoadStatus.Loading, currentProgress, "MAP SCENE LOADED");
			}
		}
		CustomMapLoader.leafGliderIndex = 0;
		yield return CustomMapLoader.FinalizeSceneLoad(mapDescriptor, useProgressCallback, currentProgress, endingProgress);
		yield return null;
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			OnLoadComplete(false, true, "");
			if (CustomMapLoader.cachedExceptionMessage.Length > 0 && useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action6 = CustomMapLoader.mapLoadProgressCallback;
				if (action6 != null)
				{
					action6(MapLoadStatus.Error, 0, CustomMapLoader.cachedExceptionMessage);
				}
			}
			yield break;
		}
		if (CustomMapLoader.errorEncounteredDuringLoad)
		{
			OnLoadComplete(false, false, "");
			if (CustomMapLoader.cachedExceptionMessage.Length > 0 && useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action7 = CustomMapLoader.mapLoadProgressCallback;
				if (action7 != null)
				{
					action7(MapLoadStatus.Error, 0, CustomMapLoader.cachedExceptionMessage);
				}
			}
			yield break;
		}
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action8 = CustomMapLoader.mapLoadProgressCallback;
			if (action8 != null)
			{
				action8(MapLoadStatus.Loading, endingProgress, "FINALIZING MAP");
			}
		}
		CustomMapLoader.loadedSceneFilePaths.AddIfNew(CustomMapLoader.attemptedSceneToLoad);
		CustomMapLoader.loadedSceneNames.AddIfNew(sceneName);
		CustomMapLoader.loadedSceneIndexes.AddIfNew(sceneIndex);
		if (CustomMapLoader.refreshReviveStations)
		{
			CustomMapLoader.instance.ghostReactorManager.reactor.RefreshReviveStations(true);
		}
		OnLoadComplete(true, false, sceneName);
		yield break;
	}

	// Token: 0x060042D3 RID: 17107 RVA: 0x001653F0 File Offset: 0x001635F0
	private static void SanitizeObjectRecursive(GameObject rootObject, GameObject mapRoot)
	{
		if (!CustomMapLoader.SanitizeObject(rootObject, mapRoot))
		{
			return;
		}
		CustomMapLoader.totalObjectsInLoadingScene++;
		for (int i = 0; i < rootObject.transform.childCount; i++)
		{
			GameObject gameObject = rootObject.transform.GetChild(i).gameObject;
			if (gameObject.IsNotNull())
			{
				CustomMapLoader.SanitizeObjectRecursive(gameObject, mapRoot);
			}
		}
	}

	// Token: 0x060042D4 RID: 17108 RVA: 0x0016544C File Offset: 0x0016364C
	private static bool SanitizeObject(GameObject gameObject, GameObject mapRoot)
	{
		if (gameObject == null)
		{
			Debug.LogError("CustomMapLoader::SanitizeObject gameobject null");
			return false;
		}
		if (!CustomMapLoader.APPROVED_LAYERS.Contains(gameObject.layer))
		{
			gameObject.layer = 0;
		}
		foreach (Component component in gameObject.GetComponents<Component>())
		{
			if (component == null)
			{
				Object.DestroyImmediate(gameObject, true);
				return false;
			}
			bool flag = true;
			foreach (Type type in CustomMapLoader.componentAllowlist)
			{
				if (component.GetType() == type)
				{
					if (type == typeof(Camera))
					{
						Camera camera = (Camera)component;
						if (camera.IsNotNull() && camera.targetTexture.IsNull())
						{
							break;
						}
					}
					flag = false;
					break;
				}
			}
			if (flag)
			{
				foreach (string value in CustomMapLoader.componentTypeStringAllowList)
				{
					if (component.GetType().ToString().Contains(value))
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				Object.DestroyImmediate(gameObject, true);
				return false;
			}
		}
		if (gameObject.transform.parent.IsNull() && gameObject.transform != mapRoot.transform)
		{
			gameObject.transform.SetParent(mapRoot.transform);
		}
		return true;
	}

	// Token: 0x060042D5 RID: 17109 RVA: 0x001655DC File Offset: 0x001637DC
	private static void ResolveVirtualStumpColliderOverlaps(string sceneName)
	{
		Vector3 vector = new Vector3(5.15f, 0.72f, 5.15f);
		Vector3 b = new Vector3(0f, 0.73f, 0f);
		float radius = vector.x * 0.5f + 2f;
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		gameObject.transform.position = CustomMapLoader.instance.virtualStumpMesh.transform.position + b;
		gameObject.transform.localScale = vector;
		Collider[] array = Physics.OverlapSphere(gameObject.transform.position, radius);
		if (array == null || array.Length == 0)
		{
			Object.DestroyImmediate(gameObject);
			return;
		}
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
		meshCollider.convex = true;
		foreach (Collider collider in array)
		{
			Vector3 vector2;
			float num;
			if (!(collider == null) && !(collider.gameObject == gameObject) && !(collider.gameObject.scene.name != sceneName) && Physics.ComputePenetration(meshCollider, gameObject.transform.position, gameObject.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector2, out num) && !collider.isTrigger)
			{
				GTDev.Log<string>("[CustomMapLoader::ResolveVirtualStumpColliderOverlaps] Gameobject " + collider.name + " has a collider overlapping with the virtual stump. Collider will be removed", null);
				Object.DestroyImmediate(collider);
			}
		}
		Object.DestroyImmediate(gameObject);
	}

	// Token: 0x060042D6 RID: 17110 RVA: 0x00165764 File Offset: 0x00163964
	private static IEnumerator FinalizeSceneLoad(MapDescriptor sceneDescriptor, bool useProgressCallback = false, int startingProgress = 50, int endingProgress = 90)
	{
		int num = endingProgress - startingProgress;
		int num2 = startingProgress;
		if (useProgressCallback)
		{
			num2 += Mathf.RoundToInt((float)num * 0.02f);
			Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
			if (action != null)
			{
				action(MapLoadStatus.Loading, num2, "PROCESSING ROOT MAP OBJECT");
			}
		}
		CustomMapLoader.objectsProcessedForLoadingScene = 0;
		CustomMapLoader.objectsProcessedThisFrame = 0;
		if (useProgressCallback)
		{
			num2 += Mathf.RoundToInt((float)num * 0.03f);
			Action<MapLoadStatus, int, string> action2 = CustomMapLoader.mapLoadProgressCallback;
			if (action2 != null)
			{
				action2(MapLoadStatus.Loading, num2, "PROCESSING CHILD OBJECTS");
			}
		}
		int processChildrenEndingProgress = endingProgress - Mathf.RoundToInt((float)num * 0.02f);
		CustomMapLoader.initializePhaseTwoComponents.Clear();
		CustomMapLoader.entitiesToCreate.Clear();
		yield return CustomMapLoader.ProcessChildObjects(sceneDescriptor.gameObject, useProgressCallback, num2, processChildrenEndingProgress);
		if (CustomMapLoader.shouldAbortSceneLoad || CustomMapLoader.errorEncounteredDuringLoad)
		{
			yield break;
		}
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action3 = CustomMapLoader.mapLoadProgressCallback;
			if (action3 != null)
			{
				action3(MapLoadStatus.Loading, processChildrenEndingProgress, "PROCESSING COMPLETE");
			}
		}
		yield return null;
		CustomMapLoader.InitializeComponentsPhaseTwo();
		CustomMapLoader.placeholderReplacements.Clear();
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action4 = CustomMapLoader.mapLoadProgressCallback;
			if (action4 != null)
			{
				action4(MapLoadStatus.Loading, endingProgress, "PROCESSING COMPLETE");
			}
		}
		if (CustomMapLoader.loadedMapPackageInfo != null && CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion < 3 && sceneDescriptor.IsInitialScene)
		{
			CustomMapLoader.maxPlayersForMap = (byte)Math.Clamp(sceneDescriptor.MaxPlayers, 1, 20);
			CustomMapLoader.cachedLuauScript = ((sceneDescriptor.CustomGamemode != null) ? sceneDescriptor.CustomGamemode.text : "");
			CustomMapLoader.devModeEnabled = sceneDescriptor.DevMode;
			CustomMapLoader.disableHoldingHandsAllModes = sceneDescriptor.DisableHoldingHandsAllGameModes;
			CustomMapLoader.disableHoldingHandsCustomMode = sceneDescriptor.DisableHoldingHandsCustomOnly;
			if (sceneDescriptor.UseUberShaderDynamicLighting)
			{
				CustomMapLoader.SetZoneDynamicLighting(true);
				GameLightingManager.instance.SetAmbientLightDynamic(sceneDescriptor.UberShaderAmbientDynamicLight);
			}
			List<int> list = new List<int>();
			foreach (GameModeType item in CustomMapLoader.instance.availableModesForOldMaps)
			{
				list.Add((int)item);
			}
			GameModeType gameModeType = CustomMapLoader.instance.defaultGameModeForNonCustomOldMaps;
			if (!CustomMapLoader.cachedLuauScript.IsNullOrEmpty())
			{
				gameModeType = GameModeType.Custom;
				list.Add(7);
			}
			CustomMapModeSelector.SetAvailableGameModes(list.ToArray(), (int)gameModeType);
			if (RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
			{
				if (GameMode.ActiveGameMode.IsNull())
				{
					GameMode.ChangeGameMode(gameModeType.ToString());
				}
				else if (GameMode.ActiveGameMode.GameType() != gameModeType)
				{
					GameMode.ChangeGameMode(gameModeType.ToString());
				}
			}
			VirtualStumpReturnWatch.SetWatchProperties(sceneDescriptor.GetReturnToVStumpWatchProps());
		}
		yield break;
	}

	// Token: 0x060042D7 RID: 17111 RVA: 0x00165788 File Offset: 0x00163988
	private static IEnumerator ProcessChildObjects(GameObject parent, bool useProgressCallback = false, int startingProgress = 75, int endingProgress = 90)
	{
		if (parent == null || CustomMapLoader.placeholderReplacements.Contains(parent))
		{
			yield break;
		}
		int progressAmount = endingProgress - startingProgress;
		int num2;
		for (int i = 0; i < parent.transform.childCount; i = num2 + 1)
		{
			Transform child = parent.transform.GetChild(i);
			if (!(child == null))
			{
				GameObject gameObject = child.gameObject;
				if (!(gameObject == null) && !CustomMapLoader.placeholderReplacements.Contains(gameObject))
				{
					try
					{
						CustomMapLoader.InitializeComponentsPhaseOne(gameObject);
					}
					catch (Exception ex)
					{
						CustomMapLoader.errorEncounteredDuringLoad = true;
						CustomMapLoader.cachedExceptionMessage = ex.ToString();
						Debug.LogError("[CML.LoadMap] Exception: " + ex.ToString());
						yield break;
					}
					if (gameObject.transform.childCount > 0)
					{
						yield return CustomMapLoader.ProcessChildObjects(gameObject, useProgressCallback, startingProgress, endingProgress);
						if (CustomMapLoader.shouldAbortSceneLoad || CustomMapLoader.errorEncounteredDuringLoad)
						{
							yield break;
						}
					}
					if (CustomMapLoader.shouldAbortSceneLoad)
					{
						yield break;
					}
					CustomMapLoader.objectsProcessedForLoadingScene++;
					CustomMapLoader.objectsProcessedThisFrame++;
					if (CustomMapLoader.objectsProcessedThisFrame >= CustomMapLoader.numObjectsToProcessPerFrame)
					{
						CustomMapLoader.objectsProcessedThisFrame = 0;
						if (useProgressCallback)
						{
							float num = (float)CustomMapLoader.objectsProcessedForLoadingScene / (float)CustomMapLoader.totalObjectsInLoadingScene;
							int arg = startingProgress + Mathf.FloorToInt((float)progressAmount * num);
							Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
							if (action != null)
							{
								action(MapLoadStatus.Loading, arg, "PROCESSING CHILD OBJECTS");
							}
						}
						yield return null;
					}
				}
			}
			num2 = i;
		}
		yield break;
	}

	// Token: 0x060042D8 RID: 17112 RVA: 0x001657AC File Offset: 0x001639AC
	private static void InitializeComponentsPhaseOne(GameObject childGameObject)
	{
		CustomMapLoader.SetupCollisions(childGameObject);
		CustomMapLoader.ReplaceDataOnlyScripts(childGameObject);
		CustomMapLoader.ReplacePlaceholders(childGameObject);
		CustomMapLoader.SetupDynamicLight(childGameObject);
		CustomMapLoader.StoreMapEntity(childGameObject);
		CustomMapLoader.SetupReviveStation(childGameObject);
	}

	// Token: 0x060042D9 RID: 17113 RVA: 0x001657D4 File Offset: 0x001639D4
	private static void InitializeComponentsPhaseTwo()
	{
		for (int i = 0; i < CustomMapLoader.initializePhaseTwoComponents.Count; i++)
		{
		}
		CustomMapLoader.initializePhaseTwoComponents.Clear();
		if (CustomMapLoader.entitiesToCreate.Count > 0)
		{
			for (int j = 0; j < CustomMapLoader.entitiesToCreate.Count; j++)
			{
				CustomMapLoader.entitiesToCreate[j].gameObject.SetActive(false);
			}
			CustomMapsGameManager.AddAgentsToCreate(CustomMapLoader.entitiesToCreate);
		}
	}

	// Token: 0x060042DA RID: 17114 RVA: 0x00165844 File Offset: 0x00163A44
	private static void SetupReviveStation(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return;
		}
		CustomMapReviveStation component = gameObject.GetComponent<CustomMapReviveStation>();
		if (component == null)
		{
			return;
		}
		GameObject gameObject2 = Object.Instantiate<GameObject>(CustomMapLoader.instance.reviveStationPrefab, gameObject.transform.parent);
		if (gameObject2 == null)
		{
			return;
		}
		gameObject2.transform.position = gameObject.transform.position;
		gameObject2.transform.rotation = gameObject.transform.rotation;
		gameObject.transform.SetParent(gameObject2.transform);
		GRReviveStation component2 = gameObject2.GetComponent<GRReviveStation>();
		if (component2 == null)
		{
			return;
		}
		component2.audioSource = component.audioSource;
		if (!component.particleEffects.IsNullOrEmpty<ParticleSystem>())
		{
			component2.particleEffects = new ParticleSystem[component.particleEffects.Length];
			for (int i = 0; i < component.particleEffects.Length; i++)
			{
				component2.particleEffects[i] = component.particleEffects[i];
			}
		}
		component2.SetReviveCooldownSeconds(component.reviveCooldownSeconds);
		CustomMapLoader.refreshReviveStations = true;
	}

	// Token: 0x060042DB RID: 17115 RVA: 0x00165944 File Offset: 0x00163B44
	private static void SetupCollisions(GameObject gameObject)
	{
		if (gameObject == null || CustomMapLoader.placeholderReplacements.Contains(gameObject))
		{
			return;
		}
		Collider[] components = gameObject.GetComponents<Collider>();
		if (components == null)
		{
			return;
		}
		bool flag = true;
		foreach (Collider collider in components)
		{
			if (!(collider == null))
			{
				if (collider.isTrigger)
				{
					if (gameObject.layer != UnityLayer.GorillaInteractable.ToLayerIndex())
					{
						gameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
						break;
					}
				}
				else
				{
					if (gameObject.layer == UnityLayer.GorillaTrigger.ToLayerIndex())
					{
						collider.isTrigger = true;
					}
					flag = false;
					if (gameObject.GetComponent<GrabbableEntity>().IsNotNull())
					{
						gameObject.layer = UnityLayer.Default.ToLayerIndex();
						return;
					}
				}
			}
		}
		if (!flag)
		{
			SurfaceOverrideSettings component = gameObject.GetComponent<SurfaceOverrideSettings>();
			GorillaSurfaceOverride gorillaSurfaceOverride = gameObject.AddComponent<GorillaSurfaceOverride>();
			if (component == null)
			{
				gorillaSurfaceOverride.overrideIndex = 0;
				return;
			}
			gorillaSurfaceOverride.overrideIndex = (int)component.soundOverride;
			gorillaSurfaceOverride.extraVelMultiplier = component.extraVelMultiplier;
			gorillaSurfaceOverride.extraVelMaxMultiplier = component.extraVelMaxMultiplier;
			gorillaSurfaceOverride.slidePercentageOverride = component.slidePercentage;
			gorillaSurfaceOverride.disablePushBackEffect = component.disablePushBackEffect;
			Object.Destroy(component);
		}
	}

	// Token: 0x060042DC RID: 17116 RVA: 0x00165A64 File Offset: 0x00163C64
	private static bool ValidateTeleporterDestination(Transform teleportTarget)
	{
		using (List<GameObject>.Enumerator enumerator = CustomMapLoader.storeCheckouts.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Vector3.Distance(enumerator.Current.transform.position, teleportTarget.position) < Constants.minTeleportDistFromStorePlaceholder)
				{
					return false;
				}
			}
		}
		using (List<GameObject>.Enumerator enumerator = CustomMapLoader.storeDisplayStands.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Vector3.Distance(enumerator.Current.transform.position, teleportTarget.position) < Constants.minTeleportDistFromStorePlaceholder)
				{
					return false;
				}
			}
		}
		return !CustomMapLoader.customMapATM.IsNotNull() || Vector3.Distance(CustomMapLoader.customMapATM.transform.position, teleportTarget.position) >= Constants.minTeleportDistFromStorePlaceholder;
	}

	// Token: 0x060042DD RID: 17117 RVA: 0x00165B5C File Offset: 0x00163D5C
	private static bool ValidateStorePlaceholderPosition(GameObject storePlaceholder)
	{
		foreach (Component component in CustomMapLoader.teleporters)
		{
			if (!(component == null))
			{
				List<Transform> list = null;
				if (component.GetType() == typeof(CMSMapBoundary))
				{
					CMSMapBoundary cmsmapBoundary = (CMSMapBoundary)component;
					if (cmsmapBoundary != null)
					{
						list = cmsmapBoundary.TeleportPoints;
					}
				}
				else if (component.GetType() == typeof(CMSTeleporter))
				{
					CMSTeleporter cmsteleporter = (CMSTeleporter)component;
					if (cmsteleporter != null)
					{
						list = cmsteleporter.TeleportPoints;
					}
				}
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						Transform transform = list[i];
						if (Vector3.Distance(storePlaceholder.transform.position, transform.position) < Constants.minTeleportDistFromStorePlaceholder)
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x060042DE RID: 17118 RVA: 0x00165C68 File Offset: 0x00163E68
	private static void ReplaceDataOnlyScripts(GameObject gameObject)
	{
		MapBoundarySettings[] components = gameObject.GetComponents<MapBoundarySettings>();
		if (components != null)
		{
			foreach (MapBoundarySettings mapBoundarySettings in components)
			{
				bool flag = false;
				for (int j = 0; j < mapBoundarySettings.TeleportPoints.Count; j++)
				{
					if (!mapBoundarySettings.TeleportPoints[j].IsNull() && !CustomMapLoader.ValidateTeleporterDestination(mapBoundarySettings.TeleportPoints[j]))
					{
						flag = true;
						Object.Destroy(mapBoundarySettings);
						break;
					}
				}
				if (!flag)
				{
					CMSMapBoundary cmsmapBoundary = gameObject.AddComponent<CMSMapBoundary>();
					if (cmsmapBoundary != null)
					{
						cmsmapBoundary.CopyTriggerSettings(mapBoundarySettings);
						CustomMapLoader.teleporters.Add(cmsmapBoundary);
					}
					Object.Destroy(mapBoundarySettings);
				}
			}
		}
		TagZoneSettings[] components2 = gameObject.GetComponents<TagZoneSettings>();
		if (components2 != null)
		{
			foreach (TagZoneSettings tagZoneSettings in components2)
			{
				CMSTagZone cmstagZone = gameObject.AddComponent<CMSTagZone>();
				if (cmstagZone != null)
				{
					cmstagZone.CopyTriggerSettings(tagZoneSettings);
				}
				Object.Destroy(tagZoneSettings);
			}
		}
		TeleporterSettings[] components3 = gameObject.GetComponents<TeleporterSettings>();
		if (components3 != null)
		{
			foreach (TeleporterSettings teleporterSettings in components3)
			{
				bool flag2 = false;
				for (int k = 0; k < teleporterSettings.TeleportPoints.Count; k++)
				{
					if (!teleporterSettings.TeleportPoints[k].IsNull() && !CustomMapLoader.ValidateTeleporterDestination(teleporterSettings.TeleportPoints[k]))
					{
						flag2 = true;
						Object.Destroy(teleporterSettings);
						break;
					}
				}
				if (!flag2)
				{
					CMSTeleporter cmsteleporter = gameObject.AddComponent<CMSTeleporter>();
					if (cmsteleporter != null)
					{
						cmsteleporter.CopyTriggerSettings(teleporterSettings);
					}
					Object.Destroy(teleporterSettings);
				}
			}
		}
		ObjectActivationTriggerSettings[] components4 = gameObject.GetComponents<ObjectActivationTriggerSettings>();
		if (components4 != null)
		{
			foreach (ObjectActivationTriggerSettings objectActivationTriggerSettings in components4)
			{
				CMSObjectActivationTrigger cmsobjectActivationTrigger = gameObject.AddComponent<CMSObjectActivationTrigger>();
				if (cmsobjectActivationTrigger != null)
				{
					cmsobjectActivationTrigger.CopyTriggerSettings(objectActivationTriggerSettings);
				}
				Object.Destroy(objectActivationTriggerSettings);
			}
		}
		LuauTriggerSettings[] components5 = gameObject.GetComponents<LuauTriggerSettings>();
		if (components5 != null)
		{
			foreach (LuauTriggerSettings luauTriggerSettings in components5)
			{
				CMSLuau cmsluau = gameObject.AddComponent<CMSLuau>();
				if (cmsluau != null)
				{
					cmsluau.CopyTriggerSettings(luauTriggerSettings);
				}
				Object.Destroy(luauTriggerSettings);
			}
		}
		PlayAnimationTriggerSettings[] components6 = gameObject.GetComponents<PlayAnimationTriggerSettings>();
		if (components6 != null)
		{
			foreach (PlayAnimationTriggerSettings playAnimationTriggerSettings in components6)
			{
				CMSPlayAnimationTrigger cmsplayAnimationTrigger = gameObject.AddComponent<CMSPlayAnimationTrigger>();
				if (cmsplayAnimationTrigger != null)
				{
					cmsplayAnimationTrigger.CopyTriggerSettings(playAnimationTriggerSettings);
				}
				Object.Destroy(playAnimationTriggerSettings);
			}
		}
		LoadZoneSettings[] components7 = gameObject.GetComponents<LoadZoneSettings>();
		if (components7 != null)
		{
			foreach (LoadZoneSettings loadZoneSettings in components7)
			{
				CMSLoadingZone cmsloadingZone = gameObject.AddComponent<CMSLoadingZone>();
				if (cmsloadingZone != null)
				{
					cmsloadingZone.SetupLoadingZone(loadZoneSettings, CustomMapLoader.assetBundleSceneFilePaths);
				}
				Object.Destroy(loadZoneSettings);
			}
		}
		ZoneShaderTriggerSettings[] components8 = gameObject.GetComponents<ZoneShaderTriggerSettings>();
		if (components8 != null)
		{
			foreach (ZoneShaderTriggerSettings zoneShaderTriggerSettings in components8)
			{
				gameObject.AddComponent<CMSZoneShaderSettingsTrigger>().CopySettings(zoneShaderTriggerSettings);
				Object.Destroy(zoneShaderTriggerSettings);
			}
		}
		CMSZoneShaderSettings component = gameObject.GetComponent<CMSZoneShaderSettings>();
		if (component.IsNotNull())
		{
			ZoneShaderSettings zoneShaderSettings = gameObject.AddComponent<ZoneShaderSettings>();
			zoneShaderSettings.CopySettings(component, false);
			if (component.isDefaultValues)
			{
				CustomMapManager.SetDefaultZoneShaderSettings(zoneShaderSettings, component.GetProperties());
			}
			CustomMapManager.AddZoneShaderSettings(zoneShaderSettings);
			Object.Destroy(component);
		}
		HandHoldSettings component2 = gameObject.GetComponent<HandHoldSettings>();
		if (component2.IsNotNull())
		{
			gameObject.AddComponent<HandHold>().CopyProperties(component2);
			Object.Destroy(component2);
		}
		CustomMapEjectButtonSettings component3 = gameObject.GetComponent<CustomMapEjectButtonSettings>();
		if (component3.IsNotNull())
		{
			CustomMapEjectButton customMapEjectButton = gameObject.AddComponent<CustomMapEjectButton>();
			customMapEjectButton.gameObject.layer = UnityLayer.GorillaInteractable.ToLayerIndex();
			customMapEjectButton.CopySettings(component3);
			Object.Destroy(component3);
		}
		MovingSurfaceSettings component4 = gameObject.GetComponent<MovingSurfaceSettings>();
		if (component4.IsNotNull())
		{
			MovingSurface movingSurface = gameObject.AddComponent<MovingSurface>();
			if (movingSurface.IsNotNull())
			{
				movingSurface.CopySettings(component4);
				Object.Destroy(component4);
			}
		}
		SurfaceMoverSettings component5 = gameObject.GetComponent<SurfaceMoverSettings>();
		if (component5.IsNotNull())
		{
			gameObject.AddComponent<SurfaceMover>().CopySettings(component5);
			Object.Destroy(component5);
		}
	}

	// Token: 0x060042DF RID: 17119 RVA: 0x0016608C File Offset: 0x0016428C
	private static void ReplacePlaceholders(GameObject placeholderGameObject)
	{
		if (placeholderGameObject.IsNull())
		{
			return;
		}
		GTObjectPlaceholder component = placeholderGameObject.GetComponent<GTObjectPlaceholder>();
		if (component.IsNull())
		{
			return;
		}
		switch (component.PlaceholderObject)
		{
		case GTObject.LeafGlider:
			if (CustomMapLoader.leafGliderIndex < CustomMapLoader.instance.leafGliders.Length)
			{
				CustomMapLoader.instance.leafGliders[CustomMapLoader.leafGliderIndex].enabled = true;
				CustomMapLoader.instance.leafGliders[CustomMapLoader.leafGliderIndex].CustomMapLoad(component.transform, component.maxDistanceBeforeRespawn);
				CustomMapLoader.instance.leafGliders[CustomMapLoader.leafGliderIndex].transform.GetChild(0).gameObject.SetActive(true);
				CustomMapLoader.leafGliderIndex++;
				return;
			}
			break;
		case GTObject.GliderWindVolume:
		{
			List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
			if (component.useDefaultPlaceholder || list.Count == 0)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(CustomMapLoader.instance.gliderWindVolume, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
				if (gameObject != null)
				{
					CustomMapLoader.placeholderReplacements.Add(gameObject);
					gameObject.transform.localScale = placeholderGameObject.transform.localScale;
					placeholderGameObject.transform.localScale = Vector3.one;
					gameObject.transform.SetParent(placeholderGameObject.transform);
					GliderWindVolume component2 = gameObject.GetComponent<GliderWindVolume>();
					if (component2 == null)
					{
						return;
					}
					component2.SetProperties(component.maxSpeed, component.maxAccel, component.SpeedVSAccelCurve, component.localWindDirection);
					return;
				}
			}
			else
			{
				placeholderGameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
				GliderWindVolume gliderWindVolume = placeholderGameObject.AddComponent<GliderWindVolume>();
				if (gliderWindVolume.IsNotNull())
				{
					gliderWindVolume.SetProperties(component.maxSpeed, component.maxAccel, component.SpeedVSAccelCurve, component.localWindDirection);
					return;
				}
			}
			break;
		}
		case GTObject.WaterVolume:
		{
			List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
			if (component.useDefaultPlaceholder || list.Count == 0)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(CustomMapLoader.instance.waterVolumePrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
				if (gameObject2 != null)
				{
					CustomMapLoader.placeholderReplacements.Add(gameObject2);
					gameObject2.layer = UnityLayer.Water.ToLayerIndex();
					gameObject2.transform.localScale = placeholderGameObject.transform.localScale;
					placeholderGameObject.transform.localScale = Vector3.one;
					gameObject2.transform.SetParent(placeholderGameObject.transform);
					MeshRenderer component3 = gameObject2.GetComponent<MeshRenderer>();
					if (component3.IsNull())
					{
						return;
					}
					if (!component.useWaterMesh)
					{
						component3.enabled = false;
						return;
					}
					component3.enabled = true;
					WaterSurfaceMaterialController component4 = gameObject2.GetComponent<WaterSurfaceMaterialController>();
					if (component4.IsNull())
					{
						return;
					}
					component4.ScrollX = component.scrollTextureX;
					component4.ScrollY = component.scrollTextureY;
					component4.Scale = component.scaleTexture;
					return;
				}
			}
			else
			{
				placeholderGameObject.layer = UnityLayer.Water.ToLayerIndex();
				WaterVolume waterVolume = placeholderGameObject.AddComponent<WaterVolume>();
				if (waterVolume.IsNotNull())
				{
					WaterParameters parameters = null;
					CMSZoneShaderSettings.EZoneLiquidType liquidType = component.liquidType;
					if (liquidType != CMSZoneShaderSettings.EZoneLiquidType.Water)
					{
						if (liquidType == CMSZoneShaderSettings.EZoneLiquidType.Lava)
						{
							parameters = CustomMapLoader.instance.defaultLavaParameters;
						}
					}
					else
					{
						parameters = CustomMapLoader.instance.defaultWaterParameters;
					}
					waterVolume.SetPropertiesFromPlaceholder(component.GetWaterVolumeProperties(), list, parameters);
					waterVolume.RefreshColliders();
					return;
				}
			}
			break;
		}
		case GTObject.ForceVolume:
		{
			List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
			if (component.useDefaultPlaceholder || list.Count == 0)
			{
				GameObject gameObject3 = Object.Instantiate<GameObject>(CustomMapLoader.instance.forceVolumePrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
				if (gameObject3.IsNotNull())
				{
					CustomMapLoader.placeholderReplacements.Add(gameObject3);
					gameObject3.transform.localScale = placeholderGameObject.transform.localScale;
					placeholderGameObject.transform.localScale = Vector3.one;
					gameObject3.transform.SetParent(placeholderGameObject.transform);
					ForceVolume component5 = gameObject3.GetComponent<ForceVolume>();
					if (component5.IsNull())
					{
						return;
					}
					component5.SetPropertiesFromPlaceholder(component.GetForceVolumeProperties(), null, null);
					return;
				}
			}
			else
			{
				ForceVolume forceVolume = placeholderGameObject.AddComponent<ForceVolume>();
				if (forceVolume.IsNotNull())
				{
					AudioSource audioSource = placeholderGameObject.GetComponent<AudioSource>();
					if (audioSource.IsNull())
					{
						audioSource = placeholderGameObject.AddComponent<AudioSource>();
						audioSource.spatialize = true;
						audioSource.playOnAwake = false;
						audioSource.priority = 128;
						audioSource.volume = 0.522f;
						audioSource.pitch = 1f;
						audioSource.panStereo = 0f;
						audioSource.spatialBlend = 1f;
						audioSource.reverbZoneMix = 1f;
						audioSource.dopplerLevel = 1f;
						audioSource.spread = 0f;
						audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
						audioSource.minDistance = 8.2f;
						audioSource.maxDistance = 43.94f;
						audioSource.enabled = true;
					}
					audioSource.outputAudioMixerGroup = CustomMapLoader.instance.masterAudioMixer;
					for (int i = list.Count - 1; i >= 0; i--)
					{
						if (i == 0)
						{
							list[i].isTrigger = true;
						}
						else
						{
							Object.Destroy(list[i]);
						}
					}
					placeholderGameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
					forceVolume.SetPropertiesFromPlaceholder(component.GetForceVolumeProperties(), audioSource, component.GetComponent<Collider>());
					return;
				}
				Debug.LogError("[CustomMapLoader::ReplacePlaceholders] Failed to add ForceVolume component to Placeholder!");
				return;
			}
			break;
		}
		case GTObject.ATM:
		{
			if (CustomMapLoader.customMapATM.IsNotNull())
			{
				Object.Destroy(component);
				return;
			}
			if (!CustomMapLoader.ValidateStorePlaceholderPosition(placeholderGameObject))
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject4 = CustomMapLoader.instance.atmPrefab;
			if (component.useCustomMesh)
			{
				gameObject4 = CustomMapLoader.instance.atmNoShellPrefab;
			}
			if (gameObject4.IsNull())
			{
				return;
			}
			GameObject gameObject5 = Object.Instantiate<GameObject>(gameObject4, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject5.IsNotNull())
			{
				gameObject5.transform.SetParent(CustomMapLoader.instance.compositeTryOnArea.transform, true);
				gameObject5.transform.localScale = Vector3.one;
				ATM_UI componentInChildren = gameObject5.GetComponentInChildren<ATM_UI>();
				if (componentInChildren.IsNotNull() && ATM_Manager.instance.IsNotNull())
				{
					componentInChildren.SetCustomMapScene(placeholderGameObject.scene);
					CustomMapLoader.customMapATM = gameObject5;
					ATM_Manager.instance.AddATM(componentInChildren, null);
					if (!component.defaultCreatorCode.IsNullOrEmpty())
					{
						ATM_Manager.instance.SetTemporaryCreatorCode(component.defaultCreatorCode);
						return;
					}
				}
			}
			break;
		}
		case GTObject.HoverboardArea:
			if (component.AddComponent<HoverboardAreaTrigger>().IsNotNull())
			{
				component.gameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
				List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
				if (list.Count != 0)
				{
					for (int j = list.Count - 1; j >= 0; j--)
					{
						if (j == 0)
						{
							list[j].isTrigger = true;
						}
						else
						{
							Object.Destroy(list[j]);
						}
					}
					return;
				}
				BoxCollider boxCollider = component.AddComponent<BoxCollider>();
				if (boxCollider.IsNotNull())
				{
					boxCollider.isTrigger = true;
					return;
				}
			}
			break;
		case GTObject.HoverboardDispenser:
		{
			if (CustomMapLoader.instance.hoverboardDispenserPrefab.IsNull())
			{
				Debug.LogError("[CustomMapLoader::ReplacePlaceholders] hoverboardDispenserPrefab is NULL!");
				return;
			}
			GameObject gameObject6 = Object.Instantiate<GameObject>(CustomMapLoader.instance.hoverboardDispenserPrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject6.IsNotNull())
			{
				CustomMapLoader.placeholderReplacements.Add(gameObject6);
				gameObject6.transform.SetParent(placeholderGameObject.transform);
				return;
			}
			break;
		}
		case GTObject.RopeSwing:
		{
			GameObject gameObject7 = Object.Instantiate<GameObject>(CustomMapLoader.instance.ropeSwingPrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject7.IsNull())
			{
				return;
			}
			gameObject7.transform.SetParent(placeholderGameObject.transform);
			CustomMapsGorillaRopeSwing component6 = gameObject7.GetComponent<CustomMapsGorillaRopeSwing>();
			if (component6.IsNull())
			{
				Object.DestroyImmediate(gameObject7);
				return;
			}
			component.ropeLength = Math.Clamp(component.ropeLength, 3, 31);
			if (component.useDefaultPlaceholder)
			{
				component6.SetRopeLength(component.ropeLength);
			}
			else
			{
				component6.SetRopeProperties(component);
			}
			CustomMapLoader.placeholderReplacements.Add(gameObject7);
			return;
		}
		case GTObject.ZipLine:
		{
			GameObject gameObject8 = Object.Instantiate<GameObject>(CustomMapLoader.instance.ziplinePrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject8.IsNull())
			{
				return;
			}
			gameObject8.transform.SetParent(placeholderGameObject.transform);
			CustomMapsGorillaZipline component7 = gameObject8.GetComponent<CustomMapsGorillaZipline>();
			if (component7.IsNull())
			{
				Object.DestroyImmediate(gameObject8);
				return;
			}
			if (component.useDefaultPlaceholder)
			{
				if (!component7.GenerateZipline(component.spline))
				{
					Object.DestroyImmediate(gameObject8);
					return;
				}
			}
			else
			{
				component7.Init(component);
			}
			CustomMapLoader.placeholderReplacements.Add(gameObject8);
			return;
		}
		case GTObject.Store_DisplayStand:
		{
			if (CustomMapLoader.instance.storeDisplayStandPrefab.IsNull())
			{
				return;
			}
			if (CustomMapLoader.storeDisplayStands.Count >= Constants.storeDisplayStandLimit)
			{
				Object.Destroy(component);
				return;
			}
			if (placeholderGameObject.transform.lossyScale != Vector3.one)
			{
				Object.Destroy(component);
				return;
			}
			if (!CustomMapLoader.ValidateStorePlaceholderPosition(placeholderGameObject))
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject9 = Object.Instantiate<GameObject>(CustomMapLoader.instance.storeDisplayStandPrefab, placeholderGameObject.transform);
			if (gameObject9.IsNull())
			{
				return;
			}
			gameObject9.transform.SetParent(CustomMapLoader.instance.compositeTryOnArea.transform, true);
			gameObject9.transform.localScale = Vector3.one;
			DynamicCosmeticStand component8 = gameObject9.GetComponent<DynamicCosmeticStand>();
			if (component8.IsNull())
			{
				Object.DestroyImmediate(gameObject9);
				return;
			}
			component8.InitializeForCustomMapCosmeticItem(component.CosmeticItem, placeholderGameObject.scene);
			CustomMapLoader.storeDisplayStands.Add(gameObject9);
			CustomMapLoader.placeholderReplacements.Add(gameObject9);
			return;
		}
		case GTObject.Store_TryOnArea:
		{
			if (CustomMapLoader.instance.storeTryOnAreaPrefab.IsNull() || CustomMapLoader.instance.compositeTryOnArea.IsNull())
			{
				return;
			}
			if (CustomMapLoader.storeTryOnAreas.Count >= Constants.storeTryOnAreaLimit)
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject10 = Object.Instantiate<GameObject>(CustomMapLoader.instance.storeTryOnAreaPrefab, placeholderGameObject.transform);
			gameObject10.transform.SetParent(CustomMapLoader.instance.compositeTryOnArea.transform);
			CMSTryOnArea component9 = gameObject10.GetComponent<CMSTryOnArea>();
			if (component9.IsNull() || component9.tryOnAreaCollider.IsNull())
			{
				Object.DestroyImmediate(gameObject10);
				return;
			}
			BoxCollider tryOnAreaCollider = component9.tryOnAreaCollider;
			Vector3 zero = Vector3.zero;
			zero.x = tryOnAreaCollider.size.x * tryOnAreaCollider.transform.lossyScale.x;
			zero.y = tryOnAreaCollider.size.y * tryOnAreaCollider.transform.lossyScale.y;
			zero.z = tryOnAreaCollider.size.z * tryOnAreaCollider.transform.lossyScale.z;
			if (Math.Abs(zero.x * zero.y * zero.z) > Constants.storeTryOnAreaVolumeLimit)
			{
				Object.DestroyImmediate(gameObject10);
				return;
			}
			component9.InitializeForCustomMap(CustomMapLoader.instance.compositeTryOnArea, placeholderGameObject.scene);
			CustomMapLoader.storeTryOnAreas.Add(gameObject10);
			CustomMapLoader.placeholderReplacements.Add(gameObject10);
			break;
		}
		case GTObject.Store_Checkout:
		{
			if (CustomMapLoader.instance.storeCheckoutCounterPrefab.IsNull())
			{
				return;
			}
			if (CustomMapLoader.storeCheckouts.Count >= Constants.storeCheckoutCounterLimit)
			{
				Object.Destroy(component);
				return;
			}
			if (placeholderGameObject.transform.lossyScale != Vector3.one)
			{
				Object.Destroy(component);
				return;
			}
			if (!CustomMapLoader.ValidateStorePlaceholderPosition(placeholderGameObject))
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject11 = Object.Instantiate<GameObject>(CustomMapLoader.instance.storeCheckoutCounterPrefab, placeholderGameObject.transform);
			if (gameObject11.IsNull())
			{
				return;
			}
			gameObject11.transform.SetParent(CustomMapLoader.instance.compositeTryOnArea.transform);
			gameObject11.transform.localScale = Vector3.one;
			ItemCheckout componentInChildren2 = gameObject11.GetComponentInChildren<ItemCheckout>();
			if (componentInChildren2.IsNull())
			{
				Object.DestroyImmediate(gameObject11);
				return;
			}
			componentInChildren2.InitializeForCustomMap(CustomMapLoader.instance.compositeTryOnArea, placeholderGameObject.scene, component.useCustomMesh);
			CustomMapLoader.storeCheckouts.Add(gameObject11);
			CustomMapLoader.placeholderReplacements.Add(gameObject11);
			return;
		}
		case GTObject.Store_TryOnConsole:
		{
			if (CustomMapLoader.instance.storeTryOnConsolePrefab.IsNull())
			{
				return;
			}
			if (CustomMapLoader.storeTryOnConsoles.Count >= Constants.storeTryOnConsoleLimit)
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject12 = Object.Instantiate<GameObject>(CustomMapLoader.instance.storeTryOnConsolePrefab, placeholderGameObject.transform);
			if (gameObject12.IsNull())
			{
				return;
			}
			FittingRoom componentInChildren3 = gameObject12.GetComponentInChildren<FittingRoom>();
			if (componentInChildren3.IsNull())
			{
				Object.DestroyImmediate(gameObject12);
				return;
			}
			componentInChildren3.InitializeForCustomMap(component.useCustomMesh);
			CustomMapLoader.storeTryOnConsoles.Add(gameObject12);
			CustomMapLoader.placeholderReplacements.Add(gameObject12);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x060042E0 RID: 17120 RVA: 0x00166D38 File Offset: 0x00164F38
	private static void SetupDynamicLight(GameObject dynamicLightGameObject)
	{
		if (dynamicLightGameObject.IsNull())
		{
			return;
		}
		UberShaderDynamicLight component = dynamicLightGameObject.GetComponent<UberShaderDynamicLight>();
		if (component.IsNull())
		{
			return;
		}
		if (component.dynamicLight.IsNull())
		{
			return;
		}
		GameObject gameObject = new GameObject(dynamicLightGameObject.name + "GameLight");
		GameLight gameLight = gameObject.AddComponent<GameLight>();
		gameLight.light = component.dynamicLight;
		GameLightingManager.instance.AddGameLight(gameLight, false);
		gameObject.transform.SetParent(dynamicLightGameObject.transform.parent);
		gameObject.transform.position = component.transform.position;
	}

	// Token: 0x060042E1 RID: 17121 RVA: 0x00166DD0 File Offset: 0x00164FD0
	private static void StoreMapEntity(GameObject entityGameObject)
	{
		if (entityGameObject.IsNull() || CustomMapsGameManager.instance.IsNull())
		{
			return;
		}
		MapEntity component = entityGameObject.GetComponent<MapEntity>();
		if (component.IsNull())
		{
			return;
		}
		if (component is AIAgent)
		{
			AIAgent aiagent = (AIAgent)component;
			if (!aiagent.IsNull())
			{
				string.Format(" | AgentID: {0}", aiagent.enemyTypeId);
			}
		}
		if (component.isTemplate)
		{
			return;
		}
		CustomMapLoader.entitiesToCreate.Add(component);
	}

	// Token: 0x060042E2 RID: 17122 RVA: 0x00166E44 File Offset: 0x00165044
	private static void CacheLightmaps()
	{
		CustomMapLoader.lightmaps = new LightmapData[LightmapSettings.lightmaps.Length];
		if (CustomMapLoader.lightmapsToKeep.Count > 0)
		{
			CustomMapLoader.lightmapsToKeep.Clear();
		}
		CustomMapLoader.lightmapsToKeep = new List<Texture2D>(LightmapSettings.lightmaps.Length * 2);
		for (int i = 0; i < LightmapSettings.lightmaps.Length; i++)
		{
			CustomMapLoader.lightmaps[i] = LightmapSettings.lightmaps[i];
			if (LightmapSettings.lightmaps[i].lightmapColor != null)
			{
				CustomMapLoader.lightmapsToKeep.Add(LightmapSettings.lightmaps[i].lightmapColor);
			}
			if (LightmapSettings.lightmaps[i].lightmapDir != null)
			{
				CustomMapLoader.lightmapsToKeep.Add(LightmapSettings.lightmaps[i].lightmapDir);
			}
		}
	}

	// Token: 0x060042E3 RID: 17123 RVA: 0x00166F00 File Offset: 0x00165100
	private static void LoadLightmaps(Texture2D[] colorMaps, Texture2D[] dirMaps)
	{
		if (colorMaps.Length == 0)
		{
			return;
		}
		CustomMapLoader.UnloadLightmaps();
		List<LightmapData> list = new List<LightmapData>(LightmapSettings.lightmaps);
		for (int i = 0; i < colorMaps.Length; i++)
		{
			bool flag = false;
			LightmapData lightmapData = new LightmapData();
			if (colorMaps[i] != null)
			{
				lightmapData.lightmapColor = colorMaps[i];
				flag = true;
				if (i < dirMaps.Length && dirMaps[i] != null)
				{
					lightmapData.lightmapDir = dirMaps[i];
				}
			}
			if (flag)
			{
				list.Add(lightmapData);
			}
		}
		LightmapSettings.lightmaps = list.ToArray();
	}

	// Token: 0x060042E4 RID: 17124 RVA: 0x00166F80 File Offset: 0x00165180
	public static void ResetToInitialZone(Action<string> onSceneLoaded, Action<string> onSceneUnloaded)
	{
		List<int> list = new List<int>(CustomMapLoader.initialSceneIndexes);
		List<int> list2 = new List<int>(CustomMapLoader.loadedSceneIndexes);
		foreach (int item in CustomMapLoader.loadedSceneIndexes)
		{
			if (CustomMapLoader.initialSceneIndexes.Contains(item))
			{
				list2.Remove(item);
				list.Remove(item);
			}
		}
		if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion <= 2 && CustomMapLoader.loadedSceneIndexes.Contains(CustomMapLoader.initialSceneIndexes[0]))
		{
			MapDescriptor[] array = Object.FindObjectsByType<MapDescriptor>(FindObjectsSortMode.None);
			bool flag = false;
			int i;
			for (i = 0; i < array.Length; i++)
			{
				if (array[i].IsInitialScene && array[i].UseUberShaderDynamicLighting)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				CustomMapLoader.SetZoneDynamicLighting(true);
				GameLightingManager.instance.SetAmbientLightDynamic(array[i].UberShaderAmbientDynamicLight);
			}
			else
			{
				CustomMapLoader.SetZoneDynamicLighting(false);
				GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
			}
		}
		else if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion > 2)
		{
			if (CustomMapLoader.loadedMapPackageInfo.useUberShaderDynamicLighting)
			{
				Color ambientLightDynamic = new Color(CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_R, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_G, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_B, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_A);
				CustomMapLoader.SetZoneDynamicLighting(true);
				GameLightingManager.instance.SetAmbientLightDynamic(ambientLightDynamic);
			}
			else
			{
				CustomMapLoader.SetZoneDynamicLighting(false);
				GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
			}
		}
		if (list.IsNullOrEmpty<int>() && list2.IsNullOrEmpty<int>())
		{
			return;
		}
		if (CustomMapLoader.zoneLoadingCoroutine != null)
		{
			CustomMapLoader.LoadZoneRequest item2 = new CustomMapLoader.LoadZoneRequest
			{
				sceneIndexesToLoad = list.ToArray(),
				sceneIndexesToUnload = list2.ToArray(),
				onSceneLoadedCallback = onSceneLoaded,
				onSceneUnloadedCallback = onSceneUnloaded
			};
			CustomMapLoader.queuedLoadZoneRequests.Add(item2);
			return;
		}
		CustomMapLoader.sceneLoadedCallback = onSceneLoaded;
		CustomMapLoader.sceneUnloadedCallback = onSceneUnloaded;
		CustomMapLoader.zoneLoadingCoroutine = CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadZoneCoroutine(list.ToArray(), list2.ToArray()));
	}

	// Token: 0x060042E5 RID: 17125 RVA: 0x001671A0 File Offset: 0x001653A0
	public static void LoadZoneTriggered(int[] loadSceneIndexes, int[] unloadSceneIndexes, Action<string> onSceneLoaded, Action<string> onSceneUnloaded)
	{
		string str = "";
		for (int i = 0; i < loadSceneIndexes.Length; i++)
		{
			str += loadSceneIndexes[i].ToString();
			if (i != loadSceneIndexes.Length - 1)
			{
				str += ", ";
			}
		}
		string str2 = "";
		for (int j = 0; j < unloadSceneIndexes.Length; j++)
		{
			str2 += unloadSceneIndexes[j].ToString();
			if (j != unloadSceneIndexes.Length - 1)
			{
				str2 += ", ";
			}
		}
		if (CustomMapLoader.zoneLoadingCoroutine != null)
		{
			CustomMapLoader.LoadZoneRequest item = new CustomMapLoader.LoadZoneRequest
			{
				sceneIndexesToLoad = loadSceneIndexes,
				sceneIndexesToUnload = unloadSceneIndexes,
				onSceneLoadedCallback = onSceneLoaded,
				onSceneUnloadedCallback = onSceneUnloaded
			};
			CustomMapLoader.queuedLoadZoneRequests.Add(item);
			return;
		}
		CustomMapLoader.sceneLoadedCallback = onSceneLoaded;
		CustomMapLoader.sceneUnloadedCallback = onSceneUnloaded;
		CustomMapLoader.zoneLoadingCoroutine = CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadZoneCoroutine(loadSceneIndexes, unloadSceneIndexes));
	}

	// Token: 0x060042E6 RID: 17126 RVA: 0x00167287 File Offset: 0x00165487
	private static IEnumerator LoadZoneCoroutine(int[] loadScenes, int[] unloadScenes)
	{
		if (!unloadScenes.IsNullOrEmpty<int>())
		{
			yield return CustomMapLoader.UnloadScenesCoroutine(unloadScenes);
		}
		if (!loadScenes.IsNullOrEmpty<int>())
		{
			yield return CustomMapLoader.LoadScenesCoroutine(loadScenes, delegate(bool successfullyLoadedAllScenes, bool loadAborted, List<string> successfullyLoadedSceneNames)
			{
				if (loadAborted)
				{
					CustomMapLoader.queuedLoadZoneRequests.Clear();
				}
			});
		}
		CustomMapLoader.zoneLoadingCoroutine = null;
		if (CustomMapLoader.queuedLoadZoneRequests.Count > 0)
		{
			CustomMapLoader.LoadZoneRequest loadZoneRequest = CustomMapLoader.queuedLoadZoneRequests[0];
			CustomMapLoader.queuedLoadZoneRequests.RemoveAt(0);
			CustomMapLoader.LoadZoneTriggered(loadZoneRequest.sceneIndexesToLoad, loadZoneRequest.sceneIndexesToUnload, loadZoneRequest.onSceneLoadedCallback, loadZoneRequest.onSceneUnloadedCallback);
		}
		yield break;
	}

	// Token: 0x060042E7 RID: 17127 RVA: 0x0016729D File Offset: 0x0016549D
	public static void CloseDoorAndUnloadMap(Action unloadCompleted = null)
	{
		if (!CustomMapLoader.IsMapLoaded() && !CustomMapLoader.isLoading)
		{
			return;
		}
		if (unloadCompleted != null)
		{
			CustomMapLoader.unloadMapCallback = unloadCompleted;
		}
		if (CustomMapLoader.isLoading)
		{
			CustomMapLoader.RequestAbortMapLoad();
			return;
		}
		CustomMapLoader.instance.StartCoroutine(CustomMapLoader.CloseDoorAndUnloadMapCoroutine());
	}

	// Token: 0x060042E8 RID: 17128 RVA: 0x001672D6 File Offset: 0x001654D6
	private static IEnumerator CloseDoorAndUnloadMapCoroutine()
	{
		if (!CustomMapLoader.IsMapLoaded())
		{
			yield break;
		}
		if (CustomMapLoader.instance.accessDoor != null)
		{
			CustomMapLoader.instance.accessDoor.CloseDoor();
		}
		if (CustomMapLoader.instance.publicJoinTrigger != null)
		{
			CustomMapLoader.instance.publicJoinTrigger.SetActive(false);
		}
		CustomMapLoader.shouldAbortMapLoading = true;
		if (CustomMapLoader.IsLoading())
		{
			yield break;
		}
		yield return CustomMapLoader.UnloadMapCoroutine();
		yield break;
	}

	// Token: 0x060042E9 RID: 17129 RVA: 0x001672DE File Offset: 0x001654DE
	private static void RequestAbortMapLoad()
	{
		CustomMapLoader.shouldAbortSceneLoad = true;
		CustomMapLoader.shouldAbortMapLoading = true;
	}

	// Token: 0x060042EA RID: 17130 RVA: 0x001672EC File Offset: 0x001654EC
	private static IEnumerator AbortMapLoad()
	{
		GTDev.Log<string>("[CML.AbortMapLoad] Aborting map load...", null);
		CustomMapLoader.shouldAbortSceneLoad = true;
		CustomMapLoader.shouldAbortMapLoading = true;
		yield return CustomMapLoader.AbortSceneLoad(-1);
		Action<bool> action = CustomMapLoader.mapLoadFinishedCallback;
		if (action != null)
		{
			action(false);
		}
		yield break;
	}

	// Token: 0x060042EB RID: 17131 RVA: 0x001672F4 File Offset: 0x001654F4
	private static IEnumerator UnloadMapCoroutine()
	{
		GTDev.Log<string>("[CML.UnloadMap_Co] Unloading Custom Map...", null);
		if (CustomMapLoader.zoneLoadingCoroutine != null)
		{
			CustomMapLoader.queuedLoadZoneRequests.Clear();
			CustomMapLoader.instance.StopCoroutine(CustomMapLoader.zoneLoadingCoroutine);
			CustomMapLoader.zoneLoadingCoroutine = null;
		}
		CustomMapLoader.isUnloading = true;
		CustomMapLoader.CanLoadEntities = false;
		CustomMapTelemetry.EndMapTracking();
		ZoneShaderSettings.ActivateDefaultSettings();
		CustomMapLoader.CleanupPlaceholders();
		CMSSerializer.ResetSyncedMapObjects();
		CustomMapLoader.instance.ghostReactorManager.reactor.RefreshReviveStations(false);
		if (!CustomMapLoader.assetBundleSceneFilePaths.IsNullOrEmpty<string>())
		{
			int num;
			for (int sceneIndex = 0; sceneIndex < CustomMapLoader.assetBundleSceneFilePaths.Length; sceneIndex = num + 1)
			{
				yield return CustomMapLoader.UnloadSceneCoroutine(sceneIndex, null);
				num = sceneIndex;
			}
		}
		GorillaNetworkJoinTrigger.EnableTriggerJoins();
		LightmapSettings.lightmaps = CustomMapLoader.lightmaps;
		CustomMapLoader.UnloadLightmaps();
		yield return CustomMapLoader.ResetLightmaps();
		CustomMapLoader.SetZoneDynamicLighting(false);
		GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
		if (CustomMapLoader.mapBundle != null)
		{
			CustomMapLoader.mapBundle.Unload(true);
		}
		CustomMapLoader.mapBundle = null;
		Resources.UnloadUnusedAssets();
		CustomMapLoader.cachedLuauScript = "";
		CustomMapLoader.devModeEnabled = false;
		CustomMapLoader.disableHoldingHandsAllModes = false;
		CustomMapLoader.disableHoldingHandsCustomMode = false;
		CustomMapLoader.queuedLoadZoneRequests.Clear();
		CustomMapLoader.assetBundleSceneFilePaths = new string[]
		{
			""
		};
		CustomMapLoader.loadedMapPackageInfo = null;
		CustomMapLoader.loadedMapModId = 0L;
		CustomMapLoader.loadedSceneFilePaths.Clear();
		CustomMapLoader.loadedSceneNames.Clear();
		CustomMapLoader.loadedSceneIndexes.Clear();
		CustomMapLoader.initialSceneIndexes.Clear();
		CustomMapLoader.initialSceneNames.Clear();
		CustomMapLoader.maxPlayersForMap = 20;
		CustomMapModeSelector.ResetButtons();
		if (RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
		{
			if (GameMode.ActiveGameMode.IsNull())
			{
				GameMode.ChangeGameMode(GameModeType.Casual.ToString());
			}
			else if (GameMode.ActiveGameMode.GameType() != GameModeType.Casual)
			{
				GameMode.ChangeGameMode(GameModeType.Casual.ToString());
			}
		}
		CustomMapLoader.shouldAbortMapLoading = false;
		CustomMapLoader.shouldAbortSceneLoad = false;
		CustomMapLoader.isUnloading = false;
		if (CustomMapLoader.unloadMapCallback != null)
		{
			Action action = CustomMapLoader.unloadMapCallback;
			if (action != null)
			{
				action();
			}
			CustomMapLoader.unloadMapCallback = null;
		}
		yield break;
	}

	// Token: 0x060042EC RID: 17132 RVA: 0x001672FC File Offset: 0x001654FC
	private static IEnumerator AbortSceneLoad(int sceneIndex)
	{
		if (sceneIndex == -1)
		{
			CustomMapLoader.shouldAbortMapLoading = true;
		}
		CustomMapLoader.isLoading = false;
		if (CustomMapLoader.shouldAbortMapLoading)
		{
			yield return CustomMapLoader.UnloadMapCoroutine();
		}
		else
		{
			yield return CustomMapLoader.UnloadSceneCoroutine(sceneIndex, null);
		}
		CustomMapLoader.shouldAbortSceneLoad = false;
		yield break;
	}

	// Token: 0x060042ED RID: 17133 RVA: 0x0016730B File Offset: 0x0016550B
	private static IEnumerator UnloadScenesCoroutine(int[] sceneIndexes)
	{
		int num;
		for (int i = 0; i < sceneIndexes.Length; i = num + 1)
		{
			yield return CustomMapLoader.UnloadSceneCoroutine(sceneIndexes[i], null);
			num = i;
		}
		yield break;
	}

	// Token: 0x060042EE RID: 17134 RVA: 0x0016731A File Offset: 0x0016551A
	private static IEnumerator UnloadSceneCoroutine(int sceneIndex, Action OnUnloadComplete = null)
	{
		if (!CustomMapLoader.hasInstance)
		{
			yield break;
		}
		if (sceneIndex < 0 || sceneIndex >= CustomMapLoader.assetBundleSceneFilePaths.Length)
		{
			Debug.LogError(string.Format("[CustomMapLoader::UnloadSceneCoroutine] SceneIndex of {0} is invalid! ", sceneIndex) + string.Format("The currently loaded AssetBundle contains {0} scenes.", CustomMapLoader.assetBundleSceneFilePaths.Length));
			yield break;
		}
		while (CustomMapLoader.runningAsyncLoad)
		{
			yield return null;
		}
		UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects;
		string scenePathWithExtension = CustomMapLoader.assetBundleSceneFilePaths[sceneIndex];
		string[] array = scenePathWithExtension.Split(".", StringSplitOptions.None);
		string text = "";
		string sceneName = "";
		if (!array.IsNullOrEmpty<string>())
		{
			text = array[0];
			if (text.Length > 0)
			{
				sceneName = Path.GetFileName(text);
			}
		}
		Scene sceneByName = SceneManager.GetSceneByName(text);
		if (sceneByName.IsValid())
		{
			CustomMapLoader.RemoveUnloadingStorePrefabs(sceneByName);
			for (int i = CustomMapLoader.teleporters.Count - 1; i >= 0; i--)
			{
				if (CustomMapLoader.teleporters[i].gameObject.scene == sceneByName)
				{
					CustomMapLoader.teleporters.RemoveAt(i);
				}
			}
			AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scenePathWithExtension, options);
			yield return asyncOperation;
			CustomMapLoader.loadedSceneFilePaths.Remove(scenePathWithExtension);
			CustomMapLoader.loadedSceneNames.Remove(sceneName);
			CustomMapLoader.loadedSceneIndexes.Remove(sceneIndex);
			Action<string> action = CustomMapLoader.sceneUnloadedCallback;
			if (action != null)
			{
				action(sceneName);
			}
			if (OnUnloadComplete != null)
			{
				OnUnloadComplete();
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x060042EF RID: 17135 RVA: 0x00167330 File Offset: 0x00165530
	private static void RemoveUnloadingStorePrefabs(Scene unloadingScene)
	{
		if (CustomMapLoader.customMapATM.IsNotNull())
		{
			ATM_UI componentInChildren = CustomMapLoader.customMapATM.GetComponentInChildren<ATM_UI>();
			if (componentInChildren.IsNotNull() && componentInChildren.IsFromCustomMapScene(unloadingScene) && ATM_Manager.instance.IsNotNull())
			{
				ATM_Manager.instance.RemoveATM(componentInChildren);
				ATM_Manager.instance.SetTemporaryCreatorCode(null);
			}
			Object.Destroy(CustomMapLoader.customMapATM);
			CustomMapLoader.customMapATM = null;
		}
		for (int i = CustomMapLoader.storeDisplayStands.Count - 1; i >= 0; i--)
		{
			if (CustomMapLoader.storeDisplayStands[i].IsNull())
			{
				CustomMapLoader.storeDisplayStands.RemoveAt(i);
			}
			else
			{
				DynamicCosmeticStand componentInChildren2 = CustomMapLoader.storeDisplayStands[i].GetComponentInChildren<DynamicCosmeticStand>();
				if (componentInChildren2.IsNotNull() && componentInChildren2.IsFromCustomMapScene(unloadingScene))
				{
					if (componentInChildren2.IsNotNull())
					{
						StoreController.instance.RemoveStandFromPlayFabIDDictionary(componentInChildren2);
					}
					Object.Destroy(CustomMapLoader.storeDisplayStands[i]);
					CustomMapLoader.storeDisplayStands.RemoveAt(i);
				}
			}
		}
		for (int i = CustomMapLoader.storeCheckouts.Count - 1; i >= 0; i--)
		{
			if (CustomMapLoader.storeCheckouts[i].IsNull())
			{
				CustomMapLoader.storeCheckouts.RemoveAt(i);
			}
			else
			{
				ItemCheckout componentInChildren3 = CustomMapLoader.storeCheckouts[i].GetComponentInChildren<ItemCheckout>();
				if (componentInChildren3.IsNotNull() && componentInChildren3.IsFromScene(unloadingScene))
				{
					componentInChildren3.RemoveFromCustomMap(CustomMapLoader.instance.compositeTryOnArea);
					CosmeticsController.instance.RemoveItemCheckout(componentInChildren3);
					Object.Destroy(CustomMapLoader.storeCheckouts[i]);
					CustomMapLoader.storeCheckouts.RemoveAt(i);
				}
			}
		}
		for (int i = CustomMapLoader.storeTryOnConsoles.Count - 1; i >= 0; i--)
		{
			if (CustomMapLoader.storeTryOnConsoles[i].IsNull())
			{
				CustomMapLoader.storeTryOnConsoles.RemoveAt(i);
			}
			else if (CustomMapLoader.storeTryOnConsoles[i].scene.Equals(unloadingScene))
			{
				FittingRoom componentInChildren4 = CustomMapLoader.storeTryOnConsoles[i].GetComponentInChildren<FittingRoom>();
				if (componentInChildren4.IsNotNull())
				{
					CosmeticsController.instance.RemoveFittingRoom(componentInChildren4);
				}
				CustomMapLoader.storeTryOnConsoles.RemoveAt(i);
			}
		}
		for (int i = CustomMapLoader.storeTryOnAreas.Count - 1; i >= 0; i--)
		{
			if (CustomMapLoader.storeTryOnAreas[i].IsNull())
			{
				CustomMapLoader.storeTryOnAreas.RemoveAt(i);
			}
			else
			{
				CMSTryOnArea component = CustomMapLoader.storeTryOnAreas[i].GetComponent<CMSTryOnArea>();
				if (component.IsNotNull() && component.IsFromScene(unloadingScene))
				{
					component.RemoveFromCustomMap(CustomMapLoader.instance.compositeTryOnArea);
					Object.Destroy(CustomMapLoader.storeTryOnAreas[i]);
					CustomMapLoader.storeTryOnAreas.RemoveAt(i);
				}
			}
		}
	}

	// Token: 0x060042F0 RID: 17136 RVA: 0x001675DC File Offset: 0x001657DC
	private static void CleanupPlaceholders()
	{
		for (int i = 0; i < CustomMapLoader.instance.leafGliders.Length; i++)
		{
			CustomMapLoader.instance.leafGliders[i].CustomMapUnload();
			CustomMapLoader.instance.leafGliders[i].enabled = false;
			CustomMapLoader.instance.leafGliders[i].transform.GetChild(0).gameObject.SetActive(false);
		}
	}

	// Token: 0x060042F1 RID: 17137 RVA: 0x0016764D File Offset: 0x0016584D
	private static IEnumerator ResetLightmaps()
	{
		CustomMapLoader.instance.dayNightManager.RequestRepopulateLightmaps();
		LoadSceneParameters parameters = new LoadSceneParameters
		{
			loadSceneMode = LoadSceneMode.Additive,
			localPhysicsMode = LocalPhysicsMode.None
		};
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(10, parameters);
		yield return asyncOperation;
		asyncOperation = SceneManager.UnloadSceneAsync(10);
		yield return asyncOperation;
		yield break;
	}

	// Token: 0x060042F2 RID: 17138 RVA: 0x00167658 File Offset: 0x00165858
	private static void UnloadLightmaps()
	{
		foreach (LightmapData lightmapData in LightmapSettings.lightmaps)
		{
			if (lightmapData.lightmapColor != null && !CustomMapLoader.lightmapsToKeep.Contains(lightmapData.lightmapColor))
			{
				Resources.UnloadAsset(lightmapData.lightmapColor);
			}
			if (lightmapData.lightmapDir != null && !CustomMapLoader.lightmapsToKeep.Contains(lightmapData.lightmapDir))
			{
				Resources.UnloadAsset(lightmapData.lightmapDir);
			}
		}
	}

	// Token: 0x060042F3 RID: 17139 RVA: 0x001676D4 File Offset: 0x001658D4
	private static int GetSceneIndex(string sceneName)
	{
		int result = -1;
		if (CustomMapLoader.assetBundleSceneFilePaths.Length == 1)
		{
			return 0;
		}
		for (int i = 0; i < CustomMapLoader.assetBundleSceneFilePaths.Length; i++)
		{
			string sceneNameFromFilePath = CustomMapLoader.GetSceneNameFromFilePath(CustomMapLoader.assetBundleSceneFilePaths[i]);
			if (sceneNameFromFilePath != null && sceneNameFromFilePath.Equals(sceneName))
			{
				result = i;
				break;
			}
		}
		return result;
	}

	// Token: 0x060042F4 RID: 17140 RVA: 0x00167723 File Offset: 0x00165923
	private static string GetSceneNameFromFilePath(string filePath)
	{
		string[] array = filePath.Split("/", StringSplitOptions.None);
		return array[array.Length - 1].Split(".", StringSplitOptions.None)[0];
	}

	// Token: 0x060042F5 RID: 17141 RVA: 0x00167744 File Offset: 0x00165944
	public static MapPackageInfo GetPackageInfo(string packageInfoFilePath)
	{
		MapPackageInfo result;
		using (StreamReader streamReader = new StreamReader(File.OpenRead(packageInfoFilePath), Encoding.Default))
		{
			result = JsonConvert.DeserializeObject<MapPackageInfo>(streamReader.ReadToEnd());
		}
		return result;
	}

	// Token: 0x17000631 RID: 1585
	// (get) Token: 0x060042F6 RID: 17142 RVA: 0x0016778C File Offset: 0x0016598C
	public static ModId LoadedMapModId
	{
		get
		{
			return CustomMapLoader.loadedMapModId;
		}
	}

	// Token: 0x17000632 RID: 1586
	// (get) Token: 0x060042F7 RID: 17143 RVA: 0x00167793 File Offset: 0x00165993
	public static long LoadedMapModFileId
	{
		get
		{
			return CustomMapLoader.loadedMapModFileId;
		}
	}

	// Token: 0x17000633 RID: 1587
	// (get) Token: 0x060042F9 RID: 17145 RVA: 0x001677A2 File Offset: 0x001659A2
	// (set) Token: 0x060042F8 RID: 17144 RVA: 0x0016779A File Offset: 0x0016599A
	public static bool CanLoadEntities { get; private set; }

	// Token: 0x060042FA RID: 17146 RVA: 0x001677A9 File Offset: 0x001659A9
	public static bool IsMapLoaded()
	{
		return CustomMapLoader.IsMapLoaded(ModId.Null);
	}

	// Token: 0x060042FB RID: 17147 RVA: 0x001677B8 File Offset: 0x001659B8
	public static bool IsMapLoaded(ModId mapModId)
	{
		if (mapModId.IsValid())
		{
			return !CustomMapLoader.IsLoading() && CustomMapLoader.LoadedMapModId == mapModId;
		}
		return !CustomMapLoader.IsLoading() && CustomMapLoader.LoadedMapModId.IsValid();
	}

	// Token: 0x060042FC RID: 17148 RVA: 0x001677F9 File Offset: 0x001659F9
	public static bool IsLoading()
	{
		return CustomMapLoader.isLoading;
	}

	// Token: 0x060042FD RID: 17149 RVA: 0x00167800 File Offset: 0x00165A00
	public static long GetLoadingMapModId()
	{
		return CustomMapLoader.attemptedLoadID;
	}

	// Token: 0x060042FE RID: 17150 RVA: 0x00167807 File Offset: 0x00165A07
	public static byte GetRoomSizeForCurrentlyLoadedMap()
	{
		if (!CustomMapLoader.IsMapLoaded())
		{
			return 20;
		}
		return CustomMapLoader.maxPlayersForMap;
	}

	// Token: 0x060042FF RID: 17151 RVA: 0x00167818 File Offset: 0x00165A18
	public static bool IsCustomScene(string sceneName)
	{
		return CustomMapLoader.loadedSceneNames.Contains(sceneName);
	}

	// Token: 0x06004300 RID: 17152 RVA: 0x00167825 File Offset: 0x00165A25
	public static string GetLuauGamemodeScript()
	{
		if (!CustomMapLoader.IsMapLoaded())
		{
			return "";
		}
		return CustomMapLoader.cachedLuauScript;
	}

	// Token: 0x06004301 RID: 17153 RVA: 0x00167839 File Offset: 0x00165A39
	public static bool IsDevModeEnabled()
	{
		return CustomMapLoader.IsMapLoaded() && CustomMapLoader.devModeEnabled;
	}

	// Token: 0x06004302 RID: 17154 RVA: 0x00167849 File Offset: 0x00165A49
	public static Transform GetCustomMapsDefaultSpawnLocation()
	{
		if (CustomMapLoader.hasInstance)
		{
			return CustomMapLoader.instance.CustomMapsDefaultSpawnLocation;
		}
		return null;
	}

	// Token: 0x06004303 RID: 17155 RVA: 0x00167860 File Offset: 0x00165A60
	public static bool LoadedMapWantsHoldingHandsDisabled()
	{
		return CustomMapLoader.IsMapLoaded() && (CustomMapLoader.disableHoldingHandsAllModes || (CustomMapLoader.disableHoldingHandsCustomMode && GorillaGameManager.instance.IsNotNull() && GorillaGameManager.instance.GameType() == GameModeType.Custom));
	}

	// Token: 0x06004304 RID: 17156 RVA: 0x00167897 File Offset: 0x00165A97
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.defaultNexusGroupId == null)
		{
			Debug.LogError("You have to set defaultNexusGroupId in " + base.name + " or things will not work!");
			return false;
		}
		return true;
	}

	// Token: 0x040054B3 RID: 21683
	[SerializeField]
	private NexusGroupId defaultNexusGroupId;

	// Token: 0x040054B4 RID: 21684
	[OnEnterPlay_SetNull]
	private static volatile CustomMapLoader instance;

	// Token: 0x040054B5 RID: 21685
	[OnEnterPlay_Set(false)]
	private static bool hasInstance;

	// Token: 0x040054B6 RID: 21686
	public Transform CustomMapsDefaultSpawnLocation;

	// Token: 0x040054B7 RID: 21687
	public CustomMapAccessDoor accessDoor;

	// Token: 0x040054B8 RID: 21688
	[FormerlySerializedAs("networkTrigger")]
	public GameObject publicJoinTrigger;

	// Token: 0x040054B9 RID: 21689
	[SerializeField]
	private BetterDayNightManager dayNightManager;

	// Token: 0x040054BA RID: 21690
	[SerializeField]
	private GhostReactorManager ghostReactorManager;

	// Token: 0x040054BB RID: 21691
	[SerializeField]
	private GameObject placeholderParent;

	// Token: 0x040054BC RID: 21692
	[SerializeField]
	private GliderHoldable[] leafGliders;

	// Token: 0x040054BD RID: 21693
	[SerializeField]
	private GameObject leafGlider;

	// Token: 0x040054BE RID: 21694
	[SerializeField]
	private GameObject gliderWindVolume;

	// Token: 0x040054BF RID: 21695
	[FormerlySerializedAs("waterVolume")]
	[SerializeField]
	private GameObject waterVolumePrefab;

	// Token: 0x040054C0 RID: 21696
	[SerializeField]
	private WaterParameters defaultWaterParameters;

	// Token: 0x040054C1 RID: 21697
	[SerializeField]
	private WaterParameters defaultLavaParameters;

	// Token: 0x040054C2 RID: 21698
	[FormerlySerializedAs("forceVolume")]
	[SerializeField]
	private GameObject forceVolumePrefab;

	// Token: 0x040054C3 RID: 21699
	[SerializeField]
	private GameObject atmPrefab;

	// Token: 0x040054C4 RID: 21700
	[SerializeField]
	private GameObject atmNoShellPrefab;

	// Token: 0x040054C5 RID: 21701
	[SerializeField]
	private GameObject storeDisplayStandPrefab;

	// Token: 0x040054C6 RID: 21702
	[SerializeField]
	private GameObject storeCheckoutCounterPrefab;

	// Token: 0x040054C7 RID: 21703
	[SerializeField]
	private GameObject storeTryOnConsolePrefab;

	// Token: 0x040054C8 RID: 21704
	[SerializeField]
	private GameObject storeTryOnAreaPrefab;

	// Token: 0x040054C9 RID: 21705
	[SerializeField]
	private GameObject hoverboardDispenserPrefab;

	// Token: 0x040054CA RID: 21706
	[SerializeField]
	private GameObject ropeSwingPrefab;

	// Token: 0x040054CB RID: 21707
	[SerializeField]
	private GameObject ziplinePrefab;

	// Token: 0x040054CC RID: 21708
	[SerializeField]
	private GameObject reviveStationPrefab;

	// Token: 0x040054CD RID: 21709
	[SerializeField]
	private GameObject zoneShaderSettingsTrigger;

	// Token: 0x040054CE RID: 21710
	[SerializeField]
	private AudioMixerGroup masterAudioMixer;

	// Token: 0x040054CF RID: 21711
	[SerializeField]
	private ZoneShaderSettings customMapZoneShaderSettings;

	// Token: 0x040054D0 RID: 21712
	[SerializeField]
	private CompositeTriggerEvents compositeTryOnArea;

	// Token: 0x040054D1 RID: 21713
	[SerializeField]
	private GameObject virtualStumpMesh;

	// Token: 0x040054D2 RID: 21714
	[SerializeField]
	private List<GameModeType> availableModesForOldMaps = new List<GameModeType>
	{
		GameModeType.Infection,
		GameModeType.FreezeTag,
		GameModeType.Paintbrawl
	};

	// Token: 0x040054D3 RID: 21715
	[SerializeField]
	private GameModeType defaultGameModeForNonCustomOldMaps = GameModeType.Infection;

	// Token: 0x040054D4 RID: 21716
	public TMP_FontAsset DefaultFont;

	// Token: 0x040054D5 RID: 21717
	private static readonly int numObjectsToProcessPerFrame = 5;

	// Token: 0x040054D6 RID: 21718
	private static readonly List<int> APPROVED_LAYERS = new List<int>
	{
		0,
		1,
		2,
		4,
		5,
		9,
		11,
		18,
		20,
		22,
		27,
		30
	};

	// Token: 0x040054D7 RID: 21719
	private static bool isLoading;

	// Token: 0x040054D8 RID: 21720
	private static bool isUnloading;

	// Token: 0x040054D9 RID: 21721
	private static bool runningAsyncLoad = false;

	// Token: 0x040054DA RID: 21722
	private static long attemptedLoadID = 0L;

	// Token: 0x040054DB RID: 21723
	private static string attemptedSceneToLoad;

	// Token: 0x040054DC RID: 21724
	private static bool shouldAbortMapLoading = false;

	// Token: 0x040054DD RID: 21725
	private static bool shouldAbortSceneLoad = false;

	// Token: 0x040054DE RID: 21726
	private static bool errorEncounteredDuringLoad = false;

	// Token: 0x040054DF RID: 21727
	private static Action unloadMapCallback;

	// Token: 0x040054E0 RID: 21728
	private static string cachedExceptionMessage = "";

	// Token: 0x040054E1 RID: 21729
	private static AssetBundle mapBundle;

	// Token: 0x040054E2 RID: 21730
	private static List<string> initialSceneNames = new List<string>();

	// Token: 0x040054E3 RID: 21731
	private static List<int> initialSceneIndexes = new List<int>();

	// Token: 0x040054E4 RID: 21732
	private static byte maxPlayersForMap = 20;

	// Token: 0x040054E5 RID: 21733
	private static ModId loadedMapModId;

	// Token: 0x040054E6 RID: 21734
	private static long loadedMapModFileId;

	// Token: 0x040054E7 RID: 21735
	private static MapPackageInfo loadedMapPackageInfo;

	// Token: 0x040054E8 RID: 21736
	private static string cachedLuauScript;

	// Token: 0x040054E9 RID: 21737
	private static bool devModeEnabled;

	// Token: 0x040054EA RID: 21738
	private static bool disableHoldingHandsAllModes;

	// Token: 0x040054EB RID: 21739
	private static bool disableHoldingHandsCustomMode;

	// Token: 0x040054EC RID: 21740
	private static Action<MapLoadStatus, int, string> mapLoadProgressCallback;

	// Token: 0x040054ED RID: 21741
	private static Action<bool> mapLoadFinishedCallback;

	// Token: 0x040054EE RID: 21742
	private static Coroutine zoneLoadingCoroutine;

	// Token: 0x040054EF RID: 21743
	private static Action<string> sceneLoadedCallback;

	// Token: 0x040054F0 RID: 21744
	private static Action<string> sceneUnloadedCallback;

	// Token: 0x040054F1 RID: 21745
	private static List<CustomMapLoader.LoadZoneRequest> queuedLoadZoneRequests = new List<CustomMapLoader.LoadZoneRequest>();

	// Token: 0x040054F2 RID: 21746
	private static string[] assetBundleSceneFilePaths;

	// Token: 0x040054F3 RID: 21747
	private static List<string> loadedSceneFilePaths = new List<string>();

	// Token: 0x040054F4 RID: 21748
	private static List<string> loadedSceneNames = new List<string>();

	// Token: 0x040054F5 RID: 21749
	private static List<int> loadedSceneIndexes = new List<int>();

	// Token: 0x040054F6 RID: 21750
	private Coroutine loadScenesCoroutine;

	// Token: 0x040054F7 RID: 21751
	private static int leafGliderIndex;

	// Token: 0x040054F8 RID: 21752
	private static bool usingDynamicLighting = false;

	// Token: 0x040054F9 RID: 21753
	private static bool refreshReviveStations = false;

	// Token: 0x040054FA RID: 21754
	private static int totalObjectsInLoadingScene = 0;

	// Token: 0x040054FB RID: 21755
	private static int objectsProcessedForLoadingScene = 0;

	// Token: 0x040054FC RID: 21756
	private static int objectsProcessedThisFrame = 0;

	// Token: 0x040054FD RID: 21757
	private static List<Component> initializePhaseTwoComponents = new List<Component>();

	// Token: 0x040054FE RID: 21758
	private static List<MapEntity> entitiesToCreate = new List<MapEntity>(Constants.aiAgentLimit);

	// Token: 0x040054FF RID: 21759
	private static LightmapData[] lightmaps;

	// Token: 0x04005500 RID: 21760
	private static List<Texture2D> lightmapsToKeep = new List<Texture2D>();

	// Token: 0x04005501 RID: 21761
	private static List<GameObject> placeholderReplacements = new List<GameObject>();

	// Token: 0x04005502 RID: 21762
	private static GameObject customMapATM = null;

	// Token: 0x04005503 RID: 21763
	private static List<GameObject> storeCheckouts = new List<GameObject>();

	// Token: 0x04005504 RID: 21764
	private static List<GameObject> storeDisplayStands = new List<GameObject>();

	// Token: 0x04005505 RID: 21765
	private static List<GameObject> storeTryOnConsoles = new List<GameObject>();

	// Token: 0x04005506 RID: 21766
	private static List<GameObject> storeTryOnAreas = new List<GameObject>();

	// Token: 0x04005507 RID: 21767
	private static List<Component> teleporters = new List<Component>();

	// Token: 0x04005508 RID: 21768
	private string dontDestroyOnLoadSceneName = "";

	// Token: 0x04005509 RID: 21769
	private static readonly List<Type> componentAllowlist = new List<Type>
	{
		typeof(MeshRenderer),
		typeof(Transform),
		typeof(MeshFilter),
		typeof(MeshRenderer),
		typeof(Collider),
		typeof(BoxCollider),
		typeof(SphereCollider),
		typeof(CapsuleCollider),
		typeof(MeshCollider),
		typeof(Light),
		typeof(ReflectionProbe),
		typeof(AudioSource),
		typeof(Animator),
		typeof(SkinnedMeshRenderer),
		typeof(TextMesh),
		typeof(ParticleSystem),
		typeof(ParticleSystemRenderer),
		typeof(RectTransform),
		typeof(SpriteRenderer),
		typeof(BillboardRenderer),
		typeof(Canvas),
		typeof(CanvasRenderer),
		typeof(CanvasScaler),
		typeof(GraphicRaycaster),
		typeof(Rigidbody),
		typeof(TrailRenderer),
		typeof(LineRenderer),
		typeof(LensFlareComponentSRP),
		typeof(Camera),
		typeof(UniversalAdditionalCameraData),
		typeof(NavMeshAgent),
		typeof(NavMesh),
		typeof(NavMeshObstacle),
		typeof(NavMeshLink),
		typeof(NavMeshModifierVolume),
		typeof(NavMeshModifier),
		typeof(NavMeshSurface),
		typeof(HingeJoint),
		typeof(ConstantForce),
		typeof(LODGroup),
		typeof(MapDescriptor),
		typeof(AccessDoorPlaceholder),
		typeof(MapOrientationPoint),
		typeof(SurfaceOverrideSettings),
		typeof(TeleporterSettings),
		typeof(TagZoneSettings),
		typeof(LuauTriggerSettings),
		typeof(MapBoundarySettings),
		typeof(ObjectActivationTriggerSettings),
		typeof(LoadZoneSettings),
		typeof(GTObjectPlaceholder),
		typeof(CMSZoneShaderSettings),
		typeof(ZoneShaderTriggerSettings),
		typeof(MultiPartFire),
		typeof(HandHoldSettings),
		typeof(CustomMapEjectButtonSettings),
		typeof(CustomMapSupport.BezierSpline),
		typeof(UberShaderDynamicLight),
		typeof(MapEntity),
		typeof(GrabbableEntity),
		typeof(AIAgent),
		typeof(AISpawnManager),
		typeof(AISpawnPoint),
		typeof(MapSpawnPoint),
		typeof(MapSpawnManager),
		typeof(RopeSwingSegment),
		typeof(ZiplineSegment),
		typeof(PlayAnimationTriggerSettings),
		typeof(SurfaceMoverSettings),
		typeof(MovingSurfaceSettings),
		typeof(CustomMapReviveStation),
		typeof(ProBuilderMesh),
		typeof(TMP_Text),
		typeof(TextMeshPro),
		typeof(TextMeshProUGUI),
		typeof(UniversalAdditionalLightData),
		typeof(BakerySkyLight),
		typeof(BakeryDirectLight),
		typeof(BakeryPointLight),
		typeof(ftLightmapsStorage),
		typeof(BakeryAlwaysRender),
		typeof(BakeryLightMesh),
		typeof(BakeryLightmapGroupSelector),
		typeof(BakeryPackAsSingleSquare),
		typeof(BakerySector),
		typeof(BakeryVolume),
		typeof(BakeryLightmapGroup)
	};

	// Token: 0x0400550A RID: 21770
	private static readonly List<string> componentTypeStringAllowList = new List<string>
	{
		"UnityEngine.Halo"
	};

	// Token: 0x0400550B RID: 21771
	private static readonly Type[] badComponents = new Type[]
	{
		typeof(EventTrigger),
		typeof(UIBehaviour),
		typeof(GorillaPressableButton),
		typeof(GorillaPressableDelayButton),
		typeof(Camera),
		typeof(AudioListener),
		typeof(VideoPlayer)
	};

	// Token: 0x02000A35 RID: 2613
	private struct LoadZoneRequest
	{
		// Token: 0x0400550D RID: 21773
		public int[] sceneIndexesToLoad;

		// Token: 0x0400550E RID: 21774
		public int[] sceneIndexesToUnload;

		// Token: 0x0400550F RID: 21775
		public Action<string> onSceneLoadedCallback;

		// Token: 0x04005510 RID: 21776
		public Action<string> onSceneUnloadedCallback;
	}
}
