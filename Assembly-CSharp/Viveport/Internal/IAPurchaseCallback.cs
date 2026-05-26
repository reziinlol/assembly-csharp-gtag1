using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000E5E RID: 3678
	// (Invoke) Token: 0x06005987 RID: 22919
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void IAPurchaseCallback(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
}
