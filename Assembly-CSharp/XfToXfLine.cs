using System;
using UnityEngine;

// Token: 0x02000919 RID: 2329
public class XfToXfLine : MonoBehaviour
{
	// Token: 0x06003CE1 RID: 15585 RVA: 0x0014B5B3 File Offset: 0x001497B3
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x06003CE2 RID: 15586 RVA: 0x0014B5C1 File Offset: 0x001497C1
	private void Update()
	{
		this.lineRenderer.SetPosition(0, this.pt0.transform.position);
		this.lineRenderer.SetPosition(1, this.pt1.transform.position);
	}

	// Token: 0x04004D78 RID: 19832
	public Transform pt0;

	// Token: 0x04004D79 RID: 19833
	public Transform pt1;

	// Token: 0x04004D7A RID: 19834
	private LineRenderer lineRenderer;
}
