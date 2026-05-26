using System;
using System.Diagnostics;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001134 RID: 4404
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class VectorLabelTextAttribute : PropertyAttribute
	{
		// Token: 0x06006FD3 RID: 28627 RVA: 0x00248296 File Offset: 0x00246496
		public VectorLabelTextAttribute(params string[] labels) : this(-1, labels)
		{
		}

		// Token: 0x06006FD4 RID: 28628 RVA: 0x0001302F File Offset: 0x0001122F
		public VectorLabelTextAttribute(int width, params string[] labels)
		{
		}
	}
}
