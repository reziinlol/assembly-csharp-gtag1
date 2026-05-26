using System;
using UnityEngine;

// Token: 0x020005B3 RID: 1459
public class FacePlayer : MonoBehaviour
{
	// Token: 0x060024C3 RID: 9411 RVA: 0x000C4E80 File Offset: 0x000C3080
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(base.transform.position - GorillaTagger.Instance.headCollider.transform.position) * Quaternion.AngleAxis(-90f, Vector3.up);
	}
}
