using System;
using UnityEngine;

// Token: 0x02000525 RID: 1317
public abstract class HoldableObject : MonoBehaviour, IHoldableObject
{
	// Token: 0x1700038E RID: 910
	// (get) Token: 0x06002106 RID: 8454 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002107 RID: 8455 RVA: 0x000B0913 File Offset: 0x000AEB13
	protected void OnDestroy()
	{
		if (EquipmentInteractor.hasInstance)
		{
			EquipmentInteractor.instance.ForceDropEquipment(this);
		}
	}

	// Token: 0x06002108 RID: 8456
	public abstract void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x06002109 RID: 8457
	public abstract void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x0600210A RID: 8458
	public abstract void DropItemCleanup();

	// Token: 0x0600210B RID: 8459 RVA: 0x000B092C File Offset: 0x000AEB2C
	public virtual bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x0600210D RID: 8461 RVA: 0x0000636B File Offset: 0x0000456B
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0600210E RID: 8462 RVA: 0x00014807 File Offset: 0x00012A07
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x0600210F RID: 8463 RVA: 0x0001480F File Offset: 0x00012A0F
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}
}
