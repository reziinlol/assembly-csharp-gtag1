using System;
using UnityEngine;

// Token: 0x020005A1 RID: 1441
public class XRaySkeleton : SyncToPlayerColor, IGorillaSimpleBackgroundWorker
{
	// Token: 0x06002487 RID: 9351 RVA: 0x000C4000 File Offset: 0x000C2200
	protected override void Awake()
	{
		base.Awake();
		this.target = this.renderer.material;
		this.mats = this.rig.materialsToChangeTo;
		this.tagMaterials = new Material[this.mats.Length];
		this.tagMaterials[0] = new Material(this.target);
		GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
	}

	// Token: 0x06002488 RID: 9352 RVA: 0x000C4064 File Offset: 0x000C2264
	public void SimpleWork()
	{
		if (this.currentIndex >= 0 && this.currentIndex < this.mats.Length)
		{
			Material material = new Material(this.mats[this.currentIndex]);
			this.tagMaterials[this.currentIndex] = material;
			this.currentIndex++;
			GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
		}
	}

	// Token: 0x06002489 RID: 9353 RVA: 0x000C40BF File Offset: 0x000C22BF
	public void SetMaterialIndex(int index)
	{
		this.renderer.sharedMaterial = this.tagMaterials[index];
		this._lastMatIndex = index;
	}

	// Token: 0x0600248A RID: 9354 RVA: 0x000C40DB File Offset: 0x000C22DB
	private void Setup()
	{
		this.colorPropertiesToSync = new ShaderHashId[]
		{
			XRaySkeleton._BaseColor,
			XRaySkeleton._EmissionColor
		};
	}

	// Token: 0x0600248B RID: 9355 RVA: 0x000C4104 File Offset: 0x000C2304
	public override void UpdateColor(Color color)
	{
		if (this._lastMatIndex != 0)
		{
			return;
		}
		Material material = this.tagMaterials[0];
		float h;
		float s;
		float value;
		Color.RGBToHSV(color, out h, out s, out value);
		Color value2 = Color.HSVToRGB(h, s, Mathf.Clamp(value, this.baseValueMinMax.x, this.baseValueMinMax.y));
		material.SetColor(XRaySkeleton._BaseColor, value2);
		float h2;
		float num;
		float num2;
		Color.RGBToHSV(color, out h2, out num, out num2);
		Color color2 = Color.HSVToRGB(h2, 0.82f, 0.9f, true);
		color2 = new Color(color2.r * 1.4f, color2.g * 1.4f, color2.b * 1.4f);
		material.SetColor(XRaySkeleton._EmissionColor, ColorUtils.ComposeHDR(new Color32(36, 191, 136, byte.MaxValue), 2f));
		this.renderer.sharedMaterial = material;
	}

	// Token: 0x04002FE9 RID: 12265
	public SkinnedMeshRenderer renderer;

	// Token: 0x04002FEA RID: 12266
	public Vector2 baseValueMinMax = new Vector2(0.69f, 1f);

	// Token: 0x04002FEB RID: 12267
	public Material[] tagMaterials = new Material[0];

	// Token: 0x04002FEC RID: 12268
	private int _lastMatIndex;

	// Token: 0x04002FED RID: 12269
	private Material[] mats;

	// Token: 0x04002FEE RID: 12270
	private int currentIndex = 1;

	// Token: 0x04002FEF RID: 12271
	private static readonly ShaderHashId _BaseColor = "_BaseColor";

	// Token: 0x04002FF0 RID: 12272
	private static readonly ShaderHashId _EmissionColor = "_EmissionColor";
}
