using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000373 RID: 883
[Serializable]
public struct GTRendererMatSlot
{
	// Token: 0x17000225 RID: 549
	// (get) Token: 0x060015A5 RID: 5541 RVA: 0x00072473 File Offset: 0x00070673
	// (set) Token: 0x060015A6 RID: 5542 RVA: 0x0007247B File Offset: 0x0007067B
	public bool isValid { readonly get; private set; }

	// Token: 0x060015A7 RID: 5543 RVA: 0x00072484 File Offset: 0x00070684
	public bool TryInitialize()
	{
		this.isValid = (this.renderer != null);
		if (!this.isValid)
		{
			return false;
		}
		List<Material> list;
		bool isValid;
		using (ListPool<Material>.Get(out list))
		{
			this.renderer.GetSharedMaterials(list);
			this.isValid = (this.slot >= 0 && this.slot < list.Count && list[this.slot] != null);
			isValid = this.isValid;
		}
		return isValid;
	}

	// Token: 0x04001A6D RID: 6765
	public Renderer renderer;

	// Token: 0x04001A6E RID: 6766
	public int slot;
}
