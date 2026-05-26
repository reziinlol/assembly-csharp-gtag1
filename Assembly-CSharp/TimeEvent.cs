using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000DC3 RID: 3523
public class TimeEvent : MonoBehaviour
{
	// Token: 0x0600565A RID: 22106 RVA: 0x001C069F File Offset: 0x001BE89F
	protected void StartEvent()
	{
		this._ongoing = true;
		UnityEvent unityEvent = this.onEventStart;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x0600565B RID: 22107 RVA: 0x001C06B8 File Offset: 0x001BE8B8
	protected void StopEvent()
	{
		this._ongoing = false;
		UnityEvent unityEvent = this.onEventStop;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04006640 RID: 26176
	public UnityEvent onEventStart;

	// Token: 0x04006641 RID: 26177
	public UnityEvent onEventStop;

	// Token: 0x04006642 RID: 26178
	[SerializeField]
	protected bool _ongoing;
}
