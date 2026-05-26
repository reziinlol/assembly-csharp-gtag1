using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using UnityEngine;

// Token: 0x020000A6 RID: 166
public class GameModeSpecificObject : MonoBehaviour
{
	// Token: 0x14000009 RID: 9
	// (add) Token: 0x06000414 RID: 1044 RVA: 0x00018300 File Offset: 0x00016500
	// (remove) Token: 0x06000415 RID: 1045 RVA: 0x00018334 File Offset: 0x00016534
	public static event GameModeSpecificObject.GameModeSpecificObjectDelegate OnAwake;

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x06000416 RID: 1046 RVA: 0x00018368 File Offset: 0x00016568
	// (remove) Token: 0x06000417 RID: 1047 RVA: 0x0001839C File Offset: 0x0001659C
	public static event GameModeSpecificObject.GameModeSpecificObjectDelegate OnDestroyed;

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x06000418 RID: 1048 RVA: 0x000183CF File Offset: 0x000165CF
	public GameModeSpecificObject.ValidationMethod Validation
	{
		get
		{
			return this.validationMethod;
		}
	}

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x06000419 RID: 1049 RVA: 0x000183D7 File Offset: 0x000165D7
	public List<GameModeType> GameModes
	{
		get
		{
			return this.gameModes;
		}
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x000183E0 File Offset: 0x000165E0
	private void Awake()
	{
		GameModeSpecificObject.<Awake>d__15 <Awake>d__;
		<Awake>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Awake>d__.<>4__this = this;
		<Awake>d__.<>1__state = -1;
		<Awake>d__.<>t__builder.Start<GameModeSpecificObject.<Awake>d__15>(ref <Awake>d__);
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x00018417 File Offset: 0x00016617
	private void OnDestroy()
	{
		if (GameModeSpecificObject.OnDestroyed != null)
		{
			GameModeSpecificObject.OnDestroyed(this);
		}
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x0001842B File Offset: 0x0001662B
	public bool CheckValid(GameModeType gameMode)
	{
		if (this.validationMethod == GameModeSpecificObject.ValidationMethod.Exclusion)
		{
			return !this.gameModes.Contains(gameMode);
		}
		return this.gameModes.Contains(gameMode);
	}

	// Token: 0x0400047D RID: 1149
	[SerializeField]
	private GameModeSpecificObject.ValidationMethod validationMethod;

	// Token: 0x0400047E RID: 1150
	[SerializeField]
	private GameModeType[] _gameModes;

	// Token: 0x0400047F RID: 1151
	private List<GameModeType> gameModes;

	// Token: 0x020000A7 RID: 167
	// (Invoke) Token: 0x0600041F RID: 1055
	public delegate void GameModeSpecificObjectDelegate(GameModeSpecificObject gameModeSpecificObject);

	// Token: 0x020000A8 RID: 168
	[Serializable]
	public enum ValidationMethod
	{
		// Token: 0x04000481 RID: 1153
		Inclusion,
		// Token: 0x04000482 RID: 1154
		Exclusion
	}
}
