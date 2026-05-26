using System;
using System.Collections.Generic;
using GorillaTag.Rendering.Shaders;
using UnityEngine;

// Token: 0x02000D2B RID: 3371
public class BetterBakerBakeMe : FlagForBaking
{
	// Token: 0x0400646A RID: 25706
	public GameObject[] stuffIncludingParentsToBake;

	// Token: 0x0400646B RID: 25707
	public GameObject getMatStuffFromHere;

	// Token: 0x0400646C RID: 25708
	public List<ShaderConfigData.ShaderConfig> allConfigs = new List<ShaderConfigData.ShaderConfig>();
}
