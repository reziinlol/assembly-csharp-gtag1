using System;
using UnityEngine;

// Token: 0x02000E07 RID: 3591
[RequireComponent(typeof(GorillaVelocityEstimator))]
public class VelocityBasedActivator : MonoBehaviour
{
	// Token: 0x06005795 RID: 22421 RVA: 0x001C5C5D File Offset: 0x001C3E5D
	private void Start()
	{
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
	}

	// Token: 0x06005796 RID: 22422 RVA: 0x001C5C6C File Offset: 0x001C3E6C
	private void Update()
	{
		this.k += this.velocityEstimator.linearVelocity.sqrMagnitude;
		this.k = Mathf.Max(this.k - Time.deltaTime * this.decay, 0f);
		if (!this.active && this.k > this.threshold)
		{
			this.activate(true);
		}
		if (this.active && this.k < this.threshold)
		{
			this.activate(false);
		}
	}

	// Token: 0x06005797 RID: 22423 RVA: 0x001C5CF8 File Offset: 0x001C3EF8
	private void activate(bool v)
	{
		this.active = v;
		for (int i = 0; i < this.activationTargets.Length; i++)
		{
			this.activationTargets[i].SetActive(v);
		}
	}

	// Token: 0x06005798 RID: 22424 RVA: 0x001C5D2D File Offset: 0x001C3F2D
	private void OnDisable()
	{
		if (this.active)
		{
			this.activate(false);
		}
	}

	// Token: 0x04006839 RID: 26681
	[SerializeField]
	private GameObject[] activationTargets;

	// Token: 0x0400683A RID: 26682
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x0400683B RID: 26683
	private float k;

	// Token: 0x0400683C RID: 26684
	private bool active;

	// Token: 0x0400683D RID: 26685
	[SerializeField]
	private float decay = 1f;

	// Token: 0x0400683E RID: 26686
	[SerializeField]
	private float threshold = 1f;
}
