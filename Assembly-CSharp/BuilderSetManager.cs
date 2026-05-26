using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200064D RID: 1613
public class BuilderSetManager : MonoBehaviour
{
	// Token: 0x170003FC RID: 1020
	// (get) Token: 0x06002833 RID: 10291 RVA: 0x000D9CB3 File Offset: 0x000D7EB3
	internal List<BuilderPieceSet> StartPieceSets
	{
		get
		{
			return this._starterPieceSets;
		}
	}

	// Token: 0x170003FD RID: 1021
	// (get) Token: 0x06002834 RID: 10292 RVA: 0x000D9CBB File Offset: 0x000D7EBB
	// (set) Token: 0x06002835 RID: 10293 RVA: 0x000D9CC2 File Offset: 0x000D7EC2
	public static bool hasInstance { get; private set; }

	// Token: 0x06002836 RID: 10294 RVA: 0x000D9CCC File Offset: 0x000D7ECC
	public string GetStarterSetsConcat()
	{
		if (BuilderSetManager.concatStarterSets.Length > 0)
		{
			return BuilderSetManager.concatStarterSets;
		}
		BuilderSetManager.concatStarterSets = string.Empty;
		foreach (BuilderPieceSet builderPieceSet in this._starterPieceSets)
		{
			BuilderSetManager.concatStarterSets += builderPieceSet.playfabID;
		}
		return BuilderSetManager.concatStarterSets;
	}

	// Token: 0x06002837 RID: 10295 RVA: 0x000D9D50 File Offset: 0x000D7F50
	public string GetAllSetsConcat()
	{
		if (BuilderSetManager.concatAllSets.Length > 0)
		{
			return BuilderSetManager.concatAllSets;
		}
		BuilderSetManager.concatAllSets = string.Empty;
		foreach (BuilderPieceSet builderPieceSet in this._allPieceSets)
		{
			BuilderSetManager.concatAllSets += builderPieceSet.playfabID;
		}
		return BuilderSetManager.concatAllSets;
	}

	// Token: 0x06002838 RID: 10296 RVA: 0x000D9DD4 File Offset: 0x000D7FD4
	public void Awake()
	{
		if (BuilderSetManager.instance == null)
		{
			BuilderSetManager.instance = this;
			BuilderSetManager.hasInstance = true;
		}
		else if (BuilderSetManager.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		this.Init();
		if (this.monitor == null)
		{
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}
	}

	// Token: 0x06002839 RID: 10297 RVA: 0x000D9E3C File Offset: 0x000D803C
	private void Init()
	{
		this.InitPieceDictionary();
		this.catalog = "DLC";
		this.currencyName = "SR";
		this.pulledStoreItems = false;
		BuilderSetManager._setIdToStoreItem = new Dictionary<int, BuilderSetManager.BuilderSetStoreItem>(this._allPieceSets.Count);
		BuilderSetManager._setIdToStoreItem.Clear();
		BuilderSetManager.pieceSetInfos = new List<BuilderSetManager.BuilderPieceSetInfo>(this._allPieceSets.Count * 45);
		BuilderSetManager.pieceSetInfoMap = new Dictionary<int, int>(this._allPieceSets.Count * 45);
		this.livePieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		this.scheduledPieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		this.displayGroups = new List<BuilderPieceSet.BuilderDisplayGroup>(this._allPieceSets.Count * 2);
		this.displayGroupMap = new Dictionary<int, int>(this._allPieceSets.Count * 2);
		this.liveDisplayGroups = new List<BuilderPieceSet.BuilderDisplayGroup>();
		Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
		foreach (BuilderPieceSet builderPieceSet in this._allPieceSets)
		{
			dictionary.Clear();
			int num = 0;
			BuilderSetManager.BuilderSetStoreItem value = new BuilderSetManager.BuilderSetStoreItem
			{
				displayName = builderPieceSet.SetName,
				playfabID = builderPieceSet.playfabID,
				setID = builderPieceSet.GetIntIdentifier(),
				cost = 0U,
				setRef = builderPieceSet,
				displayModel = builderPieceSet.displayModel,
				isNullItem = false
			};
			BuilderSetManager._setIdToStoreItem.TryAdd(builderPieceSet.GetIntIdentifier(), value);
			int num2 = -1;
			if (!string.IsNullOrEmpty(builderPieceSet.materialId))
			{
				num2 = builderPieceSet.materialId.GetHashCode();
			}
			for (int i = 0; i < builderPieceSet.subsets.Count; i++)
			{
				BuilderPieceSet.BuilderPieceSubset builderPieceSubset = builderPieceSet.subsets[i];
				if (!builderPieceSet.setName.Equals("HIDDEN"))
				{
					string text = builderPieceSet.subsets[i].GetShelfButtonName();
					if (text.IsNullOrEmpty())
					{
						text = builderPieceSet.setName;
					}
					text = text.ToUpper();
					int key;
					if (dictionary.TryGetValue(text, out key))
					{
						int index;
						this.displayGroupMap.TryGetValue(key, out index);
						BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup = this.displayGroups[index];
						builderDisplayGroup.pieceSubsets.Add(builderPieceSet.subsets[i]);
						this.displayGroups[index] = builderDisplayGroup;
					}
					else
					{
						string groupUniqueID = this.GetGroupUniqueID(builderPieceSet.playfabID, num);
						num++;
						BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup2 = new BuilderPieceSet.BuilderDisplayGroup(text, builderPieceSet.materialId, builderPieceSet.GetIntIdentifier(), groupUniqueID);
						builderDisplayGroup2.pieceSubsets.Add(builderPieceSet.subsets[i]);
						dictionary.Add(text, builderDisplayGroup2.GetDisplayGroupIdentifier());
						this.displayGroupMap.Add(builderDisplayGroup2.GetDisplayGroupIdentifier(), this.displayGroups.Count);
						this.displayGroups.Add(builderDisplayGroup2);
						if (!builderPieceSet.isScheduled)
						{
							this.liveDisplayGroups.Add(builderDisplayGroup2);
						}
					}
				}
				for (int j = 0; j < builderPieceSubset.pieceInfos.Count; j++)
				{
					BuilderPiece piecePrefab = builderPieceSubset.pieceInfos[j].piecePrefab;
					piecePrefab == null;
					int staticHash = piecePrefab.name.GetStaticHash();
					int pieceMaterial = num2;
					if (piecePrefab.materialOptions == null)
					{
						pieceMaterial = -1;
						this.AddPieceToInfoMap(staticHash, pieceMaterial, builderPieceSet.GetIntIdentifier());
					}
					else if (builderPieceSubset.pieceInfos[j].overrideSetMaterial)
					{
						if (builderPieceSubset.pieceInfos[j].pieceMaterialTypes.Length == 0)
						{
							Debug.LogErrorFormat("Material List for piece {0} in set {1} is empty", new object[]
							{
								piecePrefab.name,
								builderPieceSet.SetName
							});
						}
						foreach (string text2 in builderPieceSubset.pieceInfos[j].pieceMaterialTypes)
						{
							if (string.IsNullOrEmpty(text2))
							{
								Debug.LogErrorFormat("Material List Entry for piece {0} in set {1} is empty", new object[]
								{
									piecePrefab.name,
									builderPieceSet.SetName
								});
							}
							else
							{
								pieceMaterial = text2.GetHashCode();
								this.AddPieceToInfoMap(staticHash, pieceMaterial, builderPieceSet.GetIntIdentifier());
							}
						}
					}
					else
					{
						Material x;
						int num3;
						piecePrefab.materialOptions.GetMaterialFromType(num2, out x, out num3);
						if (x == null)
						{
							pieceMaterial = -1;
						}
						this.AddPieceToInfoMap(staticHash, pieceMaterial, builderPieceSet.GetIntIdentifier());
					}
				}
			}
			if (!builderPieceSet.isScheduled)
			{
				this.livePieceSets.Add(builderPieceSet);
			}
			else
			{
				this.scheduledPieceSets.Add(builderPieceSet);
			}
		}
		this._unlockedPieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		this._unlockedPieceSets.AddRange(this._starterPieceSets);
	}

	// Token: 0x0600283A RID: 10298 RVA: 0x000DA320 File Offset: 0x000D8520
	private string GetGroupUniqueID(string setPlayfabID, int groupNumber)
	{
		return setPlayfabID.Trim('.') + ((char)(65 + groupNumber)).ToString();
	}

	// Token: 0x0600283B RID: 10299 RVA: 0x000DA348 File Offset: 0x000D8548
	public void InitPieceDictionary()
	{
		if (this.hasPieceDictionary)
		{
			return;
		}
		BuilderSetManager.pieceTypes = new List<int>(256);
		BuilderSetManager.pieceList = new List<BuilderPiece>(256);
		BuilderSetManager.pieceTypeToIndex = new Dictionary<int, int>(256);
		int num = 0;
		for (int i = 0; i < this._allPieceSets.Count; i++)
		{
			BuilderPieceSet builderPieceSet = this._allPieceSets[i];
			if (!(builderPieceSet == null))
			{
				for (int j = 0; j < builderPieceSet.subsets.Count; j++)
				{
					BuilderPieceSet.BuilderPieceSubset builderPieceSubset = builderPieceSet.subsets[j];
					if (!(builderPieceSet == null))
					{
						for (int k = 0; k < builderPieceSubset.pieceInfos.Count; k++)
						{
							BuilderPieceSet.PieceInfo pieceInfo = builderPieceSubset.pieceInfos[k];
							if (!(pieceInfo.piecePrefab == null))
							{
								int staticHash = pieceInfo.piecePrefab.name.GetStaticHash();
								if (!BuilderSetManager.pieceTypeToIndex.ContainsKey(staticHash))
								{
									BuilderSetManager.pieceList.Add(pieceInfo.piecePrefab);
									BuilderSetManager.pieceTypes.Add(staticHash);
									BuilderSetManager.pieceTypeToIndex.Add(staticHash, num);
									num++;
								}
							}
						}
					}
				}
			}
		}
		this.hasPieceDictionary = true;
	}

	// Token: 0x0600283C RID: 10300 RVA: 0x000DA48C File Offset: 0x000D868C
	public BuilderPiece GetPiecePrefab(int pieceType)
	{
		int index;
		if (BuilderSetManager.pieceTypeToIndex.TryGetValue(pieceType, out index))
		{
			return BuilderSetManager.pieceList[index];
		}
		Debug.LogErrorFormat("No Prefab found for type {0}", new object[]
		{
			pieceType
		});
		return null;
	}

	// Token: 0x0600283D RID: 10301 RVA: 0x000DA4CE File Offset: 0x000D86CE
	private void OnEnable()
	{
		if (this.monitor == null && this.scheduledPieceSets.Count > 0)
		{
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}
	}

	// Token: 0x0600283E RID: 10302 RVA: 0x000DA4F8 File Offset: 0x000D86F8
	private void OnDisable()
	{
		if (this.monitor != null)
		{
			base.StopCoroutine(this.monitor);
		}
		this.monitor = null;
	}

	// Token: 0x0600283F RID: 10303 RVA: 0x000DA515 File Offset: 0x000D8715
	private IEnumerator MonitorTime()
	{
		while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
		{
			yield return null;
		}
		while (this.scheduledPieceSets.Count > 0)
		{
			bool flag = false;
			for (int i = this.scheduledPieceSets.Count - 1; i >= 0; i--)
			{
				BuilderPieceSet builderPieceSet = this.scheduledPieceSets[i];
				if (GorillaComputer.instance.GetServerTime() > builderPieceSet.GetScheduleDateTime())
				{
					flag = true;
					this.livePieceSets.Add(builderPieceSet);
					this.scheduledPieceSets.RemoveAt(i);
					int intIdentifier = builderPieceSet.GetIntIdentifier();
					foreach (BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup in this.displayGroups)
					{
						if (builderDisplayGroup != null && builderDisplayGroup.setID == intIdentifier && !this.liveDisplayGroups.Contains(builderDisplayGroup))
						{
							this.liveDisplayGroups.Add(builderDisplayGroup);
						}
					}
				}
			}
			if (flag)
			{
				this.OnLiveSetsUpdated.Invoke();
			}
			yield return new WaitForSecondsRealtime(60f);
		}
		this.monitor = null;
		yield break;
	}

	// Token: 0x06002840 RID: 10304 RVA: 0x000DA524 File Offset: 0x000D8724
	private void AddPieceToInfoMap(int pieceType, int pieceMaterial, int setID)
	{
		int index;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, pieceMaterial), out index))
		{
			BuilderSetManager.BuilderPieceSetInfo builderPieceSetInfo = BuilderSetManager.pieceSetInfos[index];
			if (!builderPieceSetInfo.setIds.Contains(setID))
			{
				builderPieceSetInfo.setIds.Add(setID);
			}
			BuilderSetManager.pieceSetInfos[index] = builderPieceSetInfo;
			return;
		}
		BuilderSetManager.BuilderPieceSetInfo item = new BuilderSetManager.BuilderPieceSetInfo
		{
			pieceType = pieceType,
			materialType = pieceMaterial,
			setIds = new List<int>
			{
				setID
			}
		};
		BuilderSetManager.pieceSetInfoMap.Add(HashCode.Combine<int, int>(pieceType, pieceMaterial), BuilderSetManager.pieceSetInfos.Count);
		BuilderSetManager.pieceSetInfos.Add(item);
	}

	// Token: 0x06002841 RID: 10305 RVA: 0x000DA5CC File Offset: 0x000D87CC
	public static bool IsItemIDBuilderItem(string playfabID)
	{
		return BuilderSetManager.instance.GetAllSetsConcat().Contains(playfabID);
	}

	// Token: 0x06002842 RID: 10306 RVA: 0x000DA5E0 File Offset: 0x000D87E0
	public void OnGotInventoryItems(GetUserInventoryResult inventoryResult, GetCatalogItemsResult catalogResult)
	{
		CosmeticsController cosmeticsController = CosmeticsController.instance;
		cosmeticsController.concatStringCosmeticsAllowed += this.GetStarterSetsConcat();
		this._unlockedPieceSets.Clear();
		this._unlockedPieceSets.AddRange(this._starterPieceSets);
		foreach (CatalogItem catalogItem in catalogResult.Catalog)
		{
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem;
			if (BuilderSetManager.IsItemIDBuilderItem(catalogItem.ItemId) && BuilderSetManager._setIdToStoreItem.TryGetValue(catalogItem.ItemId.GetStaticHash(), out builderSetStoreItem))
			{
				bool hasPrice = false;
				uint cost = 0U;
				if (catalogItem.VirtualCurrencyPrices.TryGetValue(this.currencyName, out cost))
				{
					hasPrice = true;
				}
				builderSetStoreItem.playfabID = catalogItem.ItemId;
				builderSetStoreItem.cost = cost;
				builderSetStoreItem.hasPrice = hasPrice;
				BuilderSetManager._setIdToStoreItem[builderSetStoreItem.setRef.GetIntIdentifier()] = builderSetStoreItem;
			}
		}
		foreach (ItemInstance itemInstance in inventoryResult.Inventory)
		{
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem2;
			if (BuilderSetManager.IsItemIDBuilderItem(itemInstance.ItemId) && BuilderSetManager._setIdToStoreItem.TryGetValue(itemInstance.ItemId.GetStaticHash(), out builderSetStoreItem2))
			{
				this._unlockedPieceSets.Add(builderSetStoreItem2.setRef);
				CosmeticsController cosmeticsController2 = CosmeticsController.instance;
				cosmeticsController2.concatStringCosmeticsAllowed += itemInstance.ItemId;
			}
		}
		this.pulledStoreItems = true;
		UnityEvent onOwnedSetsUpdated = this.OnOwnedSetsUpdated;
		if (onOwnedSetsUpdated == null)
		{
			return;
		}
		onOwnedSetsUpdated.Invoke();
	}

	// Token: 0x06002843 RID: 10307 RVA: 0x000DA78C File Offset: 0x000D898C
	public BuilderSetManager.BuilderSetStoreItem GetStoreItemFromSetID(int setID)
	{
		return BuilderSetManager._setIdToStoreItem.GetValueOrDefault(setID, BuilderKiosk.nullItem);
	}

	// Token: 0x06002844 RID: 10308 RVA: 0x000DA7A0 File Offset: 0x000D89A0
	public BuilderPieceSet GetPieceSetFromID(int setID)
	{
		BuilderSetManager.BuilderSetStoreItem builderSetStoreItem;
		if (BuilderSetManager._setIdToStoreItem.TryGetValue(setID, out builderSetStoreItem))
		{
			return builderSetStoreItem.setRef;
		}
		return null;
	}

	// Token: 0x06002845 RID: 10309 RVA: 0x000DA7C4 File Offset: 0x000D89C4
	public BuilderPieceSet.BuilderDisplayGroup GetDisplayGroupFromIndex(int groupID)
	{
		int index;
		if (this.displayGroupMap.TryGetValue(groupID, out index))
		{
			return this.displayGroups[index];
		}
		return null;
	}

	// Token: 0x06002846 RID: 10310 RVA: 0x000DA7EF File Offset: 0x000D89EF
	public List<BuilderPieceSet> GetAllPieceSets()
	{
		return this._allPieceSets;
	}

	// Token: 0x06002847 RID: 10311 RVA: 0x000DA7F7 File Offset: 0x000D89F7
	public List<BuilderPieceSet> GetLivePieceSets()
	{
		return this.livePieceSets;
	}

	// Token: 0x06002848 RID: 10312 RVA: 0x000DA7FF File Offset: 0x000D89FF
	public List<BuilderPieceSet.BuilderDisplayGroup> GetLiveDisplayGroups()
	{
		return this.liveDisplayGroups;
	}

	// Token: 0x06002849 RID: 10313 RVA: 0x000DA807 File Offset: 0x000D8A07
	public List<BuilderPieceSet> GetUnlockedPieceSets()
	{
		return this._unlockedPieceSets;
	}

	// Token: 0x0600284A RID: 10314 RVA: 0x000DA80F File Offset: 0x000D8A0F
	public List<BuilderPieceSet> GetPermanentSetsForSale()
	{
		return this._setsAlwaysForSale;
	}

	// Token: 0x0600284B RID: 10315 RVA: 0x000DA817 File Offset: 0x000D8A17
	public List<BuilderPieceSet> GetSeasonalSetsForSale()
	{
		return this._seasonalSetsForSale;
	}

	// Token: 0x0600284C RID: 10316 RVA: 0x000DA820 File Offset: 0x000D8A20
	public bool IsSetSeasonal(string playfabID)
	{
		return !this._seasonalSetsForSale.IsNullOrEmpty<BuilderPieceSet>() && this._seasonalSetsForSale.FindIndex((BuilderPieceSet x) => x.playfabID.Equals(playfabID)) >= 0;
	}

	// Token: 0x0600284D RID: 10317 RVA: 0x000DA868 File Offset: 0x000D8A68
	public bool DoesPlayerOwnDisplayGroup(Player player, int groupID)
	{
		if (player == null)
		{
			return false;
		}
		int num;
		if (!this.displayGroupMap.TryGetValue(groupID, out num))
		{
			return false;
		}
		if (num < 0 || num >= this.displayGroups.Count)
		{
			return false;
		}
		BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup = this.displayGroups[num];
		return builderDisplayGroup != null && this.DoesPlayerOwnPieceSet(player, builderDisplayGroup.setID);
	}

	// Token: 0x0600284E RID: 10318 RVA: 0x000DA8C0 File Offset: 0x000D8AC0
	public bool DoesPlayerOwnPieceSet(Player player, int setID)
	{
		BuilderPieceSet pieceSetFromID = this.GetPieceSetFromID(setID);
		if (pieceSetFromID == null)
		{
			return false;
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			bool flag = rigContainer.Rig.IsItemAllowed(pieceSetFromID.playfabID);
			Debug.LogFormat("BuilderSetManager: does player {0} own set {1} {2}", new object[]
			{
				player.ActorNumber,
				pieceSetFromID.SetName,
				flag
			});
			return flag;
		}
		Debug.LogFormat("BuilderSetManager: could not get rig for player {0}", new object[]
		{
			player.ActorNumber
		});
		return false;
	}

	// Token: 0x0600284F RID: 10319 RVA: 0x000DA954 File Offset: 0x000D8B54
	public bool DoesAnyPlayerInRoomOwnPieceSet(int setID)
	{
		BuilderPieceSet pieceSetFromID = this.GetPieceSetFromID(setID);
		if (pieceSetFromID == null)
		{
			return false;
		}
		if (this.GetStarterSetsConcat().Contains(pieceSetFromID.setName))
		{
			return true;
		}
		foreach (NetPlayer targetPlayer in RoomSystem.PlayersInRoom)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(targetPlayer, out rigContainer) && rigContainer.Rig.IsItemAllowed(pieceSetFromID.playfabID))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002850 RID: 10320 RVA: 0x000DA9F4 File Offset: 0x000D8BF4
	public bool IsPieceOwnedByRoom(int pieceType, int materialType)
	{
		int index;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, materialType), out index))
		{
			foreach (int setID in BuilderSetManager.pieceSetInfos[index].setIds)
			{
				if (this.DoesAnyPlayerInRoomOwnPieceSet(setID))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06002851 RID: 10321 RVA: 0x000DAA74 File Offset: 0x000D8C74
	public bool IsPieceOwnedLocally(int pieceType, int materialType)
	{
		int index;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, materialType), out index))
		{
			foreach (int setID in BuilderSetManager.pieceSetInfos[index].setIds)
			{
				if (this.IsPieceSetOwnedLocally(setID))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06002852 RID: 10322 RVA: 0x000DAAF4 File Offset: 0x000D8CF4
	public bool IsPieceSetOwnedLocally(int setID)
	{
		return this._unlockedPieceSets.FindIndex((BuilderPieceSet x) => setID == x.GetIntIdentifier()) >= 0;
	}

	// Token: 0x06002853 RID: 10323 RVA: 0x000DAB2C File Offset: 0x000D8D2C
	public void UnlockSet(int setID)
	{
		int num = this._allPieceSets.FindIndex((BuilderPieceSet x) => setID == x.GetIntIdentifier());
		if (num >= 0 && !this._unlockedPieceSets.Contains(this._allPieceSets[num]))
		{
			this._unlockedPieceSets.Add(this._allPieceSets[num]);
		}
		UnityEvent onOwnedSetsUpdated = this.OnOwnedSetsUpdated;
		if (onOwnedSetsUpdated == null)
		{
			return;
		}
		onOwnedSetsUpdated.Invoke();
	}

	// Token: 0x06002854 RID: 10324 RVA: 0x000DABA4 File Offset: 0x000D8DA4
	public void TryPurchaseItem(int setID, Action<bool> resultCallback)
	{
		BuilderSetManager.BuilderSetStoreItem storeItem;
		if (!BuilderSetManager._setIdToStoreItem.TryGetValue(setID, out storeItem))
		{
			Debug.Log("BuilderSetManager: no store Item for set " + setID.ToString());
			Action<bool> resultCallback2 = resultCallback;
			if (resultCallback2 == null)
			{
				return;
			}
			resultCallback2(false);
			return;
		}
		else
		{
			if (!this.IsPieceSetOwnedLocally(setID))
			{
				PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
				{
					ItemId = storeItem.playfabID,
					Price = (int)storeItem.cost,
					VirtualCurrency = this.currencyName,
					CatalogVersion = this.catalog
				}, delegate(PurchaseItemResult result)
				{
					if (result.Items.Count > 0)
					{
						foreach (ItemInstance itemInstance in result.Items)
						{
							Debug.Log("BuilderSetManager: unlocking set " + itemInstance.ItemId);
							this.UnlockSet(itemInstance.ItemId.GetStaticHash());
						}
						CosmeticsController.instance.UpdateMyCosmetics();
						if (PhotonNetwork.InRoom)
						{
							this.StartCoroutine(this.CheckIfMyCosmeticsUpdated(storeItem.playfabID));
						}
						Action<bool> resultCallback4 = resultCallback;
						if (resultCallback4 == null)
						{
							return;
						}
						resultCallback4(true);
						return;
					}
					else
					{
						Debug.Log("BuilderSetManager: no items purchased ");
						Action<bool> resultCallback5 = resultCallback;
						if (resultCallback5 == null)
						{
							return;
						}
						resultCallback5(false);
						return;
					}
				}, delegate(PlayFabError error)
				{
					Debug.LogErrorFormat("BuilderSetManager: purchase {0} Error {1}", new object[]
					{
						setID,
						error.ErrorMessage
					});
					Action<bool> resultCallback4 = resultCallback;
					if (resultCallback4 == null)
					{
						return;
					}
					resultCallback4(false);
				}, null, null);
				return;
			}
			Debug.Log("BuilderSetManager: set already owned " + setID.ToString());
			Action<bool> resultCallback3 = resultCallback;
			if (resultCallback3 == null)
			{
				return;
			}
			resultCallback3(false);
			return;
		}
	}

	// Token: 0x06002855 RID: 10325 RVA: 0x000DACA8 File Offset: 0x000D8EA8
	private IEnumerator CheckIfMyCosmeticsUpdated(string itemToBuyID)
	{
		yield return new WaitForSecondsRealtime(1f);
		this.foundCosmetic = false;
		this.attempts = 0;
		while (!this.foundCosmetic && this.attempts < 10 && PhotonNetwork.InRoom)
		{
			PlayFabClientAPI.GetSharedGroupData(new PlayFab.ClientModels.GetSharedGroupDataRequest
			{
				Keys = new List<string>
				{
					"Inventory"
				},
				SharedGroupId = PhotonNetwork.LocalPlayer.UserId + "Inventory"
			}, delegate(GetSharedGroupDataResult result)
			{
				this.attempts++;
				foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
				{
					if (keyValuePair.Value.Value.Contains(itemToBuyID))
					{
						PhotonNetwork.RaiseEvent(199, null, new RaiseEventOptions
						{
							Receivers = ReceiverGroup.Others
						}, SendOptions.SendReliable);
						this.foundCosmetic = true;
					}
				}
				bool flag = this.foundCosmetic;
			}, delegate(PlayFabError error)
			{
				this.attempts++;
				CosmeticsController.instance.ReauthOrBan(error);
			}, null, null);
			yield return new WaitForSecondsRealtime(1f);
		}
		Debug.Log("BuilderSetManager: done!");
		yield break;
	}

	// Token: 0x0400347F RID: 13439
	private const string preLog = "[GT/MonkeBlocks/BuilderSetManager]  ";

	// Token: 0x04003480 RID: 13440
	private const string preErr = "[GT/MonkeBlocks/BuilderSetManager]  ERROR!!!  ";

	// Token: 0x04003481 RID: 13441
	private const string preErrBeta = "[GT/MonkeBlocks/BuilderSetManager]  ERROR!!!  (beta only log)  ";

	// Token: 0x04003482 RID: 13442
	[SerializeField]
	private List<BuilderPieceSet> _allPieceSets;

	// Token: 0x04003483 RID: 13443
	[SerializeField]
	private List<BuilderPieceSet> _starterPieceSets;

	// Token: 0x04003484 RID: 13444
	[SerializeField]
	private List<BuilderPieceSet> _setsAlwaysForSale;

	// Token: 0x04003485 RID: 13445
	[SerializeField]
	private List<BuilderPieceSet> _seasonalSetsForSale;

	// Token: 0x04003486 RID: 13446
	private List<BuilderPieceSet> livePieceSets;

	// Token: 0x04003487 RID: 13447
	private List<BuilderPieceSet> scheduledPieceSets;

	// Token: 0x04003488 RID: 13448
	private List<BuilderPieceSet.BuilderDisplayGroup> liveDisplayGroups;

	// Token: 0x04003489 RID: 13449
	private Coroutine monitor;

	// Token: 0x0400348A RID: 13450
	private List<BuilderSetManager.BuilderSetStoreItem> _allStoreItems;

	// Token: 0x0400348B RID: 13451
	private List<BuilderPieceSet> _unlockedPieceSets;

	// Token: 0x0400348C RID: 13452
	private static Dictionary<int, BuilderSetManager.BuilderSetStoreItem> _setIdToStoreItem;

	// Token: 0x0400348D RID: 13453
	private static List<BuilderSetManager.BuilderPieceSetInfo> pieceSetInfos;

	// Token: 0x0400348E RID: 13454
	private static Dictionary<int, int> pieceSetInfoMap;

	// Token: 0x0400348F RID: 13455
	private List<BuilderPieceSet.BuilderDisplayGroup> displayGroups;

	// Token: 0x04003490 RID: 13456
	private Dictionary<int, int> displayGroupMap;

	// Token: 0x04003491 RID: 13457
	[OnEnterPlay_SetNull]
	public static volatile BuilderSetManager instance;

	// Token: 0x04003493 RID: 13459
	[HideInInspector]
	public string catalog;

	// Token: 0x04003494 RID: 13460
	[HideInInspector]
	public string currencyName;

	// Token: 0x04003495 RID: 13461
	private string[] tempStringArray;

	// Token: 0x04003496 RID: 13462
	[HideInInspector]
	public UnityEvent OnLiveSetsUpdated;

	// Token: 0x04003497 RID: 13463
	[HideInInspector]
	public UnityEvent OnOwnedSetsUpdated;

	// Token: 0x04003498 RID: 13464
	[HideInInspector]
	public bool pulledStoreItems;

	// Token: 0x04003499 RID: 13465
	[OnEnterPlay_Set("")]
	private static string concatStarterSets = string.Empty;

	// Token: 0x0400349A RID: 13466
	[OnEnterPlay_Set("")]
	private static string concatAllSets = string.Empty;

	// Token: 0x0400349B RID: 13467
	private bool foundCosmetic;

	// Token: 0x0400349C RID: 13468
	private int attempts;

	// Token: 0x0400349D RID: 13469
	private static List<int> pieceTypes;

	// Token: 0x0400349E RID: 13470
	[HideInInspector]
	public static List<BuilderPiece> pieceList;

	// Token: 0x0400349F RID: 13471
	private static Dictionary<int, int> pieceTypeToIndex;

	// Token: 0x040034A0 RID: 13472
	private bool hasPieceDictionary;

	// Token: 0x0200064E RID: 1614
	[Serializable]
	public struct BuilderSetStoreItem
	{
		// Token: 0x040034A1 RID: 13473
		public string displayName;

		// Token: 0x040034A2 RID: 13474
		public string playfabID;

		// Token: 0x040034A3 RID: 13475
		public int setID;

		// Token: 0x040034A4 RID: 13476
		public uint cost;

		// Token: 0x040034A5 RID: 13477
		public bool hasPrice;

		// Token: 0x040034A6 RID: 13478
		public BuilderPieceSet setRef;

		// Token: 0x040034A7 RID: 13479
		public GameObject displayModel;

		// Token: 0x040034A8 RID: 13480
		[NonSerialized]
		public bool isNullItem;
	}

	// Token: 0x0200064F RID: 1615
	[Serializable]
	public struct BuilderPieceSetInfo
	{
		// Token: 0x040034A9 RID: 13481
		public int pieceType;

		// Token: 0x040034AA RID: 13482
		public int materialType;

		// Token: 0x040034AB RID: 13483
		public List<int> setIds;
	}
}
