using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02001074 RID: 4212
	public class PhotonNetworkController : MonoBehaviour
	{
		// Token: 0x170009E6 RID: 2534
		// (get) Token: 0x06006997 RID: 27031 RVA: 0x0022263A File Offset: 0x0022083A
		// (set) Token: 0x06006998 RID: 27032 RVA: 0x00222642 File Offset: 0x00220842
		public List<string> FriendIDList
		{
			get
			{
				return this.friendIDList;
			}
			set
			{
				this.friendIDList = value;
			}
		}

		// Token: 0x170009E7 RID: 2535
		// (get) Token: 0x06006999 RID: 27033 RVA: 0x0022264B File Offset: 0x0022084B
		// (set) Token: 0x0600699A RID: 27034 RVA: 0x00222653 File Offset: 0x00220853
		public string StartLevel
		{
			get
			{
				return this.startLevel;
			}
			set
			{
				this.startLevel = value;
			}
		}

		// Token: 0x170009E8 RID: 2536
		// (get) Token: 0x0600699B RID: 27035 RVA: 0x0022265C File Offset: 0x0022085C
		// (set) Token: 0x0600699C RID: 27036 RVA: 0x00222664 File Offset: 0x00220864
		public GTZone StartZone
		{
			get
			{
				return this.startZone;
			}
			set
			{
				this.startZone = value;
			}
		}

		// Token: 0x170009E9 RID: 2537
		// (get) Token: 0x0600699D RID: 27037 RVA: 0x0022266D File Offset: 0x0022086D
		public GTZone CurrentRoomZone
		{
			get
			{
				if (!(this.currentJoinTrigger != null))
				{
					return GTZone.none;
				}
				return this.currentJoinTrigger.zone;
			}
		}

		// Token: 0x170009EA RID: 2538
		// (get) Token: 0x0600699E RID: 27038 RVA: 0x0022268B File Offset: 0x0022088B
		// (set) Token: 0x0600699F RID: 27039 RVA: 0x00222693 File Offset: 0x00220893
		public GorillaGeoHideShowTrigger StartGeoTrigger
		{
			get
			{
				return this.startGeoTrigger;
			}
			set
			{
				this.startGeoTrigger = value;
			}
		}

		// Token: 0x060069A0 RID: 27040 RVA: 0x0022269C File Offset: 0x0022089C
		public void Awake()
		{
			if (PhotonNetworkController.Instance == null)
			{
				PhotonNetworkController.Instance = this;
			}
			else if (PhotonNetworkController.Instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			this.updatedName = false;
			this.playersInRegion = new int[this.serverRegions.Length];
			this.pingInRegion = new int[this.serverRegions.Length];
		}

		// Token: 0x060069A1 RID: 27041 RVA: 0x0022270C File Offset: 0x0022090C
		public void Start()
		{
			base.StartCoroutine(this.DisableOnStart());
			NetworkSystem.Instance.OnJoinedRoomEvent += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnected;
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.ReuseEventInstance = true;
		}

		// Token: 0x060069A2 RID: 27042 RVA: 0x00222778 File Offset: 0x00220978
		private IEnumerator DisableOnStart()
		{
			ZoneManagement.SetActiveZone(this.StartZone);
			yield break;
		}

		// Token: 0x060069A3 RID: 27043 RVA: 0x00222788 File Offset: 0x00220988
		public void FixedUpdate()
		{
			this.headRightHandDistance = (GTPlayer.Instance.headCollider.transform.position - GTPlayer.Instance.GetControllerTransform(false).position).magnitude;
			this.headLeftHandDistance = (GTPlayer.Instance.headCollider.transform.position - GTPlayer.Instance.GetControllerTransform(true).position).magnitude;
			this.headQuat = GTPlayer.Instance.headCollider.transform.rotation;
			if (!this.disableAFKKick && Quaternion.Angle(this.headQuat, this.lastHeadQuat) <= 0.01f && Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) < 0.001f && Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) < 0.001f && this.pauseTime + this.disconnectTime < Time.realtimeSinceStartup)
			{
				this.pauseTime = Time.realtimeSinceStartup;
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			else if (Quaternion.Angle(this.headQuat, this.lastHeadQuat) > 0.01f || Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) >= 0.001f || Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) >= 0.001f)
			{
				this.pauseTime = Time.realtimeSinceStartup;
			}
			this.lastHeadRightHandDistance = this.headRightHandDistance;
			this.lastHeadLeftHandDistance = this.headLeftHandDistance;
			this.lastHeadQuat = this.headQuat;
			if (this.deferredJoin && Time.realtimeSinceStartup >= this.partyJoinDeferredUntilTimestamp)
			{
				if ((this.partyJoinDeferredUntilTimestamp != 0f || NetworkSystem.Instance.netState == NetSystemState.Idle) && this.currentJoinTrigger != null)
				{
					this.deferredJoin = false;
					this.partyJoinDeferredUntilTimestamp = 0f;
					if (!(this.currentJoinTrigger == this.privateTrigger))
					{
						this.AttemptToJoinPublicRoom(this.currentJoinTrigger, this.currentJoinType, null, false);
						return;
					}
					if (this.customRoomID == this.roomToJoin || this.customRoomID == this.autoJoinRoom || this.customRoomID == this.LastRoomToJoin)
					{
						this.AttemptToAutoJoinSpecificRoom(this.customRoomID, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
						return;
					}
					this.AttemptToJoinSpecificRoom(this.customRoomID, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
					return;
				}
				else if (NetworkSystem.Instance.netState != NetSystemState.PingRecon && NetworkSystem.Instance.netState != NetSystemState.Initialization && NetworkSystem.Instance.netState != NetSystemState.Disconnecting)
				{
					this.deferredJoin = false;
					this.partyJoinDeferredUntilTimestamp = 0f;
				}
			}
		}

		// Token: 0x060069A4 RID: 27044 RVA: 0x00222A42 File Offset: 0x00220C42
		public void DeferJoining(float duration)
		{
			this.partyJoinDeferredUntilTimestamp = Mathf.Max(this.partyJoinDeferredUntilTimestamp, Time.realtimeSinceStartup + duration);
		}

		// Token: 0x060069A5 RID: 27045 RVA: 0x00222A5C File Offset: 0x00220C5C
		public void ClearDeferredJoin()
		{
			this.partyJoinDeferredUntilTimestamp = 0f;
			this.deferredJoin = false;
		}

		// Token: 0x060069A6 RID: 27046 RVA: 0x00222A70 File Offset: 0x00220C70
		public void AttemptToJoinPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType = JoinType.Solo, List<ValueTuple<string, string>> additionalCustomProperties = null, bool filterSubscribed = false)
		{
			this.AttemptToJoinPublicRoomAsync(triggeredTrigger, roomJoinType, additionalCustomProperties, filterSubscribed);
		}

		// Token: 0x060069A7 RID: 27047 RVA: 0x00222A80 File Offset: 0x00220C80
		private void AttemptToJoinPublicRoomAsync(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType, List<ValueTuple<string, string>> additionalCustomProperties, bool filterSubscribed)
		{
			PhotonNetworkController.<AttemptToJoinPublicRoomAsync>d__69 <AttemptToJoinPublicRoomAsync>d__;
			<AttemptToJoinPublicRoomAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AttemptToJoinPublicRoomAsync>d__.<>4__this = this;
			<AttemptToJoinPublicRoomAsync>d__.triggeredTrigger = triggeredTrigger;
			<AttemptToJoinPublicRoomAsync>d__.roomJoinType = roomJoinType;
			<AttemptToJoinPublicRoomAsync>d__.additionalCustomProperties = additionalCustomProperties;
			<AttemptToJoinPublicRoomAsync>d__.filterSubscribed = filterSubscribed;
			<AttemptToJoinPublicRoomAsync>d__.<>1__state = -1;
			<AttemptToJoinPublicRoomAsync>d__.<>t__builder.Start<PhotonNetworkController.<AttemptToJoinPublicRoomAsync>d__69>(ref <AttemptToJoinPublicRoomAsync>d__);
		}

		// Token: 0x060069A8 RID: 27048 RVA: 0x00222AD8 File Offset: 0x00220CD8
		public void AttemptToJoinRankedPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType = JoinType.Solo)
		{
			string mmrTier = RankedProgressionManager.Instance.GetRankedMatchmakingTier().ToString();
			string platform = "PC";
			this.AttemptToJoinRankedPublicRoomAsync(triggeredTrigger, mmrTier, platform, roomJoinType);
		}

		// Token: 0x060069A9 RID: 27049 RVA: 0x00222B14 File Offset: 0x00220D14
		private void AttemptToJoinRankedPublicRoomAsync(GorillaNetworkJoinTrigger triggeredTrigger, string mmrTier, string platform, JoinType roomJoinType)
		{
			PhotonNetworkController.<AttemptToJoinRankedPublicRoomAsync>d__71 <AttemptToJoinRankedPublicRoomAsync>d__;
			<AttemptToJoinRankedPublicRoomAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AttemptToJoinRankedPublicRoomAsync>d__.<>4__this = this;
			<AttemptToJoinRankedPublicRoomAsync>d__.triggeredTrigger = triggeredTrigger;
			<AttemptToJoinRankedPublicRoomAsync>d__.mmrTier = mmrTier;
			<AttemptToJoinRankedPublicRoomAsync>d__.platform = platform;
			<AttemptToJoinRankedPublicRoomAsync>d__.roomJoinType = roomJoinType;
			<AttemptToJoinRankedPublicRoomAsync>d__.<>1__state = -1;
			<AttemptToJoinRankedPublicRoomAsync>d__.<>t__builder.Start<PhotonNetworkController.<AttemptToJoinRankedPublicRoomAsync>d__71>(ref <AttemptToJoinRankedPublicRoomAsync>d__);
		}

		// Token: 0x060069AA RID: 27050 RVA: 0x00222B6C File Offset: 0x00220D6C
		private Task SendPartyFollowCommands()
		{
			PhotonNetworkController.<SendPartyFollowCommands>d__72 <SendPartyFollowCommands>d__;
			<SendPartyFollowCommands>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendPartyFollowCommands>d__.<>1__state = -1;
			<SendPartyFollowCommands>d__.<>t__builder.Start<PhotonNetworkController.<SendPartyFollowCommands>d__72>(ref <SendPartyFollowCommands>d__);
			return <SendPartyFollowCommands>d__.<>t__builder.Task;
		}

		// Token: 0x060069AB RID: 27051 RVA: 0x00222BA7 File Offset: 0x00220DA7
		private void AttemptToAutoJoinRoomCallback(NetJoinResult obj)
		{
			this.LastRoomToJoin = this.roomToJoin;
			switch (obj)
			{
			case NetJoinResult.Success:
				return;
			case NetJoinResult.FallbackCreated:
				return;
			case NetJoinResult.Failed_Full:
				return;
			case NetJoinResult.AlreadyInRoom:
				return;
			default:
				return;
			}
		}

		// Token: 0x060069AC RID: 27052 RVA: 0x00222BCF File Offset: 0x00220DCF
		public void AttemptToAutoJoinSpecificRoom(string roomID, JoinType roomJoinType)
		{
			this.roomToJoin = roomID;
			this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, new Action<NetJoinResult>(this.AttemptToAutoJoinRoomCallback));
		}

		// Token: 0x060069AD RID: 27053 RVA: 0x00222BED File Offset: 0x00220DED
		public void AttemptToJoinSpecificRoom(string roomID, JoinType roomJoinType)
		{
			this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, null);
		}

		// Token: 0x060069AE RID: 27054 RVA: 0x00222BF9 File Offset: 0x00220DF9
		public void AttemptToJoinSpecificRoomWithCallback(string roomID, JoinType roomJoinType, Action<NetJoinResult> callback)
		{
			this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, callback);
		}

		// Token: 0x060069AF RID: 27055 RVA: 0x00222C08 File Offset: 0x00220E08
		public Task AttemptToJoinSpecificRoomAsync(string roomID, JoinType roomJoinType, Action<NetJoinResult> callback)
		{
			PhotonNetworkController.<AttemptToJoinSpecificRoomAsync>d__81 <AttemptToJoinSpecificRoomAsync>d__;
			<AttemptToJoinSpecificRoomAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AttemptToJoinSpecificRoomAsync>d__.<>4__this = this;
			<AttemptToJoinSpecificRoomAsync>d__.roomID = roomID;
			<AttemptToJoinSpecificRoomAsync>d__.roomJoinType = roomJoinType;
			<AttemptToJoinSpecificRoomAsync>d__.callback = callback;
			<AttemptToJoinSpecificRoomAsync>d__.<>1__state = -1;
			<AttemptToJoinSpecificRoomAsync>d__.<>t__builder.Start<PhotonNetworkController.<AttemptToJoinSpecificRoomAsync>d__81>(ref <AttemptToJoinSpecificRoomAsync>d__);
			return <AttemptToJoinSpecificRoomAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060069B0 RID: 27056 RVA: 0x00222C64 File Offset: 0x00220E64
		private void DisconnectCleanup()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (GorillaParent.instance != null)
			{
				GorillaScoreboardSpawner[] componentsInChildren = GorillaParent.instance.GetComponentsInChildren<GorillaScoreboardSpawner>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnLeftRoom();
				}
			}
			this.attemptingToConnect = true;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.offlineVRRig)
			{
				if (skinnedMeshRenderer != null)
				{
					skinnedMeshRenderer.enabled = true;
				}
			}
			if (GorillaComputer.instance != null && !ApplicationQuittingState.IsQuitting)
			{
				this.UpdateTriggerScreens();
			}
			GTPlayer.Instance.maxJumpSpeed = 6.5f;
			GTPlayer.Instance.jumpMultiplier = 1.1f;
			MonkeAgent.instance.currentMasterClient = null;
			GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(false);
			this.initialGameMode = "";
		}

		// Token: 0x060069B1 RID: 27057 RVA: 0x00222D44 File Offset: 0x00220F44
		public void OnJoinedRoom()
		{
			if (NetworkSystem.Instance.GameModeString.IsNullOrEmpty())
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			this.initialGameMode = NetworkSystem.Instance.GameModeString;
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				this.currentJoinTrigger = this.privateTrigger;
				PhotonNetworkController.Instance.UpdateTriggerScreens();
			}
			else if (this.currentJoinType != JoinType.FollowingParty)
			{
				bool flag = false;
				for (int i = 0; i < GorillaComputer.instance.allowedMapsToJoin.Length; i++)
				{
					if (NetworkSystem.Instance.GameModeString.StartsWith(GorillaComputer.instance.allowedMapsToJoin[i]))
					{
						flag = true;
						break;
					}
				}
				if (flag && GorillaComputer.instance.friendJoinCollider != null && !GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId) && !GorillaComputer.instance.GetJoinTriggerFromFullGameModeString(NetworkSystem.Instance.GameModeString).groupJoinRequiredZonesAB.HasAnyFlag(VRRig.LocalRig.zoneEntity.currentNode.groupZoneAB))
				{
					Debug.Log(string.Format("NOT ALLOWED IN ROOM: Joined {0} room but physically in {1} zone", this.ParseZoneFromGameMode(NetworkSystem.Instance.GameModeString), VRRig.LocalRig.zoneEntity.currentNode.groupZoneAB));
					flag = false;
				}
				if (!flag)
				{
					GorillaComputer.instance.roomNotAllowed = true;
					NetworkSystem.Instance.ReturnToSinglePlayer();
					return;
				}
			}
			NetworkSystem.Instance.SetMyTutorialComplete();
			VRRigCache.Instance.InstantiateNetworkObject();
			if (NetworkSystem.Instance.IsMasterClient)
			{
				GorillaGameModes.GameMode.LoadGameModeFromProperty(this.initialGameMode);
			}
			GorillaComputer.instance.roomFull = false;
			GorillaComputer.instance.roomNotAllowed = false;
			if (this.currentJoinType == JoinType.JoinWithParty || this.currentJoinType == JoinType.JoinWithNearby || this.currentJoinType == JoinType.ForceJoinWithParty || this.currentJoinType == JoinType.JoinWithElevator)
			{
				this.keyToFollow = NetworkSystem.Instance.LocalPlayer.UserId + this.keyStr;
				NetworkSystem.Instance.BroadcastMyRoom(true, this.keyToFollow, this.shuffler);
			}
			MonkeAgent.instance.currentMasterClient = null;
			this.UpdateCurrentJoinTrigger();
			this.UpdateTriggerScreens();
			NetworkSystem.Instance.MultiplayerStarted();
		}

		// Token: 0x060069B2 RID: 27058 RVA: 0x00222F8C File Offset: 0x0022118C
		public void RegisterJoinTrigger(GorillaNetworkJoinTrigger trigger)
		{
			this.allJoinTriggers.Add(trigger);
		}

		// Token: 0x060069B3 RID: 27059 RVA: 0x00222F9C File Offset: 0x0022119C
		private void UpdateCurrentJoinTrigger()
		{
			GorillaNetworkJoinTrigger joinTriggerFromFullGameModeString = GorillaComputer.instance.GetJoinTriggerFromFullGameModeString(NetworkSystem.Instance.GameModeString);
			if (joinTriggerFromFullGameModeString != null)
			{
				this.currentJoinTrigger = joinTriggerFromFullGameModeString;
				return;
			}
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				if (this.currentJoinTrigger != this.privateTrigger)
				{
					Debug.LogError("IN a private game but private trigger isnt current");
					return;
				}
			}
			else
			{
				Debug.LogError("Not in private room and unabel tp update jointrigger.");
			}
		}

		// Token: 0x060069B4 RID: 27060 RVA: 0x00223008 File Offset: 0x00221208
		public void UpdateTriggerScreens()
		{
			foreach (GorillaNetworkJoinTrigger gorillaNetworkJoinTrigger in this.allJoinTriggers)
			{
				gorillaNetworkJoinTrigger.UpdateUI();
			}
		}

		// Token: 0x060069B5 RID: 27061 RVA: 0x00223058 File Offset: 0x00221258
		public void AttemptToFollowIntoPub(string userIDToFollow, int actorNumberToFollow, string newKeyStr, string shufflerStr, JoinType joinType)
		{
			this.friendToFollow = userIDToFollow;
			this.keyToFollow = userIDToFollow + newKeyStr;
			this.shuffler = shufflerStr;
			this.currentJoinType = joinType;
			this.ClearDeferredJoin();
			if (NetworkSystem.Instance.InRoom)
			{
				NetworkSystem.Instance.JoinFriendsRoom(this.friendToFollow, actorNumberToFollow, this.keyToFollow, this.shuffler);
			}
		}

		// Token: 0x060069B6 RID: 27062 RVA: 0x002230B9 File Offset: 0x002212B9
		public void OnDisconnected()
		{
			this.DisconnectCleanup();
		}

		// Token: 0x060069B7 RID: 27063 RVA: 0x002230C1 File Offset: 0x002212C1
		public void OnApplicationQuit()
		{
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion != "dev";
			}
		}

		// Token: 0x060069B8 RID: 27064 RVA: 0x002230E4 File Offset: 0x002212E4
		private string ReturnRoomName()
		{
			if (this.isPrivate)
			{
				return this.customRoomID;
			}
			return this.RandomRoomName();
		}

		// Token: 0x060069B9 RID: 27065 RVA: 0x002230FC File Offset: 0x002212FC
		private string RandomRoomName()
		{
			string text = "";
			for (int i = 0; i < 4; i++)
			{
				text += "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Length), 1);
			}
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				return text;
			}
			return this.RandomRoomName();
		}

		// Token: 0x060069BA RID: 27066 RVA: 0x00223154 File Offset: 0x00221354
		private string GetRegionWithLowestPing()
		{
			int num = 10000;
			int num2 = 0;
			for (int i = 0; i < this.serverRegions.Length; i++)
			{
				Debug.Log("ping in region " + this.serverRegions[i] + " is " + this.pingInRegion[i].ToString());
				if (this.pingInRegion[i] < num && this.pingInRegion[i] > 0)
				{
					num = this.pingInRegion[i];
					num2 = i;
				}
			}
			return this.serverRegions[num2];
		}

		// Token: 0x060069BB RID: 27067 RVA: 0x002231D4 File Offset: 0x002213D4
		public int TotalUsers()
		{
			int num = 0;
			foreach (int num2 in this.playersInRegion)
			{
				num += num2;
			}
			return num;
		}

		// Token: 0x060069BC RID: 27068 RVA: 0x00223204 File Offset: 0x00221404
		public string CurrentState()
		{
			if (NetworkSystem.Instance == null)
			{
				Debug.Log("Null netsys!!!");
			}
			return NetworkSystem.Instance.netState.ToString();
		}

		// Token: 0x060069BD RID: 27069 RVA: 0x00223240 File Offset: 0x00221440
		private void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				this.timeWhenApplicationPaused = new DateTime?(DateTime.Now);
				return;
			}
			if ((DateTime.Now - (this.timeWhenApplicationPaused ?? DateTime.Now)).TotalSeconds > (double)this.disconnectTime)
			{
				this.timeWhenApplicationPaused = null;
				NetworkSystem instance = NetworkSystem.Instance;
				if (instance != null)
				{
					instance.ReturnToSinglePlayer();
				}
			}
			if (NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom && NetworkSystem.Instance.netState == NetSystemState.InGame)
			{
				NetworkSystem instance2 = NetworkSystem.Instance;
				if (instance2 == null)
				{
					return;
				}
				instance2.ReturnToSinglePlayer();
			}
		}

		// Token: 0x060069BE RID: 27070 RVA: 0x002232ED File Offset: 0x002214ED
		private void OnApplicationFocus(bool focus)
		{
			if (!focus && NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom && NetworkSystem.Instance.netState == NetSystemState.InGame)
			{
				NetworkSystem instance = NetworkSystem.Instance;
				if (instance == null)
				{
					return;
				}
				instance.ReturnToSinglePlayer();
			}
		}

		// Token: 0x060069BF RID: 27071 RVA: 0x00223328 File Offset: 0x00221528
		private GTZone ParseZoneFromGameMode(string gameMode)
		{
			if (string.IsNullOrEmpty(gameMode))
			{
				return GTZone.none;
			}
			foreach (object obj in Enum.GetValues(typeof(GTZone)))
			{
				GTZone gtzone = (GTZone)obj;
				if (gtzone != GTZone.none && gameMode.StartsWith(gtzone.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					return gtzone;
				}
			}
			return GTZone.none;
		}

		// Token: 0x040079C1 RID: 31169
		[OnEnterPlay_SetNull]
		public static volatile PhotonNetworkController Instance;

		// Token: 0x040079C2 RID: 31170
		public int incrementCounter;

		// Token: 0x040079C3 RID: 31171
		public PlayFabAuthenticator playFabAuthenticator;

		// Token: 0x040079C4 RID: 31172
		public string[] serverRegions;

		// Token: 0x040079C5 RID: 31173
		public bool isPrivate;

		// Token: 0x040079C6 RID: 31174
		public string customRoomID;

		// Token: 0x040079C7 RID: 31175
		public GameObject playerOffset;

		// Token: 0x040079C8 RID: 31176
		public SkinnedMeshRenderer[] offlineVRRig;

		// Token: 0x040079C9 RID: 31177
		public bool attemptingToConnect;

		// Token: 0x040079CA RID: 31178
		private int currentRegionIndex;

		// Token: 0x040079CB RID: 31179
		public string currentGameType;

		// Token: 0x040079CC RID: 31180
		public bool roomCosmeticsInitialized;

		// Token: 0x040079CD RID: 31181
		public GameObject photonVoiceObjectPrefab;

		// Token: 0x040079CE RID: 31182
		public Dictionary<string, bool> playerCosmeticsLookup = new Dictionary<string, bool>();

		// Token: 0x040079CF RID: 31183
		private float lastHeadRightHandDistance;

		// Token: 0x040079D0 RID: 31184
		private float lastHeadLeftHandDistance;

		// Token: 0x040079D1 RID: 31185
		private float pauseTime;

		// Token: 0x040079D2 RID: 31186
		private float disconnectTime = 120f;

		// Token: 0x040079D3 RID: 31187
		public bool disableAFKKick;

		// Token: 0x040079D4 RID: 31188
		private float headRightHandDistance;

		// Token: 0x040079D5 RID: 31189
		private float headLeftHandDistance;

		// Token: 0x040079D6 RID: 31190
		private Quaternion headQuat;

		// Token: 0x040079D7 RID: 31191
		private Quaternion lastHeadQuat;

		// Token: 0x040079D8 RID: 31192
		public GameObject[] disableOnStartup;

		// Token: 0x040079D9 RID: 31193
		public GameObject[] enableOnStartup;

		// Token: 0x040079DA RID: 31194
		public bool updatedName;

		// Token: 0x040079DB RID: 31195
		private int[] playersInRegion;

		// Token: 0x040079DC RID: 31196
		private int[] pingInRegion;

		// Token: 0x040079DD RID: 31197
		private List<string> friendIDList = new List<string>();

		// Token: 0x040079DE RID: 31198
		private JoinType currentJoinType;

		// Token: 0x040079DF RID: 31199
		private string friendToFollow;

		// Token: 0x040079E0 RID: 31200
		private string keyToFollow;

		// Token: 0x040079E1 RID: 31201
		public string shuffler;

		// Token: 0x040079E2 RID: 31202
		public string keyStr;

		// Token: 0x040079E3 RID: 31203
		private string platformTag = "OTHER";

		// Token: 0x040079E4 RID: 31204
		private string startLevel;

		// Token: 0x040079E5 RID: 31205
		[SerializeField]
		private GTZone startZone;

		// Token: 0x040079E6 RID: 31206
		private GorillaGeoHideShowTrigger startGeoTrigger;

		// Token: 0x040079E7 RID: 31207
		public GorillaNetworkJoinTrigger privateTrigger;

		// Token: 0x040079E8 RID: 31208
		internal string initialGameMode = "";

		// Token: 0x040079E9 RID: 31209
		public GorillaNetworkJoinTrigger currentJoinTrigger;

		// Token: 0x040079EA RID: 31210
		public string autoJoinRoom;

		// Token: 0x040079EB RID: 31211
		public int autoJoinRoomCap = 18;

		// Token: 0x040079EC RID: 31212
		public string autoJoinGameMode;

		// Token: 0x040079ED RID: 31213
		private bool deferredJoin;

		// Token: 0x040079EE RID: 31214
		private float partyJoinDeferredUntilTimestamp;

		// Token: 0x040079EF RID: 31215
		private DateTime? timeWhenApplicationPaused;

		// Token: 0x040079F0 RID: 31216
		[NetworkPrefab]
		[SerializeField]
		private NetworkObject testPlayerPrefab;

		// Token: 0x040079F1 RID: 31217
		private string roomToJoin = "";

		// Token: 0x040079F2 RID: 31218
		private int joinNextAttempt;

		// Token: 0x040079F3 RID: 31219
		private int maxNextAttempts = 10;

		// Token: 0x040079F4 RID: 31220
		private string LastRoomToJoin = "";

		// Token: 0x040079F5 RID: 31221
		private List<GorillaNetworkJoinTrigger> allJoinTriggers = new List<GorillaNetworkJoinTrigger>();
	}
}
