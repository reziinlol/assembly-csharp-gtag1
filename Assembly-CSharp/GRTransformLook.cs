using System;
using UnityEngine;

// Token: 0x02000819 RID: 2073
public class GRTransformLook : MonoBehaviour
{
	// Token: 0x0600353E RID: 13630 RVA: 0x00126CFC File Offset: 0x00124EFC
	private void Awake()
	{
		if (this.followPlayer)
		{
			this.lookTarget = Camera.main.transform;
		}
	}

	// Token: 0x0600353F RID: 13631 RVA: 0x00126D16 File Offset: 0x00124F16
	private void LateUpdate()
	{
		base.transform.LookAt(this.lookTarget);
		base.transform.rotation *= Quaternion.Euler(this.offsetRotation);
	}

	// Token: 0x040045C5 RID: 17861
	public bool followPlayer;

	// Token: 0x040045C6 RID: 17862
	public Transform lookTarget;

	// Token: 0x040045C7 RID: 17863
	public Vector3 offsetRotation;
}
