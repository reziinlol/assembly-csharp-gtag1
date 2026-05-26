using System;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class SIChargeDisplay : MonoBehaviour
{
	// Token: 0x060005E6 RID: 1510 RVA: 0x00022154 File Offset: 0x00020354
	public void UpdateDisplay(int chargeCount)
	{
		for (int i = 0; i < this.chargeDisplay.Length; i++)
		{
			this.chargeDisplay[i].material = ((i < chargeCount) ? this.chargedMat : this.unchargedMat);
		}
	}

	// Token: 0x0400076D RID: 1901
	[SerializeField]
	private MeshRenderer[] chargeDisplay;

	// Token: 0x0400076E RID: 1902
	[SerializeField]
	private Material chargedMat;

	// Token: 0x0400076F RID: 1903
	[SerializeField]
	private Material unchargedMat;
}
