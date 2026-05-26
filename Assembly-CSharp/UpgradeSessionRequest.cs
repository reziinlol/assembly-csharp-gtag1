using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x02000B1C RID: 2844
[Serializable]
public class UpgradeSessionRequest : KIDRequestData
{
	// Token: 0x04005A96 RID: 23190
	public List<RequestedPermission> Permissions;
}
