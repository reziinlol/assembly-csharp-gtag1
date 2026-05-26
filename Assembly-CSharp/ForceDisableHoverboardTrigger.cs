using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000A57 RID: 2647
public class ForceDisableHoverboardTrigger : MonoBehaviour
{
	// Token: 0x060043F3 RID: 17395 RVA: 0x0016BF4D File Offset: 0x0016A14D
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			this.wasEnabled = GTPlayer.Instance.isHoverAllowed;
			GTPlayer.Instance.SetHoverAllowed(false, true);
		}
	}

	// Token: 0x060043F4 RID: 17396 RVA: 0x0016BF80 File Offset: 0x0016A180
	public void OnTriggerExit(Collider other)
	{
		if (!this.reEnableOnExit || !this.wasEnabled)
		{
			return;
		}
		if (this.reEnableOnlyInVStump && !GorillaComputer.instance.IsPlayerInVirtualStump())
		{
			return;
		}
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(true, false);
		}
	}

	// Token: 0x040055CE RID: 21966
	[Tooltip("If TRUE and the Hoverboard was enabled when the player entered this trigger, it will be re-enabled when they exit.")]
	public bool reEnableOnExit = true;

	// Token: 0x040055CF RID: 21967
	public bool reEnableOnlyInVStump = true;

	// Token: 0x040055D0 RID: 21968
	private bool wasEnabled;
}
