using System;
using UnityEngine;

// Token: 0x020007A2 RID: 1954
public class GRGameEntityInteractionPoint : MonoBehaviour
{
	// Token: 0x060031ED RID: 12781 RVA: 0x001123CF File Offset: 0x001105CF
	public void Start()
	{
		base.transform.parent = this.targetParent;
	}

	// Token: 0x060031EE RID: 12782 RVA: 0x001123E4 File Offset: 0x001105E4
	public void OnEnable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
	}

	// Token: 0x060031EF RID: 12783 RVA: 0x00112440 File Offset: 0x00110640
	public void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.OnReleased));
	}

	// Token: 0x060031F0 RID: 12784 RVA: 0x0011249B File Offset: 0x0011069B
	public void OnGrabbed()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.TickWhileHeld));
		Action onGrabStart = this.OnGrabStart;
		if (onGrabStart == null)
		{
			return;
		}
		onGrabStart();
	}

	// Token: 0x060031F1 RID: 12785 RVA: 0x001124D4 File Offset: 0x001106D4
	public void OnReleased()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Remove(gameEntity.OnTick, new Action(this.TickWhileHeld));
		this.gameEntity.transform.parent = this.targetParent;
		this.gameEntity.transform.localRotation = Quaternion.identity;
		this.gameEntity.transform.localPosition = Vector3.zero;
		this.OnGrabEnd();
	}

	// Token: 0x060031F2 RID: 12786 RVA: 0x00112554 File Offset: 0x00110754
	public void TickWhileHeld()
	{
		if (this.targetParent != null)
		{
			Vector3 position = this.targetParent.transform.position;
			Vector3 position2 = base.transform.position;
			if (Vector3.Magnitude(position - position2) > this.autoReleaseDistance)
			{
				GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
				if (gamePlayer != null)
				{
					gamePlayer.ClearGrabbedIfHeld(this.gameEntity.id, this.gameEntity.manager);
				}
				if (gamePlayer != null && GamePlayerLocal.instance.gamePlayer == gamePlayer)
				{
					GamePlayerLocal.instance.ClearGrabbedIfHeld(this.gameEntity.id, this.gameEntity.manager);
				}
				this.OnReleased();
				return;
			}
		}
		Action onGrabContinue = this.OnGrabContinue;
		if (onGrabContinue == null)
		{
			return;
		}
		onGrabContinue();
	}

	// Token: 0x040040D5 RID: 16597
	public GameEntity gameEntity;

	// Token: 0x040040D6 RID: 16598
	public float autoReleaseDistance = 0.1f;

	// Token: 0x040040D7 RID: 16599
	public Action OnGrabStart;

	// Token: 0x040040D8 RID: 16600
	public Action OnGrabContinue;

	// Token: 0x040040D9 RID: 16601
	public Action OnGrabEnd;

	// Token: 0x040040DA RID: 16602
	public Transform targetParent;
}
