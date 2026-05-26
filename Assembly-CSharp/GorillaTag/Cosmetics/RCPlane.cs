using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001222 RID: 4642
	public class RCPlane : RCVehicle
	{
		// Token: 0x0600741B RID: 29723 RVA: 0x0025F3F4 File Offset: 0x0025D5F4
		protected override void Awake()
		{
			base.Awake();
			this.pitchAccelMinMax.x = this.pitchVelocityTargetMinMax.x / this.pitchVelocityRampTimeMinMax.x;
			this.pitchAccelMinMax.y = this.pitchVelocityTargetMinMax.y / this.pitchVelocityRampTimeMinMax.y;
			this.rollAccel = this.rollVelocityTarget / this.rollVelocityRampTime;
			this.thrustAccel = this.thrustVelocityTarget / this.thrustAccelTime;
		}

		// Token: 0x0600741C RID: 29724 RVA: 0x0025F474 File Offset: 0x0025D674
		protected override void AuthorityBeginMobilization()
		{
			base.AuthorityBeginMobilization();
			float x = base.transform.lossyScale.x;
			this.rb.linearVelocity = base.transform.forward * this.initialSpeed * x;
		}

		// Token: 0x0600741D RID: 29725 RVA: 0x0025F4C0 File Offset: 0x0025D6C0
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = this.activeInput.trigger;
			}
			this.leftAileronLevel = 0f;
			this.rightAileronLevel = 0f;
			float magnitude = this.activeInput.joystick.magnitude;
			if (magnitude > 0.01f)
			{
				float num = Mathf.Abs(this.activeInput.joystick.x) / magnitude;
				float num2 = Mathf.Abs(this.activeInput.joystick.y) / magnitude;
				this.leftAileronLevel = Mathf.Clamp(num * this.activeInput.joystick.x + num2 * -this.activeInput.joystick.y, -1f, 1f);
				this.rightAileronLevel = Mathf.Clamp(num * this.activeInput.joystick.x + num2 * this.activeInput.joystick.y, -1f, 1f);
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
				this.networkSync.syncedState.dataB = (byte)Mathf.Clamp(Mathf.FloorToInt(this.leftAileronLevel * 126f), -126, 126);
				this.networkSync.syncedState.dataC = (byte)Mathf.Clamp(Mathf.FloorToInt(this.rightAileronLevel * 126f), -126, 126);
			}
		}

		// Token: 0x0600741E RID: 29726 RVA: 0x0025F664 File Offset: 0x0025D864
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
				this.leftAileronLevel = Mathf.Clamp((float)this.networkSync.syncedState.dataB / 126f, -1f, 1f);
				this.rightAileronLevel = Mathf.Clamp((float)this.networkSync.syncedState.dataC / 126f, -1f, 1f);
			}
		}

		// Token: 0x0600741F RID: 29727 RVA: 0x0025F700 File Offset: 0x0025D900
		protected override void SharedUpdate(float dt)
		{
			base.SharedUpdate(dt);
			switch (this.localState)
			{
			case RCVehicle.State.DockedLeft:
			case RCVehicle.State.DockedRight:
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 0.6f, 6.6666665f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.audioSource.loop = true;
					this.audioSource.clip = this.motorSound;
					this.audioSource.volume = 0f;
					this.audioSource.GTPlay();
				}
				float target = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, target, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 5f, 6.6666665f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.audioSource.GTStop();
					this.audioSource.clip = null;
					this.audioSource.loop = false;
					this.audioSource.volume = this.crashSoundVolume;
					if (this.crashSound != null)
					{
						this.audioSource.GTPlayOneShot(this.crashSound, 1f);
					}
				}
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 0f, 13.333333f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			}
			float target2 = Mathf.Lerp(this.aileronAngularRange.x, this.aileronAngularRange.y, Mathf.InverseLerp(-1f, 1f, this.leftAileronLevel));
			float target3 = Mathf.Lerp(this.aileronAngularRange.x, this.aileronAngularRange.y, Mathf.InverseLerp(-1f, 1f, this.rightAileronLevel));
			this.leftAileronAngle = Mathf.MoveTowards(this.leftAileronAngle, target2, this.aileronAngularAcc * Time.deltaTime);
			this.rightAileronAngle = Mathf.MoveTowards(this.rightAileronAngle, target3, this.aileronAngularAcc * Time.deltaTime);
			Quaternion localRotation = Quaternion.Euler(0f, -90f, 90f + this.leftAileronAngle);
			Quaternion localRotation2 = Quaternion.Euler(0f, 90f, -90f + this.rightAileronAngle);
			this.leftAileronLower.localRotation = localRotation;
			this.leftAileronUpper.localRotation = localRotation;
			this.rightAileronLower.localRotation = localRotation2;
			this.rightAileronUpper.localRotation = localRotation2;
		}

		// Token: 0x06007420 RID: 29728 RVA: 0x0025FA68 File Offset: 0x0025DC68
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
			{
				return;
			}
			float x = base.transform.lossyScale.x;
			float num = this.thrustVelocityTarget * x;
			float num2 = this.thrustAccel * x;
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.pitch = base.NormalizeAngle180(this.pitch);
			this.roll = base.NormalizeAngle180(this.roll);
			float num3 = this.pitch;
			float num4 = this.roll;
			if (this.activeInput.joystick.y >= 0f)
			{
				float target = this.activeInput.joystick.y * this.pitchVelocityTargetMinMax.y;
				this.pitchVel = Mathf.MoveTowards(this.pitchVel, target, this.pitchAccelMinMax.y * fixedDeltaTime);
				this.pitch += this.pitchVel * fixedDeltaTime;
			}
			else
			{
				float target2 = -this.activeInput.joystick.y * this.pitchVelocityTargetMinMax.x;
				this.pitchVel = Mathf.MoveTowards(this.pitchVel, target2, this.pitchAccelMinMax.x * fixedDeltaTime);
				this.pitch += this.pitchVel * fixedDeltaTime;
			}
			float target3 = -this.activeInput.joystick.x * this.rollVelocityTarget;
			this.rollVel = Mathf.MoveTowards(this.rollVel, target3, this.rollAccel * fixedDeltaTime);
			this.roll += this.rollVel * fixedDeltaTime;
			Quaternion rhs = Quaternion.Euler(new Vector3(this.pitch - num3, 0f, this.roll - num4));
			base.transform.rotation = base.transform.rotation * rhs;
			this.rb.angularVelocity = Vector3.zero;
			Vector3 linearVelocity = this.rb.linearVelocity;
			float magnitude = linearVelocity.magnitude;
			float num5 = Mathf.Max(Vector3.Dot(base.transform.forward, linearVelocity), 0f);
			float num6 = this.activeInput.trigger * num;
			float num7 = 0.1f * x;
			if (num6 > num7 && num6 > num5)
			{
				float num8 = Mathf.MoveTowards(num5, num6, num2 * fixedDeltaTime);
				this.rb.AddForce(base.transform.forward * (num8 - num5) * this.rb.mass, ForceMode.Impulse);
			}
			float b = 0.01f * x;
			float time = Vector3.Dot(linearVelocity / Mathf.Max(magnitude, b), base.transform.forward);
			float num9 = this.liftVsAttackCurve.Evaluate(time);
			float num10 = Mathf.Lerp(this.liftVsSpeedOutput.x, this.liftVsSpeedOutput.y, Mathf.InverseLerp(this.liftVsSpeedInput.x, this.liftVsSpeedInput.y, magnitude / x));
			float d = num9 * num10;
			Vector3 a = Vector3.RotateTowards(linearVelocity, base.transform.forward * magnitude, this.pitchVelocityFollowRateAngle * 0.017453292f * fixedDeltaTime, this.pitchVelocityFollowRateMagnitude * fixedDeltaTime) - linearVelocity;
			this.rb.AddForce(a * d * this.rb.mass, ForceMode.Impulse);
			float time2 = Vector3.Dot(linearVelocity.normalized, base.transform.up);
			float d2 = this.dragVsAttackCurve.Evaluate(time2);
			this.rb.AddForce(-linearVelocity * this.maxDrag * d2 * this.rb.mass, ForceMode.Force);
			if (this.rb.useGravity)
			{
				float gravityCompensation = Mathf.Lerp(this.gravityCompensationRange.x, this.gravityCompensationRange.y, Mathf.InverseLerp(0f, num, num5 / x));
				RCVehicle.AddScaledGravityCompensationForce(this.rb, x, gravityCompensation);
			}
		}

		// Token: 0x06007421 RID: 29729 RVA: 0x0025FE50 File Offset: 0x0025E050
		private void OnCollisionEnter(Collision collision)
		{
			if (base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				for (int i = 0; i < collision.contactCount; i++)
				{
					ContactPoint contact = collision.GetContact(i);
					if (!this.nonCrashColliders.Contains(contact.thisCollider))
					{
						this.AuthorityBeginCrash();
					}
				}
				return;
			}
			bool flag = collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
			bool flag2 = collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaHand);
			if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
			{
				Vector3 vector = Vector3.zero;
				if (flag2)
				{
					GorillaHandClimber component = collision.collider.gameObject.GetComponent<GorillaHandClimber>();
					if (component != null)
					{
						vector = GTPlayer.Instance.GetHandVelocityTracker(component.xrNode == XRNode.LeftHand).GetAverageVelocity(true, 0.15f, false);
					}
				}
				else if (collision.rigidbody != null)
				{
					vector = collision.rigidbody.linearVelocity;
				}
				if (flag || vector.sqrMagnitude > 0.01f)
				{
					if (base.HasLocalAuthority)
					{
						this.AuthorityApplyImpact(vector, flag);
						return;
					}
					if (this.networkSync != null)
					{
						this.networkSync.photonView.RPC("HitRCVehicleRPC", RpcTarget.Others, new object[]
						{
							vector,
							flag
						});
					}
				}
			}
		}

		// Token: 0x04008517 RID: 34071
		public Vector2 pitchVelocityTargetMinMax = new Vector2(-180f, 180f);

		// Token: 0x04008518 RID: 34072
		public Vector2 pitchVelocityRampTimeMinMax = new Vector2(-0.75f, 0.75f);

		// Token: 0x04008519 RID: 34073
		public float rollVelocityTarget = 180f;

		// Token: 0x0400851A RID: 34074
		public float rollVelocityRampTime = 0.75f;

		// Token: 0x0400851B RID: 34075
		public float thrustVelocityTarget = 15f;

		// Token: 0x0400851C RID: 34076
		public float thrustAccelTime = 2f;

		// Token: 0x0400851D RID: 34077
		[SerializeField]
		private float pitchVelocityFollowRateAngle = 60f;

		// Token: 0x0400851E RID: 34078
		[SerializeField]
		private float pitchVelocityFollowRateMagnitude = 5f;

		// Token: 0x0400851F RID: 34079
		[SerializeField]
		private float maxDrag = 0.1f;

		// Token: 0x04008520 RID: 34080
		[SerializeField]
		private Vector2 liftVsSpeedInput = new Vector2(0f, 4f);

		// Token: 0x04008521 RID: 34081
		[SerializeField]
		private Vector2 liftVsSpeedOutput = new Vector2(0.5f, 1f);

		// Token: 0x04008522 RID: 34082
		[SerializeField]
		private AnimationCurve liftVsAttackCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04008523 RID: 34083
		[SerializeField]
		private AnimationCurve dragVsAttackCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04008524 RID: 34084
		[SerializeField]
		private Vector2 gravityCompensationRange = new Vector2(0.5f, 1f);

		// Token: 0x04008525 RID: 34085
		[SerializeField]
		private List<Collider> nonCrashColliders = new List<Collider>();

		// Token: 0x04008526 RID: 34086
		[SerializeField]
		private Transform propeller;

		// Token: 0x04008527 RID: 34087
		[SerializeField]
		private Transform leftAileronUpper;

		// Token: 0x04008528 RID: 34088
		[SerializeField]
		private Transform leftAileronLower;

		// Token: 0x04008529 RID: 34089
		[SerializeField]
		private Transform rightAileronUpper;

		// Token: 0x0400852A RID: 34090
		[SerializeField]
		private Transform rightAileronLower;

		// Token: 0x0400852B RID: 34091
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x0400852C RID: 34092
		[SerializeField]
		private AudioClip motorSound;

		// Token: 0x0400852D RID: 34093
		[SerializeField]
		private AudioClip crashSound;

		// Token: 0x0400852E RID: 34094
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.02f, 0.1f);

		// Token: 0x0400852F RID: 34095
		[SerializeField]
		private float crashSoundVolume = 0.12f;

		// Token: 0x04008530 RID: 34096
		private float motorVolumeRampTime = 1f;

		// Token: 0x04008531 RID: 34097
		private float propellerAngle;

		// Token: 0x04008532 RID: 34098
		private float propellerSpinRate;

		// Token: 0x04008533 RID: 34099
		private const float propellerIdleAcc = 1f;

		// Token: 0x04008534 RID: 34100
		private const float propellerIdleSpinRate = 0.6f;

		// Token: 0x04008535 RID: 34101
		private const float propellerMaxAcc = 6.6666665f;

		// Token: 0x04008536 RID: 34102
		private const float propellerMaxSpinRate = 5f;

		// Token: 0x04008537 RID: 34103
		public float initialSpeed = 3f;

		// Token: 0x04008538 RID: 34104
		private float pitch;

		// Token: 0x04008539 RID: 34105
		private float pitchVel;

		// Token: 0x0400853A RID: 34106
		private Vector2 pitchAccelMinMax;

		// Token: 0x0400853B RID: 34107
		private float roll;

		// Token: 0x0400853C RID: 34108
		private float rollVel;

		// Token: 0x0400853D RID: 34109
		private float rollAccel;

		// Token: 0x0400853E RID: 34110
		private float thrustAccel;

		// Token: 0x0400853F RID: 34111
		private float motorLevel;

		// Token: 0x04008540 RID: 34112
		private float leftAileronLevel;

		// Token: 0x04008541 RID: 34113
		private float rightAileronLevel;

		// Token: 0x04008542 RID: 34114
		private Vector2 aileronAngularRange = new Vector2(-30f, 45f);

		// Token: 0x04008543 RID: 34115
		private float aileronAngularAcc = 120f;

		// Token: 0x04008544 RID: 34116
		private float leftAileronAngle;

		// Token: 0x04008545 RID: 34117
		private float rightAileronAngle;
	}
}
