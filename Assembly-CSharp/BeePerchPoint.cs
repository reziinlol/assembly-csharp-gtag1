using System;
using UnityEngine;

// Token: 0x02000221 RID: 545
public class BeePerchPoint : MonoBehaviour
{
	// Token: 0x06000E3D RID: 3645 RVA: 0x0004E873 File Offset: 0x0004CA73
	public Vector3 GetPoint()
	{
		return base.transform.TransformPoint(this.localPosition);
	}

	// Token: 0x0400113D RID: 4413
	[SerializeField]
	private Vector3 localPosition;
}
