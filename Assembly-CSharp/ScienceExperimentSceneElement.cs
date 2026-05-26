using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020009C4 RID: 2500
public class ScienceExperimentSceneElement : MonoBehaviour, ITickSystemPost
{
	// Token: 0x170005EC RID: 1516
	// (get) Token: 0x0600400B RID: 16395 RVA: 0x00156E08 File Offset: 0x00155008
	// (set) Token: 0x0600400C RID: 16396 RVA: 0x00156E10 File Offset: 0x00155010
	bool ITickSystemPost.PostTickRunning { get; set; }

	// Token: 0x0600400D RID: 16397 RVA: 0x00156E1C File Offset: 0x0015501C
	void ITickSystemPost.PostTick()
	{
		base.transform.position = this.followElement.position;
		base.transform.rotation = this.followElement.rotation;
		base.transform.localScale = this.followElement.localScale;
	}

	// Token: 0x0600400E RID: 16398 RVA: 0x00156E6B File Offset: 0x0015506B
	private void Start()
	{
		this.followElement = ScienceExperimentManager.instance.GetElement(this.elementID);
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x0600400F RID: 16399 RVA: 0x00156E8B File Offset: 0x0015508B
	private void OnDestroy()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x04005092 RID: 20626
	public ScienceExperimentElementID elementID;

	// Token: 0x04005093 RID: 20627
	private Transform followElement;
}
