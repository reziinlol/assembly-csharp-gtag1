using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x0200050D RID: 1293
public class FriendshipCharm : HoldableObject
{
	// Token: 0x06002045 RID: 8261 RVA: 0x000AD53C File Offset: 0x000AB73C
	private void Awake()
	{
		this.parent = base.transform.parent;
	}

	// Token: 0x06002046 RID: 8262 RVA: 0x000AD550 File Offset: 0x000AB750
	private void LateUpdate()
	{
		if (!this.isBroken && (this.lineStart.transform.position - this.lineEnd.transform.position).IsLongerThan(this.breakBraceletLength * GTPlayer.Instance.scale))
		{
			this.DestroyBracelet();
		}
	}

	// Token: 0x06002047 RID: 8263 RVA: 0x000AD5A8 File Offset: 0x000AB7A8
	public void OnEnable()
	{
		this.interactionPoint.enabled = true;
		this.meshRenderer.enabled = true;
		this.isBroken = false;
		this.UpdatePosition();
	}

	// Token: 0x06002048 RID: 8264 RVA: 0x000AD5CF File Offset: 0x000AB7CF
	private void DestroyBracelet()
	{
		this.interactionPoint.enabled = false;
		this.isBroken = true;
		Debug.Log("LeaveGroup: bracelet destroyed");
		FriendshipGroupDetection.Instance.LeaveParty();
	}

	// Token: 0x06002049 RID: 8265 RVA: 0x000AD5F8 File Offset: 0x000AB7F8
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 2f);
		base.transform.SetParent(flag ? this.leftHandHoldAnchor : this.rightHandHoldAnchor);
		base.transform.localPosition = Vector3.zero;
	}

	// Token: 0x0600204A RID: 8266 RVA: 0x000AD680 File Offset: 0x000AB880
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		bool forLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(null, forLeftHand);
		this.UpdatePosition();
		return base.OnRelease(zoneReleased, releasingHand);
	}

	// Token: 0x0600204B RID: 8267 RVA: 0x000AD6BC File Offset: 0x000AB8BC
	private void UpdatePosition()
	{
		base.transform.SetParent(this.parent);
		base.transform.localPosition = this.releasePosition.localPosition;
		base.transform.localRotation = this.releasePosition.localRotation;
	}

	// Token: 0x0600204C RID: 8268 RVA: 0x000AD6FC File Offset: 0x000AB8FC
	private void OnCollisionEnter(Collision other)
	{
		if (!this.isBroken)
		{
			return;
		}
		if (this.breakItemLayerMask != (this.breakItemLayerMask | 1 << other.gameObject.layer))
		{
			return;
		}
		this.meshRenderer.enabled = false;
		this.UpdatePosition();
	}

	// Token: 0x0600204D RID: 8269 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x0600204E RID: 8270 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x04002B0D RID: 11021
	[SerializeField]
	private InteractionPoint interactionPoint;

	// Token: 0x04002B0E RID: 11022
	[SerializeField]
	private Transform rightHandHoldAnchor;

	// Token: 0x04002B0F RID: 11023
	[SerializeField]
	private Transform leftHandHoldAnchor;

	// Token: 0x04002B10 RID: 11024
	[SerializeField]
	private MeshRenderer meshRenderer;

	// Token: 0x04002B11 RID: 11025
	[SerializeField]
	private Transform lineStart;

	// Token: 0x04002B12 RID: 11026
	[SerializeField]
	private Transform lineEnd;

	// Token: 0x04002B13 RID: 11027
	[SerializeField]
	private Transform releasePosition;

	// Token: 0x04002B14 RID: 11028
	[SerializeField]
	private float breakBraceletLength;

	// Token: 0x04002B15 RID: 11029
	[SerializeField]
	private LayerMask breakItemLayerMask;

	// Token: 0x04002B16 RID: 11030
	private Transform parent;

	// Token: 0x04002B17 RID: 11031
	private bool isBroken;
}
