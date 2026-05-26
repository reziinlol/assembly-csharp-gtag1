using System;
using UnityEngine;

// Token: 0x02000C8D RID: 3213
public class PhotonAuthenticatorSettings
{
	// Token: 0x06004FBA RID: 20410 RVA: 0x001A6218 File Offset: 0x001A4418
	static PhotonAuthenticatorSettings()
	{
		PhotonAuthenticatorSettings.Load("PhotonAuthenticatorSettings");
	}

	// Token: 0x06004FBB RID: 20411 RVA: 0x001A6224 File Offset: 0x001A4424
	public static void Load(string path)
	{
		PhotonAuthenticatorSettingsScriptableObject photonAuthenticatorSettingsScriptableObject = Resources.Load<PhotonAuthenticatorSettingsScriptableObject>(path);
		PhotonAuthenticatorSettings.PunAppId = photonAuthenticatorSettingsScriptableObject.PunAppId;
		PhotonAuthenticatorSettings.FusionAppId = photonAuthenticatorSettingsScriptableObject.FusionAppId;
		PhotonAuthenticatorSettings.VoiceAppId = photonAuthenticatorSettingsScriptableObject.VoiceAppId;
	}

	// Token: 0x04006172 RID: 24946
	public static string PunAppId;

	// Token: 0x04006173 RID: 24947
	public static string FusionAppId;

	// Token: 0x04006174 RID: 24948
	public static string VoiceAppId;
}
