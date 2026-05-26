using System;
using UnityEngine;

// Token: 0x020005C7 RID: 1479
public class GorillaCameraSceneTrigger : MonoBehaviour
{
	// Token: 0x0600251F RID: 9503 RVA: 0x000C5BBC File Offset: 0x000C3DBC
	public void ChangeScene(GorillaCameraTriggerIndex triggerLeft)
	{
		if (triggerLeft == this.currentSceneTrigger || this.currentSceneTrigger == null)
		{
			if (this.mostRecentSceneTrigger != this.currentSceneTrigger)
			{
				this.sceneCamera.SetSceneCamera(this.mostRecentSceneTrigger.sceneTriggerIndex);
				this.currentSceneTrigger = this.mostRecentSceneTrigger;
				return;
			}
			this.currentSceneTrigger = null;
		}
	}

	// Token: 0x04003077 RID: 12407
	public GorillaSceneCamera sceneCamera;

	// Token: 0x04003078 RID: 12408
	public GorillaCameraTriggerIndex currentSceneTrigger;

	// Token: 0x04003079 RID: 12409
	public GorillaCameraTriggerIndex mostRecentSceneTrigger;
}
