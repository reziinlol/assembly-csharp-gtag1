using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x020011B1 RID: 4529
	public abstract class GuidedRefIdBaseSO : ScriptableObject, IGuidedRefObject
	{
		// Token: 0x0600727A RID: 29306 RVA: 0x000028C5 File Offset: 0x00000AC5
		public virtual void GuidedRefInitialize()
		{
		}

		// Token: 0x0600727C RID: 29308 RVA: 0x00018FAD File Offset: 0x000171AD
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}
	}
}
