using System;

// Token: 0x02000B27 RID: 2855
public static class KIDFeaturesExtensions
{
	// Token: 0x0600486D RID: 18541 RVA: 0x001830AC File Offset: 0x001812AC
	public static string ToStandardisedString(this EKIDFeatures feature)
	{
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
			return "multiplayer";
		case EKIDFeatures.Custom_Nametags:
			return "custom-username";
		case EKIDFeatures.Voice_Chat:
			return "voice-chat";
		case EKIDFeatures.Mods:
			return "mods";
		case EKIDFeatures.Groups:
			return "join-groups";
		default:
			return feature.ToString();
		}
	}

	// Token: 0x0600486E RID: 18542 RVA: 0x00183100 File Offset: 0x00181300
	public static EKIDFeatures? FromString(string name)
	{
		string a = name.ToLower();
		if (a == "voice-chat")
		{
			return new EKIDFeatures?(EKIDFeatures.Voice_Chat);
		}
		if (a == "custom-username")
		{
			return new EKIDFeatures?(EKIDFeatures.Custom_Nametags);
		}
		if (a == "multiplayer")
		{
			return new EKIDFeatures?(EKIDFeatures.Multiplayer);
		}
		if (a == "mods")
		{
			return new EKIDFeatures?(EKIDFeatures.Mods);
		}
		if (!(a == "join-groups"))
		{
			return null;
		}
		return new EKIDFeatures?(EKIDFeatures.Groups);
	}

	// Token: 0x0600486F RID: 18543 RVA: 0x00183184 File Offset: 0x00181384
	public static bool TryGetFromString(string name, out EKIDFeatures result)
	{
		EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(name);
		if (ekidfeatures != null)
		{
			result = ekidfeatures.Value;
			return true;
		}
		result = EKIDFeatures.Voice_Chat;
		return false;
	}
}
