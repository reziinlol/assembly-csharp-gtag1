using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200052B RID: 1323
public class ManipulatableObject : HoldableObject
{
	// Token: 0x0600214E RID: 8526 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x0600214F RID: 8527 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
	}

	// Token: 0x06002150 RID: 8528 RVA: 0x00002076 File Offset: 0x00000276
	protected virtual bool ShouldHandDetach(GameObject hand)
	{
		return false;
	}

	// Token: 0x06002151 RID: 8529 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnHeldUpdate(GameObject hand)
	{
	}

	// Token: 0x06002152 RID: 8530 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnReleasedUpdate()
	{
	}

	// Token: 0x06002153 RID: 8531 RVA: 0x000B1BD8 File Offset: 0x000AFDD8
	public virtual void LateUpdate()
	{
		if (this.isHeld)
		{
			if (this.holdingHand == null)
			{
				EquipmentInteractor.instance.ForceDropManipulatableObject(this);
				return;
			}
			this.OnHeldUpdate(this.holdingHand);
			if (this.ShouldHandDetach(this.holdingHand))
			{
				EquipmentInteractor.instance.ForceDropManipulatableObject(this);
				return;
			}
		}
		else
		{
			this.OnReleasedUpdate();
		}
	}

	// Token: 0x06002154 RID: 8532 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06002155 RID: 8533 RVA: 0x000B1C38 File Offset: 0x000AFE38
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool forLeftHand = grabbingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, forLeftHand);
		this.isHeld = true;
		this.holdingHand = grabbingHand;
		this.OnStartManipulation(this.holdingHand);
	}

	// Token: 0x06002156 RID: 8534 RVA: 0x000B1C80 File Offset: 0x000AFE80
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		Vector3 averageVelocity = GTPlayer.Instance.GetHandVelocityTracker(flag).GetAverageVelocity(true, 0.15f, false);
		if (flag)
		{
			EquipmentInteractor.instance.leftHandHeldEquipment = null;
		}
		else
		{
			EquipmentInteractor.instance.rightHandHeldEquipment = null;
		}
		this.isHeld = false;
		this.holdingHand = null;
		this.OnStopManipulation(releasingHand, averageVelocity);
		return true;
	}

	// Token: 0x06002157 RID: 8535 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x04002C0A RID: 11274
	protected bool isHeld;

	// Token: 0x04002C0B RID: 11275
	protected GameObject holdingHand;
}
