using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000891 RID: 2193
public class GorillaTagCompetitiveManager : GorillaTagManager
{
	// Token: 0x06003947 RID: 14663 RVA: 0x00138F74 File Offset: 0x00137174
	public float GetRoundDuration()
	{
		return this.roundDuration;
	}

	// Token: 0x06003948 RID: 14664 RVA: 0x00138F7C File Offset: 0x0013717C
	public GorillaTagCompetitiveManager.GameState GetCurrentGameState()
	{
		return this.gameState;
	}

	// Token: 0x06003949 RID: 14665 RVA: 0x00138F84 File Offset: 0x00137184
	public bool IsMatchActive()
	{
		return this.gameState == GorillaTagCompetitiveManager.GameState.Playing;
	}

	// Token: 0x14000065 RID: 101
	// (add) Token: 0x0600394A RID: 14666 RVA: 0x00138F90 File Offset: 0x00137190
	// (remove) Token: 0x0600394B RID: 14667 RVA: 0x00138FC4 File Offset: 0x001371C4
	public static event Action<GorillaTagCompetitiveManager.GameState> onStateChanged;

	// Token: 0x14000066 RID: 102
	// (add) Token: 0x0600394C RID: 14668 RVA: 0x00138FF8 File Offset: 0x001371F8
	// (remove) Token: 0x0600394D RID: 14669 RVA: 0x0013902C File Offset: 0x0013722C
	public static event Action<float> onUpdateRemainingTime;

	// Token: 0x14000067 RID: 103
	// (add) Token: 0x0600394E RID: 14670 RVA: 0x00139060 File Offset: 0x00137260
	// (remove) Token: 0x0600394F RID: 14671 RVA: 0x00139094 File Offset: 0x00137294
	public static event Action<NetPlayer> onPlayerJoined;

	// Token: 0x14000068 RID: 104
	// (add) Token: 0x06003950 RID: 14672 RVA: 0x001390C8 File Offset: 0x001372C8
	// (remove) Token: 0x06003951 RID: 14673 RVA: 0x001390FC File Offset: 0x001372FC
	public static event Action<NetPlayer> onPlayerLeft;

	// Token: 0x14000069 RID: 105
	// (add) Token: 0x06003952 RID: 14674 RVA: 0x00139130 File Offset: 0x00137330
	// (remove) Token: 0x06003953 RID: 14675 RVA: 0x00139164 File Offset: 0x00137364
	public static event Action onRoundStart;

	// Token: 0x1400006A RID: 106
	// (add) Token: 0x06003954 RID: 14676 RVA: 0x00139198 File Offset: 0x00137398
	// (remove) Token: 0x06003955 RID: 14677 RVA: 0x001391CC File Offset: 0x001373CC
	public static event Action onRoundEnd;

	// Token: 0x1400006B RID: 107
	// (add) Token: 0x06003956 RID: 14678 RVA: 0x00139200 File Offset: 0x00137400
	// (remove) Token: 0x06003957 RID: 14679 RVA: 0x00139234 File Offset: 0x00137434
	public static event Action<NetPlayer, NetPlayer> onTagOccurred;

	// Token: 0x06003958 RID: 14680 RVA: 0x00139267 File Offset: 0x00137467
	public static void RegisterScoreboard(GorillaTagCompetitiveScoreboard scoreboard)
	{
		GorillaTagCompetitiveManager.scoreboards.Add(scoreboard);
	}

	// Token: 0x06003959 RID: 14681 RVA: 0x00139274 File Offset: 0x00137474
	public static void DeregisterScoreboard(GorillaTagCompetitiveScoreboard scoreboard)
	{
		GorillaTagCompetitiveManager.scoreboards.Remove(scoreboard);
	}

	// Token: 0x0600395A RID: 14682 RVA: 0x00139284 File Offset: 0x00137484
	public override void StartPlaying()
	{
		base.StartPlaying();
		this.scoring = base.GetComponentInChildren<RankedMultiplayerScore>();
		if (this.scoring != null)
		{
			this.scoring.Initialize();
		}
		VRRig.LocalRig.EnableRankedTimerWatch(true);
		for (int i = 0; i < this.currentNetPlayerArray.Length; i++)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.currentNetPlayerArray[i], out rigContainer))
			{
				rigContainer.Rig.EnableRankedTimerWatch(true);
			}
		}
	}

	// Token: 0x0600395B RID: 14683 RVA: 0x001392FC File Offset: 0x001374FC
	public override void StopPlaying()
	{
		base.StopPlaying();
		VRRig.LocalRig.EnableRankedTimerWatch(false);
		if (this.scoring != null)
		{
			this.scoring.ResetMatch();
			this.scoring.Unsubscribe();
		}
		for (int i = 0; i < GorillaTagCompetitiveManager.scoreboards.Count; i++)
		{
			GorillaTagCompetitiveManager.scoreboards[i].UpdateScores(this.gameState, this.lastActiveTime, null, this.scoring.PlayerRankedTiers, this.scoring.ProjectedEloDeltas, this.currentInfected, this.scoring.Progression);
		}
	}

	// Token: 0x0600395C RID: 14684 RVA: 0x00139397 File Offset: 0x00137597
	public override void ResetGame()
	{
		base.ResetGame();
		this.gameState = GorillaTagCompetitiveManager.GameState.None;
	}

	// Token: 0x0600395D RID: 14685 RVA: 0x001393A6 File Offset: 0x001375A6
	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<GorillaTagCompetitiveRPCs>();
	}

	// Token: 0x0600395E RID: 14686 RVA: 0x001393B8 File Offset: 0x001375B8
	public override void Tick()
	{
		if (this.stateRemainingTime > 0f)
		{
			this.stateRemainingTime -= Time.deltaTime;
			if (this.stateRemainingTime <= 0f)
			{
				this.UpdateState();
			}
			Action<float> action = GorillaTagCompetitiveManager.onUpdateRemainingTime;
			if (action != null)
			{
				action(this.stateRemainingTime);
			}
		}
		base.Tick();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (Time.time - this.lastWaitingForPlayerPingRoomTime > this.waitingForPlayerPingRoomDuration)
			{
				this.PingRoom();
				this.lastWaitingForPlayerPingRoomTime = Time.time;
			}
			if (Time.time - this.lastWaitingForPlayerPingRoomTime > 3f)
			{
				this.ShowDebugPing = false;
			}
		}
		this.UpdateScoreboards();
	}

	// Token: 0x0600395F RID: 14687 RVA: 0x00139464 File Offset: 0x00137664
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.PingRoom();
			this.lastWaitingForPlayerPingRoomTime = Time.time;
		}
	}

	// Token: 0x06003960 RID: 14688 RVA: 0x0013948C File Offset: 0x0013768C
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (newPlayer == NetworkSystem.Instance.LocalPlayer)
		{
			using (List<GorillaTagCompetitiveForcedLeaveRoomVolume>.Enumerator enumerator = this.forceLeaveRoomVolumes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ContainsPoint(VRRig.LocalRig.transform.position))
					{
						NetworkSystem.Instance.ReturnToSinglePlayer();
						return;
					}
				}
			}
			object obj;
			if (NetworkSystem.Instance.IsMasterClient)
			{
				GorillaTagCompetitiveServerApi.Instance.RequestCreateMatchId(delegate(string id)
				{
					Hashtable hashtable = new Hashtable();
					hashtable.Add("matchId", id);
					PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
				});
			}
			else if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("matchId", out obj))
			{
				GorillaTagCompetitiveServerApi.Instance.RequestValidateMatchJoin((string)obj, delegate(bool valid)
				{
					if (!valid)
					{
						Debug.LogError("ValidateMatchJoin failed. Leaving room!");
						NetworkSystem.Instance.ReturnToSinglePlayer();
					}
				});
			}
		}
		Action<NetPlayer> action = GorillaTagCompetitiveManager.onPlayerJoined;
		if (action != null)
		{
			action(newPlayer);
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(newPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableRankedTimerWatch(true);
		}
	}

	// Token: 0x06003961 RID: 14689 RVA: 0x001395C0 File Offset: 0x001377C0
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		Action<NetPlayer> action = GorillaTagCompetitiveManager.onPlayerLeft;
		if (action != null)
		{
			action(otherPlayer);
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(otherPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableRankedTimerWatch(false);
		}
	}

	// Token: 0x06003962 RID: 14690 RVA: 0x00139600 File Offset: 0x00137800
	public RankedMultiplayerScore GetScoring()
	{
		return this.scoring;
	}

	// Token: 0x06003963 RID: 14691 RVA: 0x00139608 File Offset: 0x00137808
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return base.LocalCanTag(myPlayer, otherPlayer) && this.gameState != GorillaTagCompetitiveManager.GameState.StartingCountdown && this.gameState != GorillaTagCompetitiveManager.GameState.PostRound;
	}

	// Token: 0x06003964 RID: 14692 RVA: 0x0013962B File Offset: 0x0013782B
	public override bool LocalIsTagged(NetPlayer player)
	{
		return this.gameState != GorillaTagCompetitiveManager.GameState.StartingCountdown && this.gameState != GorillaTagCompetitiveManager.GameState.PostRound && base.LocalIsTagged(player);
	}

	// Token: 0x06003965 RID: 14693 RVA: 0x00139648 File Offset: 0x00137848
	public override void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		base.ReportTag(taggedPlayer, taggingPlayer);
	}

	// Token: 0x06003966 RID: 14694 RVA: 0x00139652 File Offset: 0x00137852
	public override GameModeType GameType()
	{
		return GameModeType.InfectionCompetitive;
	}

	// Token: 0x06003967 RID: 14695 RVA: 0x00139656 File Offset: 0x00137856
	public override string GameModeName()
	{
		return "COMP-INFECT";
	}

	// Token: 0x06003968 RID: 14696 RVA: 0x00139660 File Offset: 0x00137860
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_COMP_INF_ROOM_LABEL", out result, "(COMP-INFECT GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_COMP_INF_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x06003969 RID: 14697 RVA: 0x00002076 File Offset: 0x00000276
	public override bool CanJoinFrienship(NetPlayer player)
	{
		return false;
	}

	// Token: 0x0600396A RID: 14698 RVA: 0x0013968B File Offset: 0x0013788B
	public override void UpdateInfectionState()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (this.gameState == GorillaTagCompetitiveManager.GameState.Playing && this.IsEveryoneTagged())
		{
			this.HandleInfectionRoundComplete();
		}
	}

	// Token: 0x0600396B RID: 14699 RVA: 0x001396B4 File Offset: 0x001378B4
	public override void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (!this.currentInfected.Contains(taggingPlayer))
		{
			return;
		}
		RigContainer rigContainer;
		RigContainer rigContainer2;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer) && VRRigCache.Instance.TryGetVrrig(taggingPlayer, out rigContainer2))
		{
			VRRig rig = rigContainer2.Rig;
			VRRig rig2 = rigContainer.Rig;
			if (!rig.IsPositionInRange(rig2.transform.position, 6f) && !rig.CheckTagDistanceRollback(rig2, 6f, 0.2f))
			{
				return;
			}
			if (!NetworkSystem.Instance.IsMasterClient && this.gameState == GorillaTagCompetitiveManager.GameState.Playing && !this.currentInfected.Contains(taggedPlayer))
			{
				base.AddLastTagged(taggedPlayer, taggingPlayer);
				this.currentInfected.Add(taggedPlayer);
			}
			Action<NetPlayer, NetPlayer> action = GorillaTagCompetitiveManager.onTagOccurred;
			if (action == null)
			{
				return;
			}
			action(taggedPlayer, taggingPlayer);
		}
	}

	// Token: 0x0600396C RID: 14700 RVA: 0x00139778 File Offset: 0x00137978
	private void SetState(GorillaTagCompetitiveManager.GameState newState)
	{
		if (newState != this.gameState)
		{
			GorillaTagCompetitiveManager.GameState gameState = this.gameState;
			this.gameState = newState;
			switch (this.gameState)
			{
			case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
				this.EnterStateWaitingForPlayers();
				break;
			case GorillaTagCompetitiveManager.GameState.StartingCountdown:
				this.EnterStateStartingCountdown();
				break;
			case GorillaTagCompetitiveManager.GameState.Playing:
				this.EnterStatePlaying();
				break;
			case GorillaTagCompetitiveManager.GameState.PostRound:
				this.EnterStatePostRound();
				break;
			}
			Action<GorillaTagCompetitiveManager.GameState> action = GorillaTagCompetitiveManager.onStateChanged;
			if (action != null)
			{
				action(this.gameState);
			}
			Action<float> action2 = GorillaTagCompetitiveManager.onUpdateRemainingTime;
			if (action2 != null)
			{
				action2(this.stateRemainingTime);
			}
			if (this.gameState == GorillaTagCompetitiveManager.GameState.Playing)
			{
				Action action3 = GorillaTagCompetitiveManager.onRoundStart;
				if (action3 != null)
				{
					action3();
				}
			}
			else if (gameState == GorillaTagCompetitiveManager.GameState.Playing)
			{
				Action action4 = GorillaTagCompetitiveManager.onRoundEnd;
				if (action4 != null)
				{
					action4();
				}
			}
			GTDev.Log<string>(string.Format("!! Competitive SetState: {0} at: {1}", this.gameState, Time.time), null);
		}
	}

	// Token: 0x0600396D RID: 14701 RVA: 0x0013985E File Offset: 0x00137A5E
	private void EnterStateWaitingForPlayers()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			base.SetisCurrentlyTag(true);
			base.ClearInfectionState();
		}
	}

	// Token: 0x0600396E RID: 14702 RVA: 0x0013987C File Offset: 0x00137A7C
	private void EnterStateStartingCountdown()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.isCurrentlyTag)
			{
				base.SetisCurrentlyTag(false);
			}
			this.currentIt = null;
			base.ClearInfectionState();
			GameMode.RefreshPlayers();
			this.CheckForInfected();
			this.stateRemainingTime = this.startCountdownDuration;
		}
	}

	// Token: 0x0600396F RID: 14703 RVA: 0x001398C8 File Offset: 0x00137AC8
	private void EnterStatePlaying()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.isCurrentlyTag)
			{
				base.SetisCurrentlyTag(false);
			}
			this.currentIt = null;
			this.stateRemainingTime = this.roundDuration;
			this.PingRoom();
		}
		this.DisplayScoreboardPredictedResults(false);
	}

	// Token: 0x06003970 RID: 14704 RVA: 0x00139905 File Offset: 0x00137B05
	private void EnterStatePostRound()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.isCurrentlyTag)
			{
				base.SetisCurrentlyTag(false);
			}
			this.currentIt = null;
			this.stateRemainingTime = this.postRoundDuration;
		}
		this.DisplayScoreboardPredictedResults(true);
	}

	// Token: 0x06003971 RID: 14705 RVA: 0x0013993C File Offset: 0x00137B3C
	public override void UpdateState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			switch (this.gameState)
			{
			case GorillaTagCompetitiveManager.GameState.None:
				this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
				return;
			case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
				this.UpdateStateWaitingForPlayers();
				return;
			case GorillaTagCompetitiveManager.GameState.StartingCountdown:
				this.UpdateStateStartingCountdown();
				return;
			case GorillaTagCompetitiveManager.GameState.Playing:
				this.UpdateStatePlaying();
				return;
			case GorillaTagCompetitiveManager.GameState.PostRound:
				this.UpdateStatePostRound();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06003972 RID: 14706 RVA: 0x0013999C File Offset: 0x00137B9C
	private void UpdateStateWaitingForPlayers()
	{
		if (this.IsInfectionPossible())
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.StartingCountdown);
			return;
		}
		if (this.isCurrentlyTag && this.currentIt == null)
		{
			int index = Random.Range(0, GameMode.ParticipatingPlayers.Count);
			this.ChangeCurrentIt(GameMode.ParticipatingPlayers[index], false);
		}
	}

	// Token: 0x06003973 RID: 14707 RVA: 0x001399EC File Offset: 0x00137BEC
	private void UpdateStateStartingCountdown()
	{
		if (!this.IsInfectionPossible())
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
			return;
		}
		if (this.stateRemainingTime < 0f)
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.Playing);
			return;
		}
		this.CheckForInfected();
	}

	// Token: 0x06003974 RID: 14708 RVA: 0x00139A19 File Offset: 0x00137C19
	private void UpdateStatePlaying()
	{
		if (this.IsGameInvalid())
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
			return;
		}
		if (this.stateRemainingTime < 0f)
		{
			this.HandleInfectionRoundComplete();
			return;
		}
		if (this.IsEveryoneTagged())
		{
			this.HandleInfectionRoundComplete();
			return;
		}
		this.CheckForInfected();
	}

	// Token: 0x06003975 RID: 14709 RVA: 0x00139A54 File Offset: 0x00137C54
	private void HandleInfectionRoundComplete()
	{
		foreach (NetPlayer player in GameMode.ParticipatingPlayers)
		{
			RoomSystem.SendSoundEffectToPlayer(2, 0.25f, player, true);
		}
		PlayerGameEvents.GameModeCompleteRound();
		GameMode.BroadcastRoundComplete();
		this.lastTaggedActorNr.Clear();
		this.waitingToStartNextInfectionGame = true;
		this.timeInfectedGameEnded = (double)Time.time;
		this.SetState(GorillaTagCompetitiveManager.GameState.PostRound);
	}

	// Token: 0x06003976 RID: 14710 RVA: 0x00139ADC File Offset: 0x00137CDC
	private void UpdateStatePostRound()
	{
		if (this.stateRemainingTime < 0f)
		{
			if (this.IsInfectionPossible())
			{
				this.SetState(GorillaTagCompetitiveManager.GameState.StartingCountdown);
				return;
			}
			this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
		}
	}

	// Token: 0x06003977 RID: 14711 RVA: 0x00139B04 File Offset: 0x00137D04
	private void PingRoom()
	{
		object obj;
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("matchId", out obj))
		{
			GorillaTagCompetitiveServerApi.Instance.RequestPingRoom((string)obj, delegate
			{
				this.ShowDebugPing = true;
			});
		}
	}

	// Token: 0x17000518 RID: 1304
	// (get) Token: 0x06003978 RID: 14712 RVA: 0x00139B45 File Offset: 0x00137D45
	// (set) Token: 0x06003979 RID: 14713 RVA: 0x00139B4D File Offset: 0x00137D4D
	public bool ShowDebugPing { get; set; }

	// Token: 0x0600397A RID: 14714 RVA: 0x00139B56 File Offset: 0x00137D56
	private bool IsGameInvalid()
	{
		return GameMode.ParticipatingPlayers.Count <= 1;
	}

	// Token: 0x0600397B RID: 14715 RVA: 0x00139B68 File Offset: 0x00137D68
	private bool IsInfectionPossible()
	{
		return GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold;
	}

	// Token: 0x0600397C RID: 14716 RVA: 0x00139B80 File Offset: 0x00137D80
	private bool IsEveryoneTagged()
	{
		bool result = true;
		foreach (NetPlayer item in GameMode.ParticipatingPlayers)
		{
			if (!this.currentInfected.Contains(item))
			{
				result = false;
				break;
			}
		}
		return result;
	}

	// Token: 0x0600397D RID: 14717 RVA: 0x00139BE0 File Offset: 0x00137DE0
	private void CheckForInfected()
	{
		if (this.currentInfected.Count == 0)
		{
			int index = Random.Range(0, GameMode.ParticipatingPlayers.Count);
			this.AddInfectedPlayer(GameMode.ParticipatingPlayers[index], true);
		}
	}

	// Token: 0x0600397E RID: 14718 RVA: 0x00139C1D File Offset: 0x00137E1D
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeWrite(stream, info);
		stream.SendNext(this.gameState);
		stream.SendNext(this.stateRemainingTime);
	}

	// Token: 0x0600397F RID: 14719 RVA: 0x00139C4C File Offset: 0x00137E4C
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		base.OnSerializeRead(stream, info);
		GorillaTagCompetitiveManager.GameState state = (GorillaTagCompetitiveManager.GameState)stream.ReceiveNext();
		this.stateRemainingTime = (float)stream.ReceiveNext();
		this.SetState(state);
	}

	// Token: 0x06003980 RID: 14720 RVA: 0x00139C98 File Offset: 0x00137E98
	public void UpdateScoreboards()
	{
		List<RankedMultiplayerScore.PlayerScoreInRound> sortedScores = this.scoring.GetSortedScores();
		if (this.gameState == GorillaTagCompetitiveManager.GameState.Playing)
		{
			this.lastActiveTime = Time.time;
		}
		for (int i = 0; i < GorillaTagCompetitiveManager.scoreboards.Count; i++)
		{
			GorillaTagCompetitiveManager.scoreboards[i].UpdateScores(this.gameState, this.lastActiveTime, sortedScores, this.scoring.PlayerRankedTiers, this.scoring.ProjectedEloDeltas, this.currentInfected, this.scoring.Progression);
		}
	}

	// Token: 0x06003981 RID: 14721 RVA: 0x00139D20 File Offset: 0x00137F20
	public void DisplayScoreboardPredictedResults(bool bShow)
	{
		for (int i = 0; i < GorillaTagCompetitiveManager.scoreboards.Count; i++)
		{
			GorillaTagCompetitiveManager.scoreboards[i].DisplayPredictedResults(bShow);
		}
	}

	// Token: 0x06003982 RID: 14722 RVA: 0x00139D53 File Offset: 0x00137F53
	public void RegisterForcedLeaveVolume(GorillaTagCompetitiveForcedLeaveRoomVolume volume)
	{
		if (!this.forceLeaveRoomVolumes.Contains(volume))
		{
			this.forceLeaveRoomVolumes.Add(volume);
		}
	}

	// Token: 0x06003983 RID: 14723 RVA: 0x00139D6F File Offset: 0x00137F6F
	public void UnregisterForcedLeaveVolume(GorillaTagCompetitiveForcedLeaveRoomVolume volume)
	{
		this.forceLeaveRoomVolumes.Remove(volume);
	}

	// Token: 0x04004954 RID: 18772
	[SerializeField]
	private float startCountdownDuration = 3f;

	// Token: 0x04004955 RID: 18773
	[SerializeField]
	private float roundDuration = 300f;

	// Token: 0x04004956 RID: 18774
	[SerializeField]
	private float postRoundDuration = 15f;

	// Token: 0x04004957 RID: 18775
	[SerializeField]
	private float waitingForPlayerPingRoomDuration = 60f;

	// Token: 0x04004958 RID: 18776
	private GorillaTagCompetitiveManager.GameState gameState;

	// Token: 0x04004959 RID: 18777
	private float stateRemainingTime;

	// Token: 0x0400495A RID: 18778
	private float lastActiveTime;

	// Token: 0x0400495B RID: 18779
	private float lastWaitingForPlayerPingRoomTime;

	// Token: 0x04004963 RID: 18787
	private RankedMultiplayerScore scoring;

	// Token: 0x04004964 RID: 18788
	private List<GorillaTagCompetitiveForcedLeaveRoomVolume> forceLeaveRoomVolumes = new List<GorillaTagCompetitiveForcedLeaveRoomVolume>();

	// Token: 0x04004965 RID: 18789
	private static List<GorillaTagCompetitiveScoreboard> scoreboards = new List<GorillaTagCompetitiveScoreboard>();

	// Token: 0x02000892 RID: 2194
	public enum GameState
	{
		// Token: 0x04004968 RID: 18792
		None,
		// Token: 0x04004969 RID: 18793
		WaitingForPlayers,
		// Token: 0x0400496A RID: 18794
		StartingCountdown,
		// Token: 0x0400496B RID: 18795
		Playing,
		// Token: 0x0400496C RID: 18796
		PostRound
	}
}
