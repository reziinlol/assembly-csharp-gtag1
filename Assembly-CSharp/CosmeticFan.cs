using System;
using UnityEngine;

// Token: 0x020002B1 RID: 689
public class CosmeticFan : MonoBehaviour
{
	// Token: 0x060011D6 RID: 4566 RVA: 0x0005FAE7 File Offset: 0x0005DCE7
	private void Start()
	{
		this.spinUpRate = this.maxSpeed / this.spinUpDuration;
		this.spinDownRate = this.maxSpeed / this.spinDownDuration;
	}

	// Token: 0x060011D7 RID: 4567 RVA: 0x0005FB10 File Offset: 0x0005DD10
	public void Run()
	{
		this.targetSpeed = this.maxSpeed;
		if (this.spinUpDuration > 0f)
		{
			base.enabled = true;
			this.currentAccelRate = this.spinUpRate;
		}
		else
		{
			this.currentSpeed = this.maxSpeed;
		}
		base.enabled = true;
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x0005FB5E File Offset: 0x0005DD5E
	public void Stop()
	{
		this.targetSpeed = 0f;
		if (this.spinDownDuration > 0f)
		{
			base.enabled = true;
			this.currentAccelRate = this.spinDownRate;
			return;
		}
		this.currentSpeed = 0f;
	}

	// Token: 0x060011D9 RID: 4569 RVA: 0x0005FB97 File Offset: 0x0005DD97
	public void InstantStop()
	{
		this.targetSpeed = 0f;
		this.currentSpeed = 0f;
		base.enabled = false;
	}

	// Token: 0x060011DA RID: 4570 RVA: 0x0005FBB8 File Offset: 0x0005DDB8
	private void Update()
	{
		this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.targetSpeed, this.currentAccelRate * Time.deltaTime);
		base.transform.localRotation = base.transform.localRotation * Quaternion.AngleAxis(this.currentSpeed * Time.deltaTime, this.axis);
		if (this.currentSpeed == 0f && this.targetSpeed == 0f)
		{
			base.enabled = false;
		}
	}

	// Token: 0x0400157A RID: 5498
	[SerializeField]
	private Vector3 axis;

	// Token: 0x0400157B RID: 5499
	[SerializeField]
	private float spinUpDuration = 0.3f;

	// Token: 0x0400157C RID: 5500
	[SerializeField]
	private float spinDownDuration = 0.3f;

	// Token: 0x0400157D RID: 5501
	[SerializeField]
	private float maxSpeed = 360f;

	// Token: 0x0400157E RID: 5502
	private float currentSpeed;

	// Token: 0x0400157F RID: 5503
	private float targetSpeed;

	// Token: 0x04001580 RID: 5504
	private float currentAccelRate;

	// Token: 0x04001581 RID: 5505
	private float spinUpRate;

	// Token: 0x04001582 RID: 5506
	private float spinDownRate;
}
