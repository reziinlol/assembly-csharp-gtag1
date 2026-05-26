using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001241 RID: 4673
	public class TrashcanCosmetic : MonoBehaviour
	{
		// Token: 0x06007500 RID: 29952 RVA: 0x00265500 File Offset: 0x00263700
		public void OnBasket(bool isLeftHand, Collider other)
		{
			SlingshotProjectile slingshotProjectile;
			if (other.TryGetComponent<SlingshotProjectile>(out slingshotProjectile) && slingshotProjectile.GetDistanceTraveled() >= this.minScoringDistance)
			{
				UnityEvent onScored = this.OnScored;
				if (onScored != null)
				{
					onScored.Invoke();
				}
				slingshotProjectile.DestroyAfterRelease();
			}
		}

		// Token: 0x0400867D RID: 34429
		public float minScoringDistance = 2f;

		// Token: 0x0400867E RID: 34430
		public UnityEvent OnScored;
	}
}
