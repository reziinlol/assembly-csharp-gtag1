using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000C5F RID: 3167
public class FriendBackendController : MonoBehaviour
{
	// Token: 0x14000086 RID: 134
	// (add) Token: 0x06004E3A RID: 20026 RVA: 0x0019FD88 File Offset: 0x0019DF88
	// (remove) Token: 0x06004E3B RID: 20027 RVA: 0x0019FDC0 File Offset: 0x0019DFC0
	public event Action<bool> OnGetFriendsComplete;

	// Token: 0x14000087 RID: 135
	// (add) Token: 0x06004E3C RID: 20028 RVA: 0x0019FDF8 File Offset: 0x0019DFF8
	// (remove) Token: 0x06004E3D RID: 20029 RVA: 0x0019FE30 File Offset: 0x0019E030
	public event Action<bool> OnSetPrivacyStateComplete;

	// Token: 0x14000088 RID: 136
	// (add) Token: 0x06004E3E RID: 20030 RVA: 0x0019FE68 File Offset: 0x0019E068
	// (remove) Token: 0x06004E3F RID: 20031 RVA: 0x0019FEA0 File Offset: 0x0019E0A0
	public event Action<NetPlayer, bool> OnAddFriendComplete;

	// Token: 0x14000089 RID: 137
	// (add) Token: 0x06004E40 RID: 20032 RVA: 0x0019FED8 File Offset: 0x0019E0D8
	// (remove) Token: 0x06004E41 RID: 20033 RVA: 0x0019FF10 File Offset: 0x0019E110
	public event Action<FriendBackendController.Friend, bool> OnRemoveFriendComplete;

	// Token: 0x1700072C RID: 1836
	// (get) Token: 0x06004E42 RID: 20034 RVA: 0x0019FF45 File Offset: 0x0019E145
	public List<FriendBackendController.Friend> FriendsList
	{
		get
		{
			return this.lastFriendsList;
		}
	}

	// Token: 0x1700072D RID: 1837
	// (get) Token: 0x06004E43 RID: 20035 RVA: 0x0019FF4D File Offset: 0x0019E14D
	public FriendBackendController.PrivacyState MyPrivacyState
	{
		get
		{
			return this.lastPrivacyState;
		}
	}

	// Token: 0x06004E44 RID: 20036 RVA: 0x0019FF55 File Offset: 0x0019E155
	public void GetFriends()
	{
		if (!this.getFriendsInProgress)
		{
			this.getFriendsInProgress = true;
			this.GetFriendsInternal();
		}
	}

	// Token: 0x06004E45 RID: 20037 RVA: 0x0019FF6C File Offset: 0x0019E16C
	public void SetPrivacyState(FriendBackendController.PrivacyState state)
	{
		if (!this.setPrivacyStateInProgress)
		{
			this.setPrivacyStateInProgress = true;
			this.setPrivacyStateState = state;
			this.SetPrivacyStateInternal();
			return;
		}
		this.setPrivacyStateQueue.Enqueue(state);
	}

	// Token: 0x06004E46 RID: 20038 RVA: 0x0019FF98 File Offset: 0x0019E198
	public void AddFriend(NetPlayer target)
	{
		if (target == null)
		{
			return;
		}
		int hashCode = target.UserId.GetHashCode();
		if (!this.addFriendInProgress)
		{
			this.addFriendInProgress = true;
			this.addFriendTargetIdHash = hashCode;
			this.addFriendTargetPlayer = target;
			this.AddFriendInternal();
			return;
		}
		if (hashCode != this.addFriendTargetIdHash && !this.addFriendRequestQueue.Contains(new ValueTuple<int, NetPlayer>(hashCode, target)))
		{
			this.addFriendRequestQueue.Enqueue(new ValueTuple<int, NetPlayer>(hashCode, target));
		}
	}

	// Token: 0x06004E47 RID: 20039 RVA: 0x001A0008 File Offset: 0x0019E208
	public void RemoveFriend(FriendBackendController.Friend target)
	{
		if (target == null)
		{
			return;
		}
		int hashCode = target.Presence.FriendLinkId.GetHashCode();
		if (!this.removeFriendInProgress)
		{
			this.removeFriendInProgress = true;
			this.removeFriendTargetIdHash = hashCode;
			this.removeFriendTarget = target;
			this.RemoveFriendInternal();
			return;
		}
		if (hashCode != this.addFriendTargetIdHash && !this.removeFriendRequestQueue.Contains(new ValueTuple<int, FriendBackendController.Friend>(hashCode, target)))
		{
			this.removeFriendRequestQueue.Enqueue(new ValueTuple<int, FriendBackendController.Friend>(hashCode, target));
		}
	}

	// Token: 0x06004E48 RID: 20040 RVA: 0x001A007D File Offset: 0x0019E27D
	private void Awake()
	{
		if (FriendBackendController.Instance == null)
		{
			FriendBackendController.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06004E49 RID: 20041 RVA: 0x001A00A0 File Offset: 0x0019E2A0
	private void GetFriendsInternal()
	{
		base.StartCoroutine(this.SendGetFriendsRequest(new FriendBackendController.GetFriendsRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = ""
		}, new Action<FriendBackendController.GetFriendsResponse>(this.GetFriendsComplete)));
	}

	// Token: 0x06004E4A RID: 20042 RVA: 0x001A00FA File Offset: 0x0019E2FA
	private IEnumerator SendGetFriendsRequest(FriendBackendController.GetFriendsRequest data, Action<FriendBackendController.GetFriendsResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/GetFriendsV2", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			FriendBackendController.GetFriendsResponse obj = JsonConvert.DeserializeObject<FriendBackendController.GetFriendsResponse>(request.downloadHandler.text);
			callback(obj);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.getFriendsRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.getFriendsRetryCount + 1));
				this.getFriendsRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.GetFriendsInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum GetFriends retries attempted. Please check your network connection.", null);
				this.getFriendsRetryCount = 0;
				callback(null);
			}
		}
		else
		{
			this.getFriendsInProgress = false;
		}
		yield break;
	}

	// Token: 0x06004E4B RID: 20043 RVA: 0x001A0118 File Offset: 0x0019E318
	private void GetFriendsComplete([CanBeNull] FriendBackendController.GetFriendsResponse response)
	{
		this.getFriendsInProgress = false;
		if (response != null)
		{
			this.lastGetFriendsResponse = response;
			if (this.lastGetFriendsResponse.Result != null)
			{
				this.lastPrivacyState = this.lastGetFriendsResponse.Result.MyPrivacyState;
				if (this.lastGetFriendsResponse.Result.Friends != null)
				{
					this.lastFriendsList.Clear();
					foreach (FriendBackendController.Friend item in this.lastGetFriendsResponse.Result.Friends)
					{
						this.lastFriendsList.Add(item);
					}
				}
			}
			Action<bool> onGetFriendsComplete = this.OnGetFriendsComplete;
			if (onGetFriendsComplete == null)
			{
				return;
			}
			onGetFriendsComplete(true);
			return;
		}
		else
		{
			Action<bool> onGetFriendsComplete2 = this.OnGetFriendsComplete;
			if (onGetFriendsComplete2 == null)
			{
				return;
			}
			onGetFriendsComplete2(false);
			return;
		}
	}

	// Token: 0x06004E4C RID: 20044 RVA: 0x001A01F4 File Offset: 0x0019E3F4
	public void CreateTestFriends()
	{
		Debug.Log("Adding test friends");
		for (int i = 0; i < 15; i++)
		{
			FriendBackendController.FriendPresence friendPresence = new FriendBackendController.FriendPresence();
			friendPresence.FriendLinkId = i.ToString();
			friendPresence.UserName = i.ToString();
			friendPresence.RoomId = i.ToString();
			friendPresence.Zone = "TreeHouse";
			friendPresence.Region = "Jungle";
			friendPresence.IsPublic = new bool?(true);
			FriendBackendController.Friend friend = new FriendBackendController.Friend();
			friend.Presence = friendPresence;
			friend.Created = DateTime.Now;
			this.FriendsList.Add(friend);
		}
	}

	// Token: 0x06004E4D RID: 20045 RVA: 0x001A028C File Offset: 0x0019E48C
	private void SetPrivacyStateInternal()
	{
		base.StartCoroutine(this.SendSetPrivacyStateRequest(new FriendBackendController.SetPrivacyStateRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			PrivacyState = this.setPrivacyStateState.ToString()
		}, new Action<FriendBackendController.SetPrivacyStateResponse>(this.SetPrivacyStateComplete)));
	}

	// Token: 0x06004E4E RID: 20046 RVA: 0x001A02F2 File Offset: 0x0019E4F2
	private IEnumerator SendSetPrivacyStateRequest(FriendBackendController.SetPrivacyStateRequest data, Action<FriendBackendController.SetPrivacyStateResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/SetPrivacyState", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			FriendBackendController.SetPrivacyStateResponse obj = JsonConvert.DeserializeObject<FriendBackendController.SetPrivacyStateResponse>(request.downloadHandler.text);
			callback(obj);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.setPrivacyStateRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.setPrivacyStateRetryCount + 1));
				this.setPrivacyStateRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.SetPrivacyStateInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum SetPrivacyState retries attempted. Please check your network connection.", null);
				this.setPrivacyStateRetryCount = 0;
				callback(null);
			}
		}
		else
		{
			this.setPrivacyStateInProgress = false;
		}
		yield break;
	}

	// Token: 0x06004E4F RID: 20047 RVA: 0x001A0310 File Offset: 0x0019E510
	private void SetPrivacyStateComplete([CanBeNull] FriendBackendController.SetPrivacyStateResponse response)
	{
		this.setPrivacyStateInProgress = false;
		if (response != null)
		{
			this.lastPrivacyStateResponse = response;
			Action<bool> onSetPrivacyStateComplete = this.OnSetPrivacyStateComplete;
			if (onSetPrivacyStateComplete != null)
			{
				onSetPrivacyStateComplete(true);
			}
		}
		else
		{
			Action<bool> onSetPrivacyStateComplete2 = this.OnSetPrivacyStateComplete;
			if (onSetPrivacyStateComplete2 != null)
			{
				onSetPrivacyStateComplete2(false);
			}
		}
		if (this.setPrivacyStateQueue.Count > 0)
		{
			FriendBackendController.PrivacyState privacyState = this.setPrivacyStateQueue.Dequeue();
			this.SetPrivacyState(privacyState);
		}
	}

	// Token: 0x06004E50 RID: 20048 RVA: 0x001A0378 File Offset: 0x0019E578
	private void AddFriendInternal()
	{
		base.StartCoroutine(this.SendAddFriendRequest(new FriendBackendController.FriendRequestRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			MothershipId = "",
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipToken = "",
			MyFriendLinkId = NetworkSystem.Instance.LocalPlayer.UserId,
			FriendFriendLinkId = this.addFriendTargetPlayer.UserId
		}, new Action<bool>(this.AddFriendComplete)));
	}

	// Token: 0x06004E51 RID: 20049 RVA: 0x001A0403 File Offset: 0x0019E603
	private IEnumerator SendAddFriendRequest(FriendBackendController.FriendRequestRequest data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/RequestFriend", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			callback(true);
		}
		else
		{
			if (request.responseCode == 409L)
			{
				flag = false;
			}
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.addFriendRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.addFriendRetryCount + 1));
				this.addFriendRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.AddFriendInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum AddFriend retries attempted. Please check your network connection.", null);
				this.addFriendRetryCount = 0;
				callback(false);
			}
		}
		else
		{
			this.addFriendInProgress = false;
		}
		yield break;
	}

	// Token: 0x06004E52 RID: 20050 RVA: 0x001A0420 File Offset: 0x0019E620
	private void AddFriendComplete([CanBeNull] bool success)
	{
		if (success)
		{
			Action<NetPlayer, bool> onAddFriendComplete = this.OnAddFriendComplete;
			if (onAddFriendComplete != null)
			{
				onAddFriendComplete(this.addFriendTargetPlayer, true);
			}
		}
		else
		{
			Action<NetPlayer, bool> onAddFriendComplete2 = this.OnAddFriendComplete;
			if (onAddFriendComplete2 != null)
			{
				onAddFriendComplete2(this.addFriendTargetPlayer, false);
			}
		}
		this.addFriendInProgress = false;
		this.addFriendTargetIdHash = 0;
		this.addFriendTargetPlayer = null;
		if (this.addFriendRequestQueue.Count > 0)
		{
			ValueTuple<int, NetPlayer> valueTuple = this.addFriendRequestQueue.Dequeue();
			this.AddFriend(valueTuple.Item2);
		}
	}

	// Token: 0x06004E53 RID: 20051 RVA: 0x001A04A0 File Offset: 0x0019E6A0
	private void RemoveFriendInternal()
	{
		base.StartCoroutine(this.SendRemoveFriendRequest(new FriendBackendController.RemoveFriendRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			MothershipId = "",
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MyFriendLinkId = NetworkSystem.Instance.LocalPlayer.UserId,
			FriendFriendLinkId = this.removeFriendTarget.Presence.FriendLinkId
		}, new Action<bool>(this.RemoveFriendComplete)));
	}

	// Token: 0x06004E54 RID: 20052 RVA: 0x001A0525 File Offset: 0x0019E725
	private IEnumerator SendRemoveFriendRequest(FriendBackendController.RemoveFriendRequest data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/RemoveFriend", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			callback(true);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.removeFriendRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.removeFriendRetryCount + 1));
				this.removeFriendRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.AddFriendInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum AddFriend retries attempted. Please check your network connection.", null);
				this.removeFriendRetryCount = 0;
				callback(false);
			}
		}
		else
		{
			this.removeFriendInProgress = false;
		}
		yield break;
	}

	// Token: 0x06004E55 RID: 20053 RVA: 0x001A0544 File Offset: 0x0019E744
	private void RemoveFriendComplete([CanBeNull] bool success)
	{
		if (success)
		{
			Action<FriendBackendController.Friend, bool> onRemoveFriendComplete = this.OnRemoveFriendComplete;
			if (onRemoveFriendComplete != null)
			{
				onRemoveFriendComplete(this.removeFriendTarget, true);
			}
		}
		else
		{
			Action<FriendBackendController.Friend, bool> onRemoveFriendComplete2 = this.OnRemoveFriendComplete;
			if (onRemoveFriendComplete2 != null)
			{
				onRemoveFriendComplete2(this.removeFriendTarget, false);
			}
		}
		this.removeFriendInProgress = false;
		this.removeFriendTargetIdHash = 0;
		this.removeFriendTarget = null;
		if (this.removeFriendRequestQueue.Count > 0)
		{
			ValueTuple<int, FriendBackendController.Friend> valueTuple = this.removeFriendRequestQueue.Dequeue();
			this.RemoveFriend(valueTuple.Item2);
		}
	}

	// Token: 0x06004E56 RID: 20054 RVA: 0x001A05C4 File Offset: 0x0019E7C4
	private void LogNetPlayersInRoom()
	{
		Debug.Log("Local Player PlayfabId: " + PlayFabAuthenticator.instance.GetPlayFabPlayerId());
		int num = 0;
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			Debug.Log(string.Format("[{0}] Player: {1}, ActorNumber: {2}, UserID: {3}, IsMasterClient: {4}", new object[]
			{
				num,
				netPlayer.NickName,
				netPlayer.ActorNumber,
				netPlayer.UserId,
				netPlayer.IsMasterClient
			}));
			num++;
		}
	}

	// Token: 0x06004E57 RID: 20055 RVA: 0x001A065C File Offset: 0x0019E85C
	private void TestAddFriend()
	{
		this.OnAddFriendComplete -= this.TestAddFriendCompleteCallback;
		this.OnAddFriendComplete += this.TestAddFriendCompleteCallback;
		NetPlayer target = null;
		if (this.netPlayerIndexToAddFriend >= 0 && this.netPlayerIndexToAddFriend < NetworkSystem.Instance.AllNetPlayers.Length)
		{
			target = NetworkSystem.Instance.AllNetPlayers[this.netPlayerIndexToAddFriend];
		}
		this.AddFriend(target);
	}

	// Token: 0x06004E58 RID: 20056 RVA: 0x001A06C5 File Offset: 0x0019E8C5
	private void TestAddFriendCompleteCallback(NetPlayer player, bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestAddFriendCompleteCallback returned with success = true");
			return;
		}
		Debug.Log("FriendBackend: TestAddFriendCompleteCallback returned with success = false");
	}

	// Token: 0x06004E59 RID: 20057 RVA: 0x001A06E0 File Offset: 0x0019E8E0
	private void TestRemoveFriend()
	{
		this.OnRemoveFriendComplete -= this.TestRemoveFriendCompleteCallback;
		this.OnRemoveFriendComplete += this.TestRemoveFriendCompleteCallback;
		FriendBackendController.Friend target = null;
		if (this.friendListIndexToRemoveFriend >= 0 && this.friendListIndexToRemoveFriend < this.FriendsList.Count)
		{
			target = this.FriendsList[this.friendListIndexToRemoveFriend];
		}
		this.RemoveFriend(target);
	}

	// Token: 0x06004E5A RID: 20058 RVA: 0x001A0748 File Offset: 0x0019E948
	private void TestRemoveFriendCompleteCallback(FriendBackendController.Friend friend, bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestRemoveFriendCompleteCallback returned with success = true");
			return;
		}
		Debug.Log("FriendBackend: TestRemoveFriendCompleteCallback returned with success = false");
	}

	// Token: 0x06004E5B RID: 20059 RVA: 0x001A0762 File Offset: 0x0019E962
	private void TestGetFriends()
	{
		this.OnGetFriendsComplete -= this.TestGetFriendsCompleteCallback;
		this.OnGetFriendsComplete += this.TestGetFriendsCompleteCallback;
		this.GetFriends();
	}

	// Token: 0x06004E5C RID: 20060 RVA: 0x001A0790 File Offset: 0x0019E990
	private void TestGetFriendsCompleteCallback(bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestGetFriendsCompleteCallback returned with success = true");
			if (this.FriendsList != null)
			{
				string text = string.Format("Friend Count: {0} Friends: \n", this.FriendsList.Count);
				for (int i = 0; i < this.FriendsList.Count; i++)
				{
					if (this.FriendsList[i] != null && this.FriendsList[i].Presence != null)
					{
						text = string.Concat(new string[]
						{
							text,
							this.FriendsList[i].Presence.UserName,
							", ",
							this.FriendsList[i].Presence.FriendLinkId,
							", ",
							this.FriendsList[i].Presence.RoomId,
							", ",
							this.FriendsList[i].Presence.Region,
							", ",
							this.FriendsList[i].Presence.Zone,
							"\n"
						});
					}
					else
					{
						text += "null friend\n";
					}
				}
				Debug.Log(text);
				return;
			}
		}
		else
		{
			Debug.Log("FriendBackend: TestGetFriendsCompleteCallback returned with success = false");
		}
	}

	// Token: 0x06004E5D RID: 20061 RVA: 0x001A08ED File Offset: 0x0019EAED
	private void TestSetPrivacyState()
	{
		this.OnSetPrivacyStateComplete -= this.TestSetPrivacyStateCompleteCallback;
		this.OnSetPrivacyStateComplete += this.TestSetPrivacyStateCompleteCallback;
		this.SetPrivacyState(this.privacyStateToSet);
	}

	// Token: 0x06004E5E RID: 20062 RVA: 0x001A0920 File Offset: 0x0019EB20
	private void TestSetPrivacyStateCompleteCallback(bool success)
	{
		if (success)
		{
			Debug.Log(string.Format("SetPrivacyState Success: Status: {0} Error: {1}", this.lastPrivacyStateResponse.StatusCode, this.lastPrivacyStateResponse.Error));
			return;
		}
		Debug.Log(string.Format("SetPrivacyState Failed: Status: {0} Error: {1}", this.lastPrivacyStateResponse.StatusCode, this.lastPrivacyStateResponse.Error));
	}

	// Token: 0x04006041 RID: 24641
	[OnEnterPlay_SetNull]
	public static volatile FriendBackendController Instance;

	// Token: 0x04006046 RID: 24646
	private int maxRetriesOnFail = 3;

	// Token: 0x04006047 RID: 24647
	private int getFriendsRetryCount;

	// Token: 0x04006048 RID: 24648
	private int setPrivacyStateRetryCount;

	// Token: 0x04006049 RID: 24649
	private int addFriendRetryCount;

	// Token: 0x0400604A RID: 24650
	private int removeFriendRetryCount;

	// Token: 0x0400604B RID: 24651
	private bool getFriendsInProgress;

	// Token: 0x0400604C RID: 24652
	private FriendBackendController.GetFriendsResponse lastGetFriendsResponse;

	// Token: 0x0400604D RID: 24653
	private List<FriendBackendController.Friend> lastFriendsList = new List<FriendBackendController.Friend>();

	// Token: 0x0400604E RID: 24654
	private bool setPrivacyStateInProgress;

	// Token: 0x0400604F RID: 24655
	private FriendBackendController.PrivacyState setPrivacyStateState;

	// Token: 0x04006050 RID: 24656
	private FriendBackendController.SetPrivacyStateResponse lastPrivacyStateResponse;

	// Token: 0x04006051 RID: 24657
	private Queue<FriendBackendController.PrivacyState> setPrivacyStateQueue = new Queue<FriendBackendController.PrivacyState>();

	// Token: 0x04006052 RID: 24658
	private FriendBackendController.PrivacyState lastPrivacyState;

	// Token: 0x04006053 RID: 24659
	private bool addFriendInProgress;

	// Token: 0x04006054 RID: 24660
	private int addFriendTargetIdHash;

	// Token: 0x04006055 RID: 24661
	private NetPlayer addFriendTargetPlayer;

	// Token: 0x04006056 RID: 24662
	private Queue<ValueTuple<int, NetPlayer>> addFriendRequestQueue = new Queue<ValueTuple<int, NetPlayer>>();

	// Token: 0x04006057 RID: 24663
	private bool removeFriendInProgress;

	// Token: 0x04006058 RID: 24664
	private int removeFriendTargetIdHash;

	// Token: 0x04006059 RID: 24665
	private FriendBackendController.Friend removeFriendTarget;

	// Token: 0x0400605A RID: 24666
	private Queue<ValueTuple<int, FriendBackendController.Friend>> removeFriendRequestQueue = new Queue<ValueTuple<int, FriendBackendController.Friend>>();

	// Token: 0x0400605B RID: 24667
	[SerializeField]
	private int netPlayerIndexToAddFriend;

	// Token: 0x0400605C RID: 24668
	[SerializeField]
	private int friendListIndexToRemoveFriend;

	// Token: 0x0400605D RID: 24669
	[SerializeField]
	private FriendBackendController.PrivacyState privacyStateToSet;

	// Token: 0x02000C60 RID: 3168
	public class Friend
	{
		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x06004E60 RID: 20064 RVA: 0x001A09C0 File Offset: 0x0019EBC0
		// (set) Token: 0x06004E61 RID: 20065 RVA: 0x001A09C8 File Offset: 0x0019EBC8
		public FriendBackendController.FriendPresence Presence { get; set; }

		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x06004E62 RID: 20066 RVA: 0x001A09D1 File Offset: 0x0019EBD1
		// (set) Token: 0x06004E63 RID: 20067 RVA: 0x001A09D9 File Offset: 0x0019EBD9
		public DateTime Created { get; set; }
	}

	// Token: 0x02000C61 RID: 3169
	public class FriendPresence
	{
		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x06004E65 RID: 20069 RVA: 0x001A09E2 File Offset: 0x0019EBE2
		// (set) Token: 0x06004E66 RID: 20070 RVA: 0x001A09EA File Offset: 0x0019EBEA
		public string FriendLinkId { get; set; }

		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x06004E67 RID: 20071 RVA: 0x001A09F3 File Offset: 0x0019EBF3
		// (set) Token: 0x06004E68 RID: 20072 RVA: 0x001A09FB File Offset: 0x0019EBFB
		public string UserName { get; set; }

		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x06004E69 RID: 20073 RVA: 0x001A0A04 File Offset: 0x0019EC04
		// (set) Token: 0x06004E6A RID: 20074 RVA: 0x001A0A0C File Offset: 0x0019EC0C
		public string RoomId { get; set; }

		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x06004E6B RID: 20075 RVA: 0x001A0A15 File Offset: 0x0019EC15
		// (set) Token: 0x06004E6C RID: 20076 RVA: 0x001A0A1D File Offset: 0x0019EC1D
		public string Zone { get; set; }

		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x06004E6D RID: 20077 RVA: 0x001A0A26 File Offset: 0x0019EC26
		// (set) Token: 0x06004E6E RID: 20078 RVA: 0x001A0A2E File Offset: 0x0019EC2E
		public string Region { get; set; }

		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x06004E6F RID: 20079 RVA: 0x001A0A37 File Offset: 0x0019EC37
		// (set) Token: 0x06004E70 RID: 20080 RVA: 0x001A0A3F File Offset: 0x0019EC3F
		public bool? IsPublic { get; set; }
	}

	// Token: 0x02000C62 RID: 3170
	public class FriendLink
	{
		// Token: 0x17000736 RID: 1846
		// (get) Token: 0x06004E72 RID: 20082 RVA: 0x001A0A48 File Offset: 0x0019EC48
		// (set) Token: 0x06004E73 RID: 20083 RVA: 0x001A0A50 File Offset: 0x0019EC50
		public string my_playfab_id { get; set; }

		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x06004E74 RID: 20084 RVA: 0x001A0A59 File Offset: 0x0019EC59
		// (set) Token: 0x06004E75 RID: 20085 RVA: 0x001A0A61 File Offset: 0x0019EC61
		public string my_mothership_id { get; set; }

		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x06004E76 RID: 20086 RVA: 0x001A0A6A File Offset: 0x0019EC6A
		// (set) Token: 0x06004E77 RID: 20087 RVA: 0x001A0A72 File Offset: 0x0019EC72
		public string my_friendlink_id { get; set; }

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x06004E78 RID: 20088 RVA: 0x001A0A7B File Offset: 0x0019EC7B
		// (set) Token: 0x06004E79 RID: 20089 RVA: 0x001A0A83 File Offset: 0x0019EC83
		public string friend_playfab_id { get; set; }

		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x06004E7A RID: 20090 RVA: 0x001A0A8C File Offset: 0x0019EC8C
		// (set) Token: 0x06004E7B RID: 20091 RVA: 0x001A0A94 File Offset: 0x0019EC94
		public string friend_mothership_id { get; set; }

		// Token: 0x1700073B RID: 1851
		// (get) Token: 0x06004E7C RID: 20092 RVA: 0x001A0A9D File Offset: 0x0019EC9D
		// (set) Token: 0x06004E7D RID: 20093 RVA: 0x001A0AA5 File Offset: 0x0019ECA5
		public string friend_friendlink_id { get; set; }

		// Token: 0x1700073C RID: 1852
		// (get) Token: 0x06004E7E RID: 20094 RVA: 0x001A0AAE File Offset: 0x0019ECAE
		// (set) Token: 0x06004E7F RID: 20095 RVA: 0x001A0AB6 File Offset: 0x0019ECB6
		public DateTime created { get; set; }
	}

	// Token: 0x02000C63 RID: 3171
	[NullableContext(2)]
	[Nullable(0)]
	public class FriendIdResponse
	{
		// Token: 0x1700073D RID: 1853
		// (get) Token: 0x06004E81 RID: 20097 RVA: 0x001A0ABF File Offset: 0x0019ECBF
		// (set) Token: 0x06004E82 RID: 20098 RVA: 0x001A0AC7 File Offset: 0x0019ECC7
		public string PlayFabId { get; set; }

		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x06004E83 RID: 20099 RVA: 0x001A0AD0 File Offset: 0x0019ECD0
		// (set) Token: 0x06004E84 RID: 20100 RVA: 0x001A0AD8 File Offset: 0x0019ECD8
		public string MothershipId { get; set; } = "";
	}

	// Token: 0x02000C64 RID: 3172
	public class FriendRequestRequest
	{
		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x06004E86 RID: 20102 RVA: 0x001A0AF4 File Offset: 0x0019ECF4
		// (set) Token: 0x06004E87 RID: 20103 RVA: 0x001A0AFC File Offset: 0x0019ECFC
		public string PlayFabId { get; set; }

		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x06004E88 RID: 20104 RVA: 0x001A0B05 File Offset: 0x0019ED05
		// (set) Token: 0x06004E89 RID: 20105 RVA: 0x001A0B0D File Offset: 0x0019ED0D
		public string MothershipId { get; set; } = "";

		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x06004E8A RID: 20106 RVA: 0x001A0B16 File Offset: 0x0019ED16
		// (set) Token: 0x06004E8B RID: 20107 RVA: 0x001A0B1E File Offset: 0x0019ED1E
		public string PlayFabTicket { get; set; }

		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x06004E8C RID: 20108 RVA: 0x001A0B27 File Offset: 0x0019ED27
		// (set) Token: 0x06004E8D RID: 20109 RVA: 0x001A0B2F File Offset: 0x0019ED2F
		public string MothershipToken { get; set; }

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x06004E8E RID: 20110 RVA: 0x001A0B38 File Offset: 0x0019ED38
		// (set) Token: 0x06004E8F RID: 20111 RVA: 0x001A0B40 File Offset: 0x0019ED40
		public string MyFriendLinkId { get; set; }

		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x06004E90 RID: 20112 RVA: 0x001A0B49 File Offset: 0x0019ED49
		// (set) Token: 0x06004E91 RID: 20113 RVA: 0x001A0B51 File Offset: 0x0019ED51
		public string FriendFriendLinkId { get; set; }
	}

	// Token: 0x02000C65 RID: 3173
	public class GetFriendsRequest
	{
		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x06004E93 RID: 20115 RVA: 0x001A0B6D File Offset: 0x0019ED6D
		// (set) Token: 0x06004E94 RID: 20116 RVA: 0x001A0B75 File Offset: 0x0019ED75
		public string PlayFabId { get; set; }

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x06004E95 RID: 20117 RVA: 0x001A0B7E File Offset: 0x0019ED7E
		// (set) Token: 0x06004E96 RID: 20118 RVA: 0x001A0B86 File Offset: 0x0019ED86
		public string MothershipId { get; set; } = "";

		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x06004E97 RID: 20119 RVA: 0x001A0B8F File Offset: 0x0019ED8F
		// (set) Token: 0x06004E98 RID: 20120 RVA: 0x001A0B97 File Offset: 0x0019ED97
		public string MothershipToken { get; set; }

		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x06004E99 RID: 20121 RVA: 0x001A0BA0 File Offset: 0x0019EDA0
		// (set) Token: 0x06004E9A RID: 20122 RVA: 0x001A0BA8 File Offset: 0x0019EDA8
		public string PlayFabTicket { get; set; }
	}

	// Token: 0x02000C66 RID: 3174
	public class GetFriendsResponse
	{
		// Token: 0x17000749 RID: 1865
		// (get) Token: 0x06004E9C RID: 20124 RVA: 0x001A0BC4 File Offset: 0x0019EDC4
		// (set) Token: 0x06004E9D RID: 20125 RVA: 0x001A0BCC File Offset: 0x0019EDCC
		[CanBeNull]
		public FriendBackendController.GetFriendsResult Result { get; set; }

		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x06004E9E RID: 20126 RVA: 0x001A0BD5 File Offset: 0x0019EDD5
		// (set) Token: 0x06004E9F RID: 20127 RVA: 0x001A0BDD File Offset: 0x0019EDDD
		public int StatusCode { get; set; }

		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x06004EA0 RID: 20128 RVA: 0x001A0BE6 File Offset: 0x0019EDE6
		// (set) Token: 0x06004EA1 RID: 20129 RVA: 0x001A0BEE File Offset: 0x0019EDEE
		[Nullable(2)]
		public string Error { [NullableContext(2)] get; [NullableContext(2)] set; }
	}

	// Token: 0x02000C67 RID: 3175
	public class GetFriendsResult
	{
		// Token: 0x1700074C RID: 1868
		// (get) Token: 0x06004EA3 RID: 20131 RVA: 0x001A0BF7 File Offset: 0x0019EDF7
		// (set) Token: 0x06004EA4 RID: 20132 RVA: 0x001A0BFF File Offset: 0x0019EDFF
		public List<FriendBackendController.Friend> Friends { get; set; }

		// Token: 0x1700074D RID: 1869
		// (get) Token: 0x06004EA5 RID: 20133 RVA: 0x001A0C08 File Offset: 0x0019EE08
		// (set) Token: 0x06004EA6 RID: 20134 RVA: 0x001A0C10 File Offset: 0x0019EE10
		public FriendBackendController.PrivacyState MyPrivacyState { get; set; }
	}

	// Token: 0x02000C68 RID: 3176
	public class SetPrivacyStateRequest
	{
		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x06004EA8 RID: 20136 RVA: 0x001A0C19 File Offset: 0x0019EE19
		// (set) Token: 0x06004EA9 RID: 20137 RVA: 0x001A0C21 File Offset: 0x0019EE21
		public string PlayFabId { get; set; }

		// Token: 0x1700074F RID: 1871
		// (get) Token: 0x06004EAA RID: 20138 RVA: 0x001A0C2A File Offset: 0x0019EE2A
		// (set) Token: 0x06004EAB RID: 20139 RVA: 0x001A0C32 File Offset: 0x0019EE32
		public string PlayFabTicket { get; set; }

		// Token: 0x17000750 RID: 1872
		// (get) Token: 0x06004EAC RID: 20140 RVA: 0x001A0C3B File Offset: 0x0019EE3B
		// (set) Token: 0x06004EAD RID: 20141 RVA: 0x001A0C43 File Offset: 0x0019EE43
		public string PrivacyState { get; set; }
	}

	// Token: 0x02000C69 RID: 3177
	[NullableContext(2)]
	[Nullable(0)]
	public class SetPrivacyStateResponse
	{
		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x06004EAF RID: 20143 RVA: 0x001A0C4C File Offset: 0x0019EE4C
		// (set) Token: 0x06004EB0 RID: 20144 RVA: 0x001A0C54 File Offset: 0x0019EE54
		public int StatusCode { get; set; }

		// Token: 0x17000752 RID: 1874
		// (get) Token: 0x06004EB1 RID: 20145 RVA: 0x001A0C5D File Offset: 0x0019EE5D
		// (set) Token: 0x06004EB2 RID: 20146 RVA: 0x001A0C65 File Offset: 0x0019EE65
		public string Error { get; set; }
	}

	// Token: 0x02000C6A RID: 3178
	public class RemoveFriendRequest
	{
		// Token: 0x17000753 RID: 1875
		// (get) Token: 0x06004EB4 RID: 20148 RVA: 0x001A0C6E File Offset: 0x0019EE6E
		// (set) Token: 0x06004EB5 RID: 20149 RVA: 0x001A0C76 File Offset: 0x0019EE76
		public string PlayFabId { get; set; }

		// Token: 0x17000754 RID: 1876
		// (get) Token: 0x06004EB6 RID: 20150 RVA: 0x001A0C7F File Offset: 0x0019EE7F
		// (set) Token: 0x06004EB7 RID: 20151 RVA: 0x001A0C87 File Offset: 0x0019EE87
		public string MothershipId { get; set; } = "";

		// Token: 0x17000755 RID: 1877
		// (get) Token: 0x06004EB8 RID: 20152 RVA: 0x001A0C90 File Offset: 0x0019EE90
		// (set) Token: 0x06004EB9 RID: 20153 RVA: 0x001A0C98 File Offset: 0x0019EE98
		public string PlayFabTicket { get; set; }

		// Token: 0x17000756 RID: 1878
		// (get) Token: 0x06004EBA RID: 20154 RVA: 0x001A0CA1 File Offset: 0x0019EEA1
		// (set) Token: 0x06004EBB RID: 20155 RVA: 0x001A0CA9 File Offset: 0x0019EEA9
		public string MothershipToken { get; set; }

		// Token: 0x17000757 RID: 1879
		// (get) Token: 0x06004EBC RID: 20156 RVA: 0x001A0CB2 File Offset: 0x0019EEB2
		// (set) Token: 0x06004EBD RID: 20157 RVA: 0x001A0CBA File Offset: 0x0019EEBA
		public string MyFriendLinkId { get; set; }

		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x06004EBE RID: 20158 RVA: 0x001A0CC3 File Offset: 0x0019EEC3
		// (set) Token: 0x06004EBF RID: 20159 RVA: 0x001A0CCB File Offset: 0x0019EECB
		public string FriendFriendLinkId { get; set; }
	}

	// Token: 0x02000C6B RID: 3179
	public enum PendingRequestStatus
	{
		// Token: 0x0400608A RID: 24714
		I_REQUESTED,
		// Token: 0x0400608B RID: 24715
		THEY_REQUESTED,
		// Token: 0x0400608C RID: 24716
		CONFIRMED,
		// Token: 0x0400608D RID: 24717
		NOT_FOUND
	}

	// Token: 0x02000C6C RID: 3180
	public enum PrivacyState
	{
		// Token: 0x0400608F RID: 24719
		VISIBLE,
		// Token: 0x04006090 RID: 24720
		PUBLIC_ONLY,
		// Token: 0x04006091 RID: 24721
		HIDDEN
	}
}
