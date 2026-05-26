using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GorillaTag
{
	// Token: 0x02001137 RID: 4407
	[Serializable]
	public class GTAssetRef<TObject> : AssetReferenceT<TObject> where TObject : Object
	{
		// Token: 0x06006FF1 RID: 28657 RVA: 0x00248C98 File Offset: 0x00246E98
		public GTAssetRef(string guid) : base(guid)
		{
		}
	}
}
