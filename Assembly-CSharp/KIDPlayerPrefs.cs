using System;

// Token: 0x02000B63 RID: 2915
public class KIDPlayerPrefs
{
	// Token: 0x04005C18 RID: 23576
	public const string SESSION_ID_PREFIX_PLAYER_PREF = "kIDSessionID-";

	// Token: 0x04005C19 RID: 23577
	public const string SESSION_ETAG_PLAYER_PREF = "kIDSessionETAG-";

	// Token: 0x04005C1A RID: 23578
	public const string SESSION_CHANGED_PLAYER_PREF = "kIDSessionUpdated-";

	// Token: 0x04005C1B RID: 23579
	private const string KID_PERMISSIONS_CSV = "kid-permission-csv";

	// Token: 0x04005C1C RID: 23580
	private const string KID_DEFAULT_PERMISSIONS_CSV = "kid-default-permission-csv";

	// Token: 0x04005C1D RID: 23581
	private const string KID_PERMISSIONS_ENABLED_KEY = "-enabled";

	// Token: 0x04005C1E RID: 23582
	private const string KID_PERMISSIONS_MANAGED_BY_KEY = "-managed-by";

	// Token: 0x04005C1F RID: 23583
	private const string KID_EMAIL_KEY = "k-id_EmailAddress";
}
