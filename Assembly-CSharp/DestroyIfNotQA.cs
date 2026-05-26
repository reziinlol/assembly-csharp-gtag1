using System;
using UnityEngine;

// Token: 0x020005A9 RID: 1449
public class DestroyIfNotQA : MonoBehaviour
{
	// Token: 0x060024A6 RID: 9382 RVA: 0x0006BA83 File Offset: 0x00069C83
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
