using System;
using UnityEngine;

// Token: 0x02000912 RID: 2322
public class TapInnerGlow : MonoBehaviour
{
	// Token: 0x1700057C RID: 1404
	// (get) Token: 0x06003CB2 RID: 15538 RVA: 0x0014A700 File Offset: 0x00148900
	private Material targetMaterial
	{
		get
		{
			if (this._instance.AsNull<Material>() == null)
			{
				return this._instance = this._renderer.material;
			}
			return this._instance;
		}
	}

	// Token: 0x06003CB3 RID: 15539 RVA: 0x0014A73C File Offset: 0x0014893C
	public void Tap()
	{
		if (!this._renderer)
		{
			return;
		}
		Material targetMaterial = this.targetMaterial;
		float value = this.tapLength;
		float time = GTShaderGlobals.Time;
		UberShader.InnerGlowSinePeriod.SetValue<float>(targetMaterial, value);
		UberShader.InnerGlowSinePhaseShift.SetValue<float>(targetMaterial, time);
	}

	// Token: 0x04004D5C RID: 19804
	public Renderer _renderer;

	// Token: 0x04004D5D RID: 19805
	public float tapLength = 1f;

	// Token: 0x04004D5E RID: 19806
	[Space]
	private Material _instance;
}
