using System;
using GorillaTagScripts.Builder;
using GT_CustomMapSupportRuntime;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ECF RID: 3791
	public class SurfaceMover : MonoBehaviour
	{
		// Token: 0x06005D56 RID: 23894 RVA: 0x001D92BD File Offset: 0x001D74BD
		private void Start()
		{
			MovingSurfaceManager.instance == null;
			MovingSurfaceManager.instance.RegisterSurfaceMover(this);
		}

		// Token: 0x06005D57 RID: 23895 RVA: 0x001D92D6 File Offset: 0x001D74D6
		private void OnDestroy()
		{
			if (MovingSurfaceManager.instance != null)
			{
				MovingSurfaceManager.instance.UnregisterSurfaceMover(this);
			}
		}

		// Token: 0x06005D58 RID: 23896 RVA: 0x001D92F0 File Offset: 0x001D74F0
		public void InitMovingSurface()
		{
			if (this.moveType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				this.distance = Vector3.Distance(this.endXf.position, this.startXf.position);
				float num = this.distance / this.velocity;
				this.cycleDuration = num + this.cycleDelay;
			}
			else
			{
				if (this.rotationRelativeToStarting)
				{
					this.startingRotation = base.transform.localRotation.eulerAngles;
				}
				this.cycleDuration = this.rotationAmount / 360f / this.velocity;
				this.cycleDuration += this.cycleDelay;
			}
			float num2 = this.cycleDelay / this.cycleDuration;
			Vector2 vector = new Vector2(num2 / 2f, 0f);
			Vector2 vector2 = new Vector2(1f - num2 / 2f, 1f);
			float num3 = (vector2.y - vector.y) / (vector2.x - vector.x);
			this.lerpAlpha = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(num2 / 2f, 0f, 0f, num3),
				new Keyframe(1f - num2 / 2f, 1f, num3, 0f)
			});
			this.currT = this.startPercentage;
			uint num4 = (uint)(this.cycleDuration * 1000f);
			if (num4 == 0U)
			{
				num4 = 1U;
			}
			uint num5 = 2147483648U % num4;
			uint num6 = (uint)(this.startPercentage * num4);
			if (num6 >= num5)
			{
				this.startPercentageCycleOffset = num6 - num5;
				return;
			}
			this.startPercentageCycleOffset = num6 + num4 + num4 - num5;
		}

		// Token: 0x06005D59 RID: 23897 RVA: 0x001D949A File Offset: 0x001D769A
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp + (int)this.startPercentageCycleOffset + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x06005D5A RID: 23898 RVA: 0x001D94C3 File Offset: 0x001D76C3
		private long CycleLengthMs()
		{
			return (long)(this.cycleDuration * 1000f);
		}

		// Token: 0x06005D5B RID: 23899 RVA: 0x001D94D4 File Offset: 0x001D76D4
		public double PlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06005D5C RID: 23900 RVA: 0x001D94FF File Offset: 0x001D76FF
		public int CycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
		}

		// Token: 0x06005D5D RID: 23901 RVA: 0x001D950F File Offset: 0x001D770F
		public float CycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.PlatformTime() / (double)this.cycleDuration), 0f, 1f);
		}

		// Token: 0x06005D5E RID: 23902 RVA: 0x001D952F File Offset: 0x001D772F
		public bool IsEvenCycle()
		{
			return this.CycleCount() % 2 == 0;
		}

		// Token: 0x06005D5F RID: 23903 RVA: 0x001D953C File Offset: 0x001D773C
		public void Move()
		{
			this.Progress();
			BuilderMovingPart.BuilderMovingPartType builderMovingPartType = this.moveType;
			if (builderMovingPartType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				base.transform.localPosition = this.UpdatePointToPoint(this.percent);
				return;
			}
			if (builderMovingPartType != BuilderMovingPart.BuilderMovingPartType.Rotation)
			{
				return;
			}
			this.UpdateRotation(this.percent);
		}

		// Token: 0x06005D60 RID: 23904 RVA: 0x001D9584 File Offset: 0x001D7784
		private Vector3 UpdatePointToPoint(float perc)
		{
			float t = this.lerpAlpha.Evaluate(perc);
			return Vector3.Lerp(this.startXf.localPosition, this.endXf.localPosition, t);
		}

		// Token: 0x06005D61 RID: 23905 RVA: 0x001D95BC File Offset: 0x001D77BC
		private void UpdateRotation(float perc)
		{
			float num = this.lerpAlpha.Evaluate(perc) * this.rotationAmount;
			if (this.rotationRelativeToStarting)
			{
				Vector3 euler = this.startingRotation;
				switch (this.rotationAxis)
				{
				case RotationAxis.X:
					euler.x += num;
					break;
				case RotationAxis.Y:
					euler.y += num;
					break;
				case RotationAxis.Z:
					euler.z += num;
					break;
				}
				base.transform.localRotation = Quaternion.Euler(euler);
				return;
			}
			switch (this.rotationAxis)
			{
			case RotationAxis.X:
				base.transform.localRotation = Quaternion.AngleAxis(num, Vector3.right);
				return;
			case RotationAxis.Y:
				base.transform.localRotation = Quaternion.AngleAxis(num, Vector3.up);
				return;
			case RotationAxis.Z:
				base.transform.localRotation = Quaternion.AngleAxis(num, Vector3.forward);
				return;
			default:
				return;
			}
		}

		// Token: 0x06005D62 RID: 23906 RVA: 0x001D96A0 File Offset: 0x001D78A0
		private void Progress()
		{
			this.currT = this.CycleCompletionPercent();
			this.currForward = this.IsEvenCycle();
			this.percent = this.currT;
			if (this.reverseDirOnCycle)
			{
				this.percent = (this.currForward ? this.currT : (1f - this.currT));
			}
			if (this.reverseDir)
			{
				this.percent = 1f - this.percent;
			}
		}

		// Token: 0x06005D63 RID: 23907 RVA: 0x001D9718 File Offset: 0x001D7918
		public void CopySettings(SurfaceMoverSettings settings)
		{
			this.moveType = (BuilderMovingPart.BuilderMovingPartType)settings.moveType;
			this.startPercentage = 0f;
			this.velocity = Math.Clamp(settings.velocity, 0.001f, Math.Abs(settings.velocity));
			this.reverseDirOnCycle = settings.reverseDirOnCycle;
			this.reverseDir = settings.reverseDir;
			this.cycleDelay = Math.Clamp(settings.cycleDelay, 0f, Math.Abs(settings.cycleDelay));
			this.startXf = settings.start;
			this.endXf = settings.end;
			this.rotationAxis = (RotationAxis)settings.rotationAxis;
			this.rotationAmount = Math.Clamp(settings.rotationAmount, 0.001f, Math.Abs(settings.rotationAmount));
			this.rotationRelativeToStarting = settings.rotationRelativeToStarting;
		}

		// Token: 0x04006BDB RID: 27611
		[SerializeField]
		private BuilderMovingPart.BuilderMovingPartType moveType;

		// Token: 0x04006BDC RID: 27612
		[SerializeField]
		private float startPercentage = 0.5f;

		// Token: 0x04006BDD RID: 27613
		[SerializeField]
		private float velocity;

		// Token: 0x04006BDE RID: 27614
		[SerializeField]
		private bool reverseDirOnCycle = true;

		// Token: 0x04006BDF RID: 27615
		[SerializeField]
		private bool reverseDir;

		// Token: 0x04006BE0 RID: 27616
		[SerializeField]
		private float cycleDelay = 0.25f;

		// Token: 0x04006BE1 RID: 27617
		[SerializeField]
		protected Transform startXf;

		// Token: 0x04006BE2 RID: 27618
		[SerializeField]
		protected Transform endXf;

		// Token: 0x04006BE3 RID: 27619
		[SerializeField]
		public RotationAxis rotationAxis = RotationAxis.Y;

		// Token: 0x04006BE4 RID: 27620
		[SerializeField]
		public float rotationAmount = 360f;

		// Token: 0x04006BE5 RID: 27621
		[SerializeField]
		public bool rotationRelativeToStarting;

		// Token: 0x04006BE6 RID: 27622
		private AnimationCurve lerpAlpha;

		// Token: 0x04006BE7 RID: 27623
		private float cycleDuration;

		// Token: 0x04006BE8 RID: 27624
		private float distance;

		// Token: 0x04006BE9 RID: 27625
		private Vector3 startingRotation;

		// Token: 0x04006BEA RID: 27626
		private float currT;

		// Token: 0x04006BEB RID: 27627
		private float percent;

		// Token: 0x04006BEC RID: 27628
		private bool currForward;

		// Token: 0x04006BED RID: 27629
		private float dtSinceServerUpdate;

		// Token: 0x04006BEE RID: 27630
		private int lastServerTimeStamp;

		// Token: 0x04006BEF RID: 27631
		private float rotateStartAmt;

		// Token: 0x04006BF0 RID: 27632
		private float rotateAmt;

		// Token: 0x04006BF1 RID: 27633
		private uint startPercentageCycleOffset;
	}
}
