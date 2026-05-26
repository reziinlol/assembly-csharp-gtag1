using System;
using System.Collections.Generic;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x020010B0 RID: 4272
	public class StoreUpdateEvent
	{
		// Token: 0x06006B32 RID: 27442 RVA: 0x00002050 File Offset: 0x00000250
		public StoreUpdateEvent()
		{
		}

		// Token: 0x06006B33 RID: 27443 RVA: 0x0022AB58 File Offset: 0x00228D58
		public StoreUpdateEvent(string pedestalID, string itemName, DateTime startTimeUTC, DateTime endTimeUTC)
		{
			this.PedestalID = pedestalID;
			this.ItemName = itemName;
			this.StartTimeUTC = startTimeUTC;
			this.EndTimeUTC = endTimeUTC;
		}

		// Token: 0x06006B34 RID: 27444 RVA: 0x0022AB7D File Offset: 0x00228D7D
		public static string SerializeAsJSon(StoreUpdateEvent storeEvent)
		{
			return JsonUtility.ToJson(storeEvent);
		}

		// Token: 0x06006B35 RID: 27445 RVA: 0x0022AB85 File Offset: 0x00228D85
		public static string SerializeArrayAsJSon(StoreUpdateEvent[] storeEvents)
		{
			return JsonConvert.SerializeObject(storeEvents);
		}

		// Token: 0x06006B36 RID: 27446 RVA: 0x0022AB8D File Offset: 0x00228D8D
		public static StoreUpdateEvent DeserializeFromJSon(string json)
		{
			return JsonUtility.FromJson<StoreUpdateEvent>(json);
		}

		// Token: 0x06006B37 RID: 27447 RVA: 0x0022AB95 File Offset: 0x00228D95
		public static StoreUpdateEvent[] DeserializeFromJSonArray(string json)
		{
			List<StoreUpdateEvent> list = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
			list.Sort((StoreUpdateEvent x, StoreUpdateEvent y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC));
			return list.ToArray();
		}

		// Token: 0x06006B38 RID: 27448 RVA: 0x0022ABC7 File Offset: 0x00228DC7
		public static List<StoreUpdateEvent> DeserializeFromJSonList(string json)
		{
			List<StoreUpdateEvent> list = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
			list.Sort((StoreUpdateEvent x, StoreUpdateEvent y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC));
			return list;
		}

		// Token: 0x04007B52 RID: 31570
		public string PedestalID;

		// Token: 0x04007B53 RID: 31571
		public string ItemName;

		// Token: 0x04007B54 RID: 31572
		public DateTime StartTimeUTC;

		// Token: 0x04007B55 RID: 31573
		public DateTime EndTimeUTC;
	}
}
