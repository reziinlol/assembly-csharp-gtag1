using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x020011AB RID: 4523
	public static class GRef
	{
		// Token: 0x0600725C RID: 29276 RVA: 0x00253A34 File Offset: 0x00251C34
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ShouldResolveNow(GRef.EResolveModes mode)
		{
			return Application.isPlaying && (mode & GRef.EResolveModes.Runtime) == GRef.EResolveModes.Runtime;
		}

		// Token: 0x0600725D RID: 29277 RVA: 0x00253A45 File Offset: 0x00251C45
		public static bool IsAnyResolveModeOn(GRef.EResolveModes mode)
		{
			return mode > GRef.EResolveModes.None;
		}

		// Token: 0x020011AC RID: 4524
		[Flags]
		public enum EResolveModes
		{
			// Token: 0x04008235 RID: 33333
			None = 0,
			// Token: 0x04008236 RID: 33334
			Runtime = 1,
			// Token: 0x04008237 RID: 33335
			SceneProcessing = 2
		}
	}
}
