using System;
using UnityEngine;

// Token: 0x020005D0 RID: 1488
public class GorillaSceneCamera : MonoBehaviour
{
	// Token: 0x06002539 RID: 9529 RVA: 0x000C5E82 File Offset: 0x000C4082
	public void SetSceneCamera(int sceneIndex)
	{
		base.transform.position = this.sceneTransforms[sceneIndex].scenePosition;
		base.transform.eulerAngles = this.sceneTransforms[sceneIndex].sceneRotation;
	}

	// Token: 0x040030BA RID: 12474
	public GorillaSceneTransform[] sceneTransforms;
}
