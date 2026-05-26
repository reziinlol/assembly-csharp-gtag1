using System;
using System.Collections.Generic;
using AA;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x020010EA RID: 4330
	[RequireComponent(typeof(Rigidbody))]
	public class RigidbodyWaterInteraction : MonoBehaviour
	{
		// Token: 0x06006D06 RID: 27910 RVA: 0x0023847A File Offset: 0x0023667A
		protected void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.baseAngularDrag = this.rb.angularDamping;
			RigidbodyWaterInteractionManager.RegisterRBWI(this);
		}

		// Token: 0x06006D07 RID: 27911 RVA: 0x0023849F File Offset: 0x0023669F
		protected void OnEnable()
		{
			this.overlappingWaterVolumes.Clear();
			RigidbodyWaterInteractionManager.RegisterRBWI(this);
		}

		// Token: 0x06006D08 RID: 27912 RVA: 0x002384B2 File Offset: 0x002366B2
		protected void OnDisable()
		{
			this.overlappingWaterVolumes.Clear();
			RigidbodyWaterInteractionManager.UnregisterRBWI(this);
		}

		// Token: 0x06006D09 RID: 27913 RVA: 0x002384C5 File Offset: 0x002366C5
		private void OnDestroy()
		{
			RigidbodyWaterInteractionManager.UnregisterRBWI(this);
		}

		// Token: 0x06006D0A RID: 27914 RVA: 0x002384D0 File Offset: 0x002366D0
		public void InvokeFixedUpdate()
		{
			if (this.rb.isKinematic)
			{
				return;
			}
			bool flag = this.overlappingWaterVolumes.Count > 0;
			WaterVolume.SurfaceQuery surfaceQuery = default(WaterVolume.SurfaceQuery);
			float num = float.MinValue;
			if (flag && this.enablePreciseWaterCollision)
			{
				Vector3 vector = base.transform.position + GTPlayerTransform.PhysicsDown * 2f * this.objectRadiusForWaterCollision * this.buoyancyEquilibrium;
				bool flag2 = false;
				this.activeWaterCurrents.Clear();
				for (int i = 0; i < this.overlappingWaterVolumes.Count; i++)
				{
					WaterVolume.SurfaceQuery surfaceQuery2;
					if (this.overlappingWaterVolumes[i].GetSurfaceQueryForPoint(vector, out surfaceQuery2, false))
					{
						float num2 = Vector3.Dot(surfaceQuery2.surfacePoint - vector, surfaceQuery2.surfaceNormal);
						if (num2 > num)
						{
							num = num2;
							surfaceQuery = surfaceQuery2;
							flag2 = true;
						}
						WaterCurrent waterCurrent = this.overlappingWaterVolumes[i].Current;
						if (this.applyWaterCurrents && waterCurrent != null && num2 > 0f && !this.activeWaterCurrents.Contains(waterCurrent))
						{
							this.activeWaterCurrents.Add(waterCurrent);
						}
					}
				}
				if (flag2)
				{
					bool flag3 = num > -(1f - this.buoyancyEquilibrium) * 2f * this.objectRadiusForWaterCollision;
					float d = this.enablePreciseWaterCollision ? this.objectRadiusForWaterCollision : 0f;
					Vector3 b = surfaceQuery.surfacePoint - surfaceQuery.surfaceNormal * surfaceQuery.maxDepth;
					bool flag4 = Vector3.Dot(base.transform.position + surfaceQuery.surfaceNormal * d - b, surfaceQuery.surfaceNormal) > 0f;
					flag = (flag3 && flag4);
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				float fixedDeltaTime = Time.fixedDeltaTime;
				Vector3 vector2 = this.rb.linearVelocity;
				Vector3 vector3 = Vector3.zero;
				if (this.applyWaterCurrents)
				{
					Vector3 a = Vector3.zero;
					for (int j = 0; j < this.activeWaterCurrents.Count; j++)
					{
						WaterCurrent waterCurrent2 = this.activeWaterCurrents[j];
						Vector3 startingVelocity = vector2 + vector3;
						Vector3 b2;
						Vector3 b3;
						if (waterCurrent2.GetCurrentAtPoint(base.transform.position, startingVelocity, fixedDeltaTime, out b2, out b3))
						{
							a += b2;
							vector3 += b3;
						}
					}
					if (this.enablePreciseWaterCollision)
					{
						Vector3 position = (surfaceQuery.surfacePoint + (base.transform.position + GTPlayerTransform.PhysicsDown * this.objectRadiusForWaterCollision)) * 0.5f;
						this.rb.AddForceAtPosition(vector3 * this.rb.mass, position, ForceMode.Impulse);
					}
					else
					{
						vector2 += vector3;
					}
				}
				if (this.applyBuoyancyForce)
				{
					Vector3 vector4 = Vector3.zero;
					if (this.enablePreciseWaterCollision)
					{
						float b4 = 2f * this.objectRadiusForWaterCollision * this.buoyancyEquilibrium;
						float d2 = Mathf.InverseLerp(0f, b4, num);
						vector4 = GTPlayerTransform.PhysicsUp * Physics.gravity.magnitude * this.underWaterBuoyancyFactor * d2 * fixedDeltaTime;
					}
					else
					{
						vector4 = GTPlayerTransform.PhysicsUp * Physics.gravity.magnitude * this.underWaterBuoyancyFactor * fixedDeltaTime;
					}
					if (vector3.sqrMagnitude > 0.001f)
					{
						float magnitude = vector3.magnitude;
						Vector3 vector5 = vector3 / magnitude;
						float num3 = Vector3.Dot(vector4, vector5);
						if (num3 < 0f)
						{
							vector4 -= num3 * vector5;
						}
					}
					vector2 += vector4;
				}
				float magnitude2 = vector2.magnitude;
				if (magnitude2 > 0.001f && this.applyDamping)
				{
					Vector3 a2 = vector2 / magnitude2;
					float num4 = Spring.DamperDecayExact(magnitude2, this.underWaterDampingHalfLife, fixedDeltaTime, 1E-05f);
					if (this.enablePreciseWaterCollision)
					{
						float a3 = Spring.DamperDecayExact(magnitude2, this.waterSurfaceDampingHalfLife, fixedDeltaTime, 1E-05f);
						float t = Mathf.Clamp(-Vector3.Dot(base.transform.position - surfaceQuery.surfacePoint, surfaceQuery.surfaceNormal) / this.objectRadiusForWaterCollision, -1f, 1f) * 0.5f + 0.5f;
						vector2 = Mathf.Lerp(a3, num4, t) * a2;
					}
					else
					{
						vector2 = num4 * a2;
					}
				}
				if (this.applySurfaceTorque && this.enablePreciseWaterCollision)
				{
					float num5 = Vector3.Dot(base.transform.position - surfaceQuery.surfacePoint, surfaceQuery.surfaceNormal);
					if (num5 < this.objectRadiusForWaterCollision && num5 > 0f)
					{
						Vector3 rhs = vector2 - Vector3.Dot(vector2, surfaceQuery.surfaceNormal) * surfaceQuery.surfaceNormal;
						Vector3 normalized = Vector3.Cross(surfaceQuery.surfaceNormal, rhs).normalized;
						float num6 = Vector3.Dot(this.rb.angularVelocity, normalized);
						float num7 = rhs.magnitude / this.objectRadiusForWaterCollision - num6;
						if (num7 > 0f)
						{
							this.rb.AddTorque(this.surfaceTorqueAmount * num7 * normalized, ForceMode.Acceleration);
						}
					}
				}
				this.rb.linearVelocity = vector2;
				this.rb.angularDamping = this.angularDrag;
				return;
			}
			this.rb.angularDamping = this.baseAngularDrag;
		}

		// Token: 0x06006D0B RID: 27915 RVA: 0x00238A60 File Offset: 0x00236C60
		protected void OnTriggerEnter(Collider other)
		{
			WaterVolume component = other.GetComponent<WaterVolume>();
			if (component != null && !this.overlappingWaterVolumes.Contains(component))
			{
				this.overlappingWaterVolumes.Add(component);
			}
		}

		// Token: 0x06006D0C RID: 27916 RVA: 0x00238A98 File Offset: 0x00236C98
		protected void OnTriggerExit(Collider other)
		{
			WaterVolume component = other.GetComponent<WaterVolume>();
			if (component != null && this.overlappingWaterVolumes.Contains(component))
			{
				this.overlappingWaterVolumes.Remove(component);
			}
		}

		// Token: 0x04007DD2 RID: 32210
		public bool applyDamping = true;

		// Token: 0x04007DD3 RID: 32211
		public bool applyBuoyancyForce = true;

		// Token: 0x04007DD4 RID: 32212
		public bool applyAngularDrag = true;

		// Token: 0x04007DD5 RID: 32213
		public bool applyWaterCurrents = true;

		// Token: 0x04007DD6 RID: 32214
		public bool applySurfaceTorque = true;

		// Token: 0x04007DD7 RID: 32215
		public float underWaterDampingHalfLife = 0.25f;

		// Token: 0x04007DD8 RID: 32216
		public float waterSurfaceDampingHalfLife = 1f;

		// Token: 0x04007DD9 RID: 32217
		public float underWaterBuoyancyFactor = 0.5f;

		// Token: 0x04007DDA RID: 32218
		public float angularDrag = 0.5f;

		// Token: 0x04007DDB RID: 32219
		public float surfaceTorqueAmount = 0.5f;

		// Token: 0x04007DDC RID: 32220
		public bool enablePreciseWaterCollision;

		// Token: 0x04007DDD RID: 32221
		public float objectRadiusForWaterCollision = 0.25f;

		// Token: 0x04007DDE RID: 32222
		[Range(0f, 1f)]
		public float buoyancyEquilibrium = 0.8f;

		// Token: 0x04007DDF RID: 32223
		private Rigidbody rb;

		// Token: 0x04007DE0 RID: 32224
		private List<WaterVolume> overlappingWaterVolumes = new List<WaterVolume>();

		// Token: 0x04007DE1 RID: 32225
		private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16);

		// Token: 0x04007DE2 RID: 32226
		private float baseAngularDrag = 0.05f;
	}
}
