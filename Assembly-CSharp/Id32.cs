using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000AD7 RID: 2775
[Serializable]
public struct Id32
{
	// Token: 0x060046D8 RID: 18136 RVA: 0x0017EE90 File Offset: 0x0017D090
	public Id32(string idString)
	{
		if (idString == null)
		{
			throw new ArgumentNullException("idString");
		}
		if (string.IsNullOrWhiteSpace(idString.Trim()))
		{
			throw new ArgumentNullException("idString");
		}
		this._id = XXHash32.Compute(idString, 0U);
	}

	// Token: 0x060046D9 RID: 18137 RVA: 0x0017EEC5 File Offset: 0x0017D0C5
	public unsafe static implicit operator int(Id32 i32)
	{
		return *Unsafe.As<Id32, int>(ref i32);
	}

	// Token: 0x060046DA RID: 18138 RVA: 0x0017EECF File Offset: 0x0017D0CF
	public static implicit operator Id32(string s)
	{
		return Id32.ComputeID(s);
	}

	// Token: 0x060046DB RID: 18139 RVA: 0x0017EED8 File Offset: 0x0017D0D8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Id32 ComputeID(string s)
	{
		int num = Id32.ComputeHash(s);
		return *Unsafe.As<int, Id32>(ref num);
	}

	// Token: 0x060046DC RID: 18140 RVA: 0x0017EEF8 File Offset: 0x0017D0F8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ComputeHash(string s)
	{
		if (s == null)
		{
			return 0;
		}
		s = s.Trim();
		if (string.IsNullOrWhiteSpace(s))
		{
			return 0;
		}
		return XXHash32.Compute(s, 0U);
	}

	// Token: 0x060046DD RID: 18141 RVA: 0x0017EF18 File Offset: 0x0017D118
	public override int GetHashCode()
	{
		return this._id;
	}

	// Token: 0x060046DE RID: 18142 RVA: 0x0017EF20 File Offset: 0x0017D120
	public override string ToString()
	{
		return string.Format("{{ {0} : {1} }}", "Id32", this._id);
	}

	// Token: 0x04005938 RID: 22840
	[SerializeField]
	private int _id;
}
