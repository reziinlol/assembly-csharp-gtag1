using System;
using UnityEngine;

// Token: 0x02000E05 RID: 3589
public class MaterialMapping : ScriptableObject
{
	// Token: 0x06005791 RID: 22417 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void CleanUpData()
	{
	}

	// Token: 0x0400682F RID: 26671
	private static string path = "Assets/UberShaderConversion/MaterialMap.asset";

	// Token: 0x04006830 RID: 26672
	public static string materialDirectory = "Assets/UberShaderConversion/Materials/";

	// Token: 0x04006831 RID: 26673
	private static MaterialMapping instance;

	// Token: 0x04006832 RID: 26674
	public ShaderGroup[] map;

	// Token: 0x04006833 RID: 26675
	public Material mirrorMat;

	// Token: 0x04006834 RID: 26676
	public RenderTexture mirrorTexture;
}
