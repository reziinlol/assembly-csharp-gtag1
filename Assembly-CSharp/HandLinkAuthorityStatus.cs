using System;

// Token: 0x020009E0 RID: 2528
public struct HandLinkAuthorityStatus
{
	// Token: 0x060040A9 RID: 16553 RVA: 0x00159B2B File Offset: 0x00157D2B
	public HandLinkAuthorityStatus(HandLinkAuthorityType authority)
	{
		this.type = authority;
		this.timestamp = -1f;
		this.tiebreak = -1;
	}

	// Token: 0x060040AA RID: 16554 RVA: 0x00159B46 File Offset: 0x00157D46
	public HandLinkAuthorityStatus(HandLinkAuthorityType authority, float timestamp, int tiebreak)
	{
		this.type = authority;
		this.timestamp = timestamp;
		this.tiebreak = tiebreak;
	}

	// Token: 0x060040AB RID: 16555 RVA: 0x00159B60 File Offset: 0x00157D60
	public static bool operator >(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type > b.type || (b.type <= a.type && (a.timestamp > b.timestamp || (b.timestamp <= a.timestamp && a.tiebreak > b.tiebreak)));
	}

	// Token: 0x060040AC RID: 16556 RVA: 0x00159BBC File Offset: 0x00157DBC
	public static bool operator <(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type < b.type || (b.type >= a.type && (a.timestamp < b.timestamp || (b.timestamp >= a.timestamp && a.tiebreak < b.tiebreak)));
	}

	// Token: 0x060040AD RID: 16557 RVA: 0x00159C18 File Offset: 0x00157E18
	public int CompareTo(HandLinkAuthorityStatus b)
	{
		int num = this.type.CompareTo(b.type);
		if (num != 0)
		{
			return num;
		}
		int num2 = this.timestamp.CompareTo(b.timestamp);
		if (num2 != 0)
		{
			return num2;
		}
		return this.tiebreak.CompareTo(b.tiebreak);
	}

	// Token: 0x060040AE RID: 16558 RVA: 0x00159C6F File Offset: 0x00157E6F
	public static bool operator ==(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type == b.type && a.timestamp == b.timestamp && a.tiebreak == b.tiebreak;
	}

	// Token: 0x060040AF RID: 16559 RVA: 0x00159C9D File Offset: 0x00157E9D
	public static bool operator !=(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.timestamp != b.timestamp || a.tiebreak != b.tiebreak;
	}

	// Token: 0x060040B0 RID: 16560 RVA: 0x00159CC0 File Offset: 0x00157EC0
	public override string ToString()
	{
		return string.Format("{0}/{1}", this.timestamp.ToString("0.0000"), this.tiebreak);
	}

	// Token: 0x04005144 RID: 20804
	public HandLinkAuthorityType type;

	// Token: 0x04005145 RID: 20805
	public float timestamp;

	// Token: 0x04005146 RID: 20806
	public int tiebreak;
}
