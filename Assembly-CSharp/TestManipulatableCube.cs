using System;
using UnityEngine;

// Token: 0x0200052C RID: 1324
public class TestManipulatableCube : ManipulatableObject
{
	// Token: 0x06002159 RID: 8537 RVA: 0x000B1CFA File Offset: 0x000AFEFA
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x0600215A RID: 8538 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x0600215B RID: 8539 RVA: 0x000B1D1E File Offset: 0x000AFF1E
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
		if (this.applyReleaseVelocity)
		{
			this.velocity = this.localSpace.MultiplyVector(releaseVelocity);
		}
	}

	// Token: 0x0600215C RID: 8540 RVA: 0x000B1D3C File Offset: 0x000AFF3C
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x0600215D RID: 8541 RVA: 0x000B1D7C File Offset: 0x000AFF7C
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(hand.transform.position);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x000B1E14 File Offset: 0x000B0014
	protected override void OnReleasedUpdate()
	{
		if (this.velocity != Vector3.zero)
		{
			Vector3 vector = this.localSpace.MultiplyPoint(base.transform.position);
			vector += this.velocity * Time.deltaTime;
			if (vector.x < this.minXOffset)
			{
				vector.x = this.minXOffset;
				this.velocity.x = 0f;
			}
			else if (vector.x > this.maxXOffset)
			{
				vector.x = this.maxXOffset;
				this.velocity.x = 0f;
			}
			if (vector.y < this.minYOffset)
			{
				vector.y = this.minYOffset;
				this.velocity.y = 0f;
			}
			else if (vector.y > this.maxYOffset)
			{
				vector.y = this.maxYOffset;
				this.velocity.y = 0f;
			}
			if (vector.z < this.minZOffset)
			{
				vector.z = this.minZOffset;
				this.velocity.z = 0f;
			}
			else if (vector.z > this.maxZOffset)
			{
				vector.z = this.maxZOffset;
				this.velocity.z = 0f;
			}
			vector += this.startingPos;
			base.transform.localPosition = vector;
			this.velocity *= 1f - this.releaseDrag * Time.deltaTime;
			if (this.velocity.sqrMagnitude < 0.001f)
			{
				this.velocity = Vector3.zero;
			}
		}
	}

	// Token: 0x0600215F RID: 8543 RVA: 0x000B1FC5 File Offset: 0x000B01C5
	public Matrix4x4 GetLocalSpace()
	{
		return this.localSpace;
	}

	// Token: 0x06002160 RID: 8544 RVA: 0x000B1FD0 File Offset: 0x000B01D0
	public void SetCubeToSpecificPosition(Vector3 pos)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(pos);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x06002161 RID: 8545 RVA: 0x000B2060 File Offset: 0x000B0260
	public void SetCubeToSpecificPosition(float x, float y, float z)
	{
		Vector3 vector = new Vector3(0f, 0f, 0f);
		vector.x = Mathf.Clamp(x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x04002C0C RID: 11276
	public float breakDistance = 0.2f;

	// Token: 0x04002C0D RID: 11277
	public float maxXOffset;

	// Token: 0x04002C0E RID: 11278
	public float minXOffset;

	// Token: 0x04002C0F RID: 11279
	public float maxYOffset;

	// Token: 0x04002C10 RID: 11280
	public float minYOffset;

	// Token: 0x04002C11 RID: 11281
	public float maxZOffset;

	// Token: 0x04002C12 RID: 11282
	public float minZOffset;

	// Token: 0x04002C13 RID: 11283
	public bool applyReleaseVelocity;

	// Token: 0x04002C14 RID: 11284
	public float releaseDrag = 1f;

	// Token: 0x04002C15 RID: 11285
	private Matrix4x4 localSpace;

	// Token: 0x04002C16 RID: 11286
	private Vector3 startingPos;

	// Token: 0x04002C17 RID: 11287
	private Vector3 velocity;
}
