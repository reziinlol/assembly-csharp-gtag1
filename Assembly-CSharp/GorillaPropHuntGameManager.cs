using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x0200026C RID: 620
public sealed class GorillaPropHuntGameManager : GorillaTagManager
{
	// Token: 0x1700019E RID: 414
	// (get) Token: 0x0600108F RID: 4239 RVA: 0x000589DA File Offset: 0x00056BDA
	// (set) Token: 0x06001090 RID: 4240 RVA: 0x000589E1 File Offset: 0x00056BE1
	public new static GorillaPropHuntGameManager instance { get; private set; }

	// Token: 0x06001091 RID: 4241 RVA: 0x000074B9 File Offset: 0x000056B9
	public override GameModeType GameType()
	{
		return GameModeType.PropHunt;
	}

	// Token: 0x06001092 RID: 4242 RVA: 0x000589E9 File Offset: 0x00056BE9
	public override string GameModeName()
	{
		return "PROP HUNT";
	}

	// Token: 0x06001093 RID: 4243 RVA: 0x000589F0 File Offset: 0x00056BF0
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_PROP_HUNT_ROOM_LABEL", out result, "(PROP HUNT GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_PROP_HUNT_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x06001094 RID: 4244 RVA: 0x00058A1B File Offset: 0x00056C1B
	public PropPlacementRB PropDecoyPrefab
	{
		get
		{
			return this.m_ph_propDecoyPrefab;
		}
	}

	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x06001095 RID: 4245 RVA: 0x00058A23 File Offset: 0x00056C23
	public float HandFollowDistance
	{
		get
		{
			return this.m_ph_hand_follow_distance;
		}
	}

	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x06001096 RID: 4246 RVA: 0x00058A2B File Offset: 0x00056C2B
	public bool RoundIsPlaying
	{
		get
		{
			return this._roundIsPlaying;
		}
	}

	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x06001097 RID: 4247 RVA: 0x00058A33 File Offset: 0x00056C33
	public string[] AllPropIDs_NoPool
	{
		get
		{
			return PropHuntPools.AllPropCosmeticIds;
		}
	}

	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x06001098 RID: 4248 RVA: 0x00058A3A File Offset: 0x00056C3A
	// (set) Token: 0x06001099 RID: 4249 RVA: 0x00058A42 File Offset: 0x00056C42
	[DebugReadout]
	private long _ph_timeRoundStartedMillis
	{
		get
		{
			return this.__ph_timeRoundStartedMillis__;
		}
		set
		{
			this.__ph_timeRoundStartedMillis__ = value;
		}
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x00058A4B File Offset: 0x00056C4B
	public int GetSeed()
	{
		return this._ph_randomSeed;
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x00058A53 File Offset: 0x00056C53
	public override void Awake()
	{
		GorillaPropHuntGameManager.instance = this;
		PhotonNetwork.AddCallbackTarget(this);
		base.Awake();
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x00058A67 File Offset: 0x00056C67
	private void Start()
	{
		PropHuntPools.StartInitializingPropsList(this.m_ph_allCosmetics, this.m_ph_fallbackPropCosmeticSO);
		if (this._ph_gorillaGhostBodyMaterialIndex == -1)
		{
			this._Initialize_gorillaGhostBodyMaterialIndex();
		}
		this._Initialize_defaultStencilRefOfSkeletonMat();
	}

	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x0600109D RID: 4253 RVA: 0x00058A8F File Offset: 0x00056C8F
	public bool IsReadyToSpawnProps_NoPool
	{
		get
		{
			return PropHuntPools.IsReady;
		}
	}

	// Token: 0x0600109E RID: 4254 RVA: 0x00058A96 File Offset: 0x00056C96
	private void _ProcessPropsList_NoPool(string titleDataPropsLines)
	{
		this._ph_allPropIDs_noPool = titleDataPropsLines.Split(GorillaPropHuntGameManager._g_ph_titleDataSeparators, StringSplitOptions.RemoveEmptyEntries);
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x00058AAC File Offset: 0x00056CAC
	public override void StartPlaying()
	{
		base.StartPlaying();
		bool isMasterClient = PhotonNetwork.IsMasterClient;
		this._ResolveXSceneRefs();
		GameMode.ParticipatingPlayersChanged += this._OnParticipatingPlayersChanged;
		this._UpdateParticipatingPlayers();
		if (this.m_ph_soundNearBorder_audioSource != null)
		{
			this.m_ph_soundNearBorder_audioSource.volume = 0f;
		}
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x00058B00 File Offset: 0x00056D00
	public override void StopPlaying()
	{
		base.StopPlaying();
		this._ph_gameState = GorillaPropHuntGameManager.EPropHuntGameState.StoppedGameMode;
		GameMode.ParticipatingPlayersChanged -= this._OnParticipatingPlayersChanged;
		foreach (VRRig rig in VRRigCache.ActiveRigs)
		{
			GorillaSkin.ApplyToRig(rig, null, GorillaSkin.SkinType.gameMode);
			this._ResetRigAppearance(rig);
		}
		CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
		EquipmentInteractor.instance.ForceDropAnyEquipment();
		if (this.m_ph_soundNearBorder_audioSource != null)
		{
			this.m_ph_soundNearBorder_audioSource.volume = 0f;
		}
		if (this._ph_playBoundary_isResolved)
		{
			this._ph_playBoundary.enabled = false;
			if (this._ph_playBoundary_initialPosition_isInitialized)
			{
				this._ph_playBoundary.transform.position = this._ph_playBoundary_initialPosition;
			}
		}
		this._ph_playBoundary_hasTargetPositionForRound = false;
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x00058BE4 File Offset: 0x00056DE4
	public override bool CanPlayerParticipate(NetPlayer player)
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			VRRig rig = rigContainer.Rig;
			return rig.zoneEntity.currentZone == GTZone.bayou && rig.zoneEntity.currentSubZone != GTSubZone.entrance_tunnel;
		}
		return true;
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x00058C2C File Offset: 0x00056E2C
	private void _OnParticipatingPlayersChanged(List<NetPlayer> addedPlayers, List<NetPlayer> removedPlayers)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			for (int i = 0; i < addedPlayers.Count; i++)
			{
				NetPlayer infectedPlayer = addedPlayers[i];
				this.AddInfectedPlayer(infectedPlayer, true);
			}
		}
		for (int j = 0; j < removedPlayers.Count; j++)
		{
			NetPlayer netPlayer = removedPlayers[j];
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
			{
				if (PhotonNetwork.IsMasterClient)
				{
					while (this.currentInfected.Contains(netPlayer))
					{
						this.currentInfected.Remove(netPlayer);
					}
				}
				VRRig rig = rigContainer.Rig;
				this._ResetRigAppearance(rig);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.UpdateInfectionState();
		}
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x00058CCB File Offset: 0x00056ECB
	public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			bool isCurrentlyTag = this.isCurrentlyTag;
			this.UpdateState();
			if (!isCurrentlyTag && !this.isCurrentlyTag)
			{
				this.UpdateInfectionState();
			}
		}
	}

	// Token: 0x060010A4 RID: 4260 RVA: 0x00058CF8 File Offset: 0x00056EF8
	public override void Tick()
	{
		base.Tick();
		this._UpdateParticipatingPlayers();
		this._UpdateGameState();
		if (this._ph_playBoundary_isResolved)
		{
			this._ph_playBoundary.enabled = this._ph_isLocalPlayerParticipating;
			float num = (this._ph_gameState != GorillaPropHuntGameManager.EPropHuntGameState.Playing) ? 0f : Mathf.Clamp01(this._ph_roundTime / this.m_ph_playBoundary_radiusScaleOverRoundTime_maxTime);
			this._ph_playBoundary.radiusScale = this.m_ph_playBoundary_radiusScaleOverRoundTime_curve.Evaluate(num);
			if (this._ph_playBoundary_hasTargetPositionForRound)
			{
				Vector3 position = Vector3.Lerp(this._ph_playBoundary_initialPosition, this._ph_playBoundary_currentTargetPosition, num);
				this._ph_playBoundary.transform.position = position;
			}
			if (this._ph_isLocalPlayerParticipating || (PhotonNetwork.IsMasterClient && GameMode.ParticipatingPlayers.Count > 0))
			{
				this._ph_playBoundary.UpdateSim();
			}
		}
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x00058DC0 File Offset: 0x00056FC0
	public void _UpdateParticipatingPlayers()
	{
		VRRigCache.Instance.GetActiveRigs(GorillaPropHuntGameManager._g_ph_activePlayerRigs);
		for (int i = 0; i < GorillaPropHuntGameManager._g_ph_activePlayerRigs.Count; i++)
		{
			VRRig vrrig = GorillaPropHuntGameManager._g_ph_activePlayerRigs[i];
			bool flag = vrrig.zoneEntity.currentZone == GTZone.bayou && vrrig.zoneEntity.currentSubZone != GTSubZone.entrance_tunnel;
			bool flag2 = GameMode.ParticipatingPlayers.Contains(vrrig.OwningNetPlayer);
			if (flag && !flag2)
			{
				GameMode.OptIn(vrrig.OwningNetPlayer.ActorNumber);
			}
			else if (!flag && flag2)
			{
				GameMode.OptOut(vrrig.OwningNetPlayer.ActorNumber);
				this._SetPlayerBlindfoldVisibility(vrrig, vrrig.OwningNetPlayer, false);
			}
		}
		this._ph_isLocalPlayerParticipating = GameMode.ParticipatingPlayers.Contains(VRRig.LocalRig.OwningNetPlayer);
		this.m_ph_soundNearBorder_audioSource.gameObject.SetActive(this._ph_isLocalPlayerParticipating);
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x00058EA8 File Offset: 0x000570A8
	private void _UpdateGameState()
	{
		this._ph_gameState_lastUpdate = this._ph_gameState;
		long num = GTTime.TimeAsMilliseconds();
		if (GameMode.ParticipatingPlayers.Count < this.infectedModeThreshold)
		{
			this._ph_gameState = GorillaPropHuntGameManager.EPropHuntGameState.WaitingForMorePlayers;
			this._ph_roundTime = 0f;
		}
		else if (this._ph_timeRoundStartedMillis <= 0L || num < this._ph_timeRoundStartedMillis)
		{
			this._ph_gameState = GorillaPropHuntGameManager.EPropHuntGameState.WaitingForRoundToStart;
			this._ph_roundTime = 0f;
		}
		else
		{
			this._ph_roundTime = (float)(num - this._ph_timeRoundStartedMillis) / 1000f;
			this._ph_gameState = ((this._ph_roundTime < this.m_ph_hideState_duration) ? GorillaPropHuntGameManager.EPropHuntGameState.Hiding : GorillaPropHuntGameManager.EPropHuntGameState.Playing);
		}
		if (this._ph_gameState != this._ph_gameState_lastUpdate)
		{
			foreach (PlayableBoundaryTracker playableBoundaryTracker in GorillaPropHuntGameManager._g_ph_rig_to_propHuntZoneTrackers.Values)
			{
				playableBoundaryTracker.ResetValues();
			}
		}
		PlayableBoundaryTracker playableBoundaryTracker2;
		if (!this._ph_isLocalPlayerParticipating && GorillaPropHuntGameManager._g_ph_rig_to_propHuntZoneTrackers.TryGetValue(VRRig.LocalRig.GetInstanceID(), out playableBoundaryTracker2))
		{
			playableBoundaryTracker2.ResetValues();
		}
		switch (this._ph_gameState)
		{
		case GorillaPropHuntGameManager.EPropHuntGameState.Invalid:
			Debug.LogError("ERROR!!!  GorillaPropHuntGameManager: " + string.Format("Game state was `{0}` but should only be that when the app ", GorillaPropHuntGameManager.EPropHuntGameState.Invalid) + "starts and then assigned during `StartPlaying` call.");
			return;
		case GorillaPropHuntGameManager.EPropHuntGameState.StoppedGameMode:
		case GorillaPropHuntGameManager.EPropHuntGameState.StartingGameMode:
		case GorillaPropHuntGameManager.EPropHuntGameState.WaitingForMorePlayers:
			if (this._ph_gameState != this._ph_gameState_lastUpdate)
			{
				this._ph_hideState_warnSounds_timesPlayed = 0;
				VRRig rig = VRRigCache.Instance.localRig.Rig;
				this._ph_timeRoundStartedMillis = -1000L;
				this._ResetRigAppearance(rig);
				return;
			}
			break;
		case GorillaPropHuntGameManager.EPropHuntGameState.WaitingForRoundToStart:
			this._ph_hideState_warnSounds_timesPlayed = 0;
			if (PhotonNetwork.IsMasterClient && !this.waitingToStartNextInfectionGame)
			{
				base.ClearInfectionState();
				this.InfectionRoundEnd();
				return;
			}
			break;
		case GorillaPropHuntGameManager.EPropHuntGameState.Hiding:
		{
			if (this._ph_gameState != this._ph_gameState_lastUpdate && this.m_ph_hideState_startSoundBank != null && ZoneManagement.IsInZone(GTZone.bayou))
			{
				this.m_ph_hideState_startSoundBank.Play();
				if (!this._ph_isLocalPlayerSkeleton)
				{
					this.m_ph_soundNearBorder_audioSource.volume = 0f;
				}
			}
			for (int i = 0; i < GameMode.ParticipatingPlayers.Count; i++)
			{
				NetPlayer netPlayer = GameMode.ParticipatingPlayers[i];
				if (this.currentInfected.Contains(netPlayer))
				{
					this._SetPlayerBlindfoldVisibility(netPlayer, true);
				}
			}
			int num2 = this.m_ph_hideState_warnSoundBank_playCount - this._ph_hideState_warnSounds_timesPlayed;
			if (num2 > 0)
			{
				float num3 = this.m_ph_hideState_duration - (float)num2;
				if (this._ph_roundTime > num3 && ZoneManagement.IsInZone(GTZone.bayou))
				{
					if (this.m_ph_hideState_warnSoundBank != null)
					{
						this.m_ph_hideState_warnSoundBank.Play();
					}
					this._ph_hideState_warnSounds_timesPlayed++;
					return;
				}
			}
			break;
		}
		case GorillaPropHuntGameManager.EPropHuntGameState.Playing:
		{
			if (this._ph_gameState_lastUpdate != GorillaPropHuntGameManager.EPropHuntGameState.Playing)
			{
				this._ph_hideState_warnSounds_timesPlayed = 0;
				this._ph_playState_startLightning_strikeTimes_index = 0;
				if (this.m_ph_playState_startSoundBank != null && ZoneManagement.IsInZone(GTZone.bayou))
				{
					this.m_ph_playState_startSoundBank.Play();
				}
				for (int j = 0; j < GorillaPropHuntGameManager._g_ph_activePlayerRigs.Count; j++)
				{
					VRRig vrrig = GorillaPropHuntGameManager._g_ph_activePlayerRigs[j];
					this._SetPlayerBlindfoldVisibility(vrrig, vrrig.OwningNetPlayer, false);
				}
			}
			int num4 = this.m_ph_playState_startLightning_strikeTimes.Length;
			int num5 = math.min(this._ph_playState_startLightning_strikeTimes_index, num4 - 1);
			if (num5 < num4 && this._ph_playState_startLightning_manager_isResolved)
			{
				float num6 = this._ph_roundTime - this.m_ph_hideState_duration;
				if (this.m_ph_playState_startLightning_strikeTimes[num5] <= num6)
				{
					this._ph_playState_startLightning_strikeTimes_index++;
					this._ph_playState_startLightning_manager.DoLightningStrike();
				}
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x0005922C File Offset: 0x0005742C
	public override void UpdatePlayerAppearance(VRRig rig)
	{
		if (rig.zoneEntity.currentZone != GTZone.bayou || (rig.zoneEntity.currentZone == GTZone.bayou && rig.zoneEntity.currentSubZone == GTSubZone.entrance_tunnel))
		{
			return;
		}
		List<NetPlayer> participatingPlayers = GameMode.ParticipatingPlayers;
		bool flag = this._GetRigShouldBeSkeleton(rig, participatingPlayers);
		this._ph_isLocalPlayerSkeleton = (this._ph_isLocalPlayerParticipating && !base.IsInfected(NetworkSystem.Instance.LocalPlayer));
		GorillaBodyType gorillaBodyType = flag ? GorillaBodyType.Skeleton : GorillaBodyType.Default;
		int num = flag ? this._ph_gorillaGhostBodyMaterialIndex : 0;
		if (gorillaBodyType != rig.bodyRenderer.gameModeBodyType)
		{
			rig.bodyRenderer.SetGameModeBodyType(gorillaBodyType);
			if (rig.setMatIndex != num)
			{
				rig.ChangeMaterialLocal(num);
			}
		}
		if (PropHuntPools.IsReady)
		{
			bool flag2 = flag;
			if (rig.propHuntHandFollower.hasProp != flag2)
			{
				if (flag2)
				{
					rig.propHuntHandFollower.CreateProp();
				}
				else
				{
					rig.propHuntHandFollower.DestroyProp();
				}
			}
		}
		float signedDistToBoundary = this._UpdateBoundaryProximityState(rig, flag);
		bool flag3 = this._ShouldRigBeVisible(rig, flag, signedDistToBoundary);
		if (!rig.isOfflineVRRig)
		{
			rig.SetInvisibleToLocalPlayer(!flag3);
			if (flag || GorillaBodyRenderer.ForceSkeleton)
			{
				rig.bodyRenderer.SetSkeletonBodyActive(flag3);
			}
		}
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x00059353 File Offset: 0x00057553
	private bool _GetRigShouldBeSkeleton(VRRig rig, List<NetPlayer> participatingPlayers)
	{
		return rig.zoneEntity.currentZone == GTZone.bayou && participatingPlayers.Count >= 2 && participatingPlayers.Contains(rig.OwningNetPlayer) && !base.IsInfected(rig.Creator);
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x0005938C File Offset: 0x0005758C
	private bool _ShouldRigBeVisible(VRRig rig, bool shouldBeSkeleton, float signedDistToBoundary)
	{
		return this._ph_gameState != GorillaPropHuntGameManager.EPropHuntGameState.Hiding && (rig.isOfflineVRRig || !shouldBeSkeleton || signedDistToBoundary > 0f || this._ph_isLocalPlayerSkeleton);
	}

	// Token: 0x060010AA RID: 4266 RVA: 0x000593B4 File Offset: 0x000575B4
	private float _UpdateBoundaryProximityState(VRRig rig, bool isSkeleton)
	{
		float num = float.MinValue;
		float num2 = float.MinValue;
		if (isSkeleton)
		{
			PlayableBoundaryTracker playableBoundaryTracker;
			if (!GorillaPropHuntGameManager._g_ph_rig_to_propHuntZoneTrackers.TryGetValue(rig.GetInstanceID(), out playableBoundaryTracker))
			{
				rig.bodyTransform.GetOrAddComponent(out playableBoundaryTracker);
				GorillaPropHuntGameManager._g_ph_rig_to_propHuntZoneTrackers[rig.GetInstanceID()] = playableBoundaryTracker;
				if (this._ph_playBoundary_isResolved)
				{
					this._ph_playBoundary.tracked.AddIfNew(playableBoundaryTracker);
				}
			}
			num = playableBoundaryTracker.signedDistanceToBoundary;
			num2 = playableBoundaryTracker.prevSignedDistanceToBoundary;
			if (PhotonNetwork.IsMasterClient && !playableBoundaryTracker.IsInsideZone() && playableBoundaryTracker.timeSinceCrossingBorder > this.m_ph_playBoundary_timeLimit)
			{
				this.AddInfectedPlayer(rig.OwningNetPlayer, true);
			}
		}
		if (rig.isOfflineVRRig)
		{
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(isSkeleton);
			if (isSkeleton)
			{
				float time = 1f - math.saturate(-num / this.m_ph_soundNearBorder_maxDistance);
				AudioSource ph_soundNearBorder_audioSource = this.m_ph_soundNearBorder_audioSource;
				GorillaPropHuntGameManager.EPropHuntGameState ph_gameState = this._ph_gameState;
				ph_soundNearBorder_audioSource.volume = ((ph_gameState == GorillaPropHuntGameManager.EPropHuntGameState.Hiding || ph_gameState == GorillaPropHuntGameManager.EPropHuntGameState.Playing) ? (this.m_ph_soundNearBorder_baseVolume * this.m_ph_soundNearBorder_volumeCurve.Evaluate(time)) : 0f);
				if (num >= 0f && num2 < 0f && !this.m_ph_planeCrossingSoundBank.isPlaying)
				{
					this.m_ph_planeCrossingSoundBank.Play();
				}
				this._UpdateControllerHaptics(num);
			}
			else
			{
				this.m_ph_soundNearBorder_audioSource.volume = 0f;
			}
		}
		return num;
	}

	// Token: 0x060010AB RID: 4267 RVA: 0x00059500 File Offset: 0x00057700
	private void _UpdateControllerHaptics(float signedDistToBoundary)
	{
		if (Time.unscaledTime < GorillaPropHuntGameManager._g_ph_hapticsLastImpulseEndTime || math.abs(signedDistToBoundary) > this.m_ph_hapticsNearBorder_borderProximity)
		{
			return;
		}
		float time = 1f - math.saturate(-signedDistToBoundary / this.m_ph_hapticsNearBorder_borderProximity);
		float num = this.m_ph_hapticsNearBorder_ampCurve.Evaluate(time);
		float amplitude = math.saturate(this.m_ph_hapticsNearBorder_baseAmp * num * (GorillaTagger.Instance.tapHapticStrength * 2f));
		GorillaPropHuntGameManager._g_ph_hapticsLastImpulseEndTime = Time.unscaledTime + 0.1f;
		InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).SendHapticImpulse(0U, amplitude, 0.1f);
		InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0U, amplitude, 0.1f);
	}

	// Token: 0x060010AC RID: 4268 RVA: 0x000595A8 File Offset: 0x000577A8
	private void _Initialize_defaultStencilRefOfSkeletonMat()
	{
		if (GorillaPropHuntGameManager._g_ph_defaultStencilRefOfSkeletonMat == -1 && this._ph_gorillaGhostBodyMaterialIndex != -1)
		{
			Material[] materialsToChangeTo = VRRig.LocalRig.materialsToChangeTo;
			if (materialsToChangeTo != null && materialsToChangeTo.Length >= 1 && VRRig.LocalRig.materialsToChangeTo[0] != null)
			{
				GorillaPropHuntGameManager._g_ph_defaultStencilRefOfSkeletonMat = (int)VRRig.LocalRig.materialsToChangeTo[this._ph_gorillaGhostBodyMaterialIndex].GetFloat(ShaderProps._StencilReference);
				return;
			}
		}
		else
		{
			GorillaPropHuntGameManager._g_ph_defaultStencilRefOfSkeletonMat = 7;
		}
	}

	// Token: 0x060010AD RID: 4269 RVA: 0x00059618 File Offset: 0x00057818
	private void _Initialize_gorillaGhostBodyMaterialIndex()
	{
		this._ph_gorillaGhostBodyMaterialIndex = -1;
		Material[] materialsToChangeTo = VRRig.LocalRig.materialsToChangeTo;
		for (int i = 0; i < materialsToChangeTo.Length; i++)
		{
			if (materialsToChangeTo[i].name.StartsWith(this.m_ph_gorillaGhostBodyMaterial.name))
			{
				this._ph_gorillaGhostBodyMaterialIndex = i;
				break;
			}
		}
		if (this._ph_gorillaGhostBodyMaterialIndex == -1)
		{
			this._ph_gorillaGhostBodyMaterialIndex = 15;
		}
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x0005967C File Offset: 0x0005787C
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		GorillaPropHuntGameManager.EPropHuntGameState ph_gameState = this._ph_gameState;
		if ((ph_gameState != GorillaPropHuntGameManager.EPropHuntGameState.Playing && ph_gameState != GorillaPropHuntGameManager.EPropHuntGameState.Hiding) || !GameMode.ParticipatingPlayers.Contains(forPlayer) || base.IsInfected(forPlayer))
		{
			return 0;
		}
		return this._ph_gorillaGhostBodyMaterialIndex;
	}

	// Token: 0x060010AF RID: 4271 RVA: 0x000596BC File Offset: 0x000578BC
	protected override void InfectionRoundEnd()
	{
		base.InfectionRoundEnd();
		this.InfectionRoundEndCheck();
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x000596CA File Offset: 0x000578CA
	private void InfectionRoundEndCheck()
	{
		this._roundIsPlaying = false;
		if (PhotonNetwork.IsMasterClient)
		{
			this.PH_OnRoundEnd();
		}
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x000596E0 File Offset: 0x000578E0
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return this._ph_gameState == GorillaPropHuntGameManager.EPropHuntGameState.Playing && base.LocalCanTag(myPlayer, otherPlayer);
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x000596F5 File Offset: 0x000578F5
	public override bool LocalIsTagged(NetPlayer player)
	{
		return this._ph_gameState == GorillaPropHuntGameManager.EPropHuntGameState.Playing && base.LocalIsTagged(player);
	}

	// Token: 0x060010B3 RID: 4275 RVA: 0x0005970C File Offset: 0x0005790C
	private void _ResetRigAppearance(VRRig rig)
	{
		rig.bodyRenderer.SetSkeletonBodyActive(true);
		rig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Default);
		this._SetPlayerBlindfoldVisibility(rig, rig.OwningNetPlayer, false);
		rig.ChangeMaterialLocal(0);
		rig.SetInvisibleToLocalPlayer(false);
		if (rig == VRRig.LocalRig)
		{
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
		}
		for (int i = 0; i < GorillaPropHuntGameManager._g_ph_allHandFollowers.Count; i++)
		{
			PropHuntHandFollower propHuntHandFollower = GorillaPropHuntGameManager._g_ph_allHandFollowers[i];
			if (propHuntHandFollower.attachedToRig == rig && propHuntHandFollower.hasProp)
			{
				propHuntHandFollower.DestroyProp();
			}
		}
	}

	// Token: 0x060010B4 RID: 4276 RVA: 0x000597A4 File Offset: 0x000579A4
	protected override void InfectionRoundStart()
	{
		base.InfectionRoundStart();
		this.InfectionRoundStartCheck();
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x000597B2 File Offset: 0x000579B2
	private void InfectionRoundStartCheck()
	{
		this._roundIsPlaying = true;
		if (PhotonNetwork.IsMasterClient)
		{
			this._ph_randomSeed = UnityEngine.Random.Range(1, int.MaxValue);
			this.PH_OnRoundStartRPC(GTTime.TimeAsMilliseconds(), this._ph_randomSeed);
		}
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x000597E4 File Offset: 0x000579E4
	public override void AddInfectedPlayer(NetPlayer infectedPlayer, bool withTagStop = true)
	{
		base.AddInfectedPlayer(infectedPlayer, withTagStop);
		if (infectedPlayer.IsLocal)
		{
			this.m_ph_playState_taggedSoundBank.Play();
		}
	}

	// Token: 0x060010B7 RID: 4279 RVA: 0x00059804 File Offset: 0x00057A04
	private void _ResolveXSceneRefs()
	{
		if (!this._isListeningForXSceneRefLoadCallbacks)
		{
			this.m_ph_playBoundary_xSceneRef.AddCallbackOnLoad(new Action(this._OnXSceneRefLoaded_PlayBoundary));
			this.m_ph_playBoundary_xSceneRef.AddCallbackOnUnload(new Action(this._OnXSceneRefUnloaded_PlayBoundary));
			this.m_ph_playState_startLightning_manager_ref.AddCallbackOnLoad(new Action(this._OnXSceneRefLoaded_LightningManager));
			this.m_ph_playState_startLightning_manager_ref.AddCallbackOnUnload(new Action(this._OnXSceneRefUnloaded_LightningManager));
		}
		this._OnXSceneRefLoaded_PlayBoundary();
		if (VRRig.LocalRig.zoneEntity.currentZone == GTZone.bayou)
		{
			this._OnXSceneRefLoaded_LightningManager();
		}
	}

	// Token: 0x060010B8 RID: 4280 RVA: 0x00059894 File Offset: 0x00057A94
	private void _OnXSceneRefLoaded_PlayBoundary()
	{
		if (!this._ph_playBoundary_isResolved)
		{
			this._ph_playBoundary_isResolved = (this.m_ph_playBoundary_xSceneRef.TryResolve<PlayableBoundaryManager>(out this._ph_playBoundary) && this._ph_playBoundary != null);
			if (this._ph_playBoundary_isResolved)
			{
				PlayableBoundaryManager ph_playBoundary = this._ph_playBoundary;
				if (ph_playBoundary.tracked == null)
				{
					ph_playBoundary.tracked = new List<PlayableBoundaryTracker>(10);
				}
				this._ph_playBoundary.tracked.Clear();
				if (!this._ph_playBoundary_initialPosition_isInitialized)
				{
					this._ph_playBoundary_initialPosition_isInitialized = true;
					this._ph_playBoundary_initialPosition = this._ph_playBoundary.transform.position;
					this._ph_playBoundary_hasTargetPositionForRound = false;
				}
			}
		}
	}

	// Token: 0x060010B9 RID: 4281 RVA: 0x00059934 File Offset: 0x00057B34
	private void _OnXSceneRefUnloaded_PlayBoundary()
	{
		this._ph_playBoundary_isResolved = false;
		this._ph_playBoundary = null;
		this._ph_playBoundary_hasTargetPositionForRound = false;
	}

	// Token: 0x060010BA RID: 4282 RVA: 0x0005994B File Offset: 0x00057B4B
	private void _OnXSceneRefLoaded_LightningManager()
	{
		this._ph_playState_startLightning_manager_isResolved = (this.m_ph_playState_startLightning_manager_ref.TryResolve<LightningManager>(out this._ph_playState_startLightning_manager) && this._ph_playState_startLightning_manager != null);
	}

	// Token: 0x060010BB RID: 4283 RVA: 0x00059975 File Offset: 0x00057B75
	private void _OnXSceneRefUnloaded_LightningManager()
	{
		this._ph_playState_startLightning_manager_isResolved = false;
		this._ph_playState_startLightning_manager = null;
	}

	// Token: 0x060010BC RID: 4284 RVA: 0x00059988 File Offset: 0x00057B88
	public void PH_OnRoundEnd()
	{
		VRRigCache.Instance.GetActiveRigs(GorillaPropHuntGameManager._g_ph_activePlayerRigs);
		for (int i = 0; i < GorillaPropHuntGameManager._g_ph_activePlayerRigs.Count; i++)
		{
			this._ResetRigAppearance(GorillaPropHuntGameManager._g_ph_activePlayerRigs[i]);
		}
		CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
		EquipmentInteractor.instance.ForceDropAnyEquipment();
		if (LckSocialCameraManager.Instance != null)
		{
			LckSocialCameraManager.Instance.SetForceHidden(false);
		}
		this._ph_timeRoundStartedMillis = -1000L;
		if (this.m_ph_soundNearBorder_audioSource != null)
		{
			this.m_ph_soundNearBorder_audioSource.volume = 0f;
		}
		if (this._ph_playBoundary_isResolved && this._ph_playBoundary_initialPosition_isInitialized)
		{
			this._ph_playBoundary.transform.position = this._ph_playBoundary_initialPosition;
		}
		this._ph_playBoundary_hasTargetPositionForRound = false;
	}

	// Token: 0x060010BD RID: 4285 RVA: 0x00059A52 File Offset: 0x00057C52
	public void PH_OnRoundStartRPC(long timeRoundStartedMillis, int seed)
	{
		this._ph_isLocalPlayerParticipating = GameMode.ParticipatingPlayers.Contains(VRRig.LocalRig.OwningNetPlayer);
		this._ph_timeRoundStartedMillis = timeRoundStartedMillis;
		this._ph_randomSeed = seed;
		this._PH_OnRoundStart();
	}

	// Token: 0x060010BE RID: 4286 RVA: 0x00059A84 File Offset: 0x00057C84
	private void _PH_OnRoundStart()
	{
		if (this._ph_playBoundary_isResolved)
		{
			SRand srand = new SRand(this._ph_randomSeed);
			int index = srand.NextInt(this.m_ph_playBoundary_endPointTransforms.Count);
			Transform transform = this.m_ph_playBoundary_endPointTransforms[index];
			if (transform != null)
			{
				this._ph_playBoundary_currentTargetPosition = transform.position;
				this._ph_playBoundary_hasTargetPositionForRound = true;
				this._ph_playBoundary.transform.position = this._ph_playBoundary_initialPosition;
			}
		}
		else if (this._ph_playBoundary_isResolved && this._ph_playBoundary_initialPosition_isInitialized)
		{
			this._ph_playBoundary.transform.position = this._ph_playBoundary_initialPosition;
		}
		if (PropHuntPools.IsReady)
		{
			this.SpawnProps();
		}
		else if (!this._isListeningTo_Pools_OnReady)
		{
			PropHuntPools.OnReady = (Action)Delegate.Combine(PropHuntPools.OnReady, new Action(this._Pools_OnReady));
		}
		if (this._ph_isLocalPlayerParticipating)
		{
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
			if (LckSocialCameraManager.Instance != null)
			{
				LckSocialCameraManager.Instance.SetForceHidden(true);
			}
		}
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x00059B84 File Offset: 0x00057D84
	private void _Pools_OnReady()
	{
		if (PhotonNetwork.IsMasterClient || this._ph_isLocalPlayerParticipating)
		{
			this.SpawnProps();
		}
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x00059B9B File Offset: 0x00057D9B
	public static void RegisterPropZone(PropHuntPropZone propZone)
	{
		GorillaPropHuntGameManager._g_ph_allPropZones.Add(propZone);
		if (GorillaPropHuntGameManager.instance != null && PropHuntPools.IsReady)
		{
			propZone.OnRoundStart();
		}
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x00059BBC File Offset: 0x00057DBC
	public static void UnregisterPropZone(PropHuntPropZone propZone)
	{
		GorillaPropHuntGameManager._g_ph_allPropZones.Remove(propZone);
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x00059BCA File Offset: 0x00057DCA
	public static void RegisterPropHandFollower(PropHuntHandFollower hand)
	{
		GorillaPropHuntGameManager._g_ph_allHandFollowers.Add(hand);
		if (GorillaPropHuntGameManager.instance != null)
		{
			hand.OnRoundStart();
		}
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x00059BE4 File Offset: 0x00057DE4
	public static void UnregisterPropHandFollower(PropHuntHandFollower hand)
	{
		GorillaPropHuntGameManager._g_ph_allHandFollowers.Remove(hand);
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x00059BF4 File Offset: 0x00057DF4
	public void SpawnProps()
	{
		if (!PropHuntPools.IsReady)
		{
			if (!this._isListeningTo_Pools_OnReady)
			{
				PropHuntPools.OnReady = (Action)Delegate.Combine(PropHuntPools.OnReady, new Action(this._Pools_OnReady));
			}
			return;
		}
		foreach (PropHuntPropZone propHuntPropZone in GorillaPropHuntGameManager._g_ph_allPropZones)
		{
			propHuntPropZone.OnRoundStart();
		}
		foreach (PropHuntHandFollower propHuntHandFollower in GorillaPropHuntGameManager._g_ph_allHandFollowers)
		{
			if (GameMode.ParticipatingPlayers.Contains(propHuntHandFollower.attachedToRig.OwningNetPlayer))
			{
				propHuntHandFollower.OnRoundStart();
			}
		}
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x00059CCC File Offset: 0x00057ECC
	public string GetCosmeticId(uint randomUInt)
	{
		if (PropHuntPools.AllPropCosmeticIds == null)
		{
			return this.m_ph_fallbackPropCosmeticSO.info.playFabID;
		}
		return PropHuntPools.AllPropCosmeticIds[(int)(checked((IntPtr)(unchecked((ulong)randomUInt % (ulong)((long)PropHuntPools.AllPropCosmeticIds.Length)))))];
	}

	// Token: 0x060010C6 RID: 4294 RVA: 0x00059CF8 File Offset: 0x00057EF8
	public GTAssetRef<GameObject> GetPropRef_NoPool(uint randomUInt, out CosmeticSO out_debugCosmeticSO)
	{
		if (this.AllPropIDs_NoPool == null)
		{
			out_debugCosmeticSO = this.m_ph_fallbackPropCosmeticSO;
			return this.m_ph_fallbackPropCosmeticSO.info.wardrobeParts[0].prefabAssetRef;
		}
		string cosmeticID = this.AllPropIDs_NoPool[(int)(checked((IntPtr)(unchecked((ulong)randomUInt % (ulong)((long)this.AllPropIDs_NoPool.Length)))))];
		return this.GetPropRefByCosmeticID_NoPool(cosmeticID, out out_debugCosmeticSO);
	}

	// Token: 0x060010C7 RID: 4295 RVA: 0x00059D50 File Offset: 0x00057F50
	public GTAssetRef<GameObject> GetPropRefByCosmeticID_NoPool(string cosmeticID, out CosmeticSO out_debugCosmeticSO)
	{
		CosmeticSO cosmeticSO = this.m_ph_allCosmetics.SearchForCosmeticSO(cosmeticID);
		if (cosmeticSO == null)
		{
			GTDev.LogError<string>("ERROR!!!  GorillaPropHuntGameManager.GetPropRefByCosmeticID_NoPool: Got cosmetic id from title data, but could not find \"" + cosmeticID + "\".", null);
			out_debugCosmeticSO = this.m_ph_fallbackPropCosmeticSO;
			return this.m_ph_fallbackPropCosmeticSO.info.wardrobeParts[0].prefabAssetRef;
		}
		if (cosmeticSO.info.wardrobeParts.Length == 0)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Invalid prop ",
				cosmeticID,
				" ",
				cosmeticSO.info.displayName,
				" has no wardrobeParts"
			}));
			out_debugCosmeticSO = this.m_ph_fallbackPropCosmeticSO;
			return this.m_ph_fallbackPropCosmeticSO.info.wardrobeParts[0].prefabAssetRef;
		}
		out_debugCosmeticSO = cosmeticSO;
		return cosmeticSO.info.wardrobeParts[0].prefabAssetRef;
	}

	// Token: 0x060010C8 RID: 4296 RVA: 0x00059E34 File Offset: 0x00058034
	private void _SetPlayerBlindfoldVisibility(NetPlayer netPlayer, bool shouldEnable)
	{
		VRRig vrrig = this.FindPlayerVRRig(netPlayer);
		if (vrrig == null && netPlayer.InRoom)
		{
			return;
		}
		this._SetPlayerBlindfoldVisibility(vrrig, netPlayer, shouldEnable);
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x00059E64 File Offset: 0x00058064
	private void _SetPlayerBlindfoldVisibility(VRRig vrRig, NetPlayer netPlayer, bool shouldEnable)
	{
		if (netPlayer == VRRig.LocalRig.OwningNetPlayer)
		{
			if (!this._ph_blindfold_forCamera_isInitialized)
			{
				this._InitializeBlindfoldForCamera();
			}
			if (this._ph_blindfold_forCamera_isInitialized)
			{
				this._ph_blindfold_forCamera_1p.SetActive(shouldEnable);
				this._ph_blindfold_forCamera_3p.SetActive(shouldEnable);
				return;
			}
		}
		else
		{
			GameObject gameObject;
			if (!this._ph_vrRig_to_blindfolds.TryGetValue(vrRig.GetInstanceID(), out gameObject))
			{
				Transform[] boneXforms;
				string text;
				if (!GTHardCodedBones.TryGetBoneXforms(vrRig, out boneXforms, out text))
				{
					return;
				}
				Transform parent;
				if (!GTHardCodedBones.TryGetBoneXform(boneXforms, GTHardCodedBones.EBone.head, out parent))
				{
					return;
				}
				if (this.m_ph_blindfold_forAvatarPrefab == null)
				{
					return;
				}
				gameObject = Object.Instantiate<GameObject>(this.m_ph_blindfold_forAvatarPrefab, parent);
				this._ph_vrRig_to_blindfolds[vrRig.GetInstanceID()] = gameObject;
			}
			gameObject.SetActive(shouldEnable);
		}
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x00059F10 File Offset: 0x00058110
	private void _InitializeBlindfoldForCamera()
	{
		if (GorillaTagger.Instance == null)
		{
			return;
		}
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		if (mainCamera == null)
		{
			return;
		}
		if (this.m_ph_blindfold_forCameraPrefab == null)
		{
			return;
		}
		this._ph_blindfold_forCamera_1p = Object.Instantiate<GameObject>(this.m_ph_blindfold_forCameraPrefab, mainCamera.transform);
		Camera camera = null;
		if (GorillaTagger.Instance.thirdPersonCamera != null)
		{
			camera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>(true);
		}
		if (camera == null)
		{
			return;
		}
		this._ph_blindfold_forCamera_3p = Object.Instantiate<GameObject>(this.m_ph_blindfold_forCameraPrefab, camera.transform);
		this._ph_blindfold_forCamera_isInitialized = (this._ph_blindfold_forCamera_1p != null);
	}

	// Token: 0x060010CB RID: 4299 RVA: 0x00059FC0 File Offset: 0x000581C0
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeRead(stream, info);
		this._ph_randomSeed = (int)stream.ReceiveNext();
		long ph_timeRoundStartedMillis = this._ph_timeRoundStartedMillis;
		this._ph_timeRoundStartedMillis = (long)stream.ReceiveNext();
		if (ph_timeRoundStartedMillis != this._ph_timeRoundStartedMillis)
		{
			if (this._ph_timeRoundStartedMillis > 0L)
			{
				this._PH_OnRoundStart();
				return;
			}
			this.PH_OnRoundEnd();
		}
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x0005A01C File Offset: 0x0005821C
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeWrite(stream, info);
		stream.SendNext(this._ph_randomSeed);
		stream.SendNext(this._ph_timeRoundStartedMillis);
	}

	// Token: 0x040013C1 RID: 5057
	private const string preLog = "GorillaPropHuntGameManager: ";

	// Token: 0x040013C2 RID: 5058
	private const string preLogEd = "(editor only log) GorillaPropHuntGameManager: ";

	// Token: 0x040013C3 RID: 5059
	private const string preLogBeta = "(beta only log) GorillaPropHuntGameManager: ";

	// Token: 0x040013C4 RID: 5060
	private const string preErr = "ERROR!!!  GorillaPropHuntGameManager: ";

	// Token: 0x040013C5 RID: 5061
	private const string preErrEd = "ERROR!!!  (editor only log) GorillaPropHuntGameManager: ";

	// Token: 0x040013C6 RID: 5062
	private const string preErrBeta = "ERROR!!!  (beta only log) GorillaPropHuntGameManager: ";

	// Token: 0x040013C7 RID: 5063
	private const bool _k__GT_PROP_HUNT__USE_POOLING__ = true;

	// Token: 0x040013C9 RID: 5065
	[FormerlySerializedAs("allCosmetics")]
	[SerializeField]
	private AllCosmeticsArraySO m_ph_allCosmetics;

	// Token: 0x040013CA RID: 5066
	[FormerlySerializedAs("backupCosmetic")]
	[FormerlySerializedAs("m_ph_backupCosmetic")]
	[SerializeField]
	private CosmeticSO m_ph_fallbackPropCosmeticSO;

	// Token: 0x040013CB RID: 5067
	[Tooltip("This us used by PropHuntPools as the parent gameobject that the cosmetic prefab instance will be parented to.")]
	[FormerlySerializedAs("m_ph_propPlacementPrefab")]
	[SerializeField]
	private PropPlacementRB m_ph_propDecoyPrefab;

	// Token: 0x040013CC RID: 5068
	[Tooltip("The time that players have to hide before their props can be seen by the tagger monke.")]
	[FormerlySerializedAs("m_propHunt_hideState_duration")]
	[SerializeField]
	private float m_ph_hideState_duration = 10f;

	// Token: 0x040013CD RID: 5069
	[Tooltip("Prefab that will be parented to the camera if the current player is not a ghost during hiding state.")]
	[FormerlySerializedAs("m_propHunt_blindfold_1stPersonPrefab")]
	[SerializeField]
	private GameObject m_ph_blindfold_forCameraPrefab;

	// Token: 0x040013CE RID: 5070
	private GameObject _ph_blindfold_forCamera_1p;

	// Token: 0x040013CF RID: 5071
	private GameObject _ph_blindfold_forCamera_3p;

	// Token: 0x040013D0 RID: 5072
	private bool _ph_blindfold_forCamera_isInitialized;

	// Token: 0x040013D1 RID: 5073
	[Tooltip("Prefab to cover the eyes of the non-ghost gorilla's avatar during the hiding state.")]
	[FormerlySerializedAs("m_propHunt_blindfold_3rdPersonPrefab")]
	[SerializeField]
	private GameObject m_ph_blindfold_forAvatarPrefab;

	// Token: 0x040013D2 RID: 5074
	private readonly Dictionary<int, GameObject> _ph_vrRig_to_blindfolds = new Dictionary<int, GameObject>(20);

	// Token: 0x040013D3 RID: 5075
	[Tooltip("A randomly picked sound in this soundbank will be played when the hide state starts.")]
	[FormerlySerializedAs("m_propHunt_hideState_startSoundBank")]
	[SerializeField]
	private SoundBankPlayer m_ph_hideState_startSoundBank;

	// Token: 0x040013D4 RID: 5076
	[FormerlySerializedAs("m_propHunt_hideState_warnSoundBank")]
	[Tooltip("A randomly picked Sound in this Sound Bank will be played to warn players that the hiding period is ending.")]
	[FormerlySerializedAs("m_propHunt_hideState_startSoundBank")]
	[SerializeField]
	private SoundBankPlayer m_ph_hideState_warnSoundBank;

	// Token: 0x040013D5 RID: 5077
	[FormerlySerializedAs("m_propHunt_hideState_warnSoundBank_playCount")]
	[Tooltip("How many times should the warning sound play before the hiding period ends? Will play every 1 second.")]
	[SerializeField]
	private int m_ph_hideState_warnSoundBank_playCount = 3;

	// Token: 0x040013D6 RID: 5078
	private int _ph_hideState_warnSounds_timesPlayed;

	// Token: 0x040013D7 RID: 5079
	[FormerlySerializedAs("m_propHunt_playState_startSoundBank")]
	[Tooltip("A randomly picked sound in this Sound Bank will be played when the hiding state ends and the playing state has started.")]
	[SerializeField]
	private SoundBankPlayer m_ph_playState_startSoundBank;

	// Token: 0x040013D8 RID: 5080
	[FormerlySerializedAs("m_propHunt_playState_startLightning_manager_ref")]
	[Tooltip("Lightning manager for doing lightning strike strikes when playing starts.")]
	[SerializeField]
	private XSceneRef m_ph_playState_startLightning_manager_ref;

	// Token: 0x040013D9 RID: 5081
	private LightningManager _ph_playState_startLightning_manager;

	// Token: 0x040013DA RID: 5082
	private bool _ph_playState_startLightning_manager_isResolved;

	// Token: 0x040013DB RID: 5083
	[Tooltip("How long after the playing starts should the lightning strikes happen?")]
	private float[] m_ph_playState_startLightning_strikeTimes = new float[]
	{
		1f,
		1.5f,
		1.8f
	};

	// Token: 0x040013DC RID: 5084
	private int _ph_playState_startLightning_strikeTimes_index;

	// Token: 0x040013DD RID: 5085
	[Tooltip("A randomly picked sound in this Sound Bank will be played when the ghost is tagged by the hunter.")]
	[SerializeField]
	private SoundBankPlayer m_ph_playState_taggedSoundBank;

	// Token: 0x040013DE RID: 5086
	[Tooltip("Maximum distance prop can be from the center of the player's hand")]
	[SerializeField]
	private float m_ph_hand_follow_distance = 0.35f;

	// Token: 0x040013DF RID: 5087
	[FormerlySerializedAs("_playBoundary_xSceneRef")]
	[FormerlySerializedAs("_playZone_xSceneRef")]
	[SerializeField]
	private XSceneRef m_ph_playBoundary_xSceneRef;

	// Token: 0x040013E0 RID: 5088
	[Tooltip("A list of Transforms representing potential end positions for the playable boundary each round.")]
	[SerializeField]
	private List<Transform> m_ph_playBoundary_endPointTransforms = new List<Transform>();

	// Token: 0x040013E1 RID: 5089
	private PlayableBoundaryManager _ph_playBoundary;

	// Token: 0x040013E2 RID: 5090
	private bool _ph_playBoundary_isResolved;

	// Token: 0x040013E3 RID: 5091
	private Vector3 _ph_playBoundary_initialPosition;

	// Token: 0x040013E4 RID: 5092
	private bool _ph_playBoundary_initialPosition_isInitialized;

	// Token: 0x040013E5 RID: 5093
	private Vector3 _ph_playBoundary_currentTargetPosition;

	// Token: 0x040013E6 RID: 5094
	private bool _ph_playBoundary_hasTargetPositionForRound;

	// Token: 0x040013E7 RID: 5095
	[Tooltip("The maximum time a player can be outside of the boundary before being tagged.")]
	[SerializeField]
	private float m_ph_playBoundary_timeLimit = 15f;

	// Token: 0x040013E8 RID: 5096
	[Tooltip("On the What does 1.0 on the X axis")]
	[FormerlySerializedAs("_playBoundary_radiusScaleOverRoundTime_maxTime")]
	[SerializeField]
	private float m_ph_playBoundary_radiusScaleOverRoundTime_maxTime = 180f;

	// Token: 0x040013E9 RID: 5097
	[FormerlySerializedAs("_playBoundary_radiusScaleOverRoundTime_curve")]
	[FormerlySerializedAs("_playZoneRadiusOverRoundTime")]
	[SerializeField]
	private AnimationCurve m_ph_playBoundary_radiusScaleOverRoundTime_curve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f, 1f, 1f, 0f, 0f),
		new Keyframe(0.9f, 0.01f, 1f, 0f, 0f, 0f),
		new Keyframe(1f, 0.01f, 1f, 0f, 0f, 0f)
	});

	// Token: 0x040013EA RID: 5098
	[FormerlySerializedAs("_ph_gorillaGhostBodyMaterial")]
	[FormerlySerializedAs("gorillaGhostBodyMaterial")]
	[SerializeField]
	private Material m_ph_gorillaGhostBodyMaterial;

	// Token: 0x040013EB RID: 5099
	private int _ph_gorillaGhostBodyMaterialIndex = -1;

	// Token: 0x040013EC RID: 5100
	[Tooltip("A randomly picked sound in this Sound Bank will be played when the spectral plane border is crossed.")]
	[SerializeField]
	private SoundBankPlayer m_ph_planeCrossingSoundBank;

	// Token: 0x040013ED RID: 5101
	[Tooltip("This AudioSource will only be heard by the local player and is non directional.")]
	[FormerlySerializedAs("m_soundNearBorder_audioSource")]
	[FormerlySerializedAs("soundNearBorderAudioSource")]
	[FormerlySerializedAs("soundNearBoundaryAudioSource")]
	[SerializeField]
	private AudioSource m_ph_soundNearBorder_audioSource;

	// Token: 0x040013EE RID: 5102
	[FormerlySerializedAs("m_soundNearBorder_maxDistance")]
	[FormerlySerializedAs("soundNearBorderMaxDistance")]
	[FormerlySerializedAs("soundNearBoundaryMaxDistance")]
	[SerializeField]
	private float m_ph_soundNearBorder_maxDistance = 2f;

	// Token: 0x040013EF RID: 5103
	[FormerlySerializedAs("m_soundNearBorder_volumeCurve")]
	[FormerlySerializedAs("soundNearBorderVolumeCurve")]
	[FormerlySerializedAs("soundNearBoundaryVolumeCurve")]
	[SerializeField]
	private AnimationCurve m_ph_soundNearBorder_volumeCurve = AnimationCurves.Linear;

	// Token: 0x040013F0 RID: 5104
	[Tooltip("The resulting volume curve value is multiplied by this.")]
	[FormerlySerializedAs("m_soundNearBorder_baseVolume")]
	[SerializeField]
	private float m_ph_soundNearBorder_baseVolume = 0.5f;

	// Token: 0x040013F1 RID: 5105
	[FormerlySerializedAs("m_hapticsNearBorder_borderProximity")]
	[SerializeField]
	private float m_ph_hapticsNearBorder_borderProximity = 2f;

	// Token: 0x040013F2 RID: 5106
	[FormerlySerializedAs("m_hapticsNearBorder_ampCurve")]
	[SerializeField]
	private AnimationCurve m_ph_hapticsNearBorder_ampCurve = AnimationCurves.Linear;

	// Token: 0x040013F3 RID: 5107
	[FormerlySerializedAs("m_hapticsNearBorder_baseAmp")]
	[SerializeField]
	private float m_ph_hapticsNearBorder_baseAmp = 1f;

	// Token: 0x040013F4 RID: 5108
	private bool _ph_isLocalPlayerSkeleton;

	// Token: 0x040013F5 RID: 5109
	[OnEnterPlay_Clear]
	private static readonly Dictionary<int, PlayableBoundaryTracker> _g_ph_rig_to_propHuntZoneTrackers = new Dictionary<int, PlayableBoundaryTracker>(10);

	// Token: 0x040013F6 RID: 5110
	[OnEnterPlay_Set(0f)]
	private static float _g_ph_hapticsLastImpulseEndTime;

	// Token: 0x040013F7 RID: 5111
	[OnEnterPlay_Clear]
	private static readonly List<VRRig> _g_ph_activePlayerRigs = new List<VRRig>(20);

	// Token: 0x040013F8 RID: 5112
	[OnEnterPlay_Clear]
	private static readonly List<PropHuntPropZone> _g_ph_allPropZones = new List<PropHuntPropZone>();

	// Token: 0x040013F9 RID: 5113
	[OnEnterPlay_Clear]
	private static readonly List<PropHuntHandFollower> _g_ph_allHandFollowers = new List<PropHuntHandFollower>();

	// Token: 0x040013FA RID: 5114
	private static readonly string[] _g_ph_titleDataSeparators = new string[]
	{
		"\"",
		" ",
		"\\n"
	};

	// Token: 0x040013FB RID: 5115
	[OnEnterPlay_Set(-1)]
	private static int _g_ph_defaultStencilRefOfSkeletonMat = -1;

	// Token: 0x040013FC RID: 5116
	[DebugReadout]
	private GorillaPropHuntGameManager.EPropHuntGameState _ph_gameState;

	// Token: 0x040013FD RID: 5117
	private GorillaPropHuntGameManager.EPropHuntGameState _ph_gameState_lastUpdate;

	// Token: 0x040013FE RID: 5118
	private bool _roundIsPlaying;

	// Token: 0x040013FF RID: 5119
	private string[] _ph_allPropIDs_noPool;

	// Token: 0x04001400 RID: 5120
	[DebugReadout]
	private float _ph_roundTime;

	// Token: 0x04001401 RID: 5121
	private long __ph_timeRoundStartedMillis__;

	// Token: 0x04001402 RID: 5122
	private int _ph_randomSeed;

	// Token: 0x04001403 RID: 5123
	private bool _ph_isLocalPlayerParticipating;

	// Token: 0x04001404 RID: 5124
	private bool _isListeningTo_Pools_OnReady;

	// Token: 0x04001405 RID: 5125
	private bool _isListeningForXSceneRefLoadCallbacks;

	// Token: 0x0200026D RID: 621
	private enum EPropHuntGameState
	{
		// Token: 0x04001407 RID: 5127
		Invalid,
		// Token: 0x04001408 RID: 5128
		StoppedGameMode,
		// Token: 0x04001409 RID: 5129
		StartingGameMode,
		// Token: 0x0400140A RID: 5130
		WaitingForMorePlayers,
		// Token: 0x0400140B RID: 5131
		WaitingForRoundToStart,
		// Token: 0x0400140C RID: 5132
		Hiding,
		// Token: 0x0400140D RID: 5133
		Playing
	}
}
