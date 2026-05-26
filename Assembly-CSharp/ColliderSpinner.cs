using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class ColliderSpinner : MonoBehaviour
{
	// Token: 0x0600005E RID: 94 RVA: 0x000034D8 File Offset: 0x000016D8
	private void Start()
	{
		this.m_targetOffset = ((this.Target != null) ? (base.transform.position - this.Target.position) : Vector3.zero);
		this.m_spring.Reset(base.transform.position);
	}

	// Token: 0x0600005F RID: 95 RVA: 0x00003534 File Offset: 0x00001734
	private void FixedUpdate()
	{
		Vector3 targetValue = this.Target.position + this.m_targetOffset;
		base.transform.position = this.m_spring.TrackExponential(targetValue, 0.02f, Time.fixedDeltaTime);
	}

	// Token: 0x0400004F RID: 79
	public Transform Target;

	// Token: 0x04000050 RID: 80
	private Vector3 m_targetOffset;

	// Token: 0x04000051 RID: 81
	private Vector3Spring m_spring;
}
