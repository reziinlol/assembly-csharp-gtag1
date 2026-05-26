using System;
using UnityEngine;

// Token: 0x020008CA RID: 2250
public class GorillaUIParent : MonoBehaviour
{
	// Token: 0x06003AD9 RID: 15065 RVA: 0x00143369 File Offset: 0x00141569
	private void Awake()
	{
		if (GorillaUIParent.instance == null)
		{
			GorillaUIParent.instance = this;
			return;
		}
		if (GorillaUIParent.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04004B4B RID: 19275
	[OnEnterPlay_SetNull]
	public static volatile GorillaUIParent instance;
}
