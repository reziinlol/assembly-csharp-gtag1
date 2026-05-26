using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000D21 RID: 3361
public static class ArrayUtils
{
	// Token: 0x060052F6 RID: 21238 RVA: 0x001B2D5A File Offset: 0x001B0F5A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int BinarySearch<T>(this T[] array, T value) where T : IComparable<T>
	{
		return Array.BinarySearch<T>(array, 0, array.Length, value);
	}

	// Token: 0x060052F7 RID: 21239 RVA: 0x001B2D67 File Offset: 0x001B0F67
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrEmpty<T>(this T[] array)
	{
		return array == null || array.Length == 0;
	}

	// Token: 0x060052F8 RID: 21240 RVA: 0x001B2D73 File Offset: 0x001B0F73
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrEmpty<T>(this List<T> list)
	{
		return list == null || list.Count == 0;
	}

	// Token: 0x060052F9 RID: 21241 RVA: 0x001B2D84 File Offset: 0x001B0F84
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(this T[] array, int from, int to)
	{
		T t = array[from];
		T t2 = array[to];
		array[to] = t;
		array[from] = t2;
	}

	// Token: 0x060052FA RID: 21242 RVA: 0x001B2DBC File Offset: 0x001B0FBC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(this List<T> list, int from, int to)
	{
		T value = list[from];
		T value2 = list[to];
		list[to] = value;
		list[from] = value2;
	}

	// Token: 0x060052FB RID: 21243 RVA: 0x001B2DF8 File Offset: 0x001B0FF8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] Clone<T>(T[] source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.Length == 0)
		{
			return Array.Empty<T>();
		}
		T[] array = new T[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			array[i] = source[i];
		}
		return array;
	}

	// Token: 0x060052FC RID: 21244 RVA: 0x001B2E3A File Offset: 0x001B103A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static List<T> Clone<T>(List<T> source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.Count == 0)
		{
			return new List<T>();
		}
		return new List<T>(source);
	}

	// Token: 0x060052FD RID: 21245 RVA: 0x001B2E58 File Offset: 0x001B1058
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfRef<T>(this T[] array, T value) where T : class
	{
		if (array == null || array.Length == 0)
		{
			return -1;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060052FE RID: 21246 RVA: 0x001B2E94 File Offset: 0x001B1094
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfRef<T>(this List<T> list, T value) where T : class
	{
		if (list == null || list.Count == 0)
		{
			return -1;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060052FF RID: 21247 RVA: 0x001B2ED8 File Offset: 0x001B10D8
	public static bool GTEnsureNoNulls<T>(ref T[] unityObjs) where T : Object
	{
		if (unityObjs == null)
		{
			unityObjs = Array.Empty<T>();
		}
		int num = 0;
		for (int i = 0; i < unityObjs.Length; i++)
		{
			if (!(unityObjs[i] == null))
			{
				unityObjs[num] = unityObjs[i];
				num++;
			}
		}
		bool flag = num != unityObjs.Length;
		if (flag)
		{
			Array.Resize<T>(ref unityObjs, num);
		}
		return flag;
	}
}
