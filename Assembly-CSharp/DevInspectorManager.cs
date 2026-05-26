using System;
using UnityEngine;

// Token: 0x02000324 RID: 804
public class DevInspectorManager : MonoBehaviour
{
	// Token: 0x170001FF RID: 511
	// (get) Token: 0x060013ED RID: 5101 RVA: 0x0006BA9F File Offset: 0x00069C9F
	public static DevInspectorManager instance
	{
		get
		{
			if (DevInspectorManager._instance == null)
			{
				DevInspectorManager._instance = Object.FindAnyObjectByType<DevInspectorManager>();
			}
			return DevInspectorManager._instance;
		}
	}

	// Token: 0x040018CD RID: 6349
	private static DevInspectorManager _instance;
}
