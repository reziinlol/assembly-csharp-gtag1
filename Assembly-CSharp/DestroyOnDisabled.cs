using System;
using UnityEngine;

// Token: 0x02000D44 RID: 3396
public class DestroyOnDisabled : MonoBehaviour
{
	// Token: 0x060053AF RID: 21423 RVA: 0x0006BA83 File Offset: 0x00069C83
	private void OnDisable()
	{
		Object.Destroy(base.gameObject);
	}
}
