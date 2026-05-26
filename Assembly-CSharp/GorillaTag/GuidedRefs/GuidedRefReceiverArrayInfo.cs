using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x020011BB RID: 4539
	[Serializable]
	public struct GuidedRefReceiverArrayInfo
	{
		// Token: 0x060072A5 RID: 29349 RVA: 0x00254C92 File Offset: 0x00252E92
		public GuidedRefReceiverArrayInfo(bool useRecommendedDefaults)
		{
			this.resolveModes = (useRecommendedDefaults ? (GRef.EResolveModes.Runtime | GRef.EResolveModes.SceneProcessing) : GRef.EResolveModes.None);
			this.targets = Array.Empty<GuidedRefTargetIdSO>();
			this.hubId = null;
			this.fieldId = 0;
			this.resolveCount = 0;
		}

		// Token: 0x04008251 RID: 33361
		[Tooltip("Controls whether the array should be overridden by the guided refs.")]
		[SerializeField]
		public GRef.EResolveModes resolveModes;

		// Token: 0x04008252 RID: 33362
		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO hubId;

		// Token: 0x04008253 RID: 33363
		[SerializeField]
		public GuidedRefTargetIdSO[] targets;

		// Token: 0x04008254 RID: 33364
		[NonSerialized]
		public int fieldId;

		// Token: 0x04008255 RID: 33365
		[NonSerialized]
		public int resolveCount;
	}
}
