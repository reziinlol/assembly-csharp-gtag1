using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x020010AB RID: 4267
	public class StoreDisplay : MonoBehaviour
	{
		// Token: 0x06006B10 RID: 27408 RVA: 0x0022A57B File Offset: 0x0022877B
		private void GetAllDynamicCosmeticStands()
		{
			this.Stands = base.GetComponentsInChildren<DynamicCosmeticStand>();
		}

		// Token: 0x06006B11 RID: 27409 RVA: 0x0022A58C File Offset: 0x0022878C
		private void SetDisplayNameForAllStands()
		{
			DynamicCosmeticStand[] componentsInChildren = base.GetComponentsInChildren<DynamicCosmeticStand>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].CopyChildsName();
			}
		}

		// Token: 0x04007B32 RID: 31538
		public string displayName = "";

		// Token: 0x04007B33 RID: 31539
		public DynamicCosmeticStand[] Stands;
	}
}
