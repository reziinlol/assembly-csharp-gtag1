using System;
using UnityEngine;

namespace CosmeticRoom.ItemScripts
{
	// Token: 0x02000FFF RID: 4095
	public class TrickTreatHoldable : TransferrableObject
	{
		// Token: 0x0600666B RID: 26219 RVA: 0x00210191 File Offset: 0x0020E391
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
			if (this.candyCollider)
			{
				this.candyCollider.enabled = (this.IsMyItem() && this.IsHeld());
			}
		}

		// Token: 0x040075EF RID: 30191
		public MeshCollider candyCollider;
	}
}
