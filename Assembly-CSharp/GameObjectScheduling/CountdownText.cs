using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace GameObjectScheduling
{
	// Token: 0x02001324 RID: 4900
	public class CountdownText : MonoBehaviour
	{
		// Token: 0x17000BB2 RID: 2994
		// (get) Token: 0x06007B6B RID: 31595 RVA: 0x00284851 File Offset: 0x00282A51
		private bool ShouldLocalize
		{
			get
			{
				return this.shouldLocalize && (this._locTextComp != null && this._countdownLocStr != null && this._timeCountdownVar != null && this._timescaleCountdownVar != null) && this._isValidVar != null;
			}
		}

		// Token: 0x17000BB3 RID: 2995
		// (get) Token: 0x06007B6C RID: 31596 RVA: 0x0028488E File Offset: 0x00282A8E
		// (set) Token: 0x06007B6D RID: 31597 RVA: 0x00284898 File Offset: 0x00282A98
		public CountdownTextDate Countdown
		{
			get
			{
				return this.CountdownTo;
			}
			set
			{
				this.CountdownTo = value;
				if (this.CountdownTo.FormatString.Length > 0)
				{
					this.displayTextFormat = this.CountdownTo.FormatString;
				}
				this.displayText.text = this.CountdownTo.DefaultString;
				if (base.gameObject.activeInHierarchy && !this.useExternalTime && this.monitor == null && this.CountdownTo != null)
				{
					this.monitor = base.StartCoroutine(this.MonitorTime());
				}
			}
		}

		// Token: 0x06007B6E RID: 31598 RVA: 0x00284924 File Offset: 0x00282B24
		private void Awake()
		{
			this.displayText = base.GetComponent<TMP_Text>();
			this.displayTextFormat = string.Empty;
			this.displayText.text = string.Empty;
			if (this.CountdownTo == null)
			{
				return;
			}
			if (this.displayTextFormat.Length == 0 && this.CountdownTo.FormatString.Length > 0)
			{
				this.displayTextFormat = this.CountdownTo.FormatString;
			}
			this.displayText.text = this.CountdownTo.DefaultString;
			if (!this.shouldLocalize)
			{
				return;
			}
			this._locTextComp = base.GetComponent<LocalizedText>();
			if (this._locTextComp == null)
			{
				Debug.LogError("[LOCALIZATION::COUNTDOWN_TEXT] There is no [LocalizedText] component on [" + base.name + "]!", this);
				return;
			}
			this._countdownLocStr = this._locTextComp.StringReference;
			if (this._locTextComp.StringReference == null || this._locTextComp.StringReference.IsEmpty)
			{
				Debug.LogError("[LOCALIZATION::COUNTDOWN_TEXT] There is no [StringReference] assigned on [" + base.name + "]!", this);
				return;
			}
			this._timeCountdownVar = (this._countdownLocStr["time-value"] as IntVariable);
			this._timescaleCountdownVar = (this._countdownLocStr["timescale-index"] as IntVariable);
			this._isValidVar = (this._countdownLocStr["is-valid"] as BoolVariable);
		}

		// Token: 0x06007B6F RID: 31599 RVA: 0x00284A8A File Offset: 0x00282C8A
		private void OnEnable()
		{
			if (this.CountdownTo == null)
			{
				return;
			}
			if (this.monitor == null && !this.useExternalTime)
			{
				this.monitor = base.StartCoroutine(this.MonitorTime());
			}
		}

		// Token: 0x06007B70 RID: 31600 RVA: 0x00284ABD File Offset: 0x00282CBD
		private void OnDisable()
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
		}

		// Token: 0x06007B71 RID: 31601 RVA: 0x00284ACB File Offset: 0x00282CCB
		private IEnumerator MonitorTime()
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			this.monitor = null;
			this.targetTime = this.TryParseDateTime();
			if (this.updateDisplay)
			{
				this.StartDisplayRefresh();
			}
			else
			{
				this.RefreshDisplay();
			}
			yield break;
		}

		// Token: 0x06007B72 RID: 31602 RVA: 0x00284ADA File Offset: 0x00282CDA
		private IEnumerator MonitorExternalTime(DateTime countdown)
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			this.monitor = null;
			this.targetTime = countdown;
			if (this.updateDisplay)
			{
				this.StartDisplayRefresh();
			}
			else
			{
				this.RefreshDisplay();
			}
			yield break;
		}

		// Token: 0x06007B73 RID: 31603 RVA: 0x00284AF0 File Offset: 0x00282CF0
		private void StopMonitorTime()
		{
			if (this.monitor != null)
			{
				base.StopCoroutine(this.monitor);
			}
			this.monitor = null;
		}

		// Token: 0x06007B74 RID: 31604 RVA: 0x00284B0D File Offset: 0x00282D0D
		public void SetCountdownTime(DateTime countdown)
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
			this.monitor = base.StartCoroutine(this.MonitorExternalTime(countdown));
		}

		// Token: 0x06007B75 RID: 31605 RVA: 0x00284B2E File Offset: 0x00282D2E
		public void SetFixedText(string text)
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
			this.displayText.text = text;
		}

		// Token: 0x06007B76 RID: 31606 RVA: 0x00284B48 File Offset: 0x00282D48
		private void StartDisplayRefresh()
		{
			this.StopDisplayRefresh();
			this.displayRefresh = base.StartCoroutine(this.WaitForDisplayRefresh());
		}

		// Token: 0x06007B77 RID: 31607 RVA: 0x00284B62 File Offset: 0x00282D62
		private void StopDisplayRefresh()
		{
			if (this.displayRefresh != null)
			{
				base.StopCoroutine(this.displayRefresh);
			}
			this.displayRefresh = null;
		}

		// Token: 0x06007B78 RID: 31608 RVA: 0x00284B7F File Offset: 0x00282D7F
		private IEnumerator WaitForDisplayRefresh()
		{
			for (;;)
			{
				this.RefreshDisplay();
				TimeSpan timeSpan;
				if (this.countdownTime.Days > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromDays((double)this.countdownTime.Days);
				}
				else if (this.countdownTime.Hours > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromHours((double)this.countdownTime.Hours);
				}
				else if (this.countdownTime.Minutes > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromMinutes((double)this.countdownTime.Minutes);
				}
				else
				{
					if (this.countdownTime.Seconds <= 0)
					{
						break;
					}
					timeSpan = this.countdownTime - TimeSpan.FromSeconds((double)this.countdownTime.Seconds);
				}
				yield return new WaitForSeconds((float)timeSpan.TotalSeconds);
			}
			yield break;
		}

		// Token: 0x06007B79 RID: 31609 RVA: 0x00284B90 File Offset: 0x00282D90
		private void RefreshDisplay()
		{
			this.countdownTime = this.targetTime.Subtract(GorillaComputer.instance.GetServerTime());
			ValueTuple<string, int, int, bool> timeDisplay = CountdownText.GetTimeDisplay(this.countdownTime, this.displayTextFormat, this.CountdownTo.DaysThreshold, string.Empty, this.CountdownTo.DefaultString);
			string item = timeDisplay.Item1;
			int item2 = timeDisplay.Item2;
			int item3 = timeDisplay.Item3;
			bool item4 = timeDisplay.Item4;
			if (!this.ShouldLocalize)
			{
				this.displayText.text = item;
				return;
			}
			this._timescaleCountdownVar.Value = item2;
			this._timeCountdownVar.Value = item3;
			this._isValidVar.Value = item4;
		}

		// Token: 0x06007B7A RID: 31610 RVA: 0x00284C3A File Offset: 0x00282E3A
		public static string GetTimeDisplay(TimeSpan ts, string format)
		{
			return CountdownText.GetTimeDisplay(ts, format, int.MaxValue, string.Empty, string.Empty).Item1;
		}

		// Token: 0x06007B7B RID: 31611 RVA: 0x00284C58 File Offset: 0x00282E58
		[return: TupleElementNames(new string[]
		{
			"msg",
			"timescaleVar",
			"countdownVar",
			"valid"
		})]
		public static ValueTuple<string, int, int, bool> GetTimeDisplay(TimeSpan ts, string format, int maxDaysToDisplay, string elapsedString, string overMaxString)
		{
			string item = overMaxString;
			int item2 = 0;
			int item3 = ts.Days;
			bool item4 = false;
			if (ts.TotalSeconds < 0.0)
			{
				return new ValueTuple<string, int, int, bool>(elapsedString, item2, item3, item4);
			}
			if (ts.TotalDays < (double)maxDaysToDisplay)
			{
				if (ts.Days > 0)
				{
					item2 = 3;
					item3 = ts.Days;
					item4 = true;
					item = string.Format(format, ts.Days, CountdownText.getTimeChunkString(CountdownText.TimeChunk.DAY, ts.Days));
				}
				else if (ts.Hours > 0)
				{
					item2 = 2;
					item3 = ts.Hours;
					item4 = true;
					item = string.Format(format, ts.Hours, CountdownText.getTimeChunkString(CountdownText.TimeChunk.HOUR, ts.Hours));
				}
				else if (ts.Minutes > 0)
				{
					item2 = 1;
					item3 = ts.Minutes;
					item4 = true;
					item = string.Format(format, ts.Minutes, CountdownText.getTimeChunkString(CountdownText.TimeChunk.MINUTE, ts.Minutes));
				}
				else if (ts.Seconds > 0)
				{
					item2 = 0;
					item3 = ts.Seconds;
					item4 = true;
					item = string.Format(format, ts.Seconds, CountdownText.getTimeChunkString(CountdownText.TimeChunk.SECOND, ts.Seconds));
				}
			}
			return new ValueTuple<string, int, int, bool>(item, item2, item3, item4);
		}

		// Token: 0x06007B7C RID: 31612 RVA: 0x00284D8C File Offset: 0x00282F8C
		private static string getTimeChunkString(CountdownText.TimeChunk chunk, int n)
		{
			switch (chunk)
			{
			case CountdownText.TimeChunk.DAY:
				if (n == 1)
				{
					return "DAY";
				}
				return "DAYS";
			case CountdownText.TimeChunk.HOUR:
				if (n == 1)
				{
					return "HOUR";
				}
				return "HOURS";
			case CountdownText.TimeChunk.MINUTE:
				if (n == 1)
				{
					return "MINUTE";
				}
				return "MINUTES";
			case CountdownText.TimeChunk.SECOND:
				if (n == 1)
				{
					return "SECOND";
				}
				return "SECONDS";
			default:
				return string.Empty;
			}
		}

		// Token: 0x06007B7D RID: 31613 RVA: 0x00284DF8 File Offset: 0x00282FF8
		private DateTime TryParseDateTime()
		{
			DateTime result;
			try
			{
				result = DateTime.Parse(this.CountdownTo.CountdownTo, CultureInfo.InvariantCulture);
			}
			catch
			{
				result = DateTime.MinValue;
			}
			return result;
		}

		// Token: 0x04008CC4 RID: 36036
		[SerializeField]
		private CountdownTextDate CountdownTo;

		// Token: 0x04008CC5 RID: 36037
		[SerializeField]
		private bool updateDisplay;

		// Token: 0x04008CC6 RID: 36038
		[SerializeField]
		private bool useExternalTime;

		// Token: 0x04008CC7 RID: 36039
		[SerializeField]
		private bool shouldLocalize = true;

		// Token: 0x04008CC8 RID: 36040
		private TMP_Text displayText;

		// Token: 0x04008CC9 RID: 36041
		private string displayTextFormat;

		// Token: 0x04008CCA RID: 36042
		private DateTime targetTime;

		// Token: 0x04008CCB RID: 36043
		private TimeSpan countdownTime;

		// Token: 0x04008CCC RID: 36044
		private Coroutine monitor;

		// Token: 0x04008CCD RID: 36045
		private Coroutine displayRefresh;

		// Token: 0x04008CCE RID: 36046
		private LocalizedText _locTextComp;

		// Token: 0x04008CCF RID: 36047
		private LocalizedString _countdownLocStr;

		// Token: 0x04008CD0 RID: 36048
		private IntVariable _timeCountdownVar;

		// Token: 0x04008CD1 RID: 36049
		private IntVariable _timescaleCountdownVar;

		// Token: 0x04008CD2 RID: 36050
		private BoolVariable _isValidVar;

		// Token: 0x02001325 RID: 4901
		private enum TimeChunk
		{
			// Token: 0x04008CD4 RID: 36052
			DAY,
			// Token: 0x04008CD5 RID: 36053
			HOUR,
			// Token: 0x04008CD6 RID: 36054
			MINUTE,
			// Token: 0x04008CD7 RID: 36055
			SECOND
		}
	}
}
