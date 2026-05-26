using System;
using UnityEngine;

// Token: 0x020004FF RID: 1279
public static class JSonHelper
{
	// Token: 0x0600200E RID: 8206 RVA: 0x000AC644 File Offset: 0x000AA844
	public static T[] FromJson<T>(string json)
	{
		return JsonUtility.FromJson<JSonHelper.Wrapper<T>>(json).Items;
	}

	// Token: 0x0600200F RID: 8207 RVA: 0x000AC651 File Offset: 0x000AA851
	public static string ToJson<T>(T[] array)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		});
	}

	// Token: 0x06002010 RID: 8208 RVA: 0x000AC664 File Offset: 0x000AA864
	public static string ToJson<T>(T[] array, bool prettyPrint)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		}, prettyPrint);
	}

	// Token: 0x02000500 RID: 1280
	[Serializable]
	private class Wrapper<T>
	{
		// Token: 0x04002ACE RID: 10958
		public T[] Items;
	}
}
