using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000503 RID: 1283
public class CosmeticsControllerUpdateStand : MonoBehaviour
{
	// Token: 0x06002016 RID: 8214 RVA: 0x000AC87C File Offset: 0x000AAA7C
	public GameObject ReturnChildWithCosmeticNameMatch(Transform parentTransform)
	{
		GameObject gameObject = null;
		using (IEnumerator enumerator = parentTransform.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Transform child = (Transform)enumerator.Current;
				if (child.gameObject.activeInHierarchy && this.cosmeticsController.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => child.name == x.itemName) > -1)
				{
					return child.gameObject;
				}
				gameObject = this.ReturnChildWithCosmeticNameMatch(child);
				if (gameObject != null)
				{
					return gameObject;
				}
			}
		}
		return gameObject;
	}

	// Token: 0x04002AD8 RID: 10968
	public CosmeticsController cosmeticsController;

	// Token: 0x04002AD9 RID: 10969
	public bool FailEntitlement;

	// Token: 0x04002ADA RID: 10970
	public bool PlayerUnlocked;

	// Token: 0x04002ADB RID: 10971
	public bool ItemNotGrantedYet;

	// Token: 0x04002ADC RID: 10972
	public bool ItemSuccessfullyGranted;

	// Token: 0x04002ADD RID: 10973
	public bool AttemptToConsumeEntitlement;

	// Token: 0x04002ADE RID: 10974
	public bool EntitlementSuccessfullyConsumed;

	// Token: 0x04002ADF RID: 10975
	public bool LockSuccessfullyCleared;

	// Token: 0x04002AE0 RID: 10976
	public bool RunDebug;

	// Token: 0x04002AE1 RID: 10977
	public Transform textParent;

	// Token: 0x04002AE2 RID: 10978
	private CosmeticsController.CosmeticItem outItem;

	// Token: 0x04002AE3 RID: 10979
	public HeadModel[] inventoryHeadModels;

	// Token: 0x04002AE4 RID: 10980
	public string headModelsPrefabPath;
}
