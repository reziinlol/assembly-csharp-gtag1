using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x0200070D RID: 1805
public class GhostReactorShiftManager : MonoBehaviourTick
{
	// Token: 0x17000462 RID: 1122
	// (get) Token: 0x06002DD8 RID: 11736 RVA: 0x000FA954 File Offset: 0x000F8B54
	public int ShiftTotalEarned
	{
		get
		{
			return this.shiftTotalEarned;
		}
	}

	// Token: 0x17000463 RID: 1123
	// (get) Token: 0x06002DD9 RID: 11737 RVA: 0x000FA95C File Offset: 0x000F8B5C
	public bool ShiftActive
	{
		get
		{
			return this.shiftStarted;
		}
	}

	// Token: 0x17000464 RID: 1124
	// (get) Token: 0x06002DDA RID: 11738 RVA: 0x000FA964 File Offset: 0x000F8B64
	public double ShiftStartNetworkTime
	{
		get
		{
			return this.shiftStartNetworkTime;
		}
	}

	// Token: 0x17000465 RID: 1125
	// (get) Token: 0x06002DDB RID: 11739 RVA: 0x000FA96C File Offset: 0x000F8B6C
	public bool LocalPlayerInside
	{
		get
		{
			return this.localPlayerInside;
		}
	}

	// Token: 0x17000466 RID: 1126
	// (get) Token: 0x06002DDC RID: 11740 RVA: 0x000FA974 File Offset: 0x000F8B74
	public float TotalPlayTime
	{
		get
		{
			return this.totalPlayTime;
		}
	}

	// Token: 0x17000467 RID: 1127
	// (get) Token: 0x06002DDD RID: 11741 RVA: 0x000FA97C File Offset: 0x000F8B7C
	public string ShiftId
	{
		get
		{
			return this.gameIdGuid;
		}
	}

	// Token: 0x06002DDE RID: 11742 RVA: 0x000FA984 File Offset: 0x000F8B84
	public void SetShiftId(string shiftId)
	{
		this.gameIdGuid = shiftId;
	}

	// Token: 0x17000468 RID: 1128
	// (get) Token: 0x06002DDF RID: 11743 RVA: 0x000FA98D File Offset: 0x000F8B8D
	// (set) Token: 0x06002DE0 RID: 11744 RVA: 0x000FA995 File Offset: 0x000F8B95
	public GhostReactorShiftManager.State ShiftState { get; private set; }

	// Token: 0x06002DE1 RID: 11745 RVA: 0x000FA99E File Offset: 0x000F8B9E
	public void Init(GhostReactorManager grManager)
	{
		this.grManager = grManager;
		this.SetState(GhostReactorShiftManager.State.WaitingForConnect, true);
		this.depthDisplay.Setup();
	}

	// Token: 0x06002DE2 RID: 11746 RVA: 0x000FA9BC File Offset: 0x000F8BBC
	public void RefreshShiftStatsDisplay()
	{
		this.shiftStatsText.text = string.Concat(new string[]
		{
			"\n\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths).ToString("D2"),
			"\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.CoresCollected).ToString("D2"),
			"\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.SentientCoresCollected).ToString("D2"),
			"\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.PlayerDeaths).ToString("D2")
		});
		this.depthDisplay.RefreshObjectives();
	}

	// Token: 0x06002DE3 RID: 11747 RVA: 0x000FAA7A File Offset: 0x000F8C7A
	public void StartShiftButtonPressed()
	{
		this.RequestShiftStart();
	}

	// Token: 0x06002DE4 RID: 11748 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void RequestShiftStart()
	{
	}

	// Token: 0x06002DE5 RID: 11749 RVA: 0x000FAA82 File Offset: 0x000F8C82
	public void EndShift()
	{
		this.grManager.SendRequestShiftEndRPC();
	}

	// Token: 0x06002DE6 RID: 11750 RVA: 0x000FAA8F File Offset: 0x000F8C8F
	public void ClearEntities()
	{
		Debug.LogError("Need to re-implement whatever this was doing");
	}

	// Token: 0x06002DE7 RID: 11751 RVA: 0x000FAA9C File Offset: 0x000F8C9C
	public void RefreshShiftTimer()
	{
		if (this.shiftTimerText != null)
		{
			this.shiftTimerText.text = Mathf.FloorToInt(this.shiftDurationMinutes).ToString("D2") + ":00";
		}
	}

	// Token: 0x06002DE8 RID: 11752 RVA: 0x000FAAE4 File Offset: 0x000F8CE4
	public void UpdateLogoAnimations(List<TMP_Text> frames)
	{
		float num = 300f;
		float num2 = 0.5f;
		double time = PhotonNetwork.Time;
		if (frames.Count < 4)
		{
			return;
		}
		if (this.lastReactorLogoAnimationTime + (double)num < time || time < this.lastReactorLogoAnimationTime)
		{
			this.isPlayingLogoAnimation = true;
			this.lastReactorLogoAnimationTime = time;
		}
		if (this.isPlayingLogoAnimation)
		{
			if (this.lastReactorLogoAnimationTime + (double)num2 < time)
			{
				this.isPlayingLogoAnimation = false;
			}
			float f = Mathf.Clamp01((float)(time - this.lastReactorLogoAnimationTime) / num2) * 3.1415925f;
			int num3 = (int)(3.5f - Mathf.Abs(Mathf.Cos(f) * 3f));
			if (!this.isPlayingLogoAnimation)
			{
				num3 = 0;
			}
			if (this.lastReactorLogoAnimFrame != num3)
			{
				frames[this.lastReactorLogoAnimFrame].gameObject.SetActive(false);
				frames[num3].gameObject.SetActive(true);
				this.lastReactorLogoAnimFrame = num3;
			}
			return;
		}
	}

	// Token: 0x06002DE9 RID: 11753 RVA: 0x000FABC8 File Offset: 0x000F8DC8
	public void UpdateReactorDisplayMainShared(float countDownTotal)
	{
		if (this.reactorTextMain == null)
		{
			return;
		}
		double time = PhotonNetwork.Time;
		float num = 0.5f;
		if (this.lastReactorDisplayUpdate < time && this.lastReactorDisplayUpdate + (double)num > time)
		{
			return;
		}
		this.lastReactorDisplayUpdate = time;
		this.cachedStringBuilder.Clear();
		int num2 = Mathf.FloorToInt(countDownTotal / 60f);
		int num3 = Mathf.FloorToInt(countDownTotal % 60f);
		switch (this.ShiftState)
		{
		case GhostReactorShiftManager.State.WaitingForShiftStart:
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
			this.cachedStringBuilder.AppendLine(string.Format("DEPTH {0}m", this.reactor.GetDepthLevel() * 1000 + 1000));
			this.cachedStringBuilder.AppendLine("STAND BY");
			this.depthDisplay.jumbotronTitle.text = string.Format("<size=1>CURRENT DEPTH</size>\n{0}m", this.reactor.GetDepthLevel() * 1000 + 1000);
			break;
		case GhostReactorShiftManager.State.ShiftActive:
		{
			int shiftStat = this.shiftStats.GetShiftStat(GRShiftStatType.CoresCollected);
			int num4 = this.coresRequiredToDelveDeeper;
			this.depthDisplay.jumbotronTitle.text = string.Format("<size=1>CURRENT DEPTH</size>\n{0}m", this.reactor.GetDepthLevel() * 1000 + 1000);
			this.cachedStringBuilder.AppendLine(string.Format("DEPTH {0}m", this.reactor.GetDepthLevel() * 1000 + 1000));
			this.cachedStringBuilder.AppendLine("ANOMALY COLLAPSE IN " + num2.ToString("D2") + ":" + num3.ToString("D2"));
			if (shiftStat >= num4)
			{
				this.cachedStringBuilder.Append("\nPOWER REQUIREMENTS MET\n");
			}
			else
			{
				this.cachedStringBuilder.Append(string.Format("\nCORES REQUIRED ({0}/{1})\n", shiftStat, num4));
			}
			int num5 = (int)((float)shiftStat / (float)num4 * 30f);
			if (shiftStat > 1 && num5 == 0)
			{
				num5 = 1;
			}
			int num6 = num5 / 3;
			int num7 = num5 - num6 * 3;
			for (int i = 0; i < 10; i++)
			{
				if (i < num6)
				{
					this.cachedStringBuilder.Append("▐█");
				}
				else if (i > num6 || num7 == 0)
				{
					this.cachedStringBuilder.Append(" ░");
				}
				else if (num7 == 1)
				{
					this.cachedStringBuilder.Append("▐░");
				}
				else
				{
					this.cachedStringBuilder.Append("▐▌");
				}
			}
			this.cachedStringBuilder.Append("\n");
			if (shiftStat > 0)
			{
				this.cachedStringBuilder.Append(string.Format("\nTOTAL BONUS EARNED: +⑭{0}", shiftStat * 5));
			}
			break;
		}
		case GhostReactorShiftManager.State.PreparingToDrill:
			this.cachedStringBuilder.AppendLine(string.Format("DEPTH {0}m", this.reactor.GetDepthLevel() * 1000));
			this.cachedStringBuilder.AppendLine("STAND BY");
			this.depthDisplay.jumbotronTitle.text = string.Format("<size=1>CURRENT DEPTH</size>\n{0}m", this.reactor.GetDepthLevel() * 1000 + 1000);
			break;
		case GhostReactorShiftManager.State.Drilling:
		{
			int num8 = (int)((time - this.stateStartTime) / (double)this.GetDrillingDuration() * 1000.0);
			this.cachedStringBuilder.AppendLine(string.Format("DEPTH {0}m", this.reactor.GetDepthLevel() * 1000 + num8));
			this.cachedStringBuilder.AppendLine("DRILLING");
			this.depthDisplay.jumbotronTitle.text = string.Format("<size=1>CURRENT DEPTH</size>\n{0}m", this.reactor.GetDepthLevel() * 1000 + num8);
			break;
		}
		}
		this.reactorTextMain.text = this.cachedStringBuilder.ToString();
	}

	// Token: 0x06002DEA RID: 11754 RVA: 0x000FAFC4 File Offset: 0x000F91C4
	public void OnShiftStarted(string gameId, double shiftStartTime, bool wasPlayerInAtStart, bool isFirstShift)
	{
		this.gameIdGuid = gameId;
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (!this.shiftStarted && grplayer != null)
		{
			float num = (float)(PhotonNetwork.Time - shiftStartTime);
			grplayer.ResetTelemetryTracking(this.gameIdGuid, num);
			grplayer.IncrementShiftsPlayed(1);
			grplayer.SendFloorStartedTelemetry(num, wasPlayerInAtStart, this.reactor.GetDepthLevel(), this.reactor.GetCurrLevelGenConfig().name, "");
			if (grplayer.isFirstShift)
			{
				grplayer.SendGameStartedTelemetry(num, wasPlayerInAtStart, this.reactor.GetDepthLevel());
				grplayer.gameStartTime = (float)PhotonNetwork.Time;
			}
		}
		this.shiftStarted = true;
		this.shiftJustStarted = true;
		this.shiftStartNetworkTime = shiftStartTime;
		this.frontGate.OpenGate();
		this.ringTransform.gameObject.SetActive(false);
		this.anomalyLoop1.Stop();
		this.anomalyLoop2.Stop();
		this.anomalyLoop3.Stop();
		this.anomalyAlert.Stop();
		this.gateBlockerTransform.gameObject.SetActive(false);
		this.prevCountDownTotal = this.shiftDurationMinutes * 60f;
		this.shiftTotalEarned = -1;
		this.authorizedToDelveDeeper = false;
		this.ResetJoinTimes();
		this.reactor.RefreshScoreboards();
		this.reactor.RefreshDepth();
		this.isRoomClosed = false;
		if (grplayer != null)
		{
			grplayer.RefreshPlayerVisuals();
		}
	}

	// Token: 0x06002DEB RID: 11755 RVA: 0x000FB120 File Offset: 0x000F9320
	public void OnShiftEnded(double shiftEndTime, bool isShiftActuallyEnding, ZoneClearReason zoneClearReason = ZoneClearReason.JoinZone)
	{
		if (this.shiftStarted)
		{
			GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
			if (component != null)
			{
				component.SendFloorEndedTelemetry(isShiftActuallyEnding, (float)this.shiftStartNetworkTime, zoneClearReason, this.reactor.GetDepthLevel(), this.reactor.GetCurrLevelGenConfig().name, "", this.authorizedToDelveDeeper, ((this.reactor.GetDepthLevel() + 1) / 5).ToString(), this.authorizedToDelveDeeper ? (10 * this.reactor.GetDepthLevel()) : 0);
			}
		}
		this.shiftStarted = false;
		this.shiftEndNetworkTime = shiftEndTime;
		this.RefreshShiftTimer();
		this.frontGate.CloseGate();
		this.ringTransform.gameObject.SetActive(false);
		this.anomalyLoop1.Stop();
		this.anomalyLoop2.Stop();
		this.anomalyLoop3.Stop();
		this.anomalyAlert.Stop();
		this.TeleportLocalPlayerIfOutOfBounds();
		if (this.shiftEndNetworkTime > 0.0 && this.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths) > this.shiftStats.GetShiftStat(GRShiftStatType.PlayerDeaths))
		{
			PlayerGameEvents.MiscEvent("GRShiftGoodKD", 1);
		}
		if (PhotonNetwork.InRoom && !NetworkSystem.Instance.SessionIsPrivate && this.grManager.IsAuthority())
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("ghostReactorShiftStarted", "false");
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
			this.isRoomClosed = false;
		}
	}

	// Token: 0x06002DEC RID: 11756 RVA: 0x000FB294 File Offset: 0x000F9494
	public override void Tick()
	{
		if (this.grManager == null)
		{
			return;
		}
		double num = PhotonNetwork.Time - this.shiftStartNetworkTime;
		float num2 = 60f * this.shiftDurationMinutes - (float)num;
		if (this.grManager.IsAuthority())
		{
			this.AuthorityUpdate(num2);
		}
		num2 = Mathf.Clamp(num2, 0f, 60f * this.shiftDurationMinutes);
		this.SharedUpdate(num2);
		this.prevCountDownTotal = num2;
	}

	// Token: 0x06002DED RID: 11757 RVA: 0x000FB308 File Offset: 0x000F9508
	private void AuthorityUpdate(float countDownTotal)
	{
		if (PhotonNetwork.InRoom && this.grManager.IsAuthority())
		{
			if (this.shiftStarted && !NetworkSystem.Instance.SessionIsPrivate && !this.isRoomClosed && 60f * this.shiftDurationMinutes - countDownTotal >= this.roomCloseTimeSeconds)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("ghostReactorShiftStarted", "true");
				PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
				this.isRoomClosed = true;
			}
			if (this.shiftStarted && countDownTotal <= 0f)
			{
				this.grManager.RequestShiftEnd();
			}
			this.UpdateStateAuthority();
		}
	}

	// Token: 0x06002DEE RID: 11758 RVA: 0x000FB3AC File Offset: 0x000F95AC
	private void SharedUpdate(float countDownTotal)
	{
		this.UpdateStateShared();
		this.UpdateReactorDisplayMainShared(countDownTotal);
		if (this.lastLeaderboardRefreshTime + (double)this.leaderboardUpdateFrequency < (double)Time.time || (double)Time.time < this.lastLeaderboardRefreshTime)
		{
			this.RefreshShiftLeaderboard();
			this.lastLeaderboardRefreshTime = (double)Time.time;
		}
		if (this.shiftStarted)
		{
			if (this.debugFastForwarding)
			{
				float num = this.debugFastForwardRate * Time.deltaTime;
				this.shiftStartNetworkTime -= (double)num;
			}
			int num2 = Mathf.FloorToInt(countDownTotal / 60f);
			int num3 = Mathf.FloorToInt(countDownTotal % 60f);
			this.shiftTimerText.text = num2.ToString("D2") + ":" + num3.ToString("D2");
			for (int i = 0; i < this.warnings.Count; i++)
			{
				if (countDownTotal < (float)this.warnings[i].time && this.prevCountDownTotal >= (float)this.warnings[i].time && !this.shiftJustStarted)
				{
					this.warnings[i].sound.Play(this.announceAudioSource);
					break;
				}
			}
			if (this.ShiftState == GhostReactorShiftManager.State.ShiftActive && countDownTotal > 0f && countDownTotal < this.anomalyAlertCountdownTimeToStartPlayingInMinutes * 60f && !this.anomalyAlert.isPlaying)
			{
				this.anomalyAlert.Play();
			}
			if (this.localPlayerInside)
			{
				if (countDownTotal >= 0f && countDownTotal < this.ringClosingDuration * 60f)
				{
					this.ringTransform.gameObject.SetActive(true);
					float num4 = Mathf.Lerp(this.ringClosingMinRadius, this.ringClosingMaxRadius, countDownTotal / (this.ringClosingDuration * 60f));
					this.ringTransform.localScale = new Vector3(num4, 1f, num4);
					Vector3 position = VRRig.LocalRig.bodyTransform.position;
					Vector3 vector = position - this.ringTransform.position;
					vector.y = 0f;
					Vector3 normalized = vector.normalized;
					float num5 = 0.5235988f;
					Vector3 position2 = this.ringTransform.position + normalized * num4;
					Quaternion rotation = Quaternion.AngleAxis(num5, Vector3.up);
					Quaternion rotation2 = Quaternion.AngleAxis(-num5, Vector3.up);
					Vector3 position3 = this.ringTransform.position + rotation * normalized * num4;
					Vector3 position4 = this.ringTransform.position + rotation2 * normalized * num4;
					position2.y = position.y;
					position3.y = position.y;
					position4.y = position.y;
					this.anomalyLoop1.transform.position = position2;
					this.anomalyLoop2.transform.position = position3;
					this.anomalyLoop3.transform.position = position4;
					if (!this.anomalyLoop1.isPlaying)
					{
						this.anomalyLoop1.Play();
					}
					if (!this.anomalyLoop2.isPlaying)
					{
						this.anomalyLoop2.Play();
					}
					if (!this.anomalyLoop3.isPlaying)
					{
						this.anomalyLoop3.Play();
					}
					if (vector.sqrMagnitude > num4 * num4)
					{
						this.TeleportLocalPlayerIfOutOfBounds();
					}
				}
			}
			else if (this.ringTransform.gameObject.activeSelf)
			{
				this.ringTransform.gameObject.SetActive(false);
			}
			this.shiftJustStarted = false;
			return;
		}
		if (!this.shiftStarted)
		{
			this.TeleportLocalPlayerIfOutOfBounds();
		}
	}

	// Token: 0x06002DEF RID: 11759 RVA: 0x000FB738 File Offset: 0x000F9938
	private void TeleportLocalPlayerIfOutOfBounds()
	{
		if (this.localPlayerInside || (this.localPlayerOverlapping && Vector3.Dot(GTPlayer.Instance.headCollider.transform.position - this.gatePlaneTransform.position, this.gatePlaneTransform.forward) < 0f))
		{
			this.grManager.ReportLocalPlayerHit();
			GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
			component.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, this.grManager);
			GTPlayer.Instance.TeleportTo(this.playerTeleportTransform, true, true);
			this.localPlayerInside = false;
			this.localPlayerOverlapping = false;
			component.caughtByAnomaly = true;
		}
	}

	// Token: 0x06002DF0 RID: 11760 RVA: 0x000FB7DC File Offset: 0x000F99DC
	public void RevealJudgment(int evaluation)
	{
		if (evaluation <= 0)
		{
			this.shiftJugmentText.text = "DON'T QUIT YOUR DAY JOB.";
			return;
		}
		switch (evaluation)
		{
		case 1:
			this.shiftJugmentText.text = "YOU'RE LEARNING. GOOD.";
			return;
		case 2:
			this.shiftJugmentText.text = "YOU MIGHT EARN A PROMOTION.";
			return;
		case 3:
			this.shiftJugmentText.text = "YOU DID A MANAGER-TIER JOB.";
			return;
		case 4:
			this.shiftJugmentText.text = "NICE. YOU GET EXTRA SHIFTS.";
			return;
		default:
			this.shiftJugmentText.text = "YOU WORK FOR US NOW.";
			if (this.wrongStumpGoo != null)
			{
				this.wrongStumpGoo.SetActive(true);
			}
			return;
		}
	}

	// Token: 0x06002DF1 RID: 11761 RVA: 0x000FB886 File Offset: 0x000F9A86
	public void ResetJudgment()
	{
		this.shiftJugmentText.text = "";
		if (this.wrongStumpGoo != null)
		{
			this.wrongStumpGoo.SetActive(false);
		}
	}

	// Token: 0x06002DF2 RID: 11762 RVA: 0x000FB8B4 File Offset: 0x000F9AB4
	public void ResetJoinTimes()
	{
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		for (int i = 0; i < count; i++)
		{
			GRPlayer.Get(this.reactor.vrRigs[i]).shiftJoinTime = this.shiftStartNetworkTime;
		}
	}

	// Token: 0x06002DF3 RID: 11763 RVA: 0x000FB90C File Offset: 0x000F9B0C
	public void CalculatePlayerPercentages()
	{
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (this.reactor.vrRigs[i] != null && grplayer != null)
			{
				if (this.reactor.vrRigs[i].OwningNetPlayer == null)
				{
					grplayer.ShiftPlayTime = 0.1f;
				}
				else if (this.shiftStarted)
				{
					grplayer.ShiftPlayTime = Mathf.Min(this.shiftDurationMinutes * 60f, (float)(PhotonNetwork.Time - grplayer.shiftJoinTime));
				}
				else
				{
					grplayer.ShiftPlayTime = Mathf.Min(this.shiftDurationMinutes * 60f, (float)(this.shiftEndNetworkTime - grplayer.shiftJoinTime));
				}
				this.totalPlayTime += grplayer.ShiftPlayTime;
			}
		}
	}

	// Token: 0x06002DF4 RID: 11764 RVA: 0x000FBA14 File Offset: 0x000F9C14
	public void CalculateShiftTotal()
	{
		this.shiftTotalEarned = 0;
		int count = this.reactor.vrRigs.Count;
		double num = 0.0;
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (this.reactor.vrRigs[i] != null && grplayer != null)
			{
				this.shiftTotalEarned += grplayer.ShiftCredits;
				if (this.reactor.vrRigs[i].OwningNetPlayer == null)
				{
					grplayer.ShiftPlayTime = 0.1f;
				}
				else
				{
					grplayer.ShiftPlayTime = Mathf.Min(this.shiftDurationMinutes * 60f, (float)(PhotonNetwork.Time - grplayer.shiftJoinTime));
				}
				num += (double)grplayer.ShiftPlayTime;
			}
		}
		this.shiftTotalEarned = Mathf.Clamp(this.shiftTotalEarned, 0, this.shiftSanityMaximumEarned);
		num = (double)Mathf.Clamp((float)num, 0.1f, this.shiftDurationMinutes * 10f * 60f);
		for (int j = 0; j < count; j++)
		{
			GRPlayer grplayer2 = GRPlayer.Get(this.reactor.vrRigs[j]);
			if (this.reactor.vrRigs[j] != null && grplayer2 != null && this.depthDisplay != null)
			{
				int rewardXP = this.depthDisplay.GetRewardXP();
				if (this.authorizedToDelveDeeper)
				{
					grplayer2.LastShiftCut = rewardXP;
					grplayer2.CollectShiftCut();
				}
			}
		}
		this.reactor.RefreshScoreboards();
		this.reactor.promotionBot.Refresh();
		this.reactor.RefreshDepth();
	}

	// Token: 0x06002DF5 RID: 11765 RVA: 0x000FBBCC File Offset: 0x000F9DCC
	private void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			this.localPlayerOverlapping = true;
		}
	}

	// Token: 0x06002DF6 RID: 11766 RVA: 0x000FBBE8 File Offset: 0x000F9DE8
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			bool flag = Vector3.Dot(other.transform.position - this.gatePlaneTransform.position, this.gatePlaneTransform.forward) < 0f;
			this.localPlayerInside = flag;
			this.localPlayerOverlapping = false;
		}
	}

	// Token: 0x06002DF7 RID: 11767 RVA: 0x000FBC48 File Offset: 0x000F9E48
	public void OnButtonDelveDeeper()
	{
		if (this.ShiftActive)
		{
			bool flag = this.authorizedToDelveDeeper;
			return;
		}
	}

	// Token: 0x06002DF8 RID: 11768 RVA: 0x000FBC5A File Offset: 0x000F9E5A
	public void OnButtonDEBUGResetDepth()
	{
		this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DEBUG_ResetDepth);
	}

	// Token: 0x06002DF9 RID: 11769 RVA: 0x000FBC69 File Offset: 0x000F9E69
	public void OnButtonDEBUGDelveDeeper()
	{
		this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DEBUG_DelveDeeper);
	}

	// Token: 0x06002DFA RID: 11770 RVA: 0x000FBC78 File Offset: 0x000F9E78
	public void OnButtonDEBUGDelveShallower()
	{
		this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DEBUG_DelveShallower);
	}

	// Token: 0x06002DFB RID: 11771 RVA: 0x000FBC87 File Offset: 0x000F9E87
	public void RequestState(GhostReactorShiftManager.State newState)
	{
		if (!this.grManager.IsAuthority())
		{
			return;
		}
		this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DelveState, (int)newState);
	}

	// Token: 0x06002DFC RID: 11772 RVA: 0x000FBCA4 File Offset: 0x000F9EA4
	public void SetState(GhostReactorShiftManager.State newState, bool force = false)
	{
		if (this.ShiftState == newState && !force)
		{
			return;
		}
		GhostReactorShiftManager.State shiftState = this.ShiftState;
		if (shiftState != GhostReactorShiftManager.State.ReadyForShift)
		{
			if (shiftState == GhostReactorShiftManager.State.Drilling)
			{
				this.reactor.shiftManager.depthDisplay.StopDelveDeeperFX();
			}
		}
		else if (this.startShiftButton != null)
		{
			this.startShiftButton.SetActive(false);
		}
		this.ShiftState = newState;
		this.stateStartTime = PhotonNetwork.Time;
		switch (this.ShiftState)
		{
		case GhostReactorShiftManager.State.WaitingForShiftStart:
			this.announceBell.Play(this.announceBellAudioSource);
			this.announceTip.Play(this.announceAudioSource);
			goto IL_21F;
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
			break;
		case GhostReactorShiftManager.State.ReadyForShift:
			goto IL_21F;
		case GhostReactorShiftManager.State.ShiftActive:
			this.announceStartShift.Play(this.announceAudioSource);
			using (List<VRRig>.Enumerator enumerator = this.reactor.vrRigs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					VRRig vrrig = enumerator.Current;
					GRPlayer component = vrrig.GetComponent<GRPlayer>();
					if (component != null)
					{
						component.startingShiftCreditCache = component.ShiftCredits;
					}
				}
				goto IL_21F;
			}
			break;
		case GhostReactorShiftManager.State.PostShift:
			if (this.authorizedToDelveDeeper)
			{
				this.announceCompleteShift.Play(this.announceAudioSource);
				if (!string.IsNullOrEmpty(this.ShiftId))
				{
					ProgressionManager.Instance.EndOfShiftReward(this.ShiftId);
					int count = this.reactor.vrRigs.Count;
					for (int i = 0; i < count; i++)
					{
						GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
						if (grplayer != null)
						{
							grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.EarnedCredits, (float)this.shiftRewardCredits);
						}
					}
				}
				Debug.LogError("ShiftId is null or empty, skipping reward of end of shift.");
				goto IL_21F;
			}
			this.announceFailShift.Play(this.announceAudioSource);
			goto IL_21F;
		case GhostReactorShiftManager.State.PreparingToDrill:
			this.announcePrepareDrill.Play(this.announceAudioSource);
			goto IL_21F;
		case GhostReactorShiftManager.State.Drilling:
			this.reactor.DelveToNextDepth();
			this.reactor.shiftManager.depthDisplay.StartDelveDeeperFX();
			goto IL_21F;
		default:
			goto IL_21F;
		}
		this.announceBell.Play(this.announceBellAudioSource);
		this.announceTip.Play(this.announceAudioSource);
		IL_21F:
		this.RefreshDepthDisplay();
	}

	// Token: 0x06002DFD RID: 11773 RVA: 0x000FBEE8 File Offset: 0x000FA0E8
	public GhostReactorShiftManager.State GetState()
	{
		return this.ShiftState;
	}

	// Token: 0x06002DFE RID: 11774 RVA: 0x000FBEF0 File Offset: 0x000FA0F0
	public bool IsSoaking()
	{
		return GhostReactorSoak.instance != null && GhostReactorSoak.instance.IsSoaking();
	}

	// Token: 0x06002DFF RID: 11775 RVA: 0x000FBF05 File Offset: 0x000FA105
	private int GetPreShiftDuration()
	{
		if (this.IsSoaking())
		{
			return 5;
		}
		return this.preShiftDuration;
	}

	// Token: 0x06002E00 RID: 11776 RVA: 0x000FBF17 File Offset: 0x000FA117
	private int GetPreShiftDurationFirstArrive()
	{
		if (this.IsSoaking())
		{
			return 5;
		}
		return this.preShiftDurationFirstArrive;
	}

	// Token: 0x06002E01 RID: 11777 RVA: 0x000FBF29 File Offset: 0x000FA129
	private int GetPostShiftDuration()
	{
		if (this.IsSoaking())
		{
			return 5;
		}
		return this.postShiftDuration;
	}

	// Token: 0x06002E02 RID: 11778 RVA: 0x000FBF3B File Offset: 0x000FA13B
	private int GetPreparingToDrillDuration()
	{
		this.IsSoaking();
		return 5;
	}

	// Token: 0x06002E03 RID: 11779 RVA: 0x000FBF45 File Offset: 0x000FA145
	public int GetDrillingDuration()
	{
		if (this.IsSoaking())
		{
			return 5;
		}
		return this.drillDuration;
	}

	// Token: 0x06002E04 RID: 11780 RVA: 0x000FBF58 File Offset: 0x000FA158
	private void UpdateStateAuthority()
	{
		if (!this.grManager.IsAuthority())
		{
			return;
		}
		double time = PhotonNetwork.Time;
		switch (this.ShiftState)
		{
		case GhostReactorShiftManager.State.WaitingForConnect:
			if (this.reactor.grManager.IsZoneReady())
			{
				this.RequestState(GhostReactorShiftManager.State.WaitingForFirstShiftStart);
				return;
			}
			break;
		case GhostReactorShiftManager.State.WaitingForShiftStart:
			if (time - this.stateStartTime > (double)this.GetPreShiftDuration())
			{
				this.reactor.grManager.RequestShiftStartAuthority(false);
				return;
			}
			break;
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
			if (time - this.stateStartTime > (double)this.GetPreShiftDurationFirstArrive())
			{
				this.reactor.grManager.RequestShiftStartAuthority(true);
				return;
			}
			break;
		case GhostReactorShiftManager.State.ReadyForShift:
		case GhostReactorShiftManager.State.ShiftActive:
			break;
		case GhostReactorShiftManager.State.PostShift:
			if (time - this.stateStartTime > (double)this.GetPostShiftDuration())
			{
				if (this.authorizedToDelveDeeper)
				{
					this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DelveDeeper);
					this.RequestState(GhostReactorShiftManager.State.PreparingToDrill);
					return;
				}
				this.RequestState(GhostReactorShiftManager.State.WaitingForShiftStart);
				return;
			}
			break;
		case GhostReactorShiftManager.State.PreparingToDrill:
			if (time - this.stateStartTime > (double)this.GetPreparingToDrillDuration())
			{
				this.RequestState(GhostReactorShiftManager.State.Drilling);
				return;
			}
			break;
		case GhostReactorShiftManager.State.Drilling:
			if (time - this.stateStartTime > (double)this.GetDrillingDuration())
			{
				this.RequestState(GhostReactorShiftManager.State.WaitingForShiftStart);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002E05 RID: 11781 RVA: 0x000FC078 File Offset: 0x000FA278
	private void UpdateStateShared()
	{
		double time = PhotonNetwork.Time;
		switch (this.ShiftState)
		{
		case GhostReactorShiftManager.State.WaitingForShiftStart:
		{
			int b = this.GetPreShiftDuration() - Mathf.FloorToInt((float)(time - this.stateStartTime));
			b = Mathf.Max(0, b);
			this.shiftTimerText.text = ":" + b.ToString("D2");
			return;
		}
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
		{
			int b2 = this.GetPreShiftDurationFirstArrive() - Mathf.FloorToInt((float)(time - this.stateStartTime));
			b2 = Mathf.Max(0, b2);
			this.shiftTimerText.text = ":" + b2.ToString("D2");
			return;
		}
		case GhostReactorShiftManager.State.ReadyForShift:
		case GhostReactorShiftManager.State.ShiftActive:
			break;
		case GhostReactorShiftManager.State.PostShift:
		{
			int b3 = this.GetPostShiftDuration() - Mathf.FloorToInt((float)(time - this.stateStartTime));
			b3 = Mathf.Max(0, b3);
			this.shiftTimerText.text = ":" + b3.ToString("D2");
			return;
		}
		case GhostReactorShiftManager.State.PreparingToDrill:
		{
			int b4 = 5 - Mathf.FloorToInt((float)(time - this.stateStartTime));
			b4 = Mathf.Max(0, b4);
			this.shiftTimerText.text = ":" + b4.ToString("D2");
			return;
		}
		case GhostReactorShiftManager.State.Drilling:
		{
			int b5 = this.GetDrillingDuration() - Mathf.FloorToInt((float)(time - this.stateStartTime));
			b5 = Mathf.Max(0, b5);
			this.shiftTimerText.text = ":" + b5.ToString("D2");
			this.UpdateLogoAnimations(this.depthDisplay.logoFrames);
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06002E06 RID: 11782 RVA: 0x000FC20C File Offset: 0x000FA40C
	public void RefreshDepthDisplay()
	{
		GhostReactorLevelGenConfig currLevelGenConfig = this.reactor.GetCurrLevelGenConfig();
		int num = this.reactor.GetDepthLevel() + 1;
		int num2 = num / 4 + 1 + ((num % 5 == 4) ? 2 : 0);
		this.shiftRewardCoresForMothership = currLevelGenConfig.coresRequired + num2;
		this.coresRequiredToDelveDeeper = ((currLevelGenConfig.coresRequired > 0) ? ((int)(this.reactor.difficultyScalingForCurrentFloor * (float)currLevelGenConfig.coresRequired) + num2) : 0);
		this.killsRequiredToDelveDeeper = currLevelGenConfig.minEnemyKills;
		this.shiftRewardCredits = currLevelGenConfig.coresRequired * 5;
		this.sentientCoresRequiredToDelveDeeper = (int)(this.reactor.difficultyScalingForCurrentFloor * (float)currLevelGenConfig.sentientCoresRequired);
		this.shiftDurationMinutes = (float)(currLevelGenConfig.shiftDuration / 60);
		if (this.IsSoaking())
		{
			this.shiftDurationMinutes = (float)Random.Range(1, 3);
		}
		this.maxPlayerDeaths = currLevelGenConfig.maxPlayerDeaths;
		if (this.depthDisplay != null)
		{
			this.depthDisplay.RefreshDisplay();
		}
		this.RefreshShiftTimer();
	}

	// Token: 0x06002E07 RID: 11783 RVA: 0x000FC2F9 File Offset: 0x000FA4F9
	public void RefreshShiftLeaderboard()
	{
		if (this.nextRefreshLeaderboardSafety)
		{
			this.RefreshShiftLeaderboard_Safety();
		}
		else
		{
			this.RefreshShiftLeaderboard_Efficiency();
		}
		this.nextRefreshLeaderboardSafety = !this.nextRefreshLeaderboardSafety;
	}

	// Token: 0x06002E08 RID: 11784 RVA: 0x000FC320 File Offset: 0x000FA520
	public void RefreshShiftLeaderboard_Safety()
	{
		if (this.shiftLeaderboardSafety == null)
		{
			return;
		}
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		this.leaderboardDisplay.Clear();
		this.leaderboardDisplay.Append("<color=#c0c0c0c0><size=-0.4>SAFETY          GHOSTS   WORKPLACE  TEAM    CHAOS\nREPORT          BANISHED INCIDENTS  ASSISTS EXPOSURE\n----------------------------------------------------</size></color>\n");
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (!(this.reactor.vrRigs[i] == null) && !(grplayer == null) && !(grplayer.gamePlayer == null))
			{
				string playerNameVisible = grplayer.gamePlayer.rig.playerNameVisible;
				int num = (int)grplayer.synchronizedSessionStats[4];
				int num2 = (int)grplayer.synchronizedSessionStats[5];
				int num3 = (int)grplayer.synchronizedSessionStats[6];
				float num4 = grplayer.synchronizedSessionStats[7];
				int num5 = (int)num4 / 60;
				int num6 = (int)num4 % 60;
				this.leaderboardDisplay.Append((i % 2 == 0) ? "<color=#e0e0ff>" : "<color=#a0a0ff>");
				this.leaderboardDisplay.Append(string.Format("{0,-12}{1,5}{2,7}{3,7}{4,10}", new object[]
				{
					playerNameVisible,
					num2,
					num,
					num3,
					string.Format("{0,3}:{1:00}", num5, num6)
				}));
				this.leaderboardDisplay.Append("</color>\n");
			}
		}
		this.shiftLeaderboardSafety.text = this.leaderboardDisplay.ToString();
	}

	// Token: 0x06002E09 RID: 11785 RVA: 0x000FC4BC File Offset: 0x000FA6BC
	public void RefreshShiftLeaderboard_Efficiency()
	{
		if (this.shiftLeaderboardEfficiency == null)
		{
			return;
		}
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		this.leaderboardDisplay.Clear();
		this.leaderboardDisplay.Append("<color=#c0c0c0c0><size=-0.4>KEY PERFORMANCE   CORES   EARNED   SPENT    DISTANCE\nINDICATORS        FOUND   CREDITS  CREDITS  TRAVELED\n----------------------------------------------------</size></color>\n");
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (!(this.reactor.vrRigs[i] == null) && !(grplayer == null) && !(grplayer.gamePlayer == null))
			{
				string playerNameVisible = grplayer.gamePlayer.rig.playerNameVisible;
				int num = (int)grplayer.synchronizedSessionStats[0];
				int num2 = (int)grplayer.synchronizedSessionStats[1];
				int num3 = (int)grplayer.synchronizedSessionStats[2];
				int num4 = (int)grplayer.synchronizedSessionStats[3];
				this.leaderboardDisplay.Append((i % 2 == 0) ? "<color=#e0e0ff>" : "<color=#a0a0ff>");
				this.leaderboardDisplay.Append(string.Format("{0,-12}{1,6}{2,7}{3,7}{4,8}", new object[]
				{
					playerNameVisible,
					num,
					num2,
					num3,
					num4
				}));
				this.leaderboardDisplay.Append("</color>\n");
			}
		}
		this.shiftLeaderboardEfficiency.text = this.leaderboardDisplay.ToString();
	}

	// Token: 0x04003A61 RID: 14945
	private const string EVENT_GOOD_KD = "GRShiftGoodKD";

	// Token: 0x04003A62 RID: 14946
	[SerializeField]
	public GhostReactor reactor;

	// Token: 0x04003A63 RID: 14947
	[SerializeField]
	private GRMetalEnergyGate frontGate;

	// Token: 0x04003A64 RID: 14948
	[SerializeField]
	private GameObject startShiftButton;

	// Token: 0x04003A65 RID: 14949
	[SerializeField]
	private TMP_Text shiftTimerText;

	// Token: 0x04003A66 RID: 14950
	[SerializeField]
	private TMP_Text shiftStatsText;

	// Token: 0x04003A67 RID: 14951
	[SerializeField]
	private TMP_Text shiftJugmentText;

	// Token: 0x04003A68 RID: 14952
	[SerializeField]
	private TMP_Text reactorTextMain;

	// Token: 0x04003A69 RID: 14953
	[SerializeField]
	private GameObject wrongStumpGoo;

	// Token: 0x04003A6A RID: 14954
	[SerializeField]
	private float shiftDurationMinutes = 20f;

	// Token: 0x04003A6B RID: 14955
	[SerializeField]
	private Transform playerTeleportTransform;

	// Token: 0x04003A6C RID: 14956
	[SerializeField]
	private Transform gatePlaneTransform;

	// Token: 0x04003A6D RID: 14957
	[SerializeField]
	private Transform gateBlockerTransform;

	// Token: 0x04003A6E RID: 14958
	[SerializeField]
	private AudioSource anomalyLoop1;

	// Token: 0x04003A6F RID: 14959
	[SerializeField]
	private AudioSource anomalyLoop2;

	// Token: 0x04003A70 RID: 14960
	[SerializeField]
	private AudioSource anomalyLoop3;

	// Token: 0x04003A71 RID: 14961
	[SerializeField]
	private AudioSource anomalyAlert;

	// Token: 0x04003A72 RID: 14962
	[SerializeField]
	private float anomalyAlertCountdownTimeToStartPlayingInMinutes = 3f;

	// Token: 0x04003A73 RID: 14963
	[SerializeField]
	private float roomCloseTimeSeconds = 60f;

	// Token: 0x04003A74 RID: 14964
	private bool isRoomClosed;

	// Token: 0x04003A75 RID: 14965
	[SerializeField]
	private int preShiftDuration = 10;

	// Token: 0x04003A76 RID: 14966
	private int preShiftDurationFirstArrive = 60;

	// Token: 0x04003A77 RID: 14967
	private int postShiftDuration = 10;

	// Token: 0x04003A78 RID: 14968
	[SerializeField]
	public int drillDuration = 50;

	// Token: 0x04003A79 RID: 14969
	private bool bIsStartingFloorAuthorityOnly;

	// Token: 0x04003A7A RID: 14970
	[Header("Drill Announcements")]
	[SerializeField]
	private AudioSource announceAudioSource;

	// Token: 0x04003A7B RID: 14971
	[SerializeField]
	private AudioSource announceBellAudioSource;

	// Token: 0x04003A7C RID: 14972
	public AbilitySound announcePrepareShift;

	// Token: 0x04003A7D RID: 14973
	public AbilitySound announceStartShift;

	// Token: 0x04003A7E RID: 14974
	public AbilitySound announceCompleteShift;

	// Token: 0x04003A7F RID: 14975
	public AbilitySound announceFailShift;

	// Token: 0x04003A80 RID: 14976
	public AbilitySound announcePrepareDrill;

	// Token: 0x04003A81 RID: 14977
	public AbilitySound announceTip;

	// Token: 0x04003A82 RID: 14978
	public AbilitySound announceBell;

	// Token: 0x04003A83 RID: 14979
	[Header("Warning")]
	public List<GhostReactorShiftManager.WarningPres> warnings;

	// Token: 0x04003A84 RID: 14980
	[SerializeField]
	private AudioClip warningAudio;

	// Token: 0x04003A85 RID: 14981
	[SerializeField]
	[Tooltip("Must be ordered from largest time (first played) to smallest time (last played)")]
	private List<int> warningClipPlayTimes = new List<int>();

	// Token: 0x04003A86 RID: 14982
	[Header("Ring")]
	[SerializeField]
	private Transform ringTransform;

	// Token: 0x04003A87 RID: 14983
	[SerializeField]
	private float ringClosingDuration = 3f;

	// Token: 0x04003A88 RID: 14984
	[SerializeField]
	private float ringClosingMaxRadius = 100f;

	// Token: 0x04003A89 RID: 14985
	[SerializeField]
	private float ringClosingMinRadius = 7f;

	// Token: 0x04003A8A RID: 14986
	[Header("Debug")]
	[SerializeField]
	private float debugFastForwardRate = 30f;

	// Token: 0x04003A8B RID: 14987
	[SerializeField]
	private bool debugFastForwarding;

	// Token: 0x04003A8C RID: 14988
	private bool shiftStarted;

	// Token: 0x04003A8D RID: 14989
	private bool shiftJustStarted;

	// Token: 0x04003A8E RID: 14990
	private double shiftStartNetworkTime;

	// Token: 0x04003A8F RID: 14991
	private double shiftEndNetworkTime;

	// Token: 0x04003A90 RID: 14992
	private float prevCountDownTotal;

	// Token: 0x04003A91 RID: 14993
	[SerializeField]
	private int shiftTotalEarned = -1;

	// Token: 0x04003A92 RID: 14994
	[SerializeField]
	private int shiftSanityMaximumEarned = 10000;

	// Token: 0x04003A93 RID: 14995
	public GhostReactorShiftDepthDisplay depthDisplay;

	// Token: 0x04003A94 RID: 14996
	public bool authorizedToDelveDeeper;

	// Token: 0x04003A95 RID: 14997
	public int shiftRewardCoresForMothership;

	// Token: 0x04003A96 RID: 14998
	public int coresRequiredToDelveDeeper;

	// Token: 0x04003A97 RID: 14999
	public int sentientCoresRequiredToDelveDeeper;

	// Token: 0x04003A98 RID: 15000
	public List<GREnemyCount> killsRequiredToDelveDeeper;

	// Token: 0x04003A99 RID: 15001
	public int maxPlayerDeaths;

	// Token: 0x04003A9A RID: 15002
	public int shiftRewardCredits;

	// Token: 0x04003A9B RID: 15003
	private bool localPlayerInside;

	// Token: 0x04003A9C RID: 15004
	private bool localPlayerOverlapping;

	// Token: 0x04003A9D RID: 15005
	private float totalPlayTime;

	// Token: 0x04003A9E RID: 15006
	private string gameIdGuid = "";

	// Token: 0x04003A9F RID: 15007
	public GRShiftStat shiftStats = new GRShiftStat();

	// Token: 0x04003AA0 RID: 15008
	[NonSerialized]
	private GhostReactorManager grManager;

	// Token: 0x04003AA1 RID: 15009
	[SerializeField]
	private TMP_Text shiftLeaderboardEfficiency;

	// Token: 0x04003AA2 RID: 15010
	[SerializeField]
	private TMP_Text shiftLeaderboardSafety;

	// Token: 0x04003AA3 RID: 15011
	private double lastLeaderboardRefreshTime;

	// Token: 0x04003AA4 RID: 15012
	private float leaderboardUpdateFrequency = 0.5f;

	// Token: 0x04003AA6 RID: 15014
	public double stateStartTime;

	// Token: 0x04003AA7 RID: 15015
	private double lastReactorLogoAnimationTime;

	// Token: 0x04003AA8 RID: 15016
	private int lastReactorLogoAnimFrame;

	// Token: 0x04003AA9 RID: 15017
	private bool isPlayingLogoAnimation;

	// Token: 0x04003AAA RID: 15018
	private double lastReactorDisplayUpdate;

	// Token: 0x04003AAB RID: 15019
	private StringBuilder cachedStringBuilder = new StringBuilder(256);

	// Token: 0x04003AAC RID: 15020
	private bool nextRefreshLeaderboardSafety;

	// Token: 0x04003AAD RID: 15021
	private StringBuilder leaderboardDisplay = new StringBuilder(1024);

	// Token: 0x0200070E RID: 1806
	[Serializable]
	public class WarningPres
	{
		// Token: 0x04003AAE RID: 15022
		public int time;

		// Token: 0x04003AAF RID: 15023
		public AbilitySound sound;
	}

	// Token: 0x0200070F RID: 1807
	public enum State
	{
		// Token: 0x04003AB1 RID: 15025
		WaitingForConnect,
		// Token: 0x04003AB2 RID: 15026
		WaitingForShiftStart,
		// Token: 0x04003AB3 RID: 15027
		WaitingForFirstShiftStart,
		// Token: 0x04003AB4 RID: 15028
		ReadyForShift,
		// Token: 0x04003AB5 RID: 15029
		ShiftActive,
		// Token: 0x04003AB6 RID: 15030
		PostShift,
		// Token: 0x04003AB7 RID: 15031
		PreparingToDrill,
		// Token: 0x04003AB8 RID: 15032
		Drilling
	}
}
