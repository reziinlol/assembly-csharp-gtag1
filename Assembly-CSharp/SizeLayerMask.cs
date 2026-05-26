using System;
using UnityEngine;

// Token: 0x0200090E RID: 2318
[Serializable]
public class SizeLayerMask
{
	// Token: 0x17000579 RID: 1401
	// (get) Token: 0x06003C9A RID: 15514 RVA: 0x00149F4C File Offset: 0x0014814C
	public int Mask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x04004D41 RID: 19777
	[SerializeField]
	private bool affectLayerA = true;

	// Token: 0x04004D42 RID: 19778
	[SerializeField]
	private bool affectLayerB = true;

	// Token: 0x04004D43 RID: 19779
	[SerializeField]
	private bool affectLayerC = true;

	// Token: 0x04004D44 RID: 19780
	[SerializeField]
	private bool affectLayerD = true;
}
