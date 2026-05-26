using System;
using UnityEngine;

// Token: 0x0200034C RID: 844
public class HandFXModifier : FXModifier
{
	// Token: 0x060014C9 RID: 5321 RVA: 0x0006EDDF File Offset: 0x0006CFDF
	private void Awake()
	{
		this.originalScale = base.transform.localScale;
	}

	// Token: 0x060014CA RID: 5322 RVA: 0x0006EDF2 File Offset: 0x0006CFF2
	private void OnDisable()
	{
		base.transform.localScale = this.originalScale;
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x0006EE05 File Offset: 0x0006D005
	public override void UpdateScale(float scale, Color color)
	{
		scale = Mathf.Clamp(scale, this.minScale, this.maxScale);
		base.transform.localScale = this.originalScale * scale;
	}

	// Token: 0x0400197D RID: 6525
	private Vector3 originalScale;

	// Token: 0x0400197E RID: 6526
	[SerializeField]
	private float minScale;

	// Token: 0x0400197F RID: 6527
	[SerializeField]
	private float maxScale;

	// Token: 0x04001980 RID: 6528
	[SerializeField]
	private ParticleSystem dustBurst;

	// Token: 0x04001981 RID: 6529
	[SerializeField]
	private ParticleSystem dustLinger;
}
