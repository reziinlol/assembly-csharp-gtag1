using System;
using System.Collections.Generic;
using UnityEngine;

namespace PerformanceSystems
{
	// Token: 0x02000EA6 RID: 3750
	[CreateAssetMenu(menuName = "PerformanceTools/TimeSlicer/TimeSliceController", fileName = "TimeSliceController")]
	public class TimeSliceControllerAsset : ScriptableObject
	{
		// Token: 0x170008D5 RID: 2261
		// (get) Token: 0x06005C13 RID: 23571 RVA: 0x001D4029 File Offset: 0x001D2229
		public Transform ReferenceTransform
		{
			get
			{
				return this._referenceTransform;
			}
		}

		// Token: 0x06005C14 RID: 23572 RVA: 0x001D4031 File Offset: 0x001D2231
		private void RemovePendingObjects()
		{
			this._currentTimeSliceBehaviours.FastRemove(this._timeSliceBehavioursToRemove);
			this._timeSliceBehavioursToRemove.Clear();
		}

		// Token: 0x06005C15 RID: 23573 RVA: 0x001D4050 File Offset: 0x001D2250
		private void AddPendingObjects()
		{
			foreach (ITimeSlice item in this._timeSliceBehavioursToAdd)
			{
				if (!this._currentTimeSliceBehaviours.Contains(item))
				{
					this._currentTimeSliceBehaviours.Add(item);
				}
			}
			this._timeSliceBehavioursToAdd.Clear();
		}

		// Token: 0x06005C16 RID: 23574 RVA: 0x001D40C4 File Offset: 0x001D22C4
		private void UpdateCurrentSliceObjects()
		{
			int count = this._currentTimeSliceBehaviours.Count;
			if (count == 0)
			{
				return;
			}
			int num = Mathf.Max(1, this._timeSlices);
			this._sliceSize = Mathf.CeilToInt((float)count / (float)num);
			if (this._sliceSize <= 0)
			{
				this._sliceSize = 1;
			}
			int num2 = this._sliceSize * this._currentSlice;
			if (num2 >= count)
			{
				num2 = Mathf.Max(0, count - this._sliceSize);
			}
			int num3 = Mathf.Min(this._sliceSize, count - num2);
			if (num3 <= 0)
			{
				return;
			}
			for (int i = 0; i < num3; i++)
			{
				int num4 = num2 + i;
				if (num4 < 0 || num4 >= this._currentTimeSliceBehaviours.Count)
				{
					break;
				}
				ITimeSlice timeSlice = this._currentTimeSliceBehaviours[num4];
				if (timeSlice != null)
				{
					timeSlice.SliceUpdate();
				}
			}
		}

		// Token: 0x06005C17 RID: 23575 RVA: 0x001D4185 File Offset: 0x001D2385
		public void SetRefTransform(Transform refTransform)
		{
			this._referenceTransform = refTransform;
			this._isActive = (this._referenceTransform != null);
		}

		// Token: 0x06005C18 RID: 23576 RVA: 0x001D41A0 File Offset: 0x001D23A0
		public void AddTimeSliceBehaviour(ITimeSlice timeSlice)
		{
			if (this._currentTimeSliceBehaviours.Contains(timeSlice))
			{
				return;
			}
			this._timeSliceBehavioursToAdd.Add(timeSlice);
		}

		// Token: 0x06005C19 RID: 23577 RVA: 0x001D41BE File Offset: 0x001D23BE
		public void RemoveTimeSliceBehaviour(ITimeSlice timeSlice)
		{
			if (!this._currentTimeSliceBehaviours.Contains(timeSlice))
			{
				this._timeSliceBehavioursToRemove.Remove(timeSlice);
				return;
			}
			this._timeSliceBehavioursToRemove.Add(timeSlice);
		}

		// Token: 0x06005C1A RID: 23578 RVA: 0x001D41EC File Offset: 0x001D23EC
		public void Update()
		{
			this.InitializeReferenceTransformWithMainCam();
			if (!this._isActive)
			{
				return;
			}
			if (this._currentSlice == 0)
			{
				this.RemovePendingObjects();
				this.AddPendingObjects();
			}
			this.UpdateCurrentSliceObjects();
			this._currentSlice = (this._currentSlice + 1) % Mathf.Max(1, this._timeSlices);
		}

		// Token: 0x06005C1B RID: 23579 RVA: 0x001D423D File Offset: 0x001D243D
		public void InitializeReferenceTransformWithMainCam()
		{
			if (this._referenceTransform == null)
			{
				Camera main = Camera.main;
				this._referenceTransform = ((main != null) ? main.transform : null);
			}
			this._isActive = (this._referenceTransform != null);
		}

		// Token: 0x06005C1C RID: 23580 RVA: 0x001D4276 File Offset: 0x001D2476
		private void OnDisable()
		{
			this.ClearAsset();
		}

		// Token: 0x06005C1D RID: 23581 RVA: 0x001D427E File Offset: 0x001D247E
		public void ClearAsset()
		{
			this._currentTimeSliceBehaviours.Clear();
			this._timeSliceBehavioursToAdd.Clear();
			this._timeSliceBehavioursToRemove.Clear();
			this._referenceTransform = null;
		}

		// Token: 0x04006A89 RID: 27273
		private readonly List<ITimeSlice> _currentTimeSliceBehaviours = new List<ITimeSlice>();

		// Token: 0x04006A8A RID: 27274
		private readonly HashSet<ITimeSlice> _timeSliceBehavioursToAdd = new HashSet<ITimeSlice>();

		// Token: 0x04006A8B RID: 27275
		private readonly HashSet<ITimeSlice> _timeSliceBehavioursToRemove = new HashSet<ITimeSlice>();

		// Token: 0x04006A8C RID: 27276
		private Transform _referenceTransform;

		// Token: 0x04006A8D RID: 27277
		[Range(1f, 150f)]
		[SerializeField]
		private int _timeSlices = 1;

		// Token: 0x04006A8E RID: 27278
		private int _currentSlice;

		// Token: 0x04006A8F RID: 27279
		private bool _isActive;

		// Token: 0x04006A90 RID: 27280
		private int _sliceSize;
	}
}
