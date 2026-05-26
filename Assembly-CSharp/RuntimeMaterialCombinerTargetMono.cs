using System;
using UnityEngine;

// Token: 0x02000383 RID: 899
public class RuntimeMaterialCombinerTargetMono : MonoBehaviour
{
	// Token: 0x060015D9 RID: 5593 RVA: 0x0007524B File Offset: 0x0007344B
	protected void Awake()
	{
		throw new NotImplementedException("// TODO: get the material combiner manager to fingerprint and combine these materials.");
	}

	// Token: 0x04001B5C RID: 7004
	[HideInInspector]
	public GTSerializableDict<string, string>[] m_matSlot_to_texProp_to_texGuid;
}
