using System;
using UnityEngine;

// Token: 0x0200034E RID: 846
[DefaultExecutionOrder(-1000)]
public class HierarchyFlattenerReparentXform : MonoBehaviour
{
	// Token: 0x060014D0 RID: 5328 RVA: 0x0006EEB6 File Offset: 0x0006D0B6
	protected void Awake()
	{
		if (base.enabled)
		{
			this._DoIt();
		}
	}

	// Token: 0x060014D1 RID: 5329 RVA: 0x0006EEC6 File Offset: 0x0006D0C6
	protected void OnEnable()
	{
		this._DoIt();
	}

	// Token: 0x060014D2 RID: 5330 RVA: 0x0006EECE File Offset: 0x0006D0CE
	private void _DoIt()
	{
		if (this._didIt)
		{
			return;
		}
		if (this.newParent != null)
		{
			base.transform.SetParent(this.newParent, true);
		}
		Object.Destroy(this);
	}

	// Token: 0x04001983 RID: 6531
	public Transform newParent;

	// Token: 0x04001984 RID: 6532
	private bool _didIt;
}
