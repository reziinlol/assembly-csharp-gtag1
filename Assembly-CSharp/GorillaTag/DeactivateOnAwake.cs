using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200114B RID: 4427
	public class DeactivateOnAwake : MonoBehaviour
	{
		// Token: 0x0600703A RID: 28730 RVA: 0x00249781 File Offset: 0x00247981
		private void Awake()
		{
			base.gameObject.SetActive(false);
			Object.Destroy(this);
		}
	}
}
