using System;
using UnityEngine;

// Token: 0x02000C8F RID: 3215
public class PlayFabAuthenticatorSettings
{
	// Token: 0x06004FBE RID: 20414 RVA: 0x001A624C File Offset: 0x001A444C
	static PlayFabAuthenticatorSettings()
	{
		PlayFabAuthenticatorSettings.Load("PlayFabAuthenticatorSettings");
	}

	// Token: 0x06004FBF RID: 20415 RVA: 0x001A6258 File Offset: 0x001A4458
	public static void Load(string path)
	{
		PlayFabAuthenticatorSettingsScriptableObject playFabAuthenticatorSettingsScriptableObject = Resources.Load<PlayFabAuthenticatorSettingsScriptableObject>(path);
		PlayFabAuthenticatorSettings.TitleId = playFabAuthenticatorSettingsScriptableObject.TitleId;
		PlayFabAuthenticatorSettings.AuthApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.AuthApiBaseUrl;
		PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.DailyQuestsApiBaseUrl;
		PlayFabAuthenticatorSettings.FriendApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.FriendApiBaseUrl;
		PlayFabAuthenticatorSettings.HpPromoApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.HpPromoApiBaseUrl;
		PlayFabAuthenticatorSettings.IapApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.IapApiBaseUrl;
		PlayFabAuthenticatorSettings.KidApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.KidApiBaseUrl;
		PlayFabAuthenticatorSettings.MmrApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.MmrApiBaseUrl;
		PlayFabAuthenticatorSettings.ModerationApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.ModerationApiBaseUrl;
		PlayFabAuthenticatorSettings.ProgressionApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.ProgressionApiBaseUrl;
		PlayFabAuthenticatorSettings.TitleDataApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.TitleDataApiBaseUrl;
		PlayFabAuthenticatorSettings.VotingApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.VotingApiBaseUrl;
	}

	// Token: 0x04006178 RID: 24952
	public static string TitleId;

	// Token: 0x04006179 RID: 24953
	public static string AuthApiBaseUrl;

	// Token: 0x0400617A RID: 24954
	public static string DailyQuestsApiBaseUrl;

	// Token: 0x0400617B RID: 24955
	public static string FriendApiBaseUrl;

	// Token: 0x0400617C RID: 24956
	public static string HpPromoApiBaseUrl;

	// Token: 0x0400617D RID: 24957
	public static string IapApiBaseUrl;

	// Token: 0x0400617E RID: 24958
	public static string KidApiBaseUrl;

	// Token: 0x0400617F RID: 24959
	public static string MmrApiBaseUrl;

	// Token: 0x04006180 RID: 24960
	public static string ModerationApiBaseUrl;

	// Token: 0x04006181 RID: 24961
	public static string ProgressionApiBaseUrl;

	// Token: 0x04006182 RID: 24962
	public static string TitleDataApiBaseUrl;

	// Token: 0x04006183 RID: 24963
	public static string VotingApiBaseUrl;
}
