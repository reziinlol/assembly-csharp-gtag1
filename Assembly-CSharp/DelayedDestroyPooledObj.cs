using System;
using UnityEngine;

// Token: 0x02000D43 RID: 3395
public class DelayedDestroyPooledObj : MonoBehaviour
{
	// Token: 0x060053AC RID: 21420 RVA: 0x001B6352 File Offset: 0x001B4552
	protected void OnEnable()
	{
		if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
		{
			return;
		}
		this.timeToDie = Time.time + this.destroyDelay;
	}

	// Token: 0x060053AD RID: 21421 RVA: 0x001B6380 File Offset: 0x001B4580
	protected void LateUpdate()
	{
		if (Time.time > this.timeToDie)
		{
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x040064D1 RID: 25809
	[Tooltip("Return to the object pool after this many seconds.")]
	public float destroyDelay;

	// Token: 0x040064D2 RID: 25810
	private float timeToDie = -1f;
}
