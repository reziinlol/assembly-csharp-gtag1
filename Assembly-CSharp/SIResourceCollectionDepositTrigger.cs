using System;
using UnityEngine;

// Token: 0x0200015B RID: 347
public class SIResourceCollectionDepositTrigger : MonoBehaviour
{
	// Token: 0x06000931 RID: 2353 RVA: 0x00031B66 File Offset: 0x0002FD66
	private void Awake()
	{
		this.resourceDeposit = this.parentCollection.GetComponent<ISIResourceDeposit>();
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x00031B7C File Offset: 0x0002FD7C
	private void OnTriggerEnter(Collider other)
	{
		SIResource componentInParent = other.GetComponentInParent<SIResource>();
		if (componentInParent == null)
		{
			return;
		}
		if (componentInParent.CanDeposit())
		{
			this.resourceDeposit.ResourceDeposited(componentInParent);
		}
	}

	// Token: 0x04000B46 RID: 2886
	public GameObject parentCollection;

	// Token: 0x04000B47 RID: 2887
	private ISIResourceDeposit resourceDeposit;
}
