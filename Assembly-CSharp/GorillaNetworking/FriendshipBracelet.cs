using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x0200103E RID: 4158
	public class FriendshipBracelet : MonoBehaviour
	{
		// Token: 0x06006800 RID: 26624 RVA: 0x0021910E File Offset: 0x0021730E
		protected void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
		}

		// Token: 0x06006801 RID: 26625 RVA: 0x0021911C File Offset: 0x0021731C
		private AudioSource GetAudioSource()
		{
			if (!this.isLeftHand)
			{
				return this.ownerRig.rightHandPlayer;
			}
			return this.ownerRig.leftHandPlayer;
		}

		// Token: 0x06006802 RID: 26626 RVA: 0x0021913D File Offset: 0x0021733D
		private void OnEnable()
		{
			this.PlayAppearEffects();
		}

		// Token: 0x06006803 RID: 26627 RVA: 0x00219145 File Offset: 0x00217345
		public void PlayAppearEffects()
		{
			this.GetAudioSource().GTPlayOneShot(this.braceletFormedSound, 1f);
			if (this.braceletFormedParticle)
			{
				this.braceletFormedParticle.Play();
			}
		}

		// Token: 0x06006804 RID: 26628 RVA: 0x00219178 File Offset: 0x00217378
		private void OnDisable()
		{
			if (!this.ownerRig.gameObject.activeInHierarchy)
			{
				return;
			}
			this.GetAudioSource().GTPlayOneShot(this.braceletBrokenSound, 1f);
			if (this.braceletBrokenParticle)
			{
				this.braceletBrokenParticle.Play();
			}
		}

		// Token: 0x06006805 RID: 26629 RVA: 0x002191C8 File Offset: 0x002173C8
		public void UpdateBeads(List<Color> colors, int selfIndex)
		{
			int num = colors.Count - 1;
			int num2 = (this.braceletBeads.Length - num) / 2;
			for (int i = 0; i < this.braceletBeads.Length; i++)
			{
				int num3 = i - num2;
				if (num3 >= 0 && num3 < num)
				{
					this.braceletBeads[i].enabled = true;
					this.braceletBeads[i].material.color = colors[num3];
					this.braceletBananas[i].gameObject.SetActive(num3 == selfIndex);
				}
				else
				{
					this.braceletBeads[i].enabled = false;
					this.braceletBananas[i].gameObject.SetActive(false);
				}
			}
			SkinnedMeshRenderer[] array = this.braceletStrings;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].material.color = colors[colors.Count - 1];
			}
		}

		// Token: 0x04007758 RID: 30552
		[SerializeField]
		private SkinnedMeshRenderer[] braceletStrings;

		// Token: 0x04007759 RID: 30553
		[SerializeField]
		private MeshRenderer[] braceletBeads;

		// Token: 0x0400775A RID: 30554
		[SerializeField]
		private MeshRenderer[] braceletBananas;

		// Token: 0x0400775B RID: 30555
		[SerializeField]
		private bool isLeftHand;

		// Token: 0x0400775C RID: 30556
		[SerializeField]
		private AudioClip braceletFormedSound;

		// Token: 0x0400775D RID: 30557
		[SerializeField]
		private AudioClip braceletBrokenSound;

		// Token: 0x0400775E RID: 30558
		[SerializeField]
		private ParticleSystem braceletFormedParticle;

		// Token: 0x0400775F RID: 30559
		[SerializeField]
		private ParticleSystem braceletBrokenParticle;

		// Token: 0x04007760 RID: 30560
		private VRRig ownerRig;
	}
}
