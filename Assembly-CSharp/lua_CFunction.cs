using System;
using System.Runtime.InteropServices;

// Token: 0x02000C4F RID: 3151
// (Invoke) Token: 0x06004DAE RID: 19886
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate int lua_CFunction(lua_State* L);
