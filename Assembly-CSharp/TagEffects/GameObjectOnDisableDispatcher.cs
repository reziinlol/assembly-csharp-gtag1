using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x020010D9 RID: 4313
	public class GameObjectOnDisableDispatcher : MonoBehaviour
	{
		// Token: 0x140000BD RID: 189
		// (add) Token: 0x06006BF8 RID: 27640 RVA: 0x0022F36C File Offset: 0x0022D56C
		// (remove) Token: 0x06006BF9 RID: 27641 RVA: 0x0022F3A4 File Offset: 0x0022D5A4
		public event GameObjectOnDisableDispatcher.OnDisabledEvent OnDisabled;

		// Token: 0x06006BFA RID: 27642 RVA: 0x0022F3D9 File Offset: 0x0022D5D9
		private void OnDisable()
		{
			if (this.OnDisabled != null)
			{
				this.OnDisabled(this);
			}
		}

		// Token: 0x020010DA RID: 4314
		// (Invoke) Token: 0x06006BFD RID: 27645
		public delegate void OnDisabledEvent(GameObjectOnDisableDispatcher me);
	}
}
