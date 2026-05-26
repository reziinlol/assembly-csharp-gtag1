using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200123B RID: 4667
	public class SquirtingFlowerBadgeCosmetic : MonoBehaviour, ISpawnable, IFingerFlexListener
	{
		// Token: 0x17000B28 RID: 2856
		// (get) Token: 0x060074C6 RID: 29894 RVA: 0x00264106 File Offset: 0x00262306
		// (set) Token: 0x060074C7 RID: 29895 RVA: 0x0026410E File Offset: 0x0026230E
		public VRRig MyRig { get; private set; }

		// Token: 0x17000B29 RID: 2857
		// (get) Token: 0x060074C8 RID: 29896 RVA: 0x00264117 File Offset: 0x00262317
		// (set) Token: 0x060074C9 RID: 29897 RVA: 0x0026411F File Offset: 0x0026231F
		public bool IsSpawned { get; set; }

		// Token: 0x17000B2A RID: 2858
		// (get) Token: 0x060074CA RID: 29898 RVA: 0x00264128 File Offset: 0x00262328
		// (set) Token: 0x060074CB RID: 29899 RVA: 0x00264130 File Offset: 0x00262330
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x060074CC RID: 29900 RVA: 0x00264139 File Offset: 0x00262339
		public void OnSpawn(VRRig rig)
		{
			this.MyRig = rig;
		}

		// Token: 0x060074CD RID: 29901 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnDespawn()
		{
		}

		// Token: 0x060074CE RID: 29902 RVA: 0x00264142 File Offset: 0x00262342
		private void Update()
		{
			if (!this.restartTimer && Time.time - this.triggeredTime >= this.coolDownTimer)
			{
				this.restartTimer = true;
			}
		}

		// Token: 0x060074CF RID: 29903 RVA: 0x00264168 File Offset: 0x00262368
		private void OnPlayEffectLocal()
		{
			if (this.particlesToPlay != null)
			{
				this.particlesToPlay.Play();
			}
			if (this.objectToEnable != null)
			{
				this.objectToEnable.SetActive(true);
			}
			if (this.audioSource != null && this.audioToPlay != null)
			{
				this.audioSource.GTPlayOneShot(this.audioToPlay, 1f);
			}
			this.restartTimer = false;
			this.triggeredTime = Time.time;
		}

		// Token: 0x060074D0 RID: 29904 RVA: 0x002641EC File Offset: 0x002623EC
		public void OnButtonPressed(bool isLeftHand, float value)
		{
			if (!this.FingerFlexValidation(isLeftHand))
			{
				return;
			}
			if (!this.restartTimer || !this.buttonReleased)
			{
				return;
			}
			this.OnPlayEffectLocal();
			this.buttonReleased = false;
		}

		// Token: 0x060074D1 RID: 29905 RVA: 0x00264216 File Offset: 0x00262416
		public void OnButtonReleased(bool isLeftHand, float value)
		{
			if (!this.FingerFlexValidation(isLeftHand))
			{
				return;
			}
			this.buttonReleased = true;
		}

		// Token: 0x060074D2 RID: 29906 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnButtonPressStayed(bool isLeftHand, float value)
		{
		}

		// Token: 0x060074D3 RID: 29907 RVA: 0x00264229 File Offset: 0x00262429
		public bool FingerFlexValidation(bool isLeftHand)
		{
			return (!this.leftHand || isLeftHand) && (this.leftHand || !isLeftHand);
		}

		// Token: 0x0400863E RID: 34366
		[SerializeField]
		private ParticleSystem particlesToPlay;

		// Token: 0x0400863F RID: 34367
		[SerializeField]
		private GameObject objectToEnable;

		// Token: 0x04008640 RID: 34368
		[SerializeField]
		private AudioClip audioToPlay;

		// Token: 0x04008641 RID: 34369
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04008642 RID: 34370
		[SerializeField]
		private float coolDownTimer = 2f;

		// Token: 0x04008643 RID: 34371
		[SerializeField]
		private bool leftHand;

		// Token: 0x04008644 RID: 34372
		private float triggeredTime;

		// Token: 0x04008645 RID: 34373
		private bool restartTimer;

		// Token: 0x04008646 RID: 34374
		private bool buttonReleased = true;
	}
}
