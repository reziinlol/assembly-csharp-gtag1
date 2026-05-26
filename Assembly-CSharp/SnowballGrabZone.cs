using System;
using UnityEngine;

// Token: 0x02000203 RID: 515
public class SnowballGrabZone : HoldableObject
{
	// Token: 0x06000D87 RID: 3463 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x0004A1C0 File Offset: 0x000483C0
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if (flag ? EquipmentInteractor.instance.disableLeftGrab : EquipmentInteractor.instance.disableRightGrab)
		{
			return;
		}
		SnowballThrowable snowballThrowable;
		(flag ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance).TryCreateSnowball(this.materialIndex, out snowballThrowable);
	}

	// Token: 0x0400102A RID: 4138
	[GorillaSoundLookup]
	public int materialIndex;
}
