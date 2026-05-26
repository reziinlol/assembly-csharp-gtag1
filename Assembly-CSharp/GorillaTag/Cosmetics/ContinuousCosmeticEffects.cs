using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200124D RID: 4685
	public class ContinuousCosmeticEffects : MonoBehaviour
	{
		// Token: 0x0600756D RID: 30061 RVA: 0x002677E6 File Offset: 0x002659E6
		public void ApplyAll(float f)
		{
			this.continuousProperties.ApplyAll(f);
		}

		// Token: 0x0400870F RID: 34575
		[FormerlySerializedAs("properties")]
		[SerializeField]
		private ContinuousPropertyArray continuousProperties;
	}
}
