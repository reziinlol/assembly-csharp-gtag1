using System;
using UnityEngine;

// Token: 0x0200034D RID: 845
[DefaultExecutionOrder(-1000)]
public class HierarchyFlattenerRemoveXform : MonoBehaviour
{
	// Token: 0x060014CD RID: 5325 RVA: 0x0006EE32 File Offset: 0x0006D032
	protected void Awake()
	{
		this._DoIt();
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x0006EE3C File Offset: 0x0006D03C
	private void _DoIt()
	{
		if (this._didIt)
		{
			return;
		}
		if (base.GetComponentInChildren<HierarchyFlattenerRemoveXform>(true) != null)
		{
			return;
		}
		HierarchyFlattenerRemoveXform componentInParent = base.GetComponentInParent<HierarchyFlattenerRemoveXform>(true);
		this._didIt = true;
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).SetParent(transform.parent, true);
		}
		Object.Destroy(base.gameObject);
		if (componentInParent != null)
		{
			componentInParent._DoIt();
		}
	}

	// Token: 0x04001982 RID: 6530
	private bool _didIt;
}
