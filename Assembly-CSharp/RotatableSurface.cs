using System;
using UnityEngine;

// Token: 0x02000576 RID: 1398
public class RotatableSurface : MonoBehaviour
{
	// Token: 0x06002389 RID: 9097 RVA: 0x000BFB88 File Offset: 0x000BDD88
	private void LateUpdate()
	{
		float angle = this.spinner.angle;
		base.transform.localRotation = Quaternion.Euler(0f, angle * this.rotationScale, 0f);
	}

	// Token: 0x04002EB3 RID: 11955
	public ManipulatableSpinner spinner;

	// Token: 0x04002EB4 RID: 11956
	public float rotationScale = 1f;
}
