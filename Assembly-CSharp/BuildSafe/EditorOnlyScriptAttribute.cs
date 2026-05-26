using System;
using System.Diagnostics;

namespace BuildSafe
{
	// Token: 0x02001005 RID: 4101
	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Class)]
	public class EditorOnlyScriptAttribute : Attribute
	{
	}
}
