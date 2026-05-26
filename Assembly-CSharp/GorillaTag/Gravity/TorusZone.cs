using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001191 RID: 4497
	public class TorusZone : BasicGravityZone
	{
		// Token: 0x060071E2 RID: 29154 RVA: 0x00251332 File Offset: 0x0024F532
		protected override void Awake()
		{
			base.Awake();
			this.sqrDistance = this.rotationDistance * this.rotationDistance;
		}

		// Token: 0x060071E3 RID: 29155 RVA: 0x00251350 File Offset: 0x0024F550
		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			Vector3 up = base.transform.up;
			Vector3 vector = worldPosition - base.transform.position;
			Vector3 a = vector - up * Vector3.Dot(vector, up);
			float sqrMagnitude = a.sqrMagnitude;
			Vector3 b;
			if (sqrMagnitude < 1E-10f)
			{
				Vector3 vector2 = Vector3.Cross(up, Vector3.right);
				if (vector2.sqrMagnitude < 1E-10f)
				{
					vector2 = Vector3.Cross(up, Vector3.forward);
				}
				b = base.transform.position + vector2.normalized * this.majorRadius;
			}
			else
			{
				b = base.transform.position + a * (this.majorRadius / Mathf.Sqrt(sqrMagnitude));
			}
			return worldPosition - b;
		}

		// Token: 0x060071E4 RID: 29156 RVA: 0x00251424 File Offset: 0x0024F624
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

		// Token: 0x04008192 RID: 33170
		[Tooltip("Major radius of the torus (distance from torus center to the centerline of the tube). Torus axis is transform.up.")]
		[SerializeField]
		protected float majorRadius = 5f;

		// Token: 0x04008193 RID: 33171
		[Tooltip("how close to the central ring of the torus to enable rotating the player")]
		[SerializeField]
		protected float rotationDistance;

		// Token: 0x04008194 RID: 33172
		[Tooltip("if enabled, always rotates the player")]
		[SerializeField]
		protected bool alwaysRotate = true;

		// Token: 0x04008195 RID: 33173
		private float sqrDistance;
	}
}
