using System;
using UnityEngine;

// Token: 0x02000D39 RID: 3385
public class CollisionEventNotifier : MonoBehaviour
{
	// Token: 0x14000094 RID: 148
	// (add) Token: 0x0600535D RID: 21341 RVA: 0x001B4484 File Offset: 0x001B2684
	// (remove) Token: 0x0600535E RID: 21342 RVA: 0x001B44BC File Offset: 0x001B26BC
	public event CollisionEventNotifier.CollisionEvent CollisionEnterEvent;

	// Token: 0x14000095 RID: 149
	// (add) Token: 0x0600535F RID: 21343 RVA: 0x001B44F4 File Offset: 0x001B26F4
	// (remove) Token: 0x06005360 RID: 21344 RVA: 0x001B452C File Offset: 0x001B272C
	public event CollisionEventNotifier.CollisionEvent CollisionExitEvent;

	// Token: 0x06005361 RID: 21345 RVA: 0x001B4561 File Offset: 0x001B2761
	private void OnCollisionEnter(Collision collision)
	{
		CollisionEventNotifier.CollisionEvent collisionEnterEvent = this.CollisionEnterEvent;
		if (collisionEnterEvent == null)
		{
			return;
		}
		collisionEnterEvent(this, collision);
	}

	// Token: 0x06005362 RID: 21346 RVA: 0x001B4575 File Offset: 0x001B2775
	private void OnCollisionExit(Collision collision)
	{
		CollisionEventNotifier.CollisionEvent collisionExitEvent = this.CollisionExitEvent;
		if (collisionExitEvent == null)
		{
			return;
		}
		collisionExitEvent(this, collision);
	}

	// Token: 0x02000D3A RID: 3386
	// (Invoke) Token: 0x06005365 RID: 21349
	public delegate void CollisionEvent(CollisionEventNotifier notifier, Collision collision);
}
