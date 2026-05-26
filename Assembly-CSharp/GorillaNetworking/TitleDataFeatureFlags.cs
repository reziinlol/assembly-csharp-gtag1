using System;
using System.Collections.Generic;
using System.Text;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02001072 RID: 4210
	public class TitleDataFeatureFlags
	{
		// Token: 0x170009E5 RID: 2533
		// (get) Token: 0x06006990 RID: 27024 RVA: 0x0022240E File Offset: 0x0022060E
		// (set) Token: 0x06006991 RID: 27025 RVA: 0x00222416 File Offset: 0x00220616
		public bool ready { get; private set; }

		// Token: 0x06006992 RID: 27026 RVA: 0x0022241F File Offset: 0x0022061F
		public void FetchFeatureFlags()
		{
			PlayFabTitleDataCache.Instance.GetTitleData(this.TitleDataKey, delegate(string json)
			{
				try
				{
					foreach (FeatureFlagData featureFlagData in JsonUtility.FromJson<FeatureFlagListData>(json).flags)
					{
						if (featureFlagData.valueType == "percent")
						{
							this.flagValueByName.AddOrUpdate(featureFlagData.name, featureFlagData.value);
						}
						List<string> alwaysOnForUsers = featureFlagData.alwaysOnForUsers;
						if (alwaysOnForUsers != null && alwaysOnForUsers.Count > 0)
						{
							this.flagValueByUser.AddOrUpdate(featureFlagData.name, featureFlagData.alwaysOnForUsers);
						}
					}
				}
				catch (Exception arg)
				{
					Debug.LogError(string.Format("Error parsing rollout feature flags: {0}", arg));
				}
				finally
				{
					this.ready = true;
				}
			}, delegate(PlayFabError e)
			{
				Debug.LogError("Error fetching rollout feature flags: " + e.ErrorMessage);
				this.ready = true;
			}, false);
		}

		// Token: 0x06006993 RID: 27027 RVA: 0x0022244C File Offset: 0x0022064C
		public bool IsEnabledForUser(string flagName)
		{
			bool flag;
			this.logSent.TryGetValue(flagName, out flag);
			this.logSent[flagName] = true;
			string playFabPlayerId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			List<string> list;
			if (this.flagValueByUser.TryGetValue(flagName, out list) && list != null && list.Contains(playFabPlayerId))
			{
				return true;
			}
			int num;
			if (!this.flagValueByName.TryGetValue(flagName, out num))
			{
				bool flag2;
				return this.defaults.TryGetValue(flagName, out flag2) && flag2;
			}
			return num > 0 && (num >= 100 || (ulong)(XXHash32.Compute(Encoding.UTF8.GetBytes(playFabPlayerId), 0U) % 100U) < (ulong)((long)num));
		}

		// Token: 0x040079B1 RID: 31153
		public string TitleDataKey = "DeployFeatureFlags";

		// Token: 0x040079B3 RID: 31155
		public Dictionary<string, bool> defaults = new Dictionary<string, bool>
		{
			{
				"2026-04-VStumpGrabbablesFix",
				true
			},
			{
				"2026-04-SuppressZonesInVStump",
				true
			}
		};

		// Token: 0x040079B4 RID: 31156
		private Dictionary<string, int> flagValueByName = new Dictionary<string, int>();

		// Token: 0x040079B5 RID: 31157
		private Dictionary<string, List<string>> flagValueByUser = new Dictionary<string, List<string>>();

		// Token: 0x040079B6 RID: 31158
		private Dictionary<string, bool> logSent = new Dictionary<string, bool>();
	}
}
