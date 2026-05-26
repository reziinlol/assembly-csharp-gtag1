using System;
using GorillaGameModes;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200017F RID: 383
public sealed class SuperInfectionGame : GorillaTagManager
{
	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06000A2D RID: 2605 RVA: 0x00036D98 File Offset: 0x00034F98
	// (set) Token: 0x06000A2E RID: 2606 RVA: 0x00036D9F File Offset: 0x00034F9F
	public new static SuperInfectionGame instance { get; private set; }

	// Token: 0x06000A2F RID: 2607 RVA: 0x00036DA7 File Offset: 0x00034FA7
	public override GameModeType GameType()
	{
		return GameModeType.SuperInfect;
	}

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x06000A30 RID: 2608 RVA: 0x00036DAB File Offset: 0x00034FAB
	// (set) Token: 0x06000A31 RID: 2609 RVA: 0x00036DB3 File Offset: 0x00034FB3
	[DebugReadout]
	public ESuperInfectionGameState gameState { get; private set; }

	// Token: 0x06000A32 RID: 2610 RVA: 0x00036DBC File Offset: 0x00034FBC
	public override void Awake()
	{
		SuperInfectionGame.instance = this;
		this.gameState = ESuperInfectionGameState.Stopped;
		base.Awake();
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x00036DD1 File Offset: 0x00034FD1
	public override void OnEnable()
	{
		base.OnEnable();
		SIProgression instance = SIProgression.Instance;
		if (instance == null)
		{
			return;
		}
		instance.ResetTelemetryIntervalData();
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x00036DE8 File Offset: 0x00034FE8
	public override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x00036DF0 File Offset: 0x00034FF0
	public override void Tick()
	{
		base.Tick();
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x00036DF8 File Offset: 0x00034FF8
	public override void StartPlaying()
	{
		this.gameState = ESuperInfectionGameState.Starting;
		base.StartPlaying();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			SIProgression.Instance.AddRoundTelemetry();
		}
		VRRig.LocalRig.EnableSuperInfectionHands(true);
		for (int i = 0; i < this.currentNetPlayerArray.Length; i++)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.currentNetPlayerArray[i], out rigContainer))
			{
				rigContainer.Rig.EnableSuperInfectionHands(true);
			}
		}
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x00036E68 File Offset: 0x00035068
	public override void StopPlaying()
	{
		base.StopPlaying();
		this.gameState = ESuperInfectionGameState.Stopped;
		VRRig.LocalRig.EnableSuperInfectionHands(false);
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x00036E84 File Offset: 0x00035084
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(newPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableSuperInfectionHands(true);
		}
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x00036EB3 File Offset: 0x000350B3
	public override string GameModeName()
	{
		return "SUPER INFECTION";
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x00036EBC File Offset: 0x000350BC
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_SUPER_INFECTION_ROOM_LABEL", out result, "(SUPER INFECTION GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_SUPER_INFECTION_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x00036EE7 File Offset: 0x000350E7
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x00036EEF File Offset: 0x000350EF
	protected override void InfectionRoundStart()
	{
		base.InfectionRoundStart();
		this.gameState = ESuperInfectionGameState.Playing;
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x00036EFE File Offset: 0x000350FE
	protected override void InfectionRoundEnd()
	{
		base.InfectionRoundEnd();
		this.gameState = ESuperInfectionGameState.RoundRestarting;
		SuperInfectionManager.activeSuperInfectionManager.zoneSuperInfection.ResetPerRoundResources();
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x00036F1C File Offset: 0x0003511C
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return base.LocalCanTag(myPlayer, otherPlayer);
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x00036F26 File Offset: 0x00035126
	public override void UpdatePlayerAppearance(VRRig rig)
	{
		base.UpdatePlayerAppearance(rig);
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x00036F2F File Offset: 0x0003512F
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		return base.MyMatIndex(forPlayer);
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x00036F38 File Offset: 0x00035138
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeWrite(stream, info);
		stream.SendNext(this.gameState);
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x00036F54 File Offset: 0x00035154
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeRead(stream, info);
		ESuperInfectionGameState gameState = (ESuperInfectionGameState)stream.ReceiveNext();
		if (!Enum.IsDefined(typeof(ESuperInfectionGameState), this.gameState))
		{
			return;
		}
		this.gameState = gameState;
		if (this.gameState != this._gameState_previous)
		{
			this._OnGameStateChanged();
			this._gameState_previous = this.gameState;
		}
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x00036FB9 File Offset: 0x000351B9
	public void _OnGameStateChanged()
	{
		if (this.gameState == ESuperInfectionGameState.Starting)
		{
			SIProgression.Instance.AddRoundTelemetry();
		}
		GTDev.Log<string>(string.Format("Game state changed to {0} ...\n(was {1}).", this.gameState, this._gameState_previous), null);
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x00036FF4 File Offset: 0x000351F4
	public override void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		try
		{
			SIProgression.Instance.HandleTagTelemetry(taggedPlayer, taggingPlayer);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
		}
		RigContainer rigContainer;
		RigContainer rigContainer2;
		if (!VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer) || !VRRigCache.Instance.TryGetVrrig(taggingPlayer, out rigContainer2))
		{
			return;
		}
		if (taggingPlayer.ActorNumber != SIPlayer.LocalPlayer.ActorNr)
		{
			return;
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Dash] > 0)
		{
			PlayerGameEvents.MiscEvent("SIDashTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Thruster] > 0)
		{
			PlayerGameEvents.MiscEvent("SIThrusterTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Stilt] > 0)
		{
			PlayerGameEvents.MiscEvent("SIStiltTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Platform] > 0)
		{
			PlayerGameEvents.MiscEvent("SIPlatformTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Blaster] > 0)
		{
			PlayerGameEvents.MiscEvent("SIBlasterTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedOthersGadgets > 0)
		{
			PlayerGameEvents.MiscEvent("SIBorrowedGadgetTag", 1);
		}
		PlayerGameEvents.MiscEvent("SIGameModeTag", 1);
	}

	// Token: 0x04000C6C RID: 3180
	[SerializeField]
	private int _mySuperExampleSerializedField = 123;

	// Token: 0x04000C6E RID: 3182
	private ESuperInfectionGameState _gameState_previous;
}
