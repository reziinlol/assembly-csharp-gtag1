using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020004FE RID: 1278
public class CosmeticBoundaryTrigger : GorillaTriggerBox
{
	// Token: 0x0600200A RID: 8202 RVA: 0x000AC4B0 File Offset: 0x000AA6B0
	public void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null)
		{
			return;
		}
		if (CosmeticBoundaryTrigger.sinceLastTryOnEvent.HasElapsed(0.5f, true))
		{
			GorillaTelemetry.PostShopEvent(this.rigRef, GTShopEventType.item_try_on, this.rigRef.tryOnSet.items);
		}
		this.rigRef.inTryOnRoom = true;
		this.rigRef.LocalUpdateCosmeticsWithTryon(this.rigRef.cosmeticSet, this.rigRef.tryOnSet, false);
		this.rigRef.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x0600200B RID: 8203 RVA: 0x000AC560 File Offset: 0x000AA760
	public void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null)
		{
			return;
		}
		this.rigRef.inTryOnRoom = false;
		if (this.rigRef.isOfflineVRRig)
		{
			this.rigRef.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
			CosmeticsController.instance.ClearCheckout(false);
			CosmeticsController.instance.UpdateShoppingCart();
			CosmeticsController.instance.UpdateWornCosmetics(true);
			CosmeticsController.ClearTryOnCollectable();
		}
		this.rigRef.LocalUpdateCosmeticsWithTryon(this.rigRef.cosmeticSet, this.rigRef.tryOnSet, false);
		this.rigRef.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x04002ACC RID: 10956
	public VRRig rigRef;

	// Token: 0x04002ACD RID: 10957
	private static TimeSince sinceLastTryOnEvent = 0f;
}
