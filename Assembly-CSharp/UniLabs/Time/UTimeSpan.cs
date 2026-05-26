using System;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000E98 RID: 3736
	[JsonObject(MemberSerialization.OptIn)]
	[Serializable]
	public class UTimeSpan : ISerializationCallbackReceiver, IComparable<UTimeSpan>, IComparable<TimeSpan>
	{
		// Token: 0x170008CB RID: 2251
		// (get) Token: 0x06005BB2 RID: 23474 RVA: 0x001D31A7 File Offset: 0x001D13A7
		// (set) Token: 0x06005BB3 RID: 23475 RVA: 0x001D31AF File Offset: 0x001D13AF
		[JsonProperty("TimeSpan")]
		public TimeSpan TimeSpan { get; set; }

		// Token: 0x06005BB4 RID: 23476 RVA: 0x001D31B8 File Offset: 0x001D13B8
		[JsonConstructor]
		public UTimeSpan()
		{
			this.TimeSpan = TimeSpan.Zero;
		}

		// Token: 0x06005BB5 RID: 23477 RVA: 0x001D31CB File Offset: 0x001D13CB
		public UTimeSpan(TimeSpan timeSpan)
		{
			this.TimeSpan = timeSpan;
		}

		// Token: 0x06005BB6 RID: 23478 RVA: 0x001D31DA File Offset: 0x001D13DA
		public UTimeSpan(long ticks) : this(new TimeSpan(ticks))
		{
		}

		// Token: 0x06005BB7 RID: 23479 RVA: 0x001D31E8 File Offset: 0x001D13E8
		public UTimeSpan(int hours, int minutes, int seconds) : this(new TimeSpan(hours, minutes, seconds))
		{
		}

		// Token: 0x06005BB8 RID: 23480 RVA: 0x001D31F8 File Offset: 0x001D13F8
		public UTimeSpan(int days, int hours, int minutes, int seconds) : this(new TimeSpan(days, hours, minutes, seconds))
		{
		}

		// Token: 0x06005BB9 RID: 23481 RVA: 0x001D320A File Offset: 0x001D140A
		public UTimeSpan(int days, int hours, int minutes, int seconds, int milliseconds) : this(new TimeSpan(days, hours, minutes, seconds, milliseconds))
		{
		}

		// Token: 0x06005BBA RID: 23482 RVA: 0x001D321E File Offset: 0x001D141E
		public static implicit operator TimeSpan(UTimeSpan uTimeSpan)
		{
			if (uTimeSpan == null)
			{
				return TimeSpan.Zero;
			}
			return uTimeSpan.TimeSpan;
		}

		// Token: 0x06005BBB RID: 23483 RVA: 0x001D322F File Offset: 0x001D142F
		public static implicit operator UTimeSpan(TimeSpan timeSpan)
		{
			return new UTimeSpan(timeSpan);
		}

		// Token: 0x06005BBC RID: 23484 RVA: 0x001D3238 File Offset: 0x001D1438
		public int CompareTo(TimeSpan other)
		{
			return this.TimeSpan.CompareTo(other);
		}

		// Token: 0x06005BBD RID: 23485 RVA: 0x001D3254 File Offset: 0x001D1454
		public int CompareTo(UTimeSpan other)
		{
			if (this == other)
			{
				return 0;
			}
			if (other == null)
			{
				return 1;
			}
			return this.TimeSpan.CompareTo(other.TimeSpan);
		}

		// Token: 0x06005BBE RID: 23486 RVA: 0x001D3280 File Offset: 0x001D1480
		protected bool Equals(UTimeSpan other)
		{
			return this.TimeSpan.Equals(other.TimeSpan);
		}

		// Token: 0x06005BBF RID: 23487 RVA: 0x001D32A1 File Offset: 0x001D14A1
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((UTimeSpan)obj)));
		}

		// Token: 0x06005BC0 RID: 23488 RVA: 0x001D32D0 File Offset: 0x001D14D0
		public override int GetHashCode()
		{
			return this.TimeSpan.GetHashCode();
		}

		// Token: 0x06005BC1 RID: 23489 RVA: 0x001D32F4 File Offset: 0x001D14F4
		public void OnAfterDeserialize()
		{
			TimeSpan timeSpan;
			this.TimeSpan = (TimeSpan.TryParse(this._TimeSpan, CultureInfo.InvariantCulture, out timeSpan) ? timeSpan : TimeSpan.Zero);
		}

		// Token: 0x06005BC2 RID: 23490 RVA: 0x001D3324 File Offset: 0x001D1524
		public void OnBeforeSerialize()
		{
			this._TimeSpan = this.TimeSpan.ToString();
		}

		// Token: 0x06005BC3 RID: 23491 RVA: 0x001D334B File Offset: 0x001D154B
		[OnSerializing]
		internal void OnSerializingMethod(StreamingContext context)
		{
			this.OnBeforeSerialize();
		}

		// Token: 0x06005BC4 RID: 23492 RVA: 0x001D3353 File Offset: 0x001D1553
		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			this.OnAfterDeserialize();
		}

		// Token: 0x04006A7D RID: 27261
		[HideInInspector]
		[SerializeField]
		private string _TimeSpan;
	}
}
