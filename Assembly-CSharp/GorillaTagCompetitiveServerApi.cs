using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200089C RID: 2204
public class GorillaTagCompetitiveServerApi : MonoBehaviour
{
	// Token: 0x060039B8 RID: 14776 RVA: 0x0013A6A3 File Offset: 0x001388A3
	private void Awake()
	{
		if (GorillaTagCompetitiveServerApi.Instance)
		{
			GTDev.LogError<string>("Duplicate GorillaTagCompetitiveServerApi detected. Destroying self.", base.gameObject, null);
			Object.Destroy(this);
			return;
		}
		GorillaTagCompetitiveServerApi.Instance = this;
	}

	// Token: 0x060039B9 RID: 14777 RVA: 0x0013A6D0 File Offset: 0x001388D0
	public void RequestGetRankInformation(List<string> playfabs, Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData> callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestGetRankInformation Client Not Logged into Mothership", null);
			return;
		}
		if (this.GetRankInformationInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestGetRankInformation already in progress", null);
			return;
		}
		this.GetRankInformationInProgress = true;
		string platform = "PC";
		base.StartCoroutine(this.GetRankInformation(new GorillaTagCompetitiveServerApi.RankedModeProgressionRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = platform,
			playfabIds = playfabs
		}, callback));
	}

	// Token: 0x060039BA RID: 14778 RVA: 0x0013A759 File Offset: 0x00138959
	private IEnumerator GetRankInformation(GorillaTagCompetitiveServerApi.RankedModeProgressionRequestData data, Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/GetTier", "GET");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.OnCompleteGetRankInformation(request.downloadHandler.text, callback);
		}
		else if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			retry = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_136;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_136;
			}
			bool flag = true;
			goto IL_139;
			IL_136:
			flag = false;
			IL_139:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteGetRankInformation(null, callback);
			}
		}
		if (retry)
		{
			if (this.GetRankInformationRetryCount < this.MAX_SERVER_RETRIES)
			{
				float time = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.GetRankInformationRetryCount + 1)));
				this.GetRankInformationRetryCount++;
				yield return new WaitForSecondsRealtime(time);
				this.GetRankInformationInProgress = false;
				this.RequestGetRankInformation(data.playfabIds, callback);
			}
			else
			{
				this.GetRankInformationRetryCount = 0;
				this.OnCompleteGetRankInformation(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x060039BB RID: 14779 RVA: 0x0013A778 File Offset: 0x00138978
	private void OnCompleteGetRankInformation([CanBeNull] string response, Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData> callback)
	{
		this.GetRankInformationInProgress = false;
		this.GetRankInformationRetryCount = 0;
		if (response.IsNullOrEmpty())
		{
			return;
		}
		string text = "{ \"playerData\": " + response + " }";
		GorillaTagCompetitiveServerApi.RankedModeProgressionData obj;
		try
		{
			obj = JsonUtility.FromJson<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(text);
		}
		catch (ArgumentException exception)
		{
			Debug.LogException(exception);
			Debug.LogError("[GT/GorillaTagCompetitiveServerApi]  ERROR!!!  OnCompleteGetRankInformation: Encountered ArgumentException above while trying to parse json string:\n" + text);
			return;
		}
		catch (Exception exception2)
		{
			Debug.LogException(exception2);
			Debug.LogError("[GT/GorillaTagCompetitiveServerApi]  ERROR!!!  OnCompleteGetRankInformation: Encountered exception above while trying to parse json string:\n" + text);
			return;
		}
		if (callback != null)
		{
			callback(obj);
		}
	}

	// Token: 0x060039BC RID: 14780 RVA: 0x0013A80C File Offset: 0x00138A0C
	public void RequestCreateMatchId(Action<string> callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestCreateMatchId Client Not Logged into Mothership", null);
			return;
		}
		if (this.CreateMatchIdInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestCreateMatchId already in progress", null);
			return;
		}
		string platform = "PC";
		this.CreateMatchIdInProgress = true;
		base.StartCoroutine(this.CreateMatchId(new GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = platform
		}, callback));
	}

	// Token: 0x060039BD RID: 14781 RVA: 0x0013A88E File Offset: 0x00138A8E
	private IEnumerator CreateMatchId(GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed data, Action<string> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/CreateMatchId", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("CreateMatchId Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteCreateMatchId(request.downloadHandler.text, callback);
		}
		else if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			retry = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_156;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_156;
			}
			bool flag = true;
			goto IL_159;
			IL_156:
			flag = false;
			IL_159:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteCreateMatchId(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.CreateMatchIdRetryCount < this.MAX_SERVER_RETRIES)
			{
				float time = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.CreateMatchIdRetryCount + 1)));
				this.CreateMatchIdRetryCount++;
				yield return new WaitForSecondsRealtime(time);
				this.CreateMatchIdInProgress = false;
				this.RequestCreateMatchId(callback);
			}
			else
			{
				this.CreateMatchIdRetryCount = 0;
				this.OnCompleteCreateMatchId(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x060039BE RID: 14782 RVA: 0x0013A8AB File Offset: 0x00138AAB
	private void OnCompleteCreateMatchId([CanBeNull] string response, Action<string> callback)
	{
		this.CreateMatchIdInProgress = false;
		this.CreateMatchIdRetryCount = 0;
		if (response.IsNullOrEmpty())
		{
			return;
		}
		if (callback != null)
		{
			callback(response);
		}
	}

	// Token: 0x060039BF RID: 14783 RVA: 0x0013A8D0 File Offset: 0x00138AD0
	public void RequestValidateMatchJoin(string matchId, Action<bool> callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestValidateMatchJoin Client Not Logged into Mothership", null);
			return;
		}
		if (this.ValidateMatchJoinInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestValidateMatchJoin already in progress", null);
			return;
		}
		string platform = "PC";
		this.ValidateMatchJoinInProgress = true;
		base.StartCoroutine(this.ValidateMatchJoin(new GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = platform,
			matchId = matchId
		}, callback));
	}

	// Token: 0x060039C0 RID: 14784 RVA: 0x0013A959 File Offset: 0x00138B59
	private IEnumerator ValidateMatchJoin(GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/ValidateMatchJoin", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("ValidateMatchJoin Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteValidateMatchJoin(request.downloadHandler.text, callback);
		}
		else if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			retry = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_156;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_156;
			}
			bool flag = true;
			goto IL_159;
			IL_156:
			flag = false;
			IL_159:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteValidateMatchJoin(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.ValidateMatchJoinRetryCount < this.MAX_SERVER_RETRIES)
			{
				float time = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.ValidateMatchJoinRetryCount + 1)));
				this.ValidateMatchJoinRetryCount++;
				yield return new WaitForSecondsRealtime(time);
				this.ValidateMatchJoinInProgress = false;
				this.RequestValidateMatchJoin(data.matchId, callback);
			}
			else
			{
				this.ValidateMatchJoinRetryCount = 0;
				this.OnCompleteValidateMatchJoin(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x060039C1 RID: 14785 RVA: 0x0013A978 File Offset: 0x00138B78
	private void OnCompleteValidateMatchJoin([CanBeNull] string response, Action<bool> callback)
	{
		this.ValidateMatchJoinInProgress = false;
		this.ValidateMatchJoinRetryCount = 0;
		if (response.IsNullOrEmpty())
		{
			return;
		}
		GorillaTagCompetitiveServerApi.RankedModeValidateMatchJoinResponseData rankedModeValidateMatchJoinResponseData = JsonUtility.FromJson<GorillaTagCompetitiveServerApi.RankedModeValidateMatchJoinResponseData>(response);
		if (callback != null)
		{
			callback(rankedModeValidateMatchJoinResponseData.validJoin);
		}
	}

	// Token: 0x060039C2 RID: 14786 RVA: 0x0013A9B4 File Offset: 0x00138BB4
	public void RequestSubmitMatchScores(string matchId, List<RankedMultiplayerScore.PlayerScore> finalScores)
	{
		List<GorillaTagCompetitiveServerApi.RankedModePlayerScore> list = new List<GorillaTagCompetitiveServerApi.RankedModePlayerScore>();
		foreach (RankedMultiplayerScore.PlayerScore playerScore in finalScores)
		{
			NetPlayer player = NetworkSystem.Instance.GetPlayer(playerScore.PlayerId);
			list.Add(new GorillaTagCompetitiveServerApi.RankedModePlayerScore
			{
				playfabId = player.UserId,
				gameScore = playerScore.GameScore
			});
		}
		this.RequestSubmitMatchScores(matchId, list);
	}

	// Token: 0x060039C3 RID: 14787 RVA: 0x0013AA40 File Offset: 0x00138C40
	private void RequestSubmitMatchScores(string matchId, List<GorillaTagCompetitiveServerApi.RankedModePlayerScore> playerScores)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSubmitMatchScores Client Not Logged into Mothership", null);
			return;
		}
		if (this.SubmitMatchScoresInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSubmitMatchScores already in progress", null);
			return;
		}
		this.SubmitMatchScoresInProgress = true;
		base.StartCoroutine(this.SubmitMatchScores(new GorillaTagCompetitiveServerApi.RankedModeSubmitMatchScoresRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			matchId = matchId,
			playfabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			playerScores = playerScores
		}));
	}

	// Token: 0x060039C4 RID: 14788 RVA: 0x0013AACE File Offset: 0x00138CCE
	private IEnumerator SubmitMatchScores(GorillaTagCompetitiveServerApi.RankedModeSubmitMatchScoresRequestData data)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/SubmitMatchScores", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("SubmitMatchScores Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteSubmitMatchScores(request.downloadHandler.text);
		}
		else if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			retry = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_150;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_150;
			}
			bool flag = true;
			goto IL_153;
			IL_150:
			flag = false;
			IL_153:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteSubmitMatchScores(request.downloadHandler.text);
			}
		}
		if (retry)
		{
			if (this.SubmitMatchScoresRetryCount < this.MAX_SERVER_RETRIES)
			{
				float time = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.SubmitMatchScoresRetryCount + 1)));
				this.SubmitMatchScoresRetryCount++;
				yield return new WaitForSecondsRealtime(time);
				this.SubmitMatchScoresInProgress = false;
				this.RequestSubmitMatchScores(data.matchId, data.playerScores);
			}
			else
			{
				this.SubmitMatchScoresRetryCount = 0;
				this.OnCompleteSubmitMatchScores(null);
			}
		}
		yield break;
	}

	// Token: 0x060039C5 RID: 14789 RVA: 0x0013AAE4 File Offset: 0x00138CE4
	private void OnCompleteSubmitMatchScores([CanBeNull] string response)
	{
		this.SubmitMatchScoresInProgress = false;
		this.SubmitMatchScoresRetryCount = 0;
	}

	// Token: 0x060039C6 RID: 14790 RVA: 0x0013AAF4 File Offset: 0x00138CF4
	public void RequestSetEloValue(float desiredElo, Action callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSetEloValue Client Not Logged into Mothership", null);
			return;
		}
		if (this.SetEloValueInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSetEloValue already in progress", null);
			return;
		}
		string platform = "PC";
		this.SetEloValueInProgress = true;
		base.StartCoroutine(this.SetEloValue(new GorillaTagCompetitiveServerApi.RankedModeSetEloValueRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = platform,
			elo = desiredElo
		}, callback));
	}

	// Token: 0x060039C7 RID: 14791 RVA: 0x0013AB7D File Offset: 0x00138D7D
	private IEnumerator SetEloValue(GorillaTagCompetitiveServerApi.RankedModeSetEloValueRequestData data, Action callback)
	{
		GTDev.LogWarning<string>("SetEloValue is for internal use only (Is Beta)", null);
		yield break;
	}

	// Token: 0x060039C8 RID: 14792 RVA: 0x0013AB85 File Offset: 0x00138D85
	private void OnCompleteSetEloValue([CanBeNull] string response, Action callback)
	{
		this.SetEloValueInProgress = false;
		this.SetEloValueRetryCount = 0;
		if (response != null && callback != null)
		{
			callback();
		}
	}

	// Token: 0x060039C9 RID: 14793 RVA: 0x0013ABA4 File Offset: 0x00138DA4
	public void RequestPingRoom(string matchId, Action callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestPingRoom Client Not Logged into Mothership", null);
			return;
		}
		if (this.SetEloValueInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestPingRoom already in progress", null);
			return;
		}
		string platform = "PC";
		this.PingMatchInProgress = true;
		base.StartCoroutine(this.PingRoom(new GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = platform,
			matchId = matchId
		}, callback));
	}

	// Token: 0x060039CA RID: 14794 RVA: 0x0013AC2D File Offset: 0x00138E2D
	private IEnumerator PingRoom(GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId data, Action callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/PingRoom", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("PingRoom Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompletePingRoom(request.downloadHandler.text, callback);
		}
		else if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			retry = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_156;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_156;
			}
			bool flag = true;
			goto IL_159;
			IL_156:
			flag = false;
			IL_159:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompletePingRoom(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.PingMatchRetryCount < this.MAX_SERVER_RETRIES)
			{
				float time = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.PingMatchRetryCount + 1)));
				this.ValidateMatchJoinRetryCount++;
				yield return new WaitForSecondsRealtime(time);
				this.PingMatchInProgress = false;
				this.RequestPingRoom(data.matchId, callback);
			}
			else
			{
				this.PingMatchRetryCount = 0;
				this.OnCompletePingRoom(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x060039CB RID: 14795 RVA: 0x0013AC4A File Offset: 0x00138E4A
	private void OnCompletePingRoom([CanBeNull] string response, Action callback)
	{
		GTDev.Log<string>("PingRoom complete", null);
		this.PingMatchInProgress = false;
		this.PingMatchRetryCount = 0;
		if (response != null && callback != null)
		{
			callback();
		}
	}

	// Token: 0x060039CC RID: 14796 RVA: 0x0013AC74 File Offset: 0x00138E74
	public void RequestUnlockCompetitiveQueue(bool unlocked, Action callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestUnlockCompetitiveQueue Client Not Logged into Mothership", null);
			return;
		}
		if (this.UnlockCompetitiveQueueInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestUnlockCompetitiveQueue already in progress", null);
			return;
		}
		string platform = "PC";
		this.UnlockCompetitiveQueueInProgress = true;
		base.StartCoroutine(this.UnlockCompetitiveQueue(new GorillaTagCompetitiveServerApi.RankedModeUnlockCompetitiveQueueRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = platform,
			unlocked = unlocked
		}, callback));
	}

	// Token: 0x060039CD RID: 14797 RVA: 0x0013ACFD File Offset: 0x00138EFD
	private IEnumerator UnlockCompetitiveQueue(GorillaTagCompetitiveServerApi.RankedModeUnlockCompetitiveQueueRequestData data, Action callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/UnlockCompetitiveQueue", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("UnlockCompetitiveQueue Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteUnlockCompetitiveQueue(request.downloadHandler.text, callback);
		}
		else if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			retry = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_156;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_156;
			}
			bool flag = true;
			goto IL_159;
			IL_156:
			flag = false;
			IL_159:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteUnlockCompetitiveQueue(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.UnlockCompetitiveQueueRetryCount < this.MAX_SERVER_RETRIES)
			{
				float time = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.UnlockCompetitiveQueueRetryCount + 1)));
				this.ValidateMatchJoinRetryCount++;
				yield return new WaitForSecondsRealtime(time);
				this.UnlockCompetitiveQueueInProgress = false;
				this.RequestUnlockCompetitiveQueue(data.unlocked, callback);
			}
			else
			{
				this.UnlockCompetitiveQueueRetryCount = 0;
				this.OnCompleteUnlockCompetitiveQueue(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x060039CE RID: 14798 RVA: 0x0013AD1A File Offset: 0x00138F1A
	private void OnCompleteUnlockCompetitiveQueue([CanBeNull] string response, Action callback)
	{
		GTDev.Log<string>("UnlockCompetitiveQueue complete", null);
		this.UnlockCompetitiveQueueInProgress = false;
		this.UnlockCompetitiveQueueRetryCount = 0;
		if (response != null && callback != null)
		{
			callback();
		}
	}

	// Token: 0x0400499E RID: 18846
	public static GorillaTagCompetitiveServerApi Instance;

	// Token: 0x0400499F RID: 18847
	public int MAX_SERVER_RETRIES = 3;

	// Token: 0x040049A0 RID: 18848
	private bool GetRankInformationInProgress;

	// Token: 0x040049A1 RID: 18849
	private int GetRankInformationRetryCount;

	// Token: 0x040049A2 RID: 18850
	private bool CreateMatchIdInProgress;

	// Token: 0x040049A3 RID: 18851
	private int CreateMatchIdRetryCount;

	// Token: 0x040049A4 RID: 18852
	private bool ValidateMatchJoinInProgress;

	// Token: 0x040049A5 RID: 18853
	private int ValidateMatchJoinRetryCount;

	// Token: 0x040049A6 RID: 18854
	private bool SubmitMatchScoresInProgress;

	// Token: 0x040049A7 RID: 18855
	private int SubmitMatchScoresRetryCount;

	// Token: 0x040049A8 RID: 18856
	private bool SetEloValueInProgress;

	// Token: 0x040049A9 RID: 18857
	private int SetEloValueRetryCount;

	// Token: 0x040049AA RID: 18858
	private bool PingMatchInProgress;

	// Token: 0x040049AB RID: 18859
	private int PingMatchRetryCount;

	// Token: 0x040049AC RID: 18860
	private bool UnlockCompetitiveQueueInProgress;

	// Token: 0x040049AD RID: 18861
	private int UnlockCompetitiveQueueRetryCount;

	// Token: 0x0200089D RID: 2205
	public enum EPlatformType
	{
		// Token: 0x040049AF RID: 18863
		PC,
		// Token: 0x040049B0 RID: 18864
		Quest,
		// Token: 0x040049B1 RID: 18865
		NumPlatforms
	}

	// Token: 0x0200089E RID: 2206
	[Serializable]
	public class RankedModeRequestDataBase
	{
		// Token: 0x040049B2 RID: 18866
		public string mothershipId;

		// Token: 0x040049B3 RID: 18867
		public string mothershipToken;

		// Token: 0x040049B4 RID: 18868
		public string mothershipEnvId;
	}

	// Token: 0x0200089F RID: 2207
	[Serializable]
	public class RankedModeRequestDataPlatformed : GorillaTagCompetitiveServerApi.RankedModeRequestDataBase
	{
		// Token: 0x040049B5 RID: 18869
		public string platform;
	}

	// Token: 0x020008A0 RID: 2208
	[Serializable]
	public class RankedModeProgressionRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x040049B6 RID: 18870
		public List<string> playfabIds;
	}

	// Token: 0x020008A1 RID: 2209
	[Serializable]
	public class RankedModeProgressionPlatformData
	{
		// Token: 0x040049B7 RID: 18871
		public string platform;

		// Token: 0x040049B8 RID: 18872
		public float elo;

		// Token: 0x040049B9 RID: 18873
		public int majorTier;

		// Token: 0x040049BA RID: 18874
		public int minorTier;

		// Token: 0x040049BB RID: 18875
		public float rankProgress;
	}

	// Token: 0x020008A2 RID: 2210
	[Serializable]
	public class RankedModePlayerProgressionData
	{
		// Token: 0x040049BC RID: 18876
		public string playfabID;

		// Token: 0x040049BD RID: 18877
		public GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData[] platformData = new GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData[2];
	}

	// Token: 0x020008A3 RID: 2211
	[Serializable]
	public class RankedModeProgressionData
	{
		// Token: 0x040049BE RID: 18878
		public List<GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData> playerData;
	}

	// Token: 0x020008A4 RID: 2212
	[Serializable]
	public class RankedModeRequestDataWithMatchId : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x040049BF RID: 18879
		public string matchId;
	}

	// Token: 0x020008A5 RID: 2213
	[Serializable]
	public class RankedModeValidateMatchJoinResponseData
	{
		// Token: 0x040049C0 RID: 18880
		public bool validJoin;
	}

	// Token: 0x020008A6 RID: 2214
	[Serializable]
	public class RankedModePlayerScore
	{
		// Token: 0x040049C1 RID: 18881
		public string playfabId;

		// Token: 0x040049C2 RID: 18882
		public float gameScore;
	}

	// Token: 0x020008A7 RID: 2215
	[Serializable]
	public class RankedModeSubmitMatchScoresRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataBase
	{
		// Token: 0x040049C3 RID: 18883
		public string matchId;

		// Token: 0x040049C4 RID: 18884
		public string playfabId;

		// Token: 0x040049C5 RID: 18885
		public List<GorillaTagCompetitiveServerApi.RankedModePlayerScore> playerScores;
	}

	// Token: 0x020008A8 RID: 2216
	[Serializable]
	public class RankedModeSetEloValueRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x040049C6 RID: 18886
		public float elo;
	}

	// Token: 0x020008A9 RID: 2217
	[Serializable]
	public class RankedModeUnlockCompetitiveQueueRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x040049C7 RID: 18887
		public bool unlocked;
	}
}
