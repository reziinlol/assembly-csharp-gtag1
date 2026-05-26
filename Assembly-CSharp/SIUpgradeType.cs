using System;

// Token: 0x0200011F RID: 287
public enum SIUpgradeType
{
	// Token: 0x04000915 RID: 2325
	InvalidNode = -2,
	// Token: 0x04000916 RID: 2326
	Initialize,
	// Token: 0x04000917 RID: 2327
	Thruster_Unlock,
	// Token: 0x04000918 RID: 2328
	Thruster_Jet,
	// Token: 0x04000919 RID: 2329
	Thruster_Prop,
	// Token: 0x0400091A RID: 2330
	Thruster_Jet_Duration,
	// Token: 0x0400091B RID: 2331
	Thruster_Jet_Accel,
	// Token: 0x0400091C RID: 2332
	Thruster_Prop_Duration,
	// Token: 0x0400091D RID: 2333
	Thruster_Prop_Speed,
	// Token: 0x0400091E RID: 2334
	Thruster_Jet_Tag,
	// Token: 0x0400091F RID: 2335
	Thruster_Prop_Knockback,
	// Token: 0x04000920 RID: 2336
	Thruster_Fuel_Grounding,
	// Token: 0x04000921 RID: 2337
	Thruster_Throttle_Control,
	// Token: 0x04000922 RID: 2338
	Stilt_Unlock = 100,
	// Token: 0x04000923 RID: 2339
	Stilt_Tag_Tip,
	// Token: 0x04000924 RID: 2340
	Stilt_Retractable,
	// Token: 0x04000925 RID: 2341
	Stilt_Adjustable_Length,
	// Token: 0x04000926 RID: 2342
	Stilt_Retract_Speed,
	// Token: 0x04000927 RID: 2343
	Stilt_Max_Length,
	// Token: 0x04000928 RID: 2344
	Stilt_Stun_Tip,
	// Token: 0x04000929 RID: 2345
	Stilt_Muscle_Fusion,
	// Token: 0x0400092A RID: 2346
	Stilt_Short,
	// Token: 0x0400092B RID: 2347
	Stilt_Long,
	// Token: 0x0400092C RID: 2348
	Stilt_Motorized,
	// Token: 0x0400092D RID: 2349
	Stilt_Motorized_Triple,
	// Token: 0x0400092E RID: 2350
	Stilt_Turkey_Coma,
	// Token: 0x0400092F RID: 2351
	Grenade_Concussion_Unlock = 200,
	// Token: 0x04000930 RID: 2352
	Grenade_Antigravity_Unlock,
	// Token: 0x04000931 RID: 2353
	Grenade_Concussion_Stun,
	// Token: 0x04000932 RID: 2354
	Grenade_Concussion_Radius,
	// Token: 0x04000933 RID: 2355
	Grenade_Antigravity_Persists,
	// Token: 0x04000934 RID: 2356
	Grenade_Antigravity_Cooldown,
	// Token: 0x04000935 RID: 2357
	Grenade_Concussion_Self_Boost,
	// Token: 0x04000936 RID: 2358
	Grenade_Concussion_Overcharge,
	// Token: 0x04000937 RID: 2359
	Grenade_Antigravity_Pro_Gravity,
	// Token: 0x04000938 RID: 2360
	Grenade_Concussion_Impact_Accelerant,
	// Token: 0x04000939 RID: 2361
	Grenade_Antigravity_Gravity_Bomb,
	// Token: 0x0400093A RID: 2362
	Grenade_Antigravity_Black_Hole,
	// Token: 0x0400093B RID: 2363
	Grenade_Holster_Unlock,
	// Token: 0x0400093C RID: 2364
	Grenade_Stun_Unlock,
	// Token: 0x0400093D RID: 2365
	Grenade_Puller_Unlock,
	// Token: 0x0400093E RID: 2366
	Grenade_Disrupter_Unlock,
	// Token: 0x0400093F RID: 2367
	Dash_Yoyo_Unlock = 301,
	// Token: 0x04000940 RID: 2368
	Dash_Yoyo_Range = 304,
	// Token: 0x04000941 RID: 2369
	Dash_Yoyo_Speed,
	// Token: 0x04000942 RID: 2370
	Dash_Unused_306,
	// Token: 0x04000943 RID: 2371
	Dash_Unused_307,
	// Token: 0x04000944 RID: 2372
	Dash_Yoyo_Cooldown,
	// Token: 0x04000945 RID: 2373
	Dash_Yoyo_Dynamic,
	// Token: 0x04000946 RID: 2374
	Dash_Unused_310,
	// Token: 0x04000947 RID: 2375
	Dash_Yoyo_Stun,
	// Token: 0x04000948 RID: 2376
	Dash_Yoyo_Tag,
	// Token: 0x04000949 RID: 2377
	Dash_Unused_313,
	// Token: 0x0400094A RID: 2378
	Dash_Unused_314,
	// Token: 0x0400094B RID: 2379
	Platform_Unlock = 400,
	// Token: 0x0400094C RID: 2380
	Platform_Cooldown,
	// Token: 0x0400094D RID: 2381
	Platform_Duration,
	// Token: 0x0400094E RID: 2382
	Platform_Capacity,
	// Token: 0x0400094F RID: 2383
	Platform_SpeedBoost,
	// Token: 0x04000950 RID: 2384
	Tapteleport_Unlock = 500,
	// Token: 0x04000951 RID: 2385
	Tapteleport_Zone,
	// Token: 0x04000952 RID: 2386
	Tapteleport_Stealth,
	// Token: 0x04000953 RID: 2387
	Tapteleport_Portal_Selection,
	// Token: 0x04000954 RID: 2388
	Tapteleport_Keep_Velocity,
	// Token: 0x04000955 RID: 2389
	Tapteleport_Infinite_Use,
	// Token: 0x04000956 RID: 2390
	Tentacle_Unlock = 600,
	// Token: 0x04000957 RID: 2391
	Tentacle_Power_Claw,
	// Token: 0x04000958 RID: 2392
	Tentacle_Charge_Rate,
	// Token: 0x04000959 RID: 2393
	Tentacle_Efficiency,
	// Token: 0x0400095A RID: 2394
	Tentacle_Crawler,
	// Token: 0x0400095B RID: 2395
	Tentacle_Strider,
	// Token: 0x0400095C RID: 2396
	AirControl_AirJuke_Unlock = 700,
	// Token: 0x0400095D RID: 2397
	AirControl_AirJuke_Speed,
	// Token: 0x0400095E RID: 2398
	AirControl_AirGrab_Unlock,
	// Token: 0x0400095F RID: 2399
	AirControl_AirGrab_Speed,
	// Token: 0x04000960 RID: 2400
	AirControl_AirGrab_HoldTime,
	// Token: 0x04000961 RID: 2401
	AirControl_Zipline_Unlock,
	// Token: 0x04000962 RID: 2402
	AirControl_Zipline_Speed,
	// Token: 0x04000963 RID: 2403
	Prototype_SlipMitt = 800,
	// Token: 0x04000964 RID: 2404
	Prototype_Wing,
	// Token: 0x04000965 RID: 2405
	Prototype_802,
	// Token: 0x04000966 RID: 2406
	Prototype_803,
	// Token: 0x04000967 RID: 2407
	Prototype_804,
	// Token: 0x04000968 RID: 2408
	Prototype_805,
	// Token: 0x04000969 RID: 2409
	Blaster_Standard_Unlock = 1000,
	// Token: 0x0400096A RID: 2410
	Blaster_Charge_Unlock,
	// Token: 0x0400096B RID: 2411
	Blaster_Lobber_Unlock,
	// Token: 0x0400096C RID: 2412
	Blaster_PumpDart_Unlock,
	// Token: 0x0400096D RID: 2413
	Blaster_MegaCharge_Unlock,
	// Token: 0x0400096E RID: 2414
	Blaster_LongBlaster_Unlock
}
