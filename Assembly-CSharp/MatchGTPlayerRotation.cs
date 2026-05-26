using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000357 RID: 855
public class MatchGTPlayerRotation : MonoBehaviour
{
	// Token: 0x060014E7 RID: 5351 RVA: 0x0006F4F8 File Offset: 0x0006D6F8
	private void LateUpdate()
	{
		if (this.matchPosition)
		{
			base.transform.position = GTPlayer.Instance.mainCamera.transform.position;
		}
		if (this.matchRotation)
		{
			base.transform.rotation = GTPlayer.Instance.transform.rotation;
		}
	}

	// Token: 0x040019C6 RID: 6598
	public bool matchPosition;

	// Token: 0x040019C7 RID: 6599
	public bool matchRotation;
}
