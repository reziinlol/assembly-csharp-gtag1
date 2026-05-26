using System;
using UnityEngine;

// Token: 0x02000426 RID: 1062
public class NativeSizeChangerButton : GorillaPressableButton
{
	// Token: 0x06001941 RID: 6465 RVA: 0x0008DD1D File Offset: 0x0008BF1D
	public override void ButtonActivation()
	{
		this.nativeSizeChanger.Activate(this.settings);
	}

	// Token: 0x0400243F RID: 9279
	[SerializeField]
	private NativeSizeChanger nativeSizeChanger;

	// Token: 0x04002440 RID: 9280
	[SerializeField]
	private NativeSizeChangerSettings settings;
}
