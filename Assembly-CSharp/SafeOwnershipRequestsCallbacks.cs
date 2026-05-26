using System;
using UnityEngine;

// Token: 0x02000841 RID: 2113
public class SafeOwnershipRequestsCallbacks : MonoBehaviour, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x06003677 RID: 13943 RVA: 0x0012C120 File Offset: 0x0012A320
	private void Awake()
	{
		this._requestableOwnershipGuard.AddCallbackTarget(this);
	}

	// Token: 0x06003678 RID: 13944 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IRequestableOwnershipGuardCallbacks.OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
	}

	// Token: 0x06003679 RID: 13945 RVA: 0x00002076 File Offset: 0x00000276
	bool IRequestableOwnershipGuardCallbacks.OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return false;
	}

	// Token: 0x0600367A RID: 13946 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IRequestableOwnershipGuardCallbacks.OnMyOwnerLeft()
	{
	}

	// Token: 0x0600367B RID: 13947 RVA: 0x00002076 File Offset: 0x00000276
	bool IRequestableOwnershipGuardCallbacks.OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x0600367C RID: 13948 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IRequestableOwnershipGuardCallbacks.OnMyCreatorLeft()
	{
	}

	// Token: 0x040046D5 RID: 18133
	[SerializeField]
	private RequestableOwnershipGuard _requestableOwnershipGuard;
}
