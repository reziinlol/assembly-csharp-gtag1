using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

namespace CosmeticRoom
{
	// Token: 0x02000FFD RID: 4093
	public class FittingRoom : MonoBehaviour
	{
		// Token: 0x0600665E RID: 26206 RVA: 0x0020FE68 File Offset: 0x0020E068
		public void InitializeForCustomMap(bool useCustomConsoleMesh = true)
		{
			GameObject gameObject = this.consoleMesh;
			if (gameObject != null)
			{
				gameObject.SetActive(!useCustomConsoleMesh);
			}
			CosmeticsController.instance.AddFittingRoom(this);
		}

		// Token: 0x0600665F RID: 26207 RVA: 0x0020FE8C File Offset: 0x0020E08C
		private void OnEnable()
		{
			if (this.addOnEnable)
			{
				CosmeticsController.instance.AddFittingRoom(this);
			}
		}

		// Token: 0x06006660 RID: 26208 RVA: 0x0020FEA3 File Offset: 0x0020E0A3
		private void OnDisable()
		{
			if (this.addOnEnable)
			{
				CosmeticsController.instance.RemoveFittingRoom(this);
			}
		}

		// Token: 0x06006661 RID: 26209 RVA: 0x0020FEBC File Offset: 0x0020E0BC
		public void UpdateFromCart(List<CosmeticsController.CosmeticItem> currentCart, CosmeticsController.CosmeticSet tryOnSet)
		{
			this.iterator = 0;
			while (this.iterator < this.fittingRoomButtons.Length)
			{
				if (this.iterator < currentCart.Count)
				{
					bool isInTryOnSet = CosmeticsController.instance.AnyMatch(tryOnSet, currentCart[this.iterator]) || (!CosmeticsController.instance.tryOnCollectableItem.isNullItem && currentCart[this.iterator].itemName == CosmeticsController.instance.tryOnCollectableItem.itemName);
					this.fittingRoomButtons[this.iterator].SetItem(currentCart[this.iterator], isInTryOnSet);
				}
				else
				{
					this.fittingRoomButtons[this.iterator].ClearItem();
				}
				this.iterator++;
			}
		}

		// Token: 0x040075DF RID: 30175
		public FittingRoomButton[] fittingRoomButtons;

		// Token: 0x040075E0 RID: 30176
		public GameObject consoleMesh;

		// Token: 0x040075E1 RID: 30177
		private int iterator;

		// Token: 0x040075E2 RID: 30178
		public bool addOnEnable;
	}
}
