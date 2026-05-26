using System;
using Cosmetics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GorillaNetworking.Store
{
	// Token: 0x0200109B RID: 4251
	public class BundleStand : MonoBehaviour, IBuildValidation
	{
		// Token: 0x17000A06 RID: 2566
		// (get) Token: 0x06006A99 RID: 27289 RVA: 0x00227333 File Offset: 0x00225533
		public string playfabBundleID
		{
			get
			{
				return this._bundleDataReference.playfabBundleID;
			}
		}

		// Token: 0x06006A9A RID: 27290 RVA: 0x00227340 File Offset: 0x00225540
		bool IBuildValidation.BuildValidationCheck()
		{
			ICreatorCodeProvider creatorCodeProvider;
			if (this.creatorCodeProvider == null || !this.creatorCodeProvider.TryGetComponent<ICreatorCodeProvider>(out creatorCodeProvider))
			{
				Debug.LogError(base.name + " has no Creator Code Provider. This will break bundle purchasing.");
				return false;
			}
			return true;
		}

		// Token: 0x06006A9B RID: 27291 RVA: 0x00227384 File Offset: 0x00225584
		public void Awake()
		{
			this._bundlePurchaseButton.playfabID = this.playfabBundleID;
			if (this._bundleIcon != null && this._bundleDataReference != null && this._bundleDataReference.bundleImage != null)
			{
				this._bundleIcon.sprite = this._bundleDataReference.bundleImage;
			}
			this._bundlePurchaseButton.codeProvider = this.creatorCodeProvider.GetComponent<ICreatorCodeProvider>();
		}

		// Token: 0x06006A9C RID: 27292 RVA: 0x002273FD File Offset: 0x002255FD
		public void InitializeEventListeners()
		{
			this.AlreadyOwnEvent.AddListener(new UnityAction(this._bundlePurchaseButton.AlreadyOwn));
			this.ErrorHappenedEvent.AddListener(new UnityAction(this._bundlePurchaseButton.ErrorHappened));
		}

		// Token: 0x06006A9D RID: 27293 RVA: 0x00227437 File Offset: 0x00225637
		public void NotifyAlreadyOwn()
		{
			this.AlreadyOwnEvent.Invoke();
		}

		// Token: 0x06006A9E RID: 27294 RVA: 0x00227444 File Offset: 0x00225644
		public void ErrorHappened()
		{
			this.ErrorHappenedEvent.Invoke();
		}

		// Token: 0x06006A9F RID: 27295 RVA: 0x00227451 File Offset: 0x00225651
		public void UpdatePurchaseButtonText(string purchaseText)
		{
			if (this._bundlePurchaseButton != null)
			{
				this._bundlePurchaseButton.UpdatePurchaseButtonText(purchaseText);
			}
		}

		// Token: 0x06006AA0 RID: 27296 RVA: 0x0022746D File Offset: 0x0022566D
		public void UpdateDescriptionText(string descriptionText)
		{
			if (this._bundleDescriptionText != null)
			{
				this._bundleDescriptionText.text = descriptionText;
			}
		}

		// Token: 0x04007ACC RID: 31436
		public BundlePurchaseButton _bundlePurchaseButton;

		// Token: 0x04007ACD RID: 31437
		[SerializeField]
		public StoreBundleData _bundleDataReference;

		// Token: 0x04007ACE RID: 31438
		[SerializeField]
		private GameObject creatorCodeProvider;

		// Token: 0x04007ACF RID: 31439
		public GameObject[] EditorOnlyObjects;

		// Token: 0x04007AD0 RID: 31440
		public Text _bundleDescriptionText;

		// Token: 0x04007AD1 RID: 31441
		public Image _bundleIcon;

		// Token: 0x04007AD2 RID: 31442
		public UnityEvent AlreadyOwnEvent;

		// Token: 0x04007AD3 RID: 31443
		public UnityEvent ErrorHappenedEvent;
	}
}
