using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000531 RID: 1329
[NetworkBehaviourWeaved(0)]
public abstract class NetworkHoldableObject : NetworkComponent, IHoldableObject
{
	// Token: 0x17000393 RID: 915
	// (get) Token: 0x06002183 RID: 8579 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002184 RID: 8580
	public abstract void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x06002185 RID: 8581
	public abstract void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x06002186 RID: 8582
	public abstract void DropItemCleanup();

	// Token: 0x06002187 RID: 8583 RVA: 0x000B2B2C File Offset: 0x000B0D2C
	public virtual bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x06002188 RID: 8584 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06002189 RID: 8585 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x0600218A RID: 8586 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600218B RID: 8587 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x0000636B File Offset: 0x0000456B
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x00014807 File Offset: 0x00012A07
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x0001480F File Offset: 0x00012A0F
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}

	// Token: 0x06002190 RID: 8592 RVA: 0x00002B07 File Offset: 0x00000D07
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x00002B13 File Offset: 0x00000D13
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
