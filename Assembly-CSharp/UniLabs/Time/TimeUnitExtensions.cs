using System;

namespace UniLabs.Time
{
	// Token: 0x02000E94 RID: 3732
	public static class TimeUnitExtensions
	{
		// Token: 0x06005B8F RID: 23439 RVA: 0x001D28BC File Offset: 0x001D0ABC
		public static string ToShortString(this TimeUnit timeUnit)
		{
			string result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = "";
				break;
			case TimeUnit.Milliseconds:
				result = "ms";
				break;
			case TimeUnit.Seconds:
				result = "s";
				break;
			case TimeUnit.Minutes:
				result = "m";
				break;
			case TimeUnit.Hours:
				result = "h";
				break;
			case TimeUnit.Days:
				result = "D";
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06005B90 RID: 23440 RVA: 0x001D292C File Offset: 0x001D0B2C
		public static string ToSeparatorString(this TimeUnit timeUnit)
		{
			string result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = "";
				break;
			case TimeUnit.Milliseconds:
				result = "";
				break;
			case TimeUnit.Seconds:
				result = ".";
				break;
			case TimeUnit.Minutes:
				result = ":";
				break;
			case TimeUnit.Hours:
				result = ":";
				break;
			case TimeUnit.Days:
				result = ".";
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06005B91 RID: 23441 RVA: 0x001D299C File Offset: 0x001D0B9C
		public static double GetUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			int num;
			switch (timeUnit)
			{
			case TimeUnit.None:
				num = 0;
				break;
			case TimeUnit.Milliseconds:
				num = timeSpan.Milliseconds;
				break;
			case TimeUnit.Seconds:
				num = timeSpan.Seconds;
				break;
			case TimeUnit.Minutes:
				num = timeSpan.Minutes;
				break;
			case TimeUnit.Hours:
				num = timeSpan.Hours;
				break;
			case TimeUnit.Days:
				num = timeSpan.Days;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return (double)num;
		}

		// Token: 0x06005B92 RID: 23442 RVA: 0x001D2A14 File Offset: 0x001D0C14
		public static TimeSpan WithUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = timeSpan.Add(TimeSpan.FromMilliseconds(value - (double)timeSpan.Milliseconds));
				break;
			case TimeUnit.Seconds:
				result = timeSpan.Add(TimeSpan.FromSeconds(value - (double)timeSpan.Seconds));
				break;
			case TimeUnit.Minutes:
				result = timeSpan.Add(TimeSpan.FromMinutes(value - (double)timeSpan.Minutes));
				break;
			case TimeUnit.Hours:
				result = timeSpan.Add(TimeSpan.FromHours(value - (double)timeSpan.Hours));
				break;
			case TimeUnit.Days:
				result = timeSpan.Add(TimeSpan.FromDays(value - (double)timeSpan.Days));
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06005B93 RID: 23443 RVA: 0x001D2AD8 File Offset: 0x001D0CD8
		public static double GetLowestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = 0.0;
				break;
			case TimeUnit.Milliseconds:
				result = (double)timeSpan.Milliseconds;
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(0, 0, 0, timeSpan.Seconds, timeSpan.Milliseconds).TotalSeconds;
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(0, 0, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).TotalMinutes;
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(0, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).TotalHours;
				break;
			case TimeUnit.Days:
				result = timeSpan.TotalDays;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06005B94 RID: 23444 RVA: 0x001D2BB4 File Offset: 0x001D0DB4
		public static TimeSpan WithLowestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, (int)value);
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0).Add(TimeSpan.FromSeconds(value));
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0).Add(TimeSpan.FromMinutes(value));
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(timeSpan.Days, 0, 0, 0).Add(TimeSpan.FromHours(value));
				break;
			case TimeUnit.Days:
				result = TimeSpan.FromDays(value);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06005B95 RID: 23445 RVA: 0x001D2CA0 File Offset: 0x001D0EA0
		public static double GetHighestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = 0.0;
				break;
			case TimeUnit.Milliseconds:
				result = timeSpan.TotalMilliseconds;
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds).TotalSeconds;
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0).TotalMinutes;
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0).TotalHours;
				break;
			case TimeUnit.Days:
				result = (double)timeSpan.Days;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06005B96 RID: 23446 RVA: 0x001D2D7C File Offset: 0x001D0F7C
		public static TimeSpan WithHighestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = TimeSpan.FromMilliseconds(value);
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(0, 0, 0, 0, timeSpan.Milliseconds).Add(TimeSpan.FromSeconds(value));
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(0, 0, 0, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromMinutes(value));
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(0, 0, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromHours(value));
				break;
			case TimeUnit.Days:
				result = new TimeSpan(0, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromDays(value));
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06005B97 RID: 23447 RVA: 0x001D2E7C File Offset: 0x001D107C
		public static double GetSingleUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double result;
			switch (timeUnit)
			{
			case TimeUnit.Milliseconds:
				result = timeSpan.TotalMilliseconds;
				break;
			case TimeUnit.Seconds:
				result = timeSpan.TotalSeconds;
				break;
			case TimeUnit.Minutes:
				result = timeSpan.TotalMinutes;
				break;
			case TimeUnit.Hours:
				result = timeSpan.TotalHours;
				break;
			case TimeUnit.Days:
				result = timeSpan.TotalDays;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06005B98 RID: 23448 RVA: 0x001D2EEC File Offset: 0x001D10EC
		public static TimeSpan FromSingleUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = TimeSpan.Zero;
				break;
			case TimeUnit.Milliseconds:
				result = TimeSpan.FromMilliseconds(value);
				break;
			case TimeUnit.Seconds:
				result = TimeSpan.FromSeconds(value);
				break;
			case TimeUnit.Minutes:
				result = TimeSpan.FromMinutes(value);
				break;
			case TimeUnit.Hours:
				result = TimeSpan.FromHours(value);
				break;
			case TimeUnit.Days:
				result = TimeSpan.FromDays(value);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06005B99 RID: 23449 RVA: 0x001D2F64 File Offset: 0x001D1164
		public static TimeSpan SnapToUnit(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = timeSpan;
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0);
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0);
				break;
			case TimeUnit.Days:
				result = new TimeSpan(timeSpan.Days, 0, 0, 0);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x02000E95 RID: 3733
		// (Invoke) Token: 0x06005B9B RID: 23451
		public delegate TimeSpan WithUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit, double value);

		// Token: 0x02000E96 RID: 3734
		// (Invoke) Token: 0x06005B9F RID: 23455
		public delegate double GetUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit);
	}
}
