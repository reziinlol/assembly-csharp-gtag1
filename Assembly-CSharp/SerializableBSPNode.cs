using System;
using UnityEngine;

// Token: 0x02000E1D RID: 3613
[Serializable]
public struct SerializableBSPNode
{
	// Token: 0x1700084C RID: 2124
	// (get) Token: 0x06005807 RID: 22535 RVA: 0x001C9B29 File Offset: 0x001C7D29
	public int matrixIndex
	{
		get
		{
			return (int)this.leftChildIndex;
		}
	}

	// Token: 0x1700084D RID: 2125
	// (get) Token: 0x06005808 RID: 22536 RVA: 0x001C9B31 File Offset: 0x001C7D31
	public int outsideChildIndex
	{
		get
		{
			return (int)this.rightChildIndex;
		}
	}

	// Token: 0x1700084E RID: 2126
	// (get) Token: 0x06005809 RID: 22537 RVA: 0x001C9B29 File Offset: 0x001C7D29
	public int zoneIndex
	{
		get
		{
			return (int)this.leftChildIndex;
		}
	}

	// Token: 0x040068AE RID: 26798
	[SerializeField]
	public SerializableBSPNode.Axis axis;

	// Token: 0x040068AF RID: 26799
	[SerializeField]
	public float splitValue;

	// Token: 0x040068B0 RID: 26800
	[SerializeField]
	public short leftChildIndex;

	// Token: 0x040068B1 RID: 26801
	[SerializeField]
	public short rightChildIndex;

	// Token: 0x02000E1E RID: 3614
	public enum Axis
	{
		// Token: 0x040068B3 RID: 26803
		X,
		// Token: 0x040068B4 RID: 26804
		Y,
		// Token: 0x040068B5 RID: 26805
		Z,
		// Token: 0x040068B6 RID: 26806
		MatrixChain,
		// Token: 0x040068B7 RID: 26807
		MatrixFinal,
		// Token: 0x040068B8 RID: 26808
		Zone
	}
}
