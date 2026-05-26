using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GorillaExtensions;
using GorillaUtil;
using LitJson;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaNetworking
{
	// Token: 0x0200108B RID: 4235
	public class PlayFabTitleDataCache : MonoBehaviour
	{
		// Token: 0x170009FA RID: 2554
		// (get) Token: 0x06006A30 RID: 27184 RVA: 0x00225776 File Offset: 0x00223976
		// (set) Token: 0x06006A31 RID: 27185 RVA: 0x0022577D File Offset: 0x0022397D
		public static PlayFabTitleDataCache Instance { get; private set; }

		// Token: 0x170009FB RID: 2555
		// (get) Token: 0x06006A32 RID: 27186 RVA: 0x00225785 File Offset: 0x00223985
		private static string FilePath
		{
			get
			{
				return Path.Combine(Application.persistentDataPath, "TitleDataCache.json");
			}
		}

		// Token: 0x06006A33 RID: 27187 RVA: 0x00225798 File Offset: 0x00223998
		public void GetTitleData(string name, Action<string> callback, Action<PlayFabError> errorCallback, bool ignoreCache = false)
		{
			Dictionary<string, string> dictionary;
			string text;
			if (ignoreCache || this.isFirstLoad || !this.localizedTitleData.TryGetValue(LocalisationManager.CurrentLanguage.Identifier.Code, out dictionary) || !dictionary.TryGetValue(name, out text))
			{
				PlayFabTitleDataCache.DataRequest item = new PlayFabTitleDataCache.DataRequest
				{
					Name = name,
					Callback = callback,
					ErrorCallback = errorCallback
				};
				this.requests.Add(item);
				this.TryUpdateData();
				return;
			}
			callback.SafeInvoke(text);
			Action<string, string> onCachedValueRetieved = PlayFabTitleDataCache.OnCachedValueRetieved;
			if (onCachedValueRetieved == null)
			{
				return;
			}
			onCachedValueRetieved(name, text);
		}

		// Token: 0x06006A34 RID: 27188 RVA: 0x00225823 File Offset: 0x00223A23
		private void Awake()
		{
			if (PlayFabTitleDataCache.Instance != null)
			{
				Object.Destroy(this);
				return;
			}
			PlayFabTitleDataCache.Instance = this;
			Action<PlayFabTitleDataCache> action = PlayFabTitleDataCache.k_onnLoaded;
			if (action != null)
			{
				action(this);
			}
			PlayFabTitleDataCache.k_onnLoaded = null;
		}

		// Token: 0x06006A35 RID: 27189 RVA: 0x00225856 File Offset: 0x00223A56
		private void Start()
		{
			this.UpdateData();
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.TryUpdateData));
		}

		// Token: 0x06006A36 RID: 27190 RVA: 0x0022586F File Offset: 0x00223A6F
		private void OnDestroy()
		{
			LocalisationManager.UnregisterOnLanguageChanged(new Action(this.TryUpdateData));
		}

		// Token: 0x06006A37 RID: 27191 RVA: 0x00225882 File Offset: 0x00223A82
		private void TryUpdateData()
		{
			if (!this.isFirstLoad && this.updateDataCoroutine == null)
			{
				this.UpdateData();
			}
		}

		// Token: 0x06006A38 RID: 27192 RVA: 0x0022589C File Offset: 0x00223A9C
		public CacheImport LoadDataFromFile()
		{
			CacheImport result;
			try
			{
				if (!File.Exists(PlayFabTitleDataCache.FilePath))
				{
					Debug.LogWarning("[PlayFabTitleDataCache::LoadDataFromFile] Title data file " + PlayFabTitleDataCache.FilePath + " does not exist!");
					result = null;
				}
				else
				{
					result = (JsonMapper.ToObject<CacheImport>(File.ReadAllText(PlayFabTitleDataCache.FilePath)) ?? new CacheImport());
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("[PlayFabTitleDataCache::LoadDataFromFile] Error reading PlayFab title data from file: {0}", arg));
				result = null;
			}
			return result;
		}

		// Token: 0x06006A39 RID: 27193 RVA: 0x00225914 File Offset: 0x00223B14
		private static void SaveDataToFile(string filepath, Dictionary<string, Dictionary<string, string>> titleData)
		{
			try
			{
				string contents = JsonMapper.ToJson(new CacheImport
				{
					DeploymentId = MothershipClientApiUnity.DeploymentId,
					TitleData = titleData
				});
				File.WriteAllText(filepath, contents);
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("[PlayFabTitleDataCache::SaveDataToFile] Error writing PlayFab title data to file: {0}", arg));
			}
		}

		// Token: 0x06006A3A RID: 27194 RVA: 0x0022596C File Offset: 0x00223B6C
		public void UpdateData()
		{
			this.updateDataCoroutine = base.StartCoroutine(this.UpdateDataCo());
		}

		// Token: 0x06006A3B RID: 27195 RVA: 0x00225980 File Offset: 0x00223B80
		private IEnumerator UpdateDataCo()
		{
			try
			{
				PlayFabTitleDataCache.<>c__DisplayClass27_0 CS$<>8__locals1 = new PlayFabTitleDataCache.<>c__DisplayClass27_0();
				CacheImport oldCache = this.LoadDataFromFile();
				CS$<>8__locals1.currentLocale = LocalisationManager.CurrentLanguage.Identifier.Code;
				Dictionary<string, string> titleData;
				if (!this.localizedTitleData.TryGetValue(CS$<>8__locals1.currentLocale, out titleData))
				{
					this.localizedTitleData[CS$<>8__locals1.currentLocale] = new Dictionary<string, string>();
					titleData = this.localizedTitleData[CS$<>8__locals1.currentLocale];
				}
				Dictionary<string, string> oldLocalizedCache;
				if (oldCache == null || oldCache.TitleData == null || !oldCache.TitleData.TryGetValue(CS$<>8__locals1.currentLocale, out oldLocalizedCache))
				{
					oldLocalizedCache = new Dictionary<string, string>();
				}
				yield return new WaitUntil(() => MothershipClientApiUnity.IsClientLoggedIn());
				bool wipeOldData = oldCache == null || oldCache.DeploymentId != MothershipClientApiUnity.DeploymentId;
				CS$<>8__locals1.newTitleData = null;
				CS$<>8__locals1.mothershipError = null;
				Stopwatch.StartNew();
				StringVector stringVector = new StringVector();
				if (!this.isFirstLoad)
				{
					foreach (PlayFabTitleDataCache.DataRequest dataRequest in this.requests)
					{
						stringVector.Add(dataRequest.Name);
					}
				}
				CS$<>8__locals1.finished = false;
				if (!MothershipClientApiUnity.ListMothershipTitleData(MothershipClientApiUnity.TitleId, MothershipClientApiUnity.EnvironmentId, MothershipClientApiUnity.DeploymentId, stringVector, delegate(ListClientMothershipTitleDataResponse response)
				{
					if (response != null && response.Results != null)
					{
						CS$<>8__locals1.newTitleData = new Dictionary<string, string>();
						for (int j = 0; j < response.Results.Count; j++)
						{
							MothershipTitleDataShort mothershipTitleDataShort = response.Results[j];
							if (!string.IsNullOrEmpty(mothershipTitleDataShort.key))
							{
								if (mothershipTitleDataShort.data.Contains("#EN_FALLBACK="))
								{
									Debug.LogWarning(string.Concat(new string[]
									{
										"[PlayFabTitleDataCache::UpdateDataCo] Key '",
										mothershipTitleDataShort.key,
										"' exists, but it doesn't have a translation for locale '",
										CS$<>8__locals1.currentLocale,
										"'. Falling back to English."
									}));
									mothershipTitleDataShort.data = mothershipTitleDataShort.data.Split("#EN_FALLBACK=", StringSplitOptions.None)[1];
								}
								CS$<>8__locals1.newTitleData[mothershipTitleDataShort.key] = mothershipTitleDataShort.data;
							}
						}
						CS$<>8__locals1.mothershipError = null;
					}
					else
					{
						CS$<>8__locals1.mothershipError = "Failed to fetch title data - response or results were null";
						Debug.LogError("[PlayFabTitleDataCache::UpdateDataCo] " + CS$<>8__locals1.mothershipError);
					}
					CS$<>8__locals1.finished = true;
				}, delegate(MothershipError error, int statusCode)
				{
					CS$<>8__locals1.mothershipError = string.Format("Error fetching title data: {0} (Status: {1})", ((error != null) ? error.Message : null) ?? "Unknown error", statusCode);
					Debug.LogError("[PlayFabTitleDataCache::UpdateDataCo] Mothership API error callback - " + CS$<>8__locals1.mothershipError);
					CS$<>8__locals1.finished = true;
				}))
				{
					CS$<>8__locals1.mothershipError = "Mothership API call was not sent.";
					Debug.LogError("[PlayFabTitleDataCache::UpdateDataCo] " + CS$<>8__locals1.mothershipError);
				}
				yield return new WaitUntil(() => CS$<>8__locals1.finished);
				if (CS$<>8__locals1.newTitleData != null)
				{
					if (wipeOldData)
					{
						this.localizedTitleData.Clear();
						this.localizedTitleData[CS$<>8__locals1.currentLocale] = new Dictionary<string, string>();
						titleData = this.localizedTitleData[CS$<>8__locals1.currentLocale];
					}
					if (!this.localesUpdated.ContainsKey(CS$<>8__locals1.currentLocale))
					{
						titleData.Clear();
					}
					foreach (KeyValuePair<string, string> keyValuePair in CS$<>8__locals1.newTitleData)
					{
						string text;
						string text2;
						keyValuePair.Deconstruct(out text, out text2);
						string text3 = text;
						string text4 = text2;
						titleData[text3] = text4;
						for (int i = this.requests.Count - 1; i >= 0; i--)
						{
							PlayFabTitleDataCache.DataRequest dataRequest2 = this.requests[i];
							if (dataRequest2.Name == text3)
							{
								try
								{
									Action<string> callback = dataRequest2.Callback;
									if (callback != null)
									{
										callback(text4);
									}
									Action<string, string> onValueRetieved = PlayFabTitleDataCache.OnValueRetieved;
									if (onValueRetieved != null)
									{
										onValueRetieved(text3, text4);
									}
								}
								catch (Exception ex)
								{
									Debug.LogError(string.Concat(new string[]
									{
										"[PlayFabTitleDataCache::UpdateDataCo] Error running callback for key: '",
										text3,
										"' value: '",
										text4,
										"' exception: ",
										ex.Message
									}));
								}
								this.requests.RemoveAt(i);
							}
						}
						string a;
						if (oldLocalizedCache.TryGetValue(text3, out a) && a != text4)
						{
							PlayFabTitleDataCache.DataUpdate onTitleDataUpdate = this.OnTitleDataUpdate;
							if (onTitleDataUpdate != null)
							{
								onTitleDataUpdate.Invoke(text3);
							}
						}
					}
					this.localesUpdated[CS$<>8__locals1.currentLocale] = true;
					PlayFabTitleDataCache.SaveDataToFile(PlayFabTitleDataCache.FilePath, this.localizedTitleData);
				}
				CS$<>8__locals1 = null;
				oldCache = null;
				titleData = null;
				oldLocalizedCache = null;
			}
			finally
			{
				this.ClearRequestWithError(null);
				this.isFirstLoad = false;
				this.updateDataCoroutine = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06006A3C RID: 27196 RVA: 0x00225990 File Offset: 0x00223B90
		private void ClearRequestWithError(PlayFabError e = null)
		{
			if (e == null)
			{
				e = new PlayFabError
				{
					ErrorMessage = "PlayFabError was null. Maybe an exception was encountered."
				};
			}
			foreach (PlayFabTitleDataCache.DataRequest dataRequest in this.requests)
			{
				dataRequest.ErrorCallback.SafeInvoke(e);
			}
			this.requests.Clear();
		}

		// Token: 0x06006A3D RID: 27197 RVA: 0x00225A08 File Offset: 0x00223C08
		public static void RegisterOnLoad(Action<PlayFabTitleDataCache> callback)
		{
			if (PlayFabTitleDataCache.Instance.IsNotNull())
			{
				callback(PlayFabTitleDataCache.Instance);
				return;
			}
			PlayFabTitleDataCache.k_onnLoaded = (Action<PlayFabTitleDataCache>)Delegate.Combine(PlayFabTitleDataCache.k_onnLoaded, callback);
		}

		// Token: 0x04007A7D RID: 31357
		private static Action<PlayFabTitleDataCache> k_onnLoaded;

		// Token: 0x04007A7E RID: 31358
		public static Action<string, string> OnValueRetieved;

		// Token: 0x04007A7F RID: 31359
		public static Action<string, string> OnCachedValueRetieved;

		// Token: 0x04007A80 RID: 31360
		public PlayFabTitleDataCache.DataUpdate OnTitleDataUpdate;

		// Token: 0x04007A81 RID: 31361
		private const string FileName = "TitleDataCache.json";

		// Token: 0x04007A82 RID: 31362
		private readonly List<PlayFabTitleDataCache.DataRequest> requests = new List<PlayFabTitleDataCache.DataRequest>();

		// Token: 0x04007A83 RID: 31363
		private Dictionary<string, Dictionary<string, string>> localizedTitleData = new Dictionary<string, Dictionary<string, string>>();

		// Token: 0x04007A84 RID: 31364
		private Dictionary<string, bool> localesUpdated = new Dictionary<string, bool>();

		// Token: 0x04007A85 RID: 31365
		private bool isFirstLoad = true;

		// Token: 0x04007A86 RID: 31366
		private Coroutine updateDataCoroutine;

		// Token: 0x04007A87 RID: 31367
		[SerializeField]
		private StringTable betaTitleDataOveride;

		// Token: 0x0200108C RID: 4236
		[Serializable]
		public sealed class DataUpdate : UnityEvent<string>
		{
		}

		// Token: 0x0200108D RID: 4237
		private class DataRequest
		{
			// Token: 0x170009FC RID: 2556
			// (get) Token: 0x06006A40 RID: 27200 RVA: 0x00225A6F File Offset: 0x00223C6F
			// (set) Token: 0x06006A41 RID: 27201 RVA: 0x00225A77 File Offset: 0x00223C77
			public string Name { get; set; }

			// Token: 0x170009FD RID: 2557
			// (get) Token: 0x06006A42 RID: 27202 RVA: 0x00225A80 File Offset: 0x00223C80
			// (set) Token: 0x06006A43 RID: 27203 RVA: 0x00225A88 File Offset: 0x00223C88
			public Action<string> Callback { get; set; }

			// Token: 0x170009FE RID: 2558
			// (get) Token: 0x06006A44 RID: 27204 RVA: 0x00225A91 File Offset: 0x00223C91
			// (set) Token: 0x06006A45 RID: 27205 RVA: 0x00225A99 File Offset: 0x00223C99
			public Action<PlayFabError> ErrorCallback { get; set; }
		}
	}
}
