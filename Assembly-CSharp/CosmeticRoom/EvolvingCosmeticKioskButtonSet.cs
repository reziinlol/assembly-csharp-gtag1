using System;
using DefaultNamespace;
using GorillaNetworking;
using GorillaNetworking.Store;
using UnityEngine;

namespace CosmeticRoom
{
	// Token: 0x02000FFC RID: 4092
	public class EvolvingCosmeticKioskButtonSet : MonoBehaviour
	{
		// Token: 0x06006657 RID: 26199 RVA: 0x0020FC8A File Offset: 0x0020DE8A
		public void RegisterKiosk(EvolvingCosmeticKiosk kiosk)
		{
			if (this._kiosk != null)
			{
				throw new Exception("Attempted to double-register EvolvingCosmeticKiosk to a button.");
			}
			this._kiosk = kiosk;
		}

		// Token: 0x06006658 RID: 26200 RVA: 0x0020FCAC File Offset: 0x0020DEAC
		public void Reset()
		{
			this._cosmeticStand.ClearCosmetics();
			this._playfabId = null;
			this._cosmetic = null;
		}

		// Token: 0x06006659 RID: 26201 RVA: 0x0020FCC7 File Offset: 0x0020DEC7
		public void SetCosmetic(string playfabId, EvolvingCosmetic evolvingCosmetic)
		{
			this._cosmeticStand.SpawnItemOntoStand(playfabId);
			this._playfabId = playfabId;
			this._cosmetic = evolvingCosmetic;
		}

		// Token: 0x0600665A RID: 26202 RVA: 0x0020FCE3 File Offset: 0x0020DEE3
		public void GoForward()
		{
			if (this._cosmetic == null || !this._cosmetic.CanGoForward())
			{
				return;
			}
			this._cosmetic.GoForward();
			this.RefreshOnPlayer();
		}

		// Token: 0x0600665B RID: 26203 RVA: 0x0020FD12 File Offset: 0x0020DF12
		public void GoBackward()
		{
			if (this._cosmetic == null || !this._cosmetic.CanGoBack())
			{
				return;
			}
			this._cosmetic.GoBack();
			this.RefreshOnPlayer();
		}

		// Token: 0x0600665C RID: 26204 RVA: 0x0020FD44 File Offset: 0x0020DF44
		private void RefreshOnPlayer()
		{
			if (this._kiosk == null || this._playfabId == null || this._cosmetic == null)
			{
				return;
			}
			bool flag = false;
			CosmeticsController.CosmeticItem[] items = CosmeticsController.instance.currentWornSet.items;
			for (int i = 0; i < items.Length; i++)
			{
				if (!(items[i].itemName != this._playfabId))
				{
					CosmeticItemInstance cosmeticItemInstance = this._kiosk.VRRig.cosmeticsObjectRegistry.Cosmetic(this._playfabId);
					if (cosmeticItemInstance != null)
					{
						foreach (GameObject gameObject in cosmeticItemInstance.objects)
						{
							EvolvingCosmetic component = gameObject.GetComponent<EvolvingCosmetic>();
							if (component != null)
							{
								component.MatchStage(this._cosmetic);
								EvolvingCosmeticSaveData.Instance.SelectedIndices[component.PlayfabId] = component.SelectedObjectIndex;
								flag = true;
							}
						}
					}
				}
			}
			if (flag)
			{
				PlayerPrefs.SetString("EvolvingCosmeticSaveData", EvolvingCosmeticSaveData.Instance.Write());
			}
		}

		// Token: 0x040075D9 RID: 30169
		[SerializeField]
		private DynamicCosmeticStand _cosmeticStand;

		// Token: 0x040075DA RID: 30170
		[SerializeField]
		private GorillaPressableButton _plusButton;

		// Token: 0x040075DB RID: 30171
		[SerializeField]
		private GorillaPressableButton _minusButton;

		// Token: 0x040075DC RID: 30172
		private EvolvingCosmeticKiosk _kiosk;

		// Token: 0x040075DD RID: 30173
		private EvolvingCosmetic _cosmetic;

		// Token: 0x040075DE RID: 30174
		private string _playfabId;
	}
}
