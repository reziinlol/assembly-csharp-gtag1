using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaNetworking;
using GorillaTag.Rendering;
using GorillaTagScripts.CustomMapSupport;
using GorillaTagScripts.UI.ModIO;
using GT_CustomMapSupportRuntime;
using Modio;
using Modio.Mods;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000F46 RID: 3910
	public class CustomMapManager : MonoBehaviour, IBuildValidation
	{
		// Token: 0x17000931 RID: 2353
		// (get) Token: 0x0600616B RID: 24939 RVA: 0x001F602B File Offset: 0x001F422B
		public static bool WaitingForRoomJoin
		{
			get
			{
				return CustomMapManager.waitingForRoomJoin;
			}
		}

		// Token: 0x17000932 RID: 2354
		// (get) Token: 0x0600616C RID: 24940 RVA: 0x001F6032 File Offset: 0x001F4232
		public static bool WaitingForDisconnect
		{
			get
			{
				return CustomMapManager.waitingForDisconnect;
			}
		}

		// Token: 0x17000933 RID: 2355
		// (get) Token: 0x0600616D RID: 24941 RVA: 0x001F6039 File Offset: 0x001F4239
		public static long LoadingMapId
		{
			get
			{
				return CustomMapManager.loadingMapId;
			}
		}

		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x0600616E RID: 24942 RVA: 0x001F6045 File Offset: 0x001F4245
		public static long UnloadingMapId
		{
			get
			{
				return CustomMapManager.unloadingMapId;
			}
		}

		// Token: 0x0600616F RID: 24943 RVA: 0x001F6051 File Offset: 0x001F4251
		public bool BuildValidationCheck()
		{
			if (this.defaultTeleporter.IsNull())
			{
				Debug.LogError("CustomMapManager does not have its \"Default Teleporter\" property.");
				return false;
			}
			return true;
		}

		// Token: 0x06006170 RID: 24944 RVA: 0x001F606D File Offset: 0x001F426D
		private void Awake()
		{
			if (CustomMapManager.instance == null)
			{
				CustomMapManager.instance = this;
				CustomMapManager.hasInstance = true;
				return;
			}
			if (CustomMapManager.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06006171 RID: 24945 RVA: 0x001F60A8 File Offset: 0x001F42A8
		public void OnEnable()
		{
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.SubscribeToUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			UGCPermissionManager.SubscribeToUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			CMSSerializer.OnTriggerHistoryProcessedForScene.AddListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
			ModIOManager.OnModManagementEvent.AddListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnected;
		}

		// Token: 0x06006172 RID: 24946 RVA: 0x001F61CC File Offset: 0x001F43CC
		public void OnDisable()
		{
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
		}

		// Token: 0x06006173 RID: 24947 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void OnUGCEnabled()
		{
		}

		// Token: 0x06006174 RID: 24948 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void OnUGCDisabled()
		{
		}

		// Token: 0x06006175 RID: 24949 RVA: 0x001F6264 File Offset: 0x001F4464
		private void Start()
		{
			CustomMapLoader.Initialize(new Action<MapLoadStatus, int, string>(CustomMapManager.OnMapLoadProgress), new Action<bool>(CustomMapManager.OnMapLoadFinished), new Action<string>(CustomMapManager.OnSceneLoaded), new Action<string>(CustomMapManager.OnSceneUnloaded));
			for (int i = this.virtualStumpTeleportLocations.Count - 1; i >= 0; i--)
			{
				if (this.virtualStumpTeleportLocations[i] == null)
				{
					this.virtualStumpTeleportLocations.RemoveAt(i);
				}
			}
			if (this.defaultTeleporter.IsNull())
			{
				GTDev.LogError<string>("[CustomMapManager::Start] \"Default Teleporter\" property is invalid.", null);
			}
			this.virtualStumpToggleableRoot.SetActive(false);
			base.gameObject.SetActive(false);
		}

		// Token: 0x06006176 RID: 24950 RVA: 0x001F6310 File Offset: 0x001F4510
		private void OnDestroy()
		{
			if (CustomMapManager.instance == this)
			{
				CustomMapManager.instance = null;
				CustomMapManager.hasInstance = false;
			}
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
		}

		// Token: 0x06006177 RID: 24951 RVA: 0x001F63C4 File Offset: 0x001F45C4
		private void HandleModManagementEvent(Mod mod, Modfile modfile, ModInstallationManagement.OperationType jobType, ModInstallationManagement.OperationPhase jobPhase)
		{
			if (CustomMapManager.waitingForModInstall && CustomMapManager.waitingForModInstallId == mod.Id)
			{
				if (CustomMapManager.abortModLoadIds.Contains(mod.Id))
				{
					CustomMapManager.abortModLoadIds.Remove(mod.Id);
					if (CustomMapManager.waitingForModInstallId.Equals(mod.Id))
					{
						CustomMapManager.waitingForModInstall = false;
						CustomMapManager.waitingForModDownload = false;
						CustomMapManager.waitingForModInstallId = ModId.Null;
					}
					return;
				}
				switch (modfile.State)
				{
				case ModFileState.Downloading:
				case ModFileState.Updating:
					CustomMapManager.waitingForModDownload = true;
					return;
				case ModFileState.Downloaded:
					CustomMapManager.waitingForModDownload = false;
					return;
				case ModFileState.Installing:
				case ModFileState.Uninstalling:
					break;
				case ModFileState.Installed:
					CustomMapManager.waitingForModDownload = false;
					this.LoadInstalledMap(mod);
					break;
				case ModFileState.FileOperationFailed:
					switch (jobType)
					{
					case ModInstallationManagement.OperationType.Download:
						Debug.LogError("[CustomMapManager::HandleModManagementEvent] Failed to download map with modID " + mod.Id.ToString() + ", error: " + modfile.FileStateErrorCause.GetMessage());
						CustomMapManager.HandleMapLoadFailed("FAILED TO DOWNLOAD MAP: " + modfile.FileStateErrorCause.GetMessage());
						CustomMapManager.waitingForModDownload = false;
						return;
					case ModInstallationManagement.OperationType.Install:
						Debug.LogError("[CustomMapManager::HandleModManagementEvent] Failed to install map with modID " + mod.Id.ToString() + ", error: " + modfile.FileStateErrorCause.GetMessage());
						CustomMapManager.HandleMapLoadFailed("FAILED TO INSTALL MAP: " + modfile.FileStateErrorCause.GetMessage());
						return;
					case ModInstallationManagement.OperationType.Update:
						Debug.LogError("[CustomMapManager::HandleModManagementEvent] Failed to update map with modID " + mod.Id.ToString() + ", error: " + modfile.FileStateErrorCause.GetMessage());
						CustomMapManager.HandleMapLoadFailed("FAILED TO UPDATE MAP: " + modfile.FileStateErrorCause.GetMessage());
						return;
					default:
						return;
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06006178 RID: 24952 RVA: 0x001F6594 File Offset: 0x001F4794
		internal static void TeleportToVirtualStump(VirtualStumpTeleporter fromTeleporter, Action<bool> callback)
		{
			if (UGCPermissionManager.IsUGCDisabled)
			{
				return;
			}
			if (!CustomMapManager.hasInstance || fromTeleporter == null)
			{
				if (callback != null)
				{
					callback(false);
				}
				return;
			}
			CustomMapManager.instance.gameObject.SetActive(true);
			CustomMapManager.instance.StartCoroutine(CustomMapManager.Internal_TeleportToVirtualStump(fromTeleporter, callback));
		}

		// Token: 0x06006179 RID: 24953 RVA: 0x001F65EA File Offset: 0x001F47EA
		private static IEnumerator Internal_TeleportToVirtualStump(VirtualStumpTeleporter fromTeleporter, Action<bool> callback)
		{
			CustomMapManager.lastUsedTeleporter = fromTeleporter;
			CustomMapManager.preVStumpGamemode = GorillaComputer.instance.currentGameMode.Value;
			if (CustomMapManager.lastUsedTeleporter.GetAutoLoadGamemode() != GameModeType.None && CustomMapManager.lastUsedTeleporter.GetAutoLoadGamemode() != GameModeType.Count)
			{
				GorillaComputer.instance.SetGameModeWithoutButton(CustomMapManager.lastUsedTeleporter.GetAutoLoadGamemode().ToString());
			}
			GTDev.Log<string>("[CustomMapManager::TeleportToVirtualStump] Teleporting to Virtual Stump...", null);
			PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.CustomMap, "");
			GorillaTagger.Instance.overrideNotInFocus = true;
			GreyZoneManager greyZoneManager = GreyZoneManager.Instance;
			if (greyZoneManager != null)
			{
				greyZoneManager.ForceStopGreyZone();
			}
			if (CustomMapManager.instance.virtualStumpTeleportLocations.Count > 0)
			{
				int index = Random.Range(0, CustomMapManager.instance.virtualStumpTeleportLocations.Count);
				Transform randTeleportTarget = CustomMapManager.instance.virtualStumpTeleportLocations[index];
				CustomMapManager.instance.EnableTeleportHUD(true);
				CustomMapManager.lastUsedTeleporter.PlayTeleportEffects(true, true, CustomMapManager.instance.localTeleportSFXSource, true);
				yield return new WaitForSeconds(0.75f);
				CosmeticsController.instance.ClearCheckoutAndCart(false);
				CustomMapManager.instance.virtualStumpToggleableRoot.SetActive(true);
				GTPlayer.Instance.TeleportTo(randTeleportTarget, true, false);
				GorillaComputer.instance.SetInVirtualStump(true);
				yield return null;
				if (VRRig.LocalRig.IsNotNull() && VRRig.LocalRig.zoneEntity.IsNotNull())
				{
					VRRig.LocalRig.zoneEntity.DisableZoneChanges();
				}
				ZoneManagement.SetActiveZone(GTZone.customMaps);
				foreach (GameObject gameObject in CustomMapManager.instance.rootObjectsToDeactivateAfterTeleport)
				{
					if (gameObject != null)
					{
						gameObject.gameObject.SetActive(false);
					}
				}
				if (CustomMapManager.hasInstance && CustomMapManager.instance.virtualStumpZoneShaderSettings.IsNotNull())
				{
					CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(false);
				}
				else
				{
					ZoneShaderSettings.ActivateDefaultSettings();
				}
				CustomMapManager.instance.ghostReactorManager.reactor.EnableGhostReactorForVirtualStump();
				CustomMapManager.currentTeleportCallback = callback;
				CustomMapManager.pendingNewPrivateRoomName = "";
				CustomMapManager.preTeleportInPrivateRoom = false;
				if (NetworkSystem.Instance.InRoom)
				{
					if (NetworkSystem.Instance.SessionIsPrivate)
					{
						CustomMapManager.preTeleportInPrivateRoom = true;
						CustomMapManager.waitingForRoomJoin = true;
						CustomMapManager.pendingNewPrivateRoomName = GorillaComputer.instance.VStumpRoomPrepend + NetworkSystem.Instance.RoomName;
					}
					GTDev.Log<string>("[CustomMapManager::TeleportToVirtualStump] Returning to singleplayer...", null);
					CustomMapManager.waitingForLoginDisconnect = true;
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				else
				{
					GTDev.Log<string>("[CustomMapManager::TeleportToVirtualStump] Attempting auto-login to mod.io...", null);
					CustomMapManager.AttemptAutoLogin();
				}
				randTeleportTarget = null;
			}
			else
			{
				GTDev.Log<string>("[CustomMapManager::TeleportToVirtualStump] Not Teleporting, virtualStumpTeleportLocations is empty!", null);
				CustomMapManager.EndTeleport(false);
			}
			yield break;
		}

		// Token: 0x0600617A RID: 24954 RVA: 0x001F6600 File Offset: 0x001F4800
		private static void OnAutoLoginComplete(Error error)
		{
			GTDev.Log<string>(string.Format("[CustomMapManager::OnAutoLoginComplete] Error: {0}", error), null);
			if (!CustomMapManager.hasInstance)
			{
				Debug.LogError("[CustomMapManager::OnAutoLoginComplete] CustomMapManager not initialized!");
				return;
			}
			GTDev.Log<string>(string.Format("[CustomMapManager::OnAutoLoginComplete] Needs to rejoin private room: {0}", CustomMapManager.preTeleportInPrivateRoom), null);
			if (CustomMapManager.preTeleportInPrivateRoom)
			{
				if (NetworkSystem.Instance.netState != NetSystemState.Idle)
				{
					GTDev.Log<string>(string.Format("[CustomMapManager::OnAutoLoginComplete] Netstate not Idle, delaying join attempt. CurrentStatus: {0}", NetworkSystem.Instance.netState), null);
					CustomMapManager.delayedJoinCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedJoinVStumpPrivateRoom());
				}
				else
				{
					GTDev.Log<string>("[CustomMapManager::OnAutoLoginComplete] joining @ version of private room: " + CustomMapManager.pendingNewPrivateRoomName, null);
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
				}
			}
			GTDev.Log<string>(string.Format("[CustomMapManager::OnAutoLoginComplete] Waiting For D/C? {0}", CustomMapManager.waitingForDisconnect), null);
			if (!CustomMapManager.preTeleportInPrivateRoom && !CustomMapManager.waitingForDisconnect)
			{
				GTDev.Log<string>("[CustomMapManager::OnAutoLoginComplete] Ending teleport...", null);
				CustomMapManager.EndTeleport(true);
			}
			CustomMapManager.preTeleportInPrivateRoom = false;
		}

		// Token: 0x0600617B RID: 24955 RVA: 0x001F6707 File Offset: 0x001F4907
		private static IEnumerator DelayedJoinVStumpPrivateRoom()
		{
			GTDev.Log<string>("[CustomMapManager::DelayedJoinVStumpPrivateRoom] waiting for netstate to be Idle", null);
			while (NetworkSystem.Instance.netState != NetSystemState.Idle)
			{
				yield return null;
			}
			GTDev.Log<string>("[CustomMapManager::DelayedJoinVStumpPrivateRoom] joining @ version of private room: " + CustomMapManager.pendingNewPrivateRoomName, null);
			PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
			yield break;
		}

		// Token: 0x0600617C RID: 24956 RVA: 0x001F6710 File Offset: 0x001F4910
		public static void ExitVirtualStump(Action<bool> callback)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.lastUsedTeleporter.IsNull())
			{
				if (CustomMapManager.instance.defaultTeleporter.IsNull())
				{
					if (callback != null)
					{
						callback(false);
					}
				}
				else
				{
					CustomMapManager.lastUsedTeleporter = CustomMapManager.instance.defaultTeleporter;
				}
			}
			if (CustomMapManager.delayedJoinCoroutine != null)
			{
				CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedJoinCoroutine);
				CustomMapManager.delayedJoinCoroutine = null;
			}
			if (CustomMapManager.delayedTryAutoLoadCoroutine != null)
			{
				CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedTryAutoLoadCoroutine);
				CustomMapManager.delayedTryAutoLoadCoroutine = null;
			}
			CustomMapManager.instance.dayNightManager.RequestRepopulateLightmaps();
			PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.CustomMap, "");
			GorillaTagger.Instance.overrideNotInFocus = true;
			CustomMapManager.instance.EnableTeleportHUD(false);
			CustomMapManager.currentTeleportCallback = callback;
			CustomMapManager.exitVirtualStumpPending = true;
			if (!CustomMapManager.UnloadMap(false))
			{
				CustomMapManager.FinalizeExitVirtualStump();
			}
		}

		// Token: 0x0600617D RID: 24957 RVA: 0x001F67EC File Offset: 0x001F49EC
		private static void FinalizeExitVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			GTPlayer.Instance.SetHoverActive(false);
			VRRig.LocalRig.hoverboardVisual.SetNotHeld();
			RoomSystem.ClearOverridenRoomSize();
			CosmeticsController.instance.ClearCheckoutAndCart(false);
			foreach (GameObject gameObject in CustomMapManager.instance.rootObjectsToDeactivateAfterTeleport)
			{
				if (gameObject != null)
				{
					gameObject.gameObject.SetActive(true);
				}
			}
			if (CustomMapManager.lastUsedTeleporter.GetReturnGamemode() != GameModeType.None && CustomMapManager.lastUsedTeleporter.GetReturnGamemode() != GameModeType.Count)
			{
				GorillaComputer.instance.SetGameModeWithoutButton(CustomMapManager.lastUsedTeleporter.GetReturnGamemode().ToString());
			}
			else if (CustomMapManager.preVStumpGamemode != "")
			{
				GorillaComputer.instance.SetGameModeWithoutButton(CustomMapManager.preVStumpGamemode);
				CustomMapManager.preVStumpGamemode = "";
			}
			if (VRRig.LocalRig.IsNotNull())
			{
				GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
				if (component != null && component.State == GRPlayer.GRPlayerState.Ghost)
				{
					CustomMapManager.instance.defaultReviveStation.RevivePlayer(component);
				}
			}
			ZoneManagement.SetActiveZone(CustomMapManager.lastUsedTeleporter.GetZone());
			if (VRRig.LocalRig.IsNotNull() && VRRig.LocalRig.zoneEntity.IsNotNull())
			{
				VRRig.LocalRig.zoneEntity.EnableZoneChanges();
			}
			GorillaComputer.instance.SetInVirtualStump(false);
			GTPlayer.Instance.TeleportTo(CustomMapManager.lastUsedTeleporter.GetReturnTransform(), true, false);
			CustomMapManager.instance.virtualStumpToggleableRoot.SetActive(false);
			ZoneShaderSettings.ActivateDefaultSettings();
			VRRig.LocalRig.EnableVStumpReturnWatch(false);
			GTPlayer.Instance.SetHoverAllowed(false, true);
			CustomMapManager.exitVirtualStumpPending = false;
			if (CustomMapManager.delayedEndTeleportCoroutine != null)
			{
				CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedEndTeleportCoroutine);
			}
			CustomMapManager.delayedEndTeleportCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedEndTeleport());
			if (CustomMapManager.preTeleportInPrivateRoom)
			{
				CustomMapManager.waitingForRoomJoin = true;
				CustomMapManager.pendingNewPrivateRoomName = CustomMapManager.pendingNewPrivateRoomName.RemoveAll(GorillaComputer.instance.VStumpRoomPrepend, StringComparison.OrdinalIgnoreCase);
				PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
				return;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					CustomMapManager.waitingForRoomJoin = true;
					CustomMapManager.pendingNewPrivateRoomName = NetworkSystem.Instance.RoomName.RemoveAll(GorillaComputer.instance.VStumpRoomPrepend, StringComparison.OrdinalIgnoreCase);
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
					return;
				}
				if (CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger() != null)
				{
					CustomMapManager.waitingForRoomJoin = true;
					GorillaComputer.instance.allowedMapsToJoin = CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger().myCollider.myAllowedMapsToJoin;
					Debug.Log(string.Format("[CustomMapManager::FinalizeExit] allowedMaps: {0}", GorillaComputer.instance.allowedMapsToJoin));
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger(), JoinType.Solo, null, false);
					return;
				}
				NetworkSystem.Instance.ReturnToSinglePlayer();
				return;
			}
			else
			{
				if (CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger() != null)
				{
					GorillaComputer.instance.allowedMapsToJoin = CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger().myCollider.myAllowedMapsToJoin;
					Debug.Log(string.Format("[CustomMapManager::FinalizeExit] allowedMaps: {0}", GorillaComputer.instance.allowedMapsToJoin));
					CustomMapManager.waitingForRoomJoin = true;
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger(), JoinType.Solo, null, false);
					return;
				}
				CustomMapManager.EndTeleport(true);
				return;
			}
		}

		// Token: 0x0600617E RID: 24958 RVA: 0x001F6B64 File Offset: 0x001F4D64
		private static void OnJoinSpecificRoomResult(NetJoinResult result)
		{
			GTDev.Log<string>("[CustomMapManager::OnJoinSpecificRoomResult] Result: " + result.ToString(), null);
			switch (result)
			{
			case NetJoinResult.Failed_Full:
				CustomMapManager.instance.OnJoinRoomFailed();
				return;
			case NetJoinResult.AlreadyInRoom:
				CustomMapManager.instance.OnJoinedRoom();
				return;
			case NetJoinResult.Failed_Other:
				GTDev.Log<string>("[CustomMapManager::OnJoinSpecificRoomResult] Joining " + CustomMapManager.pendingNewPrivateRoomName + " failed, marking for retry... ", null);
				CustomMapManager.waitingForDisconnect = true;
				CustomMapManager.shouldRetryJoin = true;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600617F RID: 24959 RVA: 0x001F6BE4 File Offset: 0x001F4DE4
		private static void OnJoinSpecificRoomResultFailureAllowed(NetJoinResult result)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			GTDev.Log<string>("[CustomMapManager::OnJoinSpecificRoomResultFailureAllowed] Result: " + result.ToString(), null);
			switch (result)
			{
			case NetJoinResult.Success:
			case NetJoinResult.FallbackCreated:
				return;
			case NetJoinResult.Failed_Full:
			case NetJoinResult.Failed_Other:
				CustomMapManager.instance.OnJoinRoomFailed();
				return;
			case NetJoinResult.AlreadyInRoom:
				CustomMapManager.instance.OnJoinedRoom();
				return;
			default:
				return;
			}
		}

		// Token: 0x06006180 RID: 24960 RVA: 0x001F6C4C File Offset: 0x001F4E4C
		public static bool AreAllPlayersInVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return false;
			}
			foreach (VRRig vrrig in VRRigCache.ActiveRigs)
			{
				if (!CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(vrrig.creator.UserId))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06006181 RID: 24961 RVA: 0x001F6CC4 File Offset: 0x001F4EC4
		public static bool IsRemotePlayerInVirtualStump(string playerID)
		{
			return CustomMapManager.hasInstance && !CustomMapManager.instance.virtualStumpPlayerDetector.IsNull() && CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(playerID);
		}

		// Token: 0x06006182 RID: 24962 RVA: 0x001F6CFC File Offset: 0x001F4EFC
		public static bool IsLocalPlayerInVirtualStump()
		{
			return CustomMapManager.hasInstance && !CustomMapManager.instance.virtualStumpPlayerDetector.IsNull() && !VRRig.LocalRig.IsNull() && CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(VRRig.LocalRig.creator.UserId);
		}

		// Token: 0x06006183 RID: 24963 RVA: 0x001F6D5C File Offset: 0x001F4F5C
		private void OnDisconnected()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (GorillaComputer.hasInstance)
			{
				GorillaComputer.instance.IsPlayerInVirtualStump();
			}
			CustomMapManager.ClearRoomMap();
			if (CustomMapManager.waitingForLoginDisconnect)
			{
				CustomMapManager.waitingForLoginDisconnect = false;
				GTDev.Log<string>("[CustomMapManager::OnDisconnected] Attempting auto-login to mod.io...", null);
				CustomMapManager.AttemptAutoLogin();
				return;
			}
			if (CustomMapManager.waitingForDisconnect)
			{
				CustomMapManager.waitingForDisconnect = false;
				if (CustomMapManager.shouldRetryJoin)
				{
					CustomMapManager.shouldRetryJoin = false;
					GTDev.Log<string>("[CustomMapManager::OnDisconnected] Joining " + CustomMapManager.pendingNewPrivateRoomName + " failed previously, retrying once... ", null);
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResultFailureAllowed));
					return;
				}
				GTDev.Log<string>("[CustomMapManager::OnDisconnected] Ending teleport...", null);
				CustomMapManager.EndTeleport(true);
			}
		}

		// Token: 0x06006184 RID: 24964 RVA: 0x001F6E10 File Offset: 0x001F5010
		private static Task AttemptAutoLogin()
		{
			CustomMapManager.<AttemptAutoLogin>d__78 <AttemptAutoLogin>d__;
			<AttemptAutoLogin>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AttemptAutoLogin>d__.<>1__state = -1;
			<AttemptAutoLogin>d__.<>t__builder.Start<CustomMapManager.<AttemptAutoLogin>d__78>(ref <AttemptAutoLogin>d__);
			return <AttemptAutoLogin>d__.<>t__builder.Task;
		}

		// Token: 0x06006185 RID: 24965 RVA: 0x001F6E4B File Offset: 0x001F504B
		private void OnJoinRoomFailed()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.waitingForRoomJoin)
			{
				GTDev.Log<string>("[CustomMapManager::OnJoinRoomFailed] Currently waiting for room join, resetting state, ending teleport...", null);
				CustomMapManager.waitingForRoomJoin = false;
				CustomMapManager.EndTeleport(false);
			}
		}

		// Token: 0x06006186 RID: 24966 RVA: 0x001F6E74 File Offset: 0x001F5074
		private static void EndTeleport(bool teleportSuccessful)
		{
			if (CustomMapManager.hasInstance)
			{
				if (CustomMapManager.delayedEndTeleportCoroutine != null)
				{
					CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedEndTeleportCoroutine);
					CustomMapManager.delayedEndTeleportCoroutine = null;
				}
				if (CustomMapManager.delayedJoinCoroutine != null)
				{
					CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedJoinCoroutine);
					CustomMapManager.delayedJoinCoroutine = null;
				}
			}
			CustomMapManager.DisableTeleportHUD();
			GorillaTagger.Instance.overrideNotInFocus = false;
			PrivateUIRoom.StopForcedOverlay(PrivateUIRoom.OverlaySource.CustomMap);
			Action<bool> action = CustomMapManager.currentTeleportCallback;
			if (action != null)
			{
				action(teleportSuccessful);
			}
			CustomMapManager.currentTeleportCallback = null;
			if (CustomMapManager.hasInstance && !GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				GTDev.Log<string>("[CustomMapManager::EndTeleport] Player is not in VStump, disabling VStump_Lobby GameObject", null);
				CustomMapManager.instance.gameObject.SetActive(false);
			}
			if (teleportSuccessful && GorillaComputer.instance.IsPlayerInVirtualStump() && CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId() != ModId.Null)
			{
				bool flag = false;
				if (CustomMapManager.waitingForRoomJoin)
				{
					GTDev.Log<string>("[CustomMapManager::EndTeleport] Still waiting for room join, delaying auto-load...", null);
					flag = true;
				}
				else if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.IsMasterClient && VirtualStumpSerializer.IsWaitingForRoomInit())
				{
					GTDev.Log<string>("[CustomMapManager::EndTeleport] Still waiting for room init, delaying auto-load...", null);
					flag = true;
				}
				if (flag)
				{
					CustomMapManager.delayedTryAutoLoadCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedTryAutoLoad());
					return;
				}
				GTDev.Log<string>("[CustomMapManager::EndTeleport] Attempting auto-load...", null);
				if (!NetworkSystem.Instance.InRoom || (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient))
				{
					CustomMapManager.SetRoomMap(CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId());
					CustomMapManager.LoadMap(CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId());
					return;
				}
				if (CustomMapManager.GetRoomMapId() == CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId())
				{
					CustomMapManager.LoadMap(CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId());
				}
			}
		}

		// Token: 0x06006187 RID: 24967 RVA: 0x001F703A File Offset: 0x001F523A
		private static IEnumerator DelayedEndTeleport()
		{
			yield return new WaitForSecondsRealtime(CustomMapManager.instance.maxPostTeleportRoomProcessingTime);
			GTDev.Log<string>("[CustomMapManager::DelayedEndTeleport] Timer expired, force ending teleport...", null);
			CustomMapManager.EndTeleport(false);
			yield break;
		}

		// Token: 0x06006188 RID: 24968 RVA: 0x001F7042 File Offset: 0x001F5242
		private static IEnumerator DelayedTryAutoLoad()
		{
			while (CustomMapManager.waitingForRoomJoin || VirtualStumpSerializer.IsWaitingForRoomInit())
			{
				yield return new WaitForSeconds(0.1f);
			}
			GTDev.Log<string>("[CustomMapManager::DelayedTryAutoLoad] Room Init finished, attempting auto-load...", null);
			if (!NetworkSystem.Instance.InRoom || (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient))
			{
				CustomMapManager.SetRoomMap(CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId());
				CustomMapManager.LoadMap(CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId());
			}
			else if (CustomMapManager.GetRoomMapId() == CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId())
			{
				CustomMapManager.LoadMap(CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId());
			}
			yield break;
		}

		// Token: 0x06006189 RID: 24969 RVA: 0x001F704C File Offset: 0x001F524C
		private void OnJoinedRoom()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.waitingForRoomJoin)
			{
				CustomMapManager.waitingForRoomJoin = false;
				GTDev.Log<string>("[CustomMapManager::OnJoinedRoom] Ending teleport...", null);
				CustomMapManager.EndTeleport(true);
				if (CustomMapManager.lastUsedTeleporter.IsNotNull())
				{
					CustomMapManager.lastUsedTeleporter.PlayTeleportEffects(true, false, null, true);
				}
			}
		}

		// Token: 0x0600618A RID: 24970 RVA: 0x001F709C File Offset: 0x001F529C
		public static bool UnloadMap(bool returnToSinglePlayerIfInPublic = true)
		{
			if (CustomMapManager.unloadInProgress)
			{
				return false;
			}
			if (!CustomMapLoader.IsMapLoaded() && !CustomMapLoader.IsLoading())
			{
				if (CustomMapManager.loadInProgress)
				{
					GTDev.Log<string>("[CustomMapManager::UnloadMap] Map load is currently in progress... aborting...", null);
					CustomMapManager.abortModLoadIds.AddIfNew(CustomMapManager.loadingMapId);
					bool flag = CustomMapManager.waitingForModDownload;
					CustomMapManager.loadInProgress = false;
					CustomMapManager.loadingMapId = ModId.Null;
					CustomMapManager.waitingForModDownload = false;
					CustomMapManager.waitingForModInstall = false;
					CustomMapManager.waitingForModInstallId = ModId.Null;
					CustomMapManager.ClearRoomMap();
				}
				else
				{
					CustomMapManager.ClearRoomMap();
				}
				return false;
			}
			CustomMapManager.unloadInProgress = true;
			CustomMapManager.unloadingMapId = new ModId(CustomMapLoader.IsMapLoaded() ? CustomMapLoader.LoadedMapModId : CustomMapLoader.GetLoadingMapModId());
			CustomMapManager.OnMapLoadProgress(MapLoadStatus.Unloading, 0, "");
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.waitingForModDownload = false;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			CustomMapManager.ClearRoomMap();
			CustomGameMode.LuaScript = "";
			if (CustomGameMode.gameScriptRunner != null)
			{
				CustomGameMode.StopScript();
			}
			CustomMapManager.customMapDefaultZoneShaderSettingsInitialized = false;
			CustomMapManager.customMapDefaultZoneShaderProperties = default(CMSZoneShaderSettings.CMSZoneShaderProperties);
			CustomMapManager.loadedCustomMapDefaultZoneShaderSettings = null;
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.CopySettings(CustomMapManager.instance.virtualStumpZoneShaderSettings, false);
				CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(false);
				CustomMapManager.allCustomMapZoneShaderSettings.Clear();
			}
			CustomMapLoader.CloseDoorAndUnloadMap(new Action(CustomMapManager.OnMapUnloadCompleted));
			if (returnToSinglePlayerIfInPublic && NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			return true;
		}

		// Token: 0x0600618B RID: 24971 RVA: 0x001F7220 File Offset: 0x001F5420
		private static void OnMapUnloadCompleted()
		{
			CustomMapManager.unloadInProgress = false;
			CustomMapManager.OnMapUnloadComplete.Invoke();
			CustomMapManager.currentRoomMapModId = ModId.Null;
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(ModId.Null);
			if (CustomMapManager.exitVirtualStumpPending)
			{
				CustomMapManager.FinalizeExitVirtualStump();
			}
		}

		// Token: 0x0600618C RID: 24972 RVA: 0x001F7260 File Offset: 0x001F5460
		public static Task LoadMap(ModId modId)
		{
			CustomMapManager.<LoadMap>d__86 <LoadMap>d__;
			<LoadMap>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadMap>d__.modId = modId;
			<LoadMap>d__.<>1__state = -1;
			<LoadMap>d__.<>t__builder.Start<CustomMapManager.<LoadMap>d__86>(ref <LoadMap>d__);
			return <LoadMap>d__.<>t__builder.Task;
		}

		// Token: 0x0600618D RID: 24973 RVA: 0x001F72A4 File Offset: 0x001F54A4
		private Task LoadInstalledMap(Mod installedMod)
		{
			CustomMapManager.<LoadInstalledMap>d__87 <LoadInstalledMap>d__;
			<LoadInstalledMap>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadInstalledMap>d__.installedMod = installedMod;
			<LoadInstalledMap>d__.<>1__state = -1;
			<LoadInstalledMap>d__.<>t__builder.Start<CustomMapManager.<LoadInstalledMap>d__87>(ref <LoadInstalledMap>d__);
			return <LoadInstalledMap>d__.<>t__builder.Task;
		}

		// Token: 0x0600618E RID: 24974 RVA: 0x001F72E7 File Offset: 0x001F54E7
		private static void OnMapLoadProgress(MapLoadStatus loadStatus, int progress, string message)
		{
			CustomMapManager.OnMapLoadStatusChanged.Invoke(loadStatus, progress, message);
		}

		// Token: 0x0600618F RID: 24975 RVA: 0x001F72F8 File Offset: 0x001F54F8
		private static void OnMapLoadFinished(bool success)
		{
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.waitingForModDownload = false;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			if (success)
			{
				CustomMapLoader.OpenDoorToMap();
				if (!CustomMapLoader.GetLuauGamemodeScript().IsNullOrEmpty())
				{
					CustomGameMode.LuaScript = CustomMapLoader.GetLuauGamemodeScript();
					if (CustomGameMode.LuaScript != "" && CustomGameMode.GameModeInitialized && CustomGameMode.gameScriptRunner == null)
					{
						CustomGameMode.LuaStart();
					}
				}
			}
			CustomMapManager.OnMapLoadComplete.Invoke(success);
		}

		// Token: 0x06006190 RID: 24976 RVA: 0x001F737C File Offset: 0x001F557C
		private static void HandleMapLoadFailed(string message = null)
		{
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			CustomMapManager.OnMapLoadStatusChanged.Invoke(MapLoadStatus.Error, 0, message ?? "UNKNOWN ERROR");
			CustomMapManager.OnMapLoadComplete.Invoke(false);
		}

		// Token: 0x06006191 RID: 24977 RVA: 0x001F73CA File Offset: 0x001F55CA
		public static bool IsUnloading()
		{
			return CustomMapManager.unloadInProgress;
		}

		// Token: 0x06006192 RID: 24978 RVA: 0x001F73D1 File Offset: 0x001F55D1
		public static bool IsLoading()
		{
			return CustomMapManager.IsLoading(ModId.Null);
		}

		// Token: 0x06006193 RID: 24979 RVA: 0x001F73DD File Offset: 0x001F55DD
		public static bool IsLoading(ModId modId)
		{
			if (!modId.IsValid())
			{
				return CustomMapManager.loadInProgress || CustomMapLoader.IsLoading();
			}
			return CustomMapManager.loadInProgress && CustomMapManager.loadingMapId == modId;
		}

		// Token: 0x06006194 RID: 24980 RVA: 0x001F740C File Offset: 0x001F560C
		public static ModId GetRoomMapId()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (CustomMapManager.currentRoomMapModId == ModId.Null && NetworkSystem.Instance.IsMasterClient && CustomMapLoader.IsMapLoaded())
				{
					CustomMapManager.currentRoomMapModId = new ModId(CustomMapLoader.LoadedMapModId);
				}
				return CustomMapManager.currentRoomMapModId;
			}
			if (CustomMapManager.IsLoading())
			{
				return CustomMapManager.loadingMapId;
			}
			if (CustomMapLoader.IsMapLoaded())
			{
				return new ModId(CustomMapLoader.LoadedMapModId);
			}
			return ModId.Null;
		}

		// Token: 0x06006195 RID: 24981 RVA: 0x001F748C File Offset: 0x001F568C
		public static void SetRoomMap(long modId)
		{
			if (!CustomMapManager.hasInstance || modId == CustomMapManager.currentRoomMapModId._id)
			{
				return;
			}
			CustomMapManager.currentRoomMapModId = new ModId(modId);
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(CustomMapManager.currentRoomMapModId);
		}

		// Token: 0x06006196 RID: 24982 RVA: 0x001F74C4 File Offset: 0x001F56C4
		public static void ClearRoomMap()
		{
			if (!CustomMapManager.hasInstance || CustomMapManager.currentRoomMapModId.Equals(ModId.Null))
			{
				return;
			}
			CustomMapManager.currentRoomMapModId = ModId.Null;
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(ModId.Null);
		}

		// Token: 0x06006197 RID: 24983 RVA: 0x001F7514 File Offset: 0x001F5714
		public static bool CanLoadRoomMap()
		{
			return CustomMapManager.currentRoomMapModId != ModId.Null;
		}

		// Token: 0x06006198 RID: 24984 RVA: 0x001F752A File Offset: 0x001F572A
		public static void ApproveAndLoadRoomMap()
		{
			CustomMapManager.currentRoomMapApproved = true;
			CMSSerializer.ResetSyncedMapObjects();
			CustomMapManager.LoadMap(CustomMapManager.currentRoomMapModId);
		}

		// Token: 0x06006199 RID: 24985 RVA: 0x001F7542 File Offset: 0x001F5742
		public static void RequestEnableTeleportHUD(bool enteringVirtualStump)
		{
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.EnableTeleportHUD(enteringVirtualStump);
			}
		}

		// Token: 0x0600619A RID: 24986 RVA: 0x001F7558 File Offset: 0x001F5758
		private void EnableTeleportHUD(bool enteringVirtualStump)
		{
			if (CustomMapManager.teleportingHUD != null)
			{
				CustomMapManager.teleportingHUD.gameObject.SetActive(true);
				CustomMapManager.teleportingHUD.Initialize(enteringVirtualStump);
				return;
			}
			if (this.teleportingHUDPrefab != null)
			{
				Camera main = Camera.main;
				if (main != null)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.teleportingHUDPrefab, main.transform);
					if (gameObject != null)
					{
						CustomMapManager.teleportingHUD = gameObject.GetComponent<VirtualStumpTeleportingHUD>();
						if (CustomMapManager.teleportingHUD != null)
						{
							CustomMapManager.teleportingHUD.Initialize(enteringVirtualStump);
						}
					}
				}
			}
		}

		// Token: 0x0600619B RID: 24987 RVA: 0x001F75E9 File Offset: 0x001F57E9
		public static void DisableTeleportHUD()
		{
			if (CustomMapManager.teleportingHUD != null)
			{
				CustomMapManager.teleportingHUD.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600619C RID: 24988 RVA: 0x001F7608 File Offset: 0x001F5808
		public static void LoadZoneTriggered(int[] scenesToLoad, int[] scenesToUnload)
		{
			CustomMapLoader.LoadZoneTriggered(scenesToLoad, scenesToUnload, new Action<string>(CustomMapManager.OnSceneLoaded), new Action<string>(CustomMapManager.OnSceneUnloaded));
		}

		// Token: 0x0600619D RID: 24989 RVA: 0x001F7629 File Offset: 0x001F5829
		private static void OnSceneLoaded(string sceneName)
		{
			CMSSerializer.ProcessSceneLoad(sceneName);
			CustomMapManager.ProcessZoneShaderSettings(sceneName);
		}

		// Token: 0x0600619E RID: 24990 RVA: 0x001F7638 File Offset: 0x001F5838
		private static void OnSceneUnloaded(string sceneName)
		{
			CMSSerializer.UnregisterTriggers(sceneName);
			for (int i = CustomMapManager.allCustomMapZoneShaderSettings.Count - 1; i >= 0; i--)
			{
				if (CustomMapManager.allCustomMapZoneShaderSettings[i].IsNull())
				{
					CustomMapManager.allCustomMapZoneShaderSettings.RemoveAt(i);
				}
			}
		}

		// Token: 0x0600619F RID: 24991 RVA: 0x001F7680 File Offset: 0x001F5880
		private static void OnSceneTriggerHistoryProcessed(string sceneName)
		{
			CapsuleCollider bodyCollider = GTPlayer.Instance.bodyCollider;
			SphereCollider headCollider = GTPlayer.Instance.headCollider;
			Vector3 position = bodyCollider.transform.TransformPoint(bodyCollider.center);
			float radius = Mathf.Max(bodyCollider.height, bodyCollider.radius) * GTPlayer.Instance.scale;
			Collider[] array = new Collider[100];
			Physics.OverlapSphereNonAlloc(position, radius, array);
			foreach (Collider collider in array)
			{
				if (collider != null && collider.gameObject.scene.name.Equals(sceneName))
				{
					CMSTrigger[] components = collider.gameObject.GetComponents<CMSTrigger>();
					for (int j = 0; j < components.Length; j++)
					{
						if (components[j] != null)
						{
							components[j].OnTriggerEnter(bodyCollider);
							components[j].OnTriggerEnter(headCollider);
						}
					}
					CMSLoadingZone[] components2 = collider.gameObject.GetComponents<CMSLoadingZone>();
					for (int k = 0; k < components2.Length; k++)
					{
						if (components2[k] != null)
						{
							components2[k].OnTriggerEnter(bodyCollider);
						}
					}
					CMSZoneShaderSettingsTrigger[] components3 = collider.gameObject.GetComponents<CMSZoneShaderSettingsTrigger>();
					for (int l = 0; l < components3.Length; l++)
					{
						if (components3[l] != null)
						{
							components3[l].OnTriggerEnter(bodyCollider);
						}
					}
					HoverboardAreaTrigger[] components4 = collider.gameObject.GetComponents<HoverboardAreaTrigger>();
					for (int m = 0; m < components4.Length; m++)
					{
						if (components4[m] != null)
						{
							components4[m].OnTriggerEnter(headCollider);
						}
					}
					WaterVolume[] components5 = collider.gameObject.GetComponents<WaterVolume>();
					for (int n = 0; n < components5.Length; n++)
					{
						if (components5[n] != null)
						{
							components5[n].OnTriggerEnter(bodyCollider);
							components5[n].OnTriggerEnter(headCollider);
						}
					}
				}
			}
		}

		// Token: 0x060061A0 RID: 24992 RVA: 0x001F785F File Offset: 0x001F5A5F
		public static void SetDefaultZoneShaderSettings(ZoneShaderSettings defaultCustomMapShaderSettings, CMSZoneShaderSettings.CMSZoneShaderProperties defaultZoneShaderProperties)
		{
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.CopySettings(defaultCustomMapShaderSettings, true);
				CustomMapManager.loadedCustomMapDefaultZoneShaderSettings = defaultCustomMapShaderSettings;
				CustomMapManager.customMapDefaultZoneShaderProperties = defaultZoneShaderProperties;
				CustomMapManager.customMapDefaultZoneShaderSettingsInitialized = true;
			}
		}

		// Token: 0x060061A1 RID: 24993 RVA: 0x001F7890 File Offset: 0x001F5A90
		private static void ProcessZoneShaderSettings(string loadedSceneName)
		{
			if (CustomMapManager.hasInstance && CustomMapManager.customMapDefaultZoneShaderSettingsInitialized && CustomMapManager.customMapDefaultZoneShaderProperties.isInitialized)
			{
				for (int i = 0; i < CustomMapManager.allCustomMapZoneShaderSettings.Count; i++)
				{
					if (CustomMapManager.allCustomMapZoneShaderSettings[i].IsNotNull() && CustomMapManager.allCustomMapZoneShaderSettings[i] != CustomMapManager.loadedCustomMapDefaultZoneShaderSettings && CustomMapManager.allCustomMapZoneShaderSettings[i].gameObject.scene.name.Equals(loadedSceneName))
					{
						CustomMapManager.allCustomMapZoneShaderSettings[i].ReplaceDefaultValues(CustomMapManager.customMapDefaultZoneShaderProperties, true);
					}
				}
				return;
			}
			if (CustomMapManager.hasInstance && CustomMapManager.instance.virtualStumpZoneShaderSettings.IsNotNull())
			{
				for (int j = 0; j < CustomMapManager.allCustomMapZoneShaderSettings.Count; j++)
				{
					if (CustomMapManager.allCustomMapZoneShaderSettings[j].IsNotNull() && CustomMapManager.allCustomMapZoneShaderSettings[j].gameObject.scene.name.Equals(loadedSceneName))
					{
						CustomMapManager.allCustomMapZoneShaderSettings[j].ReplaceDefaultValues(CustomMapManager.instance.virtualStumpZoneShaderSettings, true);
					}
				}
			}
		}

		// Token: 0x060061A2 RID: 24994 RVA: 0x001F79BA File Offset: 0x001F5BBA
		public static void AddZoneShaderSettings(ZoneShaderSettings zoneShaderSettings)
		{
			CustomMapManager.allCustomMapZoneShaderSettings.AddIfNew(zoneShaderSettings);
		}

		// Token: 0x060061A3 RID: 24995 RVA: 0x001F79C7 File Offset: 0x001F5BC7
		public static void ActivateDefaultZoneShaderSettings()
		{
			if (CustomMapManager.hasInstance && CustomMapManager.customMapDefaultZoneShaderSettingsInitialized)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.BecomeActiveInstance(true);
				return;
			}
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(true);
			}
		}

		// Token: 0x060061A4 RID: 24996 RVA: 0x001F7A04 File Offset: 0x001F5C04
		public static void ReturnToVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (!GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				return;
			}
			if (CustomMapManager.instance.returnToVirtualStumpTeleportLocation.IsNotNull())
			{
				GTPlayer gtplayer = GTPlayer.Instance;
				if (gtplayer != null)
				{
					CustomMapLoader.ResetToInitialZone(new Action<string>(CustomMapManager.OnSceneLoaded), new Action<string>(CustomMapManager.OnSceneUnloaded));
					gtplayer.TeleportTo(CustomMapManager.instance.returnToVirtualStumpTeleportLocation, true, false);
				}
			}
		}

		// Token: 0x060061A5 RID: 24997 RVA: 0x001F7A7B File Offset: 0x001F5C7B
		public static bool WantsHoldingHandsDisabled()
		{
			if (GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				if (!CustomMapLoader.IsMapLoaded())
				{
					return true;
				}
				if (CustomMapLoader.LoadedMapWantsHoldingHandsDisabled())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04007013 RID: 28691
		[OnEnterPlay_SetNull]
		private static volatile CustomMapManager instance;

		// Token: 0x04007014 RID: 28692
		[OnEnterPlay_Set(false)]
		private static bool hasInstance = false;

		// Token: 0x04007015 RID: 28693
		[SerializeField]
		private GameObject virtualStumpToggleableRoot;

		// Token: 0x04007016 RID: 28694
		[SerializeField]
		private Transform returnToVirtualStumpTeleportLocation;

		// Token: 0x04007017 RID: 28695
		[SerializeField]
		private List<Transform> virtualStumpTeleportLocations;

		// Token: 0x04007018 RID: 28696
		[SerializeField]
		private GameObject[] rootObjectsToDeactivateAfterTeleport;

		// Token: 0x04007019 RID: 28697
		[SerializeField]
		private GorillaFriendCollider virtualStumpPlayerDetector;

		// Token: 0x0400701A RID: 28698
		[SerializeField]
		private ZoneShaderSettings virtualStumpZoneShaderSettings;

		// Token: 0x0400701B RID: 28699
		[SerializeField]
		private BetterDayNightManager dayNightManager;

		// Token: 0x0400701C RID: 28700
		[SerializeField]
		private GhostReactorManager ghostReactorManager;

		// Token: 0x0400701D RID: 28701
		[SerializeField]
		private GRReviveStation defaultReviveStation;

		// Token: 0x0400701E RID: 28702
		[SerializeField]
		private ZoneShaderSettings customMapDefaultZoneShaderSettings;

		// Token: 0x0400701F RID: 28703
		[SerializeField]
		private GameObject teleportingHUDPrefab;

		// Token: 0x04007020 RID: 28704
		[SerializeField]
		private AudioSource localTeleportSFXSource;

		// Token: 0x04007021 RID: 28705
		[SerializeField]
		private VirtualStumpTeleporter defaultTeleporter;

		// Token: 0x04007022 RID: 28706
		[SerializeField]
		private float maxPostTeleportRoomProcessingTime = 15f;

		// Token: 0x04007023 RID: 28707
		private static VirtualStumpTeleporter lastUsedTeleporter;

		// Token: 0x04007024 RID: 28708
		private static string preVStumpGamemode = "";

		// Token: 0x04007025 RID: 28709
		private static bool customMapDefaultZoneShaderSettingsInitialized;

		// Token: 0x04007026 RID: 28710
		private static ZoneShaderSettings loadedCustomMapDefaultZoneShaderSettings;

		// Token: 0x04007027 RID: 28711
		private static CMSZoneShaderSettings.CMSZoneShaderProperties customMapDefaultZoneShaderProperties;

		// Token: 0x04007028 RID: 28712
		private static readonly List<ZoneShaderSettings> allCustomMapZoneShaderSettings = new List<ZoneShaderSettings>();

		// Token: 0x04007029 RID: 28713
		private static bool loadInProgress = false;

		// Token: 0x0400702A RID: 28714
		private static ModId loadingMapId = ModId.Null;

		// Token: 0x0400702B RID: 28715
		private static bool unloadInProgress = false;

		// Token: 0x0400702C RID: 28716
		private static ModId unloadingMapId = ModId.Null;

		// Token: 0x0400702D RID: 28717
		private static List<ModId> abortModLoadIds = new List<ModId>();

		// Token: 0x0400702E RID: 28718
		private static bool waitingForModDownload = false;

		// Token: 0x0400702F RID: 28719
		private static bool waitingForModInstall = false;

		// Token: 0x04007030 RID: 28720
		private static ModId waitingForModInstallId = ModId.Null;

		// Token: 0x04007031 RID: 28721
		private static bool preTeleportInPrivateRoom = false;

		// Token: 0x04007032 RID: 28722
		private static string pendingNewPrivateRoomName = "";

		// Token: 0x04007033 RID: 28723
		private static Action<bool> currentTeleportCallback;

		// Token: 0x04007034 RID: 28724
		private static bool waitingForLoginDisconnect = false;

		// Token: 0x04007035 RID: 28725
		private static bool waitingForDisconnect = false;

		// Token: 0x04007036 RID: 28726
		private static bool waitingForRoomJoin = false;

		// Token: 0x04007037 RID: 28727
		private static bool shouldRetryJoin = false;

		// Token: 0x04007038 RID: 28728
		private static short pendingTeleportVFXIdx = -1;

		// Token: 0x04007039 RID: 28729
		private static bool exitVirtualStumpPending = false;

		// Token: 0x0400703A RID: 28730
		private static ModId currentRoomMapModId = ModId.Null;

		// Token: 0x0400703B RID: 28731
		private static bool currentRoomMapApproved = false;

		// Token: 0x0400703C RID: 28732
		private static VirtualStumpTeleportingHUD teleportingHUD;

		// Token: 0x0400703D RID: 28733
		private static Coroutine delayedEndTeleportCoroutine;

		// Token: 0x0400703E RID: 28734
		private static Coroutine delayedJoinCoroutine;

		// Token: 0x0400703F RID: 28735
		private static Coroutine delayedTryAutoLoadCoroutine;

		// Token: 0x04007040 RID: 28736
		public static UnityEvent<ModId> OnRoomMapChanged = new UnityEvent<ModId>();

		// Token: 0x04007041 RID: 28737
		public static UnityEvent<MapLoadStatus, int, string> OnMapLoadStatusChanged = new UnityEvent<MapLoadStatus, int, string>();

		// Token: 0x04007042 RID: 28738
		public static UnityEvent<bool> OnMapLoadComplete = new UnityEvent<bool>();

		// Token: 0x04007043 RID: 28739
		public static UnityEvent OnMapUnloadComplete = new UnityEvent();
	}
}
