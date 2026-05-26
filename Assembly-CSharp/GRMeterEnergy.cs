using System;
using UnityEngine;

// Token: 0x020007AF RID: 1967
public class GRMeterEnergy : MonoBehaviour
{
	// Token: 0x06003225 RID: 12837 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void Awake()
	{
	}

	// Token: 0x06003226 RID: 12838 RVA: 0x00113720 File Offset: 0x00111920
	public void Refresh()
	{
		float num = 0f;
		if (this.tool != null && this.tool.GetEnergyMax() > 0)
		{
			num = (float)this.tool.energy / (float)this.tool.GetEnergyMax();
		}
		num = Mathf.Clamp(num, 0f, 1f);
		GRMeterEnergy.MeterType meterType = this.meterType;
		if (meterType == GRMeterEnergy.MeterType.Linear || meterType != GRMeterEnergy.MeterType.Radial)
		{
			this.meter.localScale = new Vector3(1f, num, 1f);
			return;
		}
		float value = Mathf.Lerp(this.angularRange.x, this.angularRange.y, num);
		Vector3 zero = Vector3.zero;
		zero[this.rotationAxis] = value;
		this.meter.localRotation = Quaternion.Euler(zero);
	}

	// Token: 0x0400411F RID: 16671
	public GRTool tool;

	// Token: 0x04004120 RID: 16672
	public Transform meter;

	// Token: 0x04004121 RID: 16673
	public Transform chargePoint;

	// Token: 0x04004122 RID: 16674
	public GRMeterEnergy.MeterType meterType;

	// Token: 0x04004123 RID: 16675
	public Vector2 angularRange = new Vector2(-45f, 45f);

	// Token: 0x04004124 RID: 16676
	[Range(0f, 2f)]
	public int rotationAxis;

	// Token: 0x020007B0 RID: 1968
	public enum MeterType
	{
		// Token: 0x04004126 RID: 16678
		Linear,
		// Token: 0x04004127 RID: 16679
		Radial
	}
}
