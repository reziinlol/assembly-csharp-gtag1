using System;
using UnityEngine;

// Token: 0x02000DC9 RID: 3529
public class GrowUntilCollision : MonoBehaviour
{
	// Token: 0x06005679 RID: 22137 RVA: 0x001C0B68 File Offset: 0x001BED68
	private void Start()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		if (this.audioSource != null)
		{
			this.maxVolume = this.audioSource.volume;
			this.maxPitch = this.audioSource.pitch;
		}
		this.zero();
	}

	// Token: 0x0600567A RID: 22138 RVA: 0x001C0BB8 File Offset: 0x001BEDB8
	private void zero()
	{
		base.transform.localScale = Vector3.one * this.initialRadius;
		if (this.audioSource != null)
		{
			this.audioSource.volume = 0f;
			this.audioSource.pitch = 1f;
		}
		this.timeSinceTrigger = 0f;
	}

	// Token: 0x0600567B RID: 22139 RVA: 0x001C0C19 File Offset: 0x001BEE19
	private void OnTriggerEnter(Collider other)
	{
		this.tryToTrigger(base.transform.position, other.transform.position);
	}

	// Token: 0x0600567C RID: 22140 RVA: 0x001C0C19 File Offset: 0x001BEE19
	private void OnTriggerExit(Collider other)
	{
		this.tryToTrigger(base.transform.position, other.transform.position);
	}

	// Token: 0x0600567D RID: 22141 RVA: 0x001C0C38 File Offset: 0x001BEE38
	private void OnCollisionEnter(Collision collision)
	{
		this.tryToTrigger(base.transform.position, collision.GetContact(0).point);
	}

	// Token: 0x0600567E RID: 22142 RVA: 0x001C0C68 File Offset: 0x001BEE68
	private void OnCollisionExit(Collision collision)
	{
		this.tryToTrigger(base.transform.position, collision.GetContact(0).point);
	}

	// Token: 0x0600567F RID: 22143 RVA: 0x001C0C95 File Offset: 0x001BEE95
	private void tryToTrigger(Vector3 p1, Vector3 p2)
	{
		if (this.timeSinceTrigger > this.minRetriggerTime)
		{
			if (this.colliderFound != null)
			{
				this.colliderFound.Invoke(p1, p2);
			}
			this.zero();
		}
	}

	// Token: 0x06005680 RID: 22144 RVA: 0x001C0CC0 File Offset: 0x001BEEC0
	private void Update()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		if (base.transform.localScale.x < this.maxSize * num)
		{
			base.transform.localScale += Vector3.one * Time.deltaTime * num;
			if (this.audioSource != null)
			{
				this.audioSource.volume = this.maxVolume * (base.transform.localScale.x / this.maxSize);
				this.audioSource.pitch = 1f + this.maxPitch * (base.transform.localScale.x / this.maxSize);
			}
		}
		this.timeSinceTrigger += Time.deltaTime;
	}

	// Token: 0x0400665C RID: 26204
	[SerializeField]
	private float maxSize = 10f;

	// Token: 0x0400665D RID: 26205
	[SerializeField]
	private float initialRadius = 1f;

	// Token: 0x0400665E RID: 26206
	[SerializeField]
	private float minRetriggerTime = 1f;

	// Token: 0x0400665F RID: 26207
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04006660 RID: 26208
	private AudioSource audioSource;

	// Token: 0x04006661 RID: 26209
	private float maxVolume;

	// Token: 0x04006662 RID: 26210
	private float maxPitch;

	// Token: 0x04006663 RID: 26211
	private float timeSinceTrigger;
}
