using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Token: 0x02000263 RID: 611
[JsonConverter(typeof(StringEnumConverter))]
[Serializable]
public enum QuestCategory
{
	// Token: 0x04001388 RID: 5000
	NONE,
	// Token: 0x04001389 RID: 5001
	Social,
	// Token: 0x0400138A RID: 5002
	Exploration,
	// Token: 0x0400138B RID: 5003
	Gameplay,
	// Token: 0x0400138C RID: 5004
	GameRound,
	// Token: 0x0400138D RID: 5005
	Tag
}
