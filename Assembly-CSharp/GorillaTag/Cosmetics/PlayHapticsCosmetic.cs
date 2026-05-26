using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200129E RID: 4766
	public class PlayHapticsCosmetic : MonoBehaviour
	{
		// Token: 0x06007743 RID: 30531 RVA: 0x00271CE8 File Offset: 0x0026FEE8
		private void Awake()
		{
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
			this.myHeldItem = base.GetComponentInParent<IHeldItem>();
		}

		// Token: 0x06007744 RID: 30532 RVA: 0x00271D02 File Offset: 0x0026FF02
		public void PlayHaptics()
		{
			GorillaTagger.Instance.StartVibration(this.leftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06007745 RID: 30533 RVA: 0x00271D20 File Offset: 0x0026FF20
		public void PlayHapticsTransferableObject()
		{
			this.PlayHapticsHeldItem();
		}

		// Token: 0x06007746 RID: 30534 RVA: 0x00271D28 File Offset: 0x0026FF28
		public void PlayHapticsHeldItem()
		{
			bool flag;
			if (!(this.parentTransferable != null))
			{
				IHeldItem heldItem = this.myHeldItem;
				flag = (heldItem != null && heldItem.IsMyItem());
			}
			else
			{
				flag = this.parentTransferable.IsMyItem();
			}
			if (!flag)
			{
				return;
			}
			bool flag2;
			if (!(this.parentTransferable != null))
			{
				IHeldItem heldItem2 = this.myHeldItem;
				flag2 = (heldItem2 != null && heldItem2.InLeftHand());
			}
			else
			{
				flag2 = this.parentTransferable.InLeftHand();
			}
			bool forLeftController = flag2;
			GorillaTagger.Instance.StartVibration(forLeftController, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06007747 RID: 30535 RVA: 0x00271DAA File Offset: 0x0026FFAA
		public void PlayHaptics(bool isLeftHand)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06007748 RID: 30536 RVA: 0x00271DC3 File Offset: 0x0026FFC3
		public void PlayHapticsBothHands(bool isLeftHand)
		{
			this.PlayHaptics(false);
			this.PlayHaptics(true);
		}

		// Token: 0x06007749 RID: 30537 RVA: 0x00271DAA File Offset: 0x0026FFAA
		public void PlayHaptics(bool isLeftHand, float value)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x0600774A RID: 30538 RVA: 0x00271DD3 File Offset: 0x0026FFD3
		public void PlayHapticsBothHands(bool isLeftHand, float value)
		{
			this.PlayHaptics(false, value);
			this.PlayHaptics(true, value);
		}

		// Token: 0x0600774B RID: 30539 RVA: 0x00271DAA File Offset: 0x0026FFAA
		public void PlayHaptics(bool isLeftHand, Collider other)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x0600774C RID: 30540 RVA: 0x00271DE5 File Offset: 0x0026FFE5
		public void PlayHapticsBothHands(bool isLeftHand, Collider other)
		{
			this.PlayHaptics(false, other);
			this.PlayHaptics(true, other);
		}

		// Token: 0x0600774D RID: 30541 RVA: 0x00271DAA File Offset: 0x0026FFAA
		public void PlayHaptics(bool isLeftHand, Collision other)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x0600774E RID: 30542 RVA: 0x00271DF7 File Offset: 0x0026FFF7
		public void PlayHapticsBothHands(bool isLeftHand, Collision other)
		{
			this.PlayHaptics(false, other);
			this.PlayHaptics(true, other);
		}

		// Token: 0x0600774F RID: 30543 RVA: 0x00271E0C File Offset: 0x0027000C
		public void PlayHapticsByButtonValue(bool isLeftHand, float strength)
		{
			float amplitude = Mathf.InverseLerp(this.minHapticStrengthThreshold, this.maxHapticStrengthThreshold, strength);
			GorillaTagger.Instance.StartVibration(isLeftHand, amplitude, this.hapticDuration);
		}

		// Token: 0x06007750 RID: 30544 RVA: 0x00271E3E File Offset: 0x0027003E
		public void PlayHapticsByButtonValueBothHands(bool isLeftHand, float strength)
		{
			this.PlayHapticsByButtonValue(false, strength);
			this.PlayHapticsByButtonValue(true, strength);
		}

		// Token: 0x06007751 RID: 30545 RVA: 0x00271E50 File Offset: 0x00270050
		public void PlayHapticsByVelocity(bool isLeftHand, float velocity)
		{
			float num = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand).GetAverageVelocity(true, 0.15f, false).magnitude;
			num = Mathf.InverseLerp(this.minHapticStrengthThreshold, this.maxHapticStrengthThreshold, num);
			GorillaTagger.Instance.StartVibration(isLeftHand, num, this.hapticDuration);
		}

		// Token: 0x06007752 RID: 30546 RVA: 0x00271EA2 File Offset: 0x002700A2
		public void PlayHapticsByVelocityBothHands(bool isLeftHand, float velocity)
		{
			this.PlayHapticsByVelocity(false, velocity);
			this.PlayHapticsByVelocity(true, velocity);
		}

		// Token: 0x0400898A RID: 35210
		[SerializeField]
		private float hapticDuration;

		// Token: 0x0400898B RID: 35211
		[SerializeField]
		private float hapticStrength;

		// Token: 0x0400898C RID: 35212
		[SerializeField]
		private float minHapticStrengthThreshold;

		// Token: 0x0400898D RID: 35213
		[SerializeField]
		private float maxHapticStrengthThreshold;

		// Token: 0x0400898E RID: 35214
		[Tooltip("Only check this box if you are not setting the left/hand right from the subscriber")]
		[SerializeField]
		private bool leftHand;

		// Token: 0x0400898F RID: 35215
		private TransferrableObject parentTransferable;

		// Token: 0x04008990 RID: 35216
		private IHeldItem myHeldItem;
	}
}
