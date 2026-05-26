using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000E03 RID: 3587
public static class GTUberShaderUtils
{
	// Token: 0x06005787 RID: 22407 RVA: 0x001C501A File Offset: 0x001C321A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetStencilComparison(this Material m, GTShaderStencilCompare cmp)
	{
		m.SetFloat(GTUberShaderUtils._StencilComparison, (float)cmp);
	}

	// Token: 0x06005788 RID: 22408 RVA: 0x001C502E File Offset: 0x001C322E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetStencilPassFrontOp(this Material m, GTShaderStencilOp op)
	{
		m.SetFloat(GTUberShaderUtils._StencilPassFront, (float)op);
	}

	// Token: 0x06005789 RID: 22409 RVA: 0x001C5042 File Offset: 0x001C3242
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetStencilReferenceValue(this Material m, int value)
	{
		m.SetFloat(GTUberShaderUtils._StencilReference, (float)value);
	}

	// Token: 0x0600578A RID: 22410 RVA: 0x001C5058 File Offset: 0x001C3258
	public static void SetVisibleToXRay(this Material m, bool visible, bool saveToDisk = false)
	{
		GTShaderStencilCompare cmp = visible ? GTShaderStencilCompare.Equal : GTShaderStencilCompare.NotEqual;
		GTShaderStencilOp op = visible ? GTShaderStencilOp.Replace : GTShaderStencilOp.Keep;
		m.SetStencilComparison(cmp);
		m.SetStencilPassFrontOp(op);
		m.SetStencilReferenceValue(7);
	}

	// Token: 0x0600578B RID: 22411 RVA: 0x001C508C File Offset: 0x001C328C
	public static void SetRevealsXRay(this Material m, bool reveals, bool changeQueue = true, bool saveToDisk = false)
	{
		m.SetFloat(GTUberShaderUtils._ZWrite, (float)(reveals ? 0 : 1));
		m.SetFloat(GTUberShaderUtils._ColorMask_, (float)(reveals ? 0 : 14));
		m.SetStencilComparison(GTShaderStencilCompare.Disabled);
		m.SetStencilPassFrontOp(reveals ? GTShaderStencilOp.Replace : GTShaderStencilOp.Keep);
		m.SetStencilReferenceValue(reveals ? 7 : 0);
		if (changeQueue)
		{
			int renderQueue = m.renderQueue;
			m.renderQueue = renderQueue + (reveals ? -1 : 1);
		}
	}

	// Token: 0x0600578C RID: 22412 RVA: 0x001C5104 File Offset: 0x001C3304
	public static int GetNearestRenderQueue(this Material m, out RenderQueue queue)
	{
		int renderQueue = m.renderQueue;
		int num = -1;
		int num2 = int.MaxValue;
		for (int i = 0; i < GTUberShaderUtils.kRenderQueueInts.Length; i++)
		{
			int num3 = GTUberShaderUtils.kRenderQueueInts[i];
			int num4 = Math.Abs(num3 - renderQueue);
			if (num2 > num4)
			{
				num = num3;
				num2 = num4;
			}
		}
		queue = (RenderQueue)num;
		return num;
	}

	// Token: 0x0600578D RID: 22413 RVA: 0x001C5155 File Offset: 0x001C3355
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitOnLoad()
	{
		GTUberShaderUtils.kUberShader = Shader.Find("GorillaTag/UberShader");
	}

	// Token: 0x040067DA RID: 26586
	private static Shader kUberShader;

	// Token: 0x040067DB RID: 26587
	private static readonly ShaderHashId _StencilComparison = "_StencilComparison";

	// Token: 0x040067DC RID: 26588
	private static readonly ShaderHashId _StencilPassFront = "_StencilPassFront";

	// Token: 0x040067DD RID: 26589
	private static readonly ShaderHashId _StencilReference = "_StencilReference";

	// Token: 0x040067DE RID: 26590
	private static readonly ShaderHashId _ColorMask_ = "_ColorMask_";

	// Token: 0x040067DF RID: 26591
	private static readonly ShaderHashId _ManualZWrite = "_ManualZWrite";

	// Token: 0x040067E0 RID: 26592
	private static readonly ShaderHashId _ZWrite = "_ZWrite";

	// Token: 0x040067E1 RID: 26593
	private static readonly int[] kRenderQueueInts = new int[]
	{
		1000,
		2000,
		2450,
		2500,
		3000,
		4000
	};
}
