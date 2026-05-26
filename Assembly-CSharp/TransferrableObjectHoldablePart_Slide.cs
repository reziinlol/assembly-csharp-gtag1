using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020004CE RID: 1230
public class TransferrableObjectHoldablePart_Slide : TransferrableObjectHoldablePart
{
	// Token: 0x06001DDE RID: 7646 RVA: 0x000A0E7C File Offset: 0x0009F07C
	protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
		int num = isHeldLeftHand ? 0 : 1;
		GTPlayer instance = GTPlayer.Instance;
		if (!rig.isOfflineVRRig)
		{
			Vector3 b = instance.GetHandOffset(isHeldLeftHand) * rig.scaleFactor;
			VRMap vrmap = isHeldLeftHand ? rig.leftHand : rig.rightHand;
			this._snapToLine.target.position = vrmap.GetExtrapolatedControllerPosition() - b;
			return;
		}
		Transform controllerTransform = instance.GetControllerTransform(num == 0);
		Vector3 position = controllerTransform.position;
		Vector3 snappedPoint = this._snapToLine.GetSnappedPoint(position);
		if (this._maxHandSnapDistance > 0f && (controllerTransform.position - snappedPoint).IsLongerThan(this._maxHandSnapDistance))
		{
			this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
			return;
		}
		controllerTransform.position = snappedPoint;
		this._snapToLine.target.position = snappedPoint;
	}

	// Token: 0x04002832 RID: 10290
	[SerializeField]
	private float _maxHandSnapDistance;

	// Token: 0x04002833 RID: 10291
	[SerializeField]
	private SnapXformToLine _snapToLine;

	// Token: 0x04002834 RID: 10292
	private const int LEFT = 0;

	// Token: 0x04002835 RID: 10293
	private const int RIGHT = 1;
}
