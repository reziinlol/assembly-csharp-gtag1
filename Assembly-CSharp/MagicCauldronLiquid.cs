using System;
using UnityEngine;

// Token: 0x020008FD RID: 2301
public class MagicCauldronLiquid : MonoBehaviour
{
	// Token: 0x06003C23 RID: 15395 RVA: 0x001488DB File Offset: 0x00146ADB
	private void Test()
	{
		this._animProgress = 0f;
		this._animating = true;
		base.enabled = true;
	}

	// Token: 0x06003C24 RID: 15396 RVA: 0x001488F6 File Offset: 0x00146AF6
	public void AnimateColorFromTo(Color a, Color b, float length = 1f)
	{
		this._colorStart = a;
		this._colorEnd = b;
		this._animProgress = 0f;
		this._animating = true;
		this.animLength = length;
		base.enabled = true;
	}

	// Token: 0x06003C25 RID: 15397 RVA: 0x00148926 File Offset: 0x00146B26
	private void ApplyColor(Color color)
	{
		if (!this._applyMaterial)
		{
			return;
		}
		this._applyMaterial.SetColor(ShaderProps._BaseColor, color);
		this._applyMaterial.Apply();
	}

	// Token: 0x06003C26 RID: 15398 RVA: 0x00148954 File Offset: 0x00146B54
	private void ApplyWaveParams(float amplitude, float frequency, float scale, float rotation)
	{
		if (!this._applyMaterial)
		{
			return;
		}
		this._applyMaterial.SetFloat(ShaderProps._WaveAmplitude, amplitude);
		this._applyMaterial.SetFloat(ShaderProps._WaveFrequency, frequency);
		this._applyMaterial.SetFloat(ShaderProps._WaveScale, scale);
		this._applyMaterial.Apply();
	}

	// Token: 0x06003C27 RID: 15399 RVA: 0x001489AD File Offset: 0x00146BAD
	private void OnEnable()
	{
		if (this._applyMaterial)
		{
			this._applyMaterial.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
	}

	// Token: 0x06003C28 RID: 15400 RVA: 0x001489C8 File Offset: 0x00146BC8
	private void OnDisable()
	{
		this._animating = false;
		this._animProgress = 0f;
	}

	// Token: 0x06003C29 RID: 15401 RVA: 0x001489DC File Offset: 0x00146BDC
	private void Update()
	{
		if (!this._animating)
		{
			return;
		}
		float num = this._animationCurve.Evaluate(this._animProgress / this.animLength);
		float t = this._waveCurve.Evaluate(this._animProgress / this.animLength);
		if (num >= 1f)
		{
			this.ApplyColor(this._colorEnd);
			this._animating = false;
			base.enabled = false;
			return;
		}
		Color color = Color.Lerp(this._colorStart, this._colorEnd, num);
		Mathf.Lerp(this.waveNormal.frequency, this.waveAnimating.frequency, t);
		Mathf.Lerp(this.waveNormal.amplitude, this.waveAnimating.amplitude, t);
		Mathf.Lerp(this.waveNormal.scale, this.waveAnimating.scale, t);
		Mathf.Lerp(this.waveNormal.rotation, this.waveAnimating.rotation, t);
		this.ApplyColor(color);
		this._animProgress += Time.deltaTime;
	}

	// Token: 0x04004CB8 RID: 19640
	[SerializeField]
	private ApplyMaterialProperty _applyMaterial;

	// Token: 0x04004CB9 RID: 19641
	[SerializeField]
	private Color _colorStart;

	// Token: 0x04004CBA RID: 19642
	[SerializeField]
	private Color _colorEnd;

	// Token: 0x04004CBB RID: 19643
	[SerializeField]
	private bool _animating;

	// Token: 0x04004CBC RID: 19644
	[SerializeField]
	private float _animProgress;

	// Token: 0x04004CBD RID: 19645
	[SerializeField]
	private AnimationCurve _animationCurve = AnimationCurves.EaseOutCubic;

	// Token: 0x04004CBE RID: 19646
	[SerializeField]
	private AnimationCurve _waveCurve = AnimationCurves.EaseInElastic;

	// Token: 0x04004CBF RID: 19647
	public float animLength = 1f;

	// Token: 0x04004CC0 RID: 19648
	public MagicCauldronLiquid.WaveParams waveNormal;

	// Token: 0x04004CC1 RID: 19649
	public MagicCauldronLiquid.WaveParams waveAnimating;

	// Token: 0x020008FE RID: 2302
	[Serializable]
	public struct WaveParams
	{
		// Token: 0x04004CC2 RID: 19650
		public float amplitude;

		// Token: 0x04004CC3 RID: 19651
		public float frequency;

		// Token: 0x04004CC4 RID: 19652
		public float scale;

		// Token: 0x04004CC5 RID: 19653
		public float rotation;
	}
}
