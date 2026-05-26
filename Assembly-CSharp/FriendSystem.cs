using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000C78 RID: 3192
public class FriendSystem : MonoBehaviour
{
	// Token: 0x17000772 RID: 1906
	// (get) Token: 0x06004F56 RID: 20310 RVA: 0x001A4B1A File Offset: 0x001A2D1A
	public FriendSystem.PlayerPrivacy LocalPlayerPrivacy
	{
		get
		{
			return this.localPlayerPrivacy;
		}
	}

	// Token: 0x1400008A RID: 138
	// (add) Token: 0x06004F57 RID: 20311 RVA: 0x001A4B24 File Offset: 0x001A2D24
	// (remove) Token: 0x06004F58 RID: 20312 RVA: 0x001A4B5C File Offset: 0x001A2D5C
	public event Action<List<FriendBackendController.Friend>> OnFriendListRefresh;

	// Token: 0x06004F59 RID: 20313 RVA: 0x001A4B94 File Offset: 0x001A2D94
	public void SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy privacyState)
	{
		this.localPlayerPrivacy = privacyState;
		FriendBackendController.PrivacyState privacyState2;
		switch (privacyState)
		{
		default:
			privacyState2 = FriendBackendController.PrivacyState.VISIBLE;
			break;
		case FriendSystem.PlayerPrivacy.PublicOnly:
			privacyState2 = FriendBackendController.PrivacyState.PUBLIC_ONLY;
			break;
		case FriendSystem.PlayerPrivacy.Hidden:
			privacyState2 = FriendBackendController.PrivacyState.HIDDEN;
			break;
		}
		FriendBackendController.Instance.SetPrivacyState(privacyState2);
	}

	// Token: 0x06004F5A RID: 20314 RVA: 0x001A4BD1 File Offset: 0x001A2DD1
	public void RefreshFriendsList()
	{
		FriendBackendController.Instance.GetFriends();
	}

	// Token: 0x06004F5B RID: 20315 RVA: 0x001A4BE0 File Offset: 0x001A2DE0
	public void SendFriendRequest(NetPlayer targetPlayer, GTZone stationZone, FriendSystem.FriendRequestCallback callback)
	{
		FriendSystem.FriendRequestData item = new FriendSystem.FriendRequestData
		{
			completionCallback = callback,
			sendingPlayerId = NetworkSystem.Instance.LocalPlayer.UserId.GetHashCode(),
			targetPlayerId = targetPlayer.UserId.GetHashCode(),
			localTimeSent = Time.realtimeSinceStartup,
			zone = stationZone
		};
		this.pendingFriendRequests.Add(item);
		FriendBackendController.Instance.AddFriend(targetPlayer);
	}

	// Token: 0x06004F5C RID: 20316 RVA: 0x001A4C5C File Offset: 0x001A2E5C
	public void RemoveFriend(FriendBackendController.Friend friend, FriendSystem.FriendRemovalCallback callback = null)
	{
		this.pendingFriendRemovals.Add(new FriendSystem.FriendRemovalData
		{
			completionCallback = callback,
			targetPlayerId = friend.Presence.FriendLinkId.GetHashCode(),
			localTimeSent = Time.realtimeSinceStartup
		});
		FriendBackendController.Instance.RemoveFriend(friend);
	}

	// Token: 0x06004F5D RID: 20317 RVA: 0x001A4CB8 File Offset: 0x001A2EB8
	public bool HasPendingFriendRequest(GTZone zone, int senderId)
	{
		for (int i = 0; i < this.pendingFriendRequests.Count; i++)
		{
			if (this.pendingFriendRequests[i].zone == zone && this.pendingFriendRequests[i].sendingPlayerId == senderId)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004F5E RID: 20318 RVA: 0x001A4D08 File Offset: 0x001A2F08
	public bool CheckFriendshipWithPlayer(int targetActorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(targetActorNumber);
		if (player != null)
		{
			int hashCode = player.UserId.GetHashCode();
			List<FriendBackendController.Friend> friendsList = FriendBackendController.Instance.FriendsList;
			for (int i = 0; i < friendsList.Count; i++)
			{
				if (friendsList[i] != null && friendsList[i].Presence != null && friendsList[i].Presence.FriendLinkId.GetHashCode() == hashCode)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06004F5F RID: 20319 RVA: 0x001A4D81 File Offset: 0x001A2F81
	private void Awake()
	{
		if (FriendSystem.Instance == null)
		{
			FriendSystem.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06004F60 RID: 20320 RVA: 0x001A4DA4 File Offset: 0x001A2FA4
	private void Start()
	{
		FriendBackendController.Instance.OnGetFriendsComplete += this.OnGetFriendsReturned;
		FriendBackendController.Instance.OnAddFriendComplete += this.OnAddFriendReturned;
		FriendBackendController.Instance.OnRemoveFriendComplete += this.OnRemoveFriendReturned;
	}

	// Token: 0x06004F61 RID: 20321 RVA: 0x001A4DFC File Offset: 0x001A2FFC
	private void OnDestroy()
	{
		if (FriendBackendController.Instance != null)
		{
			FriendBackendController.Instance.OnGetFriendsComplete -= this.OnGetFriendsReturned;
			FriendBackendController.Instance.OnAddFriendComplete -= this.OnAddFriendReturned;
			FriendBackendController.Instance.OnRemoveFriendComplete -= this.OnRemoveFriendReturned;
		}
	}

	// Token: 0x06004F62 RID: 20322 RVA: 0x001A4E60 File Offset: 0x001A3060
	private void OnGetFriendsReturned(bool succeeded)
	{
		if (succeeded)
		{
			this.lastFriendsListRefresh = Time.realtimeSinceStartup;
			switch (FriendBackendController.Instance.MyPrivacyState)
			{
			default:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.Visible;
				break;
			case FriendBackendController.PrivacyState.PUBLIC_ONLY:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.PublicOnly;
				break;
			case FriendBackendController.PrivacyState.HIDDEN:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.Hidden;
				break;
			}
			Action<List<FriendBackendController.Friend>> onFriendListRefresh = this.OnFriendListRefresh;
			if (onFriendListRefresh == null)
			{
				return;
			}
			onFriendListRefresh(FriendBackendController.Instance.FriendsList);
		}
	}

	// Token: 0x06004F63 RID: 20323 RVA: 0x001A4ED0 File Offset: 0x001A30D0
	private void OnAddFriendReturned(NetPlayer targetPlayer, bool succeeded)
	{
		int hashCode = targetPlayer.UserId.GetHashCode();
		this.indexesToRemove.Clear();
		for (int i = 0; i < this.pendingFriendRequests.Count; i++)
		{
			if (this.pendingFriendRequests[i].targetPlayerId == hashCode)
			{
				FriendSystem.FriendRequestCallback completionCallback = this.pendingFriendRequests[i].completionCallback;
				if (completionCallback != null)
				{
					completionCallback(this.pendingFriendRequests[i].zone, this.pendingFriendRequests[i].sendingPlayerId, this.pendingFriendRequests[i].targetPlayerId, succeeded);
				}
				this.indexesToRemove.Add(i);
			}
			else if (this.pendingFriendRequests[i].localTimeSent + this.friendRequestExpirationTime < Time.realtimeSinceStartup)
			{
				this.indexesToRemove.Add(i);
			}
		}
		for (int j = this.indexesToRemove.Count - 1; j >= 0; j--)
		{
			this.pendingFriendRequests.RemoveAt(this.indexesToRemove[j]);
		}
	}

	// Token: 0x06004F64 RID: 20324 RVA: 0x001A4FDC File Offset: 0x001A31DC
	private void OnRemoveFriendReturned(FriendBackendController.Friend friend, bool succeeded)
	{
		if (friend != null && friend.Presence != null)
		{
			int hashCode = friend.Presence.FriendLinkId.GetHashCode();
			this.indexesToRemove.Clear();
			for (int i = 0; i < this.pendingFriendRemovals.Count; i++)
			{
				if (this.pendingFriendRemovals[i].targetPlayerId == hashCode)
				{
					FriendSystem.FriendRemovalCallback completionCallback = this.pendingFriendRemovals[i].completionCallback;
					if (completionCallback != null)
					{
						completionCallback(hashCode, succeeded);
					}
					this.indexesToRemove.Add(i);
				}
				else if (this.pendingFriendRemovals[i].localTimeSent + this.friendRequestExpirationTime < Time.realtimeSinceStartup)
				{
					this.indexesToRemove.Add(i);
				}
			}
			for (int j = this.indexesToRemove.Count - 1; j >= 0; j--)
			{
				this.pendingFriendRemovals.RemoveAt(this.indexesToRemove[j]);
			}
		}
	}

	// Token: 0x0400610E RID: 24846
	[OnEnterPlay_SetNull]
	public static volatile FriendSystem Instance;

	// Token: 0x0400610F RID: 24847
	[SerializeField]
	private float friendRequestExpirationTime = 10f;

	// Token: 0x04006110 RID: 24848
	private FriendSystem.PlayerPrivacy localPlayerPrivacy;

	// Token: 0x04006111 RID: 24849
	private List<FriendSystem.FriendRequestData> pendingFriendRequests = new List<FriendSystem.FriendRequestData>();

	// Token: 0x04006112 RID: 24850
	private List<FriendSystem.FriendRemovalData> pendingFriendRemovals = new List<FriendSystem.FriendRemovalData>();

	// Token: 0x04006113 RID: 24851
	private List<int> indexesToRemove = new List<int>();

	// Token: 0x04006115 RID: 24853
	private float lastFriendsListRefresh;

	// Token: 0x02000C79 RID: 3193
	// (Invoke) Token: 0x06004F67 RID: 20327
	public delegate void FriendRequestCallback(GTZone zone, int localId, int friendId, bool success);

	// Token: 0x02000C7A RID: 3194
	private struct FriendRequestData
	{
		// Token: 0x04006116 RID: 24854
		public GTZone zone;

		// Token: 0x04006117 RID: 24855
		public int sendingPlayerId;

		// Token: 0x04006118 RID: 24856
		public int targetPlayerId;

		// Token: 0x04006119 RID: 24857
		public float localTimeSent;

		// Token: 0x0400611A RID: 24858
		public FriendSystem.FriendRequestCallback completionCallback;
	}

	// Token: 0x02000C7B RID: 3195
	// (Invoke) Token: 0x06004F6B RID: 20331
	public delegate void FriendRemovalCallback(int friendId, bool success);

	// Token: 0x02000C7C RID: 3196
	private struct FriendRemovalData
	{
		// Token: 0x0400611B RID: 24859
		public int targetPlayerId;

		// Token: 0x0400611C RID: 24860
		public float localTimeSent;

		// Token: 0x0400611D RID: 24861
		public FriendSystem.FriendRemovalCallback completionCallback;
	}

	// Token: 0x02000C7D RID: 3197
	private enum FriendRequestStatus
	{
		// Token: 0x0400611F RID: 24863
		Pending,
		// Token: 0x04006120 RID: 24864
		Succeeded,
		// Token: 0x04006121 RID: 24865
		Failed
	}

	// Token: 0x02000C7E RID: 3198
	public enum PlayerPrivacy
	{
		// Token: 0x04006123 RID: 24867
		Visible,
		// Token: 0x04006124 RID: 24868
		PublicOnly,
		// Token: 0x04006125 RID: 24869
		Hidden
	}
}
