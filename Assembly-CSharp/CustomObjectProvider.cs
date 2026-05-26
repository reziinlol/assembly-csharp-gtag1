using System;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000429 RID: 1065
public class CustomObjectProvider : NetworkObjectProviderDefault
{
	// Token: 0x1700027A RID: 634
	// (get) Token: 0x06001946 RID: 6470 RVA: 0x0008DDE0 File Offset: 0x0008BFE0
	private static NetworkObjectBaker Baker
	{
		get
		{
			NetworkObjectBaker result;
			if ((result = CustomObjectProvider.baker) == null)
			{
				result = (CustomObjectProvider.baker = new NetworkObjectBaker());
			}
			return result;
		}
	}

	// Token: 0x06001947 RID: 6471 RVA: 0x0008DDF6 File Offset: 0x0008BFF6
	public override NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject instance)
	{
		NetworkObjectAcquireResult networkObjectAcquireResult = base.AcquirePrefabInstance(runner, context, out instance);
		if (networkObjectAcquireResult == NetworkObjectAcquireResult.Success)
		{
			this.IsGameMode(instance);
			return networkObjectAcquireResult;
		}
		instance = null;
		return networkObjectAcquireResult;
	}

	// Token: 0x06001948 RID: 6472 RVA: 0x0008DE10 File Offset: 0x0008C010
	private void IsGameMode(NetworkObject instance)
	{
		if (instance.gameObject.GetComponent<GameModeSerializer>() != null)
		{
			GorillaGameModes.GameMode.GetGameModeInstance(GorillaGameModes.GameMode.GetGameModeKeyFromRoomProp()).AddFusionDataBehaviour(instance);
			CustomObjectProvider.Baker.Bake(instance.gameObject);
		}
	}

	// Token: 0x06001949 RID: 6473 RVA: 0x0008DE46 File Offset: 0x0008C046
	protected override void DestroySceneObject(NetworkRunner runner, NetworkSceneObjectId sceneObjectId, NetworkObject instance)
	{
		if (this.SceneObjects != null && this.SceneObjects.Contains(instance.gameObject))
		{
			return;
		}
		base.DestroySceneObject(runner, sceneObjectId, instance);
	}

	// Token: 0x0600194A RID: 6474 RVA: 0x0008DE6D File Offset: 0x0008C06D
	protected override void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
	{
		base.DestroyPrefabInstance(runner, prefabId, instance);
	}

	// Token: 0x04002449 RID: 9289
	public const int GameModeFlag = 1;

	// Token: 0x0400244A RID: 9290
	public const int PlayerFlag = 2;

	// Token: 0x0400244B RID: 9291
	private static NetworkObjectBaker baker;

	// Token: 0x0400244C RID: 9292
	internal List<GameObject> SceneObjects;
}
