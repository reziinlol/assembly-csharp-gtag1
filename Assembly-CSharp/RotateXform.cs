using System;
using UnityEngine;

// Token: 0x020001BE RID: 446
public class RotateXform : MonoBehaviour
{
	// Token: 0x06000BE8 RID: 3048 RVA: 0x00040F90 File Offset: 0x0003F190
	private void Update()
	{
		if (!this.xform)
		{
			return;
		}
		Vector3 vector = (this.mode == RotateXform.Mode.Local) ? this.xform.localEulerAngles : this.xform.eulerAngles;
		float num = Time.deltaTime * this.speedFactor;
		vector.x += this.speed.x * num;
		vector.y += this.speed.y * num;
		vector.z += this.speed.z * num;
		if (this.mode == RotateXform.Mode.Local)
		{
			this.xform.localEulerAngles = vector;
			return;
		}
		this.xform.eulerAngles = vector;
	}

	// Token: 0x04000E84 RID: 3716
	public Transform xform;

	// Token: 0x04000E85 RID: 3717
	public Vector3 speed = Vector3.zero;

	// Token: 0x04000E86 RID: 3718
	public RotateXform.Mode mode;

	// Token: 0x04000E87 RID: 3719
	public float speedFactor = 0.0625f;

	// Token: 0x020001BF RID: 447
	public enum Mode
	{
		// Token: 0x04000E89 RID: 3721
		Local,
		// Token: 0x04000E8A RID: 3722
		World
	}
}
