using System;
using GorillaLocomotion.Swimming;
using UnityEngine;

// Token: 0x020003B0 RID: 944
public class WaterSplashEffect : MonoBehaviour
{
	// Token: 0x060016C0 RID: 5824 RVA: 0x0008447F File Offset: 0x0008267F
	private void OnEnable()
	{
		this.startTime = Time.time;
	}

	// Token: 0x060016C1 RID: 5825 RVA: 0x0008448C File Offset: 0x0008268C
	public void Destroy()
	{
		this.DeactivateParticleSystems(this.bigSplashParticleSystems);
		this.DeactivateParticleSystems(this.smallSplashParticleSystems);
		this.waterVolume = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x060016C2 RID: 5826 RVA: 0x000844C0 File Offset: 0x000826C0
	public void PlayEffect(bool isBigSplash, bool isEntry, float scale, WaterVolume volume = null)
	{
		this.waterVolume = volume;
		if (isBigSplash)
		{
			this.DeactivateParticleSystems(this.smallSplashParticleSystems);
			this.SetParticleEffectParameters(this.bigSplashParticleSystems, scale, this.bigSplashBaseGravityMultiplier, this.bigSplashBaseStartSpeed, this.bigSplashBaseSimulationSpeed, this.waterVolume);
			this.PlayParticleEffects(this.bigSplashParticleSystems);
			this.PlayRandomAudioClipWithoutRepeats(this.bigSplashAudioClips, ref WaterSplashEffect.lastPlayedBigSplashAudioClipIndex);
			return;
		}
		if (isEntry)
		{
			this.DeactivateParticleSystems(this.bigSplashParticleSystems);
			this.SetParticleEffectParameters(this.smallSplashParticleSystems, scale, this.smallSplashBaseGravityMultiplier, this.smallSplashBaseStartSpeed, this.smallSplashBaseSimulationSpeed, this.waterVolume);
			this.PlayParticleEffects(this.smallSplashParticleSystems);
			this.PlayRandomAudioClipWithoutRepeats(this.smallSplashEntryAudioClips, ref WaterSplashEffect.lastPlayedSmallSplashEntryAudioClipIndex);
			return;
		}
		this.DeactivateParticleSystems(this.bigSplashParticleSystems);
		this.SetParticleEffectParameters(this.smallSplashParticleSystems, scale, this.smallSplashBaseGravityMultiplier, this.smallSplashBaseStartSpeed, this.smallSplashBaseSimulationSpeed, this.waterVolume);
		this.PlayParticleEffects(this.smallSplashParticleSystems);
		this.PlayRandomAudioClipWithoutRepeats(this.smallSplashExitAudioClips, ref WaterSplashEffect.lastPlayedSmallSplashExitAudioClipIndex);
	}

	// Token: 0x060016C3 RID: 5827 RVA: 0x000845C8 File Offset: 0x000827C8
	private void Update()
	{
		if (this.waterVolume != null && !this.waterVolume.isStationary && this.waterVolume.surfacePlane != null)
		{
			Vector3 b = Vector3.Dot(base.transform.position - this.waterVolume.surfacePlane.position, this.waterVolume.surfacePlane.up) * this.waterVolume.surfacePlane.up;
			base.transform.position = base.transform.position - b;
		}
		if ((Time.time - this.startTime) / this.lifeTime >= 1f)
		{
			this.Destroy();
			return;
		}
	}

	// Token: 0x060016C4 RID: 5828 RVA: 0x00084690 File Offset: 0x00082890
	private void DeactivateParticleSystems(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x000846BC File Offset: 0x000828BC
	private void PlayParticleEffects(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(true);
				particleSystems[i].Play();
			}
		}
	}

	// Token: 0x060016C6 RID: 5830 RVA: 0x000846F0 File Offset: 0x000828F0
	private void SetParticleEffectParameters(ParticleSystem[] particleSystems, float scale, float baseGravMultiplier, float baseStartSpeed, float baseSimulationSpeed, WaterVolume waterVolume = null)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				ParticleSystem.MainModule main = particleSystems[i].main;
				main.startSpeed = baseStartSpeed;
				main.gravityModifier = baseGravMultiplier;
				if (scale < 0.99f)
				{
					main.startSpeed = baseStartSpeed * scale * 2f;
					main.gravityModifier = baseGravMultiplier * scale * 0.5f;
				}
				if (waterVolume != null && waterVolume.Parameters != null)
				{
					particleSystems[i].colorBySpeed.color = waterVolume.Parameters.splashColorBySpeedGradient;
				}
			}
		}
	}

	// Token: 0x060016C7 RID: 5831 RVA: 0x000847A8 File Offset: 0x000829A8
	private void PlayRandomAudioClipWithoutRepeats(AudioClip[] audioClips, ref int lastPlayedAudioClipIndex)
	{
		if (this.audioSource != null && audioClips != null && audioClips.Length != 0)
		{
			int num = 0;
			if (audioClips.Length > 1)
			{
				int num2 = Random.Range(0, audioClips.Length);
				if (num2 == lastPlayedAudioClipIndex)
				{
					num2 = ((Random.Range(0f, 1f) > 0.5f) ? ((num2 + 1) % audioClips.Length) : (num2 - 1));
					if (num2 < 0)
					{
						num2 = audioClips.Length - 1;
					}
				}
				num = num2;
			}
			lastPlayedAudioClipIndex = num;
			this.audioSource.clip = audioClips[num];
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x040021BF RID: 8639
	private static int lastPlayedBigSplashAudioClipIndex = -1;

	// Token: 0x040021C0 RID: 8640
	private static int lastPlayedSmallSplashEntryAudioClipIndex = -1;

	// Token: 0x040021C1 RID: 8641
	private static int lastPlayedSmallSplashExitAudioClipIndex = -1;

	// Token: 0x040021C2 RID: 8642
	public ParticleSystem[] bigSplashParticleSystems;

	// Token: 0x040021C3 RID: 8643
	public ParticleSystem[] smallSplashParticleSystems;

	// Token: 0x040021C4 RID: 8644
	public float bigSplashBaseGravityMultiplier = 0.9f;

	// Token: 0x040021C5 RID: 8645
	public float bigSplashBaseStartSpeed = 1.9f;

	// Token: 0x040021C6 RID: 8646
	public float bigSplashBaseSimulationSpeed = 0.9f;

	// Token: 0x040021C7 RID: 8647
	public float smallSplashBaseGravityMultiplier = 0.6f;

	// Token: 0x040021C8 RID: 8648
	public float smallSplashBaseStartSpeed = 0.6f;

	// Token: 0x040021C9 RID: 8649
	public float smallSplashBaseSimulationSpeed = 0.6f;

	// Token: 0x040021CA RID: 8650
	public float lifeTime = 1f;

	// Token: 0x040021CB RID: 8651
	private float startTime = -1f;

	// Token: 0x040021CC RID: 8652
	public AudioSource audioSource;

	// Token: 0x040021CD RID: 8653
	public AudioClip[] bigSplashAudioClips;

	// Token: 0x040021CE RID: 8654
	public AudioClip[] smallSplashEntryAudioClips;

	// Token: 0x040021CF RID: 8655
	public AudioClip[] smallSplashExitAudioClips;

	// Token: 0x040021D0 RID: 8656
	private WaterVolume waterVolume;
}
