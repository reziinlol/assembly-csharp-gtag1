using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020006D5 RID: 1749
public class GameLightingManager : MonoBehaviourTick, IGorillaSliceableSimple
{
	// Token: 0x1700044D RID: 1101
	// (get) Token: 0x06002BFD RID: 11261 RVA: 0x000EDDB6 File Offset: 0x000EBFB6
	public bool IsDynamicLightingEnabled
	{
		get
		{
			return this.customVertexLightingEnabled;
		}
	}

	// Token: 0x06002BFE RID: 11262 RVA: 0x000EDDBE File Offset: 0x000EBFBE
	private static uint PackHalf2(float a, float b)
	{
		return (uint)((int)Mathf.FloatToHalf(a) | (int)Mathf.FloatToHalf(b) << 16);
	}

	// Token: 0x06002BFF RID: 11263 RVA: 0x000EDDD0 File Offset: 0x000EBFD0
	private void Awake()
	{
		this.InitData();
	}

	// Token: 0x06002C00 RID: 11264 RVA: 0x000EDDD8 File Offset: 0x000EBFD8
	private void InitData()
	{
		GameLightingManager.instance = this;
		this.gameLights = new List<GameLight>(512);
		this.sortKeys = new float[512];
		this.sortValues = new GameLight[512];
		this.lightDataBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 50, UnsafeUtility.SizeOf<GameLightingManager.LightDataPacked>());
		this.lightData = new NativeArray<GameLightingManager.LightDataPacked>(50, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.lightDataBufferLegacy = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 50, UnsafeUtility.SizeOf<GameLightingManager.LightDataLegacy>());
		this.lightDataLegacy = new NativeArray<GameLightingManager.LightDataLegacy>(50, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.nextLightUpdate = 0;
		this.ClearGameLights();
		this.SetDesaturateAndTintEnabled(false, Color.black);
		this.SetAmbientLightDynamic(Color.black);
		this.SetCustomDynamicLightingEnabled(false);
		this.SetMaxLights(20);
	}

	// Token: 0x06002C01 RID: 11265 RVA: 0x000EDE98 File Offset: 0x000EC098
	private void OnDestroy()
	{
		this.ClearGameLights();
		this.SetDesaturateAndTintEnabled(false, Color.black);
		this.SetAmbientLightDynamic(Color.black);
		this.SetCustomDynamicLightingEnabled(false);
		GraphicsBuffer graphicsBuffer = this.lightDataBuffer;
		if (graphicsBuffer != null)
		{
			graphicsBuffer.Dispose();
		}
		if (this.lightData.IsCreated)
		{
			this.lightData.Dispose();
		}
		GraphicsBuffer graphicsBuffer2 = this.lightDataBufferLegacy;
		if (graphicsBuffer2 != null)
		{
			graphicsBuffer2.Dispose();
		}
		if (this.lightDataLegacy.IsCreated)
		{
			this.lightDataLegacy.Dispose();
		}
	}

	// Token: 0x06002C02 RID: 11266 RVA: 0x000EDF1B File Offset: 0x000EC11B
	public new void OnEnable()
	{
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002C03 RID: 11267 RVA: 0x000EDF2A File Offset: 0x000EC12A
	public new void OnDisable()
	{
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002C04 RID: 11268 RVA: 0x000EDF3C File Offset: 0x000EC13C
	public void ZoneEnableCustomDynamicLighting(bool enable)
	{
		if (enable)
		{
			if (this.zoneDynamicLightingEnableCount == 0)
			{
				this.SetCustomDynamicLightingEnabled(true);
			}
			this.zoneDynamicLightingEnableCount++;
			return;
		}
		this.zoneDynamicLightingEnableCount--;
		if (this.zoneDynamicLightingEnableCount == 0)
		{
			this.SetCustomDynamicLightingEnabled(false);
		}
		if (this.zoneDynamicLightingEnableCount < 0)
		{
			Debug.LogErrorFormat("Zone Dynamic Lighting Ref count is {0} and should never be less that 0", new object[]
			{
				this.zoneDynamicLightingEnableCount
			});
			this.zoneDynamicLightingEnableCount = 0;
		}
	}

	// Token: 0x06002C05 RID: 11269 RVA: 0x000EDFB5 File Offset: 0x000EC1B5
	public void SetCustomDynamicLightingEnabled(bool enable)
	{
		this.customVertexLightingEnabled = enable;
		if (this.customVertexLightingEnabled)
		{
			Shader.EnableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
			return;
		}
		Shader.DisableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
	}

	// Token: 0x06002C06 RID: 11270 RVA: 0x000EDFDB File Offset: 0x000EC1DB
	public void ToggleCustomDynamicLightingEnabled()
	{
		this.SetCustomDynamicLightingEnabled(!this.customVertexLightingEnabled);
	}

	// Token: 0x06002C07 RID: 11271 RVA: 0x000EDFEC File Offset: 0x000EC1EC
	public void SetAmbientLightDynamic(Color color)
	{
		Shader.SetGlobalColor(GameLightingManager._shaderPropId_GameLight_Ambient_Color, color);
	}

	// Token: 0x06002C08 RID: 11272 RVA: 0x000EDFF9 File Offset: 0x000EC1F9
	public void SetMaxLights(int maxLights)
	{
		maxLights = Mathf.Min(maxLights, 50);
		this.maxUseTestLights = maxLights;
		Shader.SetGlobalInteger(GameLightingManager._shaderPropId_GameLight_UseMaxLights, maxLights);
	}

	// Token: 0x06002C09 RID: 11273 RVA: 0x000EE017 File Offset: 0x000EC217
	public void SetDesaturateAndTintEnabled(bool enable, Color tint)
	{
		Shader.SetGlobalColor(GameLightingManager._shaderPropId_DesaturateAndTint_TintColor, tint);
		Shader.SetGlobalFloat(GameLightingManager._shaderPropId_DesaturateAndTint_TintAmount, enable ? 1f : 0f);
		this.desaturateAndTintEnabled = enable;
	}

	// Token: 0x06002C0A RID: 11274 RVA: 0x000EE044 File Offset: 0x000EC244
	public void SliceUpdate()
	{
		if (this.skipNextSlice)
		{
			this.skipNextSlice = false;
			return;
		}
		this.immediateSort = false;
		this.SortLights();
	}

	// Token: 0x06002C0B RID: 11275 RVA: 0x000EE064 File Offset: 0x000EC264
	public void SortLights()
	{
		int count = this.gameLights.Count;
		if (count <= this.maxUseTestLights)
		{
			return;
		}
		if (this.mainCameraTransform == null)
		{
			this.mainCameraTransform = Camera.main.transform;
		}
		Vector3 position = this.mainCameraTransform.position;
		if (this.sortKeys == null || this.sortKeys.Length < count)
		{
			int num = Mathf.Max(count, (this.sortKeys != null) ? (this.sortKeys.Length * 2) : 64);
			this.sortKeys = new float[num];
			this.sortValues = new GameLight[num];
		}
		for (int i = 0; i < count; i++)
		{
			GameLight gameLight = this.gameLights[i];
			if (gameLight == null || gameLight.light == null)
			{
				this.sortKeys[i] = float.MaxValue;
			}
			else
			{
				float num2 = Mathf.Clamp(gameLight.cachedColorAndIntensity.x + gameLight.cachedColorAndIntensity.y + gameLight.cachedColorAndIntensity.z, 0.01f, 6f);
				Vector3 vector = position - gameLight.cachedPosition;
				this.sortKeys[i] = (vector.x * vector.x + vector.y * vector.y + vector.z * vector.z) / num2;
			}
			this.sortValues[i] = gameLight;
		}
		Array.Sort<float, GameLight>(this.sortKeys, this.sortValues, 0, count);
		for (int j = 0; j < count; j++)
		{
			this.gameLights[j] = this.sortValues[j];
		}
	}

	// Token: 0x06002C0C RID: 11276 RVA: 0x000EE207 File Offset: 0x000EC407
	public override void Tick()
	{
		this.RefreshLightData();
	}

	// Token: 0x06002C0D RID: 11277 RVA: 0x000EE210 File Offset: 0x000EC410
	private void RefreshLightData()
	{
		if (this.lightDataBuffer == null)
		{
			return;
		}
		if (this.customVertexLightingEnabled)
		{
			int numLightsToPull = 10;
			if (this.immediateSort)
			{
				this.immediateSort = false;
				this.skipNextSlice = true;
				this.CacheAllLightData();
				this.SortLights();
				numLightsToPull = this.maxUseTestLights;
			}
			else
			{
				int numLightsToUpdateCache = 5;
				this.CacheLightDataForNonCloseLights(numLightsToUpdateCache);
			}
			this.PullLightData(numLightsToPull);
			int num = Mathf.Min(this.gameLights.Count, this.maxUseTestLights);
			if (num > 0)
			{
				bool flag = CustomMapLoader.IsMapLoaded();
				this.lightDataBuffer.SetData<GameLightingManager.LightDataPacked>(this.lightData, 0, 0, num);
				if (flag)
				{
					this.lightDataBufferLegacy.SetData<GameLightingManager.LightDataLegacy>(this.lightDataLegacy);
				}
				Shader.SetGlobalBuffer(GameLightingManager._shaderPropId_GameLight_LightsPacked, this.lightDataBuffer);
				if (flag)
				{
					Shader.SetGlobalBuffer(GameLightingManager._shaderPropId_GameLight_Lights, this.lightDataBufferLegacy);
				}
				Shader.SetGlobalInteger(GameLightingManager._shaderPropId_GameLight_UseMaxLights, num);
			}
		}
	}

	// Token: 0x06002C0E RID: 11278 RVA: 0x000EE2E4 File Offset: 0x000EC4E4
	public void CacheAllLightData()
	{
		for (int i = 0; i < this.gameLights.Count; i++)
		{
			GameLight gameLight = this.gameLights[i];
			if (gameLight != null && gameLight.light != null)
			{
				gameLight.cachedPosition = gameLight.transform.position;
				gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? -1f : 1f) * gameLight.light.color;
			}
		}
	}

	// Token: 0x06002C0F RID: 11279 RVA: 0x000EE384 File Offset: 0x000EC584
	public void CacheLightDataForNonCloseLights(int numLightsToUpdateCache)
	{
		int num = this.gameLights.Count - this.maxUseTestLights;
		if (num <= 0)
		{
			return;
		}
		for (int i = 0; i < numLightsToUpdateCache; i++)
		{
			this.nextLightCacheUpdate = (this.nextLightCacheUpdate + 1) % num;
			GameLight gameLight = this.gameLights[this.maxUseTestLights + this.nextLightCacheUpdate];
			if (gameLight != null && gameLight.light != null)
			{
				gameLight.cachedPosition = gameLight.transform.position;
				gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? -1f : 1f) * gameLight.light.color;
			}
		}
	}

	// Token: 0x06002C10 RID: 11280 RVA: 0x000EE450 File Offset: 0x000EC650
	public void PullLightData(int numLightsToPull)
	{
		for (int i = 0; i < this.maxUseTestLights; i++)
		{
			if (i < this.gameLights.Count && this.gameLights[i] != null && this.gameLights[i].isHighPriorityPlayerLight)
			{
				this.GetFromLight(i, i);
			}
		}
		for (int j = 0; j < numLightsToPull; j++)
		{
			this.nextLightUpdate = (this.nextLightUpdate + 1) % this.maxUseTestLights;
			if (this.nextLightUpdate < this.gameLights.Count)
			{
				this.GetFromLight(this.nextLightUpdate, this.nextLightUpdate);
				if (this.gameLights[this.nextLightUpdate] != null && this.gameLights[this.nextLightUpdate].isHighPriorityPlayerLight)
				{
				}
			}
			else
			{
				this.ResetLight(this.nextLightUpdate);
			}
		}
	}

	// Token: 0x06002C11 RID: 11281 RVA: 0x000EE534 File Offset: 0x000EC734
	public int AddGameLight(GameLight light, bool ignoreUnityLightDisable = false)
	{
		if (light == null || !light.gameObject.activeInHierarchy || light.light == null || !light.light.enabled)
		{
			return -1;
		}
		if (light.IsRegistered)
		{
			return -1;
		}
		if (!ignoreUnityLightDisable)
		{
			light.light.enabled = false;
		}
		this.gameLights.Add(light);
		this.immediateSort = true;
		return this.gameLights.Count - 1;
	}

	// Token: 0x06002C12 RID: 11282 RVA: 0x000EE5B0 File Offset: 0x000EC7B0
	public void RemoveGameLight(GameLight light)
	{
		if (light != null && light.light != null)
		{
			light.light.enabled = true;
		}
		if (light != null)
		{
			light.lightId = -1;
		}
		int num = this.gameLights.IndexOf(light);
		if (num >= 0)
		{
			this.gameLights.RemoveAt(num);
			if (CustomMapLoader.IsMapLoaded())
			{
				int count = this.gameLights.Count;
				if (count < 50)
				{
					this.lightDataLegacy[count] = default(GameLightingManager.LightDataLegacy);
				}
			}
		}
	}

	// Token: 0x06002C13 RID: 11283 RVA: 0x000EE63C File Offset: 0x000EC83C
	public void ClearGameLights()
	{
		if (this.gameLights != null)
		{
			this.gameLights.Clear();
		}
		if (this.lightDataBuffer == null)
		{
			return;
		}
		for (int i = 0; i < 50; i++)
		{
			this.ResetLight(i);
		}
		this.lightDataBuffer.SetData<GameLightingManager.LightDataPacked>(this.lightData);
		Shader.SetGlobalBuffer(GameLightingManager._shaderPropId_GameLight_LightsPacked, this.lightDataBuffer);
		if (CustomMapLoader.IsMapLoaded())
		{
			this.lightDataBufferLegacy.SetData<GameLightingManager.LightDataLegacy>(this.lightDataLegacy);
			Shader.SetGlobalBuffer(GameLightingManager._shaderPropId_GameLight_Lights, this.lightDataBufferLegacy);
		}
	}

	// Token: 0x06002C14 RID: 11284 RVA: 0x000EE6C4 File Offset: 0x000EC8C4
	public void GetFromLight(int lightIndex, int gameLightIndex)
	{
		if (this.lightDataBuffer == null)
		{
			return;
		}
		GameLight gameLight = null;
		if (gameLightIndex >= 0 && gameLightIndex < this.gameLights.Count)
		{
			gameLight = this.gameLights[gameLightIndex];
		}
		if (gameLight == null || gameLight.light == null)
		{
			return;
		}
		gameLight.cachedPosition = gameLight.transform.position;
		gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? -1f : 1f) * gameLight.light.color;
		Vector3 cachedPosition = gameLight.cachedPosition;
		Vector4 cachedColorAndIntensity = gameLight.cachedColorAndIntensity;
		this.lightData[lightIndex] = new GameLightingManager.LightDataPacked
		{
			posXY = GameLightingManager.PackHalf2(cachedPosition.x, cachedPosition.y),
			posZW = GameLightingManager.PackHalf2(cachedPosition.z, 1f),
			colorRG = GameLightingManager.PackHalf2(cachedColorAndIntensity.x, cachedColorAndIntensity.y),
			colorBA = GameLightingManager.PackHalf2(cachedColorAndIntensity.z, cachedColorAndIntensity.w)
		};
		this.lightDataLegacy[lightIndex] = new GameLightingManager.LightDataLegacy
		{
			position = new float4(cachedPosition.x, cachedPosition.y, cachedPosition.z, 1f),
			color = new float4(cachedColorAndIntensity.x, cachedColorAndIntensity.y, cachedColorAndIntensity.z, cachedColorAndIntensity.w),
			direction = float4.zero
		};
	}

	// Token: 0x06002C15 RID: 11285 RVA: 0x000EE854 File Offset: 0x000ECA54
	private void ResetLight(int lightIndex)
	{
		this.lightData[lightIndex] = default(GameLightingManager.LightDataPacked);
		this.lightDataLegacy[lightIndex] = default(GameLightingManager.LightDataLegacy);
	}

	// Token: 0x1700044E RID: 1102
	// (get) Token: 0x06002C16 RID: 11286 RVA: 0x000EE88B File Offset: 0x000ECA8B
	public Light GR_NearsightedDimLight
	{
		get
		{
			return this._GR_NearsightedDimLight;
		}
	}

	// Token: 0x04003867 RID: 14439
	[OnEnterPlay_SetNull]
	public static volatile GameLightingManager instance;

	// Token: 0x04003868 RID: 14440
	public const int MAX_VERTEX_LIGHTS = 50;

	// Token: 0x04003869 RID: 14441
	public const int USE_MAX_VERTEX_LIGHTS = 20;

	// Token: 0x0400386A RID: 14442
	public const int MAX_UPDATE_LIGHTS_PER_FRAME = 10;

	// Token: 0x0400386B RID: 14443
	public Transform testLightsCenter;

	// Token: 0x0400386C RID: 14444
	[ColorUsage(true, true)]
	public Color testAmbience = Color.black;

	// Token: 0x0400386D RID: 14445
	[ColorUsage(true, true)]
	public Color testLightColor = Color.white;

	// Token: 0x0400386E RID: 14446
	public float testLightBrightness = 10f;

	// Token: 0x0400386F RID: 14447
	public float testLightRadius = 2f;

	// Token: 0x04003870 RID: 14448
	public int maxUseTestLights = 1;

	// Token: 0x04003871 RID: 14449
	[ReadOnly]
	[SerializeField]
	private List<GameLight> gameLights;

	// Token: 0x04003872 RID: 14450
	private bool customVertexLightingEnabled;

	// Token: 0x04003873 RID: 14451
	private bool desaturateAndTintEnabled;

	// Token: 0x04003874 RID: 14452
	private Transform mainCameraTransform;

	// Token: 0x04003875 RID: 14453
	private int zoneDynamicLightingEnableCount;

	// Token: 0x04003876 RID: 14454
	private float[] sortKeys;

	// Token: 0x04003877 RID: 14455
	private GameLight[] sortValues;

	// Token: 0x04003878 RID: 14456
	private NativeArray<GameLightingManager.LightDataPacked> lightData;

	// Token: 0x04003879 RID: 14457
	private NativeArray<GameLightingManager.LightDataLegacy> lightDataLegacy;

	// Token: 0x0400387A RID: 14458
	private GraphicsBuffer lightDataBuffer;

	// Token: 0x0400387B RID: 14459
	private GraphicsBuffer lightDataBufferLegacy;

	// Token: 0x0400387C RID: 14460
	private bool skipNextSlice;

	// Token: 0x0400387D RID: 14461
	private bool immediateSort;

	// Token: 0x0400387E RID: 14462
	private int nextLightUpdate;

	// Token: 0x0400387F RID: 14463
	private int nextLightCacheUpdate;

	// Token: 0x04003880 RID: 14464
	[SerializeField]
	private Light _GR_NearsightedDimLight;

	// Token: 0x04003881 RID: 14465
	private static readonly int _shaderPropId_GameLight_UseMaxLights = Shader.PropertyToID("_GT_GameLight_UseMaxLights");

	// Token: 0x04003882 RID: 14466
	private static readonly int _shaderPropId_DesaturateAndTint_TintColor = Shader.PropertyToID("_GT_DesaturateAndTint_TintColor");

	// Token: 0x04003883 RID: 14467
	private static readonly int _shaderPropId_DesaturateAndTint_TintAmount = Shader.PropertyToID("_GT_DesaturateAndTint_TintAmount");

	// Token: 0x04003884 RID: 14468
	private static readonly int _shaderPropId_GameLight_Ambient_Color = Shader.PropertyToID("_GT_GameLight_Ambient_Color");

	// Token: 0x04003885 RID: 14469
	private static readonly int _shaderPropId_GameLight_Lights = Shader.PropertyToID("_GT_GameLight_Lights");

	// Token: 0x04003886 RID: 14470
	private static readonly int _shaderPropId_GameLight_LightsPacked = Shader.PropertyToID("_GT_GameLight_LightsPacked");

	// Token: 0x020006D6 RID: 1750
	private struct LightInput
	{
		// Token: 0x04003887 RID: 14471
		public Color color;

		// Token: 0x04003888 RID: 14472
		public float intensity;

		// Token: 0x04003889 RID: 14473
		public float intensityMult;
	}

	// Token: 0x020006D7 RID: 1751
	private struct LightDataPacked
	{
		// Token: 0x0400388A RID: 14474
		public uint posXY;

		// Token: 0x0400388B RID: 14475
		public uint posZW;

		// Token: 0x0400388C RID: 14476
		public uint colorRG;

		// Token: 0x0400388D RID: 14477
		public uint colorBA;
	}

	// Token: 0x020006D8 RID: 1752
	private struct LightDataLegacy
	{
		// Token: 0x0400388E RID: 14478
		public float4 position;

		// Token: 0x0400388F RID: 14479
		public float4 color;

		// Token: 0x04003890 RID: 14480
		public float4 direction;
	}
}
