using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000235 RID: 565
public class MonkeVoteController : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x17000179 RID: 377
	// (get) Token: 0x06000F08 RID: 3848 RVA: 0x00052456 File Offset: 0x00050656
	// (set) Token: 0x06000F09 RID: 3849 RVA: 0x0005245D File Offset: 0x0005065D
	public static MonkeVoteController instance { get; private set; }

	// Token: 0x1400001C RID: 28
	// (add) Token: 0x06000F0A RID: 3850 RVA: 0x00052468 File Offset: 0x00050668
	// (remove) Token: 0x06000F0B RID: 3851 RVA: 0x000524A0 File Offset: 0x000506A0
	public event Action OnPollsUpdated;

	// Token: 0x1400001D RID: 29
	// (add) Token: 0x06000F0C RID: 3852 RVA: 0x000524D8 File Offset: 0x000506D8
	// (remove) Token: 0x06000F0D RID: 3853 RVA: 0x00052510 File Offset: 0x00050710
	public event Action OnVoteAccepted;

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x06000F0E RID: 3854 RVA: 0x00052548 File Offset: 0x00050748
	// (remove) Token: 0x06000F0F RID: 3855 RVA: 0x00052580 File Offset: 0x00050780
	public event Action OnVoteFailed;

	// Token: 0x1400001F RID: 31
	// (add) Token: 0x06000F10 RID: 3856 RVA: 0x000525B8 File Offset: 0x000507B8
	// (remove) Token: 0x06000F11 RID: 3857 RVA: 0x000525F0 File Offset: 0x000507F0
	public event Action OnCurrentPollEnded;

	// Token: 0x06000F12 RID: 3858 RVA: 0x00052625 File Offset: 0x00050825
	public void Awake()
	{
		if (MonkeVoteController.instance == null)
		{
			MonkeVoteController.instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06000F13 RID: 3859 RVA: 0x00052644 File Offset: 0x00050844
	public void SliceUpdate()
	{
		if (this.isCurrentPollActive && !this.hasCurrentPollCompleted && this.currentPollCompletionTime < DateTime.UtcNow)
		{
			GTDev.Log<string>("Active vote poll completed.", null);
			this.hasCurrentPollCompleted = true;
			Action onCurrentPollEnded = this.OnCurrentPollEnded;
			if (onCurrentPollEnded == null)
			{
				return;
			}
			onCurrentPollEnded();
		}
	}

	// Token: 0x06000F14 RID: 3860 RVA: 0x00018E08 File Offset: 0x00017008
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000F15 RID: 3861 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x00052698 File Offset: 0x00050898
	public void RequestPolls()
	{
		MonkeVoteController.<RequestPolls>d__34 <RequestPolls>d__;
		<RequestPolls>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RequestPolls>d__.<>4__this = this;
		<RequestPolls>d__.<>1__state = -1;
		<RequestPolls>d__.<>t__builder.Start<MonkeVoteController.<RequestPolls>d__34>(ref <RequestPolls>d__);
	}

	// Token: 0x06000F17 RID: 3863 RVA: 0x000526D0 File Offset: 0x000508D0
	private Task WaitForSessionToken()
	{
		MonkeVoteController.<WaitForSessionToken>d__35 <WaitForSessionToken>d__;
		<WaitForSessionToken>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForSessionToken>d__.<>1__state = -1;
		<WaitForSessionToken>d__.<>t__builder.Start<MonkeVoteController.<WaitForSessionToken>d__35>(ref <WaitForSessionToken>d__);
		return <WaitForSessionToken>d__.<>t__builder.Task;
	}

	// Token: 0x06000F18 RID: 3864 RVA: 0x0005270C File Offset: 0x0005090C
	private void FetchPolls()
	{
		base.StartCoroutine(this.DoFetchPolls(new MonkeVoteController.FetchPollsRequest
		{
			TitleId = PlayFabAuthenticatorSettings.TitleId,
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			IncludeInactive = this.includeInactive
		}, new Action<List<MonkeVoteController.FetchPollsResponse>>(this.OnFetchPollsResponse)));
	}

	// Token: 0x06000F19 RID: 3865 RVA: 0x00052772 File Offset: 0x00050972
	private IEnumerator DoFetchPolls(MonkeVoteController.FetchPollsRequest data, Action<List<MonkeVoteController.FetchPollsResponse>> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.VotingApiBaseUrl + "/api/FetchPoll", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			List<MonkeVoteController.FetchPollsResponse> obj = JsonConvert.DeserializeObject<List<MonkeVoteController.FetchPollsResponse>>(request.downloadHandler.text);
			callback(obj);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this.fetchPollsRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.fetchPollsRetryCount + 1));
				this.fetchPollsRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.FetchPolls();
			}
			else
			{
				GTDev.LogError<string>("Maximum FetchPolls retries attempted. Please check your network connection.", null);
				this.fetchPollsRetryCount = 0;
				callback(null);
			}
		}
		yield break;
	}

	// Token: 0x06000F1A RID: 3866 RVA: 0x00052790 File Offset: 0x00050990
	private void OnFetchPollsResponse([CanBeNull] List<MonkeVoteController.FetchPollsResponse> response)
	{
		this.isFetchingPoll = false;
		this.hasPoll = false;
		this.lastPollData = null;
		this.currentPollData = null;
		this.isCurrentPollActive = false;
		this.hasCurrentPollCompleted = false;
		if (response != null)
		{
			DateTime minValue = DateTime.MinValue;
			using (List<MonkeVoteController.FetchPollsResponse>.Enumerator enumerator = response.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MonkeVoteController.FetchPollsResponse fetchPollsResponse = enumerator.Current;
					if (fetchPollsResponse.isActive)
					{
						this.hasPoll = true;
						this.currentPollData = fetchPollsResponse;
						if (this.currentPollData.EndTime > DateTime.UtcNow)
						{
							this.isCurrentPollActive = true;
							this.hasCurrentPollCompleted = false;
							this.currentPollCompletionTime = this.currentPollData.EndTime;
							this.currentPollCompletionTime = this.currentPollCompletionTime.AddMinutes(1.0);
						}
					}
					if (!fetchPollsResponse.isActive && fetchPollsResponse.EndTime > minValue && fetchPollsResponse.EndTime < DateTime.UtcNow)
					{
						this.lastPollData = fetchPollsResponse;
					}
				}
				goto IL_106;
			}
		}
		GTDev.LogError<string>("Error: Could not fetch polls!", null);
		IL_106:
		Action onPollsUpdated = this.OnPollsUpdated;
		if (onPollsUpdated == null)
		{
			return;
		}
		onPollsUpdated();
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x000528C4 File Offset: 0x00050AC4
	public void Vote(int pollId, int option, bool isPrediction)
	{
		if (!this.hasPoll)
		{
			return;
		}
		if (this.isSendingVote)
		{
			return;
		}
		this.isSendingVote = true;
		this.pollId = pollId;
		this.option = option;
		this.isPrediction = isPrediction;
		this.SendVote();
	}

	// Token: 0x06000F1C RID: 3868 RVA: 0x000528FA File Offset: 0x00050AFA
	private void SendVote()
	{
		this.GetNonceForVotingCallback(null);
	}

	// Token: 0x06000F1D RID: 3869 RVA: 0x00052904 File Offset: 0x00050B04
	private void GetNonceForVotingCallback([CanBeNull] Message<UserProof> message)
	{
		if (message != null)
		{
			UserProof data = message.Data;
			this.Nonce = ((data != null) ? data.Value : null);
		}
		base.StartCoroutine(this.DoVote(new MonkeVoteController.VoteRequest
		{
			PollId = this.pollId,
			TitleId = PlayFabAuthenticatorSettings.TitleId,
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			OculusId = PlayFabAuthenticator.instance.userID,
			UserPlatform = PlayFabAuthenticator.instance.platform.ToString(),
			UserNonce = this.Nonce,
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			OptionIndex = this.option,
			IsPrediction = this.isPrediction
		}, new Action<MonkeVoteController.VoteResponse>(this.OnVoteSuccess)));
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x000529D2 File Offset: 0x00050BD2
	private IEnumerator DoVote(MonkeVoteController.VoteRequest data, Action<MonkeVoteController.VoteResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.VotingApiBaseUrl + "/api/Vote", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			MonkeVoteController.VoteResponse obj = JsonConvert.DeserializeObject<MonkeVoteController.VoteResponse>(request.downloadHandler.text);
			callback(obj);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.responseCode == 429L)
			{
				GTDev.LogWarning<string>("User already voted on this poll!", null);
				callback(null);
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this.voteRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.voteRetryCount + 1));
				this.voteRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.SendVote();
			}
			else
			{
				GTDev.LogError<string>("Maximum Vote retries attempted. Please check your network connection.", null);
				this.voteRetryCount = 0;
				callback(null);
			}
		}
		else
		{
			this.isSendingVote = false;
		}
		yield break;
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x000529EF File Offset: 0x00050BEF
	private void OnVoteSuccess([CanBeNull] MonkeVoteController.VoteResponse response)
	{
		this.isSendingVote = false;
		if (response != null)
		{
			this.lastVoteData = response;
			Action onVoteAccepted = this.OnVoteAccepted;
			if (onVoteAccepted == null)
			{
				return;
			}
			onVoteAccepted();
			return;
		}
		else
		{
			Action onVoteFailed = this.OnVoteFailed;
			if (onVoteFailed == null)
			{
				return;
			}
			onVoteFailed();
			return;
		}
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x00052A23 File Offset: 0x00050C23
	public MonkeVoteController.FetchPollsResponse GetLastPollData()
	{
		return this.lastPollData;
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x00052A2B File Offset: 0x00050C2B
	public MonkeVoteController.FetchPollsResponse GetCurrentPollData()
	{
		return this.currentPollData;
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x00052A33 File Offset: 0x00050C33
	public MonkeVoteController.VoteResponse GetVoteData()
	{
		return this.lastVoteData;
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x00052A3B File Offset: 0x00050C3B
	public int GetLastVotePollId()
	{
		return this.pollId;
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x00052A43 File Offset: 0x00050C43
	public int GetLastVoteSelectedOption()
	{
		return this.option;
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x00052A4B File Offset: 0x00050C4B
	public bool GetLastVoteWasPrediction()
	{
		return this.isPrediction;
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x00052A53 File Offset: 0x00050C53
	public DateTime GetCurrentPollCompletionTime()
	{
		return this.currentPollCompletionTime;
	}

	// Token: 0x0400122D RID: 4653
	private string Nonce = "";

	// Token: 0x0400122E RID: 4654
	private bool includeInactive = true;

	// Token: 0x0400122F RID: 4655
	private int fetchPollsRetryCount;

	// Token: 0x04001230 RID: 4656
	private int maxRetriesOnFail = 3;

	// Token: 0x04001231 RID: 4657
	private int voteRetryCount;

	// Token: 0x04001236 RID: 4662
	private MonkeVoteController.FetchPollsResponse lastPollData;

	// Token: 0x04001237 RID: 4663
	private MonkeVoteController.FetchPollsResponse currentPollData;

	// Token: 0x04001238 RID: 4664
	private MonkeVoteController.VoteResponse lastVoteData;

	// Token: 0x04001239 RID: 4665
	private bool isFetchingPoll;

	// Token: 0x0400123A RID: 4666
	private bool hasPoll;

	// Token: 0x0400123B RID: 4667
	private bool isCurrentPollActive;

	// Token: 0x0400123C RID: 4668
	private bool hasCurrentPollCompleted;

	// Token: 0x0400123D RID: 4669
	private DateTime currentPollCompletionTime;

	// Token: 0x0400123E RID: 4670
	private bool isSendingVote;

	// Token: 0x0400123F RID: 4671
	private int pollId = -1;

	// Token: 0x04001240 RID: 4672
	private int option;

	// Token: 0x04001241 RID: 4673
	private bool isPrediction;

	// Token: 0x02000236 RID: 566
	[Serializable]
	private class FetchPollsRequest
	{
		// Token: 0x04001242 RID: 4674
		public string TitleId;

		// Token: 0x04001243 RID: 4675
		public string PlayFabId;

		// Token: 0x04001244 RID: 4676
		public string PlayFabTicket;

		// Token: 0x04001245 RID: 4677
		public bool IncludeInactive;
	}

	// Token: 0x02000237 RID: 567
	[Serializable]
	public class FetchPollsResponse
	{
		// Token: 0x04001246 RID: 4678
		public int PollId;

		// Token: 0x04001247 RID: 4679
		public string Question;

		// Token: 0x04001248 RID: 4680
		public List<string> VoteOptions;

		// Token: 0x04001249 RID: 4681
		public List<int> VoteCount;

		// Token: 0x0400124A RID: 4682
		public List<int> PredictionCount;

		// Token: 0x0400124B RID: 4683
		public DateTime StartTime;

		// Token: 0x0400124C RID: 4684
		public DateTime EndTime;

		// Token: 0x0400124D RID: 4685
		public bool isActive;
	}

	// Token: 0x02000238 RID: 568
	[Serializable]
	private class VoteRequest
	{
		// Token: 0x0400124E RID: 4686
		public int PollId;

		// Token: 0x0400124F RID: 4687
		public string TitleId;

		// Token: 0x04001250 RID: 4688
		public string PlayFabId;

		// Token: 0x04001251 RID: 4689
		public string OculusId;

		// Token: 0x04001252 RID: 4690
		public string UserNonce;

		// Token: 0x04001253 RID: 4691
		public string UserPlatform;

		// Token: 0x04001254 RID: 4692
		public int OptionIndex;

		// Token: 0x04001255 RID: 4693
		public bool IsPrediction;

		// Token: 0x04001256 RID: 4694
		public string PlayFabTicket;
	}

	// Token: 0x02000239 RID: 569
	[Serializable]
	public class VoteResponse
	{
		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000F2B RID: 3883 RVA: 0x00052A83 File Offset: 0x00050C83
		// (set) Token: 0x06000F2C RID: 3884 RVA: 0x00052A8B File Offset: 0x00050C8B
		public int PollId { get; set; }

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000F2D RID: 3885 RVA: 0x00052A94 File Offset: 0x00050C94
		// (set) Token: 0x06000F2E RID: 3886 RVA: 0x00052A9C File Offset: 0x00050C9C
		public string TitleId { get; set; }

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06000F2F RID: 3887 RVA: 0x00052AA5 File Offset: 0x00050CA5
		// (set) Token: 0x06000F30 RID: 3888 RVA: 0x00052AAD File Offset: 0x00050CAD
		public List<string> VoteOptions { get; set; }

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000F31 RID: 3889 RVA: 0x00052AB6 File Offset: 0x00050CB6
		// (set) Token: 0x06000F32 RID: 3890 RVA: 0x00052ABE File Offset: 0x00050CBE
		public List<int> VoteCount { get; set; }

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000F33 RID: 3891 RVA: 0x00052AC7 File Offset: 0x00050CC7
		// (set) Token: 0x06000F34 RID: 3892 RVA: 0x00052ACF File Offset: 0x00050CCF
		public List<int> PredictionCount { get; set; }
	}
}
