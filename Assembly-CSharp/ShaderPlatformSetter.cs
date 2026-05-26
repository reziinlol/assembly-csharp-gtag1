using System;
using UnityEngine;

// Token: 0x02000389 RID: 905
public static class ShaderPlatformSetter
{
	// Token: 0x060015E7 RID: 5607 RVA: 0x0007C485 File Offset: 0x0007A685
	[RuntimeInitializeOnLoadMethod]
	public static void HandleRuntimeInitializeOnLoad()
	{
		Shader.DisableKeyword("PLATFORM_IS_ANDROID");
		Shader.DisableKeyword("QATESTING");
	}
}
