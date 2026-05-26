using System;
using UnityEngine;

// Token: 0x020002E6 RID: 742
[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffect : MonoBehaviour
{
	// Token: 0x170001DD RID: 477
	// (get) Token: 0x060012E4 RID: 4836 RVA: 0x0006476F File Offset: 0x0006296F
	public long effectID
	{
		get
		{
			return this._effectID;
		}
	}

	// Token: 0x170001DE RID: 478
	// (get) Token: 0x060012E5 RID: 4837 RVA: 0x00064777 File Offset: 0x00062977
	public bool isPlaying
	{
		get
		{
			return this.system && this.system.isPlaying;
		}
	}

	// Token: 0x060012E6 RID: 4838 RVA: 0x00064793 File Offset: 0x00062993
	public virtual void Play()
	{
		base.gameObject.SetActive(true);
		this.system.Play(true);
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x000647AD File Offset: 0x000629AD
	public virtual void Stop()
	{
		this.system.Stop(true);
		base.gameObject.SetActive(false);
	}

	// Token: 0x060012E8 RID: 4840 RVA: 0x000647C7 File Offset: 0x000629C7
	private void OnParticleSystemStopped()
	{
		base.gameObject.SetActive(false);
		if (this.pool)
		{
			this.pool.Return(this);
		}
	}

	// Token: 0x04001724 RID: 5924
	public ParticleSystem system;

	// Token: 0x04001725 RID: 5925
	[SerializeField]
	private long _effectID;

	// Token: 0x04001726 RID: 5926
	public ParticleEffectsPool pool;

	// Token: 0x04001727 RID: 5927
	[NonSerialized]
	public int poolIndex = -1;
}
