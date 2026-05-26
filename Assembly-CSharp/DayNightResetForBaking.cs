using System;
using UnityEngine;

// Token: 0x020009FD RID: 2557
public class DayNightResetForBaking : MonoBehaviour
{
	// Token: 0x06004165 RID: 16741 RVA: 0x0015E140 File Offset: 0x0015C340
	public void SetMaterialsForBaking()
	{
		foreach (Material material in this.dayNightManager.dayNightSupportedMaterials)
		{
			if (material != null)
			{
				material.shader = this.dayNightManager.standard;
			}
			else
			{
				Debug.LogError("a material is missing from day night supported materials in the daynightmanager! something might have gotten deleted inappropriately, or an entry should be manually removed.", base.gameObject);
			}
		}
		foreach (Material material2 in this.dayNightManager.dayNightSupportedMaterialsCutout)
		{
			if (material2 != null)
			{
				material2.shader = this.dayNightManager.standardCutout;
			}
			else
			{
				Debug.LogError("a material is missing from day night supported materials cutout in the daynightmanager! something might have gotten deleted inappropriately, or an entry should be manually removed.", base.gameObject);
			}
		}
	}

	// Token: 0x06004166 RID: 16742 RVA: 0x0015E1E4 File Offset: 0x0015C3E4
	public void SetMaterialsForGame()
	{
		foreach (Material material in this.dayNightManager.dayNightSupportedMaterials)
		{
			if (material != null)
			{
				material.shader = this.dayNightManager.gorillaUnlit;
			}
			else
			{
				Debug.LogError("a material is missing from day night supported materials in the daynightmanager! something might have gotten deleted inappropriately, or an entry should be manually removed.", base.gameObject);
			}
		}
		foreach (Material material2 in this.dayNightManager.dayNightSupportedMaterialsCutout)
		{
			if (material2 != null)
			{
				material2.shader = this.dayNightManager.gorillaUnlitCutout;
			}
			else
			{
				Debug.LogError("a material is missing from day night supported materialsc cutout in the daynightmanager! something might have gotten deleted inappropriately, or an entry should be manually removed.", base.gameObject);
			}
		}
	}

	// Token: 0x0400530C RID: 21260
	public BetterDayNightManager dayNightManager;
}
