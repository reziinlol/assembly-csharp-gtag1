using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020008E6 RID: 2278
public class FreeHoverboardHandle : HoldableObject
{
	// Token: 0x06003B98 RID: 15256 RVA: 0x00146788 File Offset: 0x00144988
	private void Awake()
	{
		this.hasParentBoard = (this.parentFreeBoard != null);
	}

	// Token: 0x06003B99 RID: 15257 RVA: 0x0014679C File Offset: 0x0014499C
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		if (Time.frameCount > this.noHapticsUntilFrame)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.noHapticsUntilFrame = Time.frameCount + 1;
	}

	// Token: 0x06003B9A RID: 15258 RVA: 0x0014680C File Offset: 0x00144A0C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if (this.hasParentBoard)
		{
			FreeHoverboardManager.instance.SendGrabBoardRPC(this.parentFreeBoard);
			Transform transform = flag ? VRRig.LocalRig.leftHand.rigTarget : VRRig.LocalRig.rightHand.rigTarget;
			Quaternion rot = transform.InverseTransformRotation(base.transform.rotation);
			Vector3 pos = transform.InverseTransformPoint(base.transform.position);
			GTPlayer.Instance.GrabPersonalHoverboard(flag, pos, rot, this.parentFreeBoard.boardColor);
			return;
		}
		Quaternion rot2 = flag ? this.defaultHoldAngleLeft : this.defaultHoldAngleRight;
		Vector3 pos2 = flag ? this.defaultHoldPosLeft : this.defaultHoldPosRight;
		GTPlayer.Instance.GrabPersonalHoverboard(flag, pos2, rot2, VRRig.LocalRig.playerColor);
	}

	// Token: 0x06003B9B RID: 15259 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06003B9C RID: 15260 RVA: 0x00002AF8 File Offset: 0x00000CF8
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		throw new NotImplementedException();
	}

	// Token: 0x04004C2D RID: 19501
	[SerializeField]
	private FreeHoverboardInstance parentFreeBoard;

	// Token: 0x04004C2E RID: 19502
	private bool hasParentBoard;

	// Token: 0x04004C2F RID: 19503
	[SerializeField]
	private Vector3 defaultHoldPosLeft;

	// Token: 0x04004C30 RID: 19504
	[SerializeField]
	private Vector3 defaultHoldPosRight;

	// Token: 0x04004C31 RID: 19505
	[SerializeField]
	private Quaternion defaultHoldAngleLeft;

	// Token: 0x04004C32 RID: 19506
	[SerializeField]
	private Quaternion defaultHoldAngleRight;

	// Token: 0x04004C33 RID: 19507
	private int noHapticsUntilFrame = -1;
}
