using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001221 RID: 4641
	public class RCHelicopter : RCVehicle
	{
		// Token: 0x06007415 RID: 29717 RVA: 0x0025EFEC File Offset: 0x0025D1EC
		protected override void AuthorityBeginDocked()
		{
			base.AuthorityBeginDocked();
			this.turnRate = 0f;
			this.verticalPropeller.localRotation = this.verticalPropellerBaseRotation;
			this.turnPropeller.localRotation = this.turnPropellerBaseRotation;
			if (this.connectedRemote == null)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06007416 RID: 29718 RVA: 0x0025F048 File Offset: 0x0025D248
		protected override void Awake()
		{
			base.Awake();
			this.verticalPropellerBaseRotation = this.verticalPropeller.localRotation;
			this.turnPropellerBaseRotation = this.turnPropeller.localRotation;
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
		}

		// Token: 0x06007417 RID: 29719 RVA: 0x0025F0B8 File Offset: 0x0025D2B8
		protected override void SharedUpdate(float dt)
		{
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = Mathf.Lerp(this.mainPropellerSpinRateRange.x, this.mainPropellerSpinRateRange.y, this.activeInput.trigger);
				this.verticalPropeller.Rotate(new Vector3(0f, num * dt, 0f), Space.Self);
				this.turnPropeller.Rotate(new Vector3(this.activeInput.joystick.x * this.backPropellerSpinRate * dt, 0f, 0f), Space.Self);
			}
		}

		// Token: 0x06007418 RID: 29720 RVA: 0x0025F148 File Offset: 0x0025D348
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
			{
				return;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			Vector3 linearVelocity = this.rb.linearVelocity;
			float magnitude = linearVelocity.magnitude;
			float target = this.activeInput.joystick.x * this.maxTurnRate;
			this.turnRate = Mathf.MoveTowards(this.turnRate, target, this.turnAccel * fixedDeltaTime);
			float num = this.activeInput.joystick.y * this.maxHorizontalSpeed;
			float x = Mathf.Sign(this.activeInput.joystick.y) * Mathf.Lerp(0f, this.maxHorizontalTiltAngle, Mathf.Abs(this.activeInput.joystick.y));
			base.transform.rotation = Quaternion.Euler(new Vector3(x, this.turnAccel, 0f));
			float num2 = Mathf.Abs(num);
			Vector3 normalized = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
			float num3 = Vector3.Dot(normalized, linearVelocity);
			if (num2 > 0.01f && ((num > 0f && num > num3) || (num < 0f && num < num3)))
			{
				this.rb.AddForce(normalized * Mathf.Sign(num) * this.horizontalAccel * fixedDeltaTime * this.rb.mass, ForceMode.Force);
			}
			float num4 = this.activeInput.trigger * this.maxAscendSpeed;
			if (num4 > 0.01f && linearVelocity.y < num4)
			{
				this.rb.AddForce(Vector3.up * this.ascendAccel * this.rb.mass, ForceMode.Force);
			}
			if (this.rb.useGravity)
			{
				this.rb.AddForce(-Physics.gravity * this.gravityCompensation * this.rb.mass, ForceMode.Force);
			}
		}

		// Token: 0x06007419 RID: 29721 RVA: 0x0025F346 File Offset: 0x0025D546
		private void OnTriggerEnter(Collider other)
		{
			if (!other.isTrigger && base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginCrash();
			}
		}

		// Token: 0x04008505 RID: 34053
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x04008506 RID: 34054
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x04008507 RID: 34055
		[SerializeField]
		private float gravityCompensation = 0.5f;

		// Token: 0x04008508 RID: 34056
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x04008509 RID: 34057
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x0400850A RID: 34058
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x0400850B RID: 34059
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x0400850C RID: 34060
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x0400850D RID: 34061
		[SerializeField]
		private Vector2 mainPropellerSpinRateRange = new Vector2(3f, 15f);

		// Token: 0x0400850E RID: 34062
		[SerializeField]
		private float backPropellerSpinRate = 5f;

		// Token: 0x0400850F RID: 34063
		[SerializeField]
		private Transform verticalPropeller;

		// Token: 0x04008510 RID: 34064
		[SerializeField]
		private Transform turnPropeller;

		// Token: 0x04008511 RID: 34065
		private Quaternion verticalPropellerBaseRotation;

		// Token: 0x04008512 RID: 34066
		private Quaternion turnPropellerBaseRotation;

		// Token: 0x04008513 RID: 34067
		private float turnRate;

		// Token: 0x04008514 RID: 34068
		private float ascendAccel;

		// Token: 0x04008515 RID: 34069
		private float turnAccel;

		// Token: 0x04008516 RID: 34070
		private float horizontalAccel;
	}
}
