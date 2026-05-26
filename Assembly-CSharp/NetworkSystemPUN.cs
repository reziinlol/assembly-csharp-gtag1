using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Fusion;
using GorillaTag;
using GorillaTag.Audio;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x0200046D RID: 1133
[RequireComponent(typeof(PUNCallbackNotifier))]
public class NetworkSystemPUN : NetworkSystem
{
	// Token: 0x170002E5 RID: 741
	// (get) Token: 0x06001B84 RID: 7044 RVA: 0x0009515D File Offset: 0x0009335D
	public override NetPlayer[] AllNetPlayers
	{
		get
		{
			return this.m_allNetPlayers;
		}
	}

	// Token: 0x170002E6 RID: 742
	// (get) Token: 0x06001B85 RID: 7045 RVA: 0x00095165 File Offset: 0x00093365
	public override NetPlayer[] PlayerListOthers
	{
		get
		{
			return this.m_otherNetPlayers;
		}
	}

	// Token: 0x170002E7 RID: 743
	// (get) Token: 0x06001B86 RID: 7046 RVA: 0x0009516D File Offset: 0x0009336D
	public override VoiceConnection VoiceConnection
	{
		get
		{
			return this.punVoice;
		}
	}

	// Token: 0x170002E8 RID: 744
	// (get) Token: 0x06001B87 RID: 7047 RVA: 0x00095178 File Offset: 0x00093378
	private int lowestPingRegionIndex
	{
		get
		{
			int num = 9999;
			int result = -1;
			for (int i = 0; i < this.regionData.Length; i++)
			{
				if (this.regionData[i].pingToRegion < num)
				{
					num = this.regionData[i].pingToRegion;
					result = i;
				}
			}
			return result;
		}
	}

	// Token: 0x170002E9 RID: 745
	// (get) Token: 0x06001B88 RID: 7048 RVA: 0x000951C1 File Offset: 0x000933C1
	// (set) Token: 0x06001B89 RID: 7049 RVA: 0x000951C9 File Offset: 0x000933C9
	private NetworkSystemPUN.InternalState internalState
	{
		get
		{
			return this.currentState;
		}
		set
		{
			this.currentState = value;
		}
	}

	// Token: 0x170002EA RID: 746
	// (get) Token: 0x06001B8A RID: 7050 RVA: 0x000951D2 File Offset: 0x000933D2
	public override string CurrentPhotonBackend
	{
		get
		{
			return "PUN";
		}
	}

	// Token: 0x170002EB RID: 747
	// (get) Token: 0x06001B8B RID: 7051 RVA: 0x000951D9 File Offset: 0x000933D9
	public override bool IsOnline
	{
		get
		{
			return this.InRoom;
		}
	}

	// Token: 0x170002EC RID: 748
	// (get) Token: 0x06001B8C RID: 7052 RVA: 0x000951E1 File Offset: 0x000933E1
	public override bool InRoom
	{
		get
		{
			return PhotonNetwork.InRoom;
		}
	}

	// Token: 0x170002ED RID: 749
	// (get) Token: 0x06001B8D RID: 7053 RVA: 0x000951E8 File Offset: 0x000933E8
	public override string RoomName
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return ((currentRoom != null) ? currentRoom.Name : null) ?? string.Empty;
		}
	}

	// Token: 0x06001B8E RID: 7054 RVA: 0x00095204 File Offset: 0x00093404
	public override string RoomStringStripped()
	{
		Room currentRoom = PhotonNetwork.CurrentRoom;
		NetworkSystem.reusableSB.Clear();
		NetworkSystem.reusableSB.AppendFormat("Room: '{0}' ", (currentRoom.Name.Length < 20) ? currentRoom.Name : currentRoom.Name.Remove(20));
		NetworkSystem.reusableSB.AppendFormat("{0},{1} {3}/{2} players.", new object[]
		{
			currentRoom.IsVisible ? "visible" : "hidden",
			currentRoom.IsOpen ? "open" : "closed",
			currentRoom.MaxPlayers,
			currentRoom.PlayerCount
		});
		NetworkSystem.reusableSB.Append("\ncustomProps: {");
		NetworkSystem.reusableSB.AppendFormat("joinedGameMode={0}, ", (RoomSystem.RoomGameMode.Length < 50) ? RoomSystem.RoomGameMode : RoomSystem.RoomGameMode.Remove(50));
		IDictionary customProperties = currentRoom.CustomProperties;
		this.AppendStringFromDict(customProperties, "gameMode", 50, NetworkSystem.reusableSB);
		NetworkSystem.reusableSB.Append(", ");
		this.AppendStringFromDict(customProperties, "platform", 10, NetworkSystem.reusableSB);
		NetworkSystem.reusableSB.Append(", ");
		this.AppendStringFromDict(customProperties, "queueName", 15, NetworkSystem.reusableSB);
		NetworkSystem.reusableSB.Append(", ");
		this.AppendStringFromDict(customProperties, "language", 15, NetworkSystem.reusableSB);
		NetworkSystem.reusableSB.Append(", ");
		this.AppendStringFromDict(customProperties, "fan_club", 6, NetworkSystem.reusableSB);
		NetworkSystem.reusableSB.Append(", ");
		this.AppendStringFromDict(customProperties, "mmrTier", 8, NetworkSystem.reusableSB);
		NetworkSystem.reusableSB.Append("}");
		return NetworkSystem.reusableSB.ToString();
	}

	// Token: 0x06001B8F RID: 7055 RVA: 0x000953D8 File Offset: 0x000935D8
	private void AppendStringFromDict(IDictionary dict, string key, int maxStrLen, StringBuilder sb)
	{
		sb.AppendFormat("{0}=", key);
		if (dict.Contains(key))
		{
			string text = dict[key] as string;
			if (text != null)
			{
				sb.Append((text.Length < maxStrLen) ? text : text.Remove(maxStrLen));
				return;
			}
		}
		sb.Append("null");
	}

	// Token: 0x170002EE RID: 750
	// (get) Token: 0x06001B90 RID: 7056 RVA: 0x00095438 File Offset: 0x00093638
	public override string GameModeString
	{
		get
		{
			object obj;
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj);
			if (obj != null)
			{
				return obj.ToString();
			}
			return null;
		}
	}

	// Token: 0x170002EF RID: 751
	// (get) Token: 0x06001B91 RID: 7057 RVA: 0x00095467 File Offset: 0x00093667
	public override string CurrentRegion
	{
		get
		{
			return PhotonNetwork.CloudRegion;
		}
	}

	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x06001B92 RID: 7058 RVA: 0x0009546E File Offset: 0x0009366E
	public override bool SessionIsPrivate
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return currentRoom != null && !currentRoom.IsVisible;
		}
	}

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06001B93 RID: 7059 RVA: 0x00095484 File Offset: 0x00093684
	public override bool SessionIsSubscription
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			byte? b = (currentRoom != null) ? new byte?(currentRoom.MaxPlayers) : null;
			int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
			int num2 = 10;
			return num.GetValueOrDefault() > num2 & num != null;
		}
	}

	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x06001B94 RID: 7060 RVA: 0x000954E7 File Offset: 0x000936E7
	public override int LocalPlayerID
	{
		get
		{
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x06001B95 RID: 7061 RVA: 0x000954F3 File Offset: 0x000936F3
	public override int ServerTimestamp
	{
		get
		{
			return PhotonNetwork.ServerTimestamp;
		}
	}

	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x06001B96 RID: 7062 RVA: 0x000954FA File Offset: 0x000936FA
	public override double SimTime
	{
		get
		{
			return PhotonNetwork.Time;
		}
	}

	// Token: 0x170002F5 RID: 757
	// (get) Token: 0x06001B97 RID: 7063 RVA: 0x00095501 File Offset: 0x00093701
	public override float SimDeltaTime
	{
		get
		{
			return Time.deltaTime;
		}
	}

	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x06001B98 RID: 7064 RVA: 0x000954F3 File Offset: 0x000936F3
	public override int SimTick
	{
		get
		{
			return PhotonNetwork.ServerTimestamp;
		}
	}

	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x06001B99 RID: 7065 RVA: 0x00095508 File Offset: 0x00093708
	public override int TickRate
	{
		get
		{
			return PhotonNetwork.SerializationRate;
		}
	}

	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x06001B9A RID: 7066 RVA: 0x0009550F File Offset: 0x0009370F
	public override int RoomPlayerCount
	{
		get
		{
			return (int)PhotonNetwork.CurrentRoom.PlayerCount;
		}
	}

	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x06001B9B RID: 7067 RVA: 0x0009551B File Offset: 0x0009371B
	public override bool IsMasterClient
	{
		get
		{
			return PhotonNetwork.IsMasterClient;
		}
	}

	// Token: 0x06001B9C RID: 7068 RVA: 0x00095524 File Offset: 0x00093724
	public override void Initialise()
	{
		NetworkSystemPUN.<Initialise>d__56 <Initialise>d__;
		<Initialise>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialise>d__.<>4__this = this;
		<Initialise>d__.<>1__state = -1;
		<Initialise>d__.<>t__builder.Start<NetworkSystemPUN.<Initialise>d__56>(ref <Initialise>d__);
	}

	// Token: 0x06001B9D RID: 7069 RVA: 0x0009555C File Offset: 0x0009375C
	private Task CacheRegionInfo()
	{
		NetworkSystemPUN.<CacheRegionInfo>d__57 <CacheRegionInfo>d__;
		<CacheRegionInfo>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CacheRegionInfo>d__.<>4__this = this;
		<CacheRegionInfo>d__.<>1__state = -1;
		<CacheRegionInfo>d__.<>t__builder.Start<NetworkSystemPUN.<CacheRegionInfo>d__57>(ref <CacheRegionInfo>d__);
		return <CacheRegionInfo>d__.<>t__builder.Task;
	}

	// Token: 0x06001B9E RID: 7070 RVA: 0x0009559F File Offset: 0x0009379F
	public override AuthenticationValues GetAuthenticationValues()
	{
		return PhotonNetwork.AuthValues;
	}

	// Token: 0x06001B9F RID: 7071 RVA: 0x000955A6 File Offset: 0x000937A6
	public override void SetAuthenticationValues(AuthenticationValues authValues)
	{
		PhotonNetwork.AuthValues = authValues;
	}

	// Token: 0x06001BA0 RID: 7072 RVA: 0x000955B0 File Offset: 0x000937B0
	public override void FinishAuthenticating()
	{
		if (PhotonNetwork.AuthValues == null)
		{
			this._taskCancelTokens.ForEach(delegate(CancellationTokenSource cts)
			{
				cts.Cancel();
				cts.Dispose();
			});
			this._taskCancelTokens.Clear();
			return;
		}
		this.internalState = NetworkSystemPUN.InternalState.Authenticated;
	}

	// Token: 0x06001BA1 RID: 7073 RVA: 0x00095604 File Offset: 0x00093804
	private Task WaitForState(CancellationToken ct, NetworkSystemPUN.InternalState[] desiredStates, float timeout)
	{
		NetworkSystemPUN.<WaitForState>d__61 <WaitForState>d__;
		<WaitForState>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForState>d__.<>4__this = this;
		<WaitForState>d__.ct = ct;
		<WaitForState>d__.desiredStates = desiredStates;
		<WaitForState>d__.timeout = timeout;
		<WaitForState>d__.<>1__state = -1;
		<WaitForState>d__.<>t__builder.Start<NetworkSystemPUN.<WaitForState>d__61>(ref <WaitForState>d__);
		return <WaitForState>d__.<>t__builder.Task;
	}

	// Token: 0x06001BA2 RID: 7074 RVA: 0x00095660 File Offset: 0x00093860
	private Task<bool> WaitForStateCheck(NetworkSystemPUN.InternalState[] desiredStates, float timeout = 10f)
	{
		NetworkSystemPUN.<WaitForStateCheck>d__62 <WaitForStateCheck>d__;
		<WaitForStateCheck>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForStateCheck>d__.<>4__this = this;
		<WaitForStateCheck>d__.desiredStates = desiredStates;
		<WaitForStateCheck>d__.timeout = timeout;
		<WaitForStateCheck>d__.<>1__state = -1;
		<WaitForStateCheck>d__.<>t__builder.Start<NetworkSystemPUN.<WaitForStateCheck>d__62>(ref <WaitForStateCheck>d__);
		return <WaitForStateCheck>d__.<>t__builder.Task;
	}

	// Token: 0x06001BA3 RID: 7075 RVA: 0x000956B3 File Offset: 0x000938B3
	private Task<bool> WaitForStateCheck(NetworkSystemPUN.InternalState desiredState, float timeout = 10f)
	{
		return this.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
		{
			desiredState
		}, timeout);
	}

	// Token: 0x06001BA4 RID: 7076 RVA: 0x000956C8 File Offset: 0x000938C8
	private Task<NetJoinResult> MakeOrFindRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		NetworkSystemPUN.<MakeOrFindRoom>d__64 <MakeOrFindRoom>d__;
		<MakeOrFindRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<MakeOrFindRoom>d__.<>4__this = this;
		<MakeOrFindRoom>d__.roomName = roomName;
		<MakeOrFindRoom>d__.opts = opts;
		<MakeOrFindRoom>d__.regionIndex = regionIndex;
		<MakeOrFindRoom>d__.<>1__state = -1;
		<MakeOrFindRoom>d__.<>t__builder.Start<NetworkSystemPUN.<MakeOrFindRoom>d__64>(ref <MakeOrFindRoom>d__);
		return <MakeOrFindRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001BA5 RID: 7077 RVA: 0x00095724 File Offset: 0x00093924
	private Task<bool> TryJoinRoom(string roomName, RoomConfig opts)
	{
		NetworkSystemPUN.<TryJoinRoom>d__65 <TryJoinRoom>d__;
		<TryJoinRoom>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TryJoinRoom>d__.<>4__this = this;
		<TryJoinRoom>d__.roomName = roomName;
		<TryJoinRoom>d__.opts = opts;
		<TryJoinRoom>d__.<>1__state = -1;
		<TryJoinRoom>d__.<>t__builder.Start<NetworkSystemPUN.<TryJoinRoom>d__65>(ref <TryJoinRoom>d__);
		return <TryJoinRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001BA6 RID: 7078 RVA: 0x00095778 File Offset: 0x00093978
	private Task<bool> TryJoinRoomInRegion(string roomName, RoomConfig opts, int regionIndex)
	{
		NetworkSystemPUN.<TryJoinRoomInRegion>d__66 <TryJoinRoomInRegion>d__;
		<TryJoinRoomInRegion>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TryJoinRoomInRegion>d__.<>4__this = this;
		<TryJoinRoomInRegion>d__.roomName = roomName;
		<TryJoinRoomInRegion>d__.opts = opts;
		<TryJoinRoomInRegion>d__.regionIndex = regionIndex;
		<TryJoinRoomInRegion>d__.<>1__state = -1;
		<TryJoinRoomInRegion>d__.<>t__builder.Start<NetworkSystemPUN.<TryJoinRoomInRegion>d__66>(ref <TryJoinRoomInRegion>d__);
		return <TryJoinRoomInRegion>d__.<>t__builder.Task;
	}

	// Token: 0x06001BA7 RID: 7079 RVA: 0x000957D4 File Offset: 0x000939D4
	private Task<NetJoinResult> TryCreateRoom(string roomName, RoomConfig opts)
	{
		NetworkSystemPUN.<TryCreateRoom>d__67 <TryCreateRoom>d__;
		<TryCreateRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<TryCreateRoom>d__.<>4__this = this;
		<TryCreateRoom>d__.roomName = roomName;
		<TryCreateRoom>d__.opts = opts;
		<TryCreateRoom>d__.<>1__state = -1;
		<TryCreateRoom>d__.<>t__builder.Start<NetworkSystemPUN.<TryCreateRoom>d__67>(ref <TryCreateRoom>d__);
		return <TryCreateRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001BA8 RID: 7080 RVA: 0x00095828 File Offset: 0x00093A28
	private Task<NetJoinResult> JoinRandomPublicRoom(RoomConfig opts)
	{
		NetworkSystemPUN.<JoinRandomPublicRoom>d__68 <JoinRandomPublicRoom>d__;
		<JoinRandomPublicRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<JoinRandomPublicRoom>d__.<>4__this = this;
		<JoinRandomPublicRoom>d__.opts = opts;
		<JoinRandomPublicRoom>d__.<>1__state = -1;
		<JoinRandomPublicRoom>d__.<>t__builder.Start<NetworkSystemPUN.<JoinRandomPublicRoom>d__68>(ref <JoinRandomPublicRoom>d__);
		return <JoinRandomPublicRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001BA9 RID: 7081 RVA: 0x00095874 File Offset: 0x00093A74
	public override Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		NetworkSystemPUN.<ConnectToRoom>d__69 <ConnectToRoom>d__;
		<ConnectToRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<ConnectToRoom>d__.<>4__this = this;
		<ConnectToRoom>d__.roomName = roomName;
		<ConnectToRoom>d__.opts = opts;
		<ConnectToRoom>d__.regionIndex = regionIndex;
		<ConnectToRoom>d__.<>1__state = -1;
		<ConnectToRoom>d__.<>t__builder.Start<NetworkSystemPUN.<ConnectToRoom>d__69>(ref <ConnectToRoom>d__);
		return <ConnectToRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001BAA RID: 7082 RVA: 0x000958D0 File Offset: 0x00093AD0
	public override Task JoinFriendsRoom(string userID, int actorIDToFollow, string keyToFollow, string shufflerToFollow)
	{
		NetworkSystemPUN.<JoinFriendsRoom>d__70 <JoinFriendsRoom>d__;
		<JoinFriendsRoom>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<JoinFriendsRoom>d__.<>4__this = this;
		<JoinFriendsRoom>d__.userID = userID;
		<JoinFriendsRoom>d__.actorIDToFollow = actorIDToFollow;
		<JoinFriendsRoom>d__.keyToFollow = keyToFollow;
		<JoinFriendsRoom>d__.shufflerToFollow = shufflerToFollow;
		<JoinFriendsRoom>d__.<>1__state = -1;
		<JoinFriendsRoom>d__.<>t__builder.Start<NetworkSystemPUN.<JoinFriendsRoom>d__70>(ref <JoinFriendsRoom>d__);
		return <JoinFriendsRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001BAB RID: 7083 RVA: 0x00002AF8 File Offset: 0x00000CF8
	public override void JoinPubWithFriends()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001BAC RID: 7084 RVA: 0x00095934 File Offset: 0x00093B34
	public override string GetRandomWeightedRegion()
	{
		float value = Random.value;
		int num = 0;
		for (int i = 0; i < this.regionData.Length; i++)
		{
			num += this.regionData[i].playersInRegion;
		}
		float num2 = 0f;
		int num3 = -1;
		while (num2 < value && num3 < this.regionData.Length - 1)
		{
			num3++;
			num2 += (float)this.regionData[num3].playersInRegion / (float)num;
		}
		return this.regionNames[num3];
	}

	// Token: 0x06001BAD RID: 7085 RVA: 0x000959AC File Offset: 0x00093BAC
	public override Task ReturnToSinglePlayer()
	{
		NetworkSystemPUN.<ReturnToSinglePlayer>d__73 <ReturnToSinglePlayer>d__;
		<ReturnToSinglePlayer>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ReturnToSinglePlayer>d__.<>4__this = this;
		<ReturnToSinglePlayer>d__.<>1__state = -1;
		<ReturnToSinglePlayer>d__.<>t__builder.Start<NetworkSystemPUN.<ReturnToSinglePlayer>d__73>(ref <ReturnToSinglePlayer>d__);
		return <ReturnToSinglePlayer>d__.<>t__builder.Task;
	}

	// Token: 0x06001BAE RID: 7086 RVA: 0x000959F0 File Offset: 0x00093BF0
	private Task InternalDisconnect()
	{
		NetworkSystemPUN.<InternalDisconnect>d__74 <InternalDisconnect>d__;
		<InternalDisconnect>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<InternalDisconnect>d__.<>4__this = this;
		<InternalDisconnect>d__.<>1__state = -1;
		<InternalDisconnect>d__.<>t__builder.Start<NetworkSystemPUN.<InternalDisconnect>d__74>(ref <InternalDisconnect>d__);
		return <InternalDisconnect>d__.<>t__builder.Task;
	}

	// Token: 0x06001BAF RID: 7087 RVA: 0x00095A33 File Offset: 0x00093C33
	private void AddVoice()
	{
		this.SetupVoice();
	}

	// Token: 0x06001BB0 RID: 7088 RVA: 0x00095A3C File Offset: 0x00093C3C
	private void SetupVoice()
	{
		try
		{
			this.punVoice = PhotonVoiceNetwork.Instance;
			this.VoiceNetworkObject = this.punVoice.gameObject;
			this.VoiceNetworkObject.name = "VoiceNetworkObject";
			this.VoiceNetworkObject.transform.parent = base.transform;
			this.VoiceNetworkObject.transform.localPosition = Vector3.zero;
			this.punVoice.LogLevel = this.VoiceSettings.LogLevel;
			this.punVoice.GlobalRecordersLogLevel = this.VoiceSettings.GlobalRecordersLogLevel;
			this.punVoice.GlobalSpeakersLogLevel = this.VoiceSettings.GlobalSpeakersLogLevel;
			this.punVoice.AutoConnectAndJoin = this.VoiceSettings.AutoConnectAndJoin;
			this.punVoice.AutoLeaveAndDisconnect = this.VoiceSettings.AutoLeaveAndDisconnect;
			this.punVoice.WorkInOfflineMode = this.VoiceSettings.WorkInOfflineMode;
			this.punVoice.AutoCreateSpeakerIfNotFound = this.VoiceSettings.CreateSpeakerIfNotFound;
			AppSettings appSettings = new AppSettings();
			appSettings.AppIdRealtime = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
			appSettings.AppIdVoice = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice;
			this.punVoice.Settings = appSettings;
			this.remoteVoiceAddedCallbacks.ForEach(delegate(Action<RemoteVoiceLink> callback)
			{
				this.punVoice.RemoteVoiceAdded += callback;
			});
			this.localRecorder = this.VoiceNetworkObject.GetComponent<GTRecorder>();
			if (this.localRecorder == null)
			{
				this.localRecorder = this.VoiceNetworkObject.AddComponent<GTRecorder>();
				if (VRRigCache.Instance != null && VRRigCache.Instance.localRig != null)
				{
					LoudSpeakerActivator[] componentsInChildren = VRRigCache.Instance.localRig.GetComponentsInChildren<LoudSpeakerActivator>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].SetRecorder((GTRecorder)this.localRecorder);
					}
				}
			}
			this.localRecorder.LogLevel = this.VoiceSettings.LogLevel;
			this.localRecorder.RecordOnlyWhenEnabled = this.VoiceSettings.RecordOnlyWhenEnabled;
			this.localRecorder.RecordOnlyWhenJoined = this.VoiceSettings.RecordOnlyWhenJoined;
			this.localRecorder.StopRecordingWhenPaused = this.VoiceSettings.StopRecordingWhenPaused;
			this.localRecorder.TransmitEnabled = this.VoiceSettings.TransmitEnabled;
			this.localRecorder.AutoStart = this.VoiceSettings.AutoStart;
			this.localRecorder.Encrypt = this.VoiceSettings.Encrypt;
			this.localRecorder.FrameDuration = this.VoiceSettings.FrameDuration;
			this.localRecorder.InterestGroup = this.VoiceSettings.InterestGroup;
			this.localRecorder.SourceType = this.VoiceSettings.InputSourceType;
			this.localRecorder.MicrophoneType = this.VoiceSettings.MicrophoneType;
			this.localRecorder.UseMicrophoneTypeFallback = this.VoiceSettings.UseFallback;
			this.localRecorder.VoiceDetection = this.VoiceSettings.Detect;
			this.localRecorder.VoiceDetectionThreshold = this.VoiceSettings.Threshold;
			this.localRecorder.VoiceDetectionDelayMs = this.VoiceSettings.Delay;
			this.localRecorder.DebugEchoMode = this.VoiceSettings.DebugEcho;
			if (!SubscriptionManager.IsLocalSubscribed())
			{
				this.localRecorder.SamplingRate = this.VoiceSettings.SamplingRate;
				this.localRecorder.Bitrate = this.VoiceSettings.Bitrate;
			}
			else
			{
				this.localRecorder.SamplingRate = this.VoiceSettings.SubsSamplingRate;
				this.localRecorder.Bitrate = this.VoiceSettings.SubsBitrate;
			}
			this.VoiceNetworkObject.AddComponent<VoiceToLoudness>();
			this.punVoice.PrimaryRecorder = this.localRecorder;
		}
		catch (Exception ex)
		{
			Debug.LogError("An exception was thrown when trying to setup photon voice, please check microphone permissions:\n" + ex.ToString());
		}
	}

	// Token: 0x06001BB1 RID: 7089 RVA: 0x000905EF File Offset: 0x0008E7EF
	public override void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback)
	{
		this.remoteVoiceAddedCallbacks.Add(callback);
	}

	// Token: 0x06001BB2 RID: 7090 RVA: 0x00095E24 File Offset: 0x00094024
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			return null;
		}
		if (isRoomObject)
		{
			return PhotonNetwork.InstantiateRoomObject(prefab.name, position, rotation, 0, null);
		}
		return PhotonNetwork.Instantiate(prefab.name, position, rotation, 0, null);
	}

	// Token: 0x06001BB3 RID: 7091 RVA: 0x00095E52 File Offset: 0x00094052
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, position, rotation, isRoomObject);
	}

	// Token: 0x06001BB4 RID: 7092 RVA: 0x00095E5F File Offset: 0x0009405F
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			return null;
		}
		if (isRoomObject)
		{
			return PhotonNetwork.InstantiateRoomObject(prefab.name, position, rotation, group, data);
		}
		return PhotonNetwork.Instantiate(prefab.name, position, rotation, group, data);
	}

	// Token: 0x06001BB5 RID: 7093 RVA: 0x00095E94 File Offset: 0x00094094
	public override void NetDestroy(GameObject instance)
	{
		PhotonView photonView;
		if (instance.TryGetComponent<PhotonView>(out photonView) && photonView.AmOwner)
		{
			PhotonNetwork.Destroy(instance);
			return;
		}
		Object.Destroy(instance);
	}

	// Token: 0x06001BB6 RID: 7094 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null)
	{
	}

	// Token: 0x06001BB7 RID: 7095 RVA: 0x00095EC0 File Offset: 0x000940C0
	public override void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true)
	{
		RpcTarget target = sendToSelf ? RpcTarget.All : RpcTarget.Others;
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, target, new object[]
		{
			NetworkSystem.EmptyArgs
		});
	}

	// Token: 0x06001BB8 RID: 7096 RVA: 0x00095EFC File Offset: 0x000940FC
	public override void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true)
	{
		RpcTarget target = sendToSelf ? RpcTarget.All : RpcTarget.Others;
		ref args.SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, target, new object[]
		{
			args.Data
		});
	}

	// Token: 0x06001BB9 RID: 7097 RVA: 0x00095F40 File Offset: 0x00094140
	public override void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true)
	{
		RpcTarget target = sendToSelf ? RpcTarget.All : RpcTarget.Others;
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, target, new object[]
		{
			message
		});
	}

	// Token: 0x06001BBA RID: 7098 RVA: 0x00095F78 File Offset: 0x00094178
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[]
		{
			NetworkSystem.EmptyArgs
		});
	}

	// Token: 0x06001BBB RID: 7099 RVA: 0x00095FB8 File Offset: 0x000941B8
	public override void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		ref args.SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[]
		{
			args.Data
		});
	}

	// Token: 0x06001BBC RID: 7100 RVA: 0x00096000 File Offset: 0x00094200
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[]
		{
			message
		});
	}

	// Token: 0x06001BBD RID: 7101 RVA: 0x0009603C File Offset: 0x0009423C
	public override Task AwaitSceneReady()
	{
		NetworkSystemPUN.<AwaitSceneReady>d__89 <AwaitSceneReady>d__;
		<AwaitSceneReady>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AwaitSceneReady>d__.<>1__state = -1;
		<AwaitSceneReady>d__.<>t__builder.Start<NetworkSystemPUN.<AwaitSceneReady>d__89>(ref <AwaitSceneReady>d__);
		return <AwaitSceneReady>d__.<>t__builder.Task;
	}

	// Token: 0x06001BBE RID: 7102 RVA: 0x00096078 File Offset: 0x00094278
	public override NetPlayer GetLocalPlayer()
	{
		if (this.netPlayerCache.Count == 0)
		{
			base.UpdatePlayers();
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.IsLocal)
			{
				return netPlayer;
			}
		}
		Debug.LogError("Somehow no local net players found. This shouldn't happen");
		return null;
	}

	// Token: 0x06001BBF RID: 7103 RVA: 0x000960F0 File Offset: 0x000942F0
	public override NetPlayer GetPlayer(int PlayerID)
	{
		if (this.InRoom && !PhotonNetwork.CurrentRoom.Players.ContainsKey(PlayerID))
		{
			return null;
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.ActorNumber == PlayerID)
			{
				return netPlayer;
			}
		}
		base.UpdatePlayers();
		foreach (NetPlayer netPlayer2 in this.netPlayerCache)
		{
			if (netPlayer2.ActorNumber == PlayerID)
			{
				return netPlayer2;
			}
		}
		GTDev.LogWarning<string>("There is no NetPlayer with this ID currently in game. Passed ID: " + PlayerID.ToString(), null);
		return null;
	}

	// Token: 0x06001BC0 RID: 7104 RVA: 0x000961D0 File Offset: 0x000943D0
	public override void SetMyNickName(string id)
	{
		if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags) && !id.StartsWith("gorilla"))
		{
			Debug.Log("[KID] Trying to set custom nickname but that permission has been disallowed");
			PhotonNetwork.LocalPlayer.NickName = "gorilla";
			return;
		}
		PlayerPrefs.SetString("playerName", id);
		string nickName = PhotonNetwork.LocalPlayer.NickName;
		PhotonNetwork.LocalPlayer.NickName = id;
	}

	// Token: 0x06001BC1 RID: 7105 RVA: 0x0009622D File Offset: 0x0009442D
	public override string GetMyNickName()
	{
		return PhotonNetwork.LocalPlayer.NickName;
	}

	// Token: 0x06001BC2 RID: 7106 RVA: 0x00096239 File Offset: 0x00094439
	public override string GetMyDefaultName()
	{
		return PhotonNetwork.LocalPlayer.DefaultName;
	}

	// Token: 0x06001BC3 RID: 7107 RVA: 0x00096248 File Offset: 0x00094448
	public override string GetNickName(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player != null)
		{
			return player.NickName;
		}
		return null;
	}

	// Token: 0x06001BC4 RID: 7108 RVA: 0x00096268 File Offset: 0x00094468
	public override string GetNickName(NetPlayer player)
	{
		return player.NickName;
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x00096270 File Offset: 0x00094470
	public override void SetMyTutorialComplete()
	{
		bool flag = PlayerPrefs.GetString("didTutorial", "nope") == "done";
		if (!flag)
		{
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add("didTutorial", flag);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x000914D6 File Offset: 0x0008F6D6
	public override bool GetMyTutorialCompletion()
	{
		return PlayerPrefs.GetString("didTutorial", "nope") == "done";
	}

	// Token: 0x06001BC7 RID: 7111 RVA: 0x000962D4 File Offset: 0x000944D4
	public override bool GetPlayerTutorialCompletion(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player == null)
		{
			return false;
		}
		Player player2 = PhotonNetwork.CurrentRoom.GetPlayer(player.ActorNumber, false);
		if (player2 == null)
		{
			return false;
		}
		object obj;
		if (player2.CustomProperties.TryGetValue("didTutorial", out obj))
		{
			bool flag;
			bool flag2;
			if (obj is bool)
			{
				flag = (bool)obj;
				flag2 = (1 == 0);
			}
			else
			{
				flag2 = true;
			}
			return flag2 || flag;
		}
		return false;
	}

	// Token: 0x06001BC8 RID: 7112 RVA: 0x00096333 File Offset: 0x00094533
	public override string GetMyUserID()
	{
		return PhotonNetwork.LocalPlayer.UserId;
	}

	// Token: 0x06001BC9 RID: 7113 RVA: 0x00096340 File Offset: 0x00094540
	public override string GetUserID(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player != null)
		{
			return player.UserId;
		}
		return null;
	}

	// Token: 0x06001BCA RID: 7114 RVA: 0x00096360 File Offset: 0x00094560
	public override string GetUserID(NetPlayer netPlayer)
	{
		Player playerRef = ((PunNetPlayer)netPlayer).PlayerRef;
		if (playerRef != null)
		{
			return playerRef.UserId;
		}
		return null;
	}

	// Token: 0x06001BCB RID: 7115 RVA: 0x00096384 File Offset: 0x00094584
	public override int GlobalPlayerCount()
	{
		int num = 0;
		foreach (NetworkRegionInfo networkRegionInfo in this.regionData)
		{
			num += networkRegionInfo.playersInRegion;
		}
		return num;
	}

	// Token: 0x06001BCC RID: 7116 RVA: 0x000963B8 File Offset: 0x000945B8
	public override bool IsObjectLocallyOwned(GameObject obj)
	{
		PhotonView photonView;
		return !this.IsOnline || !obj.TryGetComponent<PhotonView>(out photonView) || photonView.IsMine;
	}

	// Token: 0x06001BCD RID: 7117 RVA: 0x000963E4 File Offset: 0x000945E4
	protected override void UpdateNetPlayerList()
	{
		if (!this.IsOnline)
		{
			bool flag = false;
			PunNetPlayer punNetPlayer = null;
			if (this.netPlayerCache.Count > 0)
			{
				for (int i = 0; i < this.netPlayerCache.Count; i++)
				{
					NetPlayer netPlayer = this.netPlayerCache[i];
					if (netPlayer.IsLocal)
					{
						punNetPlayer = (PunNetPlayer)netPlayer;
						flag = true;
					}
					else
					{
						this.playerPool.Return((PunNetPlayer)netPlayer);
					}
				}
				this.netPlayerCache.Clear();
			}
			if (!flag)
			{
				punNetPlayer = this.playerPool.Take();
				punNetPlayer.InitPlayer(PhotonNetwork.LocalPlayer);
			}
			this.netPlayerCache.Add(punNetPlayer);
		}
		else
		{
			Dictionary<int, Player>.ValueCollection values = PhotonNetwork.CurrentRoom.Players.Values;
			foreach (Player player in values)
			{
				bool flag2 = false;
				for (int j = 0; j < this.netPlayerCache.Count; j++)
				{
					if (player == ((PunNetPlayer)this.netPlayerCache[j]).PlayerRef)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					PunNetPlayer punNetPlayer2 = this.playerPool.Take();
					punNetPlayer2.InitPlayer(player);
					this.netPlayerCache.Add(punNetPlayer2);
				}
			}
			for (int k = 0; k < this.netPlayerCache.Count; k++)
			{
				PunNetPlayer punNetPlayer3 = (PunNetPlayer)this.netPlayerCache[k];
				bool flag3 = false;
				using (Dictionary<int, Player>.ValueCollection.Enumerator enumerator = values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == punNetPlayer3.PlayerRef)
						{
							flag3 = true;
							break;
						}
					}
				}
				if (!flag3)
				{
					this.playerPool.Return(punNetPlayer3);
					this.netPlayerCache.Remove(punNetPlayer3);
				}
			}
		}
		this.m_allNetPlayers = this.netPlayerCache.ToArray();
		this.m_otherNetPlayers = new NetPlayer[this.m_allNetPlayers.Length - 1];
		int num = 0;
		for (int l = 0; l < this.m_allNetPlayers.Length; l++)
		{
			NetPlayer netPlayer2 = this.m_allNetPlayers[l];
			if (netPlayer2.IsLocal)
			{
				num++;
			}
			else
			{
				int num2 = l - num;
				if (num2 == this.m_otherNetPlayers.Length)
				{
					break;
				}
				this.m_otherNetPlayers[num2] = netPlayer2;
			}
		}
	}

	// Token: 0x06001BCE RID: 7118 RVA: 0x00096650 File Offset: 0x00094850
	public override bool IsObjectRoomObject(GameObject obj)
	{
		PhotonView component = obj.GetComponent<PhotonView>();
		if (component == null)
		{
			Debug.LogError("No photonview found on this Object, this shouldn't happen");
			return false;
		}
		return component.IsRoomView;
	}

	// Token: 0x06001BCF RID: 7119 RVA: 0x0009667F File Offset: 0x0009487F
	public override bool ShouldUpdateObject(GameObject obj)
	{
		return this.IsObjectLocallyOwned(obj);
	}

	// Token: 0x06001BD0 RID: 7120 RVA: 0x0009667F File Offset: 0x0009487F
	public override bool ShouldWriteObjectData(GameObject obj)
	{
		return this.IsObjectLocallyOwned(obj);
	}

	// Token: 0x06001BD1 RID: 7121 RVA: 0x00096688 File Offset: 0x00094888
	public override int GetOwningPlayerID(GameObject obj)
	{
		PhotonView photonView;
		if (obj.TryGetComponent<PhotonView>(out photonView) && photonView.Owner != null)
		{
			return photonView.Owner.ActorNumber;
		}
		return -1;
	}

	// Token: 0x06001BD2 RID: 7122 RVA: 0x000966B4 File Offset: 0x000948B4
	public override bool ShouldSpawnLocally(int playerID)
	{
		return this.LocalPlayerID == playerID || (playerID == -1 && PhotonNetwork.MasterClient.IsLocal);
	}

	// Token: 0x06001BD3 RID: 7123 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsTotalAuthority()
	{
		return false;
	}

	// Token: 0x06001BD4 RID: 7124 RVA: 0x000966D1 File Offset: 0x000948D1
	public void OnConnectedtoMaster()
	{
		if (this.internalState == NetworkSystemPUN.InternalState.ConnectingToMaster)
		{
			this.internalState = NetworkSystemPUN.InternalState.ConnectedToMaster;
		}
		base.UpdatePlayers();
	}

	// Token: 0x06001BD5 RID: 7125 RVA: 0x000966E9 File Offset: 0x000948E9
	public void OnJoinedRoom()
	{
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Joining)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Joined;
		}
		else if (this.internalState == NetworkSystemPUN.InternalState.Searching_Creating)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Created;
		}
		this.AddVoice();
		base.UpdatePlayers();
		base.JoinedNetworkRoom();
	}

	// Token: 0x06001BD6 RID: 7126 RVA: 0x00096724 File Offset: 0x00094924
	public void OnJoinRoomFailed(short returnCode, string message)
	{
		PersistLog.Log("OnJoinRoomFailed " + returnCode.ToString() + " " + message);
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Joining)
		{
			if (returnCode == 32765)
			{
				this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed_Full;
				return;
			}
			this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed;
		}
	}

	// Token: 0x06001BD7 RID: 7127 RVA: 0x00096770 File Offset: 0x00094970
	public void OnCreateRoomFailed(short returnCode, string message)
	{
		PersistLog.Log("OnCreateRoomFailed " + returnCode.ToString() + " " + message);
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Creating)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_CreateFailed;
		}
	}

	// Token: 0x06001BD8 RID: 7128 RVA: 0x000967A0 File Offset: 0x000949A0
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.UpdatePlayers();
		NetPlayer player = base.GetPlayer(newPlayer);
		base.PlayerJoined(player);
	}

	// Token: 0x06001BD9 RID: 7129 RVA: 0x000967C4 File Offset: 0x000949C4
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		NetPlayer player = base.GetPlayer(otherPlayer);
		base.UpdatePlayers();
		base.PlayerLeft(player);
	}

	// Token: 0x06001BDA RID: 7130 RVA: 0x000967E8 File Offset: 0x000949E8
	public void OnDisconnected(DisconnectCause cause)
	{
		NetworkSystemPUN.<OnDisconnected>d__118 <OnDisconnected>d__;
		<OnDisconnected>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnDisconnected>d__.<>4__this = this;
		<OnDisconnected>d__.<>1__state = -1;
		<OnDisconnected>d__.<>t__builder.Start<NetworkSystemPUN.<OnDisconnected>d__118>(ref <OnDisconnected>d__);
	}

	// Token: 0x06001BDB RID: 7131 RVA: 0x0009681F File Offset: 0x00094A1F
	public void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitchedCallback(newMasterClient);
	}

	// Token: 0x06001BDC RID: 7132 RVA: 0x00096830 File Offset: 0x00094A30
	private ValueTuple<CancellationTokenSource, CancellationToken> GetCancellationToken()
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		CancellationToken token = cancellationTokenSource.Token;
		this._taskCancelTokens.Add(cancellationTokenSource);
		return new ValueTuple<CancellationTokenSource, CancellationToken>(cancellationTokenSource, token);
	}

	// Token: 0x06001BDD RID: 7133 RVA: 0x00096860 File Offset: 0x00094A60
	public void ResetSystem()
	{
		if (this.VoiceNetworkObject)
		{
			Object.Destroy(this.VoiceNetworkObject);
		}
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.lowestPingRegionIndex];
		this.currentRegionIndex = this.lowestPingRegionIndex;
		PhotonNetwork.Disconnect();
		this._taskCancelTokens.ForEach(delegate(CancellationTokenSource token)
		{
			token.Cancel();
			token.Dispose();
		});
		this._taskCancelTokens.Clear();
		this.internalState = NetworkSystemPUN.InternalState.Idle;
		base.netState = NetSystemState.Idle;
	}

	// Token: 0x06001BDE RID: 7134 RVA: 0x000968F8 File Offset: 0x00094AF8
	private void UpdateZoneInfo(bool roomIsPublic, string zoneName = null)
	{
		AuthenticationValues authenticationValues = this.GetAuthenticationValues();
		Dictionary<string, object> dictionary = ((authenticationValues != null) ? authenticationValues.AuthPostData : null) as Dictionary<string, object>;
		if (dictionary != null)
		{
			dictionary["Zone"] = ((zoneName != null) ? zoneName : ((ZoneManagement.instance.activeZones.Count > 0) ? ZoneManagement.instance.activeZones.First<GTZone>().GetName<GTZone>() : ""));
			dictionary["SubZone"] = GTSubZone.none.GetName<GTSubZone>();
			dictionary["IsPublic"] = roomIsPublic;
			authenticationValues.SetAuthPostData(dictionary);
			this.SetAuthenticationValues(authenticationValues);
		}
	}

	// Token: 0x040025AB RID: 9643
	private NetworkRegionInfo[] regionData;

	// Token: 0x040025AC RID: 9644
	private Task<NetJoinResult> roomTask;

	// Token: 0x040025AD RID: 9645
	private ObjectPool<PunNetPlayer> playerPool;

	// Token: 0x040025AE RID: 9646
	private NetPlayer[] m_allNetPlayers = new NetPlayer[0];

	// Token: 0x040025AF RID: 9647
	private NetPlayer[] m_otherNetPlayers = new NetPlayer[0];

	// Token: 0x040025B0 RID: 9648
	private List<CancellationTokenSource> _taskCancelTokens = new List<CancellationTokenSource>();

	// Token: 0x040025B1 RID: 9649
	private PhotonVoiceNetwork punVoice;

	// Token: 0x040025B2 RID: 9650
	private GameObject VoiceNetworkObject;

	// Token: 0x040025B3 RID: 9651
	private NetworkSystemPUN.InternalState currentState;

	// Token: 0x040025B4 RID: 9652
	private bool firstRoomJoin;

	// Token: 0x0200046E RID: 1134
	private enum InternalState
	{
		// Token: 0x040025B6 RID: 9654
		AwaitingAuth,
		// Token: 0x040025B7 RID: 9655
		Authenticated,
		// Token: 0x040025B8 RID: 9656
		PingGathering,
		// Token: 0x040025B9 RID: 9657
		StateCheckFailed,
		// Token: 0x040025BA RID: 9658
		ConnectingToMaster,
		// Token: 0x040025BB RID: 9659
		ConnectedToMaster,
		// Token: 0x040025BC RID: 9660
		Idle,
		// Token: 0x040025BD RID: 9661
		Internal_Disconnecting,
		// Token: 0x040025BE RID: 9662
		Internal_Disconnected,
		// Token: 0x040025BF RID: 9663
		Searching_Connecting,
		// Token: 0x040025C0 RID: 9664
		Searching_Connected,
		// Token: 0x040025C1 RID: 9665
		Searching_Joining,
		// Token: 0x040025C2 RID: 9666
		Searching_Joined,
		// Token: 0x040025C3 RID: 9667
		Searching_JoinFailed,
		// Token: 0x040025C4 RID: 9668
		Searching_JoinFailed_Full,
		// Token: 0x040025C5 RID: 9669
		Searching_Creating,
		// Token: 0x040025C6 RID: 9670
		Searching_Created,
		// Token: 0x040025C7 RID: 9671
		Searching_CreateFailed,
		// Token: 0x040025C8 RID: 9672
		Searching_Disconnecting,
		// Token: 0x040025C9 RID: 9673
		Searching_Disconnected
	}
}
