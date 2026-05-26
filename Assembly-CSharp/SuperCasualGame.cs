using System;
using Fusion;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200017D RID: 381
public sealed class SuperCasualGame : GorillaGameManager
{
	// Token: 0x06000A06 RID: 2566 RVA: 0x00002076 File Offset: 0x00000276
	public override int MyMatIndex(NetPlayer player)
	{
		return 0;
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnSerializeRead(object newData)
	{
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x00035D0D File Offset: 0x00033F0D
	public override object OnSerializeWrite()
	{
		return null;
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x00035D10 File Offset: 0x00033F10
	public override GameModeType GameType()
	{
		return GameModeType.SuperCasual;
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x00035D14 File Offset: 0x00033F14
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<CasualGameModeData>();
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x00035D1D File Offset: 0x00033F1D
	public override string GameModeName()
	{
		return "SUPER CASUAL";
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x00035D24 File Offset: 0x00033F24
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_SUPER_CASUAL_ROOM_LABEL", out result, "(SUPER CASUAL GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_SUPER_CASUAL_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x00035D50 File Offset: 0x00033F50
	public override void StartPlaying()
	{
		base.StartPlaying();
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

	// Token: 0x06000A10 RID: 2576 RVA: 0x00035DA3 File Offset: 0x00033FA3
	public override void StopPlaying()
	{
		base.StopPlaying();
		VRRig.LocalRig.EnableSuperInfectionHands(false);
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x00035DB8 File Offset: 0x00033FB8
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(newPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableSuperInfectionHands(true);
		}
	}
}
