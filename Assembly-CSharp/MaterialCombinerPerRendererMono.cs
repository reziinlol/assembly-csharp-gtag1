using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000381 RID: 897
public class MaterialCombinerPerRendererMono : MonoBehaviour
{
	// Token: 0x060015CD RID: 5581 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected void Awake()
	{
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x00073BA0 File Offset: 0x00071DA0
	public void AddEntry(Renderer r, int slot, int sliceIndex, Color baseColor, Material oldMat)
	{
		this.slotData.Add(new MaterialCombinerPerRendererInfo
		{
			renderer = r,
			slotIndex = slot,
			sliceIndex = sliceIndex,
			baseColor = baseColor,
			oldMat = oldMat
		});
	}

	// Token: 0x060015CF RID: 5583 RVA: 0x00073BEC File Offset: 0x00071DEC
	public bool TryGetData(Renderer r, int slot, out MaterialCombinerPerRendererInfo data)
	{
		foreach (MaterialCombinerPerRendererInfo materialCombinerPerRendererInfo in this.slotData)
		{
			if (materialCombinerPerRendererInfo.renderer == r && materialCombinerPerRendererInfo.slotIndex == slot)
			{
				data = materialCombinerPerRendererInfo;
				return true;
			}
		}
		data = default(MaterialCombinerPerRendererInfo);
		return false;
	}

	// Token: 0x04001AA9 RID: 6825
	public List<MaterialCombinerPerRendererInfo> slotData = new List<MaterialCombinerPerRendererInfo>();
}
