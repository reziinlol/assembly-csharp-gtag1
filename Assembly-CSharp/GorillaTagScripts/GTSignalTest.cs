using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F16 RID: 3862
	public class GTSignalTest : GTSignalListener
	{
		// Token: 0x04006EEA RID: 28394
		public MeshRenderer[] targets = new MeshRenderer[0];

		// Token: 0x04006EEB RID: 28395
		[Space]
		public MeshRenderer target;

		// Token: 0x04006EEC RID: 28396
		public List<GTSignalListener> listeners = new List<GTSignalListener>(12);
	}
}
