using System;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000E97 RID: 3735
	[JsonObject(MemberSerialization.OptIn)]
	[Serializable]
	public class UDateTime : ISerializationCallbackReceiver, IComparable<UDateTime>, IComparable<DateTime>
	{
		// Token: 0x170008CA RID: 2250
		// (get) Token: 0x06005BA2 RID: 23458 RVA: 0x001D301B File Offset: 0x001D121B
		// (set) Token: 0x06005BA3 RID: 23459 RVA: 0x001D3023 File Offset: 0x001D1223
		[JsonProperty("DateTime")]
		public DateTime DateTime { get; set; }

		// Token: 0x06005BA4 RID: 23460 RVA: 0x001D302C File Offset: 0x001D122C
		[JsonConstructor]
		public UDateTime()
		{
			this.DateTime = DateTime.UnixEpoch;
		}

		// Token: 0x06005BA5 RID: 23461 RVA: 0x001D303F File Offset: 0x001D123F
		public UDateTime(DateTime dateTime)
		{
			this.DateTime = dateTime;
		}

		// Token: 0x06005BA6 RID: 23462 RVA: 0x001D304E File Offset: 0x001D124E
		public static implicit operator DateTime(UDateTime udt)
		{
			return udt.DateTime;
		}

		// Token: 0x06005BA7 RID: 23463 RVA: 0x001D3056 File Offset: 0x001D1256
		public static implicit operator UDateTime(DateTime dt)
		{
			return new UDateTime
			{
				DateTime = dt
			};
		}

		// Token: 0x06005BA8 RID: 23464 RVA: 0x001D3064 File Offset: 0x001D1264
		public int CompareTo(DateTime other)
		{
			return this.DateTime.CompareTo(other);
		}

		// Token: 0x06005BA9 RID: 23465 RVA: 0x001D3080 File Offset: 0x001D1280
		public int CompareTo(UDateTime other)
		{
			if (this == other)
			{
				return 0;
			}
			if (other == null)
			{
				return 1;
			}
			return this.DateTime.CompareTo(other.DateTime);
		}

		// Token: 0x06005BAA RID: 23466 RVA: 0x001D30AC File Offset: 0x001D12AC
		protected bool Equals(UDateTime other)
		{
			return this.DateTime.Equals(other.DateTime);
		}

		// Token: 0x06005BAB RID: 23467 RVA: 0x001D30CD File Offset: 0x001D12CD
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((UDateTime)obj)));
		}

		// Token: 0x06005BAC RID: 23468 RVA: 0x001D30FC File Offset: 0x001D12FC
		public override int GetHashCode()
		{
			return this.DateTime.GetHashCode();
		}

		// Token: 0x06005BAD RID: 23469 RVA: 0x001D3118 File Offset: 0x001D1318
		public override string ToString()
		{
			return this.DateTime.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x06005BAE RID: 23470 RVA: 0x001D3138 File Offset: 0x001D1338
		public void OnAfterDeserialize()
		{
			DateTime dateTime;
			this.DateTime = (DateTime.TryParse(this._DateTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime) ? dateTime : DateTime.MinValue);
		}

		// Token: 0x06005BAF RID: 23471 RVA: 0x001D316C File Offset: 0x001D136C
		public void OnBeforeSerialize()
		{
			this._DateTime = this.DateTime.ToString("o", CultureInfo.InvariantCulture);
		}

		// Token: 0x06005BB0 RID: 23472 RVA: 0x001D3197 File Offset: 0x001D1397
		[OnSerializing]
		internal void OnSerializing(StreamingContext context)
		{
			this.OnBeforeSerialize();
		}

		// Token: 0x06005BB1 RID: 23473 RVA: 0x001D319F File Offset: 0x001D139F
		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			this.OnAfterDeserialize();
		}

		// Token: 0x04006A7B RID: 27259
		[HideInInspector]
		[SerializeField]
		private string _DateTime;
	}
}
