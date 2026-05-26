using System;
using System.Diagnostics;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02001006 RID: 4102
	internal static class EditorOnlyScripts
	{
		// Token: 0x0600667B RID: 26235 RVA: 0x000028C5 File Offset: 0x00000AC5
		[Conditional("UNITY_EDITOR")]
		public static void Cleanup(GameObject[] rootObjects, bool force = false)
		{
		}
	}
}
