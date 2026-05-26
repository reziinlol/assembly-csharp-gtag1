using System;
using UnityEngine;

// Token: 0x020001FC RID: 508
public class SpawnOnEnter : MonoBehaviour
{
	// Token: 0x06000D54 RID: 3412 RVA: 0x00048E08 File Offset: 0x00047008
	public void OnTriggerEnter(Collider other)
	{
		if (Time.time > this.lastSpawnTime + this.cooldown)
		{
			this.lastSpawnTime = Time.time;
			ObjectPools.instance.Instantiate(this.prefab, other.transform.position, true);
		}
	}

	// Token: 0x04000FFC RID: 4092
	public GameObject prefab;

	// Token: 0x04000FFD RID: 4093
	public float cooldown = 0.1f;

	// Token: 0x04000FFE RID: 4094
	private float lastSpawnTime;
}
