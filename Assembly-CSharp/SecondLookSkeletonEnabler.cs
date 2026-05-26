using System;
using UnityEngine;

// Token: 0x020001CF RID: 463
public class SecondLookSkeletonEnabler : Tappable
{
	// Token: 0x06000C4F RID: 3151 RVA: 0x00043663 File Offset: 0x00041863
	private void Awake()
	{
		this.isTapped = false;
		this.skele = Object.FindFirstObjectByType<SecondLookSkeleton>();
		this.skele.spookyText = this.spookyText;
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x00043688 File Offset: 0x00041888
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (!this.isTapped)
		{
			base.OnTapLocal(tapStrength, tapTime, info);
			if (this.skele != null)
			{
				this.skele.tapped = true;
			}
			base.gameObject.SetActive(false);
			this.isTapped = true;
			this.playOnDisappear.GTPlay();
			this.particles.Play();
		}
	}

	// Token: 0x04000EFC RID: 3836
	public bool isTapped;

	// Token: 0x04000EFD RID: 3837
	public AudioSource playOnDisappear;

	// Token: 0x04000EFE RID: 3838
	public ParticleSystem particles;

	// Token: 0x04000EFF RID: 3839
	public GameObject spookyText;

	// Token: 0x04000F00 RID: 3840
	private SecondLookSkeleton skele;
}
