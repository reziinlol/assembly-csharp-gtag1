using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020008EA RID: 2282
public class HoverboardAreaTrigger : MonoBehaviour
{
	// Token: 0x06003BB7 RID: 15287 RVA: 0x0014714F File Offset: 0x0014534F
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(true, false);
		}
	}

	// Token: 0x06003BB8 RID: 15288 RVA: 0x0014716F File Offset: 0x0014536F
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(false, false);
		}
	}
}
