using System;
using UnityEngine;

// Token: 0x02000D1D RID: 3357
public class TrickTreatItem : RandomComponent<MeshRenderer>
{
	// Token: 0x060052E6 RID: 21222 RVA: 0x001B2BF8 File Offset: 0x001B0DF8
	protected override void OnNextItem(MeshRenderer item)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			MeshRenderer meshRenderer = this.items[i];
			meshRenderer.enabled = (meshRenderer == item);
		}
	}

	// Token: 0x060052E7 RID: 21223 RVA: 0x001B2C2C File Offset: 0x001B0E2C
	public void Randomize()
	{
		this.NextItem();
	}
}
