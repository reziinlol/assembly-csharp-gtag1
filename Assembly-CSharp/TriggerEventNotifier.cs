using System;
using UnityEngine;

// Token: 0x02000D94 RID: 3476
public class TriggerEventNotifier : MonoBehaviour
{
	// Token: 0x14000098 RID: 152
	// (add) Token: 0x06005550 RID: 21840 RVA: 0x001BD7A8 File Offset: 0x001BB9A8
	// (remove) Token: 0x06005551 RID: 21841 RVA: 0x001BD7E0 File Offset: 0x001BB9E0
	public event TriggerEventNotifier.TriggerEvent TriggerEnterEvent;

	// Token: 0x14000099 RID: 153
	// (add) Token: 0x06005552 RID: 21842 RVA: 0x001BD818 File Offset: 0x001BBA18
	// (remove) Token: 0x06005553 RID: 21843 RVA: 0x001BD850 File Offset: 0x001BBA50
	public event TriggerEventNotifier.TriggerEvent TriggerExitEvent;

	// Token: 0x06005554 RID: 21844 RVA: 0x001BD885 File Offset: 0x001BBA85
	private void OnTriggerEnter(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerEnterEvent = this.TriggerEnterEvent;
		if (triggerEnterEvent == null)
		{
			return;
		}
		triggerEnterEvent(this, other);
	}

	// Token: 0x06005555 RID: 21845 RVA: 0x001BD899 File Offset: 0x001BBA99
	private void OnTriggerExit(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerExitEvent = this.TriggerExitEvent;
		if (triggerExitEvent == null)
		{
			return;
		}
		triggerExitEvent(this, other);
	}

	// Token: 0x040065A1 RID: 26017
	[HideInInspector]
	public int maskIndex;

	// Token: 0x02000D95 RID: 3477
	// (Invoke) Token: 0x06005558 RID: 21848
	public delegate void TriggerEvent(TriggerEventNotifier notifier, Collider collider);
}
