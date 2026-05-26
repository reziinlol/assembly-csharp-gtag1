using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004F2 RID: 1266
[Serializable]
public class VRMapMiddle : VRMap
{
	// Token: 0x06001FAB RID: 8107 RVA: 0x000AABFC File Offset: 0x000A8DFC
	public override void Initialize()
	{
		this.closedAngle1Quat = Quaternion.Euler(this.closedAngle1);
		this.closedAngle2Quat = Quaternion.Euler(this.closedAngle2);
		this.closedAngle3Quat = Quaternion.Euler(this.closedAngle3);
		this.startingAngle1Quat = Quaternion.Euler(this.startingAngle1);
		this.startingAngle2Quat = Quaternion.Euler(this.startingAngle2);
		this.startingAngle3Quat = Quaternion.Euler(this.startingAngle3);
	}

	// Token: 0x06001FAC RID: 8108 RVA: 0x000AAC6F File Offset: 0x000A8E6F
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		this.gripValue = ControllerInputPoller.GripFloat(this.vrTargetNode);
		this.calcT = 1f * this.gripValue;
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x06001FAD RID: 8109 RVA: 0x000AACA8 File Offset: 0x000A8EA8
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

	// Token: 0x04002A45 RID: 10821
	public InputFeatureUsage inputAxis;

	// Token: 0x04002A46 RID: 10822
	public float gripValue;

	// Token: 0x04002A47 RID: 10823
	public Transform fingerBone1;

	// Token: 0x04002A48 RID: 10824
	public Transform fingerBone2;

	// Token: 0x04002A49 RID: 10825
	public Transform fingerBone3;

	// Token: 0x04002A4A RID: 10826
	public Vector3 closedAngle1;

	// Token: 0x04002A4B RID: 10827
	public Vector3 closedAngle2;

	// Token: 0x04002A4C RID: 10828
	public Vector3 closedAngle3;

	// Token: 0x04002A4D RID: 10829
	public Vector3 startingAngle1;

	// Token: 0x04002A4E RID: 10830
	public Vector3 startingAngle2;

	// Token: 0x04002A4F RID: 10831
	public Vector3 startingAngle3;

	// Token: 0x04002A50 RID: 10832
	public Quaternion closedAngle1Quat;

	// Token: 0x04002A51 RID: 10833
	public Quaternion closedAngle2Quat;

	// Token: 0x04002A52 RID: 10834
	public Quaternion closedAngle3Quat;

	// Token: 0x04002A53 RID: 10835
	public Quaternion startingAngle1Quat;

	// Token: 0x04002A54 RID: 10836
	public Quaternion startingAngle2Quat;

	// Token: 0x04002A55 RID: 10837
	public Quaternion startingAngle3Quat;

	// Token: 0x04002A56 RID: 10838
	public Quaternion[] angle1Table;

	// Token: 0x04002A57 RID: 10839
	public Quaternion[] angle2Table;

	// Token: 0x04002A58 RID: 10840
	public Quaternion[] angle3Table;

	// Token: 0x04002A59 RID: 10841
	private int lastAngle1;

	// Token: 0x04002A5A RID: 10842
	private int lastAngle2;

	// Token: 0x04002A5B RID: 10843
	private int lastAngle3;

	// Token: 0x04002A5C RID: 10844
	private float currentAngle1;

	// Token: 0x04002A5D RID: 10845
	private float currentAngle2;

	// Token: 0x04002A5E RID: 10846
	private float currentAngle3;

	// Token: 0x04002A5F RID: 10847
	private InputDevice tempDevice;

	// Token: 0x04002A60 RID: 10848
	private int myTempInt;
}
