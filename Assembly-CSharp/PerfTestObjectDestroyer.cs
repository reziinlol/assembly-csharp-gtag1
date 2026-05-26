using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020003CB RID: 971
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST && !BETA")]
public class PerfTestObjectDestroyer : MonoBehaviour
{
	// Token: 0x06001730 RID: 5936 RVA: 0x00085FAE File Offset: 0x000841AE
	private void Start()
	{
		Object.DestroyImmediate(base.gameObject, true);
	}
}
