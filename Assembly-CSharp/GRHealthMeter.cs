using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007A8 RID: 1960
public class GRHealthMeter : MonoBehaviour
{
	// Token: 0x0600320F RID: 12815 RVA: 0x001131A3 File Offset: 0x001113A3
	public void Setup(int maxHP)
	{
		this.maxHP = maxHP;
	}

	// Token: 0x06003210 RID: 12816 RVA: 0x001131AC File Offset: 0x001113AC
	public void SetHP(int hp)
	{
		int num = Mathf.CeilToInt((float)hp / (float)this.maxHP * (float)this.nodes.Count);
		for (int i = 0; i < this.nodes.Count; i++)
		{
			this.nodes[i].SetEmpty(i >= num);
		}
	}

	// Token: 0x04004100 RID: 16640
	public List<GRHealthMeterNode> nodes;

	// Token: 0x04004101 RID: 16641
	private int maxHP;
}
