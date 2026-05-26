using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x02000334 RID: 820
public class FlattenerCrumb : MonoBehaviour
{
	// Token: 0x06001432 RID: 5170 RVA: 0x0006CA74 File Offset: 0x0006AC74
	private void OnDisable()
	{
		for (int i = this.flattenerList.Count - 1; i >= 0; i--)
		{
			this.flattenerList[i].CrumbDisabled();
		}
	}

	// Token: 0x06001433 RID: 5171 RVA: 0x0006CAAA File Offset: 0x0006ACAA
	public void AddFlattenerReference(ObjectHierarchyFlattener flattener)
	{
		this.flattenerList.AddIfNew(flattener);
	}

	// Token: 0x040018F8 RID: 6392
	[DebugReadout]
	private List<ObjectHierarchyFlattener> flattenerList = new List<ObjectHierarchyFlattener>();
}
