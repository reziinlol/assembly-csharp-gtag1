using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200030D RID: 781
public class DampenVolumeSpace : MonoBehaviour
{
	// Token: 0x060013C2 RID: 5058 RVA: 0x0006B466 File Offset: 0x00069666
	private void Awake()
	{
		if (this.audioSource == null)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x0006B480 File Offset: 0x00069680
	private void OnTriggerEnter(Collider other)
	{
		GTPlayer componentInParent = other.GetComponentInParent<GTPlayer>();
		if (componentInParent != null && componentInParent == GTPlayer.Instance)
		{
			this.audioSource.volume = this.setVolume;
		}
	}

	// Token: 0x0400185B RID: 6235
	public AudioSource audioSource;

	// Token: 0x0400185C RID: 6236
	public float setVolume;
}
