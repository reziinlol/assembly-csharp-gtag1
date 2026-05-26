using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02001201 RID: 4609
	[Serializable]
	public class EdMeshCombinedPrefabData
	{
		// Token: 0x06007392 RID: 29586 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void Clear()
		{
		}

		// Token: 0x040083E6 RID: 33766
		public string path;

		// Token: 0x040083E7 RID: 33767
		public List<Renderer> disabled = new List<Renderer>(512);

		// Token: 0x040083E8 RID: 33768
		public List<GameObject> combined = new List<GameObject>(64);
	}
}
