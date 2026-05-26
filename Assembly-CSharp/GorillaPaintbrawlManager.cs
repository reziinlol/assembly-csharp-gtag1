using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaGameModes;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000882 RID: 2178
public sealed class GorillaPaintbrawlManager : GorillaGameManager
{
	// Token: 0x060038B0 RID: 14512 RVA: 0x00135CA3 File Offset: 0x00133EA3
	private void ActivatePaintbrawlBalloons(bool enable)
	{
		if (GorillaTagger.Instance.offlineVRRig != null)
		{
			GorillaTagger.Instance.offlineVRRig.paintbrawlBalloons.gameObject.SetActive(enable);
		}
	}

	// Token: 0x060038B1 RID: 14513 RVA: 0x00135CD1 File Offset: 0x00133ED1
	private bool HasFlag(GorillaPaintbrawlManager.PaintbrawlStatus state, GorillaPaintbrawlManager.PaintbrawlStatus statusFlag)
	{
		return (state & statusFlag) > GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x060038B2 RID: 14514 RVA: 0x00135CD9 File Offset: 0x00133ED9
	public override GameModeType GameType()
	{
		return GameModeType.Paintbrawl;
	}

	// Token: 0x060038B3 RID: 14515 RVA: 0x00135CDC File Offset: 0x00133EDC
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<BattleGameModeData>();
	}

	// Token: 0x060038B4 RID: 14516 RVA: 0x00135CE5 File Offset: 0x00133EE5
	public override string GameModeName()
	{
		return "PAINTBRAWL";
	}

	// Token: 0x060038B5 RID: 14517 RVA: 0x00135CEC File Offset: 0x00133EEC
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_PAINTBRAWL_ROOM_LABEL", out result, "(PAINTBRAWL GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_PAINTBRAWL_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x060038B6 RID: 14518 RVA: 0x00135D18 File Offset: 0x00133F18
	private void ActivateDefaultSlingShot()
	{
		if (this._isDefaultSlingshotSynced && !Slingshot.IsSlingShotEnabled())
		{
			this._isDefaultSlingshotSynced = false;
		}
		if (this._isDefaultSlingshotSynced)
		{
			return;
		}
		Object offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		bool flag = Slingshot.IsSlingShotEnabled();
		if (offlineVRRig != null && !flag)
		{
			CosmeticsController instance = CosmeticsController.instance;
			CosmeticsController.CosmeticItem itemFromDict = instance.GetItemFromDict("Slingshot");
			instance.currentWornSet.HasItemOfCategory(CosmeticsController.CosmeticCategory.Chest);
			instance.currentWornSet.HasItem("Slingshot");
			instance.ApplyCosmeticItemToSet(instance.currentWornSet, itemFromDict, true, false);
			instance.UpdateWornCosmetics(true);
			bool isDefaultSlingshotSynced = instance.currentWornSet.HasItemOfCategory(CosmeticsController.CosmeticCategory.Chest);
			instance.currentWornSet.HasItem("Slingshot");
			this._isDefaultSlingshotSynced = isDefaultSlingshotSynced;
		}
	}

	// Token: 0x060038B7 RID: 14519 RVA: 0x00135DCC File Offset: 0x00133FCC
	private void PreloadSlingshotForActiveRigs(string caller)
	{
		int count = CosmeticsV2Spawner_Dirty._gVRRigDatas.Count;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			CosmeticsV2Spawner_Dirty.VRRigData vrrigData = CosmeticsV2Spawner_Dirty._gVRRigDatas[i];
			if (!(vrrigData.vrRig == null) && !this._slingshotPreloadedRigs.Contains(vrrigData.vrRig))
			{
				CosmeticItemRegistry cosmeticsObjectRegistry = vrrigData.vrRig.cosmeticsObjectRegistry;
				if (cosmeticsObjectRegistry != null)
				{
					CosmeticsV2Spawner_Dirty.ProcessLoadOpInfos(vrrigData.vrRig, "Slingshot", cosmeticsObjectRegistry);
					this._slingshotPreloadedRigs.Add(vrrigData.vrRig);
					num++;
				}
			}
		}
	}

	// Token: 0x060038B8 RID: 14520 RVA: 0x00135E58 File Offset: 0x00134058
	public override void Awake()
	{
		base.Awake();
		this.coroutineRunning = false;
		this.currentState = GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers;
	}

	// Token: 0x060038B9 RID: 14521 RVA: 0x00135E70 File Offset: 0x00134070
	public override void StartPlaying()
	{
		base.StartPlaying();
		this._isDefaultSlingshotSynced = false;
		this._slingshotPreloadedRigs.Clear();
		this.PreloadSlingshotForActiveRigs("StartPlaying");
		this.ActivatePaintbrawlBalloons(true);
		this.VerifyPlayersInDict<int>(this.playerLives);
		this.VerifyPlayersInDict<GorillaPaintbrawlManager.PaintbrawlStatus>(this.playerStatusDict);
		this.VerifyPlayersInDict<float>(this.playerHitTimes);
		this.VerifyPlayersInDict<float>(this.playerStunTimes);
		this.CopyBattleDictToArray();
		this.UpdateBattleState();
	}

	// Token: 0x060038BA RID: 14522 RVA: 0x00135EE4 File Offset: 0x001340E4
	public override void StopPlaying()
	{
		base.StopPlaying();
		this._isDefaultSlingshotSynced = false;
		PlayerPrefs.GetString("slot_Chest", "NOTHING");
		if (Slingshot.IsSlingShotEnabled())
		{
			CosmeticsController instance = CosmeticsController.instance;
			CosmeticsController.CosmeticItem itemFromDict = instance.GetItemFromDict("Slingshot");
			if (instance.currentWornSet.HasItem("Slingshot"))
			{
				instance.ApplyCosmeticItemToSet(instance.currentWornSet, itemFromDict, true, false);
				instance.UpdateWornCosmetics(true);
				instance.currentWornSet.HasItemOfCategory(CosmeticsController.CosmeticCategory.Chest);
				PlayerPrefs.GetString("slot_Chest", "NOTHING");
			}
		}
		this.ActivatePaintbrawlBalloons(false);
		base.StopAllCoroutines();
		this.coroutineRunning = false;
	}

	// Token: 0x060038BB RID: 14523 RVA: 0x00135F84 File Offset: 0x00134184
	public override void ResetGame()
	{
		base.ResetGame();
		this.playerLives.Clear();
		this.playerStatusDict.Clear();
		this.playerHitTimes.Clear();
		this.playerStunTimes.Clear();
		for (int i = 0; i < this.playerActorNumberArray.Length; i++)
		{
			this.playerLivesArray[i] = 0;
			this.playerActorNumberArray[i] = -1;
			this.playerStatusArray[i] = GorillaPaintbrawlManager.PaintbrawlStatus.None;
		}
		this.currentState = GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers;
	}

	// Token: 0x060038BC RID: 14524 RVA: 0x00135FF8 File Offset: 0x001341F8
	private int CopyDictKeysToBuffer<T>(Dictionary<int, T> dict)
	{
		int num = 0;
		foreach (KeyValuePair<int, T> keyValuePair in dict)
		{
			if (num >= this.reusableKeyBuffer.Length)
			{
				break;
			}
			this.reusableKeyBuffer[num++] = keyValuePair.Key;
		}
		return num;
	}

	// Token: 0x060038BD RID: 14525 RVA: 0x00136064 File Offset: 0x00134264
	private void VerifyPlayersInDict<T>(Dictionary<int, T> dict)
	{
		if (dict.Count < 1)
		{
			return;
		}
		int num = this.CopyDictKeysToBuffer<T>(dict);
		for (int i = 0; i < num; i++)
		{
			if (!Utils.PlayerInRoom(this.reusableKeyBuffer[i]))
			{
				dict.Remove(this.reusableKeyBuffer[i]);
			}
		}
	}

	// Token: 0x060038BE RID: 14526 RVA: 0x001360AD File Offset: 0x001342AD
	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<PaintbrawlRPCs>();
	}

	// Token: 0x060038BF RID: 14527 RVA: 0x001360BD File Offset: 0x001342BD
	private void Transition(GorillaPaintbrawlManager.PaintbrawlState newState)
	{
		this.currentState = newState;
		Debug.Log("current state is: " + this.currentState.ToString());
	}

	// Token: 0x060038C0 RID: 14528 RVA: 0x001360E8 File Offset: 0x001342E8
	public void UpdateBattleState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			switch (this.currentState)
			{
			case GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers:
				if ((float)RoomSystem.PlayersInRoom.Count >= this.playerMin)
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.StartCountdown);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameEnd:
				if (this.EndBattleGame())
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.GameEndWaiting);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameEndWaiting:
				if (this.BattleEnd())
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.StartCountdown);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.StartCountdown:
				if (this.teamBattle)
				{
					this.RandomizeTeams();
				}
				base.StartCoroutine(this.StartBattleCountdown());
				this.Transition(GorillaPaintbrawlManager.PaintbrawlState.CountingDownToStart);
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.CountingDownToStart:
				if (!this.coroutineRunning)
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.StartCountdown);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameStart:
				this.StartBattle();
				this.Transition(GorillaPaintbrawlManager.PaintbrawlState.GameRunning);
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameRunning:
				if (this.CheckForGameEnd())
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.GameEnd);
					PlayerGameEvents.GameModeCompleteRound();
					GorillaGameModes.GameMode.BroadcastRoundComplete();
				}
				if ((float)RoomSystem.PlayersInRoom.Count < this.playerMin)
				{
					this.InitializePlayerStatus();
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers);
				}
				break;
			}
			this.UpdatePlayerStatus();
		}
	}

	// Token: 0x060038C1 RID: 14529 RVA: 0x001361FC File Offset: 0x001343FC
	private bool CheckForGameEnd()
	{
		int num = 0;
		this.bcount = 0;
		this.rcount = 0;
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (this.playerLives.TryGetValue(netPlayer.ActorNumber, out this.lives))
			{
				if (this.lives > 0)
				{
					num++;
					if (this.teamBattle && this.playerStatusDict.TryGetValue(netPlayer.ActorNumber, out this.tempStatus))
					{
						if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam))
						{
							this.rcount++;
						}
						else if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam))
						{
							this.bcount++;
						}
					}
				}
			}
			else
			{
				this.playerLives.Add(netPlayer.ActorNumber, 0);
			}
		}
		return (this.teamBattle && (this.bcount == 0 || this.rcount == 0)) || (!this.teamBattle && num <= 1);
	}

	// Token: 0x060038C2 RID: 14530 RVA: 0x00136320 File Offset: 0x00134520
	public IEnumerator StartBattleCountdown()
	{
		this.coroutineRunning = true;
		this.countDownTime = 5;
		while (this.countDownTime > 0)
		{
			try
			{
				RoomSystem.SendSoundEffectAll(6, 0.25f, false);
				foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
				{
					this.playerLives[netPlayer.ActorNumber] = 3;
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
			this.countDownTime--;
		}
		this.coroutineRunning = false;
		this.currentState = GorillaPaintbrawlManager.PaintbrawlState.GameStart;
		yield return null;
		yield break;
	}

	// Token: 0x060038C3 RID: 14531 RVA: 0x00136330 File Offset: 0x00134530
	public void StartBattle()
	{
		RoomSystem.SendSoundEffectAll(7, 0.5f, false);
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			this.playerLives[netPlayer.ActorNumber] = 3;
		}
	}

	// Token: 0x060038C4 RID: 14532 RVA: 0x0013639C File Offset: 0x0013459C
	private bool EndBattleGame()
	{
		if ((float)RoomSystem.PlayersInRoom.Count >= this.playerMin)
		{
			RoomSystem.SendStatusEffectAll(RoomSystem.StatusEffects.TaggedTime);
			RoomSystem.SendSoundEffectAll(2, 0.25f, false);
			this.timeBattleEnded = Time.time;
			return true;
		}
		return false;
	}

	// Token: 0x060038C5 RID: 14533 RVA: 0x001363D1 File Offset: 0x001345D1
	public bool BattleEnd()
	{
		return Time.time > this.timeBattleEnded + this.tagCoolDown;
	}

	// Token: 0x060038C6 RID: 14534 RVA: 0x001363E7 File Offset: 0x001345E7
	public bool SlingshotHit(NetPlayer myPlayer, Player otherPlayer)
	{
		return this.playerLives.TryGetValue(otherPlayer.ActorNumber, out this.lives) && this.lives > 0;
	}

	// Token: 0x060038C7 RID: 14535 RVA: 0x00136410 File Offset: 0x00134610
	public void ReportSlingshotHit(NetPlayer taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (this.currentState != GorillaPaintbrawlManager.PaintbrawlState.GameRunning)
		{
			return;
		}
		if (this.OnSameTeam(taggedPlayer, player))
		{
			return;
		}
		if (this.GetPlayerLives(taggedPlayer) > 0 && this.GetPlayerLives(player) > 0 && !this.PlayerInHitCooldown(taggedPlayer))
		{
			if (!this.playerHitTimes.TryGetValue(taggedPlayer.ActorNumber, out this.outHitTime))
			{
				this.playerHitTimes.Add(taggedPlayer.ActorNumber, Time.time);
			}
			else
			{
				this.playerHitTimes[taggedPlayer.ActorNumber] = Time.time;
			}
			Dictionary<int, int> dictionary = this.playerLives;
			int actorNumber = taggedPlayer.ActorNumber;
			int num = dictionary[actorNumber];
			dictionary[actorNumber] = num - 1;
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, taggedPlayer, false);
			return;
		}
		if (this.GetPlayerLives(player) == 0 && this.GetPlayerLives(taggedPlayer) > 0)
		{
			this.tempStatus = this.GetPlayerStatus(taggedPlayer);
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal) && !this.PlayerInHitCooldown(taggedPlayer) && !this.PlayerInStunCooldown(taggedPlayer))
			{
				if (!this.playerStunTimes.TryGetValue(taggedPlayer.ActorNumber, out this.outHitTime))
				{
					this.playerStunTimes.Add(taggedPlayer.ActorNumber, Time.time);
				}
				else
				{
					this.playerStunTimes[taggedPlayer.ActorNumber] = Time.time;
				}
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.SetSlowedTime, taggedPlayer);
				RoomSystem.SendSoundEffectOnOther(5, 0.125f, taggedPlayer, false);
				RigContainer rigContainer;
				if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer))
				{
					this.tempView = rigContainer.Rig.netView;
				}
			}
		}
	}

	// Token: 0x060038C8 RID: 14536 RVA: 0x001365AC File Offset: 0x001347AC
	public override void HitPlayer(NetPlayer player)
	{
		if (!NetworkSystem.Instance.IsMasterClient || this.currentState != GorillaPaintbrawlManager.PaintbrawlState.GameRunning)
		{
			return;
		}
		if (this.GetPlayerLives(player) > 0)
		{
			this.playerLives[player.ActorNumber] = 0;
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, player, false);
		}
	}

	// Token: 0x060038C9 RID: 14537 RVA: 0x001365F8 File Offset: 0x001347F8
	public override bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		return this.playerLives.TryGetValue(player.ActorNumber, out this.lives) && this.lives > 0;
	}

	// Token: 0x060038CA RID: 14538 RVA: 0x00136620 File Offset: 0x00134820
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.currentState == GorillaPaintbrawlManager.PaintbrawlState.GameRunning)
			{
				this.playerLives.Add(newPlayer.ActorNumber, 0);
			}
			else
			{
				this.playerLives.Add(newPlayer.ActorNumber, 3);
			}
			this.playerStatusDict.Add(newPlayer.ActorNumber, GorillaPaintbrawlManager.PaintbrawlStatus.None);
			this.CopyBattleDictToArray();
			if (this.teamBattle)
			{
				this.AddPlayerToCorrectTeam(newPlayer);
			}
		}
	}

	// Token: 0x060038CB RID: 14539 RVA: 0x00136698 File Offset: 0x00134898
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (this.playerLives.ContainsKey(otherPlayer.ActorNumber))
		{
			this.playerLives.Remove(otherPlayer.ActorNumber);
		}
		if (this.playerStatusDict.ContainsKey(otherPlayer.ActorNumber))
		{
			this.playerStatusDict.Remove(otherPlayer.ActorNumber);
		}
	}

	// Token: 0x060038CC RID: 14540 RVA: 0x001366F8 File Offset: 0x001348F8
	public override void OnSerializeRead(object newData)
	{
		PaintbrawlData paintbrawlData = (PaintbrawlData)newData;
		paintbrawlData.playerActorNumberArray.CopyTo(this.playerActorNumberArray, true);
		paintbrawlData.playerLivesArray.CopyTo(this.playerLivesArray, true);
		paintbrawlData.playerStatusArray.CopyTo(this.playerStatusArray, true);
		this.currentState = paintbrawlData.currentPaintbrawlState;
		this.CopyArrayToBattleDict();
	}

	// Token: 0x060038CD RID: 14541 RVA: 0x00136760 File Offset: 0x00134960
	public override object OnSerializeWrite()
	{
		this.CopyBattleDictToArray();
		PaintbrawlData paintbrawlData = default(PaintbrawlData);
		paintbrawlData.playerActorNumberArray.CopyFrom(this.playerActorNumberArray, 0, this.playerActorNumberArray.Length);
		paintbrawlData.playerLivesArray.CopyFrom(this.playerLivesArray, 0, this.playerLivesArray.Length);
		paintbrawlData.playerStatusArray.CopyFrom(this.playerStatusArray, 0, this.playerStatusArray.Length);
		paintbrawlData.currentPaintbrawlState = this.currentState;
		return paintbrawlData;
	}

	// Token: 0x060038CE RID: 14542 RVA: 0x001367E8 File Offset: 0x001349E8
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyBattleDictToArray();
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			stream.SendNext(this.playerActorNumberArray[i]);
			stream.SendNext(this.playerLivesArray[i]);
			stream.SendNext(this.playerStatusArray[i]);
		}
		stream.SendNext((int)this.currentState);
	}

	// Token: 0x060038CF RID: 14543 RVA: 0x00136858 File Offset: 0x00134A58
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			this.playerActorNumberArray[i] = (int)stream.ReceiveNext();
			this.playerLivesArray[i] = (int)stream.ReceiveNext();
			this.playerStatusArray[i] = (GorillaPaintbrawlManager.PaintbrawlStatus)stream.ReceiveNext();
		}
		this.currentState = (GorillaPaintbrawlManager.PaintbrawlState)stream.ReceiveNext();
		this.CopyArrayToBattleDict();
	}

	// Token: 0x060038D0 RID: 14544 RVA: 0x001368DC File Offset: 0x00134ADC
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		this.tempStatus = this.GetPlayerStatus(forPlayer);
		if (this.tempStatus != GorillaPaintbrawlManager.PaintbrawlStatus.None)
		{
			if (this.OnRedTeam(this.tempStatus))
			{
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
				{
					return 8;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Hit))
				{
					return 9;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
				{
					return 10;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Grace))
				{
					return 10;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
				{
					return 11;
				}
			}
			else if (this.OnBlueTeam(this.tempStatus))
			{
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
				{
					return 4;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Hit))
				{
					return 5;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
				{
					return 6;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Grace))
				{
					return 6;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
				{
					return 7;
				}
			}
			else
			{
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
				{
					return 0;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Hit))
				{
					return 1;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
				{
					return 17;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Grace))
				{
					return 17;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
				{
					return 16;
				}
			}
		}
		return 0;
	}

	// Token: 0x060038D1 RID: 14545 RVA: 0x00136A30 File Offset: 0x00134C30
	public override float[] LocalPlayerSpeed()
	{
		if (this.playerStatusDict.TryGetValue(NetworkSystem.Instance.LocalPlayerID, out this.tempStatus))
		{
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
			{
				this.playerSpeed[0] = 6.5f;
				this.playerSpeed[1] = 1.1f;
				return this.playerSpeed;
			}
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
			{
				this.playerSpeed[0] = 2f;
				this.playerSpeed[1] = 0.5f;
				return this.playerSpeed;
			}
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
			{
				this.playerSpeed[0] = this.fastJumpLimit;
				this.playerSpeed[1] = this.fastJumpMultiplier;
				return this.playerSpeed;
			}
		}
		this.playerSpeed[0] = 6.5f;
		this.playerSpeed[1] = 1.1f;
		return this.playerSpeed;
	}

	// Token: 0x060038D2 RID: 14546 RVA: 0x00136B11 File Offset: 0x00134D11
	public override void Tick()
	{
		base.Tick();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.UpdateBattleState();
		}
		this.PreloadSlingshotForActiveRigs(null);
		this.ActivateDefaultSlingShot();
	}

	// Token: 0x060038D3 RID: 14547 RVA: 0x00136B38 File Offset: 0x00134D38
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
		foreach (int num in this.playerLives.Keys)
		{
			this.playerInList = false;
			using (List<NetPlayer>.Enumerator enumerator2 = RoomSystem.PlayersInRoom.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.ActorNumber == num)
					{
						this.playerInList = true;
					}
				}
			}
			if (!this.playerInList)
			{
				this.playerLives.Remove(num);
			}
		}
	}

	// Token: 0x060038D4 RID: 14548 RVA: 0x00136BF4 File Offset: 0x00134DF4
	public int GetPlayerLives(NetPlayer player)
	{
		if (player == null)
		{
			return 0;
		}
		if (this.playerLives.TryGetValue(player.ActorNumber, out this.outLives))
		{
			return this.outLives;
		}
		return 0;
	}

	// Token: 0x060038D5 RID: 14549 RVA: 0x00136C1C File Offset: 0x00134E1C
	public bool PlayerInHitCooldown(NetPlayer player)
	{
		float num;
		return this.playerHitTimes.TryGetValue(player.ActorNumber, out num) && num + this.hitCooldown > Time.time;
	}

	// Token: 0x060038D6 RID: 14550 RVA: 0x00136C50 File Offset: 0x00134E50
	public bool PlayerInStunCooldown(NetPlayer player)
	{
		float num;
		return this.playerStunTimes.TryGetValue(player.ActorNumber, out num) && num + this.hitCooldown + this.stunGracePeriod > Time.time;
	}

	// Token: 0x060038D7 RID: 14551 RVA: 0x00136C8A File Offset: 0x00134E8A
	public GorillaPaintbrawlManager.PaintbrawlStatus GetPlayerStatus(NetPlayer player)
	{
		if (this.playerStatusDict.TryGetValue(player.ActorNumber, out this.tempStatus))
		{
			return this.tempStatus;
		}
		return GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x060038D8 RID: 14552 RVA: 0x00136CAD File Offset: 0x00134EAD
	public bool OnRedTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		return this.HasFlag(status, GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam);
	}

	// Token: 0x060038D9 RID: 14553 RVA: 0x00136CB8 File Offset: 0x00134EB8
	public bool OnRedTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnRedTeam(playerStatus);
	}

	// Token: 0x060038DA RID: 14554 RVA: 0x00136CD4 File Offset: 0x00134ED4
	public bool OnBlueTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		return this.HasFlag(status, GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam);
	}

	// Token: 0x060038DB RID: 14555 RVA: 0x00136CE0 File Offset: 0x00134EE0
	public bool OnBlueTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnBlueTeam(playerStatus);
	}

	// Token: 0x060038DC RID: 14556 RVA: 0x00136CFC File Offset: 0x00134EFC
	public bool OnNoTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		return !this.OnRedTeam(status) && !this.OnBlueTeam(status);
	}

	// Token: 0x060038DD RID: 14557 RVA: 0x00136D14 File Offset: 0x00134F14
	public bool OnNoTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnNoTeam(playerStatus);
	}

	// Token: 0x060038DE RID: 14558 RVA: 0x00136D30 File Offset: 0x00134F30
	public GorillaPaintbrawlManager.PaintbrawlStatus GetPlayerTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		if (this.OnRedTeam(status))
		{
			return GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam;
		}
		if (this.OnBlueTeam(status))
		{
			return GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam;
		}
		return GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x060038DF RID: 14559 RVA: 0x00136D4C File Offset: 0x00134F4C
	public GorillaPaintbrawlManager.PaintbrawlStatus GetPlayerTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.GetPlayerTeam(playerStatus);
	}

	// Token: 0x060038E0 RID: 14560 RVA: 0x00002076 File Offset: 0x00000276
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return false;
	}

	// Token: 0x060038E1 RID: 14561 RVA: 0x00136D68 File Offset: 0x00134F68
	public override bool LocalIsTagged(NetPlayer player)
	{
		return this.GetPlayerLives(player) == 0;
	}

	// Token: 0x060038E2 RID: 14562 RVA: 0x00136D74 File Offset: 0x00134F74
	public bool OnSameTeam(GorillaPaintbrawlManager.PaintbrawlStatus playerA, GorillaPaintbrawlManager.PaintbrawlStatus playerB)
	{
		bool flag = this.OnRedTeam(playerA) && this.OnRedTeam(playerB);
		bool flag2 = this.OnBlueTeam(playerA) && this.OnBlueTeam(playerB);
		return flag || flag2;
	}

	// Token: 0x060038E3 RID: 14563 RVA: 0x00136DAC File Offset: 0x00134FAC
	public bool OnSameTeam(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(myPlayer);
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus2 = this.GetPlayerStatus(otherPlayer);
		return this.OnSameTeam(playerStatus, playerStatus2);
	}

	// Token: 0x060038E4 RID: 14564 RVA: 0x00136DD4 File Offset: 0x00134FD4
	public bool LocalCanHit(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		bool flag = !this.OnSameTeam(myPlayer, otherPlayer);
		bool flag2 = this.GetPlayerLives(otherPlayer) != 0;
		return flag && flag2;
	}

	// Token: 0x060038E5 RID: 14565 RVA: 0x00136DFC File Offset: 0x00134FFC
	private void CopyBattleDictToArray()
	{
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			this.playerLivesArray[i] = 0;
			this.playerActorNumberArray[i] = -1;
		}
		int num = 0;
		foreach (KeyValuePair<int, int> keyValuePair in this.playerLives)
		{
			if (num >= this.playerLivesArray.Length)
			{
				break;
			}
			this.playerActorNumberArray[num] = keyValuePair.Key;
			this.playerLivesArray[num] = keyValuePair.Value;
			this.playerStatusArray[num] = this.GetPlayerStatus(NetworkSystem.Instance.GetPlayer(keyValuePair.Key));
			num++;
		}
	}

	// Token: 0x060038E6 RID: 14566 RVA: 0x00136EC0 File Offset: 0x001350C0
	private void CopyArrayToBattleDict()
	{
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			if (this.playerActorNumberArray[i] != -1 && Utils.PlayerInRoom(this.playerActorNumberArray[i]))
			{
				if (this.playerLives.TryGetValue(this.playerActorNumberArray[i], out this.outLives))
				{
					this.playerLives[this.playerActorNumberArray[i]] = this.playerLivesArray[i];
				}
				else
				{
					this.playerLives.Add(this.playerActorNumberArray[i], this.playerLivesArray[i]);
				}
				if (this.playerStatusDict.ContainsKey(this.playerActorNumberArray[i]))
				{
					this.playerStatusDict[this.playerActorNumberArray[i]] = this.playerStatusArray[i];
				}
				else
				{
					this.playerStatusDict.Add(this.playerActorNumberArray[i], this.playerStatusArray[i]);
				}
			}
		}
	}

	// Token: 0x060038E7 RID: 14567 RVA: 0x00136FA6 File Offset: 0x001351A6
	private GorillaPaintbrawlManager.PaintbrawlStatus SetFlag(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return currState | flag;
	}

	// Token: 0x060038E8 RID: 14568 RVA: 0x000F9EC7 File Offset: 0x000F80C7
	private GorillaPaintbrawlManager.PaintbrawlStatus SetFlagExclusive(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return flag;
	}

	// Token: 0x060038E9 RID: 14569 RVA: 0x00136FAB File Offset: 0x001351AB
	private GorillaPaintbrawlManager.PaintbrawlStatus ClearFlag(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return currState & ~flag;
	}

	// Token: 0x060038EA RID: 14570 RVA: 0x00135CD1 File Offset: 0x00133ED1
	private bool FlagIsSet(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return (currState & flag) > GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x060038EB RID: 14571 RVA: 0x00136FB4 File Offset: 0x001351B4
	public void RandomizeTeams()
	{
		int[] array = new int[RoomSystem.PlayersInRoom.Count];
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			array[i] = i;
		}
		Random rand = new Random();
		int[] array2 = (from x in array
		orderby rand.Next()
		select x).ToArray<int>();
		GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus = (rand.Next(0, 2) == 0) ? GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam : GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam;
		GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus2 = (paintbrawlStatus == GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) ? GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam : GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam;
		for (int j = 0; j < RoomSystem.PlayersInRoom.Count; j++)
		{
			GorillaPaintbrawlManager.PaintbrawlStatus value = (array2[j] % 2 == 0) ? paintbrawlStatus2 : paintbrawlStatus;
			this.playerStatusDict[RoomSystem.PlayersInRoom[j].ActorNumber] = value;
		}
	}

	// Token: 0x060038EC RID: 14572 RVA: 0x00137080 File Offset: 0x00135280
	public void AddPlayerToCorrectTeam(NetPlayer newPlayer)
	{
		this.rcount = 0;
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			if (this.playerStatusDict.ContainsKey(RoomSystem.PlayersInRoom[i].ActorNumber))
			{
				GorillaPaintbrawlManager.PaintbrawlStatus state = this.playerStatusDict[RoomSystem.PlayersInRoom[i].ActorNumber];
				this.rcount = (this.HasFlag(state, GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) ? (this.rcount + 1) : this.rcount);
			}
		}
		if ((RoomSystem.PlayersInRoom.Count - 1) / 2 == this.rcount)
		{
			this.playerStatusDict[newPlayer.ActorNumber] = ((Random.Range(0, 2) == 0) ? this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) : this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam));
			return;
		}
		if (this.rcount <= (RoomSystem.PlayersInRoom.Count - 1) / 2)
		{
			this.playerStatusDict[newPlayer.ActorNumber] = this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam);
		}
	}

	// Token: 0x060038ED RID: 14573 RVA: 0x001371A4 File Offset: 0x001353A4
	private void InitializePlayerStatus()
	{
		int num = this.CopyDictKeysToBuffer<GorillaPaintbrawlManager.PaintbrawlStatus>(this.playerStatusDict);
		for (int i = 0; i < num; i++)
		{
			this.playerStatusDict[this.reusableKeyBuffer[i]] = GorillaPaintbrawlManager.PaintbrawlStatus.Normal;
		}
	}

	// Token: 0x060038EE RID: 14574 RVA: 0x001371E0 File Offset: 0x001353E0
	private void UpdatePlayerStatus()
	{
		int num = this.CopyDictKeysToBuffer<GorillaPaintbrawlManager.PaintbrawlStatus>(this.playerStatusDict);
		for (int i = 0; i < num; i++)
		{
			int key = this.reusableKeyBuffer[i];
			GorillaPaintbrawlManager.PaintbrawlStatus playerTeam = this.GetPlayerTeam(this.playerStatusDict[key]);
			if (this.playerLives.TryGetValue(key, out this.outLives) && this.outLives == 0)
			{
				this.playerStatusDict[key] = (playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated);
			}
			else if (this.playerHitTimes.TryGetValue(key, out this.outHitTime) && this.outHitTime + this.hitCooldown > Time.time)
			{
				this.playerStatusDict[key] = (playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Hit);
			}
			else if (this.playerStunTimes.TryGetValue(key, out this.outHitTime))
			{
				if (this.outHitTime + this.hitCooldown > Time.time)
				{
					this.playerStatusDict[key] = (playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Stunned);
				}
				else if (this.outHitTime + this.hitCooldown + this.stunGracePeriod > Time.time)
				{
					this.playerStatusDict[key] = (playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Grace);
				}
				else
				{
					this.playerStatusDict[key] = (playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Normal);
				}
			}
			else
			{
				this.playerStatusDict[key] = (playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Normal);
			}
		}
	}

	// Token: 0x040048CA RID: 18634
	private float playerMin = 2f;

	// Token: 0x040048CB RID: 18635
	public float tagCoolDown = 5f;

	// Token: 0x040048CC RID: 18636
	public Dictionary<int, int> playerLives = new Dictionary<int, int>();

	// Token: 0x040048CD RID: 18637
	public Dictionary<int, GorillaPaintbrawlManager.PaintbrawlStatus> playerStatusDict = new Dictionary<int, GorillaPaintbrawlManager.PaintbrawlStatus>();

	// Token: 0x040048CE RID: 18638
	public Dictionary<int, float> playerHitTimes = new Dictionary<int, float>();

	// Token: 0x040048CF RID: 18639
	public Dictionary<int, float> playerStunTimes = new Dictionary<int, float>();

	// Token: 0x040048D0 RID: 18640
	public int[] playerActorNumberArray = new int[]
	{
		-1,
		-1,
		-1,
		-1,
		-1,
		-1,
		-1,
		-1,
		-1,
		-1
	};

	// Token: 0x040048D1 RID: 18641
	public int[] playerLivesArray = new int[10];

	// Token: 0x040048D2 RID: 18642
	public GorillaPaintbrawlManager.PaintbrawlStatus[] playerStatusArray = new GorillaPaintbrawlManager.PaintbrawlStatus[10];

	// Token: 0x040048D3 RID: 18643
	public bool teamBattle = true;

	// Token: 0x040048D4 RID: 18644
	public int countDownTime;

	// Token: 0x040048D5 RID: 18645
	private float timeBattleEnded;

	// Token: 0x040048D6 RID: 18646
	public float hitCooldown = 3f;

	// Token: 0x040048D7 RID: 18647
	public float stunGracePeriod = 2f;

	// Token: 0x040048D8 RID: 18648
	public object objRef;

	// Token: 0x040048D9 RID: 18649
	private bool playerInList;

	// Token: 0x040048DA RID: 18650
	private bool coroutineRunning;

	// Token: 0x040048DB RID: 18651
	private int lives;

	// Token: 0x040048DC RID: 18652
	private int outLives;

	// Token: 0x040048DD RID: 18653
	private int bcount;

	// Token: 0x040048DE RID: 18654
	private int rcount;

	// Token: 0x040048DF RID: 18655
	private int randInt;

	// Token: 0x040048E0 RID: 18656
	private float outHitTime;

	// Token: 0x040048E1 RID: 18657
	private NetworkView tempView;

	// Token: 0x040048E2 RID: 18658
	private int[] reusableKeyBuffer = new int[20];

	// Token: 0x040048E3 RID: 18659
	private GorillaPaintbrawlManager.PaintbrawlStatus tempStatus;

	// Token: 0x040048E4 RID: 18660
	private GorillaPaintbrawlManager.PaintbrawlState currentState;

	// Token: 0x040048E5 RID: 18661
	private bool _isDefaultSlingshotSynced;

	// Token: 0x040048E6 RID: 18662
	private readonly HashSet<VRRig> _slingshotPreloadedRigs = new HashSet<VRRig>(20);

	// Token: 0x02000883 RID: 2179
	public enum PaintbrawlStatus
	{
		// Token: 0x040048E8 RID: 18664
		RedTeam = 1,
		// Token: 0x040048E9 RID: 18665
		BlueTeam,
		// Token: 0x040048EA RID: 18666
		Normal = 4,
		// Token: 0x040048EB RID: 18667
		Hit = 8,
		// Token: 0x040048EC RID: 18668
		Stunned = 16,
		// Token: 0x040048ED RID: 18669
		Grace = 32,
		// Token: 0x040048EE RID: 18670
		Eliminated = 64,
		// Token: 0x040048EF RID: 18671
		None = 0
	}

	// Token: 0x02000884 RID: 2180
	public enum PaintbrawlState
	{
		// Token: 0x040048F1 RID: 18673
		NotEnoughPlayers,
		// Token: 0x040048F2 RID: 18674
		GameEnd,
		// Token: 0x040048F3 RID: 18675
		GameEndWaiting,
		// Token: 0x040048F4 RID: 18676
		StartCountdown,
		// Token: 0x040048F5 RID: 18677
		CountingDownToStart,
		// Token: 0x040048F6 RID: 18678
		GameStart,
		// Token: 0x040048F7 RID: 18679
		GameRunning
	}
}
