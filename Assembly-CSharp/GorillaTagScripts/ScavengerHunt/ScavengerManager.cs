using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag.Scripts.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.ScavengerHunt
{
	// Token: 0x02000F74 RID: 3956
	[NullableContext(1)]
	[Nullable(0)]
	public class ScavengerManager : MonoBehaviour
	{
		// Token: 0x17000952 RID: 2386
		// (get) Token: 0x060062B3 RID: 25267 RVA: 0x001FD472 File Offset: 0x001FB672
		// (set) Token: 0x060062B4 RID: 25268 RVA: 0x001FD479 File Offset: 0x001FB679
		[Nullable(2)]
		public static ScavengerManager Instance { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x060062B5 RID: 25269 RVA: 0x001FD481 File Offset: 0x001FB681
		private void Awake()
		{
			if (ScavengerManager.Instance == null)
			{
				ScavengerManager.Instance = this;
				return;
			}
			throw new Exception("Too ScavengerManagers exist at once, this should never happen.");
		}

		// Token: 0x060062B6 RID: 25270 RVA: 0x001FD4A1 File Offset: 0x001FB6A1
		private void Start()
		{
			base.StartCoroutine(this.ImportMothershipUserData());
		}

		// Token: 0x060062B7 RID: 25271 RVA: 0x001FD4B0 File Offset: 0x001FB6B0
		private IEnumerator ImportMothershipUserData()
		{
			while (!MothershipClientContext.IsClientLoggedIn())
			{
				PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
				if (instance != null && instance.loginFailed)
				{
					Debug.LogError("ScavengerManager critical error, could not log into Mothership.");
					yield break;
				}
				yield return new WaitForSecondsRealtime(0.5f);
			}
			MothershipClientApiUnity.GetUserDataValue("ScavengerHunt", new Action<MothershipUserData>(this.OnGetUserDataSuccess), new Action<MothershipError, int>(this.OnGetUserDataFailure), "");
			yield break;
		}

		// Token: 0x060062B8 RID: 25272 RVA: 0x001FD4C0 File Offset: 0x001FB6C0
		private void OnGetUserDataSuccess(MothershipUserData data)
		{
			Debug.Log("Successfully read scavenger hunt data from Mothership.");
			byte[] bytes = Convert.FromBase64String(data.value);
			string @string = Encoding.UTF8.GetString(bytes);
			this.FromJson(@string);
		}

		// Token: 0x060062B9 RID: 25273 RVA: 0x001FD4F6 File Offset: 0x001FB6F6
		private void OnGetUserDataFailure(MothershipError error, int responseCode)
		{
			Debug.LogError(string.Format("Failed to read scavenger hunt user data (error {0} / {1}): {2}", error.Name, responseCode, error.Message));
		}

		// Token: 0x060062BA RID: 25274 RVA: 0x001FD519 File Offset: 0x001FB719
		private void OnDestroy()
		{
			ScavengerManager.Instance = null;
		}

		// Token: 0x060062BB RID: 25275 RVA: 0x001FD524 File Offset: 0x001FB724
		[return: Nullable(2)]
		public ScavengerManager.Hunt GetHunt(string huntName)
		{
			foreach (ScavengerManager.Hunt hunt in this.Hunts)
			{
				if (hunt.Name == huntName)
				{
					return hunt;
				}
			}
			return null;
		}

		// Token: 0x060062BC RID: 25276 RVA: 0x001FD55C File Offset: 0x001FB75C
		public void RegisterTarget(ScavengerTarget target)
		{
			ScavengerManager.Hunt hunt = this.GetHunt(target.HuntName);
			if (hunt == null)
			{
				throw new Exception("No hunt found with name " + target.HuntName + ".");
			}
			if (!hunt.Targets.Contains(target))
			{
				hunt.RegisterTarget(target);
			}
		}

		// Token: 0x060062BD RID: 25277 RVA: 0x001FD5AC File Offset: 0x001FB7AC
		public bool IsCollected(ScavengerTarget target)
		{
			ScavengerManager.Hunt hunt = this.GetHunt(target.HuntName);
			return hunt != null && hunt.IsCollected(target);
		}

		// Token: 0x060062BE RID: 25278 RVA: 0x001FD5D4 File Offset: 0x001FB7D4
		public void Collect(ScavengerTarget target)
		{
			ScavengerManager.Hunt hunt = this.GetHunt(target.HuntName);
			if (hunt == null)
			{
				throw new Exception(string.Concat(new string[]
				{
					"Cannot collect scavenger hunt ",
					target.TargetName,
					", hunt ",
					target.HuntName,
					" does not exist."
				}));
			}
			if (!hunt.Collect(target, false))
			{
				Debug.Log("Did not collect scavenger hunt " + target.TargetName + ". This is normally because the user already collected it.");
				return;
			}
			Debug.Log("Collected " + target.HuntName + "." + target.TargetName);
			string value = this.ToJson().Write();
			MothershipClientApiUnity.SetUserDataValue("ScavengerHunt", value, new Action<SetUserDataResponse>(this.OnSetUserDataSuccess), new Action<MothershipError, int>(this.OnSetUserDataFailure), "");
		}

		// Token: 0x060062BF RID: 25279 RVA: 0x001FD6A6 File Offset: 0x001FB8A6
		private void OnSetUserDataSuccess(SetUserDataResponse response)
		{
			Debug.Log("Successfully wrote scavenger hunt data for user " + response.user_id + " on Mothership key " + response.key_name);
		}

		// Token: 0x060062C0 RID: 25280 RVA: 0x001FD6C8 File Offset: 0x001FB8C8
		private void OnSetUserDataFailure(MothershipError error, int statusCode)
		{
			Debug.LogError(string.Format("Failed to write scavenger hunt data to Mothership (error {0} / {1}): {2}", error.Name, statusCode, error.Message));
		}

		// Token: 0x060062C1 RID: 25281 RVA: 0x001FD6EB File Offset: 0x001FB8EB
		public ScavengerManager.ScavengerJson ToJson()
		{
			return ScavengerManager.ScavengerJson.FromManager(this);
		}

		// Token: 0x060062C2 RID: 25282 RVA: 0x001FD6F3 File Offset: 0x001FB8F3
		public void FromJson(string json)
		{
			this.FromJson(ScavengerManager.ScavengerJson.FromJson(json));
		}

		// Token: 0x060062C3 RID: 25283 RVA: 0x001FD704 File Offset: 0x001FB904
		public void FromJson(ScavengerManager.ScavengerJson json)
		{
			ScavengerManager.Hunt[] hunts = this.Hunts;
			for (int i = 0; i < hunts.Length; i++)
			{
				hunts[i].ClearCollectedTargets();
			}
			foreach (KeyValuePair<string, string[]> keyValuePair in json.CollectedTargets)
			{
				ScavengerManager.Hunt hunt2 = this.GetHunt(keyValuePair.Key);
				if (hunt2 == null)
				{
					throw new Exception("Cannot import scavenger data, no hunt by name " + keyValuePair.Key + ".");
				}
				foreach (string text in keyValuePair.Value)
				{
					ScavengerTarget target = hunt2.GetTarget(text);
					if (target == null)
					{
						if (!hunt2.Deprecated)
						{
							throw new Exception("Cannot import scavenger data, no hunt/target by name " + keyValuePair.Key + "." + text);
						}
					}
					else
					{
						hunt2.Collect(target, true);
					}
				}
			}
			int num = this.Hunts.Sum((ScavengerManager.Hunt hunt) => hunt.Targets.Count);
			Debug.Log(string.Format("Imported {0} targets from {1} scavenger hunts.", num, this.Hunts.Length));
		}

		// Token: 0x0400718D RID: 29069
		public const string MothershipKey = "ScavengerHunt";

		// Token: 0x0400718F RID: 29071
		public ScavengerManager.Hunt[] Hunts = new ScavengerManager.Hunt[0];

		// Token: 0x02000F75 RID: 3957
		[Nullable(0)]
		[Serializable]
		public class Hunt
		{
			// Token: 0x17000953 RID: 2387
			// (get) Token: 0x060062C5 RID: 25285 RVA: 0x001FD864 File Offset: 0x001FBA64
			public bool IsCompleted
			{
				get
				{
					return this.Targets.Count == this.CollectedTargetNames.Count;
				}
			}

			// Token: 0x17000954 RID: 2388
			// (get) Token: 0x060062C6 RID: 25286 RVA: 0x001FD880 File Offset: 0x001FBA80
			public IReadOnlyList<ScavengerTarget> Targets
			{
				get
				{
					List<ScavengerTarget> result;
					if ((result = this._targets) == null)
					{
						result = (this._targets = new List<ScavengerTarget>());
					}
					return result;
				}
			}

			// Token: 0x17000955 RID: 2389
			// (get) Token: 0x060062C7 RID: 25287 RVA: 0x001FD8A8 File Offset: 0x001FBAA8
			public IReadOnlyCollection<string> CollectedTargetNames
			{
				get
				{
					HashSet<string> result;
					if ((result = this._collectedTargetNamesNullable) == null)
					{
						result = (this._collectedTargetNamesNullable = new HashSet<string>());
					}
					return result;
				}
			}

			// Token: 0x17000956 RID: 2390
			// (get) Token: 0x060062C8 RID: 25288 RVA: 0x001FD8D0 File Offset: 0x001FBAD0
			private HashSet<string> _collectedTargetNames
			{
				get
				{
					HashSet<string> result;
					if ((result = this._collectedTargetNamesNullable) == null)
					{
						result = (this._collectedTargetNamesNullable = new HashSet<string>());
					}
					return result;
				}
			}

			// Token: 0x060062C9 RID: 25289 RVA: 0x001FD8F8 File Offset: 0x001FBAF8
			public Hunt(string name)
			{
				this.Name = name;
			}

			// Token: 0x060062CA RID: 25290 RVA: 0x001FD950 File Offset: 0x001FBB50
			public bool Collect(ScavengerTarget target, bool initialLoad = false)
			{
				if (!this.Targets.Contains(target))
				{
					return false;
				}
				if (this._collectedTargetNames.Add(target.TargetName))
				{
					if (!initialLoad || this.SendTargetCollectedEventsOnLoad)
					{
						this.SendTargetCollectedEvents(target);
					}
					if (this.IsCompleted && (!initialLoad || this.SendHuntCompletedEventsOnLoad))
					{
						this.SendHuntCompletedEvents();
					}
					return true;
				}
				return false;
			}

			// Token: 0x060062CB RID: 25291 RVA: 0x001FD9B0 File Offset: 0x001FBBB0
			public void RegisterTarget(ScavengerTarget target)
			{
				if (this.Targets.Contains(target))
				{
					return;
				}
				if (!this.TargetNames.Contains(target.TargetName))
				{
					Debug.LogError(string.Concat(new string[]
					{
						"Scavenger hunt ",
						this.Name,
						" tried to register target ",
						target.TargetName,
						" even though it is not defined in the hunt in ScavengerManager."
					}));
					return;
				}
				this._targets.Add(target);
			}

			// Token: 0x060062CC RID: 25292 RVA: 0x001FDA26 File Offset: 0x001FBC26
			private void SendTargetCollectedEvents(ScavengerTarget target)
			{
				if (this.Deprecated)
				{
					return;
				}
				this.TargetCollected.InvokeAll();
				this.TargetCollectedArg.InvokeAll(target);
				target.TargetCollected.InvokeAll();
				target.TargetCollectedArg.InvokeAll(target);
			}

			// Token: 0x060062CD RID: 25293 RVA: 0x001FDA5F File Offset: 0x001FBC5F
			private void SendHuntCompletedEvents()
			{
				if (this.Deprecated)
				{
					return;
				}
				this.HuntCompleted.InvokeAll();
				this.HuntCompletedArg.InvokeAll(this);
			}

			// Token: 0x060062CE RID: 25294 RVA: 0x001FDA81 File Offset: 0x001FBC81
			public bool IsCollected(ScavengerTarget target)
			{
				return this._collectedTargetNames.Contains(target.TargetName);
			}

			// Token: 0x060062CF RID: 25295 RVA: 0x001FDA94 File Offset: 0x001FBC94
			public void ClearCollectedTargets()
			{
				this._collectedTargetNames.Clear();
			}

			// Token: 0x060062D0 RID: 25296 RVA: 0x001FDAA4 File Offset: 0x001FBCA4
			[return: Nullable(2)]
			public ScavengerTarget GetTarget(string name)
			{
				foreach (ScavengerTarget scavengerTarget in this.Targets)
				{
					if (scavengerTarget.TargetName == name)
					{
						return scavengerTarget;
					}
				}
				return null;
			}

			// Token: 0x04007190 RID: 29072
			public string Name;

			// Token: 0x04007191 RID: 29073
			public bool SendTargetCollectedEventsOnLoad;

			// Token: 0x04007192 RID: 29074
			public bool SendHuntCompletedEventsOnLoad;

			// Token: 0x04007193 RID: 29075
			public bool Deprecated;

			// Token: 0x04007194 RID: 29076
			public string[] TargetNames = new string[0];

			// Token: 0x04007195 RID: 29077
			public UnityEvent[] TargetCollected = new UnityEvent[0];

			// Token: 0x04007196 RID: 29078
			public UnityEvent<ScavengerTarget>[] TargetCollectedArg = new UnityEvent<ScavengerTarget>[0];

			// Token: 0x04007197 RID: 29079
			public UnityEvent[] HuntCompleted = new UnityEvent[0];

			// Token: 0x04007198 RID: 29080
			public UnityEvent<ScavengerManager.Hunt>[] HuntCompletedArg = new UnityEvent<ScavengerManager.Hunt>[0];

			// Token: 0x04007199 RID: 29081
			[Nullable(new byte[]
			{
				2,
				1
			})]
			private List<ScavengerTarget> _targets;

			// Token: 0x0400719A RID: 29082
			[Nullable(new byte[]
			{
				2,
				1
			})]
			private HashSet<string> _collectedTargetNamesNullable;
		}

		// Token: 0x02000F76 RID: 3958
		[Nullable(0)]
		public class ScavengerJson
		{
			// Token: 0x060062D1 RID: 25297 RVA: 0x001FDB00 File Offset: 0x001FBD00
			public static ScavengerManager.ScavengerJson FromManager(ScavengerManager manager)
			{
				ScavengerManager.ScavengerJson scavengerJson = new ScavengerManager.ScavengerJson();
				foreach (ScavengerManager.Hunt hunt in manager.Hunts)
				{
					string[] value = hunt.CollectedTargetNames.ToArray<string>();
					scavengerJson.CollectedTargets[hunt.Name] = value;
				}
				return scavengerJson;
			}

			// Token: 0x060062D2 RID: 25298 RVA: 0x001FDB50 File Offset: 0x001FBD50
			public static ScavengerManager.ScavengerJson FromJson(string json)
			{
				ScavengerManager.ScavengerJson scavengerJson = new ScavengerManager.ScavengerJson();
				ScavengerManager.ScavengerJson result;
				using (TextReader textReader = new StringReader(json))
				{
					using (JsonReader jsonReader = new JsonTextReader(textReader))
					{
						Debug.Log("Scavenger hunt parsing raw json " + json);
						while (jsonReader.Read())
						{
							if (jsonReader.TokenType == JsonToken.PropertyName && (string)jsonReader.Value == "CollectedTargets")
							{
								ScavengerManager.ScavengerJson.ReadCollectedTargets(scavengerJson, jsonReader);
							}
						}
						result = scavengerJson;
					}
				}
				return result;
			}

			// Token: 0x060062D3 RID: 25299 RVA: 0x001FDBE8 File Offset: 0x001FBDE8
			private static void ReadCollectedTargets(ScavengerManager.ScavengerJson json, JsonReader reader)
			{
				int num = 0;
				bool flag = false;
				string text = null;
				List<string> list = new List<string>();
				while (reader.Read())
				{
					JsonToken tokenType = reader.TokenType;
					if (tokenType <= JsonToken.String)
					{
						switch (tokenType)
						{
						case JsonToken.StartObject:
							num++;
							break;
						case JsonToken.StartArray:
							if (flag)
							{
								throw new Exception("Json read error");
							}
							flag = true;
							break;
						case JsonToken.StartConstructor:
							break;
						case JsonToken.PropertyName:
						{
							if (text != null)
							{
								throw new Exception("Json read error");
							}
							string text2 = reader.Value as string;
							if (text2 == null)
							{
								throw new Exception("Json read error");
							}
							text = text2;
							break;
						}
						default:
							if (tokenType == JsonToken.String)
							{
								if (!flag)
								{
									throw new Exception("Json read error");
								}
								string text3 = reader.Value as string;
								if (text3 == null)
								{
									throw new Exception("Json read error");
								}
								list.Add(text3);
							}
							break;
						}
					}
					else if (tokenType != JsonToken.EndObject)
					{
						if (tokenType == JsonToken.EndArray)
						{
							if (!flag)
							{
								throw new Exception("Json read error");
							}
							if (string.IsNullOrEmpty(text))
							{
								throw new Exception("Json read error");
							}
							json.CollectedTargets[text] = list.ToArray();
							text = null;
							list.Clear();
							flag = false;
						}
					}
					else
					{
						num--;
					}
					if (num <= 0)
					{
						return;
					}
				}
				throw new Exception("Json read error");
			}

			// Token: 0x060062D4 RID: 25300 RVA: 0x001FDD24 File Offset: 0x001FBF24
			public string Write()
			{
				JsonSerializer jsonSerializer = new JsonSerializer();
				string result;
				using (TextWriter textWriter = new StringWriterWithEncoding(Encoding.UTF8))
				{
					using (JsonWriter jsonWriter = new JsonTextWriter(textWriter))
					{
						jsonSerializer.Serialize(jsonWriter, this);
						result = textWriter.ToString();
					}
				}
				return result;
			}

			// Token: 0x0400719B RID: 29083
			public readonly Dictionary<string, string[]> CollectedTargets = new Dictionary<string, string[]>();
		}
	}
}
