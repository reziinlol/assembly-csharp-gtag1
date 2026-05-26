using System;

namespace Utilities
{
	// Token: 0x02000EA1 RID: 3745
	public static class PathUtils
	{
		// Token: 0x06005C00 RID: 23552 RVA: 0x001D3E98 File Offset: 0x001D2098
		public static string Resolve(params string[] subPaths)
		{
			if (subPaths == null || subPaths.Length == 0)
			{
				return null;
			}
			string[] value = string.Concat(subPaths).Split(PathUtils.kPathSeps, StringSplitOptions.RemoveEmptyEntries);
			return Uri.UnescapeDataString(new Uri(string.Join("/", value)).AbsolutePath);
		}

		// Token: 0x04006A84 RID: 27268
		private static readonly char[] kPathSeps = new char[]
		{
			'\\',
			'/'
		};

		// Token: 0x04006A85 RID: 27269
		private const string kFwdSlash = "/";
	}
}
