using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200113B RID: 4411
	public static class GTTime
	{
		// Token: 0x17000AB6 RID: 2742
		// (get) Token: 0x06007006 RID: 28678 RVA: 0x00248FD0 File Offset: 0x002471D0
		// (set) Token: 0x06007007 RID: 28679 RVA: 0x00248FD7 File Offset: 0x002471D7
		public static TimeZoneInfo timeZoneInfoLA { get; private set; }

		// Token: 0x06007008 RID: 28680 RVA: 0x00248FDF File Offset: 0x002471DF
		static GTTime()
		{
			GTTime._Init();
		}

		// Token: 0x06007009 RID: 28681 RVA: 0x00248FE8 File Offset: 0x002471E8
		[RuntimeInitializeOnLoadMethod]
		private static void _Init()
		{
			if (GTTime._isInitialized)
			{
				return;
			}
			try
			{
				GTTime.timeZoneInfoLA = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
			}
			catch
			{
				try
				{
					GTTime.timeZoneInfoLA = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
				}
				catch
				{
					TimeZoneInfo timeZoneInfoLA;
					if (GTTime._TryCreateCustomPST(out timeZoneInfoLA))
					{
						GTTime.timeZoneInfoLA = timeZoneInfoLA;
						Debug.Log("[GTTime]  _Init: Could not get US Pacific Time Zone, so using manual created Pacific time zone instead.");
					}
					else
					{
						Debug.LogError("[GTTime]  ERROR!!!  _Init: Could not get US Pacific Time Zone and manual Pacific time zone creation failed. Using UTC instead.");
						GTTime.timeZoneInfoLA = TimeZoneInfo.Utc;
					}
				}
			}
			finally
			{
				GTTime._isInitialized = true;
			}
		}

		// Token: 0x0600700A RID: 28682 RVA: 0x00249084 File Offset: 0x00247284
		private static bool _TryCreateCustomPST(out TimeZoneInfo out_tz)
		{
			TimeZoneInfo.AdjustmentRule[] adjustmentRules = new TimeZoneInfo.AdjustmentRule[]
			{
				TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2007, 1, 1), DateTime.MaxValue.Date, TimeSpan.FromHours(1.0), TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 3, 2, DayOfWeek.Sunday), TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 11, 1, DayOfWeek.Sunday))
			};
			bool result;
			try
			{
				out_tz = TimeZoneInfo.CreateCustomTimeZone("Custom/America_Los_Angeles", TimeSpan.FromHours(-8.0), "(UTC-08:00) Pacific Time (US & Canada)", "Pacific Standard Time", "Pacific Daylight Time", adjustmentRules, false);
				result = true;
			}
			catch (Exception ex)
			{
				Debug.LogError("[GTTime]  ERROR!!!  _TryCreateCustomPST: Encountered exception: " + ex.Message);
				out_tz = null;
				result = false;
			}
			return result;
		}

		// Token: 0x17000AB7 RID: 2743
		// (get) Token: 0x0600700B RID: 28683 RVA: 0x00249148 File Offset: 0x00247348
		// (set) Token: 0x0600700C RID: 28684 RVA: 0x0024914F File Offset: 0x0024734F
		public static bool usingServerTime { get; private set; }

		// Token: 0x0600700D RID: 28685 RVA: 0x00249157 File Offset: 0x00247357
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long GetServerStartupTimeAsMilliseconds()
		{
			return GorillaComputer.instance.startupMillis;
		}

		// Token: 0x0600700E RID: 28686 RVA: 0x00249168 File Offset: 0x00247368
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long GetDeviceStartupTimeAsMilliseconds()
		{
			return (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - Time.realtimeSinceStartupAsDouble * 1000.0);
		}

		// Token: 0x0600700F RID: 28687 RVA: 0x002491A0 File Offset: 0x002473A0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetStartupTimeAsMilliseconds()
		{
			GTTime.usingServerTime = true;
			long num = 0L;
			if (GorillaComputer.hasInstance)
			{
				num = GTTime.GetServerStartupTimeAsMilliseconds();
			}
			if (num == 0L)
			{
				GTTime.usingServerTime = false;
				num = GTTime.GetDeviceStartupTimeAsMilliseconds();
			}
			return num;
		}

		// Token: 0x06007010 RID: 28688 RVA: 0x002491D3 File Offset: 0x002473D3
		public static long TimeAsMilliseconds()
		{
			return GTTime.GetStartupTimeAsMilliseconds() + (long)(Time.realtimeSinceStartupAsDouble * 1000.0);
		}

		// Token: 0x06007011 RID: 28689 RVA: 0x002491EB File Offset: 0x002473EB
		public static double TimeAsDouble()
		{
			return (double)GTTime.GetStartupTimeAsMilliseconds() / 1000.0 + Time.realtimeSinceStartupAsDouble;
		}

		// Token: 0x06007012 RID: 28690 RVA: 0x00249203 File Offset: 0x00247403
		public static DateTime GetAAxiomDateTime()
		{
			return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GTTime.timeZoneInfoLA);
		}

		// Token: 0x06007013 RID: 28691 RVA: 0x00249214 File Offset: 0x00247414
		public static string GetAAxiomDateTimeAsStringForDisplay()
		{
			return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
		}

		// Token: 0x06007014 RID: 28692 RVA: 0x00249234 File Offset: 0x00247434
		public static string GetAAxiomDateTimeAsStringForFilename()
		{
			return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd_HH-mm-ss-fff");
		}

		// Token: 0x06007015 RID: 28693 RVA: 0x00249254 File Offset: 0x00247454
		public static long GetAAxiomDateTimeAsHumanReadableLong()
		{
			return long.Parse(GTTime.GetAAxiomDateTime().ToString("yyyyMMddHHmmssfff00"));
		}

		// Token: 0x06007016 RID: 28694 RVA: 0x00249278 File Offset: 0x00247478
		public static DateTime ConvertDateTimeHumanReadableLongToDateTime(long humanReadableLong)
		{
			return DateTime.ParseExact(humanReadableLong.ToString(), "yyyyMMddHHmmssfff'00'", CultureInfo.InvariantCulture);
		}

		// Token: 0x06007017 RID: 28695 RVA: 0x00249290 File Offset: 0x00247490
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryUpdateTimeText(TMP_Text textComponent, TimeSpan timeSpan, char[] chars, int index, ref int ref_lastUpdateSeconds)
		{
			int num = (int)timeSpan.TotalSeconds;
			if (ref_lastUpdateSeconds == num)
			{
				return false;
			}
			ref_lastUpdateSeconds = num;
			int num2 = Mathf.Clamp((int)timeSpan.TotalHours, 0, 99);
			int minutes = timeSpan.Minutes;
			int seconds = timeSpan.Seconds;
			chars[index] = (char)(48 + num2 / 10);
			chars[index + 1] = (char)(48 + num2 % 10);
			chars[index + 3] = (char)(48 + minutes / 10);
			chars[index + 4] = (char)(48 + minutes % 10);
			chars[index + 6] = (char)(48 + seconds / 10);
			chars[index + 7] = (char)(48 + seconds % 10);
			textComponent.SetCharArray(chars);
			return true;
		}

		// Token: 0x04008000 RID: 32768
		private const string preLog = "[GTTime]  ";

		// Token: 0x04008001 RID: 32769
		private const string preErr = "[GTTime]  ERROR!!!  ";

		// Token: 0x04008002 RID: 32770
		private static bool _isInitialized;
	}
}
