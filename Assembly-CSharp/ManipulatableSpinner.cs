using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000530 RID: 1328
[RequireComponent(typeof(BezierSpline))]
public class ManipulatableSpinner : ManipulatableObject
{
	// Token: 0x17000392 RID: 914
	// (get) Token: 0x06002177 RID: 8567 RVA: 0x000B286D File Offset: 0x000B0A6D
	// (set) Token: 0x06002178 RID: 8568 RVA: 0x000B2875 File Offset: 0x000B0A75
	public float angle { get; private set; }

	// Token: 0x06002179 RID: 8569 RVA: 0x000B287E File Offset: 0x000B0A7E
	private void Awake()
	{
		this.spline = base.GetComponent<BezierSpline>();
	}

	// Token: 0x0600217A RID: 8570 RVA: 0x000B288C File Offset: 0x000B0A8C
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
		Vector3 position = grabbingHand.transform.position;
		float num = this.FindPositionOnSpline(position);
		this.previousHandT = num;
	}

	// Token: 0x0600217B RID: 8571 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
	}

	// Token: 0x0600217C RID: 8572 RVA: 0x000B28B4 File Offset: 0x000B0AB4
	protected override bool ShouldHandDetach(GameObject hand)
	{
		if (!this.spline.Loop && (this.currentHandT >= 0.99f || this.currentHandT <= 0.01f))
		{
			return true;
		}
		Vector3 position = hand.transform.position;
		Vector3 point = this.spline.GetPoint(this.currentHandT);
		return Vector3.SqrMagnitude(position - point) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x0600217D RID: 8573 RVA: 0x000B2924 File Offset: 0x000B0B24
	protected override void OnHeldUpdate(GameObject hand)
	{
		float angle = this.angle;
		Vector3 position = hand.transform.position;
		this.currentHandT = this.FindPositionOnSpline(position);
		float num = this.currentHandT - this.previousHandT;
		if (this.spline.Loop)
		{
			if (num > 0.5f)
			{
				num -= 1f;
			}
			else if (num < -0.5f)
			{
				num += 1f;
			}
		}
		this.angle += num;
		this.previousHandT = this.currentHandT;
		if (this.applyReleaseVelocity && this.currentHandT <= 0.99f && this.currentHandT >= 0.01f)
		{
			this.tVelocity = (this.angle - angle) / Time.deltaTime;
		}
	}

	// Token: 0x0600217E RID: 8574 RVA: 0x000B29E0 File Offset: 0x000B0BE0
	protected override void OnReleasedUpdate()
	{
		if (this.tVelocity != 0f)
		{
			this.angle += this.tVelocity * Time.deltaTime;
			if (Mathf.Abs(this.tVelocity) < this.lowSpeedThreshold)
			{
				this.tVelocity *= 1f - this.lowSpeedDrag * Time.deltaTime;
				return;
			}
			this.tVelocity *= 1f - this.releaseDrag * Time.deltaTime;
		}
	}

	// Token: 0x0600217F RID: 8575 RVA: 0x000B2A68 File Offset: 0x000B0C68
	private float FindPositionOnSpline(Vector3 grabPoint)
	{
		int i = 0;
		int num = 200;
		float num2 = 0.001f;
		float num3 = 1f / (float)num;
		float3 y = base.transform.InverseTransformPoint(grabPoint);
		float result = 0f;
		float num4 = float.PositiveInfinity;
		while (i < num)
		{
			float num5 = math.distancesq(this.spline.GetPointLocal(num2), y);
			if (num5 < num4)
			{
				num4 = num5;
				result = num2;
			}
			num2 += num3;
			i++;
		}
		return result;
	}

	// Token: 0x06002180 RID: 8576 RVA: 0x000B2AE4 File Offset: 0x000B0CE4
	public void SetAngle(float newAngle)
	{
		this.angle = newAngle;
	}

	// Token: 0x06002181 RID: 8577 RVA: 0x000B2AED File Offset: 0x000B0CED
	public void SetVelocity(float newVelocity)
	{
		this.tVelocity = newVelocity;
	}

	// Token: 0x04002C32 RID: 11314
	public float breakDistance = 0.2f;

	// Token: 0x04002C33 RID: 11315
	public bool applyReleaseVelocity;

	// Token: 0x04002C34 RID: 11316
	public float releaseDrag = 1f;

	// Token: 0x04002C35 RID: 11317
	public float lowSpeedThreshold = 0.12f;

	// Token: 0x04002C36 RID: 11318
	public float lowSpeedDrag = 3f;

	// Token: 0x04002C37 RID: 11319
	private BezierSpline spline;

	// Token: 0x04002C38 RID: 11320
	private float previousHandT;

	// Token: 0x04002C39 RID: 11321
	private float currentHandT;

	// Token: 0x04002C3A RID: 11322
	private float tVelocity;
}
