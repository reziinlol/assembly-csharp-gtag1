using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200134F RID: 4943
	public class UFOController : MonoBehaviour
	{
		// Token: 0x06007C85 RID: 31877 RVA: 0x0028D6B4 File Offset: 0x0028B8B4
		private void Start()
		{
			this.m_linearVelocity = Vector3.zero;
			this.m_angularVelocity = 0f;
			this.m_yawAngle = base.transform.rotation.eulerAngles.y * MathUtil.Deg2Rad;
			this.m_hoverCenter = base.transform.position;
			this.m_hoverPhase = 0f;
			this.m_motorAngle = 0f;
			if (this.Eyes != null)
			{
				this.m_eyeInitScale = this.Eyes.localScale;
				this.m_eyeInitPositionLs = this.Eyes.localPosition;
				this.m_blinkTimer = this.BlinkInterval + Random.Range(1f, 2f);
				this.m_lastBlinkWasDouble = false;
				this.m_eyeScaleSpring.Reset(this.m_eyeInitScale);
				this.m_eyePositionLsSpring.Reset(this.m_eyeInitPositionLs);
			}
		}

		// Token: 0x06007C86 RID: 31878 RVA: 0x0028D797 File Offset: 0x0028B997
		private void OnEnable()
		{
			this.Start();
		}

		// Token: 0x06007C87 RID: 31879 RVA: 0x0028D7A0 File Offset: 0x0028B9A0
		private void FixedUpdate()
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			Vector3 a = Vector3.zero;
			if (Input.GetKey(KeyCode.W))
			{
				a += Vector3.forward;
			}
			if (Input.GetKey(KeyCode.S))
			{
				a += Vector3.back;
			}
			if (Input.GetKey(KeyCode.A))
			{
				a += Vector3.left;
			}
			if (Input.GetKey(KeyCode.D))
			{
				a += Vector3.right;
			}
			if (Input.GetKey(KeyCode.R))
			{
				a += Vector3.up;
			}
			if (Input.GetKey(KeyCode.F))
			{
				a += Vector3.down;
			}
			if (a.sqrMagnitude > MathUtil.Epsilon)
			{
				a = a.normalized * this.LinearThrust;
				this.m_linearVelocity += a * fixedDeltaTime;
				this.m_linearVelocity = VectorUtil.ClampLength(this.m_linearVelocity, 0f, this.MaxLinearSpeed);
			}
			else
			{
				this.m_linearVelocity = VectorUtil.ClampLength(this.m_linearVelocity, 0f, Mathf.Max(0f, this.m_linearVelocity.magnitude - this.LinearDrag * fixedDeltaTime));
			}
			float magnitude = this.m_linearVelocity.magnitude;
			float t = magnitude * MathUtil.InvSafe(this.MaxLinearSpeed);
			Quaternion lhs = Quaternion.identity;
			float num = 0f;
			if (magnitude > MathUtil.Epsilon)
			{
				Vector3 linearVelocity = this.m_linearVelocity;
				linearVelocity.y = 0f;
				float num2 = (this.m_linearVelocity.magnitude > 0.01f) ? (1f - Mathf.Clamp01(Mathf.Abs(this.m_linearVelocity.y) / this.m_linearVelocity.magnitude)) : 0f;
				num = Mathf.Min(1f, magnitude / Mathf.Max(MathUtil.Epsilon, this.MaxLinearSpeed)) * num2;
				Vector3 normalized = Vector3.Cross(Vector3.up, linearVelocity).normalized;
				float angle = this.Tilt * MathUtil.Deg2Rad * num;
				lhs = QuaternionUtil.AxisAngle(normalized, angle);
			}
			float num3 = 0f;
			if (Input.GetKey(KeyCode.Q))
			{
				num3 -= 1f;
			}
			if (Input.GetKey(KeyCode.E))
			{
				num3 += 1f;
			}
			bool key = Input.GetKey(KeyCode.LeftControl);
			if (Mathf.Abs(num3) > MathUtil.Epsilon)
			{
				float num4 = this.MaxAngularSpeed * (key ? 2.5f : 1f);
				num3 *= this.AngularThrust * MathUtil.Deg2Rad;
				this.m_angularVelocity += num3 * fixedDeltaTime;
				this.m_angularVelocity = Mathf.Clamp(this.m_angularVelocity, -num4 * MathUtil.Deg2Rad, num4 * MathUtil.Deg2Rad);
			}
			else
			{
				this.m_angularVelocity -= Mathf.Sign(this.m_angularVelocity) * Mathf.Min(Mathf.Abs(this.m_angularVelocity), this.AngularDrag * MathUtil.Deg2Rad * fixedDeltaTime);
			}
			this.m_yawAngle += this.m_angularVelocity * fixedDeltaTime;
			Quaternion rhs = QuaternionUtil.AxisAngle(Vector3.up, this.m_yawAngle);
			this.m_hoverCenter += this.m_linearVelocity * fixedDeltaTime;
			this.m_hoverPhase += Time.deltaTime;
			Vector3 vector = 0.05f * Mathf.Sin(1.37f * this.m_hoverPhase) * Vector3.right + 0.05f * Mathf.Sin(1.93f * this.m_hoverPhase + 1.234f) * Vector3.forward + 0.04f * Mathf.Sin(0.97f * this.m_hoverPhase + 4.321f) * Vector3.up;
			vector *= this.Hover;
			Quaternion rhs2 = Quaternion.FromToRotation(Vector3.up, vector + Vector3.up);
			base.transform.position = this.m_hoverCenter + vector;
			base.transform.rotation = lhs * rhs * rhs2;
			if (this.Motor != null)
			{
				float num5 = Mathf.Lerp(this.MotorBaseAngularSpeed, this.MotorMaxAngularSpeed, num);
				this.m_motorAngle += num5 * MathUtil.Deg2Rad * fixedDeltaTime;
				this.Motor.localRotation = QuaternionUtil.AxisAngle(Vector3.up, this.m_motorAngle - this.m_yawAngle);
			}
			if (this.BubbleEmitter != null)
			{
				this.BubbleEmitter.emission.rateOverTime = Mathf.Lerp(this.BubbleBaseEmissionRate, this.BubbleMaxEmissionRate, t);
			}
			if (this.Eyes != null)
			{
				this.m_blinkTimer -= fixedDeltaTime;
				if (this.m_blinkTimer <= 0f)
				{
					bool flag = !this.m_lastBlinkWasDouble && Random.Range(0f, 1f) > 0.75f;
					this.m_blinkTimer = (flag ? 0.2f : (this.BlinkInterval + Random.Range(1f, 2f)));
					this.m_lastBlinkWasDouble = flag;
					this.m_eyeScaleSpring.Value.y = 0f;
					this.m_eyePositionLsSpring.Value.y = this.m_eyePositionLsSpring.Value.y - 0.025f;
				}
				this.Eyes.localScale = this.m_eyeScaleSpring.TrackDampingRatio(this.m_eyeInitScale, 30f, 0.8f, fixedDeltaTime);
				this.Eyes.localPosition = this.m_eyePositionLsSpring.TrackDampingRatio(this.m_eyeInitPositionLs, 30f, 0.8f, fixedDeltaTime);
			}
		}

		// Token: 0x04008D97 RID: 36247
		public float LinearThrust = 3f;

		// Token: 0x04008D98 RID: 36248
		public float MaxLinearSpeed = 2.5f;

		// Token: 0x04008D99 RID: 36249
		public float LinearDrag = 4f;

		// Token: 0x04008D9A RID: 36250
		public float Tilt = 15f;

		// Token: 0x04008D9B RID: 36251
		public float AngularThrust = 30f;

		// Token: 0x04008D9C RID: 36252
		public float MaxAngularSpeed = 30f;

		// Token: 0x04008D9D RID: 36253
		public float AngularDrag = 30f;

		// Token: 0x04008D9E RID: 36254
		[Range(0f, 1f)]
		public float Hover = 1f;

		// Token: 0x04008D9F RID: 36255
		public Transform Eyes;

		// Token: 0x04008DA0 RID: 36256
		public float BlinkInterval = 5f;

		// Token: 0x04008DA1 RID: 36257
		private float m_blinkTimer;

		// Token: 0x04008DA2 RID: 36258
		private bool m_lastBlinkWasDouble;

		// Token: 0x04008DA3 RID: 36259
		private Vector3 m_eyeInitScale;

		// Token: 0x04008DA4 RID: 36260
		private Vector3 m_eyeInitPositionLs;

		// Token: 0x04008DA5 RID: 36261
		private Vector3Spring m_eyeScaleSpring;

		// Token: 0x04008DA6 RID: 36262
		private Vector3Spring m_eyePositionLsSpring;

		// Token: 0x04008DA7 RID: 36263
		public Transform Motor;

		// Token: 0x04008DA8 RID: 36264
		public float MotorBaseAngularSpeed = 10f;

		// Token: 0x04008DA9 RID: 36265
		public float MotorMaxAngularSpeed = 10f;

		// Token: 0x04008DAA RID: 36266
		public ParticleSystem BubbleEmitter;

		// Token: 0x04008DAB RID: 36267
		public float BubbleBaseEmissionRate = 10f;

		// Token: 0x04008DAC RID: 36268
		public float BubbleMaxEmissionRate = 10f;

		// Token: 0x04008DAD RID: 36269
		private Vector3 m_linearVelocity;

		// Token: 0x04008DAE RID: 36270
		private float m_angularVelocity;

		// Token: 0x04008DAF RID: 36271
		private float m_yawAngle;

		// Token: 0x04008DB0 RID: 36272
		private Vector3 m_hoverCenter;

		// Token: 0x04008DB1 RID: 36273
		private float m_hoverPhase;

		// Token: 0x04008DB2 RID: 36274
		private float m_motorAngle;
	}
}
