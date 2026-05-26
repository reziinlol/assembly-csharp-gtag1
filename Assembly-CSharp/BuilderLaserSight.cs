using System;
using UnityEngine;

// Token: 0x02000625 RID: 1573
public class BuilderLaserSight : MonoBehaviour
{
	// Token: 0x06002734 RID: 10036 RVA: 0x000D01F2 File Offset: 0x000CE3F2
	public void Awake()
	{
		if (this.lineRenderer == null)
		{
			this.lineRenderer = base.GetComponentInChildren<LineRenderer>();
		}
		if (this.lineRenderer != null)
		{
			this.lineRenderer.enabled = false;
		}
	}

	// Token: 0x06002735 RID: 10037 RVA: 0x000D0228 File Offset: 0x000CE428
	public void SetPoints(Vector3 start, Vector3 end)
	{
		this.lineRenderer.positionCount = 2;
		this.lineRenderer.SetPosition(0, start);
		this.lineRenderer.SetPosition(1, end);
	}

	// Token: 0x06002736 RID: 10038 RVA: 0x000D0250 File Offset: 0x000CE450
	public void Show(bool show)
	{
		if (this.lineRenderer != null)
		{
			this.lineRenderer.enabled = show;
		}
	}

	// Token: 0x040032DA RID: 13018
	public LineRenderer lineRenderer;
}
