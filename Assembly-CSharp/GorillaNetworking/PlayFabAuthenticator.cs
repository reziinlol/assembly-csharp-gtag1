using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using GorillaExtensions;
using JetBrains.Annotations;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GorillaNetworking
{
	// Token: 0x0200107A RID: 4218
	public class PlayFabAuthenticator : MonoBehaviour
	{
		// Token: 0x170009ED RID: 2541
		// (get) Token: 0x060069CF RID: 27087 RVA: 0x00223FD6 File Offset: 0x002221D6
		public GorillaComputer gorillaComputer
		{
			get
			{
				return GorillaComputer.instance;
			}
		}

		// Token: 0x170009EE RID: 2542
		// (get) Token: 0x060069D0 RID: 27088 RVA: 0x00223FDF File Offset: 0x002221DF
		// (set) Token: 0x060069D1 RID: 27089 RVA: 0x00223FE7 File Offset: 0x002221E7
		public bool IsReturningPlayer { get; private set; }

		// Token: 0x170009EF RID: 2543
		// (get) Token: 0x060069D2 RID: 27090 RVA: 0x00223FF0 File Offset: 0x002221F0
		// (set) Token: 0x060069D3 RID: 27091 RVA: 0x00223FF8 File Offset: 0x002221F8
		public bool postAuthSetSafety { get; private set; }

		// Token: 0x060069D4 RID: 27092 RVA: 0x00224004 File Offset: 0x00222204
		private void Awake()
		{
			if (PlayFabAuthenticator.instance == null)
			{
				PlayFabAuthenticator.instance = this;
			}
			else if (PlayFabAuthenticator.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			if (PlayFabAuthenticator.instance.photonAuthenticator == null)
			{
				PlayFabAuthenticator.instance.photonAuthenticator = PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<PhotonAuthenticator>();
			}
			this.platform = ScriptableObject.CreateInstance<PlatformTagJoin>();
			PlayFabSettings.CompressApiData = false;
			new byte[1];
			if (this.screenDebugMode)
			{
				this.debugText.text = "";
			}
			Debug.Log("doing steam thing");
			if (PlayFabAuthenticator.instance.steamAuthenticator == null)
			{
				PlayFabAuthenticator.instance.steamAuthenticator = PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<SteamAuthenticator>();
			}
			this.platform.PlatformTag = "Steam";
			PlayFabSettings.TitleId = PlayFabAuthenticatorSettings.TitleId;
			PlayFabSettings.DisableFocusTimeCollection = true;
			this.BeginLoginFlow();
		}

		// Token: 0x060069D5 RID: 27093 RVA: 0x00224108 File Offset: 0x00222308
		public void BeginLoginFlow()
		{
			if (!MothershipClientApiUnity.IsEnabled())
			{
				this.AuthenticateWithPlayFab();
				return;
			}
			if (PlayFabAuthenticator.instance.mothershipAuthenticator == null)
			{
				PlayFabAuthenticator.instance.mothershipAuthenticator = (MothershipAuthenticator.Instance ?? PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<MothershipAuthenticator>());
				MothershipAuthenticator mothershipAuthenticator = PlayFabAuthenticator.instance.mothershipAuthenticator;
				mothershipAuthenticator.OnLoginSuccess = (Action)Delegate.Combine(mothershipAuthenticator.OnLoginSuccess, new Action(delegate()
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				}));
				MothershipAuthenticator mothershipAuthenticator2 = PlayFabAuthenticator.instance.mothershipAuthenticator;
				mothershipAuthenticator2.OnLoginFailure = (Action<string, string, string>)Delegate.Combine(mothershipAuthenticator2.OnLoginFailure, new Action<string, string, string>(delegate(string errorMessage, string errorCode, string traceId)
				{
					this.SetLoginFailed();
					this.ShowMothershipAuthErrorMessage(errorMessage, errorCode, traceId);
				}));
				PlayFabAuthenticator.instance.mothershipAuthenticator.BeginLoginFlow();
			}
		}

		// Token: 0x060069D6 RID: 27094 RVA: 0x002241E0 File Offset: 0x002223E0
		private void SetLoginFailed()
		{
			this.loginFailed = true;
			NetworkSystem networkSystem = NetworkSystem.Instance;
			if (networkSystem == null)
			{
				return;
			}
			networkSystem.FinishAuthenticating();
		}

		// Token: 0x060069D7 RID: 27095 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void Start()
		{
		}

		// Token: 0x060069D8 RID: 27096 RVA: 0x002241F8 File Offset: 0x002223F8
		private void OnEnable()
		{
			NetworkSystem.Instance.OnCustomAuthenticationResponse += this.OnCustomAuthenticationResponse;
		}

		// Token: 0x060069D9 RID: 27097 RVA: 0x00224210 File Offset: 0x00222410
		private void OnDisable()
		{
			NetworkSystem.Instance.OnCustomAuthenticationResponse -= this.OnCustomAuthenticationResponse;
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			SteamAuthTicket steamAuthTicket2 = this.steamAuthTicketForPlayFab;
			if (steamAuthTicket2 == null)
			{
				return;
			}
			steamAuthTicket2.Dispose();
		}

		// Token: 0x060069DA RID: 27098 RVA: 0x00224249 File Offset: 0x00222449
		public void RefreshSteamAuthTicketForPhoton(Action<string> successCallback, Action<EResult> failureCallback)
		{
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			this.steamAuthTicketForPhoton = this.steamAuthenticator.GetAuthTicketForWebApi(this.steamAuthIdForPhoton, successCallback, failureCallback);
		}

		// Token: 0x060069DB RID: 27099 RVA: 0x0022427C File Offset: 0x0022247C
		private void OnCustomAuthenticationResponse(Dictionary<string, object> response)
		{
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			object obj;
			if (response.TryGetValue("SteamAuthIdForPhoton", out obj))
			{
				string text = obj as string;
				if (text != null)
				{
					this.steamAuthIdForPhoton = text;
					return;
				}
			}
			this.steamAuthIdForPhoton = null;
		}

		// Token: 0x060069DC RID: 27100 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void GetNonceForPlayFab()
		{
		}

		// Token: 0x060069DD RID: 27101 RVA: 0x002242C4 File Offset: 0x002224C4
		private void OnPlayFabAuthResponse(PlayFabAuthenticator.PlayfabAuthResponseData response)
		{
			Debug.Log("[PLAYFAB] Response Received. Response is: [" + (((response != null) ? response.PlayFabId : null) ?? "NULL") + "]");
			if (response != null)
			{
				PlayFabSettings.staticPlayer = new PlayFabAuthenticationContext(response.SessionTicket, response.EntityToken, response.PlayFabId, response.EntityId, response.EntityType);
				this._playFabPlayerIdCache = response.PlayFabId;
				this._sessionTicket = response.SessionTicket;
				DateTime accountCreationDateTime;
				if (DateTime.TryParse(response.AccountCreationIsoTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out accountCreationDateTime))
				{
					base.StartCoroutine(this.VerifyKidAuthenticated(accountCreationDateTime));
				}
				this.AdvanceLogin();
				return;
			}
			Debug.LogError("Error: Could not authenticate with PlayFab");
			this.SetLoginFailed();
		}

		// Token: 0x060069DE RID: 27102 RVA: 0x0022437C File Offset: 0x0022257C
		public void AuthenticateWithPlayFab()
		{
			Debug.Log("authenticating with playFab!");
			GorillaServer gorillaServer = GorillaServer.Instance;
			if (gorillaServer != null && gorillaServer.FeatureFlagsReady)
			{
				if (KIDManager.KidEnabled)
				{
					Debug.Log("[KID] Is Enabled - Enabling safeties by platform and age category");
					this.DefaultSafetiesByAgeCategory();
				}
			}
			else
			{
				this.postAuthSetSafety = true;
			}
			if (SteamManager.Initialized)
			{
				this.userID = SteamUser.GetSteamID().ToString();
				Debug.Log("trying to auth with steam");
				this.steamAuthTicketForPlayFab = this.steamAuthenticator.GetAuthTicket(delegate(string ticket)
				{
					Debug.Log("Got steam auth session ticket!");
					PlayFabClientAPI.LoginWithSteam(new LoginWithSteamRequest
					{
						CreateAccount = new bool?(true),
						SteamTicket = ticket
					}, new Action<LoginResult>(this.OnLoginWithSteamResponse), new Action<PlayFabError>(this.OnPlayFabError), null, null);
				}, delegate(EResult result)
				{
					base.StartCoroutine(this.DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame());
				});
				return;
			}
			base.StartCoroutine(this.DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame());
		}

		// Token: 0x060069DF RID: 27103 RVA: 0x0022442E File Offset: 0x0022262E
		private IEnumerator VerifyKidAuthenticated(DateTime accountCreationDateTime)
		{
			Task<DateTime?> getNewPlayerDateTimeTask = KIDManager.CheckKIDNewPlayerDateTime();
			yield return new WaitUntil(() => getNewPlayerDateTimeTask.IsCompleted);
			DateTime? result = getNewPlayerDateTimeTask.Result;
			if (result != null && KIDManager.KidEnabled)
			{
				this.IsReturningPlayer = (accountCreationDateTime < result);
			}
			yield break;
		}

		// Token: 0x060069E0 RID: 27104 RVA: 0x00224444 File Offset: 0x00222644
		private IEnumerator DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame()
		{
			yield return null;
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.");
				this.gorillaComputer.screenText.Set("UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.");
				Debug.Log("Couldn't authenticate steam account");
			}
			else
			{
				Debug.LogError("PlayFabAuthenticator: gorillaComputer is null, so could not set GeneralFailureMessage notifying user that the steam account could not be authenticated.", this);
			}
			yield break;
		}

		// Token: 0x060069E1 RID: 27105 RVA: 0x00224454 File Offset: 0x00222654
		private void OnLoginWithSteamResponse(LoginResult obj)
		{
			this._playFabPlayerIdCache = obj.PlayFabId;
			this._sessionTicket = obj.SessionTicket;
			base.StartCoroutine(this.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
			{
				Platform = this.platform.ToString(),
				SessionTicket = this._sessionTicket,
				PlayFabId = this._playFabPlayerIdCache,
				TitleId = PlayFabSettings.TitleId,
				MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
				MothershipToken = MothershipClientContext.Token,
				MothershipId = MothershipClientContext.MothershipId
			}, new Action<PlayFabAuthenticator.CachePlayFabIdResponse>(this.OnCachePlayFabIdRequest)));
		}

		// Token: 0x060069E2 RID: 27106 RVA: 0x002244F8 File Offset: 0x002226F8
		private void OnCachePlayFabIdRequest([CanBeNull] PlayFabAuthenticator.CachePlayFabIdResponse response)
		{
			if (response != null)
			{
				this.steamAuthIdForPhoton = response.SteamAuthIdForPhoton;
				DateTime accountCreationDateTime;
				if (DateTime.TryParse(response.AccountCreationIsoTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out accountCreationDateTime))
				{
					base.StartCoroutine(this.VerifyKidAuthenticated(accountCreationDateTime));
				}
				Debug.Log("Successfully cached PlayFab Id.  Continuing!");
				this.AdvanceLogin();
				return;
			}
			Debug.LogError("Could not cache PlayFab Id.  Cannot continue.");
		}

		// Token: 0x060069E3 RID: 27107 RVA: 0x00224556 File Offset: 0x00222756
		private void AdvanceLogin()
		{
			this.LogMessage("PlayFab authenticated ... Getting Nonce");
			this.RefreshSteamAuthTicketForPhoton(delegate(string ticket)
			{
				this._nonce = ticket;
				Debug.Log("Got nonce!  Authenticating...");
				this.AuthenticateWithPhoton();
			}, delegate(EResult result)
			{
				Debug.LogWarning("Failed to get nonce!");
				this.AuthenticateWithPhoton();
			});
		}

		// Token: 0x060069E4 RID: 27108 RVA: 0x00224584 File Offset: 0x00222784
		private void AuthenticateWithPhoton()
		{
			this.photonAuthenticator.SetCustomAuthenticationParameters(new Dictionary<string, object>
			{
				{
					"AppId",
					PlayFabSettings.TitleId
				},
				{
					"AppVersion",
					NetworkSystemConfig.AppVersion ?? "-1"
				},
				{
					"Ticket",
					this._sessionTicket
				},
				{
					"Nonce",
					this._nonce
				},
				{
					"MothershipEnvId",
					MothershipClientApiUnity.EnvironmentId
				},
				{
					"MothershipDeploymentId",
					MothershipClientApiUnity.DeploymentId
				},
				{
					"MothershipToken",
					MothershipClientContext.Token
				}
			});
			this.GetPlayerDisplayName(this._playFabPlayerIdCache);
			GorillaServer.Instance.AddOrRemoveDLCOwnership(delegate(ExecuteFunctionResult result)
			{
				Debug.Log("got results! updating!");
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
				}
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error retrieving user data:");
				Debug.Log(error.GenerateErrorReport());
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
				}
			});
			if (CosmeticsController.instance != null)
			{
				Debug.Log("initializing cosmetics");
				CosmeticsController.instance.Initialize();
			}
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.OnConnectedToMasterStuff();
			}
			else
			{
				base.StartCoroutine(this.ComputerOnConnectedToMaster());
			}
			if (RankedProgressionManager.Instance != null)
			{
				RankedProgressionManager.Instance.LoadStats();
			}
			if (PhotonNetworkController.Instance != null)
			{
				Debug.Log("Finish authenticating");
				NetworkSystem.Instance.FinishAuthenticating();
			}
		}

		// Token: 0x060069E5 RID: 27109 RVA: 0x002246F9 File Offset: 0x002228F9
		private IEnumerator ComputerOnConnectedToMaster()
		{
			WaitForEndOfFrame frameYield = new WaitForEndOfFrame();
			while (this.gorillaComputer == null)
			{
				yield return frameYield;
			}
			this.gorillaComputer.OnConnectedToMasterStuff();
			yield break;
		}

		// Token: 0x060069E6 RID: 27110 RVA: 0x00224708 File Offset: 0x00222908
		private void OnPlayFabError(PlayFabError obj)
		{
			this.LogMessage(obj.ErrorMessage);
			Debug.Log("OnPlayFabError(): " + obj.ErrorMessage);
			this.SetLoginFailed();
			if (obj.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						return;
					}
					this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair.Key);
					return;
				}
			}
			if (obj.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair2 = enumerator.Current;
					if (keyValuePair2.Value[0] != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair2.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						return;
					}
					this.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair2.Key);
					return;
				}
			}
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.GeneralFailureMessage(this.gorillaComputer.unableToConnect);
			}
		}

		// Token: 0x060069E7 RID: 27111 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void LogMessage(string message)
		{
		}

		// Token: 0x060069E8 RID: 27112 RVA: 0x0022493C File Offset: 0x00222B3C
		private void GetPlayerDisplayName(string playFabId)
		{
			GetPlayerProfileRequest getPlayerProfileRequest = new GetPlayerProfileRequest();
			getPlayerProfileRequest.PlayFabId = playFabId;
			getPlayerProfileRequest.ProfileConstraints = new PlayerProfileViewConstraints
			{
				ShowDisplayName = true
			};
			PlayFabClientAPI.GetPlayerProfile(getPlayerProfileRequest, delegate(GetPlayerProfileResult result)
			{
				this._displayName = result.PlayerProfile.DisplayName;
			}, delegate(PlayFabError error)
			{
				Debug.LogError(error.GenerateErrorReport());
			}, null, null);
		}

		// Token: 0x060069E9 RID: 27113 RVA: 0x0022499C File Offset: 0x00222B9C
		public void SetDisplayName(string playerName)
		{
			if (this._displayName == null || (this._displayName.Length > 4 && this._displayName.Substring(0, this._displayName.Length - 4) != playerName && this._displayName != playerName))
			{
				PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
				{
					DisplayName = playerName
				}, delegate(UpdateUserTitleDisplayNameResult result)
				{
					this._displayName = playerName;
				}, delegate(PlayFabError error)
				{
					Debug.LogError("Error with name: " + playerName + ". Error is " + error.GenerateErrorReport());
				}, null, null);
			}
		}

		// Token: 0x060069EA RID: 27114 RVA: 0x00224A3C File Offset: 0x00222C3C
		public void ScreenDebug(string debugString)
		{
			Debug.Log(debugString);
			if (this.screenDebugMode)
			{
				Text text = this.debugText;
				text.text = text.text + debugString + "\n";
			}
		}

		// Token: 0x060069EB RID: 27115 RVA: 0x00224A68 File Offset: 0x00222C68
		public void ScreenDebugClear()
		{
			this.debugText.text = "";
		}

		// Token: 0x060069EC RID: 27116 RVA: 0x00224A7A File Offset: 0x00222C7A
		public IEnumerator PlayfabAuthenticate(PlayFabAuthenticator.PlayfabAuthRequestData data, Action<PlayFabAuthenticator.PlayfabAuthResponseData> callback)
		{
			UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/PlayFabAuthentication", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 30;
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
			{
				PlayFabAuthenticator.PlayfabAuthResponseData obj = JsonUtility.FromJson<PlayFabAuthenticator.PlayfabAuthResponseData>(request.downloadHandler.text);
				callback(obj);
			}
			else
			{
				if (request.responseCode == 403L)
				{
					Debug.LogError(string.Format("HTTP {0}: {1}, with body: {2}", request.responseCode, request.error, request.downloadHandler.text));
					PlayFabAuthenticator.BanInfo banInfo = JsonUtility.FromJson<PlayFabAuthenticator.BanInfo>(request.downloadHandler.text);
					this.ShowBanMessage(banInfo);
					callback(null);
				}
				if (request.result == UnityWebRequest.Result.ProtocolError && request.responseCode != 400L)
				{
					retry = true;
					Debug.LogError(string.Format("HTTP {0} error: {1} message:{2}", request.responseCode, request.error, request.downloadHandler.text));
				}
				else if (request.result == UnityWebRequest.Result.ConnectionError)
				{
					retry = true;
					Debug.LogError("NETWORK ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
				}
				else
				{
					Debug.LogError("HTTP ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
					retry = true;
				}
			}
			if (retry)
			{
				if (this.playFabAuthRetryCount < this.playFabMaxRetries)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.playFabAuthRetryCount + 1));
					Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", this.playFabAuthRetryCount + 1, num));
					this.playFabAuthRetryCount++;
					yield return new WaitForSecondsRealtime((float)num);
				}
				else
				{
					Debug.LogError("Maximum retries attempted. Please check your network connection.");
					callback(null);
					this.ShowPlayFabAuthErrorMessage(request.downloadHandler.text);
				}
			}
			yield break;
		}

		// Token: 0x060069ED RID: 27117 RVA: 0x00224A98 File Offset: 0x00222C98
		private void ShowMothershipAuthErrorMessage(string errorMessage, string errorCode, string traceId)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder("UNABLE TO AUTHENTICATE WITH MOTHERSHIP.\nREASON: " + errorMessage);
				StringBuilder stringBuilder2 = stringBuilder;
				if (!char.IsPunctuation(stringBuilder2[stringBuilder2.Length - 1]))
				{
					stringBuilder.Append('.');
				}
				if (!string.IsNullOrEmpty(errorCode))
				{
					stringBuilder.Append("\nERROR CODE: " + errorCode);
				}
				if (!string.IsNullOrEmpty(traceId))
				{
					stringBuilder.Append("\nTRACE ID: " + traceId);
				}
				this.gorillaComputer.GeneralFailureMessage(stringBuilder.ToString());
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Failed to show Mothership auth error message: {0}", arg));
			}
		}

		// Token: 0x060069EE RID: 27118 RVA: 0x00224B40 File Offset: 0x00222D40
		private void ShowPlayFabAuthErrorMessage(string errorJson)
		{
			try
			{
				PlayFabAuthenticator.ErrorInfo errorInfo = JsonUtility.FromJson<PlayFabAuthenticator.ErrorInfo>(errorJson);
				StringBuilder stringBuilder = new StringBuilder("UNABLE TO AUTHENTICATE WITH PLAYFAB.\nREASON: " + errorInfo.Message);
				StringBuilder stringBuilder2 = stringBuilder;
				if (!char.IsPunctuation(stringBuilder2[stringBuilder2.Length - 1]))
				{
					stringBuilder.Append('.');
				}
				this.gorillaComputer.GeneralFailureMessage(stringBuilder.ToString());
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Failed to show PlayFab auth error message: {0}", arg));
			}
		}

		// Token: 0x060069EF RID: 27119 RVA: 0x00224BC0 File Offset: 0x00222DC0
		private void ShowBanMessage(PlayFabAuthenticator.BanInfo banInfo)
		{
			try
			{
				if (banInfo.BanExpirationTime != null && banInfo.BanMessage != null)
				{
					if (banInfo.BanExpirationTime != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + banInfo.BanMessage + "\nHOURS LEFT: " + ((int)((DateTime.Parse(banInfo.BanExpirationTime) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
					}
					else
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + banInfo.BanMessage);
					}
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Failed to show ban message: {0}", arg));
			}
		}

		// Token: 0x060069F0 RID: 27120 RVA: 0x00224C84 File Offset: 0x00222E84
		public IEnumerator CachePlayFabId(PlayFabAuthenticator.CachePlayFabIdRequest data, Action<PlayFabAuthenticator.CachePlayFabIdResponse> callback)
		{
			Debug.Log("Trying to cache playfab Id");
			UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/CachePlayFabId", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 30;
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
			{
				if (request.responseCode == 200L)
				{
					PlayFabAuthenticator.CachePlayFabIdResponse obj = JsonUtility.FromJson<PlayFabAuthenticator.CachePlayFabIdResponse>(request.downloadHandler.text);
					callback(obj);
				}
			}
			else if (request.result == UnityWebRequest.Result.ProtocolError && request.responseCode != 400L)
			{
				retry = true;
				Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
			}
			else
			{
				retry = (request.result != UnityWebRequest.Result.ConnectionError || true);
			}
			if (retry)
			{
				if (this.playFabCacheRetryCount < this.playFabCacheMaxRetries)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.playFabCacheRetryCount + 1));
					Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", this.playFabCacheRetryCount + 1, num));
					this.playFabCacheRetryCount++;
					yield return new WaitForSecondsRealtime((float)num);
					base.StartCoroutine(this.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
					{
						Platform = this.platform.ToString(),
						SessionTicket = this._sessionTicket,
						PlayFabId = this._playFabPlayerIdCache,
						TitleId = PlayFabSettings.TitleId,
						MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
						MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
						MothershipToken = MothershipClientContext.Token,
						MothershipId = MothershipClientContext.MothershipId
					}, new Action<PlayFabAuthenticator.CachePlayFabIdResponse>(this.OnCachePlayFabIdRequest)));
				}
				else
				{
					Debug.LogError("Maximum retries attempted. Please check your network connection.");
					callback(null);
					this.ShowPlayFabAuthErrorMessage(request.downloadHandler.text);
				}
			}
			yield break;
		}

		// Token: 0x060069F1 RID: 27121 RVA: 0x00224CA1 File Offset: 0x00222EA1
		public void DefaultSafetiesByAgeCategory()
		{
			Debug.Log("[KID::PLAYFAB_AUTHENTICATOR] Defaulting Safety Settings to Disabled because age category data unavailable on this platform");
			this.SetSafety(false, true, false);
		}

		// Token: 0x060069F2 RID: 27122 RVA: 0x00224CB8 File Offset: 0x00222EB8
		public void SetSafety(bool isSafety, bool isAutoSet, bool setPlayfab = false)
		{
			this.postAuthSetSafety = false;
			Action<bool> onSafetyUpdate = this.OnSafetyUpdate;
			if (onSafetyUpdate != null)
			{
				onSafetyUpdate(isSafety);
			}
			Debug.Log("[KID] Setting safety to: [" + isSafety.ToString() + "]");
			this.isSafeAccount = isSafety;
			this.safetyType = PlayFabAuthenticator.SafetyType.None;
			if (!isSafety)
			{
				if (isAutoSet)
				{
					PlayerPrefs.SetInt("autoSafety", 0);
				}
				else
				{
					PlayerPrefs.SetInt("optSafety", 0);
				}
				PlayerPrefs.Save();
				return;
			}
			if (isAutoSet)
			{
				PlayerPrefs.SetInt("autoSafety", 1);
				this.safetyType = PlayFabAuthenticator.SafetyType.Auto;
				return;
			}
			PlayerPrefs.SetInt("optSafety", 1);
			this.safetyType = PlayFabAuthenticator.SafetyType.OptIn;
		}

		// Token: 0x060069F3 RID: 27123 RVA: 0x00224D53 File Offset: 0x00222F53
		public string GetPlayFabSessionTicket()
		{
			return this._sessionTicket;
		}

		// Token: 0x060069F4 RID: 27124 RVA: 0x00224D5B File Offset: 0x00222F5B
		public string GetPlayFabPlayerId()
		{
			return this._playFabPlayerIdCache;
		}

		// Token: 0x060069F5 RID: 27125 RVA: 0x00224D63 File Offset: 0x00222F63
		public bool GetSafety()
		{
			return this.isSafeAccount;
		}

		// Token: 0x060069F6 RID: 27126 RVA: 0x00224D6B File Offset: 0x00222F6B
		public PlayFabAuthenticator.SafetyType GetSafetyType()
		{
			return this.safetyType;
		}

		// Token: 0x060069F7 RID: 27127 RVA: 0x00224D73 File Offset: 0x00222F73
		public string GetUserID()
		{
			return this.userID;
		}

		// Token: 0x04007A18 RID: 31256
		public static volatile PlayFabAuthenticator instance;

		// Token: 0x04007A19 RID: 31257
		private const int PlayFabAuthRequestTimeout = 30;

		// Token: 0x04007A1A RID: 31258
		private string _playFabPlayerIdCache;

		// Token: 0x04007A1B RID: 31259
		private string _sessionTicket;

		// Token: 0x04007A1C RID: 31260
		private string _displayName;

		// Token: 0x04007A1D RID: 31261
		private string _nonce;

		// Token: 0x04007A1E RID: 31262
		public string userID;

		// Token: 0x04007A1F RID: 31263
		private string userToken;

		// Token: 0x04007A20 RID: 31264
		public PlatformTagJoin platform;

		// Token: 0x04007A21 RID: 31265
		private bool isSafeAccount;

		// Token: 0x04007A22 RID: 31266
		public Action<bool> OnSafetyUpdate;

		// Token: 0x04007A23 RID: 31267
		private PlayFabAuthenticator.SafetyType safetyType;

		// Token: 0x04007A24 RID: 31268
		private byte[] m_Ticket;

		// Token: 0x04007A25 RID: 31269
		private uint m_pcbTicket;

		// Token: 0x04007A26 RID: 31270
		public Text debugText;

		// Token: 0x04007A27 RID: 31271
		public bool screenDebugMode;

		// Token: 0x04007A28 RID: 31272
		public bool loginFailed;

		// Token: 0x04007A29 RID: 31273
		[FormerlySerializedAs("loginDisplayID")]
		public GameObject emptyObject;

		// Token: 0x04007A2A RID: 31274
		private int playFabAuthRetryCount;

		// Token: 0x04007A2B RID: 31275
		private int playFabMaxRetries = 5;

		// Token: 0x04007A2C RID: 31276
		private int playFabCacheRetryCount;

		// Token: 0x04007A2D RID: 31277
		private int playFabCacheMaxRetries = 5;

		// Token: 0x04007A2E RID: 31278
		public MetaAuthenticator metaAuthenticator;

		// Token: 0x04007A2F RID: 31279
		public SteamAuthenticator steamAuthenticator;

		// Token: 0x04007A30 RID: 31280
		public MothershipAuthenticator mothershipAuthenticator;

		// Token: 0x04007A31 RID: 31281
		public PhotonAuthenticator photonAuthenticator;

		// Token: 0x04007A32 RID: 31282
		[SerializeField]
		private bool dbg_isReturningPlayer;

		// Token: 0x04007A34 RID: 31284
		private SteamAuthTicket steamAuthTicketForPlayFab;

		// Token: 0x04007A35 RID: 31285
		private SteamAuthTicket steamAuthTicketForPhoton;

		// Token: 0x04007A36 RID: 31286
		private string steamAuthIdForPhoton;

		// Token: 0x0200107B RID: 4219
		public enum SafetyType
		{
			// Token: 0x04007A39 RID: 31289
			None,
			// Token: 0x04007A3A RID: 31290
			Auto,
			// Token: 0x04007A3B RID: 31291
			OptIn
		}

		// Token: 0x0200107C RID: 4220
		[Serializable]
		public class CachePlayFabIdRequest
		{
			// Token: 0x04007A3C RID: 31292
			public string Platform;

			// Token: 0x04007A3D RID: 31293
			public string SessionTicket;

			// Token: 0x04007A3E RID: 31294
			public string PlayFabId;

			// Token: 0x04007A3F RID: 31295
			public string TitleId;

			// Token: 0x04007A40 RID: 31296
			public string MothershipEnvId;

			// Token: 0x04007A41 RID: 31297
			public string MothershipDeploymentId;

			// Token: 0x04007A42 RID: 31298
			public string MothershipToken;

			// Token: 0x04007A43 RID: 31299
			public string MothershipId;
		}

		// Token: 0x0200107D RID: 4221
		[Serializable]
		public class PlayfabAuthRequestData
		{
			// Token: 0x04007A44 RID: 31300
			public string AppId;

			// Token: 0x04007A45 RID: 31301
			public string Nonce;

			// Token: 0x04007A46 RID: 31302
			public string OculusId;

			// Token: 0x04007A47 RID: 31303
			public string Platform;

			// Token: 0x04007A48 RID: 31304
			public string AgeCategory;

			// Token: 0x04007A49 RID: 31305
			public string MothershipEnvId;

			// Token: 0x04007A4A RID: 31306
			public string MothershipDeploymentId;

			// Token: 0x04007A4B RID: 31307
			public string MothershipToken;

			// Token: 0x04007A4C RID: 31308
			public string MothershipId;
		}

		// Token: 0x0200107E RID: 4222
		[Serializable]
		public class PlayfabAuthResponseData
		{
			// Token: 0x04007A4D RID: 31309
			public string SessionTicket;

			// Token: 0x04007A4E RID: 31310
			public string EntityToken;

			// Token: 0x04007A4F RID: 31311
			public string PlayFabId;

			// Token: 0x04007A50 RID: 31312
			public string EntityId;

			// Token: 0x04007A51 RID: 31313
			public string EntityType;

			// Token: 0x04007A52 RID: 31314
			public string AccountCreationIsoTimestamp;
		}

		// Token: 0x0200107F RID: 4223
		[Serializable]
		public class CachePlayFabIdResponse
		{
			// Token: 0x04007A53 RID: 31315
			public string PlayFabId;

			// Token: 0x04007A54 RID: 31316
			public string SteamAuthIdForPhoton;

			// Token: 0x04007A55 RID: 31317
			public string AccountCreationIsoTimestamp;
		}

		// Token: 0x02001080 RID: 4224
		private class ErrorInfo
		{
			// Token: 0x04007A56 RID: 31318
			public string Message;

			// Token: 0x04007A57 RID: 31319
			public string Error;
		}

		// Token: 0x02001081 RID: 4225
		private class BanInfo
		{
			// Token: 0x04007A58 RID: 31320
			public string BanMessage;

			// Token: 0x04007A59 RID: 31321
			public string BanExpirationTime;
		}
	}
}
