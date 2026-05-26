using System;
using UnityEngine;

// Token: 0x020005C2 RID: 1474
public class GameModeSelectButton : GorillaPressableButton
{
	// Token: 0x06002510 RID: 9488 RVA: 0x000C5987 File Offset: 0x000C3B87
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.selector.SelectEntryOnPage(this.buttonIndex);
	}

	// Token: 0x04003061 RID: 12385
	[SerializeField]
	internal GameModePages selector;

	// Token: 0x04003062 RID: 12386
	[SerializeField]
	internal int buttonIndex;
}
