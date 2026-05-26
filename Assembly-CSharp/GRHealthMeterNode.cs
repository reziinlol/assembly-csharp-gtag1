using System;
using UnityEngine;

// Token: 0x020007A9 RID: 1961
public class GRHealthMeterNode : MonoBehaviour
{
	// Token: 0x06003212 RID: 12818 RVA: 0x00113204 File Offset: 0x00111404
	public void Setup()
	{
		this.isEmpty = true;
		this.SetEmpty(false);
	}

	// Token: 0x06003213 RID: 12819 RVA: 0x00113214 File Offset: 0x00111414
	public void SetEmpty(bool empty)
	{
		if (this.isEmpty == empty)
		{
			return;
		}
		this.isEmpty = empty;
		if (this.showFull != null)
		{
			this.showFull.SetActive(!this.isEmpty);
		}
		if (this.showEmpty != null)
		{
			this.showEmpty.SetActive(this.isEmpty);
		}
	}

	// Token: 0x04004102 RID: 16642
	public GameObject showFull;

	// Token: 0x04004103 RID: 16643
	public GameObject showEmpty;

	// Token: 0x04004104 RID: 16644
	private bool isEmpty;
}
