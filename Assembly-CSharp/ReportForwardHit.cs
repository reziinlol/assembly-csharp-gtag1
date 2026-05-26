using System;
using NetSynchrony;
using UnityEngine;

// Token: 0x02000DD0 RID: 3536
public class ReportForwardHit : MonoBehaviour
{
	// Token: 0x06005699 RID: 22169 RVA: 0x001C1385 File Offset: 0x001BF585
	private void Start()
	{
		this.seekFreq = ReportForwardHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x0600569A RID: 22170 RVA: 0x001C13A3 File Offset: 0x001BF5A3
	private void OnEnable()
	{
		if (this.seekOnEnable)
		{
			this.seek();
		}
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch += this.NsRand_Dispatch;
		}
	}

	// Token: 0x0600569B RID: 22171 RVA: 0x001C13D8 File Offset: 0x001BF5D8
	private void OnDisable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch -= this.NsRand_Dispatch;
		}
	}

	// Token: 0x0600569C RID: 22172 RVA: 0x001C13FF File Offset: 0x001BF5FF
	private void NsRand_Dispatch(RandomDispatcher randomDispatcher)
	{
		this.seek();
	}

	// Token: 0x0600569D RID: 22173 RVA: 0x001C1408 File Offset: 0x001BF608
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
			this.seekFreq = ReportForwardHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x0600569E RID: 22174 RVA: 0x001C1474 File Offset: 0x001BF674
	private void seek()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, this.maxRadias * num) && this.colliderFound != null)
		{
			this.colliderFound.Invoke(base.transform.position, raycastHit.point);
		}
	}

	// Token: 0x0400667D RID: 26237
	private static SRand rand = new SRand("ReportForwardHit");

	// Token: 0x0400667E RID: 26238
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x0400667F RID: 26239
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x04006680 RID: 26240
	[SerializeField]
	private float maxRadias = 10f;

	// Token: 0x04006681 RID: 26241
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04006682 RID: 26242
	[SerializeField]
	private RandomDispatcher nsRand;

	// Token: 0x04006683 RID: 26243
	private float timeSinceSeek;

	// Token: 0x04006684 RID: 26244
	private float seekFreq;

	// Token: 0x04006685 RID: 26245
	[SerializeField]
	private bool seekOnEnable;
}
