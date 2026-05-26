using System;
using UnityEngine;

// Token: 0x02000E06 RID: 3590
[Serializable]
public struct ShaderGroup
{
	// Token: 0x06005794 RID: 22420 RVA: 0x001C5C3E File Offset: 0x001C3E3E
	public ShaderGroup(Material material, Shader original, Shader gameplay, Shader baking)
	{
		this.material = material;
		this.originalShader = original;
		this.gameplayShader = gameplay;
		this.bakingShader = baking;
	}

	// Token: 0x04006835 RID: 26677
	public Material material;

	// Token: 0x04006836 RID: 26678
	public Shader originalShader;

	// Token: 0x04006837 RID: 26679
	public Shader gameplayShader;

	// Token: 0x04006838 RID: 26680
	public Shader bakingShader;
}
