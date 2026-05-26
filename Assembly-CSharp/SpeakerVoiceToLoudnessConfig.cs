using System;
using GorillaNetworking;
using PlayFab;
using UnityEngine;

// Token: 0x020009D3 RID: 2515
internal static class SpeakerVoiceToLoudnessConfig
{
	// Token: 0x170005F7 RID: 1527
	// (get) Token: 0x0600406B RID: 16491 RVA: 0x001583A2 File Offset: 0x001565A2
	public static bool EnableLoudnessLimit
	{
		get
		{
			return SpeakerVoiceToLoudnessConfig.k_config.EnableLoudnessLimit;
		}
	}

	// Token: 0x170005F8 RID: 1528
	// (get) Token: 0x0600406C RID: 16492 RVA: 0x001583AE File Offset: 0x001565AE
	public static float LoudnessLimitThreshold
	{
		get
		{
			return SpeakerVoiceToLoudnessConfig.k_config.LoudnessLimitThreshold;
		}
	}

	// Token: 0x0600406D RID: 16493 RVA: 0x001583BA File Offset: 0x001565BA
	[RuntimeInitializeOnLoadMethod]
	private static void StaticLoad()
	{
		PlayFabTitleDataCache.RegisterOnLoad(new Action<PlayFabTitleDataCache>(SpeakerVoiceToLoudnessConfig.OnTitleDataCacheReady));
	}

	// Token: 0x0600406E RID: 16494 RVA: 0x001583CD File Offset: 0x001565CD
	private static void OnTitleDataCacheReady(PlayFabTitleDataCache titleDataCache)
	{
		titleDataCache.GetTitleData("SpeakerVoiceToLoudnessConfig", new Action<string>(SpeakerVoiceToLoudnessConfig.OnTitleDataCacheResponse), new Action<PlayFabError>(SpeakerVoiceToLoudnessConfig.OnTitleDataCacheError), false);
	}

	// Token: 0x0600406F RID: 16495 RVA: 0x001583F4 File Offset: 0x001565F4
	private static void OnTitleDataCacheResponse(string json)
	{
		SpeakerVoiceToLoudnessConfig.SerializedConfig serializedConfig = default(SpeakerVoiceToLoudnessConfig.SerializedConfig);
		try
		{
			serializedConfig = JsonUtility.FromJson<SpeakerVoiceToLoudnessConfig.SerializedConfig>(json);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			serializedConfig = SpeakerVoiceToLoudnessConfig.k_config;
		}
		finally
		{
			SpeakerVoiceToLoudnessConfig.k_config = serializedConfig;
		}
	}

	// Token: 0x06004070 RID: 16496 RVA: 0x000028C5 File Offset: 0x00000AC5
	private static void OnTitleDataCacheError(PlayFabError errorMsg)
	{
	}

	// Token: 0x040050F6 RID: 20726
	private static SpeakerVoiceToLoudnessConfig.SerializedConfig k_config = new SpeakerVoiceToLoudnessConfig.SerializedConfig
	{
		EnableLoudnessLimit = true,
		LoudnessLimitThreshold = 0.5f
	};

	// Token: 0x040050F7 RID: 20727
	public static StaticArrayBag<float> StaticArrays = new StaticArrayBag<float>();

	// Token: 0x040050F8 RID: 20728
	private const string k_titleDataKey = "SpeakerVoiceToLoudnessConfig";

	// Token: 0x020009D4 RID: 2516
	[Serializable]
	private struct SerializedConfig
	{
		// Token: 0x040050F9 RID: 20729
		public bool EnableLoudnessLimit;

		// Token: 0x040050FA RID: 20730
		public float LoudnessLimitThreshold;
	}
}
