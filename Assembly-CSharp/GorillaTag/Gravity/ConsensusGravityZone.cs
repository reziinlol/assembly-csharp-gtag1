using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001188 RID: 4488
	public class ConsensusGravityZone : BasicGravityZone
	{
		// Token: 0x060071A0 RID: 29088 RVA: 0x00250709 File Offset: 0x0024E909
		protected override void Awake()
		{
			base.Awake();
			this.zoneCollider = base.GetComponent<Collider>();
		}

		// Token: 0x060071A1 RID: 29089 RVA: 0x0025071D File Offset: 0x0024E91D
		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return base.transform.TransformVector(new Vector3(Mathf.Sin(this.currentRot * 0.017453292f), Mathf.Cos(this.currentRot * 0.017453292f), 0f));
		}

		// Token: 0x060071A2 RID: 29090 RVA: 0x00250758 File Offset: 0x0024E958
		private void FixedUpdate()
		{
			Vector3 a = Vector3.zero;
			int num = 0;
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				Vector3 position = rigContainer.Rig.transform.position;
				if (this.zoneCollider.bounds.Contains(position))
				{
					a += position;
					num++;
				}
			}
			if (num > 0)
			{
				Vector3 position2 = a / (float)num;
				Vector3 vector = base.transform.InverseTransformPoint(position2);
				this.idealRot = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
			}
			float num2 = (this.idealRot - this.currentRot) * this.weightForce - this.currentRot * this.centeringForce;
			this.rotSpeed += num2 * Time.fixedDeltaTime;
			this.rotSpeed *= this.drag;
			this.currentRot += this.rotSpeed * Time.fixedDeltaTime;
			if (this.currentRot < this.rotMin)
			{
				this.rotSpeed = 0f;
				this.currentRot = this.rotMin;
				return;
			}
			if (this.currentRot > this.rotMax)
			{
				this.rotSpeed = 0f;
				this.currentRot = this.rotMax;
			}
		}

		// Token: 0x060071A3 RID: 29091 RVA: 0x00023994 File Offset: 0x00021B94
		protected override bool GetRotationIntent(in Vector3 offsetFromGravity)
		{
			return true;
		}

		// Token: 0x0400815E RID: 33118
		private Collider zoneCollider;

		// Token: 0x0400815F RID: 33119
		private float currentRot;

		// Token: 0x04008160 RID: 33120
		private float idealRot;

		// Token: 0x04008161 RID: 33121
		private float rotSpeed;

		// Token: 0x04008162 RID: 33122
		[SerializeField]
		private float weightForce;

		// Token: 0x04008163 RID: 33123
		[SerializeField]
		private float centeringForce;

		// Token: 0x04008164 RID: 33124
		[SerializeField]
		private float drag;

		// Token: 0x04008165 RID: 33125
		[SerializeField]
		private float rotMin = -45f;

		// Token: 0x04008166 RID: 33126
		[SerializeField]
		private float rotMax = 45f;
	}
}
