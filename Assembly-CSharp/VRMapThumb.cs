using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004F3 RID: 1267
[Serializable]
public class VRMapThumb : VRMap
{
	// Token: 0x06001FAF RID: 8111 RVA: 0x000AAE7C File Offset: 0x000A907C
	public override void Initialize()
	{
		this.closedAngle1Quat = Quaternion.Euler(this.closedAngle1);
		this.closedAngle2Quat = Quaternion.Euler(this.closedAngle2);
		this.startingAngle1Quat = Quaternion.Euler(this.startingAngle1);
		this.startingAngle2Quat = Quaternion.Euler(this.startingAngle2);
	}

	// Token: 0x06001FB0 RID: 8112 RVA: 0x000AAED0 File Offset: 0x000A90D0
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		if (this.vrTargetNode == XRNode.LeftHand)
		{
			this.primaryButtonPress = ControllerInputPoller.instance.leftControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.leftControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.leftControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.leftControllerSecondaryButtonTouch;
		}
		else
		{
			this.primaryButtonPress = ControllerInputPoller.instance.rightControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.rightControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.rightControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.rightControllerSecondaryButtonTouch;
		}
		if (this.primaryButtonPress || this.secondaryButtonPress)
		{
			this.calcT = 1f;
		}
		else if (this.primaryButtonTouch || this.secondaryButtonTouch)
		{
			this.calcT = 0.1f;
		}
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x06001FB1 RID: 8113 RVA: 0x000AAFC4 File Offset: 0x000A91C4
	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			this.currentAngle1 = Mathf.Lerp(this.currentAngle1, this.calcT, lerpValue);
			this.currentAngle2 = Mathf.Lerp(this.currentAngle2, this.calcT, lerpValue);
			this.myTempInt = (int)(this.currentAngle1 * 10.1f);
			if (this.myTempInt != this.lastAngle1)
			{
				this.lastAngle1 = this.myTempInt;
				this.fingerBone1.localRotation = this.angle1Table[this.lastAngle1];
			}
			this.myTempInt = (int)(this.currentAngle2 * 10.1f);
			if (this.myTempInt != this.lastAngle2)
			{
				this.lastAngle2 = this.myTempInt;
				this.fingerBone2.localRotation = this.angle2Table[this.lastAngle2];
				return;
			}
		}
		else
		{
			this.fingerBone1.localRotation = Quaternion.Lerp(this.fingerBone1.localRotation, Quaternion.Lerp(this.startingAngle1Quat, this.closedAngle1Quat, this.calcT), lerpValue);
			this.fingerBone2.localRotation = Quaternion.Lerp(this.fingerBone2.localRotation, Quaternion.Lerp(this.startingAngle2Quat, this.closedAngle2Quat, this.calcT), lerpValue);
		}
	}

	// Token: 0x04002A61 RID: 10849
	public InputFeatureUsage inputAxis;

	// Token: 0x04002A62 RID: 10850
	public bool primaryButtonTouch;

	// Token: 0x04002A63 RID: 10851
	public bool primaryButtonPress;

	// Token: 0x04002A64 RID: 10852
	public bool secondaryButtonTouch;

	// Token: 0x04002A65 RID: 10853
	public bool secondaryButtonPress;

	// Token: 0x04002A66 RID: 10854
	public Transform fingerBone1;

	// Token: 0x04002A67 RID: 10855
	public Transform fingerBone2;

	// Token: 0x04002A68 RID: 10856
	public Vector3 closedAngle1;

	// Token: 0x04002A69 RID: 10857
	public Vector3 closedAngle2;

	// Token: 0x04002A6A RID: 10858
	public Vector3 startingAngle1;

	// Token: 0x04002A6B RID: 10859
	public Vector3 startingAngle2;

	// Token: 0x04002A6C RID: 10860
	public Quaternion closedAngle1Quat;

	// Token: 0x04002A6D RID: 10861
	public Quaternion closedAngle2Quat;

	// Token: 0x04002A6E RID: 10862
	public Quaternion startingAngle1Quat;

	// Token: 0x04002A6F RID: 10863
	public Quaternion startingAngle2Quat;

	// Token: 0x04002A70 RID: 10864
	public Quaternion[] angle1Table;

	// Token: 0x04002A71 RID: 10865
	public Quaternion[] angle2Table;

	// Token: 0x04002A72 RID: 10866
	private float currentAngle1;

	// Token: 0x04002A73 RID: 10867
	private float currentAngle2;

	// Token: 0x04002A74 RID: 10868
	private int lastAngle1;

	// Token: 0x04002A75 RID: 10869
	private int lastAngle2;

	// Token: 0x04002A76 RID: 10870
	private InputDevice tempDevice;

	// Token: 0x04002A77 RID: 10871
	private int myTempInt;
}
