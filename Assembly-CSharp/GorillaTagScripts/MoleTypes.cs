using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000EC0 RID: 3776
	public class MoleTypes : MonoBehaviour
	{
		// Token: 0x170008EE RID: 2286
		// (get) Token: 0x06005CD9 RID: 23769 RVA: 0x001D7331 File Offset: 0x001D5531
		// (set) Token: 0x06005CDA RID: 23770 RVA: 0x001D7339 File Offset: 0x001D5539
		public bool IsLeftSideMoleType { get; set; }

		// Token: 0x170008EF RID: 2287
		// (get) Token: 0x06005CDB RID: 23771 RVA: 0x001D7342 File Offset: 0x001D5542
		// (set) Token: 0x06005CDC RID: 23772 RVA: 0x001D734A File Offset: 0x001D554A
		public Mole MoleContainerParent { get; set; }

		// Token: 0x06005CDD RID: 23773 RVA: 0x001D7353 File Offset: 0x001D5553
		private void Start()
		{
			this.MoleContainerParent = base.GetComponentInParent<Mole>();
			if (this.MoleContainerParent)
			{
				this.IsLeftSideMoleType = this.MoleContainerParent.IsLeftSideMole;
			}
		}

		// Token: 0x04006B4D RID: 27469
		public bool isHazard;

		// Token: 0x04006B4E RID: 27470
		public int scorePoint = 1;

		// Token: 0x04006B4F RID: 27471
		public MeshRenderer MeshRenderer;

		// Token: 0x04006B50 RID: 27472
		public Material monkeMoleDefaultMaterial;

		// Token: 0x04006B51 RID: 27473
		public Material monkeMoleHitMaterial;
	}
}
