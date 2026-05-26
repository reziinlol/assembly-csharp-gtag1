using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000CA2 RID: 3234
public abstract class PlayerTrigger : MonoBehaviour
{
	// Token: 0x0600502E RID: 20526 RVA: 0x001A9D3F File Offset: 0x001A7F3F
	protected virtual void Awake()
	{
		this.triggerCollisionEvents.CompositeTriggerEnter += this.OnCompositeTriggerEnter;
		this.triggerCollisionEvents.CompositeTriggerExit += this.OnCompositeTriggerExit;
	}

	// Token: 0x0600502F RID: 20527 RVA: 0x001A9D6F File Offset: 0x001A7F6F
	private void OnCompositeTriggerEnter(Collider collider)
	{
		if (!this.isPlayerCollided && collider == GTPlayer.Instance.bodyCollider)
		{
			this.playerCollider = collider;
			this.PlayerEnter();
		}
	}

	// Token: 0x06005030 RID: 20528 RVA: 0x001A9D98 File Offset: 0x001A7F98
	private void OnCompositeTriggerExit(Collider collider)
	{
		if (this.isPlayerCollided && collider == this.playerCollider)
		{
			this.PlayerExit();
		}
	}

	// Token: 0x06005031 RID: 20529 RVA: 0x001A9DB6 File Offset: 0x001A7FB6
	protected virtual void PlayerEnter()
	{
		this.isPlayerCollided = true;
	}

	// Token: 0x06005032 RID: 20530 RVA: 0x001A9DBF File Offset: 0x001A7FBF
	protected virtual void PlayerExit()
	{
		this.playerCollider = null;
		this.isPlayerCollided = false;
	}

	// Token: 0x0400623F RID: 25151
	protected bool isPlayerCollided;

	// Token: 0x04006240 RID: 25152
	protected Collider playerCollider;

	// Token: 0x04006241 RID: 25153
	[SerializeField]
	private CompositeTriggerEvents triggerCollisionEvents;
}
