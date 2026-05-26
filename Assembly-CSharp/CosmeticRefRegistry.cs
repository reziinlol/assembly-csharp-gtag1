using System;
using UnityEngine;

// Token: 0x020002B2 RID: 690
public class CosmeticRefRegistry : MonoBehaviour
{
	// Token: 0x060011DC RID: 4572 RVA: 0x0005FC64 File Offset: 0x0005DE64
	private void Awake()
	{
		foreach (CosmeticRefTarget cosmeticRefTarget in this.builtInRefTargets)
		{
			this.Register(cosmeticRefTarget.id, cosmeticRefTarget.gameObject);
		}
	}

	// Token: 0x060011DD RID: 4573 RVA: 0x0005FC9C File Offset: 0x0005DE9C
	public void Register(CosmeticRefID partID, GameObject part)
	{
		this.partsTable[(int)partID] = part;
	}

	// Token: 0x060011DE RID: 4574 RVA: 0x0005FCA7 File Offset: 0x0005DEA7
	public GameObject Get(CosmeticRefID partID)
	{
		return this.partsTable[(int)partID];
	}

	// Token: 0x04001583 RID: 5507
	private GameObject[] partsTable = new GameObject[9];

	// Token: 0x04001584 RID: 5508
	[SerializeField]
	private CosmeticRefTarget[] builtInRefTargets;
}
