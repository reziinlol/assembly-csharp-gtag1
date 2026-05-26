using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Token: 0x02000DBB RID: 3515
public static class UnityYaml
{
	// Token: 0x04006611 RID: 26129
	private static readonly Assembly EngineAssembly = Assembly.GetAssembly(typeof(MonoBehaviour));

	// Token: 0x04006612 RID: 26130
	private static readonly Assembly TerrainAssembly = Assembly.GetAssembly(typeof(Tree));

	// Token: 0x04006613 RID: 26131
	public static Dictionary<int, Type> ClassIDToType = new Dictionary<int, Type>();
}
