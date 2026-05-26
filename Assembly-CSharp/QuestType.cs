using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Token: 0x02000262 RID: 610
[JsonConverter(typeof(StringEnumConverter))]
[Serializable]
public enum QuestType
{
	// Token: 0x04001377 RID: 4983
	none,
	// Token: 0x04001378 RID: 4984
	gameModeObjective,
	// Token: 0x04001379 RID: 4985
	gameModeRound,
	// Token: 0x0400137A RID: 4986
	grabObject,
	// Token: 0x0400137B RID: 4987
	dropObject,
	// Token: 0x0400137C RID: 4988
	eatObject,
	// Token: 0x0400137D RID: 4989
	tapObject,
	// Token: 0x0400137E RID: 4990
	launchedProjectile,
	// Token: 0x0400137F RID: 4991
	moveDistance,
	// Token: 0x04001380 RID: 4992
	swimDistance,
	// Token: 0x04001381 RID: 4993
	triggerHandEffect,
	// Token: 0x04001382 RID: 4994
	enterLocation,
	// Token: 0x04001383 RID: 4995
	misc,
	// Token: 0x04001384 RID: 4996
	critter,
	// Token: 0x04001385 RID: 4997
	fetchObject,
	// Token: 0x04001386 RID: 4998
	playerInteraction
}
