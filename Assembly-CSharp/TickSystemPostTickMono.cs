using System;
using UnityEngine;

// Token: 0x02000D08 RID: 3336
internal abstract class TickSystemPostTickMono : MonoBehaviour, ITickSystemPost
{
	// Token: 0x170007CE RID: 1998
	// (get) Token: 0x060052A9 RID: 21161 RVA: 0x001B1F18 File Offset: 0x001B0118
	// (set) Token: 0x060052AA RID: 21162 RVA: 0x001B1F20 File Offset: 0x001B0120
	public bool PostTickRunning { get; set; }

	// Token: 0x060052AB RID: 21163 RVA: 0x001A578D File Offset: 0x001A398D
	public virtual void OnEnable()
	{
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x060052AC RID: 21164 RVA: 0x00156E8B File Offset: 0x0015508B
	public virtual void OnDisable()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x060052AD RID: 21165 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void PostTick()
	{
	}
}
