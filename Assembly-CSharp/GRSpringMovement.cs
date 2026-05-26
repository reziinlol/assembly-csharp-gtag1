using System;
using UnityEngine;

// Token: 0x020007E6 RID: 2022
public class GRSpringMovement
{
	// Token: 0x060033AF RID: 13231 RVA: 0x0011CAEC File Offset: 0x0011ACEC
	public GRSpringMovement(float _tension, float _dampening)
	{
		this.tension = _tension;
		this.dampening = _dampening;
	}

	// Token: 0x060033B0 RID: 13232 RVA: 0x0011CB1F File Offset: 0x0011AD1F
	public void Reset()
	{
		this.pos = 0f;
		this.target = 0f;
		this.speed = 0f;
		this.wasAlreadyAtTargetLastUpdate = false;
	}

	// Token: 0x060033B1 RID: 13233 RVA: 0x0011CB49 File Offset: 0x0011AD49
	public void SetHardStopAtTarget(bool _hardStopAtTarget)
	{
		if (this.hardStopAtTarget == _hardStopAtTarget)
		{
			return;
		}
		this.hardStopAtTarget = _hardStopAtTarget;
		this.speed = 0f;
	}

	// Token: 0x060033B2 RID: 13234 RVA: 0x0011CB68 File Offset: 0x0011AD68
	public void Update()
	{
		this.wasAlreadyAtTargetLastUpdate = (this.pos == this.target && this.speed == 0f);
		float num = this.pos;
		float num2 = 0.001f;
		float num3 = Mathf.Min(Time.deltaTime, 0.05f);
		float num4 = 6.2832f / this.tension;
		float num5 = num4 * num4 * (this.target - this.pos) - 2f * this.dampening * num4 * this.speed;
		this.speed += num5 * num3;
		this.pos += this.speed * num3;
		if (this.hardStopAtTarget)
		{
			if ((num <= this.pos && this.pos + num2 >= this.target) || (num >= this.pos && this.pos - num2 <= this.target))
			{
				this.speed = 0f;
				this.pos = this.target;
				return;
			}
		}
		else if (Mathf.Abs(num - this.target) < num2 && Mathf.Abs(this.speed) < num2)
		{
			this.speed = 0f;
			this.pos = this.target;
		}
	}

	// Token: 0x060033B3 RID: 13235 RVA: 0x0011CC99 File Offset: 0x0011AE99
	public bool HitTargetLastUpdate()
	{
		return this.IsAtTarget() && !this.wasAlreadyAtTargetLastUpdate;
	}

	// Token: 0x060033B4 RID: 13236 RVA: 0x0011CCAE File Offset: 0x0011AEAE
	public bool IsAtTarget()
	{
		return this.pos == this.target && this.speed == 0f;
	}

	// Token: 0x04004367 RID: 17255
	public float tension = 1f;

	// Token: 0x04004368 RID: 17256
	public float dampening = 0.7f;

	// Token: 0x04004369 RID: 17257
	public float target;

	// Token: 0x0400436A RID: 17258
	public bool hardStopAtTarget = true;

	// Token: 0x0400436B RID: 17259
	public float pos;

	// Token: 0x0400436C RID: 17260
	public float speed;

	// Token: 0x0400436D RID: 17261
	private bool wasAlreadyAtTargetLastUpdate;
}
