using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020001BA RID: 442
public class SkyboxController : MonoBehaviour
{
	// Token: 0x06000BD4 RID: 3028 RVA: 0x00040798 File Offset: 0x0003E998
	private void Start()
	{
		if (this._dayNightManager.AsNull<BetterDayNightManager>() == null)
		{
			this._dayNightManager = BetterDayNightManager.instance;
		}
		if (this._dayNightManager.AsNull<BetterDayNightManager>() == null)
		{
			return;
		}
		for (int i = 0; i < this._dayNightManager.timeOfDayRange.Length; i++)
		{
			this._totalSecondsInRange += this._dayNightManager.timeOfDayRange[i] * 3600.0;
		}
		this._totalSecondsInRange = Math.Floor(this._totalSecondsInRange);
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x00040826 File Offset: 0x0003EA26
	private void Update()
	{
		if (!this.lastUpdate.HasElapsed(1f, true))
		{
			return;
		}
		this.UpdateTime();
		this.UpdateSky();
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x00040848 File Offset: 0x0003EA48
	private void OnValidate()
	{
		this.UpdateSky();
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x00040850 File Offset: 0x0003EA50
	private void UpdateTime()
	{
		this._currentSeconds = ((ITimeOfDaySystem)this._dayNightManager).currentTimeInSeconds;
		this._currentSeconds = Math.Floor(this._currentSeconds);
		this._currentTime = (float)(this._currentSeconds / this._totalSecondsInRange);
	}

	// Token: 0x06000BD8 RID: 3032 RVA: 0x00040888 File Offset: 0x0003EA88
	private void UpdateSky()
	{
		if (this.skyMaterials == null || this.skyMaterials.Length == 0)
		{
			return;
		}
		int num = this.skyMaterials.Length;
		float num2 = Mathf.Clamp(this._currentTime, 0f, 1f);
		float num3 = 1f / (float)num;
		int num4 = (int)(num2 / num3);
		float num5 = (num2 - (float)num4 * num3) / num3;
		this._currentSky = this.skyMaterials[num4];
		this._nextSky = this.skyMaterials[(num4 + 1) % num];
		this.skyFront.sharedMaterial = this._currentSky;
		this.skyBack.sharedMaterial = this._nextSky;
		if (this._currentSky.renderQueue != 3000)
		{
			this.SetFrontToTransparent();
		}
		if (this._nextSky.renderQueue == 3000)
		{
			this.SetBackToOpaque();
		}
		this._currentSky.SetFloat(ShaderProps._SkyAlpha, 1f - num5);
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x00040964 File Offset: 0x0003EB64
	private void SetFrontToTransparent()
	{
		bool flag = false;
		bool flag2 = false;
		string val = "Transparent";
		int renderQueue = 3000;
		BlendMode blendMode = BlendMode.SrcAlpha;
		BlendMode blendMode2 = BlendMode.OneMinusSrcAlpha;
		BlendMode blendMode3 = BlendMode.One;
		BlendMode blendMode4 = BlendMode.OneMinusSrcAlpha;
		Material sharedMaterial = this.skyFront.sharedMaterial;
		sharedMaterial.SetFloat(ShaderProps._ZWrite, flag ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag);
		sharedMaterial.SetFloat(ShaderProps._AlphaToMask, flag2 ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", val);
		sharedMaterial.renderQueue = renderQueue;
		sharedMaterial.SetFloat(ShaderProps._SrcBlend, (float)blendMode);
		sharedMaterial.SetFloat(ShaderProps._DstBlend, (float)blendMode2);
		sharedMaterial.SetFloat(ShaderProps._SrcBlendAlpha, (float)blendMode3);
		sharedMaterial.SetFloat(ShaderProps._DstBlendAlpha, (float)blendMode4);
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x00040A24 File Offset: 0x0003EC24
	private void SetFrontToOpaque()
	{
		bool flag = false;
		bool flag2 = true;
		string val = "Opaque";
		int renderQueue = 2000;
		BlendMode blendMode = BlendMode.One;
		BlendMode blendMode2 = BlendMode.Zero;
		BlendMode blendMode3 = BlendMode.One;
		BlendMode blendMode4 = BlendMode.Zero;
		Material sharedMaterial = this.skyFront.sharedMaterial;
		sharedMaterial.SetFloat(ShaderProps._ZWrite, flag2 ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag2);
		sharedMaterial.SetFloat(ShaderProps._AlphaToMask, flag ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", val);
		sharedMaterial.renderQueue = renderQueue;
		sharedMaterial.SetFloat(ShaderProps._SrcBlend, (float)blendMode);
		sharedMaterial.SetFloat(ShaderProps._DstBlend, (float)blendMode2);
		sharedMaterial.SetFloat(ShaderProps._SrcBlendAlpha, (float)blendMode3);
		sharedMaterial.SetFloat(ShaderProps._DstBlendAlpha, (float)blendMode4);
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x00040AE4 File Offset: 0x0003ECE4
	private void SetBackToOpaque()
	{
		bool flag = false;
		bool flag2 = true;
		string val = "Opaque";
		int renderQueue = 2000;
		BlendMode blendMode = BlendMode.One;
		BlendMode blendMode2 = BlendMode.Zero;
		BlendMode blendMode3 = BlendMode.One;
		BlendMode blendMode4 = BlendMode.Zero;
		Material sharedMaterial = this.skyBack.sharedMaterial;
		sharedMaterial.SetFloat(ShaderProps._ZWrite, flag2 ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag2);
		sharedMaterial.SetFloat(ShaderProps._AlphaToMask, flag ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", val);
		sharedMaterial.renderQueue = renderQueue;
		sharedMaterial.SetFloat(ShaderProps._SrcBlend, (float)blendMode);
		sharedMaterial.SetFloat(ShaderProps._DstBlend, (float)blendMode2);
		sharedMaterial.SetFloat(ShaderProps._SrcBlendAlpha, (float)blendMode3);
		sharedMaterial.SetFloat(ShaderProps._DstBlendAlpha, (float)blendMode4);
	}

	// Token: 0x04000E60 RID: 3680
	public MeshRenderer skyFront;

	// Token: 0x04000E61 RID: 3681
	public MeshRenderer skyBack;

	// Token: 0x04000E62 RID: 3682
	public Material[] skyMaterials = new Material[0];

	// Token: 0x04000E63 RID: 3683
	[Range(0f, 1f)]
	public float lerpValue;

	// Token: 0x04000E64 RID: 3684
	[NonSerialized]
	private Material _currentSky;

	// Token: 0x04000E65 RID: 3685
	[NonSerialized]
	private Material _nextSky;

	// Token: 0x04000E66 RID: 3686
	private TimeSince lastUpdate = TimeSince.Now();

	// Token: 0x04000E67 RID: 3687
	[Space]
	private BetterDayNightManager _dayNightManager;

	// Token: 0x04000E68 RID: 3688
	private double _currentSeconds = -1.0;

	// Token: 0x04000E69 RID: 3689
	private double _totalSecondsInRange = -1.0;

	// Token: 0x04000E6A RID: 3690
	private float _currentTime = -1f;
}
