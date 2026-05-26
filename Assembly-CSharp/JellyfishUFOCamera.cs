using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000013 RID: 19
public class JellyfishUFOCamera : MonoBehaviour
{
	// Token: 0x06000052 RID: 82 RVA: 0x00002E2A File Offset: 0x0000102A
	private void Start()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_spring.Reset(this.Target.transform.position);
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00002E58 File Offset: 0x00001058
	private void FixedUpdate()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_spring.TrackExponential(this.Target.transform.position, 0.5f, Time.fixedDeltaTime);
		Vector3 normalized = (this.m_spring.Value - base.transform.position).normalized;
		base.transform.rotation = Quaternion.LookRotation(normalized);
	}

	// Token: 0x04000037 RID: 55
	public Transform Target;

	// Token: 0x04000038 RID: 56
	private Vector3Spring m_spring;
}
