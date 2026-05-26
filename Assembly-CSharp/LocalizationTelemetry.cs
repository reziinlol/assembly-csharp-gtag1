using System;
using UnityEngine;

// Token: 0x02000BDF RID: 3039
public static class LocalizationTelemetry
{
	// Token: 0x17000726 RID: 1830
	// (get) Token: 0x06004BEF RID: 19439 RVA: 0x00038A27 File Offset: 0x00036C27
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x04005F2F RID: 24367
	public const string LANGUAGE_CHANGED_EVENT_NAME = "language_changed";

	// Token: 0x04005F30 RID: 24368
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x04005F31 RID: 24369
	public const string STARTING_LANGUAGE_BODY_DATA = "starting_language";

	// Token: 0x04005F32 RID: 24370
	public const string NEW_LANGUAGE_BODY_DATA = "new_language";
}
