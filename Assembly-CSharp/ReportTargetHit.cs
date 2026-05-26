using System;
using NetSynchrony;
using UnityEngine;

// Token: 0x02000DD1 RID: 3537
public class ReportTargetHit : MonoBehaviour
{
	// Token: 0x060056A1 RID: 22177 RVA: 0x001C154C File Offset: 0x001BF74C
	private void Start()
	{
		this.seekFreq = ReportTargetHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x060056A2 RID: 22178 RVA: 0x001C156A File Offset: 0x001BF76A
	private void OnEnable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch += this.NsRand_Dispatch;
		}
	}

	// Token: 0x060056A3 RID: 22179 RVA: 0x001C1591 File Offset: 0x001BF791
	private void OnDisable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch -= this.NsRand_Dispatch;
		}
	}

	// Token: 0x060056A4 RID: 22180 RVA: 0x001C15B8 File Offset: 0x001BF7B8
	private void NsRand_Dispatch(RandomDispatcher randomDispatcher)
	{
		this.seek();
	}

	// Token: 0x060056A5 RID: 22181 RVA: 0x001C15C0 File Offset: 0x001BF7C0
	private void Update()
	{
		if (this.nsRand != null)
		{
			return;
		}
		this.timeSinceSeek += Time.deltaTime;
		if (this.timeSinceSeek > this.seekFreq)
		{
			this.seek();
			this.timeSinceSeek = 0f;
			this.seekFreq = ReportTargetHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x060056A6 RID: 22182 RVA: 0x001C162C File Offset: 0x001BF82C
	private void seek()
	{
		if (this.targets.Length != 0)
		{
			Vector3 direction = this.targets[ReportTargetHit.rand.NextInt(this.targets.Length)].position - base.transform.position;
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, direction, out raycastHit) && this.colliderFound != null)
			{
				this.colliderFound.Invoke(base.transform.position, raycastHit.point);
			}
		}
	}

	// Token: 0x04006686 RID: 26246
	private static SRand rand = new SRand("ReportForwardHit");

	// Token: 0x04006687 RID: 26247
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x04006688 RID: 26248
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x04006689 RID: 26249
	[SerializeField]
	private Transform[] targets;

	// Token: 0x0400668A RID: 26250
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x0400668B RID: 26251
	private float timeSinceSeek;

	// Token: 0x0400668C RID: 26252
	private float seekFreq;

	// Token: 0x0400668D RID: 26253
	[SerializeField]
	private RandomDispatcher nsRand;
}
