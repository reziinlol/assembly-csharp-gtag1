using System;
using KID.Model;

// Token: 0x02000B19 RID: 2841
[Serializable]
public class GetRequirementsResponse : GetAgeGateRequirementsResponse
{
	// Token: 0x170006CD RID: 1741
	// (get) Token: 0x0600484C RID: 18508 RVA: 0x0018298A File Offset: 0x00180B8A
	// (set) Token: 0x0600484D RID: 18509 RVA: 0x00182992 File Offset: 0x00180B92
	public int PlatformMinimumAge { get; set; }
}
