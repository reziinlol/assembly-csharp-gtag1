using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004A6 RID: 1190
public class BalloonString : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001CE7 RID: 7399 RVA: 0x0009CCAC File Offset: 0x0009AEAC
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.vertices = new List<Vector3>(this.numSegments + 1);
		if (this.startPositionXf != null && this.endPositionXf != null)
		{
			this.vertices.Add(this.startPositionXf.position);
			int num = this.vertices.Count - 2;
			for (int i = 0; i < num; i++)
			{
				float t = (float)((i + 1) / (this.vertices.Count - 1));
				Vector3 item = Vector3.Lerp(this.startPositionXf.position, this.endPositionXf.position, t);
				this.vertices.Add(item);
			}
			this.vertices.Add(this.endPositionXf.position);
		}
	}

	// Token: 0x06001CE8 RID: 7400 RVA: 0x0009CD7C File Offset: 0x0009AF7C
	private void UpdateDynamics()
	{
		this.vertices[0] = this.startPositionXf.position;
		this.vertices[this.vertices.Count - 1] = this.endPositionXf.position;
	}

	// Token: 0x06001CE9 RID: 7401 RVA: 0x0009CDB8 File Offset: 0x0009AFB8
	private void UpdateRenderPositions()
	{
		this.lineRenderer.SetPosition(0, this.startPositionXf.transform.position);
		this.lineRenderer.SetPosition(1, this.endPositionXf.transform.position);
	}

	// Token: 0x06001CEA RID: 7402 RVA: 0x00011DD7 File Offset: 0x0000FFD7
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001CEB RID: 7403 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001CEC RID: 7404 RVA: 0x0009CDF2 File Offset: 0x0009AFF2
	public void SliceUpdate()
	{
		if (this.startPositionXf != null && this.endPositionXf != null)
		{
			this.UpdateDynamics();
			this.UpdateRenderPositions();
		}
	}

	// Token: 0x04002713 RID: 10003
	public Transform startPositionXf;

	// Token: 0x04002714 RID: 10004
	public Transform endPositionXf;

	// Token: 0x04002715 RID: 10005
	private List<Vector3> vertices;

	// Token: 0x04002716 RID: 10006
	public int numSegments = 1;

	// Token: 0x04002717 RID: 10007
	private LineRenderer lineRenderer;
}
