using System;
using KID.Model;

// Token: 0x02000B18 RID: 2840
[Serializable]
public class GetPlayerDataResponse
{
	// Token: 0x04005A8D RID: 23181
	public SessionStatus? Status;

	// Token: 0x04005A8E RID: 23182
	public Session Session;

	// Token: 0x04005A8F RID: 23183
	public KIDDefaultSession DefaultSession;

	// Token: 0x04005A90 RID: 23184
	public string[] Permissions;

	// Token: 0x04005A91 RID: 23185
	public bool HasConfirmedSetup;
}
