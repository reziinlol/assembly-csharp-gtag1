using System;
using UnityEngine;

// Token: 0x0200075B RID: 1883
public class GRDistilleryDeposit : MonoBehaviour
{
	// Token: 0x06002FC0 RID: 12224 RVA: 0x00103750 File Offset: 0x00101950
	private void Start()
	{
		this._distillery = base.GetComponentInParent<GRDistillery>();
	}

	// Token: 0x06002FC1 RID: 12225 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnTriggerEnter(Collider other)
	{
	}

	// Token: 0x04003D2F RID: 15663
	public float hapticStrength;

	// Token: 0x04003D30 RID: 15664
	public float hapticDuration;

	// Token: 0x04003D31 RID: 15665
	private GRDistillery _distillery;
}
