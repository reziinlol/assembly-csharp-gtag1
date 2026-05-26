using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001140 RID: 4416
	public static class GTAppState
	{
		// Token: 0x17000AB8 RID: 2744
		// (get) Token: 0x0600701C RID: 28700 RVA: 0x00249327 File Offset: 0x00247527
		// (set) Token: 0x0600701D RID: 28701 RVA: 0x0024932E File Offset: 0x0024752E
		public static bool isQuitting { get; private set; }

		// Token: 0x0600701E RID: 28702 RVA: 0x00249338 File Offset: 0x00247538
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void HandleOnSubsystemRegistration()
		{
			GTAppState.isQuitting = false;
			Application.quitting += delegate()
			{
				GTAppState.isQuitting = true;
			};
			Debug.Log(string.Concat(new string[]
			{
				"GTAppState:\n- SystemInfo.operatingSystem=",
				SystemInfo.operatingSystem,
				"\n- SystemInfo.maxTextureArraySlices=",
				SystemInfo.maxTextureArraySlices.ToString(),
				"\n"
			}));
		}

		// Token: 0x0600701F RID: 28703 RVA: 0x000028C5 File Offset: 0x00000AC5
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void HandleOnAfterSceneLoad()
		{
		}
	}
}
