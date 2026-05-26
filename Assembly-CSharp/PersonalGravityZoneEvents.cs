using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000CA1 RID: 3233
public class PersonalGravityZoneEvents : MonoBehaviour
{
	// Token: 0x0600502B RID: 20523 RVA: 0x001A9D25 File Offset: 0x001A7F25
	public void SetLocalPlayerGravityDirection(Vector3 direction)
	{
		GTPlayerTransform.Instance.SetPersonalGravityDirection(direction);
	}

	// Token: 0x0600502C RID: 20524 RVA: 0x001A9D32 File Offset: 0x001A7F32
	public void SetLocalPlayerGravityDirection(Transform referenceDir)
	{
		GTPlayerTransform.Instance.SetPersonalGravityDirection(referenceDir);
	}
}
