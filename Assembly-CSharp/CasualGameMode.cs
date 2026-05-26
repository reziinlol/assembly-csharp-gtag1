using System;
using Fusion;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005B8 RID: 1464
public sealed class CasualGameMode : GorillaGameManager
{
	// Token: 0x060024D8 RID: 9432 RVA: 0x00002076 File Offset: 0x00000276
	public override int MyMatIndex(NetPlayer player)
	{
		return 0;
	}

	// Token: 0x060024D9 RID: 9433 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnSerializeRead(object newData)
	{
	}

	// Token: 0x060024DA RID: 9434 RVA: 0x00035D0D File Offset: 0x00033F0D
	public override object OnSerializeWrite()
	{
		return null;
	}

	// Token: 0x060024DB RID: 9435 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060024DC RID: 9436 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060024DD RID: 9437 RVA: 0x00002076 File Offset: 0x00000276
	public override GameModeType GameType()
	{
		return GameModeType.Casual;
	}

	// Token: 0x060024DE RID: 9438 RVA: 0x00035D14 File Offset: 0x00033F14
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<CasualGameModeData>();
	}

	// Token: 0x060024DF RID: 9439 RVA: 0x000C542C File Offset: 0x000C362C
	public override string GameModeName()
	{
		return "CASUAL";
	}

	// Token: 0x060024E0 RID: 9440 RVA: 0x000C5434 File Offset: 0x000C3634
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_CASUAL_ROOM_LABEL", out result, "(CASUAL GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_CASUAL_ROOM_LABEL]");
		}
		return result;
	}
}
