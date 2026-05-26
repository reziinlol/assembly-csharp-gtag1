using System;
using UnityEngine;

// Token: 0x02000243 RID: 579
public class MonkeVoteProximityTrigger : GorillaTriggerBox
{
	// Token: 0x14000021 RID: 33
	// (add) Token: 0x06000F81 RID: 3969 RVA: 0x000545A4 File Offset: 0x000527A4
	// (remove) Token: 0x06000F82 RID: 3970 RVA: 0x000545DC File Offset: 0x000527DC
	public event Action OnEnter;

	// Token: 0x17000186 RID: 390
	// (get) Token: 0x06000F83 RID: 3971 RVA: 0x00054611 File Offset: 0x00052811
	// (set) Token: 0x06000F84 RID: 3972 RVA: 0x00054619 File Offset: 0x00052819
	public bool isPlayerNearby { get; private set; }

	// Token: 0x06000F85 RID: 3973 RVA: 0x00054622 File Offset: 0x00052822
	public override void OnBoxTriggered()
	{
		this.isPlayerNearby = true;
		if (this.triggerTime + this.retriggerDelay < Time.unscaledTime)
		{
			this.triggerTime = Time.unscaledTime;
			Action onEnter = this.OnEnter;
			if (onEnter == null)
			{
				return;
			}
			onEnter();
		}
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x0005465A File Offset: 0x0005285A
	public override void OnBoxExited()
	{
		this.isPlayerNearby = false;
	}

	// Token: 0x040012B3 RID: 4787
	private float triggerTime = float.MinValue;

	// Token: 0x040012B4 RID: 4788
	private float retriggerDelay = 0.25f;
}
