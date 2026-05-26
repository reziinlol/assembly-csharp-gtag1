using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012B7 RID: 4791
	public class WormInApple : MonoBehaviour
	{
		// Token: 0x060077DB RID: 30683 RVA: 0x00274F28 File Offset: 0x00273128
		public void OnHandTap()
		{
			if (this.blendShapeCosmetic && this.blendShapeCosmetic.GetBlendValue() > 0.5f)
			{
				UnityEvent onHandTapped = this.OnHandTapped;
				if (onHandTapped == null)
				{
					return;
				}
				onHandTapped.Invoke();
			}
		}

		// Token: 0x04008AA3 RID: 35491
		[SerializeField]
		private UpdateBlendShapeCosmetic blendShapeCosmetic;

		// Token: 0x04008AA4 RID: 35492
		public UnityEvent OnHandTapped;
	}
}
