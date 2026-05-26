using System;
using GorillaTag;
using GorillaTag.Reactions;
using UnityEngine;

// Token: 0x02000371 RID: 881
public class BasicFireSpawner : MonoBehaviour
{
	// Token: 0x0600159C RID: 5532 RVA: 0x00072375 File Offset: 0x00070575
	private void Awake()
	{
		this.scale = this.fireScaleMinMax.y;
	}

	// Token: 0x0600159D RID: 5533 RVA: 0x00072388 File Offset: 0x00070588
	public void InterpolateScale(float f)
	{
		this.scale = Mathf.Lerp(this.fireScaleMinMax.x, this.fireScaleMinMax.y, f);
	}

	// Token: 0x0600159E RID: 5534 RVA: 0x000723AC File Offset: 0x000705AC
	public void Spawn()
	{
		if (this.firePool == null)
		{
			this.firePool = ObjectPools.instance.GetPoolByHash(this.firePrefab);
		}
		FireManager.SpawnFire(this.firePool, base.transform.position, Vector3.up, this.scale);
	}

	// Token: 0x04001A64 RID: 6756
	[SerializeField]
	private HashWrapper firePrefab;

	// Token: 0x04001A65 RID: 6757
	[SerializeField]
	private Vector2 fireScaleMinMax = Vector2.one;

	// Token: 0x04001A66 RID: 6758
	private SinglePool firePool;

	// Token: 0x04001A67 RID: 6759
	private float scale;
}
