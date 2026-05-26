using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000424 RID: 1060
public class NativeSizeChanger : MonoBehaviour
{
	// Token: 0x0600193A RID: 6458 RVA: 0x0008DCB1 File Offset: 0x0008BEB1
	public void Activate(NativeSizeChangerSettings settings)
	{
		settings.WorldPosition = base.transform.position;
		settings.ActivationTime = Time.time;
		GTPlayer.Instance.SetNativeScale(settings);
	}
}
