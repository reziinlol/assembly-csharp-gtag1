using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000489 RID: 1161
public class OnTapHandler : Tappable
{
	// Token: 0x06001C36 RID: 7222 RVA: 0x00098CAE File Offset: 0x00096EAE
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped sender)
	{
		UnityEvent onTapEvents = this.OnTapEvents;
		if (onTapEvents == null)
		{
			return;
		}
		onTapEvents.Invoke();
	}

	// Token: 0x06001C37 RID: 7223 RVA: 0x00098CC0 File Offset: 0x00096EC0
	public override void OnGrabLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
		UnityEvent onGrabEvents = this.OnGrabEvents;
		if (onGrabEvents == null)
		{
			return;
		}
		onGrabEvents.Invoke();
	}

	// Token: 0x06001C38 RID: 7224 RVA: 0x00098CD2 File Offset: 0x00096ED2
	public override void OnReleaseLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
		UnityEvent onReleaseEvents = this.OnReleaseEvents;
		if (onReleaseEvents == null)
		{
			return;
		}
		onReleaseEvents.Invoke();
	}

	// Token: 0x0400264A RID: 9802
	[SerializeField]
	private UnityEvent OnTapEvents;

	// Token: 0x0400264B RID: 9803
	[SerializeField]
	private UnityEvent OnGrabEvents;

	// Token: 0x0400264C RID: 9804
	[SerializeField]
	private UnityEvent OnReleaseEvents;
}
