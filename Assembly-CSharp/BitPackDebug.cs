using System;
using UnityEngine;

// Token: 0x02000D31 RID: 3377
public class BitPackDebug : MonoBehaviour
{
	// Token: 0x04006474 RID: 25716
	public bool debugPos;

	// Token: 0x04006475 RID: 25717
	public Vector3 pos;

	// Token: 0x04006476 RID: 25718
	public Vector3 min = Vector3.one * -2f;

	// Token: 0x04006477 RID: 25719
	public Vector3 max = Vector3.one * 2f;

	// Token: 0x04006478 RID: 25720
	public float rad = 4f;

	// Token: 0x04006479 RID: 25721
	[Space]
	public bool debug32;

	// Token: 0x0400647A RID: 25722
	public uint packed;

	// Token: 0x0400647B RID: 25723
	public Vector3 unpacked;

	// Token: 0x0400647C RID: 25724
	[Space]
	public bool debug16;

	// Token: 0x0400647D RID: 25725
	public ushort packed16;

	// Token: 0x0400647E RID: 25726
	public Vector3 unpacked16;
}
