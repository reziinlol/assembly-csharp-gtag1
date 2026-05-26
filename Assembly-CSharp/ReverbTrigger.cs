using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020004D5 RID: 1237
public class ReverbTrigger : MonoBehaviour
{
	// Token: 0x06001E1E RID: 7710 RVA: 0x000A173A File Offset: 0x0009F93A
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			this.targetSnapshot.TransitionTo(this.transitionTime);
		}
	}

	// Token: 0x06001E1F RID: 7711 RVA: 0x000A175B File Offset: 0x0009F95B
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			this.normalSnapshot.TransitionTo(this.transitionTime);
		}
	}

	// Token: 0x0400284B RID: 10315
	[SerializeField]
	private AudioMixer mixer;

	// Token: 0x0400284C RID: 10316
	[SerializeField]
	private AudioMixerSnapshot targetSnapshot;

	// Token: 0x0400284D RID: 10317
	[SerializeField]
	private AudioMixerSnapshot normalSnapshot;

	// Token: 0x0400284E RID: 10318
	[SerializeField]
	private Collider reverbTrigger;

	// Token: 0x0400284F RID: 10319
	[SerializeField]
	private float transitionTime = 1f;
}
