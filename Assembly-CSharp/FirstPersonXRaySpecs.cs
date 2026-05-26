using System;
using UnityEngine;

// Token: 0x0200057E RID: 1406
public class FirstPersonXRaySpecs : MonoBehaviour
{
	// Token: 0x060023A3 RID: 9123 RVA: 0x000BFEA5 File Offset: 0x000BE0A5
	private void OnEnable()
	{
		GorillaBodyRenderer.SetAllSkeletons(true);
	}

	// Token: 0x060023A4 RID: 9124 RVA: 0x000BFEAD File Offset: 0x000BE0AD
	private void OnDisable()
	{
		GorillaBodyRenderer.SetAllSkeletons(false);
	}
}
