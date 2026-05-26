using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000CA0 RID: 3232
public class EnableNetworkRotations : MonoBehaviour
{
	// Token: 0x06005027 RID: 20519 RVA: 0x001A9CD9 File Offset: 0x001A7ED9
	private void OnEnable()
	{
		EnableNetworkRotations.m_enabledRotationEnablers.Add(this);
		if (EnableNetworkRotations.m_enabledRotationEnablers.Count == 1)
		{
			GTPlayerTransform.EnableNetworkRotations();
		}
	}

	// Token: 0x06005028 RID: 20520 RVA: 0x001A9CF9 File Offset: 0x001A7EF9
	private void OnDisable()
	{
		EnableNetworkRotations.m_enabledRotationEnablers.Remove(this);
		if (EnableNetworkRotations.m_enabledRotationEnablers.Count == 0)
		{
			GTPlayerTransform.DisableNetworkRotations();
		}
	}

	// Token: 0x0400623E RID: 25150
	private static HashSet<EnableNetworkRotations> m_enabledRotationEnablers = new HashSet<EnableNetworkRotations>(2);
}
