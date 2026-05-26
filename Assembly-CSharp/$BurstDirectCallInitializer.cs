using System;
using Unity.Burst;
using UnityEngine;

// Token: 0x020013E0 RID: 5088
internal static class $BurstDirectCallInitializer
{
	// Token: 0x06007ECF RID: 32463 RVA: 0x00298FC4 File Offset: 0x002971C4
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void Initialize()
	{
		BurstCompilerOptions options = BurstCompiler.Options;
	}
}
