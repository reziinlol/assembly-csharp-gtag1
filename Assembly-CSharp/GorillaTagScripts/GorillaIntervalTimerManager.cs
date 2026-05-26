using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F10 RID: 3856
	public class GorillaIntervalTimerManager : MonoBehaviour
	{
		// Token: 0x06006030 RID: 24624 RVA: 0x001F0522 File Offset: 0x001EE722
		protected void Awake()
		{
			if (GorillaIntervalTimerManager.hasInstance && GorillaIntervalTimerManager.instance != null && GorillaIntervalTimerManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			GorillaIntervalTimerManager.SetInstance(this);
		}

		// Token: 0x06006031 RID: 24625 RVA: 0x001F0552 File Offset: 0x001EE752
		private static void CreateManager()
		{
			GorillaIntervalTimerManager.SetInstance(new GameObject("GorillaIntervalTimerManager").AddComponent<GorillaIntervalTimerManager>());
		}

		// Token: 0x06006032 RID: 24626 RVA: 0x001F0568 File Offset: 0x001EE768
		private static void SetInstance(GorillaIntervalTimerManager manager)
		{
			GorillaIntervalTimerManager.instance = manager;
			GorillaIntervalTimerManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x06006033 RID: 24627 RVA: 0x001F0583 File Offset: 0x001EE783
		public static void RegisterGorillaTimer(GorillaIntervalTimer gTimer)
		{
			if (!GorillaIntervalTimerManager.hasInstance)
			{
				GorillaIntervalTimerManager.CreateManager();
			}
			if (!GorillaIntervalTimerManager.allTimers.Contains(gTimer))
			{
				GorillaIntervalTimerManager.allTimers.Add(gTimer);
			}
		}

		// Token: 0x06006034 RID: 24628 RVA: 0x001F05A9 File Offset: 0x001EE7A9
		public static void UnregisterGorillaTimer(GorillaIntervalTimer gTimer)
		{
			if (!GorillaIntervalTimerManager.hasInstance)
			{
				GorillaIntervalTimerManager.CreateManager();
			}
			if (GorillaIntervalTimerManager.allTimers.Contains(gTimer))
			{
				GorillaIntervalTimerManager.allTimers.Remove(gTimer);
			}
		}

		// Token: 0x06006035 RID: 24629 RVA: 0x001F05D0 File Offset: 0x001EE7D0
		private void Update()
		{
			for (int i = 0; i < GorillaIntervalTimerManager.allTimers.Count; i++)
			{
				GorillaIntervalTimerManager.allTimers[i].InvokeUpdate();
			}
		}

		// Token: 0x04006ECB RID: 28363
		private static GorillaIntervalTimerManager instance;

		// Token: 0x04006ECC RID: 28364
		private static bool hasInstance = false;

		// Token: 0x04006ECD RID: 28365
		private static List<GorillaIntervalTimer> allTimers = new List<GorillaIntervalTimer>();
	}
}
