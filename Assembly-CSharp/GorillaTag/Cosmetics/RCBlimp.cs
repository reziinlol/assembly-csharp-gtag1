using System;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200121D RID: 4637
	public class RCBlimp : RCVehicle
	{
		// Token: 0x060073F9 RID: 29689 RVA: 0x0025D5C4 File Offset: 0x0025B7C4
		protected override void AuthorityBeginDocked()
		{
			base.AuthorityBeginDocked();
			this.turnRate = 0f;
			this.turnAngle = Vector3.SignedAngle(Vector3.forward, Vector3.ProjectOnPlane(base.transform.forward, Vector3.up), Vector3.up);
			this.motorLevel = 0f;
			if (this.connectedRemote == null)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060073FA RID: 29690 RVA: 0x0025D634 File Offset: 0x0025B834
		protected override void Awake()
		{
			base.Awake();
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
			this.tiltAccel = this.maxHorizontalTiltAngle / this.horizontalTiltTime;
		}

		// Token: 0x060073FB RID: 29691 RVA: 0x0025D693 File Offset: 0x0025B893
		protected override void OnDisable()
		{
			base.OnDisable();
			this.audioSource.GTStop();
		}

		// Token: 0x060073FC RID: 29692 RVA: 0x0025D6A8 File Offset: 0x0025B8A8
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = Mathf.Max(Mathf.Max(Mathf.Abs(this.activeInput.joystick.y), Mathf.Abs(this.activeInput.joystick.x)), this.activeInput.trigger);
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
			}
		}

		// Token: 0x060073FD RID: 29693 RVA: 0x0025D750 File Offset: 0x0025B950
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.localState == RCVehicle.State.Mobilized && this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
			}
		}

		// Token: 0x060073FE RID: 29694 RVA: 0x0025D7A0 File Offset: 0x0025B9A0
		protected override void SharedUpdate(float dt)
		{
			base.SharedUpdate(dt);
			switch (this.localState)
			{
			case RCVehicle.State.Disabled:
				break;
			case RCVehicle.State.DockedLeft:
			case RCVehicle.State.DockedRight:
				if (this.localStatePrev != RCVehicle.State.DockedLeft && this.localStatePrev != RCVehicle.State.DockedRight)
				{
					this.audioSource.GTStop();
					this.blimpDeflateBlendWeight = 0f;
					this.blimpMesh.SetBlendShapeWeight(0, 0f);
					this.crashCollider.enabled = false;
				}
				this.leftPropellerSpinRate = Mathf.MoveTowards(this.leftPropellerSpinRate, 0.6f, 6.6666665f * dt);
				this.rightPropellerSpinRate = Mathf.MoveTowards(this.rightPropellerSpinRate, 0.6f, 6.6666665f * dt);
				this.leftPropellerAngle += this.leftPropellerSpinRate * 360f * dt;
				this.rightPropellerAngle += this.rightPropellerSpinRate * 360f * dt;
				this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.leftPropellerAngle, 0f, -90f));
				this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.rightPropellerAngle, 0f, 90f));
				return;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.audioSource.loop = true;
					this.audioSource.clip = this.motorSound;
					this.audioSource.volume = 0f;
					this.audioSource.GTPlay();
					this.blimpDeflateBlendWeight = 0f;
					this.blimpMesh.SetBlendShapeWeight(0, 0f);
					this.crashCollider.enabled = false;
				}
				float target = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, target, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				this.blimpDeflateBlendWeight = 0f;
				float num = this.activeInput.joystick.y * 5f;
				float num2 = this.activeInput.joystick.x * 5f;
				float target2 = Mathf.Clamp(num2 + num + 0.6f, -5f, 5f);
				float target3 = Mathf.Clamp(-num2 + num + 0.6f, -5f, 5f);
				this.leftPropellerSpinRate = Mathf.MoveTowards(this.leftPropellerSpinRate, target2, 6.6666665f * dt);
				this.rightPropellerSpinRate = Mathf.MoveTowards(this.rightPropellerSpinRate, target3, 6.6666665f * dt);
				this.leftPropellerAngle += this.leftPropellerSpinRate * 360f * dt;
				this.rightPropellerAngle += this.rightPropellerSpinRate * 360f * dt;
				this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.leftPropellerAngle, 0f, -90f));
				this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.rightPropellerAngle, 0f, 90f));
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.audioSource.GTStop();
					this.audioSource.clip = null;
					this.audioSource.loop = false;
					this.audioSource.volume = this.deflateSoundVolume;
					if (this.deflateSound != null)
					{
						this.audioSource.GTPlayOneShot(this.deflateSound, 1f);
					}
					this.leftPropellerSpinRate = 0f;
					this.rightPropellerSpinRate = 0f;
					this.leftPropellerAngle = 0f;
					this.rightPropellerAngle = 0f;
					this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
					this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
					this.crashCollider.enabled = true;
				}
				this.blimpDeflateBlendWeight = Mathf.Lerp(1f, this.blimpDeflateBlendWeight, Mathf.Exp(-this.deflateRate * dt));
				this.blimpMesh.SetBlendShapeWeight(0, this.blimpDeflateBlendWeight * 100f);
				return;
			default:
				return;
			}
		}

		// Token: 0x060073FF RID: 29695 RVA: 0x0025DBF0 File Offset: 0x0025BDF0
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority)
			{
				return;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			float x = base.transform.lossyScale.x;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = this.maxAscendSpeed * x;
				float num2 = this.maxHorizontalSpeed * x;
				float d = this.ascendAccel * x;
				Vector3 linearVelocity = this.rb.linearVelocity;
				Vector3 normalized = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z).normalized;
				this.turnAngle = Vector3.SignedAngle(Vector3.forward, normalized, Vector3.up);
				this.tiltAngle = Vector3.SignedAngle(normalized, base.transform.forward, base.transform.right);
				float target = this.activeInput.joystick.x * this.maxTurnRate;
				this.turnRate = Mathf.MoveTowards(this.turnRate, target, this.turnAccel * fixedDeltaTime);
				this.turnAngle += this.turnRate * fixedDeltaTime;
				float value = Vector3.Dot(normalized, linearVelocity);
				float t = Mathf.InverseLerp(-num2, num2, value);
				float target2 = Mathf.Lerp(-this.maxHorizontalTiltAngle, this.maxHorizontalTiltAngle, t);
				this.tiltAngle = Mathf.MoveTowards(this.tiltAngle, target2, this.tiltAccel * fixedDeltaTime);
				base.transform.rotation = Quaternion.Euler(new Vector3(this.tiltAngle, this.turnAngle, 0f));
				Vector3 b = new Vector3(linearVelocity.x, 0f, linearVelocity.z);
				Vector3 a = Vector3.Lerp(normalized * this.activeInput.joystick.y * num2, b, Mathf.Exp(-this.horizontalAccelTime * fixedDeltaTime));
				this.rb.AddForce((a - b) * this.rb.mass, ForceMode.Impulse);
				float num3 = this.activeInput.trigger * num;
				if (num3 > 0.01f && linearVelocity.y < num3)
				{
					this.rb.AddForce(Vector3.up * d * this.rb.mass, ForceMode.Force);
				}
				if (this.rb.useGravity)
				{
					RCVehicle.AddScaledGravityCompensationForce(this.rb, x, this.gravityCompensation);
					return;
				}
			}
			else if (this.localState == RCVehicle.State.Crashed && this.rb.useGravity)
			{
				RCVehicle.AddScaledGravityCompensationForce(this.rb, x, this.crashedGravityCompensation);
			}
		}

		// Token: 0x06007400 RID: 29696 RVA: 0x0025DE80 File Offset: 0x0025C080
		private void OnTriggerEnter(Collider other)
		{
			bool flag = other.gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
			bool flag2 = other.gameObject.IsOnLayer(UnityLayer.GorillaHand);
			if (!other.isTrigger && base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginCrash();
				return;
			}
			if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
			{
				Vector3 vector = Vector3.zero;
				if (flag2)
				{
					GorillaHandClimber component = other.gameObject.GetComponent<GorillaHandClimber>();
					if (component != null)
					{
						vector = GTPlayer.Instance.GetHandVelocityTracker(component.xrNode == XRNode.LeftHand).GetAverageVelocity(true, 0.15f, false);
					}
				}
				else if (other.attachedRigidbody != null)
				{
					vector = other.attachedRigidbody.linearVelocity;
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

		// Token: 0x040084AA RID: 33962
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x040084AB RID: 33963
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x040084AC RID: 33964
		[SerializeField]
		private float gravityCompensation = 0.9f;

		// Token: 0x040084AD RID: 33965
		[SerializeField]
		private float crashedGravityCompensation = 0.5f;

		// Token: 0x040084AE RID: 33966
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x040084AF RID: 33967
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x040084B0 RID: 33968
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x040084B1 RID: 33969
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x040084B2 RID: 33970
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x040084B3 RID: 33971
		[SerializeField]
		private float horizontalTiltTime = 2f;

		// Token: 0x040084B4 RID: 33972
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.1f, 0.8f);

		// Token: 0x040084B5 RID: 33973
		[SerializeField]
		private float deflateSoundVolume = 0.1f;

		// Token: 0x040084B6 RID: 33974
		[SerializeField]
		private Collider crashCollider;

		// Token: 0x040084B7 RID: 33975
		[SerializeField]
		private Transform leftPropeller;

		// Token: 0x040084B8 RID: 33976
		[SerializeField]
		private Transform rightPropeller;

		// Token: 0x040084B9 RID: 33977
		[SerializeField]
		private SkinnedMeshRenderer blimpMesh;

		// Token: 0x040084BA RID: 33978
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040084BB RID: 33979
		[SerializeField]
		private AudioClip motorSound;

		// Token: 0x040084BC RID: 33980
		[SerializeField]
		private AudioClip deflateSound;

		// Token: 0x040084BD RID: 33981
		private float turnRate;

		// Token: 0x040084BE RID: 33982
		private float turnAngle;

		// Token: 0x040084BF RID: 33983
		private float tiltAngle;

		// Token: 0x040084C0 RID: 33984
		private float ascendAccel;

		// Token: 0x040084C1 RID: 33985
		private float turnAccel;

		// Token: 0x040084C2 RID: 33986
		private float tiltAccel;

		// Token: 0x040084C3 RID: 33987
		private float horizontalAccel;

		// Token: 0x040084C4 RID: 33988
		private float leftPropellerAngle;

		// Token: 0x040084C5 RID: 33989
		private float rightPropellerAngle;

		// Token: 0x040084C6 RID: 33990
		private float leftPropellerSpinRate;

		// Token: 0x040084C7 RID: 33991
		private float rightPropellerSpinRate;

		// Token: 0x040084C8 RID: 33992
		private float blimpDeflateBlendWeight;

		// Token: 0x040084C9 RID: 33993
		private float deflateRate = Mathf.Exp(1f);

		// Token: 0x040084CA RID: 33994
		private const float propellerIdleAcc = 1f;

		// Token: 0x040084CB RID: 33995
		private const float propellerIdleSpinRate = 0.6f;

		// Token: 0x040084CC RID: 33996
		private const float propellerMaxAcc = 6.6666665f;

		// Token: 0x040084CD RID: 33997
		private const float propellerMaxSpinRate = 5f;

		// Token: 0x040084CE RID: 33998
		private float motorVolumeRampTime = 1f;

		// Token: 0x040084CF RID: 33999
		private float motorLevel;
	}
}
