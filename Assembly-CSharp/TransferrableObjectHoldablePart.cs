using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004CB RID: 1227
public class TransferrableObjectHoldablePart : HoldableObject, ITickSystemTick
{
	// Token: 0x17000322 RID: 802
	// (get) Token: 0x06001DCD RID: 7629 RVA: 0x000A088F File Offset: 0x0009EA8F
	// (set) Token: 0x06001DCE RID: 7630 RVA: 0x000A0897 File Offset: 0x0009EA97
	public bool TickRunning { get; set; }

	// Token: 0x06001DCF RID: 7631 RVA: 0x00019E3F File Offset: 0x0001803F
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06001DD0 RID: 7632 RVA: 0x00019E47 File Offset: 0x00018047
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06001DD1 RID: 7633 RVA: 0x000A08A0 File Offset: 0x0009EAA0
	public void Tick()
	{
		VRRig rig;
		if (!this.transferrableParentObject.IsLocalObject())
		{
			rig = this.transferrableParentObject.myOnlineRig;
			this.isHeld = ((this.transferrableParentObject.itemState & this.heldBit) > (TransferrableObject.ItemStates)0);
			TransferrableObject.PositionState currentState = this.transferrableParentObject.currentState;
			if (currentState == TransferrableObject.PositionState.OnRightArm || currentState == TransferrableObject.PositionState.InRightHand)
			{
				this.isHeldLeftHand = this.isHeld;
			}
			else
			{
				this.isHeldLeftHand = false;
			}
		}
		else
		{
			rig = VRRig.LocalRig;
		}
		if (this.isHeld)
		{
			if (this.transferrableParentObject.InHand())
			{
				this.UpdateHeld(rig, this.isHeldLeftHand);
				return;
			}
			if (this.transferrableParentObject.IsLocalObject())
			{
				this.OnRelease(null, this.isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
			}
		}
	}

	// Token: 0x06001DD2 RID: 7634 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
	}

	// Token: 0x06001DD3 RID: 7635 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06001DD4 RID: 7636 RVA: 0x000A096C File Offset: 0x0009EB6C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (this.transferrableParentObject.ownerRig && !this.transferrableParentObject.ownerRig.isLocal)
		{
			return;
		}
		this.isHeld = true;
		this.isHeldLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
		this.transferrableParentObject.itemState |= this.heldBit;
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		UnityEvent unityEvent = this.onGrab;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06001DD5 RID: 7637 RVA: 0x000A09F8 File Offset: 0x0009EBF8
	public override void DropItemCleanup()
	{
		this.isHeld = false;
		this.isHeldLeftHand = false;
		this.transferrableParentObject.itemState &= ~this.heldBit;
	}

	// Token: 0x06001DD6 RID: 7638 RVA: 0x000A0A24 File Offset: 0x0009EC24
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
		{
			return false;
		}
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
		{
			return false;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
		this.isHeld = false;
		this.isHeldLeftHand = false;
		this.transferrableParentObject.itemState &= ~this.heldBit;
		UnityEvent unityEvent = this.onRelease;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		return true;
	}

	// Token: 0x0400281A RID: 10266
	[SerializeField]
	protected TransferrableObject transferrableParentObject;

	// Token: 0x0400281B RID: 10267
	[SerializeField]
	private TransferrableObject.ItemStates heldBit = TransferrableObject.ItemStates.Part0Held;

	// Token: 0x0400281C RID: 10268
	private bool isHeld;

	// Token: 0x0400281D RID: 10269
	protected bool isHeldLeftHand;

	// Token: 0x0400281E RID: 10270
	public UnityEvent onGrab;

	// Token: 0x0400281F RID: 10271
	public UnityEvent onRelease;

	// Token: 0x04002820 RID: 10272
	public UnityEvent onDrop;
}
