using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class PropHuntGrabbableProp : HoldableObject
{
	// Token: 0x060010FB RID: 4347 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x060010FC RID: 4348 RVA: 0x0005AB7C File Offset: 0x00058D7C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		this.handFollower.SwitchHand(flag);
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x0005ABB8 File Offset: 0x00058DB8
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x04001430 RID: 5168
	public PropHuntHandFollower handFollower;

	// Token: 0x04001431 RID: 5169
	public Vector3 offset;

	// Token: 0x04001432 RID: 5170
	public List<InteractionPoint> interactionPoints;
}
