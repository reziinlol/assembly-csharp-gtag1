using System;
using UnityEngine;

// Token: 0x020005D6 RID: 1494
public class GorillaTriggerBoxTeleport : GorillaTriggerBox
{
	// Token: 0x06002547 RID: 9543 RVA: 0x000C5FB9 File Offset: 0x000C41B9
	public override void OnBoxTriggered()
	{
		this.cameraOffest.GetComponent<Rigidbody>().linearVelocity = new Vector3(0f, 0f, 0f);
		this.cameraOffest.transform.position = this.teleportLocation;
	}

	// Token: 0x040030C7 RID: 12487
	public Vector3 teleportLocation;

	// Token: 0x040030C8 RID: 12488
	public GameObject cameraOffest;
}
