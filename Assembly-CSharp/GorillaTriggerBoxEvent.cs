using System;
using UnityEngine.Events;

// Token: 0x020005D3 RID: 1491
public class GorillaTriggerBoxEvent : GorillaTriggerBox
{
	// Token: 0x0600253F RID: 9535 RVA: 0x000C5EDD File Offset: 0x000C40DD
	public override void OnBoxTriggered()
	{
		UnityEvent unityEvent = this.onBoxTriggered;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06002540 RID: 9536 RVA: 0x000C5EEF File Offset: 0x000C40EF
	public override void OnBoxExited()
	{
		UnityEvent unityEvent = this.onBoxExited;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x040030C1 RID: 12481
	public UnityEvent onBoxTriggered;

	// Token: 0x040030C2 RID: 12482
	public UnityEvent onBoxExited;
}
