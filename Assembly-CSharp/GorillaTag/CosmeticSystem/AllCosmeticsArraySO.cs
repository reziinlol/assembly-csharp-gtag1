using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x020011C4 RID: 4548
	public class AllCosmeticsArraySO : ScriptableObject
	{
		// Token: 0x060072B2 RID: 29362 RVA: 0x00255244 File Offset: 0x00253444
		public CosmeticSO SearchForCosmeticSO(string playfabId)
		{
			GTDirectAssetRef<CosmeticSO>[] array = this.sturdyAssetRefs;
			for (int i = 0; i < array.Length; i++)
			{
				CosmeticSO cosmeticSO = array[i];
				if (cosmeticSO.info.playFabID == playfabId)
				{
					return cosmeticSO;
				}
			}
			Debug.LogWarning("AllCosmeticsArraySO - SearchForCosmeticSO - No Cosmetic found with playfabId: " + playfabId, this);
			return null;
		}

		// Token: 0x04008273 RID: 33395
		[SerializeField]
		public GTDirectAssetRef<CosmeticSO>[] sturdyAssetRefs;
	}
}
