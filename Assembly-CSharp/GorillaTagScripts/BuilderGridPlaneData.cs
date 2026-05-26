using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000EEA RID: 3818
	public struct BuilderGridPlaneData
	{
		// Token: 0x06005ED3 RID: 24275 RVA: 0x001E6DB8 File Offset: 0x001E4FB8
		public BuilderGridPlaneData(BuilderAttachGridPlane gridPlane, int pieceIndex)
		{
			gridPlane.center.transform.GetPositionAndRotation(out this.position, out this.rotation);
			this.localPosition = gridPlane.pieceToGridPosition;
			this.localRotation = gridPlane.pieceToGridRotation;
			this.width = gridPlane.width;
			this.length = gridPlane.length;
			this.male = gridPlane.male;
			this.pieceId = gridPlane.piece.pieceId;
			this.pieceIndex = pieceIndex;
			this.boundingRadius = gridPlane.boundingRadius;
			this.attachIndex = gridPlane.attachIndex;
		}

		// Token: 0x04006D73 RID: 28019
		public int width;

		// Token: 0x04006D74 RID: 28020
		public int length;

		// Token: 0x04006D75 RID: 28021
		public bool male;

		// Token: 0x04006D76 RID: 28022
		public int pieceId;

		// Token: 0x04006D77 RID: 28023
		public int pieceIndex;

		// Token: 0x04006D78 RID: 28024
		public float boundingRadius;

		// Token: 0x04006D79 RID: 28025
		public int attachIndex;

		// Token: 0x04006D7A RID: 28026
		public Vector3 position;

		// Token: 0x04006D7B RID: 28027
		public Quaternion rotation;

		// Token: 0x04006D7C RID: 28028
		public Vector3 localPosition;

		// Token: 0x04006D7D RID: 28029
		public Quaternion localRotation;
	}
}
