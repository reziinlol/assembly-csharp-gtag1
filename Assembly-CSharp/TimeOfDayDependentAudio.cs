using System;
using UnityEngine;

// Token: 0x020003A3 RID: 931
public class TimeOfDayDependentAudio : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
{
	// Token: 0x06001689 RID: 5769 RVA: 0x000825C4 File Offset: 0x000807C4
	private void Awake()
	{
		this.stepTime = 1f;
		if (this.myParticleSystem != null)
		{
			this.myEmissionModule = this.myParticleSystem.emission;
			this.startingEmissionRate = this.myEmissionModule.rateOverTime.constant;
		}
		if (this.isModified)
		{
			this.positionMultiplier = this.positionMultiplierSet;
		}
		else
		{
			this.positionMultiplier = 1f;
		}
		if (this.volumes == null)
		{
			this.volumes = new float[10];
		}
	}

	// Token: 0x0600168A RID: 5770 RVA: 0x0008264A File Offset: 0x0008084A
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x0600168B RID: 5771 RVA: 0x00082653 File Offset: 0x00080853
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x0600168C RID: 5772 RVA: 0x0008265D File Offset: 0x0008085D
	public void SliceUpdate()
	{
		this.isModified = false;
		this.UpdateTimeOfDay();
	}

	// Token: 0x0600168D RID: 5773 RVA: 0x0008266C File Offset: 0x0008086C
	private void UpdateTimeOfDay()
	{
		if (BetterDayNightManager.instance == null)
		{
			return;
		}
		BetterDayNightManager.WeatherType weatherType = BetterDayNightManager.instance.CurrentWeather();
		BetterDayNightManager.WeatherType weatherType2 = BetterDayNightManager.instance.NextWeather();
		bool flag = this.myWeather == BetterDayNightManager.WeatherType.All || this.myWeather == weatherType || this.myWeather == weatherType2;
		bool flag2 = this.myWeather != BetterDayNightManager.WeatherType.All && weatherType != weatherType2;
		int currentTimeIndex = BetterDayNightManager.instance.currentTimeIndex;
		int num = (currentTimeIndex + 1) % BetterDayNightManager.instance.timeOfDayRange.Length;
		int num2 = (currentTimeIndex - 1) % BetterDayNightManager.instance.timeOfDayRange.Length;
		if (num2 < 0)
		{
			num2 = BetterDayNightManager.instance.timeOfDayRange.Length - 1;
		}
		float currentLerp = BetterDayNightManager.instance.currentLerp;
		if (!flag)
		{
			if (this.dependentStuff.activeSelf)
			{
				this.dependentStuff.SetActive(false);
			}
			return;
		}
		if (!this.dependentStuff.activeSelf && (!this.includesAudio || this.dependentStuff != this.timeOfDayDependent))
		{
			this.dependentStuff.SetActive(true);
		}
		if (this.includesAudio && this.timeOfDayDependent != null)
		{
			bool flag3 = this.volumes[currentTimeIndex] != 0f;
			if (this.timeOfDayDependent.activeSelf != flag3)
			{
				this.timeOfDayDependent.SetActive(flag3);
			}
		}
		if (!flag2)
		{
			this.newRate = this.startingEmissionRate;
			this.currentVolume = Mathf.Lerp(this.volumes[num2], this.volumes[currentTimeIndex], Mathf.Clamp(currentLerp * 20f, 0f, 1f));
		}
		else if (this.myWeather == weatherType2)
		{
			float t = Mathf.Clamp(currentLerp * 2f - 1f, 0f, 1f);
			this.newRate = Mathf.Lerp(0f, this.startingEmissionRate, t);
			this.currentVolume = Mathf.Lerp(0f, this.volumes[num], currentLerp);
		}
		else
		{
			float t2 = Mathf.Clamp(currentLerp * 2f, 0f, 1f);
			this.newRate = Mathf.Lerp(this.startingEmissionRate, 0f, t2);
			this.currentVolume = Mathf.Lerp(this.volumes[currentTimeIndex], 0f, currentLerp);
		}
		if (this.myParticleSystem != null)
		{
			this.myEmissionModule = this.myParticleSystem.emission;
			this.myEmissionModule.rateOverTime = this.newRate;
			bool flag4 = this.newRate != 0f;
			if (this.myParticleSystem.gameObject.activeSelf != flag4)
			{
				this.myParticleSystem.gameObject.SetActive(flag4);
			}
		}
		if (this.includesAudio)
		{
			for (int i = 0; i < this.audioSources.Length; i++)
			{
				MusicSource component = this.audioSources[i].gameObject.GetComponent<MusicSource>();
				if (!(component != null) || !component.VolumeOverridden)
				{
					this.audioSources[i].volume = this.currentVolume * this.positionMultiplier;
					this.audioSources[i].enabled = (this.currentVolume != 0f);
				}
			}
		}
	}

	// Token: 0x0600168E RID: 5774 RVA: 0x000829A4 File Offset: 0x00080BA4
	public bool BuildValidationCheck()
	{
		for (int i = 0; i < this.audioSources.Length; i++)
		{
			if (this.audioSources[i] == null)
			{
				Debug.LogError("audio source array contains null references", this);
				return false;
			}
		}
		if (this.volumes.Length != 10)
		{
			Debug.LogError("volumes array is the wrong length! you'll pay for this!", this);
			return false;
		}
		return true;
	}

	// Token: 0x0400209B RID: 8347
	public AudioSource[] audioSources;

	// Token: 0x0400209C RID: 8348
	public float[] volumes;

	// Token: 0x0400209D RID: 8349
	public float currentVolume;

	// Token: 0x0400209E RID: 8350
	public float stepTime;

	// Token: 0x0400209F RID: 8351
	public BetterDayNightManager.WeatherType myWeather;

	// Token: 0x040020A0 RID: 8352
	public GameObject dependentStuff;

	// Token: 0x040020A1 RID: 8353
	public GameObject timeOfDayDependent;

	// Token: 0x040020A2 RID: 8354
	public bool includesAudio;

	// Token: 0x040020A3 RID: 8355
	public ParticleSystem myParticleSystem;

	// Token: 0x040020A4 RID: 8356
	private float startingEmissionRate;

	// Token: 0x040020A5 RID: 8357
	private ParticleSystem.MinMaxCurve newCurve;

	// Token: 0x040020A6 RID: 8358
	private ParticleSystem.EmissionModule myEmissionModule;

	// Token: 0x040020A7 RID: 8359
	private float newRate;

	// Token: 0x040020A8 RID: 8360
	public float positionMultiplierSet;

	// Token: 0x040020A9 RID: 8361
	public float positionMultiplier = 1f;

	// Token: 0x040020AA RID: 8362
	public bool isModified;
}
