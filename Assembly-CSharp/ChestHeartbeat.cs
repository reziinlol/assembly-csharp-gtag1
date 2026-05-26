using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000669 RID: 1641
public class ChestHeartbeat : MonoBehaviour
{
	// Token: 0x06002905 RID: 10501 RVA: 0x000DE350 File Offset: 0x000DC550
	public void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			if ((PhotonNetwork.ServerTimestamp > this.lastShot + this.millisMin || Mathf.Abs(PhotonNetwork.ServerTimestamp - this.lastShot) > 10000) && PhotonNetwork.ServerTimestamp % 1500 <= 10)
			{
				this.lastShot = PhotonNetwork.ServerTimestamp;
				this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
				base.StartCoroutine(this.HeartBeat());
				return;
			}
		}
		else if ((Time.time * 1000f > (float)(this.lastShot + this.millisMin) || Mathf.Abs(Time.time * 1000f - (float)this.lastShot) > 10000f) && Time.time * 1000f % 1500f <= 10f)
		{
			this.lastShot = PhotonNetwork.ServerTimestamp;
			this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
			base.StartCoroutine(this.HeartBeat());
		}
	}

	// Token: 0x06002906 RID: 10502 RVA: 0x000DE45E File Offset: 0x000DC65E
	private IEnumerator HeartBeat()
	{
		float startTime = Time.time;
		while (Time.time < startTime + this.endtime)
		{
			if (Time.time < startTime + this.minTime)
			{
				this.deltaTime = Time.time - startTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * this.heartMinSize, this.deltaTime / this.minTime);
			}
			else if (Time.time < startTime + this.maxTime)
			{
				this.deltaTime = Time.time - startTime - this.minTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMinSize, Vector3.one * this.heartMaxSize, this.deltaTime / (this.maxTime - this.minTime));
			}
			else if (Time.time < startTime + this.endtime)
			{
				this.deltaTime = Time.time - startTime - this.maxTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMaxSize, Vector3.one, this.deltaTime / (this.endtime - this.maxTime));
			}
			yield return new WaitForFixedUpdate();
		}
		yield break;
	}

	// Token: 0x04003588 RID: 13704
	public int millisToWait;

	// Token: 0x04003589 RID: 13705
	public int millisMin = 300;

	// Token: 0x0400358A RID: 13706
	public int lastShot;

	// Token: 0x0400358B RID: 13707
	public AudioSource audioSource;

	// Token: 0x0400358C RID: 13708
	public Transform scaleTransform;

	// Token: 0x0400358D RID: 13709
	private float deltaTime;

	// Token: 0x0400358E RID: 13710
	private float heartMinSize = 0.9f;

	// Token: 0x0400358F RID: 13711
	private float heartMaxSize = 1.2f;

	// Token: 0x04003590 RID: 13712
	private float minTime = 0.05f;

	// Token: 0x04003591 RID: 13713
	private float maxTime = 0.1f;

	// Token: 0x04003592 RID: 13714
	private float endtime = 0.25f;

	// Token: 0x04003593 RID: 13715
	private float currentTime;
}
