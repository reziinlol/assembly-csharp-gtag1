using System;

// Token: 0x02000B23 RID: 2851
public class UpgradeSessionData
{
	// Token: 0x0600486B RID: 18539 RVA: 0x0018302C File Offset: 0x0018122C
	public UpgradeSessionData(UpgradeSessionResponse response)
	{
		this.status = response.status;
		this.session = new TMPSession(response.session, null, this.status);
	}

	// Token: 0x04005AB0 RID: 23216
	public readonly SessionStatus status;

	// Token: 0x04005AB1 RID: 23217
	public readonly TMPSession session;
}
