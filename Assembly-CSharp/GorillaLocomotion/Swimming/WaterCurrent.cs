using System;
using System.Collections.Generic;
using AA;
using CjLib;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x020010EF RID: 4335
	public class WaterCurrent : MonoBehaviour
	{
		// Token: 0x17000A87 RID: 2695
		// (get) Token: 0x06006D24 RID: 27940 RVA: 0x00239AF5 File Offset: 0x00237CF5
		public float Speed
		{
			get
			{
				return this.currentSpeed;
			}
		}

		// Token: 0x17000A88 RID: 2696
		// (get) Token: 0x06006D25 RID: 27941 RVA: 0x00239AFD File Offset: 0x00237CFD
		public float Accel
		{
			get
			{
				return this.currentAccel;
			}
		}

		// Token: 0x17000A89 RID: 2697
		// (get) Token: 0x06006D26 RID: 27942 RVA: 0x00239B05 File Offset: 0x00237D05
		public float InwardSpeed
		{
			get
			{
				return this.inwardCurrentSpeed;
			}
		}

		// Token: 0x17000A8A RID: 2698
		// (get) Token: 0x06006D27 RID: 27943 RVA: 0x00239B0D File Offset: 0x00237D0D
		public float InwardAccel
		{
			get
			{
				return this.inwardCurrentAccel;
			}
		}

		// Token: 0x06006D28 RID: 27944 RVA: 0x00239B18 File Offset: 0x00237D18
		public bool GetCurrentAtPoint(Vector3 worldPoint, Vector3 startingVelocity, float dt, out Vector3 currentVelocity, out Vector3 velocityChange)
		{
			float num = (this.fullEffectDistance + this.fadeDistance) * (this.fullEffectDistance + this.fadeDistance);
			bool result = false;
			velocityChange = Vector3.zero;
			currentVelocity = Vector3.zero;
			float num2 = 0.0001f;
			float magnitude = startingVelocity.magnitude;
			if (magnitude > num2)
			{
				Vector3 a = startingVelocity / magnitude;
				float d = Spring.DamperDecayExact(magnitude, this.dampingHalfLife, dt, 1E-05f);
				Vector3 a2 = a * d;
				velocityChange += a2 - startingVelocity;
			}
			for (int i = 0; i < this.splines.Count; i++)
			{
				CatmullRomSpline catmullRomSpline = this.splines[i];
				Vector3 vector;
				float closestEvaluationOnSpline = catmullRomSpline.GetClosestEvaluationOnSpline(worldPoint, out vector);
				Vector3 a3 = catmullRomSpline.Evaluate(closestEvaluationOnSpline);
				Vector3 vector2 = a3 - worldPoint;
				if (vector2.sqrMagnitude < num)
				{
					result = true;
					float magnitude2 = vector2.magnitude;
					float num3 = (magnitude2 > this.fullEffectDistance) ? (1f - Mathf.Clamp01((magnitude2 - this.fullEffectDistance) / this.fadeDistance)) : 1f;
					float t = Mathf.Clamp01(closestEvaluationOnSpline + this.velocityAnticipationAdjustment);
					Vector3 forwardTangent = catmullRomSpline.GetForwardTangent(t, 0.01f);
					if (this.currentSpeed > num2 && Vector3.Dot(startingVelocity, forwardTangent) < num3 * this.currentSpeed)
					{
						velocityChange += forwardTangent * (this.currentAccel * dt);
					}
					else if (this.currentSpeed < num2 && Vector3.Dot(startingVelocity, forwardTangent) > num3 * this.currentSpeed)
					{
						velocityChange -= forwardTangent * (this.currentAccel * dt);
					}
					currentVelocity += forwardTangent * num3 * this.currentSpeed;
					float num4 = Mathf.InverseLerp(this.inwardCurrentNoEffectRadius, this.inwardCurrentFullEffectRadius, magnitude2);
					if (num4 > num2)
					{
						vector = Vector3.ProjectOnPlane(vector2, forwardTangent);
						Vector3 normalized = vector.normalized;
						if (this.inwardCurrentSpeed > num2 && Vector3.Dot(startingVelocity, normalized) < num4 * this.inwardCurrentSpeed)
						{
							velocityChange += normalized * (this.InwardAccel * dt);
						}
						else if (this.inwardCurrentSpeed < num2 && Vector3.Dot(startingVelocity, normalized) > num4 * this.inwardCurrentSpeed)
						{
							velocityChange -= normalized * (this.InwardAccel * dt);
						}
					}
					this.debugSplinePoint = a3;
				}
			}
			this.debugCurrentVelocity = velocityChange.normalized;
			return result;
		}

		// Token: 0x06006D29 RID: 27945 RVA: 0x00239DCC File Offset: 0x00237FCC
		private void Update()
		{
			if (this.debugDrawCurrentQueries)
			{
				DebugUtil.DrawSphere(this.debugSplinePoint, 0.15f, 12, 12, Color.green, false, DebugUtil.Style.Wireframe);
				DebugUtil.DrawArrow(this.debugSplinePoint, this.debugSplinePoint + this.debugCurrentVelocity, 0.1f, 0.1f, 12, 0.1f, Color.yellow, false, DebugUtil.Style.Wireframe);
			}
		}

		// Token: 0x06006D2A RID: 27946 RVA: 0x00239E30 File Offset: 0x00238030
		private void OnDrawGizmosSelected()
		{
			int num = 16;
			for (int i = 0; i < this.splines.Count; i++)
			{
				CatmullRomSpline catmullRomSpline = this.splines[i];
				Vector3 b = catmullRomSpline.Evaluate(0f);
				for (int j = 1; j <= num; j++)
				{
					float t = (float)j / (float)num;
					Vector3 vector = catmullRomSpline.Evaluate(t);
					vector - b;
					Quaternion rotation = Quaternion.LookRotation(catmullRomSpline.GetForwardTangent(t, 0.01f), Vector3.up);
					Gizmos.color = new Color(0f, 0.5f, 0.75f);
					this.DrawGizmoCircle(vector, rotation, this.fullEffectDistance);
					Gizmos.color = new Color(0f, 0.25f, 0.5f);
					this.DrawGizmoCircle(vector, rotation, this.fullEffectDistance + this.fadeDistance);
				}
			}
		}

		// Token: 0x06006D2B RID: 27947 RVA: 0x00239F18 File Offset: 0x00238118
		private void DrawGizmoCircle(Vector3 center, Quaternion rotation, float radius)
		{
			Vector3 point = Vector3.right * radius;
			int num = 16;
			for (int i = 1; i <= num; i++)
			{
				float f = (float)i / (float)num * 2f * 3.1415927f;
				Vector3 vector = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f) * radius;
				Gizmos.DrawLine(center + rotation * point, center + rotation * vector);
				point = vector;
			}
		}

		// Token: 0x04007E04 RID: 32260
		[SerializeField]
		private List<CatmullRomSpline> splines = new List<CatmullRomSpline>();

		// Token: 0x04007E05 RID: 32261
		[SerializeField]
		private float fullEffectDistance = 1f;

		// Token: 0x04007E06 RID: 32262
		[SerializeField]
		private float fadeDistance = 0.5f;

		// Token: 0x04007E07 RID: 32263
		[SerializeField]
		private float currentSpeed = 1f;

		// Token: 0x04007E08 RID: 32264
		[SerializeField]
		private float currentAccel = 10f;

		// Token: 0x04007E09 RID: 32265
		[SerializeField]
		private float velocityAnticipationAdjustment = 0.05f;

		// Token: 0x04007E0A RID: 32266
		[SerializeField]
		private float inwardCurrentFullEffectRadius = 1f;

		// Token: 0x04007E0B RID: 32267
		[SerializeField]
		private float inwardCurrentNoEffectRadius = 0.25f;

		// Token: 0x04007E0C RID: 32268
		[SerializeField]
		private float inwardCurrentSpeed = 1f;

		// Token: 0x04007E0D RID: 32269
		[SerializeField]
		private float inwardCurrentAccel = 10f;

		// Token: 0x04007E0E RID: 32270
		[SerializeField]
		private float dampingHalfLife = 0.25f;

		// Token: 0x04007E0F RID: 32271
		[SerializeField]
		private bool debugDrawCurrentQueries;

		// Token: 0x04007E10 RID: 32272
		private Vector3 debugCurrentVelocity = Vector3.zero;

		// Token: 0x04007E11 RID: 32273
		private Vector3 debugSplinePoint = Vector3.zero;
	}
}
