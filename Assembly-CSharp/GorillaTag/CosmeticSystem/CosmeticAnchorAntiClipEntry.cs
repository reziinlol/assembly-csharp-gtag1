using System;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x020011C7 RID: 4551
	[Serializable]
	public struct CosmeticAnchorAntiClipEntry
	{
		// Token: 0x04008288 RID: 33416
		public bool enabled;

		// Token: 0x04008289 RID: 33417
		public XformOffset offset;

		// Token: 0x0400828A RID: 33418
		public static readonly CosmeticAnchorAntiClipEntry Identity = new CosmeticAnchorAntiClipEntry
		{
			offset = XformOffset.Identity
		};
	}
}
