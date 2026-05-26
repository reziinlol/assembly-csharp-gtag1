using System;
using UnityEngine;

// Token: 0x02000415 RID: 1045
public class LineRendererDraw : MonoBehaviour
{
	// Token: 0x060018D7 RID: 6359 RVA: 0x0008CDBA File Offset: 0x0008AFBA
	public void SetUpLine(Transform[] points)
	{
		this.lr.positionCount = points.Length;
		this.points = points;
	}

	// Token: 0x060018D8 RID: 6360 RVA: 0x0008CDD4 File Offset: 0x0008AFD4
	private void LateUpdate()
	{
		for (int i = 0; i < this.points.Length; i++)
		{
			this.lr.SetPosition(i, this.points[i].position);
		}
	}

	// Token: 0x060018D9 RID: 6361 RVA: 0x0008CE0D File Offset: 0x0008B00D
	public void Enable(bool enable)
	{
		this.lr.enabled = enable;
	}

	// Token: 0x04002400 RID: 9216
	public LineRenderer lr;

	// Token: 0x04002401 RID: 9217
	public Transform[] points;
}
