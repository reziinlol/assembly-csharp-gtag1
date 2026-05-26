using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020003CC RID: 972
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST && !BETA")]
public class TestTeleportDestination : MonoBehaviour
{
	// Token: 0x06001732 RID: 5938 RVA: 0x00085FBC File Offset: 0x000841BC
	private void OnDrawGizmosSelected()
	{
		Debug.DrawRay(base.transform.position, base.transform.forward * 2f, Color.magenta);
	}

	// Token: 0x0400226C RID: 8812
	public GTZone[] zones;

	// Token: 0x0400226D RID: 8813
	public GameObject teleportTransform;
}
