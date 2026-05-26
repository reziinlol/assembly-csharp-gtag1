using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000AF1 RID: 2801
public class TaggedColliderTrigger : MonoBehaviour
{
	// Token: 0x060047A9 RID: 18345 RVA: 0x001811BE File Offset: 0x0017F3BE
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag(this.tag))
		{
			return;
		}
		if (this._sinceLastEnter.HasElapsed(this.enterHysteresis, true))
		{
			UnityEvent<Collider> unityEvent = this.onEnter;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(other);
		}
	}

	// Token: 0x060047AA RID: 18346 RVA: 0x001811F4 File Offset: 0x0017F3F4
	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag(this.tag))
		{
			return;
		}
		if (this._sinceLastExit.HasElapsed(this.exitHysteresis, true))
		{
			UnityEvent<Collider> unityEvent = this.onExit;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(other);
		}
	}

	// Token: 0x040059D6 RID: 22998
	public new UnityTag tag;

	// Token: 0x040059D7 RID: 22999
	public UnityEvent<Collider> onEnter = new UnityEvent<Collider>();

	// Token: 0x040059D8 RID: 23000
	public UnityEvent<Collider> onExit = new UnityEvent<Collider>();

	// Token: 0x040059D9 RID: 23001
	public float enterHysteresis = 0.125f;

	// Token: 0x040059DA RID: 23002
	public float exitHysteresis = 0.125f;

	// Token: 0x040059DB RID: 23003
	private TimeSince _sinceLastEnter;

	// Token: 0x040059DC RID: 23004
	private TimeSince _sinceLastExit;
}
