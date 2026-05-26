using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag;
using GorillaTagScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000842 RID: 2114
public class VRRigCache : MonoBehaviour
{
	// Token: 0x170004D0 RID: 1232
	// (get) Token: 0x0600367E RID: 13950 RVA: 0x0012C12E File Offset: 0x0012A32E
	// (set) Token: 0x0600367F RID: 13951 RVA: 0x0012C135 File Offset: 0x0012A335
	public static VRRigCache Instance { get; private set; }

	// Token: 0x170004D1 RID: 1233
	// (get) Token: 0x06003680 RID: 13952 RVA: 0x0012C13D File Offset: 0x0012A33D
	public Transform NetworkParent
	{
		get
		{
			return this.networkParent;
		}
	}

	// Token: 0x170004D2 RID: 1234
	// (get) Token: 0x06003681 RID: 13953 RVA: 0x0012C145 File Offset: 0x0012A345
	public static IReadOnlyList<RigContainer> ActiveRigContainers
	{
		get
		{
			return VRRigCache.m_activeRigContainers;
		}
	}

	// Token: 0x170004D3 RID: 1235
	// (get) Token: 0x06003682 RID: 13954 RVA: 0x0012C14C File Offset: 0x0012A34C
	public static IReadOnlyList<VRRig> ActiveRigs
	{
		get
		{
			return VRRigCache.m_activeRigs;
		}
	}

	// Token: 0x170004D4 RID: 1236
	// (get) Token: 0x06003683 RID: 13955 RVA: 0x0012C153 File Offset: 0x0012A353
	// (set) Token: 0x06003684 RID: 13956 RVA: 0x0012C15A File Offset: 0x0012A35A
	public static bool isInitialized { get; private set; }

	// Token: 0x1400005C RID: 92
	// (add) Token: 0x06003685 RID: 13957 RVA: 0x0012C164 File Offset: 0x0012A364
	// (remove) Token: 0x06003686 RID: 13958 RVA: 0x0012C198 File Offset: 0x0012A398
	public static event Action OnActiveRigsChanged;

	// Token: 0x1400005D RID: 93
	// (add) Token: 0x06003687 RID: 13959 RVA: 0x0012C1CC File Offset: 0x0012A3CC
	// (remove) Token: 0x06003688 RID: 13960 RVA: 0x0012C200 File Offset: 0x0012A400
	public static event Action OnPostInitialize;

	// Token: 0x1400005E RID: 94
	// (add) Token: 0x06003689 RID: 13961 RVA: 0x0012C234 File Offset: 0x0012A434
	// (remove) Token: 0x0600368A RID: 13962 RVA: 0x0012C268 File Offset: 0x0012A468
	public static event Action OnPostSpawnRig;

	// Token: 0x1400005F RID: 95
	// (add) Token: 0x0600368B RID: 13963 RVA: 0x0012C29C File Offset: 0x0012A49C
	// (remove) Token: 0x0600368C RID: 13964 RVA: 0x0012C2D0 File Offset: 0x0012A4D0
	public static event Action<RigContainer> OnRigActivated;

	// Token: 0x14000060 RID: 96
	// (add) Token: 0x0600368D RID: 13965 RVA: 0x0012C304 File Offset: 0x0012A504
	// (remove) Token: 0x0600368E RID: 13966 RVA: 0x0012C338 File Offset: 0x0012A538
	public static event Action<RigContainer> OnRigDeactivated;

	// Token: 0x14000061 RID: 97
	// (add) Token: 0x0600368F RID: 13967 RVA: 0x0012C36C File Offset: 0x0012A56C
	// (remove) Token: 0x06003690 RID: 13968 RVA: 0x0012C3A0 File Offset: 0x0012A5A0
	public static event Action<RigContainer> OnRigNameChanged;

	// Token: 0x06003691 RID: 13969 RVA: 0x0012C3D4 File Offset: 0x0012A5D4
	private void Awake()
	{
		this.InitializeVRRigCache();
		if (this.localRig != null && this.localRig.Rig != null)
		{
			VRRig rig = this.localRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Combine(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
			if (this.localRig.Rig.bodyRenderer != null)
			{
				this.localRig.Rig.bodyRenderer.SetupAsLocalPlayerBody();
			}
		}
		TickSystemTimer ensureNetworkObjectTimer = this.m_ensureNetworkObjectTimer;
		ensureNetworkObjectTimer.callback = (Action)Delegate.Combine(ensureNetworkObjectTimer.callback, new Action(this.InstantiateNetworkObject));
		NetworkedPlayerColourNotifier.SetLocalRigReference(this.localRig);
	}

	// Token: 0x06003692 RID: 13970 RVA: 0x0012C48C File Offset: 0x0012A68C
	private void OnDestroy()
	{
		if (VRRigCache.Instance == this)
		{
			VRRigCache.Instance = null;
		}
		VRRigCache.isInitialized = false;
		if (this.localRig != null && this.localRig.Rig != null)
		{
			VRRig rig = this.localRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
		}
	}

	// Token: 0x06003693 RID: 13971 RVA: 0x0012C4F8 File Offset: 0x0012A6F8
	public void InitializeVRRigCache()
	{
		if (VRRigCache.isInitialized || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (VRRigCache.Instance != null && VRRigCache.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		VRRigCache.Instance = this;
		if (this.rigParent == null)
		{
			this.rigParent = base.transform;
		}
		if (this.networkParent == null)
		{
			this.networkParent = base.transform;
		}
		for (int i = 0; i < this.rigAmount; i++)
		{
			RigContainer rigContainer = this.SpawnRig();
			VRRigCache.freeRigs.Enqueue(rigContainer);
			rigContainer.Rig.BuildInitialize();
			rigContainer.Rig.transform.parent = null;
		}
		VRRigCache.m_activeRigContainers.Add(this.localRig);
		VRRigCache.m_activeRigs.Add(this.localRig.Rig);
		VRRigCache.isInitialized = true;
		Action onPostInitialize = VRRigCache.OnPostInitialize;
		if (onPostInitialize != null)
		{
			onPostInitialize();
		}
		Action onPostSpawnRig = VRRigCache.OnPostSpawnRig;
		if (onPostSpawnRig == null)
		{
			return;
		}
		onPostSpawnRig();
	}

	// Token: 0x06003694 RID: 13972 RVA: 0x0012C5F8 File Offset: 0x0012A7F8
	private RigContainer SpawnRig()
	{
		if (this.rigTemplate.activeSelf)
		{
			this.rigTemplate.SetActive(false);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.rigTemplate, this.rigParent, false);
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<RigContainer>();
	}

	// Token: 0x06003695 RID: 13973 RVA: 0x0012C630 File Offset: 0x0012A830
	internal bool TryGetVrrig(Player targetPlayer, out RigContainer playerRig)
	{
		return this.TryGetVrrig(NetworkSystem.Instance.GetPlayer(targetPlayer.ActorNumber), out playerRig);
	}

	// Token: 0x06003696 RID: 13974 RVA: 0x0012C649 File Offset: 0x0012A849
	internal bool TryGetVrrig(int targetPlayerId, out RigContainer playerRig)
	{
		return this.TryGetVrrig(NetworkSystem.Instance.GetPlayer(targetPlayerId), out playerRig);
	}

	// Token: 0x06003697 RID: 13975 RVA: 0x0012C660 File Offset: 0x0012A860
	internal bool TryGetVrrig(NetPlayer targetPlayer, out RigContainer playerRig)
	{
		playerRig = null;
		if (ApplicationQuittingState.IsQuitting)
		{
			return false;
		}
		if (targetPlayer == null || targetPlayer.IsNull)
		{
			GTDev.LogError<string>("[GT/VRRigCache]  ERROR!!!  TryGetVrrig: Supplied targetPlayer cannot be null!", null);
			return false;
		}
		if (targetPlayer.IsLocal)
		{
			playerRig = this.localRig;
			return true;
		}
		if (!targetPlayer.InRoom)
		{
			return false;
		}
		if (!VRRigCache.rigsInUse.TryGetValue(targetPlayer, out playerRig))
		{
			if (VRRigCache.freeRigs.Count <= 0)
			{
				return false;
			}
			playerRig = VRRigCache.freeRigs.Dequeue();
			playerRig.Creator = targetPlayer;
			VRRigCache.rigsInUse.Add(targetPlayer, playerRig);
			VRRigCache.m_activeRigContainers.Add(playerRig);
			VRRigCache.m_activeRigs.Add(playerRig.Rig);
			VRRig rig = playerRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
			VRRig rig2 = playerRig.Rig;
			rig2.OnNameChanged = (Action<RigContainer>)Delegate.Combine(rig2.OnNameChanged, VRRigCache.OnRigNameChanged);
			playerRig.gameObject.SetActive(true);
			playerRig.RigEvents.SendPostEnableEvent();
			if (!VRRigCache._isBatchingRigActivations)
			{
				GamePlayer.UpdateStaticLookupCaches();
			}
			Action<RigContainer> onRigActivated = VRRigCache.OnRigActivated;
			if (onRigActivated != null)
			{
				onRigActivated(playerRig);
			}
			if (!VRRigCache._isBatchingRigActivations)
			{
				Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
				if (onActiveRigsChanged != null)
				{
					onActiveRigsChanged();
				}
			}
		}
		return true;
	}

	// Token: 0x06003698 RID: 13976 RVA: 0x0012C7A4 File Offset: 0x0012A9A4
	public void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		if (newPlayer.ActorNumber == -1)
		{
			Debug.LogError("LocalPlayer returned, vrrig no correctly initialised");
		}
		RigContainer rigContainer;
		this.TryGetVrrig(newPlayer, out rigContainer);
	}

	// Token: 0x06003699 RID: 13977 RVA: 0x0012C7D0 File Offset: 0x0012A9D0
	public void OnJoinedRoom()
	{
		VRRigCache._isBatchingRigActivations = true;
		foreach (NetPlayer targetPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			RigContainer rigContainer;
			this.TryGetVrrig(targetPlayer, out rigContainer);
		}
		VRRigCache._isBatchingRigActivations = false;
		this.m_ensureNetworkObjectTimer.Start();
		GamePlayer.UpdateStaticLookupCaches();
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged();
	}

	// Token: 0x0600369A RID: 13978 RVA: 0x0012C830 File Offset: 0x0012AA30
	public void OnPlayerLeftRoom(NetPlayer leavingPlayer)
	{
		if (leavingPlayer.IsNull)
		{
			Debug.LogError("Leaving players NetPlayer is Null");
			this.CheckForMissingPlayer();
		}
		RigContainer rigContainer;
		if (!VRRigCache.rigsInUse.TryGetValue(leavingPlayer, out rigContainer))
		{
			this.LogError("failed to find player's vrrig who left " + leavingPlayer.UserId);
			return;
		}
		rigContainer.gameObject.Disable();
		VRRig rig = rigContainer.Rig;
		rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
		VRRigCache.freeRigs.Enqueue(rigContainer);
		VRRigCache.rigsInUse.Remove(leavingPlayer);
		VRRigCache.m_activeRigContainers.Remove(rigContainer);
		VRRigCache.m_activeRigs.Remove(rigContainer.Rig);
		GamePlayer.UpdateStaticLookupCaches();
		Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
		if (onRigDeactivated != null)
		{
			onRigDeactivated(rigContainer);
		}
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged();
	}

	// Token: 0x0600369B RID: 13979 RVA: 0x0012C904 File Offset: 0x0012AB04
	private void CheckForMissingPlayer()
	{
		foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in VRRigCache.rigsInUse)
		{
			if (keyValuePair.Key == null || keyValuePair.Value == null)
			{
				Debug.LogError("Somehow null reference in rigsInUse");
			}
			else if (!keyValuePair.Key.InRoom)
			{
				keyValuePair.Value.gameObject.Disable();
				VRRig rig = keyValuePair.Value.Rig;
				rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
				VRRigCache.freeRigs.Enqueue(keyValuePair.Value);
				VRRigCache.rigsInUse.Remove(keyValuePair.Key);
				VRRigCache.m_activeRigContainers.Remove(keyValuePair.Value);
				VRRigCache.m_activeRigs.Remove(keyValuePair.Value.Rig);
				GamePlayer.UpdateStaticLookupCaches();
				Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
				if (onRigDeactivated != null)
				{
					onRigDeactivated(keyValuePair.Value);
				}
				Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
				if (onActiveRigsChanged != null)
				{
					onActiveRigsChanged();
				}
			}
		}
	}

	// Token: 0x0600369C RID: 13980 RVA: 0x0012CA48 File Offset: 0x0012AC48
	public void OnLeftRoom()
	{
		this.m_ensureNetworkObjectTimer.Stop();
		Dictionary<NetPlayer, RigContainer> dictionary;
		using (DictionaryPool<NetPlayer, RigContainer>.Get(out dictionary))
		{
			dictionary.EnsureCapacity(VRRigCache.rigsInUse.Count);
			dictionary.Clear();
			foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in VRRigCache.rigsInUse)
			{
				NetPlayer netPlayer;
				RigContainer rigContainer;
				keyValuePair.Deconstruct(out netPlayer, out rigContainer);
				NetPlayer key = netPlayer;
				RigContainer value = rigContainer;
				dictionary.Add(key, value);
			}
			foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in dictionary)
			{
				NetPlayer netPlayer;
				RigContainer rigContainer;
				keyValuePair.Deconstruct(out netPlayer, out rigContainer);
				NetPlayer key2 = netPlayer;
				RigContainer rigContainer2 = rigContainer;
				if (!(rigContainer2 == null))
				{
					VRRig rig = VRRigCache.rigsInUse[key2].Rig;
					VRRig rig2 = rigContainer2.Rig;
					rig2.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig2.OnNameChanged, VRRigCache.OnRigNameChanged);
					rigContainer2.gameObject.Disable();
					VRRigCache.rigsInUse.Remove(key2);
					VRRigCache.freeRigs.Enqueue(rigContainer2);
				}
			}
			VRRigCache.m_activeRigContainers.Clear();
			VRRigCache.m_activeRigContainers.Add(this.localRig);
			VRRigCache.m_activeRigs.Clear();
			VRRigCache.m_activeRigs.Add(this.localRig.Rig);
			GamePlayer.UpdateStaticLookupCaches();
			if (VRRigCache.OnRigDeactivated != null)
			{
				foreach (RigContainer obj in dictionary.Values)
				{
					VRRigCache.OnRigDeactivated(obj);
				}
			}
			Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
			if (onActiveRigsChanged != null)
			{
				onActiveRigsChanged();
			}
		}
	}

	// Token: 0x0600369D RID: 13981 RVA: 0x0012CC74 File Offset: 0x0012AE74
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal VRRig[] GetAllRigs()
	{
		VRRig[] array = new VRRig[VRRigCache.rigsInUse.Count + VRRigCache.freeRigs.Count];
		int num = 0;
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			array[num] = rigContainer.Rig;
			num++;
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			array[num] = rigContainer2.Rig;
			num++;
		}
		return array;
	}

	// Token: 0x0600369E RID: 13982 RVA: 0x0012CD3C File Offset: 0x0012AF3C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void GetAllUsedRigs(List<VRRig> rigs)
	{
		if (rigs == null)
		{
			return;
		}
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			rigs.Add(rigContainer.Rig);
		}
	}

	// Token: 0x0600369F RID: 13983 RVA: 0x0012CD9C File Offset: 0x0012AF9C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void GetActiveRigs(List<VRRig> rigsListToUpdate)
	{
		if (rigsListToUpdate == null)
		{
			return;
		}
		rigsListToUpdate.Clear();
		if (!VRRigCache.isInitialized)
		{
			return;
		}
		rigsListToUpdate.Add(VRRigCache.Instance.localRig.Rig);
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			rigsListToUpdate.Add(rigContainer.Rig);
		}
	}

	// Token: 0x060036A0 RID: 13984 RVA: 0x0012CE20 File Offset: 0x0012B020
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void ApplyToAllRigs(Action<VRRig> action)
	{
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			action(rigContainer.Rig);
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			action(rigContainer2.Rig);
		}
	}

	// Token: 0x060036A1 RID: 13985 RVA: 0x0012CEC4 File Offset: 0x0012B0C4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void ApplyToAllActiveRigs(Action<VRRig> action)
	{
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			action(rigContainer.Rig);
		}
	}

	// Token: 0x060036A2 RID: 13986 RVA: 0x0012CF20 File Offset: 0x0012B120
	internal int GetAllRigsHash()
	{
		int num = 0;
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			num += rigContainer.GetInstanceID();
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			num += rigContainer2.GetInstanceID();
		}
		return num;
	}

	// Token: 0x060036A3 RID: 13987 RVA: 0x0012CFC4 File Offset: 0x0012B1C4
	internal void InstantiateNetworkObject()
	{
		if (this.localRig.netView.IsNotNull() || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		PrefabType prefabType;
		if (!VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("Player Network Controller", out prefabType) || prefabType.prefab == null)
		{
			Debug.LogError("OnJoinedRoom: Unable to find player prefab to spawn");
			return;
		}
		GameObject gameObject = GTPlayer.Instance.gameObject;
		Color playerColor = this.localRig.Rig.playerColor;
		VRRigCache.rigRGBData[0] = playerColor.r;
		VRRigCache.rigRGBData[1] = playerColor.g;
		VRRigCache.rigRGBData[2] = playerColor.b;
		NetworkSystem.Instance.NetInstantiate(prefabType.prefab, gameObject.transform.position, gameObject.transform.rotation, false, 0, VRRigCache.rigRGBData, null);
	}

	// Token: 0x060036A4 RID: 13988 RVA: 0x0012D0A7 File Offset: 0x0012B2A7
	internal void OnVrrigSerializerSuccesfullySpawned()
	{
		GamePlayer.UpdateStaticLookupCaches();
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged();
	}

	// Token: 0x060036A5 RID: 13989 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void LogInfo(string log)
	{
	}

	// Token: 0x060036A6 RID: 13990 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void LogWarning(string log)
	{
	}

	// Token: 0x060036A7 RID: 13991 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void LogError(string log)
	{
	}

	// Token: 0x040046D6 RID: 18134
	private const string preLog = "[GT/VRRigCache] ";

	// Token: 0x040046D7 RID: 18135
	private const string preErr = "[GT/VRRigCache]  ERROR!!!  ";

	// Token: 0x040046D8 RID: 18136
	private const string preErrBeta = "[GT/VRRigCache]  ERROR!!!  (beta only log) ";

	// Token: 0x040046D9 RID: 18137
	private const string preErrEd = "[GT/VRRigCache]  ERROR!!!  (editor only log) ";

	// Token: 0x040046DB RID: 18139
	public RigContainer localRig;

	// Token: 0x040046DC RID: 18140
	[SerializeField]
	private Transform rigParent;

	// Token: 0x040046DD RID: 18141
	[SerializeField]
	private Transform networkParent;

	// Token: 0x040046DE RID: 18142
	[SerializeField]
	private GameObject rigTemplate;

	// Token: 0x040046DF RID: 18143
	private int rigAmount = 19;

	// Token: 0x040046E0 RID: 18144
	[SerializeField]
	private TickSystemTimer m_ensureNetworkObjectTimer = new TickSystemTimer(0.1f);

	// Token: 0x040046E1 RID: 18145
	[OnEnterPlay_Clear]
	private static Queue<RigContainer> freeRigs = new Queue<RigContainer>(19);

	// Token: 0x040046E2 RID: 18146
	[OnEnterPlay_Clear]
	private static Dictionary<NetPlayer, RigContainer> rigsInUse = new Dictionary<NetPlayer, RigContainer>(19);

	// Token: 0x040046E3 RID: 18147
	[OnEnterPlay_Clear]
	private static readonly List<RigContainer> m_activeRigContainers = new List<RigContainer>(20);

	// Token: 0x040046E4 RID: 18148
	[OnEnterPlay_Clear]
	private static readonly List<VRRig> m_activeRigs = new List<VRRig>(20);

	// Token: 0x040046E5 RID: 18149
	[OnEnterPlay_Set(false)]
	private static bool _isBatchingRigActivations;

	// Token: 0x040046ED RID: 18157
	private static object[] rigRGBData = new object[]
	{
		0f,
		0f,
		0f
	};
}
