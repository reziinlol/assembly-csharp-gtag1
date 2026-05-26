using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000755 RID: 1877
public class GRCurrencyDepositor : MonoBehaviour
{
	// Token: 0x06002F7F RID: 12159 RVA: 0x00102B24 File Offset: 0x00100D24
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06002F80 RID: 12160 RVA: 0x00102B30 File Offset: 0x00100D30
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody != null)
		{
			GRCollectible component = other.attachedRigidbody.GetComponent<GRCollectible>();
			if (component != null)
			{
				if ((component.type == ProgressionManager.CoreType.ChaosSeed && !this.collectSentientCores) || (component.type != ProgressionManager.CoreType.ChaosSeed && this.collectSentientCores))
				{
					return;
				}
				if (this.reactor.grManager.IsAuthority())
				{
					this.reactor.grManager.RequestDepositCollectible(component.entity.id);
				}
				this.collectibleDepositedEffect.Play();
				this.audioSource.volume = this.collectibleDepositedClipVolume;
				this.audioSource.PlayOneShot(this.collectibleDepositedClip);
				if (component.entity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					if (GamePlayerLocal.instance.gamePlayer.GetGrabbedGameEntityId(0) == component.entity.id)
					{
						GorillaTagger.Instance.StartVibration(true, 0.5f, 0.15f);
						return;
					}
					if (GamePlayerLocal.instance.gamePlayer.GetGrabbedGameEntityId(1) == component.entity.id)
					{
						GorillaTagger.Instance.StartVibration(false, 0.5f, 0.15f);
					}
				}
			}
		}
	}

	// Token: 0x04003CFE RID: 15614
	public Transform depositingChargePoint;

	// Token: 0x04003CFF RID: 15615
	[SerializeField]
	private ParticleSystem collectibleDepositedEffect;

	// Token: 0x04003D00 RID: 15616
	[SerializeField]
	private AudioClip collectibleDepositedClip;

	// Token: 0x04003D01 RID: 15617
	[SerializeField]
	private float collectibleDepositedClipVolume;

	// Token: 0x04003D02 RID: 15618
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003D03 RID: 15619
	[SerializeField]
	private bool collectSentientCores;

	// Token: 0x04003D04 RID: 15620
	private const float hapticStrength = 0.5f;

	// Token: 0x04003D05 RID: 15621
	private const float hapticDuration = 0.15f;

	// Token: 0x04003D06 RID: 15622
	[NonSerialized]
	public GhostReactor reactor;
}
