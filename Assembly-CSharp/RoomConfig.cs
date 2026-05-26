using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Realtime;

// Token: 0x0200046A RID: 1130
public class RoomConfig
{
	// Token: 0x170002E4 RID: 740
	// (get) Token: 0x06001B75 RID: 7029 RVA: 0x00094D93 File Offset: 0x00092F93
	public bool IsJoiningWithFriends
	{
		get
		{
			return this.joinFriendIDs != null && this.joinFriendIDs.Length != 0;
		}
	}

	// Token: 0x06001B76 RID: 7030 RVA: 0x00094DAC File Offset: 0x00092FAC
	public void SetFriendIDs(List<string> friendIDs)
	{
		for (int i = 0; i < friendIDs.Count; i++)
		{
			if (friendIDs[i] == NetworkSystem.Instance.GetMyNickName())
			{
				friendIDs.RemoveAt(i);
				i--;
			}
		}
		this.joinFriendIDs = new string[friendIDs.Count];
		for (int j = 0; j < friendIDs.Count; j++)
		{
			this.joinFriendIDs[j] = friendIDs[j];
		}
	}

	// Token: 0x06001B77 RID: 7031 RVA: 0x00094E1E File Offset: 0x0009301E
	public void ClearExpectedUsers()
	{
		if (this.joinFriendIDs == null || this.joinFriendIDs.Length == 0)
		{
			return;
		}
		this.joinFriendIDs = new string[0];
	}

	// Token: 0x06001B78 RID: 7032 RVA: 0x00094E40 File Offset: 0x00093040
	public RoomOptions ToPUNOpts()
	{
		return new RoomOptions
		{
			IsVisible = this.isPublic,
			IsOpen = this.isJoinable,
			MaxPlayers = this.MaxPlayers,
			CustomRoomProperties = this.CustomProps,
			PublishUserId = true,
			CustomRoomPropertiesForLobby = this.AutoCustomLobbyProps()
		};
	}

	// Token: 0x06001B79 RID: 7033 RVA: 0x00094E95 File Offset: 0x00093095
	public void SetFusionOpts(NetworkRunner runnerInst)
	{
		runnerInst.SessionInfo.IsVisible = this.isPublic;
		runnerInst.SessionInfo.IsOpen = this.isJoinable;
	}

	// Token: 0x06001B7A RID: 7034 RVA: 0x00094EB9 File Offset: 0x000930B9
	public static RoomConfig SPConfig()
	{
		return new RoomConfig
		{
			isPublic = false,
			isJoinable = false,
			MaxPlayers = 1
		};
	}

	// Token: 0x06001B7B RID: 7035 RVA: 0x00094ED5 File Offset: 0x000930D5
	public static RoomConfig AnyPublicConfig()
	{
		return new RoomConfig
		{
			isPublic = true,
			isJoinable = true,
			createIfMissing = true,
			MaxPlayers = 10
		};
	}

	// Token: 0x06001B7C RID: 7036 RVA: 0x00094EFC File Offset: 0x000930FC
	private string[] AutoCustomLobbyProps()
	{
		string[] array = new string[this.CustomProps.Count];
		int num = 0;
		foreach (DictionaryEntry dictionaryEntry in this.CustomProps)
		{
			array[num] = (string)dictionaryEntry.Key;
			num++;
		}
		return array;
	}

	// Token: 0x04002598 RID: 9624
	public const string Room_GameModePropKey = "gameMode";

	// Token: 0x04002599 RID: 9625
	public const string Room_PlatformPropKey = "platform";

	// Token: 0x0400259A RID: 9626
	public bool isPublic;

	// Token: 0x0400259B RID: 9627
	public bool isJoinable;

	// Token: 0x0400259C RID: 9628
	public byte MaxPlayers;

	// Token: 0x0400259D RID: 9629
	public Hashtable CustomProps = new Hashtable();

	// Token: 0x0400259E RID: 9630
	public bool createIfMissing;

	// Token: 0x0400259F RID: 9631
	public string[] joinFriendIDs;
}
