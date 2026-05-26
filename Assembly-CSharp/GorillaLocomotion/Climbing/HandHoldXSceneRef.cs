using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x0200110D RID: 4365
	public class HandHoldXSceneRef : MonoBehaviour
	{
		// Token: 0x17000A9C RID: 2716
		// (get) Token: 0x06006DFF RID: 28159 RVA: 0x0023FF50 File Offset: 0x0023E150
		public HandHold target
		{
			get
			{
				HandHold result;
				if (this.reference.TryResolve<HandHold>(out result))
				{
					return result;
				}
				return null;
			}
		}

		// Token: 0x17000A9D RID: 2717
		// (get) Token: 0x06006E00 RID: 28160 RVA: 0x0023FF70 File Offset: 0x0023E170
		public GameObject targetObject
		{
			get
			{
				GameObject result;
				if (this.reference.TryResolve(out result))
				{
					return result;
				}
				return null;
			}
		}

		// Token: 0x04007F10 RID: 32528
		[SerializeField]
		public XSceneRef reference;
	}
}
