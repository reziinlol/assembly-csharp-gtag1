using System;
using UnityEngine;
using UnityEngine.Events;

namespace PerformanceSystems
{
	// Token: 0x02000EA8 RID: 3752
	public class TimeSliceLodBehaviour : ATimeSliceBehaviour, ILod
	{
		// Token: 0x170008D6 RID: 2262
		// (get) Token: 0x06005C22 RID: 23586 RVA: 0x001D42F2 File Offset: 0x001D24F2
		public Vector3 Position
		{
			get
			{
				return this._transform.position;
			}
		}

		// Token: 0x170008D7 RID: 2263
		// (get) Token: 0x06005C23 RID: 23587 RVA: 0x001D42FF File Offset: 0x001D24FF
		public float[] LodRanges
		{
			get
			{
				return this._lodRanges;
			}
		}

		// Token: 0x170008D8 RID: 2264
		// (get) Token: 0x06005C24 RID: 23588 RVA: 0x001D4307 File Offset: 0x001D2507
		public UnityEvent[] OnLodRangeEvents
		{
			get
			{
				return this._onLodRangeEvents;
			}
		}

		// Token: 0x170008D9 RID: 2265
		// (get) Token: 0x06005C25 RID: 23589 RVA: 0x001D430F File Offset: 0x001D250F
		public UnityEvent OnCulledEvent
		{
			get
			{
				return this._onCulledEvent;
			}
		}

		// Token: 0x170008DA RID: 2266
		// (get) Token: 0x06005C26 RID: 23590 RVA: 0x001D4317 File Offset: 0x001D2517
		public int CurrentLod
		{
			get
			{
				return this._currentLod;
			}
		}

		// Token: 0x06005C27 RID: 23591 RVA: 0x001D431F File Offset: 0x001D251F
		protected void Start()
		{
			this._updateIfDisabled = true;
			this._transform = base.transform;
		}

		// Token: 0x06005C28 RID: 23592 RVA: 0x001D4334 File Offset: 0x001D2534
		protected void SetLod(int newLod)
		{
			if (newLod == this._currentLod)
			{
				return;
			}
			this._currentLod = newLod;
			if (newLod < this._onLodRangeEvents.Length)
			{
				this._onLodRangeEvents[newLod].Invoke();
				return;
			}
			if (newLod == this._onLodRangeEvents.Length)
			{
				this._onCulledEvent.Invoke();
				return;
			}
			Debug.LogWarning(string.Format("No event for LOD [{0}]", newLod), this);
		}

		// Token: 0x06005C29 RID: 23593 RVA: 0x001D4398 File Offset: 0x001D2598
		public void UpdateLod(Vector3 refPos)
		{
			Vector3 position = this._transform.position;
			float num = Vector3.Distance(refPos, position);
			for (int i = 0; i < this._lodRanges.Length; i++)
			{
				float num2 = this._lodRanges[i];
				if (num <= num2)
				{
					this.SetLod(i);
					return;
				}
			}
			this.SetLod(this._lodRanges.Length);
		}

		// Token: 0x06005C2A RID: 23594 RVA: 0x000028C5 File Offset: 0x00000AC5
		public override void SliceUpdate(float deltaTime)
		{
		}

		// Token: 0x06005C2B RID: 23595 RVA: 0x001D43EF File Offset: 0x001D25EF
		public override void SliceUpdateAlways(float deltaTime)
		{
			this.UpdateLod(this._timeSliceControllerAsset.ReferenceTransform.position);
		}

		// Token: 0x04006A92 RID: 27282
		[Space]
		[SerializeField]
		protected int _currentLod = -1;

		// Token: 0x04006A93 RID: 27283
		[SerializeField]
		protected float[] _lodRanges;

		// Token: 0x04006A94 RID: 27284
		[Space]
		[SerializeField]
		protected UnityEvent[] _onLodRangeEvents;

		// Token: 0x04006A95 RID: 27285
		[Space]
		[SerializeField]
		protected UnityEvent _onCulledEvent;

		// Token: 0x04006A96 RID: 27286
		protected Transform _transform;
	}
}
