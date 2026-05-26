using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using GorillaTagScripts;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

// Token: 0x02000CE1 RID: 3297
internal class PlayerCosmeticsSystem : MonoBehaviour, ITickSystemPre
{
	// Token: 0x170007A3 RID: 1955
	// (get) Token: 0x060051AB RID: 20907 RVA: 0x001ADFEA File Offset: 0x001AC1EA
	// (set) Token: 0x060051AC RID: 20908 RVA: 0x001ADFF2 File Offset: 0x001AC1F2
	bool ITickSystemPre.PreTickRunning { get; set; }

	// Token: 0x060051AD RID: 20909 RVA: 0x001ADFFC File Offset: 0x001AC1FC
	private void Awake()
	{
		if (PlayerCosmeticsSystem.instance == null)
		{
			PlayerCosmeticsSystem.instance = this;
			base.transform.SetParent(null, true);
			Object.DontDestroyOnLoad(this);
			this.inventory = new List<string>();
			this.inventory.Add("InventoryDict");
			this.inventory.Add(PlayerCosmeticsSystem.subscriptionKey);
			NetworkSystem.Instance.OnRaiseEvent += this.OnNetEvent;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060051AE RID: 20910 RVA: 0x001AE07C File Offset: 0x001AC27C
	private void Start()
	{
		this.playerLookUpCooldown = Mathf.Max(this.playerLookUpCooldown, 3f);
		PlayFabTitleDataCache.Instance.GetTitleData("EnableTempCosmeticUnlocks", delegate(string data)
		{
			bool tempUnlocksEnabled;
			if (bool.TryParse(data, out tempUnlocksEnabled))
			{
				PlayerCosmeticsSystem.TempUnlocksEnabled = tempUnlocksEnabled;
				return;
			}
			Debug.LogError("PlayerCosmeticsSystem: error parsing EnableTempCosmeticUnlocks data");
		}, delegate(PlayFabError error)
		{
		}, false);
	}

	// Token: 0x060051AF RID: 20911 RVA: 0x001AE0ED File Offset: 0x001AC2ED
	private void OnDestroy()
	{
		if (PlayerCosmeticsSystem.instance == this)
		{
			PlayerCosmeticsSystem.instance = null;
		}
	}

	// Token: 0x060051B0 RID: 20912 RVA: 0x001AE102 File Offset: 0x001AC302
	private void LookUpPlayerCosmetics(bool wait = false)
	{
		if (!this.isLookingUp)
		{
			TickSystem<object>.AddPreTickCallback(this);
			this.startSearchingTime = (wait ? Time.realtimeSinceStartup : float.MinValue);
			this.isLookingUp = true;
		}
	}

	// Token: 0x060051B1 RID: 20913 RVA: 0x001AE130 File Offset: 0x001AC330
	public void PreTick()
	{
		if (PlayerCosmeticsSystem.playersToLookUp.Count < 1)
		{
			TickSystem<object>.RemovePreTickCallback(this);
			this.startSearchingTime = float.MinValue;
			this.isLookingUp = false;
			return;
		}
		if (this.startSearchingTime + this.playerLookUpCooldown > Time.realtimeSinceStartup)
		{
			return;
		}
		this.NewCosmeticsPath();
	}

	// Token: 0x060051B2 RID: 20914 RVA: 0x001AE17E File Offset: 0x001AC37E
	private void NewCosmeticsPath()
	{
		if (this.isLookingUpNew)
		{
			return;
		}
		base.StartCoroutine(this.NewCosmeticsPathCoroutine());
	}

	// Token: 0x060051B3 RID: 20915 RVA: 0x001AE196 File Offset: 0x001AC396
	private IEnumerator NewCosmeticsPathCoroutine()
	{
		this.isLookingUpNew = true;
		NetPlayer player = null;
		PlayerCosmeticsSystem.playerIDsList.Clear();
		PlayerCosmeticsSystem.playerActorNumberList.Clear();
		while (PlayerCosmeticsSystem.playersToLookUp.Count > 0)
		{
			player = PlayerCosmeticsSystem.playersToLookUp.Dequeue();
			string item = player.ActorNumber.ToString();
			if (player.InRoom() && !PlayerCosmeticsSystem.playerIDsList.Contains(item))
			{
				PlayerCosmeticsSystem.playerIDsList.Add(player.UserId);
				PlayerCosmeticsSystem.playerActorNumberList.Add(player.ActorNumber);
			}
		}
		int num;
		for (int i = 0; i < PlayerCosmeticsSystem.playerIDsList.Count; i = num + 1)
		{
			int j = i;
			PlayFab.ClientModels.GetSharedGroupDataRequest getSharedGroupDataRequest = new PlayFab.ClientModels.GetSharedGroupDataRequest();
			getSharedGroupDataRequest.Keys = this.inventory;
			getSharedGroupDataRequest.SharedGroupId = PlayerCosmeticsSystem.playerIDsList[j] + "Inventory";
			PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, delegate(GetSharedGroupDataResult result)
			{
				if (!NetworkSystem.Instance.InRoom)
				{
					PlayerCosmeticsSystem.playersWaiting.Clear();
					return;
				}
				bool flag = false;
				foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
				{
					if (keyValuePair.Key == "InventoryDict")
					{
						int j;
						if (Utils.PlayerInRoom(PlayerCosmeticsSystem.playerActorNumberList[j]))
						{
							this.tempCosmetics = keyValuePair.Value.Value;
							IUserCosmeticsCallback userCosmeticsCallback;
							if (!PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(PlayerCosmeticsSystem.playerActorNumberList[j], out userCosmeticsCallback))
							{
								PlayerCosmeticsSystem.userCosmeticsWaiting[PlayerCosmeticsSystem.playerActorNumberList[j]] = this.tempCosmetics;
							}
							else
							{
								userCosmeticsCallback.PendingUpdate = false;
								if (!userCosmeticsCallback.OnGetUserCosmetics(this.tempCosmetics))
								{
									PlayerCosmeticsSystem.playersToLookUp.Enqueue(player);
									userCosmeticsCallback.PendingUpdate = true;
								}
							}
						}
					}
					else if (keyValuePair.Key == PlayerCosmeticsSystem.subscriptionKey)
					{
						flag = true;
						NetPlayer netPlayer = null;
						NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
						for (int j = 0; j < allNetPlayers.Length; j++)
						{
							NetPlayer netPlayer2 = allNetPlayers[j];
							if (netPlayer2.ActorNumber == PlayerCosmeticsSystem.playerActorNumberList[j])
							{
								netPlayer = netPlayer2;
								break;
							}
						}
						if (netPlayer != null)
						{
							bool isSubscribed = false;
							if (!string.IsNullOrEmpty(keyValuePair.Value.Value))
							{
								try
								{
									PlayerCosmeticsSystem.SharedSubscriptionData sharedSubscriptionData = JsonConvert.DeserializeObject<PlayerCosmeticsSystem.SharedSubscriptionData>(keyValuePair.Value.Value);
									isSubscribed = (DateTimeOffset.UtcNow < sharedSubscriptionData.ExpirationTime);
								}
								catch (Exception ex)
								{
									Debug.LogError("Failed to deserialize subscription data for " + netPlayer.NickName + ": " + ex.Message);
								}
							}
							SubscriptionManager.UpdatePlayerSubscriptionData(netPlayer, isSubscribed, 0);
						}
					}
				}
				if (!flag)
				{
					NetPlayer netPlayer3 = null;
					NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
					for (int j = 0; j < allNetPlayers.Length; j++)
					{
						NetPlayer netPlayer4 = allNetPlayers[j];
						if (netPlayer4.ActorNumber == PlayerCosmeticsSystem.playerActorNumberList[j])
						{
							netPlayer3 = netPlayer4;
							break;
						}
					}
					if (netPlayer3 != null)
					{
						SubscriptionManager.UpdatePlayerSubscriptionData(netPlayer3, false, 0);
					}
				}
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
				}
			}, null, null);
			yield return new WaitForSecondsRealtime(this.getSharedGroupDataCooldown);
			num = i;
		}
		this.isLookingUpNew = false;
		yield break;
	}

	// Token: 0x060051B4 RID: 20916 RVA: 0x001AE1A5 File Offset: 0x001AC3A5
	private void OnNetEvent(byte code, object data, int source)
	{
		if (code != 199 || source < 0)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(source);
		MonkeAgent.IncrementRPCCall(new PhotonMessageInfoWrapped(source, NetworkSystem.Instance.ServerTimestamp), "UpdatePlayerCosmetics");
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(player);
	}

	// Token: 0x170007A4 RID: 1956
	// (get) Token: 0x060051B5 RID: 20917 RVA: 0x001AE1DE File Offset: 0x001AC3DE
	private static bool nullInstance
	{
		get
		{
			return PlayerCosmeticsSystem.instance == null || !PlayerCosmeticsSystem.instance;
		}
	}

	// Token: 0x170007A5 RID: 1957
	// (get) Token: 0x060051B6 RID: 20918 RVA: 0x001AE1F6 File Offset: 0x001AC3F6
	// (set) Token: 0x060051B7 RID: 20919 RVA: 0x001AE1FD File Offset: 0x001AC3FD
	public static bool TempUnlocksEnabled { get; private set; } = false;

	// Token: 0x170007A6 RID: 1958
	// (get) Token: 0x060051B8 RID: 20920 RVA: 0x001AE205 File Offset: 0x001AC405
	// (set) Token: 0x060051B9 RID: 20921 RVA: 0x001AE20C File Offset: 0x001AC40C
	public static string[] TempUnlockCosmeticString { get; private set; } = Array.Empty<string>();

	// Token: 0x060051BA RID: 20922 RVA: 0x001AE214 File Offset: 0x001AC414
	public static void RegisterCosmeticCallback(int playerID, IUserCosmeticsCallback callback)
	{
		PlayerCosmeticsSystem.userCosmeticCallback[playerID] = callback;
		string cosmetics;
		if (PlayerCosmeticsSystem.userCosmeticsWaiting.TryGetValue(playerID, out cosmetics))
		{
			callback.PendingUpdate = false;
			callback.OnGetUserCosmetics(cosmetics);
			PlayerCosmeticsSystem.userCosmeticsWaiting.Remove(playerID);
		}
	}

	// Token: 0x060051BB RID: 20923 RVA: 0x001AE257 File Offset: 0x001AC457
	public static void RemoveCosmeticCallback(int playerID)
	{
		if (PlayerCosmeticsSystem.userCosmeticCallback.ContainsKey(playerID))
		{
			PlayerCosmeticsSystem.userCosmeticCallback.Remove(playerID);
		}
	}

	// Token: 0x060051BC RID: 20924 RVA: 0x001AE274 File Offset: 0x001AC474
	public static void UpdatePlayerCosmetics(NetPlayer player)
	{
		if (player == null || player.IsLocal)
		{
			return;
		}
		PlayerCosmeticsSystem.playersToLookUp.Enqueue(player);
		IUserCosmeticsCallback userCosmeticsCallback;
		if (PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(player.ActorNumber, out userCosmeticsCallback))
		{
			userCosmeticsCallback.PendingUpdate = true;
		}
		if (!PlayerCosmeticsSystem.nullInstance)
		{
			PlayerCosmeticsSystem.instance.LookUpPlayerCosmetics(true);
		}
	}

	// Token: 0x060051BD RID: 20925 RVA: 0x001AE2C8 File Offset: 0x001AC4C8
	public static void UpdatePlayerCosmetics(List<NetPlayer> players)
	{
		foreach (NetPlayer netPlayer in players)
		{
			if (netPlayer != null && !netPlayer.IsLocal)
			{
				PlayerCosmeticsSystem.playersToLookUp.Enqueue(netPlayer);
				IUserCosmeticsCallback userCosmeticsCallback;
				if (PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(netPlayer.ActorNumber, out userCosmeticsCallback))
				{
					userCosmeticsCallback.PendingUpdate = true;
				}
			}
		}
		if (!PlayerCosmeticsSystem.nullInstance)
		{
			PlayerCosmeticsSystem.instance.LookUpPlayerCosmetics(false);
		}
	}

	// Token: 0x060051BE RID: 20926 RVA: 0x001AE354 File Offset: 0x001AC554
	public static void SetRigTryOn(bool inTryon, RigContainer rigRefg)
	{
		VRRig rig = rigRefg.Rig;
		rig.inTryOnRoom = inTryon;
		if (inTryon)
		{
			if (PlayerCosmeticsSystem.sinceLastTryOnEvent.HasElapsed(0.5f, true))
			{
				GorillaTelemetry.PostShopEvent(rig, GTShopEventType.item_try_on, rig.tryOnSet.items);
			}
			if (rig.isOfflineVRRig)
			{
				CosmeticsController.ClearTryOnCollectable();
			}
		}
		else if (rig.isOfflineVRRig)
		{
			rig.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
			CosmeticsController.ClearTryOnCollectable();
			CosmeticsController.instance.ClearCheckout(false);
			CosmeticsController.instance.UpdateShoppingCart();
			CosmeticsController.instance.UpdateWornCosmetics(true);
			rig.myBodyDockPositions.RefreshTransferrableItems();
			return;
		}
		rig.LocalUpdateCosmeticsWithTryon(rig.cosmeticSet, rig.tryOnSet, false);
		rig.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x060051BF RID: 20927 RVA: 0x001AE41C File Offset: 0x001AC61C
	public static void SetRigTemporarySpace(bool enteringSpace, RigContainer rigRef, IReadOnlyList<string> cosmeticIds)
	{
		rigRef.Rig.inTempCosmSpace = enteringSpace;
		if (enteringSpace)
		{
			CosmeticsController.CosmeticSet currentWornSet = CosmeticsController.instance.currentWornSet;
			CosmeticsController.instance.tempUnlockedSet.CopyItemsIntoEmpty(currentWornSet);
			PlayerCosmeticsSystem.UnlockTemporaryCosmeticsForPlayer(rigRef, cosmeticIds);
			return;
		}
		PlayerCosmeticsSystem.LockTemporaryCosmeticsForPlayer(rigRef, cosmeticIds);
	}

	// Token: 0x060051C0 RID: 20928 RVA: 0x001AE466 File Offset: 0x001AC666
	public static void UnlockTemporaryCosmeticsForPlayer(RigContainer rigRef)
	{
		PlayerCosmeticsSystem.UnlockTemporaryCosmeticsForPlayer(rigRef, PlayerCosmeticsSystem.TempUnlockCosmeticString);
	}

	// Token: 0x060051C1 RID: 20929 RVA: 0x001AE474 File Offset: 0x001AC674
	public static void UnlockTemporaryCosmeticsForPlayer(RigContainer rigRef, IReadOnlyList<string> cosmeticIds)
	{
		if (cosmeticIds == null)
		{
			Debug.LogError("PlayerCosmeticsSystem failed to unlock temporary cosmetics, cosmetic IDs are null");
			return;
		}
		VRRig rig = rigRef.Rig;
		foreach (string text in cosmeticIds)
		{
			if (rig.TemporaryCosmetics.Add(text) && rig.isOfflineVRRig && !rig.HasCosmetic(text))
			{
				CosmeticsController.instance.AddTempUnlockToWardrobe(text);
			}
		}
		Action onCosmeticsUpdated = CosmeticsController.instance.OnCosmeticsUpdated;
		if (onCosmeticsUpdated != null)
		{
			onCosmeticsUpdated();
		}
		if (rig.isOfflineVRRig)
		{
			CosmeticsController.instance.UpdateWornCosmetics(true);
			return;
		}
		rig.RefreshCosmetics();
	}

	// Token: 0x060051C2 RID: 20930 RVA: 0x001AE52C File Offset: 0x001AC72C
	public static void LockTemporaryCosmeticsForPlayer(RigContainer rigRef)
	{
		PlayerCosmeticsSystem.LockTemporaryCosmeticsForPlayer(rigRef, PlayerCosmeticsSystem.TempUnlockCosmeticString);
	}

	// Token: 0x060051C3 RID: 20931 RVA: 0x001AE53C File Offset: 0x001AC73C
	public static void LockTemporaryCosmeticsForPlayer(RigContainer rigRef, IReadOnlyList<string> cosmeticIds)
	{
		if (cosmeticIds == null)
		{
			Debug.LogError("PlayerCosmeticsSystem failed to unlock temporary cosmetics, cosmetic IDs are null");
			return;
		}
		VRRig rig = rigRef.Rig;
		foreach (string text in cosmeticIds)
		{
			if (rig.TemporaryCosmetics.Remove(text) && rig.isOfflineVRRig && !rig.HasCosmetic(text))
			{
				CosmeticsController.instance.RemoveTempUnlockFromWardrobe(text);
			}
		}
		Action onCosmeticsUpdated = CosmeticsController.instance.OnCosmeticsUpdated;
		if (onCosmeticsUpdated != null)
		{
			onCosmeticsUpdated();
		}
		if (rig.isOfflineVRRig)
		{
			CosmeticsController.instance.UpdateWornCosmetics(true);
			return;
		}
		rig.RefreshCosmetics();
	}

	// Token: 0x060051C4 RID: 20932 RVA: 0x001AE5F4 File Offset: 0x001AC7F4
	internal static void UnlockTemporaryCosmeticsGlobal(IReadOnlyList<string> cosmeticIds)
	{
		int count = cosmeticIds.Count;
		for (int i = 0; i < count; i++)
		{
			PlayerCosmeticsSystem.UnlockTemporaryCosmeticGlobal(cosmeticIds[i]);
		}
	}

	// Token: 0x060051C5 RID: 20933 RVA: 0x001AE620 File Offset: 0x001AC820
	internal static void UnlockTemporaryCosmeticGlobal(string cosmeticId)
	{
		int num = 0;
		if (PlayerCosmeticsSystem.k_tempUnlockedCosmetics.ContainsKey(cosmeticId))
		{
			num = PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId];
		}
		num++;
		PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId] = num;
	}

	// Token: 0x060051C6 RID: 20934 RVA: 0x001AE658 File Offset: 0x001AC858
	internal static void LockTemporaryCosmeticsGlobal(IReadOnlyList<string> cosmeticIds)
	{
		int count = cosmeticIds.Count;
		for (int i = 0; i < count; i++)
		{
			PlayerCosmeticsSystem.LockTemporaryCosmeticGlobal(cosmeticIds[i]);
		}
	}

	// Token: 0x060051C7 RID: 20935 RVA: 0x001AE684 File Offset: 0x001AC884
	internal static void LockTemporaryCosmeticGlobal(string cosmeticId)
	{
		if (!PlayerCosmeticsSystem.k_tempUnlockedCosmetics.ContainsKey(cosmeticId))
		{
			Debug.LogError("PlayerCosmeticsSystem: Unable to lock cosmetic, ID:-" + cosmeticId + " not found!");
			return;
		}
		int num = PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId];
		num--;
		PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId] = num;
	}

	// Token: 0x060051C8 RID: 20936 RVA: 0x001AE6D0 File Offset: 0x001AC8D0
	public static bool IsTemporaryCosmeticAllowed(VRRig rigRef, string cosmeticId)
	{
		int num;
		return rigRef.TemporaryCosmetics.Contains(cosmeticId) || (PlayerCosmeticsSystem.k_tempUnlockedCosmetics.TryGetValue(cosmeticId, out num) && num > 0);
	}

	// Token: 0x060051C9 RID: 20937 RVA: 0x001AE704 File Offset: 0x001AC904
	public static bool LocalIsTemporaryCosmetic(string cosmeticId)
	{
		VRRig rig = VRRigCache.Instance.localRig.Rig;
		return !rig.HasCosmetic(cosmeticId) && PlayerCosmeticsSystem.IsTemporaryCosmeticAllowed(rig, cosmeticId);
	}

	// Token: 0x060051CA RID: 20938 RVA: 0x001AE733 File Offset: 0x001AC933
	public static bool LocalPlayerInTemporaryCosmeticSpace()
	{
		return VRRigCache.Instance.localRig.Rig.inTempCosmSpace;
	}

	// Token: 0x060051CB RID: 20939 RVA: 0x001AE749 File Offset: 0x001AC949
	public static void StaticReset()
	{
		PlayerCosmeticsSystem.playersToLookUp.Clear();
		PlayerCosmeticsSystem.userCosmeticCallback.Clear();
		PlayerCosmeticsSystem.userCosmeticsWaiting.Clear();
		PlayerCosmeticsSystem.playerIDsList.Clear();
		PlayerCosmeticsSystem.playersWaiting.Clear();
	}

	// Token: 0x040062F8 RID: 25336
	public float playerLookUpCooldown = 3f;

	// Token: 0x040062F9 RID: 25337
	public float getSharedGroupDataCooldown = 0.1f;

	// Token: 0x040062FA RID: 25338
	private float startSearchingTime = float.MinValue;

	// Token: 0x040062FB RID: 25339
	private bool isLookingUp;

	// Token: 0x040062FC RID: 25340
	private bool isLookingUpNew;

	// Token: 0x040062FD RID: 25341
	private string tempCosmetics;

	// Token: 0x040062FE RID: 25342
	private NetPlayer playerTemp;

	// Token: 0x040062FF RID: 25343
	private RigContainer tempRC;

	// Token: 0x04006300 RID: 25344
	private List<string> inventory;

	// Token: 0x04006301 RID: 25345
	private const string inventoryKey = "InventoryDict";

	// Token: 0x04006302 RID: 25346
	private static readonly string subscriptionKey = "subscriptions.fan_club";

	// Token: 0x04006303 RID: 25347
	private static PlayerCosmeticsSystem instance;

	// Token: 0x04006304 RID: 25348
	private static Queue<NetPlayer> playersToLookUp = new Queue<NetPlayer>(20);

	// Token: 0x04006305 RID: 25349
	private static Dictionary<int, IUserCosmeticsCallback> userCosmeticCallback = new Dictionary<int, IUserCosmeticsCallback>(20);

	// Token: 0x04006306 RID: 25350
	private static Dictionary<int, string> userCosmeticsWaiting = new Dictionary<int, string>(5);

	// Token: 0x04006307 RID: 25351
	private static List<string> playerIDsList = new List<string>(20);

	// Token: 0x04006308 RID: 25352
	private static List<int> playerActorNumberList = new List<int>(20);

	// Token: 0x04006309 RID: 25353
	private static List<int> playersWaiting = new List<int>();

	// Token: 0x0400630A RID: 25354
	private static TimeSince sinceLastTryOnEvent = 0f;

	// Token: 0x0400630B RID: 25355
	private static readonly Dictionary<string, int> k_tempUnlockedCosmetics = new Dictionary<string, int>(20);

	// Token: 0x02000CE2 RID: 3298
	[Serializable]
	public class SharedSubscriptionData
	{
		// Token: 0x0400630E RID: 25358
		public string Sku;

		// Token: 0x0400630F RID: 25359
		public DateTimeOffset? ExpirationTime;
	}
}
