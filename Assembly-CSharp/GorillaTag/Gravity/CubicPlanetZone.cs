using System;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001189 RID: 4489
	public class CubicPlanetZone : PlanetZone
	{
		// Token: 0x060071A5 RID: 29093 RVA: 0x002508E4 File Offset: 0x0024EAE4
		private void UpdateConstraint()
		{
			float num = Mathf.Abs(this.constraints.x * 0.5f);
			float num2 = Mathf.Abs(this.constraints.y * 0.5f);
			float num3 = Mathf.Abs(this.constraints.z * 0.5f);
			this.minConstraints.x = num * -1f;
			this.minConstraints.y = num2 * -1f;
			this.minConstraints.z = num3 * -1f;
			this.maxConstraints.x = num;
			this.maxConstraints.y = num2;
			this.maxConstraints.z = num3;
		}

		// Token: 0x060071A6 RID: 29094 RVA: 0x00250990 File Offset: 0x0024EB90
		protected override void Awake()
		{
			base.Awake();
			this.inverseRotation = Quaternion.Inverse(base.transform.rotation);
			this.UpdateConstraint();
		}

		// Token: 0x060071A7 RID: 29095 RVA: 0x002509B4 File Offset: 0x0024EBB4
		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return worldPosition - this.GetPointOnBounds(worldPosition);
		}

		// Token: 0x060071A8 RID: 29096 RVA: 0x002509C8 File Offset: 0x0024EBC8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetPointOnBounds(in Vector3 point)
		{
			Transform transform = base.transform;
			Vector3 position = transform.position;
			Vector3 vector = this.inverseRotation * (point - position);
			float x = vector.x;
			float y = vector.y;
			float z = vector.z;
			if (x <= this.maxConstraints.x && x >= this.minConstraints.x && y <= this.maxConstraints.y && y >= this.minConstraints.y && z <= this.maxConstraints.z && z >= this.minConstraints.z)
			{
				Vector3 vector2 = new Vector3((x > 0f) ? this.maxConstraints.x : this.minConstraints.x, (y > 0f) ? this.maxConstraints.y : this.minConstraints.y, (z > 0f) ? this.maxConstraints.z : this.minConstraints.z);
				float num = Mathf.Abs(vector2.x - x);
				float num2 = Mathf.Abs(vector2.y - y);
				float num3 = Mathf.Abs(vector2.z - z);
				Vector3 vector3 = new Vector3((num <= num2 && num <= num3) ? 1f : 0f, (num2 <= num && num2 <= num3) ? 1f : 0f, (num3 <= num && num3 <= num2) ? 1f : 0f);
				Vector3 vector4 = vector.MultiplyBy(vector3);
				Vector3 vector5 = vector2.MultiplyBy(vector3);
				float magnitude = vector4.magnitude;
				float magnitude2 = vector5.magnitude;
				float num4 = 1f / magnitude2;
				float num5 = magnitude * num4;
				num5 *= 0.99f;
				return position + transform.rotation * vector.Clamp(this.minConstraints * num5, this.maxConstraints * num5);
			}
			return position + transform.rotation * vector.Clamp(this.minConstraints, this.maxConstraints);
		}

		// Token: 0x04008167 RID: 33127
		[Header("box constraint for where gravity center can be")]
		[SerializeField]
		protected Vector3 constraints;

		// Token: 0x04008168 RID: 33128
		[SerializeField]
		protected Vector3 minConstraints;

		// Token: 0x04008169 RID: 33129
		[SerializeField]
		protected Vector3 maxConstraints;

		// Token: 0x0400816A RID: 33130
		protected Quaternion inverseRotation;
	}
}
