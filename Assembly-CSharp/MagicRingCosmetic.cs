using System;
using UnityEngine;

// Token: 0x0200020C RID: 524
public class MagicRingCosmetic : MonoBehaviour
{
	// Token: 0x06000DCF RID: 3535 RVA: 0x0004B93E File Offset: 0x00049B3E
	protected void Awake()
	{
		this.materialPropertyBlock = new MaterialPropertyBlock();
		this.defaultEmissiveColor = this.ringRenderer.sharedMaterial.GetColor(ShaderProps._EmissionColor);
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x0004B968 File Offset: 0x00049B68
	protected void LateUpdate()
	{
		float celsius = this.thermalReceiver.celsius;
		if (celsius >= this.fadeInTemperatureThreshold && this.fadeState != MagicRingCosmetic.FadeState.FadedIn)
		{
			this.fadeInSounds.Play();
			this.fadeState = MagicRingCosmetic.FadeState.FadedIn;
		}
		else if (celsius <= this.fadeOutTemperatureThreshold && this.fadeState != MagicRingCosmetic.FadeState.FadedOut)
		{
			this.fadeOutSounds.Play();
			this.fadeState = MagicRingCosmetic.FadeState.FadedOut;
		}
		this.emissiveAmount = Mathf.MoveTowards(this.emissiveAmount, (this.fadeState == MagicRingCosmetic.FadeState.FadedIn) ? 1f : 0f, Time.deltaTime / this.fadeTime);
		this.ringRenderer.GetPropertyBlock(this.materialPropertyBlock);
		this.materialPropertyBlock.SetColor(ShaderProps._EmissionColor, new Color(this.defaultEmissiveColor.r, this.defaultEmissiveColor.g, this.defaultEmissiveColor.b, this.emissiveAmount));
		this.ringRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	// Token: 0x04001076 RID: 4214
	[Tooltip("The ring will fade in the emissive texture based on temperature from this ThermalReceiver.")]
	public ThermalReceiver thermalReceiver;

	// Token: 0x04001077 RID: 4215
	public Renderer ringRenderer;

	// Token: 0x04001078 RID: 4216
	public float fadeInTemperatureThreshold = 200f;

	// Token: 0x04001079 RID: 4217
	public float fadeOutTemperatureThreshold = 190f;

	// Token: 0x0400107A RID: 4218
	public float fadeTime = 1.5f;

	// Token: 0x0400107B RID: 4219
	public SoundBankPlayer fadeInSounds;

	// Token: 0x0400107C RID: 4220
	public SoundBankPlayer fadeOutSounds;

	// Token: 0x0400107D RID: 4221
	private MagicRingCosmetic.FadeState fadeState;

	// Token: 0x0400107E RID: 4222
	private Color defaultEmissiveColor;

	// Token: 0x0400107F RID: 4223
	private float emissiveAmount;

	// Token: 0x04001080 RID: 4224
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x0200020D RID: 525
	private enum FadeState
	{
		// Token: 0x04001082 RID: 4226
		FadedOut,
		// Token: 0x04001083 RID: 4227
		FadedIn
	}
}
