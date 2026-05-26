using System;
using UnityEngine;

// Token: 0x02000206 RID: 518
public class CloudUmbrellaCloud : MonoBehaviour
{
	// Token: 0x06000DB8 RID: 3512 RVA: 0x0004B333 File Offset: 0x00049533
	protected void Awake()
	{
		this.umbrellaXform = this.umbrella.transform;
		this.cloudScaleXform = this.cloudRenderer.transform;
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x0004B358 File Offset: 0x00049558
	protected void LateUpdate()
	{
		float time = Vector3.Dot(this.umbrellaXform.up, Vector3.up);
		float num = Mathf.Clamp01(this.scaleCurve.Evaluate(time));
		this.rendererOn = ((num > 0.09f && num < 0.1f) ? this.rendererOn : (num > 0.1f));
		this.cloudRenderer.enabled = this.rendererOn;
		this.cloudScaleXform.localScale = new Vector3(num, num, num);
		this.cloudRotateXform.up = Vector3.up;
	}

	// Token: 0x04001050 RID: 4176
	public UmbrellaItem umbrella;

	// Token: 0x04001051 RID: 4177
	public Transform cloudRotateXform;

	// Token: 0x04001052 RID: 4178
	public Renderer cloudRenderer;

	// Token: 0x04001053 RID: 4179
	public AnimationCurve scaleCurve;

	// Token: 0x04001054 RID: 4180
	private const float kHideAtScale = 0.1f;

	// Token: 0x04001055 RID: 4181
	private const float kHideAtScaleTolerance = 0.01f;

	// Token: 0x04001056 RID: 4182
	private bool rendererOn;

	// Token: 0x04001057 RID: 4183
	private Transform umbrellaXform;

	// Token: 0x04001058 RID: 4184
	private Transform cloudScaleXform;
}
