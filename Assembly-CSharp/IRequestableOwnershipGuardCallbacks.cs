using System;

// Token: 0x02000394 RID: 916
public interface IRequestableOwnershipGuardCallbacks
{
	// Token: 0x06001646 RID: 5702
	void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer);

	// Token: 0x06001647 RID: 5703
	bool OnOwnershipRequest(NetPlayer fromPlayer);

	// Token: 0x06001648 RID: 5704
	void OnMyOwnerLeft();

	// Token: 0x06001649 RID: 5705
	bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer);

	// Token: 0x0600164A RID: 5706
	void OnMyCreatorLeft();
}
