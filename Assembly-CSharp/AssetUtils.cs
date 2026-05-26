using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000D22 RID: 3362
public static class AssetUtils
{
	// Token: 0x06005300 RID: 21248 RVA: 0x000028C5 File Offset: 0x00000AC5
	[Conditional("UNITY_EDITOR")]
	public static void ExecAndUnloadUnused(Action action)
	{
	}

	// Token: 0x06005301 RID: 21249 RVA: 0x001B2F3E File Offset: 0x001B113E
	[Conditional("UNITY_EDITOR")]
	public static void LoadAssetOfType<T>(ref T result, ref string resultPath) where T : Object
	{
		result = default(T);
		resultPath = null;
	}

	// Token: 0x06005302 RID: 21250 RVA: 0x001B2F4A File Offset: 0x001B114A
	[Conditional("UNITY_EDITOR")]
	public static void FindAllAssetsOfType<T>(ref T[] results, ref string[] assetPaths) where T : Object
	{
		results = Array.Empty<T>();
	}

	// Token: 0x06005303 RID: 21251 RVA: 0x000028C5 File Offset: 0x00000AC5
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void ForceSave<T>(this IList<T> assets, Action<T> onPreSave = null, bool unloadUnusedAfter = false) where T : Object
	{
	}

	// Token: 0x06005304 RID: 21252 RVA: 0x000028C5 File Offset: 0x00000AC5
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void ForceSave(this Object asset)
	{
	}

	// Token: 0x06005305 RID: 21253 RVA: 0x001B2F53 File Offset: 0x001B1153
	public static long ComputeAssetId(this Object asset, bool unsigned = false)
	{
		return 0L;
	}

	// Token: 0x06005306 RID: 21254 RVA: 0x001B2F58 File Offset: 0x001B1158
	public static string GetGameObjectPath(GameObject obj)
	{
		string text = "/" + obj.name;
		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			text = "/" + obj.name + text;
		}
		return text;
	}
}
