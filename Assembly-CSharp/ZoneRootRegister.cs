using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020003BA RID: 954
public class ZoneRootRegister : MonoBehaviour
{
	// Token: 0x060016F4 RID: 5876 RVA: 0x00085324 File Offset: 0x00083524
	private void Awake()
	{
		this.watchableSlot.Value = base.gameObject;
	}

	// Token: 0x060016F5 RID: 5877 RVA: 0x00085337 File Offset: 0x00083537
	private void OnDestroy()
	{
		this.watchableSlot.Value = null;
	}

	// Token: 0x04002223 RID: 8739
	[SerializeField]
	private WatchableGameObjectSO watchableSlot;
}
