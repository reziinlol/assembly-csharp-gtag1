using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x0200118F RID: 4495
	public class PlanetZone : BasicGravityZone
	{
		// Token: 0x060071DC RID: 29148 RVA: 0x0025127A File Offset: 0x0024F47A
		protected override void Awake()
		{
			base.Awake();
			this.sqrDistance = this.rotationDistance * this.rotationDistance;
		}

		// Token: 0x060071DD RID: 29149 RVA: 0x00251295 File Offset: 0x0024F495
		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return worldPosition - base.transform.position;
		}

		// Token: 0x060071DE RID: 29150 RVA: 0x002512B0 File Offset: 0x0024F4B0
		protected override bool GetRotationIntent(in Vector3 offsetFromGravity)
		{
			if (this.alwaysRotate)
			{
				return true;
			}
			if (this.rotateTarget)
			{
				Vector3 vector = offsetFromGravity;
				return vector.sqrMagnitude < this.sqrDistance;
			}
			return false;
		}

		// Token: 0x0400818E RID: 33166
		[Tooltip("how close to the center of the zone to enable rotating the player")]
		[SerializeField]
		protected float rotationDistance;

		// Token: 0x0400818F RID: 33167
		[Tooltip("if enabled, always rotates the player")]
		[SerializeField]
		protected bool alwaysRotate = true;

		// Token: 0x04008190 RID: 33168
		private float sqrDistance;
	}
}
