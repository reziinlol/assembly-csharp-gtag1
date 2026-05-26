using System;
using UnityEngine;

// Token: 0x02000716 RID: 1814
public class GhostReactorTelemetry : MonoBehaviour
{
	// Token: 0x17000469 RID: 1129
	// (get) Token: 0x06002E16 RID: 11798 RVA: 0x00038A27 File Offset: 0x00036C27
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x1700046A RID: 1130
	// (get) Token: 0x06002E17 RID: 11799 RVA: 0x00038A38 File Offset: 0x00036C38
	public static string GameEnvironment
	{
		get
		{
			return "game_environment_live";
		}
	}

	// Token: 0x04003AE1 RID: 15073
	public const string SHIFT_START_EVENT_NAME = "ghost_game_start";

	// Token: 0x04003AE2 RID: 15074
	public const string SHIFT_END_EVENT_NAME = "ghost_game_end";

	// Token: 0x04003AE3 RID: 15075
	public const string FLOOR_START_EVENT_NAME = "ghost_floor_start";

	// Token: 0x04003AE4 RID: 15076
	public const string FLOOR_END_EVENT_NAME = "ghost_floor_end";

	// Token: 0x04003AE5 RID: 15077
	public const string TOOL_PURCHASED_EVENT_NAME = "ghost_tool_purchased";

	// Token: 0x04003AE6 RID: 15078
	public const string RANK_UP_EVENT_NAME = "ghost_game_rank_up";

	// Token: 0x04003AE7 RID: 15079
	public const string TOOL_UNLOCK_EVENT_NAME = "ghost_game_tool_unlock";

	// Token: 0x04003AE8 RID: 15080
	public const string POD_UPGRADE_PURCHASED_EVENT_NAME = "ghost_pod_upgrade_purchased";

	// Token: 0x04003AE9 RID: 15081
	public const string TOOL_UPGRADE_EVENT_NAME = "ghost_game_tool_upgrade";

	// Token: 0x04003AEA RID: 15082
	public const string CHAOS_SEED_START_EVENT_NAME = "ghost_chaos_seed_start";

	// Token: 0x04003AEB RID: 15083
	public const string CHAOS_JUICE_COLLECTED_EVENT_NAME = "ghost_chaos_juice_collected";

	// Token: 0x04003AEC RID: 15084
	public const string OVERDRIVE_PURCHASED_EVENT_NAME = "ghost_overdrive_purchased";

	// Token: 0x04003AED RID: 15085
	public const string CREDITS_REFILL_PURCHASED_EVENT_NAME = "ghost_credits_refill_purchased";

	// Token: 0x04003AEE RID: 15086
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x04003AEF RID: 15087
	private const string METRIC_ACTION_CUSTOM_TAG_PREFIX = "metric_action_";

	// Token: 0x04003AF0 RID: 15088
	public const string GHOST_GAME_ID_BODY_DATA = "ghost_game_id";

	// Token: 0x04003AF1 RID: 15089
	public const string EVENT_TIMESTAMP_BODY_DATA = "event_timestamp";

	// Token: 0x04003AF2 RID: 15090
	public const string INITIAL_CORES_BALANCE_BODY_DATA = "initial_cores_balance";

	// Token: 0x04003AF3 RID: 15091
	public const string FINAL_CORES_BALANCE_BODY_DATA = "final_cores_balance";

	// Token: 0x04003AF4 RID: 15092
	public const string CORES_SPENT_WAITING_IN_BREAKROOM_BODY_DATA = "cores_spent_waiting_in_breakroom";

	// Token: 0x04003AF5 RID: 15093
	public const string CORES_COLLECTED_FROM_GHOSTS_BODY_DATA = "cores_collected_from_ghosts";

	// Token: 0x04003AF6 RID: 15094
	public const string CORES_COLLECTED_FROM_GATHERING_BODY_DATA = "cores_collected_from_gathering";

	// Token: 0x04003AF7 RID: 15095
	public const string CORES_SPENT_ON_ITEMS_BODY_DATA = "cores_spent_on_items";

	// Token: 0x04003AF8 RID: 15096
	public const string CORES_SPENT_ON_GATES_BODY_DATA = "cores_spent_on_gates";

	// Token: 0x04003AF9 RID: 15097
	public const string CORES_SPENT_ON_LEVELS_BODY_DATA = "cores_spent_on_levels";

	// Token: 0x04003AFA RID: 15098
	public const string CORES_GIVEN_TO_OTHERS_BODY_DATA = "cores_given_to_others";

	// Token: 0x04003AFB RID: 15099
	public const string CORES_RECEIVED_FROM_OTHERS_BODY_DATA = "cores_received_from_others";

	// Token: 0x04003AFC RID: 15100
	public const string SHIFT_CUT_DATA = "shift_cut_data";

	// Token: 0x04003AFD RID: 15101
	public const string GATES_UNLOCKED_BODY_DATA = "gates_unlocked";

	// Token: 0x04003AFE RID: 15102
	public const string DIED_BODY_DATA = "died";

	// Token: 0x04003AFF RID: 15103
	public const string CAUGHT_IN_ANAMOLE_BODY_DATA = "caught_in_anamole";

	// Token: 0x04003B00 RID: 15104
	public const string ITEMS_PURCHASED_BODY_DATA = "items_purchased";

	// Token: 0x04003B01 RID: 15105
	public const string LEVELS_UNLOCKED_BODY_DATA = "levels_unlocked";

	// Token: 0x04003B02 RID: 15106
	public const string NUMBER_OF_PLAYERS_BODY_DATA = "number_of_players";

	// Token: 0x04003B03 RID: 15107
	public const string START_AT_BEGINNING_BODY_DATA = "start_at_beginning";

	// Token: 0x04003B04 RID: 15108
	public const string SECONDS_INTO_SHIFT_AT_JOIN_BODY_DATA = "seconds_into_shift_at_join";

	// Token: 0x04003B05 RID: 15109
	public const string REASON_BODY_DATA = "reason";

	// Token: 0x04003B06 RID: 15110
	public const string PLAY_DURATION_BODY_DATA = "play_duration";

	// Token: 0x04003B07 RID: 15111
	public const string STARTED_LATE_BODY_DATA = "started_late";

	// Token: 0x04003B08 RID: 15112
	public const string TIME_STARTED_BODY_DATA = "time_started";

	// Token: 0x04003B09 RID: 15113
	public const string CORES_COLLECTED_BODY_DATA = "cores_collected";

	// Token: 0x04003B0A RID: 15114
	public const string MAX_NUMBER_IN_GAME_BODY_DATA = "max_number_in_game";

	// Token: 0x04003B0B RID: 15115
	public const string END_NUMBER_IN_GAME_BODY_DATA = "end_number_in_game";

	// Token: 0x04003B0C RID: 15116
	public const string ITEMS_PICKED_UP_BODY_DATA = "items_picked_up";

	// Token: 0x04003B0D RID: 15117
	public const string FLOOR_JOINED_BODY_DATA = "floor_joined";

	// Token: 0x04003B0E RID: 15118
	public const string PLAYER_RANK_BODY_DATA = "player_rank";

	// Token: 0x04003B0F RID: 15119
	public const string TOTAL_CORES_COLLECTED_BY_PLAYER_BODY_DATA = "total_cores_collected_by_player";

	// Token: 0x04003B10 RID: 15120
	public const string TOTAL_CORES_COLLECTED_BY_GROUP_BODY_DATA = "total_cores_collected_by_group";

	// Token: 0x04003B11 RID: 15121
	public const string TOTAL_CORES_SPENT_BY_PLAYER_BODY_DATA = "total_cores_spent_by_player";

	// Token: 0x04003B12 RID: 15122
	public const string TOTAL_CORES_SPENT_BY_GROUP_BODY_DATA = "total_cores_spent_by_group";

	// Token: 0x04003B13 RID: 15123
	public const string FLOOR_BODY_DATA = "floor";

	// Token: 0x04003B14 RID: 15124
	public const string PRESET_BODY_DATA = "preset";

	// Token: 0x04003B15 RID: 15125
	public const string MODIFIER_BODY_DATA = "modifier";

	// Token: 0x04003B16 RID: 15126
	public const string SECTION_BODY_DATA = "section";

	// Token: 0x04003B17 RID: 15127
	public const string XP_GAINED_BODY_DATA = "xp_gained";

	// Token: 0x04003B18 RID: 15128
	public const string CHAOS_SEEDS_COLLECTED_BODY_DATA = "chaos_seeds_collected";

	// Token: 0x04003B19 RID: 15129
	public const string OBJECTIVES_COMPLETED_BODY_DATA = "objectives_completed";

	// Token: 0x04003B1A RID: 15130
	public const string REVIVES_BODY_DATA = "revives";

	// Token: 0x04003B1B RID: 15131
	public const string TOOL_BODY_DATA = "tool";

	// Token: 0x04003B1C RID: 15132
	public const string TOOL_LEVEL_BODY_DATA = "tool_level";

	// Token: 0x04003B1D RID: 15133
	public const string CORES_SPENT_BODY_DATA = "cores_spent";

	// Token: 0x04003B1E RID: 15134
	public const string SHINY_ROCKS_SPENT_BODY_DATA = "shiny_rocks_spent";

	// Token: 0x04003B1F RID: 15135
	public const string NEW_RANK_BODY_DATA = "new_rank";

	// Token: 0x04003B20 RID: 15136
	public const string UPGRADE_BODY_DATA = "upgrade";

	// Token: 0x04003B21 RID: 15137
	public const string GRIFT_PRICE_BODY_DATA = "grift_price";

	// Token: 0x04003B22 RID: 15138
	public const string TYPE_BODY_DATA = "type";

	// Token: 0x04003B23 RID: 15139
	public const string NEW_LEVEL_BODY_DATA = "new_level";

	// Token: 0x04003B24 RID: 15140
	public const string JUICE_SPENT_BODY_DATA = "juice_spent";

	// Token: 0x04003B25 RID: 15141
	public const string GRIFT_SPENT_BODY_DATA = "grift_spent";

	// Token: 0x04003B26 RID: 15142
	public const string CHAOS_SEEDS_IN_QUEUE_BODY_DATA = "chaos_seeds_in_queue";

	// Token: 0x04003B27 RID: 15143
	public const string UNLOCK_TIME_BODY_DATA = "unlock_time";

	// Token: 0x04003B28 RID: 15144
	public const string SHINY_ROCKS_USED_BODY_DATA = "shiny_rocks_used";

	// Token: 0x04003B29 RID: 15145
	public const string JUICE_COLLECTED_BODY_DATA = "juice_collected";

	// Token: 0x04003B2A RID: 15146
	public const string CORES_PROCESSED_BY_OVERDRIVE_BODY_DATA = "cores_processed_by_overdrive";

	// Token: 0x04003B2B RID: 15147
	public const string FINAL_CREDITS_BODY_DATA = "final_credits";

	// Token: 0x04003B2C RID: 15148
	public const string IS_PRIVATE_ROOM_BODY_DATA = "is_private_room";

	// Token: 0x04003B2D RID: 15149
	public const string NUM_SHIFTS_PLAYED_BODY_DATA = "num_shifts_played";
}
