using System;
using UnityEngine;

// Token: 0x02000852 RID: 2130
public class GorillaColorizableParticle : GorillaColorizableBase
{
	// Token: 0x0600372D RID: 14125 RVA: 0x0012EC98 File Offset: 0x0012CE98
	public override void SetColor(Color color)
	{
		ParticleSystem.MainModule main = this.particleSystem.main;
		Color color2 = new Color(Mathf.Pow(color.r, this.gradientColorPower), Mathf.Pow(color.g, this.gradientColorPower), Mathf.Pow(color.b, this.gradientColorPower), color.a);
		main.startColor = new ParticleSystem.MinMaxGradient(this.useLinearColor ? color.linear : color, this.useLinearColor ? color2.linear : color2);
	}

	// Token: 0x04004743 RID: 18243
	public ParticleSystem particleSystem;

	// Token: 0x04004744 RID: 18244
	public float gradientColorPower = 2f;

	// Token: 0x04004745 RID: 18245
	public bool useLinearColor = true;
}
