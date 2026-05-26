using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x0200118B RID: 4491
	public class MonkeGravityController : MonoBehaviour, ICallbackUnique, ICallBack
	{
		// Token: 0x17000AD9 RID: 2777
		// (get) Token: 0x060071AA RID: 29098 RVA: 0x00250C02 File Offset: 0x0024EE02
		public Collider ActivatorCollider
		{
			get
			{
				return this.m_activatorCollider;
			}
		}

		// Token: 0x17000ADA RID: 2778
		// (get) Token: 0x060071AB RID: 29099 RVA: 0x00250C0A File Offset: 0x0024EE0A
		public Rigidbody TargetRigidBody
		{
			get
			{
				return this.m_targetRigidBody;
			}
		}

		// Token: 0x17000ADB RID: 2779
		// (get) Token: 0x060071AC RID: 29100 RVA: 0x00250C12 File Offset: 0x0024EE12
		public Transform TargetTransform
		{
			get
			{
				return this.m_targetTransform;
			}
		}

		// Token: 0x17000ADC RID: 2780
		// (get) Token: 0x060071AD RID: 29101 RVA: 0x00250C1A File Offset: 0x0024EE1A
		public virtual float Scale
		{
			get
			{
				return this.TargetTransform.localScale.x;
			}
		}

		// Token: 0x17000ADD RID: 2781
		// (get) Token: 0x060071AE RID: 29102 RVA: 0x00250C2C File Offset: 0x0024EE2C
		protected bool InstantRotation
		{
			get
			{
				return this.m_instantRotation;
			}
		}

		// Token: 0x17000ADE RID: 2782
		// (get) Token: 0x060071AF RID: 29103 RVA: 0x00250C34 File Offset: 0x0024EE34
		protected bool OverrideForceMode
		{
			get
			{
				return this.m_overrideForceMode;
			}
		}

		// Token: 0x17000ADF RID: 2783
		// (get) Token: 0x060071B0 RID: 29104 RVA: 0x00250C3C File Offset: 0x0024EE3C
		// (set) Token: 0x060071B1 RID: 29105 RVA: 0x00250C44 File Offset: 0x0024EE44
		public RotationDirection PreferredRotationDirection
		{
			get
			{
				return this.m_preferredRotationDirection;
			}
			set
			{
				this.m_preferredRotationDirection = value;
			}
		}

		// Token: 0x17000AE0 RID: 2784
		// (get) Token: 0x060071B2 RID: 29106 RVA: 0x00250C4D File Offset: 0x0024EE4D
		public bool Register
		{
			get
			{
				return this.m_register;
			}
		}

		// Token: 0x17000AE1 RID: 2785
		// (get) Token: 0x060071B3 RID: 29107 RVA: 0x00250C55 File Offset: 0x0024EE55
		// (set) Token: 0x060071B4 RID: 29108 RVA: 0x00250C5D File Offset: 0x0024EE5D
		public Vector3 GravityUp { get; private set; } = Vector3.up;

		// Token: 0x17000AE2 RID: 2786
		// (get) Token: 0x060071B5 RID: 29109 RVA: 0x00250C66 File Offset: 0x0024EE66
		// (set) Token: 0x060071B6 RID: 29110 RVA: 0x00250C6E File Offset: 0x0024EE6E
		public Vector3 GravityDown { get; private set; } = Vector3.down;

		// Token: 0x17000AE3 RID: 2787
		// (get) Token: 0x060071B7 RID: 29111 RVA: 0x00250C77 File Offset: 0x0024EE77
		// (set) Token: 0x060071B8 RID: 29112 RVA: 0x00250C7F File Offset: 0x0024EE7F
		public float GravityMultiplier { get; set; } = 1f;

		// Token: 0x17000AE4 RID: 2788
		// (get) Token: 0x060071B9 RID: 29113 RVA: 0x00250C88 File Offset: 0x0024EE88
		// (set) Token: 0x060071BA RID: 29114 RVA: 0x00250C90 File Offset: 0x0024EE90
		public Vector3 PersonalGravityDirection { get; set; } = Vector3.up;

		// Token: 0x060071BB RID: 29115 RVA: 0x00250C99 File Offset: 0x0024EE99
		public void SetPersonalGravityDirection(Vector3 direction)
		{
			this.PersonalGravityDirection = direction.normalized;
		}

		// Token: 0x060071BC RID: 29116 RVA: 0x00250CA8 File Offset: 0x0024EEA8
		public void SetPersonalGravityDirection(Transform reference)
		{
			this.PersonalGravityDirection = reference.up;
		}

		// Token: 0x17000AE5 RID: 2789
		// (get) Token: 0x060071BD RID: 29117 RVA: 0x00250CB6 File Offset: 0x0024EEB6
		public int GravityZonesCount
		{
			get
			{
				return this.m_gravityZones.Count;
			}
		}

		// Token: 0x17000AE6 RID: 2790
		// (get) Token: 0x060071BE RID: 29118 RVA: 0x00250CC3 File Offset: 0x0024EEC3
		// (set) Token: 0x060071BF RID: 29119 RVA: 0x00250CCB File Offset: 0x0024EECB
		bool ICallbackUnique.Registered { get; set; }

		// Token: 0x060071C0 RID: 29120 RVA: 0x00250CD4 File Offset: 0x0024EED4
		protected virtual void Awake()
		{
			if (this.m_targetRigidBody.IsNull())
			{
				this.m_targetRigidBody = base.GetComponent<Rigidbody>();
			}
			if (this.m_targetTransform.IsNull())
			{
				this.m_targetTransform = base.transform;
			}
			if (this.m_alwaysInZone != null)
			{
				this.m_alwaysInZone.AddTarget(this);
			}
			else if (this.m_activatorCollider.IsNull())
			{
				this.m_activatorCollider = base.GetComponent<Collider>();
				if (this.m_activatorCollider.IsNull())
				{
					return;
				}
			}
			if (this.m_targetRigidBody.IsNull())
			{
				return;
			}
			this.m_register = true;
			this.m_globalGravityIntent = this.m_targetRigidBody.useGravity;
		}

		// Token: 0x060071C1 RID: 29121 RVA: 0x00250D7C File Offset: 0x0024EF7C
		protected virtual void OnEnable()
		{
			if (!this.m_register)
			{
				return;
			}
			MonkeGravityManager.AddMonkeGravityController(this);
		}

		// Token: 0x060071C2 RID: 29122 RVA: 0x00250D90 File Offset: 0x0024EF90
		protected virtual void OnDisable()
		{
			if (!this.m_register)
			{
				return;
			}
			this.m_targetRigidBody.useGravity = this.m_globalGravityIntent;
			MonkeGravityManager.RemoveMonkeGravityController(this);
			for (int i = this.m_gravityZones.Count - 1; i > -1; i--)
			{
				BasicGravityZone basicGravityZone = this.m_gravityZones[i];
				basicGravityZone.RemoveTarget(this);
				this.OnLeftGravityZone(basicGravityZone);
			}
		}

		// Token: 0x060071C3 RID: 29123 RVA: 0x00250DF0 File Offset: 0x0024EFF0
		public virtual void CallBack()
		{
			GravityInfo defaultGravityInfo;
			if (this.m_gravityZones.Count < 1 || !this.m_gravityZones[this.m_gravityZones.Count - 1].GetGravityInfo(this, out defaultGravityInfo))
			{
				defaultGravityInfo = MonkeGravityManager.DefaultGravityInfo;
			}
			this.GravityUp = defaultGravityInfo.gravityUpDirection;
			this.GravityDown = -this.GravityUp;
			if (this.m_gravityZones.Count < 1)
			{
				Vector3 gravity = Physics.gravity;
				this.ApplyGravityForce(gravity, ForceMode.Acceleration);
			}
			if ((defaultGravityInfo.rotate || this.m_needsRotationRecovery) && this.m_useRotation)
			{
				this.ApplyGravityUpRotation(defaultGravityInfo.rotationDirection, defaultGravityInfo.rotationSpeed * Time.fixedDeltaTime);
			}
		}

		// Token: 0x060071C4 RID: 29124 RVA: 0x00250E9E File Offset: 0x0024F09E
		public virtual Vector3 GetWorldPoint()
		{
			return this.m_targetTransform.position;
		}

		// Token: 0x060071C5 RID: 29125 RVA: 0x00250EAB File Offset: 0x0024F0AB
		public virtual void OnEnteredGravityZone(BasicGravityZone zone)
		{
			if (!this.m_gravityZones.Contains(zone))
			{
				this.m_gravityZones.Add(zone);
			}
			if (this.m_targetRigidBody.useGravity)
			{
				this.m_targetRigidBody.useGravity = false;
			}
		}

		// Token: 0x060071C6 RID: 29126 RVA: 0x00250EE0 File Offset: 0x0024F0E0
		public virtual void OnLeftGravityZone(BasicGravityZone zone)
		{
			this.m_gravityZones.Remove(zone);
			if (this.m_gravityZones.Count < 1)
			{
				this.m_targetRigidBody.useGravity = this.m_globalGravityIntent;
				this.m_needsRotationRecovery = true;
			}
		}

		// Token: 0x060071C7 RID: 29127 RVA: 0x00250F15 File Offset: 0x0024F115
		public virtual void ApplyGravityForce(in Vector3 force, ForceMode forceType = ForceMode.Acceleration)
		{
			if (this.m_targetRigidBody.isKinematic)
			{
				return;
			}
			this.m_targetRigidBody.AddForce(force * this.GravityMultiplier, this.m_overrideForceMode ? this.m_forceModeOverride : forceType);
		}

		// Token: 0x060071C8 RID: 29128 RVA: 0x00250F52 File Offset: 0x0024F152
		public void ClearRotationRecovery()
		{
			this.m_needsRotationRecovery = false;
		}

		// Token: 0x060071C9 RID: 29129 RVA: 0x00250F5C File Offset: 0x0024F15C
		public virtual void ApplyGravityUpRotation(in Vector3 upDir, float speed)
		{
			Vector3 up = this.m_targetTransform.up;
			Vector3 toDirection;
			if (!this.m_instantRotation)
			{
				Vector3 vector = upDir;
				if (vector == up * -1f)
				{
					switch (this.m_preferredRotationDirection)
					{
					case RotationDirection.Forward:
						vector = this.m_targetTransform.forward;
						break;
					case RotationDirection.Backward:
						vector = this.m_targetTransform.forward * -1f;
						break;
					case RotationDirection.Left:
						vector = this.m_targetTransform.right * -1f;
						break;
					case RotationDirection.Right:
						vector = this.m_targetTransform.right;
						break;
					}
				}
				toDirection = Vector3.RotateTowards(up, vector, speed, 0f);
			}
			else
			{
				toDirection = upDir;
			}
			Quaternion lhs = Quaternion.FromToRotation(up, toDirection);
			this.m_targetRigidBody.MoveRotation(lhs * this.m_targetTransform.rotation);
			if (lhs == Quaternion.identity)
			{
				this.m_needsRotationRecovery = false;
			}
		}

		// Token: 0x04008171 RID: 33137
		[SerializeField]
		private Collider m_activatorCollider;

		// Token: 0x04008172 RID: 33138
		[SerializeField]
		protected Rigidbody m_targetRigidBody;

		// Token: 0x04008173 RID: 33139
		[SerializeField]
		protected Transform m_targetTransform;

		// Token: 0x04008174 RID: 33140
		[SerializeField]
		private bool m_instantRotation;

		// Token: 0x04008175 RID: 33141
		[SerializeField]
		private bool m_useRotation;

		// Token: 0x04008176 RID: 33142
		[SerializeField]
		private bool m_overrideForceMode;

		// Token: 0x04008177 RID: 33143
		[SerializeField]
		private ForceMode m_forceModeOverride;

		// Token: 0x04008178 RID: 33144
		[SerializeField]
		private BasicGravityZone m_alwaysInZone;

		// Token: 0x04008179 RID: 33145
		[Tooltip("The direction we wish to rotate if the target is 180 degrees off.")]
		[SerializeField]
		protected RotationDirection m_preferredRotationDirection = RotationDirection.Right;

		// Token: 0x0400817A RID: 33146
		private bool m_register;

		// Token: 0x0400817B RID: 33147
		private readonly List<BasicGravityZone> m_gravityZones = new List<BasicGravityZone>(3);

		// Token: 0x0400817C RID: 33148
		private bool m_needsRotationRecovery;

		// Token: 0x04008181 RID: 33153
		protected bool m_globalGravityIntent;
	}
}
