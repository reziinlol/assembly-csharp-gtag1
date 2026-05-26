using System;
using UnityEngine;

// Token: 0x0200052F RID: 1327
public class ManipulatableSlider : ManipulatableObject
{
	// Token: 0x0600216C RID: 8556 RVA: 0x000B243E File Offset: 0x000B063E
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x0600216D RID: 8557 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x0600216E RID: 8558 RVA: 0x000B2462 File Offset: 0x000B0662
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
		if (this.applyReleaseVelocity)
		{
			this.velocity = this.localSpace.MultiplyVector(releaseVelocity);
		}
	}

	// Token: 0x0600216F RID: 8559 RVA: 0x000B2480 File Offset: 0x000B0680
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06002170 RID: 8560 RVA: 0x000B24C0 File Offset: 0x000B06C0
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(hand.transform.position);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x06002171 RID: 8561 RVA: 0x000B2558 File Offset: 0x000B0758
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

	// Token: 0x06002172 RID: 8562 RVA: 0x000B270C File Offset: 0x000B090C
	public void SetProgress(float x, float y, float z)
	{
		x = Mathf.Clamp(x, 0f, 1f);
		y = Mathf.Clamp(y, 0f, 1f);
		z = Mathf.Clamp(z, 0f, 1f);
		Vector3 localPosition = this.startingPos;
		localPosition.x += Mathf.Lerp(this.minXOffset, this.maxXOffset, x);
		localPosition.y += Mathf.Lerp(this.minYOffset, this.maxYOffset, y);
		localPosition.z += Mathf.Lerp(this.minZOffset, this.maxZOffset, z);
		base.transform.localPosition = localPosition;
	}

	// Token: 0x06002173 RID: 8563 RVA: 0x000B27B9 File Offset: 0x000B09B9
	public float GetProgressX()
	{
		return ((base.transform.localPosition - this.startingPos).x - this.minXOffset) / (this.maxXOffset - this.minXOffset);
	}

	// Token: 0x06002174 RID: 8564 RVA: 0x000B27EB File Offset: 0x000B09EB
	public float GetProgressY()
	{
		return ((base.transform.localPosition - this.startingPos).y - this.minYOffset) / (this.maxYOffset - this.minYOffset);
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x000B281D File Offset: 0x000B0A1D
	public float GetProgressZ()
	{
		return ((base.transform.localPosition - this.startingPos).z - this.minZOffset) / (this.maxZOffset - this.minZOffset);
	}

	// Token: 0x04002C26 RID: 11302
	public float breakDistance = 0.2f;

	// Token: 0x04002C27 RID: 11303
	public float maxXOffset;

	// Token: 0x04002C28 RID: 11304
	public float minXOffset;

	// Token: 0x04002C29 RID: 11305
	public float maxYOffset;

	// Token: 0x04002C2A RID: 11306
	public float minYOffset;

	// Token: 0x04002C2B RID: 11307
	public float maxZOffset;

	// Token: 0x04002C2C RID: 11308
	public float minZOffset;

	// Token: 0x04002C2D RID: 11309
	public bool applyReleaseVelocity;

	// Token: 0x04002C2E RID: 11310
	public float releaseDrag = 1f;

	// Token: 0x04002C2F RID: 11311
	private Matrix4x4 localSpace;

	// Token: 0x04002C30 RID: 11312
	private Vector3 startingPos;

	// Token: 0x04002C31 RID: 11313
	private Vector3 velocity;
}
