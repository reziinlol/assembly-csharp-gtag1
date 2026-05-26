using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000B03 RID: 2819
public class GetPlayerData_Data
{
	// Token: 0x0600481B RID: 18459 RVA: 0x0018278C File Offset: 0x0018098C
	public GetPlayerData_Data(GetSessionResponseType type, GetPlayerDataResponse response)
	{
		this.responseType = type;
		if (response == null)
		{
			if (this.responseType == GetSessionResponseType.OK)
			{
				this.responseType = GetSessionResponseType.ERROR;
				Debug.LogError("[KID::GET_PLAYER_DATA_DATA] Incoming [GetPlayerDataResponse] is NULL");
			}
			return;
		}
		this.status = response.Status;
		if (this.status != null)
		{
			this.session = new TMPSession(response.Session, response.DefaultSession, this.status.Value);
			this.session.SetOptInPermissions(response.Permissions);
			Debug.Log("[KID::GET_PLAYER_DATA_DATA::OptInRefactor] Setting Opt-in Permissions: " + string.Join(", ", this.session.GetOptedInPermissions()));
		}
		this.HasConfirmedSetup = response.HasConfirmedSetup;
	}

	// Token: 0x04005A49 RID: 23113
	public readonly GetSessionResponseType responseType;

	// Token: 0x04005A4A RID: 23114
	public readonly SessionStatus? status;

	// Token: 0x04005A4B RID: 23115
	public readonly TMPSession session;

	// Token: 0x04005A4C RID: 23116
	[Nullable(new byte[]
	{
		2,
		0
	})]
	public readonly string[] OptInPermissions;

	// Token: 0x04005A4D RID: 23117
	public readonly bool HasConfirmedSetup;
}
