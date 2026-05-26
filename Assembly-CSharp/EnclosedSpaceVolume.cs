using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000682 RID: 1666
public class EnclosedSpaceVolume : GorillaTriggerBox
{
	// Token: 0x06002983 RID: 10627 RVA: 0x000DFF72 File Offset: 0x000DE172
	private void Awake()
	{
		this.audioSourceInside.volume = this.quietVolume;
		this.audioSourceOutside.volume = this.loudVolume;
	}

	// Token: 0x06002984 RID: 10628 RVA: 0x000DFF96 File Offset: 0x000DE196
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody.GetComponentInParent<GTPlayer>() != null)
		{
			this.audioSourceInside.volume = this.loudVolume;
			this.audioSourceOutside.volume = this.quietVolume;
		}
	}

	// Token: 0x06002985 RID: 10629 RVA: 0x000DFFCD File Offset: 0x000DE1CD
	private void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody.GetComponentInParent<GTPlayer>() != null)
		{
			this.audioSourceInside.volume = this.quietVolume;
			this.audioSourceOutside.volume = this.loudVolume;
		}
	}

	// Token: 0x0400360F RID: 13839
	public AudioSource audioSourceInside;

	// Token: 0x04003610 RID: 13840
	public AudioSource audioSourceOutside;

	// Token: 0x04003611 RID: 13841
	public float loudVolume;

	// Token: 0x04003612 RID: 13842
	public float quietVolume;
}
