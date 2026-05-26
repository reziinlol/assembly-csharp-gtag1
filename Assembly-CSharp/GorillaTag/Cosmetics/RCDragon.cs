using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001220 RID: 4640
	public class RCDragon : RCVehicle
	{
		// Token: 0x06007407 RID: 29703 RVA: 0x0025E3E4 File Offset: 0x0025C5E4
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

		// Token: 0x06007408 RID: 29704 RVA: 0x0025E454 File Offset: 0x0025C654
		protected override void Awake()
		{
			base.Awake();
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
			this.tiltAccel = this.maxHorizontalTiltAngle / this.horizontalTiltTime;
			this.shouldFlap = false;
			this.isFlapping = false;
			this.StopBreathFire();
			if (this.animation != null)
			{
				this.animation[this.wingFlapAnimName].speed = this.wingFlapAnimSpeed;
				this.animation[this.crashAnimName].speed = this.crashAnimSpeed;
				this.animation[this.mouthClosedAnimName].layer = 1;
				this.animation[this.mouthBreathFireAnimName].layer = 1;
			}
			this.nextFlapEventAnimTime = this.flapAnimEventTime;
		}

		// Token: 0x06007409 RID: 29705 RVA: 0x0025E547 File Offset: 0x0025C747
		protected override void OnDisable()
		{
			base.OnDisable();
			this.audioSource.GTStop();
		}

		// Token: 0x0600740A RID: 29706 RVA: 0x0025E55C File Offset: 0x0025C75C
		public void StartBreathFire()
		{
			if (!string.IsNullOrEmpty(this.mouthBreathFireAnimName))
			{
				this.animation.CrossFade(this.mouthBreathFireAnimName, 0.1f);
			}
			if (this.fireBreath != null)
			{
				this.fireBreath.SetActive(true);
			}
			this.PlayRandomSound(this.breathFireSound, this.breathFireVolume);
			this.fireBreathTimeRemaining = this.fireBreathDuration;
		}

		// Token: 0x0600740B RID: 29707 RVA: 0x0025E5C4 File Offset: 0x0025C7C4
		public void StopBreathFire()
		{
			if (!string.IsNullOrEmpty(this.mouthClosedAnimName))
			{
				this.animation.CrossFade(this.mouthClosedAnimName, 0.1f);
			}
			if (this.fireBreath != null)
			{
				this.fireBreath.SetActive(false);
			}
			this.fireBreathTimeRemaining = -1f;
		}

		// Token: 0x0600740C RID: 29708 RVA: 0x0025E619 File Offset: 0x0025C819
		public bool IsBreathingFire()
		{
			return this.fireBreathTimeRemaining >= 0f;
		}

		// Token: 0x0600740D RID: 29709 RVA: 0x0025E62B File Offset: 0x0025C82B
		private void PlayRandomSound(List<AudioClip> clips, float volume)
		{
			if (clips == null || clips.Count == 0)
			{
				return;
			}
			this.PlaySound(clips[Random.Range(0, clips.Count)], volume);
		}

		// Token: 0x0600740E RID: 29710 RVA: 0x0025E654 File Offset: 0x0025C854
		private void PlaySound(AudioClip clip, float volume)
		{
			if (this.audioSource == null || clip == null)
			{
				return;
			}
			this.audioSource.GTStop();
			this.audioSource.clip = null;
			this.audioSource.loop = false;
			this.audioSource.volume = volume;
			this.audioSource.GTPlayOneShot(clip, 1f);
		}

		// Token: 0x0600740F RID: 29711 RVA: 0x0025E6BC File Offset: 0x0025C8BC
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = Mathf.Max(Mathf.Max(Mathf.Abs(this.activeInput.joystick.y), Mathf.Abs(this.activeInput.joystick.x)), this.activeInput.trigger);
				if (!this.IsBreathingFire() && this.activeInput.buttons > 0)
				{
					this.StartBreathFire();
				}
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
				this.networkSync.syncedState.dataB = this.activeInput.buttons;
				this.networkSync.syncedState.dataC = (this.shouldFlap ? 1 : 0);
			}
		}

		// Token: 0x06007410 RID: 29712 RVA: 0x0025E7B8 File Offset: 0x0025C9B8
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.localState == RCVehicle.State.Mobilized && this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
				if (!this.IsBreathingFire() && this.networkSync.syncedState.dataB > 0)
				{
					this.StartBreathFire();
				}
				this.shouldFlap = (this.networkSync.syncedState.dataC > 0);
			}
		}

		// Token: 0x06007411 RID: 29713 RVA: 0x0025E840 File Offset: 0x0025CA40
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
					if (this.crashCollider != null)
					{
						this.crashCollider.enabled = false;
					}
					if (this.animation != null)
					{
						this.animation.Play(this.dockedAnimName);
					}
					if (this.IsBreathingFire())
					{
						this.StopBreathFire();
						return;
					}
				}
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized && this.crashCollider != null)
				{
					this.crashCollider.enabled = false;
				}
				if (this.animation != null)
				{
					if (!this.isFlapping && this.shouldFlap)
					{
						this.animation.CrossFade(this.wingFlapAnimName, 0.1f);
						this.nextFlapEventAnimTime = this.flapAnimEventTime;
					}
					else if (this.isFlapping && !this.shouldFlap)
					{
						this.animation.CrossFade(this.idleAnimName, 0.15f);
					}
					this.isFlapping = this.shouldFlap;
					if (this.isFlapping && !this.IsBreathingFire())
					{
						AnimationState animationState = this.animation[this.wingFlapAnimName];
						if (animationState.normalizedTime * animationState.length > this.nextFlapEventAnimTime)
						{
							this.PlayRandomSound(this.wingFlapSound, this.wingFlapVolume);
							this.nextFlapEventAnimTime = (Mathf.Floor(animationState.normalizedTime) + 1f) * animationState.length + this.flapAnimEventTime;
						}
					}
				}
				GTTime.TimeAsDouble();
				if (this.IsBreathingFire())
				{
					this.fireBreathTimeRemaining -= dt;
					if (this.fireBreathTimeRemaining <= 0f)
					{
						this.StopBreathFire();
					}
				}
				float target = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, target, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.PlaySound(this.crashSound, this.crashSoundVolume);
					if (this.crashCollider != null)
					{
						this.crashCollider.enabled = true;
					}
					if (this.animation != null)
					{
						this.animation.CrossFade(this.crashAnimName, 0.05f);
					}
					if (this.IsBreathingFire())
					{
						this.StopBreathFire();
						return;
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06007412 RID: 29714 RVA: 0x0025EADC File Offset: 0x0025CCDC
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority)
			{
				return;
			}
			float x = base.transform.lossyScale.x;
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.shouldFlap = false;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = this.maxAscendSpeed * x;
				float num2 = this.maxHorizontalSpeed * x;
				float d = this.ascendAccel * x;
				float d2 = this.ascendWhileFlyingAccelBoost * x;
				float num3 = 0.5f * x;
				float num4 = 45f;
				Vector3 linearVelocity = this.rb.linearVelocity;
				Vector3 normalized = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z).normalized;
				this.turnAngle = Vector3.SignedAngle(Vector3.forward, normalized, Vector3.up);
				this.tiltAngle = Vector3.SignedAngle(normalized, base.transform.forward, base.transform.right);
				float target = this.activeInput.joystick.x * this.maxTurnRate;
				this.turnRate = Mathf.MoveTowards(this.turnRate, target, this.turnAccel * fixedDeltaTime);
				this.turnAngle += this.turnRate * fixedDeltaTime;
				float num5 = Vector3.Dot(normalized, linearVelocity);
				float t = Mathf.InverseLerp(-num2, num2, num5);
				float target2 = Mathf.Lerp(-this.maxHorizontalTiltAngle, this.maxHorizontalTiltAngle, t);
				this.tiltAngle = Mathf.MoveTowards(this.tiltAngle, target2, this.tiltAccel * fixedDeltaTime);
				base.transform.rotation = Quaternion.Euler(new Vector3(this.tiltAngle, this.turnAngle, 0f));
				Vector3 b = new Vector3(linearVelocity.x, 0f, linearVelocity.z);
				Vector3 a = Vector3.Lerp(normalized * this.activeInput.joystick.y * num2, b, Mathf.Exp(-this.horizontalAccelTime * fixedDeltaTime));
				this.rb.AddForce((a - b) * this.rb.mass, ForceMode.Impulse);
				float num6 = this.activeInput.trigger * num;
				if (num6 > 0.01f && linearVelocity.y < num6)
				{
					this.rb.AddForce(Vector3.up * d * this.rb.mass, ForceMode.Force);
				}
				bool flag = Mathf.Abs(num5) > num3;
				bool flag2 = Mathf.Abs(this.turnRate) > num4;
				if (flag || flag2)
				{
					this.rb.AddForce(Vector3.up * d2 * this.rb.mass, ForceMode.Force);
				}
				this.shouldFlap = (num6 > 0.01f || flag || flag2);
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

		// Token: 0x06007413 RID: 29715 RVA: 0x0025EDF0 File Offset: 0x0025CFF0
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

		// Token: 0x040084D8 RID: 34008
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x040084D9 RID: 34009
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x040084DA RID: 34010
		[SerializeField]
		private float ascendWhileFlyingAccelBoost;

		// Token: 0x040084DB RID: 34011
		[SerializeField]
		private float gravityCompensation = 0.9f;

		// Token: 0x040084DC RID: 34012
		[SerializeField]
		private float crashedGravityCompensation = 0.5f;

		// Token: 0x040084DD RID: 34013
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x040084DE RID: 34014
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x040084DF RID: 34015
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x040084E0 RID: 34016
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x040084E1 RID: 34017
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x040084E2 RID: 34018
		[SerializeField]
		private float horizontalTiltTime = 2f;

		// Token: 0x040084E3 RID: 34019
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.1f, 0.8f);

		// Token: 0x040084E4 RID: 34020
		[SerializeField]
		private float crashSoundVolume = 0.1f;

		// Token: 0x040084E5 RID: 34021
		[SerializeField]
		private float breathFireVolume = 0.5f;

		// Token: 0x040084E6 RID: 34022
		[SerializeField]
		private float wingFlapVolume = 0.1f;

		// Token: 0x040084E7 RID: 34023
		[SerializeField]
		private Animation animation;

		// Token: 0x040084E8 RID: 34024
		[SerializeField]
		private string wingFlapAnimName;

		// Token: 0x040084E9 RID: 34025
		[SerializeField]
		private float wingFlapAnimSpeed = 1f;

		// Token: 0x040084EA RID: 34026
		[SerializeField]
		private string dockedAnimName;

		// Token: 0x040084EB RID: 34027
		[SerializeField]
		private string idleAnimName;

		// Token: 0x040084EC RID: 34028
		[SerializeField]
		private string crashAnimName;

		// Token: 0x040084ED RID: 34029
		[SerializeField]
		private float crashAnimSpeed = 1f;

		// Token: 0x040084EE RID: 34030
		[SerializeField]
		private string mouthClosedAnimName;

		// Token: 0x040084EF RID: 34031
		[SerializeField]
		private string mouthBreathFireAnimName;

		// Token: 0x040084F0 RID: 34032
		private bool shouldFlap;

		// Token: 0x040084F1 RID: 34033
		private bool isFlapping;

		// Token: 0x040084F2 RID: 34034
		private float nextFlapEventAnimTime;

		// Token: 0x040084F3 RID: 34035
		[SerializeField]
		private float flapAnimEventTime = 0.25f;

		// Token: 0x040084F4 RID: 34036
		[SerializeField]
		private GameObject fireBreath;

		// Token: 0x040084F5 RID: 34037
		[SerializeField]
		private float fireBreathDuration;

		// Token: 0x040084F6 RID: 34038
		private float fireBreathTimeRemaining;

		// Token: 0x040084F7 RID: 34039
		[SerializeField]
		private Collider crashCollider;

		// Token: 0x040084F8 RID: 34040
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040084F9 RID: 34041
		[SerializeField]
		private List<AudioClip> breathFireSound;

		// Token: 0x040084FA RID: 34042
		[SerializeField]
		private List<AudioClip> wingFlapSound;

		// Token: 0x040084FB RID: 34043
		[SerializeField]
		private AudioClip crashSound;

		// Token: 0x040084FC RID: 34044
		private float turnRate;

		// Token: 0x040084FD RID: 34045
		private float turnAngle;

		// Token: 0x040084FE RID: 34046
		private float tiltAngle;

		// Token: 0x040084FF RID: 34047
		private float ascendAccel;

		// Token: 0x04008500 RID: 34048
		private float turnAccel;

		// Token: 0x04008501 RID: 34049
		private float tiltAccel;

		// Token: 0x04008502 RID: 34050
		private float horizontalAccel;

		// Token: 0x04008503 RID: 34051
		private float motorVolumeRampTime = 1f;

		// Token: 0x04008504 RID: 34052
		private float motorLevel;
	}
}
