using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Timers;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using TagEffects;
using UnityEngine;
using Voxels;

// Token: 0x02000CE7 RID: 3303
internal class RoomSystem : MonoBehaviour
{
	// Token: 0x060051DD RID: 20957 RVA: 0x001AED44 File Offset: 0x001ACF44
	internal static void DeserializeLaunchProjectile(object[] projectileData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "LaunchSlingshotProjectile");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return;
		}
		byte b = Convert.ToByte(projectileData[5]);
		byte b2 = Convert.ToByte(projectileData[6]);
		byte b3 = Convert.ToByte(projectileData[7]);
		byte b4 = Convert.ToByte(projectileData[8]);
		Color32 c = new Color32(b, b2, b3, b4);
		Vector3 position = (Vector3)projectileData[0];
		Vector3 velocity = (Vector3)projectileData[1];
		float num = 10000f;
		if (position.IsValid(num))
		{
			float num2 = 10000f;
			if (velocity.IsValid(num2) && float.IsFinite((float)b) && float.IsFinite((float)b2) && float.IsFinite((float)b3) && float.IsFinite((float)b4))
			{
				RoomSystem.ProjectileSource projectileSource = (RoomSystem.ProjectileSource)Convert.ToInt32(projectileData[2]);
				int projectileIndex = Convert.ToInt32(projectileData[3]);
				bool overridecolour = Convert.ToBoolean(projectileData[4]);
				VRRig rig = rigContainer.Rig;
				if (rig.isOfflineVRRig || rig.IsPositionInRange(position, 4f))
				{
					RoomSystem.launchProjectile.targetRig = rig;
					RoomSystem.launchProjectile.position = position;
					RoomSystem.launchProjectile.velocity = velocity;
					RoomSystem.launchProjectile.overridecolour = overridecolour;
					RoomSystem.launchProjectile.colour = c;
					RoomSystem.launchProjectile.projectileIndex = projectileIndex;
					RoomSystem.launchProjectile.projectileSource = projectileSource;
					RoomSystem.launchProjectile.messageInfo = info;
					FXSystem.PlayFXForRig(FXType.Projectile, RoomSystem.launchProjectile, info);
				}
				return;
			}
		}
		MonkeAgent.instance.SendReport("invalid projectile state", player.UserId, player.NickName);
	}

	// Token: 0x060051DE RID: 20958 RVA: 0x001AEEDC File Offset: 0x001AD0DC
	internal static void SendLaunchProjectile(Vector3 position, Vector3 velocity, RoomSystem.ProjectileSource projectileSource, int projectileCount, bool randomColour, byte r, byte g, byte b, byte a)
	{
		if (!RoomSystem.JoinedRoom)
		{
			return;
		}
		RoomSystem.projectileSendData[0] = position;
		RoomSystem.projectileSendData[1] = velocity;
		RoomSystem.projectileSendData[2] = projectileSource;
		RoomSystem.projectileSendData[3] = projectileCount;
		RoomSystem.projectileSendData[4] = randomColour;
		RoomSystem.projectileSendData[5] = r;
		RoomSystem.projectileSendData[6] = g;
		RoomSystem.projectileSendData[7] = b;
		RoomSystem.projectileSendData[8] = a;
		RoomSystem.SendEvent(0, RoomSystem.projectileSendData, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x060051DF RID: 20959 RVA: 0x001AEF7C File Offset: 0x001AD17C
	internal static void ImpactEffect(VRRig targetRig, Vector3 position, float r, float g, float b, float a, int projectileCount, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		RoomSystem.impactEffect.targetRig = targetRig;
		RoomSystem.impactEffect.position = position;
		RoomSystem.impactEffect.colour = new Color(r, g, b, a);
		RoomSystem.impactEffect.projectileIndex = projectileCount;
		FXSystem.PlayFXForRig(FXType.Impact, RoomSystem.impactEffect, info);
	}

	// Token: 0x060051E0 RID: 20960 RVA: 0x001AEFD0 File Offset: 0x001AD1D0
	internal static void DeserializeImpactEffect(object[] impactData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "SpawnSlingshotPlayerImpactEffect");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || rigContainer.Rig.projectileWeapon.IsNull())
		{
			return;
		}
		float num = Convert.ToSingle(impactData[1]);
		float num2 = Convert.ToSingle(impactData[2]);
		float num3 = Convert.ToSingle(impactData[3]);
		float num4 = Convert.ToSingle(impactData[4]);
		Vector3 position = (Vector3)impactData[0];
		float num5 = 10000f;
		if (!position.IsValid(num5) || !float.IsFinite(num) || !float.IsFinite(num2) || !float.IsFinite(num3) || !float.IsFinite(num4))
		{
			MonkeAgent.instance.SendReport("invalid impact state", player.UserId, player.NickName);
			return;
		}
		int projectileCount = Convert.ToInt32(impactData[5]);
		RoomSystem.ImpactEffect(rigContainer.Rig, position, num, num2, num3, num4, projectileCount, info);
	}

	// Token: 0x060051E1 RID: 20961 RVA: 0x001AF0C0 File Offset: 0x001AD2C0
	internal static void SendImpactEffect(Vector3 position, float r, float g, float b, float a, int projectileCount)
	{
		RoomSystem.ImpactEffect(VRRigCache.Instance.localRig.Rig, position, r, g, b, a, projectileCount, default(PhotonMessageInfoWrapped));
		if (RoomSystem.joinedRoom)
		{
			RoomSystem.impactSendData[0] = position;
			RoomSystem.impactSendData[1] = r;
			RoomSystem.impactSendData[2] = g;
			RoomSystem.impactSendData[3] = b;
			RoomSystem.impactSendData[4] = a;
			RoomSystem.impactSendData[5] = projectileCount;
			RoomSystem.SendEvent(1, RoomSystem.impactSendData, NetworkSystemRaiseEvent.neoOthers, false);
		}
	}

	// Token: 0x060051E2 RID: 20962 RVA: 0x001AF15A File Offset: 0x001AD35A
	internal static void SendLavaSync(byte zone, byte state, double stateStartTime, float activationProgress, int voteCount, int[] votePlayerIds)
	{
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.PackLavaSyncData(zone, state, stateStartTime, activationProgress, voteCount, votePlayerIds);
		RoomSystem.SendEvent(12, RoomSystem.lavaSyncSendData, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x060051E3 RID: 20963 RVA: 0x001AF183 File Offset: 0x001AD383
	internal static void SendLavaSyncToPlayer(byte zone, byte state, double stateStartTime, float activationProgress, int voteCount, int[] votePlayerIds, NetPlayer target)
	{
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.PackLavaSyncData(zone, state, stateStartTime, activationProgress, voteCount, votePlayerIds);
		RoomSystem.SendEvent(12, RoomSystem.lavaSyncSendData, target, false);
	}

	// Token: 0x060051E4 RID: 20964 RVA: 0x001AF1AC File Offset: 0x001AD3AC
	private static void PackLavaSyncData(byte zone, byte state, double stateStartTime, float activationProgress, int voteCount, int[] votePlayerIds)
	{
		RoomSystem.lavaSyncSendData[0] = zone;
		RoomSystem.lavaSyncSendData[1] = state;
		RoomSystem.lavaSyncSendData[2] = stateStartTime;
		RoomSystem.lavaSyncSendData[3] = activationProgress;
		RoomSystem.lavaSyncSendData[4] = voteCount;
		for (int i = 0; i < 20; i++)
		{
			RoomSystem.lavaSyncSendData[5 + i] = votePlayerIds[i];
		}
	}

	// Token: 0x060051E5 RID: 20965 RVA: 0x001AF21C File Offset: 0x001AD41C
	private unsafe static void DeserializeLavaSync(object[] data, PhotonMessageInfoWrapped info)
	{
		NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "DeserializeLavaSync");
		if (!RoomSystem.callbackInstance.roomSettings.LavaSyncLimiter.CheckCallServerTime(info.SentServerTime))
		{
			Debug.LogWarning(string.Format("[RoomSystem] LavaSync dropped by rate limiter: sender={0} sentTime={1:F3} photonTime={2:F3}", info.senderID, info.SentServerTime, PhotonNetwork.Time));
			return;
		}
		if (data != null && data.Length >= 25)
		{
			object obj = data[0];
			if (obj is byte)
			{
				byte zone = (byte)obj;
				obj = data[1];
				if (obj is byte)
				{
					byte b = (byte)obj;
					obj = data[2];
					if (obj is double)
					{
						double value = (double)obj;
						obj = data[3];
						if (obj is float)
						{
							float value2 = (float)obj;
							obj = data[4];
							if (obj is int)
							{
								int value3 = (int)obj;
								for (int i = 0; i < 20; i++)
								{
									if (!(data[5 + i] is int))
									{
										return;
									}
								}
								if (b > 4)
								{
									return;
								}
								RoomSystem.LavaSyncEventData obj2;
								obj2.zone = zone;
								obj2.state = b;
								obj2.stateStartTime = value.GetFinite();
								obj2.activationProgress = value2.ClampSafe(0f, 2f);
								obj2.voteCount = Mathf.Clamp(value3, 0, 20);
								obj2.senderActorNumber = info.senderID;
								for (int j = 0; j < 20; j++)
								{
									*(ref obj2.votes.FixedElementField + (IntPtr)j * 4) = (int)data[5 + j];
								}
								Action<RoomSystem.LavaSyncEventData> onLavaSyncReceived = RoomSystem.OnLavaSyncReceived;
								if (onLavaSyncReceived == null)
								{
									return;
								}
								onLavaSyncReceived(obj2);
								return;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060051E6 RID: 20966 RVA: 0x001AF3C5 File Offset: 0x001AD5C5
	internal static void SendMonkePointsRedeemed(int redeemedPointCount)
	{
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.monkePointsRedeemedSendData[0] = redeemedPointCount;
		RoomSystem.SendEvent(13, RoomSystem.monkePointsRedeemedSendData, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x060051E7 RID: 20967 RVA: 0x001AF3F0 File Offset: 0x001AD5F0
	private static void DeserializeMonkePointsRedeemed(object[] data, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "BroadcastRedeemQuestPoints");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (player == null)
		{
			return;
		}
		if (data != null && data.Length >= 1)
		{
			object obj = data[0];
			if (obj is int)
			{
				int num = (int)obj;
				num = Mathf.Clamp(num, 0, 50);
				Action<NetPlayer, int> onMonkePointsRedeemedReceived = RoomSystem.OnMonkePointsRedeemedReceived;
				if (onMonkePointsRedeemedReceived == null)
				{
					return;
				}
				onMonkePointsRedeemedReceived(player, num);
				return;
			}
		}
	}

	// Token: 0x060051E8 RID: 20968 RVA: 0x001AF458 File Offset: 0x001AD658
	private void Awake()
	{
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(this);
		RoomSystem.playerImpactEffectPrefab = this.roomSettings.PlayerImpactEffect;
		RoomSystem.callbackInstance = this;
		RoomSystem.disconnectTimer.Interval = (double)(this.roomSettings.PausedDCTimer * 1000);
		RoomSystem.playerEffectDictionary.Clear();
		foreach (RoomSystem.PlayerEffectConfig playerEffectConfig in this.roomSettings.PlayerEffects)
		{
			RoomSystem.playerEffectDictionary.Add(playerEffectConfig.type, playerEffectConfig);
		}
		this.roomSettings.ResyncNetworkTimeTimer.callback = new Action(PhotonNetwork.FetchServerTimestamp);
		RoomSystem.__roomSettings = this.roomSettings;
	}

	// Token: 0x060051E9 RID: 20969 RVA: 0x001AF530 File Offset: 0x001AD730
	private void Start()
	{
		List<PhotonView> list = new List<PhotonView>(20);
		foreach (PhotonView photonView in PhotonNetwork.PhotonViewCollection)
		{
			if (photonView.IsRoomView)
			{
				list.Add(photonView);
			}
		}
		RoomSystem.sceneViews = list.ToArray();
		NetworkSystem.Instance.OnRaiseEvent += RoomSystem.OnEvent;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerEnteredRoom;
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnLeftRoom;
	}

	// Token: 0x060051EA RID: 20970 RVA: 0x001AF63C File Offset: 0x001AD83C
	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			RoomSystem.disconnectTimer.Stop();
			return;
		}
		if (RoomSystem.JoinedRoom)
		{
			RoomSystem.disconnectTimer.Start();
		}
	}

	// Token: 0x060051EB RID: 20971 RVA: 0x001AF660 File Offset: 0x001AD860
	private void OnJoinedRoom()
	{
		RoomSystem.joinedRoom = true;
		foreach (NetPlayer item in NetworkSystem.Instance.AllNetPlayers)
		{
			RoomSystem.netPlayersInRoom.Add(item);
		}
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(RoomSystem.netPlayersInRoom);
		RoomSystem.roomGameMode = NetworkSystem.Instance.GameModeString;
		RoomSystem.WasRoomPrivate = NetworkSystem.Instance.SessionIsPrivate;
		RoomSystem.WasRoomSubscription = NetworkSystem.Instance.SessionIsSubscription;
		RoomSystem.IsVStumpRoom = NetworkSystem.Instance.RoomName.StartsWith(GorillaComputer.instance.VStumpRoomPrepend);
		RoomSystem.InitialJoinTrigger = GorillaComputer.instance.GetJoinTriggerFromFullGameModeString(RoomSystem.roomGameMode);
		if (!RoomSystem.WasRoomPrivate)
		{
			RoomSystem.WasRoomSubscription = PhotonNetwork.CurrentRoom.Name.EndsWith(":GTFC");
		}
		bool wasRoomSubscription = RoomSystem.WasRoomSubscription;
		if (NetworkSystem.Instance.IsMasterClient)
		{
			for (int j = 0; j < this.prefabsToInstantiateByPath.Length; j++)
			{
				this.prefabsInstantiated.Add(NetworkSystem.Instance.NetInstantiate(this.prefabsToInstantiate[j], Vector3.zero, Quaternion.identity, true));
			}
		}
		try
		{
			RoomSystem.m_roomSizeOnJoin = PhotonNetwork.CurrentRoom.MaxPlayers;
			this.roomSettings.ExpectedUsersTimer.Start();
			this.roomSettings.ResyncNetworkTimeTimer.Start();
			DelegateListProcessor joinedRoomEvent = RoomSystem.JoinedRoomEvent;
			if (joinedRoomEvent != null)
			{
				joinedRoomEvent.InvokeSafe();
			}
			this.roomSettings.ResyncNetworkTimeTimer.OnTimedEvent();
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	// Token: 0x060051EC RID: 20972 RVA: 0x001AF7E4 File Offset: 0x001AD9E4
	private void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		if (newPlayer.IsLocal)
		{
			return;
		}
		Debug.Log(string.Format("Player {0} entered room", (newPlayer != null) ? new int?(newPlayer.ActorNumber) : null));
		if (!RoomSystem.netPlayersInRoom.Contains(newPlayer))
		{
			RoomSystem.netPlayersInRoom.Add(newPlayer);
		}
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(newPlayer);
		try
		{
			DelegateListProcessor<NetPlayer> playerJoinedEvent = RoomSystem.PlayerJoinedEvent;
			if (playerJoinedEvent != null)
			{
				playerJoinedEvent.InvokeSafe(newPlayer);
			}
			DelegateListProcessor playersChangedEvent = RoomSystem.PlayersChangedEvent;
			if (playersChangedEvent != null)
			{
				playersChangedEvent.InvokeSafe();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	// Token: 0x060051ED RID: 20973 RVA: 0x001AF888 File Offset: 0x001ADA88
	private void OnLeftRoom()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		RoomSystem.joinedRoom = false;
		RoomSystem.netPlayersInRoom.Clear();
		RoomSystem.roomGameMode = "";
		PlayerCosmeticsSystem.StaticReset();
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		for (int i = 0; i < RoomSystem.sceneViews.Length; i++)
		{
			RoomSystem.sceneViews[i].ControllerActorNr = actorNumber;
			RoomSystem.sceneViews[i].OwnerActorNr = actorNumber;
		}
		this.roomSettings.StatusEffectLimiter.Reset();
		this.roomSettings.SoundEffectLimiter.Reset();
		this.roomSettings.SoundEffectOtherLimiter.Reset();
		this.roomSettings.PlayerEffectLimiter.Reset();
		this.roomSettings.LavaSyncLimiter.Reset();
		try
		{
			RoomSystem.m_roomSizeOnJoin = 0;
			this.roomSettings.ExpectedUsersTimer.Stop();
			this.roomSettings.ResyncNetworkTimeTimer.Stop();
			DelegateListProcessor leftRoomEvent = RoomSystem.LeftRoomEvent;
			if (leftRoomEvent != null)
			{
				leftRoomEvent.InvokeSafe();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
		finally
		{
			RoomSystem.WasRoomSubscription = false;
			RoomSystem.InitialJoinTrigger = null;
		}
		GC.Collect(0);
	}

	// Token: 0x060051EE RID: 20974 RVA: 0x001AF9BC File Offset: 0x001ADBBC
	private void OnPlayerLeftRoom(NetPlayer netPlayer)
	{
		if (netPlayer == null)
		{
			Debug.LogError("Player that left doesn't have a reference somehow...");
		}
		RoomSystem.netPlayersInRoom.Remove(netPlayer);
		try
		{
			DelegateListProcessor<NetPlayer> playerLeftEvent = RoomSystem.PlayerLeftEvent;
			if (playerLeftEvent != null)
			{
				playerLeftEvent.InvokeSafe(netPlayer);
			}
			DelegateListProcessor playersChangedEvent = RoomSystem.PlayersChangedEvent;
			if (playersChangedEvent != null)
			{
				playersChangedEvent.InvokeSafe();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	// Token: 0x170007A9 RID: 1961
	// (get) Token: 0x060051EF RID: 20975 RVA: 0x001AFA24 File Offset: 0x001ADC24
	// (set) Token: 0x060051F0 RID: 20976 RVA: 0x001AFA2B File Offset: 0x001ADC2B
	private static bool UseRoomSizeOverride { get; set; }

	// Token: 0x170007AA RID: 1962
	// (get) Token: 0x060051F1 RID: 20977 RVA: 0x001AFA33 File Offset: 0x001ADC33
	// (set) Token: 0x060051F2 RID: 20978 RVA: 0x001AFA3A File Offset: 0x001ADC3A
	public static byte RoomSizeOverride { get; set; }

	// Token: 0x170007AB RID: 1963
	// (get) Token: 0x060051F3 RID: 20979 RVA: 0x001AFA42 File Offset: 0x001ADC42
	// (set) Token: 0x060051F4 RID: 20980 RVA: 0x001AFA49 File Offset: 0x001ADC49
	public static byte RoomSizeReduction { get; set; }

	// Token: 0x170007AC RID: 1964
	// (get) Token: 0x060051F5 RID: 20981 RVA: 0x001AFA51 File Offset: 0x001ADC51
	public static List<NetPlayer> PlayersInRoom
	{
		get
		{
			return RoomSystem.netPlayersInRoom;
		}
	}

	// Token: 0x170007AD RID: 1965
	// (get) Token: 0x060051F6 RID: 20982 RVA: 0x001AFA58 File Offset: 0x001ADC58
	public static string RoomGameMode
	{
		get
		{
			return RoomSystem.roomGameMode;
		}
	}

	// Token: 0x170007AE RID: 1966
	// (get) Token: 0x060051F7 RID: 20983 RVA: 0x001AFA5F File Offset: 0x001ADC5F
	public static bool JoinedRoom
	{
		get
		{
			return NetworkSystem.Instance.InRoom && RoomSystem.joinedRoom;
		}
	}

	// Token: 0x170007AF RID: 1967
	// (get) Token: 0x060051F8 RID: 20984 RVA: 0x001AFA74 File Offset: 0x001ADC74
	public static bool AmITheHost
	{
		get
		{
			return NetworkSystem.Instance.IsMasterClient || !NetworkSystem.Instance.InRoom;
		}
	}

	// Token: 0x170007B0 RID: 1968
	// (get) Token: 0x060051F9 RID: 20985 RVA: 0x001AFA91 File Offset: 0x001ADC91
	// (set) Token: 0x060051FA RID: 20986 RVA: 0x001AFA98 File Offset: 0x001ADC98
	public static bool IsVStumpRoom { get; private set; }

	// Token: 0x170007B1 RID: 1969
	// (get) Token: 0x060051FB RID: 20987 RVA: 0x001AFAA0 File Offset: 0x001ADCA0
	// (set) Token: 0x060051FC RID: 20988 RVA: 0x001AFAA7 File Offset: 0x001ADCA7
	public static bool WasRoomPrivate { get; private set; }

	// Token: 0x170007B2 RID: 1970
	// (get) Token: 0x060051FD RID: 20989 RVA: 0x001AFAAF File Offset: 0x001ADCAF
	// (set) Token: 0x060051FE RID: 20990 RVA: 0x001AFAB6 File Offset: 0x001ADCB6
	public static bool WasRoomSubscription { get; private set; }

	// Token: 0x170007B3 RID: 1971
	// (get) Token: 0x060051FF RID: 20991 RVA: 0x001AFABE File Offset: 0x001ADCBE
	// (set) Token: 0x06005200 RID: 20992 RVA: 0x001AFAC5 File Offset: 0x001ADCC5
	public static GorillaNetworkJoinTrigger InitialJoinTrigger { get; private set; }

	// Token: 0x06005201 RID: 20993 RVA: 0x001AFAD0 File Offset: 0x001ADCD0
	static RoomSystem()
	{
		RoomSystem.disconnectTimer.Elapsed += RoomSystem.TimerDC;
		RoomSystem.disconnectTimer.AutoReset = false;
		RoomSystem.StaticLoad();
	}

	// Token: 0x06005202 RID: 20994 RVA: 0x001AFC48 File Offset: 0x001ADE48
	[OnEnterPlay_Run]
	private static void StaticLoad()
	{
		RoomSystem.netEventCallbacks[0] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeLaunchProjectile);
		RoomSystem.netEventCallbacks[1] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeImpactEffect);
		RoomSystem.netEventCallbacks[4] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForNearby);
		RoomSystem.netEventCallbacks[7] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForParty);
		RoomSystem.netEventCallbacks[10] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForElevator);
		RoomSystem.netEventCallbacks[11] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForShuttle);
		RoomSystem.netEventCallbacks[2] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeStatusEffect);
		RoomSystem.netEventCallbacks[3] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeSoundEffect);
		RoomSystem.netEventCallbacks[5] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeReportTouch);
		RoomSystem.netEventCallbacks[8] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializePlayerLaunched);
		RoomSystem.netEventCallbacks[6] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializePlayerEffect);
		RoomSystem.netEventCallbacks[9] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializePlayerHit);
		RoomSystem.netEventCallbacks[12] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeLavaSync);
		RoomSystem.netEventCallbacks[13] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeMonkePointsRedeemed);
		RoomSystem.soundEffectCallback = new Action<RoomSystem.SoundEffect, NetPlayer>(RoomSystem.OnPlaySoundEffect);
		RoomSystem.statusEffectCallback = new Action<RoomSystem.StatusEffects>(RoomSystem.OnStatusEffect);
		VoxelManager.RegisterNetEventCallbacks();
	}

	// Token: 0x06005203 RID: 20995 RVA: 0x001AFDC3 File Offset: 0x001ADFC3
	private static void TimerDC(object sender, ElapsedEventArgs args)
	{
		RoomSystem.disconnectTimer.Stop();
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		PhotonNetwork.Disconnect();
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x06005204 RID: 20996 RVA: 0x001AFDE1 File Offset: 0x001ADFE1
	public static byte GetMaxRoomSize()
	{
		return (byte)RoomSystem.__roomSettings.GetRoomCount(true, true);
	}

	// Token: 0x06005205 RID: 20997 RVA: 0x001AFDF0 File Offset: 0x001ADFF0
	public static byte GetCurrentRoomExpectedSize()
	{
		if (!RoomSystem.joinedRoom)
		{
			return 10;
		}
		if (RoomSystem.IsVStumpRoom)
		{
			if (RoomSystem.m_roomSizeOnJoin >= 10)
			{
				return 10;
			}
			return RoomSystem.m_roomSizeOnJoin;
		}
		else
		{
			NetPlayer lowestActorNumberPlayer = RoomSystem.GetLowestActorNumberPlayer();
			RigContainer rigContainer;
			if (lowestActorNumberPlayer == null || !VRRigCache.Instance.TryGetVrrig(lowestActorNumberPlayer, out rigContainer))
			{
				return 10;
			}
			bool active = SubscriptionManager.GetSubscriptionDetails(lowestActorNumberPlayer).active;
			byte b;
			if (RoomSystem.WasRoomPrivate)
			{
				Room currentRoom = PhotonNetwork.CurrentRoom;
				if (!rigContainer.Rig.InitializedCosmetics)
				{
					b = currentRoom.MaxPlayers;
					if (b >= 20)
					{
						return 20;
					}
					return b;
				}
				else
				{
					b = (byte)RoomSystem.__roomSettings.GetRoomCount(true, active);
					if (!active && PhotonNetwork.CurrentRoom.PlayerCount > 10)
					{
						b = PhotonNetwork.CurrentRoom.PlayerCount;
					}
				}
			}
			else
			{
				GorillaNetworkJoinTrigger initialJoinTrigger = RoomSystem.InitialJoinTrigger;
				GTZone zone = GTZone.none;
				if (initialJoinTrigger.IsNotNull())
				{
					zone = initialJoinTrigger.zone;
				}
				b = (byte)RoomSystem.__roomSettings.GetRoomCount(zone, GameMode.CurrentGameModeType, false, RoomSystem.WasRoomSubscription);
			}
			if (b >= 20)
			{
				return 20;
			}
			return b;
		}
	}

	// Token: 0x06005206 RID: 20998 RVA: 0x001AFEE4 File Offset: 0x001AE0E4
	public static byte GetRoomSizeForCreate(GTZone zone, GameModeType mode, bool privateRoom, bool sub)
	{
		if (RoomSystem.UseRoomSizeOverride)
		{
			return RoomSystem.RoomSizeOverride;
		}
		return (byte)RoomSystem.__roomSettings.GetRoomCount(zone, mode, privateRoom, sub);
	}

	// Token: 0x06005207 RID: 20999 RVA: 0x001AFF02 File Offset: 0x001AE102
	public static void OverrideRoomSize(byte size)
	{
		if (size < 1)
		{
			size = 1;
		}
		else if (size > 10)
		{
			size = 10;
		}
		if (size == 10)
		{
			RoomSystem.UseRoomSizeOverride = false;
		}
		else
		{
			RoomSystem.UseRoomSizeOverride = true;
		}
		RoomSystem.RoomSizeOverride = size;
	}

	// Token: 0x06005208 RID: 21000 RVA: 0x001AFF2F File Offset: 0x001AE12F
	public static byte GetOverridenRoomSize()
	{
		if (RoomSystem.UseRoomSizeOverride)
		{
			return RoomSystem.RoomSizeOverride;
		}
		return 10;
	}

	// Token: 0x06005209 RID: 21001 RVA: 0x001AFF40 File Offset: 0x001AE140
	public static void ClearOverridenRoomSize()
	{
		RoomSystem.UseRoomSizeOverride = false;
		RoomSystem.RoomSizeOverride = 10;
	}

	// Token: 0x0600520A RID: 21002 RVA: 0x001AFF4F File Offset: 0x001AE14F
	public static void MakeRoomMultiplayer(byte roomSize)
	{
		if (!RoomSystem.joinedRoom || RoomSystem.m_roomSizeOnJoin > 1)
		{
			return;
		}
		if (roomSize > 20)
		{
			roomSize = 20;
		}
		RoomSystem.m_roomSizeOnJoin = roomSize;
		PhotonNetwork.CurrentRoom.MaxPlayers = roomSize;
	}

	// Token: 0x0600520B RID: 21003 RVA: 0x001AFF7C File Offset: 0x001AE17C
	public static NetPlayer GetLowestActorNumberPlayer()
	{
		if (!RoomSystem.joinedRoom || RoomSystem.netPlayersInRoom.Count == 0)
		{
			return null;
		}
		NetPlayer netPlayer = RoomSystem.netPlayersInRoom[0];
		for (int i = 1; i < RoomSystem.netPlayersInRoom.Count; i++)
		{
			NetPlayer netPlayer2 = RoomSystem.netPlayersInRoom[i];
			if (netPlayer2.ActorNumber < netPlayer.ActorNumber)
			{
				netPlayer = netPlayer2;
			}
		}
		return netPlayer;
	}

	// Token: 0x0600520C RID: 21004 RVA: 0x001AFFDC File Offset: 0x001AE1DC
	internal static void SendEvent(byte code, object[] evData, in NetPlayer target, bool reliable)
	{
		NetworkSystemRaiseEvent.neoTarget.TargetActors[0] = target.ActorNumber;
		RoomSystem.SendEvent(code, evData, NetworkSystemRaiseEvent.neoTarget, reliable);
	}

	// Token: 0x0600520D RID: 21005 RVA: 0x001AFFFE File Offset: 0x001AE1FE
	internal static void SendEvent(byte code, object[] evData, in NetEventOptions neo, bool reliable)
	{
		RoomSystem.sendEventData[0] = NetworkSystem.Instance.ServerTimestamp;
		RoomSystem.sendEventData[1] = code;
		RoomSystem.sendEventData[2] = evData;
		NetworkSystemRaiseEvent.RaiseEvent(3, RoomSystem.sendEventData, neo, reliable);
	}

	// Token: 0x0600520E RID: 21006 RVA: 0x001B0039 File Offset: 0x001AE239
	private static void OnEvent(EventData data)
	{
		RoomSystem.OnEvent(data.Code, data.CustomData, data.Sender);
	}

	// Token: 0x0600520F RID: 21007 RVA: 0x001B0054 File Offset: 0x001AE254
	private static void OnEvent(byte code, object data, int source)
	{
		NetPlayer netPlayer;
		if (code != 3 || !Utils.PlayerInRoom(source, out netPlayer))
		{
			return;
		}
		try
		{
			object[] array = (object[])data;
			int tick = Convert.ToInt32(array[0]);
			byte key = Convert.ToByte(array[1]);
			object[] arg = null;
			if (array.Length > 2)
			{
				object obj = array[2];
				arg = ((obj == null) ? null : ((object[])obj));
			}
			PhotonMessageInfoWrapped arg2 = new PhotonMessageInfoWrapped(netPlayer.ActorNumber, tick);
			Action<object[], PhotonMessageInfoWrapped> action;
			if (RoomSystem.netEventCallbacks.TryGetValue(key, out action))
			{
				action(arg, arg2);
			}
		}
		catch
		{
		}
	}

	// Token: 0x06005210 RID: 21008 RVA: 0x001B00E8 File Offset: 0x001AE2E8
	internal static void SearchForNearby(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "JoinPubWithNearby");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 23, NetworkSystem.Instance.SimTime))
		{
			return;
		}
		string shufflerStr = (string)shuffleData[0];
		string newKeyStr = (string)shuffleData[1];
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
		if (!GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId))
		{
			MonkeAgent.instance.SendReport("possible kick attempt", player.UserId, player.NickName);
			return;
		}
		if (!flag || !RoomSystem.WasRoomPrivate)
		{
			return;
		}
		PhotonNetworkController.Instance.AttemptToFollowIntoPub(player.UserId, player.ActorNumber, newKeyStr, shufflerStr, JoinType.FollowingNearby);
	}

	// Token: 0x06005211 RID: 21009 RVA: 0x001B01C8 File Offset: 0x001AE3C8
	internal static void SearchForParty(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "PARTY_JOIN");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 23, NetworkSystem.Instance.SimTime))
		{
			return;
		}
		string shufflerStr = (string)shuffleData[0];
		string newKeyStr = (string)shuffleData[1];
		if (!FriendshipGroupDetection.Instance.IsInMyGroup(info.Sender.UserId))
		{
			MonkeAgent.instance.SendReport("possible kick attempt", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (PlayFabAuthenticator.instance.GetSafety())
		{
			return;
		}
		PhotonNetworkController.Instance.AttemptToFollowIntoPub(info.Sender.UserId, info.Sender.ActorNumber, newKeyStr, shufflerStr, JoinType.FollowingParty);
	}

	// Token: 0x06005212 RID: 21010 RVA: 0x001B0298 File Offset: 0x001AE498
	internal static void SearchForElevator(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "JoinPubWithElevator");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 23, NetworkSystem.Instance.SimTime))
		{
			return;
		}
		string shufflerStr = (string)shuffleData[0];
		string newKeyStr = (string)shuffleData[1];
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
		if (GRElevatorManager.ValidElevatorNetworking(info.Sender.ActorNumber) && GRElevatorManager.ValidElevatorNetworking(NetworkSystem.Instance.LocalPlayer.ActorNumber))
		{
			if (!flag)
			{
				GRElevatorManager.JoinPublicRoom();
				return;
			}
			PhotonNetworkController.Instance.AttemptToFollowIntoPub(player.UserId, player.ActorNumber, newKeyStr, shufflerStr, JoinType.JoinWithElevator);
		}
	}

	// Token: 0x06005213 RID: 21011 RVA: 0x001B0358 File Offset: 0x001AE558
	internal static void SearchForShuttle(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "JoinPubWithElevator");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 23, NetworkSystem.Instance.SimTime))
		{
			return;
		}
		string shufflerStr = (string)shuffleData[0];
		string newKeyStr = (string)shuffleData[1];
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
		bool flag2 = GRElevatorManager.ValidShuttleNetworking(info.Sender.ActorNumber);
		bool flag3 = GRElevatorManager.ValidShuttleNetworking(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		if (flag2 && flag3)
		{
			if (!flag)
			{
				GRElevatorManager.JoinPublicRoom();
				return;
			}
			PhotonNetworkController.Instance.AttemptToFollowIntoPub(player.UserId, player.ActorNumber, newKeyStr, shufflerStr, JoinType.JoinWithElevator);
		}
	}

	// Token: 0x06005214 RID: 21012 RVA: 0x001B041C File Offset: 0x001AE61C
	internal static void SendNearbyFollowCommand(GorillaFriendCollider friendCollider, string shuffler, string keyStr)
	{
		RoomSystem.groupJoinSendData[0] = shuffler;
		RoomSystem.groupJoinSendData[1] = keyStr;
		NetEventOptions netEventOptions = new NetEventOptions
		{
			TargetActors = new int[1]
		};
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) && netPlayer != NetworkSystem.Instance.LocalPlayer)
			{
				netEventOptions.TargetActors[0] = netPlayer.ActorNumber;
				RoomSystem.SendEvent(4, RoomSystem.groupJoinSendData, netEventOptions, false);
			}
		}
	}

	// Token: 0x06005215 RID: 21013 RVA: 0x001B04C8 File Offset: 0x001AE6C8
	internal static void SendPartyFollowCommand(string shuffler, string keyStr)
	{
		RoomSystem.groupJoinSendData[0] = shuffler;
		RoomSystem.groupJoinSendData[1] = keyStr;
		NetEventOptions netEventOptions = new NetEventOptions
		{
			TargetActors = new int[1]
		};
		foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
		{
			VRRig rig = rigContainer.Rig;
			if (rig.IsLocalPartyMember && rig.creator != NetworkSystem.Instance.LocalPlayer)
			{
				netEventOptions.TargetActors[0] = rig.creator.ActorNumber;
				RoomSystem.SendEvent(7, RoomSystem.groupJoinSendData, netEventOptions, false);
			}
		}
	}

	// Token: 0x06005216 RID: 21014 RVA: 0x001B0570 File Offset: 0x001AE770
	internal static void SendElevatorFollowCommand(string shuffler, string keyStr, GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider targetFriendCollider)
	{
		RoomSystem.SendGroupJoinFollowCommand(10, shuffler, keyStr, sourceFriendCollider, targetFriendCollider);
	}

	// Token: 0x06005217 RID: 21015 RVA: 0x001B057D File Offset: 0x001AE77D
	internal static void SendShuttleFollowCommand(string shuffler, string keyStr, GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider targetFriendCollider)
	{
		RoomSystem.SendGroupJoinFollowCommand(11, shuffler, keyStr, sourceFriendCollider, targetFriendCollider);
	}

	// Token: 0x06005218 RID: 21016 RVA: 0x001B058C File Offset: 0x001AE78C
	internal static void SendGroupJoinFollowCommand(byte eventType, string shuffler, string keyStr, GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider targetFriendCollider)
	{
		RoomSystem.groupJoinSendData[0] = shuffler;
		RoomSystem.groupJoinSendData[1] = keyStr;
		NetEventOptions netEventOptions = new NetEventOptions
		{
			TargetActors = new int[1]
		};
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (sourceFriendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) || (targetFriendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) && netPlayer != NetworkSystem.Instance.LocalPlayer))
			{
				netEventOptions.TargetActors[0] = netPlayer.ActorNumber;
				RoomSystem.SendEvent(eventType, RoomSystem.groupJoinSendData, netEventOptions, false);
			}
		}
	}

	// Token: 0x06005219 RID: 21017 RVA: 0x001B064C File Offset: 0x001AE84C
	private static void DeserializeReportTouch(object[] data, PhotonMessageInfoWrapped info)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer arg = (NetPlayer)data[0];
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		Action<NetPlayer, NetPlayer> action = RoomSystem.playerTouchedCallback;
		if (action == null)
		{
			return;
		}
		action(arg, player);
	}

	// Token: 0x0600521A RID: 21018 RVA: 0x001B0694 File Offset: 0x001AE894
	internal static void SendReportTouch(NetPlayer touchedNetPlayer)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			RoomSystem.reportTouchSendData[0] = touchedNetPlayer;
			RoomSystem.SendEvent(5, RoomSystem.reportTouchSendData, NetworkSystemRaiseEvent.neoMaster, false);
			return;
		}
		Action<NetPlayer, NetPlayer> action = RoomSystem.playerTouchedCallback;
		if (action == null)
		{
			return;
		}
		action(touchedNetPlayer, NetworkSystem.Instance.LocalPlayer);
	}

	// Token: 0x0600521B RID: 21019 RVA: 0x001B06E1 File Offset: 0x001AE8E1
	internal static void LaunchPlayer(NetPlayer player, Vector3 velocity)
	{
		RoomSystem.reportTouchSendData[0] = velocity;
		RoomSystem.SendEvent(8, RoomSystem.reportTouchSendData, player, false);
	}

	// Token: 0x0600521C RID: 21020 RVA: 0x001B0700 File Offset: 0x001AE900
	private static void DeserializePlayerLaunched(object[] data, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "DeserializePlayerLaunched");
		GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
		if (activeGameMode != null && activeGameMode.GameType() == GameModeType.Guardian && info.Sender == NetworkSystem.Instance.MasterClient)
		{
			object obj = data[0];
			if (obj is Vector3)
			{
				Vector3 velocity = (Vector3)obj;
				float num = 10000f;
				if (velocity.IsValid(num) && velocity.magnitude <= 20f && RoomSystem.playerLaunchedCallLimiter.CheckCallTime(Time.time))
				{
					GTPlayer.Instance.DoLaunch(velocity);
					return;
				}
			}
		}
	}

	// Token: 0x0600521D RID: 21021 RVA: 0x001B0794 File Offset: 0x001AE994
	internal static void HitPlayer(NetPlayer player, Vector3 direction, float strength)
	{
		RoomSystem.reportHitSendData[0] = direction;
		RoomSystem.reportHitSendData[1] = strength;
		RoomSystem.reportHitSendData[2] = player.ActorNumber;
		RoomSystem.SendEvent(9, RoomSystem.reportHitSendData, NetworkSystemRaiseEvent.neoOthers, false);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			rigContainer.Rig.DisableHitWithKnockBack();
		}
	}

	// Token: 0x0600521E RID: 21022 RVA: 0x001B07FC File Offset: 0x001AE9FC
	private static void DeserializePlayerHit(object[] data, PhotonMessageInfoWrapped info)
	{
		object obj = data[0];
		if (obj is Vector3)
		{
			Vector3 vector = (Vector3)obj;
			obj = data[1];
			if (obj is float)
			{
				float value = (float)obj;
				obj = data[2];
				if (obj is int)
				{
					int num = (int)obj;
					float num2 = 10000f;
					RigContainer rigContainer;
					if (vector.IsValid(num2) && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) && FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 20, info.SentServerTime))
					{
						float num3 = value.ClampSafe(0f, 10f);
						MonkeAgent.IncrementRPCCall(info, "DeserializePlayerHit");
						if (num == NetworkSystem.Instance.LocalPlayer.ActorNumber)
						{
							CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
							CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect2;
							if (GorillaTagger.Instance.offlineVRRig.TemporaryCosmeticEffects.TryGetValue(CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback, out cosmeticEffect))
							{
								if (!cosmeticEffect.IsGameModeAllowed())
								{
									return;
								}
								float num4 = (num3 * cosmeticEffect.knockbackStrength * cosmeticEffect.knockbackStrengthMultiplier).ClampSafe(cosmeticEffect.minKnockbackStrength, cosmeticEffect.maxKnockbackStrength);
								if (cosmeticEffect.applyScaleToKnockbackStrength)
								{
									num4 *= GTPlayer.Instance.scale;
								}
								GTPlayer.Instance.ApplyKnockback(vector.normalized, num4, cosmeticEffect.forceOffTheGround);
							}
							else if (GorillaTagger.Instance.offlineVRRig.TemporaryCosmeticEffects.TryGetValue(CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback, out cosmeticEffect2))
							{
								if (!cosmeticEffect2.IsGameModeAllowed())
								{
									return;
								}
								float num5 = (num3 * cosmeticEffect2.knockbackStrength * cosmeticEffect2.knockbackStrengthMultiplier).ClampSafe(cosmeticEffect2.minKnockbackStrength, cosmeticEffect2.maxKnockbackStrength);
								if (cosmeticEffect.applyScaleToKnockbackStrength)
								{
									num5 *= GTPlayer.Instance.scale;
								}
								GTPlayer.Instance.ApplyKnockback(vector.normalized, num5, cosmeticEffect2.forceOffTheGround);
							}
						}
						NetPlayer player = NetworkSystem.Instance.GetPlayer(num);
						RigContainer rigContainer2;
						if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer2))
						{
							rigContainer2.Rig.DisableHitWithKnockBack();
						}
						return;
					}
				}
			}
		}
	}

	// Token: 0x0600521F RID: 21023 RVA: 0x001B09E8 File Offset: 0x001AEBE8
	private static void SetSlowedTime()
	{
		if (GorillaTagger.Instance.currentStatus != GorillaTagger.StatusEffect.Slowed)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		}
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Slowed, GorillaTagger.Instance.slowCooldown);
		GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
	}

	// Token: 0x06005220 RID: 21024 RVA: 0x001B0A64 File Offset: 0x001AEC64
	private static void SetTaggedTime()
	{
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
	}

	// Token: 0x06005221 RID: 21025 RVA: 0x001B0AD4 File Offset: 0x001AECD4
	private static void SetFrozenTime()
	{
		GorillaFreezeTagManager gorillaFreezeTagManager = GameMode.ActiveGameMode as GorillaFreezeTagManager;
		if (gorillaFreezeTagManager != null)
		{
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Slowed, gorillaFreezeTagManager.freezeDuration);
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
		}
	}

	// Token: 0x06005222 RID: 21026 RVA: 0x001B0B4D File Offset: 0x001AED4D
	private static void SetJoinedTaggedTime()
	{
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
	}

	// Token: 0x06005223 RID: 21027 RVA: 0x001B0B90 File Offset: 0x001AED90
	private static void SetUntaggedTime()
	{
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.None, 0f);
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
	}

	// Token: 0x06005224 RID: 21028 RVA: 0x001B0BEB File Offset: 0x001AEDEB
	private static void OnStatusEffect(RoomSystem.StatusEffects status)
	{
		switch (status)
		{
		case RoomSystem.StatusEffects.TaggedTime:
			RoomSystem.SetTaggedTime();
			return;
		case RoomSystem.StatusEffects.JoinedTaggedTime:
			RoomSystem.SetJoinedTaggedTime();
			return;
		case RoomSystem.StatusEffects.SetSlowedTime:
			RoomSystem.SetSlowedTime();
			return;
		case RoomSystem.StatusEffects.UnTagged:
			RoomSystem.SetUntaggedTime();
			return;
		case RoomSystem.StatusEffects.FrozenTime:
			RoomSystem.SetFrozenTime();
			return;
		default:
			return;
		}
	}

	// Token: 0x06005225 RID: 21029 RVA: 0x001B0C28 File Offset: 0x001AEE28
	private static void DeserializeStatusEffect(object[] data, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "DeserializeStatusEffect");
		if (!player.IsMasterClient)
		{
			MonkeAgent.instance.SendReport("invalid status", player.UserId, player.NickName);
			return;
		}
		if (!RoomSystem.callbackInstance.roomSettings.StatusEffectLimiter.CheckCallServerTime(info.SentServerTime))
		{
			return;
		}
		RoomSystem.StatusEffects obj = (RoomSystem.StatusEffects)Convert.ToInt32(data[0]);
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action == null)
		{
			return;
		}
		action(obj);
	}

	// Token: 0x06005226 RID: 21030 RVA: 0x001B0CAE File Offset: 0x001AEEAE
	internal static void SendStatusEffectAll(RoomSystem.StatusEffects status)
	{
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action != null)
		{
			action(status);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.statusSendData[0] = (int)status;
		RoomSystem.SendEvent(2, RoomSystem.statusSendData, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x06005227 RID: 21031 RVA: 0x001B0CE7 File Offset: 0x001AEEE7
	internal static void SendStatusEffectToPlayer(RoomSystem.StatusEffects status, NetPlayer target)
	{
		if (!target.IsLocal)
		{
			RoomSystem.statusSendData[0] = (int)status;
			RoomSystem.SendEvent(2, RoomSystem.statusSendData, target, false);
			return;
		}
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action == null)
		{
			return;
		}
		action(status);
	}

	// Token: 0x06005228 RID: 21032 RVA: 0x001B0D1D File Offset: 0x001AEF1D
	internal static void PlaySoundEffect(int soundIndex, float soundVolume, bool stopCurrentAudio)
	{
		VRRigCache.Instance.localRig.Rig.PlayTagSoundLocal(soundIndex, soundVolume, stopCurrentAudio);
	}

	// Token: 0x06005229 RID: 21033 RVA: 0x001B0D38 File Offset: 0x001AEF38
	internal static void PlaySoundEffect(int soundIndex, float soundVolume, bool stopCurrentAudio, NetPlayer target)
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(target, out rigContainer))
		{
			rigContainer.Rig.PlayTagSoundLocal(soundIndex, soundVolume, stopCurrentAudio);
		}
	}

	// Token: 0x0600522A RID: 21034 RVA: 0x001B0D62 File Offset: 0x001AEF62
	private static void OnPlaySoundEffect(RoomSystem.SoundEffect sound, NetPlayer target)
	{
		if (target.IsLocal)
		{
			RoomSystem.PlaySoundEffect(sound.id, sound.volume, sound.stopCurrentAudio);
			return;
		}
		RoomSystem.PlaySoundEffect(sound.id, sound.volume, sound.stopCurrentAudio, target);
	}

	// Token: 0x0600522B RID: 21035 RVA: 0x001B0D9C File Offset: 0x001AEF9C
	private static void DeserializeSoundEffect(object[] data, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "DeserializeSoundEffect");
		if (!player.Equals(RoomSystem.GetLowestActorNumberPlayer()))
		{
			MonkeAgent.instance.SendReport("invalid sound effect", player.UserId, player.NickName);
			return;
		}
		RoomSystem.SoundEffect soundEffect;
		soundEffect.id = Convert.ToInt32(data[0]);
		soundEffect.volume = Convert.ToSingle(data[1]);
		soundEffect.stopCurrentAudio = Convert.ToBoolean(data[2]);
		if (!float.IsFinite(soundEffect.volume))
		{
			return;
		}
		NetPlayer netPlayer;
		if (data.Length > 3)
		{
			if (!RoomSystem.callbackInstance.roomSettings.SoundEffectOtherLimiter.CheckCallServerTime(info.SentServerTime))
			{
				return;
			}
			int playerID = Convert.ToInt32(data[3]);
			netPlayer = NetworkSystem.Instance.GetPlayer(playerID);
		}
		else
		{
			if (!RoomSystem.callbackInstance.roomSettings.SoundEffectLimiter.CheckCallServerTime(info.SentServerTime))
			{
				return;
			}
			netPlayer = NetworkSystem.Instance.LocalPlayer;
		}
		if (netPlayer != null)
		{
			RoomSystem.soundEffectCallback(soundEffect, netPlayer);
		}
	}

	// Token: 0x0600522C RID: 21036 RVA: 0x001B0EA0 File Offset: 0x001AF0A0
	internal static void SendSoundEffectAll(int soundIndex, float soundVolume, bool stopCurrentAudio = false)
	{
		RoomSystem.SendSoundEffectAll(new RoomSystem.SoundEffect(soundIndex, soundVolume, stopCurrentAudio));
	}

	// Token: 0x0600522D RID: 21037 RVA: 0x001B0EB0 File Offset: 0x001AF0B0
	internal static void SendSoundEffectAll(RoomSystem.SoundEffect sound)
	{
		Action<RoomSystem.SoundEffect, NetPlayer> action = RoomSystem.soundEffectCallback;
		if (action != null)
		{
			action(sound, NetworkSystem.Instance.LocalPlayer);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.soundSendData[0] = sound.id;
		RoomSystem.soundSendData[1] = sound.volume;
		RoomSystem.soundSendData[2] = sound.stopCurrentAudio;
		RoomSystem.SendEvent(3, RoomSystem.soundSendData, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x0600522E RID: 21038 RVA: 0x001B0F27 File Offset: 0x001AF127
	internal static void SendSoundEffectToPlayer(int soundIndex, float soundVolume, NetPlayer player, bool stopCurrentAudio = false)
	{
		RoomSystem.SendSoundEffectToPlayer(new RoomSystem.SoundEffect(soundIndex, soundVolume, stopCurrentAudio), player);
	}

	// Token: 0x0600522F RID: 21039 RVA: 0x001B0F38 File Offset: 0x001AF138
	internal static void SendSoundEffectToPlayer(RoomSystem.SoundEffect sound, NetPlayer player)
	{
		if (player.IsLocal)
		{
			Action<RoomSystem.SoundEffect, NetPlayer> action = RoomSystem.soundEffectCallback;
			if (action == null)
			{
				return;
			}
			action(sound, player);
			return;
		}
		else
		{
			if (!RoomSystem.joinedRoom)
			{
				return;
			}
			RoomSystem.soundSendData[0] = sound.id;
			RoomSystem.soundSendData[1] = sound.volume;
			RoomSystem.soundSendData[2] = sound.stopCurrentAudio;
			RoomSystem.SendEvent(3, RoomSystem.soundSendData, player, false);
			return;
		}
	}

	// Token: 0x06005230 RID: 21040 RVA: 0x001B0FAB File Offset: 0x001AF1AB
	internal static void SendSoundEffectOnOther(int soundIndex, float soundvolume, NetPlayer target, bool stopCurrentAudio = false)
	{
		RoomSystem.SendSoundEffectOnOther(new RoomSystem.SoundEffect(soundIndex, soundvolume, stopCurrentAudio), target);
	}

	// Token: 0x06005231 RID: 21041 RVA: 0x001B0FBC File Offset: 0x001AF1BC
	internal static void SendSoundEffectOnOther(RoomSystem.SoundEffect sound, NetPlayer target)
	{
		Action<RoomSystem.SoundEffect, NetPlayer> action = RoomSystem.soundEffectCallback;
		if (action != null)
		{
			action(sound, target);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.sendSoundDataOther[0] = sound.id;
		RoomSystem.sendSoundDataOther[1] = sound.volume;
		RoomSystem.sendSoundDataOther[2] = sound.stopCurrentAudio;
		RoomSystem.sendSoundDataOther[3] = target.ActorNumber;
		RoomSystem.SendEvent(3, RoomSystem.sendSoundDataOther, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x06005232 RID: 21042 RVA: 0x001B103C File Offset: 0x001AF23C
	internal static void OnPlayerEffect(PlayerEffect effect, NetPlayer target)
	{
		if (target == null)
		{
			return;
		}
		RoomSystem.PlayerEffectConfig playerEffectConfig;
		RigContainer rigContainer;
		if (RoomSystem.playerEffectDictionary.TryGetValue(effect, out playerEffectConfig) && VRRigCache.Instance.TryGetVrrig(target, out rigContainer) && rigContainer != null && rigContainer.Rig != null && playerEffectConfig.tagEffectPack != null)
		{
			TagEffectsLibrary.PlayEffect(rigContainer.Rig.transform, false, rigContainer.Rig.scaleFactor, target.IsLocal ? TagEffectsLibrary.EffectType.FIRST_PERSON : TagEffectsLibrary.EffectType.THIRD_PERSON, playerEffectConfig.tagEffectPack, playerEffectConfig.tagEffectPack, rigContainer.Rig.transform.rotation);
		}
	}

	// Token: 0x06005233 RID: 21043 RVA: 0x001B10D4 File Offset: 0x001AF2D4
	private static void DeserializePlayerEffect(object[] data, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "DeserializePlayerEffect");
		if (!RoomSystem.callbackInstance.roomSettings.PlayerEffectLimiter.CheckCallServerTime(info.SentServerTime))
		{
			return;
		}
		int playerID = Convert.ToInt32(data[0]);
		PlayerEffect effect = (PlayerEffect)Convert.ToInt32(data[1]);
		NetPlayer player = NetworkSystem.Instance.GetPlayer(playerID);
		RoomSystem.OnPlayerEffect(effect, player);
	}

	// Token: 0x06005234 RID: 21044 RVA: 0x001B1130 File Offset: 0x001AF330
	internal static void SendPlayerEffect(PlayerEffect effect, NetPlayer target)
	{
		RoomSystem.OnPlayerEffect(effect, target);
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.playerEffectData[0] = target.ActorNumber;
		RoomSystem.playerEffectData[1] = effect;
		RoomSystem.SendEvent(6, RoomSystem.playerEffectData, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x0400631D RID: 25373
	private static RoomSystem.ImpactFxContainer impactEffect = new RoomSystem.ImpactFxContainer();

	// Token: 0x0400631E RID: 25374
	private static RoomSystem.LaunchProjectileContainer launchProjectile = new RoomSystem.LaunchProjectileContainer();

	// Token: 0x0400631F RID: 25375
	public static GameObject playerImpactEffectPrefab = null;

	// Token: 0x04006320 RID: 25376
	private static readonly object[] projectileSendData = new object[9];

	// Token: 0x04006321 RID: 25377
	private static readonly object[] impactSendData = new object[6];

	// Token: 0x04006322 RID: 25378
	private static readonly List<int> hashValues = new List<int>(2);

	// Token: 0x04006323 RID: 25379
	[OnExitPlay_SetNull]
	internal static Action<RoomSystem.LavaSyncEventData> OnLavaSyncReceived;

	// Token: 0x04006324 RID: 25380
	private const int lavaSyncHeaderSize = 5;

	// Token: 0x04006325 RID: 25381
	private const int lavaSyncTotalSize = 25;

	// Token: 0x04006326 RID: 25382
	private static readonly object[] lavaSyncSendData = new object[25];

	// Token: 0x04006327 RID: 25383
	[OnExitPlay_SetNull]
	internal static Action<NetPlayer, int> OnMonkePointsRedeemedReceived;

	// Token: 0x04006328 RID: 25384
	private const int monkePointsRedeemedMaxCount = 50;

	// Token: 0x04006329 RID: 25385
	private static readonly object[] monkePointsRedeemedSendData = new object[1];

	// Token: 0x0400632A RID: 25386
	[SerializeField]
	private RoomSystemSettings roomSettings;

	// Token: 0x0400632B RID: 25387
	[SerializeField]
	private string[] prefabsToInstantiateByPath;

	// Token: 0x0400632C RID: 25388
	[SerializeField]
	private GameObject[] prefabsToInstantiate;

	// Token: 0x0400632D RID: 25389
	private List<GameObject> prefabsInstantiated = new List<GameObject>();

	// Token: 0x0400632E RID: 25390
	public static Dictionary<PlayerEffect, RoomSystem.PlayerEffectConfig> playerEffectDictionary = new Dictionary<PlayerEffect, RoomSystem.PlayerEffectConfig>();

	// Token: 0x0400632F RID: 25391
	private static RoomSystemSettings __roomSettings;

	// Token: 0x04006330 RID: 25392
	[OnEnterPlay_SetNull]
	private static RoomSystem callbackInstance;

	// Token: 0x04006333 RID: 25395
	private static byte m_roomSizeOnJoin;

	// Token: 0x04006335 RID: 25397
	[OnEnterPlay_Clear]
	private static List<NetPlayer> netPlayersInRoom = new List<NetPlayer>(20);

	// Token: 0x04006336 RID: 25398
	[OnEnterPlay_Set("")]
	private static string roomGameMode = "";

	// Token: 0x04006337 RID: 25399
	[OnEnterPlay_Set(false)]
	private static bool joinedRoom = false;

	// Token: 0x0400633C RID: 25404
	[OnEnterPlay_SetNull]
	private static PhotonView[] sceneViews;

	// Token: 0x0400633D RID: 25405
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor LeftRoomEvent = new DelegateListProcessor();

	// Token: 0x0400633E RID: 25406
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor JoinedRoomEvent = new DelegateListProcessor();

	// Token: 0x0400633F RID: 25407
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor<NetPlayer> PlayerJoinedEvent = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04006340 RID: 25408
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor<NetPlayer> PlayerLeftEvent = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04006341 RID: 25409
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor PlayersChangedEvent = new DelegateListProcessor();

	// Token: 0x04006342 RID: 25410
	private static Timer disconnectTimer = new Timer();

	// Token: 0x04006343 RID: 25411
	[OnExitPlay_Clear]
	internal static readonly Dictionary<byte, Action<object[], PhotonMessageInfoWrapped>> netEventCallbacks = new Dictionary<byte, Action<object[], PhotonMessageInfoWrapped>>(20);

	// Token: 0x04006344 RID: 25412
	private static readonly object[] sendEventData = new object[3];

	// Token: 0x04006345 RID: 25413
	private static readonly object[] groupJoinSendData = new object[2];

	// Token: 0x04006346 RID: 25414
	private static readonly object[] reportTouchSendData = new object[1];

	// Token: 0x04006347 RID: 25415
	private static readonly object[] reportHitSendData = new object[3];

	// Token: 0x04006348 RID: 25416
	[OnExitPlay_SetNull]
	public static Action<NetPlayer, NetPlayer> playerTouchedCallback;

	// Token: 0x04006349 RID: 25417
	private static CallLimiter playerLaunchedCallLimiter = new CallLimiter(3, 15f, 0.5f);

	// Token: 0x0400634A RID: 25418
	private static CallLimiter hitPlayerCallLimiter = new CallLimiter(10, 2f, 0.5f);

	// Token: 0x0400634B RID: 25419
	private static object[] statusSendData = new object[1];

	// Token: 0x0400634C RID: 25420
	public static Action<RoomSystem.StatusEffects> statusEffectCallback;

	// Token: 0x0400634D RID: 25421
	private static object[] soundSendData = new object[3];

	// Token: 0x0400634E RID: 25422
	private static object[] sendSoundDataOther = new object[4];

	// Token: 0x0400634F RID: 25423
	public static Action<RoomSystem.SoundEffect, NetPlayer> soundEffectCallback;

	// Token: 0x04006350 RID: 25424
	private static object[] playerEffectData = new object[2];

	// Token: 0x02000CE8 RID: 3304
	private class ImpactFxContainer : IFXContext
	{
		// Token: 0x170007B4 RID: 1972
		// (get) Token: 0x06005236 RID: 21046 RVA: 0x001B118F File Offset: 0x001AF38F
		public FXSystemSettings settings
		{
			get
			{
				return this.targetRig.fxSettings;
			}
		}

		// Token: 0x06005237 RID: 21047 RVA: 0x001B119C File Offset: 0x001AF39C
		public virtual void OnPlayFX()
		{
			NetPlayer creator = this.targetRig.creator;
			ProjectileTracker.ProjectileInfo projectileInfo;
			if (this.targetRig.isOfflineVRRig)
			{
				projectileInfo = ProjectileTracker.GetLocalProjectile(this.projectileIndex);
			}
			else
			{
				ValueTuple<bool, ProjectileTracker.ProjectileInfo> andRemoveRemotePlayerProjectile = ProjectileTracker.GetAndRemoveRemotePlayerProjectile(creator, this.projectileIndex);
				if (!andRemoveRemotePlayerProjectile.Item1)
				{
					return;
				}
				projectileInfo = andRemoveRemotePlayerProjectile.Item2;
			}
			SlingshotProjectile projectileInstance = projectileInfo.projectileInstance;
			GameObject obj = projectileInfo.hasImpactOverride ? projectileInstance.playerImpactEffectPrefab : RoomSystem.playerImpactEffectPrefab;
			GameObject gameObject = ObjectPools.instance.Instantiate(obj, this.position, true);
			gameObject.transform.localScale = Vector3.one * this.targetRig.scaleFactor;
			GorillaColorizableBase gorillaColorizableBase;
			if (gameObject.TryGetComponent<GorillaColorizableBase>(out gorillaColorizableBase))
			{
				gorillaColorizableBase.SetColor(this.colour);
			}
			SurfaceImpactFX component = gameObject.GetComponent<SurfaceImpactFX>();
			if (component != null)
			{
				component.SetScale(projectileInstance.transform.localScale.x * projectileInstance.impactEffectScaleMultiplier);
			}
			SoundBankPlayer component2 = gameObject.GetComponent<SoundBankPlayer>();
			if (component2 != null && !component2.playOnEnable)
			{
				component2.Play(projectileInstance.impactSoundVolumeOverride, projectileInstance.impactSoundPitchOverride);
			}
			if (projectileInstance.gameObject.activeSelf && projectileInstance.projectileOwner == creator)
			{
				projectileInstance.Deactivate();
			}
		}

		// Token: 0x04006351 RID: 25425
		public VRRig targetRig;

		// Token: 0x04006352 RID: 25426
		public Vector3 position;

		// Token: 0x04006353 RID: 25427
		public Color colour;

		// Token: 0x04006354 RID: 25428
		public int projectileIndex;
	}

	// Token: 0x02000CE9 RID: 3305
	private class LaunchProjectileContainer : RoomSystem.ImpactFxContainer
	{
		// Token: 0x06005239 RID: 21049 RVA: 0x001B12D0 File Offset: 0x001AF4D0
		public override void OnPlayFX()
		{
			GameObject gameObject = null;
			SlingshotProjectile slingshotProjectile = null;
			try
			{
				switch (this.projectileSource)
				{
				case RoomSystem.ProjectileSource.ProjectileWeapon:
					if (this.targetRig.projectileWeapon.IsNotNull() && this.targetRig.projectileWeapon.IsNotNull())
					{
						this.velocity = this.targetRig.ClampVelocityRelativeToPlayerSafe(this.velocity, 70f, 100f);
						SlingshotProjectile slingshotProjectile2 = this.targetRig.projectileWeapon.LaunchNetworkedProjectile(this.position, this.velocity, this.projectileSource, this.projectileIndex, this.targetRig.scaleFactor, this.overridecolour, this.colour, this.messageInfo);
						if (slingshotProjectile2.IsNotNull())
						{
							ProjectileTracker.AddRemotePlayerProjectile(this.messageInfo.Sender, slingshotProjectile2, this.projectileIndex, this.messageInfo.SentServerTime, this.velocity, this.position, this.targetRig.scaleFactor);
						}
					}
					return;
				case RoomSystem.ProjectileSource.LeftHand:
					this.tempThrowableGO = this.targetRig.myBodyDockPositions.GetLeftHandThrowable();
					break;
				case RoomSystem.ProjectileSource.RightHand:
					this.tempThrowableGO = this.targetRig.myBodyDockPositions.GetRightHandThrowable();
					break;
				default:
					return;
				}
				if (!this.tempThrowableGO.IsNull() && this.tempThrowableGO.TryGetComponent<SnowballThrowable>(out this.tempThrowableRef) && !(this.tempThrowableRef is GrowingSnowballThrowable))
				{
					this.velocity = this.targetRig.ClampVelocityRelativeToPlayerSafe(this.velocity, 50f, 100f);
					int projectileHash = this.tempThrowableRef.ProjectileHash;
					gameObject = ObjectPools.instance.Instantiate(projectileHash, true);
					slingshotProjectile = gameObject.GetComponent<SlingshotProjectile>();
					ProjectileTracker.AddRemotePlayerProjectile(this.targetRig.creator, slingshotProjectile, this.projectileIndex, this.messageInfo.SentServerTime, this.velocity, this.position, this.targetRig.scaleFactor);
					slingshotProjectile.Launch(this.position, this.velocity, this.messageInfo.Sender, false, false, this.projectileIndex, this.targetRig.scaleFactor, this.overridecolour, this.colour);
				}
			}
			catch
			{
				if (slingshotProjectile != null && slingshotProjectile)
				{
					slingshotProjectile.transform.position = Vector3.zero;
					slingshotProjectile.Deactivate();
				}
				else if (gameObject.IsNotNull())
				{
					ObjectPools.instance.Destroy(gameObject);
				}
			}
		}

		// Token: 0x04006355 RID: 25429
		public Vector3 velocity;

		// Token: 0x04006356 RID: 25430
		public RoomSystem.ProjectileSource projectileSource;

		// Token: 0x04006357 RID: 25431
		public bool overridecolour;

		// Token: 0x04006358 RID: 25432
		public PhotonMessageInfoWrapped messageInfo;

		// Token: 0x04006359 RID: 25433
		private GameObject tempThrowableGO;

		// Token: 0x0400635A RID: 25434
		private SnowballThrowable tempThrowableRef;
	}

	// Token: 0x02000CEA RID: 3306
	internal enum ProjectileSource
	{
		// Token: 0x0400635C RID: 25436
		ProjectileWeapon,
		// Token: 0x0400635D RID: 25437
		LeftHand,
		// Token: 0x0400635E RID: 25438
		RightHand
	}

	// Token: 0x02000CEB RID: 3307
	internal struct LavaSyncEventData
	{
		// Token: 0x0400635F RID: 25439
		public byte zone;

		// Token: 0x04006360 RID: 25440
		public byte state;

		// Token: 0x04006361 RID: 25441
		public double stateStartTime;

		// Token: 0x04006362 RID: 25442
		public float activationProgress;

		// Token: 0x04006363 RID: 25443
		public int voteCount;

		// Token: 0x04006364 RID: 25444
		public int senderActorNumber;

		// Token: 0x04006365 RID: 25445
		[FixedBuffer(typeof(int), 20)]
		public RoomSystem.LavaSyncEventData.<votes>e__FixedBuffer votes;

		// Token: 0x02000CEC RID: 3308
		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <votes>e__FixedBuffer
		{
			// Token: 0x04006366 RID: 25446
			public int FixedElementField;
		}
	}

	// Token: 0x02000CED RID: 3309
	internal struct Events
	{
		// Token: 0x04006367 RID: 25447
		public const byte PROJECTILE = 0;

		// Token: 0x04006368 RID: 25448
		public const byte IMPACT = 1;

		// Token: 0x04006369 RID: 25449
		public const byte STATUS_EFFECT = 2;

		// Token: 0x0400636A RID: 25450
		public const byte SOUND_EFFECT = 3;

		// Token: 0x0400636B RID: 25451
		public const byte NEARBY_JOIN = 4;

		// Token: 0x0400636C RID: 25452
		public const byte PLAYER_TOUCHED = 5;

		// Token: 0x0400636D RID: 25453
		public const byte PLAYER_EFFECT = 6;

		// Token: 0x0400636E RID: 25454
		public const byte PARTY_JOIN = 7;

		// Token: 0x0400636F RID: 25455
		public const byte PLAYER_LAUNCHED = 8;

		// Token: 0x04006370 RID: 25456
		public const byte PLAYER_HIT = 9;

		// Token: 0x04006371 RID: 25457
		public const byte ELEVATOR_JOIN = 10;

		// Token: 0x04006372 RID: 25458
		public const byte SHUTTLE_JOIN = 11;

		// Token: 0x04006373 RID: 25459
		public const byte LAVA_SYNC = 12;

		// Token: 0x04006374 RID: 25460
		public const byte MONKE_BIZ_STATION__POINTS_REDEEMED = 13;

		// Token: 0x04006375 RID: 25461
		public const byte VOX_REQ_WORLD = 100;

		// Token: 0x04006376 RID: 25462
		public const byte VOX_REQ_OPERATION = 101;

		// Token: 0x04006377 RID: 25463
		public const byte VOX_REQ_MINE = 102;

		// Token: 0x04006378 RID: 25464
		public const byte VOX_START_CHUNK = 103;

		// Token: 0x04006379 RID: 25465
		public const byte VOX_CONTINUE_CHUNK = 104;

		// Token: 0x0400637A RID: 25466
		public const byte VOX_SET_DENSITY = 105;

		// Token: 0x0400637B RID: 25467
		public const byte VOX_PLAY_FX = 106;

		// Token: 0x0400637C RID: 25468
		public const byte RPC = 255;
	}

	// Token: 0x02000CEE RID: 3310
	public enum StatusEffects
	{
		// Token: 0x0400637E RID: 25470
		TaggedTime,
		// Token: 0x0400637F RID: 25471
		JoinedTaggedTime,
		// Token: 0x04006380 RID: 25472
		SetSlowedTime,
		// Token: 0x04006381 RID: 25473
		UnTagged,
		// Token: 0x04006382 RID: 25474
		FrozenTime
	}

	// Token: 0x02000CEF RID: 3311
	public struct SoundEffect
	{
		// Token: 0x0600523B RID: 21051 RVA: 0x001B1558 File Offset: 0x001AF758
		public SoundEffect(int soundID, float soundVolume, bool _stopCurrentAudio)
		{
			this.id = soundID;
			this.volume = soundVolume;
			this.volume = soundVolume;
			this.stopCurrentAudio = _stopCurrentAudio;
		}

		// Token: 0x04006383 RID: 25475
		public int id;

		// Token: 0x04006384 RID: 25476
		public float volume;

		// Token: 0x04006385 RID: 25477
		public bool stopCurrentAudio;
	}

	// Token: 0x02000CF0 RID: 3312
	[Serializable]
	public struct PlayerEffectConfig
	{
		// Token: 0x04006386 RID: 25478
		public PlayerEffect type;

		// Token: 0x04006387 RID: 25479
		public TagEffectPack tagEffectPack;
	}
}
