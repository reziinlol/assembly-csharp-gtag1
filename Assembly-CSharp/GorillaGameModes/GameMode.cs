using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using UnityEngine;

namespace GorillaGameModes
{
	// Token: 0x02000EAB RID: 3755
	public class GameMode : MonoBehaviour
	{
		// Token: 0x06005C32 RID: 23602 RVA: 0x001D4564 File Offset: 0x001D2764
		private void Awake()
		{
			if (GameMode.instance.IsNull())
			{
				GameMode.instance = this;
				foreach (GorillaGameManager gorillaGameManager in base.gameObject.GetComponentsInChildren<GorillaGameManager>(true))
				{
					int num = (int)gorillaGameManager.GameType();
					string text = gorillaGameManager.GameTypeName();
					if (GameMode.gameModeTable.ContainsKey(num))
					{
						Debug.LogWarning("Duplicate gamemode type, skipping this instance", gorillaGameManager);
					}
					else
					{
						GameMode.gameModeTable.Add((int)gorillaGameManager.GameType(), gorillaGameManager);
						GameMode.gameModeKeyByName.Add(text, num);
						GameMode.gameModes.Add(gorillaGameManager);
						GameMode.gameModeNames.Add(text);
					}
				}
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x06005C33 RID: 23603 RVA: 0x001D4609 File Offset: 0x001D2809
		private void OnDestroy()
		{
			if (GameMode.instance == this)
			{
				GameMode.instance = null;
			}
		}

		// Token: 0x140000A1 RID: 161
		// (add) Token: 0x06005C34 RID: 23604 RVA: 0x001D4620 File Offset: 0x001D2820
		// (remove) Token: 0x06005C35 RID: 23605 RVA: 0x001D4654 File Offset: 0x001D2854
		public static event GameMode.OnStartGameModeAction OnStartGameMode;

		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x06005C36 RID: 23606 RVA: 0x001D4687 File Offset: 0x001D2887
		public static GorillaGameManager ActiveGameMode
		{
			get
			{
				return GameMode.activeGameMode;
			}
		}

		// Token: 0x170008DC RID: 2268
		// (get) Token: 0x06005C37 RID: 23607 RVA: 0x001D468E File Offset: 0x001D288E
		internal static GameModeSerializer ActiveNetworkHandler
		{
			get
			{
				return GameMode.activeNetworkHandler;
			}
		}

		// Token: 0x170008DD RID: 2269
		// (get) Token: 0x06005C38 RID: 23608 RVA: 0x001D4695 File Offset: 0x001D2895
		public static GameModeZoneMapping GameModeZoneMapping
		{
			get
			{
				return GameMode.instance.gameModeZoneMapping;
			}
		}

		// Token: 0x170008DE RID: 2270
		// (get) Token: 0x06005C39 RID: 23609 RVA: 0x001D46A1 File Offset: 0x001D28A1
		// (set) Token: 0x06005C3A RID: 23610 RVA: 0x001D46A8 File Offset: 0x001D28A8
		public static GameModeType CurrentGameModeType { get; private set; } = GameModeType.None;

		// Token: 0x170008DF RID: 2271
		// (get) Token: 0x06005C3B RID: 23611 RVA: 0x001D46B0 File Offset: 0x001D28B0
		public static int CurrentGameModeFlag
		{
			get
			{
				return 1 << (int)GameMode.CurrentGameModeType;
			}
		}

		// Token: 0x140000A2 RID: 162
		// (add) Token: 0x06005C3C RID: 23612 RVA: 0x001D46BC File Offset: 0x001D28BC
		// (remove) Token: 0x06005C3D RID: 23613 RVA: 0x001D46F0 File Offset: 0x001D28F0
		public static event Action<List<NetPlayer>, List<NetPlayer>> ParticipatingPlayersChanged;

		// Token: 0x06005C3E RID: 23614 RVA: 0x001D4724 File Offset: 0x001D2924
		static GameMode()
		{
			GameMode.StaticLoad();
		}

		// Token: 0x06005C3F RID: 23615 RVA: 0x001D47C8 File Offset: 0x001D29C8
		[OnEnterPlay_Run]
		private static void StaticLoad()
		{
			RoomSystem.LeftRoomEvent += new Action(GameMode.ResetGameModes);
			RoomSystem.JoinedRoomEvent += new Action(GameMode.RefreshPlayers);
			RoomSystem.PlayersChangedEvent += new Action(GameMode.RefreshPlayers);
		}

		// Token: 0x06005C40 RID: 23616 RVA: 0x001D4826 File Offset: 0x001D2A26
		public static bool IsPlaying(GameModeType type)
		{
			return type == GameMode.CurrentGameModeType;
		}

		// Token: 0x06005C41 RID: 23617 RVA: 0x001D4830 File Offset: 0x001D2A30
		internal static bool LoadGameModeFromProperty()
		{
			return GameMode.LoadGameMode(GameMode.FindGameModeFromRoomProperty());
		}

		// Token: 0x06005C42 RID: 23618 RVA: 0x001D483C File Offset: 0x001D2A3C
		internal static bool ChangeGameFromProperty()
		{
			return GameMode.ChangeGameMode(GameMode.FindGameModeFromRoomProperty());
		}

		// Token: 0x06005C43 RID: 23619 RVA: 0x001D4848 File Offset: 0x001D2A48
		internal static bool LoadGameModeFromProperty(string prop)
		{
			return GameMode.LoadGameMode(GameMode.FindGameModeInPropertyString(prop));
		}

		// Token: 0x06005C44 RID: 23620 RVA: 0x001D4855 File Offset: 0x001D2A55
		internal static bool ChangeGameFromProperty(string prop)
		{
			return GameMode.ChangeGameMode(GameMode.FindGameModeInPropertyString(prop));
		}

		// Token: 0x06005C45 RID: 23621 RVA: 0x001D4864 File Offset: 0x001D2A64
		public static int GetGameModeKeyFromRoomProp()
		{
			string text = GameMode.FindGameModeFromRoomProperty();
			int result;
			if (string.IsNullOrEmpty(text) || !GameMode.gameModeKeyByName.TryGetValue(text, out result))
			{
				GTDev.LogWarning<string>("Unable to find game mode key for " + text, null);
				return -1;
			}
			return result;
		}

		// Token: 0x06005C46 RID: 23622 RVA: 0x001D48A2 File Offset: 0x001D2AA2
		private static string FindGameModeFromRoomProperty()
		{
			if (!NetworkSystem.Instance.InRoom || string.IsNullOrEmpty(NetworkSystem.Instance.GameModeString))
			{
				return null;
			}
			return GameMode.FindGameModeInPropertyString(NetworkSystem.Instance.GameModeString);
		}

		// Token: 0x06005C47 RID: 23623 RVA: 0x001D48D2 File Offset: 0x001D2AD2
		public static bool IsValidGameMode(string gameMode)
		{
			return !string.IsNullOrEmpty(gameMode) && GameMode.gameModeKeyByName.ContainsKey(gameMode);
		}

		// Token: 0x06005C48 RID: 23624 RVA: 0x001D48E9 File Offset: 0x001D2AE9
		private static string FindGameModeInPropertyString(string gmString)
		{
			return new string(GameModeString.GameTypeFromPropertyString(gmString));
		}

		// Token: 0x06005C49 RID: 23625 RVA: 0x001D48F8 File Offset: 0x001D2AF8
		public static bool LoadGameMode(string gameMode)
		{
			if (gameMode == null)
			{
				Debug.LogError("GAME MODE NULL");
				return false;
			}
			int key;
			if (!GameMode.gameModeKeyByName.TryGetValue(gameMode, out key))
			{
				Debug.LogWarning("Unable to find game mode key for " + gameMode);
				return false;
			}
			return GameMode.LoadGameMode(key);
		}

		// Token: 0x06005C4A RID: 23626 RVA: 0x001D493C File Offset: 0x001D2B3C
		public static bool LoadGameMode(int key)
		{
			foreach (KeyValuePair<int, GorillaGameManager> keyValuePair in GameMode.gameModeTable)
			{
			}
			if (!GameMode.gameModeTable.ContainsKey(key))
			{
				Debug.LogWarning("Missing game mode for key " + key.ToString());
				return false;
			}
			PrefabType prefabType;
			VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("GameMode", out prefabType);
			GameObject prefab = prefabType.prefab;
			if (prefab == null)
			{
				GTDev.LogError<string>("Unable to find game mode prefab to spawn", null);
				return false;
			}
			if (NetworkSystem.Instance.NetInstantiate(prefab, Vector3.zero, Quaternion.identity, true, 0, new object[]
			{
				key
			}, delegate(NetworkRunner runner, NetworkObject no)
			{
				no.GetComponent<GameModeSerializer>().Init(key);
			}).IsNull())
			{
				GTDev.LogWarning<string>("Unable to create GameManager with key " + key.ToString(), null);
				return false;
			}
			return true;
		}

		// Token: 0x06005C4B RID: 23627 RVA: 0x001D4A58 File Offset: 0x001D2C58
		internal static bool ChangeGameMode(string gameMode)
		{
			if (gameMode == null)
			{
				return false;
			}
			int key;
			if (!GameMode.gameModeKeyByName.TryGetValue(gameMode, out key))
			{
				Debug.LogWarning("Unable to find game mode key for " + gameMode);
				return false;
			}
			return GameMode.ChangeGameMode(key);
		}

		// Token: 0x06005C4C RID: 23628 RVA: 0x001D4A94 File Offset: 0x001D2C94
		internal static bool ChangeGameMode(int key)
		{
			GorillaGameManager x;
			if (!NetworkSystem.Instance.IsMasterClient || !GameMode.gameModeTable.TryGetValue(key, out x) || x == GameMode.activeGameMode)
			{
				return false;
			}
			if (GameMode.activeNetworkHandler.IsNotNull())
			{
				NetworkSystem.Instance.NetDestroy(GameMode.activeNetworkHandler.gameObject);
			}
			GameMode.StopGameModeSafe(GameMode.activeGameMode);
			GameMode.activeGameMode = null;
			GameMode.activeNetworkHandler = null;
			GameMode.CurrentGameModeType = GameModeType.None;
			return GameMode.LoadGameMode(key);
		}

		// Token: 0x06005C4D RID: 23629 RVA: 0x001D4B10 File Offset: 0x001D2D10
		internal static void SetupGameModeRemote(GameModeSerializer networkSerializer)
		{
			GorillaGameManager gameModeInstance = networkSerializer.GameModeInstance;
			bool flag = gameModeInstance != GameMode.activeGameMode;
			if (GameMode.activeGameMode.IsNotNull() && gameModeInstance.IsNotNull() && flag)
			{
				GameMode.StopGameModeSafe(GameMode.activeGameMode);
			}
			GameMode.activeNetworkHandler = networkSerializer;
			GameMode.activeGameMode = gameModeInstance;
			GameMode.activeGameMode.NetworkLinkSetup(networkSerializer);
			GameMode.CurrentGameModeType = GameMode.activeGameMode.GameType();
			if (!GameMode.activatedGameModes.Contains(GameMode.activeGameMode))
			{
				GameMode.activatedGameModes.Add(GameMode.activeGameMode);
			}
			if (flag)
			{
				GameMode.StartGameModeSafe(GameMode.activeGameMode);
				if (GameMode.OnStartGameMode != null)
				{
					GameMode.OnStartGameMode(GameMode.activeGameMode.GameType());
				}
			}
		}

		// Token: 0x06005C4E RID: 23630 RVA: 0x001D4BC1 File Offset: 0x001D2DC1
		internal static void RemoveNetworkLink(GameModeSerializer networkSerializer)
		{
			if (GameMode.activeGameMode.IsNotNull() && networkSerializer == GameMode.activeNetworkHandler)
			{
				GameMode.activeGameMode.NetworkLinkDestroyed(networkSerializer);
				GameMode.activeNetworkHandler = null;
				return;
			}
		}

		// Token: 0x06005C4F RID: 23631 RVA: 0x001D4BEE File Offset: 0x001D2DEE
		public static GorillaGameManager GetGameModeInstance(GameModeType type)
		{
			return GameMode.GetGameModeInstance((int)type);
		}

		// Token: 0x06005C50 RID: 23632 RVA: 0x001D4BF8 File Offset: 0x001D2DF8
		public static GorillaGameManager GetGameModeInstance(int type)
		{
			GorillaGameManager gorillaGameManager;
			if (GameMode.gameModeTable.TryGetValue(type, out gorillaGameManager))
			{
				if (gorillaGameManager == null)
				{
					Debug.LogError("Couldnt get mode from table");
					foreach (KeyValuePair<int, GorillaGameManager> keyValuePair in GameMode.gameModeTable)
					{
					}
				}
				return gorillaGameManager;
			}
			return null;
		}

		// Token: 0x06005C51 RID: 23633 RVA: 0x001D4C68 File Offset: 0x001D2E68
		public static T GetGameModeInstance<T>(GameModeType type) where T : GorillaGameManager
		{
			return GameMode.GetGameModeInstance<T>((int)type);
		}

		// Token: 0x06005C52 RID: 23634 RVA: 0x001D4C70 File Offset: 0x001D2E70
		public static T GetGameModeInstance<T>(int type) where T : GorillaGameManager
		{
			T t = GameMode.GetGameModeInstance(type) as T;
			if (t != null)
			{
				return t;
			}
			return default(T);
		}

		// Token: 0x06005C53 RID: 23635 RVA: 0x001D4CA4 File Offset: 0x001D2EA4
		public static void ResetGameModes()
		{
			GameMode.CurrentGameModeType = GameModeType.None;
			GameMode.activeGameMode = null;
			GameMode.activeNetworkHandler = null;
			GameMode.optOutPlayers.Clear();
			GameMode.ParticipatingPlayers.Clear();
			for (int i = 0; i < GameMode.activatedGameModes.Count; i++)
			{
				GorillaGameManager gameMode = GameMode.activatedGameModes[i];
				GameMode.StopGameModeSafe(gameMode);
				GameMode.ResetGameModeSafe(gameMode);
			}
			GameMode.activatedGameModes.Clear();
		}

		// Token: 0x06005C54 RID: 23636 RVA: 0x001D4D0C File Offset: 0x001D2F0C
		private static void StartGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.StartPlaying();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06005C55 RID: 23637 RVA: 0x001D4D34 File Offset: 0x001D2F34
		private static void StopGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.StopPlaying();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06005C56 RID: 23638 RVA: 0x001D4D5C File Offset: 0x001D2F5C
		private static void ResetGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.ResetGame();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06005C57 RID: 23639 RVA: 0x001D4D84 File Offset: 0x001D2F84
		public static void ReportTag(NetPlayer player)
		{
			if (NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_ReportTag", false, new object[]
				{
					player.ActorNumber
				});
			}
		}

		// Token: 0x06005C58 RID: 23640 RVA: 0x001D4DC4 File Offset: 0x001D2FC4
		public static void ReportHit()
		{
			if (GorillaGameManager.instance.GameType() == GameModeType.Custom)
			{
				CustomGameMode.TaggedByEnvironment();
			}
			if (NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_ReportHit", false, Array.Empty<object>());
			}
		}

		// Token: 0x06005C59 RID: 23641 RVA: 0x001D4E10 File Offset: 0x001D3010
		public static bool LocalIsTagged(NetPlayer player)
		{
			return !GameMode.ActiveGameMode.IsNull() && GameMode.ActiveGameMode.LocalIsTagged(player);
		}

		// Token: 0x06005C5A RID: 23642 RVA: 0x001D4E2B File Offset: 0x001D302B
		public static void BroadcastRoundComplete()
		{
			if (NetworkSystem.Instance.IsMasterClient && NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_BroadcastRoundComplete", true, Array.Empty<object>());
			}
		}

		// Token: 0x06005C5B RID: 23643 RVA: 0x001D4E68 File Offset: 0x001D3068
		public static void BroadcastTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient && NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_BroadcastTag", true, new object[]
				{
					taggedPlayer.ActorNumber,
					taggingPlayer.ActorNumber
				});
			}
		}

		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x06005C5C RID: 23644 RVA: 0x001D4ECB File Offset: 0x001D30CB
		public static List<NetPlayer> ParticipatingPlayers
		{
			get
			{
				return GameMode._participatingPlayers;
			}
		}

		// Token: 0x06005C5D RID: 23645 RVA: 0x001D4ED4 File Offset: 0x001D30D4
		public static void RefreshPlayers()
		{
			GameMode._oldPlayersCount = GameMode._participatingPlayers.Count;
			for (int i = 0; i < GameMode._oldPlayersCount; i++)
			{
				GameMode._oldPlayersBuffer[i] = GameMode._participatingPlayers[i];
			}
			GameMode._participatingPlayers.Clear();
			List<NetPlayer> playersInRoom = RoomSystem.PlayersInRoom;
			int num = Mathf.Min(playersInRoom.Count, 20);
			for (int j = 0; j < num; j++)
			{
				if (GameMode.CanParticipate(playersInRoom[j]))
				{
					GameMode.ParticipatingPlayers.Add(playersInRoom[j]);
				}
			}
			GameMode._tempRemovedPlayers.Clear();
			for (int k = 0; k < GameMode._oldPlayersCount; k++)
			{
				NetPlayer netPlayer = GameMode._oldPlayersBuffer[k];
				if (!GameMode.ContainsNetPlayer(GameMode._participatingPlayers, netPlayer))
				{
					GameMode._tempRemovedPlayers.Add(netPlayer);
				}
			}
			GameMode._tempAddedPlayers.Clear();
			int count = GameMode._participatingPlayers.Count;
			for (int l = 0; l < count; l++)
			{
				NetPlayer netPlayer2 = GameMode._participatingPlayers[l];
				if (!GameMode.ContainsNetPlayer(GameMode._oldPlayersBuffer, netPlayer2, GameMode._oldPlayersCount))
				{
					GameMode._tempAddedPlayers.Add(netPlayer2);
				}
			}
			if ((GameMode._tempAddedPlayers.Count > 0 || GameMode._tempRemovedPlayers.Count > 0) && GameMode.ParticipatingPlayersChanged != null)
			{
				GameMode.ParticipatingPlayersChanged(GameMode._tempAddedPlayers, GameMode._tempRemovedPlayers);
			}
		}

		// Token: 0x06005C5E RID: 23646 RVA: 0x001D502C File Offset: 0x001D322C
		private static bool ContainsNetPlayer(List<NetPlayer> list, NetPlayer candidate)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i] == candidate)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005C5F RID: 23647 RVA: 0x001D505C File Offset: 0x001D325C
		private static bool ContainsNetPlayer(NetPlayer[] array, NetPlayer candidate, int length)
		{
			for (int i = 0; i < length; i++)
			{
				if (array[i] == candidate)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005C60 RID: 23648 RVA: 0x001D507E File Offset: 0x001D327E
		public static void OptOut(VRRig rig)
		{
			GameMode.OptOut(rig.creator.ActorNumber);
		}

		// Token: 0x06005C61 RID: 23649 RVA: 0x001D5090 File Offset: 0x001D3290
		public static void OptOut(NetPlayer player)
		{
			GameMode.OptOut(player.ActorNumber);
		}

		// Token: 0x06005C62 RID: 23650 RVA: 0x001D509D File Offset: 0x001D329D
		public static void OptOut(int playerActorNumber)
		{
			if (GameMode.optOutPlayers.Add(playerActorNumber))
			{
				GameMode.RefreshPlayers();
			}
		}

		// Token: 0x06005C63 RID: 23651 RVA: 0x001D50B1 File Offset: 0x001D32B1
		public static void OptIn(VRRig rig)
		{
			GameMode.OptIn(rig.creator.ActorNumber);
		}

		// Token: 0x06005C64 RID: 23652 RVA: 0x001D50C3 File Offset: 0x001D32C3
		public static void OptIn(NetPlayer player)
		{
			GameMode.OptIn(player.ActorNumber);
		}

		// Token: 0x06005C65 RID: 23653 RVA: 0x001D50D0 File Offset: 0x001D32D0
		public static void OptIn(int playerActorNumber)
		{
			if (GameMode.optOutPlayers.Remove(playerActorNumber))
			{
				GameMode.RefreshPlayers();
			}
		}

		// Token: 0x06005C66 RID: 23654 RVA: 0x001D50E4 File Offset: 0x001D32E4
		private static bool CanParticipate(NetPlayer player)
		{
			return player.InRoom() && !GameMode.optOutPlayers.Contains(player.ActorNumber) && NetworkSystem.Instance.GetPlayerTutorialCompletion(player.ActorNumber) && (!(GorillaGameManager.instance != null) || GorillaGameManager.instance.CanPlayerParticipate(player));
		}

		// Token: 0x04006AAC RID: 27308
		[SerializeField]
		private GameModeZoneMapping gameModeZoneMapping;

		// Token: 0x04006AAE RID: 27310
		[OnEnterPlay_SetNull]
		private static GameMode instance;

		// Token: 0x04006AAF RID: 27311
		[OnEnterPlay_Clear]
		private static Dictionary<int, GorillaGameManager> gameModeTable = new Dictionary<int, GorillaGameManager>();

		// Token: 0x04006AB0 RID: 27312
		[OnEnterPlay_Clear]
		public static Dictionary<string, int> gameModeKeyByName = new Dictionary<string, int>();

		// Token: 0x04006AB1 RID: 27313
		[OnEnterPlay_Clear]
		private static Dictionary<int, FusionGameModeData> fusionTypeTable = new Dictionary<int, FusionGameModeData>();

		// Token: 0x04006AB2 RID: 27314
		[OnEnterPlay_Clear]
		public static List<GorillaGameManager> gameModes = new List<GorillaGameManager>(10);

		// Token: 0x04006AB3 RID: 27315
		[OnEnterPlay_Clear]
		public static readonly List<string> gameModeNames = new List<string>(10);

		// Token: 0x04006AB4 RID: 27316
		[OnEnterPlay_Clear]
		private static readonly List<GorillaGameManager> activatedGameModes = new List<GorillaGameManager>(13);

		// Token: 0x04006AB5 RID: 27317
		[OnEnterPlay_SetNull]
		private static GorillaGameManager activeGameMode = null;

		// Token: 0x04006AB6 RID: 27318
		[OnEnterPlay_SetNull]
		private static GameModeSerializer activeNetworkHandler = null;

		// Token: 0x04006AB9 RID: 27321
		[OnEnterPlay_Clear]
		private static readonly HashSet<int> optOutPlayers = new HashSet<int>(20);

		// Token: 0x04006ABA RID: 27322
		[OnEnterPlay_Clear]
		private static readonly List<NetPlayer> _participatingPlayers = new List<NetPlayer>(20);

		// Token: 0x04006ABB RID: 27323
		private static readonly NetPlayer[] _oldPlayersBuffer = new NetPlayer[20];

		// Token: 0x04006ABC RID: 27324
		private static int _oldPlayersCount;

		// Token: 0x04006ABD RID: 27325
		private static readonly List<NetPlayer> _tempAddedPlayers = new List<NetPlayer>(20);

		// Token: 0x04006ABE RID: 27326
		private static readonly List<NetPlayer> _tempRemovedPlayers = new List<NetPlayer>(20);

		// Token: 0x02000EAC RID: 3756
		// (Invoke) Token: 0x06005C69 RID: 23657
		public delegate void OnStartGameModeAction(GameModeType newGameModeType);
	}
}
