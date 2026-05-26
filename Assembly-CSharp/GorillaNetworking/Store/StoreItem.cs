using System;
using System.IO;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x020010AF RID: 4271
	[Serializable]
	public class StoreItem
	{
		// Token: 0x06006B2F RID: 27439 RVA: 0x0022A9B4 File Offset: 0x00228BB4
		public static void SerializeItemsAsJSON(StoreItem[] items)
		{
			string text = "";
			foreach (StoreItem obj in items)
			{
				text = text + JsonUtility.ToJson(obj) + ";";
			}
			Debug.LogError(text);
			File.WriteAllText(Application.dataPath + "/Resources/StoreItems/FeaturedStoreItemsList.json", text);
		}

		// Token: 0x06006B30 RID: 27440 RVA: 0x0022AA08 File Offset: 0x00228C08
		public static void ConvertCosmeticItemToSToreItem(CosmeticsController.CosmeticItem cosmeticItem, ref StoreItem storeItem)
		{
			storeItem.itemName = cosmeticItem.itemName;
			storeItem.itemCategory = (int)cosmeticItem.itemCategory;
			storeItem.itemPictureResourceString = cosmeticItem.itemPictureResourceString;
			storeItem.displayName = cosmeticItem.displayName;
			storeItem.overrideDisplayName = cosmeticItem.overrideDisplayName;
			storeItem.bundledItems = cosmeticItem.bundledItems;
			storeItem.canTryOn = cosmeticItem.canTryOn;
			storeItem.bothHandsHoldable = cosmeticItem.bothHandsHoldable;
			storeItem.AssetBundleName = "";
			storeItem.bUsesMeshAtlas = cosmeticItem.bUsesMeshAtlas;
			storeItem.MeshResourceName = cosmeticItem.meshResourceString;
			storeItem.MeshAtlasResourceName = cosmeticItem.meshAtlasResourceString;
			storeItem.MaterialResrouceName = cosmeticItem.materialResourceString;
		}

		// Token: 0x04007B42 RID: 31554
		public string itemName = "";

		// Token: 0x04007B43 RID: 31555
		public int itemCategory;

		// Token: 0x04007B44 RID: 31556
		public string itemPictureResourceString = "";

		// Token: 0x04007B45 RID: 31557
		public string displayName = "";

		// Token: 0x04007B46 RID: 31558
		public string overrideDisplayName = "";

		// Token: 0x04007B47 RID: 31559
		public string[] bundledItems = new string[0];

		// Token: 0x04007B48 RID: 31560
		public bool canTryOn;

		// Token: 0x04007B49 RID: 31561
		public bool bothHandsHoldable;

		// Token: 0x04007B4A RID: 31562
		public string AssetBundleName = "";

		// Token: 0x04007B4B RID: 31563
		public bool bUsesMeshAtlas;

		// Token: 0x04007B4C RID: 31564
		public string MeshAtlasResourceName = "";

		// Token: 0x04007B4D RID: 31565
		public string MeshResourceName = "";

		// Token: 0x04007B4E RID: 31566
		public string MaterialResrouceName = "";

		// Token: 0x04007B4F RID: 31567
		public Vector3 translationOffset = Vector3.zero;

		// Token: 0x04007B50 RID: 31568
		public Vector3 rotationOffset = Vector3.zero;

		// Token: 0x04007B51 RID: 31569
		public Vector3 scale = Vector3.one;
	}
}
