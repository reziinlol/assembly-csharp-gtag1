using System;
using System.Collections.Generic;

// Token: 0x0200032D RID: 813
public static class DelegateExtensions
{
	// Token: 0x0600141B RID: 5147 RVA: 0x0006C778 File Offset: 0x0006A978
	public static List<string> ToStringList(this Delegate[] invocationList)
	{
		List<string> list = new List<string>();
		if (invocationList != null)
		{
			foreach (Delegate @delegate in invocationList)
			{
				string name = @delegate.Method.Name;
				string str = (@delegate.Target != null) ? @delegate.Target.GetType().FullName : "Static Method";
				list.Add(str + "." + name);
			}
		}
		return list;
	}

	// Token: 0x0600141C RID: 5148 RVA: 0x0006C7E5 File Offset: 0x0006A9E5
	public static string ToText(this Delegate[] invocationList)
	{
		return string.Join(", ", invocationList.ToStringList());
	}
}
