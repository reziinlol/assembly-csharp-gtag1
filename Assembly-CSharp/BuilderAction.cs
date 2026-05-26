using System;
using UnityEngine;

// Token: 0x02000615 RID: 1557
public struct BuilderAction
{
	// Token: 0x04003237 RID: 12855
	public BuilderActionType type;

	// Token: 0x04003238 RID: 12856
	public int pieceId;

	// Token: 0x04003239 RID: 12857
	public int parentPieceId;

	// Token: 0x0400323A RID: 12858
	public Vector3 localPosition;

	// Token: 0x0400323B RID: 12859
	public Quaternion localRotation;

	// Token: 0x0400323C RID: 12860
	public byte twist;

	// Token: 0x0400323D RID: 12861
	public sbyte bumpOffsetx;

	// Token: 0x0400323E RID: 12862
	public sbyte bumpOffsetz;

	// Token: 0x0400323F RID: 12863
	public bool isLeftHand;

	// Token: 0x04003240 RID: 12864
	public int playerActorNumber;

	// Token: 0x04003241 RID: 12865
	public int parentAttachIndex;

	// Token: 0x04003242 RID: 12866
	public int attachIndex;

	// Token: 0x04003243 RID: 12867
	public SnapBounds attachBounds;

	// Token: 0x04003244 RID: 12868
	public SnapBounds parentAttachBounds;

	// Token: 0x04003245 RID: 12869
	public Vector3 velocity;

	// Token: 0x04003246 RID: 12870
	public Vector3 angVelocity;

	// Token: 0x04003247 RID: 12871
	public int localCommandId;

	// Token: 0x04003248 RID: 12872
	public int timeStamp;
}
