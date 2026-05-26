using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020009F2 RID: 2546
public class BetterDayNightManager : MonoBehaviour, IGorillaSliceableSimple, ITimeOfDaySystem
{
	// Token: 0x06004128 RID: 16680 RVA: 0x0015C9EC File Offset: 0x0015ABEC
	public static void Register(PerSceneRenderData data)
	{
		BetterDayNightManager.allScenesRenderData.Add(data);
	}

	// Token: 0x06004129 RID: 16681 RVA: 0x0015C9F9 File Offset: 0x0015ABF9
	public static void Unregister(PerSceneRenderData data)
	{
		BetterDayNightManager.allScenesRenderData.Remove(data);
	}

	// Token: 0x1700060E RID: 1550
	// (get) Token: 0x0600412A RID: 16682 RVA: 0x0015CA07 File Offset: 0x0015AC07
	public double[] timeOfDayRange
	{
		get
		{
			if (this.currentSeason == BetterDayNightManager.Season.Winter)
			{
				return this.winterTimeOfDayRange;
			}
			return this.summerTimeOfDayRange;
		}
	}

	// Token: 0x1700060F RID: 1551
	// (get) Token: 0x0600412B RID: 16683 RVA: 0x0015CA1E File Offset: 0x0015AC1E
	// (set) Token: 0x0600412C RID: 16684 RVA: 0x0015CA26 File Offset: 0x0015AC26
	public string currentTimeOfDay { get; private set; }

	// Token: 0x17000610 RID: 1552
	// (get) Token: 0x0600412D RID: 16685 RVA: 0x0015CA2F File Offset: 0x0015AC2F
	public float NormalizedTimeOfDay
	{
		get
		{
			return Mathf.Clamp01((float)((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds / this.totalSeconds));
		}
	}

	// Token: 0x17000611 RID: 1553
	// (get) Token: 0x0600412E RID: 16686 RVA: 0x0015CA59 File Offset: 0x0015AC59
	double ITimeOfDaySystem.currentTimeInSeconds
	{
		get
		{
			return this.currentTime;
		}
	}

	// Token: 0x17000612 RID: 1554
	// (get) Token: 0x0600412F RID: 16687 RVA: 0x0015CA61 File Offset: 0x0015AC61
	double ITimeOfDaySystem.totalTimeInSeconds
	{
		get
		{
			return this.totalSeconds;
		}
	}

	// Token: 0x06004130 RID: 16688 RVA: 0x0015CA6C File Offset: 0x0015AC6C
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (BetterDayNightManager.instance == null)
		{
			BetterDayNightManager.instance = this;
		}
		else if (BetterDayNightManager.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.currentLerp = 0f;
		this.totalHours = 0.0;
		for (int i = 0; i < this.timeOfDayRange.Length; i++)
		{
			this.totalHours += this.timeOfDayRange[i];
		}
		this.totalSeconds = this.totalHours * 60.0 * 60.0;
		this.currentTimeIndex = 0;
		this.baseSeconds = 0.0;
		this.computerInit = false;
		this.randomNumberGenerator = new Random(this.mySeed);
		this.GenerateWeatherEventTimes();
		this.ChangeMaps(0, 1);
		base.StartCoroutine(this.InitialUpdate());
	}

	// Token: 0x06004131 RID: 16689 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004132 RID: 16690 RVA: 0x0015CB60 File Offset: 0x0015AD60
	public void UpdateTimeOfDay()
	{
		if (Time.time < this.lastTimeChecked + this.currentTimestep)
		{
			return;
		}
		this.lastTimeChecked = Time.time;
		if (this.animatingLightFlash != null)
		{
			return;
		}
		try
		{
			if (!this.computerInit && GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
			{
				this.computerInit = true;
				this.initialDayCycles = (long)(TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * this.timeMultiplier / this.totalSeconds);
				this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmapNames.Length) % this.weatherCycle.Length;
				this.baseSeconds = TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * this.timeMultiplier % this.totalSeconds;
				this.currentTime = (this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds;
				this.currentIndexSeconds = 0.0;
				for (int i = 0; i < this.timeOfDayRange.Length; i++)
				{
					this.currentIndexSeconds += this.timeOfDayRange[i] * 3600.0;
					if (this.currentIndexSeconds > this.currentTime)
					{
						this.currentTimeIndex = i;
						break;
					}
				}
				this.currentWeatherIndex += this.currentTimeIndex;
			}
			else if (!this.computerInit && this.baseSeconds == 0.0)
			{
				this.initialDayCycles = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * this.timeMultiplier / this.totalSeconds);
				this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmapNames.Length) % this.weatherCycle.Length;
				this.baseSeconds = TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * this.timeMultiplier % this.totalSeconds;
				this.currentTime = this.baseSeconds % this.totalSeconds;
				this.currentIndexSeconds = 0.0;
				for (int j = 0; j < this.timeOfDayRange.Length; j++)
				{
					this.currentIndexSeconds += this.timeOfDayRange[j] * 3600.0;
					if (this.currentIndexSeconds > this.currentTime)
					{
						this.currentTimeIndex = j;
						break;
					}
				}
				this.currentWeatherIndex += this.currentTimeIndex - 1;
				if (this.currentWeatherIndex < 0)
				{
					this.currentWeatherIndex = this.weatherCycle.Length - 1;
				}
			}
			this.currentTime = ((this.currentSetting == TimeSettings.Normal) ? ((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds) : this.currentTime);
			this.currentIndexSeconds = 0.0;
			for (int k = 0; k < this.timeOfDayRange.Length; k++)
			{
				this.currentIndexSeconds += this.timeOfDayRange[k] * 3600.0;
				if (this.currentIndexSeconds > this.currentTime)
				{
					this.currentTimeIndex = k;
					break;
				}
			}
			if (this.timeIndexOverrideFunc != null)
			{
				this.currentTimeIndex = this.timeIndexOverrideFunc(this.currentTimeIndex);
			}
			if (this.currentTimeIndex != this.lastIndex)
			{
				this.currentWeatherIndex = (this.currentWeatherIndex + 1) % this.weatherCycle.Length;
				this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
			}
			this.currentLerp = (float)(1.0 - (this.currentIndexSeconds - this.currentTime) / (this.timeOfDayRange[this.currentTimeIndex] * 3600.0));
			Shader.SetGlobalFloat(this._GT_DayCycleTimeProgress, this.NormalizedTimeOfDay);
			this.ChangeLerps(this.currentLerp);
			this.lastIndex = this.currentTimeIndex;
			this.currentTimeOfDay = this.dayNightLightmapNames[this.currentTimeIndex];
		}
		catch (Exception ex)
		{
			string str = "Error in BetterDayNightManager: ";
			Exception ex2 = ex;
			Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null), this);
		}
		this.gameEpochDay = (long)((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) / this.totalSeconds + (double)this.initialDayCycles);
		foreach (BetterDayNightManager.ScheduledEvent scheduledEvent in BetterDayNightManager.scheduledEvents.Values)
		{
			if (scheduledEvent.lastDayCalled != this.gameEpochDay && scheduledEvent.hour == this.currentTimeIndex)
			{
				scheduledEvent.lastDayCalled = this.gameEpochDay;
				scheduledEvent.action();
			}
		}
	}

	// Token: 0x06004133 RID: 16691 RVA: 0x0015D068 File Offset: 0x0015B268
	private void ChangeLerps(float newLerp)
	{
		Shader.SetGlobalFloat(this._GlobalDayNightLerpValue, newLerp);
		Shader.SetGlobalFloat(this._GT_DayCycleBrightnessOption1_Id, Mathf.Lerp(this.colorFrom, this.colorTo, newLerp));
		Shader.SetGlobalFloat(this._GT_DayCycleBrightnessOption2_Id, Mathf.Lerp(this.colorFromDarker, this.colorToDarker, newLerp));
	}

	// Token: 0x06004134 RID: 16692 RVA: 0x0015D0CC File Offset: 0x0015B2CC
	private void ChangeMaps(int fromIndex, int toIndex)
	{
		this.fromWeatherIndex = this.currentWeatherIndex;
		this.toWeatherIndex = (this.currentWeatherIndex + 1) % this.weatherCycle.Length;
		if (this.weatherCycle[this.fromWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			this.fromSky = this.dayNightWeatherSkyboxTextures[fromIndex];
		}
		else
		{
			this.fromSky = this.dayNightSkyboxTextures[fromIndex];
		}
		this.fromSky2 = this.cloudsDayNightSkyboxTextures[fromIndex];
		this.fromSky3 = this.beachDayNightSkyboxTextures[fromIndex];
		if (this.weatherCycle[this.toWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			this.toSky = this.dayNightWeatherSkyboxTextures[toIndex];
		}
		else
		{
			this.toSky = this.dayNightSkyboxTextures[toIndex];
		}
		this.toSky2 = this.cloudsDayNightSkyboxTextures[toIndex];
		this.toSky3 = this.beachDayNightSkyboxTextures[toIndex];
		this.PopulateAllLightmaps(fromIndex, toIndex);
		Shader.SetGlobalTexture(this._GlobalDayNightSkyTex1, this.fromSky);
		Shader.SetGlobalTexture(this._GlobalDayNightSkyTex2, this.toSky);
		Shader.SetGlobalTexture(this._GlobalDayNightSky2Tex1, this.fromSky2);
		Shader.SetGlobalTexture(this._GlobalDayNightSky2Tex2, this.toSky2);
		Shader.SetGlobalTexture(this._GlobalDayNightSky3Tex1, this.fromSky3);
		Shader.SetGlobalTexture(this._GlobalDayNightSky3Tex2, this.toSky3);
		this.colorFrom = this.standardUnlitColor[fromIndex];
		this.colorTo = this.standardUnlitColor[toIndex];
		this.colorFromDarker = this.standardUnlitColorWithPremadeColorDarker[fromIndex];
		this.colorToDarker = this.standardUnlitColorWithPremadeColorDarker[toIndex];
	}

	// Token: 0x06004135 RID: 16693 RVA: 0x0015D254 File Offset: 0x0015B454
	public void SliceUpdate()
	{
		if (!this.shouldRepopulate)
		{
			using (List<PerSceneRenderData>.Enumerator enumerator = BetterDayNightManager.allScenesRenderData.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CheckShouldRepopulate())
					{
						this.shouldRepopulate = true;
					}
				}
			}
		}
		if (this.shouldRepopulate)
		{
			this.PopulateAllLightmaps();
			this.shouldRepopulate = false;
		}
		this.UpdateTimeOfDay();
	}

	// Token: 0x06004136 RID: 16694 RVA: 0x0015D2D0 File Offset: 0x0015B4D0
	private IEnumerator InitialUpdate()
	{
		yield return null;
		this.SliceUpdate();
		yield break;
	}

	// Token: 0x06004137 RID: 16695 RVA: 0x0015D2DF File Offset: 0x0015B4DF
	public void RequestRepopulateLightmaps()
	{
		this.shouldRepopulate = true;
	}

	// Token: 0x06004138 RID: 16696 RVA: 0x0015D2E8 File Offset: 0x0015B4E8
	public void PopulateAllLightmaps()
	{
		this.PopulateAllLightmaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	// Token: 0x06004139 RID: 16697 RVA: 0x0015D308 File Offset: 0x0015B508
	public void PopulateAllLightmaps(int fromIndex, int toIndex)
	{
		string fromTimeOfDay;
		if (this.weatherCycle[this.fromWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			fromTimeOfDay = this.dayNightWeatherLightmapNames[fromIndex];
		}
		else
		{
			fromTimeOfDay = this.dayNightLightmapNames[fromIndex];
		}
		string toTimeOfDay;
		if (this.weatherCycle[this.toWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			toTimeOfDay = this.dayNightWeatherLightmapNames[toIndex];
		}
		else
		{
			toTimeOfDay = this.dayNightLightmapNames[toIndex];
		}
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		foreach (PerSceneRenderData perSceneRenderData in BetterDayNightManager.allScenesRenderData)
		{
			perSceneRenderData.PopulateLightmaps(fromTimeOfDay, toTimeOfDay, lightmaps);
		}
		LightmapSettings.lightmaps = lightmaps;
	}

	// Token: 0x0600413A RID: 16698 RVA: 0x0015D3B0 File Offset: 0x0015B5B0
	public BetterDayNightManager.WeatherType CurrentWeather()
	{
		if (!this.overrideWeather)
		{
			return this.weatherCycle[this.currentWeatherIndex];
		}
		return this.overrideWeatherType;
	}

	// Token: 0x0600413B RID: 16699 RVA: 0x0015D3CE File Offset: 0x0015B5CE
	public BetterDayNightManager.WeatherType NextWeather()
	{
		if (!this.overrideWeather)
		{
			return this.weatherCycle[(this.currentWeatherIndex + 1) % this.weatherCycle.Length];
		}
		return this.overrideWeatherType;
	}

	// Token: 0x0600413C RID: 16700 RVA: 0x0015D3F7 File Offset: 0x0015B5F7
	public BetterDayNightManager.WeatherType LastWeather()
	{
		if (!this.overrideWeather)
		{
			return this.weatherCycle[(this.currentWeatherIndex - 1) % this.weatherCycle.Length];
		}
		return this.overrideWeatherType;
	}

	// Token: 0x0600413D RID: 16701 RVA: 0x0015D420 File Offset: 0x0015B620
	private void GenerateWeatherEventTimes()
	{
		this.weatherCycle = new BetterDayNightManager.WeatherType[100 * this.dayNightLightmapNames.Length];
		this.rainChance = this.rainChance * 2f / (float)this.maxRainDuration;
		for (int i = 1; i < this.weatherCycle.Length; i++)
		{
			this.weatherCycle[i] = (((float)this.randomNumberGenerator.Next(100) < this.rainChance * 100f) ? BetterDayNightManager.WeatherType.Raining : BetterDayNightManager.WeatherType.None);
			if (this.weatherCycle[i] == BetterDayNightManager.WeatherType.Raining)
			{
				this.rainDuration = this.randomNumberGenerator.Next(1, this.maxRainDuration + 1);
				for (int j = 1; j < this.rainDuration; j++)
				{
					if (i + j < this.weatherCycle.Length)
					{
						this.weatherCycle[i + j] = BetterDayNightManager.WeatherType.Raining;
					}
				}
				i += this.rainDuration - 1;
			}
		}
	}

	// Token: 0x0600413E RID: 16702 RVA: 0x0015D4F8 File Offset: 0x0015B6F8
	public static int RegisterScheduledEvent(int hour, Action action)
	{
		int num = (int)(DateTime.Now.Ticks % 2147483647L);
		while (BetterDayNightManager.scheduledEvents.ContainsKey(num))
		{
			num++;
		}
		BetterDayNightManager.scheduledEvents.Add(num, new BetterDayNightManager.ScheduledEvent
		{
			lastDayCalled = -1L,
			hour = hour,
			action = action
		});
		return num;
	}

	// Token: 0x0600413F RID: 16703 RVA: 0x0015D555 File Offset: 0x0015B755
	public static void UnregisterScheduledEvent(int id)
	{
		BetterDayNightManager.scheduledEvents.Remove(id);
	}

	// Token: 0x06004140 RID: 16704 RVA: 0x0015D563 File Offset: 0x0015B763
	public void SetTimeIndexOverrideFunction(Func<int, int> overrideFunction)
	{
		this.timeIndexOverrideFunc = overrideFunction;
	}

	// Token: 0x06004141 RID: 16705 RVA: 0x0015D56C File Offset: 0x0015B76C
	public void UnsetTimeIndexOverrideFunction()
	{
		this.timeIndexOverrideFunc = null;
	}

	// Token: 0x06004142 RID: 16706 RVA: 0x0015D578 File Offset: 0x0015B778
	public void SetOverrideIndex(int index)
	{
		this.overrideIndex = index;
		this.currentWeatherIndex = this.overrideIndex;
		this.currentTimeIndex = this.overrideIndex;
		this.currentTimeOfDay = this.dayNightLightmapNames[this.currentTimeIndex];
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	// Token: 0x06004143 RID: 16707 RVA: 0x0015D5D4 File Offset: 0x0015B7D4
	public void AnimateLightFlash(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		if (this.animatingLightFlash != null)
		{
			base.StopCoroutine(this.animatingLightFlash);
		}
		this.animatingLightFlash = base.StartCoroutine(this.AnimateLightFlashCo(index, fadeInDuration, holdDuration, fadeOutDuration));
	}

	// Token: 0x06004144 RID: 16708 RVA: 0x0015D601 File Offset: 0x0015B801
	private IEnumerator AnimateLightFlashCo(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		int startMap = (this.currentLerp < 0.5f) ? this.currentTimeIndex : ((this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
		this.ChangeMaps(startMap, index);
		float endTimestamp = Time.time + fadeInDuration;
		while (Time.time < endTimestamp)
		{
			this.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
			yield return null;
		}
		this.ChangeMaps(index, index);
		this.ChangeLerps(0f);
		endTimestamp = Time.time + fadeInDuration;
		while (Time.time < endTimestamp)
		{
			yield return null;
		}
		this.ChangeMaps(index, startMap);
		endTimestamp = Time.time + fadeOutDuration;
		while (Time.time < endTimestamp)
		{
			this.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
			yield return null;
		}
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
		this.ChangeLerps(this.currentLerp);
		this.animatingLightFlash = null;
		yield break;
	}

	// Token: 0x06004145 RID: 16709 RVA: 0x0015D628 File Offset: 0x0015B828
	public void SetTimeOfDay(int timeIndex)
	{
		double num = 0.0;
		for (int i = 0; i < timeIndex; i++)
		{
			num += this.timeOfDayRange[i];
		}
		this.currentTime = num * 3600.0;
		this.currentSetting = TimeSettings.Static;
	}

	// Token: 0x06004146 RID: 16710 RVA: 0x0015D66E File Offset: 0x0015B86E
	public void FastForward(float seconds)
	{
		this.baseSeconds += (double)seconds;
	}

	// Token: 0x06004147 RID: 16711 RVA: 0x0015D67F File Offset: 0x0015B87F
	public void SetFixedWeather(BetterDayNightManager.WeatherType weather)
	{
		this.overrideWeather = true;
		this.overrideWeatherType = weather;
	}

	// Token: 0x06004148 RID: 16712 RVA: 0x0015D68F File Offset: 0x0015B88F
	public void ClearFixedWeather()
	{
		this.overrideWeather = false;
	}

	// Token: 0x04005279 RID: 21113
	public const int TIME_OF_DAY_COUNT = 10;

	// Token: 0x0400527A RID: 21114
	[OnEnterPlay_SetNull]
	public static volatile BetterDayNightManager instance;

	// Token: 0x0400527B RID: 21115
	[OnEnterPlay_Clear]
	public static List<PerSceneRenderData> allScenesRenderData = new List<PerSceneRenderData>();

	// Token: 0x0400527C RID: 21116
	public Shader standard;

	// Token: 0x0400527D RID: 21117
	public Shader standardCutout;

	// Token: 0x0400527E RID: 21118
	public Shader gorillaUnlit;

	// Token: 0x0400527F RID: 21119
	public Shader gorillaUnlitCutout;

	// Token: 0x04005280 RID: 21120
	public Material[] dayNightSupportedMaterials;

	// Token: 0x04005281 RID: 21121
	public Material[] dayNightSupportedMaterialsCutout;

	// Token: 0x04005282 RID: 21122
	public string[] dayNightLightmapNames;

	// Token: 0x04005283 RID: 21123
	public string[] dayNightWeatherLightmapNames;

	// Token: 0x04005284 RID: 21124
	public Texture2D[] dayNightSkyboxTextures;

	// Token: 0x04005285 RID: 21125
	public Texture2D[] cloudsDayNightSkyboxTextures;

	// Token: 0x04005286 RID: 21126
	public Texture2D[] beachDayNightSkyboxTextures;

	// Token: 0x04005287 RID: 21127
	public Texture2D[] dayNightWeatherSkyboxTextures;

	// Token: 0x04005288 RID: 21128
	public float[] standardUnlitColor;

	// Token: 0x04005289 RID: 21129
	public float[] standardUnlitColorWithPremadeColorDarker;

	// Token: 0x0400528A RID: 21130
	public float currentLerp;

	// Token: 0x0400528B RID: 21131
	public float currentTimestep;

	// Token: 0x0400528C RID: 21132
	public BetterDayNightManager.Season currentSeason;

	// Token: 0x0400528D RID: 21133
	public double[] summerTimeOfDayRange;

	// Token: 0x0400528E RID: 21134
	public double[] winterTimeOfDayRange;

	// Token: 0x0400528F RID: 21135
	public double timeMultiplier;

	// Token: 0x04005290 RID: 21136
	private float lastTime;

	// Token: 0x04005291 RID: 21137
	private double currentTime;

	// Token: 0x04005292 RID: 21138
	private double totalHours;

	// Token: 0x04005293 RID: 21139
	private double totalSeconds;

	// Token: 0x04005294 RID: 21140
	private float colorFrom;

	// Token: 0x04005295 RID: 21141
	private float colorTo;

	// Token: 0x04005296 RID: 21142
	private float colorFromDarker;

	// Token: 0x04005297 RID: 21143
	private float colorToDarker;

	// Token: 0x04005298 RID: 21144
	public int currentTimeIndex;

	// Token: 0x04005299 RID: 21145
	public int currentWeatherIndex;

	// Token: 0x0400529A RID: 21146
	private int lastIndex;

	// Token: 0x0400529B RID: 21147
	private double currentIndexSeconds;

	// Token: 0x0400529C RID: 21148
	private double baseSeconds;

	// Token: 0x0400529D RID: 21149
	private bool computerInit;

	// Token: 0x0400529E RID: 21150
	public int mySeed;

	// Token: 0x0400529F RID: 21151
	public Random randomNumberGenerator = new Random();

	// Token: 0x040052A0 RID: 21152
	public BetterDayNightManager.WeatherType[] weatherCycle;

	// Token: 0x040052A1 RID: 21153
	public bool overrideWeather;

	// Token: 0x040052A2 RID: 21154
	public BetterDayNightManager.WeatherType overrideWeatherType;

	// Token: 0x040052A4 RID: 21156
	public float rainChance = 0.3f;

	// Token: 0x040052A5 RID: 21157
	public int maxRainDuration = 5;

	// Token: 0x040052A6 RID: 21158
	private int rainDuration;

	// Token: 0x040052A7 RID: 21159
	private float remainingSeconds;

	// Token: 0x040052A8 RID: 21160
	private long initialDayCycles;

	// Token: 0x040052A9 RID: 21161
	private long gameEpochDay;

	// Token: 0x040052AA RID: 21162
	private int currentWeatherCycle;

	// Token: 0x040052AB RID: 21163
	private int fromWeatherIndex;

	// Token: 0x040052AC RID: 21164
	private int toWeatherIndex;

	// Token: 0x040052AD RID: 21165
	private Texture2D fromSky;

	// Token: 0x040052AE RID: 21166
	private Texture2D fromSky2;

	// Token: 0x040052AF RID: 21167
	private Texture2D fromSky3;

	// Token: 0x040052B0 RID: 21168
	private Texture2D toSky;

	// Token: 0x040052B1 RID: 21169
	private Texture2D toSky2;

	// Token: 0x040052B2 RID: 21170
	private Texture2D toSky3;

	// Token: 0x040052B3 RID: 21171
	public AddCollidersToParticleSystemTriggers[] weatherSystems;

	// Token: 0x040052B4 RID: 21172
	public List<Collider> collidersToAddToWeatherSystems = new List<Collider>();

	// Token: 0x040052B5 RID: 21173
	private float lastTimeChecked;

	// Token: 0x040052B6 RID: 21174
	private Func<int, int> timeIndexOverrideFunc;

	// Token: 0x040052B7 RID: 21175
	public int overrideIndex = -1;

	// Token: 0x040052B8 RID: 21176
	[OnEnterPlay_Clear]
	private static readonly Dictionary<int, BetterDayNightManager.ScheduledEvent> scheduledEvents = new Dictionary<int, BetterDayNightManager.ScheduledEvent>(256);

	// Token: 0x040052B9 RID: 21177
	public TimeSettings currentSetting;

	// Token: 0x040052BA RID: 21178
	private ShaderHashId _GT_DayCycleTimeProgress = "_GT_DayCycleTimeProgress";

	// Token: 0x040052BB RID: 21179
	private ShaderHashId _GT_DayCycleBrightnessOption1_Id = "_GT_DayCycleBrightnessOption1";

	// Token: 0x040052BC RID: 21180
	private ShaderHashId _GT_DayCycleBrightnessOption2_Id = "_GT_DayCycleBrightnessOption2";

	// Token: 0x040052BD RID: 21181
	private ShaderHashId _GlobalDayNightLerpValue = "_GlobalDayNightLerpValue";

	// Token: 0x040052BE RID: 21182
	private ShaderHashId _GlobalDayNightSkyTex1 = "_GlobalDayNightSkyTex1";

	// Token: 0x040052BF RID: 21183
	private ShaderHashId _GlobalDayNightSkyTex2 = "_GlobalDayNightSkyTex2";

	// Token: 0x040052C0 RID: 21184
	private ShaderHashId _GlobalDayNightSky2Tex1 = "_GlobalDayNightSky2Tex1";

	// Token: 0x040052C1 RID: 21185
	private ShaderHashId _GlobalDayNightSky2Tex2 = "_GlobalDayNightSky2Tex2";

	// Token: 0x040052C2 RID: 21186
	private ShaderHashId _GlobalDayNightSky3Tex1 = "_GlobalDayNightSky3Tex1";

	// Token: 0x040052C3 RID: 21187
	private ShaderHashId _GlobalDayNightSky3Tex2 = "_GlobalDayNightSky3Tex2";

	// Token: 0x040052C4 RID: 21188
	private bool shouldRepopulate;

	// Token: 0x040052C5 RID: 21189
	private Coroutine animatingLightFlash;

	// Token: 0x020009F3 RID: 2547
	public enum Season
	{
		// Token: 0x040052C7 RID: 21191
		Winter,
		// Token: 0x040052C8 RID: 21192
		Spring,
		// Token: 0x040052C9 RID: 21193
		Summer,
		// Token: 0x040052CA RID: 21194
		Fall
	}

	// Token: 0x020009F4 RID: 2548
	public enum WeatherType
	{
		// Token: 0x040052CC RID: 21196
		None,
		// Token: 0x040052CD RID: 21197
		Raining,
		// Token: 0x040052CE RID: 21198
		All
	}

	// Token: 0x020009F5 RID: 2549
	private class ScheduledEvent
	{
		// Token: 0x040052CF RID: 21199
		public long lastDayCalled;

		// Token: 0x040052D0 RID: 21200
		public int hour;

		// Token: 0x040052D1 RID: 21201
		public Action action;
	}
}
