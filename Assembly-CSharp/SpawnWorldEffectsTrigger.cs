using System;
using GorillaTag.Reactions;
using UnityEngine;

// Token: 0x02000DB2 RID: 3506
[RequireComponent(typeof(SpawnWorldEffects))]
public class SpawnWorldEffectsTrigger : MonoBehaviour
{
	// Token: 0x060055E7 RID: 21991 RVA: 0x001BF70A File Offset: 0x001BD90A
	private void OnEnable()
	{
		if (this.swe == null)
		{
			this.swe = base.GetComponent<SpawnWorldEffects>();
		}
	}

	// Token: 0x060055E8 RID: 21992 RVA: 0x001BF726 File Offset: 0x001BD926
	private void OnTriggerEnter(Collider other)
	{
		this.spawnTime = Time.time;
		this.swe.RequestSpawn(base.transform.position);
	}

	// Token: 0x060055E9 RID: 21993 RVA: 0x001BF749 File Offset: 0x001BD949
	private void OnTriggerStay(Collider other)
	{
		if (Time.time - this.spawnTime < this.spawnCooldown)
		{
			return;
		}
		this.swe.RequestSpawn(base.transform.position);
		this.spawnTime = Time.time;
	}

	// Token: 0x040065FB RID: 26107
	private SpawnWorldEffects swe;

	// Token: 0x040065FC RID: 26108
	private float spawnTime;

	// Token: 0x040065FD RID: 26109
	[SerializeField]
	private float spawnCooldown = 1f;
}
