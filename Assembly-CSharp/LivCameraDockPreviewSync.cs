using System;
using Docking;
using UnityEngine;

// Token: 0x02000417 RID: 1047
[ExecuteAlways]
public class LivCameraDockPreviewSync : MonoBehaviour
{
	// Token: 0x04002404 RID: 9220
	private LivCameraDock dock;

	// Token: 0x04002405 RID: 9221
	private Camera parentCamera;

	// Token: 0x04002406 RID: 9222
	private float _lastCameraFOV = -1f;

	// Token: 0x04002407 RID: 9223
	private float _lastDockFOV = -1f;
}
