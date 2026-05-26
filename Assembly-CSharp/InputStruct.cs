using System;
using System.Runtime.InteropServices;
using Fusion;
using UnityEngine;

// Token: 0x0200044F RID: 1103
[NetworkStructWeaved(42)]
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 168)]
public struct InputStruct : INetworkStruct
{
	// Token: 0x04002509 RID: 9481
	[FieldOffset(0)]
	public int headRotation;

	// Token: 0x0400250A RID: 9482
	[FieldOffset(4)]
	public bool usingNewIK;

	// Token: 0x0400250B RID: 9483
	[FieldOffset(8)]
	public int bodyRotation;

	// Token: 0x0400250C RID: 9484
	[FieldOffset(12)]
	public short leftUpperArmRotation;

	// Token: 0x0400250D RID: 9485
	[FieldOffset(16)]
	public short rightUpperArmRotation;

	// Token: 0x0400250E RID: 9486
	[FieldOffset(20)]
	public long rightHandLong;

	// Token: 0x0400250F RID: 9487
	[FieldOffset(28)]
	public long leftHandLong;

	// Token: 0x04002510 RID: 9488
	[FieldOffset(36)]
	public long position;

	// Token: 0x04002511 RID: 9489
	[FieldOffset(44)]
	public int handPosition;

	// Token: 0x04002512 RID: 9490
	[FieldOffset(48)]
	public int rotation;

	// Token: 0x04002513 RID: 9491
	[FieldOffset(52)]
	public int packedFields;

	// Token: 0x04002514 RID: 9492
	[FieldOffset(56)]
	public short packedCompetitiveData;

	// Token: 0x04002515 RID: 9493
	[FieldOffset(60)]
	public Vector3 velocity;

	// Token: 0x04002516 RID: 9494
	[FieldOffset(72)]
	public int grabbedRopeIndex;

	// Token: 0x04002517 RID: 9495
	[FieldOffset(76)]
	public int ropeBoneIndex;

	// Token: 0x04002518 RID: 9496
	[FieldOffset(80)]
	public bool ropeGrabIsLeft;

	// Token: 0x04002519 RID: 9497
	[FieldOffset(84)]
	public bool ropeGrabIsBody;

	// Token: 0x0400251A RID: 9498
	[FieldOffset(88)]
	public Vector3 ropeGrabOffset;

	// Token: 0x0400251B RID: 9499
	[FieldOffset(100)]
	public bool movingSurfaceIsMonkeBlock;

	// Token: 0x0400251C RID: 9500
	[FieldOffset(104)]
	public long hoverboardPosRot;

	// Token: 0x0400251D RID: 9501
	[FieldOffset(112)]
	public short hoverboardColor;

	// Token: 0x0400251E RID: 9502
	[FieldOffset(116)]
	public long propHuntPosRot;

	// Token: 0x0400251F RID: 9503
	[FieldOffset(124)]
	public double serverTimeStamp;

	// Token: 0x04002520 RID: 9504
	[FieldOffset(132)]
	public short taggedById;

	// Token: 0x04002521 RID: 9505
	[FieldOffset(136)]
	public bool isGroundedHand;

	// Token: 0x04002522 RID: 9506
	[FieldOffset(140)]
	public bool isGroundedButt;

	// Token: 0x04002523 RID: 9507
	[FieldOffset(144)]
	public int leftHandGrabbedActorNumber;

	// Token: 0x04002524 RID: 9508
	[FieldOffset(148)]
	public bool leftGrabbedHandIsLeft;

	// Token: 0x04002525 RID: 9509
	[FieldOffset(152)]
	public int rightHandGrabbedActorNumber;

	// Token: 0x04002526 RID: 9510
	[FieldOffset(156)]
	public bool rightGrabbedHandIsLeft;

	// Token: 0x04002527 RID: 9511
	[FieldOffset(160)]
	public float lastTouchedGroundAtTime;

	// Token: 0x04002528 RID: 9512
	[FieldOffset(164)]
	public float lastHandTouchedGroundAtTime;
}
