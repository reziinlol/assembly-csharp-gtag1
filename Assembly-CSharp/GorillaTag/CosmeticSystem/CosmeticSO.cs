using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x020011CF RID: 4559
	[CreateAssetMenu(fileName = "Untitled_CosmeticSO", menuName = "- Gorilla Tag/CosmeticSO", order = 0)]
	public class CosmeticSO : ScriptableObject
	{
		// Token: 0x060072C2 RID: 29378 RVA: 0x00023994 File Offset: 0x00021B94
		private bool ShowPropHuntWeight()
		{
			return true;
		}

		// Token: 0x060072C3 RID: 29379 RVA: 0x00255613 File Offset: 0x00253813
		public void OnEnable()
		{
			this.info.debugCosmeticSOName = base.name;
		}

		// Token: 0x040082BF RID: 33471
		public CosmeticInfoV2 info = new CosmeticInfoV2("UNNAMED");

		// Token: 0x040082C0 RID: 33472
		public int propHuntWeight = 1;
	}
}
