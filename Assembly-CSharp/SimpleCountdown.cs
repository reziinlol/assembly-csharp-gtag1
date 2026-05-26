using System;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using PlayFab;
using TMPro;
using UnityEngine;

// Token: 0x020001D5 RID: 469
[RequireComponent(typeof(TextMeshPro))]
public class SimpleCountdown : ObservableBehavior
{
	// Token: 0x06000C8A RID: 3210 RVA: 0x0004492C File Offset: 0x00042B2C
	private void Start()
	{
		SimpleCountdown.<Start>d__11 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<SimpleCountdown.<Start>d__11>(ref <Start>d__);
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x00044963 File Offset: 0x00042B63
	private void onTD(string s)
	{
		this.date = s;
		this.ParseDateTime();
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x00044974 File Offset: 0x00042B74
	private void onTDError(PlayFabError error)
	{
		Debug.Log(string.Concat(new string[]
		{
			"SimpleCountdown component on ",
			base.name,
			" failed to get '",
			this.titleDataKey,
			"' from title data. Using Fallback: '",
			this.date,
			"'"
		}));
		this.ParseDateTime();
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x000449D4 File Offset: 0x00042BD4
	private void ParseDateTime()
	{
		if (!DateTime.TryParse(this.date, out this.dt))
		{
			Debug.Log(string.Concat(new string[]
			{
				"SimpleCountdown component on ",
				base.name,
				" has an unparsable date string: '",
				this.date,
				"'"
			}));
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x00044A3C File Offset: 0x00042C3C
	protected override void ObservableSliceUpdate()
	{
		if (GorillaComputer.instance == null)
		{
			return;
		}
		DateTime dateTime = this.dt;
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		TimeSpan timeSpan;
		if (this.overrideDt < serverTime)
		{
			if (this.mode == SimpleCountdown.Mode.TimeSync)
			{
				this.dt = this.timeSyncRule.GetNext(serverTime);
			}
			timeSpan = this.dt - serverTime;
		}
		else
		{
			timeSpan = this.overrideDt - serverTime;
		}
		if (timeSpan.TotalHours <= (double)this.hourRange.x || timeSpan.TotalHours >= (double)this.hourRange.y)
		{
			timeSpan = timeSpan.Multiply(0.0);
		}
		switch (this.displayFormat)
		{
		case SimpleCountdown.DisplayFormat.DD_HH_MM_SS:
			this.tmp.text = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", new object[]
			{
				timeSpan.Days,
				timeSpan.Hours,
				timeSpan.Minutes,
				timeSpan.Seconds
			});
			return;
		case SimpleCountdown.DisplayFormat.HH_MM_SS:
			this.tmp.text = string.Format("{0:00}:{1:00}:{2:00}", Math.Floor(timeSpan.TotalHours), timeSpan.Minutes, timeSpan.Seconds);
			return;
		case SimpleCountdown.DisplayFormat.DD_HH_MM:
			this.tmp.text = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
			return;
		case SimpleCountdown.DisplayFormat.HH_MM:
			this.tmp.text = string.Format("{0:00}:{1:00}", Math.Floor(timeSpan.TotalHours), timeSpan.Minutes);
			return;
		case SimpleCountdown.DisplayFormat.MM_SS:
			this.tmp.text = string.Format("{0:00}:{1:00}", Math.Floor(timeSpan.TotalMinutes), timeSpan.Seconds);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnBecameObservable()
	{
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnLostObservable()
	{
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x00044C48 File Offset: 0x00042E48
	public void StartCountdown(int seconds)
	{
		this.overrideDt = GorillaComputer.instance.GetServerTime().AddSeconds((double)seconds);
	}

	// Token: 0x04000F3E RID: 3902
	[SerializeField]
	private SimpleCountdown.DisplayFormat displayFormat;

	// Token: 0x04000F3F RID: 3903
	[SerializeField]
	private SimpleCountdown.Mode mode = SimpleCountdown.Mode.TitleData;

	// Token: 0x04000F40 RID: 3904
	[SerializeField]
	private string titleDataKey;

	// Token: 0x04000F41 RID: 3905
	[SerializeField]
	private string date;

	// Token: 0x04000F42 RID: 3906
	[SerializeField]
	private ServerTimeSyncRule timeSyncRule;

	// Token: 0x04000F43 RID: 3907
	[SerializeField]
	private Vector2 hourRange = new Vector2(float.MinValue, float.MaxValue);

	// Token: 0x04000F44 RID: 3908
	private DateTime dt;

	// Token: 0x04000F45 RID: 3909
	private TextMeshPro tmp;

	// Token: 0x04000F46 RID: 3910
	private DateTime overrideDt = DateTime.MinValue;

	// Token: 0x020001D6 RID: 470
	private enum Mode
	{
		// Token: 0x04000F48 RID: 3912
		None,
		// Token: 0x04000F49 RID: 3913
		TitleData,
		// Token: 0x04000F4A RID: 3914
		FixedDate,
		// Token: 0x04000F4B RID: 3915
		TimeSync
	}

	// Token: 0x020001D7 RID: 471
	private enum DisplayFormat
	{
		// Token: 0x04000F4D RID: 3917
		DD_HH_MM_SS,
		// Token: 0x04000F4E RID: 3918
		HH_MM_SS,
		// Token: 0x04000F4F RID: 3919
		DD_HH_MM,
		// Token: 0x04000F50 RID: 3920
		HH_MM,
		// Token: 0x04000F51 RID: 3921
		MM_SS
	}
}
