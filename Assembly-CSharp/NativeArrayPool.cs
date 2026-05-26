using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

// Token: 0x020001E7 RID: 487
public static class NativeArrayPool<T> where T : struct
{
	// Token: 0x06000CBD RID: 3261 RVA: 0x00046173 File Offset: 0x00044373
	static NativeArrayPool()
	{
		Application.quitting += NativeArrayPool<T>.OnQuit;
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x00046190 File Offset: 0x00044390
	private static void OnQuit()
	{
		NativeArrayPool<T>.Dispose();
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x00046198 File Offset: 0x00044398
	[OnEnterPlay_Run]
	public static void Dispose()
	{
		if (NativeArrayPool<T>._lookup == null)
		{
			return;
		}
		foreach (Stack<NativeArray<T>> stack in NativeArrayPool<T>._lookup.Values)
		{
			foreach (NativeArray<T> nativeArray in stack)
			{
				nativeArray.Dispose();
			}
		}
		NativeArrayPool<T>._lookup.Clear();
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x00046238 File Offset: 0x00044438
	public static NativeArray<T> Get(int length)
	{
		NativeArray<T> result;
		if (!NativeArrayPool<T>.GetCollectionForLength(length).TryPop(out result))
		{
			result = new NativeArray<T>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		}
		return result;
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x0004625F File Offset: 0x0004445F
	public static void Return(NativeArray<T> item)
	{
		NativeArrayPool<T>.GetCollectionForLength(item.Length).Push(item);
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x00046274 File Offset: 0x00044474
	private static Stack<NativeArray<T>> GetCollectionForLength(int length)
	{
		Stack<NativeArray<T>> stack;
		if (!NativeArrayPool<T>._lookup.TryGetValue(length, out stack))
		{
			stack = new Stack<NativeArray<T>>();
			NativeArrayPool<T>._lookup.Add(length, stack);
		}
		return stack;
	}

	// Token: 0x04000F6D RID: 3949
	private static Dictionary<int, Stack<NativeArray<T>>> _lookup = new Dictionary<int, Stack<NativeArray<T>>>();
}
