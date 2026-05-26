using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020003C6 RID: 966
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST && !BETA")]
public class PerfTestFPSCaptureController : MonoBehaviour
{
	// Token: 0x04002260 RID: 8800
	[SerializeField]
	private SerializablePerformanceReport<ScenePerformanceData> performanceSummary;
}
