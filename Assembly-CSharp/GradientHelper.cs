using System;
using UnityEngine;

// Token: 0x02000AD1 RID: 2769
public static class GradientHelper
{
	// Token: 0x060046A7 RID: 18087 RVA: 0x0017E63C File Offset: 0x0017C83C
	public static Gradient FromColor(Color color)
	{
		float a = color.a;
		Color col = color;
		col.a = 1f;
		return new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(col, 1f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(a, 1f)
			}
		};
	}
}
