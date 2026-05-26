using System;
using UnityEngine;

// Token: 0x02000752 RID: 1874
public class GRCollectible : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06002F6D RID: 12141 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Awake()
	{
	}

	// Token: 0x06002F6E RID: 12142 RVA: 0x00102668 File Offset: 0x00100868
	public void OnEntityInit()
	{
		GameEntityManager manager = this.entity.manager;
		GameEntity gameEntity = manager.GetGameEntity(manager.GetEntityIdFromNetId((int)this.entity.createData));
		if (gameEntity != null)
		{
			GRCollectibleDispenser component = gameEntity.GetComponent<GRCollectibleDispenser>();
			if (component != null)
			{
				component.GetSpawnedCollectible(this);
			}
		}
	}

	// Token: 0x06002F6F RID: 12143 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002F70 RID: 12144 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002F71 RID: 12145 RVA: 0x001026B8 File Offset: 0x001008B8
	public void InvokeOnCollected()
	{
		Action onCollected = this.OnCollected;
		if (onCollected == null)
		{
			return;
		}
		onCollected();
	}

	// Token: 0x04003CE3 RID: 15587
	public GameEntity entity;

	// Token: 0x04003CE4 RID: 15588
	public int energyValue = 100;

	// Token: 0x04003CE5 RID: 15589
	public ProgressionManager.CoreType type;

	// Token: 0x04003CE6 RID: 15590
	public Action OnCollected;
}
