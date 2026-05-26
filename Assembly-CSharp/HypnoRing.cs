using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000586 RID: 1414
public class HypnoRing : MonoBehaviour, ISpawnable
{
	// Token: 0x170003C1 RID: 961
	// (get) Token: 0x060023C9 RID: 9161 RVA: 0x000C0551 File Offset: 0x000BE751
	// (set) Token: 0x060023CA RID: 9162 RVA: 0x000C0559 File Offset: 0x000BE759
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170003C2 RID: 962
	// (get) Token: 0x060023CB RID: 9163 RVA: 0x000C0562 File Offset: 0x000BE762
	// (set) Token: 0x060023CC RID: 9164 RVA: 0x000C056A File Offset: 0x000BE76A
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060023CD RID: 9165 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x000C0573 File Offset: 0x000BE773
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x000C057C File Offset: 0x000BE77C
	private void Update()
	{
		if ((this.attachedToLeftHand ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT) > 0.5f)
		{
			base.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * this.rotationSpeed, Vector3.up);
			this.currentVolume = Mathf.MoveTowards(this.currentVolume, this.maxVolume, Time.deltaTime / this.fadeInDuration);
			this.audioSource.volume = this.currentVolume;
			if (!this.audioSource.isPlaying)
			{
				this.audioSource.GTPlay();
				return;
			}
		}
		else
		{
			this.currentVolume = Mathf.MoveTowards(this.currentVolume, 0f, Time.deltaTime / this.fadeOutDuration);
			if (this.audioSource.isPlaying)
			{
				if (this.currentVolume == 0f)
				{
					this.audioSource.GTStop();
					return;
				}
				this.audioSource.volume = this.currentVolume;
			}
		}
	}

	// Token: 0x04002EEE RID: 12014
	[SerializeField]
	private bool attachedToLeftHand;

	// Token: 0x04002EEF RID: 12015
	private VRRig myRig;

	// Token: 0x04002EF0 RID: 12016
	[SerializeField]
	private float rotationSpeed;

	// Token: 0x04002EF1 RID: 12017
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002EF2 RID: 12018
	[SerializeField]
	private float maxVolume = 1f;

	// Token: 0x04002EF3 RID: 12019
	[SerializeField]
	private float fadeInDuration;

	// Token: 0x04002EF4 RID: 12020
	[SerializeField]
	private float fadeOutDuration;

	// Token: 0x04002EF7 RID: 12023
	private float currentVolume;
}
