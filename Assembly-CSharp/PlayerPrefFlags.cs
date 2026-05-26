using System;
using UnityEngine;

// Token: 0x02000498 RID: 1176
public class PlayerPrefFlags
{
	// Token: 0x06001C75 RID: 7285 RVA: 0x0009A237 File Offset: 0x00098437
	internal static bool Check(PlayerPrefFlags.Flag flag)
	{
		return (PlayerPrefs.GetInt("PlayerPrefFlags0", 5) & (int)flag) == (int)flag;
	}

	// Token: 0x06001C76 RID: 7286 RVA: 0x0009A24C File Offset: 0x0009844C
	internal static void Touch(PlayerPrefFlags.Flag flag)
	{
		bool arg = (PlayerPrefs.GetInt("PlayerPrefFlags0", 5) & (int)flag) == (int)flag;
		if (PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange(flag, arg);
		}
	}

	// Token: 0x06001C77 RID: 7287 RVA: 0x0009A280 File Offset: 0x00098480
	internal static void TouchIf(PlayerPrefFlags.Flag flag, bool value)
	{
		int @int = PlayerPrefs.GetInt("PlayerPrefFlags0", 5);
		if (value == ((@int & (int)flag) == (int)flag) && PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange(flag, value);
		}
	}

	// Token: 0x06001C78 RID: 7288 RVA: 0x0009A2B8 File Offset: 0x000984B8
	internal static void Set(PlayerPrefFlags.Flag flag, bool value)
	{
		int num = PlayerPrefs.GetInt("PlayerPrefFlags0", 5);
		if (value)
		{
			num |= (int)flag;
		}
		else
		{
			num &= (int)(~(int)flag);
		}
		PlayerPrefs.SetInt("PlayerPrefFlags0", num);
		if (PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange(flag, value);
		}
	}

	// Token: 0x06001C79 RID: 7289 RVA: 0x0009A300 File Offset: 0x00098500
	internal static bool Flip(PlayerPrefFlags.Flag flag)
	{
		int num = PlayerPrefs.GetInt("PlayerPrefFlags0", 5);
		bool flag2 = (num & (int)flag) != (int)flag;
		if (flag2)
		{
			num |= (int)flag;
		}
		else
		{
			num &= (int)(~(int)flag);
		}
		PlayerPrefs.SetInt("PlayerPrefFlags0", num);
		if (PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange(flag, flag2);
		}
		return flag2;
	}

	// Token: 0x04002689 RID: 9865
	public static Action<PlayerPrefFlags.Flag, bool> OnFlagChange;

	// Token: 0x0400268A RID: 9866
	private const int defaultValue = 5;

	// Token: 0x02000499 RID: 1177
	public enum Flag
	{
		// Token: 0x0400268C RID: 9868
		SHOW_1P_COSMETICS = 1,
		// Token: 0x0400268D RID: 9869
		SWAP_HELD_COSMETICS,
		// Token: 0x0400268E RID: 9870
		GAME_MODE_SELECTOR_IS_SUPER = 4
	}
}
