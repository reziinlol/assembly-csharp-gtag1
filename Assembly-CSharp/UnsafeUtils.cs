using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Token: 0x02000D9B RID: 3483
public static class UnsafeUtils
{
	// Token: 0x0600557B RID: 21883 RVA: 0x001BDE20 File Offset: 0x001BC020
	public unsafe static ref readonly T[] GetInternalArray<T>(this List<T> list)
	{
		if (list == null)
		{
			return Unsafe.NullRef<T[]>();
		}
		return ref Unsafe.As<List<T>, StrongBox<T[]>>(ref list)->Value;
	}

	// Token: 0x0600557C RID: 21884 RVA: 0x001BDE38 File Offset: 0x001BC038
	public unsafe static ref readonly T[] GetInvocationListUnsafe<T>(this T @delegate) where T : MulticastDelegate
	{
		if (@delegate == null)
		{
			return Unsafe.NullRef<T[]>();
		}
		return Unsafe.As<Delegate[], T[]>(ref Unsafe.As<T, UnsafeUtils._MultiDelegateFields>(ref @delegate)->delegates);
	}

	// Token: 0x02000D9C RID: 3484
	[StructLayout(LayoutKind.Sequential)]
	private class _MultiDelegateFields : UnsafeUtils._DelegateFields
	{
		// Token: 0x040065AB RID: 26027
		public Delegate[] delegates;
	}

	// Token: 0x02000D9D RID: 3485
	[StructLayout(LayoutKind.Sequential)]
	private class _DelegateFields
	{
		// Token: 0x040065AC RID: 26028
		public IntPtr method_ptr;

		// Token: 0x040065AD RID: 26029
		public IntPtr invoke_impl;

		// Token: 0x040065AE RID: 26030
		public object m_target;

		// Token: 0x040065AF RID: 26031
		public IntPtr method;

		// Token: 0x040065B0 RID: 26032
		public IntPtr delegate_trampoline;

		// Token: 0x040065B1 RID: 26033
		public IntPtr extra_arg;

		// Token: 0x040065B2 RID: 26034
		public IntPtr method_code;

		// Token: 0x040065B3 RID: 26035
		public IntPtr interp_method;

		// Token: 0x040065B4 RID: 26036
		public IntPtr interp_invoke_impl;

		// Token: 0x040065B5 RID: 26037
		public MethodInfo method_info;

		// Token: 0x040065B6 RID: 26038
		public MethodInfo original_method_info;

		// Token: 0x040065B7 RID: 26039
		public UnsafeUtils._DelegateData data;

		// Token: 0x040065B8 RID: 26040
		public bool method_is_virtual;
	}

	// Token: 0x02000D9E RID: 3486
	[StructLayout(LayoutKind.Sequential)]
	private class _DelegateData
	{
		// Token: 0x040065B9 RID: 26041
		public Type target_type;

		// Token: 0x040065BA RID: 26042
		public string method_name;

		// Token: 0x040065BB RID: 26043
		public bool curried_first_arg;
	}
}
