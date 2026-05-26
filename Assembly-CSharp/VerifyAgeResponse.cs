using System;
using System.Runtime.CompilerServices;
using KID.Model;

// Token: 0x02000B1F RID: 2847
public class VerifyAgeResponse
{
	// Token: 0x170006CE RID: 1742
	// (get) Token: 0x06004854 RID: 18516 RVA: 0x001829A3 File Offset: 0x00180BA3
	// (set) Token: 0x06004855 RID: 18517 RVA: 0x001829AB File Offset: 0x00180BAB
	public SessionStatus Status { get; set; }

	// Token: 0x170006CF RID: 1743
	// (get) Token: 0x06004856 RID: 18518 RVA: 0x001829B4 File Offset: 0x00180BB4
	// (set) Token: 0x06004857 RID: 18519 RVA: 0x001829BC File Offset: 0x00180BBC
	[Nullable(2)]
	public Session Session { [NullableContext(2)] get; [NullableContext(2)] set; }

	// Token: 0x170006D0 RID: 1744
	// (get) Token: 0x06004858 RID: 18520 RVA: 0x001829C5 File Offset: 0x00180BC5
	// (set) Token: 0x06004859 RID: 18521 RVA: 0x001829CD File Offset: 0x00180BCD
	public KIDDefaultSession DefaultSession { get; set; }
}
