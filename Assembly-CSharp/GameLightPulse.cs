using System;
using UnityEngine;

// Token: 0x020006DA RID: 1754
public class GameLightPulse : GameLight, IGorillaSliceableSimple
{
	// Token: 0x06002C1C RID: 11292 RVA: 0x000EE956 File Offset: 0x000ECB56
	public new void Awake()
	{
		base.Awake();
		this.startingIntensity = this.light.intensity;
		this.offsetTime = Random.value / this.frequency;
	}

	// Token: 0x06002C1D RID: 11293 RVA: 0x000EE981 File Offset: 0x000ECB81
	protected new void OnEnable()
	{
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this);
	}

	// Token: 0x06002C1E RID: 11294 RVA: 0x000EE98F File Offset: 0x000ECB8F
	protected new void OnDisable()
	{
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this);
	}

	// Token: 0x06002C1F RID: 11295 RVA: 0x000EE9A0 File Offset: 0x000ECBA0
	public void SliceUpdate()
	{
		this.light.intensity = this.startingIntensity / 2f * Mathf.Sin((Time.time + this.offsetTime) * this.frequency * 2f * 3.1415927f % 6.2831855f) + this.startingIntensity / 2f;
	}

	// Token: 0x04003891 RID: 14481
	private float startingIntensity;

	// Token: 0x04003892 RID: 14482
	public float frequency;

	// Token: 0x04003893 RID: 14483
	private float offsetTime;
}
