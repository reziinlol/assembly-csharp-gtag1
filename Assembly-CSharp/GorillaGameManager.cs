using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000858 RID: 2136
public abstract class GorillaGameManager : MonoBehaviourPunCallbacks, ITickSystemTick, IWrappedSerializable, INetworkStruct
{
	// Token: 0x06003748 RID: 14152 RVA: 0x0012F546 File Offset: 0x0012D746
	public static string GameModeEnumToName(GameModeType gameMode)
	{
		return gameMode.ToString();
	}

	// Token: 0x14000062 RID: 98
	// (add) Token: 0x06003749 RID: 14153 RVA: 0x0012F558 File Offset: 0x0012D758
	// (remove) Token: 0x0600374A RID: 14154 RVA: 0x0012F58C File Offset: 0x0012D78C
	public static event GorillaGameManager.OnTouchDelegate OnTouch;

	// Token: 0x170004E7 RID: 1255
	// (get) Token: 0x0600374B RID: 14155 RVA: 0x0012F5BF File Offset: 0x0012D7BF
	public static GorillaGameManager instance
	{
		get
		{
			return GorillaGameModes.GameMode.ActiveGameMode;
		}
	}

	// Token: 0x170004E8 RID: 1256
	// (get) Token: 0x0600374C RID: 14156 RVA: 0x0012F5C6 File Offset: 0x0012D7C6
	// (set) Token: 0x0600374D RID: 14157 RVA: 0x0012F5CE File Offset: 0x0012D7CE
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x0600374E RID: 14158 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void Awake()
	{
	}

	// Token: 0x0600374F RID: 14159 RVA: 0x000028C5 File Offset: 0x00000AC5
	private new void OnEnable()
	{
	}

	// Token: 0x06003750 RID: 14160 RVA: 0x000028C5 File Offset: 0x00000AC5
	private new void OnDisable()
	{
	}

	// Token: 0x06003751 RID: 14161 RVA: 0x0012F5D8 File Offset: 0x0012D7D8
	public virtual void Tick()
	{
		if (this.lastCheck + this.checkCooldown < Time.time)
		{
			this.lastCheck = Time.time;
			if (NetworkSystem.Instance.IsMasterClient && !this.ValidGameMode())
			{
				GorillaGameModes.GameMode.ChangeGameFromProperty();
				return;
			}
			this.InfrequentUpdate();
		}
	}

	// Token: 0x06003752 RID: 14162 RVA: 0x0012F625 File Offset: 0x0012D825
	public virtual void InfrequentUpdate()
	{
		GorillaGameModes.GameMode.RefreshPlayers();
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
	}

	// Token: 0x06003753 RID: 14163 RVA: 0x0012F63C File Offset: 0x0012D83C
	public virtual string GameModeName()
	{
		if (this._gameModeName == null)
		{
			this._gameModeName = this.GameType().ToString().ToUpper();
		}
		return this._gameModeName;
	}

	// Token: 0x06003754 RID: 14164 RVA: 0x0012F678 File Offset: 0x0012D878
	public virtual string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_NONE_ROOM_LABEL", out result, "(NONE GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_NONE_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x06003755 RID: 14165 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
	}

	// Token: 0x06003756 RID: 14166 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
	}

	// Token: 0x06003757 RID: 14167 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void HitPlayer(NetPlayer player)
	{
	}

	// Token: 0x06003758 RID: 14168 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		return false;
	}

	// Token: 0x06003759 RID: 14169 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void HandleHandTap(NetPlayer tappingPlayer, Tappable hitTappable, bool leftHand, Vector3 handVelocity, Vector3 tapSurfaceNormal)
	{
	}

	// Token: 0x0600375A RID: 14170 RVA: 0x00023994 File Offset: 0x00021B94
	public virtual bool CanJoinFrienship(NetPlayer player)
	{
		return true;
	}

	// Token: 0x0600375B RID: 14171 RVA: 0x00023994 File Offset: 0x00021B94
	public virtual bool CanPlayerParticipate(NetPlayer player)
	{
		return true;
	}

	// Token: 0x0600375C RID: 14172 RVA: 0x0012F6A3 File Offset: 0x0012D8A3
	public virtual void HandleRoundComplete()
	{
		PlayerGameEvents.GameModeCompleteRound();
	}

	// Token: 0x0600375D RID: 14173 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
	}

	// Token: 0x0600375E RID: 14174 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer, double tagTime)
	{
	}

	// Token: 0x0600375F RID: 14175 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
	}

	// Token: 0x06003760 RID: 14176 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return false;
	}

	// Token: 0x06003761 RID: 14177 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool LocalIsTagged(NetPlayer player)
	{
		return false;
	}

	// Token: 0x06003762 RID: 14178 RVA: 0x0012F6AC File Offset: 0x0012D8AC
	public virtual VRRig FindPlayerVRRig(NetPlayer player)
	{
		RigContainer rigContainer;
		if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return rigContainer.Rig;
		}
		return null;
	}

	// Token: 0x06003763 RID: 14179 RVA: 0x0012F6D4 File Offset: 0x0012D8D4
	public static VRRig StaticFindRigForPlayer(NetPlayer player)
	{
		VRRig result = null;
		RigContainer rigContainer;
		if (GorillaGameManager.instance != null)
		{
			result = GorillaGameManager.instance.FindPlayerVRRig(player);
		}
		else if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			result = rigContainer.Rig;
		}
		return result;
	}

	// Token: 0x06003764 RID: 14180 RVA: 0x0012F715 File Offset: 0x0012D915
	public virtual float[] LocalPlayerSpeed()
	{
		this.playerSpeed[0] = this.slowJumpLimit;
		this.playerSpeed[1] = this.slowJumpMultiplier;
		return this.playerSpeed;
	}

	// Token: 0x06003765 RID: 14181 RVA: 0x0012F73C File Offset: 0x0012D93C
	public virtual void UpdatePlayerAppearance(VRRig rig)
	{
		ScienceExperimentManager instance = ScienceExperimentManager.instance;
		int materialIndex;
		if (instance != null && instance.GetMaterialIfPlayerInGame(rig.creator.ActorNumber, out materialIndex))
		{
			rig.ChangeMaterialLocal(materialIndex);
			return;
		}
		int materialIndex2 = this.MyMatIndex(rig.creator);
		rig.ChangeMaterialLocal(materialIndex2);
	}

	// Token: 0x06003766 RID: 14182 RVA: 0x00002076 File Offset: 0x00000276
	public virtual int MyMatIndex(NetPlayer forPlayer)
	{
		return 0;
	}

	// Token: 0x06003767 RID: 14183 RVA: 0x001138AD File Offset: 0x00111AAD
	public virtual int SpecialHandFX(NetPlayer player, RigContainer rigContainer)
	{
		return -1;
	}

	// Token: 0x06003768 RID: 14184 RVA: 0x0012F78B File Offset: 0x0012D98B
	public virtual bool ValidGameMode()
	{
		return NetworkSystem.Instance.InRoom && ((NetworkSystem.Instance.SessionIsPrivate && RoomSystem.IsVStumpRoom) || GameModeString.DoesPropertyStringContainGameMode(NetworkSystem.Instance.GameModeString, this.GameTypeName()));
	}

	// Token: 0x06003769 RID: 14185 RVA: 0x0012F7C8 File Offset: 0x0012D9C8
	public static void OnInstanceReady(Action action)
	{
		GorillaParent.OnReplicatedClientReady(delegate
		{
			if (GorillaGameManager.instance)
			{
				action();
				return;
			}
			GorillaGameManager.onInstanceReady = (Action)Delegate.Combine(GorillaGameManager.onInstanceReady, action);
		});
	}

	// Token: 0x0600376A RID: 14186 RVA: 0x0012F7E6 File Offset: 0x0012D9E6
	public static void ReplicatedClientReady()
	{
		GorillaGameManager.replicatedClientReady = true;
	}

	// Token: 0x0600376B RID: 14187 RVA: 0x0012F7EE File Offset: 0x0012D9EE
	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaGameManager.replicatedClientReady)
		{
			action();
			return;
		}
		GorillaGameManager.onReplicatedClientReady = (Action)Delegate.Combine(GorillaGameManager.onReplicatedClientReady, action);
	}

	// Token: 0x170004E9 RID: 1257
	// (get) Token: 0x0600376C RID: 14188 RVA: 0x0012F813 File Offset: 0x0012DA13
	internal GameModeSerializer Serializer
	{
		get
		{
			return this.serializer;
		}
	}

	// Token: 0x0600376D RID: 14189 RVA: 0x0012F81B File Offset: 0x0012DA1B
	internal virtual void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		this.serializer = netSerializer;
	}

	// Token: 0x0600376E RID: 14190 RVA: 0x0012F824 File Offset: 0x0012DA24
	internal virtual void NetworkLinkDestroyed(GameModeSerializer netSerializer)
	{
		if (this.serializer == netSerializer)
		{
			this.serializer = null;
		}
	}

	// Token: 0x0600376F RID: 14191
	public abstract GameModeType GameType();

	// Token: 0x06003770 RID: 14192 RVA: 0x0012F83C File Offset: 0x0012DA3C
	public string GameTypeName()
	{
		return this.GameType().ToString();
	}

	// Token: 0x06003771 RID: 14193
	public abstract void AddFusionDataBehaviour(NetworkObject behaviour);

	// Token: 0x06003772 RID: 14194
	public abstract void OnSerializeRead(object newData);

	// Token: 0x06003773 RID: 14195
	public abstract object OnSerializeWrite();

	// Token: 0x06003774 RID: 14196
	public abstract void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06003775 RID: 14197
	public abstract void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06003776 RID: 14198 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ResetGame()
	{
	}

	// Token: 0x06003777 RID: 14199 RVA: 0x0012F860 File Offset: 0x0012DA60
	public virtual void StartPlaying()
	{
		TickSystem<object>.AddTickCallback(this);
		NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerEnteredRoom;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent += this.OnMasterClientSwitched;
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
		GorillaTelemetry.PostGameModeEvent(GTGameModeEventType.game_mode_start, this.GameType());
	}

	// Token: 0x06003778 RID: 14200 RVA: 0x0012F8F8 File Offset: 0x0012DAF8
	public virtual void StopPlaying()
	{
		TickSystem<object>.RemoveTickCallback(this);
		NetworkSystem.Instance.OnPlayerJoined -= this.OnPlayerEnteredRoom;
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent -= this.OnMasterClientSwitched;
		this.lastCheck = 0f;
	}

	// Token: 0x06003779 RID: 14201 RVA: 0x000028C5 File Offset: 0x00000AC5
	public new virtual void OnMasterClientSwitched(Player newMaster)
	{
	}

	// Token: 0x0600377A RID: 14202 RVA: 0x000028C5 File Offset: 0x00000AC5
	public new virtual void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x0600377B RID: 14203 RVA: 0x000028C5 File Offset: 0x00000AC5
	public new virtual void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x0600377C RID: 14204 RVA: 0x0012F97C File Offset: 0x0012DB7C
	public virtual void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
		if (this.lastTaggedActorNr.ContainsKey(otherPlayer.ActorNumber))
		{
			this.lastTaggedActorNr.Remove(otherPlayer.ActorNumber);
		}
	}

	// Token: 0x0600377D RID: 14205 RVA: 0x0012F9B3 File Offset: 0x0012DBB3
	public virtual void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
	}

	// Token: 0x0600377E RID: 14206 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnMasterClientSwitched(NetPlayer newMaster)
	{
	}

	// Token: 0x0600377F RID: 14207 RVA: 0x0012F9C8 File Offset: 0x0012DBC8
	internal static void ForceStopGame_DisconnectAndDestroy()
	{
		Application.Quit();
		NetworkSystem instance = NetworkSystem.Instance;
		if (instance != null)
		{
			instance.ReturnToSinglePlayer();
		}
		Object.DestroyImmediate(PhotonNetworkController.Instance);
		Object.DestroyImmediate(GTPlayer.Instance);
		GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i]);
		}
	}

	// Token: 0x06003780 RID: 14208 RVA: 0x0012FA20 File Offset: 0x0012DC20
	public void AddLastTagged(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (this.lastTaggedActorNr.ContainsKey(taggedPlayer.ActorNumber))
		{
			this.lastTaggedActorNr[taggedPlayer.ActorNumber] = taggingPlayer.ActorNumber;
			return;
		}
		this.lastTaggedActorNr.Add(taggedPlayer.ActorNumber, taggingPlayer.ActorNumber);
	}

	// Token: 0x06003781 RID: 14209 RVA: 0x0012FA70 File Offset: 0x0012DC70
	public void WriteLastTagged(PhotonStream stream)
	{
		stream.SendNext(this.lastTaggedActorNr.Count);
		foreach (KeyValuePair<int, int> keyValuePair in this.lastTaggedActorNr)
		{
			stream.SendNext(keyValuePair.Key);
			stream.SendNext(keyValuePair.Value);
		}
	}

	// Token: 0x06003782 RID: 14210 RVA: 0x0012FAF8 File Offset: 0x0012DCF8
	public void ReadLastTagged(PhotonStream stream)
	{
		this.lastTaggedActorNr.Clear();
		int num = Mathf.Min((int)stream.ReceiveNext(), 20);
		for (int i = 0; i < num; i++)
		{
			this.lastTaggedActorNr.Add((int)stream.ReceiveNext(), (int)stream.ReceiveNext());
		}
	}

	// Token: 0x04004770 RID: 18288
	protected const string GAME_MODE_NONE_KEY = "GAME_MODE_NONE";

	// Token: 0x04004771 RID: 18289
	protected const string GAME_MODE_CASUAL_ROOM_LABEL_KEY = "GAME_MODE_CASUAL_ROOM_LABEL";

	// Token: 0x04004772 RID: 18290
	protected const string GAME_MODE_INFECTION_ROOM_LABEL_KEY = "GAME_MODE_INFECTION_ROOM_LABEL";

	// Token: 0x04004773 RID: 18291
	protected const string GAME_MODE_HUNT_ROOM_LABEL_KEY = "GAME_MODE_HUNT_ROOM_LABEL";

	// Token: 0x04004774 RID: 18292
	protected const string GAME_MODE_PAINTBRAWL_ROOM_LABEL_KEY = "GAME_MODE_PAINTBRAWL_ROOM_LABEL";

	// Token: 0x04004775 RID: 18293
	protected const string GAME_MODE_SUPER_INFECTION_ROOM_LABEL_KEY = "GAME_MODE_SUPER_INFECTION_ROOM_LABEL";

	// Token: 0x04004776 RID: 18294
	protected const string GAME_MODE_SUPER_CASUAL_ROOM_LABEL_KEY = "GAME_MODE_SUPER_CASUAL_ROOM_LABEL";

	// Token: 0x04004777 RID: 18295
	protected const string GAME_MODE_NONE_ROOM_LABEL_KEY = "GAME_MODE_NONE_ROOM_LABEL";

	// Token: 0x04004778 RID: 18296
	protected const string GAME_MODE_CUSTOM_ROOM_LABEL_KEY = "GAME_MODE_CUSTOM_ROOM_LABEL";

	// Token: 0x04004779 RID: 18297
	protected const string GAME_MODE_GHOST_ROOM_LABEL_KEY = "GAME_MODE_GHOST_ROOM_LABEL";

	// Token: 0x0400477A RID: 18298
	protected const string GAME_MODE_AMBUSH_ROOM_LABEL_KEY = "GAME_MODE_AMBUSH_ROOM_LABEL";

	// Token: 0x0400477B RID: 18299
	protected const string GAME_MODE_FREEZE_TAG_ROOM_LABEL_KEY = "GAME_MODE_FREEZE_TAG_ROOM_LABEL";

	// Token: 0x0400477C RID: 18300
	protected const string GAME_MODE_GUARDIAN_ROOM_LABEL_KEY = "GAME_MODE_GUARDIAN_ROOM_LABEL";

	// Token: 0x0400477D RID: 18301
	protected const string GAME_MODE_PROP_HUNT_ROOM_LABEL_KEY = "GAME_MODE_PROP_HUNT_ROOM_LABEL";

	// Token: 0x0400477E RID: 18302
	protected const string GAME_MODE_COMP_INF_ROOM_LABEL_KEY = "GAME_MODE_COMP_INF_ROOM_LABEL";

	// Token: 0x0400477F RID: 18303
	public const int k_defaultMatIndex = 0;

	// Token: 0x04004781 RID: 18305
	public float fastJumpLimit;

	// Token: 0x04004782 RID: 18306
	public float fastJumpMultiplier;

	// Token: 0x04004783 RID: 18307
	public float slowJumpLimit;

	// Token: 0x04004784 RID: 18308
	public float slowJumpMultiplier;

	// Token: 0x04004785 RID: 18309
	public float lastCheck;

	// Token: 0x04004786 RID: 18310
	public float checkCooldown = 3f;

	// Token: 0x04004787 RID: 18311
	public float tagDistanceThreshold = 4f;

	// Token: 0x04004788 RID: 18312
	private NetPlayer outPlayer;

	// Token: 0x04004789 RID: 18313
	private int outInt;

	// Token: 0x0400478A RID: 18314
	private VRRig tempRig;

	// Token: 0x0400478B RID: 18315
	public NetPlayer[] currentNetPlayerArray;

	// Token: 0x0400478C RID: 18316
	public float[] playerSpeed = new float[2];

	// Token: 0x0400478D RID: 18317
	public Dictionary<int, int> lastTaggedActorNr = new Dictionary<int, int>();

	// Token: 0x0400478F RID: 18319
	private string _gameModeName;

	// Token: 0x04004790 RID: 18320
	private static Action onInstanceReady;

	// Token: 0x04004791 RID: 18321
	private static bool replicatedClientReady;

	// Token: 0x04004792 RID: 18322
	private static Action onReplicatedClientReady;

	// Token: 0x04004793 RID: 18323
	private GameModeSerializer serializer;

	// Token: 0x02000859 RID: 2137
	// (Invoke) Token: 0x06003785 RID: 14213
	public delegate void OnTouchDelegate(NetPlayer taggedPlayer, NetPlayer taggingPlayer);
}
