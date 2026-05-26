using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000AB8 RID: 2744
public class HalloweenWatcherEyes : MonoBehaviour
{
	// Token: 0x06004629 RID: 17961 RVA: 0x0017B6FC File Offset: 0x001798FC
	private void Start()
	{
		this.playersViewCenterCosAngle = Mathf.Cos(this.playersViewCenterAngle * 0.017453292f);
		this.watchMinCosAngle = Mathf.Cos(this.watchMaxAngle * 0.017453292f);
		base.StartCoroutine(this.CheckIfNearPlayer(Random.Range(0f, this.timeBetweenUpdates)));
		base.enabled = false;
	}

	// Token: 0x0600462A RID: 17962 RVA: 0x0017B75B File Offset: 0x0017995B
	private IEnumerator CheckIfNearPlayer(float initialSleep)
	{
		yield return new WaitForSeconds(initialSleep);
		for (;;)
		{
			base.enabled = ((base.transform.position - GTPlayer.Instance.transform.position).sqrMagnitude < this.watchRange * this.watchRange);
			if (!base.enabled)
			{
				this.LookNormal();
			}
			yield return new WaitForSeconds(this.timeBetweenUpdates);
		}
		yield break;
	}

	// Token: 0x0600462B RID: 17963 RVA: 0x0017B774 File Offset: 0x00179974
	private void Update()
	{
		Vector3 normalized = (GTPlayer.Instance.headCollider.transform.position - base.transform.position).normalized;
		if (Vector3.Dot(GTPlayer.Instance.headCollider.transform.forward, -normalized) > this.playersViewCenterCosAngle)
		{
			this.LookNormal();
			this.pretendingToBeNormalUntilTimestamp = Time.time + this.durationToBeNormalWhenPlayerLooks;
		}
		if (this.pretendingToBeNormalUntilTimestamp > Time.time)
		{
			return;
		}
		if (Vector3.Dot(base.transform.forward, normalized) < this.watchMinCosAngle)
		{
			this.LookNormal();
			return;
		}
		Quaternion b = Quaternion.LookRotation(normalized, base.transform.up);
		Quaternion rotation = Quaternion.Lerp(base.transform.rotation, b, this.lerpValue);
		this.leftEye.transform.rotation = rotation;
		this.rightEye.transform.rotation = rotation;
		if (this.lerpDuration > 0f)
		{
			this.lerpValue = Mathf.MoveTowards(this.lerpValue, 1f, Time.deltaTime / this.lerpDuration);
			return;
		}
		this.lerpValue = 1f;
	}

	// Token: 0x0600462C RID: 17964 RVA: 0x0017B8A2 File Offset: 0x00179AA2
	private void LookNormal()
	{
		this.leftEye.transform.localRotation = Quaternion.identity;
		this.rightEye.transform.localRotation = Quaternion.identity;
		this.lerpValue = 0f;
	}

	// Token: 0x0400589D RID: 22685
	public float timeBetweenUpdates = 5f;

	// Token: 0x0400589E RID: 22686
	public float watchRange;

	// Token: 0x0400589F RID: 22687
	public float watchMaxAngle;

	// Token: 0x040058A0 RID: 22688
	public float lerpDuration = 1f;

	// Token: 0x040058A1 RID: 22689
	public float playersViewCenterAngle = 30f;

	// Token: 0x040058A2 RID: 22690
	public float durationToBeNormalWhenPlayerLooks = 3f;

	// Token: 0x040058A3 RID: 22691
	public GameObject leftEye;

	// Token: 0x040058A4 RID: 22692
	public GameObject rightEye;

	// Token: 0x040058A5 RID: 22693
	private float playersViewCenterCosAngle;

	// Token: 0x040058A6 RID: 22694
	private float watchMinCosAngle;

	// Token: 0x040058A7 RID: 22695
	private float pretendingToBeNormalUntilTimestamp;

	// Token: 0x040058A8 RID: 22696
	private float lerpValue;
}
