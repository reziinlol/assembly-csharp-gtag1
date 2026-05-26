using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using JetBrains.Annotations;
using KID.Model;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x020008C1 RID: 2241
public static class GorillaTelemetry
{
	// Token: 0x06003A9D RID: 15005 RVA: 0x001406FC File Offset: 0x0013E8FC
	static GorillaTelemetry()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["User"] = null;
		dictionary["EventType"] = null;
		dictionary["ZoneId"] = null;
		dictionary["SubZoneId"] = null;
		GorillaTelemetry.gZoneEventArgs = dictionary;
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		dictionary2["User"] = null;
		dictionary2["EventType"] = null;
		GorillaTelemetry.gNotifEventArgs = dictionary2;
		GorillaTelemetry.nextStayTimestamp = 0f;
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		dictionary3["User"] = null;
		dictionary3["EventType"] = null;
		dictionary3["game_mode"] = null;
		GorillaTelemetry.gGameModeStartEventArgs = dictionary3;
		Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
		dictionary4["User"] = null;
		dictionary4["EventType"] = null;
		dictionary4["Items"] = null;
		GorillaTelemetry.gShopEventArgs = dictionary4;
		GorillaTelemetry.gSingleItemParam = new CosmeticsController.CosmeticItem[1];
		GorillaTelemetry.gSingleItemBuilderParam = new BuilderSetManager.BuilderSetStoreItem[1];
		Dictionary<string, object> dictionary5 = new Dictionary<string, object>();
		dictionary5["User"] = null;
		dictionary5["EventType"] = null;
		dictionary5["AgeCategory"] = null;
		dictionary5["VoiceChatEnabled"] = null;
		dictionary5["CustomUsernameEnabled"] = null;
		dictionary5["JoinGroups"] = null;
		GorillaTelemetry.gKidEventArgs = dictionary5;
		Dictionary<string, object> dictionary6 = new Dictionary<string, object>();
		dictionary6["User"] = null;
		dictionary6["WamGameId"] = null;
		dictionary6["WamMachineId"] = null;
		GorillaTelemetry.gWamGameStartArgs = dictionary6;
		Dictionary<string, object> dictionary7 = new Dictionary<string, object>();
		dictionary7["User"] = null;
		dictionary7["WamGameId"] = null;
		dictionary7["WamMachineId"] = null;
		dictionary7["WamMLevelNumber"] = null;
		dictionary7["WamGoodMolesShown"] = null;
		dictionary7["WamHazardMolesShown"] = null;
		dictionary7["WamLevelMinScore"] = null;
		dictionary7["WamLevelScore"] = null;
		dictionary7["WamHazardMolesHit"] = null;
		dictionary7["WamGameState"] = null;
		GorillaTelemetry.gWamLevelEndArgs = dictionary7;
		Dictionary<string, object> dictionary8 = new Dictionary<string, object>();
		dictionary8["CustomMapName"] = null;
		dictionary8["CustomMapModId"] = null;
		dictionary8["LowestFPS"] = null;
		dictionary8["LowestFPSDrawCalls"] = null;
		dictionary8["LowestFPSPlayerCount"] = null;
		dictionary8["AverageFPS"] = null;
		dictionary8["AverageDrawCalls"] = null;
		dictionary8["AveragePlayerCount"] = null;
		dictionary8["HighestFPS"] = null;
		dictionary8["HighestFPSDrawCalls"] = null;
		dictionary8["HighestFPSPlayerCount"] = null;
		dictionary8["PlaytimeInSeconds"] = null;
		GorillaTelemetry.gCustomMapPerfArgs = dictionary8;
		Dictionary<string, object> dictionary9 = new Dictionary<string, object>();
		dictionary9["User"] = null;
		dictionary9["CustomMapName"] = null;
		dictionary9["CustomMapModId"] = null;
		dictionary9["CustomMapCreator"] = null;
		dictionary9["MinPlayerCount"] = null;
		dictionary9["MaxPlayerCount"] = null;
		dictionary9["PlaytimeOnMap"] = null;
		dictionary9["PrivateRoom"] = null;
		GorillaTelemetry.gCustomMapTrackingMetrics = dictionary9;
		Dictionary<string, object> dictionary10 = new Dictionary<string, object>();
		dictionary10["User"] = null;
		dictionary10["CustomMapName"] = null;
		dictionary10["CustomMapModId"] = null;
		dictionary10["CustomMapCreator"] = null;
		GorillaTelemetry.gCustomMapDownloadMetrics = dictionary10;
		GorillaTelemetry.gGhostReactorShiftStartArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_game_start",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"ghost_game_id",
					null
				},
				{
					"event_timestamp",
					null
				},
				{
					"initial_cores_balance",
					null
				},
				{
					"number_of_players",
					null
				},
				{
					"start_at_beginning",
					null
				},
				{
					"seconds_into_shift_at_join",
					null
				},
				{
					"floor_joined",
					null
				},
				{
					"player_rank",
					null
				},
				{
					"is_private_room",
					null
				}
			}
		};
		GorillaTelemetry.gGhostReactorShiftEndArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_game_end",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"ghost_game_id",
					null
				},
				{
					"event_timestamp",
					null
				},
				{
					"final_cores_balance",
					null
				},
				{
					"total_cores_collected_by_player",
					null
				},
				{
					"total_cores_collected_by_group",
					null
				},
				{
					"total_cores_spent_by_player",
					null
				},
				{
					"total_cores_spent_by_group",
					null
				},
				{
					"gates_unlocked",
					null
				},
				{
					"died",
					null
				},
				{
					"items_purchased",
					null
				},
				{
					"shift_cut_data",
					null
				},
				{
					"play_duration",
					null
				},
				{
					"started_late",
					null
				},
				{
					"time_started",
					null
				},
				{
					"reason",
					null
				},
				{
					"max_number_in_game",
					null
				},
				{
					"end_number_in_game",
					null
				},
				{
					"items_picked_up",
					null
				},
				{
					"revives",
					null
				},
				{
					"num_shifts_played",
					null
				}
			}
		};
		GorillaTelemetry.gGhostReactorFloorStartArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_floor_start",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"ghost_game_id",
					null
				},
				{
					"event_timestamp",
					null
				},
				{
					"initial_cores_balance",
					null
				},
				{
					"number_of_players",
					null
				},
				{
					"start_at_beginning",
					null
				},
				{
					"seconds_into_shift_at_join",
					null
				},
				{
					"player_rank",
					null
				},
				{
					"floor",
					null
				},
				{
					"preset",
					null
				},
				{
					"modifier",
					null
				},
				{
					"is_private_room",
					null
				}
			}
		};
		GorillaTelemetry.gGhostReactorFloorEndArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_floor_end",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"ghost_game_id",
					null
				},
				{
					"event_timestamp",
					null
				},
				{
					"final_cores_balance",
					null
				},
				{
					"total_cores_collected_by_player",
					null
				},
				{
					"total_cores_collected_by_group",
					null
				},
				{
					"total_cores_spent_by_player",
					null
				},
				{
					"total_cores_spent_by_group",
					null
				},
				{
					"gates_unlocked",
					null
				},
				{
					"died",
					null
				},
				{
					"items_purchased",
					null
				},
				{
					"shift_cut_data",
					null
				},
				{
					"play_duration",
					null
				},
				{
					"started_late",
					null
				},
				{
					"time_started",
					null
				},
				{
					"reason",
					null
				},
				{
					"max_number_in_game",
					null
				},
				{
					"end_number_in_game",
					null
				},
				{
					"items_picked_up",
					null
				},
				{
					"revives",
					null
				},
				{
					"floor",
					null
				},
				{
					"preset",
					null
				},
				{
					"modifier",
					null
				},
				{
					"chaos_seeds_collected",
					null
				},
				{
					"objectives_completed",
					null
				},
				{
					"section",
					null
				},
				{
					"xp_gained",
					null
				}
			}
		};
		GorillaTelemetry.gGhostReactorToolPurchasedArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_tool_purchased",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"ghost_game_id",
					null
				},
				{
					"event_timestamp",
					null
				},
				{
					"tool",
					null
				},
				{
					"tool_level",
					null
				},
				{
					"cores_spent",
					null
				},
				{
					"shiny_rocks_spent",
					null
				},
				{
					"floor",
					null
				},
				{
					"preset",
					null
				}
			}
		};
		GorillaTelemetry.gGhostReactorRankUpArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_game_rank_up",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"ghost_game_id",
					null
				},
				{
					"event_timestamp",
					null
				},
				{
					"new_rank",
					null
				},
				{
					"floor",
					null
				},
				{
					"preset",
					null
				}
			}
		};
		GorillaTelemetry.gGhostReactorToolUnlockArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_game_tool_unlock",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"ghost_game_id",
					null
				},
				{
					"event_timestamp",
					null
				},
				{
					"tool",
					null
				}
			}
		};
		GorillaTelemetry.gGhostReactorPodUpgradePurchasedArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_pod_upgrade_purchased",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"ghost_game_id",
					null
				},
				{
					"event_timestamp",
					null
				},
				{
					"tool",
					null
				},
				{
					"new_level",
					null
				},
				{
					"shiny_rocks_spent",
					null
				},
				{
					"juice_spent",
					null
				}
			}
		};
		GorillaTelemetry.gGhostReactorToolUpgradeArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_game_tool_upgrade",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"ghost_game_id",
					null
				},
				{
					"event_timestamp",
					null
				},
				{
					"type",
					null
				},
				{
					"tool",
					null
				},
				{
					"new_level",
					null
				},
				{
					"juice_spent",
					null
				},
				{
					"grift_spent",
					null
				},
				{
					"cores_spent",
					null
				},
				{
					"floor",
					null
				},
				{
					"preset",
					null
				}
			}
		};
		GorillaTelemetry.gGhostReactorChaosSeedStartArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_chaos_seed_start",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"ghost_game_id",
					null
				},
				{
					"event_timestamp",
					null
				},
				{
					"unlock_time",
					null
				},
				{
					"chaos_seeds_in_queue",
					null
				},
				{
					"floor",
					null
				},
				{
					"preset",
					null
				}
			}
		};
		GorillaTelemetry.gGhostReactorChaosJuiceCollectedArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_chaos_juice_collected",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"ghost_game_id",
					null
				},
				{
					"event_timestamp",
					null
				},
				{
					"juice_collected",
					null
				},
				{
					"cores_processed_by_overdrive",
					null
				}
			}
		};
		GorillaTelemetry.gGhostReactorOverdrivePurchasedArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_overdrive_purchased",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"ghost_game_id",
					null
				},
				{
					"event_timestamp",
					null
				},
				{
					"shiny_rocks_used",
					null
				},
				{
					"chaos_seeds_in_queue",
					null
				},
				{
					"floor",
					null
				},
				{
					"preset",
					null
				}
			}
		};
		GorillaTelemetry.gGhostReactorCreditsRefillPurchasedArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_credits_refill_purchased",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"ghost_game_id",
					null
				},
				{
					"event_timestamp",
					null
				},
				{
					"shiny_rocks_spent",
					null
				},
				{
					"final_credits",
					null
				},
				{
					"floor",
					null
				},
				{
					"preset",
					null
				}
			}
		};
		GorillaTelemetry.gSuperInfectionArgs = new SuperInfectionTelemetryData
		{
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"event_timestamp",
					null
				},
				{
					"total_play_time",
					null
				},
				{
					"room_play_time",
					null
				},
				{
					"session_play_time",
					null
				},
				{
					"interval_play_time",
					null
				},
				{
					"terminal_total_time",
					null
				},
				{
					"terminal_interval_time",
					null
				},
				{
					"time_holding_gadget_type_total",
					null
				},
				{
					"time_holding_gadget_type_interval",
					null
				},
				{
					"time_holding_own_gadgets_total",
					null
				},
				{
					"time_holding_own_gadgets_interval",
					null
				},
				{
					"time_holding_others_gadgets_total",
					null
				},
				{
					"time_holding_others_gadgets_interval",
					null
				},
				{
					"tags_holding_gadget_type_total",
					null
				},
				{
					"tags_holding_gadget_type_interval",
					null
				},
				{
					"tags_holding_own_gadgets_total",
					null
				},
				{
					"tags_holding_own_gadgets_interval",
					null
				},
				{
					"tags_holding_others_gadgets_total",
					null
				},
				{
					"tags_holding_others_gadgets_interval",
					null
				},
				{
					"resource_type_collected_total",
					null
				},
				{
					"resource_type_collected_interval",
					null
				},
				{
					"rounds_played_total",
					null
				},
				{
					"rounds_played_interval",
					null
				},
				{
					"unlocked_nodes",
					null
				},
				{
					"player_count",
					null
				}
			}
		};
		GorillaTelemetry.gSuperInfectionPurchaseArgs = new SuperInfectionTelemetryData
		{
			EventName = "super_infection_purchase",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{
					"event_timestamp",
					null
				},
				{
					"total_play_time",
					null
				},
				{
					"room_play_time",
					null
				},
				{
					"session_play_time",
					null
				},
				{
					"si_purchase_type",
					null
				},
				{
					"si_shiny_rock_cost",
					null
				},
				{
					"si_tech_points_purchased",
					null
				}
			}
		};
		GameObject gameObject = new GameObject("GorillaTelemetryBatcher");
		Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<GorillaTelemetry.BatchRunner>();
	}

	// Token: 0x06003A9E RID: 15006 RVA: 0x00141558 File Offset: 0x0013F758
	public static void EnqueueTelemetryEvent(string eventName, object content, [CanBeNull] string[] customTags = null)
	{
		if (content == null || string.IsNullOrWhiteSpace(eventName) || !GorillaServer.Instance.CheckIsMothershipTelemetryEnabled())
		{
			return;
		}
		if (GorillaTelemetry.telemetryEventsQueueMothership.Count > 100)
		{
			Debug.LogError("[Telemetry] Too many telemetry events!  Not enqueueing " + eventName + ": " + content.ToJson(true));
			return;
		}
		GorillaTelemetry.telemetryEventsQueueMothership.Enqueue(new MothershipAnalyticsEvent
		{
			event_name = eventName,
			event_timestamp = DateTime.UtcNow.ToString("O"),
			body = JsonConvert.SerializeObject(content),
			custom_tags = ((customTags != null && customTags.Length != 0) ? GorillaTelemetry.SerializeCustomTags(customTags) : string.Empty)
		});
	}

	// Token: 0x06003A9F RID: 15007 RVA: 0x00141600 File Offset: 0x0013F800
	private static void FlushMothershipTelemetry()
	{
		int count = GorillaTelemetry.telemetryEventsQueueMothership.Count;
		if (count == 0)
		{
			return;
		}
		MothershipAnalyticsEvent[] array = ArrayPool<MothershipAnalyticsEvent>.Shared.Rent(count);
		try
		{
			int j;
			for (j = 0; j < count; j++)
			{
				MothershipAnalyticsEvent mothershipAnalyticsEvent;
				array[j] = (GorillaTelemetry.telemetryEventsQueueMothership.TryDequeue(out mothershipAnalyticsEvent) ? mothershipAnalyticsEvent : null);
			}
			if (j == 0)
			{
				ArrayPool<MothershipAnalyticsEvent>.Shared.Return(array, false);
			}
			else
			{
				MothershipWriteEventsRequest req = new MothershipWriteEventsRequest
				{
					title_id = MothershipClientApiUnity.TitleId,
					deployment_id = MothershipClientApiUnity.DeploymentId,
					env_id = MothershipClientApiUnity.EnvironmentId,
					events = new AnalyticsRequestVector(GorillaTelemetry.GetEventListForArrayMothership(array, j))
				};
				MothershipClientApiUnity.WriteEvents(MothershipClientContext.MothershipId, req, delegate(MothershipWriteEventsResponse resp)
				{
				}, delegate(MothershipError err, int i)
				{
				});
			}
		}
		finally
		{
			ArrayPool<MothershipAnalyticsEvent>.Shared.Return(array, false);
		}
	}

	// Token: 0x06003AA0 RID: 15008 RVA: 0x00141700 File Offset: 0x0013F900
	private static List<MothershipAnalyticsEvent> GetEventListForArrayMothership(MothershipAnalyticsEvent[] array, int count)
	{
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			if (array[i] != null)
			{
				num++;
			}
		}
		List<MothershipAnalyticsEvent> list;
		if (!GorillaTelemetry.gListPoolMothership.TryGetValue(num, out list))
		{
			list = new List<MothershipAnalyticsEvent>(num);
			GorillaTelemetry.gListPoolMothership.TryAdd(num, list);
		}
		else
		{
			list.Clear();
		}
		string code = LocalisationManager.CurrentLanguage.Identifier.Code;
		for (int j = 0; j < count; j++)
		{
			if (array[j] != null)
			{
				list.Add(array[j]);
			}
		}
		return list;
	}

	// Token: 0x06003AA1 RID: 15009 RVA: 0x00141783 File Offset: 0x0013F983
	private static bool IsConnected()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return false;
		}
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			GorillaTelemetry.gPlayFabAuth = PlayFabAuthenticator.instance;
		}
		return !(GorillaTelemetry.gPlayFabAuth == null);
	}

	// Token: 0x06003AA2 RID: 15010 RVA: 0x001417B6 File Offset: 0x0013F9B6
	private static bool IsConnectedToPlayfab()
	{
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			GorillaTelemetry.gPlayFabAuth = PlayFabAuthenticator.instance;
		}
		return !(GorillaTelemetry.gPlayFabAuth == null);
	}

	// Token: 0x06003AA3 RID: 15011 RVA: 0x001417B6 File Offset: 0x0013F9B6
	private static bool IsConnectedIgnoreRoom()
	{
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			GorillaTelemetry.gPlayFabAuth = PlayFabAuthenticator.instance;
		}
		return !(GorillaTelemetry.gPlayFabAuth == null);
	}

	// Token: 0x06003AA4 RID: 15012 RVA: 0x001417DB File Offset: 0x0013F9DB
	private static string PlayFabUserId()
	{
		return GorillaTelemetry.gPlayFabAuth.GetPlayFabPlayerId();
	}

	// Token: 0x06003AA5 RID: 15013 RVA: 0x001417E8 File Offset: 0x0013F9E8
	private static string SerializeCustomTags(string[] customTags)
	{
		string result = string.Empty;
		if (customTags != null && customTags.Length != 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 0; i < customTags.Length; i++)
			{
				dictionary.Add(string.Format("tag{0}", i + 1), customTags[i]);
			}
			result = JsonConvert.SerializeObject(dictionary);
		}
		return result;
	}

	// Token: 0x06003AA6 RID: 15014 RVA: 0x0014183C File Offset: 0x0013FA3C
	public static void EnqueueZoneEvent(ZoneDef zone, GTZoneEventType zoneEventType)
	{
		if (zoneEventType == GTZoneEventType.zone_stay && Time.realtimeSinceStartup < GorillaTelemetry.nextStayTimestamp)
		{
			return;
		}
		GorillaTelemetry.nextStayTimestamp = Time.realtimeSinceStartup + (float)zone.trackStayIntervalSec;
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (!GorillaServer.Instance.CheckIsTZE_Enabled())
		{
			return;
		}
		string value = GorillaTelemetry.PlayFabUserId();
		string name = zoneEventType.GetName<GTZoneEventType>();
		string name2 = zone.zoneId.GetName<GTZone>();
		string name3 = zone.subZoneId.GetName<GTSubZone>();
		bool sessionIsPrivate = NetworkSystem.Instance.SessionIsPrivate;
		Dictionary<string, object> dictionary = GorillaTelemetry.gZoneEventArgs;
		dictionary["User"] = value;
		dictionary["EventType"] = name;
		dictionary["ZoneId"] = name2;
		dictionary["SubZoneId"] = name3;
		dictionary["IsPrivateRoom"] = sessionIsPrivate;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_zone_event", dictionary, null);
	}

	// Token: 0x06003AA7 RID: 15015 RVA: 0x00141910 File Offset: 0x0013FB10
	public static void PostGameModeEvent(GTGameModeEventType gameModeEvent, GameModeType gameMode)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string value = GorillaTelemetry.PlayFabUserId();
		string name = gameModeEvent.GetName<GTGameModeEventType>();
		string name2 = gameMode.GetName<GameModeType>();
		Dictionary<string, object> dictionary = GorillaTelemetry.gGameModeStartEventArgs;
		dictionary["User"] = value;
		dictionary["EventType"] = name;
		dictionary["game_mode"] = name2;
		GorillaTelemetry.EnqueueTelemetryEvent("game_mode_played_event", dictionary, null);
	}

	// Token: 0x06003AA8 RID: 15016 RVA: 0x0014196F File Offset: 0x0013FB6F
	public static void PostShopEvent(VRRig playerRig, GTShopEventType shopEvent, CosmeticsController.CosmeticItem item)
	{
		GorillaTelemetry.gSingleItemParam[0] = item;
		GorillaTelemetry.PostShopEvent(playerRig, shopEvent, GorillaTelemetry.gSingleItemParam);
		GorillaTelemetry.gSingleItemParam[0] = default(CosmeticsController.CosmeticItem);
	}

	// Token: 0x06003AA9 RID: 15017 RVA: 0x0014199C File Offset: 0x0013FB9C
	private static string[] FetchItemArgs(IList<CosmeticsController.CosmeticItem> items)
	{
		int count = items.Count;
		if (count == 0)
		{
			return Array.Empty<string>();
		}
		HashSet<string> hashSet = new HashSet<string>(count);
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			CosmeticsController.CosmeticItem cosmeticItem = items[i];
			if (!cosmeticItem.isNullItem)
			{
				string itemName = cosmeticItem.itemName;
				if (!string.IsNullOrWhiteSpace(itemName) && !itemName.Contains("NOTHING", StringComparison.InvariantCultureIgnoreCase) && hashSet.Add(itemName))
				{
					num++;
				}
			}
		}
		string[] array = new string[num];
		hashSet.CopyTo(array);
		return array;
	}

	// Token: 0x06003AAA RID: 15018 RVA: 0x00141A28 File Offset: 0x0013FC28
	public static void PostShopEvent(VRRig playerRig, GTShopEventType shopEvent, IList<CosmeticsController.CosmeticItem> items)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (!playerRig.isLocal)
		{
			return;
		}
		string value = GorillaTelemetry.PlayFabUserId();
		string name = shopEvent.GetName<GTShopEventType>();
		string[] value2 = GorillaTelemetry.FetchItemArgs(items);
		Dictionary<string, object> dictionary = GorillaTelemetry.gShopEventArgs;
		dictionary["User"] = value;
		dictionary["EventType"] = name;
		dictionary["Items"] = value2;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_shop_event", dictionary, null);
	}

	// Token: 0x06003AAB RID: 15019 RVA: 0x00141A90 File Offset: 0x0013FC90
	public static void PostBuilderKioskEvent(VRRig playerRig, GTShopEventType shopEvent, BuilderSetManager.BuilderSetStoreItem item)
	{
		GorillaTelemetry.gSingleItemBuilderParam[0] = item;
		GorillaTelemetry.PostBuilderKioskEvent(playerRig, shopEvent, GorillaTelemetry.gSingleItemBuilderParam);
		GorillaTelemetry.gSingleItemBuilderParam[0] = default(BuilderSetManager.BuilderSetStoreItem);
	}

	// Token: 0x06003AAC RID: 15020 RVA: 0x00141ABC File Offset: 0x0013FCBC
	private static string[] BuilderItemsToStrings(IList<BuilderSetManager.BuilderSetStoreItem> items)
	{
		int count = items.Count;
		if (count == 0)
		{
			return Array.Empty<string>();
		}
		HashSet<string> hashSet = new HashSet<string>(count);
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem = items[i];
			if (!builderSetStoreItem.isNullItem)
			{
				string playfabID = builderSetStoreItem.playfabID;
				if (!string.IsNullOrWhiteSpace(playfabID) && !playfabID.Contains("NOTHING", StringComparison.InvariantCultureIgnoreCase) && hashSet.Add(playfabID))
				{
					num++;
				}
			}
		}
		string[] array = new string[num];
		hashSet.CopyTo(array);
		return array;
	}

	// Token: 0x06003AAD RID: 15021 RVA: 0x00141B48 File Offset: 0x0013FD48
	public static void PostBuilderKioskEvent(VRRig playerRig, GTShopEventType shopEvent, IList<BuilderSetManager.BuilderSetStoreItem> items)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (!playerRig.isLocal)
		{
			return;
		}
		string value = GorillaTelemetry.PlayFabUserId();
		string name = shopEvent.GetName<GTShopEventType>();
		string[] value2 = GorillaTelemetry.BuilderItemsToStrings(items);
		Dictionary<string, object> dictionary = GorillaTelemetry.gShopEventArgs;
		dictionary["User"] = value;
		dictionary["EventType"] = name;
		dictionary["Items"] = value2;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_shop_event", dictionary, null);
	}

	// Token: 0x06003AAE RID: 15022 RVA: 0x00141BB0 File Offset: 0x0013FDB0
	public static void PostKidEvent(bool joinGroupsEnabled, bool voiceChatEnabled, bool customUsernamesEnabled, AgeStatusType ageCategory, GTKidEventType kidEvent)
	{
		if ((double)Random.value < 0.1)
		{
			return;
		}
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string value = GorillaTelemetry.PlayFabUserId();
		string name = kidEvent.GetName<GTKidEventType>();
		string value2 = (ageCategory == AgeStatusType.LEGALADULT) ? "Not_Managed_Account" : "Managed_Account";
		string value3 = joinGroupsEnabled.ToString().ToUpper();
		string value4 = voiceChatEnabled.ToString().ToUpper();
		string value5 = customUsernamesEnabled.ToString().ToUpper();
		Dictionary<string, object> dictionary = GorillaTelemetry.gKidEventArgs;
		dictionary["User"] = value;
		dictionary["EventType"] = name;
		dictionary["AgeCategory"] = value2;
		dictionary["VoiceChatEnabled"] = value4;
		dictionary["CustomUsernameEnabled"] = value5;
		dictionary["JoinGroups"] = value3;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_kid_event", dictionary, null);
	}

	// Token: 0x06003AAF RID: 15023 RVA: 0x00141C84 File Offset: 0x0013FE84
	public static void WamGameStart(string playerId, string gameId, string machineId)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gWamGameStartArgs["User"] = playerId;
		GorillaTelemetry.gWamGameStartArgs["WamGameId"] = gameId;
		GorillaTelemetry.gWamGameStartArgs["WamMachineId"] = machineId;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_wam_gameStartEvent", GorillaTelemetry.gWamGameStartArgs, null);
	}

	// Token: 0x06003AB0 RID: 15024 RVA: 0x00141CDC File Offset: 0x0013FEDC
	public static void WamLevelEnd(string playerId, int gameId, string machineId, int currentLevelNumber, int levelGoodMolesShown, int levelHazardMolesShown, int levelMinScore, int currentScore, int levelHazardMolesHit, string currentGameResult)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gWamLevelEndArgs["User"] = playerId;
		GorillaTelemetry.gWamLevelEndArgs["WamGameId"] = gameId.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamMachineId"] = machineId;
		GorillaTelemetry.gWamLevelEndArgs["WamMLevelNumber"] = currentLevelNumber.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamGoodMolesShown"] = levelGoodMolesShown.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamHazardMolesShown"] = levelHazardMolesShown.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamLevelMinScore"] = levelMinScore.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamLevelScore"] = currentScore.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamHazardMolesHit"] = levelHazardMolesHit.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamGameState"] = currentGameResult;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_wam_levelEndEvent", GorillaTelemetry.gWamLevelEndArgs, null);
	}

	// Token: 0x06003AB1 RID: 15025 RVA: 0x00141DCC File Offset: 0x0013FFCC
	public static void PostCustomMapPerformance(string mapName, long mapModId, int lowestFPS, int lowestDC, int lowestPC, int avgFPS, int avgDC, int avgPC, int highestFPS, int highestDC, int highestPC, int playtime)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		Dictionary<string, object> dictionary = GorillaTelemetry.gCustomMapPerfArgs;
		dictionary["CustomMapName"] = mapName;
		dictionary["CustomMapModId"] = mapModId.ToString();
		dictionary["LowestFPS"] = lowestFPS.ToString();
		dictionary["LowestFPSDrawCalls"] = lowestDC.ToString();
		dictionary["LowestFPSPlayerCount"] = lowestPC.ToString();
		dictionary["AverageFPS"] = avgFPS.ToString();
		dictionary["AverageDrawCalls"] = avgDC.ToString();
		dictionary["AveragePlayerCount"] = avgPC.ToString();
		dictionary["HighestFPS"] = highestFPS.ToString();
		dictionary["HighestFPSDrawCalls"] = highestDC.ToString();
		dictionary["HighestFPSPlayerCount"] = highestPC.ToString();
		dictionary["PlaytimeInSeconds"] = playtime.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent("CustomMapPerformance", dictionary, null);
	}

	// Token: 0x06003AB2 RID: 15026 RVA: 0x00141EC8 File Offset: 0x001400C8
	public static void PostCustomMapTracking(string mapName, long mapModId, string mapCreatorUsername, int minPlayers, int maxPlayers, int playtime, bool privateRoom)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		int num = playtime % 60;
		int num2 = (playtime - num) / 60;
		int num3 = num2 % 60;
		int num4 = (num2 - num3) / 60;
		string value = string.Format("{0}.{1}.{2}", num4, num3, num);
		Dictionary<string, object> dictionary = GorillaTelemetry.gCustomMapTrackingMetrics;
		dictionary["User"] = GorillaTelemetry.PlayFabUserId();
		dictionary["CustomMapName"] = mapName;
		dictionary["CustomMapModId"] = mapModId.ToString();
		dictionary["CustomMapCreator"] = mapCreatorUsername;
		dictionary["MinPlayerCount"] = minPlayers.ToString();
		dictionary["MaxPlayerCount"] = maxPlayers.ToString();
		dictionary["PlaytimeInSeconds"] = playtime.ToString();
		dictionary["PrivateRoom"] = privateRoom.ToString();
		dictionary["PlaytimeOnMap"] = value;
		GorillaTelemetry.EnqueueTelemetryEvent("CustomMapTracking", dictionary, null);
	}

	// Token: 0x06003AB3 RID: 15027 RVA: 0x000028C5 File Offset: 0x00000AC5
	public static void PostCustomMapDownloadEvent(string mapName, long mapModId, string mapCreatorUsername)
	{
	}

	// Token: 0x06003AB4 RID: 15028 RVA: 0x00141FC0 File Offset: 0x001401C0
	public static void GhostReactorShiftStart(string gameId, int initialCores, float timeIntoShift, bool wasPlayerInAtStart, int numPlayers, int floorJoined, string playerRank)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorShiftStartArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["initial_cores_balance"] = initialCores.ToString();
		ghostReactorTelemetryData.BodyData["number_of_players"] = numPlayers.ToString();
		ghostReactorTelemetryData.BodyData["start_at_beginning"] = wasPlayerInAtStart.ToString();
		ghostReactorTelemetryData.BodyData["seconds_into_shift_at_join"] = timeIntoShift.ToString();
		ghostReactorTelemetryData.BodyData["floor_joined"] = floorJoined.ToString();
		ghostReactorTelemetryData.BodyData["player_rank"] = playerRank;
		ghostReactorTelemetryData.BodyData["is_private_room"] = NetworkSystem.Instance.SessionIsPrivate.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003AB5 RID: 15029 RVA: 0x001420C8 File Offset: 0x001402C8
	public static void GhostReactorGameEnd(string gameId, int finalCores, int totalCoresCollectedByPlayer, int totalCoresCollectedByGroup, int totalCoresSpentByPlayer, int totalCoresSpentByGroup, int gatesUnlocked, int deaths, List<string> itemsPurchased, int shiftCut, bool isShiftActuallyEnding, float timeIntoShiftAtJoin, float playDuration, bool wasPlayerInAtStart, ZoneClearReason zoneClearReason, int maxNumberOfPlayersInShift, int endNumberOfPlayers, Dictionary<string, int> itemTypesHeldThisShift, int revives, int numShiftsPlayed)
	{
		if (!GorillaTelemetry.IsConnectedToPlayfab())
		{
			return;
		}
		string value = "shift_ended";
		if (!isShiftActuallyEnding)
		{
			if (zoneClearReason == ZoneClearReason.LeaveZone)
			{
				value = "left_zone";
			}
			else
			{
				value = "disconnect";
			}
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorShiftEndArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["final_cores_balance"] = finalCores.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_collected_by_player"] = totalCoresCollectedByPlayer.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_collected_by_group"] = totalCoresCollectedByGroup.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_spent_by_player"] = totalCoresSpentByPlayer.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_spent_by_group"] = totalCoresSpentByGroup.ToString();
		ghostReactorTelemetryData.BodyData["gates_unlocked"] = gatesUnlocked.ToString();
		ghostReactorTelemetryData.BodyData["died"] = deaths.ToString();
		ghostReactorTelemetryData.BodyData["items_purchased"] = itemsPurchased.ToJson(true);
		ghostReactorTelemetryData.BodyData["shift_cut_data"] = shiftCut.ToJson(true);
		ghostReactorTelemetryData.BodyData["play_duration"] = playDuration.ToString();
		ghostReactorTelemetryData.BodyData["started_late"] = (!wasPlayerInAtStart).ToString();
		ghostReactorTelemetryData.BodyData["time_started"] = timeIntoShiftAtJoin.ToString();
		ghostReactorTelemetryData.BodyData["reason"] = value;
		ghostReactorTelemetryData.BodyData["max_number_in_game"] = maxNumberOfPlayersInShift.ToString();
		ghostReactorTelemetryData.BodyData["end_number_in_game"] = endNumberOfPlayers.ToString();
		ghostReactorTelemetryData.BodyData["items_picked_up"] = itemTypesHeldThisShift.ToJson(true);
		ghostReactorTelemetryData.BodyData["revives"] = revives.ToString();
		ghostReactorTelemetryData.BodyData["num_shifts_played"] = numShiftsPlayed.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003AB6 RID: 15030 RVA: 0x001422E8 File Offset: 0x001404E8
	public static void GhostReactorFloorStart(string gameId, int initialCores, float timeIntoShift, bool wasPlayerInAtStart, int numPlayers, string playerRank, int floor, string preset, string modifier)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorFloorStartArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["initial_cores_balance"] = initialCores.ToString();
		ghostReactorTelemetryData.BodyData["number_of_players"] = numPlayers.ToString();
		ghostReactorTelemetryData.BodyData["start_at_beginning"] = wasPlayerInAtStart.ToString();
		ghostReactorTelemetryData.BodyData["seconds_into_shift_at_join"] = timeIntoShift.ToString();
		ghostReactorTelemetryData.BodyData["player_rank"] = playerRank;
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		ghostReactorTelemetryData.BodyData["modifier"] = modifier;
		ghostReactorTelemetryData.BodyData["is_private_room"] = NetworkSystem.Instance.SessionIsPrivate.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003AB7 RID: 15031 RVA: 0x00142414 File Offset: 0x00140614
	public static void GhostReactorFloorComplete(string gameId, int finalCores, int totalCoresCollectedByPlayer, int totalCoresCollectedByGroup, int totalCoresSpentByPlayer, int totalCoresSpentByGroup, int gatesUnlocked, int deaths, List<string> itemsPurchased, int shiftCut, bool isShiftActuallyEnding, float timeIntoShiftAtJoin, float playDuration, bool wasPlayerInAtStart, ZoneClearReason zoneClearReason, int maxNumberOfPlayersInShift, int endNumberOfPlayers, Dictionary<string, int> itemTypesHeldThisShift, int revives, int floor, string preset, string modifier, int chaosSeedsCollected, bool objectivesCompleted, string section, int xpGained)
	{
		if (!GorillaTelemetry.IsConnectedToPlayfab())
		{
			return;
		}
		string value = "shift_ended";
		if (!isShiftActuallyEnding)
		{
			if (zoneClearReason == ZoneClearReason.LeaveZone)
			{
				value = "left_zone";
			}
			else
			{
				value = "disconnect";
			}
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorFloorEndArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["final_cores_balance"] = finalCores.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_collected_by_player"] = totalCoresCollectedByPlayer.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_collected_by_group"] = totalCoresCollectedByGroup.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_spent_by_player"] = totalCoresSpentByPlayer.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_spent_by_group"] = totalCoresSpentByGroup.ToString();
		ghostReactorTelemetryData.BodyData["gates_unlocked"] = gatesUnlocked.ToString();
		ghostReactorTelemetryData.BodyData["died"] = deaths.ToString();
		ghostReactorTelemetryData.BodyData["items_purchased"] = itemsPurchased.ToJson(true);
		ghostReactorTelemetryData.BodyData["shift_cut_data"] = shiftCut.ToJson(true);
		ghostReactorTelemetryData.BodyData["play_duration"] = playDuration.ToString();
		ghostReactorTelemetryData.BodyData["started_late"] = (!wasPlayerInAtStart).ToString();
		ghostReactorTelemetryData.BodyData["time_started"] = timeIntoShiftAtJoin.ToString();
		ghostReactorTelemetryData.BodyData["reason"] = value;
		ghostReactorTelemetryData.BodyData["max_number_in_game"] = maxNumberOfPlayersInShift.ToString();
		ghostReactorTelemetryData.BodyData["end_number_in_game"] = endNumberOfPlayers.ToString();
		ghostReactorTelemetryData.BodyData["items_picked_up"] = itemTypesHeldThisShift.ToJson(true);
		ghostReactorTelemetryData.BodyData["revives"] = revives.ToString();
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		ghostReactorTelemetryData.BodyData["modifier"] = modifier;
		ghostReactorTelemetryData.BodyData["chaos_seeds_collected"] = chaosSeedsCollected.ToString();
		ghostReactorTelemetryData.BodyData["objectives_completed"] = objectivesCompleted.ToString();
		ghostReactorTelemetryData.BodyData["section"] = section;
		ghostReactorTelemetryData.BodyData["xp_gained"] = xpGained.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003AB8 RID: 15032 RVA: 0x001426B0 File Offset: 0x001408B0
	public static void GhostReactorToolPurchased(string gameId, string toolName, int toolLevel, int coresSpent, int shinyRocksSpent, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorToolPurchasedArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["tool"] = toolName;
		ghostReactorTelemetryData.BodyData["tool_level"] = toolLevel.ToString();
		ghostReactorTelemetryData.BodyData["cores_spent"] = coresSpent.ToString();
		ghostReactorTelemetryData.BodyData["shiny_rocks_spent"] = shinyRocksSpent.ToString();
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003AB9 RID: 15033 RVA: 0x00142790 File Offset: 0x00140990
	public static void GhostReactorRankUp(string gameId, string newRank, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorRankUpArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["new_rank"] = newRank;
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003ABA RID: 15034 RVA: 0x0014282C File Offset: 0x00140A2C
	public static void GhostReactorToolUnlock(string gameId, string toolName)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorToolUnlockArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["tool"] = toolName;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003ABB RID: 15035 RVA: 0x001428A0 File Offset: 0x00140AA0
	public static void GhostReactorPodUpgradePurchased(string gameId, string toolName, int level, int shinyRocksSpent, int juiceSpent)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorPodUpgradePurchasedArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["tool"] = toolName;
		ghostReactorTelemetryData.BodyData["new_level"] = level.ToString();
		ghostReactorTelemetryData.BodyData["shiny_rocks_spent"] = shinyRocksSpent.ToString();
		ghostReactorTelemetryData.BodyData["juice_spent"] = juiceSpent.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003ABC RID: 15036 RVA: 0x00142958 File Offset: 0x00140B58
	public static void GhostReactorToolUpgrade(string gameId, string upgradeType, string toolName, int newLevel, int juiceSpent, int griftSpent, int coresSpent, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorToolUpgradeArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["type"] = upgradeType;
		ghostReactorTelemetryData.BodyData["tool"] = toolName;
		ghostReactorTelemetryData.BodyData["new_level"] = newLevel.ToString();
		ghostReactorTelemetryData.BodyData["juice_spent"] = juiceSpent.ToString();
		ghostReactorTelemetryData.BodyData["grift_spent"] = griftSpent.ToString();
		ghostReactorTelemetryData.BodyData["cores_spent"] = coresSpent.ToString();
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003ABD RID: 15037 RVA: 0x00142A60 File Offset: 0x00140C60
	public static void GhostReactorChaosSeedStart(string gameId, string unlockTime, int chaosSeedsInQueue, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorChaosSeedStartArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["unlock_time"] = unlockTime;
		ghostReactorTelemetryData.BodyData["chaos_seeds_in_queue"] = chaosSeedsInQueue.ToString();
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003ABE RID: 15038 RVA: 0x00142B14 File Offset: 0x00140D14
	public static void GhostReactorChaosJuiceCollected(string gameId, int juiceCollected, int coresProcessedByOverdrive)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorChaosJuiceCollectedArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["juice_collected"] = juiceCollected.ToString();
		ghostReactorTelemetryData.BodyData["cores_processed_by_overdrive"] = coresProcessedByOverdrive.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003ABF RID: 15039 RVA: 0x00142BA4 File Offset: 0x00140DA4
	public static void GhostReactorOverdrivePurchased(string gameId, int shinyRocksUsed, int chaosSeedsInQueue, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorOverdrivePurchasedArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["shiny_rocks_used"] = shinyRocksUsed.ToString();
		ghostReactorTelemetryData.BodyData["chaos_seeds_in_queue"] = chaosSeedsInQueue.ToString();
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003AC0 RID: 15040 RVA: 0x00142C5C File Offset: 0x00140E5C
	public static void GhostReactorCreditsRefillPurchased(string gameId, int shinyRocksSpent, int finalCredits, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorCreditsRefillPurchasedArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["shiny_rocks_spent"] = shinyRocksSpent.ToString();
		ghostReactorTelemetryData.BodyData["final_credits"] = finalCredits.ToString();
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003AC1 RID: 15041 RVA: 0x00142D14 File Offset: 0x00140F14
	public unsafe static void SuperInfectionEvent(bool roomDisconnect, float totalPlayTime, float roomPlayTime, float sessionPlayTime, float intervalPlayTime, float terminalTotalTime, float terminalIntervalTime, Dictionary<SITechTreePageId, float> timeUsingGadgetsTotal, Dictionary<SITechTreePageId, float> timeUsingGadgetsInterval, float timeUsingOwnGadgetsTotal, float timeUsingOwnGadgetsInterval, float timeUsingOthersGadgetsTotal, float timeUsingOthersGadgetsInterval, Dictionary<SITechTreePageId, int> tagsUsingGadgetsTotal, Dictionary<SITechTreePageId, int> tagsUsingGadgetsInterval, int tagsHoldingOwnGadgetsTotal, int tagsHoldingOwnGadgetsInterval, int tagsHoldingOthersGadgetsTotal, int tagsHoldingOthersGadgetsInterval, Dictionary<SIResource.ResourceType, int> resourcesGatheredTotal, Dictionary<SIResource.ResourceType, int> resourcesGatheredInterval, int roundsPlayedTotal, int roundsPlayedInterval, bool[][] unlockedNodes, int numberOfPlayers)
	{
		if (!GorillaTelemetry.IsConnectedIgnoreRoom())
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < unlockedNodes.Length; i++)
		{
			num += unlockedNodes[i].Length;
		}
		int num2 = num;
		Span<char> span = new Span<char>(stackalloc byte[checked(unchecked((UIntPtr)num2) * 2)], num2);
		num = 0;
		for (int j = 0; j < unlockedNodes.Length; j++)
		{
			for (int k = 0; k < unlockedNodes[j].Length; k++)
			{
				*span[num] = (unlockedNodes[j][k] ? '1' : '0');
				num++;
			}
		}
		SuperInfectionTelemetryData superInfectionTelemetryData = GorillaTelemetry.gSuperInfectionArgs;
		superInfectionTelemetryData.EventName = (roomDisconnect ? "super_infection_room_left" : "super_infection_interval");
		Dictionary<string, object> bodyData = superInfectionTelemetryData.BodyData;
		if (bodyData["tags_holding_gadget_type_total"] == null)
		{
			bodyData["tags_holding_gadget_type_total"] = new Dictionary<string, object>();
		}
		bodyData = superInfectionTelemetryData.BodyData;
		if (bodyData["tags_holding_gadget_type_interval"] == null)
		{
			bodyData["tags_holding_gadget_type_interval"] = new Dictionary<string, object>();
		}
		Dictionary<string, object> dictionary = (Dictionary<string, object>)superInfectionTelemetryData.BodyData["tags_holding_gadget_type_total"];
		Dictionary<string, object> dictionary2 = (Dictionary<string, object>)superInfectionTelemetryData.BodyData["tags_holding_gadget_type_interval"];
		for (int l = 0; l < 11; l++)
		{
			SITechTreePageId key = (SITechTreePageId)l;
			int num3;
			tagsUsingGadgetsTotal.TryGetValue(key, out num3);
			int num4;
			tagsUsingGadgetsInterval.TryGetValue(key, out num4);
			string key2 = key.ToString();
			dictionary[key2] = num3.ToString();
			dictionary2[key2] = num4.ToString();
		}
		bodyData = superInfectionTelemetryData.BodyData;
		if (bodyData["resource_type_collected_total"] == null)
		{
			bodyData["resource_type_collected_total"] = new Dictionary<string, object>();
		}
		bodyData = superInfectionTelemetryData.BodyData;
		if (bodyData["resource_type_collected_interval"] == null)
		{
			bodyData["resource_type_collected_interval"] = new Dictionary<string, object>();
		}
		Dictionary<string, object> dictionary3 = (Dictionary<string, object>)superInfectionTelemetryData.BodyData["resource_type_collected_total"];
		Dictionary<string, object> dictionary4 = (Dictionary<string, object>)superInfectionTelemetryData.BodyData["resource_type_collected_interval"];
		for (int m = 0; m < 6; m++)
		{
			SIResource.ResourceType key3 = (SIResource.ResourceType)m;
			int num5;
			resourcesGatheredTotal.TryGetValue(key3, out num5);
			int num6;
			resourcesGatheredInterval.TryGetValue(key3, out num6);
			string key4 = key3.ToString();
			dictionary3[key4] = num5.ToString();
			dictionary4[key4] = num6.ToString();
		}
		superInfectionTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		superInfectionTelemetryData.BodyData["total_play_time"] = totalPlayTime.ToString();
		superInfectionTelemetryData.BodyData["room_play_time"] = roomPlayTime.ToString();
		superInfectionTelemetryData.BodyData["session_play_time"] = sessionPlayTime.ToString();
		superInfectionTelemetryData.BodyData["interval_play_time"] = intervalPlayTime.ToString();
		superInfectionTelemetryData.BodyData["terminal_total_time"] = terminalTotalTime.ToString();
		superInfectionTelemetryData.BodyData["terminal_interval_time"] = terminalIntervalTime.ToString();
		superInfectionTelemetryData.BodyData["time_holding_gadget_type_total"] = timeUsingGadgetsTotal;
		superInfectionTelemetryData.BodyData["time_holding_gadget_type_interval"] = timeUsingGadgetsInterval;
		superInfectionTelemetryData.BodyData["time_holding_own_gadgets_total"] = timeUsingOwnGadgetsTotal.ToString();
		superInfectionTelemetryData.BodyData["time_holding_own_gadgets_interval"] = timeUsingOwnGadgetsInterval.ToString();
		superInfectionTelemetryData.BodyData["time_holding_others_gadgets_total"] = timeUsingOthersGadgetsTotal.ToString();
		superInfectionTelemetryData.BodyData["time_holding_others_gadgets_interval"] = timeUsingOthersGadgetsInterval.ToString();
		superInfectionTelemetryData.BodyData["tags_holding_gadget_type_total"] = dictionary;
		superInfectionTelemetryData.BodyData["tags_holding_gadget_type_interval"] = dictionary2;
		superInfectionTelemetryData.BodyData["tags_holding_own_gadgets_total"] = tagsHoldingOwnGadgetsTotal.ToString();
		superInfectionTelemetryData.BodyData["tags_holding_own_gadgets_interval"] = tagsHoldingOwnGadgetsInterval.ToString();
		superInfectionTelemetryData.BodyData["tags_holding_others_gadgets_total"] = tagsHoldingOthersGadgetsTotal.ToString();
		superInfectionTelemetryData.BodyData["tags_holding_others_gadgets_interval"] = tagsHoldingOthersGadgetsInterval.ToString();
		superInfectionTelemetryData.BodyData["resource_type_collected_total"] = dictionary3;
		superInfectionTelemetryData.BodyData["resource_type_collected_interval"] = dictionary4;
		superInfectionTelemetryData.BodyData["rounds_played_total"] = roundsPlayedTotal.ToString();
		superInfectionTelemetryData.BodyData["rounds_played_interval"] = roundsPlayedInterval.ToString();
		superInfectionTelemetryData.BodyData["unlocked_nodes"] = new string(span);
		superInfectionTelemetryData.BodyData["player_count"] = numberOfPlayers.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(superInfectionTelemetryData.EventName, superInfectionTelemetryData.BodyData, superInfectionTelemetryData.CustomTags);
	}

	// Token: 0x06003AC2 RID: 15042 RVA: 0x001431BC File Offset: 0x001413BC
	public static void SuperInfectionEvent(string purchaseType, int shinyRockCost, int techPointsPurchased, float totalPlayTime, float roomPlayTime, float sessionPlayTime)
	{
		if (!GorillaTelemetry.IsConnectedIgnoreRoom())
		{
			return;
		}
		SuperInfectionTelemetryData superInfectionTelemetryData = GorillaTelemetry.gSuperInfectionPurchaseArgs;
		superInfectionTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		superInfectionTelemetryData.BodyData["total_play_time"] = totalPlayTime.ToString();
		superInfectionTelemetryData.BodyData["room_play_time"] = roomPlayTime.ToString();
		superInfectionTelemetryData.BodyData["session_play_time"] = sessionPlayTime.ToString();
		superInfectionTelemetryData.BodyData["si_purchase_type"] = purchaseType;
		superInfectionTelemetryData.BodyData["si_shiny_rock_cost"] = shinyRockCost.ToString();
		superInfectionTelemetryData.BodyData["si_tech_points_purchased"] = techPointsPurchased.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(superInfectionTelemetryData.EventName, superInfectionTelemetryData.BodyData, superInfectionTelemetryData.CustomTags);
	}

	// Token: 0x06003AC3 RID: 15043 RVA: 0x00143290 File Offset: 0x00141490
	public static void PostNotificationEvent(string notificationType)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string value = GorillaTelemetry.PlayFabUserId();
		Dictionary<string, object> dictionary = GorillaTelemetry.gNotifEventArgs;
		dictionary["User"] = value;
		dictionary["EventType"] = notificationType;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_ggwp_event", dictionary, null);
	}

	// Token: 0x04004AEA RID: 19178
	private static readonly float TELEMETRY_FLUSH_SEC = 10f;

	// Token: 0x04004AEB RID: 19179
	private static readonly ConcurrentQueue<MothershipAnalyticsEvent> telemetryEventsQueueMothership = new ConcurrentQueue<MothershipAnalyticsEvent>();

	// Token: 0x04004AEC RID: 19180
	private static readonly Dictionary<int, List<MothershipAnalyticsEvent>> gListPoolMothership = new Dictionary<int, List<MothershipAnalyticsEvent>>();

	// Token: 0x04004AED RID: 19181
	private static PlayFabAuthenticator gPlayFabAuth;

	// Token: 0x04004AEE RID: 19182
	private static readonly Dictionary<string, object> gZoneEventArgs;

	// Token: 0x04004AEF RID: 19183
	private static readonly Dictionary<string, object> gNotifEventArgs;

	// Token: 0x04004AF0 RID: 19184
	public static float nextStayTimestamp;

	// Token: 0x04004AF1 RID: 19185
	private static readonly Dictionary<string, object> gGameModeStartEventArgs;

	// Token: 0x04004AF2 RID: 19186
	private static readonly Dictionary<string, object> gShopEventArgs;

	// Token: 0x04004AF3 RID: 19187
	private static CosmeticsController.CosmeticItem[] gSingleItemParam;

	// Token: 0x04004AF4 RID: 19188
	private static BuilderSetManager.BuilderSetStoreItem[] gSingleItemBuilderParam;

	// Token: 0x04004AF5 RID: 19189
	private static Dictionary<string, object> gKidEventArgs;

	// Token: 0x04004AF6 RID: 19190
	private static readonly Dictionary<string, object> gWamGameStartArgs;

	// Token: 0x04004AF7 RID: 19191
	private static readonly Dictionary<string, object> gWamLevelEndArgs;

	// Token: 0x04004AF8 RID: 19192
	private static Dictionary<string, object> gCustomMapPerfArgs;

	// Token: 0x04004AF9 RID: 19193
	private static Dictionary<string, object> gCustomMapTrackingMetrics;

	// Token: 0x04004AFA RID: 19194
	private static Dictionary<string, object> gCustomMapDownloadMetrics;

	// Token: 0x04004AFB RID: 19195
	private static readonly GhostReactorTelemetryData gGhostReactorShiftStartArgs;

	// Token: 0x04004AFC RID: 19196
	private static readonly GhostReactorTelemetryData gGhostReactorShiftEndArgs;

	// Token: 0x04004AFD RID: 19197
	private static readonly GhostReactorTelemetryData gGhostReactorFloorStartArgs;

	// Token: 0x04004AFE RID: 19198
	private static readonly GhostReactorTelemetryData gGhostReactorFloorEndArgs;

	// Token: 0x04004AFF RID: 19199
	private static readonly GhostReactorTelemetryData gGhostReactorToolPurchasedArgs;

	// Token: 0x04004B00 RID: 19200
	private static readonly GhostReactorTelemetryData gGhostReactorRankUpArgs;

	// Token: 0x04004B01 RID: 19201
	private static readonly GhostReactorTelemetryData gGhostReactorToolUnlockArgs;

	// Token: 0x04004B02 RID: 19202
	private static readonly GhostReactorTelemetryData gGhostReactorPodUpgradePurchasedArgs;

	// Token: 0x04004B03 RID: 19203
	private static readonly GhostReactorTelemetryData gGhostReactorToolUpgradeArgs;

	// Token: 0x04004B04 RID: 19204
	private static readonly GhostReactorTelemetryData gGhostReactorChaosSeedStartArgs;

	// Token: 0x04004B05 RID: 19205
	private static readonly GhostReactorTelemetryData gGhostReactorChaosJuiceCollectedArgs;

	// Token: 0x04004B06 RID: 19206
	private static readonly GhostReactorTelemetryData gGhostReactorOverdrivePurchasedArgs;

	// Token: 0x04004B07 RID: 19207
	private static readonly GhostReactorTelemetryData gGhostReactorCreditsRefillPurchasedArgs;

	// Token: 0x04004B08 RID: 19208
	private static readonly SuperInfectionTelemetryData gSuperInfectionArgs;

	// Token: 0x04004B09 RID: 19209
	private static readonly SuperInfectionTelemetryData gSuperInfectionPurchaseArgs;

	// Token: 0x020008C2 RID: 2242
	public static class k
	{
		// Token: 0x04004B0A RID: 19210
		public const string User = "User";

		// Token: 0x04004B0B RID: 19211
		public const string ZoneId = "ZoneId";

		// Token: 0x04004B0C RID: 19212
		public const string SubZoneId = "SubZoneId";

		// Token: 0x04004B0D RID: 19213
		public const string EventType = "EventType";

		// Token: 0x04004B0E RID: 19214
		public const string IsPrivateRoom = "IsPrivateRoom";

		// Token: 0x04004B0F RID: 19215
		public const string Items = "Items";

		// Token: 0x04004B10 RID: 19216
		public const string VoiceChatEnabled = "VoiceChatEnabled";

		// Token: 0x04004B11 RID: 19217
		public const string JoinGroups = "JoinGroups";

		// Token: 0x04004B12 RID: 19218
		public const string CustomUsernameEnabled = "CustomUsernameEnabled";

		// Token: 0x04004B13 RID: 19219
		public const string AgeCategory = "AgeCategory";

		// Token: 0x04004B14 RID: 19220
		public const string telemetry_zone_event = "telemetry_zone_event";

		// Token: 0x04004B15 RID: 19221
		public const string telemetry_shop_event = "telemetry_shop_event";

		// Token: 0x04004B16 RID: 19222
		public const string telemetry_kid_event = "telemetry_kid_event";

		// Token: 0x04004B17 RID: 19223
		public const string telemetry_ggwp_event = "telemetry_ggwp_event";

		// Token: 0x04004B18 RID: 19224
		public const string NOTHING = "NOTHING";

		// Token: 0x04004B19 RID: 19225
		public const string telemetry_wam_gameStartEvent = "telemetry_wam_gameStartEvent";

		// Token: 0x04004B1A RID: 19226
		public const string telemetry_wam_levelEndEvent = "telemetry_wam_levelEndEvent";

		// Token: 0x04004B1B RID: 19227
		public const string WamMachineId = "WamMachineId";

		// Token: 0x04004B1C RID: 19228
		public const string WamGameId = "WamGameId";

		// Token: 0x04004B1D RID: 19229
		public const string WamMLevelNumber = "WamMLevelNumber";

		// Token: 0x04004B1E RID: 19230
		public const string WamGoodMolesShown = "WamGoodMolesShown";

		// Token: 0x04004B1F RID: 19231
		public const string WamHazardMolesShown = "WamHazardMolesShown";

		// Token: 0x04004B20 RID: 19232
		public const string WamLevelMinScore = "WamLevelMinScore";

		// Token: 0x04004B21 RID: 19233
		public const string WamLevelScore = "WamLevelScore";

		// Token: 0x04004B22 RID: 19234
		public const string WamHazardMolesHit = "WamHazardMolesHit";

		// Token: 0x04004B23 RID: 19235
		public const string WamGameState = "WamGameState";

		// Token: 0x04004B24 RID: 19236
		public const string CustomMapName = "CustomMapName";

		// Token: 0x04004B25 RID: 19237
		public const string LowestFPS = "LowestFPS";

		// Token: 0x04004B26 RID: 19238
		public const string LowestFPSDrawCalls = "LowestFPSDrawCalls";

		// Token: 0x04004B27 RID: 19239
		public const string LowestFPSPlayerCount = "LowestFPSPlayerCount";

		// Token: 0x04004B28 RID: 19240
		public const string AverageFPS = "AverageFPS";

		// Token: 0x04004B29 RID: 19241
		public const string AverageDrawCalls = "AverageDrawCalls";

		// Token: 0x04004B2A RID: 19242
		public const string AveragePlayerCount = "AveragePlayerCount";

		// Token: 0x04004B2B RID: 19243
		public const string HighestFPS = "HighestFPS";

		// Token: 0x04004B2C RID: 19244
		public const string HighestFPSDrawCalls = "HighestFPSDrawCalls";

		// Token: 0x04004B2D RID: 19245
		public const string HighestFPSPlayerCount = "HighestFPSPlayerCount";

		// Token: 0x04004B2E RID: 19246
		public const string CustomMapCreator = "CustomMapCreator";

		// Token: 0x04004B2F RID: 19247
		public const string CustomMapModId = "CustomMapModId";

		// Token: 0x04004B30 RID: 19248
		public const string MinPlayerCount = "MinPlayerCount";

		// Token: 0x04004B31 RID: 19249
		public const string MaxPlayerCount = "MaxPlayerCount";

		// Token: 0x04004B32 RID: 19250
		public const string PlaytimeOnMap = "PlaytimeOnMap";

		// Token: 0x04004B33 RID: 19251
		public const string PlaytimeInSeconds = "PlaytimeInSeconds";

		// Token: 0x04004B34 RID: 19252
		public const string PrivateRoom = "PrivateRoom";

		// Token: 0x04004B35 RID: 19253
		public const string game_mode_played_event = "game_mode_played_event";

		// Token: 0x04004B36 RID: 19254
		public const string game_mode = "game_mode";
	}

	// Token: 0x020008C3 RID: 2243
	private class BatchRunner : MonoBehaviour
	{
		// Token: 0x06003AC4 RID: 15044 RVA: 0x001432D5 File Offset: 0x001414D5
		private IEnumerator Start()
		{
			for (;;)
			{
				float start = Time.realtimeSinceStartup;
				while (Time.realtimeSinceStartup < start + GorillaTelemetry.TELEMETRY_FLUSH_SEC)
				{
					yield return null;
				}
				GorillaTelemetry.FlushMothershipTelemetry();
			}
			yield break;
		}
	}
}
