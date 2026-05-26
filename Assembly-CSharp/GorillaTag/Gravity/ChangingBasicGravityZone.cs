using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001187 RID: 4487
	public class ChangingBasicGravityZone : BasicGravityZone
	{
		// Token: 0x06007196 RID: 29078 RVA: 0x002504BB File Offset: 0x0024E6BB
		protected override void Awake()
		{
			base.Awake();
			this.m_thisCallbackUnique = this;
			this.m_strengthDirty = false;
			this.m_directionDity = false;
		}

		// Token: 0x06007197 RID: 29079 RVA: 0x002504D8 File Offset: 0x0024E6D8
		protected override void OnDisable()
		{
			base.OnDisable();
			if (this.m_strengthDirty)
			{
				this.m_strengthDirty = false;
				this.gravityStrength = this.m_targetGravityStrength;
			}
			if (this.m_directionDity)
			{
				this.m_directionDity = false;
				this.m_gravityDirection = this.m_targetGravityDirection;
			}
		}

		// Token: 0x06007198 RID: 29080 RVA: 0x00250518 File Offset: 0x0024E718
		public void Update()
		{
			if (this.lastValueWhenSet == this.ExternalTriggerSetGravityStrength)
			{
				this.lastExternalTriggerSetMatched = true;
				return;
			}
			if (!this.lastExternalTriggerSetMatched)
			{
				this.SetGravityStrength(this.ExternalSetGravityStrength);
				this.lastValueWhenSet = this.ExternalTriggerSetGravityStrength;
				this.lastExternalTriggerSetMatched = true;
				return;
			}
			this.ExternalTriggerSetGravityStrength = this.lastValueWhenSet;
			this.lastExternalTriggerSetMatched = false;
		}

		// Token: 0x06007199 RID: 29081 RVA: 0x00250576 File Offset: 0x0024E776
		public void SetGravityStrength(float strength)
		{
			this.SetGravityStrength(strength, this.m_changeStrengthTime);
		}

		// Token: 0x0600719A RID: 29082 RVA: 0x00250585 File Offset: 0x0024E785
		public void SetGravityDirection(Vector3 dir)
		{
			this.SetGravityDirection(dir, this.m_changeDirectionTime);
		}

		// Token: 0x0600719B RID: 29083 RVA: 0x00250594 File Offset: 0x0024E794
		public void SetGravityStrength(float strength, float time)
		{
			this.m_targetGravityStrength = strength;
			if (time == 0f || !this.m_thisCallbackUnique.Registered)
			{
				this.gravityStrength = this.m_targetGravityStrength;
				this.m_strengthDirty = false;
				return;
			}
			this.m_lerpToGravitySpeed = (strength - this.gravityStrength) / time;
			this.m_strengthDirty = true;
		}

		// Token: 0x0600719C RID: 29084 RVA: 0x002505E8 File Offset: 0x0024E7E8
		public void SetGravityDirection(Vector3 direction, float time)
		{
			this.m_targetGravityDirection = direction.normalized;
			if (time == 0f || !this.m_thisCallbackUnique.Registered)
			{
				this.m_gravityDirection = this.m_targetGravityDirection;
				this.m_directionDity = false;
				return;
			}
			float num = Vector3.Angle(this.m_gravityDirection, direction) * 0.017453292f;
			this.m_lerpToDirectionSpeed = num / time;
			this.m_directionDity = true;
		}

		// Token: 0x0600719D RID: 29085 RVA: 0x0025064E File Offset: 0x0024E84E
		public void SetRotationIntent(bool rotate)
		{
			this.rotateTarget = rotate;
		}

		// Token: 0x0600719E RID: 29086 RVA: 0x00250658 File Offset: 0x0024E858
		public override void CallBack()
		{
			if (this.m_strengthDirty)
			{
				this.gravityStrength = Mathf.MoveTowards(this.gravityStrength, this.m_targetGravityStrength, this.m_lerpToGravitySpeed * Time.fixedDeltaTime);
				if (Mathf.Approximately(this.gravityStrength, this.m_targetGravityStrength))
				{
					this.m_strengthDirty = false;
				}
			}
			if (this.m_directionDity)
			{
				this.m_gravityDirection = Vector3.RotateTowards(this.m_gravityDirection, this.m_targetGravityDirection, this.m_lerpToDirectionSpeed * Time.fixedDeltaTime, 0f);
				if (this.m_gravityDirection == this.m_targetGravityDirection)
				{
					this.m_directionDity = false;
				}
			}
			base.CallBack();
		}

		// Token: 0x04008151 RID: 33105
		[Header("Change Value To Trigger Gravity Strength Change At Set Value (false to true and true to false both work, but value must change the frame you want it changed)")]
		public bool ExternalTriggerSetGravityStrength;

		// Token: 0x04008152 RID: 33106
		public float ExternalSetGravityStrength;

		// Token: 0x04008153 RID: 33107
		private bool lastExternalTriggerSetMatched = true;

		// Token: 0x04008154 RID: 33108
		private bool lastValueWhenSet;

		// Token: 0x04008155 RID: 33109
		private bool m_strengthDirty;

		// Token: 0x04008156 RID: 33110
		private float m_targetGravityStrength;

		// Token: 0x04008157 RID: 33111
		private float m_lerpToGravitySpeed;

		// Token: 0x04008158 RID: 33112
		private bool m_directionDity;

		// Token: 0x04008159 RID: 33113
		private Vector3 m_targetGravityDirection;

		// Token: 0x0400815A RID: 33114
		private float m_lerpToDirectionSpeed;

		// Token: 0x0400815B RID: 33115
		[SerializeField]
		private float m_changeStrengthTime;

		// Token: 0x0400815C RID: 33116
		[SerializeField]
		private float m_changeDirectionTime;

		// Token: 0x0400815D RID: 33117
		private ICallbackUnique m_thisCallbackUnique;
	}
}
