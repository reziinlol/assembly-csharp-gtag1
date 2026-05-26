using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CosmeticRoom
{
	// Token: 0x02000FFE RID: 4094
	public class ItemCheckout : MonoBehaviour
	{
		// Token: 0x06006663 RID: 26211 RVA: 0x0020FF93 File Offset: 0x0020E193
		private void OnEnable()
		{
			if (this.addOnEnable)
			{
				CosmeticsController.instance.AddItemCheckout(this);
			}
		}

		// Token: 0x06006664 RID: 26212 RVA: 0x0020FFAA File Offset: 0x0020E1AA
		private void OnDisable()
		{
			if (this.addOnEnable)
			{
				CosmeticsController.instance.RemoveItemCheckout(this);
			}
		}

		// Token: 0x06006665 RID: 26213 RVA: 0x0020FFC4 File Offset: 0x0020E1C4
		public void InitializeForCustomMap(CompositeTriggerEvents customMapTryOnArea, Scene customMapScene, bool useCustomCounterMesh = true)
		{
			GameObject gameObject = this.checkoutCounterMesh;
			if (gameObject != null)
			{
				gameObject.SetActive(!useCustomCounterMesh);
			}
			GameObject gameObject2 = this.purchaseScreenMesh;
			if (gameObject2 != null)
			{
				gameObject2.SetActive(useCustomCounterMesh);
			}
			this.originalScene = customMapScene;
			customMapTryOnArea.AddCollider(this.checkoutTryOnArea);
			CosmeticsController.instance.AddItemCheckout(this);
		}

		// Token: 0x06006666 RID: 26214 RVA: 0x00210018 File Offset: 0x0020E218
		public void RemoveFromCustomMap(CompositeTriggerEvents customMapTryOnArea)
		{
			if (customMapTryOnArea.IsNull())
			{
				return;
			}
			customMapTryOnArea.RemoveCollider(this.checkoutTryOnArea);
		}

		// Token: 0x06006667 RID: 26215 RVA: 0x00210030 File Offset: 0x0020E230
		public void UpdateFromCart(List<CosmeticsController.CosmeticItem> currentCart, CosmeticsController.CosmeticItem itemToBuy)
		{
			this.iterator = 0;
			while (this.iterator < this.checkoutCartButtons.Length)
			{
				if (this.iterator < currentCart.Count)
				{
					bool isCurrentItemToBuy = currentCart[this.iterator].itemName == itemToBuy.itemName;
					this.checkoutCartButtons[this.iterator].SetItem(currentCart[this.iterator], isCurrentItemToBuy);
				}
				else
				{
					this.checkoutCartButtons[this.iterator].ClearItem();
				}
				this.iterator++;
			}
		}

		// Token: 0x06006668 RID: 26216 RVA: 0x002100C4 File Offset: 0x0020E2C4
		public void UpdatePurchaseText(string newText, string leftPurchaseButtonText, string rightPurchaseButtonText, bool leftButtonOn, bool rightButtonOn)
		{
			if (this.purchaseText.IsNotNull())
			{
				this.purchaseText.text = newText;
			}
			if (this.purchaseTextTMP.IsNotNull())
			{
				this.purchaseTextTMP.text = newText;
			}
			if (!leftPurchaseButtonText.IsNullOrEmpty())
			{
				this.leftPurchaseButton.SetText(leftPurchaseButtonText);
				this.leftPurchaseButton.buttonRenderer.material = (leftButtonOn ? this.leftPurchaseButton.pressedMaterial : this.leftPurchaseButton.unpressedMaterial);
			}
			if (!rightPurchaseButtonText.IsNullOrEmpty())
			{
				this.rightPurchaseButton.SetText(rightPurchaseButtonText);
				this.rightPurchaseButton.buttonRenderer.material = (rightButtonOn ? this.rightPurchaseButton.pressedMaterial : this.rightPurchaseButton.unpressedMaterial);
			}
		}

		// Token: 0x06006669 RID: 26217 RVA: 0x00210183 File Offset: 0x0020E383
		public bool IsFromScene(Scene unloadingScene)
		{
			return unloadingScene == this.originalScene;
		}

		// Token: 0x040075E3 RID: 30179
		public CheckoutCartButton[] checkoutCartButtons;

		// Token: 0x040075E4 RID: 30180
		public PurchaseItemButton leftPurchaseButton;

		// Token: 0x040075E5 RID: 30181
		public PurchaseItemButton rightPurchaseButton;

		// Token: 0x040075E6 RID: 30182
		[HideInInspector]
		public Text purchaseText;

		// Token: 0x040075E7 RID: 30183
		public TMP_Text purchaseTextTMP;

		// Token: 0x040075E8 RID: 30184
		public HeadModel checkoutHeadModel;

		// Token: 0x040075E9 RID: 30185
		public Collider checkoutTryOnArea;

		// Token: 0x040075EA RID: 30186
		public GameObject checkoutCounterMesh;

		// Token: 0x040075EB RID: 30187
		public GameObject purchaseScreenMesh;

		// Token: 0x040075EC RID: 30188
		private Scene originalScene;

		// Token: 0x040075ED RID: 30189
		private int iterator;

		// Token: 0x040075EE RID: 30190
		public bool addOnEnable;
	}
}
