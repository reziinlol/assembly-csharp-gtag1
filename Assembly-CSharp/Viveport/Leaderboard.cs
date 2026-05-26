using System;

namespace Viveport
{
	// Token: 0x02000E31 RID: 3633
	public class Leaderboard
	{
		// Token: 0x1700085E RID: 2142
		// (get) Token: 0x0600586E RID: 22638 RVA: 0x001CA86F File Offset: 0x001C8A6F
		// (set) Token: 0x0600586F RID: 22639 RVA: 0x001CA877 File Offset: 0x001C8A77
		public int Rank { get; set; }

		// Token: 0x1700085F RID: 2143
		// (get) Token: 0x06005870 RID: 22640 RVA: 0x001CA880 File Offset: 0x001C8A80
		// (set) Token: 0x06005871 RID: 22641 RVA: 0x001CA888 File Offset: 0x001C8A88
		public int Score { get; set; }

		// Token: 0x17000860 RID: 2144
		// (get) Token: 0x06005872 RID: 22642 RVA: 0x001CA891 File Offset: 0x001C8A91
		// (set) Token: 0x06005873 RID: 22643 RVA: 0x001CA899 File Offset: 0x001C8A99
		public string UserName { get; set; }
	}
}
