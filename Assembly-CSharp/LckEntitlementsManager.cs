using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck;
using Liv.Lck.Core.Cosmetics;
using Liv.Lck.DependencyInjection;
using Photon.Pun;
using UnityEngine;

// Token: 0x020003EF RID: 1007
public class LckEntitlementsManager : MonoBehaviour
{
	// Token: 0x17000257 RID: 599
	// (get) Token: 0x060017FB RID: 6139 RVA: 0x00088F58 File Offset: 0x00087158
	// (set) Token: 0x060017FC RID: 6140 RVA: 0x00088F5F File Offset: 0x0008715F
	public static bool LckEntitlementsEnabled { get; private set; }

	// Token: 0x17000258 RID: 600
	// (get) Token: 0x060017FD RID: 6141 RVA: 0x00088F67 File Offset: 0x00087167
	// (set) Token: 0x060017FE RID: 6142 RVA: 0x00088F6E File Offset: 0x0008716E
	public static LckEntitlementsManager Instance { get; private set; }

	// Token: 0x060017FF RID: 6143 RVA: 0x00088F76 File Offset: 0x00087176
	private void Awake()
	{
		if (LckEntitlementsManager.Instance != null && LckEntitlementsManager.Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		LckEntitlementsManager.Instance = this;
	}

	// Token: 0x06001800 RID: 6144 RVA: 0x00088FA4 File Offset: 0x000871A4
	private void OnEnable()
	{
		this.InitializeFeatureAsync();
		this._cleanupProcessedPlayersCoroutine = base.StartCoroutine(this.CleanupProcessedPlayersCoroutine());
		this._getEntitlementsBatchingCoroutine = base.StartCoroutine(this.ProcessBatchedRemotePlayersCoroutine());
	}

	// Token: 0x06001801 RID: 6145 RVA: 0x00088FD1 File Offset: 0x000871D1
	private void OnDisable()
	{
		if (this._cleanupProcessedPlayersCoroutine != null)
		{
			base.StopCoroutine(this._cleanupProcessedPlayersCoroutine);
			this._cleanupProcessedPlayersCoroutine = null;
		}
		if (this._getEntitlementsBatchingCoroutine != null)
		{
			base.StopCoroutine(this._getEntitlementsBatchingCoroutine);
			this._getEntitlementsBatchingCoroutine = null;
		}
	}

	// Token: 0x06001802 RID: 6146 RVA: 0x0008900C File Offset: 0x0008720C
	private Task InitializeFeatureAsync()
	{
		LckEntitlementsManager.<InitializeFeatureAsync>d__27 <InitializeFeatureAsync>d__;
		<InitializeFeatureAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<InitializeFeatureAsync>d__.<>4__this = this;
		<InitializeFeatureAsync>d__.<>1__state = -1;
		<InitializeFeatureAsync>d__.<>t__builder.Start<LckEntitlementsManager.<InitializeFeatureAsync>d__27>(ref <InitializeFeatureAsync>d__);
		return <InitializeFeatureAsync>d__.<>t__builder.Task;
	}

	// Token: 0x06001803 RID: 6147 RVA: 0x0008904F File Offset: 0x0008724F
	public void OnLocalPlayerSpawned(string localUserId)
	{
		if (!this.ShouldProcessPlayer(localUserId))
		{
			return;
		}
		base.StartCoroutine(this.ProcessLocalPlayerSpawn(localUserId));
	}

	// Token: 0x06001804 RID: 6148 RVA: 0x0008906C File Offset: 0x0008726C
	public void OnRemotePlayerSpawned(string remoteUserId)
	{
		if (this._currentState == LckEntitlementsManager.FeatureState.Disabled)
		{
			return;
		}
		if (!this.ShouldProcessPlayer(remoteUserId))
		{
			return;
		}
		HashSet<string> remotePlayersToGetEntitlementsFor = this._remotePlayersToGetEntitlementsFor;
		lock (remotePlayersToGetEntitlementsFor)
		{
			this._remotePlayersToGetEntitlementsFor.Add(remoteUserId);
		}
	}

	// Token: 0x06001805 RID: 6149 RVA: 0x000890C8 File Offset: 0x000872C8
	private IEnumerator ProcessLocalPlayerSpawn(string userId)
	{
		yield return new WaitUntil(() => this._currentState > LckEntitlementsManager.FeatureState.Checking);
		if (this._currentState == LckEntitlementsManager.FeatureState.Disabled)
		{
			yield break;
		}
		base.StartCoroutine(this.AnnouncePlayerPresenceForSession(userId));
		yield break;
	}

	// Token: 0x06001806 RID: 6150 RVA: 0x000890E0 File Offset: 0x000872E0
	private bool ShouldProcessPlayer(string userId)
	{
		LckEntitlementsManager.PlayerProcessRecord playerProcessRecord;
		if (!this._processedPlayers.TryGetValue(userId, out playerProcessRecord))
		{
			playerProcessRecord = new LckEntitlementsManager.PlayerProcessRecord();
			this._processedPlayers[userId] = playerProcessRecord;
		}
		playerProcessRecord.LastSeenTimestamp = Time.time;
		if (Time.time < playerProcessRecord.TimeoutUntilTimestamp)
		{
			return false;
		}
		if (playerProcessRecord.AttemptCount > 3)
		{
			playerProcessRecord.AttemptCount = 0;
		}
		playerProcessRecord.AttemptCount++;
		if (playerProcessRecord.AttemptCount > 3)
		{
			playerProcessRecord.TimeoutUntilTimestamp = Time.time + 60f;
			return false;
		}
		return true;
	}

	// Token: 0x06001807 RID: 6151 RVA: 0x00089165 File Offset: 0x00087365
	private IEnumerator ProcessBatchedRemotePlayersCoroutine()
	{
		for (;;)
		{
			yield return new WaitForSeconds(15f);
			if (!this._isProcessingBatch)
			{
				HashSet<string> remotePlayersToGetEntitlementsFor = this._remotePlayersToGetEntitlementsFor;
				List<string> list;
				lock (remotePlayersToGetEntitlementsFor)
				{
					if (this._remotePlayersToGetEntitlementsFor.Count == 0)
					{
						continue;
					}
					list = this._remotePlayersToGetEntitlementsFor.ToList<string>();
					this._remotePlayersToGetEntitlementsFor.Clear();
				}
				if (list.Count > 0)
				{
					this._isProcessingBatch = true;
					this.GetCosmeticsForPlayersAsync(list, "ProcessBatchedRemotePlayers");
				}
			}
		}
		yield break;
	}

	// Token: 0x06001808 RID: 6152 RVA: 0x00089174 File Offset: 0x00087374
	private IEnumerator AnnouncePlayerPresenceForSession(string localPlayerId)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			Debug.LogError("LCK: Called AnnouncePlayerPresenceForSession() but no room was found. Player not announced.");
			yield break;
		}
		string sessionId = "DefaultSessionId";
		int num;
		for (int attempt = 1; attempt <= 2; attempt = num + 1)
		{
			LckEntitlementsManager.<>c__DisplayClass33_0 CS$<>8__locals1 = new LckEntitlementsManager.<>c__DisplayClass33_0();
			CS$<>8__locals1.announcementAsync = this._lckCosmeticsCoordinator.AnnouncePlayerPresenceForSessionAsync(localPlayerId, sessionId);
			yield return new WaitUntil(() => CS$<>8__locals1.announcementAsync.IsCompleted);
			if (!CS$<>8__locals1.announcementAsync.IsFaulted && CS$<>8__locals1.announcementAsync.Result.IsOk)
			{
				yield break;
			}
			string arg = CS$<>8__locals1.announcementAsync.IsFaulted ? CS$<>8__locals1.announcementAsync.Exception.ToString() : CS$<>8__locals1.announcementAsync.Result.Message.ToString();
			Debug.LogError(string.Format("LCK: Error setting session entitlement (Attempt {0}/{1}): {2}", attempt, 2, arg));
			CS$<>8__locals1 = null;
			num = attempt;
		}
		Debug.LogError("LCK: All attempts to set session entitlement failed.");
		yield break;
	}

	// Token: 0x06001809 RID: 6153 RVA: 0x0008918C File Offset: 0x0008738C
	private Task GetCosmeticsForPlayersAsync(List<string> userIdList, string methodNameForLogging)
	{
		LckEntitlementsManager.<GetCosmeticsForPlayersAsync>d__34 <GetCosmeticsForPlayersAsync>d__;
		<GetCosmeticsForPlayersAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<GetCosmeticsForPlayersAsync>d__.<>4__this = this;
		<GetCosmeticsForPlayersAsync>d__.userIdList = userIdList;
		<GetCosmeticsForPlayersAsync>d__.methodNameForLogging = methodNameForLogging;
		<GetCosmeticsForPlayersAsync>d__.<>1__state = -1;
		<GetCosmeticsForPlayersAsync>d__.<>t__builder.Start<LckEntitlementsManager.<GetCosmeticsForPlayersAsync>d__34>(ref <GetCosmeticsForPlayersAsync>d__);
		return <GetCosmeticsForPlayersAsync>d__.<>t__builder.Task;
	}

	// Token: 0x0600180A RID: 6154 RVA: 0x000891DF File Offset: 0x000873DF
	private IEnumerator CleanupProcessedPlayersCoroutine()
	{
		List<string> playersToRemove = new List<string>();
		for (;;)
		{
			yield return new WaitForSeconds(60f);
			playersToRemove.Clear();
			float time = Time.time;
			foreach (KeyValuePair<string, LckEntitlementsManager.PlayerProcessRecord> keyValuePair in this._processedPlayers)
			{
				if (time > keyValuePair.Value.LastSeenTimestamp + 300f)
				{
					playersToRemove.Add(keyValuePair.Key);
				}
			}
			if (playersToRemove.Count > 0)
			{
				using (List<string>.Enumerator enumerator2 = playersToRemove.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						string key = enumerator2.Current;
						this._processedPlayers.Remove(key);
					}
					continue;
				}
				yield break;
			}
		}
	}

	// Token: 0x04002324 RID: 8996
	[InjectLck]
	private ILckCosmeticsCoordinator _lckCosmeticsCoordinator;

	// Token: 0x04002325 RID: 8997
	[InjectLck]
	private ILckCosmeticsFeatureFlagManager _featureFlagManager;

	// Token: 0x04002327 RID: 8999
	private const int MAX_API_CALL_ATTEMPTS = 2;

	// Token: 0x04002328 RID: 9000
	private const int MAX_CONSECUTIVE_ATTEMPTS = 3;

	// Token: 0x04002329 RID: 9001
	private const float ABUSE_TIMEOUT_MINUTES = 1f;

	// Token: 0x0400232A RID: 9002
	private const float BATCH_GET_ENTITLEMENTS_INTERVAL_SECONDS = 15f;

	// Token: 0x0400232B RID: 9003
	private const float STALE_PLAYER_TIMEOUT_MINUTES = 5f;

	// Token: 0x0400232C RID: 9004
	private const string DEFAULT_SESSION_ID = "DefaultSessionId";

	// Token: 0x0400232D RID: 9005
	private LckEntitlementsManager.FeatureState _currentState;

	// Token: 0x0400232E RID: 9006
	private readonly HashSet<string> _remotePlayersToGetEntitlementsFor = new HashSet<string>();

	// Token: 0x0400232F RID: 9007
	private Coroutine _getEntitlementsBatchingCoroutine;

	// Token: 0x04002330 RID: 9008
	private readonly Dictionary<string, LckEntitlementsManager.PlayerProcessRecord> _processedPlayers = new Dictionary<string, LckEntitlementsManager.PlayerProcessRecord>();

	// Token: 0x04002331 RID: 9009
	private Coroutine _cleanupProcessedPlayersCoroutine;

	// Token: 0x04002332 RID: 9010
	private bool _isProcessingBatch;

	// Token: 0x020003F0 RID: 1008
	private class PlayerProcessRecord
	{
		// Token: 0x04002334 RID: 9012
		public int AttemptCount;

		// Token: 0x04002335 RID: 9013
		public float TimeoutUntilTimestamp;

		// Token: 0x04002336 RID: 9014
		public float LastSeenTimestamp;
	}

	// Token: 0x020003F1 RID: 1009
	private enum FeatureState
	{
		// Token: 0x04002338 RID: 9016
		Checking,
		// Token: 0x04002339 RID: 9017
		Enabled,
		// Token: 0x0400233A RID: 9018
		Disabled
	}
}
