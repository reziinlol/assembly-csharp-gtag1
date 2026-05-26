using System;
using UnityEngine;

// Token: 0x02000A1F RID: 2591
public class GorillaTurnSlider : MonoBehaviour
{
	// Token: 0x06004245 RID: 16965 RVA: 0x0016216D File Offset: 0x0016036D
	private void Awake()
	{
		this.startingLocation = base.transform.position;
		this.SetPosition(this.gorillaTurn.currentSpeed);
	}

	// Token: 0x06004246 RID: 16966 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void FixedUpdate()
	{
	}

	// Token: 0x06004247 RID: 16967 RVA: 0x00162194 File Offset: 0x00160394
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float x = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(x, this.startingLocation.y, this.startingLocation.z);
	}

	// Token: 0x06004248 RID: 16968 RVA: 0x00162218 File Offset: 0x00160418
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x06004249 RID: 16969 RVA: 0x00162274 File Offset: 0x00160474
	public void OnSliderRelease()
	{
		if (this.zRange != 0f && (base.transform.position - this.startingLocation).magnitude > this.zRange / 2f)
		{
			if (base.transform.position.x > this.startingLocation.x)
			{
				base.transform.position = new Vector3(this.startingLocation.x + this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
				return;
			}
			base.transform.position = new Vector3(this.startingLocation.x - this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
		}
	}

	// Token: 0x04005421 RID: 21537
	public float zRange;

	// Token: 0x04005422 RID: 21538
	public float maxValue;

	// Token: 0x04005423 RID: 21539
	public float minValue;

	// Token: 0x04005424 RID: 21540
	public GorillaTurning gorillaTurn;

	// Token: 0x04005425 RID: 21541
	private float startingZ;

	// Token: 0x04005426 RID: 21542
	public Vector3 startingLocation;
}
