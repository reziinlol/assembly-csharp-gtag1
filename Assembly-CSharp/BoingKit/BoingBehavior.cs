using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001351 RID: 4945
	public class BoingBehavior : BoingBase
	{
		// Token: 0x17000BCC RID: 3020
		// (get) Token: 0x06007C8E RID: 31886 RVA: 0x0028DE47 File Offset: 0x0028C047
		// (set) Token: 0x06007C8F RID: 31887 RVA: 0x0028DE59 File Offset: 0x0028C059
		public Vector3Spring PositionSpring
		{
			get
			{
				return this.Params.Instance.PositionSpring;
			}
			set
			{
				this.Params.Instance.PositionSpring = value;
				this.PositionSpringDirty = true;
			}
		}

		// Token: 0x17000BCD RID: 3021
		// (get) Token: 0x06007C90 RID: 31888 RVA: 0x0028DE73 File Offset: 0x0028C073
		// (set) Token: 0x06007C91 RID: 31889 RVA: 0x0028DE85 File Offset: 0x0028C085
		public QuaternionSpring RotationSpring
		{
			get
			{
				return this.Params.Instance.RotationSpring;
			}
			set
			{
				this.Params.Instance.RotationSpring = value;
				this.RotationSpringDirty = true;
			}
		}

		// Token: 0x17000BCE RID: 3022
		// (get) Token: 0x06007C92 RID: 31890 RVA: 0x0028DE9F File Offset: 0x0028C09F
		// (set) Token: 0x06007C93 RID: 31891 RVA: 0x0028DEB1 File Offset: 0x0028C0B1
		public Vector3Spring ScaleSpring
		{
			get
			{
				return this.Params.Instance.ScaleSpring;
			}
			set
			{
				this.Params.Instance.ScaleSpring = value;
				this.ScaleSpringDirty = true;
			}
		}

		// Token: 0x06007C94 RID: 31892 RVA: 0x0028DECB File Offset: 0x0028C0CB
		public BoingBehavior()
		{
			this.Params.Init();
		}

		// Token: 0x06007C95 RID: 31893 RVA: 0x0028DEF4 File Offset: 0x0028C0F4
		public virtual void Reboot()
		{
			this.Params.Instance.PositionSpring.Reset(base.transform.position);
			this.Params.Instance.RotationSpring.Reset(base.transform.rotation);
			this.Params.Instance.ScaleSpring.Reset(base.transform.localScale);
			this.CachedPositionLs = base.transform.localPosition;
			this.CachedRotationLs = base.transform.localRotation;
			this.CachedPositionWs = base.transform.position;
			this.CachedRotationWs = base.transform.rotation;
			this.CachedScaleLs = base.transform.localScale;
			this.CachedTransformValid = true;
		}

		// Token: 0x06007C96 RID: 31894 RVA: 0x0028DFBD File Offset: 0x0028C1BD
		public virtual void OnEnable()
		{
			this.CachedTransformValid = false;
			this.InitRebooted = false;
			this.Register();
		}

		// Token: 0x06007C97 RID: 31895 RVA: 0x0028DFD3 File Offset: 0x0028C1D3
		public void Start()
		{
			this.InitRebooted = false;
		}

		// Token: 0x06007C98 RID: 31896 RVA: 0x0028DFDC File Offset: 0x0028C1DC
		public virtual void OnDisable()
		{
			this.Unregister();
		}

		// Token: 0x06007C99 RID: 31897 RVA: 0x0028DFE4 File Offset: 0x0028C1E4
		protected virtual void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06007C9A RID: 31898 RVA: 0x0028DFEC File Offset: 0x0028C1EC
		protected virtual void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06007C9B RID: 31899 RVA: 0x0028DFF4 File Offset: 0x0028C1F4
		public void UpdateFlags()
		{
			this.Params.Bits.SetBit(0, this.TwoDDistanceCheck);
			this.Params.Bits.SetBit(1, this.TwoDPositionInfluence);
			this.Params.Bits.SetBit(2, this.TwoDRotationInfluence);
			this.Params.Bits.SetBit(3, this.EnablePositionEffect);
			this.Params.Bits.SetBit(4, this.EnableRotationEffect);
			this.Params.Bits.SetBit(5, this.EnableScaleEffect);
			this.Params.Bits.SetBit(6, this.GlobalReactionUpVector);
			this.Params.Bits.SetBit(9, this.UpdateMode == BoingManager.UpdateMode.FixedUpdate);
			this.Params.Bits.SetBit(10, this.UpdateMode == BoingManager.UpdateMode.EarlyUpdate);
			this.Params.Bits.SetBit(11, this.UpdateMode == BoingManager.UpdateMode.LateUpdate);
		}

		// Token: 0x06007C9C RID: 31900 RVA: 0x0028E0F3 File Offset: 0x0028C2F3
		public virtual void PrepareExecute()
		{
			this.PrepareExecute(false);
		}

		// Token: 0x06007C9D RID: 31901 RVA: 0x0028E0FC File Offset: 0x0028C2FC
		protected void PrepareExecute(bool accumulateEffectors)
		{
			if (this.SharedParams != null)
			{
				BoingWork.Params.Copy(ref this.SharedParams.Params, ref this.Params);
			}
			this.UpdateFlags();
			this.Params.InstanceID = base.GetInstanceID();
			this.Params.Instance.PrepareExecute(ref this.Params, this.CachedPositionWs, this.CachedRotationWs, base.transform.localScale, accumulateEffectors);
		}

		// Token: 0x06007C9E RID: 31902 RVA: 0x0028E172 File Offset: 0x0028C372
		public void Execute(float dt)
		{
			this.Params.Execute(dt);
		}

		// Token: 0x06007C9F RID: 31903 RVA: 0x0028E180 File Offset: 0x0028C380
		public void PullResults()
		{
			this.PullResults(ref this.Params);
		}

		// Token: 0x06007CA0 RID: 31904 RVA: 0x0028E190 File Offset: 0x0028C390
		public void GatherOutput(ref BoingWork.Output o)
		{
			if (!BoingManager.UseAsynchronousJobs)
			{
				this.Params.Instance.PositionSpring = o.PositionSpring;
				this.Params.Instance.RotationSpring = o.RotationSpring;
				this.Params.Instance.ScaleSpring = o.ScaleSpring;
				return;
			}
			if (this.PositionSpringDirty)
			{
				this.PositionSpringDirty = false;
			}
			else
			{
				this.Params.Instance.PositionSpring = o.PositionSpring;
			}
			if (this.RotationSpringDirty)
			{
				this.RotationSpringDirty = false;
			}
			else
			{
				this.Params.Instance.RotationSpring = o.RotationSpring;
			}
			if (this.ScaleSpringDirty)
			{
				this.ScaleSpringDirty = false;
				return;
			}
			this.Params.Instance.ScaleSpring = o.ScaleSpring;
		}

		// Token: 0x06007CA1 RID: 31905 RVA: 0x0028E25C File Offset: 0x0028C45C
		private void PullResults(ref BoingWork.Params p)
		{
			this.CachedPositionLs = base.transform.localPosition;
			this.CachedPositionWs = base.transform.position;
			this.RenderPositionWs = BoingWork.ComputeTranslationalResults(base.transform, base.transform.position, p.Instance.PositionSpring.Value, this);
			base.transform.position = this.RenderPositionWs;
			this.CachedRotationLs = base.transform.localRotation;
			this.CachedRotationWs = base.transform.rotation;
			this.RenderRotationWs = p.Instance.RotationSpring.ValueQuat;
			base.transform.rotation = this.RenderRotationWs;
			this.CachedScaleLs = base.transform.localScale;
			this.RenderScaleLs = p.Instance.ScaleSpring.Value;
			base.transform.localScale = this.RenderScaleLs;
			this.CachedTransformValid = true;
		}

		// Token: 0x06007CA2 RID: 31906 RVA: 0x0028E354 File Offset: 0x0028C554
		public virtual void Restore()
		{
			if (!this.CachedTransformValid)
			{
				return;
			}
			if (Application.isEditor)
			{
				if ((base.transform.position - this.RenderPositionWs).sqrMagnitude < 0.0001f)
				{
					base.transform.localPosition = this.CachedPositionLs;
				}
				if (QuaternionUtil.GetAngle(base.transform.rotation * Quaternion.Inverse(this.RenderRotationWs)) < 0.01f)
				{
					base.transform.localRotation = this.CachedRotationLs;
				}
				if ((base.transform.localScale - this.RenderScaleLs).sqrMagnitude < 0.0001f)
				{
					base.transform.localScale = this.CachedScaleLs;
					return;
				}
			}
			else
			{
				base.transform.localPosition = this.CachedPositionLs;
				base.transform.localRotation = this.CachedRotationLs;
				base.transform.localScale = this.CachedScaleLs;
			}
		}

		// Token: 0x04008DB6 RID: 36278
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x04008DB7 RID: 36279
		public bool TwoDDistanceCheck;

		// Token: 0x04008DB8 RID: 36280
		public bool TwoDPositionInfluence;

		// Token: 0x04008DB9 RID: 36281
		public bool TwoDRotationInfluence;

		// Token: 0x04008DBA RID: 36282
		public bool EnablePositionEffect = true;

		// Token: 0x04008DBB RID: 36283
		public bool EnableRotationEffect = true;

		// Token: 0x04008DBC RID: 36284
		public bool EnableScaleEffect;

		// Token: 0x04008DBD RID: 36285
		public bool GlobalReactionUpVector;

		// Token: 0x04008DBE RID: 36286
		public BoingManager.TranslationLockSpace TranslationLockSpace;

		// Token: 0x04008DBF RID: 36287
		public bool LockTranslationX;

		// Token: 0x04008DC0 RID: 36288
		public bool LockTranslationY;

		// Token: 0x04008DC1 RID: 36289
		public bool LockTranslationZ;

		// Token: 0x04008DC2 RID: 36290
		public BoingWork.Params Params;

		// Token: 0x04008DC3 RID: 36291
		public SharedBoingParams SharedParams;

		// Token: 0x04008DC4 RID: 36292
		internal bool PositionSpringDirty;

		// Token: 0x04008DC5 RID: 36293
		internal bool RotationSpringDirty;

		// Token: 0x04008DC6 RID: 36294
		internal bool ScaleSpringDirty;

		// Token: 0x04008DC7 RID: 36295
		internal bool CachedTransformValid;

		// Token: 0x04008DC8 RID: 36296
		internal Vector3 CachedPositionLs;

		// Token: 0x04008DC9 RID: 36297
		internal Vector3 CachedPositionWs;

		// Token: 0x04008DCA RID: 36298
		internal Vector3 RenderPositionWs;

		// Token: 0x04008DCB RID: 36299
		internal Quaternion CachedRotationLs;

		// Token: 0x04008DCC RID: 36300
		internal Quaternion CachedRotationWs;

		// Token: 0x04008DCD RID: 36301
		internal Quaternion RenderRotationWs;

		// Token: 0x04008DCE RID: 36302
		internal Vector3 CachedScaleLs;

		// Token: 0x04008DCF RID: 36303
		internal Vector3 RenderScaleLs;

		// Token: 0x04008DD0 RID: 36304
		internal bool InitRebooted;
	}
}
