using System;
using System.Runtime.InteropServices;
using Fusion;
using UnityEngine;

// Token: 0x02000438 RID: 1080
[NetworkInputWeaved(35)]
[StructLayout(LayoutKind.Explicit, Size = 140)]
public struct NetworkedInput : INetworkInput
{
	// Token: 0x0400246F RID: 9327
	[FieldOffset(0)]
	public Quaternion headRot_LS;

	// Token: 0x04002470 RID: 9328
	[FieldOffset(16)]
	public Vector3 rightHandPos_LS;

	// Token: 0x04002471 RID: 9329
	[FieldOffset(28)]
	public Quaternion rightHandRot_LS;

	// Token: 0x04002472 RID: 9330
	[FieldOffset(44)]
	public Vector3 leftHandPos_LS;

	// Token: 0x04002473 RID: 9331
	[FieldOffset(56)]
	public Quaternion leftHandRot_LS;

	// Token: 0x04002474 RID: 9332
	[FieldOffset(72)]
	public Vector3 rootPosition;

	// Token: 0x04002475 RID: 9333
	[FieldOffset(84)]
	public Quaternion rootRotation;

	// Token: 0x04002476 RID: 9334
	[FieldOffset(100)]
	public bool leftThumbTouch;

	// Token: 0x04002477 RID: 9335
	[FieldOffset(104)]
	public bool leftThumbPress;

	// Token: 0x04002478 RID: 9336
	[FieldOffset(108)]
	public float leftIndexValue;

	// Token: 0x04002479 RID: 9337
	[FieldOffset(112)]
	public float leftMiddleValue;

	// Token: 0x0400247A RID: 9338
	[FieldOffset(116)]
	public bool rightThumbTouch;

	// Token: 0x0400247B RID: 9339
	[FieldOffset(120)]
	public bool rightThumbPress;

	// Token: 0x0400247C RID: 9340
	[FieldOffset(124)]
	public float rightIndexValue;

	// Token: 0x0400247D RID: 9341
	[FieldOffset(128)]
	public float rightMiddleValue;

	// Token: 0x0400247E RID: 9342
	[FieldOffset(132)]
	public float scale;

	// Token: 0x0400247F RID: 9343
	[FieldOffset(136)]
	public int handPoseData;
}
