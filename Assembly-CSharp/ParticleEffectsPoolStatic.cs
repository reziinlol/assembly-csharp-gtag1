using System;
using UnityEngine;

// Token: 0x020002E9 RID: 745
public class ParticleEffectsPoolStatic<T> : ParticleEffectsPool where T : ParticleEffectsPool
{
	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x060012FF RID: 4863 RVA: 0x00064ADF File Offset: 0x00062CDF
	public static T Instance
	{
		get
		{
			return ParticleEffectsPoolStatic<T>.gInstance;
		}
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x00064AE6 File Offset: 0x00062CE6
	protected override void OnPoolAwake()
	{
		if (ParticleEffectsPoolStatic<T>.gInstance && ParticleEffectsPoolStatic<T>.gInstance != this)
		{
			Object.Destroy(this);
			return;
		}
		ParticleEffectsPoolStatic<T>.gInstance = (this as T);
	}

	// Token: 0x04001732 RID: 5938
	protected static T gInstance;
}
