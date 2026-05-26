using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using GorillaNetworking;
using GorillaTag;
using Photon.Realtime;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;

// Token: 0x0200045B RID: 1115
public abstract class NetworkSystem : MonoBehaviour
{
	// Token: 0x170002BC RID: 700
	// (get) Token: 0x06001AB8 RID: 6840 RVA: 0x00093F7B File Offset: 0x0009217B
	// (set) Token: 0x06001AB9 RID: 6841 RVA: 0x00093F83 File Offset: 0x00092183
	public bool groupJoinInProgress { get; protected set; }

	// Token: 0x170002BD RID: 701
	// (get) Token: 0x06001ABA RID: 6842 RVA: 0x00093F8C File Offset: 0x0009218C
	// (set) Token: 0x06001ABB RID: 6843 RVA: 0x00093F94 File Offset: 0x00092194
	public NetSystemState netState
	{
		get
		{
			return this.testState;
		}
		protected set
		{
			Debug.Log("netstate set to:" + value.ToString());
			this.testState = value;
		}
	}

	// Token: 0x170002BE RID: 702
	// (get) Token: 0x06001ABC RID: 6844 RVA: 0x00093FB9 File Offset: 0x000921B9
	public NetPlayer LocalPlayer
	{
		get
		{
			return this.netPlayerCache.Find((NetPlayer p) => p.IsLocal);
		}
	}

	// Token: 0x170002BF RID: 703
	// (get) Token: 0x06001ABD RID: 6845 RVA: 0x00093FE5 File Offset: 0x000921E5
	public virtual bool IsMasterClient { get; }

	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x06001ABE RID: 6846 RVA: 0x00093FED File Offset: 0x000921ED
	public virtual NetPlayer MasterClient
	{
		get
		{
			return this.netPlayerCache.Find((NetPlayer p) => p.IsMasterClient);
		}
	}

	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x06001ABF RID: 6847 RVA: 0x00094019 File Offset: 0x00092219
	public Recorder LocalRecorder
	{
		get
		{
			return this.localRecorder;
		}
	}

	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x06001AC0 RID: 6848 RVA: 0x00094021 File Offset: 0x00092221
	public Speaker LocalSpeaker
	{
		get
		{
			return this.localSpeaker;
		}
	}

	// Token: 0x06001AC1 RID: 6849 RVA: 0x00094029 File Offset: 0x00092229
	protected void JoinedNetworkRoom()
	{
		VRRigCache.Instance.OnJoinedRoom();
		DelegateListProcessor onJoinedRoomEvent = this.OnJoinedRoomEvent;
		if (onJoinedRoomEvent == null)
		{
			return;
		}
		onJoinedRoomEvent.InvokeSafe();
	}

	// Token: 0x06001AC2 RID: 6850 RVA: 0x00094045 File Offset: 0x00092245
	internal void MultiplayerStarted()
	{
		DelegateListProcessor onMultiplayerStarted = this.OnMultiplayerStarted;
		if (onMultiplayerStarted == null)
		{
			return;
		}
		onMultiplayerStarted.InvokeSafe();
	}

	// Token: 0x06001AC3 RID: 6851 RVA: 0x00094057 File Offset: 0x00092257
	internal void PreLeavingRoom()
	{
		DelegateListProcessor onPreLeavingRoom = this.OnPreLeavingRoom;
		if (onPreLeavingRoom == null)
		{
			return;
		}
		onPreLeavingRoom.InvokeSafe();
	}

	// Token: 0x06001AC4 RID: 6852 RVA: 0x0009406C File Offset: 0x0009226C
	protected void SinglePlayerStarted()
	{
		try
		{
			DelegateListProcessor onReturnedToSinglePlayer = this.OnReturnedToSinglePlayer;
			if (onReturnedToSinglePlayer != null)
			{
				onReturnedToSinglePlayer.InvokeSafe();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		VRRigCache.Instance.OnLeftRoom();
	}

	// Token: 0x06001AC5 RID: 6853 RVA: 0x000940B0 File Offset: 0x000922B0
	protected void PlayerJoined(NetPlayer netPlayer)
	{
		if (this.IsOnline)
		{
			VRRigCache.Instance.OnPlayerEnteredRoom(netPlayer);
			DelegateListProcessor<NetPlayer> onPlayerJoined = this.OnPlayerJoined;
			if (onPlayerJoined == null)
			{
				return;
			}
			onPlayerJoined.InvokeSafe(netPlayer);
		}
	}

	// Token: 0x06001AC6 RID: 6854 RVA: 0x000940D8 File Offset: 0x000922D8
	protected void PlayerLeft(NetPlayer netPlayer)
	{
		try
		{
			DelegateListProcessor<NetPlayer> onPlayerLeft = this.OnPlayerLeft;
			if (onPlayerLeft != null)
			{
				onPlayerLeft.InvokeSafe(netPlayer);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		VRRigCache.Instance.OnPlayerLeftRoom(netPlayer);
	}

	// Token: 0x06001AC7 RID: 6855 RVA: 0x0009411C File Offset: 0x0009231C
	protected void OnMasterClientSwitchedCallback(NetPlayer nMaster)
	{
		DelegateListProcessor<NetPlayer> onMasterClientSwitchedEvent = this.OnMasterClientSwitchedEvent;
		if (onMasterClientSwitchedEvent == null)
		{
			return;
		}
		onMasterClientSwitchedEvent.InvokeSafe(nMaster);
	}

	// Token: 0x1400003A RID: 58
	// (add) Token: 0x06001AC8 RID: 6856 RVA: 0x00094130 File Offset: 0x00092330
	// (remove) Token: 0x06001AC9 RID: 6857 RVA: 0x00094168 File Offset: 0x00092368
	public event Action<byte, object, int> OnRaiseEvent;

	// Token: 0x06001ACA RID: 6858 RVA: 0x0009419D File Offset: 0x0009239D
	internal void RaiseEvent(byte eventCode, object data, int source)
	{
		Action<byte, object, int> onRaiseEvent = this.OnRaiseEvent;
		if (onRaiseEvent == null)
		{
			return;
		}
		onRaiseEvent(eventCode, data, source);
	}

	// Token: 0x1400003B RID: 59
	// (add) Token: 0x06001ACB RID: 6859 RVA: 0x000941B4 File Offset: 0x000923B4
	// (remove) Token: 0x06001ACC RID: 6860 RVA: 0x000941EC File Offset: 0x000923EC
	public event Action<Dictionary<string, object>> OnCustomAuthenticationResponse;

	// Token: 0x06001ACD RID: 6861 RVA: 0x00094221 File Offset: 0x00092421
	internal void CustomAuthenticationResponse(Dictionary<string, object> response)
	{
		Action<Dictionary<string, object>> onCustomAuthenticationResponse = this.OnCustomAuthenticationResponse;
		if (onCustomAuthenticationResponse == null)
		{
			return;
		}
		onCustomAuthenticationResponse(response);
	}

	// Token: 0x06001ACE RID: 6862 RVA: 0x00094234 File Offset: 0x00092434
	public virtual void Initialise()
	{
		Debug.Log("INITIALISING NETWORKSYSTEMS");
		if (NetworkSystem.Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		NetworkSystem.Instance = this;
		NetCrossoverUtils.Prewarm();
	}

	// Token: 0x06001ACF RID: 6863 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void Update()
	{
	}

	// Token: 0x06001AD0 RID: 6864 RVA: 0x00094263 File Offset: 0x00092463
	public void RegisterSceneNetworkItem(GameObject item)
	{
		if (!this.SceneObjectsToAttach.Contains(item))
		{
			this.SceneObjectsToAttach.Add(item);
		}
	}

	// Token: 0x06001AD1 RID: 6865 RVA: 0x0009427F File Offset: 0x0009247F
	public virtual void AttachObjectInGame(GameObject item)
	{
		this.RegisterSceneNetworkItem(item);
	}

	// Token: 0x06001AD2 RID: 6866 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void DetatchSceneObjectInGame(GameObject item)
	{
	}

	// Token: 0x06001AD3 RID: 6867 RVA: 0x00094288 File Offset: 0x00092488
	public virtual AuthenticationValues GetAuthenticationValues()
	{
		Debug.LogWarning("NetworkSystem.GetAuthenticationValues should be overridden");
		return new AuthenticationValues();
	}

	// Token: 0x06001AD4 RID: 6868 RVA: 0x00094299 File Offset: 0x00092499
	public virtual void SetAuthenticationValues(AuthenticationValues authValues)
	{
		Debug.LogWarning("NetworkSystem.SetAuthenticationValues should be overridden");
	}

	// Token: 0x06001AD5 RID: 6869
	public abstract void FinishAuthenticating();

	// Token: 0x06001AD6 RID: 6870
	public abstract Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1);

	// Token: 0x06001AD7 RID: 6871
	public abstract Task JoinFriendsRoom(string userID, int actorID, string keyToFollow, string shufflerToFollow);

	// Token: 0x06001AD8 RID: 6872
	public abstract Task ReturnToSinglePlayer();

	// Token: 0x06001AD9 RID: 6873
	public abstract void JoinPubWithFriends();

	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x06001ADA RID: 6874 RVA: 0x000942A5 File Offset: 0x000924A5
	public bool WrongVersion
	{
		get
		{
			return this.isWrongVersion;
		}
	}

	// Token: 0x06001ADB RID: 6875 RVA: 0x000942AD File Offset: 0x000924AD
	public void SetWrongVersion()
	{
		this.isWrongVersion = true;
	}

	// Token: 0x06001ADC RID: 6876 RVA: 0x000942B6 File Offset: 0x000924B6
	public GameObject NetInstantiate(GameObject prefab, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, Vector3.zero, Quaternion.identity, false);
	}

	// Token: 0x06001ADD RID: 6877 RVA: 0x000942CA File Offset: 0x000924CA
	public GameObject NetInstantiate(GameObject prefab, Vector3 position, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, position, Quaternion.identity, false);
	}

	// Token: 0x06001ADE RID: 6878
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false);

	// Token: 0x06001ADF RID: 6879
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false);

	// Token: 0x06001AE0 RID: 6880
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null);

	// Token: 0x06001AE1 RID: 6881
	public abstract void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null);

	// Token: 0x06001AE2 RID: 6882
	public abstract void NetDestroy(GameObject instance);

	// Token: 0x06001AE3 RID: 6883
	public abstract void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true);

	// Token: 0x06001AE4 RID: 6884
	public abstract void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true) where T : struct;

	// Token: 0x06001AE5 RID: 6885
	public abstract void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true);

	// Token: 0x06001AE6 RID: 6886
	public abstract void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod);

	// Token: 0x06001AE7 RID: 6887
	public abstract void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args) where T : struct;

	// Token: 0x06001AE8 RID: 6888
	public abstract void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message);

	// Token: 0x06001AE9 RID: 6889 RVA: 0x000942DC File Offset: 0x000924DC
	public static string GetRandomRoomName()
	{
		string text = "";
		for (int i = 0; i < 4; i++)
		{
			text += "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Length), 1);
		}
		if (GorillaComputer.instance.IsPlayerInVirtualStump())
		{
			text = GorillaComputer.instance.VStumpRoomPrepend + text;
		}
		if (GorillaComputer.instance.CheckAutoBanListForName(text))
		{
			return text;
		}
		return NetworkSystem.GetRandomRoomName();
	}

	// Token: 0x06001AEA RID: 6890
	public abstract string GetRandomWeightedRegion();

	// Token: 0x06001AEB RID: 6891 RVA: 0x00094354 File Offset: 0x00092554
	protected Task RefreshNonce()
	{
		NetworkSystem.<RefreshNonce>d__91 <RefreshNonce>d__;
		<RefreshNonce>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RefreshNonce>d__.<>4__this = this;
		<RefreshNonce>d__.<>1__state = -1;
		<RefreshNonce>d__.<>t__builder.Start<NetworkSystem.<RefreshNonce>d__91>(ref <RefreshNonce>d__);
		return <RefreshNonce>d__.<>t__builder.Task;
	}

	// Token: 0x06001AEC RID: 6892 RVA: 0x00094398 File Offset: 0x00092598
	private void GetSteamAuthTicketSuccessCallback(string ticket)
	{
		AuthenticationValues authenticationValues = this.GetAuthenticationValues();
		Dictionary<string, object> dictionary = ((authenticationValues != null) ? authenticationValues.AuthPostData : null) as Dictionary<string, object>;
		if (dictionary != null)
		{
			dictionary["Nonce"] = ticket;
			authenticationValues.SetAuthPostData(dictionary);
			this.SetAuthenticationValues(authenticationValues);
			this.nonceRefreshed = true;
		}
	}

	// Token: 0x06001AED RID: 6893 RVA: 0x000943E2 File Offset: 0x000925E2
	private void GetSteamAuthTicketFailureCallback(EResult result)
	{
		base.StartCoroutine(this.ReGetNonce());
	}

	// Token: 0x06001AEE RID: 6894 RVA: 0x000943F1 File Offset: 0x000925F1
	private IEnumerator ReGetNonce()
	{
		yield return new WaitForSecondsRealtime(3f);
		PlayFabAuthenticator.instance.RefreshSteamAuthTicketForPhoton(new Action<string>(this.GetSteamAuthTicketSuccessCallback), new Action<EResult>(this.GetSteamAuthTicketFailureCallback));
		yield return null;
		yield break;
	}

	// Token: 0x06001AEF RID: 6895 RVA: 0x00094400 File Offset: 0x00092600
	public void BroadcastMyRoom(bool create, string key, string shuffler)
	{
		string roomToJoin = NetworkSystem.ShuffleRoomName(NetworkSystem.Instance.RoomName, shuffler.Substring(2, 8), true) + "|" + NetworkSystem.ShuffleRoomName("ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(NetworkSystem.Instance.currentRegionIndex, 1), shuffler.Substring(0, 2), true);
		GorillaServer instance = GorillaServer.Instance;
		BroadcastMyRoomRequest broadcastMyRoomRequest = new BroadcastMyRoomRequest();
		broadcastMyRoomRequest.KeyToFollow = key;
		broadcastMyRoomRequest.RoomToJoin = roomToJoin;
		broadcastMyRoomRequest.Set = create;
		instance.BroadcastMyRoom(broadcastMyRoomRequest, delegate(ExecuteFunctionResult result)
		{
		}, delegate(PlayFabError error)
		{
		});
	}

	// Token: 0x06001AF0 RID: 6896 RVA: 0x000944B8 File Offset: 0x000926B8
	public bool InstantCheckGroupData(string userID, string keyToFollow)
	{
		bool success = false;
		PlayFab.ClientModels.GetSharedGroupDataRequest getSharedGroupDataRequest = new PlayFab.ClientModels.GetSharedGroupDataRequest();
		getSharedGroupDataRequest.Keys = new List<string>
		{
			keyToFollow
		};
		getSharedGroupDataRequest.SharedGroupId = userID;
		PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, delegate(GetSharedGroupDataResult result)
		{
			if (result.Data.Count > 0)
			{
				success = true;
				return;
			}
		}, delegate(PlayFabError error)
		{
		}, null, null);
		return success;
	}

	// Token: 0x06001AF1 RID: 6897 RVA: 0x00094528 File Offset: 0x00092728
	public NetPlayer GetNetPlayerByID(int playerActorNumber)
	{
		return this.netPlayerCache.Find((NetPlayer a) => a.ActorNumber == playerActorNumber);
	}

	// Token: 0x06001AF2 RID: 6898 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void NetRaiseEventReliable(byte eventCode, object data)
	{
	}

	// Token: 0x06001AF3 RID: 6899 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void NetRaiseEventUnreliable(byte eventCode, object data)
	{
	}

	// Token: 0x06001AF4 RID: 6900 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void NetRaiseEventReliable(byte eventCode, object data, NetEventOptions options)
	{
	}

	// Token: 0x06001AF5 RID: 6901 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void NetRaiseEventUnreliable(byte eventCode, object data, NetEventOptions options)
	{
	}

	// Token: 0x06001AF6 RID: 6902 RVA: 0x0009455C File Offset: 0x0009275C
	public static string ShuffleRoomName(string room, string shuffle, bool encode)
	{
		NetworkSystem.shuffleStringBuilder.Clear();
		int num;
		if (!int.TryParse(shuffle, out num))
		{
			Debug.Log("Shuffle room failed");
			return "";
		}
		for (int i = 0; i < room.Length; i++)
		{
			int num2 = int.Parse(shuffle.Substring(i * 2 % (shuffle.Length - 1), 2));
			int index = NetworkSystem.mod("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".IndexOf(room[i]) + (encode ? num2 : (-num2)), "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Length);
			NetworkSystem.shuffleStringBuilder.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"[index]);
		}
		return NetworkSystem.shuffleStringBuilder.ToString();
	}

	// Token: 0x06001AF7 RID: 6903 RVA: 0x0004BCD1 File Offset: 0x00049ED1
	public static int mod(int x, int m)
	{
		return (x % m + m) % m;
	}

	// Token: 0x06001AF8 RID: 6904
	public abstract Task AwaitSceneReady();

	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x06001AF9 RID: 6905
	public abstract string CurrentPhotonBackend { get; }

	// Token: 0x06001AFA RID: 6906
	public abstract NetPlayer GetLocalPlayer();

	// Token: 0x06001AFB RID: 6907
	public abstract NetPlayer GetPlayer(int PlayerID);

	// Token: 0x06001AFC RID: 6908 RVA: 0x00094604 File Offset: 0x00092804
	public NetPlayer GetPlayer(Player punPlayer)
	{
		if (punPlayer == null)
		{
			return null;
		}
		NetPlayer netPlayer = this.FindPlayer(punPlayer);
		if (netPlayer == null)
		{
			this.UpdatePlayers();
			netPlayer = this.FindPlayer(punPlayer);
			if (netPlayer == null)
			{
				Debug.LogError(string.Format("There is no NetPlayer with this ID currently in game. Passed ID: {0} nickname {1}", punPlayer.ActorNumber, punPlayer.NickName));
				return null;
			}
		}
		return netPlayer;
	}

	// Token: 0x06001AFD RID: 6909 RVA: 0x00094658 File Offset: 0x00092858
	private NetPlayer FindPlayer(Player punPlayer)
	{
		for (int i = 0; i < this.netPlayerCache.Count; i++)
		{
			if (this.netPlayerCache[i].GetPlayerRef() == punPlayer)
			{
				return this.netPlayerCache[i];
			}
		}
		return null;
	}

	// Token: 0x06001AFE RID: 6910 RVA: 0x00035D0D File Offset: 0x00033F0D
	public NetPlayer GetPlayer(PlayerRef playerRef)
	{
		return null;
	}

	// Token: 0x06001AFF RID: 6911
	public abstract void SetMyNickName(string name);

	// Token: 0x06001B00 RID: 6912
	public abstract string GetMyNickName();

	// Token: 0x06001B01 RID: 6913
	public abstract string GetMyDefaultName();

	// Token: 0x06001B02 RID: 6914
	public abstract string GetNickName(int playerID);

	// Token: 0x06001B03 RID: 6915
	public abstract string GetNickName(NetPlayer player);

	// Token: 0x06001B04 RID: 6916
	public abstract string GetMyUserID();

	// Token: 0x06001B05 RID: 6917
	public abstract string GetUserID(int playerID);

	// Token: 0x06001B06 RID: 6918
	public abstract string GetUserID(NetPlayer player);

	// Token: 0x06001B07 RID: 6919
	public abstract void SetMyTutorialComplete();

	// Token: 0x06001B08 RID: 6920
	public abstract bool GetMyTutorialCompletion();

	// Token: 0x06001B09 RID: 6921
	public abstract bool GetPlayerTutorialCompletion(int playerID);

	// Token: 0x06001B0A RID: 6922 RVA: 0x0009469D File Offset: 0x0009289D
	public void AddVoiceSettings(SO_NetworkVoiceSettings settings)
	{
		this.VoiceSettings = settings;
	}

	// Token: 0x06001B0B RID: 6923
	public abstract void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback);

	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x06001B0C RID: 6924
	public abstract VoiceConnection VoiceConnection { get; }

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x06001B0D RID: 6925
	public abstract bool IsOnline { get; }

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x06001B0E RID: 6926
	public abstract bool InRoom { get; }

	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x06001B0F RID: 6927
	public abstract string RoomName { get; }

	// Token: 0x06001B10 RID: 6928
	public abstract string RoomStringStripped();

	// Token: 0x06001B11 RID: 6929 RVA: 0x000946A8 File Offset: 0x000928A8
	public string RoomString()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", new object[]
		{
			this.RoomName,
			this.CurrentRoom.isPublic ? "visible" : "hidden",
			this.CurrentRoom.isJoinable ? "open" : "closed",
			this.CurrentRoom.MaxPlayers,
			this.RoomPlayerCount,
			this.CurrentRoom.CustomProps.ToStringFull()
		});
	}

	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x06001B12 RID: 6930
	public abstract string GameModeString { get; }

	// Token: 0x170002CA RID: 714
	// (get) Token: 0x06001B13 RID: 6931
	public abstract string CurrentRegion { get; }

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x06001B14 RID: 6932
	public abstract bool SessionIsPrivate { get; }

	// Token: 0x170002CC RID: 716
	// (get) Token: 0x06001B15 RID: 6933
	public abstract bool SessionIsSubscription { get; }

	// Token: 0x170002CD RID: 717
	// (get) Token: 0x06001B16 RID: 6934
	public abstract int LocalPlayerID { get; }

	// Token: 0x170002CE RID: 718
	// (get) Token: 0x06001B17 RID: 6935 RVA: 0x0009473A File Offset: 0x0009293A
	public virtual NetPlayer[] AllNetPlayers
	{
		get
		{
			return this.netPlayerCache.ToArray();
		}
	}

	// Token: 0x170002CF RID: 719
	// (get) Token: 0x06001B18 RID: 6936 RVA: 0x00094747 File Offset: 0x00092947
	public virtual NetPlayer[] PlayerListOthers
	{
		get
		{
			return this.netPlayerCache.FindAll((NetPlayer p) => !p.IsLocal).ToArray();
		}
	}

	// Token: 0x06001B19 RID: 6937
	protected abstract void UpdateNetPlayerList();

	// Token: 0x06001B1A RID: 6938 RVA: 0x00094778 File Offset: 0x00092978
	public void UpdatePlayers()
	{
		this.UpdateNetPlayerList();
	}

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x06001B1B RID: 6939
	public abstract double SimTime { get; }

	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x06001B1C RID: 6940
	public abstract float SimDeltaTime { get; }

	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x06001B1D RID: 6941
	public abstract int SimTick { get; }

	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x06001B1E RID: 6942
	public abstract int TickRate { get; }

	// Token: 0x170002D4 RID: 724
	// (get) Token: 0x06001B1F RID: 6943
	public abstract int ServerTimestamp { get; }

	// Token: 0x170002D5 RID: 725
	// (get) Token: 0x06001B20 RID: 6944
	public abstract int RoomPlayerCount { get; }

	// Token: 0x06001B21 RID: 6945
	public abstract int GlobalPlayerCount();

	// Token: 0x170002D6 RID: 726
	// (get) Token: 0x06001B22 RID: 6946 RVA: 0x00094780 File Offset: 0x00092980
	// (set) Token: 0x06001B23 RID: 6947 RVA: 0x00094788 File Offset: 0x00092988
	public RoomConfig CurrentRoom { get; protected set; }

	// Token: 0x06001B24 RID: 6948
	public abstract bool IsObjectLocallyOwned(GameObject obj);

	// Token: 0x06001B25 RID: 6949
	public abstract bool IsObjectRoomObject(GameObject obj);

	// Token: 0x06001B26 RID: 6950
	public abstract bool ShouldUpdateObject(GameObject obj);

	// Token: 0x06001B27 RID: 6951
	public abstract bool ShouldWriteObjectData(GameObject obj);

	// Token: 0x06001B28 RID: 6952
	public abstract int GetOwningPlayerID(GameObject obj);

	// Token: 0x06001B29 RID: 6953
	public abstract bool ShouldSpawnLocally(int playerID);

	// Token: 0x06001B2A RID: 6954
	public abstract bool IsTotalAuthority();

	// Token: 0x04002551 RID: 9553
	public static NetworkSystem Instance;

	// Token: 0x04002552 RID: 9554
	public NetworkSystemConfig config;

	// Token: 0x04002553 RID: 9555
	public bool changingSceneManually;

	// Token: 0x04002554 RID: 9556
	public string[] regionNames;

	// Token: 0x04002555 RID: 9557
	public int currentRegionIndex;

	// Token: 0x04002557 RID: 9559
	private bool nonceRefreshed;

	// Token: 0x04002558 RID: 9560
	protected bool isWrongVersion;

	// Token: 0x04002559 RID: 9561
	private NetSystemState testState;

	// Token: 0x0400255A RID: 9562
	protected List<NetPlayer> netPlayerCache = new List<NetPlayer>();

	// Token: 0x0400255B RID: 9563
	protected Recorder localRecorder;

	// Token: 0x0400255C RID: 9564
	protected Speaker localSpeaker;

	// Token: 0x0400255E RID: 9566
	public List<GameObject> SceneObjectsToAttach = new List<GameObject>();

	// Token: 0x0400255F RID: 9567
	protected SO_NetworkVoiceSettings VoiceSettings;

	// Token: 0x04002560 RID: 9568
	protected List<Action<RemoteVoiceLink>> remoteVoiceAddedCallbacks = new List<Action<RemoteVoiceLink>>();

	// Token: 0x04002561 RID: 9569
	public DelegateListProcessor OnJoinedRoomEvent = new DelegateListProcessor();

	// Token: 0x04002562 RID: 9570
	public DelegateListProcessor OnMultiplayerStarted = new DelegateListProcessor();

	// Token: 0x04002563 RID: 9571
	public DelegateListProcessor OnReturnedToSinglePlayer = new DelegateListProcessor();

	// Token: 0x04002564 RID: 9572
	public DelegateListProcessor OnPreLeavingRoom = new DelegateListProcessor();

	// Token: 0x04002565 RID: 9573
	public DelegateListProcessor<NetPlayer> OnPlayerJoined = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04002566 RID: 9574
	public DelegateListProcessor<NetPlayer> OnPlayerLeft = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04002567 RID: 9575
	internal DelegateListProcessor<NetPlayer> OnMasterClientSwitchedEvent = new DelegateListProcessor<NetPlayer>();

	// Token: 0x0400256A RID: 9578
	protected static readonly byte[] EmptyArgs = new byte[0];

	// Token: 0x0400256B RID: 9579
	public const string roomCharacters = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";

	// Token: 0x0400256C RID: 9580
	public const string shuffleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

	// Token: 0x0400256D RID: 9581
	private static StringBuilder shuffleStringBuilder = new StringBuilder(4);

	// Token: 0x0400256E RID: 9582
	protected static StringBuilder reusableSB = new StringBuilder();

	// Token: 0x0400256F RID: 9583
	[NonSerialized]
	public string groupJoinOverrideGameMode = "";

	// Token: 0x0200045C RID: 1116
	// (Invoke) Token: 0x06001B2E RID: 6958
	public delegate void RPC(byte[] data);

	// Token: 0x0200045D RID: 1117
	// (Invoke) Token: 0x06001B32 RID: 6962
	public delegate void StringRPC(string message);

	// Token: 0x0200045E RID: 1118
	// (Invoke) Token: 0x06001B36 RID: 6966
	public delegate void StaticRPC(byte[] data);

	// Token: 0x0200045F RID: 1119
	// (Invoke) Token: 0x06001B3A RID: 6970
	public delegate void StaticRPCPlaceholder(byte[] args);
}
