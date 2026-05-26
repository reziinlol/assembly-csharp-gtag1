using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x0200115A RID: 4442
	public class DayNightWatchWearable : MonoBehaviour
	{
		// Token: 0x06007075 RID: 28789 RVA: 0x0024A8C4 File Offset: 0x00248AC4
		private void Start()
		{
			if (!this.dayNightManager)
			{
				this.dayNightManager = BetterDayNightManager.instance;
			}
			this.rotationDegree = 0f;
			if (this.clockNeedle)
			{
				this.initialRotation = this.clockNeedle.localRotation;
			}
		}

		// Token: 0x06007076 RID: 28790 RVA: 0x0024A914 File Offset: 0x00248B14
		private void Update()
		{
			this.currentTimeOfDay = this.dayNightManager.currentTimeOfDay;
			double currentTimeInSeconds = ((ITimeOfDaySystem)this.dayNightManager).currentTimeInSeconds;
			double totalTimeInSeconds = ((ITimeOfDaySystem)this.dayNightManager).totalTimeInSeconds;
			this.rotationDegree = (float)(360.0 * currentTimeInSeconds / totalTimeInSeconds);
			this.rotationDegree = Mathf.Floor(this.rotationDegree);
			if (this.clockNeedle)
			{
				this.clockNeedle.localRotation = this.initialRotation * Quaternion.AngleAxis(this.rotationDegree, this.needleRotationAxis);
			}
		}

		// Token: 0x04008055 RID: 32853
		[Tooltip("The transform that will be rotated to indicate the current time.")]
		public Transform clockNeedle;

		// Token: 0x04008056 RID: 32854
		[FormerlySerializedAs("dialRotationAxis")]
		[Tooltip("The axis that the needle will rotate around.")]
		public Vector3 needleRotationAxis = Vector3.right;

		// Token: 0x04008057 RID: 32855
		private BetterDayNightManager dayNightManager;

		// Token: 0x04008058 RID: 32856
		[DebugOption]
		private float rotationDegree;

		// Token: 0x04008059 RID: 32857
		private string currentTimeOfDay;

		// Token: 0x0400805A RID: 32858
		private Quaternion initialRotation;
	}
}
