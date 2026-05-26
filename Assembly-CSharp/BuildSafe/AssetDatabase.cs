using System;
using System.Diagnostics;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02001000 RID: 4096
	public static class AssetDatabase
	{
		// Token: 0x0600666D RID: 26221 RVA: 0x002101C4 File Offset: 0x0020E3C4
		public static T LoadAssetAtPath<T>(string assetPath) where T : Object
		{
			return default(T);
		}

		// Token: 0x0600666E RID: 26222 RVA: 0x002101DA File Offset: 0x0020E3DA
		public static T[] LoadAssetsOfType<T>() where T : Object
		{
			return Array.Empty<T>();
		}

		// Token: 0x0600666F RID: 26223 RVA: 0x002101E1 File Offset: 0x0020E3E1
		public static string[] FindAssetsOfType<T>() where T : Object
		{
			return Array.Empty<string>();
		}

		// Token: 0x06006670 RID: 26224 RVA: 0x002101E8 File Offset: 0x0020E3E8
		[Conditional("UNITY_EDITOR")]
		public static void SaveToDisk(params Object[] assetsToSave)
		{
			AssetDatabase.SaveAssetsToDisk(assetsToSave, true);
		}

		// Token: 0x06006671 RID: 26225 RVA: 0x000028C5 File Offset: 0x00000AC5
		public static void SaveAssetsToDisk(Object[] assetsToSave, bool saveProject = true)
		{
		}
	}
}
