using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02001111 RID: 4369
	public class GorillaVelocityTracker : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000A9F RID: 2719
		// (get) Token: 0x06006E0F RID: 28175 RVA: 0x00240394 File Offset: 0x0023E594
		// (set) Token: 0x06006E10 RID: 28176 RVA: 0x0024039C File Offset: 0x0023E59C
		public bool TickRunning { get; set; }

		// Token: 0x06006E11 RID: 28177 RVA: 0x002403A8 File Offset: 0x0023E5A8
		public void ResetState()
		{
			this.trans = base.transform;
			this.localSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
			this.<ResetState>g__PopulateArray|20_0(this.localSpaceData);
			this.worldSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
			this.<ResetState>g__PopulateArray|20_0(this.worldSpaceData);
			this.isRelativeTo = (this.relativeTo != null);
			this.lastLocalSpacePos = this.GetPosition(false);
			this.lastWorldSpacePos = this.GetPosition(true);
			this.wasAboveThreshold = false;
		}

		// Token: 0x06006E12 RID: 28178 RVA: 0x0024042E File Offset: 0x0023E62E
		private void Awake()
		{
			this.ResetState();
		}

		// Token: 0x06006E13 RID: 28179 RVA: 0x00019E3F File Offset: 0x0001803F
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06006E14 RID: 28180 RVA: 0x00240436 File Offset: 0x0023E636
		private void OnDisable()
		{
			this.ResetState();
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06006E15 RID: 28181 RVA: 0x00240444 File Offset: 0x0023E644
		public void SetRelativeTo(Transform tf)
		{
			this.relativeTo = tf;
			this.isRelativeTo = (tf != null);
		}

		// Token: 0x06006E16 RID: 28182 RVA: 0x0024045A File Offset: 0x0023E65A
		private Vector3 GetPosition(bool worldSpace)
		{
			if (worldSpace)
			{
				return this.trans.position;
			}
			if (this.isRelativeTo)
			{
				return this.relativeTo.InverseTransformPoint(this.trans.position);
			}
			return this.trans.localPosition;
		}

		// Token: 0x06006E17 RID: 28183 RVA: 0x00240498 File Offset: 0x0023E698
		public void Tick()
		{
			if (Time.frameCount <= this.lastTickedFrame)
			{
				return;
			}
			Vector3 position = this.GetPosition(false);
			Vector3 position2 = this.GetPosition(true);
			GorillaVelocityTracker.VelocityDataPoint velocityDataPoint = this.localSpaceData[this.currentDataPointIndex];
			velocityDataPoint.delta = (position - this.lastLocalSpacePos) / Time.deltaTime;
			velocityDataPoint.time = Time.time;
			this.localSpaceData[this.currentDataPointIndex] = velocityDataPoint;
			GorillaVelocityTracker.VelocityDataPoint velocityDataPoint2 = this.worldSpaceData[this.currentDataPointIndex];
			velocityDataPoint2.delta = (position2 - this.lastWorldSpacePos) / Time.deltaTime;
			velocityDataPoint2.time = Time.time;
			this.worldSpaceData[this.currentDataPointIndex] = velocityDataPoint2;
			this.lastLocalSpacePos = position;
			this.lastWorldSpacePos = position2;
			this.currentDataPointIndex++;
			if (this.currentDataPointIndex >= this.maxDataPoints)
			{
				this.currentDataPointIndex = 0;
			}
			if (this.useVelocityEvents)
			{
				this.GetLatestVelocity(this.useWorldSpaceForEvents);
			}
			this.lastTickedFrame = Time.frameCount;
		}

		// Token: 0x06006E18 RID: 28184 RVA: 0x0024059A File Offset: 0x0023E79A
		private void AddToQueue(ref List<GorillaVelocityTracker.VelocityDataPoint> dataPoints, GorillaVelocityTracker.VelocityDataPoint newData)
		{
			dataPoints.Add(newData);
			if (dataPoints.Count >= this.maxDataPoints)
			{
				dataPoints.RemoveAt(0);
			}
		}

		// Token: 0x06006E19 RID: 28185 RVA: 0x002405BC File Offset: 0x0023E7BC
		public Vector3 GetAverageVelocity(bool worldSpace = false, float maxTimeFromPast = 0.15f, bool doMagnitudeCheck = false)
		{
			float num = maxTimeFromPast / 2f;
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array.Length <= 1)
			{
				return Vector3.zero;
			}
			GorillaVelocityTracker.<>c__DisplayClass28_0 CS$<>8__locals1;
			CS$<>8__locals1.total = Vector3.zero;
			CS$<>8__locals1.totalMag = 0f;
			CS$<>8__locals1.added = 0;
			float num2 = Time.time - maxTimeFromPast;
			float num3 = Time.time - num;
			int i = 0;
			int num4 = this.currentDataPointIndex;
			while (i < this.maxDataPoints)
			{
				GorillaVelocityTracker.VelocityDataPoint velocityDataPoint = array[num4];
				if (doMagnitudeCheck && CS$<>8__locals1.added > 1 && velocityDataPoint.time >= num3)
				{
					if (velocityDataPoint.delta.magnitude >= CS$<>8__locals1.totalMag / (float)CS$<>8__locals1.added)
					{
						GorillaVelocityTracker.<GetAverageVelocity>g__AddPoint|28_0(velocityDataPoint, ref CS$<>8__locals1);
					}
				}
				else if (velocityDataPoint.time >= num2)
				{
					GorillaVelocityTracker.<GetAverageVelocity>g__AddPoint|28_0(velocityDataPoint, ref CS$<>8__locals1);
				}
				num4++;
				if (num4 >= this.maxDataPoints)
				{
					num4 = 0;
				}
				i++;
			}
			if (CS$<>8__locals1.added > 0)
			{
				return CS$<>8__locals1.total / (float)CS$<>8__locals1.added;
			}
			return Vector3.zero;
		}

		// Token: 0x06006E1A RID: 28186 RVA: 0x002406CC File Offset: 0x0023E8CC
		public Vector3 GetLatestVelocity(bool worldSpace = false)
		{
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array[this.currentDataPointIndex].delta.magnitude >= this.latestVelocityThreshold && !this.wasAboveThreshold)
			{
				UnityEvent onLatestAboveThreshold = this.OnLatestAboveThreshold;
				if (onLatestAboveThreshold != null)
				{
					onLatestAboveThreshold.Invoke();
				}
				this.wasAboveThreshold = true;
			}
			else if (array[this.currentDataPointIndex].delta.magnitude < this.latestVelocityThreshold && this.wasAboveThreshold)
			{
				UnityEvent onLatestBelowThreshold = this.OnLatestBelowThreshold;
				if (onLatestBelowThreshold != null)
				{
					onLatestBelowThreshold.Invoke();
				}
				this.wasAboveThreshold = false;
			}
			return array[this.currentDataPointIndex].delta;
		}

		// Token: 0x06006E1B RID: 28187 RVA: 0x00240770 File Offset: 0x0023E970
		public float GetAverageSpeedChangeMagnitudeInDirection(Vector3 dir, bool worldSpace = false, float maxTimeFromPast = 0.05f)
		{
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array.Length <= 1)
			{
				return 0f;
			}
			float num = 0f;
			int num2 = 0;
			float num3 = Time.time - maxTimeFromPast;
			bool flag = false;
			Vector3 b = Vector3.zero;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].time >= num3)
				{
					if (!flag)
					{
						b = array[i].delta;
						flag = true;
					}
					else
					{
						num += Mathf.Abs(Vector3.Dot(array[i].delta - b, dir));
						num2++;
					}
				}
			}
			if (num2 <= 0)
			{
				return 0f;
			}
			return num / (float)num2;
		}

		// Token: 0x06006E1D RID: 28189 RVA: 0x00240830 File Offset: 0x0023EA30
		[CompilerGenerated]
		private void <ResetState>g__PopulateArray|20_0(GorillaVelocityTracker.VelocityDataPoint[] array)
		{
			for (int i = 0; i < this.maxDataPoints; i++)
			{
				array[i] = new GorillaVelocityTracker.VelocityDataPoint();
			}
		}

		// Token: 0x06006E1E RID: 28190 RVA: 0x00240858 File Offset: 0x0023EA58
		[CompilerGenerated]
		internal static void <GetAverageVelocity>g__AddPoint|28_0(GorillaVelocityTracker.VelocityDataPoint point, ref GorillaVelocityTracker.<>c__DisplayClass28_0 A_1)
		{
			A_1.total += point.delta;
			A_1.totalMag += point.delta.magnitude;
			int added = A_1.added;
			A_1.added = added + 1;
		}

		// Token: 0x04007F2B RID: 32555
		[SerializeField]
		private int maxDataPoints = 20;

		// Token: 0x04007F2C RID: 32556
		[SerializeField]
		private Transform relativeTo;

		// Token: 0x04007F2D RID: 32557
		[Tooltip("Use in Editor to trigger events when above or higher than a desired latest velocity.")]
		[SerializeField]
		private bool useVelocityEvents;

		// Token: 0x04007F2E RID: 32558
		[SerializeField]
		private float latestVelocityThreshold;

		// Token: 0x04007F2F RID: 32559
		public UnityEvent OnLatestBelowThreshold;

		// Token: 0x04007F30 RID: 32560
		public UnityEvent OnLatestAboveThreshold;

		// Token: 0x04007F31 RID: 32561
		[SerializeField]
		private bool useWorldSpaceForEvents;

		// Token: 0x04007F32 RID: 32562
		private bool wasAboveThreshold;

		// Token: 0x04007F33 RID: 32563
		private int currentDataPointIndex;

		// Token: 0x04007F34 RID: 32564
		private GorillaVelocityTracker.VelocityDataPoint[] localSpaceData;

		// Token: 0x04007F35 RID: 32565
		private GorillaVelocityTracker.VelocityDataPoint[] worldSpaceData;

		// Token: 0x04007F36 RID: 32566
		private Transform trans;

		// Token: 0x04007F37 RID: 32567
		private Vector3 lastWorldSpacePos;

		// Token: 0x04007F38 RID: 32568
		private Vector3 lastLocalSpacePos;

		// Token: 0x04007F39 RID: 32569
		private bool isRelativeTo;

		// Token: 0x04007F3A RID: 32570
		private int lastTickedFrame = -1;

		// Token: 0x02001112 RID: 4370
		public class VelocityDataPoint
		{
			// Token: 0x04007F3C RID: 32572
			public Vector3 delta;

			// Token: 0x04007F3D RID: 32573
			public float time = -1f;
		}
	}
}
