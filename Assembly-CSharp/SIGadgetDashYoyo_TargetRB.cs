using System;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class SIGadgetDashYoyo_TargetRB : MonoBehaviour
{
	// Token: 0x06000584 RID: 1412 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected void OnEnable()
	{
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x0001F9B0 File Offset: 0x0001DBB0
	protected void OnTriggerEnter(Collider otherCollider)
	{
		if (base.isActiveAndEnabled && this.gadget.gameEntity.IsAuthority() && (this.gadget.gameEntity.heldByActorNumber != -1 || this.gadget.gameEntity.snappedByActorNumber != -1) && (otherCollider.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider) || otherCollider.gameObject.IsOnLayer(UnityLayer.GorillaSlingshotCollider)) && !ApplicationQuittingState.IsQuitting)
		{
			SuperInfectionGame superInfectionGame = GorillaGameManager.instance as SuperInfectionGame;
			if (superInfectionGame != null)
			{
				VRRig componentInParent = otherCollider.GetComponentInParent<VRRig>();
				if (componentInParent == null)
				{
					return;
				}
				NetPlayer creator = componentInParent.creator;
				if (creator == null)
				{
					return;
				}
				if (SuperInfectionManager.GetSIManagerForZone(this.gadget.gameEntity.manager.zone) == null)
				{
					return;
				}
				this.gadget.OnHitPlayer_Authority(superInfectionGame, creator);
				return;
			}
		}
	}

	// Token: 0x0400069C RID: 1692
	[SerializeField]
	private SIGadgetDashYoyo gadget;
}
