using System;
using UnityEngine;

// Token: 0x02000188 RID: 392
public class SuperInfectionTelemetry : MonoBehaviour
{
	// Token: 0x170000ED RID: 237
	// (get) Token: 0x06000A87 RID: 2695 RVA: 0x00038A27 File Offset: 0x00036C27
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x06000A88 RID: 2696 RVA: 0x00038A38 File Offset: 0x00036C38
	public static string GameEnvironment
	{
		get
		{
			return "game_environment_live";
		}
	}

	// Token: 0x04000CA7 RID: 3239
	public const string ROOM_LEFT_EVENT_NAME = "super_infection_room_left";

	// Token: 0x04000CA8 RID: 3240
	public const string INTERVAL_EVENT_NAME = "super_infection_interval";

	// Token: 0x04000CA9 RID: 3241
	public const string SI_PURCHASE_EVENT_NAME = "super_infection_purchase";

	// Token: 0x04000CAA RID: 3242
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x04000CAB RID: 3243
	private const string METRIC_ACTION_CUSTOM_TAG_PREFIX = "metric_action_";

	// Token: 0x04000CAC RID: 3244
	public const string SUPER_INFECTION_PREPEND = "super_infection_";

	// Token: 0x04000CAD RID: 3245
	public const string SUPER_INFECTION_GAME_ID_BODY_DATA = "super_infection_round_id";

	// Token: 0x04000CAE RID: 3246
	public const string EVENT_TIMESTAMP_BODY_DATA = "event_timestamp";

	// Token: 0x04000CAF RID: 3247
	public const string TOTAL_PLAY_TIME_BODY_DATA = "total_play_time";

	// Token: 0x04000CB0 RID: 3248
	public const string ROOM_PLAY_TIME_BODY_DATA = "room_play_time";

	// Token: 0x04000CB1 RID: 3249
	public const string SESSION_PLAY_TIME_BODY_DATA = "session_play_time";

	// Token: 0x04000CB2 RID: 3250
	public const string INTERVAL_PLAY_TIME_BODY_DATA = "interval_play_time";

	// Token: 0x04000CB3 RID: 3251
	public const string TERMINAL_TOTAL_TIME_BODY_DATA = "terminal_total_time";

	// Token: 0x04000CB4 RID: 3252
	public const string TERMINAL_INTERVAL_TIME_BODY_DATA = "terminal_interval_time";

	// Token: 0x04000CB5 RID: 3253
	public const string TIME_USING_GADGET_TYPE_TOTAL_BODY_DATA = "time_holding_gadget_type_total";

	// Token: 0x04000CB6 RID: 3254
	public const string TIME_USING_GADGET_TYPE_INTERVAL_BODY_DATA = "time_holding_gadget_type_interval";

	// Token: 0x04000CB7 RID: 3255
	public const string TIME_HOLDING_OWN_GADGETS_TOTAL_BODY_DATA = "time_holding_own_gadgets_total";

	// Token: 0x04000CB8 RID: 3256
	public const string TIME_HOLDING_OWN_GADGETS_INTERVAL_BODY_DATA = "time_holding_own_gadgets_interval";

	// Token: 0x04000CB9 RID: 3257
	public const string TIME_HOLDING_OTHERS_GADGETS_TOTAL_BODY_DATA = "time_holding_others_gadgets_total";

	// Token: 0x04000CBA RID: 3258
	public const string TIME_HOLDING_OTHERS_GADGETS_INTERVAL_BODY_DATA = "time_holding_others_gadgets_interval";

	// Token: 0x04000CBB RID: 3259
	public const string TAGS_HOLDING_GADGET_TYPE_TOTAL_BODY_DATA = "tags_holding_gadget_type_total";

	// Token: 0x04000CBC RID: 3260
	public const string TAGS_HOLDING_GADGET_TYPE_INTERVAL_BODY_DATA = "tags_holding_gadget_type_interval";

	// Token: 0x04000CBD RID: 3261
	public const string TAGS_HOLDING_OWN_GADGETS_TOTAL_BODY_DATA = "tags_holding_own_gadgets_total";

	// Token: 0x04000CBE RID: 3262
	public const string TAGS_HOLDING_OWN_GADGETS_INTERVAL_BODY_DATA = "tags_holding_own_gadgets_interval";

	// Token: 0x04000CBF RID: 3263
	public const string TAGS_HOLDING_OTHERS_GADGETS_TOTAL_BODY_DATA = "tags_holding_others_gadgets_total";

	// Token: 0x04000CC0 RID: 3264
	public const string TAGS_HOLDING_OTHERS_GADGETS_INTERVAL_BODY_DATA = "tags_holding_others_gadgets_interval";

	// Token: 0x04000CC1 RID: 3265
	public const string RESOURCE_TYPE_COLLECTED_TOTAL_BODY_DATA = "resource_type_collected_total";

	// Token: 0x04000CC2 RID: 3266
	public const string RESOURCE_TYPE_COLLECTED_INTERVAL_BODY_DATA = "resource_type_collected_interval";

	// Token: 0x04000CC3 RID: 3267
	public const string ROUNDS_PLAYED_TOTAL_BODY_DATA = "rounds_played_total";

	// Token: 0x04000CC4 RID: 3268
	public const string ROUNDS_PLAYED_INTERVAL_BODY_DATA = "rounds_played_interval";

	// Token: 0x04000CC5 RID: 3269
	public const string UNLOCKED_NODES_BODY_DATA = "unlocked_nodes";

	// Token: 0x04000CC6 RID: 3270
	public const string PLAYER_COUNT_BODY_DATA = "player_count";

	// Token: 0x04000CC7 RID: 3271
	public const string SI_SHINY_ROCK_COST = "si_shiny_rock_cost";

	// Token: 0x04000CC8 RID: 3272
	public const string SI_PURCHASE_TYPE = "si_purchase_type";

	// Token: 0x04000CC9 RID: 3273
	public const string SI_TECH_POINTS_PURCHASED = "si_tech_points_purchased";
}
