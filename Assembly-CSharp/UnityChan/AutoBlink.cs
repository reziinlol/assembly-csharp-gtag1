using System;
using System.Collections;
using UnityEngine;

namespace UnityChan
{
	// Token: 0x0200134A RID: 4938
	public class AutoBlink : MonoBehaviour
	{
		// Token: 0x06007C6A RID: 31850 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void Awake()
		{
		}

		// Token: 0x06007C6B RID: 31851 RVA: 0x0028D0EE File Offset: 0x0028B2EE
		private void Start()
		{
			this.ResetTimer();
			base.StartCoroutine("RandomChange");
		}

		// Token: 0x06007C6C RID: 31852 RVA: 0x0028D102 File Offset: 0x0028B302
		private void ResetTimer()
		{
			this.timeRemining = this.timeBlink;
			this.timerStarted = false;
		}

		// Token: 0x06007C6D RID: 31853 RVA: 0x0028D118 File Offset: 0x0028B318
		private void Update()
		{
			if (!this.timerStarted)
			{
				this.eyeStatus = AutoBlink.Status.Close;
				this.timerStarted = true;
			}
			if (this.timerStarted)
			{
				this.timeRemining -= Time.deltaTime;
				if (this.timeRemining <= 0f)
				{
					this.eyeStatus = AutoBlink.Status.Open;
					this.ResetTimer();
					return;
				}
				if (this.timeRemining <= this.timeBlink * 0.3f)
				{
					this.eyeStatus = AutoBlink.Status.HalfClose;
				}
			}
		}

		// Token: 0x06007C6E RID: 31854 RVA: 0x0028D18C File Offset: 0x0028B38C
		private void LateUpdate()
		{
			if (this.isActive && this.isBlink)
			{
				switch (this.eyeStatus)
				{
				case AutoBlink.Status.Close:
					this.SetCloseEyes();
					return;
				case AutoBlink.Status.HalfClose:
					this.SetHalfCloseEyes();
					return;
				case AutoBlink.Status.Open:
					this.SetOpenEyes();
					this.isBlink = false;
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06007C6F RID: 31855 RVA: 0x0028D1DE File Offset: 0x0028B3DE
		private void SetCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Close);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Close);
		}

		// Token: 0x06007C70 RID: 31856 RVA: 0x0028D204 File Offset: 0x0028B404
		private void SetHalfCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
		}

		// Token: 0x06007C71 RID: 31857 RVA: 0x0028D22A File Offset: 0x0028B42A
		private void SetOpenEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Open);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Open);
		}

		// Token: 0x06007C72 RID: 31858 RVA: 0x0028D250 File Offset: 0x0028B450
		private IEnumerator RandomChange()
		{
			for (;;)
			{
				float num = Random.Range(0f, 1f);
				if (!this.isBlink && num > this.threshold)
				{
					this.isBlink = true;
				}
				yield return new WaitForSeconds(this.interval);
			}
			yield break;
		}

		// Token: 0x04008D77 RID: 36215
		public bool isActive = true;

		// Token: 0x04008D78 RID: 36216
		public SkinnedMeshRenderer ref_SMR_EYE_DEF;

		// Token: 0x04008D79 RID: 36217
		public SkinnedMeshRenderer ref_SMR_EL_DEF;

		// Token: 0x04008D7A RID: 36218
		public float ratio_Close = 85f;

		// Token: 0x04008D7B RID: 36219
		public float ratio_HalfClose = 20f;

		// Token: 0x04008D7C RID: 36220
		[HideInInspector]
		public float ratio_Open;

		// Token: 0x04008D7D RID: 36221
		private bool timerStarted;

		// Token: 0x04008D7E RID: 36222
		private bool isBlink;

		// Token: 0x04008D7F RID: 36223
		public float timeBlink = 0.4f;

		// Token: 0x04008D80 RID: 36224
		private float timeRemining;

		// Token: 0x04008D81 RID: 36225
		public float threshold = 0.3f;

		// Token: 0x04008D82 RID: 36226
		public float interval = 3f;

		// Token: 0x04008D83 RID: 36227
		private AutoBlink.Status eyeStatus;

		// Token: 0x0200134B RID: 4939
		private enum Status
		{
			// Token: 0x04008D85 RID: 36229
			Close,
			// Token: 0x04008D86 RID: 36230
			HalfClose,
			// Token: 0x04008D87 RID: 36231
			Open
		}
	}
}
