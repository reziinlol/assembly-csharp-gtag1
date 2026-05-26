using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000D50 RID: 3408
public class HideInQuest1AtRuntime : MonoBehaviour
{
	// Token: 0x060053C2 RID: 21442 RVA: 0x001B6612 File Offset: 0x001B4812
	private void OnEnable()
	{
		if (PlayFabAuthenticator.instance != null && "Quest1" == PlayFabAuthenticator.instance.platform.ToString())
		{
			Object.Destroy(base.gameObject);
		}
	}
}
