using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02000C3E RID: 3134
[BurstCompile]
public static class BurstClassInfo
{
	// Token: 0x06004D80 RID: 19840 RVA: 0x0019CF60 File Offset: 0x0019B160
	public unsafe static void NewClass<[IsUnmanaged] T>(string className, Dictionary<int, FieldInfo> fieldList, Dictionary<int, lua_CFunction> functionList, Dictionary<int, FunctionPointer<lua_CFunction>> functionPtrList) where T : struct, ValueType
	{
		if (!BurstClassInfo.ClassList.InfoFields.Data.IsCreated)
		{
			*BurstClassInfo.ClassList.InfoFields.Data = new NativeHashMap<int, BurstClassInfo.ClassInfo>(20, Allocator.Persistent);
		}
		BurstClassInfo.ClassList.MetatableNames<T>.Name = className;
		ReflectionMetaNames.ReflectedNames.TryAdd(typeof(T), className);
		BurstClassInfo.ClassInfo classInfo = default(BurstClassInfo.ClassInfo);
		classInfo.NameHash = LuaHashing.ByteHash(className);
		if (className.Length > 30)
		{
			throw new Exception("Name to long");
		}
		classInfo.Name = className;
		classInfo.Size = sizeof(T);
		classInfo.FieldList = new NativeHashMap<int, BurstClassInfo.BurstFieldInfo>(fieldList.Count, Allocator.Persistent);
		foreach (KeyValuePair<int, FieldInfo> keyValuePair in fieldList)
		{
			BurstClassInfo.BurstFieldInfo item = default(BurstClassInfo.BurstFieldInfo);
			item.NameHash = keyValuePair.Key;
			item.Name = keyValuePair.Value.Name;
			item.Offset = (int)Marshal.OffsetOf<T>(keyValuePair.Value.Name);
			Type fieldType = keyValuePair.Value.FieldType;
			if (fieldType == typeof(float))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.Float;
			}
			else if (fieldType == typeof(int))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.Int;
			}
			else if (fieldType == typeof(double))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.Double;
			}
			else if (fieldType == typeof(bool))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.Bool;
			}
			else if (fieldType == typeof(FixedString32Bytes))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.String;
			}
			else if (!fieldType.IsPrimitive)
			{
				item.FieldType = BurstClassInfo.EFieldTypes.LightUserData;
				ReflectionMetaNames.ReflectedNames.TryGetValue(fieldType, out item.MetatableName);
			}
			item.Size = Marshal.SizeOf(fieldType);
			classInfo.FieldList.TryAdd(keyValuePair.Key, item);
		}
		classInfo.FunctionList = new NativeHashMap<int, IntPtr>(functionList.Count + functionPtrList.Count, Allocator.Persistent);
		foreach (KeyValuePair<int, lua_CFunction> keyValuePair2 in functionList)
		{
			classInfo.FunctionList.TryAdd(keyValuePair2.Key, Marshal.GetFunctionPointerForDelegate<lua_CFunction>(keyValuePair2.Value));
		}
		foreach (KeyValuePair<int, FunctionPointer<lua_CFunction>> keyValuePair3 in functionPtrList)
		{
			classInfo.FunctionList.TryAdd(keyValuePair3.Key, keyValuePair3.Value.Value);
		}
		BurstClassInfo.ClassList.InfoFields.Data.Add(classInfo.NameHash, classInfo);
	}

	// Token: 0x06004D81 RID: 19841 RVA: 0x0019D2A0 File Offset: 0x0019B4A0
	[BurstCompile]
	[MonoPInvokeCallback(typeof(BurstClassInfo.Index_00004CB1$PostfixBurstDelegate))]
	public unsafe static int Index(lua_State* L)
	{
		return BurstClassInfo.Index_00004CB1$BurstDirectCall.Invoke(L);
	}

	// Token: 0x06004D82 RID: 19842 RVA: 0x0019D2A8 File Offset: 0x0019B4A8
	[BurstCompile]
	[MonoPInvokeCallback(typeof(BurstClassInfo.NewIndex_00004CB2$PostfixBurstDelegate))]
	public unsafe static int NewIndex(lua_State* L)
	{
		return BurstClassInfo.NewIndex_00004CB2$BurstDirectCall.Invoke(L);
	}

	// Token: 0x06004D83 RID: 19843 RVA: 0x0019D2B0 File Offset: 0x0019B4B0
	[BurstCompile]
	[MonoPInvokeCallback(typeof(BurstClassInfo.NameCall_00004CB3$PostfixBurstDelegate))]
	public unsafe static int NameCall(lua_State* L)
	{
		return BurstClassInfo.NameCall_00004CB3$BurstDirectCall.Invoke(L);
	}

	// Token: 0x06004D85 RID: 19845 RVA: 0x0019D2CC File Offset: 0x0019B4CC
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int Index$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* k = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, k);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
		{
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		byte* tname = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref classInfo.Name) + 2;
		IntPtr pointer = IntPtr.Zero;
		Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, 1);
		if (lua_Types == Luau.lua_Types.LUA_TUSERDATA)
		{
			pointer = (IntPtr)Luau.luaL_checkudata(L, 1, tname);
		}
		else
		{
			if (lua_Types != Luau.lua_Types.LUA_TTABLE)
			{
				FixedString32Bytes fixedString32Bytes2 = "\"Unknown type for __index\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
				return 0;
			}
			pointer = Luau.lua_light_ptr(L, 1);
		}
		int len = Luau.lua_objlen(L, 2);
		int key = LuaHashing.ByteHash(Luau.luaL_checkstring(L, 2), len);
		BurstClassInfo.BurstFieldInfo burstFieldInfo;
		if (classInfo.FieldList.TryGetValue(key, out burstFieldInfo))
		{
			IntPtr intPtr = pointer + burstFieldInfo.Offset;
			switch (burstFieldInfo.FieldType)
			{
			case BurstClassInfo.EFieldTypes.Float:
				Luau.lua_pushnumber(L, (double)(*(float*)((void*)intPtr)));
				return 1;
			case BurstClassInfo.EFieldTypes.Int:
				Luau.lua_pushnumber(L, (double)(*(int*)((void*)intPtr)));
				return 1;
			case BurstClassInfo.EFieldTypes.Double:
				Luau.lua_pushnumber(L, *(double*)((void*)intPtr));
				return 1;
			case BurstClassInfo.EFieldTypes.Bool:
				Luau.lua_pushboolean(L, (*(byte*)((void*)intPtr) != 0) ? 1 : 0);
				return 1;
			case BurstClassInfo.EFieldTypes.String:
				Luau.lua_pushstring(L, (byte*)((void*)intPtr) + 2);
				return 1;
			case BurstClassInfo.EFieldTypes.LightUserData:
				Luau.lua_class_push(L, burstFieldInfo.MetatableName, intPtr);
				return 1;
			}
		}
		IntPtr ptr;
		if (classInfo.FunctionList.TryGetValue(key, out ptr))
		{
			FunctionPointer<lua_CFunction> fn = new FunctionPointer<lua_CFunction>(ptr);
			FixedString32Bytes fixedString32Bytes3 = "";
			Luau.lua_pushcclosurek(L, fn, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2, 0, null);
			return 1;
		}
		FixedString32Bytes fixedString32Bytes4 = "\"Unknown Type?\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes4) + 2));
		return 0;
	}

	// Token: 0x06004D86 RID: 19846 RVA: 0x0019D4C0 File Offset: 0x0019B6C0
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int NewIndex$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* k = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, k);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
		{
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		byte* tname = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref classInfo.Name) + 2;
		IntPtr pointer = IntPtr.Zero;
		Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, 1);
		if (lua_Types == Luau.lua_Types.LUA_TUSERDATA)
		{
			pointer = (IntPtr)Luau.luaL_checkudata(L, 1, tname);
		}
		else
		{
			if (lua_Types != Luau.lua_Types.LUA_TTABLE)
			{
				FixedString32Bytes fixedString32Bytes2 = "\"Unknown type for __newindex\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
				return 0;
			}
			pointer = Luau.lua_light_ptr(L, 1);
		}
		int len = Luau.lua_objlen(L, 2);
		int key = LuaHashing.ByteHash(Luau.luaL_checkstring(L, 2), len);
		BurstClassInfo.BurstFieldInfo burstFieldInfo;
		if (classInfo.FieldList.TryGetValue(key, out burstFieldInfo))
		{
			IntPtr value = pointer + burstFieldInfo.Offset;
			switch (burstFieldInfo.FieldType)
			{
			case BurstClassInfo.EFieldTypes.Float:
				*(float*)((void*)value) = (float)Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Int:
				*(int*)((void*)value) = (int)Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Double:
				*(double*)((void*)value) = Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Bool:
				*(byte*)((void*)value) = ((Luau.lua_toboolean(L, 3) != 0) ? 1 : 0);
				return 0;
			case BurstClassInfo.EFieldTypes.LightUserData:
				Buffer.MemoryCopy((void*)((IntPtr)((void*)Luau.lua_class_get(L, 3, burstFieldInfo.MetatableName))), (void*)value, (long)burstFieldInfo.Size, (long)burstFieldInfo.Size);
				return 0;
			}
		}
		FixedString32Bytes fixedString32Bytes3 = "\"Unknown Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
		return 0;
	}

	// Token: 0x06004D87 RID: 19847 RVA: 0x0019D690 File Offset: 0x0019B890
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int NameCall$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* k = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, k);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
		{
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		int key = LuaHashing.ByteHash(Luau.lua_namecallatom(L, null));
		IntPtr ptr;
		if (classInfo.FunctionList.TryGetValue(key, out ptr))
		{
			FunctionPointer<lua_CFunction> functionPointer = new FunctionPointer<lua_CFunction>(ptr);
			return functionPointer.Invoke(L);
		}
		FixedString32Bytes fixedString32Bytes2 = "\"Function not found in function list\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
		return 0;
	}

	// Token: 0x04005FC1 RID: 24513
	private static readonly FixedString32Bytes _k_metatableLookup = "metahash";

	// Token: 0x02000C3F RID: 3135
	public enum EFieldTypes
	{
		// Token: 0x04005FC3 RID: 24515
		Float,
		// Token: 0x04005FC4 RID: 24516
		Int,
		// Token: 0x04005FC5 RID: 24517
		Double,
		// Token: 0x04005FC6 RID: 24518
		Bool,
		// Token: 0x04005FC7 RID: 24519
		String,
		// Token: 0x04005FC8 RID: 24520
		LightUserData
	}

	// Token: 0x02000C40 RID: 3136
	[BurstCompile]
	public struct BurstFieldInfo
	{
		// Token: 0x04005FC9 RID: 24521
		public int NameHash;

		// Token: 0x04005FCA RID: 24522
		public FixedString32Bytes Name;

		// Token: 0x04005FCB RID: 24523
		public FixedString32Bytes MetatableName;

		// Token: 0x04005FCC RID: 24524
		public int Offset;

		// Token: 0x04005FCD RID: 24525
		public BurstClassInfo.EFieldTypes FieldType;

		// Token: 0x04005FCE RID: 24526
		public int Size;
	}

	// Token: 0x02000C41 RID: 3137
	[BurstCompile]
	public struct ClassInfo
	{
		// Token: 0x04005FCF RID: 24527
		public int NameHash;

		// Token: 0x04005FD0 RID: 24528
		public int Size;

		// Token: 0x04005FD1 RID: 24529
		public FixedString32Bytes Name;

		// Token: 0x04005FD2 RID: 24530
		public NativeHashMap<int, BurstClassInfo.BurstFieldInfo> FieldList;

		// Token: 0x04005FD3 RID: 24531
		public NativeHashMap<int, IntPtr> FunctionList;
	}

	// Token: 0x02000C42 RID: 3138
	public abstract class ClassList
	{
		// Token: 0x04005FD4 RID: 24532
		public static readonly SharedStatic<NativeHashMap<int, BurstClassInfo.ClassInfo>> InfoFields = SharedStatic<NativeHashMap<int, BurstClassInfo.ClassInfo>>.GetOrCreateUnsafe(0U, -7258312696341931442L, -7445903157129162016L);

		// Token: 0x02000C43 RID: 3139
		private class FieldKey
		{
		}

		// Token: 0x02000C44 RID: 3140
		public static class MetatableNames<T>
		{
			// Token: 0x04005FD5 RID: 24533
			public static FixedString32Bytes Name;
		}
	}

	// Token: 0x02000C45 RID: 3141
	// (Invoke) Token: 0x06004D8C RID: 19852
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal unsafe delegate int Index_00004CB1$PostfixBurstDelegate(lua_State* L);

	// Token: 0x02000C46 RID: 3142
	internal static class Index_00004CB1$BurstDirectCall
	{
		// Token: 0x06004D8F RID: 19855 RVA: 0x0019D768 File Offset: 0x0019B968
		[BurstDiscard]
		private static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.Index_00004CB1$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.Index_00004CB1$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstClassInfo.Index_00004CB1$PostfixBurstDelegate>(new BurstClassInfo.Index_00004CB1$PostfixBurstDelegate(BurstClassInfo.Index)).Value;
			}
			A_0 = BurstClassInfo.Index_00004CB1$BurstDirectCall.Pointer;
		}

		// Token: 0x06004D90 RID: 19856 RVA: 0x0019D7A8 File Offset: 0x0019B9A8
		private static IntPtr GetFunctionPointer()
		{
			IntPtr result = (IntPtr)0;
			BurstClassInfo.Index_00004CB1$BurstDirectCall.GetFunctionPointerDiscard(ref result);
			return result;
		}

		// Token: 0x06004D91 RID: 19857 RVA: 0x0019D7C0 File Offset: 0x0019B9C0
		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.Index_00004CB1$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.Index$BurstManaged(L);
		}

		// Token: 0x04005FD6 RID: 24534
		private static IntPtr Pointer;
	}

	// Token: 0x02000C47 RID: 3143
	// (Invoke) Token: 0x06004D93 RID: 19859
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal unsafe delegate int NewIndex_00004CB2$PostfixBurstDelegate(lua_State* L);

	// Token: 0x02000C48 RID: 3144
	internal static class NewIndex_00004CB2$BurstDirectCall
	{
		// Token: 0x06004D96 RID: 19862 RVA: 0x0019D7F4 File Offset: 0x0019B9F4
		[BurstDiscard]
		private static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.NewIndex_00004CB2$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.NewIndex_00004CB2$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstClassInfo.NewIndex_00004CB2$PostfixBurstDelegate>(new BurstClassInfo.NewIndex_00004CB2$PostfixBurstDelegate(BurstClassInfo.NewIndex)).Value;
			}
			A_0 = BurstClassInfo.NewIndex_00004CB2$BurstDirectCall.Pointer;
		}

		// Token: 0x06004D97 RID: 19863 RVA: 0x0019D834 File Offset: 0x0019BA34
		private static IntPtr GetFunctionPointer()
		{
			IntPtr result = (IntPtr)0;
			BurstClassInfo.NewIndex_00004CB2$BurstDirectCall.GetFunctionPointerDiscard(ref result);
			return result;
		}

		// Token: 0x06004D98 RID: 19864 RVA: 0x0019D84C File Offset: 0x0019BA4C
		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.NewIndex_00004CB2$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.NewIndex$BurstManaged(L);
		}

		// Token: 0x04005FD7 RID: 24535
		private static IntPtr Pointer;
	}

	// Token: 0x02000C49 RID: 3145
	// (Invoke) Token: 0x06004D9A RID: 19866
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal unsafe delegate int NameCall_00004CB3$PostfixBurstDelegate(lua_State* L);

	// Token: 0x02000C4A RID: 3146
	internal static class NameCall_00004CB3$BurstDirectCall
	{
		// Token: 0x06004D9D RID: 19869 RVA: 0x0019D880 File Offset: 0x0019BA80
		[BurstDiscard]
		private static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.NameCall_00004CB3$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.NameCall_00004CB3$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstClassInfo.NameCall_00004CB3$PostfixBurstDelegate>(new BurstClassInfo.NameCall_00004CB3$PostfixBurstDelegate(BurstClassInfo.NameCall)).Value;
			}
			A_0 = BurstClassInfo.NameCall_00004CB3$BurstDirectCall.Pointer;
		}

		// Token: 0x06004D9E RID: 19870 RVA: 0x0019D8C0 File Offset: 0x0019BAC0
		private static IntPtr GetFunctionPointer()
		{
			IntPtr result = (IntPtr)0;
			BurstClassInfo.NameCall_00004CB3$BurstDirectCall.GetFunctionPointerDiscard(ref result);
			return result;
		}

		// Token: 0x06004D9F RID: 19871 RVA: 0x0019D8D8 File Offset: 0x0019BAD8
		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.NameCall_00004CB3$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.NameCall$BurstManaged(L);
		}

		// Token: 0x04005FD8 RID: 24536
		private static IntPtr Pointer;
	}
}
