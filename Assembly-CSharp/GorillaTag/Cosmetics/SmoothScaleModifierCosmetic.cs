using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012A5 RID: 4773
	public class SmoothScaleModifierCosmetic : MonoBehaviour
	{
		// Token: 0x0600777C RID: 30588 RVA: 0x00273265 File Offset: 0x00271465
		private void Awake()
		{
			this.initialScale = this.objectPrefab.transform.localScale;
		}

		// Token: 0x0600777D RID: 30589 RVA: 0x0027327D File Offset: 0x0027147D
		private void OnEnable()
		{
			this.UpdateState(SmoothScaleModifierCosmetic.State.Reset);
		}

		// Token: 0x0600777E RID: 30590 RVA: 0x00273288 File Offset: 0x00271488
		private void Update()
		{
			switch (this.currentState)
			{
			case SmoothScaleModifierCosmetic.State.None:
			case SmoothScaleModifierCosmetic.State.Scaled:
				break;
			case SmoothScaleModifierCosmetic.State.Reset:
				this.SmoothScale(this.objectPrefab.transform.localScale, this.initialScale);
				if (Vector3.Distance(this.objectPrefab.transform.localScale, this.initialScale) < 0.01f)
				{
					this.objectPrefab.transform.localScale = this.initialScale;
					if (this.onReset != null)
					{
						this.onReset.Invoke();
					}
					this.UpdateState(SmoothScaleModifierCosmetic.State.None);
					return;
				}
				break;
			case SmoothScaleModifierCosmetic.State.Scaling:
				this.SmoothScale(this.objectPrefab.transform.localScale, this.targetScale);
				if (Vector3.Distance(this.objectPrefab.transform.localScale, this.targetScale) < 0.01f)
				{
					this.objectPrefab.transform.localScale = this.targetScale;
					if (this.onScaled != null)
					{
						this.onScaled.Invoke();
					}
					this.UpdateState(SmoothScaleModifierCosmetic.State.Scaled);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x0600777F RID: 30591 RVA: 0x00273393 File Offset: 0x00271593
		private void SmoothScale(Vector3 initial, Vector3 target)
		{
			this.objectPrefab.transform.localScale = Vector3.MoveTowards(initial, target, this.speed * Time.deltaTime);
		}

		// Token: 0x06007780 RID: 30592 RVA: 0x002733B8 File Offset: 0x002715B8
		private void UpdateState(SmoothScaleModifierCosmetic.State newState)
		{
			this.currentState = newState;
		}

		// Token: 0x06007781 RID: 30593 RVA: 0x002733C1 File Offset: 0x002715C1
		public void TriggerScale()
		{
			if (this.currentState != SmoothScaleModifierCosmetic.State.Scaled)
			{
				this.UpdateState(SmoothScaleModifierCosmetic.State.Scaling);
			}
		}

		// Token: 0x06007782 RID: 30594 RVA: 0x002733D3 File Offset: 0x002715D3
		public void TriggerReset()
		{
			if (this.currentState != SmoothScaleModifierCosmetic.State.Reset)
			{
				this.UpdateState(SmoothScaleModifierCosmetic.State.Reset);
			}
		}

		// Token: 0x040089FF RID: 35327
		[Tooltip("The GameObject to scale up or down. This should reference the cosmetic mesh or object you want to visually modify.")]
		[SerializeField]
		private GameObject objectPrefab;

		// Token: 0x04008A00 RID: 35328
		[Tooltip("The target scale applied when scaling is triggered.")]
		[SerializeField]
		private Vector3 targetScale = new Vector3(2f, 2f, 2f);

		// Token: 0x04008A01 RID: 35329
		[Tooltip("Speed at which the object scales toward its target or initial size")]
		[SerializeField]
		private float speed = 2f;

		// Token: 0x04008A02 RID: 35330
		[Tooltip("Invoked once when the object reaches the target scale.")]
		public UnityEvent onScaled;

		// Token: 0x04008A03 RID: 35331
		[Tooltip("Invoked once when the object returns to its initial scale.")]
		public UnityEvent onReset;

		// Token: 0x04008A04 RID: 35332
		private SmoothScaleModifierCosmetic.State currentState;

		// Token: 0x04008A05 RID: 35333
		private Vector3 initialScale;

		// Token: 0x020012A6 RID: 4774
		private enum State
		{
			// Token: 0x04008A07 RID: 35335
			None,
			// Token: 0x04008A08 RID: 35336
			Reset,
			// Token: 0x04008A09 RID: 35337
			Scaling,
			// Token: 0x04008A0A RID: 35338
			Scaled
		}
	}
}
