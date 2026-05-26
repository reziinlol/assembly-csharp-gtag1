using System;
using UnityEngine;

// Token: 0x0200052D RID: 1325
public class TestManipulatableSpinner : MonoBehaviour
{
	// Token: 0x06002163 RID: 8547 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Start()
	{
	}

	// Token: 0x06002164 RID: 8548 RVA: 0x000B2108 File Offset: 0x000B0308
	private void LateUpdate()
	{
		float angle = this.spinner.angle;
		base.transform.rotation = Quaternion.Euler(0f, angle * this.rotationScale, 0f);
	}

	// Token: 0x04002C18 RID: 11288
	public ManipulatableSpinner spinner;

	// Token: 0x04002C19 RID: 11289
	public float rotationScale = 1f;
}
