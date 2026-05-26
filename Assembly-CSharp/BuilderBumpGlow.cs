using System;
using UnityEngine;

// Token: 0x02000618 RID: 1560
public class BuilderBumpGlow : MonoBehaviour
{
	// Token: 0x060026C1 RID: 9921 RVA: 0x000CD044 File Offset: 0x000CB244
	public void Awake()
	{
		this.blendIn = 1f;
		this.intensity = 0f;
		this.UpdateRender();
	}

	// Token: 0x060026C2 RID: 9922 RVA: 0x000CD062 File Offset: 0x000CB262
	public void SetIntensity(float intensity)
	{
		this.intensity = intensity;
		this.UpdateRender();
	}

	// Token: 0x060026C3 RID: 9923 RVA: 0x000CD071 File Offset: 0x000CB271
	public void SetBlendIn(float blendIn)
	{
		this.blendIn = blendIn;
		this.UpdateRender();
	}

	// Token: 0x060026C4 RID: 9924 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void UpdateRender()
	{
	}

	// Token: 0x0400324C RID: 12876
	public MeshRenderer glowRenderer;

	// Token: 0x0400324D RID: 12877
	private float blendIn;

	// Token: 0x0400324E RID: 12878
	private float intensity;
}
