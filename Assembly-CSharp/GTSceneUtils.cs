using System;
using System.Diagnostics;
using UnityEngine.SceneManagement;

// Token: 0x02000DB5 RID: 3509
public static class GTSceneUtils
{
	// Token: 0x060055FF RID: 22015 RVA: 0x000028C5 File Offset: 0x00000AC5
	[Conditional("UNITY_EDITOR")]
	public static void AddToBuild(GTScene scene)
	{
	}

	// Token: 0x06005600 RID: 22016 RVA: 0x001BF931 File Offset: 0x001BDB31
	public static bool Equals(GTScene x, Scene y)
	{
		return !(x == null) && y.IsValid() && x.Equals(y);
	}

	// Token: 0x06005601 RID: 22017 RVA: 0x001BF955 File Offset: 0x001BDB55
	public static GTScene[] ScenesInBuild()
	{
		return Array.Empty<GTScene>();
	}

	// Token: 0x06005602 RID: 22018 RVA: 0x000028C5 File Offset: 0x00000AC5
	[Conditional("UNITY_EDITOR")]
	public static void SyncBuildScenes()
	{
	}
}
