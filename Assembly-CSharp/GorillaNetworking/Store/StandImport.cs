using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x020010A7 RID: 4263
	public class StandImport
	{
		// Token: 0x06006B07 RID: 27399 RVA: 0x0022A230 File Offset: 0x00228430
		public void DecomposeFromTitleDataString(string data)
		{
			string[] array = data.Split("\\n", StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				this.DecomposeStandDataTitleData(array[i]);
			}
		}

		// Token: 0x06006B08 RID: 27400 RVA: 0x0022A264 File Offset: 0x00228464
		public void DecomposeStandDataTitleData(string dataString)
		{
			string[] array = dataString.Split("\\t", StringSplitOptions.None);
			if (array.Length == 5)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			if (array.Length == 4)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			string text = "";
			foreach (string str in array)
			{
				text = text + str + "|";
			}
			Debug.LogError("Store Importer Data String is not valid : " + text);
		}

		// Token: 0x06006B09 RID: 27401 RVA: 0x0022A2E7 File Offset: 0x002284E7
		public void DeserializeFromJSON(string JSONString)
		{
			this.standData = JsonConvert.DeserializeObject<List<StandTypeData>>(JSONString);
		}

		// Token: 0x06006B0A RID: 27402 RVA: 0x0022A2F8 File Offset: 0x002284F8
		public void DecomposeStandData(string dataString)
		{
			string[] array = dataString.Split('\t', StringSplitOptions.None);
			if (array.Length == 5)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			if (array.Length == 4)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			string text = "";
			foreach (string str in array)
			{
				text = text + str + "|";
			}
			Debug.LogError("Store Importer Data String is not valid : " + text);
		}

		// Token: 0x04007B22 RID: 31522
		public List<StandTypeData> standData = new List<StandTypeData>();

		// Token: 0x04007B23 RID: 31523
		public Dictionary<string, StandTypeData> standKeyToDataDict = new Dictionary<string, StandTypeData>();
	}
}
