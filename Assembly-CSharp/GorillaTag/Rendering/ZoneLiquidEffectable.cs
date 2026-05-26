using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02001208 RID: 4616
	public sealed class ZoneLiquidEffectable : MonoBehaviour
	{
		// Token: 0x060073A1 RID: 29601 RVA: 0x0025A443 File Offset: 0x00258643
		private void Awake()
		{
			this.childRenderers = base.GetComponentsInChildren<Renderer>(false);
		}

		// Token: 0x060073A2 RID: 29602 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void OnEnable()
		{
		}

		// Token: 0x060073A3 RID: 29603 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void OnDisable()
		{
		}

		// Token: 0x04008413 RID: 33811
		public float radius = 1f;

		// Token: 0x04008414 RID: 33812
		[NonSerialized]
		public bool inLiquidVolume;

		// Token: 0x04008415 RID: 33813
		[NonSerialized]
		public bool wasInLiquidVolume;

		// Token: 0x04008416 RID: 33814
		[NonSerialized]
		public Renderer[] childRenderers;
	}
}
