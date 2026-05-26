using System;
using UnityEngine;

// Token: 0x02000CF6 RID: 3318
[Serializable]
internal class RoomCountForZone
{
	// Token: 0x170007BF RID: 1983
	// (get) Token: 0x06005259 RID: 21081 RVA: 0x001B1A85 File Offset: 0x001AFC85
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	// Token: 0x170007C0 RID: 1984
	// (get) Token: 0x0600525A RID: 21082 RVA: 0x001B1A8D File Offset: 0x001AFC8D
	public GTZone Zone
	{
		get
		{
			return this.zone;
		}
	}

	// Token: 0x040063A2 RID: 25506
	[SerializeField]
	private GTZone zone;

	// Token: 0x040063A3 RID: 25507
	[SerializeField]
	private int count;
}
