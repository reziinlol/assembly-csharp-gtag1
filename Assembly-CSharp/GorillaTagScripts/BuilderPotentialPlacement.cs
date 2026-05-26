using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000EDE RID: 3806
	public struct BuilderPotentialPlacement
	{
		// Token: 0x06005DD4 RID: 24020 RVA: 0x001DC518 File Offset: 0x001DA718
		public void Reset()
		{
			this.attachPiece = null;
			this.parentPiece = null;
			this.attachIndex = -1;
			this.parentAttachIndex = -1;
			this.localPosition = Vector3.zero;
			this.localRotation = Quaternion.identity;
			this.attachDistance = float.MaxValue;
			this.attachPlaneNormal = Vector3.zero;
			this.score = float.MinValue;
			this.twist = 0;
			this.bumpOffsetX = 0;
			this.bumpOffsetZ = 0;
		}

		// Token: 0x04006C73 RID: 27763
		public BuilderPiece attachPiece;

		// Token: 0x04006C74 RID: 27764
		public BuilderPiece parentPiece;

		// Token: 0x04006C75 RID: 27765
		public int attachIndex;

		// Token: 0x04006C76 RID: 27766
		public int parentAttachIndex;

		// Token: 0x04006C77 RID: 27767
		public Vector3 localPosition;

		// Token: 0x04006C78 RID: 27768
		public Quaternion localRotation;

		// Token: 0x04006C79 RID: 27769
		public Vector3 attachPlaneNormal;

		// Token: 0x04006C7A RID: 27770
		public float attachDistance;

		// Token: 0x04006C7B RID: 27771
		public float score;

		// Token: 0x04006C7C RID: 27772
		public SnapBounds attachBounds;

		// Token: 0x04006C7D RID: 27773
		public SnapBounds parentAttachBounds;

		// Token: 0x04006C7E RID: 27774
		public byte twist;

		// Token: 0x04006C7F RID: 27775
		public sbyte bumpOffsetX;

		// Token: 0x04006C80 RID: 27776
		public sbyte bumpOffsetZ;
	}
}
