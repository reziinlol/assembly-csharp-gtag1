using System;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000E99 RID: 3737
	[JsonObject(MemberSerialization.OptIn)]
	[Serializable]
	public class UTimeSpanRange
	{
		// Token: 0x170008CC RID: 2252
		// (get) Token: 0x06005BC5 RID: 23493 RVA: 0x001D335B File Offset: 0x001D155B
		// (set) Token: 0x06005BC6 RID: 23494 RVA: 0x001D3368 File Offset: 0x001D1568
		public TimeSpan Start
		{
			get
			{
				return this._Start;
			}
			set
			{
				this._Start = value;
			}
		}

		// Token: 0x170008CD RID: 2253
		// (get) Token: 0x06005BC7 RID: 23495 RVA: 0x001D3376 File Offset: 0x001D1576
		// (set) Token: 0x06005BC8 RID: 23496 RVA: 0x001D3383 File Offset: 0x001D1583
		public TimeSpan End
		{
			get
			{
				return this._End;
			}
			set
			{
				this._End = value;
			}
		}

		// Token: 0x170008CE RID: 2254
		// (get) Token: 0x06005BC9 RID: 23497 RVA: 0x001D3391 File Offset: 0x001D1591
		public TimeSpan Duration
		{
			get
			{
				return this.End - this.Start;
			}
		}

		// Token: 0x06005BCA RID: 23498 RVA: 0x001D33A4 File Offset: 0x001D15A4
		public bool IsInRange(TimeSpan time)
		{
			return time >= this.Start && time <= this.End;
		}

		// Token: 0x06005BCB RID: 23499 RVA: 0x00002050 File Offset: 0x00000250
		[JsonConstructor]
		public UTimeSpanRange()
		{
		}

		// Token: 0x06005BCC RID: 23500 RVA: 0x001D33C2 File Offset: 0x001D15C2
		public UTimeSpanRange(TimeSpan start)
		{
			this._Start = start;
			this._End = start;
		}

		// Token: 0x06005BCD RID: 23501 RVA: 0x001D33E2 File Offset: 0x001D15E2
		public UTimeSpanRange(TimeSpan start, TimeSpan end)
		{
			this._Start = start;
			this._End = end;
		}

		// Token: 0x06005BCE RID: 23502 RVA: 0x001D3402 File Offset: 0x001D1602
		private void OnStartChanged()
		{
			if (this._Start.CompareTo(this._End) > 0)
			{
				this._End.TimeSpan = this._Start.TimeSpan;
			}
		}

		// Token: 0x06005BCF RID: 23503 RVA: 0x001D342E File Offset: 0x001D162E
		private void OnEndChanged()
		{
			if (this._End.CompareTo(this._Start) < 0)
			{
				this._Start.TimeSpan = this._End.TimeSpan;
			}
		}

		// Token: 0x04006A7E RID: 27262
		[JsonProperty("Start")]
		[SerializeField]
		private UTimeSpan _Start;

		// Token: 0x04006A7F RID: 27263
		[JsonProperty("End")]
		[SerializeField]
		private UTimeSpan _End;
	}
}
