using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000930 RID: 2352
public class MonkeAgent : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x1700058A RID: 1418
	// (get) Token: 0x06003D94 RID: 15764 RVA: 0x0008F059 File Offset: 0x0008D259
	private NetworkRunner runner
	{
		get
		{
			return ((NetworkSystemFusion)NetworkSystem.Instance).runner;
		}
	}

	// Token: 0x1700058B RID: 1419
	// (get) Token: 0x06003D95 RID: 15765 RVA: 0x0014DAA5 File Offset: 0x0014BCA5
	// (set) Token: 0x06003D96 RID: 15766 RVA: 0x0014DAAD File Offset: 0x0014BCAD
	private bool sendReport
	{
		get
		{
			return this._sendReport;
		}
		set
		{
			if (!this._sendReport)
			{
				this._sendReport = true;
			}
		}
	}

	// Token: 0x1700058C RID: 1420
	// (get) Token: 0x06003D97 RID: 15767 RVA: 0x0014DABE File Offset: 0x0014BCBE
	// (set) Token: 0x06003D98 RID: 15768 RVA: 0x0014DAC6 File Offset: 0x0014BCC6
	private string suspiciousPlayerId
	{
		get
		{
			return this._suspiciousPlayerId;
		}
		set
		{
			if (this._suspiciousPlayerId == "")
			{
				this._suspiciousPlayerId = value;
			}
		}
	}

	// Token: 0x1700058D RID: 1421
	// (get) Token: 0x06003D99 RID: 15769 RVA: 0x0014DAE1 File Offset: 0x0014BCE1
	// (set) Token: 0x06003D9A RID: 15770 RVA: 0x0014DAE9 File Offset: 0x0014BCE9
	private string suspiciousPlayerName
	{
		get
		{
			return this._suspiciousPlayerName;
		}
		set
		{
			if (this._suspiciousPlayerName == "")
			{
				this._suspiciousPlayerName = value;
			}
		}
	}

	// Token: 0x1700058E RID: 1422
	// (get) Token: 0x06003D9B RID: 15771 RVA: 0x0014DB04 File Offset: 0x0014BD04
	// (set) Token: 0x06003D9C RID: 15772 RVA: 0x0014DB0C File Offset: 0x0014BD0C
	private string suspiciousReason
	{
		get
		{
			return this._suspiciousReason;
		}
		set
		{
			if (this._suspiciousReason == "")
			{
				this._suspiciousReason = value;
			}
		}
	}

	// Token: 0x06003D9D RID: 15773 RVA: 0x00018E08 File Offset: 0x00017008
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003D9E RID: 15774 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003D9F RID: 15775 RVA: 0x0014DB27 File Offset: 0x0014BD27
	public void SliceUpdate()
	{
		this.CheckReports();
	}

	// Token: 0x06003DA0 RID: 15776 RVA: 0x0014DB30 File Offset: 0x0014BD30
	private void Start()
	{
		if (MonkeAgent.instance == null)
		{
			MonkeAgent.instance = this;
		}
		else if (MonkeAgent.instance != this)
		{
			Object.Destroy(this);
		}
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerEnteredRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		RoomSystem.JoinedRoomEvent += delegate
		{
			this.cachedPlayerList = (NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0]);
		};
		this.logErrorCount = 0;
		Application.logMessageReceived += this.LogErrorCount;
	}

	// Token: 0x06003DA1 RID: 15777 RVA: 0x0014DBD4 File Offset: 0x0014BDD4
	private void OnApplicationPause(bool paused)
	{
		if (paused || !RoomSystem.JoinedRoom)
		{
			return;
		}
		this.lastServerTimestamp = NetworkSystem.Instance.SimTick;
		this.RefreshRPCs();
	}

	// Token: 0x06003DA2 RID: 15778 RVA: 0x0014DBF8 File Offset: 0x0014BDF8
	public void LogErrorCount(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error)
		{
			this.logErrorCount++;
			this.stringIndex = logString.LastIndexOf("Sender is ");
			if (logString.Contains("RPC") && this.stringIndex >= 0)
			{
				this.playerID = logString.Substring(this.stringIndex + 10);
				this.tempPlayer = null;
				for (int i = 0; i < this.cachedPlayerList.Length; i++)
				{
					if (this.cachedPlayerList[i].UserId == this.playerID)
					{
						this.tempPlayer = this.cachedPlayerList[i];
						break;
					}
				}
				string text = "invalid RPC stuff";
				if (!this.IncrementRPCTracker(this.tempPlayer, text, this.rpcErrorMax))
				{
					this.SendReport("invalid RPC stuff", this.tempPlayer.UserId, this.tempPlayer.NickName);
				}
				this.tempPlayer = null;
			}
			if (this.logErrorCount > this.logErrorMax)
			{
				Debug.unityLogger.logEnabled = false;
			}
		}
	}

	// Token: 0x06003DA3 RID: 15779 RVA: 0x0014DCFC File Offset: 0x0014BEFC
	public void SendReport(string susReason, string susId, string susNick)
	{
		this.suspiciousReason = susReason;
		this.suspiciousPlayerId = susId;
		this.suspiciousPlayerName = susNick;
		this.sendReport = true;
	}

	// Token: 0x06003DA4 RID: 15780 RVA: 0x0014DD1C File Offset: 0x0014BF1C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void DispatchReport()
	{
		if ((this.sendReport || this.testAssault) && this.suspiciousPlayerId != "" && this.reportedPlayers.IndexOf(this.suspiciousPlayerId) == -1)
		{
			if (this._suspiciousPlayerName.Length > 12)
			{
				this._suspiciousPlayerName = this._suspiciousPlayerName.Remove(12);
			}
			this.reportedPlayers.Add(this.suspiciousPlayerId);
			this.testAssault = false;
			WebFlags flags = new WebFlags(3);
			NetEventOptions options = new NetEventOptions
			{
				TargetActors = MonkeAgent.targetActors,
				Reciever = NetEventOptions.RecieverTarget.master,
				Flags = flags
			};
			string[] array = new string[this.cachedPlayerList.Length];
			int num = 0;
			foreach (NetPlayer netPlayer in this.cachedPlayerList)
			{
				array[num] = netPlayer.UserId;
				num++;
			}
			object[] data = new object[]
			{
				NetworkSystem.Instance.RoomStringStripped(),
				array,
				NetworkSystem.Instance.MasterClient.UserId,
				this.suspiciousPlayerId,
				this.suspiciousPlayerName,
				this.suspiciousReason,
				NetworkSystemConfig.AppVersion
			};
			NetworkSystemRaiseEvent.RaiseEvent(8, data, options, true);
			if (this.ShouldDisconnectFromRoom())
			{
				base.StartCoroutine(this.QuitDelay(1f));
			}
		}
		this._sendReport = false;
		this._suspiciousPlayerId = "";
		this._suspiciousPlayerName = "";
		this._suspiciousReason = "";
	}

	// Token: 0x06003DA5 RID: 15781 RVA: 0x0014DEA4 File Offset: 0x0014C0A4
	private void CheckReports()
	{
		if (Time.time < this.lastCheck + this.reportCheckCooldown)
		{
			return;
		}
		this.lastCheck = Time.time;
		try
		{
			this.logErrorCount = 0;
			if (RoomSystem.JoinedRoom)
			{
				this.lastCheck = Time.time;
				this.lastServerTimestamp = NetworkSystem.Instance.SimTick;
				if (!PhotonNetwork.CurrentRoom.PublishUserId)
				{
					this.sendReport = true;
					this.suspiciousReason = "missing player ids";
					this.SetToRoomCreatorIfHere();
					this.CloseInvalidRoom();
				}
				if ((!RoomSystem.WasRoomSubscription && this.cachedPlayerList.Length > (int)RoomSystem.GetCurrentRoomExpectedSize()) || this.cachedPlayerList.Length > 20)
				{
					this.sendReport = true;
					this.suspiciousReason = "too many players";
					this.SetToRoomCreatorIfHere();
					this.CloseInvalidRoom();
				}
				if (this.currentMasterClient != NetworkSystem.Instance.MasterClient || this.LowestActorNumber() != NetworkSystem.Instance.MasterClient.ActorNumber)
				{
					foreach (NetPlayer netPlayer in this.cachedPlayerList)
					{
						if (this.currentMasterClient == netPlayer)
						{
							this.sendReport = true;
							this.suspiciousReason = "room host force changed";
							this.suspiciousPlayerId = NetworkSystem.Instance.MasterClient.UserId;
							this.suspiciousPlayerName = NetworkSystem.Instance.MasterClient.NickName;
						}
					}
					this.currentMasterClient = NetworkSystem.Instance.MasterClient;
				}
				this.RefreshRPCs();
				this.DispatchReport();
			}
		}
		catch
		{
		}
	}

	// Token: 0x06003DA6 RID: 15782 RVA: 0x0014E02C File Offset: 0x0014C22C
	private void RefreshRPCs()
	{
		foreach (Dictionary<string, MonkeAgent.RPCCallTracker> dictionary in this.userRPCCalls.Values)
		{
			foreach (MonkeAgent.RPCCallTracker rpccallTracker in dictionary.Values)
			{
				rpccallTracker.RPCCalls = 0;
			}
		}
	}

	// Token: 0x06003DA7 RID: 15783 RVA: 0x0014E0BC File Offset: 0x0014C2BC
	private int LowestActorNumber()
	{
		this.lowestActorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		foreach (NetPlayer netPlayer in this.cachedPlayerList)
		{
			if (netPlayer.ActorNumber < this.lowestActorNumber)
			{
				this.lowestActorNumber = netPlayer.ActorNumber;
			}
		}
		return this.lowestActorNumber;
	}

	// Token: 0x06003DA8 RID: 15784 RVA: 0x0014E117 File Offset: 0x0014C317
	public void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		this.cachedPlayerList = (NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0]);
	}

	// Token: 0x06003DA9 RID: 15785 RVA: 0x0014E134 File Offset: 0x0014C334
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.cachedPlayerList = (NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0]);
		Dictionary<string, MonkeAgent.RPCCallTracker> dictionary;
		if (this.userRPCCalls.TryGetValue(otherPlayer.UserId, out dictionary))
		{
			this.userRPCCalls.Remove(otherPlayer.UserId);
		}
	}

	// Token: 0x06003DAA RID: 15786 RVA: 0x0014E182 File Offset: 0x0014C382
	public static void IncrementRPCCall(PhotonMessageInfo info, [CallerMemberName] string callingMethod = "")
	{
		MonkeAgent.IncrementRPCCall(new PhotonMessageInfoWrapped(info), callingMethod);
	}

	// Token: 0x06003DAB RID: 15787 RVA: 0x0014E190 File Offset: 0x0014C390
	public static void IncrementRPCCall(PhotonMessageInfoWrapped infoWrapped, [CallerMemberName] string callingMethod = "")
	{
		MonkeAgent.instance.IncrementRPCCallLocal(infoWrapped, callingMethod);
	}

	// Token: 0x06003DAC RID: 15788 RVA: 0x0014E1A0 File Offset: 0x0014C3A0
	private void IncrementRPCCallLocal(PhotonMessageInfoWrapped infoWrapped, string rpcFunction)
	{
		if (infoWrapped.sentTick < this.lastServerTimestamp)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(infoWrapped.senderID);
		if (player == null)
		{
			return;
		}
		string userId = player.UserId;
		if (!this.IncrementRPCTracker(userId, rpcFunction, this.rpcCallLimit))
		{
			this.SendReport("too many rpc calls! " + rpcFunction, player.UserId, player.NickName);
			return;
		}
	}

	// Token: 0x06003DAD RID: 15789 RVA: 0x0014E208 File Offset: 0x0014C408
	private bool IncrementRPCTracker(in NetPlayer sender, in string rpcFunction, in int callLimit)
	{
		string userId = sender.UserId;
		return this.IncrementRPCTracker(userId, rpcFunction, callLimit);
	}

	// Token: 0x06003DAE RID: 15790 RVA: 0x0014E228 File Offset: 0x0014C428
	private bool IncrementRPCTracker(in Player sender, in string rpcFunction, in int callLimit)
	{
		string userId = sender.UserId;
		return this.IncrementRPCTracker(userId, rpcFunction, callLimit);
	}

	// Token: 0x06003DAF RID: 15791 RVA: 0x0014E248 File Offset: 0x0014C448
	private bool IncrementRPCTracker(in string userId, in string rpcFunction, in int callLimit)
	{
		MonkeAgent.RPCCallTracker rpccallTracker = this.GetRPCCallTracker(userId, rpcFunction);
		if (rpccallTracker == null)
		{
			return true;
		}
		rpccallTracker.RPCCalls++;
		if (rpccallTracker.RPCCalls > rpccallTracker.RPCCallsMax)
		{
			rpccallTracker.RPCCallsMax = rpccallTracker.RPCCalls;
		}
		return rpccallTracker.RPCCalls <= callLimit;
	}

	// Token: 0x06003DB0 RID: 15792 RVA: 0x0014E29C File Offset: 0x0014C49C
	private MonkeAgent.RPCCallTracker GetRPCCallTracker(string userID, string rpcFunction)
	{
		if (userID == null)
		{
			return null;
		}
		MonkeAgent.RPCCallTracker rpccallTracker = null;
		Dictionary<string, MonkeAgent.RPCCallTracker> dictionary;
		if (!this.userRPCCalls.TryGetValue(userID, out dictionary))
		{
			rpccallTracker = new MonkeAgent.RPCCallTracker
			{
				RPCCalls = 0,
				RPCCallsMax = 0
			};
			Dictionary<string, MonkeAgent.RPCCallTracker> dictionary2 = new Dictionary<string, MonkeAgent.RPCCallTracker>();
			dictionary2.Add(rpcFunction, rpccallTracker);
			this.userRPCCalls.Add(userID, dictionary2);
		}
		else if (!dictionary.TryGetValue(rpcFunction, out rpccallTracker))
		{
			rpccallTracker = new MonkeAgent.RPCCallTracker
			{
				RPCCalls = 0,
				RPCCallsMax = 0
			};
			dictionary.Add(rpcFunction, rpccallTracker);
		}
		return rpccallTracker;
	}

	// Token: 0x06003DB1 RID: 15793 RVA: 0x0014E319 File Offset: 0x0014C519
	private IEnumerator QuitDelay(float time = 1f)
	{
		yield return new WaitForSeconds(1f);
		NetworkSystem.Instance.ReturnToSinglePlayer();
		yield break;
	}

	// Token: 0x06003DB2 RID: 15794 RVA: 0x0014E324 File Offset: 0x0014C524
	private void SetToRoomCreatorIfHere()
	{
		this.tempPlayer = PhotonNetwork.CurrentRoom.GetPlayer(1, false);
		if (this.tempPlayer != null)
		{
			this.suspiciousPlayerId = this.tempPlayer.UserId;
			this.suspiciousPlayerName = this.tempPlayer.NickName;
			return;
		}
		this.suspiciousPlayerId = "n/a";
		this.suspiciousPlayerName = "n/a";
	}

	// Token: 0x06003DB3 RID: 15795 RVA: 0x0014E38C File Offset: 0x0014C58C
	private bool ShouldDisconnectFromRoom()
	{
		return this._suspiciousReason.Contains("too many players") || this._suspiciousReason.Contains("invalid room name") || this._suspiciousReason.Contains("invalid game mode") || this._suspiciousReason.Contains("missing player ids");
	}

	// Token: 0x06003DB4 RID: 15796 RVA: 0x0014E3E1 File Offset: 0x0014C5E1
	private void CloseInvalidRoom()
	{
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;
		PhotonNetwork.CurrentRoom.MaxPlayers = RoomSystem.GetCurrentRoomExpectedSize();
	}

	// Token: 0x04004DED RID: 19949
	[OnEnterPlay_SetNull]
	public static volatile MonkeAgent instance;

	// Token: 0x04004DEE RID: 19950
	private bool _sendReport;

	// Token: 0x04004DEF RID: 19951
	private string _suspiciousPlayerId = "";

	// Token: 0x04004DF0 RID: 19952
	private string _suspiciousPlayerName = "";

	// Token: 0x04004DF1 RID: 19953
	private string _suspiciousReason = "";

	// Token: 0x04004DF2 RID: 19954
	internal List<string> reportedPlayers = new List<string>();

	// Token: 0x04004DF3 RID: 19955
	public byte roomSize;

	// Token: 0x04004DF4 RID: 19956
	public float lastCheck;

	// Token: 0x04004DF5 RID: 19957
	public float userDecayTime = 15f;

	// Token: 0x04004DF6 RID: 19958
	public NetPlayer currentMasterClient;

	// Token: 0x04004DF7 RID: 19959
	public bool testAssault;

	// Token: 0x04004DF8 RID: 19960
	private const byte ReportAssault = 8;

	// Token: 0x04004DF9 RID: 19961
	private int lowestActorNumber;

	// Token: 0x04004DFA RID: 19962
	private int calls;

	// Token: 0x04004DFB RID: 19963
	public int rpcCallLimit = 50;

	// Token: 0x04004DFC RID: 19964
	public int logErrorMax = 50;

	// Token: 0x04004DFD RID: 19965
	public int rpcErrorMax = 10;

	// Token: 0x04004DFE RID: 19966
	private object outObj;

	// Token: 0x04004DFF RID: 19967
	private NetPlayer tempPlayer;

	// Token: 0x04004E00 RID: 19968
	private int logErrorCount;

	// Token: 0x04004E01 RID: 19969
	private int stringIndex;

	// Token: 0x04004E02 RID: 19970
	private string playerID;

	// Token: 0x04004E03 RID: 19971
	private string playerNick;

	// Token: 0x04004E04 RID: 19972
	private int lastServerTimestamp;

	// Token: 0x04004E05 RID: 19973
	private const string InvalidRPC = "invalid RPC stuff";

	// Token: 0x04004E06 RID: 19974
	public NetPlayer[] cachedPlayerList;

	// Token: 0x04004E07 RID: 19975
	private float lastReportChecked;

	// Token: 0x04004E08 RID: 19976
	private float reportCheckCooldown = 1f;

	// Token: 0x04004E09 RID: 19977
	private static int[] targetActors = new int[]
	{
		-1
	};

	// Token: 0x04004E0A RID: 19978
	private Dictionary<string, Dictionary<string, MonkeAgent.RPCCallTracker>> userRPCCalls = new Dictionary<string, Dictionary<string, MonkeAgent.RPCCallTracker>>();

	// Token: 0x04004E0B RID: 19979
	private Hashtable hashTable;

	// Token: 0x02000931 RID: 2353
	private class RPCCallTracker
	{
		// Token: 0x04004E0C RID: 19980
		public int RPCCalls;

		// Token: 0x04004E0D RID: 19981
		public int RPCCallsMax;
	}
}
