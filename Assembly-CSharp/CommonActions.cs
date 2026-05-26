using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200003B RID: 59
public class CommonActions : MonoBehaviour
{
	// Token: 0x060000F5 RID: 245 RVA: 0x00005FDD File Offset: 0x000041DD
	public void LoadSavedOutfit(int index)
	{
		if (CosmeticsController.instance)
		{
			CosmeticsController.instance.LoadSavedOutfit(index);
		}
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x00005FFA File Offset: 0x000041FA
	public void LoadPrevOutfit()
	{
		if (CosmeticsController.instance)
		{
			CosmeticsController.instance.PressWardrobeScrollOutfit(false);
		}
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x00006017 File Offset: 0x00004217
	public void LoadNextOutfit()
	{
		if (CosmeticsController.instance)
		{
			CosmeticsController.instance.PressWardrobeScrollOutfit(true);
		}
	}
}
