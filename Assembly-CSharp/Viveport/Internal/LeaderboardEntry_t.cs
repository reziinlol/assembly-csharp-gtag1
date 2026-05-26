using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000E5D RID: 3677
	internal struct LeaderboardEntry_t
	{
		// Token: 0x0400699E RID: 27038
		internal int m_nGlobalRank;

		// Token: 0x0400699F RID: 27039
		internal int m_nScore;

		// Token: 0x040069A0 RID: 27040
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		internal string m_pUserName;
	}
}
