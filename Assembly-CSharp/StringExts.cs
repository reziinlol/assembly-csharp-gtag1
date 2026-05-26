using System;

// Token: 0x02000AEF RID: 2799
public static class StringExts
{
	// Token: 0x0600479E RID: 18334 RVA: 0x00180FA9 File Offset: 0x0017F1A9
	public static string EscapeCsv(this string field)
	{
		if (StringExts._escapeChars == null)
		{
			StringExts._escapeChars = new char[]
			{
				',',
				'"',
				'\n',
				'\r'
			};
		}
		if (field.IndexOfAny(StringExts._escapeChars) != -1)
		{
			return field.Replace("\"", "\"\"");
		}
		return field;
	}

	// Token: 0x040059CB RID: 22987
	private static char[] _escapeChars;
}
