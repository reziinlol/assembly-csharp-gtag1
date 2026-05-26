using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Text;
using UnityEngine;

// Token: 0x02000D8E RID: 3470
public static class StringUtils
{
	// Token: 0x0600552E RID: 21806 RVA: 0x001BCEF9 File Offset: 0x001BB0F9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrEmpty(this string s)
	{
		return string.IsNullOrEmpty(s);
	}

	// Token: 0x0600552F RID: 21807 RVA: 0x001BCF01 File Offset: 0x001BB101
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrWhiteSpace(this string s)
	{
		return string.IsNullOrWhiteSpace(s);
	}

	// Token: 0x06005530 RID: 21808 RVA: 0x001BCF0C File Offset: 0x001BB10C
	public static string ToAlphaNumeric(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return string.Empty;
		}
		string result;
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
		{
			foreach (char c in s)
			{
				if (char.IsLetterOrDigit(c))
				{
					utf16ValueStringBuilder.Append(c);
				}
			}
			result = utf16ValueStringBuilder.ToString();
		}
		return result;
	}

	// Token: 0x06005531 RID: 21809 RVA: 0x001BCF88 File Offset: 0x001BB188
	public static string Capitalize(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		char[] array = s.ToCharArray();
		array[0] = char.ToUpperInvariant(array[0]);
		return new string(array);
	}

	// Token: 0x06005532 RID: 21810 RVA: 0x001BCFB7 File Offset: 0x001BB1B7
	public static string Concat(this IEnumerable<string> source)
	{
		return string.Concat(source);
	}

	// Token: 0x06005533 RID: 21811 RVA: 0x001BCFBF File Offset: 0x001BB1BF
	public static string Join(this IEnumerable<string> source, string separator)
	{
		return string.Join(separator, source);
	}

	// Token: 0x06005534 RID: 21812 RVA: 0x001BCFC8 File Offset: 0x001BB1C8
	public static string Join(this IEnumerable<string> source, char separator)
	{
		return string.Join<string>(separator, source);
	}

	// Token: 0x06005535 RID: 21813 RVA: 0x001BCFD1 File Offset: 0x001BB1D1
	public static string RemoveAll(this string s, string value, StringComparison mode = StringComparison.OrdinalIgnoreCase)
	{
		if (string.IsNullOrEmpty(s))
		{
			return s;
		}
		return s.Replace(value, string.Empty, mode);
	}

	// Token: 0x06005536 RID: 21814 RVA: 0x001BCFEA File Offset: 0x001BB1EA
	public static string RemoveAll(this string s, char value, StringComparison mode = StringComparison.OrdinalIgnoreCase)
	{
		return s.RemoveAll(value.ToString(), mode);
	}

	// Token: 0x06005537 RID: 21815 RVA: 0x001BCFFA File Offset: 0x001BB1FA
	public static byte[] ToBytesASCII(this string s)
	{
		return Encoding.ASCII.GetBytes(s);
	}

	// Token: 0x06005538 RID: 21816 RVA: 0x001BD007 File Offset: 0x001BB207
	public static byte[] ToBytesUTF8(this string s)
	{
		return Encoding.UTF8.GetBytes(s);
	}

	// Token: 0x06005539 RID: 21817 RVA: 0x001BD014 File Offset: 0x001BB214
	public static byte[] ToBytesUnicode(this string s)
	{
		return Encoding.Unicode.GetBytes(s);
	}

	// Token: 0x0600553A RID: 21818 RVA: 0x001BD024 File Offset: 0x001BB224
	public static string ComputeSHV2(this string s)
	{
		return Hash128.Compute(s).ToString();
	}

	// Token: 0x0600553B RID: 21819 RVA: 0x001BD045 File Offset: 0x001BB245
	public static string ToQueryString(this Dictionary<string, string> d)
	{
		if (d == null)
		{
			return null;
		}
		return "?" + string.Join("&", from x in d
		select x.Key + "=" + x.Value);
	}

	// Token: 0x0600553C RID: 21820 RVA: 0x001BD088 File Offset: 0x001BB288
	public static string Combine(string separator, params string[] values)
	{
		if (values == null || values.Length == 0)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = !string.IsNullOrEmpty(separator);
		for (int i = 0; i < values.Length; i++)
		{
			if (flag)
			{
				stringBuilder.Append(separator);
			}
			stringBuilder.Append(values);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600553D RID: 21821 RVA: 0x001BD0D8 File Offset: 0x001BB2D8
	public static string ToUpperCamelCase(this string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return string.Empty;
		}
		string[] array = Regex.Split(input, "[^A-Za-z0-9]+");
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Length > 0)
			{
				string[] array2 = array;
				int num = i;
				string str = char.ToUpper(array[i][0]).ToString();
				string str2;
				if (array[i].Length <= 1)
				{
					str2 = "";
				}
				else
				{
					string text = array[i];
					str2 = text.Substring(1, text.Length - 1).ToLower();
				}
				array2[num] = str + str2;
			}
		}
		return string.Join("", array);
	}

	// Token: 0x0600553E RID: 21822 RVA: 0x001BD16C File Offset: 0x001BB36C
	public static string ToUpperCaseFromCamelCase(this string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		input = input.Trim();
		string result;
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
		{
			bool flag = true;
			foreach (char c in input)
			{
				if (char.IsUpper(c) && !flag)
				{
					utf16ValueStringBuilder.Append(' ');
				}
				utf16ValueStringBuilder.Append(char.ToUpper(c));
				flag = char.IsUpper(c);
			}
			result = utf16ValueStringBuilder.ToString().Trim();
		}
		return result;
	}

	// Token: 0x0600553F RID: 21823 RVA: 0x001BD20C File Offset: 0x001BB40C
	public static string RemoveStart(this string s, string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
	{
		if (string.IsNullOrEmpty(s) || !s.StartsWith(value, comparison))
		{
			return s;
		}
		return s.Substring(value.Length);
	}

	// Token: 0x06005540 RID: 21824 RVA: 0x001BD22E File Offset: 0x001BB42E
	public static string RemoveEnd(this string s, string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
	{
		if (string.IsNullOrEmpty(s) || !s.EndsWith(value, comparison))
		{
			return s;
		}
		return s.Substring(0, s.Length - value.Length);
	}

	// Token: 0x06005541 RID: 21825 RVA: 0x001BD258 File Offset: 0x001BB458
	public static string RemoveBothEnds(this string s, string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
	{
		return s.RemoveEnd(value, comparison).RemoveStart(value, comparison);
	}

	// Token: 0x06005542 RID: 21826 RVA: 0x001BD269 File Offset: 0x001BB469
	public static string TrailingSpace(this string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			Debug.LogError("[STRING::UTILS] Trying to add Space, but string is null or empty");
			return s;
		}
		if (s[s.Length - 1] == ' ')
		{
			return s;
		}
		return s + " ";
	}

	// Token: 0x04006588 RID: 25992
	public const string kForwardSlash = "/";

	// Token: 0x04006589 RID: 25993
	public const string kBackSlash = "/";

	// Token: 0x0400658A RID: 25994
	public const string kBackTick = "`";

	// Token: 0x0400658B RID: 25995
	public const string kMinusDash = "-";

	// Token: 0x0400658C RID: 25996
	public const string kPeriod = ".";

	// Token: 0x0400658D RID: 25997
	public const string kUnderScore = "_";

	// Token: 0x0400658E RID: 25998
	public const string kColon = ":";
}
