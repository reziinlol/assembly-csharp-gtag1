using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003B2 RID: 946
public class LowEffortZone : GorillaTriggerBox
{
	// Token: 0x060016CC RID: 5836 RVA: 0x000848DA File Offset: 0x00082ADA
	private void Awake()
	{
		if (this.triggerOnAwake)
		{
			this.OnBoxTriggered();
		}
	}

	// Token: 0x060016CD RID: 5837 RVA: 0x000848EC File Offset: 0x00082AEC
	public override void OnBoxTriggered()
	{
		for (int i = 0; i < this.objectsToEnable.Length; i++)
		{
			if (this.objectsToEnable[i] != null)
			{
				this.objectsToEnable[i].SetActive(true);
			}
		}
		for (int j = 0; j < this.objectsToDisable.Length; j++)
		{
			if (this.objectsToDisable[j] != null)
			{
				this.objectsToDisable[j].SetActive(false);
			}
		}
		UnityEvent unityEvent = this.onTriggeredEvents;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x040021D2 RID: 8658
	public GameObject[] objectsToEnable;

	// Token: 0x040021D3 RID: 8659
	public GameObject[] objectsToDisable;

	// Token: 0x040021D4 RID: 8660
	public bool triggerOnAwake;

	// Token: 0x040021D5 RID: 8661
	public UnityEvent onTriggeredEvents;
}
