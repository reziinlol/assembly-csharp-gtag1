using System;
using UnityEngine;

// Token: 0x020007DD RID: 2013
public class GRShiftSirenLight : MonoBehaviourTick
{
	// Token: 0x06003369 RID: 13161 RVA: 0x0011B410 File Offset: 0x00119610
	public override void Tick()
	{
		if (this.shiftManager == null)
		{
			this.shiftManager = GhostReactor.instance.shiftManager;
			return;
		}
		if (this.redLight.activeSelf != this.shiftManager.ShiftActive)
		{
			this.redLight.SetActive(this.shiftManager.ShiftActive);
		}
		if (this.greenLight.activeSelf == this.shiftManager.ShiftActive)
		{
			this.greenLight.SetActive(!this.shiftManager.ShiftActive);
		}
		if (this.readyRoomLight != null)
		{
			this.readyRoomLight.intensity = (this.shiftManager.ShiftActive ? this.dimLight : this.brightLight);
		}
		if (this.shiftManager.ShiftActive)
		{
			this.redLightParent.localEulerAngles = new Vector3(0f, Time.time * this.rotationRate, 0f);
			return;
		}
		this.greenLightParent.localEulerAngles = new Vector3(0f, Time.time * this.rotationRate, 0f);
	}

	// Token: 0x04004318 RID: 17176
	public float rotationRate = 1.25f;

	// Token: 0x04004319 RID: 17177
	public Transform greenLightParent;

	// Token: 0x0400431A RID: 17178
	public Transform redLightParent;

	// Token: 0x0400431B RID: 17179
	public GameObject redLight;

	// Token: 0x0400431C RID: 17180
	public GameObject greenLight;

	// Token: 0x0400431D RID: 17181
	public GhostReactorShiftManager shiftManager;

	// Token: 0x0400431E RID: 17182
	public float dimLight;

	// Token: 0x0400431F RID: 17183
	public float brightLight;

	// Token: 0x04004320 RID: 17184
	public Light readyRoomLight;
}
