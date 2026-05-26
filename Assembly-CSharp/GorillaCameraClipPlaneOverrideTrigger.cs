using System;
using UnityEngine;

// Token: 0x020005C5 RID: 1477
public class GorillaCameraClipPlaneOverrideTrigger : GorillaTriggerBox
{
	// Token: 0x06002519 RID: 9497 RVA: 0x000C5A49 File Offset: 0x000C3C49
	private void Awake()
	{
		this.mainCamera = Camera.main;
	}

	// Token: 0x0600251A RID: 9498 RVA: 0x000C5A56 File Offset: 0x000C3C56
	public override void OnBoxTriggered()
	{
		this.mainCamera.farClipPlane = this.clipPlaneFarDistanceOverride;
	}

	// Token: 0x0400306B RID: 12395
	private Camera mainCamera;

	// Token: 0x0400306C RID: 12396
	public float clipPlaneFarDistanceOverride;
}
