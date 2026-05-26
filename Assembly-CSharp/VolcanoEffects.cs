using System;
using System.Collections;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020009C9 RID: 2505
public class VolcanoEffects : MonoBehaviour
{
	// Token: 0x0600401D RID: 16413 RVA: 0x00156FA4 File Offset: 0x001551A4
	private void Awake()
	{
		if (this.RemoveNullsFromArray<ParticleSystem>(ref this.lavaSpewParticleSystems))
		{
			this.LogNullsFoundInArray("lavaSpewParticleSystems");
		}
		if (this.RemoveNullsFromArray<ParticleSystem>(ref this.smokeParticleSystems))
		{
			this.LogNullsFoundInArray("smokeParticleSystems");
		}
		this.hasVolcanoAudioSrc = (this.volcanoAudioSource != null);
		this.hasForestSpeakerAudioSrc = (this.forestSpeakerAudioSrc != null);
		this.lavaSpewEmissionModules = new ParticleSystem.EmissionModule[this.lavaSpewParticleSystems.Length];
		this.lavaSpewEmissionDefaultRateMultipliers = new float[this.lavaSpewParticleSystems.Length];
		this.lavaSpewDefaultEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
		this.lavaSpewAdjustedEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
		for (int i = 0; i < this.lavaSpewParticleSystems.Length; i++)
		{
			ParticleSystem.EmissionModule emission = this.lavaSpewParticleSystems[i].emission;
			this.lavaSpewEmissionDefaultRateMultipliers[i] = emission.rateOverTimeMultiplier;
			this.lavaSpewDefaultEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
			this.lavaSpewAdjustedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
			for (int j = 0; j < emission.burstCount; j++)
			{
				ParticleSystem.Burst burst = emission.GetBurst(j);
				this.lavaSpewDefaultEmitBursts[i][j] = burst;
				this.lavaSpewAdjustedEmitBursts[i][j] = new ParticleSystem.Burst(burst.time, burst.minCount, burst.maxCount, burst.cycleCount, burst.repeatInterval);
				this.lavaSpewAdjustedEmitBursts[i][j].count = burst.count;
			}
			this.lavaSpewEmissionModules[i] = emission;
		}
		this.smokeMainModules = new ParticleSystem.MainModule[this.smokeParticleSystems.Length];
		this.smokeEmissionModules = new ParticleSystem.EmissionModule[this.smokeParticleSystems.Length];
		this.smokeEmissionDefaultRateMultipliers = new float[this.smokeParticleSystems.Length];
		for (int k = 0; k < this.smokeParticleSystems.Length; k++)
		{
			this.smokeMainModules[k] = this.smokeParticleSystems[k].main;
			this.smokeEmissionModules[k] = this.smokeParticleSystems[k].emission;
			this.smokeEmissionDefaultRateMultipliers[k] = this.smokeEmissionModules[k].rateOverTimeMultiplier;
		}
		this.InitState(this.drainedStateFX);
		this.InitState(this.eruptingStateFX);
		this.InitState(this.risingStateFX);
		this.InitState(this.fullStateFX);
		this.InitState(this.drainingStateFX);
		this.currentStateFX = this.drainedStateFX;
		this.UpdateDrainedState(0f);
	}

	// Token: 0x0600401E RID: 16414 RVA: 0x0015722C File Offset: 0x0015542C
	public void PreloadAssets()
	{
		VolcanoEffects.PreloadClip(this.warnVolcanoBellyEmptied);
		VolcanoEffects.PreloadClip(this.volcanoAcceptStone);
		VolcanoEffects.PreloadClip(this.volcanoAcceptLastStone);
		VolcanoEffects.PreloadStateFXClips(this.drainedStateFX);
		VolcanoEffects.PreloadStateFXClips(this.eruptingStateFX);
		VolcanoEffects.PreloadStateFXClips(this.risingStateFX);
		VolcanoEffects.PreloadStateFXClips(this.fullStateFX);
		VolcanoEffects.PreloadStateFXClips(this.drainingStateFX);
		VolcanoEffects.WarmUpAudioSourceGO(this.forestSpeakerAudioSrc);
		VolcanoEffects.WarmUpAudioSourceGO(this.volcanoAudioSource);
		VolcanoEffects.WarmUpStateFXSources(this.drainedStateFX);
		VolcanoEffects.WarmUpStateFXSources(this.eruptingStateFX);
		VolcanoEffects.WarmUpStateFXSources(this.risingStateFX);
		VolcanoEffects.WarmUpStateFXSources(this.fullStateFX);
		VolcanoEffects.WarmUpStateFXSources(this.drainingStateFX);
		for (int i = 0; i < this.lavaSurfaceAudioSrcs.Length; i++)
		{
			VolcanoEffects.WarmUpAudioSourceGO(this.lavaSurfaceAudioSrcs[i]);
		}
		if (this.prewarmCoroutine != null)
		{
			base.StopCoroutine(this.prewarmCoroutine);
		}
		this.prewarmCoroutine = base.StartCoroutine(this._PrewarmLavaSpewRenderers());
	}

	// Token: 0x0600401F RID: 16415 RVA: 0x00157324 File Offset: 0x00155524
	private static void PreloadClip(AudioClip clip)
	{
		if (clip != null && clip.loadState != AudioDataLoadState.Loaded)
		{
			clip.LoadAudioData();
		}
	}

	// Token: 0x06004020 RID: 16416 RVA: 0x00157340 File Offset: 0x00155540
	private static void PreloadStateFXClips(VolcanoEffects.LavaStateFX fx)
	{
		VolcanoEffects.PreloadClip(fx.startSound);
		VolcanoEffects.PreloadClip(fx.endSound);
		if (fx.loop1AudioSrc != null && fx.loop1AudioSrc.clip != null)
		{
			VolcanoEffects.PreloadClip(fx.loop1AudioSrc.clip);
		}
		if (fx.loop2AudioSrc != null && fx.loop2AudioSrc.clip != null)
		{
			VolcanoEffects.PreloadClip(fx.loop2AudioSrc.clip);
		}
	}

	// Token: 0x06004021 RID: 16417 RVA: 0x001573C8 File Offset: 0x001555C8
	private static void WarmUpAudioSourceGO(AudioSource src)
	{
		if (src == null)
		{
			return;
		}
		GameObject gameObject = src.gameObject;
		if (gameObject.activeSelf)
		{
			return;
		}
		gameObject.SetActive(true);
		gameObject.SetActive(false);
	}

	// Token: 0x06004022 RID: 16418 RVA: 0x00157400 File Offset: 0x00155600
	private static void WarmUpStateFXSources(VolcanoEffects.LavaStateFX fx)
	{
		if (fx.startSoundExists)
		{
			VolcanoEffects.WarmUpAudioSourceGO(fx.startSoundAudioSrc);
		}
		if (fx.endSoundExists)
		{
			VolcanoEffects.WarmUpAudioSourceGO(fx.endSoundAudioSrc);
		}
		if (fx.loop1Exists)
		{
			VolcanoEffects.WarmUpAudioSourceGO(fx.loop1AudioSrc);
		}
		if (fx.loop2Exists)
		{
			VolcanoEffects.WarmUpAudioSourceGO(fx.loop2AudioSrc);
		}
	}

	// Token: 0x06004023 RID: 16419 RVA: 0x00157459 File Offset: 0x00155659
	private IEnumerator _PrewarmLavaSpewRenderers()
	{
		for (int i = 0; i < this.lavaSpewParticleSystems.Length; i++)
		{
			this.lavaSpewParticleSystems[i].Emit(1);
		}
		yield return null;
		for (int j = 0; j < this.lavaSpewParticleSystems.Length; j++)
		{
			this.lavaSpewParticleSystems[j].Clear(true);
		}
		this.prewarmCoroutine = null;
		yield break;
	}

	// Token: 0x06004024 RID: 16420 RVA: 0x00157468 File Offset: 0x00155668
	private void OnDisable()
	{
		if (this.prewarmCoroutine != null)
		{
			base.StopCoroutine(this.prewarmCoroutine);
			this.prewarmCoroutine = null;
		}
	}

	// Token: 0x06004025 RID: 16421 RVA: 0x00157488 File Offset: 0x00155688
	public void OnVolcanoBellyEmpty()
	{
		if (!this.hasForestSpeakerAudioSrc)
		{
			return;
		}
		if (Time.time - this.timeVolcanoBellyWasLastEmpty < this.warnVolcanoBellyEmptied.length)
		{
			return;
		}
		this.forestSpeakerAudioSrc.gameObject.SetActive(true);
		this.forestSpeakerAudioSrc.GTPlayOneShot(this.warnVolcanoBellyEmptied, 1f);
	}

	// Token: 0x06004026 RID: 16422 RVA: 0x001574E0 File Offset: 0x001556E0
	public void OnStoneAccepted(float activationProgress)
	{
		if (!this.hasVolcanoAudioSrc)
		{
			return;
		}
		this.volcanoAudioSource.gameObject.SetActive(true);
		if (activationProgress > 1f)
		{
			this.volcanoAudioSource.GTPlayOneShot(this.volcanoAcceptLastStone, 1f);
			return;
		}
		this.volcanoAudioSource.GTPlayOneShot(this.volcanoAcceptStone, 1f);
	}

	// Token: 0x06004027 RID: 16423 RVA: 0x0015753C File Offset: 0x0015573C
	private void InitState(VolcanoEffects.LavaStateFX fx)
	{
		fx.startSoundExists = (fx.startSound != null);
		fx.endSoundExists = (fx.endSound != null);
		fx.loop1Exists = (fx.loop1AudioSrc != null);
		fx.loop2Exists = (fx.loop2AudioSrc != null);
		if (fx.loop1Exists)
		{
			fx.loop1DefaultVolume = fx.loop1AudioSrc.volume;
			fx.loop1AudioSrc.volume = 0f;
		}
		if (fx.loop2Exists)
		{
			fx.loop2DefaultVolume = fx.loop2AudioSrc.volume;
			fx.loop2AudioSrc.volume = 0f;
		}
	}

	// Token: 0x06004028 RID: 16424 RVA: 0x001575E4 File Offset: 0x001557E4
	private void SetLavaAudioEnabled(bool toEnable)
	{
		AudioSource[] array = this.lavaSurfaceAudioSrcs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(toEnable);
		}
	}

	// Token: 0x06004029 RID: 16425 RVA: 0x00157614 File Offset: 0x00155814
	private void SetLavaAudioEnabled(bool toEnable, float volume)
	{
		foreach (AudioSource audioSource in this.lavaSurfaceAudioSrcs)
		{
			audioSource.volume = volume;
			audioSource.gameObject.SetActive(toEnable);
		}
	}

	// Token: 0x0600402A RID: 16426 RVA: 0x0015764C File Offset: 0x0015584C
	private void ResetState()
	{
		if (this.currentStateFX == null)
		{
			return;
		}
		this.currentStateFX.startSoundPlayed = false;
		this.currentStateFX.endSoundPlayed = false;
		if (this.currentStateFX.startSoundExists)
		{
			this.currentStateFX.startSoundAudioSrc.gameObject.SetActive(false);
		}
		if (this.currentStateFX.endSoundExists)
		{
			this.currentStateFX.endSoundAudioSrc.gameObject.SetActive(false);
		}
		if (this.currentStateFX.loop1Exists)
		{
			this.currentStateFX.loop1AudioSrc.gameObject.SetActive(false);
		}
		if (this.currentStateFX.loop2Exists)
		{
			this.currentStateFX.loop2AudioSrc.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600402B RID: 16427 RVA: 0x00157708 File Offset: 0x00155908
	private void UpdateState(float time, float timeRemaining, float progress)
	{
		if (this.currentStateFX == null)
		{
			return;
		}
		if (this.currentStateFX.startSoundExists && !this.currentStateFX.startSoundPlayed && time >= this.currentStateFX.startSoundDelay)
		{
			this.currentStateFX.startSoundPlayed = true;
			this.currentStateFX.startSoundAudioSrc.gameObject.SetActive(true);
			this.currentStateFX.startSoundAudioSrc.GTPlayOneShot(this.currentStateFX.startSound, this.currentStateFX.startSoundVol);
		}
		if (this.currentStateFX.endSoundExists && !this.currentStateFX.endSoundPlayed && timeRemaining <= this.currentStateFX.endSound.length + this.currentStateFX.endSoundPadTime)
		{
			this.currentStateFX.endSoundPlayed = true;
			this.currentStateFX.endSoundAudioSrc.gameObject.SetActive(true);
			this.currentStateFX.endSoundAudioSrc.GTPlayOneShot(this.currentStateFX.endSound, this.currentStateFX.endSoundVol);
		}
		if (this.currentStateFX.loop1Exists)
		{
			this.currentStateFX.loop1AudioSrc.volume = this.currentStateFX.loop1VolAnim.Evaluate(progress) * this.currentStateFX.loop1DefaultVolume;
			if (!this.currentStateFX.loop1AudioSrc.isPlaying)
			{
				this.currentStateFX.loop1AudioSrc.gameObject.SetActive(true);
				this.currentStateFX.loop1AudioSrc.GTPlay();
			}
		}
		if (this.currentStateFX.loop2Exists)
		{
			this.currentStateFX.loop2AudioSrc.volume = this.currentStateFX.loop2VolAnim.Evaluate(progress) * this.currentStateFX.loop2DefaultVolume;
			if (!this.currentStateFX.loop2AudioSrc.isPlaying)
			{
				this.currentStateFX.loop2AudioSrc.gameObject.SetActive(true);
				this.currentStateFX.loop2AudioSrc.GTPlay();
			}
		}
		for (int i = 0; i < this.smokeMainModules.Length; i++)
		{
			this.smokeMainModules[i].startColor = this.currentStateFX.smokeStartColorAnim.Evaluate(progress);
			this.smokeEmissionModules[i].rateOverTimeMultiplier = this.currentStateFX.smokeEmissionAnim.Evaluate(progress) * this.smokeEmissionDefaultRateMultipliers[i];
		}
		this.SetParticleEmissionRateAndBurst(this.currentStateFX.lavaSpewEmissionAnim.Evaluate(progress), this.lavaSpewEmissionModules, this.lavaSpewEmissionDefaultRateMultipliers, this.lavaSpewDefaultEmitBursts, this.lavaSpewAdjustedEmitBursts);
		if (this.applyShaderGlobals)
		{
			Shader.SetGlobalColor(this.shaderProp_ZoneLiquidLightColor, this.currentStateFX.lavaLightColor.Evaluate(progress) * this.currentStateFX.lavaLightIntensityAnim.Evaluate(progress));
			Shader.SetGlobalFloat(this.shaderProp_ZoneLiquidLightDistScale, this.currentStateFX.lavaLightAttenuationAnim.Evaluate(progress));
		}
	}

	// Token: 0x0600402C RID: 16428 RVA: 0x001579DD File Offset: 0x00155BDD
	public void SetDrainedState()
	{
		this.ResetState();
		this.SetLavaAudioEnabled(false);
		this.currentStateFX = this.drainedStateFX;
	}

	// Token: 0x0600402D RID: 16429 RVA: 0x001579F8 File Offset: 0x00155BF8
	public void UpdateDrainedState(float time)
	{
		this.UpdateState(time, float.MaxValue, float.MinValue);
	}

	// Token: 0x0600402E RID: 16430 RVA: 0x00157A0B File Offset: 0x00155C0B
	public void SetEruptingState()
	{
		this.ResetState();
		this.SetLavaAudioEnabled(false, 0f);
		this.currentStateFX = this.eruptingStateFX;
	}

	// Token: 0x0600402F RID: 16431 RVA: 0x00157A2B File Offset: 0x00155C2B
	public void UpdateEruptingState(float time, float timeRemaining, float progress)
	{
		this.UpdateState(time, timeRemaining, progress);
	}

	// Token: 0x06004030 RID: 16432 RVA: 0x00157A36 File Offset: 0x00155C36
	public void SetRisingState()
	{
		this.ResetState();
		this.SetLavaAudioEnabled(true, 0f);
		this.currentStateFX = this.risingStateFX;
	}

	// Token: 0x06004031 RID: 16433 RVA: 0x00157A58 File Offset: 0x00155C58
	public void UpdateRisingState(float time, float timeRemaining, float progress)
	{
		this.UpdateState(time, timeRemaining, progress);
		AudioSource[] array = this.lavaSurfaceAudioSrcs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].volume = Mathf.Lerp(0f, 1f, Mathf.Clamp01(time));
		}
	}

	// Token: 0x06004032 RID: 16434 RVA: 0x00157AA0 File Offset: 0x00155CA0
	public void SetFullState()
	{
		this.ResetState();
		this.SetLavaAudioEnabled(true, 1f);
		this.currentStateFX = this.fullStateFX;
	}

	// Token: 0x06004033 RID: 16435 RVA: 0x00157A2B File Offset: 0x00155C2B
	public void UpdateFullState(float time, float timeRemaining, float progress)
	{
		this.UpdateState(time, timeRemaining, progress);
	}

	// Token: 0x06004034 RID: 16436 RVA: 0x00157AC0 File Offset: 0x00155CC0
	public void SetDrainingState()
	{
		this.ResetState();
		this.SetLavaAudioEnabled(true, 1f);
		this.currentStateFX = this.drainingStateFX;
	}

	// Token: 0x06004035 RID: 16437 RVA: 0x00157AE0 File Offset: 0x00155CE0
	public void UpdateDrainingState(float time, float timeRemaining, float progress)
	{
		this.UpdateState(time, timeRemaining, progress);
		AudioSource[] array = this.lavaSurfaceAudioSrcs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].volume = Mathf.Lerp(1f, 0f, progress);
		}
	}

	// Token: 0x06004036 RID: 16438 RVA: 0x00157B24 File Offset: 0x00155D24
	private void SetParticleEmissionRateAndBurst(float multiplier, ParticleSystem.EmissionModule[] emissionModules, float[] defaultRateMultipliers, ParticleSystem.Burst[][] defaultEmitBursts, ParticleSystem.Burst[][] adjustedEmitBursts)
	{
		for (int i = 0; i < emissionModules.Length; i++)
		{
			emissionModules[i].rateOverTimeMultiplier = multiplier * defaultRateMultipliers[i];
			int num = Mathf.Min(emissionModules[i].burstCount, defaultEmitBursts[i].Length);
			for (int j = 0; j < num; j++)
			{
				adjustedEmitBursts[i][j].probability = defaultEmitBursts[i][j].probability * multiplier;
			}
			emissionModules[i].SetBursts(adjustedEmitBursts[i]);
		}
	}

	// Token: 0x06004037 RID: 16439 RVA: 0x00157BA4 File Offset: 0x00155DA4
	private bool RemoveNullsFromArray<T>(ref T[] array) where T : Object
	{
		List<T> list = new List<T>(array.Length);
		foreach (T t in array)
		{
			if (t != null)
			{
				list.Add(t);
			}
		}
		int num = array.Length;
		array = list.ToArray();
		return num != array.Length;
	}

	// Token: 0x06004038 RID: 16440 RVA: 0x00157BFE File Offset: 0x00155DFE
	private void LogNullsFoundInArray(string nameOfArray)
	{
		Debug.LogError(string.Concat(new string[]
		{
			"Null reference found in ",
			nameOfArray,
			" array of component: \"",
			this.GetComponentPath(int.MaxValue),
			"\""
		}), this);
	}

	// Token: 0x040050A1 RID: 20641
	[Tooltip("Only one VolcanoEffects should change shader globals in the scene (lava color, lava light) at a time.")]
	[SerializeField]
	private bool applyShaderGlobals = true;

	// Token: 0x040050A2 RID: 20642
	[Tooltip("Game trigger notification sounds will play through this.")]
	[SerializeField]
	private AudioSource forestSpeakerAudioSrc;

	// Token: 0x040050A3 RID: 20643
	[Tooltip("The accumulator value of rocks being thrown into the volcano has been reset.")]
	[SerializeField]
	private AudioClip warnVolcanoBellyEmptied;

	// Token: 0x040050A4 RID: 20644
	[Tooltip("Accept stone sounds will play through here.")]
	[SerializeField]
	private AudioSource volcanoAudioSource;

	// Token: 0x040050A5 RID: 20645
	[Tooltip("volcano ate rock but needs more.")]
	[SerializeField]
	private AudioClip volcanoAcceptStone;

	// Token: 0x040050A6 RID: 20646
	[Tooltip("volcano ate last needed rock.")]
	[SerializeField]
	private AudioClip volcanoAcceptLastStone;

	// Token: 0x040050A7 RID: 20647
	[Tooltip("This will be faded in while lava is rising.")]
	[SerializeField]
	private AudioSource[] lavaSurfaceAudioSrcs;

	// Token: 0x040050A8 RID: 20648
	[Tooltip("Emission will be adjusted for these particles during eruption.")]
	[SerializeField]
	private ParticleSystem[] lavaSpewParticleSystems;

	// Token: 0x040050A9 RID: 20649
	[Tooltip("Smoke emits during all states but it's intensity and color will change when erupting/idling.")]
	[SerializeField]
	private ParticleSystem[] smokeParticleSystems;

	// Token: 0x040050AA RID: 20650
	[SerializeField]
	private VolcanoEffects.LavaStateFX drainedStateFX;

	// Token: 0x040050AB RID: 20651
	[SerializeField]
	private VolcanoEffects.LavaStateFX eruptingStateFX;

	// Token: 0x040050AC RID: 20652
	[SerializeField]
	private VolcanoEffects.LavaStateFX risingStateFX;

	// Token: 0x040050AD RID: 20653
	[SerializeField]
	private VolcanoEffects.LavaStateFX fullStateFX;

	// Token: 0x040050AE RID: 20654
	[SerializeField]
	private VolcanoEffects.LavaStateFX drainingStateFX;

	// Token: 0x040050AF RID: 20655
	private VolcanoEffects.LavaStateFX currentStateFX;

	// Token: 0x040050B0 RID: 20656
	private ParticleSystem.EmissionModule[] lavaSpewEmissionModules;

	// Token: 0x040050B1 RID: 20657
	private float[] lavaSpewEmissionDefaultRateMultipliers;

	// Token: 0x040050B2 RID: 20658
	private ParticleSystem.Burst[][] lavaSpewDefaultEmitBursts;

	// Token: 0x040050B3 RID: 20659
	private ParticleSystem.Burst[][] lavaSpewAdjustedEmitBursts;

	// Token: 0x040050B4 RID: 20660
	private ParticleSystem.MainModule[] smokeMainModules;

	// Token: 0x040050B5 RID: 20661
	private ParticleSystem.EmissionModule[] smokeEmissionModules;

	// Token: 0x040050B6 RID: 20662
	private float[] smokeEmissionDefaultRateMultipliers;

	// Token: 0x040050B7 RID: 20663
	private readonly int shaderProp_ZoneLiquidLightColor = Shader.PropertyToID("_ZoneLiquidLightColor");

	// Token: 0x040050B8 RID: 20664
	private readonly int shaderProp_ZoneLiquidLightDistScale = Shader.PropertyToID("_ZoneLiquidLightDistScale");

	// Token: 0x040050B9 RID: 20665
	private float timeVolcanoBellyWasLastEmpty;

	// Token: 0x040050BA RID: 20666
	private bool hasVolcanoAudioSrc;

	// Token: 0x040050BB RID: 20667
	private bool hasForestSpeakerAudioSrc;

	// Token: 0x040050BC RID: 20668
	private Coroutine prewarmCoroutine;

	// Token: 0x020009CA RID: 2506
	[Serializable]
	public class LavaStateFX
	{
		// Token: 0x040050BD RID: 20669
		public AudioClip startSound;

		// Token: 0x040050BE RID: 20670
		public AudioSource startSoundAudioSrc;

		// Token: 0x040050BF RID: 20671
		[Tooltip("Multiplied by the AudioSource's volume.")]
		public float startSoundVol = 1f;

		// Token: 0x040050C0 RID: 20672
		[FormerlySerializedAs("startSoundPad")]
		public float startSoundDelay;

		// Token: 0x040050C1 RID: 20673
		public AudioClip endSound;

		// Token: 0x040050C2 RID: 20674
		public AudioSource endSoundAudioSrc;

		// Token: 0x040050C3 RID: 20675
		[Tooltip("Multiplied by the AudioSource's volume.")]
		public float endSoundVol = 1f;

		// Token: 0x040050C4 RID: 20676
		[Tooltip("How much time should there be between the end of the clip playing and the end of the state.")]
		public float endSoundPadTime;

		// Token: 0x040050C5 RID: 20677
		public AudioSource loop1AudioSrc;

		// Token: 0x040050C6 RID: 20678
		public AnimationCurve loop1VolAnim;

		// Token: 0x040050C7 RID: 20679
		public AudioSource loop2AudioSrc;

		// Token: 0x040050C8 RID: 20680
		public AnimationCurve loop2VolAnim;

		// Token: 0x040050C9 RID: 20681
		public AnimationCurve lavaSpewEmissionAnim;

		// Token: 0x040050CA RID: 20682
		public AnimationCurve smokeEmissionAnim;

		// Token: 0x040050CB RID: 20683
		public Gradient smokeStartColorAnim;

		// Token: 0x040050CC RID: 20684
		public Gradient lavaLightColor;

		// Token: 0x040050CD RID: 20685
		public AnimationCurve lavaLightIntensityAnim = AnimationCurve.Constant(0f, 1f, 60f);

		// Token: 0x040050CE RID: 20686
		public AnimationCurve lavaLightAttenuationAnim = AnimationCurve.Constant(0f, 1f, 0.1f);

		// Token: 0x040050CF RID: 20687
		[NonSerialized]
		public bool startSoundExists;

		// Token: 0x040050D0 RID: 20688
		[NonSerialized]
		public bool startSoundPlayed;

		// Token: 0x040050D1 RID: 20689
		[NonSerialized]
		public bool endSoundExists;

		// Token: 0x040050D2 RID: 20690
		[NonSerialized]
		public bool endSoundPlayed;

		// Token: 0x040050D3 RID: 20691
		[NonSerialized]
		public bool loop1Exists;

		// Token: 0x040050D4 RID: 20692
		[NonSerialized]
		public float loop1DefaultVolume;

		// Token: 0x040050D5 RID: 20693
		[NonSerialized]
		public bool loop2Exists;

		// Token: 0x040050D6 RID: 20694
		[NonSerialized]
		public float loop2DefaultVolume;
	}
}
