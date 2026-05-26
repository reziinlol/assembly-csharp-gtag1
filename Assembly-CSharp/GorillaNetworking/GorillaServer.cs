using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x0200106C RID: 4204
	public class GorillaServer : MonoBehaviour, ISerializationCallbackReceiver
	{
		// Token: 0x170009E3 RID: 2531
		// (get) Token: 0x06006968 RID: 26984 RVA: 0x00221A85 File Offset: 0x0021FC85
		public bool FeatureFlagsReady
		{
			get
			{
				return this.featureFlags.ready;
			}
		}

		// Token: 0x170009E4 RID: 2532
		// (get) Token: 0x06006969 RID: 26985 RVA: 0x00221A92 File Offset: 0x0021FC92
		private PlayFab.CloudScriptModels.EntityKey playerEntity
		{
			get
			{
				return new PlayFab.CloudScriptModels.EntityKey
				{
					Id = PlayFabSettings.staticPlayer.EntityId,
					Type = PlayFabSettings.staticPlayer.EntityType
				};
			}
		}

		// Token: 0x0600696A RID: 26986 RVA: 0x00221AB9 File Offset: 0x0021FCB9
		public void Start()
		{
			this.featureFlags.FetchFeatureFlags();
		}

		// Token: 0x0600696B RID: 26987 RVA: 0x00221AC6 File Offset: 0x0021FCC6
		private void Awake()
		{
			if (GorillaServer.Instance == null)
			{
				GorillaServer.Instance = this;
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x0600696C RID: 26988 RVA: 0x00221AE8 File Offset: 0x0021FCE8
		public void ReturnCurrentVersion(ReturnCurrentVersionRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnCurrentVersion result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnCurrentVersion error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "ReturnCurrentVersionV2",
				FunctionParameter = request
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x0600696D RID: 26989 RVA: 0x00221B40 File Offset: 0x0021FD40
		public void TryDistributeCurrency(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "TryDistributeCurrency result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "TryDistributeCurrency error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "TryDistributeCurrencyV2",
				FunctionParameter = new
				{

				}
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x0600696E RID: 26990 RVA: 0x00221B9C File Offset: 0x0021FD9C
		public void AddOrRemoveDLCOwnership(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "AddOrRemoveDLCOwnership result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "AddOrRemoveDLCOwnership error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "AddOrRemoveDLCOwnershipV2",
				FunctionParameter = new
				{

				}
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x0600696F RID: 26991 RVA: 0x00221BF8 File Offset: 0x0021FDF8
		public void BroadcastMyRoom(BroadcastMyRoomRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "BroadcastMyRoom result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "BroadcastMyRoom error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "BroadcastMyRoomV2",
				FunctionParameter = request
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006970 RID: 26992 RVA: 0x00221C50 File Offset: 0x0021FE50
		public void UpdateUserCosmetics()
		{
			ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
			executeFunctionRequest.Entity = this.playerEntity;
			executeFunctionRequest.FunctionName = "UpdatePersonalCosmeticsList";
			executeFunctionRequest.FunctionParameter = new
			{

			};
			executeFunctionRequest.GeneratePlayStreamEvent = new bool?(false);
			PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
			{
				if (CosmeticsController.instance != null)
				{
					CosmeticsController.instance.CheckCosmeticsSharedGroup();
				}
			}, delegate(PlayFabError error)
			{
			}, null, null);
		}

		// Token: 0x06006971 RID: 26993 RVA: 0x00221CD8 File Offset: 0x0021FED8
		public void GetAcceptedAgreements(GetAcceptedAgreementsRequest request, Action<Dictionary<string, string>> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<Dictionary<string, string>>(successCallback, "GetAcceptedAgreements result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "GetAcceptedAgreements json error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "GetAcceptedAgreements",
				FunctionParameter = string.Join(",", request.AgreementKeys),
				GeneratePlayStreamEvent = new bool?(false)
			}, delegate(ExecuteFunctionResult result)
			{
				try
				{
					string value = Convert.ToString(result.FunctionResult);
					successCallback(JsonConvert.DeserializeObject<Dictionary<string, string>>(value));
				}
				catch (Exception arg)
				{
					errorCallback(new PlayFabError
					{
						ErrorMessage = string.Format("Invalid format for GetAcceptedAgreements ({0})", arg),
						Error = PlayFabErrorCode.JsonParseError
					});
				}
			}, errorCallback, null, null);
		}

		// Token: 0x06006972 RID: 26994 RVA: 0x00221D80 File Offset: 0x0021FF80
		public void SubmitAcceptedAgreements(SubmitAcceptedAgreementsRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "SubmitAcceptedAgreements result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "SubmitAcceptedAgreements error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "SubmitAcceptedAgreements",
				FunctionParameter = request.Agreements,
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006973 RID: 26995 RVA: 0x00221DE8 File Offset: 0x0021FFE8
		public void UploadGorillanalytics(object uploadData)
		{
			ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
			executeFunctionRequest.Entity = this.playerEntity;
			executeFunctionRequest.FunctionName = "Gorillanalytics";
			executeFunctionRequest.FunctionParameter = uploadData;
			executeFunctionRequest.GeneratePlayStreamEvent = new bool?(false);
			PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
			{
			}, delegate(PlayFabError error)
			{
			}, null, null);
		}

		// Token: 0x06006974 RID: 26996 RVA: 0x00221E6C File Offset: 0x0022006C
		public void CheckForBadName(CheckForBadNameRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "CheckForBadName result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "CheckForBadName error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "CheckForBadName",
				FunctionParameter = new
				{
					name = request.name,
					forRoom = request.forRoom.ToString(),
					forTroop = request.forTroop.ToString()
				},
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006975 RID: 26997 RVA: 0x00221EF0 File Offset: 0x002200F0
		public void GetRandomName(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "GetRandomName result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "GetRandomName error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "GetRandomName",
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006976 RID: 26998 RVA: 0x00221F4C File Offset: 0x0022014C
		public void ReturnQueueStats(ReturnQueueStatsRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnQueueStats result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnQueueStats error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "ReturnQueueStats",
				FunctionParameter = new
				{
					QueueName = request.queueName
				},
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006977 RID: 26999 RVA: 0x00221FB8 File Offset: 0x002201B8
		public void ReturnVstumpMapStats(ReturnVstumpMapStatsRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnVstumpMapStats result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnVstumpMapStats error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "ReturnVstumpMapStats",
				FunctionParameter = new
				{
					MapIds = request.mapIds
				},
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006978 RID: 27000 RVA: 0x00222023 File Offset: 0x00220223
		private Action<T> DebugWrapCb<T>(Action<T> cb, string label)
		{
			return delegate(T arg)
			{
				bool flag = this.debug;
				cb(arg);
			};
		}

		// Token: 0x06006979 RID: 27001 RVA: 0x00222044 File Offset: 0x00220244
		private ExecuteFunctionResult toFunctionResult(PlayFab.ClientModels.ExecuteCloudScriptResult csResult)
		{
			FunctionExecutionError error = null;
			if (csResult.Error != null)
			{
				error = new FunctionExecutionError
				{
					Error = csResult.Error.Error,
					Message = csResult.Error.Message,
					StackTrace = csResult.Error.StackTrace
				};
			}
			return new ExecuteFunctionResult
			{
				CustomData = csResult.CustomData,
				Error = error,
				ExecutionTimeMilliseconds = Convert.ToInt32(Math.Round(csResult.ExecutionTimeSeconds * 1000.0)),
				FunctionName = csResult.FunctionName,
				FunctionResult = csResult.FunctionResult,
				FunctionResultTooLarge = csResult.FunctionResultTooLarge
			};
		}

		// Token: 0x0600697A RID: 27002 RVA: 0x002220F0 File Offset: 0x002202F0
		public void OnBeforeSerialize()
		{
			this.FeatureFlagsTitleDataKey = this.featureFlags.TitleDataKey;
			this.DefaultDeployFeatureFlagsEnabled.Clear();
			foreach (KeyValuePair<string, bool> keyValuePair in this.featureFlags.defaults)
			{
				if (keyValuePair.Value)
				{
					this.DefaultDeployFeatureFlagsEnabled.Add(keyValuePair.Key);
				}
			}
		}

		// Token: 0x0600697B RID: 27003 RVA: 0x00222178 File Offset: 0x00220378
		public void OnAfterDeserialize()
		{
			this.featureFlags.TitleDataKey = this.FeatureFlagsTitleDataKey;
			foreach (string key in this.DefaultDeployFeatureFlagsEnabled)
			{
				this.featureFlags.defaults.AddOrUpdate(key, true);
			}
		}

		// Token: 0x0600697C RID: 27004 RVA: 0x002221E8 File Offset: 0x002203E8
		public bool CheckIsInKIDOptInCohort()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-KIDOptIn");
		}

		// Token: 0x0600697D RID: 27005 RVA: 0x002221FA File Offset: 0x002203FA
		public bool CheckIsInKIDRequiredCohort()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-KIDRequired");
		}

		// Token: 0x0600697E RID: 27006 RVA: 0x0022220C File Offset: 0x0022040C
		public bool CheckOptedInKID()
		{
			return KIDManager.HasOptedInToKID;
		}

		// Token: 0x0600697F RID: 27007 RVA: 0x00222213 File Offset: 0x00220413
		public bool CheckIsTZE_Enabled()
		{
			return this.featureFlags.IsEnabledForUser("2025-10-TelemetryZoneEventSampling");
		}

		// Token: 0x06006980 RID: 27008 RVA: 0x00222225 File Offset: 0x00220425
		public bool CheckIsMothershipTelemetryEnabled()
		{
			return this.featureFlags.IsEnabledForUser("2025-09-MothershipAnalyticsSampleRate");
		}

		// Token: 0x06006981 RID: 27009 RVA: 0x00222238 File Offset: 0x00220438
		public bool CheckIsVStumpGrabbablesFixEnabled()
		{
			if (this.cachedVStumpGrabbablesFix.Item1)
			{
				return this.cachedVStumpGrabbablesFix.Item2;
			}
			bool flag = this.featureFlags.IsEnabledForUser("2026-04-VStumpGrabbablesFix");
			if (this.featureFlags.ready)
			{
				this.cachedVStumpGrabbablesFix.Item2 = flag;
				this.cachedVStumpGrabbablesFix.Item1 = true;
			}
			return flag;
		}

		// Token: 0x06006982 RID: 27010 RVA: 0x00222298 File Offset: 0x00220498
		public bool CheckIsSuppressZonesInVStumpEnabled()
		{
			if (this.cachedSuppressZonesInVStump.Item1)
			{
				return this.cachedSuppressZonesInVStump.Item2;
			}
			bool flag = this.featureFlags.IsEnabledForUser("2026-04-SuppressZonesInVStump");
			if (this.featureFlags.ready)
			{
				this.cachedSuppressZonesInVStump.Item2 = flag;
				this.cachedSuppressZonesInVStump.Item1 = true;
			}
			return flag;
		}

		// Token: 0x0400799B RID: 31131
		public static volatile GorillaServer Instance;

		// Token: 0x0400799C RID: 31132
		public string FeatureFlagsTitleDataKey = "DeployFeatureFlags";

		// Token: 0x0400799D RID: 31133
		public List<string> DefaultDeployFeatureFlagsEnabled = new List<string>();

		// Token: 0x0400799E RID: 31134
		private TitleDataFeatureFlags featureFlags = new TitleDataFeatureFlags();

		// Token: 0x0400799F RID: 31135
		private bool debug;

		// Token: 0x040079A0 RID: 31136
		private JsonSerializerSettings serializationSettings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.Ignore,
			MissingMemberHandling = MissingMemberHandling.Ignore,
			ObjectCreationHandling = ObjectCreationHandling.Replace,
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			TypeNameHandling = TypeNameHandling.Auto
		};

		// Token: 0x040079A1 RID: 31137
		[TupleElementNames(new string[]
		{
			"valid",
			"value"
		})]
		private ValueTuple<bool, bool> cachedVStumpGrabbablesFix;

		// Token: 0x040079A2 RID: 31138
		[TupleElementNames(new string[]
		{
			"valid",
			"value"
		})]
		private ValueTuple<bool, bool> cachedSuppressZonesInVStump;
	}
}
