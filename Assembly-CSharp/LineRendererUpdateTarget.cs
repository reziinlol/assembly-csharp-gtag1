using System;
using UnityEngine;

// Token: 0x02000282 RID: 642
public class LineRendererUpdateTarget : MonoBehaviourPostTick
{
	// Token: 0x0600114C RID: 4428 RVA: 0x0005CE70 File Offset: 0x0005B070
	public override void PostTick()
	{
		if (this.lineRenderer == null || this.targetTransform == null || this.lineRenderer.positionCount != 2)
		{
			return;
		}
		if (!this.targetTransform.gameObject.activeSelf)
		{
			this.lineRenderer.enabled = false;
			return;
		}
		this.lineRenderer.enabled = true;
		this.lineRenderer.SetPosition(0, base.transform.position);
		this.lineRenderer.SetPosition(1, this.targetTransform.position);
	}

	// Token: 0x0600114D RID: 4429 RVA: 0x0005CF01 File Offset: 0x0005B101
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.lineRenderer.useWorldSpace = true;
	}

	// Token: 0x040014A1 RID: 5281
	private LineRenderer lineRenderer;

	// Token: 0x040014A2 RID: 5282
	public Transform targetTransform;
}
