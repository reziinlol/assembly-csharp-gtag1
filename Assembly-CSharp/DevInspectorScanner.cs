using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000325 RID: 805
public class DevInspectorScanner : MonoBehaviour
{
	// Token: 0x040018CE RID: 6350
	public Text hintTextOutput;

	// Token: 0x040018CF RID: 6351
	public float scanDistance = 10f;

	// Token: 0x040018D0 RID: 6352
	public float scanAngle = 30f;

	// Token: 0x040018D1 RID: 6353
	public LayerMask scanLayerMask;

	// Token: 0x040018D2 RID: 6354
	public string targetComponentName;

	// Token: 0x040018D3 RID: 6355
	public float rayPerDegree = 10f;
}
