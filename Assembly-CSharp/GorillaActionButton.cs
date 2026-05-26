using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200033B RID: 827
public class GorillaActionButton : GorillaPressableButton
{
	// Token: 0x06001450 RID: 5200 RVA: 0x0006CEFB File Offset: 0x0006B0FB
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.onPress.Invoke();
	}

	// Token: 0x04001923 RID: 6435
	[SerializeField]
	public UnityEvent onPress;
}
