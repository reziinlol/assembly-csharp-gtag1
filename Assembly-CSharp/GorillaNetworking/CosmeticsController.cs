using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using CosmeticRoom;
using Cosmetics;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking.Store;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using GorillaTagScripts.Subscription;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace GorillaNetworking
{
	// Token: 0x0200101A RID: 4122
	public class CosmeticsController : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
	{
		// Token: 0x170009AF RID: 2479
		// (get) Token: 0x060066DD RID: 26333 RVA: 0x0021175D File Offset: 0x0020F95D
		// (set) Token: 0x060066DE RID: 26334 RVA: 0x00211765 File Offset: 0x0020F965
		public CosmeticInfoV2[] v2_allCosmetics { get; private set; }

		// Token: 0x170009B0 RID: 2480
		// (get) Token: 0x060066DF RID: 26335 RVA: 0x0021176E File Offset: 0x0020F96E
		// (set) Token: 0x060066E0 RID: 26336 RVA: 0x00211776 File Offset: 0x0020F976
		public bool v2_allCosmeticsInfoAssetRef_isLoaded { get; private set; }

		// Token: 0x170009B1 RID: 2481
		// (get) Token: 0x060066E1 RID: 26337 RVA: 0x0021177F File Offset: 0x0020F97F
		// (set) Token: 0x060066E2 RID: 26338 RVA: 0x00211787 File Offset: 0x0020F987
		public bool v2_isGetCosmeticsPlayCatalogDataWaitingForCallback { get; private set; }

		// Token: 0x170009B2 RID: 2482
		// (get) Token: 0x060066E3 RID: 26339 RVA: 0x00211790 File Offset: 0x0020F990
		// (set) Token: 0x060066E4 RID: 26340 RVA: 0x00211798 File Offset: 0x0020F998
		public bool v2_isCosmeticPlayFabCatalogDataLoaded { get; private set; }

		// Token: 0x060066E5 RID: 26341 RVA: 0x002117A1 File Offset: 0x0020F9A1
		private void V2Awake()
		{
			this._allCosmetics = null;
			base.StartCoroutine(this.V2_allCosmeticsInfoAssetRefSO_LoadCoroutine());
		}

		// Token: 0x060066E6 RID: 26342 RVA: 0x002117B7 File Offset: 0x0020F9B7
		private IEnumerator V2_allCosmeticsInfoAssetRefSO_LoadCoroutine()
		{
			while (!PlayFabAuthenticator.instance)
			{
				yield return new WaitForSecondsRealtime(1f);
			}
			float[] retryWaitTimes = new float[]
			{
				1f,
				2f,
				4f,
				4f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				30f
			};
			int retryCount = 0;
			AsyncOperationHandle<AllCosmeticsArraySO> newSysAllCosmeticsAsyncOp;
			for (;;)
			{
				Debug.Log(string.Format("Attempting to load runtime key \"{0}\" ", this.v2_allCosmeticsInfoAssetRef.RuntimeKey) + string.Format("(Attempt: {0})", retryCount + 1));
				newSysAllCosmeticsAsyncOp = this.v2_allCosmeticsInfoAssetRef.LoadAssetAsync();
				yield return newSysAllCosmeticsAsyncOp;
				if (ApplicationQuittingState.IsQuitting)
				{
					break;
				}
				if (!newSysAllCosmeticsAsyncOp.IsValid())
				{
					Debug.LogError("`newSysAllCosmeticsAsyncOp` (should never happen) became invalid some how.");
				}
				if (newSysAllCosmeticsAsyncOp.Status == AsyncOperationStatus.Succeeded)
				{
					goto Block_4;
				}
				Debug.LogError(string.Format("Failed to load \"{0}\". ", this.v2_allCosmeticsInfoAssetRef.RuntimeKey) + "Error: " + newSysAllCosmeticsAsyncOp.OperationException.Message);
				float time = retryWaitTimes[Mathf.Min(retryCount, retryWaitTimes.Length - 1)];
				yield return new WaitForSecondsRealtime(time);
				int num = retryCount;
				retryCount = num + 1;
				newSysAllCosmeticsAsyncOp = default(AsyncOperationHandle<AllCosmeticsArraySO>);
			}
			yield break;
			Block_4:
			this.V2_allCosmeticsInfoAssetRef_LoadSucceeded(newSysAllCosmeticsAsyncOp.Result);
			yield break;
		}

		// Token: 0x060066E7 RID: 26343 RVA: 0x002117C8 File Offset: 0x0020F9C8
		private void V2_allCosmeticsInfoAssetRef_LoadSucceeded(AllCosmeticsArraySO allCosmeticsSO)
		{
			this.v2_allCosmetics = new CosmeticInfoV2[allCosmeticsSO.sturdyAssetRefs.Length];
			for (int i = 0; i < allCosmeticsSO.sturdyAssetRefs.Length; i++)
			{
				this.v2_allCosmetics[i] = allCosmeticsSO.sturdyAssetRefs[i].obj.info;
			}
			this._allCosmetics = new List<CosmeticsController.CosmeticItem>(allCosmeticsSO.sturdyAssetRefs.Length);
			for (int j = 0; j < this.v2_allCosmetics.Length; j++)
			{
				CosmeticInfoV2 cosmeticInfoV = this.v2_allCosmetics[j];
				string playFabID = cosmeticInfoV.playFabID;
				this._allCosmeticsDictV2[playFabID] = cosmeticInfoV;
				CosmeticsController.CosmeticItem cosmeticItem = default(CosmeticsController.CosmeticItem);
				cosmeticItem.itemName = playFabID;
				cosmeticItem.itemCategory = cosmeticInfoV.category;
				cosmeticItem.isHoldable = cosmeticInfoV.hasHoldableParts;
				cosmeticItem.displayName = playFabID;
				cosmeticItem.itemPicture = cosmeticInfoV.icon;
				cosmeticItem.overrideDisplayName = cosmeticInfoV.displayName;
				cosmeticItem.bothHandsHoldable = cosmeticInfoV.usesBothHandSlots;
				cosmeticItem.isNullItem = false;
				cosmeticItem.collectionParentPlayFabID = cosmeticInfoV.collectionParentPlayFabID;
				CosmeticCollectionSlotDefinition[] collectionSlots = cosmeticInfoV.collectionSlots;
				cosmeticItem.collectionSlotCount = ((collectionSlots != null) ? collectionSlots.Length : 0);
				cosmeticItem.collectionIsCycling = cosmeticInfoV.collectionIsCycling;
				cosmeticItem.collectionUsesIndexTargeting = cosmeticInfoV.collectionUsesIndexTargeting;
				cosmeticItem.collectionTargetSlotIndex = cosmeticInfoV.collectionTargetSlotIndex;
				cosmeticItem.appliedCosmeticPlayFabID = (cosmeticInfoV.appliedCosmeticPlayFabID ?? string.Empty);
				CosmeticsController.CosmeticItem item = cosmeticItem;
				this._allCosmetics.Add(item);
			}
			this.collectablesByParentID = new Dictionary<string, List<CosmeticsController.CosmeticItem>>();
			for (int k = 0; k < this._allCosmetics.Count; k++)
			{
				string collectionParentPlayFabID = this._allCosmetics[k].collectionParentPlayFabID;
				if (!string.IsNullOrEmpty(collectionParentPlayFabID))
				{
					List<CosmeticsController.CosmeticItem> list;
					if (!this.collectablesByParentID.TryGetValue(collectionParentPlayFabID, out list))
					{
						list = new List<CosmeticsController.CosmeticItem>();
						this.collectablesByParentID[collectionParentPlayFabID] = list;
					}
					list.Add(this._allCosmetics[k]);
				}
			}
			this.v2_allCosmeticsInfoAssetRef_isLoaded = true;
			Action v2_allCosmeticsInfoAssetRef_OnPostLoad = this.V2_allCosmeticsInfoAssetRef_OnPostLoad;
			if (v2_allCosmeticsInfoAssetRef_OnPostLoad == null)
			{
				return;
			}
			v2_allCosmeticsInfoAssetRef_OnPostLoad();
		}

		// Token: 0x060066E8 RID: 26344 RVA: 0x002119D6 File Offset: 0x0020FBD6
		public bool TryGetCosmeticInfoV2(string playFabId, out CosmeticInfoV2 cosmeticInfo)
		{
			return this._allCosmeticsDictV2.TryGetValue(playFabId, out cosmeticInfo);
		}

		// Token: 0x060066E9 RID: 26345 RVA: 0x002119E5 File Offset: 0x0020FBE5
		private void V2_ConformCosmeticItemV1DisplayName(ref CosmeticsController.CosmeticItem cosmetic)
		{
			if (cosmetic.itemName == cosmetic.displayName)
			{
				return;
			}
			cosmetic.overrideDisplayName = cosmetic.displayName;
			cosmetic.displayName = cosmetic.itemName;
		}

		// Token: 0x060066EA RID: 26346 RVA: 0x00211A14 File Offset: 0x0020FC14
		internal void InitializeCosmeticStands()
		{
			foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
			{
				if (cosmeticStand != null)
				{
					cosmeticStand.InitializeCosmetic();
				}
			}
		}

		// Token: 0x170009B3 RID: 2483
		// (get) Token: 0x060066EB RID: 26347 RVA: 0x00211A49 File Offset: 0x0020FC49
		// (set) Token: 0x060066EC RID: 26348 RVA: 0x00211A50 File Offset: 0x0020FC50
		public static bool hasInstance { get; private set; }

		// Token: 0x170009B4 RID: 2484
		// (get) Token: 0x060066ED RID: 26349 RVA: 0x00211A58 File Offset: 0x0020FC58
		// (set) Token: 0x060066EE RID: 26350 RVA: 0x00211A60 File Offset: 0x0020FC60
		public string PurchaseLocation
		{
			get
			{
				return this.purchaseLocation;
			}
			set
			{
				this.purchaseLocation = value;
			}
		}

		// Token: 0x060066EF RID: 26351 RVA: 0x00211A6C File Offset: 0x0020FC6C
		private string ConsumePurchaseLocation()
		{
			if (this.purchaseLocation.IsNullOrEmpty())
			{
				return GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone.ToString();
			}
			string result = this.purchaseLocation;
			this.purchaseLocation = null;
			return result;
		}

		// Token: 0x170009B5 RID: 2485
		// (get) Token: 0x060066F0 RID: 26352 RVA: 0x00211AB6 File Offset: 0x0020FCB6
		// (set) Token: 0x060066F1 RID: 26353 RVA: 0x00211ABE File Offset: 0x0020FCBE
		public List<CosmeticsController.CosmeticItem> allCosmetics
		{
			get
			{
				return this._allCosmetics;
			}
			set
			{
				this._allCosmetics = value;
			}
		}

		// Token: 0x170009B6 RID: 2486
		// (get) Token: 0x060066F2 RID: 26354 RVA: 0x00211AC7 File Offset: 0x0020FCC7
		// (set) Token: 0x060066F3 RID: 26355 RVA: 0x00211ACF File Offset: 0x0020FCCF
		public bool allCosmeticsDict_isInitialized { get; private set; }

		// Token: 0x170009B7 RID: 2487
		// (get) Token: 0x060066F4 RID: 26356 RVA: 0x00211AD8 File Offset: 0x0020FCD8
		public Dictionary<string, CosmeticsController.CosmeticItem> allCosmeticsDict
		{
			get
			{
				return this._allCosmeticsDict;
			}
		}

		// Token: 0x170009B8 RID: 2488
		// (get) Token: 0x060066F5 RID: 26357 RVA: 0x00211AE0 File Offset: 0x0020FCE0
		// (set) Token: 0x060066F6 RID: 26358 RVA: 0x00211AE8 File Offset: 0x0020FCE8
		public bool allCosmeticsItemIDsfromDisplayNamesDict_isInitialized { get; private set; }

		// Token: 0x170009B9 RID: 2489
		// (get) Token: 0x060066F7 RID: 26359 RVA: 0x00211AF1 File Offset: 0x0020FCF1
		public Dictionary<string, string> allCosmeticsItemIDsfromDisplayNamesDict
		{
			get
			{
				return this._allCosmeticsItemIDsfromDisplayNamesDict;
			}
		}

		// Token: 0x170009BA RID: 2490
		// (get) Token: 0x060066F8 RID: 26360 RVA: 0x00211AF9 File Offset: 0x0020FCF9
		public CosmeticAnchorAntiIntersectOffsets defaultClipOffsets
		{
			get
			{
				return CosmeticAnchorAntiIntersectOffsets.Identity;
			}
		}

		// Token: 0x170009BB RID: 2491
		// (get) Token: 0x060066F9 RID: 26361 RVA: 0x00211B00 File Offset: 0x0020FD00
		// (set) Token: 0x060066FA RID: 26362 RVA: 0x00211B08 File Offset: 0x0020FD08
		public bool isHidingCosmeticsFromRemotePlayers { get; private set; }

		// Token: 0x060066FB RID: 26363 RVA: 0x00211B11 File Offset: 0x0020FD11
		public void AddWardrobeInstance(WardrobeInstance instance)
		{
			this.wardrobes.Add(instance);
			this.UpdateWardrobeModelsAndButtons();
		}

		// Token: 0x060066FC RID: 26364 RVA: 0x00211B25 File Offset: 0x0020FD25
		public void RemoveWardrobeInstance(WardrobeInstance instance)
		{
			this.wardrobes.Remove(instance);
		}

		// Token: 0x060066FD RID: 26365 RVA: 0x00211B34 File Offset: 0x0020FD34
		public bool IsOwnedByPlayFabID(string playFabID)
		{
			return this.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => x.itemName == playFabID) >= 0;
		}

		// Token: 0x060066FE RID: 26366 RVA: 0x00211B6C File Offset: 0x0020FD6C
		public int GetOwnedCollectableCount(string parentPlayFabID)
		{
			int num = 0;
			for (int i = 0; i < this.unlockedCosmetics.Count; i++)
			{
				if (this.unlockedCosmetics[i].collectionParentPlayFabID == parentPlayFabID)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x060066FF RID: 26367 RVA: 0x00211BB0 File Offset: 0x0020FDB0
		public bool CanPurchaseCollectable(string collectablePlayFabID)
		{
			CosmeticsController.CosmeticItem cosmeticItem;
			if (!this.allCosmeticsDict.TryGetValue(collectablePlayFabID, out cosmeticItem))
			{
				return false;
			}
			string collectionParentPlayFabID = cosmeticItem.collectionParentPlayFabID;
			if (string.IsNullOrEmpty(collectionParentPlayFabID))
			{
				return true;
			}
			if (!this.IsOwnedByPlayFabID(collectionParentPlayFabID))
			{
				return false;
			}
			CosmeticsController.CosmeticItem cosmeticItem2;
			if (!this.allCosmeticsDict.TryGetValue(collectionParentPlayFabID, out cosmeticItem2))
			{
				return false;
			}
			List<CosmeticsController.CosmeticItem> list;
			int num = cosmeticItem2.collectionIsCycling ? (this.collectablesByParentID.TryGetValue(collectionParentPlayFabID, out list) ? list.Count : 0) : cosmeticItem2.collectionSlotCount;
			return this.GetOwnedCollectableCount(collectionParentPlayFabID) < num;
		}

		// Token: 0x170009BC RID: 2492
		// (get) Token: 0x06006700 RID: 26368 RVA: 0x00211C32 File Offset: 0x0020FE32
		public int CurrencyBalance
		{
			get
			{
				return this.currencyBalance;
			}
		}

		// Token: 0x170009BD RID: 2493
		// (get) Token: 0x06006701 RID: 26369 RVA: 0x00211C3A File Offset: 0x0020FE3A
		public CosmeticSO EarlyAccessSupporterPackCosmeticSO
		{
			get
			{
				return this.m_earlyAccessSupporterPackCosmeticSO;
			}
		}

		// Token: 0x06006702 RID: 26370 RVA: 0x00211C44 File Offset: 0x0020FE44
		public void Awake()
		{
			if (CosmeticsController.instance == null)
			{
				CosmeticsController.instance = this;
				CosmeticsController.hasInstance = true;
			}
			else if (CosmeticsController.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.V2Awake();
			if (base.gameObject.activeSelf)
			{
				this.catalog = "DLC";
				this.currencyName = "SR";
				this.nullItem = default(CosmeticsController.CosmeticItem);
				this.nullItem.itemName = "null";
				this.nullItem.displayName = "NOTHING";
				this.nullItem.itemPicture = Resources.Load<Sprite>("CosmeticNull_Icon");
				this.nullItem.itemPictureResourceString = "";
				this.nullItem.overrideDisplayName = "NOTHING";
				this.nullItem.meshAtlasResourceString = "";
				this.nullItem.meshResourceString = "";
				this.nullItem.materialResourceString = "";
				this.nullItem.isNullItem = true;
				this._allCosmeticsDict[this.nullItem.itemName] = this.nullItem;
				this._allCosmeticsItemIDsfromDisplayNamesDict[this.nullItem.displayName] = this.nullItem.itemName;
				this.tryOnCollectableItem = this.nullItem;
				for (int i = 0; i < 16; i++)
				{
					this.tryOnSet.items[i] = this.nullItem;
					this.tempUnlockedSet.items[i] = this.nullItem;
					this.activeMergedSet.items[i] = this.nullItem;
				}
				this.cosmeticsPages[0] = 0;
				this.cosmeticsPages[1] = 0;
				this.cosmeticsPages[2] = 0;
				this.cosmeticsPages[3] = 0;
				this.cosmeticsPages[4] = 0;
				this.cosmeticsPages[5] = 0;
				this.cosmeticsPages[6] = 0;
				this.cosmeticsPages[7] = 0;
				this.cosmeticsPages[8] = 0;
				this.cosmeticsPages[9] = 0;
				this.cosmeticsPages[10] = 0;
				this.itemLists[0] = this.unlockedHats;
				this.itemLists[1] = this.unlockedFaces;
				this.itemLists[2] = this.unlockedBadges;
				this.itemLists[3] = this.unlockedPaws;
				this.itemLists[4] = this.unlockedFurs;
				this.itemLists[5] = this.unlockedShirts;
				this.itemLists[6] = this.unlockedPants;
				this.itemLists[7] = this.unlockedArms;
				this.itemLists[8] = this.unlockedBacks;
				this.itemLists[9] = this.unlockedChests;
				this.itemLists[10] = this.unlockedTagFX;
				this.updateCosmeticsRetries = 0;
				this.maxUpdateCosmeticsRetries = 5;
				base.StartCoroutine(this.CheckCanGetDaily());
			}
			CreatorCodes.Initialize();
		}

		// Token: 0x06006703 RID: 26371 RVA: 0x00211F10 File Offset: 0x00210110
		public void Start()
		{
			PlayFabTitleDataCache.Instance.GetTitleData("BundleData", delegate(string data)
			{
				this.bundleList.FromJson(data);
			}, delegate(PlayFabError e)
			{
				Debug.LogError(string.Format("Error getting bundle data: {0}", e));
			}, false);
			this.anchorOverrides = GorillaTagger.Instance.offlineVRRig.GetComponent<VRRigAnchorOverrides>();
		}

		// Token: 0x06006704 RID: 26372 RVA: 0x00211F6D File Offset: 0x0021016D
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (SteamManager.Initialized && this._steamMicroTransactionAuthorizationResponse == null)
			{
				this._steamMicroTransactionAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(new Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate(this.ProcessSteamCallback));
			}
		}

		// Token: 0x06006705 RID: 26373 RVA: 0x00211F9C File Offset: 0x0021019C
		public void OnDisable()
		{
			Callback<MicroTxnAuthorizationResponse_t> steamMicroTransactionAuthorizationResponse = this._steamMicroTransactionAuthorizationResponse;
			if (steamMicroTransactionAuthorizationResponse != null)
			{
				steamMicroTransactionAuthorizationResponse.Unregister();
			}
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06006706 RID: 26374 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void SliceUpdate()
		{
		}

		// Token: 0x06006707 RID: 26375 RVA: 0x00211FB8 File Offset: 0x002101B8
		public static bool CompareCategoryToSavedCosmeticSlots(CosmeticsController.CosmeticCategory category, CosmeticsController.CosmeticSlots slot)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return slot == CosmeticsController.CosmeticSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return CosmeticsController.CosmeticSlots.Badge == slot;
			case CosmeticsController.CosmeticCategory.Face:
				return CosmeticsController.CosmeticSlots.Face == slot;
			case CosmeticsController.CosmeticCategory.Paw:
				return slot == CosmeticsController.CosmeticSlots.HandRight || slot == CosmeticsController.CosmeticSlots.HandLeft;
			case CosmeticsController.CosmeticCategory.Chest:
				return CosmeticsController.CosmeticSlots.Chest == slot;
			case CosmeticsController.CosmeticCategory.Fur:
				return CosmeticsController.CosmeticSlots.Fur == slot;
			case CosmeticsController.CosmeticCategory.Shirt:
				return CosmeticsController.CosmeticSlots.Shirt == slot;
			case CosmeticsController.CosmeticCategory.Back:
				return slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.BackRight;
			case CosmeticsController.CosmeticCategory.Arms:
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.ArmRight;
			case CosmeticsController.CosmeticCategory.Pants:
				return CosmeticsController.CosmeticSlots.Pants == slot;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return CosmeticsController.CosmeticSlots.TagEffect == slot;
			default:
				return false;
			}
		}

		// Token: 0x06006708 RID: 26376 RVA: 0x0021204C File Offset: 0x0021024C
		public static CosmeticsController.CosmeticSlots CategoryToNonTransferrableSlot(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return CosmeticsController.CosmeticSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return CosmeticsController.CosmeticSlots.Badge;
			case CosmeticsController.CosmeticCategory.Face:
				return CosmeticsController.CosmeticSlots.Face;
			case CosmeticsController.CosmeticCategory.Paw:
				return CosmeticsController.CosmeticSlots.HandRight;
			case CosmeticsController.CosmeticCategory.Chest:
				return CosmeticsController.CosmeticSlots.Chest;
			case CosmeticsController.CosmeticCategory.Fur:
				return CosmeticsController.CosmeticSlots.Fur;
			case CosmeticsController.CosmeticCategory.Shirt:
				return CosmeticsController.CosmeticSlots.Shirt;
			case CosmeticsController.CosmeticCategory.Back:
				return CosmeticsController.CosmeticSlots.Back;
			case CosmeticsController.CosmeticCategory.Arms:
				return CosmeticsController.CosmeticSlots.Arms;
			case CosmeticsController.CosmeticCategory.Pants:
				return CosmeticsController.CosmeticSlots.Pants;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return CosmeticsController.CosmeticSlots.TagEffect;
			default:
				return CosmeticsController.CosmeticSlots.Count;
			}
		}

		// Token: 0x06006709 RID: 26377 RVA: 0x002120AE File Offset: 0x002102AE
		private CosmeticsController.CosmeticSlots DropPositionToCosmeticSlot(BodyDockPositions.DropPositions pos)
		{
			switch (pos)
			{
			case BodyDockPositions.DropPositions.LeftArm:
				return CosmeticsController.CosmeticSlots.ArmLeft;
			case BodyDockPositions.DropPositions.RightArm:
				return CosmeticsController.CosmeticSlots.ArmRight;
			case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
				break;
			case BodyDockPositions.DropPositions.Chest:
				return CosmeticsController.CosmeticSlots.Chest;
			default:
				if (pos == BodyDockPositions.DropPositions.LeftBack)
				{
					return CosmeticsController.CosmeticSlots.BackLeft;
				}
				if (pos == BodyDockPositions.DropPositions.RightBack)
				{
					return CosmeticsController.CosmeticSlots.BackRight;
				}
				break;
			}
			return CosmeticsController.CosmeticSlots.Count;
		}

		// Token: 0x0600670A RID: 26378 RVA: 0x002120E0 File Offset: 0x002102E0
		private static BodyDockPositions.DropPositions CosmeticSlotToDropPosition(CosmeticsController.CosmeticSlots slot)
		{
			switch (slot)
			{
			case CosmeticsController.CosmeticSlots.ArmLeft:
				return BodyDockPositions.DropPositions.LeftArm;
			case CosmeticsController.CosmeticSlots.ArmRight:
				return BodyDockPositions.DropPositions.RightArm;
			case CosmeticsController.CosmeticSlots.BackLeft:
				return BodyDockPositions.DropPositions.LeftBack;
			case CosmeticsController.CosmeticSlots.BackRight:
				return BodyDockPositions.DropPositions.RightBack;
			case CosmeticsController.CosmeticSlots.Chest:
				return BodyDockPositions.DropPositions.Chest;
			}
			return BodyDockPositions.DropPositions.None;
		}

		// Token: 0x0600670B RID: 26379 RVA: 0x00212114 File Offset: 0x00210314
		public void AddItemCheckout(ItemCheckout newItemCheckout)
		{
			if (this.itemCheckouts.Contains(newItemCheckout))
			{
				return;
			}
			this.itemCheckouts.Add(newItemCheckout);
			this.UpdateShoppingCart();
			this.FormattedPurchaseText(this.finalLine, this.leftCheckoutPurchaseButtonString, this.rightCheckoutPurchaseButtonString, this.leftCheckoutPurchaseButtonOn, this.rightCheckoutPurchaseButtonOn);
			if (!this.itemToBuy.isNullItem)
			{
				this.RefreshItemToBuyPreview();
			}
		}

		// Token: 0x0600670C RID: 26380 RVA: 0x00212179 File Offset: 0x00210379
		public void RemoveItemCheckout(ItemCheckout checkoutToRemove)
		{
			this.itemCheckouts.RemoveIfContains(checkoutToRemove);
		}

		// Token: 0x0600670D RID: 26381 RVA: 0x00212187 File Offset: 0x00210387
		public void AddFittingRoom(FittingRoom newFittingRoom)
		{
			if (this.fittingRooms.Contains(newFittingRoom))
			{
				return;
			}
			this.fittingRooms.Add(newFittingRoom);
			this.UpdateShoppingCart();
		}

		// Token: 0x0600670E RID: 26382 RVA: 0x002121AA File Offset: 0x002103AA
		public void RemoveFittingRoom(FittingRoom fittingRoomToRemove)
		{
			this.fittingRooms.RemoveIfContains(fittingRoomToRemove);
		}

		// Token: 0x0600670F RID: 26383 RVA: 0x002121B8 File Offset: 0x002103B8
		private void SaveItemPreference(CosmeticsController.CosmeticSlots slot, int slotIdx, CosmeticsController.CosmeticItem newItem)
		{
			PlayerPrefs.SetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), newItem.itemName);
			PlayerPrefs.Save();
		}

		// Token: 0x06006710 RID: 26384 RVA: 0x002121D0 File Offset: 0x002103D0
		public void SaveCurrentItemPreferences()
		{
			for (int i = 0; i < 16; i++)
			{
				CosmeticsController.CosmeticSlots slot = (CosmeticsController.CosmeticSlots)i;
				CosmeticsController.CosmeticItem cosmeticItem = this.currentWornSet.items[i];
				if (cosmeticItem.itemName == "Slingshot")
				{
					cosmeticItem = this.nullItem;
				}
				this.SaveItemPreference(slot, i, cosmeticItem);
			}
		}

		// Token: 0x06006711 RID: 26385 RVA: 0x00212220 File Offset: 0x00210420
		private void ApplyCosmeticToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, int slotIdx, CosmeticsController.CosmeticSlots slot, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			CosmeticsController.CosmeticItem cosmeticItem = (set.items[slotIdx].itemName == newItem.itemName) ? this.nullItem : newItem;
			set.items[slotIdx] = cosmeticItem;
			if (applyToPlayerPrefs)
			{
				this.SaveItemPreference(slot, slotIdx, cosmeticItem);
			}
			appliedSlots.Add(slot);
		}

		// Token: 0x06006712 RID: 26386 RVA: 0x00212279 File Offset: 0x00210479
		public static void ClearTryOnCollectable()
		{
			if (!CosmeticsController.hasInstance)
			{
				return;
			}
			CosmeticsController.instance.tryOnCollectableItem = CosmeticsController.instance.nullItem;
		}

		// Token: 0x06006713 RID: 26387 RVA: 0x0021229C File Offset: 0x0021049C
		private void PrivApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			if (newItem.isNullItem)
			{
				return;
			}
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Collectable)
			{
				if (set == this.tryOnSet)
				{
					this.tryOnCollectableItem = newItem;
				}
				return;
			}
			if (set == this.tryOnSet)
			{
				CosmeticsController.ClearTryOnCollectable();
			}
			VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(newItem.itemName);
			if (CosmeticsController.CosmeticSet.IsHoldable(newItem))
			{
				BodyDockPositions.DockingResult dockingResult = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>().ToggleWithHandedness(newItem.displayName, isLeftHand, newItem.bothHandsHoldable);
				foreach (BodyDockPositions.DropPositions pos in dockingResult.positionsDisabled)
				{
					CosmeticsController.CosmeticSlots cosmeticSlots = this.DropPositionToCosmeticSlot(pos);
					if (cosmeticSlots != CosmeticsController.CosmeticSlots.Count)
					{
						int num = (int)cosmeticSlots;
						set.items[num] = this.nullItem;
						if (applyToPlayerPrefs)
						{
							this.SaveItemPreference(cosmeticSlots, num, this.nullItem);
						}
					}
				}
				using (List<BodyDockPositions.DropPositions>.Enumerator enumerator = dockingResult.dockedPosition.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BodyDockPositions.DropPositions dropPositions = enumerator.Current;
						if (dropPositions != BodyDockPositions.DropPositions.None)
						{
							CosmeticsController.CosmeticSlots cosmeticSlots2 = this.DropPositionToCosmeticSlot(dropPositions);
							int num2 = (int)cosmeticSlots2;
							set.items[num2] = newItem;
							if (applyToPlayerPrefs)
							{
								this.SaveItemPreference(cosmeticSlots2, num2, newItem);
							}
							appliedSlots.Add(cosmeticSlots2);
						}
					}
					return;
				}
			}
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Paw)
			{
				CosmeticsController.CosmeticSlots cosmeticSlots3 = isLeftHand ? CosmeticsController.CosmeticSlots.HandLeft : CosmeticsController.CosmeticSlots.HandRight;
				int slotIdx = (int)cosmeticSlots3;
				this.ApplyCosmeticToSet(set, newItem, slotIdx, cosmeticSlots3, applyToPlayerPrefs, appliedSlots);
				CosmeticsController.CosmeticSlots cosmeticSlots4 = CosmeticsController.CosmeticSet.OppositeSlot(cosmeticSlots3);
				int num3 = (int)cosmeticSlots4;
				if (newItem.bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
				if (set.items[num3].itemName == newItem.itemName)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
				}
				if (set.items[num3].bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
			}
			else
			{
				CosmeticsController.CosmeticSlots cosmeticSlots5 = CosmeticsController.CategoryToNonTransferrableSlot(newItem.itemCategory);
				int slotIdx2 = (int)cosmeticSlots5;
				this.ApplyCosmeticToSet(set, newItem, slotIdx2, cosmeticSlots5, applyToPlayerPrefs, appliedSlots);
			}
		}

		// Token: 0x06006714 RID: 26388 RVA: 0x002124E0 File Offset: 0x002106E0
		public void ApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs)
		{
			this.ApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, CosmeticsController._g_default_outAppliedSlotsList_for_applyCosmeticItemToSet);
		}

		// Token: 0x06006715 RID: 26389 RVA: 0x002124F4 File Offset: 0x002106F4
		public void ApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> outAppliedSlotsList)
		{
			outAppliedSlotsList.Clear();
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Set)
			{
				bool flag = false;
				Dictionary<CosmeticsController.CosmeticItem, bool> dictionary = new Dictionary<CosmeticsController.CosmeticItem, bool>();
				foreach (string itemID in newItem.bundledItems)
				{
					CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(itemID);
					if (this.AnyMatch(set, itemFromDict))
					{
						flag = true;
						dictionary.Add(itemFromDict, true);
					}
					else
					{
						dictionary.Add(itemFromDict, false);
					}
				}
				using (Dictionary<CosmeticsController.CosmeticItem, bool>.Enumerator enumerator = dictionary.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<CosmeticsController.CosmeticItem, bool> keyValuePair = enumerator.Current;
						if (flag)
						{
							if (keyValuePair.Value)
							{
								this.PrivApplyCosmeticItemToSet(set, keyValuePair.Key, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
							}
						}
						else
						{
							this.PrivApplyCosmeticItemToSet(set, keyValuePair.Key, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
						}
					}
					return;
				}
			}
			this.PrivApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
		}

		// Token: 0x06006716 RID: 26390 RVA: 0x002125E0 File Offset: 0x002107E0
		public void RemoveCosmeticItemFromSet(CosmeticsController.CosmeticSet set, string itemName, bool applyToPlayerPrefs)
		{
			this.cachedSet.CopyItems(set);
			for (int i = 0; i < 16; i++)
			{
				if (set.items[i].displayName == itemName)
				{
					set.items[i] = this.nullItem;
					if (applyToPlayerPrefs)
					{
						this.SaveItemPreference((CosmeticsController.CosmeticSlots)i, i, this.nullItem);
					}
				}
			}
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			BodyDockPositions component = offlineVRRig.GetComponent<BodyDockPositions>();
			set.ActivateCosmetics(this.cachedSet, offlineVRRig, component, offlineVRRig.cosmeticsObjectRegistry);
		}

		// Token: 0x06006717 RID: 26391 RVA: 0x00212668 File Offset: 0x00210868
		private void RepressButton(FittingRoomButton pressedButton, bool isLeftHand)
		{
			CosmeticsController.<RepressButton>d__184 <RepressButton>d__;
			<RepressButton>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<RepressButton>d__.<>4__this = this;
			<RepressButton>d__.pressedButton = pressedButton;
			<RepressButton>d__.isLeftHand = isLeftHand;
			<RepressButton>d__.<>1__state = -1;
			<RepressButton>d__.<>t__builder.Start<CosmeticsController.<RepressButton>d__184>(ref <RepressButton>d__);
		}

		// Token: 0x06006718 RID: 26392 RVA: 0x002126B0 File Offset: 0x002108B0
		public void PressFittingRoomButton(FittingRoomButton pressedFittingRoomButton, bool isLeftHand)
		{
			if (pressedFittingRoomButton.currentCosmeticItem.itemName == null || pressedFittingRoomButton.currentCosmeticItem.itemName == this.nullItem.itemName || pressedFittingRoomButton.currentCosmeticItem.itemName == "")
			{
				return;
			}
			if (pressedFittingRoomButton.currentCosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Set)
			{
				CosmeticsController.CosmeticItem currentCosmeticItem = pressedFittingRoomButton.currentCosmeticItem;
				bool flag = false;
				for (int i = 0; i < currentCosmeticItem.bundledItems.Length; i++)
				{
					if (VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(currentCosmeticItem.bundledItems[i]) == null)
					{
						flag = true;
					}
				}
				if (flag)
				{
					this.RepressButton(pressedFittingRoomButton, isLeftHand);
					return;
				}
			}
			else if (VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(pressedFittingRoomButton.currentCosmeticItem.itemName) == null)
			{
				this.RepressButton(pressedFittingRoomButton, isLeftHand);
				return;
			}
			TryOnBundlesStand tryOnBundlesStand = BundleManager.instance._tryOnBundlesStand;
			if (tryOnBundlesStand != null)
			{
				tryOnBundlesStand.ClearSelectedBundle();
			}
			this.ApplyCosmeticItemToSet(this.tryOnSet, pressedFittingRoomButton.currentCosmeticItem, isLeftHand, false);
			this.UpdateShoppingCart();
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06006719 RID: 26393 RVA: 0x002127B0 File Offset: 0x002109B0
		public CosmeticsController.EWearingCosmeticSet CheckIfCosmeticSetMatchesItemSet(CosmeticsController.CosmeticSet set, string itemName)
		{
			CosmeticsController.EWearingCosmeticSet ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.NotASet;
			CosmeticsController.CosmeticItem cosmeticItem = this.allCosmeticsDict[itemName];
			if (cosmeticItem.bundledItems.Length != 0)
			{
				foreach (string key in cosmeticItem.bundledItems)
				{
					if (this.AnyMatch(set, this.allCosmeticsDict[key]))
					{
						if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotASet)
						{
							ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Complete;
						}
						else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotWearing)
						{
							ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Partial;
						}
					}
					else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotASet)
					{
						ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.NotWearing;
					}
					else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.Complete)
					{
						ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Partial;
					}
				}
			}
			return ewearingCosmeticSet;
		}

		// Token: 0x0600671A RID: 26394 RVA: 0x00212824 File Offset: 0x00210A24
		public void PressCosmeticStandButton(CosmeticStand pressedStand)
		{
			this.searchIndex = this.currentCart.IndexOf(pressedStand.thisCosmeticItem);
			if (this.searchIndex != -1)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_remove, pressedStand.thisCosmeticItem);
				this.currentCart.RemoveAt(this.searchIndex);
				pressedStand.isOn = false;
				for (int i = 0; i < 16; i++)
				{
					if (pressedStand.thisCosmeticItem.itemName == this.tryOnSet.items[i].itemName)
					{
						this.tryOnSet.items[i] = this.nullItem;
					}
				}
			}
			else
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_add, pressedStand.thisCosmeticItem);
				this.currentCart.Insert(0, pressedStand.thisCosmeticItem);
				pressedStand.isOn = true;
				if (this.currentCart.Count > this.numFittingRoomButtons)
				{
					foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
					{
						if (!(cosmeticStand == null) && cosmeticStand.thisCosmeticItem.itemName == this.currentCart[this.numFittingRoomButtons].itemName)
						{
							cosmeticStand.isOn = false;
							cosmeticStand.UpdateColor();
							break;
						}
					}
					this.currentCart.RemoveAt(this.numFittingRoomButtons);
				}
			}
			pressedStand.UpdateColor();
			this.UpdateShoppingCart();
		}

		// Token: 0x0600671B RID: 26395 RVA: 0x00212988 File Offset: 0x00210B88
		public void PressWardrobeItemButton(CosmeticsController.CosmeticItem cosmeticItem, bool isLeftHand, bool isTempCosm)
		{
			if (cosmeticItem.isNullItem)
			{
				return;
			}
			CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(cosmeticItem.itemName);
			if (isTempCosm)
			{
				this.PressTemporaryWardrobeItemButton(itemFromDict, isLeftHand);
			}
			else
			{
				this.PressWardrobeItemButton(itemFromDict, isLeftHand);
			}
			this.UpdateWornCosmetics(true);
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated();
		}

		// Token: 0x0600671C RID: 26396 RVA: 0x002129D8 File Offset: 0x00210BD8
		private void PressWardrobeItemButton(CosmeticsController.CosmeticItem item, bool isLeftHand)
		{
			List<CosmeticsController.CosmeticSlots> list = CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Get();
			if (list.Capacity < 16)
			{
				list.Capacity = 16;
			}
			this.ApplyCosmeticItemToSet(this.currentWornSet, item, isLeftHand, true, list);
			foreach (CosmeticsController.CosmeticSlots cosmeticSlots in list)
			{
				this.tryOnSet.items[(int)cosmeticSlots] = this.nullItem;
			}
			CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Release(list);
			this.UpdateShoppingCart();
		}

		// Token: 0x0600671D RID: 26397 RVA: 0x00212A6C File Offset: 0x00210C6C
		private void PressTemporaryWardrobeItemButton(CosmeticsController.CosmeticItem item, bool isLeftHand)
		{
			this.ApplyCosmeticItemToSet(this.tempUnlockedSet, item, isLeftHand, false);
		}

		// Token: 0x0600671E RID: 26398 RVA: 0x00212A80 File Offset: 0x00210C80
		public void PressWardrobeFunctionButton(string function)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(function);
			if (num <= 2554875734U)
			{
				if (num <= 895779448U)
				{
					if (num != 292255708U)
					{
						if (num != 306900080U)
						{
							if (num == 895779448U)
							{
								if (function == "badge")
								{
									if (this.wardrobeType == 2)
									{
										return;
									}
									this.wardrobeType = 2;
								}
							}
						}
						else if (function == "left")
						{
							this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] - 1;
							if (this.cosmeticsPages[this.wardrobeType] < 0)
							{
								this.cosmeticsPages[this.wardrobeType] = (this.itemLists[this.wardrobeType].Count - 1) / 3;
							}
						}
					}
					else if (function == "face")
					{
						if (this.wardrobeType == 1)
						{
							return;
						}
						this.wardrobeType = 1;
					}
				}
				else if (num != 1538531746U)
				{
					if (num != 2028154341U)
					{
						if (num == 2554875734U)
						{
							if (function == "chest")
							{
								if (this.wardrobeType == 8)
								{
									return;
								}
								this.wardrobeType = 8;
							}
						}
					}
					else if (function == "right")
					{
						this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] + 1;
						if (this.cosmeticsPages[this.wardrobeType] > (this.itemLists[this.wardrobeType].Count - 1) / 3)
						{
							this.cosmeticsPages[this.wardrobeType] = 0;
						}
					}
				}
				else if (function == "back")
				{
					if (this.wardrobeType == 7)
					{
						return;
					}
					this.wardrobeType = 7;
				}
			}
			else if (num <= 3034286914U)
			{
				if (num != 2633735346U)
				{
					if (num != 2953262278U)
					{
						if (num == 3034286914U)
						{
							if (function == "fur")
							{
								if (this.wardrobeType == 4)
								{
									return;
								}
								this.wardrobeType = 4;
							}
						}
					}
					else if (function == "outfit")
					{
						if (this.wardrobeType == 5)
						{
							return;
						}
						this.wardrobeType = 5;
					}
				}
				else if (function == "arms")
				{
					if (this.wardrobeType == 6)
					{
						return;
					}
					this.wardrobeType = 6;
				}
			}
			else if (num <= 3300536096U)
			{
				if (num != 3081164502U)
				{
					if (num == 3300536096U)
					{
						if (function == "hand")
						{
							if (this.wardrobeType == 3)
							{
								return;
							}
							this.wardrobeType = 3;
						}
					}
				}
				else if (function == "tagEffect")
				{
					if (this.wardrobeType == 10)
					{
						return;
					}
					this.wardrobeType = 10;
				}
			}
			else if (num != 3568683773U)
			{
				if (num == 4072609730U)
				{
					if (function == "hat")
					{
						if (this.wardrobeType == 0)
						{
							return;
						}
						this.wardrobeType = 0;
					}
				}
			}
			else if (function == "reserved")
			{
				if (this.wardrobeType == 9)
				{
					return;
				}
				this.wardrobeType = 9;
			}
			this.UpdateWardrobeModelsAndButtons();
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated();
		}

		// Token: 0x0600671F RID: 26399 RVA: 0x00212E0C File Offset: 0x0021100C
		public void ClearCheckout(bool sendEvent)
		{
			if (sendEvent)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_cancel, this.currentCart);
			}
			this.itemToBuy = this.nullItem;
			this.RefreshItemToBuyPreview();
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
			this.ProcessPurchaseItemState(null, false);
		}

		// Token: 0x06006720 RID: 26400 RVA: 0x00212E48 File Offset: 0x00211048
		public bool RemoveItemFromCart(CosmeticsController.CosmeticItem cosmeticItem)
		{
			this.searchIndex = this.currentCart.IndexOf(cosmeticItem);
			if (this.searchIndex != -1)
			{
				this.currentCart.RemoveAt(this.searchIndex);
				for (int i = 0; i < 16; i++)
				{
					if (cosmeticItem.itemName == this.tryOnSet.items[i].itemName)
					{
						this.tryOnSet.items[i] = this.nullItem;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06006721 RID: 26401 RVA: 0x00212ECB File Offset: 0x002110CB
		public void ClearCheckoutAndCart(bool sendEvent)
		{
			this.currentCart.Clear();
			this.tryOnSet.ClearSet(this.nullItem);
			CosmeticsController.ClearTryOnCollectable();
			this.ClearCheckout(sendEvent);
		}

		// Token: 0x06006722 RID: 26402 RVA: 0x00212EF8 File Offset: 0x002110F8
		public void PressCheckoutCartButton(CheckoutCartButton pressedCheckoutCartButton, bool isLeftHand)
		{
			if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Buying)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.tryOnSet.ClearSet(this.nullItem);
				CosmeticsController.ClearTryOnCollectable();
				if (this.itemToBuy.displayName == pressedCheckoutCartButton.currentCosmeticItem.displayName)
				{
					this.itemToBuy = this.nullItem;
					this.RefreshItemToBuyPreview();
				}
				else
				{
					this.itemToBuy = pressedCheckoutCartButton.currentCosmeticItem;
					this.checkoutCartButtonPressedWithLeft = isLeftHand;
					this.RefreshItemToBuyPreview();
				}
				this.ProcessPurchaseItemState(null, isLeftHand);
				this.UpdateShoppingCart();
			}
		}

		// Token: 0x06006723 RID: 26403 RVA: 0x00212F84 File Offset: 0x00211184
		private void RefreshItemToBuyPreview()
		{
			if (this.itemToBuy.bundledItems != null && this.itemToBuy.bundledItems.Length != 0)
			{
				List<string> list = new List<string>();
				foreach (string itemID in this.itemToBuy.bundledItems)
				{
					this.tempItem = this.GetItemFromDict(itemID);
					list.Add(this.tempItem.displayName);
				}
				this.iterator = 0;
				while (this.iterator < this.itemCheckouts.Count)
				{
					if (!this.itemCheckouts[this.iterator].IsNull())
					{
						this.itemCheckouts[this.iterator].checkoutHeadModel.SetCosmeticActiveArray(list.ToArray(), new bool[list.Count]);
					}
					this.iterator++;
				}
			}
			else
			{
				this.iterator = 0;
				while (this.iterator < this.itemCheckouts.Count)
				{
					if (!this.itemCheckouts[this.iterator].IsNull())
					{
						this.itemCheckouts[this.iterator].checkoutHeadModel.SetCosmeticActive(this.itemToBuy.displayName, false);
					}
					this.iterator++;
				}
			}
			this.ApplyCosmeticItemToSet(this.tryOnSet, this.itemToBuy, this.checkoutCartButtonPressedWithLeft, false);
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06006724 RID: 26404 RVA: 0x002130ED File Offset: 0x002112ED
		public void PressPurchaseItemButton(PurchaseItemButton pressedPurchaseItemButton, bool isLeftHand)
		{
			this.ProcessPurchaseItemState(pressedPurchaseItemButton.buttonSide, isLeftHand);
		}

		// Token: 0x06006725 RID: 26405 RVA: 0x002130FC File Offset: 0x002112FC
		public void PurchaseBundle(StoreBundle bundleToPurchase, ICreatorCodeProvider ccp)
		{
			CosmeticsController.<PurchaseBundle>d__199 <PurchaseBundle>d__;
			<PurchaseBundle>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<PurchaseBundle>d__.<>4__this = this;
			<PurchaseBundle>d__.bundleToPurchase = bundleToPurchase;
			<PurchaseBundle>d__.ccp = ccp;
			<PurchaseBundle>d__.<>1__state = -1;
			<PurchaseBundle>d__.<>t__builder.Start<CosmeticsController.<PurchaseBundle>d__199>(ref <PurchaseBundle>d__);
		}

		// Token: 0x06006726 RID: 26406 RVA: 0x00213143 File Offset: 0x00211343
		private void OnCreatorCodeFailure()
		{
			this.buyingBundle = false;
		}

		// Token: 0x06006727 RID: 26407 RVA: 0x0021314C File Offset: 0x0021134C
		public void PressEarlyAccessButton()
		{
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
			this.ProcessPurchaseItemState("left", false);
			this.buyingBundle = true;
			this.itemToPurchase = this.BundlePlayfabItemName;
			ATM_Manager.instance.shinyRocksCost = (float)this.BundleShinyRocks;
			this.SteamPurchase();
		}

		// Token: 0x06006728 RID: 26408 RVA: 0x00213198 File Offset: 0x00211398
		public void ProcessPurchaseItemState(string buttonSide, bool isLeftHand)
		{
			switch (this.currentPurchaseItemStage)
			{
			case CosmeticsController.PurchaseItemStages.Start:
				this.itemToBuy = this.nullItem;
				this.FormattedPurchaseText("SELECT AN ITEM FROM YOUR CART TO PURCHASE!", null, null, false, false);
				this.UpdateShoppingCart();
				return;
			case CosmeticsController.PurchaseItemStages.CheckoutButtonPressed:
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_start, this.currentCart);
				this.searchIndex = this.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => this.itemToBuy.itemName == x.itemName);
				if (this.searchIndex > -1)
				{
					this.FormattedPurchaseText("YOU ALREADY OWN THIS ITEM!", "-", "-", true, true);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemOwned;
					return;
				}
				if (this.itemToBuy.cost <= this.currencyBalance)
				{
					this.FormattedPurchaseText("DO YOU WANT TO BUY THIS ITEM?", "NO!", "YES!", false, false);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemSelected;
					return;
				}
				this.FormattedPurchaseText("INSUFFICIENT SHINY ROCKS FOR THIS ITEM!", "-", "-", true, true);
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				return;
			case CosmeticsController.PurchaseItemStages.ItemSelected:
				if (buttonSide == "right")
				{
					GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.item_select, this.itemToBuy);
					this.FormattedPurchaseText("ARE YOU REALLY SURE?", "YES! I NEED IT!", "LET ME THINK ABOUT IT", false, false);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement;
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.ProcessPurchaseItemState(null, isLeftHand);
				return;
			case CosmeticsController.PurchaseItemStages.ItemOwned:
			case CosmeticsController.PurchaseItemStages.Buying:
				break;
			case CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement:
				if (buttonSide == "left")
				{
					this.FormattedPurchaseText("PURCHASING ITEM...", "-", "-", true, true);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Buying;
					this.isLastHandTouchedLeft = isLeftHand;
					this.PurchaseItem();
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.ProcessPurchaseItemState(null, isLeftHand);
				return;
			case CosmeticsController.PurchaseItemStages.Success:
			{
				this.FormattedPurchaseText("SUCCESS! ENJOY YOUR NEW ITEM!", "-", "-", true, true);
				GorillaTagger.Instance.offlineVRRig.AddCosmetic(this.itemToBuy.itemName);
				CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
				if (itemFromDict.bundledItems != null)
				{
					foreach (string cosmeticId in itemFromDict.bundledItems)
					{
						GorillaTagger.Instance.offlineVRRig.AddCosmetic(cosmeticId);
					}
				}
				this.tryOnSet.ClearSet(this.nullItem);
				CosmeticsController.ClearTryOnCollectable();
				this.UpdateShoppingCart();
				this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true);
				this.UpdateShoppingCart();
				this.UpdateWornCosmetics();
				this.UpdateWardrobeModelsAndButtons();
				Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
				if (onCosmeticsUpdated == null)
				{
					return;
				}
				onCosmeticsUpdated();
				break;
			}
			case CosmeticsController.PurchaseItemStages.Failure:
				this.FormattedPurchaseText("ERROR IN PURCHASING ITEM! NO MONEY WAS SPENT. SELECT ANOTHER ITEM.", "-", "-", true, true);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006729 RID: 26409 RVA: 0x00213418 File Offset: 0x00211618
		public void FormattedPurchaseText(string finalLineVar, string leftPurchaseButtonText = null, string rightPurchaseButtonText = null, bool leftButtonOn = false, bool rightButtonOn = false)
		{
			this.finalLine = finalLineVar;
			if (leftPurchaseButtonText != null)
			{
				this.leftCheckoutPurchaseButtonString = leftPurchaseButtonText;
				this.leftCheckoutPurchaseButtonOn = leftButtonOn;
			}
			if (rightPurchaseButtonText != null)
			{
				this.rightCheckoutPurchaseButtonString = rightPurchaseButtonText;
				this.rightCheckoutPurchaseButtonOn = rightButtonOn;
			}
			string newText = string.Concat(new string[]
			{
				"SELECTION: ",
				this.GetItemDisplayName(this.itemToBuy),
				"\nITEM COST: ",
				this.itemToBuy.cost.ToString(),
				"\nYOU HAVE: ",
				this.currencyBalance.ToString(),
				"\n\n",
				this.finalLine
			});
			this.iterator = 0;
			while (this.iterator < this.itemCheckouts.Count)
			{
				if (!this.itemCheckouts[this.iterator].IsNull())
				{
					this.itemCheckouts[this.iterator].UpdatePurchaseText(newText, leftPurchaseButtonText, rightPurchaseButtonText, leftButtonOn, rightButtonOn);
				}
				this.iterator++;
			}
		}

		// Token: 0x0600672A RID: 26410 RVA: 0x00213514 File Offset: 0x00211714
		public void PurchaseItem()
		{
			PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
			{
				ItemId = this.itemToBuy.itemName,
				Price = this.itemToBuy.cost,
				VirtualCurrency = this.currencyName,
				CatalogVersion = this.catalog
			}, delegate(PurchaseItemResult result)
			{
				if (result.Items.Count > 0)
				{
					foreach (ItemInstance itemInstance in result.Items)
					{
						CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
						if (itemFromDict.itemCategory == CosmeticsController.CosmeticCategory.Set)
						{
							this.UnlockItem(itemInstance.ItemId, false);
							foreach (string itemIdToUnlock in itemFromDict.bundledItems)
							{
								this.UnlockItem(itemIdToUnlock, false);
							}
						}
						else
						{
							this.UnlockItem(itemInstance.ItemId, false);
						}
					}
					this.UpdateMyCosmetics();
					if (NetworkSystem.Instance.InRoom)
					{
						base.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.itemToBuy.itemName));
					}
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Success;
					this.currencyBalance -= this.itemToBuy.cost;
					this.UpdateShoppingCart();
					this.ProcessPurchaseItemState(null, this.isLastHandTouchedLeft);
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
				this.ProcessPurchaseItemState(null, false);
			}, delegate(PlayFabError error)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
				this.ProcessPurchaseItemState(null, false);
			}, null, null);
		}

		// Token: 0x0600672B RID: 26411 RVA: 0x00213580 File Offset: 0x00211780
		private void UnlockItem(string itemIdToUnlock, bool relock = false)
		{
			int num = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => itemIdToUnlock == x.itemName);
			if (num > -1)
			{
				this.ModifyUnlockList(this.unlockedCosmetics, num, relock);
				if (relock)
				{
					this.concatStringCosmeticsAllowed.Replace(this.allCosmetics[num].itemName, string.Empty);
				}
				else
				{
					this.concatStringCosmeticsAllowed += this.allCosmetics[num].itemName;
				}
				switch (this.allCosmetics[num].itemCategory)
				{
				case CosmeticsController.CosmeticCategory.Hat:
					this.ModifyUnlockList(this.unlockedHats, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Badge:
					this.ModifyUnlockList(this.unlockedBadges, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Face:
					this.ModifyUnlockList(this.unlockedFaces, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Paw:
					if (!this.allCosmetics[num].isThrowable)
					{
						this.ModifyUnlockList(this.unlockedPaws, num, relock);
						return;
					}
					this.ModifyUnlockList(this.unlockedThrowables, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Chest:
					this.ModifyUnlockList(this.unlockedChests, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Fur:
					this.ModifyUnlockList(this.unlockedFurs, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Shirt:
					this.ModifyUnlockList(this.unlockedShirts, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Back:
					this.ModifyUnlockList(this.unlockedBacks, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Arms:
					this.ModifyUnlockList(this.unlockedArms, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Pants:
					this.ModifyUnlockList(this.unlockedPants, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.TagEffect:
					this.ModifyUnlockList(this.unlockedTagFX, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Count:
				case CosmeticsController.CosmeticCategory.Collectable:
					break;
				case CosmeticsController.CosmeticCategory.Set:
					foreach (string itemIdToUnlock2 in this.allCosmetics[num].bundledItems)
					{
						this.UnlockItem(itemIdToUnlock2, false);
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x0600672C RID: 26412 RVA: 0x00213760 File Offset: 0x00211960
		private void ModifyUnlockList(List<CosmeticsController.CosmeticItem> list, int index, bool relock)
		{
			if (!relock && !list.Contains(this.allCosmetics[index]))
			{
				list.Add(this.allCosmetics[index]);
				return;
			}
			if (relock && list.Contains(this.allCosmetics[index]))
			{
				list.Remove(this.allCosmetics[index]);
			}
		}

		// Token: 0x0600672D RID: 26413 RVA: 0x002137C1 File Offset: 0x002119C1
		private IEnumerator CheckIfMyCosmeticsUpdated(string itemToBuyID)
		{
			Debug.Log("Cosmetic updated check!");
			yield return new WaitForSecondsRealtime(1f);
			this.foundCosmetic = false;
			this.attempts = 0;
			while (!this.foundCosmetic && this.attempts < 10 && NetworkSystem.Instance.InRoom)
			{
				PlayFabClientAPI.GetSharedGroupData(new PlayFab.ClientModels.GetSharedGroupDataRequest
				{
					Keys = new List<string>
					{
						"Inventory"
					},
					SharedGroupId = NetworkSystem.Instance.LocalPlayer.UserId + "Inventory"
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
					if (this.foundCosmetic)
					{
						this.UpdateWornCosmetics(true);
					}
				}, delegate(PlayFabError error)
				{
					this.attempts++;
					this.ReauthOrBan(error);
				}, null, null);
				yield return new WaitForSecondsRealtime(1f);
			}
			Debug.Log("done!");
			yield break;
		}

		// Token: 0x0600672E RID: 26414 RVA: 0x002137D8 File Offset: 0x002119D8
		public void UpdateWardrobeModelsAndButtons()
		{
			foreach (WardrobeInstance wardrobeInstance in this.wardrobes)
			{
				wardrobeInstance.wardrobeItemButtons[0].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3] : this.nullItem);
				wardrobeInstance.wardrobeItemButtons[1].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 1 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 1] : this.nullItem);
				wardrobeInstance.wardrobeItemButtons[2].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 2 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 2] : this.nullItem);
				this.iterator = 0;
				while (this.iterator < wardrobeInstance.wardrobeItemButtons.Length)
				{
					CosmeticsController.CosmeticItem currentCosmeticItem = wardrobeInstance.wardrobeItemButtons[this.iterator].currentCosmeticItem;
					wardrobeInstance.wardrobeItemButtons[this.iterator].isOn = (!currentCosmeticItem.isNullItem && this.AnyMatch(this.currentWornSet, currentCosmeticItem));
					wardrobeInstance.wardrobeItemButtons[this.iterator].UpdateColor();
					this.iterator++;
				}
				wardrobeInstance.wardrobeItemButtons[0].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[0].currentCosmeticItem.displayName, false);
				wardrobeInstance.wardrobeItemButtons[1].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[1].currentCosmeticItem.displayName, false);
				wardrobeInstance.wardrobeItemButtons[2].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[2].currentCosmeticItem.displayName, false);
				wardrobeInstance.selfDoll.SetCosmeticActiveArray(this.currentWornSet.ToDisplayNameArray(), this.currentWornSet.ToOnRightSideArray());
			}
		}

		// Token: 0x0600672F RID: 26415 RVA: 0x00213A50 File Offset: 0x00211C50
		public int GetCategorySize(CosmeticsController.CosmeticCategory category)
		{
			int indexForCategory = this.GetIndexForCategory(category);
			if (indexForCategory != -1)
			{
				return this.itemLists[indexForCategory].Count;
			}
			return 0;
		}

		// Token: 0x06006730 RID: 26416 RVA: 0x00213A78 File Offset: 0x00211C78
		public CosmeticsController.CosmeticItem GetCosmetic(int category, int cosmeticIndex)
		{
			if (cosmeticIndex >= this.itemLists[category].Count || cosmeticIndex < 0)
			{
				return this.nullItem;
			}
			return this.itemLists[category][cosmeticIndex];
		}

		// Token: 0x06006731 RID: 26417 RVA: 0x00213AA3 File Offset: 0x00211CA3
		public CosmeticsController.CosmeticItem GetCosmetic(CosmeticsController.CosmeticCategory category, int cosmeticIndex)
		{
			return this.GetCosmetic(this.GetIndexForCategory(category), cosmeticIndex);
		}

		// Token: 0x06006732 RID: 26418 RVA: 0x00213AB4 File Offset: 0x00211CB4
		private int GetIndexForCategory(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return 0;
			case CosmeticsController.CosmeticCategory.Badge:
				return 2;
			case CosmeticsController.CosmeticCategory.Face:
				return 1;
			case CosmeticsController.CosmeticCategory.Paw:
				return 3;
			case CosmeticsController.CosmeticCategory.Chest:
				return 9;
			case CosmeticsController.CosmeticCategory.Fur:
				return 4;
			case CosmeticsController.CosmeticCategory.Shirt:
				return 5;
			case CosmeticsController.CosmeticCategory.Back:
				return 8;
			case CosmeticsController.CosmeticCategory.Arms:
				return 7;
			case CosmeticsController.CosmeticCategory.Pants:
				return 6;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return 10;
			default:
				return 0;
			}
		}

		// Token: 0x06006733 RID: 26419 RVA: 0x00213B10 File Offset: 0x00211D10
		public bool IsCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic)
		{
			return this.AnyMatch(this.currentWornSet, cosmetic);
		}

		// Token: 0x06006734 RID: 26420 RVA: 0x00213B1F File Offset: 0x00211D1F
		public bool IsCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic, bool tempSet)
		{
			if (!tempSet)
			{
				return this.IsCosmeticEquipped(cosmetic);
			}
			return this.IsTemporaryCosmeticEquipped(cosmetic);
		}

		// Token: 0x06006735 RID: 26421 RVA: 0x00213B33 File Offset: 0x00211D33
		public bool IsTemporaryCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic)
		{
			return this.AnyMatch(this.tempUnlockedSet, cosmetic);
		}

		// Token: 0x06006736 RID: 26422 RVA: 0x00213B44 File Offset: 0x00211D44
		public CosmeticsController.CosmeticItem GetSlotItem(CosmeticsController.CosmeticSlots slot, bool checkOpposite = true, bool tempSet = false)
		{
			int num = (int)slot;
			if (checkOpposite)
			{
				num = (int)CosmeticsController.CosmeticSet.OppositeSlot(slot);
			}
			if (!tempSet)
			{
				return this.currentWornSet.items[num];
			}
			return this.tempUnlockedSet.items[num];
		}

		// Token: 0x06006737 RID: 26423 RVA: 0x00213B83 File Offset: 0x00211D83
		public string[] GetCurrentlyWornCosmetics(bool tempSet = false)
		{
			if (!tempSet)
			{
				return this.currentWornSet.ToDisplayNameArray();
			}
			return this.tempUnlockedSet.ToDisplayNameArray();
		}

		// Token: 0x06006738 RID: 26424 RVA: 0x00213B9F File Offset: 0x00211D9F
		public bool[] GetCurrentRightEquippedSided(bool tempSet = false)
		{
			if (!tempSet)
			{
				return this.currentWornSet.ToOnRightSideArray();
			}
			return this.tempUnlockedSet.ToOnRightSideArray();
		}

		// Token: 0x06006739 RID: 26425 RVA: 0x00213BBC File Offset: 0x00211DBC
		public void UpdateShoppingCart()
		{
			this.iterator = 0;
			while (this.iterator < this.itemCheckouts.Count)
			{
				if (!this.itemCheckouts[this.iterator].IsNull())
				{
					this.itemCheckouts[this.iterator].UpdateFromCart(this.currentCart, this.itemToBuy);
				}
				this.iterator++;
			}
			this.iterator = 0;
			while (this.iterator < this.fittingRooms.Count)
			{
				if (!this.fittingRooms[this.iterator].IsNull())
				{
					this.fittingRooms[this.iterator].UpdateFromCart(this.currentCart, this.tryOnSet);
				}
				this.iterator++;
			}
			this.UpdateWardrobeModelsAndButtons();
		}

		// Token: 0x0600673A RID: 26426 RVA: 0x00213C97 File Offset: 0x00211E97
		public void UpdateWornCosmetics()
		{
			this.UpdateWornCosmetics(false, false);
		}

		// Token: 0x0600673B RID: 26427 RVA: 0x00213CA1 File Offset: 0x00211EA1
		public void UpdateWornCosmetics(bool sync)
		{
			this.UpdateWornCosmetics(sync, false);
		}

		// Token: 0x0600673C RID: 26428 RVA: 0x00213CAC File Offset: 0x00211EAC
		public void UpdateWornCosmetics(bool sync, bool playfx)
		{
			VRRig localRig = VRRig.LocalRig;
			this.activeMergedSet.MergeInSets(this.currentWornSet, this.tempUnlockedSet, (string id) => PlayerCosmeticsSystem.LocalPlayerInTemporaryCosmeticSpace() || PlayerCosmeticsSystem.IsTemporaryCosmeticAllowed(localRig, id));
			GorillaTagger.Instance.offlineVRRig.LocalUpdateCosmeticsWithTryon(this.activeMergedSet, this.tryOnSet, playfx);
			if (sync && GorillaTagger.Instance.myVRRig != null)
			{
				if (this.isHidingCosmeticsFromRemotePlayers)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_HideAllCosmetics", RpcTarget.All, Array.Empty<object>());
					return;
				}
				int[] array = this.activeMergedSet.ToPackedIDArray();
				int[] array2 = this.tryOnSet.ToPackedIDArray();
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, new object[]
				{
					array,
					array2,
					playfx
				});
				CosmeticCollectionDisplay.GetDisplaysForRig(GorillaTagger.Instance.offlineVRRig.GetInstanceID(), CosmeticsController.scratchDisplayList);
				if (CosmeticsController.scratchDisplayList.Count > 0)
				{
					int num = CosmeticsController.scratchDisplayList.Count * 2;
					if (CosmeticsController.cycleStatesArray.Length != num)
					{
						CosmeticsController.cycleStatesArray = new int[num];
					}
					for (int i = 0; i < CosmeticsController.scratchDisplayList.Count; i++)
					{
						string parentPlayFabID = CosmeticsController.scratchDisplayList[i].ParentPlayFabID;
						CosmeticsController.cycleStatesArray[i * 2] = (int)(parentPlayFabID[0] - 'A' + '\u001a' * (parentPlayFabID[1] - 'A' + '\u001a' * (parentPlayFabID[2] - 'A' + '\u001a' * (parentPlayFabID[3] - 'A' + '\u001a' * (parentPlayFabID[4] - 'A')))));
						CosmeticsController.cycleStatesArray[i * 2 + 1] = CosmeticsController.scratchDisplayList[i].ActiveIndex;
					}
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithCollectablesPacked", RpcTarget.Others, new object[]
					{
						CosmeticsController.cycleStatesArray
					});
				}
			}
		}

		// Token: 0x0600673D RID: 26429 RVA: 0x00213E99 File Offset: 0x00212099
		public CosmeticsController.CosmeticItem GetItemFromDict(string itemID)
		{
			if (!this.allCosmeticsDict.TryGetValue(itemID, out this.cosmeticItemVar))
			{
				return this.nullItem;
			}
			return this.cosmeticItemVar;
		}

		// Token: 0x0600673E RID: 26430 RVA: 0x00213EBC File Offset: 0x002120BC
		public string GetItemNameFromDisplayName(string displayName)
		{
			if (displayName == "" || displayName == null)
			{
				return "null";
			}
			if (!this.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(displayName, out this.returnString))
			{
				return "null";
			}
			return this.returnString;
		}

		// Token: 0x0600673F RID: 26431 RVA: 0x00213EF4 File Offset: 0x002120F4
		public CosmeticSO GetCosmeticSOFromDisplayName(string displayName)
		{
			string itemNameFromDisplayName = this.GetItemNameFromDisplayName(displayName);
			if (itemNameFromDisplayName.Equals("null"))
			{
				return null;
			}
			AllCosmeticsArraySO allCosmeticsArraySO = this.v2_allCosmeticsInfoAssetRef.Asset as AllCosmeticsArraySO;
			if (allCosmeticsArraySO == null)
			{
				GTDev.LogWarning<string>("null AllCosmeticsArraySO", null);
				return null;
			}
			CosmeticSO cosmeticSO = allCosmeticsArraySO.SearchForCosmeticSO(itemNameFromDisplayName);
			if (cosmeticSO != null)
			{
				return cosmeticSO;
			}
			GTDev.Log<string>("Could not find cosmetic info for " + itemNameFromDisplayName, null);
			return null;
		}

		// Token: 0x06006740 RID: 26432 RVA: 0x00213F64 File Offset: 0x00212164
		public CosmeticAnchorAntiIntersectOffsets GetClipOffsetsFromDisplayName(string displayName)
		{
			string itemNameFromDisplayName = this.GetItemNameFromDisplayName(displayName);
			if (itemNameFromDisplayName.Equals("null"))
			{
				return this.defaultClipOffsets;
			}
			AllCosmeticsArraySO allCosmeticsArraySO = this.v2_allCosmeticsInfoAssetRef.Asset as AllCosmeticsArraySO;
			if (allCosmeticsArraySO == null)
			{
				GTDev.LogWarning<string>("null AllCosmeticsArraySO", null);
				return this.defaultClipOffsets;
			}
			CosmeticSO cosmeticSO = allCosmeticsArraySO.SearchForCosmeticSO(itemNameFromDisplayName);
			if (cosmeticSO != null)
			{
				return cosmeticSO.info.anchorAntiIntersectOffsets;
			}
			GTDev.Log<string>("Could not find cosmetic info for " + itemNameFromDisplayName, null);
			return this.defaultClipOffsets;
		}

		// Token: 0x06006741 RID: 26433 RVA: 0x00213FF0 File Offset: 0x002121F0
		public bool AnyMatch(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem item)
		{
			if (item.itemCategory != CosmeticsController.CosmeticCategory.Set)
			{
				return set.IsActive(item.displayName);
			}
			if (item.itemCategory == CosmeticsController.CosmeticCategory.Set && item.bundledItems != null)
			{
				if (item.bundledItems.Length == 1)
				{
					return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0]));
				}
				if (item.bundledItems.Length == 2)
				{
					return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1]));
				}
				if (item.bundledItems.Length >= 3)
				{
					return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[2]));
				}
			}
			return false;
		}

		// Token: 0x06006742 RID: 26434 RVA: 0x002140DC File Offset: 0x002122DC
		public void Initialize()
		{
			if (!base.gameObject.activeSelf || this.v2_isCosmeticPlayFabCatalogDataLoaded || this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback)
			{
				return;
			}
			if (this.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				this.GetCosmeticsPlayFabCatalogData();
				return;
			}
			this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback = true;
			this.V2_allCosmeticsInfoAssetRef_OnPostLoad = (Action)Delegate.Combine(this.V2_allCosmeticsInfoAssetRef_OnPostLoad, new Action(this.GetCosmeticsPlayFabCatalogData));
		}

		// Token: 0x06006743 RID: 26435 RVA: 0x0021413F File Offset: 0x0021233F
		public void GetLastDailyLogin()
		{
			PlayFabClientAPI.GetUserReadOnlyData(new PlayFab.ClientModels.GetUserDataRequest(), delegate(GetUserDataResult result)
			{
				if (result.Data.TryGetValue("DailyLogin", out this.userDataRecord))
				{
					this.lastDailyLogin = this.userDataRecord.Value;
					return;
				}
				this.lastDailyLogin = "NONE";
				base.StartCoroutine(this.GetMyDaily());
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error getting read-only user data:");
				Debug.Log(error.GenerateErrorReport());
				this.lastDailyLogin = "FAILED";
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
		}

		// Token: 0x06006744 RID: 26436 RVA: 0x00214165 File Offset: 0x00212365
		private IEnumerator CheckCanGetDaily()
		{
			while (!KIDManager.InitialisationComplete)
			{
				yield return new WaitForSecondsRealtime(1f);
			}
			while (!PlayFabClientAPI.IsClientLoggedIn())
			{
				yield return new WaitForSecondsRealtime(1f);
			}
			for (;;)
			{
				if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
				{
					this.currentTime = new DateTime((GorillaComputer.instance.startupMillis + (long)(Time.realtimeSinceStartup * 1000f)) * 10000L);
					this.secondsUntilTomorrow = (int)(this.currentTime.AddDays(1.0).Date - this.currentTime).TotalSeconds;
					if (string.IsNullOrEmpty(this.lastDailyLogin))
					{
						this.GetLastDailyLogin();
					}
					else
					{
						string a = this.currentTime.ToString("o").Substring(0, 10);
						if (a == this.lastDailyLogin)
						{
							this.checkedDaily = true;
							this.gotMyDaily = true;
						}
						else if (a != this.lastDailyLogin)
						{
							this.checkedDaily = true;
							this.gotMyDaily = false;
							base.StartCoroutine(this.GetMyDaily());
						}
						else if (this.lastDailyLogin == "FAILED")
						{
							this.GetLastDailyLogin();
						}
					}
					this.secondsToWaitToCheckDaily = (this.checkedDaily ? 60f : 10f);
					this.UpdateCurrencyBoards();
					yield return new WaitForSecondsRealtime(this.secondsToWaitToCheckDaily);
				}
				else
				{
					yield return new WaitForSecondsRealtime(1f);
				}
			}
			yield break;
		}

		// Token: 0x06006745 RID: 26437 RVA: 0x00214174 File Offset: 0x00212374
		private IEnumerator GetMyDaily()
		{
			yield return new WaitForSecondsRealtime(10f);
			GorillaServer.Instance.TryDistributeCurrency(delegate(ExecuteFunctionResult result)
			{
				this.GetCurrencyBalance();
				this.GetLastDailyLogin();
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			});
			yield break;
		}

		// Token: 0x06006746 RID: 26438 RVA: 0x00214183 File Offset: 0x00212383
		public void GetCosmeticsPlayFabCatalogData()
		{
			this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback = false;
			if (!this.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				throw new Exception("Method `GetCosmeticsPlayFabCatalogData` was called before `v2_allCosmeticsInfoAssetRef` was loaded. Listen to callback `V2_allCosmeticsInfoAssetRef_OnPostLoad` or check `v2_allCosmeticsInfoAssetRef_isLoaded` before trying to get PlayFab catalog data.");
			}
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest
				{
					CatalogVersion = this.catalog
				}, delegate(GetCatalogItemsResult result2)
				{
					this.unlockedCosmetics.Clear();
					this.unlockedHats.Clear();
					this.unlockedBadges.Clear();
					this.unlockedFaces.Clear();
					this.unlockedPaws.Clear();
					this.unlockedFurs.Clear();
					this.unlockedShirts.Clear();
					this.unlockedPants.Clear();
					this.unlockedArms.Clear();
					this.unlockedBacks.Clear();
					this.unlockedChests.Clear();
					this.unlockedTagFX.Clear();
					this.unlockedThrowables.Clear();
					this.catalogItems = result2.Catalog;
					using (List<CatalogItem>.Enumerator enumerator = this.catalogItems.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CatalogItem catalogItem = enumerator.Current;
							if (!BuilderSetManager.IsItemIDBuilderItem(catalogItem.ItemId))
							{
								this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => catalogItem.ItemId == x.itemName);
								if (this.searchIndex > -1)
								{
									this.tempStringArray = null;
									this.hasPrice = false;
									if (catalogItem.Bundle != null)
									{
										this.tempStringArray = catalogItem.Bundle.BundledItems.ToArray();
									}
									uint cost;
									if (catalogItem.VirtualCurrencyPrices.TryGetValue(this.currencyName, out cost))
									{
										this.hasPrice = true;
									}
									CosmeticsController.CosmeticItem cosmeticItem = this.allCosmetics[this.searchIndex];
									cosmeticItem.itemName = catalogItem.ItemId;
									cosmeticItem.displayName = catalogItem.DisplayName;
									cosmeticItem.cost = (int)cost;
									cosmeticItem.bundledItems = this.tempStringArray;
									cosmeticItem.canTryOn = this.hasPrice;
									if (cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Paw)
									{
										CosmeticInfoV2 cosmeticInfoV = this.v2_allCosmetics[this.searchIndex];
										cosmeticItem.isThrowable = (cosmeticInfoV.isThrowable && !cosmeticInfoV.hasWardrobeParts);
									}
									if (cosmeticItem.displayName == null)
									{
										string text = "null";
										if (this.allCosmetics[this.searchIndex].itemPicture)
										{
											text = this.allCosmetics[this.searchIndex].itemPicture.name;
										}
										string debugCosmeticSOName = this.v2_allCosmetics[this.searchIndex].debugCosmeticSOName;
										Debug.LogError(string.Concat(new string[]
										{
											string.Format("Cosmetic encountered with a null displayName at index {0}! ", this.searchIndex),
											"Setting displayName to id: \"",
											this.allCosmetics[this.searchIndex].itemName,
											"\". iconName=\"",
											text,
											"\".cosmeticSOName=\"",
											debugCosmeticSOName,
											"\". "
										}));
										cosmeticItem.displayName = cosmeticItem.itemName;
									}
									this.V2_ConformCosmeticItemV1DisplayName(ref cosmeticItem);
									this._allCosmetics[this.searchIndex] = cosmeticItem;
									this._allCosmeticsDict[cosmeticItem.itemName] = cosmeticItem;
									this._allCosmeticsItemIDsfromDisplayNamesDict[cosmeticItem.displayName] = cosmeticItem.itemName;
									this._allCosmeticsItemIDsfromDisplayNamesDict[cosmeticItem.overrideDisplayName] = cosmeticItem.itemName;
								}
							}
						}
					}
					for (int i = this._allCosmetics.Count - 1; i > -1; i--)
					{
						this.tempItem = this._allCosmetics[i];
						if (this.tempItem.itemCategory == CosmeticsController.CosmeticCategory.Set && this.tempItem.canTryOn)
						{
							string[] bundledItems = this.tempItem.bundledItems;
							for (int j = 0; j < bundledItems.Length; j++)
							{
								string setItemName = bundledItems[j];
								this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => setItemName == x.itemName);
								if (this.searchIndex > -1)
								{
									this.tempItem = this._allCosmetics[this.searchIndex];
									this.tempItem.canTryOn = true;
									this._allCosmetics[this.searchIndex] = this.tempItem;
									this._allCosmeticsDict[this._allCosmetics[this.searchIndex].itemName] = this.tempItem;
									this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this.tempItem.itemName;
								}
							}
						}
					}
					foreach (KeyValuePair<string, StoreBundle> keyValuePair in BundleManager.instance.storeBundlesById)
					{
						string text2;
						StoreBundle bundleData2;
						keyValuePair.Deconstruct(out text2, out bundleData2);
						string key = text2;
						StoreBundle bundleData = bundleData2;
						int num = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => bundleData.playfabBundleID == x.itemName);
						if (num > 0 && this._allCosmetics[num].bundledItems != null)
						{
							string[] bundledItems = this._allCosmetics[num].bundledItems;
							for (int j = 0; j < bundledItems.Length; j++)
							{
								string setItemName = bundledItems[j];
								this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => setItemName == x.itemName);
								if (this.searchIndex > -1)
								{
									this.tempItem = this._allCosmetics[this.searchIndex];
									this.tempItem.canTryOn = true;
									this._allCosmetics[this.searchIndex] = this.tempItem;
									this._allCosmeticsDict[this._allCosmetics[this.searchIndex].itemName] = this.tempItem;
									this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this.tempItem.itemName;
								}
							}
						}
						if (!bundleData.HasPrice)
						{
							num = this.catalogItems.FindIndex((CatalogItem ci) => ci.Bundle != null && ci.ItemId == bundleData.playfabBundleID);
							if (num > 0)
							{
								uint bundlePrice;
								if (this.catalogItems[num].VirtualCurrencyPrices.TryGetValue("RM", out bundlePrice))
								{
									BundleManager.instance.storeBundlesById[key].TryUpdatePrice(bundlePrice);
								}
								else
								{
									BundleManager.instance.storeBundlesById[key].TryUpdatePrice(null);
								}
							}
						}
					}
					this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => "Slingshot" == x.itemName);
					if (this.searchIndex < 0)
					{
						throw new MissingReferenceException("CosmeticsController: Cannot find default slingshot! it is required for players that do not have another slingshot equipped and are playing Paintbrawl.");
					}
					this._allCosmeticsDict["Slingshot"] = this._allCosmetics[this.searchIndex];
					this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this._allCosmetics[this.searchIndex].itemName;
					this.allCosmeticsDict_isInitialized = true;
					this.allCosmeticsItemIDsfromDisplayNamesDict_isInitialized = true;
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					using (List<ItemInstance>.Enumerator enumerator3 = result.Inventory.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							ItemInstance item = enumerator3.Current;
							if (!BuilderSetManager.IsItemIDBuilderItem(item.ItemId))
							{
								if (item.ItemId == this.m_earlyAccessSupporterPackCosmeticSO.info.playFabID)
								{
									foreach (CosmeticSO cosmeticSO in this.m_earlyAccessSupporterPackCosmeticSO.info.setCosmetics)
									{
										CosmeticsController.CosmeticItem item2;
										if (this.allCosmeticsDict.TryGetValue(cosmeticSO.info.playFabID, out item2))
										{
											this.unlockedCosmetics.Add(item2);
										}
									}
								}
								BundleManager.instance.MarkBundleOwnedByPlayFabID(item.ItemId);
								if (!dictionary.ContainsKey(item.ItemId))
								{
									this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.ItemId == x.itemName);
									if (this.searchIndex > -1)
									{
										dictionary[item.ItemId] = item.ItemId;
										this.unlockedCosmetics.Add(this.allCosmetics[this.searchIndex]);
									}
								}
							}
						}
					}
					foreach (CosmeticsController.CosmeticItem cosmeticItem2 in this.unlockedCosmetics)
					{
						if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Hat && !this.unlockedHats.Contains(cosmeticItem2))
						{
							this.unlockedHats.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Face && !this.unlockedFaces.Contains(cosmeticItem2))
						{
							this.unlockedFaces.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Badge && !this.unlockedBadges.Contains(cosmeticItem2))
						{
							this.unlockedBadges.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Paw)
						{
							if (!cosmeticItem2.isThrowable && !this.unlockedPaws.Contains(cosmeticItem2))
							{
								this.unlockedPaws.Add(cosmeticItem2);
							}
							else if (cosmeticItem2.isThrowable && !this.unlockedThrowables.Contains(cosmeticItem2))
							{
								this.unlockedThrowables.Add(cosmeticItem2);
							}
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Fur && !this.unlockedFurs.Contains(cosmeticItem2))
						{
							this.unlockedFurs.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Shirt && !this.unlockedShirts.Contains(cosmeticItem2))
						{
							this.unlockedShirts.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Arms && !this.unlockedArms.Contains(cosmeticItem2))
						{
							this.unlockedArms.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Back && !this.unlockedBacks.Contains(cosmeticItem2))
						{
							this.unlockedBacks.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Chest && !this.unlockedChests.Contains(cosmeticItem2))
						{
							this.unlockedChests.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Pants && !this.unlockedPants.Contains(cosmeticItem2))
						{
							this.unlockedPants.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.TagEffect && !this.unlockedTagFX.Contains(cosmeticItem2))
						{
							this.unlockedTagFX.Add(cosmeticItem2);
						}
						this.concatStringCosmeticsAllowed += cosmeticItem2.itemName;
					}
					BuilderSetManager.instance.OnGotInventoryItems(result, result2);
					this.currencyBalance = result.VirtualCurrency[this.currencyName];
					int num2;
					this.playedInBeta = (result.VirtualCurrency.TryGetValue("TC", out num2) && num2 > 0);
					Action onGetCurrency = this.OnGetCurrency;
					if (onGetCurrency != null)
					{
						onGetCurrency();
					}
					BundleManager.instance.CheckIfBundlesOwned();
					StoreUpdater.instance.Initialize();
					this.currentWornSet.LoadFromPlayerPreferences(this);
					this.LoadSavedOutfits();
					if (!ATM_Manager.instance.alreadyBegan)
					{
						ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Begin);
						ATM_Manager.instance.alreadyBegan = true;
					}
					this.ProcessPurchaseItemState(null, false);
					this.UpdateShoppingCart();
					this.UpdateCurrencyBoards();
					this.ConfirmIndividualCosmeticsSharedGroup(result);
					Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
					if (onCosmeticsUpdated != null)
					{
						onCosmeticsUpdated();
					}
					this.v2_isCosmeticPlayFabCatalogDataLoaded = true;
					Action v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess = this.V2_OnGetCosmeticsPlayFabCatalogData_PostSuccess;
					if (v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess != null)
					{
						v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess();
					}
					CosmeticsV2Spawner_Dirty.PrepareLoadOpInfos();
				}, delegate(PlayFabError error)
				{
					if (error.Error == PlayFabErrorCode.NotAuthenticated)
					{
						PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					}
					else if (error.Error == PlayFabErrorCode.AccountBanned)
					{
						Application.Quit();
						NetworkSystem.Instance.ReturnToSinglePlayer();
						Object.DestroyImmediate(PhotonNetworkController.Instance);
						Object.DestroyImmediate(GTPlayer.Instance);
						GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
						for (int i = 0; i < array.Length; i++)
						{
							Object.Destroy(array[i]);
						}
					}
					if (!this.tryTwice)
					{
						this.tryTwice = true;
						this.GetCosmeticsPlayFabCatalogData();
					}
				}, null, null);
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				}
				else if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
				if (!this.tryTwice)
				{
					this.tryTwice = true;
					this.GetCosmeticsPlayFabCatalogData();
				}
			}, null, null);
		}

		// Token: 0x06006747 RID: 26439 RVA: 0x002141C4 File Offset: 0x002123C4
		public void SteamPurchase()
		{
			if (string.IsNullOrEmpty(this.itemToPurchase))
			{
				Debug.Log("Unable to start steam purchase process. itemToPurchase is not set.");
				return;
			}
			Debug.Log(string.Format("attempting to purchase item through steam. Is this a bundle purchase: {0}", this.buyingBundle));
			PlayFabClientAPI.StartPurchase(this.GetStartPurchaseRequest(), new Action<StartPurchaseResult>(this.ProcessStartPurchaseResponse), new Action<PlayFabError>(this.ProcessSteamPurchaseError), null, null);
		}

		// Token: 0x06006748 RID: 26440 RVA: 0x00214228 File Offset: 0x00212428
		private StartPurchaseRequest GetStartPurchaseRequest()
		{
			return new StartPurchaseRequest
			{
				CatalogVersion = this.catalog,
				Items = new List<ItemPurchaseRequest>
				{
					new ItemPurchaseRequest
					{
						ItemId = this.itemToPurchase,
						Quantity = 1U,
						Annotation = "Purchased via in-game store"
					}
				}
			};
		}

		// Token: 0x06006749 RID: 26441 RVA: 0x0021427C File Offset: 0x0021247C
		private void ProcessStartPurchaseResponse(StartPurchaseResult result)
		{
			Debug.Log("successfully started purchase. attempted to pay for purchase through steam");
			this.currentPurchaseID = result.OrderId;
			PlayFabClientAPI.PayForPurchase(CosmeticsController.GetPayForPurchaseRequest(this.currentPurchaseID), new Action<PayForPurchaseResult>(CosmeticsController.ProcessPayForPurchaseResult), new Action<PlayFabError>(this.ProcessSteamPurchaseError), null, null);
		}

		// Token: 0x0600674A RID: 26442 RVA: 0x002142C9 File Offset: 0x002124C9
		private static PayForPurchaseRequest GetPayForPurchaseRequest(string orderId)
		{
			return new PayForPurchaseRequest
			{
				OrderId = orderId,
				ProviderName = "Steam",
				Currency = "RM"
			};
		}

		// Token: 0x0600674B RID: 26443 RVA: 0x002142ED File Offset: 0x002124ED
		private static void ProcessPayForPurchaseResult(PayForPurchaseResult result)
		{
			Debug.Log("succeeded on sending request for paying with steam! waiting for response");
		}

		// Token: 0x0600674C RID: 26444 RVA: 0x002142FC File Offset: 0x002124FC
		private void ProcessSteamCallback(MicroTxnAuthorizationResponse_t callBackResponse)
		{
			if (SubscriptionKiosk.ProcessingSubscriptionPurchase)
			{
				return;
			}
			Debug.Log("Steam has called back that the user has finished the payment interaction");
			if (callBackResponse.m_bAuthorized == 0)
			{
				Debug.Log("Steam has indicated that the payment was not authorised.");
			}
			if (this.buyingBundle)
			{
				PlayFabClientAPI.ConfirmPurchase(this.GetConfirmBundlePurchaseRequest(), delegate(ConfirmPurchaseResult _)
				{
					this.ProcessConfirmPurchaseSuccess();
				}, new Action<PlayFabError>(this.ProcessConfirmPurchaseError), null, null);
				return;
			}
			PlayFabClientAPI.ConfirmPurchase(this.GetConfirmATMPurchaseRequest(), delegate(ConfirmPurchaseResult _)
			{
				this.ProcessConfirmPurchaseSuccess();
			}, new Action<PlayFabError>(this.ProcessConfirmPurchaseError), null, null);
		}

		// Token: 0x0600674D RID: 26445 RVA: 0x00214380 File Offset: 0x00212580
		private ConfirmPurchaseRequest GetConfirmBundlePurchaseRequest()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"PlayerName",
					GorillaComputer.instance.savedName
				},
				{
					"Location",
					this.ConsumePurchaseLocation()
				}
			};
			if (this.validatedCreatorCode != null)
			{
				dictionary.Add("NexusCreatorId", this.validatedCreatorCode.memberCode);
				dictionary.Add("NexusGroupId", this.validatedCreatorCode.groupId);
			}
			return new ConfirmPurchaseRequest
			{
				OrderId = this.currentPurchaseID,
				CustomTags = dictionary
			};
		}

		// Token: 0x0600674E RID: 26446 RVA: 0x00214408 File Offset: 0x00212608
		private ConfirmPurchaseRequest GetConfirmATMPurchaseRequest()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"PlayerName",
					GorillaComputer.instance.savedName
				},
				{
					"Location",
					this.ConsumePurchaseLocation()
				}
			};
			if (this.validatedCreatorCode != null)
			{
				dictionary.Add("NexusCreatorId", this.validatedCreatorCode.memberCode);
				dictionary.Add("NexusGroupId", this.validatedCreatorCode.groupId);
			}
			return new ConfirmPurchaseRequest
			{
				OrderId = this.currentPurchaseID,
				CustomTags = dictionary
			};
		}

		// Token: 0x0600674F RID: 26447 RVA: 0x00214490 File Offset: 0x00212690
		private void ProcessConfirmPurchaseSuccess()
		{
			if (this.buyingBundle)
			{
				if (this.validatedCreatorCode != null && CosmeticsController.PushTerminalMessage != null)
				{
					CosmeticsController.PushTerminalMessage(this.validatedCreatorCode.terminalId, "THIS PURCHASE SUPPORTED\n" + CreatorCodes.supportedMember.name + "!");
				}
				this.buyingBundle = false;
				this.UpdateMyCosmetics();
				base.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.BundlePlayfabItemName));
			}
			else
			{
				ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Success);
			}
			this.GetCurrencyBalance();
			this.UpdateCurrencyBoards();
			this.GetCosmeticsPlayFabCatalogData();
			GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
		}

		// Token: 0x06006750 RID: 26448 RVA: 0x00214532 File Offset: 0x00212732
		private void ProcessConfirmPurchaseError(PlayFabError error)
		{
			this.ProcessSteamPurchaseError(error);
			ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Failure);
			this.UpdateCurrencyBoards();
		}

		// Token: 0x06006751 RID: 26449 RVA: 0x00214550 File Offset: 0x00212750
		private void ProcessSteamPurchaseError(PlayFabError error)
		{
			PlayFabErrorCode error2 = error.Error;
			if (error2 <= PlayFabErrorCode.PurchaseInitializationFailure)
			{
				if (error2 <= PlayFabErrorCode.FailedByPaymentProvider)
				{
					if (error2 == PlayFabErrorCode.AccountBanned)
					{
						PhotonNetwork.Disconnect();
						Object.DestroyImmediate(PhotonNetworkController.Instance);
						Object.DestroyImmediate(GTPlayer.Instance);
						GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
						for (int i = 0; i < array.Length; i++)
						{
							Object.Destroy(array[i]);
						}
						Application.Quit();
						goto IL_1A2;
					}
					if (error2 != PlayFabErrorCode.FailedByPaymentProvider)
					{
						goto IL_192;
					}
					Debug.Log(string.Format("Attempted to pay for order, but has been Failed by Steam with error: {0}", error));
					goto IL_1A2;
				}
				else
				{
					if (error2 == PlayFabErrorCode.InsufficientFunds)
					{
						Debug.Log(string.Format("Attempting to do purchase through steam, steam has returned insufficient funds: {0}", error));
						goto IL_1A2;
					}
					if (error2 == PlayFabErrorCode.InvalidPaymentProvider)
					{
						Debug.Log(string.Format("Attempted to connect to steam as payment provider, but received error: {0}", error));
						goto IL_1A2;
					}
					if (error2 != PlayFabErrorCode.PurchaseInitializationFailure)
					{
						goto IL_192;
					}
				}
			}
			else if (error2 <= PlayFabErrorCode.InvalidPurchaseTransactionStatus)
			{
				if (error2 == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					goto IL_1A2;
				}
				if (error2 == PlayFabErrorCode.PurchaseDoesNotExist)
				{
					Debug.Log(string.Format("Attempting to confirm purchase for order {0} but received error: {1}", this.currentPurchaseID, error));
					goto IL_1A2;
				}
				if (error2 != PlayFabErrorCode.InvalidPurchaseTransactionStatus)
				{
					goto IL_192;
				}
			}
			else
			{
				if (error2 == PlayFabErrorCode.InternalServerError)
				{
					Debug.Log(string.Format("PlayFab threw an internal server error: {0}", error));
					goto IL_1A2;
				}
				if (error2 == PlayFabErrorCode.StoreNotFound)
				{
					Debug.Log(string.Format("Attempted to load {0} from {1} but received an error: {2}", this.itemToPurchase, this.catalog, error));
					goto IL_1A2;
				}
				if (error2 != PlayFabErrorCode.DuplicatePurchaseTransactionId)
				{
					goto IL_192;
				}
			}
			Debug.Log(string.Format("Attempted to pay for order {0}, however received an error: {1}", this.currentPurchaseID, error));
			goto IL_1A2;
			IL_192:
			Debug.Log(string.Format("Steam purchase flow returned error: {0}", error));
			IL_1A2:
			ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Failure);
		}

		// Token: 0x06006752 RID: 26450 RVA: 0x0021470C File Offset: 0x0021290C
		public void UpdateCurrencyBoards()
		{
			this.FormattedPurchaseText(this.finalLine, null, null, false, false);
			this.iterator = 0;
			while (this.iterator < this.currencyBoards.Count)
			{
				if (this.currencyBoards[this.iterator].IsNotNull())
				{
					this.currencyBoards[this.iterator].UpdateCurrencyBoard(this.checkedDaily, this.gotMyDaily, this.currencyBalance, this.secondsUntilTomorrow);
				}
				this.iterator++;
			}
		}

		// Token: 0x06006753 RID: 26451 RVA: 0x00214799 File Offset: 0x00212999
		public void AddCurrencyBoard(CurrencyBoard newCurrencyBoard)
		{
			if (this.currencyBoards.Contains(newCurrencyBoard))
			{
				return;
			}
			this.currencyBoards.Add(newCurrencyBoard);
			newCurrencyBoard.UpdateCurrencyBoard(this.checkedDaily, this.gotMyDaily, this.currencyBalance, this.secondsUntilTomorrow);
		}

		// Token: 0x06006754 RID: 26452 RVA: 0x002147D4 File Offset: 0x002129D4
		public void RemoveCurrencyBoard(CurrencyBoard currencyBoardToRemove)
		{
			this.currencyBoards.Remove(currencyBoardToRemove);
		}

		// Token: 0x06006755 RID: 26453 RVA: 0x002147E3 File Offset: 0x002129E3
		public void GetCurrencyBalance()
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				this.currencyBalance = result.VirtualCurrency[this.currencyName];
				this.UpdateCurrencyBoards();
				Action onGetCurrency = this.OnGetCurrency;
				if (onGetCurrency == null)
				{
					return;
				}
				onGetCurrency();
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
		}

		// Token: 0x06006756 RID: 26454 RVA: 0x0021481C File Offset: 0x00212A1C
		public string GetItemDisplayName(CosmeticsController.CosmeticItem item)
		{
			if (item.overrideDisplayName != null && item.overrideDisplayName != "")
			{
				return item.overrideDisplayName;
			}
			return item.displayName;
		}

		// Token: 0x06006757 RID: 26455 RVA: 0x00214845 File Offset: 0x00212A45
		public void UpdateMyCosmetics()
		{
			if (GorillaServer.Instance != null)
			{
				GorillaServer.Instance.UpdateUserCosmetics();
			}
		}

		// Token: 0x06006758 RID: 26456 RVA: 0x00214864 File Offset: 0x00212A64
		private void AlreadyOwnAllBundleButtons()
		{
			EarlyAccessButton[] array = this.earlyAccessButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AlreadyOwn();
			}
		}

		// Token: 0x06006759 RID: 26457 RVA: 0x0021488E File Offset: 0x00212A8E
		public void CheckCosmeticsSharedGroup()
		{
			this.updateCosmeticsRetries++;
			if (this.updateCosmeticsRetries < this.maxUpdateCosmeticsRetries)
			{
				base.StartCoroutine(this.WaitForNextCosmeticsAttempt());
			}
		}

		// Token: 0x0600675A RID: 26458 RVA: 0x002148B9 File Offset: 0x00212AB9
		private IEnumerator WaitForNextCosmeticsAttempt()
		{
			int num = (int)Mathf.Pow(3f, (float)(this.updateCosmeticsRetries + 1));
			yield return new WaitForSecondsRealtime((float)num);
			this.ConfirmIndividualCosmeticsSharedGroup(this.latestInventory);
			yield break;
		}

		// Token: 0x0600675B RID: 26459 RVA: 0x002148C8 File Offset: 0x00212AC8
		private void ConfirmIndividualCosmeticsSharedGroup(GetUserInventoryResult inventory)
		{
			this.latestInventory = inventory;
			if (PhotonNetwork.LocalPlayer.UserId == null)
			{
				base.StartCoroutine(this.WaitForNextCosmeticsAttempt());
				return;
			}
			PlayFabClientAPI.GetSharedGroupData(new PlayFab.ClientModels.GetSharedGroupDataRequest
			{
				Keys = new List<string>
				{
					"Inventory"
				},
				SharedGroupId = PhotonNetwork.LocalPlayer.UserId + "Inventory"
			}, delegate(GetSharedGroupDataResult result)
			{
				bool flag = true;
				foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
				{
					foreach (ItemInstance itemInstance in inventory.Inventory)
					{
						if (itemInstance.CatalogVersion == CosmeticsController.instance.catalog && !keyValuePair.Value.Value.Contains(itemInstance.ItemId))
						{
							flag = false;
							break;
						}
					}
				}
				if (!flag || result.Data.Count == 0)
				{
					this.UpdateMyCosmetics();
					return;
				}
				this.updateCosmeticsRetries = 0;
			}, delegate(PlayFabError error)
			{
				this.ReauthOrBan(error);
				this.CheckCosmeticsSharedGroup();
			}, null, null);
		}

		// Token: 0x0600675C RID: 26460 RVA: 0x00214964 File Offset: 0x00212B64
		public void ReauthOrBan(PlayFabError error)
		{
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				return;
			}
			if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				Application.Quit();
				PhotonNetwork.Disconnect();
				Object.DestroyImmediate(PhotonNetworkController.Instance);
				Object.DestroyImmediate(GTPlayer.Instance);
				GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
				for (int i = 0; i < array.Length; i++)
				{
					Object.Destroy(array[i]);
				}
			}
		}

		// Token: 0x0600675D RID: 26461 RVA: 0x002149D8 File Offset: 0x00212BD8
		public void ProcessExternalUnlock(string itemID, bool autoEquip, bool isLeftHand)
		{
			this.UnlockItem(itemID, false);
			GorillaTagger.Instance.offlineVRRig.AddCosmetic(itemID);
			this.UpdateMyCosmetics();
			if (autoEquip)
			{
				CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(itemID);
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.external_item_claim, itemFromDict);
				List<CosmeticsController.CosmeticSlots> list = CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Get();
				if (list.Capacity < 16)
				{
					list.Capacity = 16;
				}
				this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true, list);
				foreach (CosmeticsController.CosmeticSlots cosmeticSlots in list)
				{
					this.tryOnSet.items[(int)cosmeticSlots] = this.nullItem;
				}
				CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Release(list);
				this.UpdateShoppingCart();
				this.UpdateWornCosmetics(true);
				Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
				if (onCosmeticsUpdated == null)
				{
					return;
				}
				onCosmeticsUpdated();
			}
		}

		// Token: 0x0600675E RID: 26462 RVA: 0x00214AC0 File Offset: 0x00212CC0
		public void AddTempUnlockToWardrobe(string cosmeticID)
		{
			int num = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => cosmeticID == x.itemName);
			if (num < 0)
			{
				return;
			}
			switch (this.allCosmetics[num].itemCategory)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				this.ModifyUnlockList(this.unlockedHats, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Badge:
				this.ModifyUnlockList(this.unlockedBadges, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Face:
				this.ModifyUnlockList(this.unlockedFaces, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Paw:
				if (!this.allCosmetics[num].isThrowable)
				{
					this.ModifyUnlockList(this.unlockedPaws, num, false);
					return;
				}
				this.ModifyUnlockList(this.unlockedThrowables, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Chest:
				this.ModifyUnlockList(this.unlockedChests, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Fur:
				this.ModifyUnlockList(this.unlockedFurs, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Shirt:
				this.ModifyUnlockList(this.unlockedShirts, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Back:
				this.ModifyUnlockList(this.unlockedBacks, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Arms:
				this.ModifyUnlockList(this.unlockedArms, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Pants:
				this.ModifyUnlockList(this.unlockedPants, num, false);
				return;
			case CosmeticsController.CosmeticCategory.TagEffect:
				this.ModifyUnlockList(this.unlockedTagFX, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Count:
				break;
			case CosmeticsController.CosmeticCategory.Set:
				foreach (string cosmeticID2 in this.allCosmetics[num].bundledItems)
				{
					this.AddTempUnlockToWardrobe(cosmeticID2);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x0600675F RID: 26463 RVA: 0x00214C40 File Offset: 0x00212E40
		public void RemoveTempUnlockFromWardrobe(string cosmeticID)
		{
			int num = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => cosmeticID == x.itemName);
			if (num < 0)
			{
				return;
			}
			switch (this.allCosmetics[num].itemCategory)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				this.ModifyUnlockList(this.unlockedHats, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Badge:
				this.ModifyUnlockList(this.unlockedBadges, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Face:
				this.ModifyUnlockList(this.unlockedFaces, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Paw:
				if (!this.allCosmetics[num].isThrowable)
				{
					this.ModifyUnlockList(this.unlockedPaws, num, true);
					return;
				}
				this.ModifyUnlockList(this.unlockedThrowables, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Chest:
				this.ModifyUnlockList(this.unlockedChests, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Fur:
				this.ModifyUnlockList(this.unlockedFurs, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Shirt:
				this.ModifyUnlockList(this.unlockedShirts, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Back:
				this.ModifyUnlockList(this.unlockedBacks, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Arms:
				this.ModifyUnlockList(this.unlockedArms, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Pants:
				this.ModifyUnlockList(this.unlockedPants, num, true);
				return;
			case CosmeticsController.CosmeticCategory.TagEffect:
				this.ModifyUnlockList(this.unlockedTagFX, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Count:
				break;
			case CosmeticsController.CosmeticCategory.Set:
				foreach (string cosmeticID2 in this.allCosmetics[num].bundledItems)
				{
					this.RemoveTempUnlockFromWardrobe(cosmeticID2);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006760 RID: 26464 RVA: 0x00214DBF File Offset: 0x00212FBF
		public bool BuildValidationCheck()
		{
			if (this.m_earlyAccessSupporterPackCosmeticSO == null)
			{
				Debug.LogError("m_earlyAccessSupporterPackCosmeticSO is empty, everything will break!");
				return false;
			}
			return true;
		}

		// Token: 0x06006761 RID: 26465 RVA: 0x00214DDC File Offset: 0x00212FDC
		public void SetHideCosmeticsFromRemotePlayers(bool hideCosmetics)
		{
			if (hideCosmetics == this.isHidingCosmeticsFromRemotePlayers)
			{
				return;
			}
			this.isHidingCosmeticsFromRemotePlayers = hideCosmetics;
			GorillaTagger.Instance.offlineVRRig.reliableState.SetIsDirty();
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06006762 RID: 26466 RVA: 0x00214E0C File Offset: 0x0021300C
		public bool ValidatePackedItems(int[] packed)
		{
			if (packed == null)
			{
				return false;
			}
			if (packed.Length == 0)
			{
				return true;
			}
			int num = 0;
			int num2 = packed[0];
			for (int i = 0; i < 16; i++)
			{
				if ((num2 & 1 << i) != 0)
				{
					num++;
				}
			}
			return packed.Length == num + 1;
		}

		// Token: 0x06006763 RID: 26467 RVA: 0x00214E50 File Offset: 0x00213050
		public static int[] PackCollectableItems(List<CosmeticsController.CosmeticItem> items)
		{
			if (items == null || items.Count == 0)
			{
				return Array.Empty<int>();
			}
			int[] array = new int[items.Count];
			for (int i = 0; i < items.Count; i++)
			{
				string itemName = items[i].itemName;
				array[i] = (int)(itemName[0] - 'A' + '\u001a' * (itemName[1] - 'A' + '\u001a' * (itemName[2] - 'A' + '\u001a' * (itemName[3] - 'A' + '\u001a' * (itemName[4] - 'A')))));
			}
			return array;
		}

		// Token: 0x06006764 RID: 26468 RVA: 0x00214EE0 File Offset: 0x002130E0
		public CosmeticsController.CosmeticItem[] UnpackCollectableItems(int[] packed)
		{
			if (packed == null || packed.Length == 0)
			{
				return Array.Empty<CosmeticsController.CosmeticItem>();
			}
			char[] array = new char[]
			{
				'\0',
				'\0',
				'\0',
				'\0',
				'\0',
				'.'
			};
			CosmeticsController.CosmeticItem[] array2 = new CosmeticsController.CosmeticItem[packed.Length];
			for (int i = 0; i < packed.Length; i++)
			{
				int num = packed[i];
				array[0] = (char)(65 + num % 26);
				array[1] = (char)(65 + num / 26 % 26);
				array[2] = (char)(65 + num / 676 % 26);
				array[3] = (char)(65 + num / 17576 % 26);
				array[4] = (char)(65 + num / 456976 % 26);
				array2[i] = this.GetItemFromDict(new string(array));
			}
			return array2;
		}

		// Token: 0x06006765 RID: 26469 RVA: 0x00214F81 File Offset: 0x00213181
		public void SetValidatedCreatorCode(string memberCode, string groupCode, string terminalId)
		{
			this.validatedCreatorCode = new CosmeticsController.ValidatedCreatorCode();
			this.validatedCreatorCode.memberCode = memberCode;
			this.validatedCreatorCode.groupId = groupCode;
			this.validatedCreatorCode.terminalId = terminalId;
		}

		// Token: 0x170009BE RID: 2494
		// (get) Token: 0x06006766 RID: 26470 RVA: 0x00214FB2 File Offset: 0x002131B2
		public static int SelectedOutfit
		{
			get
			{
				return CosmeticsController.selectedOutfit;
			}
		}

		// Token: 0x06006767 RID: 26471 RVA: 0x00214FB9 File Offset: 0x002131B9
		public static bool CanScrollOutfits()
		{
			return CosmeticsController.loadedSavedOutfits && !CosmeticsController.saveOutfitInProgress;
		}

		// Token: 0x06006768 RID: 26472 RVA: 0x00214FCC File Offset: 0x002131CC
		public void PressWardrobeScrollOutfit(bool forward)
		{
			int num = CosmeticsController.selectedOutfit;
			if (forward)
			{
				num = (num + 1) % CosmeticsController.maxOutfits;
			}
			else
			{
				num--;
				if (num < 0)
				{
					num = CosmeticsController.maxOutfits - 1;
				}
			}
			this.LoadSavedOutfit(num);
		}

		// Token: 0x06006769 RID: 26473 RVA: 0x00215008 File Offset: 0x00213208
		public void LoadSavedOutfit(int newOutfitIndex)
		{
			if (!CosmeticsController.CanScrollOutfits() || newOutfitIndex == CosmeticsController.selectedOutfit || newOutfitIndex < 0 || newOutfitIndex >= CosmeticsController.maxOutfits)
			{
				return;
			}
			this.savedOutfits[CosmeticsController.selectedOutfit].CopyItems(this.currentWornSet);
			this.savedColors[CosmeticsController.selectedOutfit] = new Vector3(VRRig.LocalRig.playerColor.r, VRRig.LocalRig.playerColor.g, VRRig.LocalRig.playerColor.b);
			this.SaveOutfitsToMothership();
			CosmeticsController.selectedOutfit = newOutfitIndex;
			PlayerPrefs.SetInt(this.outfitSystemConfig.selectedOutfitPref, CosmeticsController.selectedOutfit);
			PlayerPrefs.Save();
			CosmeticsController.CosmeticSet outfit = this.savedOutfits[CosmeticsController.selectedOutfit];
			bool flag = true;
			for (int i = 0; i < 16; i++)
			{
				CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots)i;
				if ((cosmeticSlots != CosmeticsController.CosmeticSlots.ArmLeft && cosmeticSlots != CosmeticsController.CosmeticSlots.ArmRight) || flag)
				{
					this.ApplyNewItem(outfit, i);
				}
			}
			this.UpdateMonkeColor(this.savedColors[CosmeticsController.selectedOutfit], true);
			this.SaveCurrentItemPreferences();
			this.UpdateShoppingCart();
			this.UpdateWornCosmetics(true, true);
			this.UpdateWardrobeModelsAndButtons();
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated();
		}

		// Token: 0x0600676A RID: 26474 RVA: 0x0021512C File Offset: 0x0021332C
		private void ApplyNewItem(CosmeticsController.CosmeticSet outfit, int i)
		{
			this.currentWornSet.items[i] = outfit.items[i];
			if (!outfit.items[i].isNullItem)
			{
				this.tryOnSet.items[i] = this.nullItem;
			}
		}

		// Token: 0x0600676B RID: 26475 RVA: 0x00215180 File Offset: 0x00213380
		private void LoadSavedOutfits()
		{
			CosmeticsController.<LoadSavedOutfits>d__284 <LoadSavedOutfits>d__;
			<LoadSavedOutfits>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<LoadSavedOutfits>d__.<>4__this = this;
			<LoadSavedOutfits>d__.<>1__state = -1;
			<LoadSavedOutfits>d__.<>t__builder.Start<CosmeticsController.<LoadSavedOutfits>d__284>(ref <LoadSavedOutfits>d__);
		}

		// Token: 0x0600676C RID: 26476 RVA: 0x002151B8 File Offset: 0x002133B8
		private void GetSavedOutfitsSuccess(MothershipUserData response)
		{
			if (response != null && response.value != null && response.value.Length > 0)
			{
				try
				{
					byte[] bytes = Convert.FromBase64String(response.value);
					this.outfitStringMothership = Encoding.UTF8.GetString(bytes);
					this.StringToOutfits(this.outfitStringMothership);
					goto IL_6E;
				}
				catch (Exception ex)
				{
					GTDev.LogError<string>("CosmeticsController GetSavedOutfitsSuccess error decoding " + ex.Message, null);
					this.ClearOutfits();
					goto IL_6E;
				}
			}
			this.ClearOutfits();
			IL_6E:
			this.GetSavedOutfitsComplete();
		}

		// Token: 0x0600676D RID: 26477 RVA: 0x0021524C File Offset: 0x0021344C
		private void GetSavedOutfitsFail(MothershipError error, int status)
		{
			GTDev.LogError<string>(string.Format("CosmeticsController GetSavedOutfitsFail {0} {1}", status, error.Message), null);
			this.ClearOutfits();
			this.GetSavedOutfitsComplete();
		}

		// Token: 0x0600676E RID: 26478 RVA: 0x00215278 File Offset: 0x00213478
		private void GetSavedOutfitsComplete()
		{
			int num = PlayerPrefs.GetInt(this.outfitSystemConfig.selectedOutfitPref, 0);
			if (num < 0 || num >= CosmeticsController.maxOutfits)
			{
				num = 0;
			}
			else
			{
				CosmeticsController.CosmeticSet cosmeticSet = new CosmeticsController.CosmeticSet();
				cosmeticSet.LoadFromPlayerPreferences(this);
				if (cosmeticSet.HasAnyItems())
				{
					this.savedOutfits[num].CopyItems(cosmeticSet);
				}
				float @float = PlayerPrefs.GetFloat("redValue", 0f);
				float float2 = PlayerPrefs.GetFloat("greenValue", 0f);
				float float3 = PlayerPrefs.GetFloat("blueValue", 0f);
				if (@float > 0f || float2 > 0f || float3 > 0f)
				{
					this.savedColors[num] = new Vector3(@float, float2, float3);
				}
			}
			CosmeticsController.selectedOutfit = num;
			this.currentWornSet.CopyItems(this.savedOutfits[CosmeticsController.selectedOutfit]);
			this.UpdateMonkeColor(this.savedColors[CosmeticsController.selectedOutfit], true);
			CosmeticsController.loadedSavedOutfits = true;
			CosmeticsController.loadOutfitsInProgress = false;
			Action onOutfitsUpdated = this.OnOutfitsUpdated;
			if (onOutfitsUpdated == null)
			{
				return;
			}
			onOutfitsUpdated();
		}

		// Token: 0x0600676F RID: 26479 RVA: 0x0021537C File Offset: 0x0021357C
		private void UpdateMonkeColor(Vector3 col, bool saveToPrefs)
		{
			float num = Mathf.Clamp(col.x, 0f, 1f);
			float num2 = Mathf.Clamp(col.y, 0f, 1f);
			float num3 = Mathf.Clamp(col.z, 0f, 1f);
			GorillaTagger.Instance.UpdateColor(num, num2, num3);
			GorillaComputer.instance.UpdateColor(num, num2, num3);
			if (CosmeticsController.OnPlayerColorSet != null)
			{
				CosmeticsController.OnPlayerColorSet(num, num2, num3);
			}
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
				{
					num,
					num2,
					num3
				});
			}
			if (saveToPrefs)
			{
				PlayerPrefs.SetFloat("redValue", num);
				PlayerPrefs.SetFloat("greenValue", num2);
				PlayerPrefs.SetFloat("blueValue", num3);
				PlayerPrefs.Save();
			}
		}

		// Token: 0x06006770 RID: 26480 RVA: 0x00215468 File Offset: 0x00213668
		private void SaveOutfitsToMothership()
		{
			if (!CosmeticsController.loadedSavedOutfits || CosmeticsController.saveOutfitInProgress)
			{
				return;
			}
			string mothershipKey = this.outfitSystemConfig.mothershipKey;
			this.outfitStringPendingSave = this.OutfitsToString();
			if (this.outfitStringPendingSave.Equals(this.outfitStringMothership))
			{
				return;
			}
			CosmeticsController.saveOutfitInProgress = true;
			if (!MothershipClientApiUnity.SetUserDataValue(mothershipKey, this.outfitStringPendingSave, new Action<SetUserDataResponse>(this.SaveOutfitsToMothershipSuccess), new Action<MothershipError, int>(this.SaveOutfitsToMothershipFail), ""))
			{
				GTDev.LogError<string>("CosmeticsController SaveOutfitToMothership SetUserDataValue failed", null);
				CosmeticsController.saveOutfitInProgress = false;
			}
		}

		// Token: 0x06006771 RID: 26481 RVA: 0x002154F2 File Offset: 0x002136F2
		private void SaveOutfitsToMothershipSuccess(SetUserDataResponse response)
		{
			this.outfitStringMothership = this.outfitStringPendingSave;
			CosmeticsController.saveOutfitInProgress = false;
			Action onOutfitsUpdated = this.OnOutfitsUpdated;
			if (onOutfitsUpdated != null)
			{
				onOutfitsUpdated();
			}
			response.Dispose();
		}

		// Token: 0x06006772 RID: 26482 RVA: 0x0021551D File Offset: 0x0021371D
		private void SaveOutfitsToMothershipFail(MothershipError error, int status)
		{
			GTDev.LogError<string>(string.Format("CosmeticsController SaveOutfitsToMothershipFail {0} ", status) + error.Message, null);
			CosmeticsController.saveOutfitInProgress = false;
		}

		// Token: 0x06006773 RID: 26483 RVA: 0x00215548 File Offset: 0x00213748
		private string OutfitsToString()
		{
			if (!CosmeticsController.loadedSavedOutfits)
			{
				return string.Empty;
			}
			CosmeticsController.outfitDataTemp = new CosmeticsController.OutfitData();
			this.sb.Clear();
			for (int i = 0; i < this.savedOutfits.Length; i++)
			{
				CosmeticsController.outfitDataTemp.Clear();
				CosmeticsController.CosmeticSet cosmeticSet = this.savedOutfits[i];
				for (int j = 0; j < cosmeticSet.items.Length; j++)
				{
					CosmeticsController.CosmeticItem cosmeticItem = cosmeticSet.items[j];
					string item = (cosmeticItem.isNullItem || string.IsNullOrEmpty(cosmeticItem.displayName)) ? "null" : cosmeticItem.displayName;
					CosmeticsController.outfitDataTemp.itemIDs.Add(item);
				}
				if (VRRig.LocalRig != null)
				{
					CosmeticsController.outfitDataTemp.color = this.savedColors[i];
				}
				this.sb.Append(JsonUtility.ToJson(CosmeticsController.outfitDataTemp));
				if (i < this.savedOutfits.Length - 1)
				{
					this.sb.Append(this.outfitSystemConfig.outfitSeparator);
				}
			}
			return this.sb.ToString();
		}

		// Token: 0x06006774 RID: 26484 RVA: 0x00215664 File Offset: 0x00213864
		private void ClearOutfits()
		{
			for (int i = 0; i < this.savedOutfits.Length; i++)
			{
				this.savedOutfits[i] = new CosmeticsController.CosmeticSet();
				this.savedOutfits[i].ClearSet(this.nullItem);
				this.savedColors[i] = CosmeticsController.defaultColor;
			}
		}

		// Token: 0x06006775 RID: 26485 RVA: 0x002156B8 File Offset: 0x002138B8
		private void StringToOutfits(string response)
		{
			if (response.IsNullOrEmpty())
			{
				this.ClearOutfits();
				return;
			}
			try
			{
				string[] array = response.Split(this.outfitSystemConfig.outfitSeparator, StringSplitOptions.None);
				for (int i = 0; i < CosmeticsController.maxOutfits; i++)
				{
					this.savedOutfits[i] = new CosmeticsController.CosmeticSet();
					if (i >= array.Length)
					{
						this.savedOutfits[i].ClearSet(this.nullItem);
						this.savedColors[i] = CosmeticsController.defaultColor;
					}
					else
					{
						string text = array[i];
						if (text.IsNullOrEmpty())
						{
							this.savedOutfits[i].ClearSet(this.nullItem);
							this.savedColors[i] = CosmeticsController.defaultColor;
						}
						else
						{
							Vector3 vector;
							this.savedOutfits[i].ParseSetFromString(this, text, out vector);
							this.savedColors[i] = vector;
						}
					}
				}
			}
			catch (Exception ex)
			{
				GTDev.LogError<string>("CosmeticsController StringToOutfit Error parsing " + ex.Message, null);
				this.ClearOutfits();
			}
		}

		// Token: 0x04007644 RID: 30276
		[FormerlySerializedAs("v2AllCosmeticsInfoAssetRef")]
		[FormerlySerializedAs("newSysAllCosmeticsAssetRef")]
		[SerializeField]
		public GTAssetRef<AllCosmeticsArraySO> v2_allCosmeticsInfoAssetRef;

		// Token: 0x04007646 RID: 30278
		private readonly Dictionary<string, CosmeticInfoV2> _allCosmeticsDictV2 = new Dictionary<string, CosmeticInfoV2>();

		// Token: 0x04007647 RID: 30279
		public Action V2_allCosmeticsInfoAssetRef_OnPostLoad;

		// Token: 0x0400764B RID: 30283
		public const int maximumTransferrableItems = 5;

		// Token: 0x0400764C RID: 30284
		[OnEnterPlay_SetNull]
		public static volatile CosmeticsController instance;

		// Token: 0x0400764E RID: 30286
		public static Action<string, string> PushTerminalMessage;

		// Token: 0x0400764F RID: 30287
		public Action V2_OnGetCosmeticsPlayFabCatalogData_PostSuccess;

		// Token: 0x04007650 RID: 30288
		public Action OnGetCurrency;

		// Token: 0x04007651 RID: 30289
		private string purchaseLocation;

		// Token: 0x04007652 RID: 30290
		[FormerlySerializedAs("allCosmetics")]
		[SerializeField]
		private List<CosmeticsController.CosmeticItem> _allCosmetics;

		// Token: 0x04007654 RID: 30292
		public Dictionary<string, CosmeticsController.CosmeticItem> _allCosmeticsDict = new Dictionary<string, CosmeticsController.CosmeticItem>(2048);

		// Token: 0x04007656 RID: 30294
		public Dictionary<string, string> _allCosmeticsItemIDsfromDisplayNamesDict = new Dictionary<string, string>(2048);

		// Token: 0x04007657 RID: 30295
		public CosmeticsController.CosmeticItem nullItem;

		// Token: 0x04007658 RID: 30296
		public string catalog;

		// Token: 0x04007659 RID: 30297
		private string[] tempStringArray;

		// Token: 0x0400765A RID: 30298
		private CosmeticsController.CosmeticItem tempItem;

		// Token: 0x0400765B RID: 30299
		private VRRigAnchorOverrides anchorOverrides;

		// Token: 0x0400765C RID: 30300
		public List<CatalogItem> catalogItems;

		// Token: 0x0400765D RID: 30301
		public bool tryTwice;

		// Token: 0x0400765E RID: 30302
		public CustomMapCosmeticsData customMapCosmeticsData;

		// Token: 0x0400765F RID: 30303
		[NonSerialized]
		public CosmeticsController.CosmeticSet tryOnSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04007660 RID: 30304
		public int numFittingRoomButtons = 12;

		// Token: 0x04007661 RID: 30305
		public List<FittingRoom> fittingRooms = new List<FittingRoom>();

		// Token: 0x04007662 RID: 30306
		public CosmeticStand[] cosmeticStands;

		// Token: 0x04007663 RID: 30307
		public List<CosmeticsController.CosmeticItem> currentCart = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x04007664 RID: 30308
		public CosmeticsController.PurchaseItemStages currentPurchaseItemStage;

		// Token: 0x04007665 RID: 30309
		public List<ItemCheckout> itemCheckouts = new List<ItemCheckout>();

		// Token: 0x04007666 RID: 30310
		public CosmeticsController.CosmeticItem itemToBuy;

		// Token: 0x04007667 RID: 30311
		private bool foundCosmetic;

		// Token: 0x04007668 RID: 30312
		private int attempts;

		// Token: 0x04007669 RID: 30313
		private string finalLine;

		// Token: 0x0400766A RID: 30314
		private string leftCheckoutPurchaseButtonString;

		// Token: 0x0400766B RID: 30315
		private string rightCheckoutPurchaseButtonString;

		// Token: 0x0400766C RID: 30316
		private bool leftCheckoutPurchaseButtonOn;

		// Token: 0x0400766D RID: 30317
		private bool rightCheckoutPurchaseButtonOn;

		// Token: 0x0400766E RID: 30318
		private bool isLastHandTouchedLeft;

		// Token: 0x0400766F RID: 30319
		private CosmeticsController.CosmeticSet cachedSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04007671 RID: 30321
		public readonly List<WardrobeInstance> wardrobes = new List<WardrobeInstance>();

		// Token: 0x04007672 RID: 30322
		public List<CosmeticsController.CosmeticItem> unlockedCosmetics = new List<CosmeticsController.CosmeticItem>(2048);

		// Token: 0x04007673 RID: 30323
		public List<CosmeticsController.CosmeticItem> unlockedHats = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007674 RID: 30324
		public List<CosmeticsController.CosmeticItem> unlockedFaces = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007675 RID: 30325
		public List<CosmeticsController.CosmeticItem> unlockedBadges = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007676 RID: 30326
		public List<CosmeticsController.CosmeticItem> unlockedPaws = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007677 RID: 30327
		public List<CosmeticsController.CosmeticItem> unlockedChests = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007678 RID: 30328
		public List<CosmeticsController.CosmeticItem> unlockedFurs = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007679 RID: 30329
		public List<CosmeticsController.CosmeticItem> unlockedShirts = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x0400767A RID: 30330
		public List<CosmeticsController.CosmeticItem> unlockedPants = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x0400767B RID: 30331
		public List<CosmeticsController.CosmeticItem> unlockedBacks = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x0400767C RID: 30332
		public List<CosmeticsController.CosmeticItem> unlockedArms = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x0400767D RID: 30333
		public List<CosmeticsController.CosmeticItem> unlockedTagFX = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x0400767E RID: 30334
		public List<CosmeticsController.CosmeticItem> unlockedThrowables = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x0400767F RID: 30335
		public int[] cosmeticsPages = new int[11];

		// Token: 0x04007680 RID: 30336
		private List<CosmeticsController.CosmeticItem>[] itemLists = new List<CosmeticsController.CosmeticItem>[11];

		// Token: 0x04007681 RID: 30337
		private int wardrobeType;

		// Token: 0x04007682 RID: 30338
		[NonSerialized]
		public CosmeticsController.CosmeticSet currentWornSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04007683 RID: 30339
		[NonSerialized]
		public CosmeticsController.CosmeticSet tempUnlockedSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04007684 RID: 30340
		[NonSerialized]
		public CosmeticsController.CosmeticSet activeMergedSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04007685 RID: 30341
		[NonSerialized]
		public CosmeticsController.CosmeticItem tryOnCollectableItem;

		// Token: 0x04007686 RID: 30342
		public string concatStringCosmeticsAllowed = "";

		// Token: 0x04007687 RID: 30343
		public Action OnCosmeticsUpdated;

		// Token: 0x04007688 RID: 30344
		[NonSerialized]
		public Dictionary<string, List<CosmeticsController.CosmeticItem>> collectablesByParentID = new Dictionary<string, List<CosmeticsController.CosmeticItem>>();

		// Token: 0x04007689 RID: 30345
		private static readonly List<CosmeticCollectionDisplay> scratchDisplayList = new List<CosmeticCollectionDisplay>();

		// Token: 0x0400768A RID: 30346
		private static int[] cycleStatesArray = Array.Empty<int>();

		// Token: 0x0400768B RID: 30347
		public int currencyBalance;

		// Token: 0x0400768C RID: 30348
		public string currencyName;

		// Token: 0x0400768D RID: 30349
		public List<CurrencyBoard> currencyBoards;

		// Token: 0x0400768E RID: 30350
		public string itemToPurchase;

		// Token: 0x0400768F RID: 30351
		public bool buyingBundle;

		// Token: 0x04007690 RID: 30352
		public bool confirmedDidntPlayInBeta;

		// Token: 0x04007691 RID: 30353
		public bool playedInBeta;

		// Token: 0x04007692 RID: 30354
		public bool gotMyDaily;

		// Token: 0x04007693 RID: 30355
		public bool checkedDaily;

		// Token: 0x04007694 RID: 30356
		public string currentPurchaseID;

		// Token: 0x04007695 RID: 30357
		public bool hasPrice;

		// Token: 0x04007696 RID: 30358
		private int searchIndex;

		// Token: 0x04007697 RID: 30359
		private int iterator;

		// Token: 0x04007698 RID: 30360
		private CosmeticsController.CosmeticItem cosmeticItemVar;

		// Token: 0x04007699 RID: 30361
		[SerializeField]
		private CosmeticSO m_earlyAccessSupporterPackCosmeticSO;

		// Token: 0x0400769A RID: 30362
		public EarlyAccessButton[] earlyAccessButtons;

		// Token: 0x0400769B RID: 30363
		private BundleList bundleList = new BundleList();

		// Token: 0x0400769C RID: 30364
		public string BundleSkuName = "2024_i_lava_you_pack";

		// Token: 0x0400769D RID: 30365
		public string BundlePlayfabItemName = "LSABG.";

		// Token: 0x0400769E RID: 30366
		public int BundleShinyRocks = 10000;

		// Token: 0x0400769F RID: 30367
		public DateTime currentTime;

		// Token: 0x040076A0 RID: 30368
		public string lastDailyLogin;

		// Token: 0x040076A1 RID: 30369
		public UserDataRecord userDataRecord;

		// Token: 0x040076A2 RID: 30370
		public int secondsUntilTomorrow;

		// Token: 0x040076A3 RID: 30371
		public float secondsToWaitToCheckDaily = 10f;

		// Token: 0x040076A4 RID: 30372
		private int updateCosmeticsRetries;

		// Token: 0x040076A5 RID: 30373
		private int maxUpdateCosmeticsRetries;

		// Token: 0x040076A6 RID: 30374
		private GetUserInventoryResult latestInventory;

		// Token: 0x040076A7 RID: 30375
		private string returnString;

		// Token: 0x040076A8 RID: 30376
		private bool checkoutCartButtonPressedWithLeft;

		// Token: 0x040076A9 RID: 30377
		private CosmeticsController.ValidatedCreatorCode validatedCreatorCode;

		// Token: 0x040076AA RID: 30378
		private Callback<MicroTxnAuthorizationResponse_t> _steamMicroTransactionAuthorizationResponse;

		// Token: 0x040076AB RID: 30379
		private static readonly List<CosmeticsController.CosmeticSlots> _g_default_outAppliedSlotsList_for_applyCosmeticItemToSet = new List<CosmeticsController.CosmeticSlots>(16);

		// Token: 0x040076AC RID: 30380
		[SerializeField]
		private CosmeticOutfitSystemConfig outfitSystemConfig;

		// Token: 0x040076AD RID: 30381
		private CosmeticsController.CosmeticSet[] savedOutfits;

		// Token: 0x040076AE RID: 30382
		private Vector3[] savedColors;

		// Token: 0x040076AF RID: 30383
		private static CosmeticsController.OutfitData outfitDataTemp;

		// Token: 0x040076B0 RID: 30384
		private string outfitStringMothership = string.Empty;

		// Token: 0x040076B1 RID: 30385
		private string outfitStringPendingSave = string.Empty;

		// Token: 0x040076B2 RID: 30386
		private static bool saveOutfitInProgress = false;

		// Token: 0x040076B3 RID: 30387
		private static bool loadOutfitsInProgress = false;

		// Token: 0x040076B4 RID: 30388
		private static bool loadedSavedOutfits = false;

		// Token: 0x040076B5 RID: 30389
		private static int selectedOutfit = 0;

		// Token: 0x040076B6 RID: 30390
		private static int maxOutfits = -1;

		// Token: 0x040076B7 RID: 30391
		private static readonly Vector3 defaultColor = new Vector3(0f, 0f, 0f);

		// Token: 0x040076B8 RID: 30392
		public Action OnOutfitsUpdated;

		// Token: 0x040076B9 RID: 30393
		public static Action<float, float, float> OnPlayerColorSet;

		// Token: 0x040076BA RID: 30394
		private StringBuilder sb = new StringBuilder(256);

		// Token: 0x0200101B RID: 4123
		public enum PurchaseItemStages
		{
			// Token: 0x040076BC RID: 30396
			Start,
			// Token: 0x040076BD RID: 30397
			CheckoutButtonPressed,
			// Token: 0x040076BE RID: 30398
			ItemSelected,
			// Token: 0x040076BF RID: 30399
			ItemOwned,
			// Token: 0x040076C0 RID: 30400
			FinalPurchaseAcknowledgement,
			// Token: 0x040076C1 RID: 30401
			Buying,
			// Token: 0x040076C2 RID: 30402
			Success,
			// Token: 0x040076C3 RID: 30403
			Failure
		}

		// Token: 0x0200101C RID: 4124
		public enum CosmeticCategory
		{
			// Token: 0x040076C5 RID: 30405
			None,
			// Token: 0x040076C6 RID: 30406
			Hat,
			// Token: 0x040076C7 RID: 30407
			Badge,
			// Token: 0x040076C8 RID: 30408
			Face,
			// Token: 0x040076C9 RID: 30409
			Paw,
			// Token: 0x040076CA RID: 30410
			Chest,
			// Token: 0x040076CB RID: 30411
			Fur,
			// Token: 0x040076CC RID: 30412
			Shirt,
			// Token: 0x040076CD RID: 30413
			Back,
			// Token: 0x040076CE RID: 30414
			Arms,
			// Token: 0x040076CF RID: 30415
			Pants,
			// Token: 0x040076D0 RID: 30416
			TagEffect,
			// Token: 0x040076D1 RID: 30417
			Count,
			// Token: 0x040076D2 RID: 30418
			Set,
			// Token: 0x040076D3 RID: 30419
			Collectable
		}

		// Token: 0x0200101D RID: 4125
		public enum CosmeticSlots
		{
			// Token: 0x040076D5 RID: 30421
			Hat,
			// Token: 0x040076D6 RID: 30422
			Badge,
			// Token: 0x040076D7 RID: 30423
			Face,
			// Token: 0x040076D8 RID: 30424
			ArmLeft,
			// Token: 0x040076D9 RID: 30425
			ArmRight,
			// Token: 0x040076DA RID: 30426
			BackLeft,
			// Token: 0x040076DB RID: 30427
			BackRight,
			// Token: 0x040076DC RID: 30428
			HandLeft,
			// Token: 0x040076DD RID: 30429
			HandRight,
			// Token: 0x040076DE RID: 30430
			Chest,
			// Token: 0x040076DF RID: 30431
			Fur,
			// Token: 0x040076E0 RID: 30432
			Shirt,
			// Token: 0x040076E1 RID: 30433
			Pants,
			// Token: 0x040076E2 RID: 30434
			Back,
			// Token: 0x040076E3 RID: 30435
			Arms,
			// Token: 0x040076E4 RID: 30436
			TagEffect,
			// Token: 0x040076E5 RID: 30437
			Count
		}

		// Token: 0x0200101E RID: 4126
		[Serializable]
		public class CosmeticSet
		{
			// Token: 0x140000BB RID: 187
			// (add) Token: 0x06006785 RID: 26501 RVA: 0x00215E30 File Offset: 0x00214030
			// (remove) Token: 0x06006786 RID: 26502 RVA: 0x00215E68 File Offset: 0x00214068
			public event CosmeticsController.CosmeticSet.OnSetActivatedHandler onSetActivatedEvent;

			// Token: 0x06006787 RID: 26503 RVA: 0x00215E9D File Offset: 0x0021409D
			protected void OnSetActivated(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, NetPlayer netPlayer)
			{
				if (this.onSetActivatedEvent != null)
				{
					this.onSetActivatedEvent(prevSet, currentSet, netPlayer);
				}
			}

			// Token: 0x170009BF RID: 2495
			// (get) Token: 0x06006788 RID: 26504 RVA: 0x00215EB8 File Offset: 0x002140B8
			public static CosmeticsController.CosmeticSet EmptySet
			{
				get
				{
					if (CosmeticsController.CosmeticSet._emptySet == null)
					{
						string[] array = new string[16];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = "NOTHING";
						}
						CosmeticsController.CosmeticSet._emptySet = new CosmeticsController.CosmeticSet(array, CosmeticsController.instance);
					}
					return CosmeticsController.CosmeticSet._emptySet;
				}
			}

			// Token: 0x06006789 RID: 26505 RVA: 0x00215F01 File Offset: 0x00214101
			public CosmeticSet()
			{
				this.items = new CosmeticsController.CosmeticItem[16];
			}

			// Token: 0x0600678A RID: 26506 RVA: 0x00215F24 File Offset: 0x00214124
			public CosmeticSet(string[] itemNames, CosmeticsController controller)
			{
				this.items = new CosmeticsController.CosmeticItem[16];
				for (int i = 0; i < itemNames.Length; i++)
				{
					string displayName = itemNames[i];
					string itemNameFromDisplayName = controller.GetItemNameFromDisplayName(displayName);
					this.items[i] = controller.GetItemFromDict(itemNameFromDisplayName);
				}
			}

			// Token: 0x0600678B RID: 26507 RVA: 0x00215F80 File Offset: 0x00214180
			public CosmeticSet(int[] itemNamesPacked, CosmeticsController controller)
			{
				this.items = new CosmeticsController.CosmeticItem[16];
				int num = (itemNamesPacked.Length != 0) ? itemNamesPacked[0] : 0;
				int num2 = 1;
				for (int i = 0; i < this.items.Length; i++)
				{
					if ((num & 1 << i) != 0)
					{
						int num3 = itemNamesPacked[num2];
						if (num3 == -55)
						{
							this.items[i] = controller.GetItemFromDict("Slingshot");
						}
						else
						{
							CosmeticsController.CosmeticSet.nameScratchSpace[0] = (char)(65 + num3 % 26);
							CosmeticsController.CosmeticSet.nameScratchSpace[1] = (char)(65 + num3 / 26 % 26);
							CosmeticsController.CosmeticSet.nameScratchSpace[2] = (char)(65 + num3 / 676 % 26);
							CosmeticsController.CosmeticSet.nameScratchSpace[3] = (char)(65 + num3 / 17576 % 26);
							CosmeticsController.CosmeticSet.nameScratchSpace[4] = (char)(65 + num3 / 456976 % 26);
							CosmeticsController.CosmeticSet.nameScratchSpace[5] = '.';
							this.items[i] = controller.GetItemFromDict(new string(CosmeticsController.CosmeticSet.nameScratchSpace));
						}
						num2++;
					}
					else
					{
						this.items[i] = controller.GetItemFromDict("null");
					}
				}
			}

			// Token: 0x0600678C RID: 26508 RVA: 0x002160A8 File Offset: 0x002142A8
			public void CopyItems(CosmeticsController.CosmeticSet other)
			{
				for (int i = 0; i < this.items.Length; i++)
				{
					this.items[i] = other.items[i];
				}
			}

			// Token: 0x0600678D RID: 26509 RVA: 0x002160E0 File Offset: 0x002142E0
			public void CopyItemsIntoEmpty(CosmeticsController.CosmeticSet other)
			{
				for (int i = 0; i < this.items.Length; i++)
				{
					if (this.items[i].isNullItem)
					{
						this.items[i] = other.items[i];
					}
				}
			}

			// Token: 0x0600678E RID: 26510 RVA: 0x0021612C File Offset: 0x0021432C
			public void MergeSets(CosmeticsController.CosmeticSet tryOn, CosmeticsController.CosmeticSet current)
			{
				for (int i = 0; i < 16; i++)
				{
					if (tryOn == null)
					{
						this.items[i] = current.items[i];
					}
					else
					{
						this.items[i] = (tryOn.items[i].isNullItem ? current.items[i] : tryOn.items[i]);
					}
				}
			}

			// Token: 0x0600678F RID: 26511 RVA: 0x0021619C File Offset: 0x0021439C
			public void MergeInSets(CosmeticsController.CosmeticSet playerPref, CosmeticsController.CosmeticSet tempOverrideSet, Predicate<string> predicate)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticItem[] array = tempOverrideSet.items;
					bool flag = predicate(tempOverrideSet.items[i].itemName);
					this.items[i] = (flag ? tempOverrideSet.items[i] : playerPref.items[i]);
				}
			}

			// Token: 0x06006790 RID: 26512 RVA: 0x00216208 File Offset: 0x00214408
			public void ClearSet(CosmeticsController.CosmeticItem nullItem)
			{
				for (int i = 0; i < 16; i++)
				{
					this.items[i] = nullItem;
				}
			}

			// Token: 0x06006791 RID: 26513 RVA: 0x00216230 File Offset: 0x00214430
			public bool IsActive(string name)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06006792 RID: 26514 RVA: 0x00216268 File Offset: 0x00214468
			public bool HasItemOfCategory(CosmeticsController.CosmeticCategory category)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].itemCategory == category)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06006793 RID: 26515 RVA: 0x002162B0 File Offset: 0x002144B0
			public bool HasItem(string name)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06006794 RID: 26516 RVA: 0x002162FC File Offset: 0x002144FC
			public bool HasAnyItems()
			{
				if (this.items == null || this.items.Length < 1)
				{
					return false;
				}
				for (int i = 0; i < this.items.Length; i++)
				{
					if (!this.items[i].isNullItem)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06006795 RID: 26517 RVA: 0x00216347 File Offset: 0x00214547
			public static bool IsSlotLeftHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.HandLeft;
			}

			// Token: 0x06006796 RID: 26518 RVA: 0x00216357 File Offset: 0x00214557
			public static bool IsSlotRightHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmRight || slot == CosmeticsController.CosmeticSlots.BackRight || slot == CosmeticsController.CosmeticSlots.HandRight;
			}

			// Token: 0x06006797 RID: 26519 RVA: 0x00216367 File Offset: 0x00214567
			public static bool IsHoldable(CosmeticsController.CosmeticItem item)
			{
				return item.isHoldable;
			}

			// Token: 0x06006798 RID: 26520 RVA: 0x00216370 File Offset: 0x00214570
			public static CosmeticsController.CosmeticSlots OppositeSlot(CosmeticsController.CosmeticSlots slot)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Hat:
					return CosmeticsController.CosmeticSlots.Hat;
				case CosmeticsController.CosmeticSlots.Badge:
					return CosmeticsController.CosmeticSlots.Badge;
				case CosmeticsController.CosmeticSlots.Face:
					return CosmeticsController.CosmeticSlots.Face;
				case CosmeticsController.CosmeticSlots.ArmLeft:
					return CosmeticsController.CosmeticSlots.ArmRight;
				case CosmeticsController.CosmeticSlots.ArmRight:
					return CosmeticsController.CosmeticSlots.ArmLeft;
				case CosmeticsController.CosmeticSlots.BackLeft:
					return CosmeticsController.CosmeticSlots.BackRight;
				case CosmeticsController.CosmeticSlots.BackRight:
					return CosmeticsController.CosmeticSlots.BackLeft;
				case CosmeticsController.CosmeticSlots.HandLeft:
					return CosmeticsController.CosmeticSlots.HandRight;
				case CosmeticsController.CosmeticSlots.HandRight:
					return CosmeticsController.CosmeticSlots.HandLeft;
				case CosmeticsController.CosmeticSlots.Chest:
					return CosmeticsController.CosmeticSlots.Chest;
				case CosmeticsController.CosmeticSlots.Fur:
					return CosmeticsController.CosmeticSlots.Fur;
				case CosmeticsController.CosmeticSlots.Shirt:
					return CosmeticsController.CosmeticSlots.Shirt;
				case CosmeticsController.CosmeticSlots.Pants:
					return CosmeticsController.CosmeticSlots.Pants;
				case CosmeticsController.CosmeticSlots.Back:
					return CosmeticsController.CosmeticSlots.Back;
				case CosmeticsController.CosmeticSlots.Arms:
					return CosmeticsController.CosmeticSlots.Arms;
				case CosmeticsController.CosmeticSlots.TagEffect:
					return CosmeticsController.CosmeticSlots.TagEffect;
				default:
					return CosmeticsController.CosmeticSlots.Count;
				}
			}

			// Token: 0x06006799 RID: 26521 RVA: 0x002163EE File Offset: 0x002145EE
			public static string SlotPlayerPreferenceName(CosmeticsController.CosmeticSlots slot)
			{
				return "slot_" + slot.ToString();
			}

			// Token: 0x0600679A RID: 26522 RVA: 0x00216408 File Offset: 0x00214608
			private void ActivateCosmetic(CosmeticsController.CosmeticSet prevSet, VRRig rig, int slotIndex, CosmeticItemRegistry cosmeticsObjectRegistry, BodyDockPositions bDock)
			{
				CosmeticsController.CosmeticItem cosmeticItem = prevSet.items[slotIndex];
				string itemNameFromDisplayName = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem.displayName);
				CosmeticsController.CosmeticItem cosmeticItem2 = this.items[slotIndex];
				string itemNameFromDisplayName2 = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem2.displayName);
				BodyDockPositions.DropPositions dropPositions = CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)slotIndex);
				if (cosmeticItem2.itemCategory != CosmeticsController.CosmeticCategory.None && !CosmeticsController.CompareCategoryToSavedCosmeticSlots(cosmeticItem2.itemCategory, (CosmeticsController.CosmeticSlots)slotIndex))
				{
					return;
				}
				if (cosmeticItem2.isHoldable && dropPositions == BodyDockPositions.DropPositions.None)
				{
					return;
				}
				if (!(itemNameFromDisplayName == itemNameFromDisplayName2))
				{
					if (!cosmeticItem.isNullItem)
					{
						if (cosmeticItem.isHoldable)
						{
							bDock.TransferrableItemDisableAtPosition(dropPositions);
						}
						CosmeticItemInstance cosmeticItemInstance = cosmeticsObjectRegistry.Cosmetic(cosmeticItem.displayName);
						if (cosmeticItemInstance != null)
						{
							cosmeticItemInstance.DisableItem((CosmeticsController.CosmeticSlots)slotIndex);
						}
					}
					if (!cosmeticItem2.isNullItem)
					{
						if (cosmeticItem2.isHoldable)
						{
							bDock.TransferrableItemEnableAtPosition(cosmeticItem2.displayName, dropPositions);
						}
						CosmeticItemInstance cosmeticItemInstance2 = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
						if (rig.IsItemAllowed(itemNameFromDisplayName2) && cosmeticItemInstance2 != null)
						{
							cosmeticItemInstance2.EnableItem((CosmeticsController.CosmeticSlots)slotIndex, rig);
							if (rig.isLocal && (slotIndex == 0 || slotIndex == 2))
							{
								PlayerPrefFlags.TouchIf(PlayerPrefFlags.Flag.SHOW_1P_COSMETICS, false);
							}
							CosmeticsController.CosmeticSet.PopulateCollectionDisplay(cosmeticItemInstance2, cosmeticItem2, rig);
						}
					}
					return;
				}
				if (cosmeticItem2.isNullItem)
				{
					return;
				}
				CosmeticItemInstance cosmeticItemInstance3 = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
				if (cosmeticItemInstance3 != null)
				{
					if (!rig.IsItemAllowed(itemNameFromDisplayName2))
					{
						cosmeticItemInstance3.DisableItem((CosmeticsController.CosmeticSlots)slotIndex);
						return;
					}
					if (cosmeticItem2.isHoldable)
					{
						bDock.TransferrableItemEnableAtPosition(cosmeticItem2.displayName, dropPositions);
					}
					cosmeticItemInstance3.EnableItem((CosmeticsController.CosmeticSlots)slotIndex, rig);
					CosmeticsController.CosmeticSet.PopulateCollectionDisplay(cosmeticItemInstance3, cosmeticItem2, rig);
				}
			}

			// Token: 0x0600679B RID: 26523 RVA: 0x00216584 File Offset: 0x00214784
			public void ActivateCosmetics(CosmeticsController.CosmeticSet prevSet, VRRig rig, BodyDockPositions bDock, CosmeticItemRegistry cosmeticsObjectRegistry)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					this.ActivateCosmetic(prevSet, rig, i, cosmeticsObjectRegistry, bDock);
				}
				this.OnSetActivated(prevSet, this, rig.creator);
			}

			// Token: 0x0600679C RID: 26524 RVA: 0x002165BC File Offset: 0x002147BC
			private static void PopulateCollectionDisplay(CosmeticItemInstance instance, CosmeticsController.CosmeticItem parentItem, VRRig rig)
			{
				if (parentItem.collectionSlotCount <= 0 || !CosmeticsController.hasInstance)
				{
					return;
				}
				CosmeticsController instance2 = CosmeticsController.instance;
				CosmeticInfoV2 cosmeticInfoV;
				if (!instance2.TryGetCosmeticInfoV2(parentItem.itemName, out cosmeticInfoV))
				{
					return;
				}
				if (cosmeticInfoV.collectionSlots == null || cosmeticInfoV.collectionSlots.Length == 0)
				{
					return;
				}
				GameObject gameObject = null;
				foreach (GameObject gameObject2 in instance.objects)
				{
					if (gameObject2 != null)
					{
						gameObject = gameObject2;
						break;
					}
				}
				if (gameObject == null)
				{
					foreach (GameObject gameObject3 in instance.holdableObjects)
					{
						if (gameObject3 != null)
						{
							gameObject = gameObject3;
							break;
						}
					}
				}
				if (gameObject == null)
				{
					return;
				}
				CosmeticCollectionDisplay cosmeticCollectionDisplay;
				if (!gameObject.TryGetComponent<CosmeticCollectionDisplay>(out cosmeticCollectionDisplay))
				{
					cosmeticCollectionDisplay = gameObject.AddComponent<CosmeticCollectionDisplay>();
				}
				List<CosmeticsController.CosmeticItem> list = new List<CosmeticsController.CosmeticItem>();
				List<CosmeticsController.CosmeticItem> collection;
				if (rig.isLocal)
				{
					for (int i = 0; i < instance2.unlockedCosmetics.Count; i++)
					{
						if (instance2.unlockedCosmetics[i].collectionParentPlayFabID == parentItem.itemName)
						{
							list.Add(instance2.unlockedCosmetics[i]);
						}
					}
					CosmeticsController.CosmeticItem tryOnC = instance2.tryOnCollectableItem;
					if (!tryOnC.isNullItem && tryOnC.collectionParentPlayFabID == parentItem.itemName && !list.Exists((CosmeticsController.CosmeticItem x) => x.itemName == tryOnC.itemName) && VRRig.LocalRig != null && VRRig.LocalRig.inTryOnRoom)
					{
						list.Add(tryOnC);
					}
					if (cosmeticCollectionDisplay.ContentMatches(list))
					{
						return;
					}
				}
				else if (instance2.collectablesByParentID.TryGetValue(parentItem.itemName, out collection))
				{
					list.AddRange(collection);
				}
				cosmeticCollectionDisplay.Populate(list, cosmeticInfoV, gameObject.transform);
				CosmeticCollectionDisplay.Register(rig.GetInstanceID(), parentItem.itemName, cosmeticCollectionDisplay);
				int activeIndex;
				if (!rig.isLocal && rig.remoteCycleStates.TryGetValue(parentItem.itemName, out activeIndex))
				{
					cosmeticCollectionDisplay.SetActiveIndex(activeIndex);
				}
			}

			// Token: 0x0600679D RID: 26525 RVA: 0x00216810 File Offset: 0x00214A10
			public void DeactivateAllCosmetcs(BodyDockPositions bDock, CosmeticsController.CosmeticItem nullItem, CosmeticItemRegistry cosmeticObjectRegistry)
			{
				bDock.DisableAllTransferableItems();
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticItem cosmeticItem = this.items[i];
					if (!cosmeticItem.isNullItem)
					{
						CosmeticsController.CosmeticSlots cosmeticSlot = (CosmeticsController.CosmeticSlots)i;
						CosmeticItemInstance cosmeticItemInstance = cosmeticObjectRegistry.Cosmetic(cosmeticItem.displayName);
						if (cosmeticItemInstance != null)
						{
							cosmeticItemInstance.DisableItem(cosmeticSlot);
						}
						this.items[i] = nullItem;
					}
				}
			}

			// Token: 0x0600679E RID: 26526 RVA: 0x00216870 File Offset: 0x00214A70
			public void LoadFromPlayerPreferences(CosmeticsController controller)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticSlots slot = (CosmeticsController.CosmeticSlots)i;
					string @string = PlayerPrefs.GetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), "NOTHING");
					if (@string == "null" || @string == "NOTHING")
					{
						this.items[i] = controller.nullItem;
					}
					else
					{
						CosmeticsController.CosmeticItem item = controller.GetItemFromDict(@string);
						if (item.isNullItem)
						{
							Debug.Log("LoadFromPlayerPreferences: Could not find item stored in player prefs: \"" + @string + "\"");
							this.items[i] = controller.nullItem;
						}
						else if (item.itemName == "Slingshot")
						{
							this.items[i] = controller.nullItem;
							PlayerPrefs.SetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), "NOTHING");
						}
						else if (!CosmeticsController.CompareCategoryToSavedCosmeticSlots(item.itemCategory, slot))
						{
							this.items[i] = controller.nullItem;
						}
						else if (controller.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.itemName == x.itemName) >= 0)
						{
							this.items[i] = item;
						}
						else
						{
							this.items[i] = controller.nullItem;
						}
					}
				}
			}

			// Token: 0x0600679F RID: 26527 RVA: 0x002169CC File Offset: 0x00214BCC
			public void ParseSetFromString(CosmeticsController controller, string setString, out Vector3 color)
			{
				color = CosmeticsController.defaultColor;
				if (setString.IsNullOrEmpty())
				{
					this.ClearSet(controller.nullItem);
					GTDev.LogError<string>("CosmeticsController ParseSetFromString: null string", null);
					return;
				}
				int num = 16;
				CosmeticsController.OutfitData outfitData = new CosmeticsController.OutfitData();
				try
				{
					outfitData = JsonUtility.FromJson<CosmeticsController.OutfitData>(setString);
					color = outfitData.color;
				}
				catch (Exception)
				{
					char separator = ',';
					if (controller.outfitSystemConfig != null)
					{
						separator = controller.outfitSystemConfig.itemSeparator;
					}
					string[] array = setString.Split(separator, num, StringSplitOptions.None);
					if (array == null || array.Length > num)
					{
						this.ClearSet(controller.nullItem);
						GTDev.LogError<string>(string.Format("CosmeticsController ParseSetFromString: wrong number of slots {0} {1}", array.Length, setString), null);
						return;
					}
					outfitData.Clear();
					outfitData.itemIDs = new List<string>(array);
				}
				try
				{
					for (int i = 0; i < num; i++)
					{
						CosmeticsController.CosmeticSlots slot = (CosmeticsController.CosmeticSlots)i;
						string text = (i < outfitData.itemIDs.Count) ? outfitData.itemIDs[i] : "null";
						if (text.IsNullOrEmpty() || text == "null" || text == "NOTHING")
						{
							this.items[i] = controller.nullItem;
						}
						else
						{
							CosmeticsController.CosmeticItem item = controller.GetItemFromDict(text);
							if (item.isNullItem)
							{
								GTDev.Log<string>("CosmeticsController ParseSetFromString: Could not find item stored in player prefs: \"" + text + "\"", null);
								this.items[i] = controller.nullItem;
							}
							else if (!CosmeticsController.CompareCategoryToSavedCosmeticSlots(item.itemCategory, slot))
							{
								this.items[i] = controller.nullItem;
							}
							else if (controller.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.itemName == x.itemName) >= 0)
							{
								this.items[i] = item;
							}
							else
							{
								this.items[i] = controller.nullItem;
							}
						}
					}
				}
				catch (Exception ex)
				{
					this.ClearSet(controller.nullItem);
					GTDev.LogError<string>("CosmeticsController: Issue parsing saved outfit string: " + ex.Message, null);
				}
			}

			// Token: 0x060067A0 RID: 26528 RVA: 0x00216C2C File Offset: 0x00214E2C
			public string[] ToDisplayNameArray()
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					this.returnArray[i] = (string.IsNullOrEmpty(this.items[i].displayName) ? "null" : this.items[i].displayName);
				}
				return this.returnArray;
			}

			// Token: 0x060067A1 RID: 26529 RVA: 0x00216C88 File Offset: 0x00214E88
			public int[] ToPackedIDArray()
			{
				int num = 0;
				int num2 = 0;
				int num3 = 16;
				for (int i = 0; i < num3; i++)
				{
					if (!this.items[i].isNullItem && (this.items[i].itemName.Length == 6 || this.items[i].itemName == "Slingshot"))
					{
						num |= 1 << i;
						num2++;
					}
				}
				if (num == 0)
				{
					return CosmeticsController.CosmeticSet.intArrays[0];
				}
				int[] array = CosmeticsController.CosmeticSet.intArrays[num2 + 1];
				array[0] = num;
				int num4 = 1;
				for (int j = 0; j < num3; j++)
				{
					if ((num & 1 << j) != 0)
					{
						string itemName = this.items[j].itemName;
						if (itemName == "Slingshot")
						{
							array[num4] = -55;
						}
						else
						{
							array[num4] = (int)(itemName[0] - 'A' + '\u001a' * (itemName[1] - 'A' + '\u001a' * (itemName[2] - 'A' + '\u001a' * (itemName[3] - 'A' + '\u001a' * (itemName[4] - 'A')))));
						}
						num4++;
					}
				}
				return array;
			}

			// Token: 0x060067A2 RID: 26530 RVA: 0x00216DC0 File Offset: 0x00214FC0
			public string[] HoldableDisplayNames(bool leftHoldables)
			{
				int num = 16;
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].isHoldable && this.items[i].isHoldable && this.items[i].itemCategory != CosmeticsController.CosmeticCategory.Chest)
					{
						if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i)))
						{
							num2++;
						}
						else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i)))
						{
							num2++;
						}
					}
				}
				if (num2 == 0)
				{
					return null;
				}
				int num3 = 0;
				string[] array = new string[num2];
				for (int j = 0; j < num; j++)
				{
					if (this.items[j].isHoldable)
					{
						if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)j)))
						{
							array[num3] = this.items[j].displayName;
							num3++;
						}
						else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)j)))
						{
							array[num3] = this.items[j].displayName;
							num3++;
						}
					}
				}
				return array;
			}

			// Token: 0x060067A3 RID: 26531 RVA: 0x00216ED4 File Offset: 0x002150D4
			public bool[] ToOnRightSideArray()
			{
				int num = 16;
				bool[] array = new bool[num];
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].isHoldable && this.items[i].itemCategory != CosmeticsController.CosmeticCategory.Chest)
					{
						array[i] = !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i));
					}
					else
					{
						array[i] = false;
					}
				}
				return array;
			}

			// Token: 0x040076E6 RID: 30438
			public const int k_fakePackedSlingshotID = -55;

			// Token: 0x040076E7 RID: 30439
			public CosmeticsController.CosmeticItem[] items;

			// Token: 0x040076E9 RID: 30441
			public string[] returnArray = new string[16];

			// Token: 0x040076EA RID: 30442
			private static int[][] intArrays = new int[][]
			{
				new int[0],
				new int[1],
				new int[2],
				new int[3],
				new int[4],
				new int[5],
				new int[6],
				new int[7],
				new int[8],
				new int[9],
				new int[10],
				new int[11],
				new int[12],
				new int[13],
				new int[14],
				new int[15],
				new int[16],
				new int[17],
				new int[18],
				new int[19],
				new int[20],
				new int[21]
			};

			// Token: 0x040076EB RID: 30443
			private static CosmeticsController.CosmeticSet _emptySet;

			// Token: 0x040076EC RID: 30444
			private static char[] nameScratchSpace = new char[6];

			// Token: 0x0200101F RID: 4127
			// (Invoke) Token: 0x060067A6 RID: 26534
			public delegate void OnSetActivatedHandler(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, NetPlayer netPlayer);
		}

		// Token: 0x02001023 RID: 4131
		[Serializable]
		public struct CosmeticItem
		{
			// Token: 0x040076F0 RID: 30448
			[Tooltip("Should match the spreadsheet item name.")]
			public string itemName;

			// Token: 0x040076F1 RID: 30449
			[Tooltip("Determines what wardrobe section the item will show up in.")]
			public CosmeticsController.CosmeticCategory itemCategory;

			// Token: 0x040076F2 RID: 30450
			[Tooltip("If this is a holdable item.")]
			public bool isHoldable;

			// Token: 0x040076F3 RID: 30451
			[Tooltip("If this is a throwable item and hidden on the wardrobe.")]
			public bool isThrowable;

			// Token: 0x040076F4 RID: 30452
			[Tooltip("Icon shown in the store menus & hunt watch.")]
			public Sprite itemPicture;

			// Token: 0x040076F5 RID: 30453
			public string displayName;

			// Token: 0x040076F6 RID: 30454
			public string itemPictureResourceString;

			// Token: 0x040076F7 RID: 30455
			[Tooltip("The name shown on the store checkout screen.")]
			public string overrideDisplayName;

			// Token: 0x040076F8 RID: 30456
			[DebugReadout]
			[NonSerialized]
			public int cost;

			// Token: 0x040076F9 RID: 30457
			[DebugReadout]
			[NonSerialized]
			public string[] bundledItems;

			// Token: 0x040076FA RID: 30458
			[DebugReadout]
			[NonSerialized]
			public bool canTryOn;

			// Token: 0x040076FB RID: 30459
			[Tooltip("Set to true if the item takes up both left and right wearable hand slots at the same time. Used for things like mittens/gloves.")]
			public bool bothHandsHoldable;

			// Token: 0x040076FC RID: 30460
			public bool bLoadsFromResources;

			// Token: 0x040076FD RID: 30461
			public bool bUsesMeshAtlas;

			// Token: 0x040076FE RID: 30462
			public Vector3 rotationOffset;

			// Token: 0x040076FF RID: 30463
			public Vector3 positionOffset;

			// Token: 0x04007700 RID: 30464
			public string meshAtlasResourceString;

			// Token: 0x04007701 RID: 30465
			public string meshResourceString;

			// Token: 0x04007702 RID: 30466
			public string materialResourceString;

			// Token: 0x04007703 RID: 30467
			[HideInInspector]
			public bool isNullItem;

			// Token: 0x04007704 RID: 30468
			[NonSerialized]
			public string collectionParentPlayFabID;

			// Token: 0x04007705 RID: 30469
			[NonSerialized]
			public int collectionSlotCount;

			// Token: 0x04007706 RID: 30470
			[NonSerialized]
			public bool collectionIsCycling;

			// Token: 0x04007707 RID: 30471
			[NonSerialized]
			public bool collectionUsesIndexTargeting;

			// Token: 0x04007708 RID: 30472
			[NonSerialized]
			public int collectionTargetSlotIndex;

			// Token: 0x04007709 RID: 30473
			[NonSerialized]
			public string appliedCosmeticPlayFabID;
		}

		// Token: 0x02001024 RID: 4132
		[Serializable]
		public class IAPRequestBody
		{
			// Token: 0x0400770A RID: 30474
			public string sku;

			// Token: 0x0400770B RID: 30475
			public string mothershipId;

			// Token: 0x0400770C RID: 30476
			public string mothershipToken;

			// Token: 0x0400770D RID: 30477
			public string mothershipEnvId;

			// Token: 0x0400770E RID: 30478
			public string mothershipDeploymentId;

			// Token: 0x0400770F RID: 30479
			public Dictionary<string, string> customTags;
		}

		// Token: 0x02001025 RID: 4133
		private class ValidatedCreatorCode
		{
			// Token: 0x170009C0 RID: 2496
			// (get) Token: 0x060067B0 RID: 26544 RVA: 0x00217084 File Offset: 0x00215284
			// (set) Token: 0x060067B1 RID: 26545 RVA: 0x0021708C File Offset: 0x0021528C
			public string terminalId { get; set; }

			// Token: 0x170009C1 RID: 2497
			// (get) Token: 0x060067B2 RID: 26546 RVA: 0x00217095 File Offset: 0x00215295
			// (set) Token: 0x060067B3 RID: 26547 RVA: 0x0021709D File Offset: 0x0021529D
			public string memberCode { get; set; }

			// Token: 0x170009C2 RID: 2498
			// (get) Token: 0x060067B4 RID: 26548 RVA: 0x002170A6 File Offset: 0x002152A6
			// (set) Token: 0x060067B5 RID: 26549 RVA: 0x002170AE File Offset: 0x002152AE
			public string groupId { get; set; }
		}

		// Token: 0x02001026 RID: 4134
		public enum EWearingCosmeticSet
		{
			// Token: 0x04007714 RID: 30484
			NotASet,
			// Token: 0x04007715 RID: 30485
			NotWearing,
			// Token: 0x04007716 RID: 30486
			Partial,
			// Token: 0x04007717 RID: 30487
			Complete
		}

		// Token: 0x02001027 RID: 4135
		public class OutfitData
		{
			// Token: 0x060067B7 RID: 26551 RVA: 0x002170B7 File Offset: 0x002152B7
			public OutfitData()
			{
				this.version = 1;
				this.itemIDs = new List<string>(16);
				this.color = CosmeticsController.defaultColor;
			}

			// Token: 0x060067B8 RID: 26552 RVA: 0x002170DE File Offset: 0x002152DE
			public void Clear()
			{
				this.itemIDs.Clear();
				this.color = CosmeticsController.defaultColor;
			}

			// Token: 0x04007718 RID: 30488
			public const int OUTFIT_DATA_VERSION = 1;

			// Token: 0x04007719 RID: 30489
			public int version;

			// Token: 0x0400771A RID: 30490
			public List<string> itemIDs;

			// Token: 0x0400771B RID: 30491
			public Vector3 color;
		}
	}
}
