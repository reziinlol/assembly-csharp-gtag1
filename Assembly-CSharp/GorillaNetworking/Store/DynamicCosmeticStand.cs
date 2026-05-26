using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GorillaNetworking.Store
{
	// Token: 0x0200109F RID: 4255
	public class DynamicCosmeticStand : MonoBehaviour, iFlagForBaking
	{
		// Token: 0x06006AB6 RID: 27318 RVA: 0x00227960 File Offset: 0x00225B60
		public virtual void SetForBaking()
		{
			this.GorillaHeadModel.SetActive(true);
			this.GorillaTorsoModel.SetActive(true);
			this.GorillaTorsoPostModel.SetActive(true);
			this.GorillaMannequinModel.SetActive(true);
			this.JeweleryBoxModel.SetActive(true);
			this.root.SetActive(true);
			this.DisplayHeadModel.gameObject.SetActive(false);
		}

		// Token: 0x06006AB7 RID: 27319 RVA: 0x002279C8 File Offset: 0x00225BC8
		public void OnEnable()
		{
			TMP_Text tmp_Text = this.addToCartTextTMP;
			if (tmp_Text != null)
			{
				tmp_Text.gameObject.SetActive(true);
			}
			TMP_Text tmp_Text2 = this.slotPriceTextTMP;
			if (tmp_Text2 != null)
			{
				tmp_Text2.gameObject.SetActive(true);
			}
			this.AddStandToStoreController();
			if (CosmeticsController.hasInstance)
			{
				CosmeticsController instance = CosmeticsController.instance;
				instance.OnCosmeticsUpdated = (Action)Delegate.Combine(instance.OnCosmeticsUpdated, new Action(this.RefreshPurchaseGate));
			}
		}

		// Token: 0x06006AB8 RID: 27320 RVA: 0x00227A38 File Offset: 0x00225C38
		public void OnDisable()
		{
			TMP_Text tmp_Text = this.addToCartTextTMP;
			if (tmp_Text != null)
			{
				tmp_Text.gameObject.SetActive(false);
			}
			TMP_Text tmp_Text2 = this.slotPriceTextTMP;
			if (tmp_Text2 != null)
			{
				tmp_Text2.gameObject.SetActive(false);
			}
			this.RemoveStandFromStoreController();
			if (CosmeticsController.hasInstance)
			{
				CosmeticsController instance = CosmeticsController.instance;
				instance.OnCosmeticsUpdated = (Action)Delegate.Remove(instance.OnCosmeticsUpdated, new Action(this.RefreshPurchaseGate));
			}
		}

		// Token: 0x06006AB9 RID: 27321 RVA: 0x00227AA8 File Offset: 0x00225CA8
		public void AddStandToStoreController()
		{
			if (StoreController.instance != null && StoreController.instance.cosmeticsInitialized)
			{
				this._AddStandToStoreController();
				return;
			}
			base.StartCoroutine(this.ConnectToStoreController());
		}

		// Token: 0x06006ABA RID: 27322 RVA: 0x00227ADB File Offset: 0x00225CDB
		private IEnumerator ConnectToStoreController()
		{
			int i = 0;
			while (i < 30 && !(StoreController.instance != null))
			{
				if (i == 29)
				{
					Object.Destroy(this);
					throw new Exception("Could not connect to store controller.");
				}
				yield return null;
				int num = i + 1;
				i = num;
			}
			if (!StoreController.instance.cosmeticsInitialized)
			{
				this.AsyncAddStandToStoreController();
				yield break;
			}
			while (Application.isPlaying && (!CosmeticsController.hasInstance || !CosmeticsController.instance.v2_allCosmeticsInfoAssetRef_isLoaded))
			{
				yield return null;
			}
			this._AddStandToStoreController();
			yield break;
		}

		// Token: 0x06006ABB RID: 27323 RVA: 0x00227AEC File Offset: 0x00225CEC
		public void AsyncAddStandToStoreController()
		{
			DynamicCosmeticStand.<AsyncAddStandToStoreController>d__30 <AsyncAddStandToStoreController>d__;
			<AsyncAddStandToStoreController>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AsyncAddStandToStoreController>d__.<>4__this = this;
			<AsyncAddStandToStoreController>d__.<>1__state = -1;
			<AsyncAddStandToStoreController>d__.<>t__builder.Start<DynamicCosmeticStand.<AsyncAddStandToStoreController>d__30>(ref <AsyncAddStandToStoreController>d__);
		}

		// Token: 0x06006ABC RID: 27324 RVA: 0x00227B23 File Offset: 0x00225D23
		public void _AddStandToStoreController()
		{
			StoreController.instance.AddStandToCosmeticStandsDictionary(this);
			StoreController.instance.AddStandToPlayfabIDDictionary(this);
			if (StoreController.instance.LoadFromTitleData)
			{
				StoreController.instance.InitializeStandFromTitleData(this);
				return;
			}
			this.InitializeCosmetic();
		}

		// Token: 0x06006ABD RID: 27325 RVA: 0x00227B61 File Offset: 0x00225D61
		public void RemoveStandFromStoreController()
		{
			if (StoreController.instance == null || !StoreController.instance.cosmeticsInitialized)
			{
				return;
			}
			StoreController.instance.RemoveStandFromDynamicCosmeticStandsDictionary(this);
			StoreController.instance.RemoveStandFromPlayFabIDDictionary(this);
		}

		// Token: 0x06006ABE RID: 27326 RVA: 0x00227B9B File Offset: 0x00225D9B
		public virtual void SetForGame()
		{
			this.DisplayHeadModel.gameObject.SetActive(true);
			this.SetStandType(this.DisplayHeadModel.bustType);
			this.parentDisplay = base.GetComponentInParent<StoreDisplay>();
			this.parentDepartment = base.GetComponentInParent<StoreDepartment>();
		}

		// Token: 0x17000A0F RID: 2575
		// (get) Token: 0x06006ABF RID: 27327 RVA: 0x00227BD7 File Offset: 0x00225DD7
		// (set) Token: 0x06006AC0 RID: 27328 RVA: 0x00227BDF File Offset: 0x00225DDF
		public string thisCosmeticName
		{
			get
			{
				return this._thisCosmeticName;
			}
			set
			{
				this._thisCosmeticName = value;
			}
		}

		// Token: 0x06006AC1 RID: 27329 RVA: 0x00227BE8 File Offset: 0x00225DE8
		public void InitializeCosmetic()
		{
			this.thisCosmeticItem = CosmeticsController.instance.allCosmetics.Find((CosmeticsController.CosmeticItem x) => this.thisCosmeticName == x.displayName || this.thisCosmeticName == x.overrideDisplayName || this.thisCosmeticName == x.itemName);
			if (this.slotPriceText != null)
			{
				this.slotPriceText.text = this.thisCosmeticItem.itemCategory.ToString().ToUpper() + " " + this.thisCosmeticItem.cost.ToString();
			}
			if (this.slotPriceTextTMP != null)
			{
				this.slotPriceTextTMP.text = this.thisCosmeticItem.itemCategory.ToString().ToUpper() + " " + this.thisCosmeticItem.cost.ToString();
			}
			this.RefreshPurchaseGate();
		}

		// Token: 0x06006AC2 RID: 27330 RVA: 0x00227CBC File Offset: 0x00225EBC
		public void RefreshPurchaseGate()
		{
			if (this.thisCosmeticItem.itemCategory != CosmeticsController.CosmeticCategory.Collectable)
			{
				return;
			}
			CosmeticsController instance = CosmeticsController.instance;
			string collectionParentPlayFabID = this.thisCosmeticItem.collectionParentPlayFabID;
			if (string.IsNullOrEmpty(collectionParentPlayFabID) || !instance.IsOwnedByPlayFabID(collectionParentPlayFabID))
			{
				this.AddToCartButton.gameObject.SetActive(false);
				CosmeticsController.CosmeticItem cosmeticItem;
				string str = instance.allCosmeticsDict.TryGetValue(collectionParentPlayFabID, out cosmeticItem) ? cosmeticItem.overrideDisplayName : collectionParentPlayFabID;
				if (this.slotPriceTextTMP != null)
				{
					this.slotPriceTextTMP.text = "REQUIRES\n" + str;
				}
				return;
			}
			if (!instance.CanPurchaseCollectable(this.thisCosmeticItem.itemName))
			{
				this.AddToCartButton.gameObject.SetActive(false);
				if (this.slotPriceTextTMP != null)
				{
					this.slotPriceTextTMP.text = "SLOTS FULL";
				}
				return;
			}
			this.AddToCartButton.gameObject.SetActive(true);
			if (this.slotPriceTextTMP != null)
			{
				this.slotPriceTextTMP.text = "ADD-ON   " + this.thisCosmeticItem.cost.ToString();
			}
		}

		// Token: 0x06006AC3 RID: 27331 RVA: 0x00227DD8 File Offset: 0x00225FD8
		public void SpawnItemOntoStand(string PlayFabID)
		{
			this.ClearCosmetics();
			if (PlayFabID.IsNullOrEmpty())
			{
				GTDev.LogWarning<string>("ManuallyInitialize: PlayFabID is null or empty for " + this.StandName, null);
				return;
			}
			if (StoreController.instance.IsNotNull() && Application.isPlaying)
			{
				StoreController.instance.RemoveStandFromPlayFabIDDictionary(this);
			}
			this.thisCosmeticName = PlayFabID;
			if (this.thisCosmeticName.Length == 5)
			{
				this.thisCosmeticName += ".";
			}
			if (Application.isPlaying)
			{
				this.DisplayHeadModel.LoadCosmeticPartsV2(this.thisCosmeticName, false);
			}
			else
			{
				this.DisplayHeadModel.LoadCosmeticParts(StoreController.FindCosmeticInAllCosmeticsArraySO(this.thisCosmeticName), false);
			}
			if (StoreController.instance.IsNotNull() && Application.isPlaying)
			{
				StoreController.instance.AddStandToPlayfabIDDictionary(this);
			}
		}

		// Token: 0x06006AC4 RID: 27332 RVA: 0x00227EAB File Offset: 0x002260AB
		public void ClearCosmetics()
		{
			this.thisCosmeticName = "";
			this.DisplayHeadModel.ClearManuallySpawnedCosmeticParts();
			this.DisplayHeadModel.ClearCosmetics();
		}

		// Token: 0x06006AC5 RID: 27333 RVA: 0x00227ED0 File Offset: 0x002260D0
		public void SetStandType(HeadModel_CosmeticStand.BustType newBustType)
		{
			this.DisplayHeadModel.SetStandType(newBustType);
			this.GorillaHeadModel.SetActive(false);
			this.GorillaTorsoModel.SetActive(false);
			this.GorillaTorsoPostModel.SetActive(false);
			this.GorillaMannequinModel.SetActive(false);
			this.GuitarStandModel.SetActive(false);
			this.JeweleryBoxModel.SetActive(false);
			this.AddToCartButton.gameObject.SetActive(true);
			Text text = this.slotPriceText;
			if (text != null)
			{
				text.gameObject.SetActive(true);
			}
			TMP_Text tmp_Text = this.slotPriceTextTMP;
			if (tmp_Text != null)
			{
				tmp_Text.gameObject.SetActive(true);
			}
			Text text2 = this.addToCartText;
			if (text2 != null)
			{
				text2.gameObject.SetActive(true);
			}
			TMP_Text tmp_Text2 = this.addToCartTextTMP;
			if (tmp_Text2 != null)
			{
				tmp_Text2.gameObject.SetActive(true);
			}
			switch (newBustType)
			{
			case HeadModel_CosmeticStand.BustType.Disabled:
			{
				this.ClearCosmetics();
				this.thisCosmeticName = "";
				this.AddToCartButton.gameObject.SetActive(false);
				Text text3 = this.slotPriceText;
				if (text3 != null)
				{
					text3.gameObject.SetActive(false);
				}
				TMP_Text tmp_Text3 = this.slotPriceTextTMP;
				if (tmp_Text3 != null)
				{
					tmp_Text3.gameObject.SetActive(false);
				}
				Text text4 = this.addToCartText;
				if (text4 != null)
				{
					text4.gameObject.SetActive(false);
				}
				TMP_Text tmp_Text4 = this.addToCartTextTMP;
				if (tmp_Text4 != null)
				{
					tmp_Text4.gameObject.SetActive(false);
				}
				this.DisplayHeadModel.transform.localPosition = Vector3.zero;
				this.DisplayHeadModel.transform.localRotation = Quaternion.identity;
				this.root.SetActive(false);
				break;
			}
			case HeadModel_CosmeticStand.BustType.GorillaHead:
				this.root.SetActive(true);
				this.GorillaHeadModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaHeadModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaHeadModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorso:
				this.root.SetActive(true);
				this.GorillaTorsoModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaTorsoModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaTorsoModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
				this.root.SetActive(true);
				this.GorillaTorsoPostModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaTorsoPostModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaTorsoPostModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GorillaMannequin:
				this.root.SetActive(true);
				this.GorillaMannequinModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaMannequinModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaMannequinModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GuitarStand:
				this.root.SetActive(true);
				this.GuitarStandModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GuitarStandMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GuitarStandMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.JewelryBox:
				this.root.SetActive(true);
				this.JeweleryBoxModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.JeweleryBoxMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.JeweleryBoxMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.Table:
				this.root.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.TableMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.TableMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.PinDisplay:
				this.root.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.PinDisplayMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.PinDisplayMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
				this.root.SetActive(true);
				break;
			default:
				this.root.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = Vector3.zero;
				this.DisplayHeadModel.transform.localRotation = Quaternion.identity;
				break;
			}
			this.SpawnItemOntoStand(this.thisCosmeticName);
		}

		// Token: 0x06006AC6 RID: 27334 RVA: 0x002283AC File Offset: 0x002265AC
		public void CopyChildsName()
		{
			foreach (DynamicCosmeticStand dynamicCosmeticStand in base.gameObject.GetComponentsInChildren<DynamicCosmeticStand>(true))
			{
				if (dynamicCosmeticStand != this)
				{
					this.StandName = dynamicCosmeticStand.StandName;
				}
			}
		}

		// Token: 0x06006AC7 RID: 27335 RVA: 0x002283F0 File Offset: 0x002265F0
		public void PressCosmeticStandButton()
		{
			if (!StoreController.instance.StandsByPlayfabID.ContainsKey(this.thisCosmeticName) || CosmeticsController.instance.GetCosmeticSOFromDisplayName(this.thisCosmeticName) == null)
			{
				return;
			}
			this.searchIndex = CosmeticsController.instance.currentCart.IndexOf(this.thisCosmeticItem);
			if (this.searchIndex != -1)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_remove, this.thisCosmeticItem);
				CosmeticsController.instance.currentCart.RemoveAt(this.searchIndex);
				foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.StandsByPlayfabID[this.thisCosmeticItem.itemName])
				{
					dynamicCosmeticStand.AddToCartButton.isOn = false;
					dynamicCosmeticStand.AddToCartButton.UpdateColor();
				}
				for (int i = 0; i < 16; i++)
				{
					if (this.thisCosmeticItem.itemName == CosmeticsController.instance.tryOnSet.items[i].itemName)
					{
						CosmeticsController.instance.tryOnSet.items[i] = CosmeticsController.instance.nullItem;
					}
				}
			}
			else
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_add, this.thisCosmeticItem);
				CosmeticsController.instance.currentCart.Insert(0, this.thisCosmeticItem);
				foreach (DynamicCosmeticStand dynamicCosmeticStand2 in StoreController.instance.StandsByPlayfabID[this.thisCosmeticName])
				{
					dynamicCosmeticStand2.AddToCartButton.isOn = true;
					dynamicCosmeticStand2.AddToCartButton.UpdateColor();
				}
				if (CosmeticsController.instance.currentCart.Count > CosmeticsController.instance.numFittingRoomButtons)
				{
					foreach (DynamicCosmeticStand dynamicCosmeticStand3 in StoreController.instance.StandsByPlayfabID[CosmeticsController.instance.currentCart[CosmeticsController.instance.numFittingRoomButtons].itemName])
					{
						dynamicCosmeticStand3.AddToCartButton.isOn = false;
						dynamicCosmeticStand3.AddToCartButton.UpdateColor();
					}
					CosmeticsController.instance.currentCart.RemoveAt(CosmeticsController.instance.numFittingRoomButtons);
				}
			}
			CosmeticsController.instance.UpdateShoppingCart();
		}

		// Token: 0x06006AC8 RID: 27336 RVA: 0x002286AC File Offset: 0x002268AC
		public void SetStandTypeString(string bustTypeString)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(bustTypeString);
			if (num <= 1590453963U)
			{
				if (num <= 1121133049U)
				{
					if (num != 214514339U)
					{
						if (num == 1121133049U)
						{
							if (bustTypeString == "GuitarStand")
							{
								this.SetStandType(HeadModel_CosmeticStand.BustType.GuitarStand);
								return;
							}
						}
					}
					else if (bustTypeString == "GorillaHead")
					{
						this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaHead);
						return;
					}
				}
				else if (num != 1364530810U)
				{
					if (num != 1520673798U)
					{
						if (num == 1590453963U)
						{
							if (bustTypeString == "GorillaMannequin")
							{
								this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaMannequin);
								return;
							}
						}
					}
					else if (bustTypeString == "JewelryBox")
					{
						this.SetStandType(HeadModel_CosmeticStand.BustType.JewelryBox);
						return;
					}
				}
				else if (bustTypeString == "PinDisplay")
				{
					this.SetStandType(HeadModel_CosmeticStand.BustType.PinDisplay);
					return;
				}
			}
			else if (num <= 2111326094U)
			{
				if (num != 1952506660U)
				{
					if (num == 2111326094U)
					{
						if (bustTypeString == "GorillaTorsoPost")
						{
							this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaTorsoPost);
							return;
						}
					}
				}
				else if (bustTypeString == "GorillaTorso")
				{
					this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaTorso);
					return;
				}
			}
			else if (num != 3217987877U)
			{
				if (num != 3607948159U)
				{
					if (num == 3845287012U)
					{
						if (bustTypeString == "TagEffectDisplay")
						{
							this.SetStandType(HeadModel_CosmeticStand.BustType.TagEffectDisplay);
							return;
						}
					}
				}
				else if (bustTypeString == "Table")
				{
					this.SetStandType(HeadModel_CosmeticStand.BustType.Table);
					return;
				}
			}
			else if (bustTypeString == "Disabled")
			{
				this.SetStandType(HeadModel_CosmeticStand.BustType.Disabled);
				return;
			}
			this.SetStandType(HeadModel_CosmeticStand.BustType.Table);
		}

		// Token: 0x06006AC9 RID: 27337 RVA: 0x0022885A File Offset: 0x00226A5A
		public void UpdateCosmeticsMountPositions()
		{
			this.DisplayHeadModel.UpdateCosmeticsMountPositions(StoreController.FindCosmeticInAllCosmeticsArraySO(this.thisCosmeticName));
		}

		// Token: 0x06006ACA RID: 27338 RVA: 0x00228874 File Offset: 0x00226A74
		public void InitializeForCustomMapCosmeticItem(GTObjectPlaceholder.ECustomMapCosmeticItem cosmeticItemSlot, Scene scene)
		{
			this.StandName = "CustomMapCosmeticItemStand-" + cosmeticItemSlot.ToString();
			this.customMapScene = scene;
			this.ClearCosmetics();
			CustomMapCosmeticItem customMapCosmeticItem;
			if (CosmeticsController.instance.customMapCosmeticsData.TryGetItem(cosmeticItemSlot, out customMapCosmeticItem))
			{
				this.thisCosmeticName = customMapCosmeticItem.playFabID;
				this.SetStandType(customMapCosmeticItem.bustType);
				this.InitializeCosmetic();
			}
		}

		// Token: 0x06006ACB RID: 27339 RVA: 0x002288DF File Offset: 0x00226ADF
		public bool IsFromCustomMapScene(Scene scene)
		{
			return this.customMapScene == scene;
		}

		// Token: 0x04007AE2 RID: 31458
		public HeadModel_CosmeticStand DisplayHeadModel;

		// Token: 0x04007AE3 RID: 31459
		public GorillaPressableButton AddToCartButton;

		// Token: 0x04007AE4 RID: 31460
		[HideInInspector]
		public Text slotPriceText;

		// Token: 0x04007AE5 RID: 31461
		[HideInInspector]
		public Text addToCartText;

		// Token: 0x04007AE6 RID: 31462
		public TMP_Text slotPriceTextTMP;

		// Token: 0x04007AE7 RID: 31463
		public TMP_Text addToCartTextTMP;

		// Token: 0x04007AE8 RID: 31464
		private CosmeticsController.CosmeticItem thisCosmeticItem;

		// Token: 0x04007AE9 RID: 31465
		[FormerlySerializedAs("StandID")]
		public string StandName;

		// Token: 0x04007AEA RID: 31466
		public string _thisCosmeticName = "";

		// Token: 0x04007AEB RID: 31467
		public GameObject GorillaHeadModel;

		// Token: 0x04007AEC RID: 31468
		public GameObject GorillaTorsoModel;

		// Token: 0x04007AED RID: 31469
		public GameObject GorillaTorsoPostModel;

		// Token: 0x04007AEE RID: 31470
		public GameObject GorillaMannequinModel;

		// Token: 0x04007AEF RID: 31471
		public GameObject GuitarStandModel;

		// Token: 0x04007AF0 RID: 31472
		public GameObject GuitarStandMount;

		// Token: 0x04007AF1 RID: 31473
		public GameObject JeweleryBoxModel;

		// Token: 0x04007AF2 RID: 31474
		public GameObject JeweleryBoxMount;

		// Token: 0x04007AF3 RID: 31475
		public GameObject TableMount;

		// Token: 0x04007AF4 RID: 31476
		[FormerlySerializedAs("PinDisplayMounnt")]
		[FormerlySerializedAs("PinDisplayMountn")]
		public GameObject PinDisplayMount;

		// Token: 0x04007AF5 RID: 31477
		public GameObject root;

		// Token: 0x04007AF6 RID: 31478
		public GameObject TagEffectDisplayMount;

		// Token: 0x04007AF7 RID: 31479
		public GameObject TageEffectDisplayModel;

		// Token: 0x04007AF8 RID: 31480
		private Scene customMapScene;

		// Token: 0x04007AF9 RID: 31481
		[HideInInspector]
		public StoreDisplay parentDisplay;

		// Token: 0x04007AFA RID: 31482
		[HideInInspector]
		public StoreDepartment parentDepartment;

		// Token: 0x04007AFB RID: 31483
		private int searchIndex;
	}
}
