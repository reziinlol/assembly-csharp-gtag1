using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTagScripts
{
	// Token: 0x02000EC1 RID: 3777
	[NetworkBehaviourWeaved(210)]
	public class WhackAMole : NetworkComponent
	{
		// Token: 0x06005CDF RID: 23775 RVA: 0x001D7390 File Offset: 0x001D5590
		private void UpdateMeshRendererList()
		{
			List<MeshRenderer> list = new List<MeshRenderer>();
			ZoneBasedObject[] array = this.zoneBasedVisuals;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (MeshRenderer meshRenderer in array[i].GetComponentsInChildren<MeshRenderer>(true))
				{
					if (meshRenderer.enabled)
					{
						list.Add(meshRenderer);
					}
				}
			}
			this.zoneBasedMeshRenderers = list.ToArray();
		}

		// Token: 0x06005CE0 RID: 23776 RVA: 0x001D73F8 File Offset: 0x001D55F8
		protected override void Awake()
		{
			base.Awake();
			if (this.molesContainerRight != null)
			{
				this.rightMolesList = new List<Mole>(this.molesContainerRight.GetComponentsInChildren<Mole>());
				if (this.rightMolesList.Count > 0)
				{
					this.molesList.AddRange(this.rightMolesList);
				}
			}
			if (this.molesContainerLeft != null)
			{
				this.leftMolesList = new List<Mole>(this.molesContainerLeft.GetComponentsInChildren<Mole>());
				if (this.leftMolesList.Count > 0)
				{
					this.molesList.AddRange(this.leftMolesList);
					foreach (Mole mole in this.leftMolesList)
					{
						mole.IsLeftSideMole = true;
					}
				}
			}
			this.currentLevelIndex = -1;
			foreach (Mole mole2 in this.molesList)
			{
				mole2.OnTapped += this.OnMoleTapped;
			}
			List<Mole> list = this.leftMolesList;
			bool flag;
			if (list != null && list.Count > 0)
			{
				list = this.rightMolesList;
				flag = (list != null && list.Count > 0);
			}
			else
			{
				flag = false;
			}
			this.isMultiplayer = flag;
			this.welcomeUI.SetActive(false);
			this.ongoingGameUI.SetActive(false);
			this.levelEndedUI.SetActive(false);
			this.ContinuePressedUI.SetActive(false);
			this.multiplyareScoresUI.SetActive(false);
			this.bestScore = 0;
			this.bestScoreText.text = string.Empty;
			this.highScorePlayerName = string.Empty;
			this.victoryParticles = this.victoryFX.GetComponentsInChildren<ParticleSystem>();
		}

		// Token: 0x06005CE1 RID: 23777 RVA: 0x001D75C8 File Offset: 0x001D57C8
		protected override void Start()
		{
			base.Start();
			this.SwitchState(WhackAMole.GameState.Off);
			if (WhackAMoleManager.instance)
			{
				WhackAMoleManager.instance.Register(this);
			}
		}

		// Token: 0x06005CE2 RID: 23778 RVA: 0x001D75F0 File Offset: 0x001D57F0
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (Mole mole in this.molesList)
			{
				mole.OnTapped -= this.OnMoleTapped;
			}
			if (WhackAMoleManager.instance)
			{
				WhackAMoleManager.instance.Unregister(this);
			}
			this.molesList.Clear();
		}

		// Token: 0x06005CE3 RID: 23779 RVA: 0x001D7674 File Offset: 0x001D5874
		public void InvokeUpdate()
		{
			bool isMasterClient = NetworkSystem.Instance.IsMasterClient;
			bool flag = this.zoneBasedVisuals[0].IsLocalPlayerInZone();
			if (isMasterClient != this.wasMasterClient || flag != this.wasLocalPlayerInZone)
			{
				MeshRenderer[] array = this.zoneBasedMeshRenderers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = flag;
				}
				bool active = isMasterClient || flag;
				ZoneBasedObject[] array2 = this.zoneBasedVisuals;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].gameObject.SetActive(active);
				}
				this.wasMasterClient = isMasterClient;
				this.wasLocalPlayerInZone = flag;
			}
		}

		// Token: 0x06005CE4 RID: 23780 RVA: 0x001D770C File Offset: 0x001D590C
		private void SwitchState(WhackAMole.GameState state)
		{
			this.lastState = this.currentState;
			this.currentState = state;
			switch (this.currentState)
			{
			case WhackAMole.GameState.Off:
				this.ResetGame();
				this.currentLevelIndex = -1;
				this.currentLevel = null;
				this.UpdateLevelUI(1);
				break;
			case WhackAMole.GameState.ContinuePressed:
				this.continuePressedTime = Time.time;
				this.audioSource.GTStop();
				this.audioSource.GTPlayOneShot(this.counterClip, 1f);
				if (base.IsMine)
				{
					this.pickedMolesIndex.Clear();
				}
				this.ResetGame();
				if (base.IsMine)
				{
					this.LoadNextLevel();
				}
				break;
			case WhackAMole.GameState.Ongoing:
				this.UpdateScoreUI(this.currentScore, this.leftPlayerScore, this.rightPlayerScore);
				break;
			case WhackAMole.GameState.TimesUp:
				if (this.currentLevel != null)
				{
					foreach (Mole mole in this.molesList)
					{
						mole.HideMole(false);
					}
					this.curentGameResult = this.GetGameResult();
					this.UpdateResultUI(this.curentGameResult);
					this.levelEndedTotalScoreText.text = "SCORE " + this.totalScore.ToString();
					this.levelEndedCurrentScoreText.text = string.Format("{0}/{1}", this.currentScore, this.currentLevel.GetMinScore(this.isMultiplayer));
					if (this.totalScore > this.bestScore)
					{
						this.bestScore = this.totalScore;
						this.highScorePlayerName = this.playerName;
					}
					this.bestScoreText.text = (this.isMultiplayer ? this.bestScore.ToString() : (this.highScorePlayerName + "  " + this.bestScore.ToString()));
					this.audioSource.GTStop();
					if (this.curentGameResult == WhackAMole.GameResult.LevelComplete)
					{
						this.audioSource.GTPlayOneShot(this.levelCompleteClip, 1f);
						if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
						{
							PlayerGameEvents.MiscEvent("WhackComplete" + this.currentLevel.levelNumber.ToString(), 1);
						}
					}
					else if (this.curentGameResult == WhackAMole.GameResult.GameOver)
					{
						this.audioSource.GTPlayOneShot(this.gameOverClip, 1f);
					}
					else if (this.curentGameResult == WhackAMole.GameResult.Win)
					{
						this.audioSource.GTPlayOneShot(this.winClip, 1f);
						if (this.victoryFX)
						{
							ParticleSystem[] array = this.victoryParticles;
							for (int i = 0; i < array.Length; i++)
							{
								array[i].Play();
							}
						}
						if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
						{
							PlayerGameEvents.MiscEvent("WhackComplete" + this.currentLevel.levelNumber.ToString(), 1);
						}
					}
					int minScore = this.currentLevel.GetMinScore(this.isMultiplayer);
					if (this.levelGoodMolesPicked < minScore)
					{
						GTDev.LogError<string>(string.Format("[WAM] Lvl:{0} Only Picked {1}/{2} good moles!", this.currentLevel.levelNumber, this.levelGoodMolesPicked, minScore), null);
					}
					if (base.IsMine)
					{
						GorillaTelemetry.WamLevelEnd(this.playerId, this.gameId, this.machineId, this.currentLevel.levelNumber, this.levelGoodMolesPicked, this.levelHazardMolesPicked, minScore, this.currentScore, this.levelHazardMolesHit, this.curentGameResult.ToString());
					}
				}
				break;
			}
			this.UpdateScreenData();
		}

		// Token: 0x06005CE5 RID: 23781 RVA: 0x001D7AD4 File Offset: 0x001D5CD4
		private void UpdateScreenData()
		{
			switch (this.currentState)
			{
			case WhackAMole.GameState.Off:
				this.welcomeUI.SetActive(true);
				this.ContinuePressedUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.levelEndedUI.SetActive(false);
				this.multiplyareScoresUI.SetActive(false);
				return;
			case WhackAMole.GameState.ContinuePressed:
				this.levelEndedUI.SetActive(false);
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.multiplyareScoresUI.SetActive(false);
				this.ContinuePressedUI.SetActive(true);
				break;
			case WhackAMole.GameState.Ongoing:
				this.ContinuePressedUI.SetActive(false);
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(true);
				this.levelEndedUI.SetActive(false);
				if (this.isMultiplayer)
				{
					this.multiplyareScoresUI.SetActive(true);
					return;
				}
				break;
			case WhackAMole.GameState.PickMoles:
				break;
			case WhackAMole.GameState.TimesUp:
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.ContinuePressedUI.SetActive(false);
				if (this.isMultiplayer)
				{
					this.multiplyareScoresUI.SetActive(true);
				}
				this.levelEndedUI.SetActive(true);
				return;
			default:
				return;
			}
		}

		// Token: 0x06005CE6 RID: 23782 RVA: 0x001D7C0C File Offset: 0x001D5E0C
		public static int CreateNewGameID()
		{
			int num = (int)((DateTime.Now - WhackAMole.epoch).TotalSeconds * 8.0 % 2147483646.0) + 1;
			if (num <= WhackAMole.lastAssignedID)
			{
				WhackAMole.lastAssignedID++;
				return WhackAMole.lastAssignedID;
			}
			WhackAMole.lastAssignedID = num;
			return num;
		}

		// Token: 0x06005CE7 RID: 23783 RVA: 0x001D7C6C File Offset: 0x001D5E6C
		private void OnMoleTapped(MoleTypes moleType, Vector3 position, bool isLocalTap, bool isLeftHand)
		{
			WhackAMole.GameState gameState = this.currentState;
			if (gameState == WhackAMole.GameState.Off || gameState == WhackAMole.GameState.TimesUp)
			{
				return;
			}
			AudioClip clip = moleType.isHazard ? this.whackHazardClips[Random.Range(0, this.whackHazardClips.Length)] : this.whackMonkeClips[Random.Range(0, this.whackMonkeClips.Length)];
			if (moleType.isHazard)
			{
				this.audioSource.GTPlayOneShot(clip, 1f);
				this.levelHazardMolesHit++;
			}
			else
			{
				this.audioSource.GTPlayOneShot(clip, 1f);
			}
			if (moleType.monkeMoleHitMaterial != null)
			{
				moleType.MeshRenderer.material = moleType.monkeMoleHitMaterial;
			}
			this.currentScore += moleType.scorePoint;
			this.totalScore += moleType.scorePoint;
			if (moleType.IsLeftSideMoleType)
			{
				this.leftPlayerScore += moleType.scorePoint;
			}
			else
			{
				this.rightPlayerScore += moleType.scorePoint;
			}
			this.UpdateScoreUI(this.currentScore, this.leftPlayerScore, this.rightPlayerScore);
			moleType.MoleContainerParent.HideMole(true);
		}

		// Token: 0x06005CE8 RID: 23784 RVA: 0x001D7D90 File Offset: 0x001D5F90
		public void HandleOnTimerStopped()
		{
			this.gameEndedTime = Time.time;
			this.SwitchState(WhackAMole.GameState.TimesUp);
		}

		// Token: 0x06005CE9 RID: 23785 RVA: 0x001D7DA4 File Offset: 0x001D5FA4
		private IEnumerator PlayHazardAudio(AudioClip clip)
		{
			this.audioSource.clip = clip;
			this.audioSource.GTPlay();
			yield return new WaitForSeconds(this.audioSource.clip.length);
			this.audioSource.clip = this.errorClip;
			this.audioSource.GTPlay();
			yield break;
		}

		// Token: 0x06005CEA RID: 23786 RVA: 0x001D7DBC File Offset: 0x001D5FBC
		private bool PickMoles()
		{
			WhackAMole.<>c__DisplayClass85_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			this.pickedMolesIndex.Clear();
			float passedTime = this.timer.GetPassedTime();
			if (passedTime > this.currentLevel.levelDuration - this.currentLevel.showMoleDuration)
			{
				return true;
			}
			float t = passedTime / this.currentLevel.levelDuration;
			CS$<>8__locals1.minMoleCount = Mathf.Lerp(this.currentLevel.minimumMoleCount.x, this.currentLevel.minimumMoleCount.y, t);
			CS$<>8__locals1.maxMoleCount = Mathf.Lerp(this.currentLevel.maximumMoleCount.x, this.currentLevel.maximumMoleCount.y, t);
			this.curentTime = Time.time;
			CS$<>8__locals1.hazardMoleChance = Mathf.Lerp(this.currentLevel.hazardMoleChance.x, this.currentLevel.hazardMoleChance.y, t);
			if (this.isMultiplayer)
			{
				this.<PickMoles>g__PickMolesFrom|85_0(this.rightMolesList, ref CS$<>8__locals1);
				this.<PickMoles>g__PickMolesFrom|85_0(this.leftMolesList, ref CS$<>8__locals1);
			}
			else
			{
				this.<PickMoles>g__PickMolesFrom|85_0(this.molesList, ref CS$<>8__locals1);
			}
			return this.pickedMolesIndex.Count != 0;
		}

		// Token: 0x06005CEB RID: 23787 RVA: 0x001D7EE8 File Offset: 0x001D60E8
		private void LoadNextLevel()
		{
			if (this.currentLevel != null)
			{
				this.resetToFirstLevel = (this.currentScore < this.currentLevel.GetMinScore(this.isMultiplayer));
				if (this.resetToFirstLevel)
				{
					this.currentLevelIndex = 0;
				}
				else
				{
					this.currentLevelIndex++;
				}
				if (this.currentLevelIndex >= this.allLevels.Length)
				{
					this.currentLevelIndex = 0;
				}
			}
			else
			{
				this.currentLevelIndex++;
			}
			this.currentLevel = this.allLevels[this.currentLevelIndex];
			this.timer.SetTimerDuration(this.currentLevel.levelDuration);
			this.timer.RestartTimer();
			this.curentTime = Time.time;
			this.currentScore = 0;
			this.leftPlayerScore = 0;
			this.rightPlayerScore = 0;
			this.levelGoodMolesPicked = (this.levelHazardMolesPicked = 0);
			this.levelHazardMolesHit = 0;
			if (this.currentLevelIndex == 0)
			{
				this.totalScore = 0;
			}
			if (this.currentLevelIndex == 0 && base.IsMine)
			{
				this.gameId = WhackAMole.CreateNewGameID();
				Debug.LogWarning("GAME ID" + this.gameId.ToString());
			}
		}

		// Token: 0x06005CEC RID: 23788 RVA: 0x001D8018 File Offset: 0x001D6218
		private bool PickSingleMole(int randomMoleIndex, float hazardMoleChance)
		{
			bool flag = hazardMoleChance > 0f && Random.value <= hazardMoleChance;
			int moleTypeIndex = this.molesList[randomMoleIndex].GetMoleTypeIndex(flag);
			this.molesList[randomMoleIndex].ShowMole(this.currentLevel.showMoleDuration, moleTypeIndex);
			this.pickedMolesIndex.Add(randomMoleIndex, moleTypeIndex);
			if (flag)
			{
				this.levelHazardMolesPicked++;
			}
			else
			{
				this.levelGoodMolesPicked++;
			}
			return flag;
		}

		// Token: 0x06005CED RID: 23789 RVA: 0x001D809C File Offset: 0x001D629C
		private void ResetGame()
		{
			foreach (Mole mole in this.molesList)
			{
				mole.ResetPosition();
			}
		}

		// Token: 0x06005CEE RID: 23790 RVA: 0x001D80EC File Offset: 0x001D62EC
		private void UpdateScoreUI(int totalScore, int _leftPlayerScore, int _rightPlayerScore)
		{
			if (this.currentLevel != null)
			{
				this.scoreText.text = string.Format("SCORE\n{0}/{1}", totalScore, this.currentLevel.GetMinScore(this.isMultiplayer));
				this.leftPlayerScoreText.text = _leftPlayerScore.ToString();
				this.rightPlayerScoreText.text = _rightPlayerScore.ToString();
			}
		}

		// Token: 0x06005CEF RID: 23791 RVA: 0x001D815C File Offset: 0x001D635C
		private void UpdateLevelUI(int levelNumber)
		{
			this.arrowTargetRotation = Quaternion.Euler(0f, 0f, (float)(18 * (levelNumber - 1)));
			this.arrowRotationNeedsUpdate = true;
		}

		// Token: 0x06005CF0 RID: 23792 RVA: 0x001D8184 File Offset: 0x001D6384
		private void UpdateArrowRotation()
		{
			Quaternion quaternion = Quaternion.Slerp(this.levelArrow.transform.localRotation, this.arrowTargetRotation, Time.deltaTime * 5f);
			if (Quaternion.Angle(quaternion, this.arrowTargetRotation) < 0.1f)
			{
				quaternion = this.arrowTargetRotation;
				this.arrowRotationNeedsUpdate = false;
			}
			this.levelArrow.transform.localRotation = quaternion;
		}

		// Token: 0x06005CF1 RID: 23793 RVA: 0x001D81EA File Offset: 0x001D63EA
		private void UpdateTimerUI(int time)
		{
			if (time == this.previousTime)
			{
				return;
			}
			this.timeText.text = "TIME " + time.ToString();
			this.previousTime = time;
		}

		// Token: 0x06005CF2 RID: 23794 RVA: 0x001D8219 File Offset: 0x001D6419
		private void UpdateResultUI(WhackAMole.GameResult gameResult)
		{
			if (gameResult == WhackAMole.GameResult.LevelComplete)
			{
				this.resultText.text = "LEVEL COMPLETE";
				return;
			}
			if (gameResult == WhackAMole.GameResult.Win)
			{
				this.resultText.text = "YOU WIN!";
				return;
			}
			if (gameResult == WhackAMole.GameResult.GameOver)
			{
				this.resultText.text = "GAME OVER";
			}
		}

		// Token: 0x06005CF3 RID: 23795 RVA: 0x001D8258 File Offset: 0x001D6458
		public void OnStartButtonPressed()
		{
			WhackAMole.GameState gameState = this.currentState;
			if (gameState == WhackAMole.GameState.TimesUp || gameState == WhackAMole.GameState.Off)
			{
				base.GetView.RPC("WhackAMoleButtonPressed", RpcTarget.All, Array.Empty<object>());
			}
		}

		// Token: 0x06005CF4 RID: 23796 RVA: 0x001D8289 File Offset: 0x001D6489
		[PunRPC]
		private void WhackAMoleButtonPressed(PhotonMessageInfo info)
		{
			this.WhackAMoleButtonPressedShared(info);
		}

		// Token: 0x06005CF5 RID: 23797 RVA: 0x001D8298 File Offset: 0x001D6498
		[Rpc]
		private unsafe void RPC_WhackAMoleButtonPressed(RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) == 0)
					{
						NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTagScripts.WhackAMole::RPC_WhackAMoleButtonPressed(Fusion.RpcInfo)", base.Object, 7);
					}
					else
					{
						int num = 8;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.WhackAMole::RPC_WhackAMoleButtonPressed(Fusion.RpcInfo)", num);
						}
						else
						{
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
								int num2 = 8;
								ptr->Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 7) != 0)
							{
								info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
								goto IL_12;
							}
						}
					}
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			this.WhackAMoleButtonPressedShared(info);
		}

		// Token: 0x06005CF6 RID: 23798 RVA: 0x001D83D8 File Offset: 0x001D65D8
		private void WhackAMoleButtonPressedShared(PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "WhackAMoleButtonPressedShared");
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
			if (vrrig)
			{
				this.playerName = vrrig.playerNameVisible;
				if (this.currentState == WhackAMole.GameState.Off)
				{
					this.playerId = info.Sender.UserId;
					if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
					{
						PlayerGameEvents.MiscEvent("PlayArcadeGame", 1);
					}
				}
			}
			this.SwitchState(WhackAMole.GameState.ContinuePressed);
		}

		// Token: 0x06005CF7 RID: 23799 RVA: 0x001D8457 File Offset: 0x001D6657
		private WhackAMole.GameResult GetGameResult()
		{
			if (this.currentScore < this.currentLevel.GetMinScore(this.isMultiplayer))
			{
				return WhackAMole.GameResult.GameOver;
			}
			if (this.currentLevelIndex >= this.allLevels.Length - 1)
			{
				return WhackAMole.GameResult.Win;
			}
			return WhackAMole.GameResult.LevelComplete;
		}

		// Token: 0x06005CF8 RID: 23800 RVA: 0x001D8489 File Offset: 0x001D6689
		public int GetCurrentLevel()
		{
			if (this.currentLevel != null)
			{
				return this.currentLevel.levelNumber;
			}
			return 0;
		}

		// Token: 0x06005CF9 RID: 23801 RVA: 0x001D84A6 File Offset: 0x001D66A6
		public int GetTotalLevelNumbers()
		{
			if (this.allLevels != null)
			{
				return this.allLevels.Length;
			}
			return 0;
		}

		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x06005CFA RID: 23802 RVA: 0x001D84BA File Offset: 0x001D66BA
		// (set) Token: 0x06005CFB RID: 23803 RVA: 0x001D84E4 File Offset: 0x001D66E4
		[Networked]
		[NetworkedWeaved(0, 210)]
		public unsafe WhackAMole.WhackAMoleData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing WhackAMole.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(WhackAMole.WhackAMoleData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing WhackAMole.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(WhackAMole.WhackAMoleData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06005CFC RID: 23804 RVA: 0x001D8510 File Offset: 0x001D6710
		public override void WriteDataFusion()
		{
			this.Data = new WhackAMole.WhackAMoleData(this.currentState, this.currentLevelIndex, this.currentScore, this.totalScore, this.bestScore, this.rightPlayerScore, this.highScorePlayerName, this.timer.GetRemainingTime(), this.gameEndedTime, this.gameId, this.pickedMolesIndex);
			this.pickedMolesIndex.Clear();
		}

		// Token: 0x06005CFD RID: 23805 RVA: 0x001D857C File Offset: 0x001D677C
		public override void ReadDataFusion()
		{
			this.ReadDataShared(this.Data.CurrentState, this.Data.CurrentLevelIndex, this.Data.CurrentScore, this.Data.TotalScore, this.Data.BestScore, this.Data.RightPlayerScore, this.Data.HighScorePlayerName.Value, this.Data.RemainingTime, this.Data.GameEndedTime, this.Data.GameId);
			for (int i = 0; i < this.Data.PickedMolesIndexCount; i++)
			{
				int randomMoleTypeIndex = this.Data.PickedMolesIndex[i];
				if (i >= 0 && i < this.molesList.Count && this.currentLevel)
				{
					this.molesList[i].ShowMole(this.currentLevel.showMoleDuration, randomMoleTypeIndex);
				}
			}
		}

		// Token: 0x06005CFE RID: 23806 RVA: 0x001D8694 File Offset: 0x001D6894
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
		}

		// Token: 0x06005CFF RID: 23807 RVA: 0x001D86A4 File Offset: 0x001D68A4
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
		}

		// Token: 0x06005D00 RID: 23808 RVA: 0x001D86B4 File Offset: 0x001D68B4
		private void ReadDataShared(WhackAMole.GameState _currentState, int _currentLevelIndex, int cScore, int tScore, int bScore, int rPScore, string hScorePName, float _remainingTime, float endedTime, int _gameId)
		{
			WhackAMole.GameState gameState = this.currentState;
			if (_currentState != gameState)
			{
				this.SwitchState(_currentState);
			}
			this.currentLevelIndex = _currentLevelIndex;
			if (this.currentLevelIndex >= 0 && this.currentLevelIndex < this.allLevels.Length)
			{
				this.currentLevel = this.allLevels[this.currentLevelIndex];
				this.UpdateLevelUI(this.currentLevel.levelNumber);
			}
			this.currentScore = cScore;
			this.totalScore = tScore;
			this.bestScore = bScore;
			this.rightPlayerScore = rPScore;
			this.leftPlayerScore = this.currentScore - this.rightPlayerScore;
			this.highScorePlayerName = hScorePName;
			this.bestScoreText.text = (this.isMultiplayer ? this.bestScore.ToString() : (this.highScorePlayerName + "  " + this.bestScore.ToString()));
			this.remainingTime = _remainingTime;
			if (float.IsFinite(this.remainingTime) && this.currentLevel)
			{
				this.remainingTime = this.remainingTime.ClampSafe(0f, this.currentLevel.levelDuration);
				this.UpdateTimerUI((int)this.remainingTime);
			}
			if (float.IsFinite(endedTime))
			{
				this.gameEndedTime = endedTime.ClampSafe(0f, Time.time);
			}
			this.gameId = _gameId;
		}

		// Token: 0x06005D01 RID: 23809 RVA: 0x001D8808 File Offset: 0x001D6A08
		protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
		{
			base.OnOwnerSwitched(newOwningPlayer);
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.timer.RestartTimer();
				this.timer.SetTimerDuration(this.remainingTime);
				this.curentTime = Time.time;
				if (this.currentLevelIndex >= 0 && this.currentLevelIndex < this.allLevels.Length)
				{
					this.currentLevel = this.allLevels[this.currentLevelIndex];
				}
				this.SwitchState(this.currentState);
			}
		}

		// Token: 0x06005D04 RID: 23812 RVA: 0x001D890C File Offset: 0x001D6B0C
		[CompilerGenerated]
		private void <PickMoles>g__PickMolesFrom|85_0(List<Mole> moles, ref WhackAMole.<>c__DisplayClass85_0 A_2)
		{
			int a = Mathf.RoundToInt(Random.Range(A_2.minMoleCount, A_2.maxMoleCount));
			this.potentialMoles.Clear();
			foreach (Mole mole in moles)
			{
				if (mole.CanPickMole())
				{
					this.potentialMoles.Add(mole);
				}
			}
			int num = Mathf.Min(a, this.potentialMoles.Count);
			int num2 = Mathf.CeilToInt((float)num * A_2.hazardMoleChance);
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				int index = Random.Range(0, this.potentialMoles.Count);
				if (this.PickSingleMole(this.molesList.IndexOf(this.potentialMoles[index]), (num3 < num2) ? A_2.hazardMoleChance : 0f))
				{
					num3++;
				}
				this.potentialMoles.RemoveAt(index);
			}
		}

		// Token: 0x06005D05 RID: 23813 RVA: 0x001D8A18 File Offset: 0x001D6C18
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06005D06 RID: 23814 RVA: 0x001D8A30 File Offset: 0x001D6C30
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x06005D07 RID: 23815 RVA: 0x001D8A44 File Offset: 0x001D6C44
		[NetworkRpcWeavedInvoker(1, 7, 7)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_WhackAMoleButtonPressed@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((WhackAMole)behaviour).RPC_WhackAMoleButtonPressed(info);
		}

		// Token: 0x04006B54 RID: 27476
		public string machineId = "default";

		// Token: 0x04006B55 RID: 27477
		public GameObject molesContainerRight;

		// Token: 0x04006B56 RID: 27478
		[Tooltip("Only for co-op version")]
		public GameObject molesContainerLeft;

		// Token: 0x04006B57 RID: 27479
		public int betweenLevelPauseDuration = 3;

		// Token: 0x04006B58 RID: 27480
		public int countdownDuration = 5;

		// Token: 0x04006B59 RID: 27481
		public WhackAMoleLevelSO[] allLevels;

		// Token: 0x04006B5A RID: 27482
		[SerializeField]
		private GorillaTimer timer;

		// Token: 0x04006B5B RID: 27483
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006B5C RID: 27484
		public GameObject levelArrow;

		// Token: 0x04006B5D RID: 27485
		public GameObject victoryFX;

		// Token: 0x04006B5E RID: 27486
		public ZoneBasedObject[] zoneBasedVisuals;

		// Token: 0x04006B5F RID: 27487
		[SerializeField]
		private MeshRenderer[] zoneBasedMeshRenderers;

		// Token: 0x04006B60 RID: 27488
		[Space]
		public AudioClip backgroundLoop;

		// Token: 0x04006B61 RID: 27489
		public AudioClip errorClip;

		// Token: 0x04006B62 RID: 27490
		public AudioClip counterClip;

		// Token: 0x04006B63 RID: 27491
		public AudioClip levelCompleteClip;

		// Token: 0x04006B64 RID: 27492
		public AudioClip winClip;

		// Token: 0x04006B65 RID: 27493
		public AudioClip gameOverClip;

		// Token: 0x04006B66 RID: 27494
		public AudioClip[] whackHazardClips;

		// Token: 0x04006B67 RID: 27495
		public AudioClip[] whackMonkeClips;

		// Token: 0x04006B68 RID: 27496
		[Space]
		public GameObject welcomeUI;

		// Token: 0x04006B69 RID: 27497
		public GameObject ongoingGameUI;

		// Token: 0x04006B6A RID: 27498
		public GameObject levelEndedUI;

		// Token: 0x04006B6B RID: 27499
		public GameObject ContinuePressedUI;

		// Token: 0x04006B6C RID: 27500
		public GameObject multiplyareScoresUI;

		// Token: 0x04006B6D RID: 27501
		[Space]
		public TextMeshPro scoreText;

		// Token: 0x04006B6E RID: 27502
		public TextMeshPro bestScoreText;

		// Token: 0x04006B6F RID: 27503
		[Tooltip("Only for co-op version")]
		public TextMeshPro rightPlayerScoreText;

		// Token: 0x04006B70 RID: 27504
		[Tooltip("Only for co-op version")]
		public TextMeshPro leftPlayerScoreText;

		// Token: 0x04006B71 RID: 27505
		public TextMeshPro timeText;

		// Token: 0x04006B72 RID: 27506
		public TextMeshPro counterText;

		// Token: 0x04006B73 RID: 27507
		public TextMeshPro resultText;

		// Token: 0x04006B74 RID: 27508
		public TextMeshPro levelEndedOptionsText;

		// Token: 0x04006B75 RID: 27509
		public TextMeshPro levelEndedCountdownText;

		// Token: 0x04006B76 RID: 27510
		public TextMeshPro levelEndedTotalScoreText;

		// Token: 0x04006B77 RID: 27511
		public TextMeshPro levelEndedCurrentScoreText;

		// Token: 0x04006B78 RID: 27512
		private List<Mole> rightMolesList;

		// Token: 0x04006B79 RID: 27513
		private List<Mole> leftMolesList;

		// Token: 0x04006B7A RID: 27514
		private List<Mole> molesList = new List<Mole>();

		// Token: 0x04006B7B RID: 27515
		private WhackAMoleLevelSO currentLevel;

		// Token: 0x04006B7C RID: 27516
		private int currentScore;

		// Token: 0x04006B7D RID: 27517
		private int totalScore;

		// Token: 0x04006B7E RID: 27518
		private int leftPlayerScore;

		// Token: 0x04006B7F RID: 27519
		private int rightPlayerScore;

		// Token: 0x04006B80 RID: 27520
		private int bestScore;

		// Token: 0x04006B81 RID: 27521
		private float curentTime;

		// Token: 0x04006B82 RID: 27522
		private int currentLevelIndex;

		// Token: 0x04006B83 RID: 27523
		private float continuePressedTime;

		// Token: 0x04006B84 RID: 27524
		private bool resetToFirstLevel;

		// Token: 0x04006B85 RID: 27525
		private Quaternion arrowTargetRotation;

		// Token: 0x04006B86 RID: 27526
		private bool arrowRotationNeedsUpdate;

		// Token: 0x04006B87 RID: 27527
		private List<Mole> potentialMoles = new List<Mole>();

		// Token: 0x04006B88 RID: 27528
		private Dictionary<int, int> pickedMolesIndex = new Dictionary<int, int>();

		// Token: 0x04006B89 RID: 27529
		private WhackAMole.GameState currentState;

		// Token: 0x04006B8A RID: 27530
		private WhackAMole.GameState lastState;

		// Token: 0x04006B8B RID: 27531
		private float remainingTime;

		// Token: 0x04006B8C RID: 27532
		private int previousTime = -1;

		// Token: 0x04006B8D RID: 27533
		private bool isMultiplayer;

		// Token: 0x04006B8E RID: 27534
		private float gameEndedTime;

		// Token: 0x04006B8F RID: 27535
		private WhackAMole.GameResult curentGameResult;

		// Token: 0x04006B90 RID: 27536
		private string playerName = string.Empty;

		// Token: 0x04006B91 RID: 27537
		private string highScorePlayerName = string.Empty;

		// Token: 0x04006B92 RID: 27538
		private ParticleSystem[] victoryParticles;

		// Token: 0x04006B93 RID: 27539
		private int levelHazardMolesPicked;

		// Token: 0x04006B94 RID: 27540
		private int levelGoodMolesPicked;

		// Token: 0x04006B95 RID: 27541
		private string playerId;

		// Token: 0x04006B96 RID: 27542
		private int gameId;

		// Token: 0x04006B97 RID: 27543
		private int levelHazardMolesHit;

		// Token: 0x04006B98 RID: 27544
		private static DateTime epoch = new DateTime(2024, 1, 1);

		// Token: 0x04006B99 RID: 27545
		private static int lastAssignedID;

		// Token: 0x04006B9A RID: 27546
		private bool wasMasterClient;

		// Token: 0x04006B9B RID: 27547
		private bool wasLocalPlayerInZone = true;

		// Token: 0x04006B9C RID: 27548
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 210)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private WhackAMole.WhackAMoleData _Data;

		// Token: 0x02000EC2 RID: 3778
		public enum GameState
		{
			// Token: 0x04006B9E RID: 27550
			Off,
			// Token: 0x04006B9F RID: 27551
			ContinuePressed,
			// Token: 0x04006BA0 RID: 27552
			Ongoing,
			// Token: 0x04006BA1 RID: 27553
			PickMoles,
			// Token: 0x04006BA2 RID: 27554
			TimesUp,
			// Token: 0x04006BA3 RID: 27555
			LevelStarted
		}

		// Token: 0x02000EC3 RID: 3779
		private enum GameResult
		{
			// Token: 0x04006BA5 RID: 27557
			GameOver,
			// Token: 0x04006BA6 RID: 27558
			Win,
			// Token: 0x04006BA7 RID: 27559
			LevelComplete,
			// Token: 0x04006BA8 RID: 27560
			Unknown
		}

		// Token: 0x02000EC4 RID: 3780
		[NetworkStructWeaved(210)]
		[StructLayout(LayoutKind.Explicit, Size = 840)]
		public struct WhackAMoleData : INetworkStruct
		{
			// Token: 0x170008F1 RID: 2289
			// (get) Token: 0x06005D08 RID: 23816 RVA: 0x001D8A88 File Offset: 0x001D6C88
			// (set) Token: 0x06005D09 RID: 23817 RVA: 0x001D8A90 File Offset: 0x001D6C90
			public WhackAMole.GameState CurrentState { readonly get; set; }

			// Token: 0x170008F2 RID: 2290
			// (get) Token: 0x06005D0A RID: 23818 RVA: 0x001D8A99 File Offset: 0x001D6C99
			// (set) Token: 0x06005D0B RID: 23819 RVA: 0x001D8AA1 File Offset: 0x001D6CA1
			public int CurrentLevelIndex { readonly get; set; }

			// Token: 0x170008F3 RID: 2291
			// (get) Token: 0x06005D0C RID: 23820 RVA: 0x001D8AAA File Offset: 0x001D6CAA
			// (set) Token: 0x06005D0D RID: 23821 RVA: 0x001D8AB2 File Offset: 0x001D6CB2
			public int CurrentScore { readonly get; set; }

			// Token: 0x170008F4 RID: 2292
			// (get) Token: 0x06005D0E RID: 23822 RVA: 0x001D8ABB File Offset: 0x001D6CBB
			// (set) Token: 0x06005D0F RID: 23823 RVA: 0x001D8AC3 File Offset: 0x001D6CC3
			public int TotalScore { readonly get; set; }

			// Token: 0x170008F5 RID: 2293
			// (get) Token: 0x06005D10 RID: 23824 RVA: 0x001D8ACC File Offset: 0x001D6CCC
			// (set) Token: 0x06005D11 RID: 23825 RVA: 0x001D8AD4 File Offset: 0x001D6CD4
			public int BestScore { readonly get; set; }

			// Token: 0x170008F6 RID: 2294
			// (get) Token: 0x06005D12 RID: 23826 RVA: 0x001D8ADD File Offset: 0x001D6CDD
			// (set) Token: 0x06005D13 RID: 23827 RVA: 0x001D8AE5 File Offset: 0x001D6CE5
			public int RightPlayerScore { readonly get; set; }

			// Token: 0x170008F7 RID: 2295
			// (get) Token: 0x06005D14 RID: 23828 RVA: 0x001D8AEE File Offset: 0x001D6CEE
			// (set) Token: 0x06005D15 RID: 23829 RVA: 0x001D8B00 File Offset: 0x001D6D00
			[Networked]
			[NetworkedWeaved(6, 129)]
			public unsafe NetworkString<_128> HighScorePlayerName
			{
				readonly get
				{
					return *(NetworkString<_128>*)Native.ReferenceToPointer<FixedStorage@129>(ref this._HighScorePlayerName);
				}
				set
				{
					*(NetworkString<_128>*)Native.ReferenceToPointer<FixedStorage@129>(ref this._HighScorePlayerName) = value;
				}
			}

			// Token: 0x170008F8 RID: 2296
			// (get) Token: 0x06005D16 RID: 23830 RVA: 0x001D8B13 File Offset: 0x001D6D13
			// (set) Token: 0x06005D17 RID: 23831 RVA: 0x001D8B1B File Offset: 0x001D6D1B
			public float RemainingTime { readonly get; set; }

			// Token: 0x170008F9 RID: 2297
			// (get) Token: 0x06005D18 RID: 23832 RVA: 0x001D8B24 File Offset: 0x001D6D24
			// (set) Token: 0x06005D19 RID: 23833 RVA: 0x001D8B2C File Offset: 0x001D6D2C
			public float GameEndedTime { readonly get; set; }

			// Token: 0x170008FA RID: 2298
			// (get) Token: 0x06005D1A RID: 23834 RVA: 0x001D8B35 File Offset: 0x001D6D35
			// (set) Token: 0x06005D1B RID: 23835 RVA: 0x001D8B3D File Offset: 0x001D6D3D
			public int GameId { readonly get; set; }

			// Token: 0x170008FB RID: 2299
			// (get) Token: 0x06005D1C RID: 23836 RVA: 0x001D8B46 File Offset: 0x001D6D46
			// (set) Token: 0x06005D1D RID: 23837 RVA: 0x001D8B4E File Offset: 0x001D6D4E
			public int PickedMolesIndexCount { readonly get; set; }

			// Token: 0x170008FC RID: 2300
			// (get) Token: 0x06005D1E RID: 23838 RVA: 0x001D8B58 File Offset: 0x001D6D58
			[Networked]
			[Capacity(10)]
			[NetworkedWeavedDictionary(17, 1, 1, typeof(ElementReaderWriterInt32), typeof(ElementReaderWriterInt32))]
			[NetworkedWeaved(139, 71)]
			public unsafe NetworkDictionary<int, int> PickedMolesIndex
			{
				get
				{
					return new NetworkDictionary<int, int>((int*)Native.ReferenceToPointer<FixedStorage@71>(ref this._PickedMolesIndex), 17, ElementReaderWriterInt32.GetInstance(), ElementReaderWriterInt32.GetInstance());
				}
			}

			// Token: 0x06005D1F RID: 23839 RVA: 0x001D8B84 File Offset: 0x001D6D84
			public WhackAMoleData(WhackAMole.GameState state, int currentLevelIndex, int cScore, int tScore, int bScore, int rPScore, string hScorePName, float remainingTime, float endedTime, int gameId, Dictionary<int, int> moleIndexs)
			{
				this.CurrentState = state;
				this.CurrentLevelIndex = currentLevelIndex;
				this.CurrentScore = cScore;
				this.TotalScore = tScore;
				this.BestScore = bScore;
				this.RightPlayerScore = rPScore;
				this.HighScorePlayerName = hScorePName;
				this.RemainingTime = remainingTime;
				this.GameEndedTime = endedTime;
				this.GameId = gameId;
				this.PickedMolesIndexCount = moleIndexs.Count;
				foreach (KeyValuePair<int, int> keyValuePair in moleIndexs)
				{
					this.PickedMolesIndex.Set(keyValuePair.Key, keyValuePair.Value);
				}
			}

			// Token: 0x04006BAF RID: 27567
			[FixedBufferProperty(typeof(NetworkString<_128>), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkString), 0, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(24)]
			private FixedStorage@129 _HighScorePlayerName;

			// Token: 0x04006BB4 RID: 27572
			[FixedBufferProperty(typeof(NetworkDictionary<int, int>), typeof(UnityDictionarySurrogate@ElementReaderWriterInt32@ElementReaderWriterInt32), 17, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(556)]
			private FixedStorage@71 _PickedMolesIndex;
		}
	}
}
