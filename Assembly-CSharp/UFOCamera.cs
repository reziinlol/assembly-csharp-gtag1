using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class UFOCamera : MonoBehaviour
{
	// Token: 0x06000061 RID: 97 RVA: 0x0000357C File Offset: 0x0000177C
	private void Start()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_targetOffset = base.transform.position - this.Target.position;
		this.m_spring.Reset(base.transform.position);
	}

	// Token: 0x06000062 RID: 98 RVA: 0x000035D0 File Offset: 0x000017D0
	private void FixedUpdate()
	{
		if (this.Target == null)
		{
			return;
		}
		Vector3 targetValue = this.Target.position + this.m_targetOffset;
		base.transform.position = this.m_spring.TrackExponential(targetValue, 0.02f, Time.fixedDeltaTime);
	}

	// Token: 0x04000052 RID: 82
	public Transform Target;

	// Token: 0x04000053 RID: 83
	private Vector3 m_targetOffset;

	// Token: 0x04000054 RID: 84
	private Vector3Spring m_spring;
}
