using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000A4B RID: 2635
public class CustomMapTelemetryTrigger : MonoBehaviour
{
	// Token: 0x06004379 RID: 17273 RVA: 0x0016A2E7 File Offset: 0x001684E7
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider && CustomMapTelemetry.IsActive)
		{
			CustomMapTelemetry.EndMapTracking();
		}
	}

	// Token: 0x0600437A RID: 17274 RVA: 0x0016A307 File Offset: 0x00168507
	public void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider && GorillaComputer.instance.IsPlayerInVirtualStump() && !CustomMapTelemetry.IsActive)
		{
			CustomMapTelemetry.StartMapTracking();
		}
	}
}
