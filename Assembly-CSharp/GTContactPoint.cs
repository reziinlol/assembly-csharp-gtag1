using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000338 RID: 824
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public class GTContactPoint
{
	// Token: 0x04001900 RID: 6400
	[NonSerialized]
	[FieldOffset(0)]
	public Matrix4x4 data;

	// Token: 0x04001901 RID: 6401
	[NonSerialized]
	[FieldOffset(0)]
	public Vector4 data0;

	// Token: 0x04001902 RID: 6402
	[NonSerialized]
	[FieldOffset(16)]
	public Vector4 data1;

	// Token: 0x04001903 RID: 6403
	[NonSerialized]
	[FieldOffset(32)]
	public Vector4 data2;

	// Token: 0x04001904 RID: 6404
	[NonSerialized]
	[FieldOffset(48)]
	public Vector4 data3;

	// Token: 0x04001905 RID: 6405
	[FieldOffset(0)]
	public Vector3 contactPoint;

	// Token: 0x04001906 RID: 6406
	[FieldOffset(12)]
	public float radius;

	// Token: 0x04001907 RID: 6407
	[FieldOffset(16)]
	public Vector3 counterVelocity;

	// Token: 0x04001908 RID: 6408
	[FieldOffset(28)]
	public float timestamp;

	// Token: 0x04001909 RID: 6409
	[FieldOffset(32)]
	public Color color;

	// Token: 0x0400190A RID: 6410
	[FieldOffset(48)]
	public GTContactType contactType;

	// Token: 0x0400190B RID: 6411
	[FieldOffset(52)]
	public float lifetime = 1f;

	// Token: 0x0400190C RID: 6412
	[FieldOffset(56)]
	public uint free = 1U;
}
