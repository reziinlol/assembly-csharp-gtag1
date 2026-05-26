using System;
using System.Collections.Generic;

// Token: 0x02000AC7 RID: 2759
public static class EnumUtil
{
	// Token: 0x0600467A RID: 18042 RVA: 0x0017DF20 File Offset: 0x0017C120
	public static string[] GetNames<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<string>(EnumData<TEnum>.Shared.Names);
	}

	// Token: 0x0600467B RID: 18043 RVA: 0x0017DF31 File Offset: 0x0017C131
	public static TEnum[] GetValues<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<TEnum>(EnumData<TEnum>.Shared.Values);
	}

	// Token: 0x0600467C RID: 18044 RVA: 0x0017DF42 File Offset: 0x0017C142
	public static long[] GetLongValues<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<long>(EnumData<TEnum>.Shared.LongValues);
	}

	// Token: 0x0600467D RID: 18045 RVA: 0x0017DF53 File Offset: 0x0017C153
	public static string EnumToName<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[e];
	}

	// Token: 0x0600467E RID: 18046 RVA: 0x0017DF65 File Offset: 0x0017C165
	public static TEnum NameToEnum<TEnum>(string n) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.NameToEnum[n];
	}

	// Token: 0x0600467F RID: 18047 RVA: 0x0017DF77 File Offset: 0x0017C177
	public static int EnumToIndex<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[e];
	}

	// Token: 0x06004680 RID: 18048 RVA: 0x0017DF89 File Offset: 0x0017C189
	public static TEnum IndexToEnum<TEnum>(int i) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.IndexToEnum[i];
	}

	// Token: 0x06004681 RID: 18049 RVA: 0x0017DF9B File Offset: 0x0017C19B
	public static long EnumToLong<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[e];
	}

	// Token: 0x06004682 RID: 18050 RVA: 0x0017DFAD File Offset: 0x0017C1AD
	public static TEnum LongToEnum<TEnum>(long l) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.LongToEnum[l];
	}

	// Token: 0x06004683 RID: 18051 RVA: 0x0017DFBF File Offset: 0x0017C1BF
	public static TEnum GetValue<TEnum>(int index) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.Values[index];
	}

	// Token: 0x06004684 RID: 18052 RVA: 0x0017DF77 File Offset: 0x0017C177
	public static int GetIndex<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[value];
	}

	// Token: 0x06004685 RID: 18053 RVA: 0x0017DF53 File Offset: 0x0017C153
	public static string GetName<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[value];
	}

	// Token: 0x06004686 RID: 18054 RVA: 0x0017DF65 File Offset: 0x0017C165
	public static TEnum GetValue<TEnum>(string name) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.NameToEnum[name];
	}

	// Token: 0x06004687 RID: 18055 RVA: 0x0017DF9B File Offset: 0x0017C19B
	public static long GetLongValue<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[value];
	}

	// Token: 0x06004688 RID: 18056 RVA: 0x0017DFAD File Offset: 0x0017C1AD
	public static TEnum GetValue<TEnum>(long longValue) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.LongToEnum[longValue];
	}

	// Token: 0x06004689 RID: 18057 RVA: 0x0017DFD1 File Offset: 0x0017C1D1
	public static TEnum[] SplitBitmask<TEnum>(TEnum bitmask) where TEnum : struct, Enum
	{
		return EnumUtil.SplitBitmask<TEnum>(Convert.ToInt64(bitmask));
	}

	// Token: 0x0600468A RID: 18058 RVA: 0x0017DFE4 File Offset: 0x0017C1E4
	public static TEnum[] SplitBitmask<TEnum>(long bitmaskLong) where TEnum : struct, Enum
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		if (!shared.IsBitMaskCompatible)
		{
			throw new ArgumentException("The enum type " + typeof(TEnum).Name + " is not bitmask-compatible.");
		}
		if (bitmaskLong == 0L)
		{
			return new TEnum[]
			{
				(TEnum)((object)Enum.ToObject(typeof(TEnum), 0L))
			};
		}
		List<TEnum> list = new List<TEnum>(shared.Values.Length);
		for (int i = 0; i < shared.Values.Length; i++)
		{
			TEnum item = shared.Values[i];
			long num = shared.LongValues[i];
			if (num != 0L && (bitmaskLong & num) == num)
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}
}
