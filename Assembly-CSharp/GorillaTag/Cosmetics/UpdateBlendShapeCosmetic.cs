using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012B0 RID: 4784
	public class UpdateBlendShapeCosmetic : MonoBehaviour
	{
		// Token: 0x060077B3 RID: 30643 RVA: 0x00274223 File Offset: 0x00272423
		private void Awake()
		{
			this.targetWeight = this.blendStartWeight;
			this.currentWeight = 0f;
		}

		// Token: 0x060077B4 RID: 30644 RVA: 0x0027423C File Offset: 0x0027243C
		private void Update()
		{
			this.currentWeight = Mathf.Lerp(this.currentWeight, this.targetWeight, Time.deltaTime * this.blendSpeed);
			this.skinnedMeshRenderer.SetBlendShapeWeight(this.blendShapeIndex, this.currentWeight);
		}

		// Token: 0x060077B5 RID: 30645 RVA: 0x00274278 File Offset: 0x00272478
		public void SetBlendValue(bool leftHand, float value)
		{
			this.targetWeight = Mathf.Clamp01(this.invertPassedBlend ? (1f - value) : value) * this.maxBlendShapeWeight;
		}

		// Token: 0x060077B6 RID: 30646 RVA: 0x0027429E File Offset: 0x0027249E
		public void SetBlendValue(float value)
		{
			this.targetWeight = Mathf.Clamp01(this.invertPassedBlend ? (1f - value) : value) * this.maxBlendShapeWeight;
		}

		// Token: 0x060077B7 RID: 30647 RVA: 0x002742C4 File Offset: 0x002724C4
		public void FullyBlend()
		{
			this.targetWeight = this.maxBlendShapeWeight;
		}

		// Token: 0x060077B8 RID: 30648 RVA: 0x002742D2 File Offset: 0x002724D2
		public void ResetBlend()
		{
			this.targetWeight = 0f;
		}

		// Token: 0x060077B9 RID: 30649 RVA: 0x002742DF File Offset: 0x002724DF
		public float GetBlendValue()
		{
			return this.skinnedMeshRenderer.GetBlendShapeWeight(this.blendShapeIndex);
		}

		// Token: 0x04008A53 RID: 35411
		[Tooltip("The SkinnedMeshRenderer whose BlendShape weight will be updated. This must reference a mesh that has BlendShapes defined in its import settings.")]
		[SerializeField]
		private SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x04008A54 RID: 35412
		[Tooltip("Maximum blend shape weight applied when fully blended. Usually 100 for standard Unity BlendShapes.")]
		public float maxBlendShapeWeight = 100f;

		// Token: 0x04008A55 RID: 35413
		[Tooltip("Index of the BlendShape to control. You can find this index in the SkinnedMeshRenderer inspector under 'BlendShapes'.")]
		[SerializeField]
		private int blendShapeIndex;

		// Token: 0x04008A56 RID: 35414
		[Tooltip("Speed at which the BlendShape transitions toward its target weight. Higher values make blending more responsive, lower values make it smoother.")]
		[SerializeField]
		private float blendSpeed = 10f;

		// Token: 0x04008A57 RID: 35415
		[Tooltip("Initial BlendShape weight set when the component awakens. Useful for setting a default deformation state.")]
		[SerializeField]
		private float blendStartWeight;

		// Token: 0x04008A58 RID: 35416
		[Tooltip("If enabled, inverts the incoming blend value (e.g. 0 → 1, 0.2 → 0.8). Useful when an input should drive the opposite direction of deformation.")]
		[SerializeField]
		private bool invertPassedBlend;

		// Token: 0x04008A59 RID: 35417
		private float targetWeight;

		// Token: 0x04008A5A RID: 35418
		private float currentWeight;
	}
}
