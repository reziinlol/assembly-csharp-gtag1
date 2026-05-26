using System;
using UnityEngine.Serialization;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x020011C5 RID: 4549
	[Serializable]
	public struct CosmeticAnchorAntiIntersectOffsets
	{
		// Token: 0x04008274 RID: 33396
		public CosmeticAnchorAntiClipEntry nameTag;

		// Token: 0x04008275 RID: 33397
		public CosmeticAnchorAntiClipEntry leftArm;

		// Token: 0x04008276 RID: 33398
		public CosmeticAnchorAntiClipEntry rightArm;

		// Token: 0x04008277 RID: 33399
		public CosmeticAnchorAntiClipEntry chest;

		// Token: 0x04008278 RID: 33400
		public CosmeticAnchorAntiClipEntry huntComputer;

		// Token: 0x04008279 RID: 33401
		public CosmeticAnchorAntiClipEntry badge;

		// Token: 0x0400827A RID: 33402
		public CosmeticAnchorAntiClipEntry builderWatch;

		// Token: 0x0400827B RID: 33403
		public CosmeticAnchorAntiClipEntry friendshipBraceletLeft;

		// Token: 0x0400827C RID: 33404
		[FormerlySerializedAs("friendshipBradceletRight")]
		public CosmeticAnchorAntiClipEntry friendshipBraceletRight;

		// Token: 0x0400827D RID: 33405
		public static readonly CosmeticAnchorAntiIntersectOffsets Identity = new CosmeticAnchorAntiIntersectOffsets
		{
			nameTag = CosmeticAnchorAntiClipEntry.Identity,
			leftArm = CosmeticAnchorAntiClipEntry.Identity,
			rightArm = CosmeticAnchorAntiClipEntry.Identity,
			chest = CosmeticAnchorAntiClipEntry.Identity,
			huntComputer = CosmeticAnchorAntiClipEntry.Identity,
			badge = CosmeticAnchorAntiClipEntry.Identity,
			builderWatch = CosmeticAnchorAntiClipEntry.Identity
		};
	}
}
