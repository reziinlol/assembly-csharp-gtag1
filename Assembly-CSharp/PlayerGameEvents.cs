using System;

// Token: 0x0200024E RID: 590
public class PlayerGameEvents
{
	// Token: 0x14000022 RID: 34
	// (add) Token: 0x06000FC4 RID: 4036 RVA: 0x000555DC File Offset: 0x000537DC
	// (remove) Token: 0x06000FC5 RID: 4037 RVA: 0x00055610 File Offset: 0x00053810
	public static event Action<string> OnGameModeObjectiveTrigger;

	// Token: 0x14000023 RID: 35
	// (add) Token: 0x06000FC6 RID: 4038 RVA: 0x00055644 File Offset: 0x00053844
	// (remove) Token: 0x06000FC7 RID: 4039 RVA: 0x00055678 File Offset: 0x00053878
	public static event Action<string> OnGameModeCompleteRound;

	// Token: 0x14000024 RID: 36
	// (add) Token: 0x06000FC8 RID: 4040 RVA: 0x000556AC File Offset: 0x000538AC
	// (remove) Token: 0x06000FC9 RID: 4041 RVA: 0x000556E0 File Offset: 0x000538E0
	public static event Action<string> OnGrabbedObject;

	// Token: 0x14000025 RID: 37
	// (add) Token: 0x06000FCA RID: 4042 RVA: 0x00055714 File Offset: 0x00053914
	// (remove) Token: 0x06000FCB RID: 4043 RVA: 0x00055748 File Offset: 0x00053948
	public static event Action<string> OnDroppedObject;

	// Token: 0x14000026 RID: 38
	// (add) Token: 0x06000FCC RID: 4044 RVA: 0x0005577C File Offset: 0x0005397C
	// (remove) Token: 0x06000FCD RID: 4045 RVA: 0x000557B0 File Offset: 0x000539B0
	public static event Action<string> OnEatObject;

	// Token: 0x14000027 RID: 39
	// (add) Token: 0x06000FCE RID: 4046 RVA: 0x000557E4 File Offset: 0x000539E4
	// (remove) Token: 0x06000FCF RID: 4047 RVA: 0x00055818 File Offset: 0x00053A18
	public static event Action<string> OnTapObject;

	// Token: 0x14000028 RID: 40
	// (add) Token: 0x06000FD0 RID: 4048 RVA: 0x0005584C File Offset: 0x00053A4C
	// (remove) Token: 0x06000FD1 RID: 4049 RVA: 0x00055880 File Offset: 0x00053A80
	public static event Action<string> OnLaunchedProjectile;

	// Token: 0x14000029 RID: 41
	// (add) Token: 0x06000FD2 RID: 4050 RVA: 0x000558B4 File Offset: 0x00053AB4
	// (remove) Token: 0x06000FD3 RID: 4051 RVA: 0x000558E8 File Offset: 0x00053AE8
	public static event Action<float, float> OnPlayerMoved;

	// Token: 0x1400002A RID: 42
	// (add) Token: 0x06000FD4 RID: 4052 RVA: 0x0005591C File Offset: 0x00053B1C
	// (remove) Token: 0x06000FD5 RID: 4053 RVA: 0x00055950 File Offset: 0x00053B50
	public static event Action<float, float> OnPlayerSwam;

	// Token: 0x1400002B RID: 43
	// (add) Token: 0x06000FD6 RID: 4054 RVA: 0x00055984 File Offset: 0x00053B84
	// (remove) Token: 0x06000FD7 RID: 4055 RVA: 0x000559B8 File Offset: 0x00053BB8
	public static event Action<string> OnTriggerHandEffect;

	// Token: 0x1400002C RID: 44
	// (add) Token: 0x06000FD8 RID: 4056 RVA: 0x000559EC File Offset: 0x00053BEC
	// (remove) Token: 0x06000FD9 RID: 4057 RVA: 0x00055A20 File Offset: 0x00053C20
	public static event Action<string> OnEnterLocation;

	// Token: 0x1400002D RID: 45
	// (add) Token: 0x06000FDA RID: 4058 RVA: 0x00055A54 File Offset: 0x00053C54
	// (remove) Token: 0x06000FDB RID: 4059 RVA: 0x00055A88 File Offset: 0x00053C88
	public static event Action<string, int> OnMiscEvent;

	// Token: 0x1400002E RID: 46
	// (add) Token: 0x06000FDC RID: 4060 RVA: 0x00055ABC File Offset: 0x00053CBC
	// (remove) Token: 0x06000FDD RID: 4061 RVA: 0x00055AF0 File Offset: 0x00053CF0
	public static event Action<string> OnCritterEvent;

	// Token: 0x06000FDE RID: 4062 RVA: 0x00055B24 File Offset: 0x00053D24
	public static void GameModeObjectiveTriggered()
	{
		string obj = GorillaGameManager.instance.GameModeName();
		Action<string> onGameModeObjectiveTrigger = PlayerGameEvents.OnGameModeObjectiveTrigger;
		if (onGameModeObjectiveTrigger == null)
		{
			return;
		}
		onGameModeObjectiveTrigger(obj);
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x00055B4C File Offset: 0x00053D4C
	public static void GameModeCompleteRound()
	{
		string obj = GorillaGameManager.instance.GameModeName();
		Action<string> onGameModeCompleteRound = PlayerGameEvents.OnGameModeCompleteRound;
		if (onGameModeCompleteRound == null)
		{
			return;
		}
		onGameModeCompleteRound(obj);
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x00055B74 File Offset: 0x00053D74
	public static void GrabbedObject(string objectName)
	{
		Action<string> onGrabbedObject = PlayerGameEvents.OnGrabbedObject;
		if (onGrabbedObject == null)
		{
			return;
		}
		onGrabbedObject(objectName);
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x00055B86 File Offset: 0x00053D86
	public static void DroppedObject(string objectName)
	{
		Action<string> onDroppedObject = PlayerGameEvents.OnDroppedObject;
		if (onDroppedObject == null)
		{
			return;
		}
		onDroppedObject(objectName);
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x00055B98 File Offset: 0x00053D98
	public static void EatObject(string objectName)
	{
		Action<string> onEatObject = PlayerGameEvents.OnEatObject;
		if (onEatObject == null)
		{
			return;
		}
		onEatObject(objectName);
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x00055BAA File Offset: 0x00053DAA
	public static void TapObject(string objectName)
	{
		Action<string> onTapObject = PlayerGameEvents.OnTapObject;
		if (onTapObject == null)
		{
			return;
		}
		onTapObject(objectName);
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x00055BBC File Offset: 0x00053DBC
	public static void LaunchedProjectile(string objectName)
	{
		Action<string> onLaunchedProjectile = PlayerGameEvents.OnLaunchedProjectile;
		if (onLaunchedProjectile == null)
		{
			return;
		}
		onLaunchedProjectile(objectName);
	}

	// Token: 0x06000FE5 RID: 4069 RVA: 0x00055BCE File Offset: 0x00053DCE
	public static void PlayerMoved(float distance, float speed)
	{
		Action<float, float> onPlayerMoved = PlayerGameEvents.OnPlayerMoved;
		if (onPlayerMoved == null)
		{
			return;
		}
		onPlayerMoved(distance, speed);
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x00055BE1 File Offset: 0x00053DE1
	public static void PlayerSwam(float distance, float speed)
	{
		Action<float, float> onPlayerSwam = PlayerGameEvents.OnPlayerSwam;
		if (onPlayerSwam == null)
		{
			return;
		}
		onPlayerSwam(distance, speed);
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x00055BF4 File Offset: 0x00053DF4
	public static void TriggerHandEffect(string effectName)
	{
		Action<string> onTriggerHandEffect = PlayerGameEvents.OnTriggerHandEffect;
		if (onTriggerHandEffect == null)
		{
			return;
		}
		onTriggerHandEffect(effectName);
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x00055C06 File Offset: 0x00053E06
	public static void TriggerEnterLocation(string locationName)
	{
		Action<string> onEnterLocation = PlayerGameEvents.OnEnterLocation;
		if (onEnterLocation == null)
		{
			return;
		}
		onEnterLocation(locationName);
	}

	// Token: 0x06000FE9 RID: 4073 RVA: 0x00055C18 File Offset: 0x00053E18
	public static void MiscEvent(string eventName, int count = 1)
	{
		Action<string, int> onMiscEvent = PlayerGameEvents.OnMiscEvent;
		if (onMiscEvent == null)
		{
			return;
		}
		onMiscEvent(eventName, count);
	}

	// Token: 0x06000FEA RID: 4074 RVA: 0x00055C2B File Offset: 0x00053E2B
	public static void CritterEvent(string eventName)
	{
		Action<string> onCritterEvent = PlayerGameEvents.OnCritterEvent;
		if (onCritterEvent == null)
		{
			return;
		}
		onCritterEvent(eventName);
	}

	// Token: 0x0200024F RID: 591
	public enum EventType
	{
		// Token: 0x04001309 RID: 4873
		NONE,
		// Token: 0x0400130A RID: 4874
		GameModeObjective,
		// Token: 0x0400130B RID: 4875
		GameModeCompleteRound,
		// Token: 0x0400130C RID: 4876
		GrabbedObject,
		// Token: 0x0400130D RID: 4877
		DroppedObject,
		// Token: 0x0400130E RID: 4878
		EatObject,
		// Token: 0x0400130F RID: 4879
		TapObject,
		// Token: 0x04001310 RID: 4880
		LaunchedProjectile,
		// Token: 0x04001311 RID: 4881
		PlayerMoved,
		// Token: 0x04001312 RID: 4882
		PlayerSwam,
		// Token: 0x04001313 RID: 4883
		TriggerHandEfffect,
		// Token: 0x04001314 RID: 4884
		EnterLocation,
		// Token: 0x04001315 RID: 4885
		MiscEvent
	}
}
