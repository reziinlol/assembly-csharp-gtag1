using System;
using UnityEngine;

// Token: 0x02000082 RID: 130
public class DelayedDestroyCrittersPooledObject : MonoBehaviour
{
	// Token: 0x06000330 RID: 816 RVA: 0x00013503 File Offset: 0x00011703
	protected void OnEnable()
	{
		if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
		{
			return;
		}
		this.timeToDie = Time.time + this.destroyDelay;
	}

	// Token: 0x06000331 RID: 817 RVA: 0x00013531 File Offset: 0x00011731
	protected void LateUpdate()
	{
		if (Time.time >= this.timeToDie)
		{
			CrittersPool.Return(base.gameObject);
		}
	}

	// Token: 0x040003CF RID: 975
	public float destroyDelay = 1f;

	// Token: 0x040003D0 RID: 976
	private float timeToDie = -1f;
}
