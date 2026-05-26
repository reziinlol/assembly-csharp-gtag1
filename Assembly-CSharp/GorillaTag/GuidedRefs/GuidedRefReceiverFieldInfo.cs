using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x020011B4 RID: 4532
	[Serializable]
	public struct GuidedRefReceiverFieldInfo
	{
		// Token: 0x06007280 RID: 29312 RVA: 0x00254BDC File Offset: 0x00252DDC
		public GuidedRefReceiverFieldInfo(bool useRecommendedDefaults)
		{
			this.resolveModes = (useRecommendedDefaults ? (GRef.EResolveModes.Runtime | GRef.EResolveModes.SceneProcessing) : GRef.EResolveModes.None);
			this.targetId = null;
			this.hubId = null;
			this.fieldId = 0;
		}

		// Token: 0x04008249 RID: 33353
		[SerializeField]
		public GRef.EResolveModes resolveModes;

		// Token: 0x0400824A RID: 33354
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x0400824B RID: 33355
		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO hubId;

		// Token: 0x0400824C RID: 33356
		[NonSerialized]
		public int fieldId;
	}
}
