using System;

// Token: 0x0200071B RID: 1819
[Serializable]
public class AbilityHaptic
{
	// Token: 0x06002E24 RID: 11812 RVA: 0x000FCD74 File Offset: 0x000FAF74
	public void PlayIfHeldLocal(GameEntity gameEntity)
	{
		if (gameEntity == null || !gameEntity.IsHeldByLocalPlayer())
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), this.strength, this.duration);
	}

	// Token: 0x06002E25 RID: 11813 RVA: 0x000FCDD8 File Offset: 0x000FAFD8
	public void PlayIfSnappedLocal(GameEntity gameEntity)
	{
		if (gameEntity == null || !gameEntity.IsSnappedByLocalPlayer())
		{
			return;
		}
		GameSnappable component = gameEntity.GetComponent<GameSnappable>();
		if (component == null)
		{
			return;
		}
		if (component.IsSnappedToLeftArm())
		{
			GorillaTagger.Instance.StartVibration(true, this.strength, this.duration);
		}
		if (component.IsSnappedToRightArm())
		{
			GorillaTagger.Instance.StartVibration(false, this.strength, this.duration);
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), this.strength, this.duration);
	}

	// Token: 0x04003B40 RID: 15168
	public float strength = 0.2f;

	// Token: 0x04003B41 RID: 15169
	public float duration = 0.1f;
}
