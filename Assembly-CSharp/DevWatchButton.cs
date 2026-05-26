using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000B0 RID: 176
public class DevWatchButton : MonoBehaviour
{
	// Token: 0x0600043D RID: 1085 RVA: 0x00018BC4 File Offset: 0x00016DC4
	public void OnTriggerEnter(Collider other)
	{
		this.SearchEvent.Invoke();
	}

	// Token: 0x040004A8 RID: 1192
	public UnityEvent SearchEvent = new UnityEvent();
}
