using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x020011CC RID: 4556
	[Serializable]
	public struct CosmeticPart
	{
		// Token: 0x040082B8 RID: 33464
		public GTAssetRef<GameObject> prefabAssetRef;

		// Token: 0x040082B9 RID: 33465
		[Tooltip("Determines how the cosmetic part will be attached to the player.")]
		public CosmeticAttachInfo[] attachAnchors;

		// Token: 0x040082BA RID: 33466
		[NonSerialized]
		public ECosmeticPartType partType;
	}
}
