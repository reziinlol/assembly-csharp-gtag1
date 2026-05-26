using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// Token: 0x02000AD5 RID: 2773
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct Id128 : IEquatable<Id128>, IComparable<Id128>, IEquatable<Guid>, IEquatable<Hash128>
{
	// Token: 0x060046AF RID: 18095 RVA: 0x0017E948 File Offset: 0x0017CB48
	public Id128(int a, int b, int c, int d)
	{
		this.guid = Guid.Empty;
		this.h128 = default(Hash128);
		this.x = (this.y = 0L);
		this.a = a;
		this.b = b;
		this.c = c;
		this.d = d;
	}

	// Token: 0x060046B0 RID: 18096 RVA: 0x0017E99C File Offset: 0x0017CB9C
	public Id128(long x, long y)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = default(Hash128);
		this.x = x;
		this.y = y;
	}

	// Token: 0x060046B1 RID: 18097 RVA: 0x0017E9F0 File Offset: 0x0017CBF0
	public Id128(Hash128 hash)
	{
		this.x = (this.y = 0L);
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = hash;
	}

	// Token: 0x060046B2 RID: 18098 RVA: 0x0017EA44 File Offset: 0x0017CC44
	public Id128(Guid guid)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = guid;
	}

	// Token: 0x060046B3 RID: 18099 RVA: 0x0017EA98 File Offset: 0x0017CC98
	public Id128(string guid)
	{
		if (string.IsNullOrWhiteSpace(guid))
		{
			throw new ArgumentNullException("guid");
		}
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = Guid.Parse(guid);
	}

	// Token: 0x060046B4 RID: 18100 RVA: 0x0017EB04 File Offset: 0x0017CD04
	public Id128(byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		if (bytes.Length != 16)
		{
			throw new ArgumentException("Input buffer must be exactly 16 bytes", "bytes");
		}
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = new Guid(bytes);
	}

	// Token: 0x060046B5 RID: 18101 RVA: 0x0017EB81 File Offset: 0x0017CD81
	[return: TupleElementNames(new string[]
	{
		"l1",
		"l2"
	})]
	public ValueTuple<long, long> ToLongs()
	{
		return new ValueTuple<long, long>(this.x, this.y);
	}

	// Token: 0x060046B6 RID: 18102 RVA: 0x0017EB94 File Offset: 0x0017CD94
	[return: TupleElementNames(new string[]
	{
		"i1",
		"i2",
		"i3",
		"i4"
	})]
	public ValueTuple<int, int, int, int> ToInts()
	{
		return new ValueTuple<int, int, int, int>(this.a, this.b, this.c, this.d);
	}

	// Token: 0x060046B7 RID: 18103 RVA: 0x0017EBB3 File Offset: 0x0017CDB3
	public byte[] ToByteArray()
	{
		return this.guid.ToByteArray();
	}

	// Token: 0x060046B8 RID: 18104 RVA: 0x0017EBC0 File Offset: 0x0017CDC0
	public bool Equals(Id128 id)
	{
		return this.x == id.x && this.y == id.y;
	}

	// Token: 0x060046B9 RID: 18105 RVA: 0x0017EBE0 File Offset: 0x0017CDE0
	public bool Equals(Guid g)
	{
		return this.guid == g;
	}

	// Token: 0x060046BA RID: 18106 RVA: 0x0017EBEE File Offset: 0x0017CDEE
	public bool Equals(Hash128 h)
	{
		return this.h128 == h;
	}

	// Token: 0x060046BB RID: 18107 RVA: 0x0017EBFC File Offset: 0x0017CDFC
	public override bool Equals(object obj)
	{
		if (obj is Id128)
		{
			Id128 id = (Id128)obj;
			return this.Equals(id);
		}
		if (obj is Guid)
		{
			Guid g = (Guid)obj;
			return this.Equals(g);
		}
		if (obj is Hash128)
		{
			Hash128 h = (Hash128)obj;
			return this.Equals(h);
		}
		return false;
	}

	// Token: 0x060046BC RID: 18108 RVA: 0x0017EC4F File Offset: 0x0017CE4F
	public override string ToString()
	{
		return this.guid.ToString();
	}

	// Token: 0x060046BD RID: 18109 RVA: 0x0017EC62 File Offset: 0x0017CE62
	public override int GetHashCode()
	{
		return StaticHash.Compute(this.a, this.b, this.c, this.d);
	}

	// Token: 0x060046BE RID: 18110 RVA: 0x0017EC84 File Offset: 0x0017CE84
	public int CompareTo(Id128 id)
	{
		int num = this.x.CompareTo(id.x);
		if (num == 0)
		{
			num = this.y.CompareTo(id.y);
		}
		return num;
	}

	// Token: 0x060046BF RID: 18111 RVA: 0x0017ECBC File Offset: 0x0017CEBC
	public int CompareTo(object obj)
	{
		if (obj is Id128)
		{
			Id128 id = (Id128)obj;
			return this.CompareTo(id);
		}
		if (obj is Guid)
		{
			Guid value = (Guid)obj;
			return this.guid.CompareTo(value);
		}
		if (obj is Hash128)
		{
			Hash128 rhs = (Hash128)obj;
			return this.h128.CompareTo(rhs);
		}
		throw new ArgumentException("Object must be of type Id128 or Guid");
	}

	// Token: 0x060046C0 RID: 18112 RVA: 0x0017ED22 File Offset: 0x0017CF22
	public static Id128 NewId()
	{
		return new Id128(Guid.NewGuid());
	}

	// Token: 0x060046C1 RID: 18113 RVA: 0x0017ED30 File Offset: 0x0017CF30
	public static Id128 ComputeMD5(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return Id128.Empty;
		}
		Id128 result;
		using (MD5 md = MD5.Create())
		{
			result = new Guid(md.ComputeHash(Encoding.UTF8.GetBytes(s)));
		}
		return result;
	}

	// Token: 0x060046C2 RID: 18114 RVA: 0x0017ED8C File Offset: 0x0017CF8C
	public static Id128 ComputeSHV2(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return Id128.Empty;
		}
		return Hash128.Compute(s);
	}

	// Token: 0x060046C3 RID: 18115 RVA: 0x0017EDA7 File Offset: 0x0017CFA7
	public static bool operator ==(Id128 j, Id128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x060046C4 RID: 18116 RVA: 0x0017EDB1 File Offset: 0x0017CFB1
	public static bool operator !=(Id128 j, Id128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x060046C5 RID: 18117 RVA: 0x0017EDBE File Offset: 0x0017CFBE
	public static bool operator ==(Id128 j, Guid k)
	{
		return j.Equals(k);
	}

	// Token: 0x060046C6 RID: 18118 RVA: 0x0017EDC8 File Offset: 0x0017CFC8
	public static bool operator !=(Id128 j, Guid k)
	{
		return !j.Equals(k);
	}

	// Token: 0x060046C7 RID: 18119 RVA: 0x0017EDD5 File Offset: 0x0017CFD5
	public static bool operator ==(Guid j, Id128 k)
	{
		return j.Equals(k.guid);
	}

	// Token: 0x060046C8 RID: 18120 RVA: 0x0017EDE4 File Offset: 0x0017CFE4
	public static bool operator !=(Guid j, Id128 k)
	{
		return !j.Equals(k.guid);
	}

	// Token: 0x060046C9 RID: 18121 RVA: 0x0017EDF6 File Offset: 0x0017CFF6
	public static bool operator ==(Id128 j, Hash128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x060046CA RID: 18122 RVA: 0x0017EE00 File Offset: 0x0017D000
	public static bool operator !=(Id128 j, Hash128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x060046CB RID: 18123 RVA: 0x0017EE0D File Offset: 0x0017D00D
	public static bool operator ==(Hash128 j, Id128 k)
	{
		return j.Equals(k.h128);
	}

	// Token: 0x060046CC RID: 18124 RVA: 0x0017EE1C File Offset: 0x0017D01C
	public static bool operator !=(Hash128 j, Id128 k)
	{
		return !j.Equals(k.h128);
	}

	// Token: 0x060046CD RID: 18125 RVA: 0x0017EE2E File Offset: 0x0017D02E
	public static bool operator <(Id128 j, Id128 k)
	{
		return j.CompareTo(k) < 0;
	}

	// Token: 0x060046CE RID: 18126 RVA: 0x0017EE3B File Offset: 0x0017D03B
	public static bool operator >(Id128 j, Id128 k)
	{
		return j.CompareTo(k) > 0;
	}

	// Token: 0x060046CF RID: 18127 RVA: 0x0017EE48 File Offset: 0x0017D048
	public static bool operator <=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) <= 0;
	}

	// Token: 0x060046D0 RID: 18128 RVA: 0x0017EE58 File Offset: 0x0017D058
	public static bool operator >=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) >= 0;
	}

	// Token: 0x060046D1 RID: 18129 RVA: 0x0017EE68 File Offset: 0x0017D068
	public static implicit operator Guid(Id128 id)
	{
		return id.guid;
	}

	// Token: 0x060046D2 RID: 18130 RVA: 0x0017EE70 File Offset: 0x0017D070
	public static implicit operator Id128(Guid guid)
	{
		return new Id128(guid);
	}

	// Token: 0x060046D3 RID: 18131 RVA: 0x0017EE78 File Offset: 0x0017D078
	public static implicit operator Id128(Hash128 h)
	{
		return new Id128(h);
	}

	// Token: 0x060046D4 RID: 18132 RVA: 0x0017EE80 File Offset: 0x0017D080
	public static implicit operator Hash128(Id128 id)
	{
		return id.h128;
	}

	// Token: 0x060046D5 RID: 18133 RVA: 0x0017EE88 File Offset: 0x0017D088
	public static explicit operator Id128(string s)
	{
		return Id128.ComputeMD5(s);
	}

	// Token: 0x0400592F RID: 22831
	[SerializeField]
	[FieldOffset(0)]
	public long x;

	// Token: 0x04005930 RID: 22832
	[SerializeField]
	[FieldOffset(8)]
	public long y;

	// Token: 0x04005931 RID: 22833
	[NonSerialized]
	[FieldOffset(0)]
	public int a;

	// Token: 0x04005932 RID: 22834
	[NonSerialized]
	[FieldOffset(4)]
	public int b;

	// Token: 0x04005933 RID: 22835
	[NonSerialized]
	[FieldOffset(8)]
	public int c;

	// Token: 0x04005934 RID: 22836
	[NonSerialized]
	[FieldOffset(12)]
	public int d;

	// Token: 0x04005935 RID: 22837
	[NonSerialized]
	[FieldOffset(0)]
	public Guid guid;

	// Token: 0x04005936 RID: 22838
	[NonSerialized]
	[FieldOffset(0)]
	public Hash128 h128;

	// Token: 0x04005937 RID: 22839
	public static readonly Id128 Empty;
}
