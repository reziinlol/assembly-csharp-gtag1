using System;
using UnityEngine;

// Token: 0x020005C0 RID: 1472
public class GameModePageButton : GorillaPressableButton
{
	// Token: 0x06002501 RID: 9473 RVA: 0x000C56B0 File Offset: 0x000C38B0
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.selector.ChangePage(this.left);
	}

	// Token: 0x04003058 RID: 12376
	[SerializeField]
	private GameModePages selector;

	// Token: 0x04003059 RID: 12377
	[SerializeField]
	private bool left;
}
