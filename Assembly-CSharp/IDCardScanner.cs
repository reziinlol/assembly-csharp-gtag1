using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001CA RID: 458
public class IDCardScanner : MonoBehaviour
{
	// Token: 0x1400001B RID: 27
	// (add) Token: 0x06000C25 RID: 3109 RVA: 0x00041EF0 File Offset: 0x000400F0
	// (remove) Token: 0x06000C26 RID: 3110 RVA: 0x00041F28 File Offset: 0x00040128
	public event IDCardScanner.CardSwipeEvent OnPlayerCardSwipe;

	// Token: 0x06000C27 RID: 3111 RVA: 0x00041F60 File Offset: 0x00040160
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<ScannableIDCard>() != null)
		{
			UnityEvent unityEvent = this.onCardSwiped;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			GameEntity component = other.GetComponent<GameEntity>();
			if (component == null && other.attachedRigidbody != null)
			{
				component = other.attachedRigidbody.GetComponent<GameEntity>();
			}
			if (component != null && component.heldByActorNumber != -1)
			{
				bool flag = !this.requireSpecificPlayer || (this.restrictToPlayer != null && this.restrictToPlayer.ActorNumber == component.heldByActorNumber && component.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
				bool flag2 = !this.requireAuthority || component.manager.IsAuthority();
				if (flag && flag2)
				{
					UnityEvent<int> unityEvent2 = this.onCardSwipedByPlayer;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke(component.heldByActorNumber);
					}
					IDCardScanner.CardSwipeEvent onPlayerCardSwipe = this.OnPlayerCardSwipe;
					if (onPlayerCardSwipe == null)
					{
						return;
					}
					onPlayerCardSwipe(component.heldByActorNumber);
				}
			}
		}
	}

	// Token: 0x04000EB5 RID: 3765
	public UnityEvent onCardSwiped;

	// Token: 0x04000EB6 RID: 3766
	public UnityEvent<int> onCardSwipedByPlayer;

	// Token: 0x04000EB7 RID: 3767
	[Tooltip("Has to be risen externally, by the receiver of the card swipe")]
	public UnityEvent onSucceeded;

	// Token: 0x04000EB8 RID: 3768
	[Tooltip("Has to be risen externally, by the receiver of the card swipe")]
	public UnityEvent onFailed;

	// Token: 0x04000EB9 RID: 3769
	public bool requireSpecificPlayer;

	// Token: 0x04000EBA RID: 3770
	public bool requireAuthority;

	// Token: 0x04000EBB RID: 3771
	[NonSerialized]
	public NetPlayer restrictToPlayer;

	// Token: 0x020001CB RID: 459
	// (Invoke) Token: 0x06000C2A RID: 3114
	public delegate void CardSwipeEvent(int actorNumber);
}
