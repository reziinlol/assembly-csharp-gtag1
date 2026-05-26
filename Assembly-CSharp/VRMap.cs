using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004F0 RID: 1264
[Serializable]
public class VRMap
{
	// Token: 0x1700035F RID: 863
	// (get) Token: 0x06001F9D RID: 8093 RVA: 0x000AA691 File Offset: 0x000A8891
	// (set) Token: 0x06001F9E RID: 8094 RVA: 0x000AA69E File Offset: 0x000A889E
	public Vector3 syncPos
	{
		get
		{
			return this.netSyncPos.CurrentSyncTarget;
		}
		set
		{
			this.netSyncPos.SetNewSyncTarget(value);
		}
	}

	// Token: 0x06001F9F RID: 8095 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void Initialize()
	{
	}

	// Token: 0x06001FA0 RID: 8096 RVA: 0x000AA6AC File Offset: 0x000A88AC
	public void MapOther(float lerpValue)
	{
		Vector3 a;
		Quaternion a2;
		this.rigTarget.GetLocalPositionAndRotation(out a, out a2);
		this.rigTarget.SetLocalPositionAndRotation(Vector3.Lerp(a, this.syncPos, lerpValue), Quaternion.Lerp(a2, this.syncRotation, lerpValue));
	}

	// Token: 0x06001FA1 RID: 8097 RVA: 0x000AA6F0 File Offset: 0x000A88F0
	public void MapMine(float ratio, Transform playerOffsetTransform)
	{
		Vector3 current;
		Quaternion rotation;
		this.rigTarget.GetPositionAndRotation(out current, out rotation);
		if (this.overrideTarget != null)
		{
			Vector3 a;
			Quaternion lhs;
			this.overrideTarget.GetPositionAndRotation(out a, out lhs);
			this.rigTarget.SetPositionAndRotation(a + rotation * this.trackingPositionOffset * ratio, lhs * Quaternion.Euler(this.trackingRotationOffset));
		}
		else
		{
			if (!this.hasInputDevice && ConnectedControllerHandler.Instance.GetValidForXRNode(this.vrTargetNode))
			{
				this.myInputDevice = InputDevices.GetDeviceAtXRNode(this.vrTargetNode);
				this.hasInputDevice = true;
				if (this.vrTargetNode != XRNode.LeftHand && this.vrTargetNode != XRNode.RightHand)
				{
					this.hasInputDevice = this.myInputDevice.isValid;
				}
			}
			Quaternion rhs;
			Vector3 a2;
			if (this.hasInputDevice && this.myInputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out rhs) && this.myInputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out a2))
			{
				Quaternion lhs2 = Quaternion.identity;
				Transform parent = playerOffsetTransform.parent;
				if (parent.IsNotNull())
				{
					lhs2 = parent.rotation;
				}
				this.rigTarget.SetPositionAndRotation(a2 + rotation * this.trackingPositionOffset * ratio + playerOffsetTransform.position, lhs2 * rhs * Quaternion.Euler(this.trackingRotationOffset));
				this.rigTarget.RotateAround(playerOffsetTransform.position, playerOffsetTransform.up, playerOffsetTransform.localEulerAngles.y);
			}
		}
		if (this.handholdOverrideTarget != null)
		{
			this.rigTarget.position = Vector3.MoveTowards(current, this.handholdOverrideTarget.position - this.handholdOverrideTargetOffset + rotation * this.trackingPositionOffset * ratio, Time.deltaTime * 2f);
		}
	}

	// Token: 0x06001FA2 RID: 8098 RVA: 0x000AA8D4 File Offset: 0x000A8AD4
	public Vector3 GetExtrapolatedControllerPosition()
	{
		Vector3 a;
		Quaternion rotation;
		this.rigTarget.GetPositionAndRotation(out a, out rotation);
		return a - rotation * this.trackingPositionOffset * this.rigTarget.lossyScale.x;
	}

	// Token: 0x06001FA3 RID: 8099 RVA: 0x000AA917 File Offset: 0x000A8B17
	public virtual void MapOtherFinger(float handSync, float lerpValue)
	{
		this.calcT = handSync;
		this.LerpFinger(lerpValue, true);
	}

	// Token: 0x06001FA4 RID: 8100 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void MapMyFinger(float lerpValue)
	{
	}

	// Token: 0x06001FA5 RID: 8101 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void LerpFinger(float lerpValue, bool isOther)
	{
	}

	// Token: 0x04002A1C RID: 10780
	public XRNode vrTargetNode;

	// Token: 0x04002A1D RID: 10781
	public Transform overrideTarget;

	// Token: 0x04002A1E RID: 10782
	public Transform rigTarget;

	// Token: 0x04002A1F RID: 10783
	public Vector3 trackingPositionOffset;

	// Token: 0x04002A20 RID: 10784
	public Vector3 trackingRotationOffset;

	// Token: 0x04002A21 RID: 10785
	internal NetworkVector3 netSyncPos = new NetworkVector3();

	// Token: 0x04002A22 RID: 10786
	public Quaternion syncRotation;

	// Token: 0x04002A23 RID: 10787
	public float calcT;

	// Token: 0x04002A24 RID: 10788
	private InputDevice myInputDevice;

	// Token: 0x04002A25 RID: 10789
	private bool hasInputDevice;

	// Token: 0x04002A26 RID: 10790
	public Transform handholdOverrideTarget;

	// Token: 0x04002A27 RID: 10791
	public Vector3 handholdOverrideTargetOffset;
}
