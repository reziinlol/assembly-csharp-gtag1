using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020005E7 RID: 1511
public class AutoSyncTransforms : MonoBehaviour
{
	// Token: 0x170003ED RID: 1005
	// (get) Token: 0x0600258A RID: 9610 RVA: 0x000C6BA6 File Offset: 0x000C4DA6
	public Transform TargetTransform
	{
		get
		{
			return this.m_transform;
		}
	}

	// Token: 0x170003EE RID: 1006
	// (get) Token: 0x0600258B RID: 9611 RVA: 0x000C6BAE File Offset: 0x000C4DAE
	public Rigidbody TargetRigidbody
	{
		get
		{
			return this.m_rigidbody;
		}
	}

	// Token: 0x0600258C RID: 9612 RVA: 0x000C6BB8 File Offset: 0x000C4DB8
	private void Awake()
	{
		if (this.m_transform.IsNull())
		{
			this.m_transform = base.transform;
		}
		if (this.m_rigidbody.IsNull())
		{
			this.m_rigidbody = base.GetComponent<Rigidbody>();
		}
		if (this.m_transform.IsNull() || this.m_rigidbody.IsNull())
		{
			base.enabled = false;
			Debug.LogError("AutoSyncTransforms: Rigidbody or Transform is null, disabling!! Please add the missing reference or component", this);
			return;
		}
		this.clean = true;
	}

	// Token: 0x0600258D RID: 9613 RVA: 0x000C6C2B File Offset: 0x000C4E2B
	private void OnEnable()
	{
		if (this.clean)
		{
			PostVRRigPhysicsSynch.AddSyncTarget(this);
		}
	}

	// Token: 0x0600258E RID: 9614 RVA: 0x000C6C3B File Offset: 0x000C4E3B
	private void OnDisable()
	{
		if (this.clean)
		{
			PostVRRigPhysicsSynch.RemoveSyncTarget(this);
		}
	}

	// Token: 0x04003103 RID: 12547
	[SerializeField]
	private Transform m_transform;

	// Token: 0x04003104 RID: 12548
	[SerializeField]
	private Rigidbody m_rigidbody;

	// Token: 0x04003105 RID: 12549
	private bool clean;
}
