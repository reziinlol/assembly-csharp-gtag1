using System;
using UnityEngine;

// Token: 0x02000D58 RID: 3416
public class LookAtTransform : MonoBehaviour
{
	// Token: 0x060053F0 RID: 21488 RVA: 0x001B6D2B File Offset: 0x001B4F2B
	private void Update()
	{
		base.transform.rotation = Quaternion.LookRotation(this.lookAt.position - base.transform.position);
	}

	// Token: 0x040064FA RID: 25850
	[SerializeField]
	private Transform lookAt;
}
