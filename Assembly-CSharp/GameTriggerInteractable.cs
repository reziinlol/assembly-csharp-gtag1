using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006EC RID: 1772
[RequireComponent(typeof(GameEntity))]
public class GameTriggerInteractable : MonoBehaviour
{
	// Token: 0x06002CAC RID: 11436 RVA: 0x000F1AE8 File Offset: 0x000EFCE8
	private void OnEnable()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
		if (this.interactableWhileGrabbed)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.StartHolding));
			GameEntity gameEntity2 = this.gameEntity;
			gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.StopHolding));
		}
		if (this.interactableWhileSnapped)
		{
			GameEntity gameEntity3 = this.gameEntity;
			gameEntity3.OnSnapped = (Action)Delegate.Combine(gameEntity3.OnSnapped, new Action(this.StartHolding));
			GameEntity gameEntity4 = this.gameEntity;
			gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.StopHolding));
		}
	}

	// Token: 0x06002CAD RID: 11437 RVA: 0x000F1BBB File Offset: 0x000EFDBB
	public void StartHolding()
	{
		GameTriggerInteractable.LocalInteractableTriggers.AddIfNew(this);
	}

	// Token: 0x06002CAE RID: 11438 RVA: 0x000F1BC8 File Offset: 0x000EFDC8
	public void StopHolding()
	{
		GameTriggerInteractable.LocalInteractableTriggers.RemoveIfContains(this);
	}

	// Token: 0x06002CAF RID: 11439 RVA: 0x000F1BD8 File Offset: 0x000EFDD8
	public bool PointWithinInteractableArea(Vector3 point)
	{
		return (this.interactableCenter.position - point).magnitude < this.interactableRadius;
	}

	// Token: 0x06002CB0 RID: 11440 RVA: 0x000F1C06 File Offset: 0x000EFE06
	public void BeginTriggerInteraction(int _handIndex)
	{
		this.triggerInteractionActive = true;
		this.handIndex = _handIndex;
	}

	// Token: 0x06002CB1 RID: 11441 RVA: 0x000F1C16 File Offset: 0x000EFE16
	public void EndTriggerInteraction()
	{
		this.triggerInteractionActive = false;
		this.handIndex = -1;
	}

	// Token: 0x04003917 RID: 14615
	public GameEntity gameEntity;

	// Token: 0x04003918 RID: 14616
	public Transform interactableCenter;

	// Token: 0x04003919 RID: 14617
	public float interactableRadius;

	// Token: 0x0400391A RID: 14618
	public bool interactableWhileGrabbed;

	// Token: 0x0400391B RID: 14619
	public bool interactableWhileSnapped;

	// Token: 0x0400391C RID: 14620
	public bool interactablePermanently;

	// Token: 0x0400391D RID: 14621
	public bool interactableOnOthers;

	// Token: 0x0400391E RID: 14622
	public bool triggerInteractionActive;

	// Token: 0x0400391F RID: 14623
	public int handIndex = -1;

	// Token: 0x04003920 RID: 14624
	public static List<GameTriggerInteractable> LocalInteractableTriggers = new List<GameTriggerInteractable>();
}
