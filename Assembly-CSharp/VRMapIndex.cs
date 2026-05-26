using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004F1 RID: 1265
[Serializable]
public class VRMapIndex : VRMap
{
	// Token: 0x06001FA7 RID: 8103 RVA: 0x000AA93C File Offset: 0x000A8B3C
	public override void Initialize()
	{
		this.closedAngle1Quat = Quaternion.Euler(this.closedAngle1);
		this.closedAngle2Quat = Quaternion.Euler(this.closedAngle2);
		this.closedAngle3Quat = Quaternion.Euler(this.closedAngle3);
		this.startingAngle1Quat = Quaternion.Euler(this.startingAngle1);
		this.startingAngle2Quat = Quaternion.Euler(this.startingAngle2);
		this.startingAngle3Quat = Quaternion.Euler(this.startingAngle3);
	}

	// Token: 0x06001FA8 RID: 8104 RVA: 0x000AA9B0 File Offset: 0x000A8BB0
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		this.triggerValue = ControllerInputPoller.TriggerFloat(this.vrTargetNode);
		this.triggerTouch = ControllerInputPoller.TriggerTouch(this.vrTargetNode);
		this.calcT = 0.1f * this.triggerTouch;
		this.calcT += 0.9f * this.triggerValue;
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x06001FA9 RID: 8105 RVA: 0x000AAA20 File Offset: 0x000A8C20
	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			this.currentAngle1 = Mathf.Lerp(this.currentAngle1, this.calcT, lerpValue);
			this.currentAngle2 = Mathf.Lerp(this.currentAngle2, this.calcT, lerpValue);
			this.currentAngle3 = Mathf.Lerp(this.currentAngle3, this.calcT, lerpValue);
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
			}
			this.myTempInt = (int)(this.currentAngle3 * 10.1f);
			if (this.myTempInt != this.lastAngle3)
			{
				this.lastAngle3 = this.myTempInt;
				this.fingerBone3.localRotation = this.angle3Table[this.lastAngle3];
				return;
			}
		}
		else
		{
			this.fingerBone1.localRotation = Quaternion.Lerp(this.fingerBone1.localRotation, Quaternion.Lerp(this.startingAngle1Quat, this.closedAngle1Quat, this.calcT), lerpValue);
			this.fingerBone2.localRotation = Quaternion.Lerp(this.fingerBone2.localRotation, Quaternion.Lerp(this.startingAngle2Quat, this.closedAngle2Quat, this.calcT), lerpValue);
			this.fingerBone3.localRotation = Quaternion.Lerp(this.fingerBone3.localRotation, Quaternion.Lerp(this.startingAngle3Quat, this.closedAngle3Quat, this.calcT), lerpValue);
		}
	}

	// Token: 0x04002A28 RID: 10792
	public InputFeatureUsage inputAxis;

	// Token: 0x04002A29 RID: 10793
	public float triggerTouch;

	// Token: 0x04002A2A RID: 10794
	public float triggerValue;

	// Token: 0x04002A2B RID: 10795
	public Transform fingerBone1;

	// Token: 0x04002A2C RID: 10796
	public Transform fingerBone2;

	// Token: 0x04002A2D RID: 10797
	public Transform fingerBone3;

	// Token: 0x04002A2E RID: 10798
	public Vector3 closedAngle1;

	// Token: 0x04002A2F RID: 10799
	public Vector3 closedAngle2;

	// Token: 0x04002A30 RID: 10800
	public Vector3 closedAngle3;

	// Token: 0x04002A31 RID: 10801
	public Vector3 startingAngle1;

	// Token: 0x04002A32 RID: 10802
	public Vector3 startingAngle2;

	// Token: 0x04002A33 RID: 10803
	public Vector3 startingAngle3;

	// Token: 0x04002A34 RID: 10804
	public Quaternion closedAngle1Quat;

	// Token: 0x04002A35 RID: 10805
	public Quaternion closedAngle2Quat;

	// Token: 0x04002A36 RID: 10806
	public Quaternion closedAngle3Quat;

	// Token: 0x04002A37 RID: 10807
	public Quaternion startingAngle1Quat;

	// Token: 0x04002A38 RID: 10808
	public Quaternion startingAngle2Quat;

	// Token: 0x04002A39 RID: 10809
	public Quaternion startingAngle3Quat;

	// Token: 0x04002A3A RID: 10810
	private int lastAngle1;

	// Token: 0x04002A3B RID: 10811
	private int lastAngle2;

	// Token: 0x04002A3C RID: 10812
	private int lastAngle3;

	// Token: 0x04002A3D RID: 10813
	private InputDevice myInputDevice;

	// Token: 0x04002A3E RID: 10814
	public Quaternion[] angle1Table;

	// Token: 0x04002A3F RID: 10815
	public Quaternion[] angle2Table;

	// Token: 0x04002A40 RID: 10816
	public Quaternion[] angle3Table;

	// Token: 0x04002A41 RID: 10817
	private float currentAngle1;

	// Token: 0x04002A42 RID: 10818
	private float currentAngle2;

	// Token: 0x04002A43 RID: 10819
	private float currentAngle3;

	// Token: 0x04002A44 RID: 10820
	private int myTempInt;
}
