using System;

// Token: 0x02000AC8 RID: 2760
public static class EnumUtilExt
{
	// Token: 0x0600468B RID: 18059 RVA: 0x0017DF53 File Offset: 0x0017C153
	public static string GetName<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[e];
	}

	// Token: 0x0600468C RID: 18060 RVA: 0x0017DF77 File Offset: 0x0017C177
	public static int GetIndex<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[e];
	}

	// Token: 0x0600468D RID: 18061 RVA: 0x0017DF9B File Offset: 0x0017C19B
	public static long GetLongValue<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[e];
	}

	// Token: 0x0600468E RID: 18062 RVA: 0x0017E09C File Offset: 0x0017C29C
	public static TEnum GetNextValue<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		return shared.Values[shared.EnumToIndex[e] + 1 % shared.Values.Length];
	}
}
