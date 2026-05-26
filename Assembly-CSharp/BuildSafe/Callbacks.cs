using System;
using System.Diagnostics;

namespace BuildSafe
{
	// Token: 0x02001001 RID: 4097
	public static class Callbacks
	{
		// Token: 0x02001002 RID: 4098
		[Conditional("UNITY_EDITOR")]
		public class DidReloadScripts : Attribute
		{
			// Token: 0x06006672 RID: 26226 RVA: 0x002101F1 File Offset: 0x0020E3F1
			public DidReloadScripts(bool activeOnly = false)
			{
				this.activeOnly = activeOnly;
			}

			// Token: 0x040075F0 RID: 30192
			public bool activeOnly;
		}
	}
}
