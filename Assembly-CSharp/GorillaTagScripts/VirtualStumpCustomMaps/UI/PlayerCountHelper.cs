using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaNetworking;
using Modio.Mods;
using PlayFab;
using PlayFab.CloudScriptModels;
using PlayFab.Json;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps.UI
{
	// Token: 0x02000F57 RID: 3927
	[NullableContext(1)]
	[Nullable(0)]
	public static class PlayerCountHelper
	{
		// Token: 0x060061F2 RID: 25074 RVA: 0x001F96CC File Offset: 0x001F78CC
		public static void GetPlayerCount(Mod mod, Action<string> successCallback, [Nullable(new byte[]
		{
			2,
			1
		})] Action<PlayFabError> errorCallback = null)
		{
			PlayerCountHelper.GetPlayerCountInternal(mod.Id.ToString(), delegate(ulong count)
			{
				successCallback(PlayerCountHelper.FormatPlayerCount(count));
			}, errorCallback);
		}

		// Token: 0x060061F3 RID: 25075 RVA: 0x001F970C File Offset: 0x001F790C
		public static void GetPlayerCountBatched(IDictionary<Mod, Action<string>> modsAndCallbacks, [Nullable(new byte[]
		{
			2,
			1
		})] Action<PlayFabError> errorCallback = null)
		{
			GorillaServer instance = GorillaServer.Instance;
			if (instance == null)
			{
				return;
			}
			ReturnVstumpMapStatsRequest returnVstumpMapStatsRequest = new ReturnVstumpMapStatsRequest();
			returnVstumpMapStatsRequest.mapIds = (from mod in modsAndCallbacks.Keys
			select mod.Id.ToString()).ToList<string>();
			ReturnVstumpMapStatsRequest request = returnVstumpMapStatsRequest;
			instance.ReturnVstumpMapStats(request, delegate(ExecuteFunctionResult executeFunctionResult)
			{
				PlayerCountHelper.UnpackSuccessBatched(executeFunctionResult, modsAndCallbacks);
			}, errorCallback ?? new Action<PlayFabError>(PlayerCountHelper.DefaultErrorCallback));
		}

		// Token: 0x060061F4 RID: 25076 RVA: 0x001F9798 File Offset: 0x001F7998
		private static void GetPlayerCountInternal(string modId, Action<ulong> successCallback, [Nullable(new byte[]
		{
			2,
			1
		})] Action<PlayFabError> errorCallback = null)
		{
			GorillaServer instance = GorillaServer.Instance;
			if (instance == null)
			{
				return;
			}
			ReturnVstumpMapStatsRequest request = new ReturnVstumpMapStatsRequest
			{
				mapIds = new List<string>
				{
					modId
				}
			};
			instance.ReturnVstumpMapStats(request, delegate(ExecuteFunctionResult executeFunctionResult)
			{
				PlayerCountHelper.UnpackSuccess(executeFunctionResult, modId, successCallback);
			}, errorCallback ?? new Action<PlayFabError>(PlayerCountHelper.DefaultErrorCallback));
		}

		// Token: 0x060061F5 RID: 25077 RVA: 0x001F9808 File Offset: 0x001F7A08
		private static void UnpackSuccess(ExecuteFunctionResult result, string modId, Action<ulong> callback)
		{
			JsonObject jsonObject = result.FunctionResult as JsonObject;
			if (jsonObject == null)
			{
				return;
			}
			JsonObject jsonObject2;
			if (!jsonObject.TryGetValue("Maps", out jsonObject2))
			{
				return;
			}
			JsonObject jsonObject3;
			if (jsonObject2 == null || !jsonObject2.TryGetValue(modId, out jsonObject3))
			{
				return;
			}
			ulong obj;
			if (jsonObject3 == null || !jsonObject3.TryGetValue("PlayerCount", out obj))
			{
				return;
			}
			callback(obj);
		}

		// Token: 0x060061F6 RID: 25078 RVA: 0x001F9860 File Offset: 0x001F7A60
		private static void UnpackSuccessBatched(ExecuteFunctionResult result, IDictionary<Mod, Action<string>> modsAndCallbacks)
		{
			JsonObject jsonObject = result.FunctionResult as JsonObject;
			if (jsonObject == null)
			{
				return;
			}
			JsonObject jsonObject2;
			if (!jsonObject.TryGetValue("Maps", out jsonObject2))
			{
				return;
			}
			if (jsonObject2 == null)
			{
				return;
			}
			Dictionary<string, ulong> dictionary = new Dictionary<string, ulong>();
			foreach (string key in jsonObject2.Keys)
			{
				JsonObject jsonObject3;
				ulong value;
				if (jsonObject2.TryGetValue(key, out jsonObject3) && jsonObject3 != null && jsonObject3.TryGetValue("PlayerCount", out value))
				{
					dictionary[key] = value;
				}
			}
			foreach (KeyValuePair<Mod, Action<string>> keyValuePair in modsAndCallbacks)
			{
				ulong count;
				if (dictionary.TryGetValue(keyValuePair.Key.Id.ToString(), out count))
				{
					string obj = PlayerCountHelper.FormatPlayerCount(count);
					keyValuePair.Value(obj);
				}
			}
		}

		// Token: 0x060061F7 RID: 25079 RVA: 0x001F9970 File Offset: 0x001F7B70
		private static void DefaultErrorCallback(PlayFabError error)
		{
			Debug.Log("Error fetching player count: " + error.ErrorMessage);
		}

		// Token: 0x060061F8 RID: 25080 RVA: 0x001F9988 File Offset: 0x001F7B88
		private static string FormatPlayerCount(ulong count)
		{
			if (count < 1000UL)
			{
				return count.ToString();
			}
			float num = count;
			foreach (char c in new char[]
			{
				'K',
				'M',
				'B'
			})
			{
				num /= 1000f;
				if (num < 1000f)
				{
					return num.ToString("###.###") + c.ToString();
				}
			}
			throw new Exception("Tried to format too-large player count.");
		}

		// Token: 0x040070BD RID: 28861
		private const string MapsJsonKey = "Maps";

		// Token: 0x040070BE RID: 28862
		private const string PlayerCountJsonKey = "PlayerCount";
	}
}
