using System;
using System.Collections.Generic;

// Token: 0x02000AC6 RID: 2758
public class EnumData<TEnum> where TEnum : struct, Enum
{
	// Token: 0x17000691 RID: 1681
	// (get) Token: 0x06004677 RID: 18039 RVA: 0x0017DCAD File Offset: 0x0017BEAD
	public static EnumData<TEnum> Shared { get; } = new EnumData<TEnum>();

	// Token: 0x06004678 RID: 18040 RVA: 0x0017DCB4 File Offset: 0x0017BEB4
	private EnumData()
	{
		this.Names = Enum.GetNames(typeof(TEnum));
		this.Values = (TEnum[])Enum.GetValues(typeof(TEnum));
		int num = this.Names.Length;
		this.LongValues = new long[num];
		this.EnumToName = new Dictionary<TEnum, string>(num);
		this.NameToEnum = new Dictionary<string, TEnum>(num * 2);
		this.EnumToIndex = new Dictionary<TEnum, int>(num);
		this.IndexToEnum = new Dictionary<int, TEnum>(num);
		this.EnumToLong = new Dictionary<TEnum, long>(num);
		this.LongToEnum = new Dictionary<long, TEnum>(num);
		long num2 = long.MaxValue;
		long num3 = long.MinValue;
		for (int i = 0; i < this.Names.Length; i++)
		{
			string text = this.Names[i];
			TEnum tenum = this.Values[i];
			long num4 = Convert.ToInt64(tenum);
			this.LongValues[i] = num4;
			this.EnumToName[tenum] = text;
			this.NameToEnum[text] = tenum;
			this.NameToEnum.TryAdd(text.ToLowerInvariant(), tenum);
			this.EnumToIndex[tenum] = i;
			this.IndexToEnum[i] = tenum;
			this.EnumToLong[tenum] = num4;
			this.LongToEnum[num4] = tenum;
			num2 = Math.Min(num4, num2);
			num3 = Math.Max(num4, num3);
		}
		for (int j = 0; j < this.Names.Length; j++)
		{
			string key = this.Names[j];
			TEnum value = this.Values[j];
			this.NameToEnum[key] = value;
		}
		this.MinValue = this.LongToEnum[num2];
		this.MaxValue = this.LongToEnum[num3];
		this.MinInt = Convert.ToInt32(num2);
		this.MaxInt = Convert.ToInt32(num3);
		this.MinLong = num2;
		this.MaxLong = num3;
		long num5 = 0L;
		bool isBitMaskCompatible = true;
		foreach (long num6 in this.LongValues)
		{
			if (num6 != 0L && (num6 & num6 - 1L) != 0L && (num5 & num6) != num6)
			{
				isBitMaskCompatible = false;
				break;
			}
			num5 |= num6;
		}
		this.IsBitMaskCompatible = isBitMaskCompatible;
	}

	// Token: 0x040058E3 RID: 22755
	public readonly string[] Names;

	// Token: 0x040058E4 RID: 22756
	public readonly TEnum[] Values;

	// Token: 0x040058E5 RID: 22757
	public readonly long[] LongValues;

	// Token: 0x040058E6 RID: 22758
	public readonly bool IsBitMaskCompatible;

	// Token: 0x040058E7 RID: 22759
	public readonly Dictionary<TEnum, string> EnumToName;

	// Token: 0x040058E8 RID: 22760
	public readonly Dictionary<string, TEnum> NameToEnum;

	// Token: 0x040058E9 RID: 22761
	public readonly Dictionary<TEnum, int> EnumToIndex;

	// Token: 0x040058EA RID: 22762
	public readonly Dictionary<int, TEnum> IndexToEnum;

	// Token: 0x040058EB RID: 22763
	public readonly Dictionary<TEnum, long> EnumToLong;

	// Token: 0x040058EC RID: 22764
	public readonly Dictionary<long, TEnum> LongToEnum;

	// Token: 0x040058ED RID: 22765
	public readonly TEnum MinValue;

	// Token: 0x040058EE RID: 22766
	public readonly TEnum MaxValue;

	// Token: 0x040058EF RID: 22767
	public readonly int MinInt;

	// Token: 0x040058F0 RID: 22768
	public readonly int MaxInt;

	// Token: 0x040058F1 RID: 22769
	public readonly long MinLong;

	// Token: 0x040058F2 RID: 22770
	public readonly long MaxLong;
}
