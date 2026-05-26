using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020005E6 RID: 1510
public class AutomaticAdjustIPD : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002586 RID: 9606 RVA: 0x00018E08 File Offset: 0x00017008
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002587 RID: 9607 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002588 RID: 9608 RVA: 0x000C6A7C File Offset: 0x000C4C7C
	public void SliceUpdate()
	{
		if (!this.headset.isValid)
		{
			this.headset = InputDevices.GetDeviceAtXRNode(XRNode.Head);
		}
		if (this.headset.isValid && this.headset.TryGetFeatureValue(CommonUsages.leftEyePosition, out this.leftEyePosition) && this.headset.TryGetFeatureValue(CommonUsages.rightEyePosition, out this.rightEyePosition))
		{
			this.currentIPD = (this.rightEyePosition - this.leftEyePosition).magnitude;
			if (Mathf.Abs(this.lastIPD - this.currentIPD) < 0.01f)
			{
				return;
			}
			this.lastIPD = this.currentIPD;
			for (int i = 0; i < this.adjustXScaleObjects.Length; i++)
			{
				Transform transform = this.adjustXScaleObjects[i];
				if (!transform)
				{
					return;
				}
				transform.localScale = new Vector3(Mathf.LerpUnclamped(1f, 1.12f, (this.currentIPD - 0.058f) / 0.0050000027f), 1f, 1f);
			}
		}
	}

	// Token: 0x040030FA RID: 12538
	public InputDevice headset;

	// Token: 0x040030FB RID: 12539
	public float currentIPD;

	// Token: 0x040030FC RID: 12540
	public Vector3 leftEyePosition;

	// Token: 0x040030FD RID: 12541
	public Vector3 rightEyePosition;

	// Token: 0x040030FE RID: 12542
	public bool testOverride;

	// Token: 0x040030FF RID: 12543
	public Transform[] adjustXScaleObjects;

	// Token: 0x04003100 RID: 12544
	public float sizeAt58mm = 1f;

	// Token: 0x04003101 RID: 12545
	public float sizeAt63mm = 1.12f;

	// Token: 0x04003102 RID: 12546
	public float lastIPD;
}
