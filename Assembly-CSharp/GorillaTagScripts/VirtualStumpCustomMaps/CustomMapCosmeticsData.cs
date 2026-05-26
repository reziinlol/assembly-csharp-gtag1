using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using GorillaNetworking.Store;
using GT_CustomMapSupportRuntime;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000F40 RID: 3904
	[CreateAssetMenu(menuName = "ScriptableObjects/CustomMapCosmeticDataSO", order = 0)]
	[Serializable]
	public class CustomMapCosmeticsData : ScriptableObject
	{
		// Token: 0x06006155 RID: 24917 RVA: 0x001F5C9B File Offset: 0x001F3E9B
		public void OnEnable()
		{
			this.initializedFromTitleData = false;
		}

		// Token: 0x06006156 RID: 24918 RVA: 0x001F5CA4 File Offset: 0x001F3EA4
		public void OnDestroy()
		{
			if (PlayFabTitleDataCache.Instance.IsNotNull())
			{
				PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnTitleDataUpdated));
			}
		}

		// Token: 0x06006157 RID: 24919 RVA: 0x001F5CD0 File Offset: 0x001F3ED0
		public bool TryGetItem(GTObjectPlaceholder.ECustomMapCosmeticItem customMapItemSlot, out CustomMapCosmeticItem foundItem)
		{
			if (!this.initializedFromTitleData)
			{
				this.UpdateFromTitleData();
			}
			foundItem = new CustomMapCosmeticItem
			{
				bustType = HeadModel_CosmeticStand.BustType.Disabled,
				playFabID = "INVALID"
			};
			for (int i = 0; i < this.customMapCosmeticItemList.Count; i++)
			{
				if (this.customMapCosmeticItemList[i].customMapItemSlot == customMapItemSlot)
				{
					foundItem = this.customMapCosmeticItemList[i];
					return true;
				}
			}
			for (int j = 0; j < this.fallbackItems.Count; j++)
			{
				if (this.fallbackItems[j].customMapItemSlot == customMapItemSlot)
				{
					foundItem = this.fallbackItems[j];
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006158 RID: 24920 RVA: 0x001F5D8C File Offset: 0x001F3F8C
		private void UpdateFromTitleData()
		{
			if (this.initializedFromTitleData)
			{
				return;
			}
			if (PlayFabTitleDataCache.Instance.IsNull())
			{
				return;
			}
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnTitleDataUpdated));
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnTitleDataUpdated));
			if (PlayFabTitleDataCache.Instance == null)
			{
				Debug.LogError("[CustomMapCosmeticsData::UpdateFromTitleData] TitleData not available, using fallback item data.");
				this.initializedFromTitleData = true;
				return;
			}
			PlayFabTitleDataCache.Instance.GetTitleData(this.titleDataKey, new Action<string>(this.OnGetCosmeticsDataFromTitleData), new Action<PlayFabError>(this.OnPlayFabError), false);
			this.initializedFromTitleData = true;
		}

		// Token: 0x06006159 RID: 24921 RVA: 0x001F5E34 File Offset: 0x001F4034
		private void OnTitleDataUpdated(string updatedKey)
		{
			if (updatedKey == this.titleDataKey)
			{
				this.initializedFromTitleData = false;
				this.UpdateFromTitleData();
			}
		}

		// Token: 0x0600615A RID: 24922 RVA: 0x001F5E54 File Offset: 0x001F4054
		private void OnGetCosmeticsDataFromTitleData(string cosmeticsData)
		{
			string[] array = cosmeticsData.Split("|", StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				string text2 = text;
				text2 = text2.RemoveAll('\\', StringComparison.OrdinalIgnoreCase);
				text2 = text2.Trim('"');
				CustomMapCosmeticItem itemFromJson = JsonUtility.FromJson<CustomMapCosmeticItem>(text2);
				this.customMapCosmeticItemList.RemoveAll((CustomMapCosmeticItem item) => item.customMapItemSlot == itemFromJson.customMapItemSlot);
				this.customMapCosmeticItemList.Add(itemFromJson);
			}
		}

		// Token: 0x0600615B RID: 24923 RVA: 0x001F5ECE File Offset: 0x001F40CE
		private void OnPlayFabError(PlayFabError error)
		{
			Debug.LogError("[CustomMapCosmeticsData::OnPlayFabError] failed to retrieve CosmeticsData from PlayFab: " + error.ErrorMessage);
		}

		// Token: 0x04007000 RID: 28672
		[SerializeField]
		private List<CustomMapCosmeticItem> fallbackItems;

		// Token: 0x04007001 RID: 28673
		[SerializeField]
		private List<CustomMapCosmeticItem> customMapCosmeticItemList;

		// Token: 0x04007002 RID: 28674
		public string titleDataKey = "CustomMapCosmeticData";

		// Token: 0x04007003 RID: 28675
		private bool initializedFromTitleData;
	}
}
