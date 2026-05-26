using System;
using System.Collections.Generic;
using GorillaGameModes;
using UnityEngine;

// Token: 0x020000AA RID: 170
public class GameModeSpecificObjectRegistry : MonoBehaviour
{
	// Token: 0x06000424 RID: 1060 RVA: 0x0001853E File Offset: 0x0001673E
	private void OnEnable()
	{
		GameModeSpecificObject.OnAwake += this.GameModeSpecificObject_OnAwake;
		GameModeSpecificObject.OnDestroyed += this.GameModeSpecificObject_OnDestroyed;
		GameMode.OnStartGameMode += this.GameMode_OnStartGameMode;
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x00018573 File Offset: 0x00016773
	private void OnDisable()
	{
		GameModeSpecificObject.OnAwake -= this.GameModeSpecificObject_OnAwake;
		GameModeSpecificObject.OnDestroyed -= this.GameModeSpecificObject_OnDestroyed;
		GameMode.OnStartGameMode -= this.GameMode_OnStartGameMode;
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x000185A8 File Offset: 0x000167A8
	private void GameModeSpecificObject_OnAwake(GameModeSpecificObject obj)
	{
		foreach (GameModeType key in obj.GameModes)
		{
			if (!this.gameModeSpecificObjects.ContainsKey(key))
			{
				this.gameModeSpecificObjects.Add(key, new List<GameModeSpecificObject>());
			}
			this.gameModeSpecificObjects[key].Add(obj);
		}
		if (GameMode.ActiveGameMode == null)
		{
			obj.gameObject.SetActive(obj.Validation == GameModeSpecificObject.ValidationMethod.Exclusion);
			return;
		}
		obj.gameObject.SetActive(obj.CheckValid(GameMode.ActiveGameMode.GameType()));
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x00018664 File Offset: 0x00016864
	private void GameModeSpecificObject_OnDestroyed(GameModeSpecificObject obj)
	{
		foreach (GameModeType key in obj.GameModes)
		{
			if (this.gameModeSpecificObjects.ContainsKey(key))
			{
				this.gameModeSpecificObjects[key].Remove(obj);
			}
		}
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x000186D4 File Offset: 0x000168D4
	private void GameMode_OnStartGameMode(GameModeType newGameModeType)
	{
		if (this.currentGameType == newGameModeType)
		{
			return;
		}
		if (this.gameModeSpecificObjects.ContainsKey(this.currentGameType))
		{
			foreach (GameModeSpecificObject gameModeSpecificObject in this.gameModeSpecificObjects[this.currentGameType])
			{
				gameModeSpecificObject.gameObject.SetActive(gameModeSpecificObject.CheckValid(newGameModeType));
			}
		}
		if (this.gameModeSpecificObjects.ContainsKey(newGameModeType))
		{
			foreach (GameModeSpecificObject gameModeSpecificObject2 in this.gameModeSpecificObjects[newGameModeType])
			{
				gameModeSpecificObject2.gameObject.SetActive(gameModeSpecificObject2.CheckValid(newGameModeType));
			}
		}
		this.currentGameType = newGameModeType;
	}

	// Token: 0x04000487 RID: 1159
	private Dictionary<GameModeType, List<GameModeSpecificObject>> gameModeSpecificObjects = new Dictionary<GameModeType, List<GameModeSpecificObject>>();

	// Token: 0x04000488 RID: 1160
	private GameModeType currentGameType = GameModeType.Count;
}
