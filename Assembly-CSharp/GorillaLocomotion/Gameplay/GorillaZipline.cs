using System;
using GorillaLocomotion.Climbing;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020010FE RID: 4350
	public class GorillaZipline : MonoBehaviour
	{
		// Token: 0x17000A95 RID: 2709
		// (get) Token: 0x06006D92 RID: 28050 RVA: 0x0023CE4E File Offset: 0x0023B04E
		// (set) Token: 0x06006D93 RID: 28051 RVA: 0x0023CE56 File Offset: 0x0023B056
		public float currentSpeed { get; private set; }

		// Token: 0x06006D94 RID: 28052 RVA: 0x0023CE60 File Offset: 0x0023B060
		protected void FindTFromDistance(ref float t, float distance, int steps = 1000)
		{
			float num = distance / (float)steps;
			Vector3 b = this.spline.GetPointLocal(t);
			float num2 = 0f;
			for (int i = 0; i < 1000; i++)
			{
				t += num;
				if (t >= 1f || t <= 0f)
				{
					break;
				}
				Vector3 pointLocal = this.spline.GetPointLocal(t);
				num2 += Vector3.Distance(pointLocal, b);
				if (num2 >= Mathf.Abs(distance))
				{
					break;
				}
				b = pointLocal;
			}
		}

		// Token: 0x06006D95 RID: 28053 RVA: 0x0023CED4 File Offset: 0x0023B0D4
		private float FindSlideHelperSpot(Vector3 grabPoint)
		{
			int i = 0;
			int num = 200;
			float num2 = 0.001f;
			float num3 = 1f / (float)num;
			float3 y = base.transform.InverseTransformPoint(grabPoint);
			float result = 0f;
			float num4 = float.PositiveInfinity;
			while (i < num)
			{
				float num5 = math.distancesq(this.spline.GetPointLocal(num2), y);
				if (num5 < num4)
				{
					num4 = num5;
					result = num2;
				}
				num2 += num3;
				i++;
			}
			return result;
		}

		// Token: 0x06006D96 RID: 28054 RVA: 0x0023CF50 File Offset: 0x0023B150
		protected virtual void Start()
		{
			this.spline = base.GetComponent<BezierSpline>();
			GorillaClimbable gorillaClimbable = this.slideHelper;
			gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Combine(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb));
		}

		// Token: 0x06006D97 RID: 28055 RVA: 0x0023CF86 File Offset: 0x0023B186
		private void OnDestroy()
		{
			GorillaClimbable gorillaClimbable = this.slideHelper;
			gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Remove(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb));
		}

		// Token: 0x06006D98 RID: 28056 RVA: 0x0023CFB0 File Offset: 0x0023B1B0
		public Vector3 GetCurrentDirection()
		{
			return this.spline.GetDirection(this.currentT);
		}

		// Token: 0x06006D99 RID: 28057 RVA: 0x0023CFC4 File Offset: 0x0023B1C4
		protected virtual void OnBeforeClimb(GorillaHandClimber hand, GorillaClimbableRef climbRef)
		{
			bool flag = this.currentClimber == null;
			this.currentClimber = hand;
			if (climbRef)
			{
				this.climbOffsetHelper.SetParent(climbRef.transform);
				this.climbOffsetHelper.position = hand.transform.position;
				this.climbOffsetHelper.localPosition = new Vector3(0f, 0f, this.climbOffsetHelper.localPosition.z);
			}
			this.currentT = this.FindSlideHelperSpot(this.climbOffsetHelper.position);
			this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
			if (flag)
			{
				Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
				float num = Vector3.Dot(averagedVelocity.normalized, this.spline.GetDirection(this.currentT));
				this.currentSpeed = averagedVelocity.magnitude * num * this.currentInheritVelocityMulti;
			}
		}

		// Token: 0x06006D9A RID: 28058 RVA: 0x0023D0B8 File Offset: 0x0023B2B8
		private void Update()
		{
			if (this.currentClimber)
			{
				Vector3 direction = this.spline.GetDirection(this.currentT);
				float num = Physics.gravity.y * direction.y * this.settings.gravityMulti;
				this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.settings.maxSpeed, num * Time.deltaTime);
				float num2 = MathUtils.Linear(this.currentSpeed, 0f, this.settings.maxFrictionSpeed, this.settings.friction, this.settings.maxFriction);
				this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, 0f, num2 * Time.deltaTime);
				this.currentSpeed = Mathf.Min(this.currentSpeed, this.settings.maxSpeed);
				this.currentSpeed = Mathf.Max(this.currentSpeed, -this.settings.maxSpeed);
				float value = Mathf.Abs(this.currentSpeed);
				this.FindTFromDistance(ref this.currentT, this.currentSpeed * Time.deltaTime, 1000);
				this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
				if (!this.audioSlide.gameObject.activeSelf)
				{
					this.audioSlide.gameObject.SetActive(true);
				}
				this.audioSlide.volume = MathUtils.Linear(value, 0f, this.settings.maxSpeed, this.settings.minSlideVolume, this.settings.maxSlideVolume);
				this.audioSlide.pitch = MathUtils.Linear(value, 0f, this.settings.maxSpeed, this.settings.minSlidePitch, this.settings.maxSlidePitch);
				if (!this.audioSlide.isPlaying)
				{
					this.audioSlide.GTPlay();
				}
				float num3 = MathUtils.Linear(value, 0f, this.settings.maxSpeed, -0.1f, 0.75f);
				if (num3 > 0f)
				{
					GorillaTagger.Instance.DoVibration(this.currentClimber.xrNode, num3, Time.deltaTime);
				}
				if (!this.spline.Loop)
				{
					if (this.currentT >= 1f || this.currentT <= 0f)
					{
						this.currentClimber.ForceStopClimbing(false, true);
					}
				}
				else if (this.currentT >= 1f)
				{
					this.currentT = 0f;
				}
				else if (this.currentT <= 0f)
				{
					this.currentT = 1f;
				}
				if (!this.slideHelper.isBeingClimbed)
				{
					this.Stop();
				}
			}
			if (this.currentInheritVelocityMulti < 1f)
			{
				this.currentInheritVelocityMulti += Time.deltaTime * 0.2f;
				this.currentInheritVelocityMulti = Mathf.Min(this.currentInheritVelocityMulti, 1f);
			}
		}

		// Token: 0x06006D9B RID: 28059 RVA: 0x0023D3A6 File Offset: 0x0023B5A6
		private void Stop()
		{
			this.currentClimber = null;
			this.audioSlide.GTStop();
			this.audioSlide.gameObject.SetActive(false);
			this.currentInheritVelocityMulti = 0.55f;
			this.currentSpeed = 0f;
		}

		// Token: 0x04007E92 RID: 32402
		[SerializeField]
		protected Transform segmentsRoot;

		// Token: 0x04007E93 RID: 32403
		[SerializeField]
		protected GameObject segmentPrefab;

		// Token: 0x04007E94 RID: 32404
		[SerializeField]
		protected GorillaClimbable slideHelper;

		// Token: 0x04007E95 RID: 32405
		[SerializeField]
		private AudioSource audioSlide;

		// Token: 0x04007E96 RID: 32406
		protected BezierSpline spline;

		// Token: 0x04007E97 RID: 32407
		[SerializeField]
		private Transform climbOffsetHelper;

		// Token: 0x04007E98 RID: 32408
		[SerializeField]
		private GorillaZiplineSettings settings;

		// Token: 0x04007E9A RID: 32410
		[SerializeField]
		protected float ziplineDistance = 15f;

		// Token: 0x04007E9B RID: 32411
		[SerializeField]
		protected float segmentDistance = 0.9f;

		// Token: 0x04007E9C RID: 32412
		private GorillaHandClimber currentClimber;

		// Token: 0x04007E9D RID: 32413
		private float currentT;

		// Token: 0x04007E9E RID: 32414
		private const float inheritVelocityRechargeRate = 0.2f;

		// Token: 0x04007E9F RID: 32415
		private const float inheritVelocityValueOnRelease = 0.55f;

		// Token: 0x04007EA0 RID: 32416
		private float currentInheritVelocityMulti = 1f;
	}
}
