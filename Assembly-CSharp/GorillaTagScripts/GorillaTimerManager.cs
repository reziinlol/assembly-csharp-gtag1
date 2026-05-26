using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F15 RID: 3861
	public class GorillaTimerManager : MonoBehaviour
	{
		// Token: 0x06006061 RID: 24673 RVA: 0x001F0BAB File Offset: 0x001EEDAB
		protected void Awake()
		{
			if (GorillaTimerManager.hasInstance && GorillaTimerManager.instance != null && GorillaTimerManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			GorillaTimerManager.SetInstance(this);
		}

		// Token: 0x06006062 RID: 24674 RVA: 0x001F0BDB File Offset: 0x001EEDDB
		public static void CreateManager()
		{
			GorillaTimerManager.SetInstance(new GameObject("GorillaTimerManager").AddComponent<GorillaTimerManager>());
		}

		// Token: 0x06006063 RID: 24675 RVA: 0x001F0BF1 File Offset: 0x001EEDF1
		private static void SetInstance(GorillaTimerManager manager)
		{
			GorillaTimerManager.instance = manager;
			GorillaTimerManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x06006064 RID: 24676 RVA: 0x001F0C0C File Offset: 0x001EEE0C
		public static void RegisterGorillaTimer(GorillaTimer gTimer)
		{
			if (!GorillaTimerManager.hasInstance)
			{
				GorillaTimerManager.CreateManager();
			}
			if (!GorillaTimerManager.allTimers.Contains(gTimer))
			{
				GorillaTimerManager.allTimers.Add(gTimer);
			}
		}

		// Token: 0x06006065 RID: 24677 RVA: 0x001F0C32 File Offset: 0x001EEE32
		public static void UnregisterGorillaTimer(GorillaTimer gTimer)
		{
			if (!GorillaTimerManager.hasInstance)
			{
				GorillaTimerManager.CreateManager();
			}
			if (GorillaTimerManager.allTimers.Contains(gTimer))
			{
				GorillaTimerManager.allTimers.Remove(gTimer);
			}
		}

		// Token: 0x06006066 RID: 24678 RVA: 0x001F0C5C File Offset: 0x001EEE5C
		public void Update()
		{
			for (int i = 0; i < GorillaTimerManager.allTimers.Count; i++)
			{
				GorillaTimerManager.allTimers[i].InvokeUpdate();
			}
		}

		// Token: 0x04006EE7 RID: 28391
		public static GorillaTimerManager instance;

		// Token: 0x04006EE8 RID: 28392
		public static bool hasInstance = false;

		// Token: 0x04006EE9 RID: 28393
		public static List<GorillaTimer> allTimers = new List<GorillaTimer>();
	}
}
