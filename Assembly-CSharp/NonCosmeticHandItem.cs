using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000486 RID: 1158
public class NonCosmeticHandItem : MonoBehaviour
{
	// Token: 0x06001C31 RID: 7217 RVA: 0x00098C0F File Offset: 0x00096E0F
	public void EnableItem(bool enable)
	{
		if (this.itemPrefab)
		{
			this.itemPrefab.gameObject.SetActive(enable);
		}
	}

	// Token: 0x17000307 RID: 775
	// (get) Token: 0x06001C32 RID: 7218 RVA: 0x00098C2F File Offset: 0x00096E2F
	public bool IsEnabled
	{
		get
		{
			return this.itemPrefab && this.itemPrefab.gameObject.activeSelf;
		}
	}

	// Token: 0x04002642 RID: 9794
	public CosmeticsController.CosmeticSlots cosmeticSlots;

	// Token: 0x04002643 RID: 9795
	public GameObject itemPrefab;
}
