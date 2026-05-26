using System;
using UnityEngine;

// Token: 0x02000DCF RID: 3535
public class RandomLocalColliders : MonoBehaviour
{
	// Token: 0x06005694 RID: 22164 RVA: 0x001C11BB File Offset: 0x001BF3BB
	private void Start()
	{
		this.seekFreq = RandomLocalColliders.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x06005695 RID: 22165 RVA: 0x001C11DC File Offset: 0x001BF3DC
	private void Update()
	{
		if (this.colliderFound == null)
		{
			return;
		}
		this.timeSinceSeek += Time.deltaTime;
		if (this.timeSinceSeek > this.seekFreq)
		{
			this.seek();
			this.timeSinceSeek = 0f;
			this.seekFreq = RandomLocalColliders.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x06005696 RID: 22166 RVA: 0x001C1240 File Offset: 0x001BF440
	private void seek()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		int num2 = Physics.RaycastNonAlloc(base.transform.position, RandomLocalColliders.rand.NextPointOnSphere(1f), this.raycastHits, this.maxRadias * num);
		if (num2 <= 0)
		{
			return;
		}
		int num3 = RandomLocalColliders.rand.NextInt(num2);
		for (int i = 0; i < num2; i++)
		{
			if (this.raycastHits[(i + num3) % num2].distance >= this.minRadias * num)
			{
				this.colliderFound.Invoke(base.transform.position, this.raycastHits[(i + num3) % num2].point);
				return;
			}
		}
	}

	// Token: 0x04006674 RID: 26228
	private static SRand rand = new SRand("RandomLocalColliders");

	// Token: 0x04006675 RID: 26229
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x04006676 RID: 26230
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x04006677 RID: 26231
	[SerializeField]
	private float minRadias = 1f;

	// Token: 0x04006678 RID: 26232
	[SerializeField]
	private float maxRadias = 10f;

	// Token: 0x04006679 RID: 26233
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x0400667A RID: 26234
	private float timeSinceSeek;

	// Token: 0x0400667B RID: 26235
	private float seekFreq;

	// Token: 0x0400667C RID: 26236
	private RaycastHit[] raycastHits = new RaycastHit[100];
}
