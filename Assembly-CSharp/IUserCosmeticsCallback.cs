using System;

// Token: 0x02000CE0 RID: 3296
internal interface IUserCosmeticsCallback
{
	// Token: 0x060051A8 RID: 20904
	bool OnGetUserCosmetics(string cosmetics);

	// Token: 0x170007A2 RID: 1954
	// (get) Token: 0x060051A9 RID: 20905
	// (set) Token: 0x060051AA RID: 20906
	bool PendingUpdate { get; set; }
}
