using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ED3 RID: 3795
	public class BuilderAttachPoint : MonoBehaviour
	{
		// Token: 0x06005D78 RID: 23928 RVA: 0x001DA281 File Offset: 0x001D8481
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x04006C0B RID: 27659
		public Transform center;
	}
}
