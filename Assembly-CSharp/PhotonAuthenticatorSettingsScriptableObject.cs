using System;
using UnityEngine;

// Token: 0x02000C8E RID: 3214
[CreateAssetMenu(fileName = "PhotonAuthenticatorSettings", menuName = "ScriptableObjects/PhotonAuthenticatorSettings")]
public class PhotonAuthenticatorSettingsScriptableObject : ScriptableObject
{
	// Token: 0x04006175 RID: 24949
	public string PunAppId;

	// Token: 0x04006176 RID: 24950
	public string FusionAppId;

	// Token: 0x04006177 RID: 24951
	public string VoiceAppId;
}
