using System;
using UnityEngine;

// Token: 0x0200019D RID: 413
public class Pendulum : MonoBehaviour
{
	// Token: 0x06000B1F RID: 2847 RVA: 0x0003B860 File Offset: 0x00039A60
	private void Start()
	{
		this.pendulum = (this.ClockPendulum = base.gameObject.GetComponent<Transform>());
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0003B888 File Offset: 0x00039A88
	private void Update()
	{
		if (this.pendulum)
		{
			float z = this.MaxAngleDeflection * Mathf.Sin(Time.time * this.SpeedOfPendulum);
			this.pendulum.localRotation = Quaternion.Euler(0f, 0f, z);
			return;
		}
	}

	// Token: 0x04000D61 RID: 3425
	public float MaxAngleDeflection = 10f;

	// Token: 0x04000D62 RID: 3426
	public float SpeedOfPendulum = 1f;

	// Token: 0x04000D63 RID: 3427
	public Transform ClockPendulum;

	// Token: 0x04000D64 RID: 3428
	private Transform pendulum;
}
