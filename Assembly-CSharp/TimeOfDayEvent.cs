using System;
using UnityEngine;

// Token: 0x02000DC4 RID: 3524
public class TimeOfDayEvent : TimeEvent
{
	// Token: 0x1700081F RID: 2079
	// (get) Token: 0x0600565D RID: 22109 RVA: 0x001C06D1 File Offset: 0x001BE8D1
	public float currentTime
	{
		get
		{
			return this._currentTime;
		}
	}

	// Token: 0x17000820 RID: 2080
	// (get) Token: 0x0600565E RID: 22110 RVA: 0x001C06D9 File Offset: 0x001BE8D9
	// (set) Token: 0x0600565F RID: 22111 RVA: 0x001C06E1 File Offset: 0x001BE8E1
	public float timeStart
	{
		get
		{
			return this._timeStart;
		}
		set
		{
			this._timeStart = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000821 RID: 2081
	// (get) Token: 0x06005660 RID: 22112 RVA: 0x001C06EF File Offset: 0x001BE8EF
	// (set) Token: 0x06005661 RID: 22113 RVA: 0x001C06F7 File Offset: 0x001BE8F7
	public float timeEnd
	{
		get
		{
			return this._timeEnd;
		}
		set
		{
			this._timeEnd = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000822 RID: 2082
	// (get) Token: 0x06005662 RID: 22114 RVA: 0x001C0705 File Offset: 0x001BE905
	public bool isOngoing
	{
		get
		{
			return this._ongoing;
		}
	}

	// Token: 0x06005663 RID: 22115 RVA: 0x001C0710 File Offset: 0x001BE910
	private void Start()
	{
		if (!this._dayNightManager)
		{
			this._dayNightManager = BetterDayNightManager.instance;
		}
		if (!this._dayNightManager)
		{
			return;
		}
		for (int i = 0; i < this._dayNightManager.timeOfDayRange.Length; i++)
		{
			this._totalSecondsInRange += this._dayNightManager.timeOfDayRange[i] * 3600.0;
		}
		this._totalSecondsInRange = Math.Floor(this._totalSecondsInRange);
	}

	// Token: 0x06005664 RID: 22116 RVA: 0x001C0792 File Offset: 0x001BE992
	private void Update()
	{
		this._elapsed += Time.deltaTime;
		if (this._elapsed < 1f)
		{
			return;
		}
		this._elapsed = 0f;
		this.UpdateTime();
	}

	// Token: 0x06005665 RID: 22117 RVA: 0x001C07C8 File Offset: 0x001BE9C8
	private void UpdateTime()
	{
		this._currentSeconds = ((ITimeOfDaySystem)this._dayNightManager).currentTimeInSeconds;
		this._currentSeconds = Math.Floor(this._currentSeconds);
		this._currentTime = (float)(this._currentSeconds / this._totalSecondsInRange);
		bool flag = this._currentTime >= 0f && this._currentTime >= this._timeStart && this._currentTime <= this._timeEnd;
		if (!this._ongoing && flag)
		{
			base.StartEvent();
		}
		if (this._ongoing && !flag)
		{
			base.StopEvent();
		}
	}

	// Token: 0x06005666 RID: 22118 RVA: 0x001C085F File Offset: 0x001BEA5F
	public static implicit operator bool(TimeOfDayEvent ev)
	{
		return ev && ev.isOngoing;
	}

	// Token: 0x04006643 RID: 26179
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeStart;

	// Token: 0x04006644 RID: 26180
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeEnd = 1f;

	// Token: 0x04006645 RID: 26181
	[SerializeField]
	private float _currentTime = -1f;

	// Token: 0x04006646 RID: 26182
	[Space]
	[SerializeField]
	private double _currentSeconds = -1.0;

	// Token: 0x04006647 RID: 26183
	[SerializeField]
	private double _totalSecondsInRange = -1.0;

	// Token: 0x04006648 RID: 26184
	[NonSerialized]
	private float _elapsed = -1f;

	// Token: 0x04006649 RID: 26185
	[SerializeField]
	private BetterDayNightManager _dayNightManager;
}
