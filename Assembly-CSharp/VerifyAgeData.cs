using System;

// Token: 0x02000B24 RID: 2852
public class VerifyAgeData
{
	// Token: 0x0600486C RID: 18540 RVA: 0x00183058 File Offset: 0x00181258
	public VerifyAgeData(VerifyAgeResponse response)
	{
		if (response == null)
		{
			return;
		}
		this.Status = response.Status;
		if (response.Session == null && response.DefaultSession == null)
		{
			return;
		}
		this.Session = new TMPSession(response.Session, response.DefaultSession, this.Status);
	}

	// Token: 0x04005AB2 RID: 23218
	public readonly SessionStatus Status;

	// Token: 0x04005AB3 RID: 23219
	public readonly TMPSession Session;
}
