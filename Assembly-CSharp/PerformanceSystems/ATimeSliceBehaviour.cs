using System;
using UnityEngine;

namespace PerformanceSystems
{
	// Token: 0x02000EA2 RID: 3746
	public abstract class ATimeSliceBehaviour : MonoBehaviour, ITimeSlice
	{
		// Token: 0x06005C02 RID: 23554 RVA: 0x001D3EF1 File Offset: 0x001D20F1
		protected void Awake()
		{
			this._timeSliceControllerAsset.AddTimeSliceBehaviour(this);
		}

		// Token: 0x06005C03 RID: 23555 RVA: 0x001D3EFF File Offset: 0x001D20FF
		protected void OnDestroy()
		{
			this._timeSliceControllerAsset.RemoveTimeSliceBehaviour(this);
		}

		// Token: 0x06005C04 RID: 23556 RVA: 0x001D3F10 File Offset: 0x001D2110
		public void SliceUpdate()
		{
			float deltaTime = Time.realtimeSinceStartup - this._lastUpdateTime;
			this._lastUpdateTime = Time.realtimeSinceStartup;
			this.SliceUpdateAlways(deltaTime);
			if (this._updateIfDisabled || base.gameObject.activeSelf)
			{
				this.SliceUpdate(deltaTime);
			}
		}

		// Token: 0x06005C05 RID: 23557
		public abstract void SliceUpdate(float deltaTime);

		// Token: 0x06005C06 RID: 23558
		public abstract void SliceUpdateAlways(float deltaTime);

		// Token: 0x04006A86 RID: 27270
		[SerializeField]
		protected TimeSliceControllerAsset _timeSliceControllerAsset;

		// Token: 0x04006A87 RID: 27271
		[SerializeField]
		protected bool _updateIfDisabled = true;

		// Token: 0x04006A88 RID: 27272
		protected float _lastUpdateTime;
	}
}
