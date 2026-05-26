using System;
using UnityEngine;

// Token: 0x020003A6 RID: 934
public static class UnityLayerExtensions
{
	// Token: 0x06001697 RID: 5783 RVA: 0x00082EE6 File Offset: 0x000810E6
	public static int ToLayerMask(this UnityLayer self)
	{
		return 1 << (int)self;
	}

	// Token: 0x06001698 RID: 5784 RVA: 0x00082EEE File Offset: 0x000810EE
	public static int ToLayerIndex(this UnityLayer self)
	{
		return (int)self;
	}

	// Token: 0x06001699 RID: 5785 RVA: 0x00082EF1 File Offset: 0x000810F1
	public static bool IsOnLayer(this GameObject obj, UnityLayer layer)
	{
		return obj.layer == (int)layer;
	}

	// Token: 0x0600169A RID: 5786 RVA: 0x00082EFC File Offset: 0x000810FC
	public static void SetLayer(this GameObject obj, UnityLayer layer)
	{
		obj.layer = (int)layer;
	}

	// Token: 0x0600169B RID: 5787 RVA: 0x00082F08 File Offset: 0x00081108
	public static void SetLayerRecursively(this GameObject obj, UnityLayer layer)
	{
		obj.layer = (int)layer;
		foreach (object obj2 in obj.transform)
		{
			((Transform)obj2).gameObject.SetLayerRecursively(layer);
		}
	}
}
