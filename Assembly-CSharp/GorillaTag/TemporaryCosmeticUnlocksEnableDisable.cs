using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200115D RID: 4445
	public class TemporaryCosmeticUnlocksEnableDisable : MonoBehaviour
	{
		// Token: 0x06007083 RID: 28803 RVA: 0x0024ADFC File Offset: 0x00248FFC
		private void Awake()
		{
			if (this.m_wardrobe.IsNull() || this.m_cosmeticAreaTrigger.IsNull())
			{
				Debug.LogError("TemporaryCosmeticUnlocksEnableDisable: reference is null, disabling self");
				base.enabled = false;
			}
			if (CosmeticsController.instance.IsNull() || !this.m_wardrobe.WardrobeButtonsInitialized())
			{
				base.enabled = false;
				this.m_timer = new TickSystemTimer(0.05f, new Action(this.CheckWardrobeRady));
				this.m_timer.Start();
			}
		}

		// Token: 0x06007084 RID: 28804 RVA: 0x0024AE80 File Offset: 0x00249080
		private void OnEnable()
		{
			bool tempUnlocksEnabled = PlayerCosmeticsSystem.TempUnlocksEnabled;
			this.m_wardrobe.UseTemporarySet = tempUnlocksEnabled;
			this.m_cosmeticAreaTrigger.SetActive(tempUnlocksEnabled);
		}

		// Token: 0x06007085 RID: 28805 RVA: 0x0024AEAC File Offset: 0x002490AC
		private void CheckWardrobeRady()
		{
			if (CosmeticsController.instance.IsNotNull() && this.m_wardrobe.WardrobeButtonsInitialized())
			{
				this.m_timer.Stop();
				this.m_timer = null;
				base.enabled = true;
				return;
			}
			this.m_timer.Start();
		}

		// Token: 0x04008067 RID: 32871
		[SerializeField]
		private CosmeticWardrobe m_wardrobe;

		// Token: 0x04008068 RID: 32872
		[SerializeField]
		private GameObject m_cosmeticAreaTrigger;

		// Token: 0x04008069 RID: 32873
		private TickSystemTimer m_timer;
	}
}
