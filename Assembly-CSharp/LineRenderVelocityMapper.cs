using System;
using UnityEngine;

// Token: 0x02000416 RID: 1046
[RequireComponent(typeof(LineRenderer))]
public class LineRenderVelocityMapper : MonoBehaviour
{
	// Token: 0x060018DB RID: 6363 RVA: 0x0008CE1B File Offset: 0x0008B01B
	private void Awake()
	{
		this._lr = base.GetComponent<LineRenderer>();
		this._lr.useWorldSpace = true;
	}

	// Token: 0x060018DC RID: 6364 RVA: 0x0008CE38 File Offset: 0x0008B038
	private void LateUpdate()
	{
		if (this.velocityEstimator == null)
		{
			return;
		}
		this._lr.SetPosition(0, this.velocityEstimator.transform.position);
		if (this.velocityEstimator.linearVelocity.sqrMagnitude > 0.1f)
		{
			this._lr.SetPosition(1, this.velocityEstimator.transform.position + this.velocityEstimator.linearVelocity.normalized * 0.2f);
			return;
		}
		this._lr.SetPosition(1, this.velocityEstimator.transform.position);
	}

	// Token: 0x04002402 RID: 9218
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04002403 RID: 9219
	private LineRenderer _lr;
}
