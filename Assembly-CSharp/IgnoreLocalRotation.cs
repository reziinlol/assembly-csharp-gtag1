using System;
using UnityEngine;

// Token: 0x020002E0 RID: 736
public class IgnoreLocalRotation : MonoBehaviour
{
	// Token: 0x060012B8 RID: 4792 RVA: 0x00063A17 File Offset: 0x00061C17
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.identity;
	}
}
