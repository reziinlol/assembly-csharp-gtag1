using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E1A RID: 3610
public class BSPZoneData : MonoBehaviour
{
	// Token: 0x1700084A RID: 2122
	// (get) Token: 0x06005804 RID: 22532 RVA: 0x001C9B21 File Offset: 0x001C7D21
	public int Priority
	{
		get
		{
			return this.priority;
		}
	}

	// Token: 0x1700084B RID: 2123
	// (get) Token: 0x06005805 RID: 22533 RVA: 0x0001749C File Offset: 0x0001569C
	public string ZoneName
	{
		get
		{
			return base.gameObject.name;
		}
	}

	// Token: 0x040068A8 RID: 26792
	[SerializeField]
	private int priority;

	// Token: 0x040068A9 RID: 26793
	[NonSerialized]
	public List<BoxCollider> boxList;
}
