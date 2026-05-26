using System;
using UnityEngine;

// Token: 0x0200045A RID: 1114
[Serializable]
public struct NetworkSystemConfig
{
	// Token: 0x170002B5 RID: 693
	// (get) Token: 0x06001AB0 RID: 6832 RVA: 0x00093E6D File Offset: 0x0009206D
	public static string AppVersion
	{
		get
		{
			return NetworkSystemConfig.prependCode + "." + NetworkSystemConfig.AppVersionStripped;
		}
	}

	// Token: 0x170002B6 RID: 694
	// (get) Token: 0x06001AB1 RID: 6833 RVA: 0x00093E84 File Offset: 0x00092084
	public static string AppVersionStripped
	{
		get
		{
			return string.Concat(new string[]
			{
				NetworkSystemConfig.gameVersionType,
				".",
				NetworkSystemConfig.majorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion2.ToString()
			});
		}
	}

	// Token: 0x170002B7 RID: 695
	// (get) Token: 0x06001AB2 RID: 6834 RVA: 0x00093EE4 File Offset: 0x000920E4
	public static string BundleVersion
	{
		get
		{
			return string.Concat(new string[]
			{
				NetworkSystemConfig.majorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion2.ToString()
			});
		}
	}

	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x06001AB3 RID: 6835 RVA: 0x00093F33 File Offset: 0x00092133
	public static string GameVersionType
	{
		get
		{
			return NetworkSystemConfig.gameVersionType;
		}
	}

	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x06001AB4 RID: 6836 RVA: 0x00093F3A File Offset: 0x0009213A
	public static int GameMajorVersion
	{
		get
		{
			return NetworkSystemConfig.majorVersion;
		}
	}

	// Token: 0x170002BA RID: 698
	// (get) Token: 0x06001AB5 RID: 6837 RVA: 0x00093F41 File Offset: 0x00092141
	public static int GameMinorVersion
	{
		get
		{
			return NetworkSystemConfig.minorVersion;
		}
	}

	// Token: 0x170002BB RID: 699
	// (get) Token: 0x06001AB6 RID: 6838 RVA: 0x00093F48 File Offset: 0x00092148
	public static int GameMinorVersion2
	{
		get
		{
			return NetworkSystemConfig.minorVersion2;
		}
	}

	// Token: 0x0400254B RID: 9547
	[HideInInspector]
	public int MaxPlayerCount;

	// Token: 0x0400254C RID: 9548
	private static string gameVersionType = "live1";

	// Token: 0x0400254D RID: 9549
	public static string prependCode = "prependrandommayrooms";

	// Token: 0x0400254E RID: 9550
	public static int majorVersion = 1;

	// Token: 0x0400254F RID: 9551
	public static int minorVersion = 1;

	// Token: 0x04002550 RID: 9552
	public static int minorVersion2 = 137;
}
