using System;

// Token: 0x02000AF2 RID: 2802
public struct TimeSince
{
	// Token: 0x170006A9 RID: 1705
	// (get) Token: 0x060047AC RID: 18348 RVA: 0x00181260 File Offset: 0x0017F460
	public double secondsElapsed
	{
		get
		{
			double totalSeconds = (DateTime.UtcNow - this._dt).TotalSeconds;
			if (totalSeconds <= 2147483647.0)
			{
				return totalSeconds;
			}
			return 2147483647.0;
		}
	}

	// Token: 0x170006AA RID: 1706
	// (get) Token: 0x060047AD RID: 18349 RVA: 0x0018129D File Offset: 0x0017F49D
	public float secondsElapsedFloat
	{
		get
		{
			return (float)this.secondsElapsed;
		}
	}

	// Token: 0x170006AB RID: 1707
	// (get) Token: 0x060047AE RID: 18350 RVA: 0x001812A6 File Offset: 0x0017F4A6
	public int secondsElapsedInt
	{
		get
		{
			return (int)this.secondsElapsed;
		}
	}

	// Token: 0x170006AC RID: 1708
	// (get) Token: 0x060047AF RID: 18351 RVA: 0x001812AF File Offset: 0x0017F4AF
	public uint secondsElapsedUint
	{
		get
		{
			return (uint)this.secondsElapsed;
		}
	}

	// Token: 0x170006AD RID: 1709
	// (get) Token: 0x060047B0 RID: 18352 RVA: 0x001812B8 File Offset: 0x0017F4B8
	public long secondsElapsedLong
	{
		get
		{
			return (long)this.secondsElapsed;
		}
	}

	// Token: 0x170006AE RID: 1710
	// (get) Token: 0x060047B1 RID: 18353 RVA: 0x001812C1 File Offset: 0x0017F4C1
	public TimeSpan secondsElapsedSpan
	{
		get
		{
			return TimeSpan.FromSeconds(this.secondsElapsed);
		}
	}

	// Token: 0x060047B2 RID: 18354 RVA: 0x001812CE File Offset: 0x0017F4CE
	public TimeSince(DateTime dt)
	{
		this._dt = dt;
	}

	// Token: 0x060047B3 RID: 18355 RVA: 0x001812D8 File Offset: 0x0017F4D8
	public TimeSince(int elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x060047B4 RID: 18356 RVA: 0x001812FC File Offset: 0x0017F4FC
	public TimeSince(uint elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-1.0 * elapsed);
	}

	// Token: 0x060047B5 RID: 18357 RVA: 0x0018132C File Offset: 0x0017F52C
	public TimeSince(float elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x060047B6 RID: 18358 RVA: 0x00181350 File Offset: 0x0017F550
	public TimeSince(double elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-elapsed);
	}

	// Token: 0x060047B7 RID: 18359 RVA: 0x00181374 File Offset: 0x0017F574
	public TimeSince(long elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x060047B8 RID: 18360 RVA: 0x00181398 File Offset: 0x0017F598
	public TimeSince(TimeSpan elapsed)
	{
		this._dt = DateTime.UtcNow.Add(-elapsed);
	}

	// Token: 0x060047B9 RID: 18361 RVA: 0x001813BE File Offset: 0x0017F5BE
	public bool HasElapsed(int seconds)
	{
		return this.secondsElapsedInt >= seconds;
	}

	// Token: 0x060047BA RID: 18362 RVA: 0x001813CC File Offset: 0x0017F5CC
	public bool HasElapsed(uint seconds)
	{
		return this.secondsElapsedUint >= seconds;
	}

	// Token: 0x060047BB RID: 18363 RVA: 0x001813DA File Offset: 0x0017F5DA
	public bool HasElapsed(float seconds)
	{
		return this.secondsElapsedFloat >= seconds;
	}

	// Token: 0x060047BC RID: 18364 RVA: 0x001813E8 File Offset: 0x0017F5E8
	public bool HasElapsed(double seconds)
	{
		return this.secondsElapsed >= seconds;
	}

	// Token: 0x060047BD RID: 18365 RVA: 0x001813F6 File Offset: 0x0017F5F6
	public bool HasElapsed(long seconds)
	{
		return this.secondsElapsedLong >= seconds;
	}

	// Token: 0x060047BE RID: 18366 RVA: 0x00181404 File Offset: 0x0017F604
	public bool HasElapsed(TimeSpan seconds)
	{
		return this.secondsElapsedSpan >= seconds;
	}

	// Token: 0x060047BF RID: 18367 RVA: 0x00181412 File Offset: 0x0017F612
	public void Reset()
	{
		this._dt = DateTime.UtcNow;
	}

	// Token: 0x060047C0 RID: 18368 RVA: 0x0018141F File Offset: 0x0017F61F
	public bool HasElapsed(int seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedInt >= seconds;
		}
		if (this.secondsElapsedInt < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x060047C1 RID: 18369 RVA: 0x00181443 File Offset: 0x0017F643
	public bool HasElapsed(uint seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedUint >= seconds;
		}
		if (this.secondsElapsedUint < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x060047C2 RID: 18370 RVA: 0x00181467 File Offset: 0x0017F667
	public bool HasElapsed(float seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedFloat >= seconds;
		}
		if (this.secondsElapsedFloat < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x060047C3 RID: 18371 RVA: 0x0018148B File Offset: 0x0017F68B
	public bool HasElapsed(double seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsed >= seconds;
		}
		if (this.secondsElapsed < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x060047C4 RID: 18372 RVA: 0x001814AF File Offset: 0x0017F6AF
	public bool HasElapsed(long seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedLong >= seconds;
		}
		if (this.secondsElapsedLong < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x060047C5 RID: 18373 RVA: 0x001814D3 File Offset: 0x0017F6D3
	public bool HasElapsed(TimeSpan seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedSpan >= seconds;
		}
		if (this.secondsElapsedSpan < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x060047C6 RID: 18374 RVA: 0x001814FC File Offset: 0x0017F6FC
	public override string ToString()
	{
		return string.Format("{0:F3} seconds since {{{1:s}", this.secondsElapsed, this._dt);
	}

	// Token: 0x060047C7 RID: 18375 RVA: 0x0018151E File Offset: 0x0017F71E
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._dt);
	}

	// Token: 0x060047C8 RID: 18376 RVA: 0x0018152B File Offset: 0x0017F72B
	public static TimeSince Now()
	{
		return new TimeSince(DateTime.UtcNow);
	}

	// Token: 0x060047C9 RID: 18377 RVA: 0x00181537 File Offset: 0x0017F737
	public static implicit operator long(TimeSince ts)
	{
		return ts.secondsElapsedLong;
	}

	// Token: 0x060047CA RID: 18378 RVA: 0x00181540 File Offset: 0x0017F740
	public static implicit operator double(TimeSince ts)
	{
		return ts.secondsElapsed;
	}

	// Token: 0x060047CB RID: 18379 RVA: 0x00181549 File Offset: 0x0017F749
	public static implicit operator float(TimeSince ts)
	{
		return ts.secondsElapsedFloat;
	}

	// Token: 0x060047CC RID: 18380 RVA: 0x00181552 File Offset: 0x0017F752
	public static implicit operator int(TimeSince ts)
	{
		return ts.secondsElapsedInt;
	}

	// Token: 0x060047CD RID: 18381 RVA: 0x0018155B File Offset: 0x0017F75B
	public static implicit operator uint(TimeSince ts)
	{
		return ts.secondsElapsedUint;
	}

	// Token: 0x060047CE RID: 18382 RVA: 0x00181564 File Offset: 0x0017F764
	public static implicit operator TimeSpan(TimeSince ts)
	{
		return ts.secondsElapsedSpan;
	}

	// Token: 0x060047CF RID: 18383 RVA: 0x0018156D File Offset: 0x0017F76D
	public static implicit operator TimeSince(int elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x060047D0 RID: 18384 RVA: 0x00181575 File Offset: 0x0017F775
	public static implicit operator TimeSince(uint elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x060047D1 RID: 18385 RVA: 0x0018157D File Offset: 0x0017F77D
	public static implicit operator TimeSince(float elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x060047D2 RID: 18386 RVA: 0x00181585 File Offset: 0x0017F785
	public static implicit operator TimeSince(double elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x060047D3 RID: 18387 RVA: 0x0018158D File Offset: 0x0017F78D
	public static implicit operator TimeSince(long elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x060047D4 RID: 18388 RVA: 0x00181595 File Offset: 0x0017F795
	public static implicit operator TimeSince(TimeSpan elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x060047D5 RID: 18389 RVA: 0x0018159D File Offset: 0x0017F79D
	public static implicit operator TimeSince(DateTime dt)
	{
		return new TimeSince(dt);
	}

	// Token: 0x040059DD RID: 23005
	private DateTime _dt;

	// Token: 0x040059DE RID: 23006
	private const double INT32_MAX = 2147483647.0;
}
