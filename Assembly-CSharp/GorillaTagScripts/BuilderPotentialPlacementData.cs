using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000EEE RID: 3822
	public struct BuilderPotentialPlacementData
	{
		// Token: 0x06005ED6 RID: 24278 RVA: 0x001E6F24 File Offset: 0x001E5124
		public BuilderPotentialPlacement ToPotentialPlacement(BuilderTable table)
		{
			BuilderPotentialPlacement builderPotentialPlacement = new BuilderPotentialPlacement
			{
				attachPiece = table.GetPiece(this.pieceId),
				parentPiece = table.GetPiece(this.parentPieceId),
				score = this.score,
				localPosition = this.localPosition,
				localRotation = this.localRotation,
				attachIndex = this.attachIndex,
				parentAttachIndex = this.parentAttachIndex,
				attachDistance = this.attachDistance,
				attachPlaneNormal = this.attachPlaneNormal,
				attachBounds = this.attachBounds,
				parentAttachBounds = this.parentAttachBounds,
				twist = this.twist,
				bumpOffsetX = this.bumpOffsetX,
				bumpOffsetZ = this.bumpOffsetZ
			};
			if (builderPotentialPlacement.parentPiece != null)
			{
				BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement.parentPiece.gridPlanes[builderPotentialPlacement.parentAttachIndex];
				builderPotentialPlacement.localPosition = builderAttachGridPlane.transform.InverseTransformPoint(builderPotentialPlacement.localPosition);
				builderPotentialPlacement.localRotation = Quaternion.Inverse(builderAttachGridPlane.transform.rotation) * builderPotentialPlacement.localRotation;
			}
			return builderPotentialPlacement;
		}

		// Token: 0x04006D8E RID: 28046
		public int pieceId;

		// Token: 0x04006D8F RID: 28047
		public int parentPieceId;

		// Token: 0x04006D90 RID: 28048
		public float score;

		// Token: 0x04006D91 RID: 28049
		public Vector3 localPosition;

		// Token: 0x04006D92 RID: 28050
		public Quaternion localRotation;

		// Token: 0x04006D93 RID: 28051
		public int attachIndex;

		// Token: 0x04006D94 RID: 28052
		public int parentAttachIndex;

		// Token: 0x04006D95 RID: 28053
		public float attachDistance;

		// Token: 0x04006D96 RID: 28054
		public Vector3 attachPlaneNormal;

		// Token: 0x04006D97 RID: 28055
		public SnapBounds attachBounds;

		// Token: 0x04006D98 RID: 28056
		public SnapBounds parentAttachBounds;

		// Token: 0x04006D99 RID: 28057
		public byte twist;

		// Token: 0x04006D9A RID: 28058
		public sbyte bumpOffsetX;

		// Token: 0x04006D9B RID: 28059
		public sbyte bumpOffsetZ;
	}
}
