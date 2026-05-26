using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02001109 RID: 4361
	[NetworkBehaviourWeaved(1)]
	public class TraverseSpline : NetworkComponent
	{
		// Token: 0x06006DDD RID: 28125 RVA: 0x0023E4D1 File Offset: 0x0023C6D1
		protected override void Awake()
		{
			base.Awake();
			this.progress = this.SplineProgressOffet % 1f;
		}

		// Token: 0x06006DDE RID: 28126 RVA: 0x0023E4EC File Offset: 0x0023C6EC
		protected virtual void FixedUpdate()
		{
			if (!base.IsMine && this.progressLerpStartTime + 1f > Time.time)
			{
				this.progress = Mathf.Lerp(this.progressLerpStart, this.progressLerpEnd, (Time.time - this.progressLerpStartTime) / 1f);
			}
			else
			{
				if (this.isHeldByLocalPlayer)
				{
					this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier, this.speedMultiplierWhileHeld, this.acceleration * Time.deltaTime);
				}
				else
				{
					this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier, 1f, this.deceleration * Time.deltaTime);
				}
				if (this.goingForward)
				{
					this.progress += Time.deltaTime * this.currentSpeedMultiplier / this.duration;
					if (this.progress > 1f)
					{
						if (this.mode == SplineWalkerMode.Once)
						{
							this.progress = 1f;
						}
						else if (this.mode == SplineWalkerMode.Loop)
						{
							this.progress %= 1f;
						}
						else
						{
							this.progress = 2f - this.progress;
							this.goingForward = false;
						}
					}
				}
				else
				{
					this.progress -= Time.deltaTime * this.currentSpeedMultiplier / this.duration;
					if (this.progress < 0f)
					{
						this.progress = -this.progress;
						this.goingForward = true;
					}
				}
			}
			Vector3 point = this.spline.GetPoint(this.progress, this.constantVelocity);
			base.transform.position = point;
			if (this.lookForward)
			{
				base.transform.LookAt(base.transform.position + this.spline.GetDirection(this.progress, this.constantVelocity));
			}
		}

		// Token: 0x17000A9B RID: 2715
		// (get) Token: 0x06006DDF RID: 28127 RVA: 0x0023E6B5 File Offset: 0x0023C8B5
		// (set) Token: 0x06006DE0 RID: 28128 RVA: 0x0023E6DB File Offset: 0x0023C8DB
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe float Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing TraverseSpline.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(float*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing TraverseSpline.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(float*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06006DE1 RID: 28129 RVA: 0x0023E702 File Offset: 0x0023C902
		public override void WriteDataFusion()
		{
			this.Data = this.progress + this.currentSpeedMultiplier * 1f / this.duration;
		}

		// Token: 0x06006DE2 RID: 28130 RVA: 0x0023E724 File Offset: 0x0023C924
		public override void ReadDataFusion()
		{
			this.progressLerpEnd = this.Data;
			this.ReadDataShared();
		}

		// Token: 0x06006DE3 RID: 28131 RVA: 0x0023E738 File Offset: 0x0023C938
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext(this.progress + this.currentSpeedMultiplier * 1f / this.duration);
		}

		// Token: 0x06006DE4 RID: 28132 RVA: 0x0023E75F File Offset: 0x0023C95F
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			this.progressLerpEnd = (float)stream.ReceiveNext();
			this.ReadDataShared();
		}

		// Token: 0x06006DE5 RID: 28133 RVA: 0x0023E778 File Offset: 0x0023C978
		private void ReadDataShared()
		{
			if (float.IsNaN(this.progressLerpEnd) || float.IsInfinity(this.progressLerpEnd))
			{
				this.progressLerpEnd = 1f;
			}
			else
			{
				this.progressLerpEnd = Mathf.Abs(this.progressLerpEnd);
				if (this.progressLerpEnd > 1f)
				{
					this.progressLerpEnd = (float)((double)this.progressLerpEnd % 1.0);
				}
			}
			this.progressLerpStart = ((Mathf.Abs(this.progressLerpEnd - this.progress) > Mathf.Abs(this.progressLerpEnd - (this.progress - 1f))) ? (this.progress - 1f) : this.progress);
			this.progressLerpStartTime = Time.time;
		}

		// Token: 0x06006DE6 RID: 28134 RVA: 0x0023E833 File Offset: 0x0023CA33
		protected float GetProgress()
		{
			return this.progress;
		}

		// Token: 0x06006DE7 RID: 28135 RVA: 0x0023E83B File Offset: 0x0023CA3B
		public float GetCurrentSpeed()
		{
			return this.currentSpeedMultiplier;
		}

		// Token: 0x06006DE9 RID: 28137 RVA: 0x0023E891 File Offset: 0x0023CA91
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06006DEA RID: 28138 RVA: 0x0023E8A9 File Offset: 0x0023CAA9
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x04007EE0 RID: 32480
		public BezierSpline spline;

		// Token: 0x04007EE1 RID: 32481
		public float duration = 30f;

		// Token: 0x04007EE2 RID: 32482
		public float speedMultiplierWhileHeld = 2f;

		// Token: 0x04007EE3 RID: 32483
		private float currentSpeedMultiplier;

		// Token: 0x04007EE4 RID: 32484
		public float acceleration = 1f;

		// Token: 0x04007EE5 RID: 32485
		public float deceleration = 1f;

		// Token: 0x04007EE6 RID: 32486
		private bool isHeldByLocalPlayer;

		// Token: 0x04007EE7 RID: 32487
		public bool lookForward = true;

		// Token: 0x04007EE8 RID: 32488
		public SplineWalkerMode mode;

		// Token: 0x04007EE9 RID: 32489
		[SerializeField]
		private float SplineProgressOffet;

		// Token: 0x04007EEA RID: 32490
		private float progress;

		// Token: 0x04007EEB RID: 32491
		private float progressLerpStart;

		// Token: 0x04007EEC RID: 32492
		private float progressLerpEnd;

		// Token: 0x04007EED RID: 32493
		private const float progressLerpDuration = 1f;

		// Token: 0x04007EEE RID: 32494
		private float progressLerpStartTime;

		// Token: 0x04007EEF RID: 32495
		private bool goingForward = true;

		// Token: 0x04007EF0 RID: 32496
		[SerializeField]
		private bool constantVelocity;

		// Token: 0x04007EF1 RID: 32497
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private float _Data;
	}
}
