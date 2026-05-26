using System;
using System.Globalization;
using UnityEngine;

// Token: 0x02000308 RID: 776
[Serializable]
public struct GTDateTimeSerializable : ISerializationCallbackReceiver
{
	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x060013B1 RID: 5041 RVA: 0x0006B12C File Offset: 0x0006932C
	// (set) Token: 0x060013B2 RID: 5042 RVA: 0x0006B134 File Offset: 0x00069334
	public DateTime dateTime
	{
		get
		{
			return this._dateTime;
		}
		set
		{
			this._dateTime = value;
			this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
		}
	}

	// Token: 0x060013B3 RID: 5043 RVA: 0x0006B14E File Offset: 0x0006934E
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
	}

	// Token: 0x060013B4 RID: 5044 RVA: 0x0006B164 File Offset: 0x00069364
	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		DateTime dateTime;
		if (GTDateTimeSerializable.TryParseDateTime(this._dateTimeString, out dateTime))
		{
			this._dateTime = dateTime;
		}
	}

	// Token: 0x060013B5 RID: 5045 RVA: 0x0006B188 File Offset: 0x00069388
	public GTDateTimeSerializable(int dummyValue)
	{
		DateTime now = DateTime.Now;
		this._dateTime = new DateTime(now.Year, now.Month, now.Day, 11, 0, 0);
		this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
	}

	// Token: 0x060013B6 RID: 5046 RVA: 0x0006B1D0 File Offset: 0x000693D0
	private static string FormatDateTime(DateTime dateTime)
	{
		return dateTime.ToString("yyyy-MM-dd HH:mm");
	}

	// Token: 0x060013B7 RID: 5047 RVA: 0x0006B1E0 File Offset: 0x000693E0
	private static bool TryParseDateTime(string value, out DateTime result)
	{
		if (DateTime.TryParseExact(value, new string[]
		{
			"yyyy-MM-dd HH:mm",
			"yyyy-MM-dd",
			"yyyy-MM"
		}, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
		{
			DateTime dateTime = result;
			if (dateTime.Hour == 0 && dateTime.Minute == 0)
			{
				result = result.AddHours(11.0);
			}
			return true;
		}
		return false;
	}

	// Token: 0x0400184A RID: 6218
	[HideInInspector]
	[SerializeField]
	private string _dateTimeString;

	// Token: 0x0400184B RID: 6219
	private DateTime _dateTime;
}
