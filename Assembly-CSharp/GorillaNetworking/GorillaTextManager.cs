using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x0200105A RID: 4186
	public class GorillaTextManager : MonoBehaviourPostTick
	{
		// Token: 0x06006924 RID: 26916 RVA: 0x00220826 File Offset: 0x0021EA26
		public static void RegisterText(GorillaText text)
		{
			if (GorillaTextManager.instance == null)
			{
				GorillaTextManager.CreateManager();
			}
			if (!GorillaTextManager.instance.gorillaTexts.Contains(text))
			{
				GorillaTextManager.instance.gorillaTexts.Add(text);
			}
		}

		// Token: 0x06006925 RID: 26917 RVA: 0x0022085C File Offset: 0x0021EA5C
		private void Awake()
		{
			if (GorillaTextManager.instance != null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			GorillaTextManager.instance = this;
		}

		// Token: 0x06006926 RID: 26918 RVA: 0x00220880 File Offset: 0x0021EA80
		public override void PostTick()
		{
			for (int i = 0; i < this.gorillaTexts.Count; i++)
			{
				this.gorillaTexts[i].InvokeIfUpdated();
			}
		}

		// Token: 0x06006927 RID: 26919 RVA: 0x002208B4 File Offset: 0x0021EAB4
		public static void CreateManager()
		{
			GorillaTextManager gorillaTextManager = new GameObject("GorillaTextManager").AddComponent<GorillaTextManager>();
			gorillaTextManager.gorillaTexts = new List<GorillaText>();
			GorillaTextManager.instance = gorillaTextManager;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(gorillaTextManager);
			}
		}

		// Token: 0x0400795D RID: 31069
		public static GorillaTextManager instance;

		// Token: 0x0400795E RID: 31070
		public List<GorillaText> gorillaTexts = new List<GorillaText>();
	}
}
