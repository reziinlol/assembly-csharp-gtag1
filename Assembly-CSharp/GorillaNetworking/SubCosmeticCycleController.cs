using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02001040 RID: 4160
	public class SubCosmeticCycleController : MonoBehaviour
	{
		// Token: 0x170009CD RID: 2509
		// (get) Token: 0x0600680B RID: 26635 RVA: 0x002193A3 File Offset: 0x002175A3
		private CosmeticCollectionDisplay Display
		{
			get
			{
				if (this.display == null)
				{
					this.display = base.GetComponentInChildren<CosmeticCollectionDisplay>();
				}
				return this.display;
			}
		}

		// Token: 0x170009CE RID: 2510
		// (get) Token: 0x0600680C RID: 26636 RVA: 0x002193C8 File Offset: 0x002175C8
		public CosmeticsController.CosmeticItem? ActiveCollectable
		{
			get
			{
				CosmeticCollectionDisplay cosmeticCollectionDisplay = this.Display;
				if (cosmeticCollectionDisplay == null)
				{
					return null;
				}
				return cosmeticCollectionDisplay.ActiveCollectable;
			}
		}

		// Token: 0x170009CF RID: 2511
		// (get) Token: 0x0600680D RID: 26637 RVA: 0x002193EE File Offset: 0x002175EE
		public int ActiveIndex
		{
			get
			{
				CosmeticCollectionDisplay cosmeticCollectionDisplay = this.Display;
				if (cosmeticCollectionDisplay == null)
				{
					return 0;
				}
				return cosmeticCollectionDisplay.ActiveIndex;
			}
		}

		// Token: 0x170009D0 RID: 2512
		// (get) Token: 0x0600680E RID: 26638 RVA: 0x00219401 File Offset: 0x00217601
		public int Count
		{
			get
			{
				CosmeticCollectionDisplay cosmeticCollectionDisplay = this.Display;
				if (cosmeticCollectionDisplay == null)
				{
					return 0;
				}
				return cosmeticCollectionDisplay.Count;
			}
		}

		// Token: 0x0600680F RID: 26639 RVA: 0x00219414 File Offset: 0x00217614
		public string GetAppliedCosmeticID()
		{
			CosmeticsController.CosmeticItem? cosmeticItem;
			return ((this.ActiveCollectable != null) ? cosmeticItem.GetValueOrDefault().appliedCosmeticPlayFabID : null) ?? string.Empty;
		}

		// Token: 0x06006810 RID: 26640 RVA: 0x0021944C File Offset: 0x0021764C
		public void CycleForward()
		{
			if (this.Display == null || this.Display.Count <= 1)
			{
				return;
			}
			int num = (this.Display.ActiveIndex + 1) % this.Display.Count;
			this.Display.SetActiveIndex(num);
			this.SendCycleRPC(num);
		}

		// Token: 0x06006811 RID: 26641 RVA: 0x002194A4 File Offset: 0x002176A4
		public void CycleBackward()
		{
			if (this.Display == null || this.Display.Count <= 1)
			{
				return;
			}
			int num = (this.Display.ActiveIndex - 1 + this.Display.Count) % this.Display.Count;
			this.Display.SetActiveIndex(num);
			this.SendCycleRPC(num);
		}

		// Token: 0x06006812 RID: 26642 RVA: 0x00219508 File Offset: 0x00217708
		public void CycleRandom()
		{
			if (this.Display == null || this.Display.Count <= 1)
			{
				return;
			}
			int num;
			do
			{
				num = Random.Range(0, this.Display.Count);
			}
			while (num == this.Display.ActiveIndex);
			this.Display.SetActiveIndex(num);
			this.SendCycleRPC(num);
		}

		// Token: 0x06006813 RID: 26643 RVA: 0x00219565 File Offset: 0x00217765
		public void SetIndex(int index)
		{
			CosmeticCollectionDisplay cosmeticCollectionDisplay = this.Display;
			if (cosmeticCollectionDisplay != null)
			{
				cosmeticCollectionDisplay.SetActiveIndex(index);
			}
			this.SendCycleRPC(index);
		}

		// Token: 0x06006814 RID: 26644 RVA: 0x00219580 File Offset: 0x00217780
		public void SetDisplayVisible(bool visible)
		{
			CosmeticCollectionDisplay cosmeticCollectionDisplay = this.Display;
			if (cosmeticCollectionDisplay == null)
			{
				return;
			}
			cosmeticCollectionDisplay.SetVisible(visible);
		}

		// Token: 0x06006815 RID: 26645 RVA: 0x00219594 File Offset: 0x00217794
		private void SendCycleRPC(int newIndex)
		{
			if (!this.syncCycleOverNetwork)
			{
				return;
			}
			CosmeticCollectionDisplay cosmeticCollectionDisplay = this.Display;
			string text = (cosmeticCollectionDisplay != null) ? cosmeticCollectionDisplay.ParentPlayFabID : null;
			if (string.IsNullOrEmpty(text) || text.Length < 5)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			int num = (int)(text[0] - 'A' + '\u001a' * (text[1] - 'A' + '\u001a' * (text[2] - 'A' + '\u001a' * (text[3] - 'A' + '\u001a' * (text[4] - 'A')))));
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_SetCollectionCycleIndex", RpcTarget.Others, new object[]
			{
				new int[]
				{
					num,
					newIndex
				}
			});
		}

		// Token: 0x04007765 RID: 30565
		[SerializeField]
		private bool syncCycleOverNetwork = true;

		// Token: 0x04007766 RID: 30566
		private CosmeticCollectionDisplay display;
	}
}
