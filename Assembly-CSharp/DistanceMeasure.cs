using System;
using UnityEngine;

// Token: 0x02000AC3 RID: 2755
public class DistanceMeasure : MonoBehaviour
{
	// Token: 0x06004673 RID: 18035 RVA: 0x0017DC77 File Offset: 0x0017BE77
	private void Awake()
	{
		if (this.from == null)
		{
			this.from = base.transform;
		}
		if (this.to == null)
		{
			this.to = base.transform;
		}
	}

	// Token: 0x040058E0 RID: 22752
	public Transform from;

	// Token: 0x040058E1 RID: 22753
	public Transform to;
}
