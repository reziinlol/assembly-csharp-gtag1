using System;
using GorillaTag.Cosmetics;
using UnityEngine;

// Token: 0x020009BF RID: 2495
public class ReplacementVoice : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003FD9 RID: 16345 RVA: 0x00011DD7 File Offset: 0x0000FFD7
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06003FDA RID: 16346 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06003FDB RID: 16347 RVA: 0x001551F4 File Offset: 0x001533F4
	public void SliceUpdate()
	{
		if (!this.replacementVoiceSource.isPlaying && this.myVRRig.ShouldPlayReplacementVoice())
		{
			if (!Mathf.Approximately(this.myVRRig.voiceAudio.pitch, this.replacementVoiceSource.pitch))
			{
				this.replacementVoiceSource.pitch = this.myVRRig.voiceAudio.pitch;
			}
			if (this.myVRRig.SpeakingLoudness < this.loudReplacementVoiceThreshold)
			{
				this.replacementVoiceSource.clip = this.replacementVoiceClips[Random.Range(0, this.replacementVoiceClips.Length - 1)];
				this.replacementVoiceSource.volume = this.normalVolume;
			}
			else
			{
				this.replacementVoiceSource.clip = this.replacementVoiceClipsLoud[Random.Range(0, this.replacementVoiceClipsLoud.Length - 1)];
				this.replacementVoiceSource.volume = this.loudVolume;
			}
			this.replacementVoiceSource.GTPlay();
			return;
		}
		CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
		if (!this.replacementVoiceSource.isPlaying && this.myVRRig.TryGetCosmeticVoiceOverride(CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride, out cosmeticEffect))
		{
			if (this.myVRRig.SpeakingLoudness < this.myVRRig.replacementVoiceLoudnessThreshold)
			{
				return;
			}
			if (!Mathf.Approximately(this.myVRRig.voiceAudio.pitch, this.replacementVoiceSource.pitch))
			{
				this.replacementVoiceSource.pitch = this.myVRRig.voiceAudio.pitch;
			}
			if (this.myVRRig.SpeakingLoudness < cosmeticEffect.voiceOverrideLoudThreshold)
			{
				this.replacementVoiceSource.clip = cosmeticEffect.voiceOverrideNormalClips[Random.Range(0, cosmeticEffect.voiceOverrideNormalClips.Length - 1)];
				this.replacementVoiceSource.volume = cosmeticEffect.voiceOverrideNormalVolume;
			}
			else
			{
				this.replacementVoiceSource.clip = cosmeticEffect.voiceOverrideLoudClips[Random.Range(0, cosmeticEffect.voiceOverrideLoudClips.Length - 1)];
				this.replacementVoiceSource.volume = cosmeticEffect.voiceOverrideLoudVolume;
			}
			this.replacementVoiceSource.GTPlay();
		}
	}

	// Token: 0x04005044 RID: 20548
	public AudioSource replacementVoiceSource;

	// Token: 0x04005045 RID: 20549
	public AudioClip[] replacementVoiceClips;

	// Token: 0x04005046 RID: 20550
	public AudioClip[] replacementVoiceClipsLoud;

	// Token: 0x04005047 RID: 20551
	public float loudReplacementVoiceThreshold = 0.1f;

	// Token: 0x04005048 RID: 20552
	public VRRig myVRRig;

	// Token: 0x04005049 RID: 20553
	public float normalVolume = 0.5f;

	// Token: 0x0400504A RID: 20554
	public float loudVolume = 0.8f;
}
