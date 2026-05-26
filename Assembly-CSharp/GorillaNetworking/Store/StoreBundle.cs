using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x0200109D RID: 4253
	[Serializable]
	public class StoreBundle
	{
		// Token: 0x17000A07 RID: 2567
		// (get) Token: 0x06006AA3 RID: 27299 RVA: 0x00227489 File Offset: 0x00225689
		public string playfabBundleID
		{
			get
			{
				return this._storeBundleDataReference.playfabBundleID;
			}
		}

		// Token: 0x17000A08 RID: 2568
		// (get) Token: 0x06006AA4 RID: 27300 RVA: 0x00227496 File Offset: 0x00225696
		public string bundleSKU
		{
			get
			{
				return this._storeBundleDataReference.bundleSKU;
			}
		}

		// Token: 0x17000A09 RID: 2569
		// (get) Token: 0x06006AA5 RID: 27301 RVA: 0x002274A3 File Offset: 0x002256A3
		public Sprite bundleImage
		{
			get
			{
				return this._storeBundleDataReference.bundleImage;
			}
		}

		// Token: 0x17000A0A RID: 2570
		// (get) Token: 0x06006AA6 RID: 27302 RVA: 0x002274B0 File Offset: 0x002256B0
		public NexusCreatorCode nexusCreatorCode
		{
			get
			{
				return this._storeBundleDataReference.creatorCode;
			}
		}

		// Token: 0x17000A0B RID: 2571
		// (get) Token: 0x06006AA7 RID: 27303 RVA: 0x002274BD File Offset: 0x002256BD
		public string price
		{
			get
			{
				return this._price;
			}
		}

		// Token: 0x17000A0C RID: 2572
		// (get) Token: 0x06006AA8 RID: 27304 RVA: 0x002274C8 File Offset: 0x002256C8
		public string bundleName
		{
			get
			{
				if (this._bundleName.IsNullOrEmpty())
				{
					int num = CosmeticsController.instance.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => this.playfabBundleID == x.itemName);
					if (num > -1)
					{
						if (!CosmeticsController.instance.allCosmetics[num].overrideDisplayName.IsNullOrEmpty())
						{
							this._bundleName = CosmeticsController.instance.allCosmetics[num].overrideDisplayName;
						}
						else
						{
							this._bundleName = CosmeticsController.instance.allCosmetics[num].displayName;
						}
					}
					else
					{
						this._bundleName = "NULL_BUNDLE_NAME";
					}
				}
				return this._bundleName;
			}
		}

		// Token: 0x17000A0D RID: 2573
		// (get) Token: 0x06006AA9 RID: 27305 RVA: 0x00227574 File Offset: 0x00225774
		public bool HasPrice
		{
			get
			{
				return !string.IsNullOrEmpty(this.price) && this.price != StoreBundle.defaultPrice;
			}
		}

		// Token: 0x17000A0E RID: 2574
		// (get) Token: 0x06006AAA RID: 27306 RVA: 0x00227595 File Offset: 0x00225795
		public string bundleDescriptionText
		{
			get
			{
				return this._storeBundleDataReference.bundleDescriptionText;
			}
		}

		// Token: 0x06006AAB RID: 27307 RVA: 0x002275A4 File Offset: 0x002257A4
		public StoreBundle()
		{
			this.isOwned = false;
			this.bundleStands = new List<BundleStand>();
		}

		// Token: 0x06006AAC RID: 27308 RVA: 0x002275F8 File Offset: 0x002257F8
		public StoreBundle(StoreBundleData data)
		{
			this.isOwned = false;
			this.bundleStands = new List<BundleStand>();
			this._storeBundleDataReference = data;
		}

		// Token: 0x06006AAD RID: 27309 RVA: 0x00227650 File Offset: 0x00225850
		public void InitializebundleStands()
		{
			foreach (BundleStand bundleStand in this.bundleStands)
			{
				bundleStand.UpdateDescriptionText(this.bundleDescriptionText);
				bundleStand.InitializeEventListeners();
			}
		}

		// Token: 0x06006AAE RID: 27310 RVA: 0x002276AC File Offset: 0x002258AC
		public void TryUpdatePrice(uint bundlePrice)
		{
			this.TryUpdatePrice((bundlePrice / 100m).ToString());
		}

		// Token: 0x06006AAF RID: 27311 RVA: 0x002276DC File Offset: 0x002258DC
		public void TryUpdatePrice(string bundlePrice = null)
		{
			if (!string.IsNullOrEmpty(bundlePrice))
			{
				decimal num;
				this._price = (decimal.TryParse(bundlePrice, out num) ? (StoreBundle.defaultCurrencySymbol + bundlePrice) : bundlePrice);
			}
			this.UpdatePurchaseButtonText();
		}

		// Token: 0x06006AB0 RID: 27312 RVA: 0x00227718 File Offset: 0x00225918
		public void UpdatePurchaseButtonText()
		{
			this.purchaseButtonText = string.Format(this.purchaseButtonStringFormat, this.bundleName, this.price);
			foreach (BundleStand bundleStand in this.bundleStands)
			{
				bundleStand.UpdatePurchaseButtonText(this.purchaseButtonText);
			}
		}

		// Token: 0x06006AB1 RID: 27313 RVA: 0x0022778C File Offset: 0x0022598C
		public void ValidateBundleData()
		{
			if (this._storeBundleDataReference == null)
			{
				Debug.LogError("StoreBundleData is null");
				foreach (BundleStand bundleStand in this.bundleStands)
				{
					if (bundleStand == null)
					{
						Debug.LogError("BundleStand is null");
					}
					else if (bundleStand._bundleDataReference != null)
					{
						this._storeBundleDataReference = bundleStand._bundleDataReference;
						Debug.LogError("BundleStand StoreBundleData is not equal to StoreBundle StoreBundleData");
					}
				}
			}
			if (this._storeBundleDataReference == null)
			{
				Debug.LogError("StoreBundleData is null");
				return;
			}
			if (this._storeBundleDataReference.playfabBundleID.IsNullOrEmpty())
			{
				Debug.LogError("playfabBundleID is null");
			}
			if (this._storeBundleDataReference.bundleSKU.IsNullOrEmpty())
			{
				Debug.LogError("bundleSKU is null");
			}
			if (this._storeBundleDataReference.bundleImage == null)
			{
				Debug.LogError("bundleImage is null");
			}
			if (this._storeBundleDataReference.bundleDescriptionText.IsNullOrEmpty())
			{
				Debug.LogError("bundleDescriptionText is null");
			}
		}

		// Token: 0x04007AD4 RID: 31444
		private static readonly string defaultPrice = "$--.--";

		// Token: 0x04007AD5 RID: 31445
		private static readonly string defaultCurrencySymbol = "$";

		// Token: 0x04007AD6 RID: 31446
		[NonSerialized]
		public string purchaseButtonStringFormat = "THE {0}\n{1}";

		// Token: 0x04007AD7 RID: 31447
		[SerializeField]
		public List<BundleStand> bundleStands;

		// Token: 0x04007AD8 RID: 31448
		public bool isOwned;

		// Token: 0x04007AD9 RID: 31449
		private string _price = StoreBundle.defaultPrice;

		// Token: 0x04007ADA RID: 31450
		private string _bundleName = "";

		// Token: 0x04007ADB RID: 31451
		public string purchaseButtonText = "";

		// Token: 0x04007ADC RID: 31452
		[FormerlySerializedAs("storeBundleDataReference")]
		[SerializeField]
		[ReadOnly]
		private StoreBundleData _storeBundleDataReference;
	}
}
