using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001237 RID: 4663
	public class PickupableVariant : MonoBehaviour
	{
		// Token: 0x060074AF RID: 29871 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected internal virtual void Release(HoldableObject holdable, Vector3 startPosition, Vector3 releaseVelocity, float playerScale)
		{
		}

		// Token: 0x060074B0 RID: 29872 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected internal virtual void Pickup(bool isAutoPickup = false)
		{
		}

		// Token: 0x060074B1 RID: 29873 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected internal virtual void DelayedPickup()
		{
		}
	}
}
