using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009BA RID: 2490
public class RadialBoundsTrigger : MonoBehaviour
{
	// Token: 0x06003FBC RID: 16316 RVA: 0x00154B1A File Offset: 0x00152D1A
	public void TestOverlap()
	{
		this.TestOverlap(this._raiseEvents);
	}

	// Token: 0x06003FBD RID: 16317 RVA: 0x00154B28 File Offset: 0x00152D28
	public void TestOverlap(bool raiseEvents)
	{
		if (!this.object1 || !this.object2)
		{
			this._overlapping = false;
			this._timeOverlapStarted = -1f;
			this._timeOverlapStopped = -1f;
			this._timeSpentInOverlap = 0f;
			return;
		}
		float time = Time.time;
		float num = this.object1.radius + this.object2.radius;
		bool flag = (this.object2.center - this.object1.center).sqrMagnitude <= num * num;
		if (this._overlapping && flag)
		{
			this._overlapping = true;
			this._timeSpentInOverlap = time - this._timeOverlapStarted;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds, float> onOverlapStay = this.object1.onOverlapStay;
				if (onOverlapStay != null)
				{
					onOverlapStay.Invoke(this.object2, this._timeSpentInOverlap);
				}
				UnityEvent<RadialBounds, float> onOverlapStay2 = this.object2.onOverlapStay;
				if (onOverlapStay2 == null)
				{
					return;
				}
				onOverlapStay2.Invoke(this.object1, this._timeSpentInOverlap);
				return;
			}
		}
		else if (!this._overlapping && flag)
		{
			if (time - this._timeOverlapStopped < this.hysteresis)
			{
				return;
			}
			this._overlapping = true;
			this._timeOverlapStarted = time;
			this._timeOverlapStopped = -1f;
			this._timeSpentInOverlap = 0f;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds> onOverlapEnter = this.object1.onOverlapEnter;
				if (onOverlapEnter != null)
				{
					onOverlapEnter.Invoke(this.object2);
				}
				UnityEvent<RadialBounds> onOverlapEnter2 = this.object2.onOverlapEnter;
				if (onOverlapEnter2 == null)
				{
					return;
				}
				onOverlapEnter2.Invoke(this.object1);
				return;
			}
		}
		else if (!flag && this._overlapping)
		{
			this._overlapping = false;
			this._timeOverlapStarted = -1f;
			this._timeOverlapStopped = time;
			this._timeSpentInOverlap = 0f;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds> onOverlapExit = this.object1.onOverlapExit;
				if (onOverlapExit != null)
				{
					onOverlapExit.Invoke(this.object2);
				}
				UnityEvent<RadialBounds> onOverlapExit2 = this.object2.onOverlapExit;
				if (onOverlapExit2 == null)
				{
					return;
				}
				onOverlapExit2.Invoke(this.object1);
			}
		}
	}

	// Token: 0x06003FBE RID: 16318 RVA: 0x00154D14 File Offset: 0x00152F14
	private void FixedUpdate()
	{
		this.TestOverlap();
	}

	// Token: 0x06003FBF RID: 16319 RVA: 0x00154D1C File Offset: 0x00152F1C
	private void OnDisable()
	{
		if (this._raiseEvents && this.object1 && this.object2 && this._overlapping)
		{
			UnityEvent<RadialBounds> onOverlapExit = this.object1.onOverlapExit;
			if (onOverlapExit != null)
			{
				onOverlapExit.Invoke(this.object2);
			}
			UnityEvent<RadialBounds> onOverlapExit2 = this.object2.onOverlapExit;
			if (onOverlapExit2 != null)
			{
				onOverlapExit2.Invoke(this.object1);
			}
		}
		this._timeOverlapStarted = -1f;
		this._timeSpentInOverlap = 0f;
		this._overlapping = false;
	}

	// Token: 0x0400502A RID: 20522
	[SerializeField]
	private Id32 _triggerID;

	// Token: 0x0400502B RID: 20523
	[Space]
	public RadialBounds object1 = new RadialBounds();

	// Token: 0x0400502C RID: 20524
	[Space]
	public RadialBounds object2 = new RadialBounds();

	// Token: 0x0400502D RID: 20525
	[Space]
	public float hysteresis = 0.5f;

	// Token: 0x0400502E RID: 20526
	[SerializeField]
	private bool _raiseEvents = true;

	// Token: 0x0400502F RID: 20527
	[Space]
	private bool _overlapping;

	// Token: 0x04005030 RID: 20528
	private float _timeSpentInOverlap;

	// Token: 0x04005031 RID: 20529
	[Space]
	private float _timeOverlapStarted;

	// Token: 0x04005032 RID: 20530
	private float _timeOverlapStopped;
}
