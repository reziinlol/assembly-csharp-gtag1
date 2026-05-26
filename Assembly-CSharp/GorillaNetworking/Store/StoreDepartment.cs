using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x020010AA RID: 4266
	public class StoreDepartment : MonoBehaviour
	{
		// Token: 0x06006B0E RID: 27406 RVA: 0x0022A4F8 File Offset: 0x002286F8
		private void FindAllDisplays()
		{
			this.Displays = base.GetComponentsInChildren<StoreDisplay>();
			for (int i = this.Displays.Length - 1; i >= 0; i--)
			{
				if (string.IsNullOrEmpty(this.Displays[i].displayName))
				{
					this.Displays[i] = this.Displays[this.Displays.Length - 1];
					Array.Resize<StoreDisplay>(ref this.Displays, this.Displays.Length - 1);
				}
			}
		}

		// Token: 0x04007B30 RID: 31536
		public StoreDisplay[] Displays;

		// Token: 0x04007B31 RID: 31537
		public string departmentName = "";
	}
}
