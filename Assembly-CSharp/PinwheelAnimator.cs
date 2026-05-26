using System;
using UnityEngine;

// Token: 0x02000209 RID: 521
public class PinwheelAnimator : MonoBehaviour
{
	// Token: 0x06000DC3 RID: 3523 RVA: 0x0004B6F5 File Offset: 0x000498F5
	protected void OnEnable()
	{
		this.oldPos = this.spinnerTransform.position;
		this.spinSpeed = 0f;
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x0004B714 File Offset: 0x00049914
	protected void LateUpdate()
	{
		Vector3 position = this.spinnerTransform.position;
		Vector3 forward = base.transform.forward;
		Vector3 vector = position - this.oldPos;
		float b = Mathf.Clamp(vector.magnitude / Time.deltaTime * Vector3.Dot(vector.normalized, forward) * this.spinSpeedMultiplier, -this.maxSpinSpeed, this.maxSpinSpeed);
		this.spinSpeed = Mathf.Lerp(this.spinSpeed, b, Time.deltaTime * this.damping);
		this.spinnerTransform.Rotate(Vector3.forward, this.spinSpeed * 360f * Time.deltaTime);
		this.oldPos = position;
	}

	// Token: 0x04001067 RID: 4199
	public Transform spinnerTransform;

	// Token: 0x04001068 RID: 4200
	[Tooltip("In revolutions per second.")]
	public float maxSpinSpeed = 4f;

	// Token: 0x04001069 RID: 4201
	public float spinSpeedMultiplier = 5f;

	// Token: 0x0400106A RID: 4202
	public float damping = 0.5f;

	// Token: 0x0400106B RID: 4203
	private Vector3 oldPos;

	// Token: 0x0400106C RID: 4204
	private float spinSpeed;
}
