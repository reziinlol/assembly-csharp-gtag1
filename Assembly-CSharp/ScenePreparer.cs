using System;
using UnityEngine;

// Token: 0x02000396 RID: 918
[DefaultExecutionOrder(-9999)]
public class ScenePreparer : MonoBehaviour
{
	// Token: 0x0600164F RID: 5711 RVA: 0x0008177C File Offset: 0x0007F97C
	protected void Awake()
	{
		bool flag = false;
		GameObject[] array = this.betaEnableObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		array = this.betaDisableObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!flag);
		}
	}

	// Token: 0x04002068 RID: 8296
	public OVRManager ovrManager;

	// Token: 0x04002069 RID: 8297
	public GameObject[] betaDisableObjects;

	// Token: 0x0400206A RID: 8298
	public GameObject[] betaEnableObjects;
}
