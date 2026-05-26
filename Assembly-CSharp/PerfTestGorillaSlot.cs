using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020003C8 RID: 968
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST && !BETA")]
public class PerfTestGorillaSlot : MonoBehaviour
{
	// Token: 0x0600172D RID: 5933 RVA: 0x00085F9B File Offset: 0x0008419B
	private void Start()
	{
		this.localStartPosition = base.transform.localPosition;
	}

	// Token: 0x04002267 RID: 8807
	public PerfTestGorillaSlot.SlotType slotType;

	// Token: 0x04002268 RID: 8808
	public Vector3 localStartPosition;

	// Token: 0x020003C9 RID: 969
	public enum SlotType
	{
		// Token: 0x0400226A RID: 8810
		VR_PLAYER,
		// Token: 0x0400226B RID: 8811
		DUMMY
	}
}
