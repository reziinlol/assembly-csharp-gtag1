using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000E53 RID: 3667
	// (Invoke) Token: 0x0600597F RID: 22911
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void StatusCallback2(int nResult, [MarshalAs(UnmanagedType.LPStr)] string message);
}
